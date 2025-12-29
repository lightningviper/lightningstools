#pragma warning disable 67
using Henkie.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Henkie.SDI
{
    /// <summary>
    ///   The <see cref = "Device" /> class provides methods for
    ///   communicating with the Synchro-to-Digital Interface (SDI) card.
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
        ///   Creates an instance of the <see cref = "Device" /> class that will communicate with the SDI board over USB.
        /// </summary>
        /// <param name = "COMPort">The name of the COM port to use for
        ///   communicating with the device (i.e. "COM1", "COM2",
        ///   etc.)</param>
        public Device(string COMPort): this()
        {
            this.COMPort = COMPort;
            _commandDispatcher = new UsbCommandDispatcher(COMPort);
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        /// <param name = "phccDevice"><see cref="Phcc.Device"/> object which will be used to communicate with DOA peripherals</param>
        /// <param name = "address">Specifies the address of the SDI device on the PHCC DOA bus.</param>
        public Device(Phcc.Device phccDevice, byte address): this()
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
        public const int POWER_DOWN_MAX_DELAY_TIME_MILLIS = 2016;
        public const short STATOR_BASE_ANGLE_MAX_OFFSET = 1023; //10 bits of precision allowed
        public const byte UPDATE_RATE_CONTROL_LIMIT_MODE_MAX_LIMIT_THRESHOLD = 63; //6 bits
        public const byte UPDATE_RATE_CONTROL_SMOOTH_MODE_MAX_SMOOTHING_MINIMUM_THRESHOLD = 15; //4 bits
        public const short UPDATE_RATE_CONTROL_MIN_STEP_UPDATE_DELAY_MILLIS = 8;
        public const short UPDATE_RATE_CONTROL_MAX_STEP_UPDATE_DELAY_MILLIS = 256;
        public const byte DEMO_MODE_MAX_MOVEMENT_STEP_SIZE = 15; //4 bits
        public const short WATCHDOG_MAX_COUNTDOWN = 63; //6 bits
        #endregion

        public void MoveIndicatorFine(Quadrant quadrant, byte position)
        {
            switch (quadrant)
            {
                case Quadrant.One:
                    SendCommand(CommandSubaddress.SSYNQ1, position);
                    break;
                case Quadrant.Two:
                    SendCommand(CommandSubaddress.SSYNQ2, position);
                    break;
                case Quadrant.Three:
                    SendCommand(CommandSubaddress.SSYNQ3, position);
                    break;
                case Quadrant.Four:
                    SendCommand(CommandSubaddress.SSYNQ4, position);
                    break;
                default:
                    throw new ArgumentException("Unknown quadrant.", nameof(quadrant));
            }
        }
        public void MoveIndicatorCoarse(byte position)
        {
            SendCommand(CommandSubaddress.SYN8BIT, position);
        }


        public void ConfigurePowerDown(PowerDownState powerDownState, PowerDownLevel powerDownLevel, short delayTimeMilliseconds)
        {
            if (delayTimeMilliseconds <0 || delayTimeMilliseconds > POWER_DOWN_MAX_DELAY_TIME_MILLIS)
            {
                throw new ArgumentOutOfRangeException(nameof(delayTimeMilliseconds), delayTimeMilliseconds, string.Format(CultureInfo.InvariantCulture, "Value must be >=0 and <= {0}", POWER_DOWN_MAX_DELAY_TIME_MILLIS));
            }
            var data =(byte)
                (
                    (byte)powerDownState |
                    (byte)powerDownLevel |
                    (delayTimeMilliseconds / 32)
                );

            SendCommand(CommandSubaddress.POWER_DOWN,data);
        }

        public void SetStatorBaseAngle(StatorSignals statorSignal, short offset)
        {
            const ushort LSB_BITMASK = 0xFF; //bits 0-7
            const ushort MSB_BITMASK = 0x300; //bits 8-9

            if (offset <0 || offset > STATOR_BASE_ANGLE_MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", STATOR_BASE_ANGLE_MAX_OFFSET));
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
        public void SetStatorSignalAmplitudeImmediate(StatorSignals statorSignal, byte amplitude)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.S1PWM, amplitude);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.S2PWM, amplitude);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.S3PWM, amplitude);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetStatorSignalsPolarityImmediate(Polarity s1, Polarity s2, Polarity s3 )
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.SXPOL, (byte)statorSignalPolarities);
        }
        public void SetStatorSignalAmplitudeDeferred(StatorSignals statorSignal, byte amplitude)
        {
            switch (statorSignal)
            {
                case StatorSignals.S1:
                    SendCommand(CommandSubaddress.S1PWMD, amplitude);
                    break;
                case StatorSignals.S2:
                    SendCommand(CommandSubaddress.S2PWMD, amplitude);
                    break;
                case StatorSignals.S3:
                    SendCommand(CommandSubaddress.S3PWMD, amplitude);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statorSignal));
            }
        }
        public void SetStatorSignalsPolarityAndLoadDeferred(Polarity s1, Polarity s2, Polarity s3)
        {
            var statorSignalPolarities = StatorSignals.Unknown;
            if (s1 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S1;
            if (s2 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S2;
            if (s3 == Polarity.Positive) statorSignalPolarities |= StatorSignals.S3;
            SendCommand(CommandSubaddress.SXPOLD, (byte)statorSignalPolarities);
        }

        public void SetIndicatorMovementLimitMinimum(byte limitMinimum)
        {
            SendCommand(CommandSubaddress.LIMIT_MIN, limitMinimum);
        }
        public void SetIndicatorMovementLimitMaximum(byte limitMaximum)
        {
            SendCommand(CommandSubaddress.LIMIT_MAX, limitMaximum);
        }


        public void ConfigureOutputChannels(OutputChannelMode digPwm1, OutputChannelMode digPwm2, OutputChannelMode digPwm3, OutputChannelMode digPwm4, OutputChannelMode digPwm5, OutputChannelMode digPwm6, OutputChannelMode digPwm7)
        {
            var outputChannelConfigurations = OutputChannels.Unknown;
            if (digPwm1 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_1;
            if (digPwm2 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_2;
            if (digPwm3 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_3;
            if (digPwm4 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_4;
            if (digPwm5 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_5;
            if (digPwm6 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_6;
            if (digPwm7 == OutputChannelMode.PWM) outputChannelConfigurations |= OutputChannels.DIG_PWM_7;
            SendCommand(CommandSubaddress.DIG_PWM, (byte)outputChannelConfigurations);
        }

        public void SetOutputChannelValue(OutputChannels outputChannel, byte value)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_PWM_1:
                    SendCommand(CommandSubaddress.DIG_PWM_1, value);
                    break;
                case OutputChannels.DIG_PWM_2:
                    SendCommand(CommandSubaddress.DIG_PWM_2, value);
                    break;
                case OutputChannels.DIG_PWM_3:
                    SendCommand(CommandSubaddress.DIG_PWM_3, value);
                    break;
                case OutputChannels.DIG_PWM_4:
                    SendCommand(CommandSubaddress.DIG_PWM_4, value);
                    break;
                case OutputChannels.DIG_PWM_5:
                    SendCommand(CommandSubaddress.DIG_PWM_5, value);
                    break;
                case OutputChannels.DIG_PWM_6:
                    SendCommand(CommandSubaddress.DIG_PWM_6, value);
                    break;
                case OutputChannels.DIG_PWM_7:
                    SendCommand(CommandSubaddress.DIG_PWM_7, value);
                    break;
                case OutputChannels.PWM_OUT:
                    SendCommand(CommandSubaddress.ONBOARD_PWM, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputChannel));
            }

        }

        public void SetUpdateRateControlModeLimit(byte limitThreshold)
        {
            if (limitThreshold > UPDATE_RATE_CONTROL_LIMIT_MODE_MAX_LIMIT_THRESHOLD)
            {
                throw new ArgumentOutOfRangeException(nameof(limitThreshold), string.Format(CultureInfo.InvariantCulture, "Must be <= {0}", UPDATE_RATE_CONTROL_LIMIT_MODE_MAX_LIMIT_THRESHOLD));
            }
            var data = (byte)((byte)UpdateRateControlModes.Limit | limitThreshold);
            SendCommand(CommandSubaddress.UPDATE_RATE_CONTROL, data);
        }
        public void SetUpdateRateControlModeSmooth(byte smoothingMinimumThresholdValue, UpdateRateControlSmoothingMode smoothingMode)
        {
            if ((smoothingMinimumThresholdValue > UPDATE_RATE_CONTROL_SMOOTH_MODE_MAX_SMOOTHING_MINIMUM_THRESHOLD))
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingMinimumThresholdValue), string.Format(CultureInfo.InvariantCulture, "Must be <= {0}", UPDATE_RATE_CONTROL_SMOOTH_MODE_MAX_SMOOTHING_MINIMUM_THRESHOLD));
            }
            var data = (byte)((byte)UpdateRateControlModes.Smooth  |
                (byte)(smoothingMinimumThresholdValue <<2) |
                (byte)smoothingMode);
            SendCommand(CommandSubaddress.UPDATE_RATE_CONTROL, data);
        }

        public void SetUpdateRateControlSpeed(short stepUpdateDelayMillis)
        {
            if (stepUpdateDelayMillis < UPDATE_RATE_CONTROL_MIN_STEP_UPDATE_DELAY_MILLIS || stepUpdateDelayMillis > UPDATE_RATE_CONTROL_MAX_STEP_UPDATE_DELAY_MILLIS)
            {
                throw new ArgumentOutOfRangeException(nameof(stepUpdateDelayMillis), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", UPDATE_RATE_CONTROL_MAX_STEP_UPDATE_DELAY_MILLIS));
            }
            var data = (byte)((byte)UpdateRateControlModes.Speed | ((stepUpdateDelayMillis-8)/8));
            SendCommand(CommandSubaddress.UPDATE_RATE_CONTROL, data);
        }

        public void SetUpdateRateControlMiscellaneous(bool shortPath)
        {
            var data = (byte)((byte)UpdateRateControlModes.Miscellaneous | (byte)(shortPath ? 1: 0));
            SendCommand(CommandSubaddress.UPDATE_RATE_CONTROL, data);
        }
        public void ConfigureDiagnosticLEDBehavior(DiagnosticLEDMode mode)
        {
            SendCommand(CommandSubaddress.DIAG_LED, (byte)mode);
        }
        public void ConfigureDemoMode(DemoMovementSpeeds movementSpeed, byte movementStepSize, DemoModus modus, bool start)
        {
            if (movementStepSize > DEMO_MODE_MAX_MOVEMENT_STEP_SIZE)
            {
                throw new ArgumentOutOfRangeException(nameof(movementStepSize), string.Format(CultureInfo.InvariantCulture, "Must be <= {0}", DEMO_MODE_MAX_MOVEMENT_STEP_SIZE));
            }
            var data = (byte)
                       (
                           ((byte)DemoBits.MovementSpeed & ((byte)((byte)movementSpeed << 6))) |
                           ((byte)DemoBits.MovementStepSize & ((byte)(movementStepSize << 2))) |
                           ((byte)DemoBits.Modus & ((byte)modus <<1)) |
                           ((byte)DemoBits.Start & ((byte)(start ? 1 : 0)))
                        );
            SendCommand(CommandSubaddress.DEMO_MODE, data);
        }
        public void SetDemoModeStartPosition(byte data)
        {
            SendCommand(CommandSubaddress.DEMO_MODE_START_POSITION, data);
        }
        public void SetDemoModeEndPosition(byte data)
        {
            SendCommand(CommandSubaddress.DEMO_MODE_END_POSITION, data);
        }

        public string Identify()
        {
            return ConnectionType == ConnectionType.USB 
                ? Encoding.ASCII.GetString(SendQuery(CommandSubaddress.IDENTIFY, 0x00, 14), 0, 14)
                : null;
        }

        public void DisableWatchdog()
        {
            SendCommand(CommandSubaddress.DISABLE_WATCHDOG);
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
        public byte[] SendQuery(CommandSubaddress subaddress, byte? data = null, int bytesToRead=0)
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