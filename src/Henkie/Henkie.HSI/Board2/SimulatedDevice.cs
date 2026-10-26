using Henkie.Common;
using System;
using System.IO;
using System.Text;

namespace Henkie.HSI.Board2
{
    public class SimulatedDevice
    {
        private readonly Stream _dataStream = new MemoryStream();
        private readonly byte[] _buffer = new byte[1024];
        private readonly ISerialPortConnection _serialPortConnection = null;
        private double _desiredCourse = 0;
        private double _desiredHeading = 0;

        public SimulatedDevice(string comPort)
        {
            _serialPortConnection = new SerialPortConnection(comPort);
            _serialPortConnection.DataReceived += SerialPortConnection_DataReceived;
        }

        private void SerialPortConnection_DataReceived(object sender, DeviceDataReceivedEventArgs e)
        {
            if (_serialPortConnection != null && e.EventType == SerialPortDataReceivedEventType.Chars)
            {
                while (_serialPortConnection.BytesAvailable > 0)
                {
                    Array.Clear(_buffer, 0, _buffer.Length);
                    var numBytes = _serialPortConnection.BytesAvailable <= _buffer.Length ? _serialPortConnection.BytesAvailable : _buffer.Length;
                    _serialPortConnection.Read(_buffer, 0, numBytes);
                    _dataStream.Write(_buffer, 0, numBytes);
                    _dataStream.Flush();
                }
                if (_dataStream.Length >= 4 + _dataStream.Position)
                {
                    var subaddress = (byte)_dataStream.ReadByte();
                    var data = (byte)_dataStream.ReadByte();
                    var checksum = (byte)_dataStream.ReadByte();
                    var delimeter = (byte)_dataStream.ReadByte();
                    HandleCommandOrQuery((CommandSubaddress)subaddress, data);
                    _dataStream.SetLength(0);
                    _dataStream.Position = 0;
                }
            }
        }

        private void HandleCommandOrQuery(CommandSubaddress subaddress, byte data)
        {
            switch (subaddress)
            {
                case CommandSubaddress.CDI_000TO255:
                    CourseDeviationIndicatorPosition = data;
                    break;
                case CommandSubaddress.CDI_256TO511:
                    CourseDeviationIndicatorPosition = (short)(data + 256);
                    break;
                case CommandSubaddress.CDI_512TO767:
                    CourseDeviationIndicatorPosition = (short)(data + 512);
                    break;
                case CommandSubaddress.CDI_768TO1023:
                    CourseDeviationIndicatorPosition = (short)(data + 768);
                    break;
                case CommandSubaddress.NAVIGATION_WARNING_FLAG:
                    NavigationWarningFlag = data;
                    break;
                case CommandSubaddress.TO_FROM_INDICATION:
                    ToFromIndication = data;
                    break;
                case CommandSubaddress.REQUEST_HEADING_INFO_UPDATE:
                    SendCourseAndHeadingUpdatePacket();
                    break;
                case CommandSubaddress.CONVERT_HEADING_VALUE_TO_DEGREES:
                    ConvertHeadingValueToDegrees = data;
                    break;
                case CommandSubaddress.HEADING_VALUE_HYSTERESIS_THRESHOLD:
                    HeadingValueHysteresisThreshold = data;
                    break;
                case CommandSubaddress.REQUEST_COURSE_INFO_UPDATE:
                    SendCourseAndHeadingUpdatePacket();
                    break;
                case CommandSubaddress.CONVERT_COURSE_VALUE_TO_DEGREES:
                    ConvertHeadingValueToDegrees = data;
                    break;
                case CommandSubaddress.COURSE_VALUE_HYSTERISIS_THRESHOLD:
                    CourseValueHysteresisThreshold = data;
                    break;
                case CommandSubaddress.COURSE_45_DEGREE_SIN_COS_CROSSOVER_VALUE:
                    Course45DegreeSinCosCrossoverValue = data;
                    break;
                case CommandSubaddress.USB_MESSAGING_OPTION:
                    UsbMessagingOption = data;
                    break;
                case CommandSubaddress.USB_MESSAGE_TIME_INTERVAL:
                    UsbMessageTimeInterval = data;
                    break;
                case CommandSubaddress.SINE_COSINE_ALIGNMENT:
                    SinCosAlignment = data;
                    break;
                case CommandSubaddress.COURSE_SYNCHRO_EXCITER_000TO255:
                    CourseSynchroExciterSetpoint = data;
                    break;
                case CommandSubaddress.COURSE_SYNCHRO_EXCITER_256TO511:
                    CourseSynchroExciterSetpoint = (short)(data + 256);
                    break;
                case CommandSubaddress.COURSE_SYNCHRO_EXCITER_512TO767:
                    CourseSynchroExciterSetpoint = (short)(data + 512);
                    break;
                case CommandSubaddress.COURSE_SYNCHRO_EXCITER_768TO1023:
                    CourseSynchroExciterSetpoint = (short)(data + 768);
                    break;
                case CommandSubaddress.SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_LSB:
                    CourseSynchroExciterStatorCoilOffsetLSB = data;
                    break;
                case CommandSubaddress.SET_COURSE_SYNCHRO_EXCITER_STATOR_COIL_OFFSET_MSB:
                    CourseSynchroExciterStatorCoilOffsetMSB = data;
                    break;
                case CommandSubaddress.LOAD_COURSE_SYNCHRO_EXCITER_OFFSET_STATOR_COIL_MASK:
                    break;
                case CommandSubaddress.DIG_OUT_A:
                    DigOutA = data;
                    break;
                case CommandSubaddress.DIG_OUT_B:
                    DigOutB = data;
                    break;
                case CommandSubaddress.DIG_OUT_X:
                    DigOutX = data;
                    break;
                case CommandSubaddress.COURSE_EXCITER_S1_COARSE_SETPOINT_DEFERRED:
                    CourseExciterS1 = data;
                    break;
                case CommandSubaddress.COURSE_EXCITER_S2_COARSE_SETPOINT_DEFERRED:
                    CourseExciterS2 = data;
                    break;
                case CommandSubaddress.COURSE_EXCITER_S3_COARSE_SETPOINT_DEFERRED:
                    CourseExciterS3 = data;
                    break;
                case CommandSubaddress.COURSE_EXCITER_SX_POLARITY_AND_LOAD:
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
            var identificationString = "HSI #2 v2.1 $49\r\n";
            var identificationBytes = Encoding.ASCII.GetBytes(identificationString);
            _serialPortConnection.Write(identificationBytes, 0, identificationBytes.Length);
        }
        private void SendCourseAndHeadingUpdatePacket()
        {
            var headerBytes = BitConverter.GetBytes(0x7777);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(headerBytes);
            }
            _serialPortConnection.Write(headerBytes, 0, headerBytes.Length);

            var desiredHeadingAsInt16 = Convert.ToInt16((_desiredHeading / 360.0000d) * 4096.0000d);
            var desiredHeadingBytes = BitConverter.GetBytes(desiredHeadingAsInt16);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(desiredHeadingBytes);
            }
            _serialPortConnection.Write(desiredHeadingBytes, 0, desiredHeadingBytes.Length);

