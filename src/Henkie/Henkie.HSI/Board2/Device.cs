using Henkie.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Henkie.HSI.Board2
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
        private readonly ICommandDispatcher _commandDispatcher;
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
            _commandDispatcher = new UsbCommandDispatcher(COMPort, typeof(CommandSubaddress));
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
        public const byte MAX_HYSTERESIS_THRESHOLD = 0x7F;
        public const byte MIN_COURSE_45_SIN_COS_CROSSOVER_VALUE = 125;
        public const byte MAX_COURSE_45_SIN_COS_CROSSOVER_VALUE = 225;

        #endregion

        public void SetCourseDeviationIndication(short courseDeviationIndicatorPosition)
        {
			var rangeNum = (byte)(courseDeviationIndicatorPosition / 256);
			var positionInRange=(byte)(courseDeviationIndicatorPosition % 256);
            if (rangeNum >=0 && rangeNum <=3)
            {
				SendCommand(CommandSubaddress.CDI_000TO255 + rangeNum, positionInRange);
			}
			else 
			{
                throw new ArgumentOutOfRangeException(nameof(courseDeviationIndicatorPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }

        public void SetCourseSynchroExciter(short courseSynchroExciterPosition)
        {
            var rangeNum = (byte)(courseSynchroExciterPosition / 256);
            var positionInRange = (byte)(courseSynchroExciterPosition % 256);
            if (rangeNum >= 0 && rangeNum <= 3)
            {
                SendCommand(CommandSubaddress.COURSE_SYNCHRO_EXCITER_000TO255 + rangeNum, positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(courseSynchroExciterPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }

        public void SetDeviationInvalidFlagVisible(bool visible)
        {
            SendCommand(CommandSubaddress.NAVIGATION_WARNING_FLAG, visible ? (byte)0 : (byte)1);
        }

        public void SetToFromFlagsVisible(ToFromFlagsState toFromFlagsState)
        {
            SendCommand(CommandSubaddress.TO_FROM_INDICATION, (byte)toFromFlagsState);
        }

        public HeadingAndCourseState RequestHeadingInfoUpdate()
        {
            var headingAndCourseDataPacketBytes = SendQuery(CommandSubaddress.REQUEST_HEADING_INFO_UPDATE, null, 10);
            return ParseHeadingAndCourseInfoUpdatePacket(headingAndCourseDataPacketBytes);
        }

        private HeadingAndCourseState ParseHeadingAndCourseInfoUpdatePacket(byte[] headingAndCourseDataPacketBytes)
        {
            var headingDatumValue = ReadInt16BigEndian(headingAndCourseDataPacketBytes, 2);
            if (headingDatumValue < -1023) headingDatumValue = 1023;
            if (headingDatumValue > 1023) headingDatumValue = 1023;

            var desiredCourseValue = ReadInt16BigEndian(headingAndCourseDataPacketBytes, 4);
            if (desiredCourseValue < 0) desiredCourseValue = 0;
            if (desiredCourseValue > 4095) desiredCourseValue = 4095;
            var toReturn = new HeadingAndCourseState()
            {
                HeadingDatumRaw = headingDatumValue,
                CourseKnobRaw = desiredCourseValue
            };
            return toReturn;
        }
        private static short ReadInt16BigEndian(byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length - 2) throw new ArgumentOutOfRangeException(nameof(offset));

            return (short)((buffer[offset] << 8) | buffer[offset + 1]);
        }

        public void SetHeadingValueConvertToDegreesOption(bool convertToDegrees)
        {
            SendCommand(CommandSubaddress.CONVERT_HEADING_VALUE_TO_DEGREES, (byte)(convertToDegrees ? 0x01: 0x00));
        }
        
        public void SetHeadingValueHysteresisThreshold(byte hysteresisThreshold)
        {
            if (hysteresisThreshold <0 || hysteresisThreshold > MAX_HYSTERESIS_THRESHOLD)
            {
                throw new ArgumentOutOfRangeException(nameof(hysteresisThreshold), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_HYSTERESIS_THRESHOLD));
            }
            SendCommand(CommandSubaddress.HEADING_VALUE_HYSTERESIS_THRESHOLD, hysteresisThreshold);
        }

        public HeadingAndCourseState RequestCourseInfoUpdate()
        {
            var headingAndCourseDataPacketBytes = SendQuery(CommandSubaddress.REQUEST_COURSE_INFO_UPDATE, null, 10);
            return ParseHeadingAndCourseInfoUpdatePacket(headingAndCourseDataPacketBytes);
        }

        public void SetCourseValueConvertOption(CourseValueConvertOption courseValueConvertOption)
        {
            SendCommand(CommandSubaddress.CONVERT_COURSE_VALUE_TO_DEGREES, (byte)courseValueConvertOption);
        }

        public void SetCourseValueHysteresisThreshold(byte hysteresisThreshold)
        {
            if (hysteresisThreshold < 0 || hysteresisThreshold > MAX_HYSTERESIS_THRESHOLD)
            {
                throw new ArgumentOutOfRangeException(nameof(hysteresisThreshold), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_HYSTERESIS_THRESHOLD));
            }
            SendCommand(CommandSubaddress.COURSE_VALUE_HYSTERISIS_THRESHOLD, hysteresisThreshold);
        }

        public void SetCourse45DegreeSinCosCrossoverValue(byte value)
        {
            if (value < MIN_COURSE_45_SIN_COS_CROSSOVER_VALUE || value > MAX_COURSE_45_SIN_COS_CROSSOVER_VALUE)
            {
                throw new ArgumentOutOfRangeException(nameof(value), string.Format(CultureInfo.InvariantCulture, "Must be >={0} and <= {1}", MIN_COURSE_45_SIN_COS_CROSSOVER_VALUE, MAX_COURSE_45_SIN_COS_CROSSOVER_VALUE));
            }
        }

        public void SetUsbMessagingOption(UsbMessagingOption option)
        {
            SendCommand(CommandSubaddress.USB_MESSAGING_OPTION, (byte)option);
        }

        public void SetUsbMessageTimeInterval(byte numTicks)
        {
            SendCommand(CommandSubaddress.USB_MESSAGE_TIME_INTERVAL, numTicks);
        }

        public void SetEnableSinCosRawDataOutputForAlignment(bool enabled)
        {
            SendCommand(CommandSubaddress.SINE_COSINE_ALIGNMENT, enabled ? (byte)0x01 : (byte)0x00);
        }

        public void SetDigitalOutputChannelValue(OutputChannels outputChannel, bool value)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_A:
                    SendCommand(CommandSubaddress.DIG_OUT_A, (byte)(value ? 0x01 : 0x00));
                    break;
                case OutputChannels.DIG_OUT_B:
                    SendCommand(CommandSubaddress.DIG_OUT_B, (byte)(value ? 0x01 : 0x00));
                    break;
                case OutputChannels.DIG_OUT_X:
                    SendCommand(CommandSubaddress.DIG_OUT_X, (byte)(value ? 0x01 : 0x00));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputChannel));
            }
        }

        public void SetCourseSynchroExciterStatorCoilOffset(short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0x300; //bits 8-9

            if (offset <0 || offset > STATOR_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_ANGLE_MAX_OFFSET));
            }
            var lsb = (byte)(offset & LSB_BITMASK);
            var msb = (byte)((offset & MSB_BITMASK) >>8);
            SendCommand(CommandSubaddress.SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_LSB, lsb);
            SendCommand(CommandSubaddress.SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_MSB, msb);
        }

        public void LoadCourseSynchroExciterStatorCoilOffset(StatorSignals statorSignal)
        {
            byte val;
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    val = 0x01;
                    break;
                case StatorSignals.S2:
                    val = 0x02;
                    break;
                case StatorSignals.S3:
                    val = 0x04;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
            SendCommand(CommandSubaddress.LOAD_COURSE_SYNCHRO_EXCITER_OFFSET_STATOR_COIL_MASK, val);
        }

        public void SetCourseSynchroExciterStatorCoilValueDeferred(StatorSignals statorSignal, byte value)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.COURSE_EXCITER_S1_COARSE_SETPOINT_DEFERRED, value);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.COURSE_EXCITER_S2_COARSE_SETPOINT_DEFERRED, value);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.COURSE_EXCITER_S3_COARSE_SETPOINT_DEFERRED, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetCourseSynchroExciterStatorCoilSignalsPolaritiesAndLoad(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.COURSE_EXCITER_SX_POLARITY_AND_LOAD, (byte)statorSignalPolarities);
        }

        public void ConfigureDiagnosticLEDBehavior(DiagnosticLEDMode mode)
        {
            SendCommand(CommandSubaddress.DIAG_LED, (byte)mode);
        }

        public string Identify()
        {
            return ConnectionType == ConnectionType.USB
                ? Encoding.ASCII.GetString(SendQuery(CommandSubaddress.IDENTIFY, 0x00, 17), 0, 17)
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
            if (data == 0xFF) data = 0xFE;
            _commandDispatcher.SendCommand((byte)subaddress, data, sendChecksumAndDelimiter: ConnectionType == ConnectionType.USB);
        }
        public byte[] SendQuery(CommandSubaddress subaddress, byte? data = null, int bytesToRead = 0)
        {
            if (data == 0xFF) data = 0xFE;
            return _commandDispatcher.SendQuery((byte)subaddress, data, bytesToRead, sendChecksumAndDelimiter: ConnectionType == ConnectionType.USB);
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