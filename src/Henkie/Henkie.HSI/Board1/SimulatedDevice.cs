using Henkie.Common;
using System.Text;

namespace Henkie.HSI.Board1
{
    public class SimulatedDevice
    {
        private readonly ISerialPortConnection _serialPortConnection = null;

        public SimulatedDevice(string comPort)
        {
            _serialPortConnection = new SerialPortConnection(comPort, openPort: true);
        }

        public void ProcessIncomingData()
        {
            while (_serialPortConnection.BytesAvailable >= 4)
            {
                var buffer = new byte[4];
                var bytesRead = _serialPortConnection.Read(buffer, 0, buffer.Length);
                if (bytesRead >= 4)
                {
                    var subaddress = buffer[0];
                    var data = buffer[1];
                    var checksum = buffer[2];
                    var delimeter = buffer[3];
                    HandleCommandOrQuery((CommandSubaddress)subaddress, data);
                }
            }
        }

        private void HandleCommandOrQuery(CommandSubaddress subaddress, byte data)
        {
            switch (subaddress)
            {
                case CommandSubaddress.BEARING_0TO90:
                    Bearing = data;
                    break;
                case CommandSubaddress.BEARING_90TO180:
                    Bearing = (short)(256 + data);
                    break;
                case CommandSubaddress.BEARING_180TO270:
                    Bearing = (short)(512 + data);
                    break;
                case CommandSubaddress.BEARING_270TO360:
                    Bearing = (short)(768 + data);
                    break;
                case CommandSubaddress.HEADING_0TO90:
                    Heading = data;
                    break;
                case CommandSubaddress.HEADING_90TO180:
                    Heading = (short)(256 + data);
                    break;
                case CommandSubaddress.HEADING_180TO270:
                    Heading = (short)(512 + data);
                    break;
                case CommandSubaddress.HEADING_270TO360:
                    Heading = (short)(768 + data);
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_0TO90:
                    RangeOnesDigit= data;
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_90TO180:
                    RangeOnesDigit= (short)(256 + data);
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_180TO270:
                    RangeOnesDigit = (short)(512 + data);
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_270TO360:
                    RangeOnesDigit = (short)(768 + data);
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_0TO90:
                    RangeTensDigit = data;
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_90TO180:
                    RangeTensDigit = (short)(256 + data);
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_180TO270:
                    RangeTensDigit = (short)(512 + data);
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_270TO360:
                    RangeTensDigit = (short)(768 + data);
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_0TO90:
                    RangeHundredsDigit= data;
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_90TO180:
                    RangeHundredsDigit= (short)(256 + data);
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_180TO270:
                    RangeHundredsDigit = (short)(512 + data);
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_270TO360:
                    RangeHundredsDigit = (short)(768 + data);
                    break;
                case CommandSubaddress.RANGE_INVALID:
                    RangeInvalid = data;
                    break;
                case CommandSubaddress.DIG_OUT_1:
                    DigOut1 = data;
                    break;
                case CommandSubaddress.DIG_OUT_2:
                    DigOut2 = data;
                    break;
                case CommandSubaddress.SET_STATOR_COIL_OFFSET_LSB:
                    StatorCoilOffsetLsb = data;
                    break;
                case CommandSubaddress.SET_STATOR_COIL_OFFSET_MSB:
                    StatorCoilOffsetMsb = data;
                    break;
                case CommandSubaddress.LOAD_BEARING_OFFSET_STATOR_COIL_MASK:
                    break;
                case CommandSubaddress.LOAD_HEADING_OFFSET_STATOR_COIL_MASK:
                    break;
                case CommandSubaddress.LOAD_RANGE_OFFSET_STATOR_COIL_MASK:
                    break;
                case CommandSubaddress.BEARING_S1_COARSE_SETPOINT_DEFERRED:
                    BearingS1CoarseSetpoint = data;
                    break;
                case CommandSubaddress.BEARING_S2_COARSE_SETPOINT_DEFERRED:
                    BearingS2CoarseSetpoint = data;
                    break;
                case CommandSubaddress.BEARING_S3_COARSE_SETPOINT_DEFERRED:
                    BearingS3CoarseSetpoint = data;
                    break;
                case CommandSubaddress.BEARING_SX_POLARITY_AND_LOAD:
                    break;
                case CommandSubaddress.HEADING_S1_COARSE_SETPOINT_DEFERRED:
                    HeadingS1CoarseSetpoint = data;
                    break;
                case CommandSubaddress.HEADING_S2_COARSE_SETPOINT_DEFERRED:
                    HeadingS2CoarseSetpoint = data;
                    break;
                case CommandSubaddress.HEADING_S3_COARSE_SETPOINT_DEFERRED:
                    HeadingS3CoarseSetpoint = data;
                    break;
                case CommandSubaddress.HEADING_SX_POLARITY_AND_LOAD:
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeOnesDigitXStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeOnesDigitYStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_ONES_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT:
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeTensDigitXStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeTensDigitYStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_TENS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT:
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_X_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeHundredsDigitXStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_Y_STATOR_COARSE_SETPOINT_DEFERRED:
                    RangeHundredsDigitYStatorCoarseSetpoint = data;
                    break;
                case CommandSubaddress.RANGE_HUNDREDS_DIGIT_POLARITY_AND_LOAD_COARSE_SETPOINT:
                    break;
                case CommandSubaddress.DISABLE_WATCHDOG:
                    break;
                case CommandSubaddress.WATCHDOG_CONTROL:
                    WatchdogControl = data;
                    break;
                case CommandSubaddress.DIAG_LED:
                    DiagLed = data;
                    break;
                case CommandSubaddress.IDENTIFY:
                    SendIdentificationString();
                    break;
                case CommandSubaddress.USB_DEBUG:
                    UsbDebug = data;
                    break;
                default:
                    break;
            }

        }
        private void SendIdentificationString()
        {
            var identificationString = "HSI #1 v2.1 $48\r\n";
            var identificationBytes = Encoding.ASCII.GetBytes(identificationString);
            _serialPortConnection.Write(identificationBytes, 0, identificationBytes.Length);
        }

        public short Bearing { get; private set; } = 0;
        public short Heading { get; private set; } = 0;
        public short RangeOnesDigit { get; private set; } = 0;
        public short RangeTensDigit { get; private set; } = 0;
        public short RangeHundredsDigit { get; private set; } = 0;
        public byte RangeInvalid { get; private set; } = 0x00;
        public byte DigOut1 { get; private set; } = 0x00;
        public byte DigOut2 { get; private set; } = 0x00;
        public byte StatorCoilOffsetLsb { get; private set; } = 0x00;
        public byte StatorCoilOffsetMsb { get; private set; } = 0x00;
        public byte BearingS1CoarseSetpoint { get; private set; } = 0x00;
        public byte BearingS2CoarseSetpoint { get; private set; } = 0x00;
        public byte BearingS3CoarseSetpoint { get; private set; } = 0x00;
        public byte HeadingS1CoarseSetpoint { get; private set; } = 0x00;
        public byte HeadingS2CoarseSetpoint { get; private set; } = 0x00;
        public byte HeadingS3CoarseSetpoint { get; private set; } = 0x00;
        public byte RangeOnesDigitXStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte RangeOnesDigitYStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte RangeTensDigitXStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte RangeTensDigitYStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte RangeHundredsDigitXStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte RangeHundredsDigitYStatorCoarseSetpoint { get; private set; } = 0x00;
        public byte DiagLed { get; private set; } = 0x00;
        public byte UsbDebug { get; private set; } = (byte)'N';
        public byte WatchdogControl { get; private set; } = 0x00;

    }
}