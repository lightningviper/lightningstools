using Henkie.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Henkie.QuadSinCos
{
    /// <summary>
    ///   The <see cref = "Device" /> class provides methods for
    ///   communicating with the Quad SIN/COS interface.
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
        public Device() { }

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class that will communicate with the Quad SIN/COS board over USB.
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
        /// <param name = "address">Specifies the address of the Quad SIN/COS device on the PHCC DOA bus.</param>
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
        public const short MAX_POSITION = 1023; //10 bits of precision allowed
        public const short WATCHDOG_MAX_COUNTDOWN = 63; //6 bits
        #endregion

        public void SetPitchAngleDegrees(float pitchAngleDegrees)
        {
            if (!IsNumeric(pitchAngleDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(pitchAngleDegrees), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }
            SetPitchPosition(CalculatePositionForAngle(pitchAngleDegrees));
        }

        public void SetPitchPosition(short pitchPosition)
        {
            var rangeNum = (byte)(pitchPosition / 256);
            var positionInRange = (byte)(pitchPosition % 256);
            if (rangeNum >= 0 && rangeNum <= 3)
            {
                SendCommand((CommandSubaddress.PITCH_0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(pitchPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }

        }

        public void SetRollAngleDegrees(float rollAngleDegrees)
        {
            if (!IsNumeric(rollAngleDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(rollAngleDegrees), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }
            SetRollPosition(CalculatePositionForAngle(rollAngleDegrees));
        }

        public void SetRollPosition(short rollPosition)
        {
            var rangeNum = (byte)(rollPosition / 256);
            var positionInRange = (byte)(rollPosition % 256);
            if (rangeNum >= 0 && rangeNum <= 3)
            {
                SendCommand((CommandSubaddress.ROLL_0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rollPosition), string.Format(CultureInfo.InvariantCulture, "Must be >=0 and <= {0}", MAX_POSITION));
            }
        }

        public void SetSADIOffFlag(bool newValue)
        {
            SendCommand(CommandSubaddress.SADI_OFF_FLAG, (byte)(newValue ? 1 : 0));
        }

        public void SetAngleImmediate(float angleDegrees, byte deviceNum)
        {
            if (!IsNumeric(angleDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(angleDegrees), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }
            if (deviceNum > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(deviceNum), string.Format(CultureInfo.InvariantCulture, "Must be >= 0 and <= 3"));
            }

            var sine = (float)Math.Sin(angleDegrees * (Math.PI / 180.0f));
            var cosine = (float)Math.Cos(angleDegrees * (Math.PI / 180.0f));
            SetSineDeferred(sine);
            SetCosineDeffered(cosine);
            LoadDeferredSineAndCosine(deviceNum);
        }

        public void SetPullToCageRollAngle(float angleDegrees)
        {
            if (!IsNumeric(angleDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(angleDegrees), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }

            var sine = (float)Math.Sin(angleDegrees * (Math.PI / 180.0f));
            var cosine = (float)Math.Cos(angleDegrees * (Math.PI / 180.0f));
            SetPullToCageRollAngleSine(sine);
            SetPullToCageRollAngleCosine(cosine);
        }

        public void SetPullToCageRollAngleSine(float sine)
        {
            if (IsValidSineOrCosineValue(sine))
            {
                var position = (sine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.SIN_R0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(sine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }

        public void SetPullToCageRollAngleCosine(float cosine)
        {
            if (IsValidSineOrCosineValue(cosine))
            {
                var position = (cosine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.COS_R0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(cosine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }

        public void SetPullToCagePitchAngle(float angleDegrees)
        {
            if (!IsNumeric(angleDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(angleDegrees), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }

            var sine = (float)Math.Sin(angleDegrees * (Math.PI / 180.0f));
            var cosine = (float)Math.Cos(angleDegrees * (Math.PI / 180.0f));
            SetPullToCagePitchAngleSine(sine);
            SetPullToCagePitchAngleCosine(cosine);
        }

        public void SetPullToCagePitchAngleSine(float sine)
        {
            if (IsValidSineOrCosineValue(sine))
            {
                var position = (sine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.SIN_P0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(sine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }

        public void SetPullToCagePitchAngleCosine(float cosine)
        {
            if (IsValidSineOrCosineValue(cosine))
            {
                var position = (cosine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.COS_P0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(cosine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }


        public void SetSineDeferred(float sine)
        {
            if (IsValidSineOrCosineValue(sine))
            {
                var position = (sine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.SIN_0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(sine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }

        public void SetCosineDeffered(float cosine)
        {
            if (IsValidSineOrCosineValue(cosine))
            {
                var position = (cosine * 512) + 512;
                if (position < 0) position = 0;
                if (position > MAX_POSITION) position = MAX_POSITION;

                var rangeNum = (byte)(position / 256);
                var positionInRange = (byte)(position % 256);

                SendCommand((CommandSubaddress.COS_0 + rangeNum), positionInRange);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(cosine), string.Format(CultureInfo.InvariantCulture, "Must be between -1.0 and 1.0"));
            }
        }

        public void LoadDeferredSineAndCosine(byte deviceNum)
        {
            if (deviceNum <= 3)
            {
                SendCommand(CommandSubaddress.LOAD_SIN_COS, deviceNum);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(deviceNum), string.Format(CultureInfo.InvariantCulture, "Must be >= 0 and <= 3"));
            }
        }


        public void SetDigitalOutputDevice2And3Connector(bool newValue)
        {
            SendCommand(CommandSubaddress.DIG_OUT_2_3, (byte)(newValue ? 1 : 0));
        }


        public short CalculatePositionForAngle(float angle)
        {
            if (!IsNumeric(angle))
            {
                throw new ArgumentOutOfRangeException(nameof(angle), string.Format(CultureInfo.InvariantCulture, "Must be numeric"));
            }
            angle = angle % 360;
            if (angle < 0)
            {
                angle = 360 - Math.Abs(angle);
            }

            return (short)((angle / 360) * 1023);
        }

        private bool IsNumeric(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private bool IsValidSineOrCosineValue(float value)
        {
            return IsNumeric(value) && value >= -1 && value <= 1;
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

        public void SendCommand(CommandSubaddress subaddress, byte? data = null)
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