using Henkie.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Henkie.HSI.Board1
{
    /// <summary>
    ///   The <see cref = "Device" /> class provides methods for
    ///   communicating with the HSI interface.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComSourceInterfaces(typeof(IDeviceEvents))]
    public sealed class Device : IDisposable
    {


        private bool _isDisposed;
        private ICommandDispatcher _commandDispatcher;
        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        public Device(){}

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class that will communicate with the HSI board over USB.
        /// </summary>
        /// <param name = "COMPort">The name of the COM port to use for
        ///   communicating with the device (i.e. "COM1", "COM2",
        ///   etc.)</param>
        public Device(string COMPort) : this()
        {
            this.COMPort = COMPort;
            _commandDispatcher = new UsbCommandDispatcher(COMPort);
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        /// <param name = "phccDevice"><see cref="Phcc.Device"/> object which will be used to communicate with DOA peripherals</param>
        /// <param name = "address">Specifies the address of the HSI device on the PHCC DOA bus.</param>
        public Device(Phcc.Device phccDevice, byte address) : this()
        {
            Address = address;
            PhccDevice = phccDevice;
            _commandDispatcher = new PhccCommandDispatcher(phccDevice, address);
        }

        /// <summary>
        ///   The <see cref = "DeviceDataReceived" /> event is raised when
        ///   the device transmits data back to the host (PC).
        /// </summary>
        public event EventHandler<DeviceDataReceivedEventArgs> DeviceDataReceived;
        public ConnectionType ConnectionType { get; set; }

        public string COMPort { get; set; }
        public byte Address { get; set; }
        public Phcc.Device PhccDevice { get; set; }



        #region Protocol

        #region Public constants
        public const short STATOR_ANGLE_MAX_OFFSET = 1023; //10 bits of precision allowed
        public const short MAX_POSITION = 1023; //10 bits of precision allowed
        public const short WATCHDOG_MAX_COUNTDOWN = 63; //6 bits
        #endregion

        public void SetBearingIndication(short bearingIndicationPosition)
        {
			var rangeNum = (byte)(bearingIndicationPosition / 256);
			var positionInRange=(byte)(bearingIndicationPosition % 256);
            if (rangeNum >=0 && rangeNum <=3)
            {
				SendCommand(CommandSubaddress.BEARING_0TO90 + rangeNum, positionInRange);
			}
			else 
			{
                throw new ArgumentOutOfRangeException(nameof(bearingIndicationPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }
        public void SetHeadingIndication(short headingIndicationPosition)
        {
            var rangeNum = (byte)(headingIndicationPosition / 256);
            var positionInRange = (byte)(headingIndicationPosition % 256);
            if (rangeNum >= 0 && rangeNum <= 3)
            {
                SendCommand(CommandSubaddress.HEADING_0TO90 + rangeNum, positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(headingIndicationPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }

        public void SetRangeOnesDigitIndication(byte rangeOnesDigitValue)
        {
            if (rangeOnesDigitValue >= 0 && rangeOnesDigitValue <= 9)
            {
                SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_0TO9, rangeOnesDigitValue);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rangeOnesDigitValue),  "Must be >=0 and <=9");
            }
        }

        public void SetRangeTensDigitIndication(byte rangeTensDigitValue)
        {
            if (rangeTensDigitValue >= 0 && rangeTensDigitValue <= 9)
            {
                SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_0TO9, rangeTensDigitValue);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rangeTensDigitValue), "Must be >=0 and <=9");
            }
        }

        public void SetRangeHundredsDigitIndication(byte rangeHundredsDigitValue)
        {
            if (rangeHundredsDigitValue >= 0 && rangeHundredsDigitValue <= 9)
            {
                SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_0TO9, rangeHundredsDigitValue);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rangeHundredsDigitValue), "Must be >=0 and <=9");
            }
        }

        public void SetRangeInvalidIndicatorVisible(bool visible)
        {
            SendCommand(CommandSubaddress.RANGE_INVALID, visible ? (byte)0 : (byte)1);
        }

        public void SetDigitalOutputChannelValue(OutputChannels outputChannel, bool value)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_1:
                    SendCommand(CommandSubaddress.DIG_OUT_1, (byte)(value ? 1 : 0));
                    break;
                case OutputChannels.DIG_OUT_2:
                    SendCommand(CommandSubaddress.DIG_OUT_2, (byte)(value ? 1 : 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputChannel));
            }

        }

        public void SetBearingStatorOffset(StatorSignals statorSignal, short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0x300; //bits 8-9

            if (offset <0 || offset > STATOR_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_ANGLE_MAX_OFFSET));
            }
            var lsb = (byte)(offset & LSB_BITMASK);
            var msb = (byte)((offset & MSB_BITMASK) >>8);
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.BEARING_S1_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.BEARING_S1_OFFSET_MSB, msb);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.BEARING_S2_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.BEARING_S2_OFFSET_MSB, msb);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.BEARING_S3_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.BEARING_S3_OFFSET_MSB, msb);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetHeadingStatorOffset(StatorSignals statorSignal, short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0x300; //bits 8-9

            if (offset < 0 || offset > STATOR_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_ANGLE_MAX_OFFSET));
            }
            var lsb = (byte)(offset & LSB_BITMASK);
            var msb = (byte)((offset & MSB_BITMASK) >> 8);
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.HEADING_S1_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.HEADING_S1_OFFSET_MSB, msb);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.HEADING_S2_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.HEADING_S2_OFFSET_MSB, msb);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.HEADING_S3_OFFSET_LSB, lsb);
                    SendCommand(CommandSubaddress.HEADING_S3_OFFSET_MSB, msb);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeOnesDigitStatorOffset(StatorSignals statorSignal, byte offset)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_X_STATOR_OFFSET, offset);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_Y_STATOR_OFFSET, offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeTensDigitStatorOffset(StatorSignals statorSignal, byte offset)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_X_STATOR_OFFSET, offset);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_Y_STATOR_OFFSET, offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeHundredsDigitStatorOffset(StatorSignals statorSignal, byte offset)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_X_STATOR_OFFSET, offset);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_Y_STATOR_OFFSET, offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeIndication3DigitsAllAtOnce(short indicatedRangeMiles)
        {
            var rangeNum = (byte)(indicatedRangeMiles / 256);
            var positionInRange = (byte)(indicatedRangeMiles % 256);
            if (indicatedRangeMiles >= 0 && indicatedRangeMiles <= 999)
            {
                SendCommand(CommandSubaddress.RANGE_3DIGIT_0TO255 + rangeNum, positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(indicatedRangeMiles), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= 999"));
            }
        }
        public void SetRangeIndicationScrollMode(RangeDigitsScrollMode rangeDigitsScrollMode)
        {
            SendCommand(CommandSubaddress.RANGE_DIGITS_SCROLL_MODE, (byte)rangeDigitsScrollMode);
        }

        public void SetBearingStatorSignalCoarseSetpointDeferred(StatorSignals statorSignal, byte coarseSetpoint)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.BEARING_S1_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.BEARING_S2_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.BEARING_S3_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetBearingStatorSignalsPolaritiesAndLoad(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.BEARING_SX_POLARITY_AND_LOAD, (byte)statorSignalPolarities);
        }

        public void SetHeadingStatorSignalCoarseSetpointDeferred(StatorSignals statorSignal, byte coarseSetpoint)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.HEADING_S1_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.HEADING_S2_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.HEADING_S3_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetHeadingStatorSignalsPolaritiesAndLoad(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.HEADING_SX_POLARITY_AND_LOAD, (byte)statorSignalPolarities);
        }
        public void SetRangeOnesDigitStatorSignalCoarseSetpointDeferred(StatorSignals statorSignal, byte coarseSetpoint)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeOnesDigitStatorSignalsPolaritiesAndLoad(Polarity x, Polarity y)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (x == Polarity.Positive) statorSignalPolarities |= StatorSignals.X;
            if (y == Polarity.Positive) statorSignalPolarities |= StatorSignals.Y;
            SendCommand(CommandSubaddress.RANGE_ONES_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT, (byte)statorSignalPolarities);
        }
        public void SetRangeTensDigitStatorSignalCoarseSetpointDeferred(StatorSignals statorSignal, byte coarseSetpoint)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeTensDigitStatorSignalsPolaritiesAndLoad(Polarity x, Polarity y)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (x == Polarity.Positive) statorSignalPolarities |= StatorSignals.X;
            if (y == Polarity.Positive) statorSignalPolarities |= StatorSignals.Y;
            SendCommand(CommandSubaddress.RANGE_TENS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT, (byte)statorSignalPolarities);
        }
        public void SetRangeHundredsDigitStatorSignalCoarseSetpointDeferred(StatorSignals statorSignal, byte coarseSetpoint)
        {
            switch (statorSignal)
            {
                case StatorSignals.X:
                    SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                case StatorSignals.Y:
                    SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED, coarseSetpoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetRangeHundredsDigitStatorSignalsPolaritiesAndLoad(Polarity x, Polarity y)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (x == Polarity.Positive) statorSignalPolarities |= StatorSignals.X;
            if (y == Polarity.Positive) statorSignalPolarities |= StatorSignals.Y;
            SendCommand(CommandSubaddress.RANGE_HUNDREDS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT, (byte)statorSignalPolarities);
        }


        public void ConfigureDiagnosticLEDBehavior(DiagnosticLEDMode mode)
        {
            SendCommand(CommandSubaddress.DIAG_LED, (byte)mode);
        }

        public string Identify()
        {
            return ConnectionType == ConnectionType.USB
                ? Encoding.ASCII.GetString(SendQuery(CommandSubaddress.IDENTIFY, 0x00, 14), 0, 14)
                : null;
        }

        public void DisableWatchdog()
        {
            SendCommand(CommandSubaddress.DISABLE_WATCHDOG, 0x00);
        }
        public void ConfigureWatchdog(bool enable, byte countdown)
        {
            if (countdown > WATCHDOG_MAX_COUNTDOWN)
            {
                throw new ArgumentOutOfRangeException(nameof(countdown), string.Format(CultureInfo.InvariantCulture, "Must be <= {0}", WATCHDOG_MAX_COUNTDOWN));
            }
            var data = (byte)((enable ? 1 : 0) << 7) | countdown;
            SendCommand(CommandSubaddress.WATCHDOG_CONTROL, (byte)data);
        }

        public void ConfigureUsbDebug(bool enable)
        {
            if (ConnectionType == ConnectionType.USB)
            {
                SendCommand(CommandSubaddress.USB_DEBUG, enable ? Convert.ToByte('Y') : Convert.ToByte('N'));
            }
        }

        public void SendCommand(CommandSubaddress subaddress, byte? data=null)
        {
            _commandDispatcher.SendCommand((byte)subaddress, data);
        }
        public byte[] SendQuery(CommandSubaddress subaddress, byte? data = null, int bytesToRead = 0)
        {
            return _commandDispatcher.SendQuery((byte)subaddress, data, bytesToRead);
        }
        #endregion


        #region Destructors

        /// <summary>
        ///   Public implementation of IDisposable.Dispose().  Cleans up
        ///   managed and unmanaged resources used by this
        ///   object before allowing garbage collection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Standard finalizer, which will call Dispose() if this object
        ///   is not manually disposed.  Ordinarily called only
        ///   by the garbage collector.
        /// </summary>
        ~Device()
        {
            Dispose();
        }

        /// <summary>
        ///   Private implementation of Dispose()
        /// </summary>
        /// <param name = "disposing">flag to indicate if we should actually
        ///   perform disposal.  Distinguishes the private method signature
        ///   from the public signature.</param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                _commandDispatcher?.Dispose();
            }
            _isDisposed = true;
        }

        #endregion
    }
}