            var desiredCourseAsInt16 = Convert.ToInt16(_desiredCourse / 360.0000d * 4096.0000d);
            var desiredCourseBytes = BitConverter.GetBytes(desiredCourseAsInt16);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(desiredCourseBytes);
            }
            _serialPortConnection.Write(desiredCourseBytes, 0, desiredCourseBytes.Length);

            var footerBytes = BitConverter.GetBytes(0x0D0A);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(footerBytes);
            }
            _serialPortConnection.Write(footerBytes, 0, footerBytes.Length);

        }
        public short CourseDeviationIndicatorPosition { get; private set; } = 0;
        public byte NavigationWarningFlag { get; private set; } = 0x00;
        public byte ToFromIndication { get; private set; } = 0x00;
        public byte ConvertHeadingValueToDegrees { get; private set; } = 0x00;
        public byte HeadingValueHysteresisThreshold { get; private set; } = 0x00;
        public byte ConvertCourseValueToDegrees { get; private set; } = 0x00;
        public byte CourseValueHysteresisThreshold { get; private set; } = 0x00;
        public byte Course45DegreeSinCosCrossoverValue { get; private set; } = 708 / 4;
        public byte UsbMessagingOption { get; private set; } = 0x00;
        public byte UsbMessageTimeInterval { get; private set; } = 0x25;
        public byte SinCosAlignment { get; private set; } = 0x00;
        public short CourseSynchroExciterSetpoint { get; private set; } = 0;
        public byte DigOutA { get; private set; } = 0x00;
        public byte DigOutB { get; private set; } = 0x00;
        public byte DigOutX { get; private set; } = 0x00;
        public byte DiagLed { get; private set; } = 0x00;
        public byte UsbDebug { get; private set; } = (byte)'N';
        public byte WatchdogControl { get; private set; } = 0x00;
        public byte CourseExciterS1 { get; private set; } = 0x00;
        public byte CourseExciterS2 { get; private set; } = 0x00;
        public byte CourseExciterS3 { get; private set; } = 0x00;
        public byte CourseSynchroExciterStatorCoilOffsetLSB { get; private set; } = 0x00;
        public byte CourseSynchroExciterStatorCoilOffsetMSB { get; private set; } = 0x00;

        public void SetDesiredCourse(double desiredCourse)
        {
            _desiredCourse = desiredCourse % 360.0000d;
        }
        public void SetDesiredHeading(double desiredHeading)
        {
            _desiredHeading = desiredHeading % 360.0000d;
        }
    }
}