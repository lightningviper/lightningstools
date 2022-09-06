#pragma warning disable 67
using Henkie.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Henkie.Altimeter
{
    /// <summary>
    ///   The <see cref = "Device" /> class provides methods for
    ///   communicating with the Altimeter interface.
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
        ///   Creates an instance of the <see cref = "Device" /> class that will communicate with the Altimeter board over USB.
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
        /// <param name = "address">Specifies the address of the Altimeter device on the PHCC DOA bus.</param>
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
        public const short STATOR_ANGLE_MAX_OFFSET = 4095; //12 bits of precision allowed
        public const short MAX_POSITION = 4095; //12 bits of precision allowed
        public const short WATCHDOG_MAX_COUNTDOWN = 63; //6 bits
        #endregion

        public void SetPosition(short position)
        {

			var rangeNum = (byte)(position /256);
			var positionInRange=(byte)(position %256);
            if (rangeNum >=0 && rangeNum <=15)
            {
				SendCommand(CommandSubaddress.SALT_0 + rangeNum, positionInRange);
			}
			else 
			{
                throw new ArgumentOutOfRangeException(nameof(position), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }

        public void SetStatorBaseAngle(StatorSignals statorSignal, short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0xF00; //bits 8-11

            if (offset <0 || offset > STATOR_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_ANGLE_MAX_OFFSET));
            }
            var lsb = (byte)(offset & LSB_BITMASK);
            var msb = (byte)((offset & MSB_BITMASK) >>8);
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.S1_BASE_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S1_BASE_ANGLE_MSB, msb);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.S2_BASE_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S2_BASE_ANGLE_MSB, msb);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.S3_BASE_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S3_BASE_ANGLE_MSB, msb);
                    break;
                case StatorSignals.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetStatorSignalAmplitudeDeferred(StatorSignals statorSignal, byte amplitude)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.S1AMPL, amplitude);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.S2AMPL, amplitude);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.S3AMPL, amplitude);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetStatorSignalsPolaritiesAndLoad(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.SxPOL, (byte)statorSignalPolarities);
        }

		
		public void SetStatorHighAccuracyAngleDeferred(StatorSignals statorSignal, short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0xF00; //bits 8-11

            if (offset <0 || offset > STATOR_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_ANGLE_MAX_OFFSET));
            }
            var lsb = (byte)(offset & LSB_BITMASK);
            var msb = (byte)((offset & MSB_BITMASK) >>8);
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.S1_HIGH_ACCURACY_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S1_HIGH_ACCURACY_ANGLE_MSB, msb);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.S2_HIGH_ACCURACY_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S2_HIGH_ACCURACY_ANGLE_MSB, msb);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.S3_HIGH_ACCURACY_ANGLE_LSB, lsb);
                    SendCommand(CommandSubaddress.S3_HIGH_ACCURACY_ANGLE_MSB, msb);
                    break;
                case StatorSignals.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetStatorHighAccuracyPolaritiesAndLoad(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.SXPOLD, (byte)statorSignalPolarities);
        }

		
		
        public void SetDigitalOutputChannelValue(OutputChannels outputChannel, bool value)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_1:
                    SendCommand(CommandSubaddress.DIG_OUT_1, (byte)(value ? 1:0));
                    break;
                case OutputChannels.DIG_OUT_2:
                    SendCommand(CommandSubaddress.DIG_OUT_2, (byte)(value ? 1 : 0));
                    break;
                case OutputChannels.DIG_OUT_3:
                    SendCommand(CommandSubaddress.DIG_OUT_3, (byte)(value ? 1 : 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputChannel));
            }

        }
		public void TestModeStart() 
		{
			SendCommand(CommandSubaddress.TEST_MODE_START, 0x00);
		}
		public void TestModeStop() 
		{
			SendCommand(CommandSubaddress.TEST_MODE_STOP, 0x00);
		}
		public void TestModeSetStepUpdateRate(byte updateRate) 
		{
			SendCommand(CommandSubaddress.TEST_MODE_STEP_UPDATE_RATE, updateRate);
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