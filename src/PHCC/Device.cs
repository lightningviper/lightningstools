using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;

namespace Phcc
{
    /// <summary>
    ///   The <see cref = "Device" /> class provides methods for
    ///   communicating with the PHCC motherboard and 
    ///   any attached peripherals, via RS232.  
    ///   The PHCC USB interface also appears to Windows as 
    ///   a standard RS232 COM port.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComSourceInterfaces(typeof (PhccEvents))]
    [Synchronization]
    public sealed class Device : ContextBoundObject, IDisposable
    {
        /// <summary>
        ///   Bitmask for determining the index of the digital 
        ///   input whose value has changed.  
        ///   This is useful when interpreting PHCC update data packets 
        ///   that are received when the PHCC is in "talking" mode.
        /// </summary>
        private const ushort DigitalInputUpdatedIndexMask = 0x07FE;

        /// <summary>
        ///   Bitmask for determining the new value of the digital 
        ///   input whose value has 
        ///   changed.  This is useful when interpreting PHCC update 
        ///   data packets that are received when the PHCC is in "talking" 
        ///   mode.
        /// </summary>
        private const ushort DigitalInputNewValueMask = 0x0001;

        /// <summary>
        ///   Bitmask for determining the index of the analog input 
        ///   whose value has changed.  This is useful when 
        ///   interpreting PHCC update data packets
        ///   that are received when the PHCC is in "talking" mode.
        /// </summary>
        private const ushort AnalogInputUpdatedIndexMask = 0xFC00;

        /// <summary>
        ///   Bitmask for determining the new value of the analog 
        ///   input whose value has changed.  This is useful when 
        ///   interpreting PHCC update data packets that are received 
        ///   when the PHCC is in "talking" mode.
        /// </summary>
        private const ushort AnalogInputNewValueMask = 0x03FF;

        /// <summary>
        ///   Bitmask for determining the high-order bits of the address of the I2C peripheral which is sending data.
        /// </summary>
        private const ushort I2CDataReceivedAddressHighOrderBitsMask = 0x03;

        private readonly object _rs232lock = new object();

        /// <summary>
        ///   A byte buffer to store the parsed values of the 
        ///   current analog input values list.
        /// </summary>
        private short[] _currentAnalogInputsParsed = new short[35];

        /// <summary>
        ///   A byte buffer to store the un-parsed values of the 
        ///   current analog input values list.
        /// </summary>
        /* Commenting out this declaration, which matches the implementation in PHCC2HostProtocol, but which does not match the actual implementation in Firmware14 *?
        //private volatile byte[] _currentAnalogInputsRaw = new byte[45];
        */
        private byte[] _currentAnalogInputsRaw = new byte[70]; //this matches the implementation in Firmware18

        /// <summary>
        ///   A byte buffer to store the current values of the 
        ///   digital inputs.
        /// </summary>
        private byte[] _currentDigitalInputValues = new byte[128];

        /// <summary>
        ///   Flag for preventing the read buffer from being read 
        ///   during certain operations
        /// </summary>
        private bool _dontRead;

        /// <summary>
        ///   Boolean flag indicating whether this object instance has 
        ///   been <see cref = "IDisposable.Dispose()" />d.
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        ///   The name of the COM port to communicate over 
        ///   (i.e. "COM1", "COM2", etc.)
        /// </summary>
        private string _portName;

        /// <summary>
        ///   A byte buffer to use when reading data from the PHCC.
        /// </summary>
        private byte[] _readBuffer = new byte[20];

        /// <summary>
        ///   A <see cref = "System.IO.Ports.SerialPort" /> object that
        ///   hides the details of communicating via RS232 over a COM port.
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        ///   Flag to keep track of whether the PHCC motherboard is sending automatic change
        ///   notifications or not
        /// </summary>
        private bool _talking;

        /// <summary>
        ///   A byte buffer to use when writing data to the PHCC.
        /// </summary>
        private byte[] _writeBuffer = new byte[4];

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        public Device()
        {
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        /// <param name = "portName">The name of the COM port to use for 
        ///   communicating with the PHCC motherboard (i.e. "COM1", "COM2",
        ///   etc.)</param>
        public Device(string portName) : this(portName, true)
        {
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "Device" /> class.
        /// </summary>
        /// <param name = "portName">The name of the COM port to use for 
        ///   communicating with the PHCC motherboard (i.e. "COM1", "COM2",
        ///   etc.)</param>
        /// <param name = "openPort">Specifies whether to open the COM port immediately or wait till the first operation that requires doing so.</param>
        public Device(string portName, bool openPort)
            : this()
        {
            _portName = portName;
            if (openPort)
            {
                EnsurePortIsReady();
            }
        }

        /// <summary>
        ///   Gets the underlying <see cref = "System.IO.Ports.SerialPort" /> object,
        ///   which allows direct communication with the PHCC motherboard via
        ///   RS232.
        /// </summary>
        public SerialPort SerialPort
        {
            get { return _serialPort; }
        }

        /// <summary>
        ///   Gets/sets the name of the COM port to use for 
        ///   communicating with the PHCC motherboard (i.e. "COM1", "COM2", 
        ///   etc.)
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set
            {
                if (value == null || value.Trim().Equals(string.Empty))
                {
                    throw new ArgumentException(
                        "must contain a string that identifies a valid serial port on the machine (i.e. COM1, COM2, etc.)",
                        nameof(value));
                }
                ClosePort();
                _portName = value;
            }
        }

        /// <summary>
        ///   Gets a bool array containing the current values of 
        ///   all digital inputs.
        /// </summary>
        /// <returns>A bool array containing the current values of 
        ///   all digital inputs.  Each value in the array represents
        ///   a single discrete digital input, out of a total of 
        ///   1024 inputs.</returns>
        public bool[] DigitalInputs
        {
            get
            {
                var toReturn = new bool[1024];
                PollDigitalInputs();
                for (var i = 0; i < toReturn.Length; i++)
                {
                    var relevantByte = _currentDigitalInputValues[((i)/8)];
                    toReturn[i] = ((relevantByte & (byte) (Math.Pow(2, (i%8)))) != 0);
                }
                return toReturn;
            }
        }

        /// <summary>
        ///   Gets an array of 16-bit signed integers containing the current values 
        ///   of all analog inputs.  Only the low 10 bits contain information; 
        ///   the high 6 bits are always zero because the precision of the 
        ///   analog inputs is currently limited to 10 bits.
        /// </summary>
        /// <returns>An array of 16-bit signed integers containing the current
        ///   values of all analog inputs.  Only the low 10 bits 
        ///   in each array element contain useful information; 
        ///   the high 6 bits are always zero because the 
        ///   precision of the PHCC analog inputs is currently 
        ///   limited to 10 bits.</returns>
        public short[] AnalogInputs
        {
            get
            {
                PollAnalogInputs();
                return _currentAnalogInputsParsed;
            }
        }

        /// <summary>
        ///   Gets a string containing the PHCC motherboard's firmware 
        ///   version.
        /// </summary>
        /// <returns>A <see cref = "string" /> containing the PHCC motherboard's
        ///   firmware version.</returns>
        public string FirmwareVersion
        {
            get
            {
                string toReturn = null;
                var wasTalking = _talking;
                var oldDontRead = _dontRead;
                try
                {
                    EnsurePortIsReady();
                    _dontRead = true; //temporarily disable the buffer-reader event handler
                    if (_talking)
                    {
                        StopTalking();
                    }
                    else
                    {
                        WaitForInputBufferQuiesce();
                    }
                    RS232Write(" ");
                    _readBuffer.Initialize();
                    Rs232Read(_readBuffer, 0, 10);
                    toReturn = Encoding.ASCII.GetString(_readBuffer, 0, 10);
                }
                finally
                {
                    _dontRead = oldDontRead;
                    if (wasTalking) StartTalking();
                }
                return toReturn;
            }
        }

        /// <summary>
        ///   The <see cref = "DigitalInputChanged" /> event is raised when 
        ///   the PHCC motherboard detects that one of the digital inputs
        ///   has changed (i.e. whenever a button that is wired 
        ///   into the digital input key matrix is pressed or released).
        /// </summary>
        public event DigitalInputChangedEventHandler DigitalInputChanged;

        /// <summary>
        ///   The <see cref = "AnalogInputChanged" /> event is raised when
        ///   the PHCC motherboard detects that one of the analog inputs 
        ///   has changed values (i.e. whenever an analog input signal 
        ///   changes state).
        /// </summary>
        public event AnalogInputChangedEventHandler AnalogInputChanged;

        /// <summary>
        ///   The <see cref = "I2CDataReceived" /> event is raised when
        ///   the PHCC motherboard receives data from one of the attached 
        ///   I2C peripherals (if any).
        /// </summary>
        public event I2CDataReceivedEventHandler I2CDataReceived;

        /// <summary>
        ///   Closes the serial port connection.
        /// </summary>
        private void ClosePort()
        {
            lock (_rs232lock)
            {
                if (_serialPort != null)
                {
                    try
                    {
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Close();
                        }
                        _serialPort.Dispose();
                    }
                    catch 
                    {
                    }

                    _serialPort = null;
                }
            }
            Thread.Sleep(500);
        }

        /// <summary>
        ///   Establishes a serial port connection to the PHCC motherboard.
        /// </summary>
        private void InitializeSerialPort()
        {
            lock (_rs232lock)
            {
                ClosePort();
                _serialPort = new SerialPort();
                _serialPort.PortName = _portName;
                _serialPort.BaudRate = 115200;
                _serialPort.DataBits = 8;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;
                _serialPort.ReceivedBytesThreshold = 1;
                _serialPort.RtsEnable = true;
                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
                _serialPort.DataReceived += _serialPort_DataReceived;
                _serialPort.ErrorReceived += _serialPort_ErrorReceived;
                _serialPort.Open();
                GC.SuppressFinalize(_serialPort.BaseStream);
            }
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //do nothing   
        }

        /// <summary>
        ///   Instructs the PHCC motherboard to perform a software reset 
        ///   on itself.
        /// </summary>
        public void Reset()
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.Reset;
            _writeBuffer[1] = (byte) Host2PhccCommands.Reset;
            _writeBuffer[2] = (byte) Host2PhccCommands.Reset;
            RS232Write(_writeBuffer, 0, 3);
        }

        /// <summary>
        ///   Sends data to an individual HD44780-compatible character LCD 
        ///   display wired to a PHCC DOA_char_lcd character LCD driver 
        ///   daughtercard.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_char_lcd character LCD driver daughtercard to send data to.</param>
        /// <param name = "displayNum">The display number of the individual
        ///   HD44780-compatible character LCD wired to the indicated 
        ///   DOA_char_lcd character LCD driver daughtercard, to which, 
        ///   the specified <paramref name = "data" /> will be sent. </param>
        /// <param name = "mode">A value from the <see cref = "LcdDataModes" /> 
        ///   enumeration, specifying whether the value contained in 
        ///   the <paramref name = "data" /> parameter is to be considered 
        ///   Display Data or Control Data.</param>
        /// <param name = "data">The actual data value to send to the 
        ///   indicated  HD44780-compatible character LCD display.</param>
        public void DoaSendCharLcd(byte deviceAddr, byte displayNum, LcdDataModes mode, byte data)
        {
            if (displayNum == 0 || displayNum > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(displayNum), "must be between 1 and 8");
            }
            DoaSendRaw(deviceAddr, (byte) (displayNum & ((byte) mode)), data);
        }

        /// <summary>
        ///   Sends data to a DOA_AnOut1 analog output daughtercard, to
        ///   control the gain parameter which is in effect for all 
        ///   channels simultaneously.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_AnOut1 daughtercard to send data to.</param>
        /// <param name = "gain">A byte whose integer value specifies the new
        ///   value to set for the all-channels gain parameter.</param>
        public void DoaSendAnOut1GainAllChannels(byte deviceAddr, byte gain)
        {
            DoaSendRaw(deviceAddr, 0x10, gain);
        }

        /// <summary>
        ///   Sends data to a DOA_AnOut1 analog output daughtercard, to 
        ///   control the RMS voltage (using PWM) being supplied by 
        ///   that channel.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_AnOut1 daughtercard to send data to.</param>
        /// <param name = "channelNum">The channel number of the channel 
        ///   on the DOA_AnOut1 daughtercard whose PWM output voltage is 
        ///   to be set.</param>
        /// <param name = "value">A byte whose integer value controls the PWM pulse width (
        ///   delay time), which, in turn, dictates the RMS (average) voltage between
        ///   the control pins on the specified channel.</param>
        public void DoaSendAnOut1(byte deviceAddr, byte channelNum, byte value)
        {
            if (channelNum == 0 || channelNum > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(channelNum), "must be between 1 and 16");
            }
            DoaSendRaw(deviceAddr, (byte) (channelNum - 1), value);
        }

        /// <summary>
        ///   Sends data to a DOA_servo daughtercard to control the gain 
        ///   parameter of an individual servo wired to the card.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_servo daughtercard to send data to.</param>
        /// <param name = "servoNum">The number of the specific servo on the
        ///   DOA_servo daughtercard, to which this gain parameter value will be applied.</param>
        /// <param name = "gain">The new gain value to use for the specified 
        ///   servo.</param>
        public void DoaSend8ServoGain(byte deviceAddr, byte servoNum, byte gain)
        {
            if (servoNum == 0 || servoNum > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(servoNum), "must be between 1 and 8");
            }
            DoaSendRaw(deviceAddr, (byte) ((servoNum - 1) + 24), gain);
        }

        /// <summary>
        ///   Sends data to a DOA_servo daughtercard to control the 
        ///   calibration of an individual servo wired to the card.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_servo daughtercard to send data to.</param>
        /// <param name = "servoNum">The number of the specific servo on the
        ///   DOA_servo daughtercard, to which this calibration data will 
        ///   apply.</param>
        /// <param name = "calibrationOffset">The new 16-bit calibration 
        ///   offset value to use with this specific servo.</param>
        public void DoaSend8ServoCalibration(byte deviceAddr, byte servoNum, Int16 calibrationOffset)
        {
            if (servoNum == 0 || servoNum > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(servoNum), "must be between 1 and 8");
            }
            var calibrationBytes = BitConverter.GetBytes(calibrationOffset);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(calibrationBytes);
            }
            DoaSendRaw(deviceAddr, (byte) ((servoNum - 1) + 8), calibrationBytes[0]);
            DoaSendRaw(deviceAddr, (byte) ((servoNum - 1) + 16), calibrationBytes[1]);
        }

        /// <summary>
        ///   Sends data to a DOA_servo daughtercard to control the position 
        ///   of an individual servo wired to the card.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_servo daughtercard to send data to.</param>
        /// <param name = "servoNum">The number of the specific servo on the
        ///   DOA_servo daughtercard, to which this position update will 
        ///   be sent.</param>
        /// <param name = "position">The position value to set this servo 
        ///   to.</param>
        public void DoaSend8ServoPosition(byte deviceAddr, byte servoNum, byte position)
        {
            if (servoNum == 0 || servoNum > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(servoNum), "must be between 1 and 8");
            }

            DoaSendRaw(deviceAddr, (byte) (servoNum - 1), position);
        }

        /// <summary>
        ///   Sends data to a DOA_877_4067 daughtercard to control the 
        ///   7-segment LCDs (or individual LEDs) wired to the card.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_877_4067 daughtercard to send data to.</param>
        /// <param name = "displayNum">The number of the 7-segment display 
        ///   (1-48), on the specified daughtercard, that 
        ///   the <paramref name = "bits" /> parameter will control.</param>
        /// <param name = "bits">A byte, whose bits correspond to individual 
        ///   segments of the specified 7-segment display 
        ///   (including the decimal point).  
        ///   Each bit in this byte that is set to <see langref = "true" /> will 
        ///   result in a logic HIGH being sent to the 
        ///   corresponding segment (or LED).</param>
        public void DoaSend7Seg8774067(byte deviceAddr, byte displayNum, byte bits)
        {
            if (displayNum == 0 || displayNum > 48)
            {
                throw new ArgumentOutOfRangeException(nameof(displayNum), "must be between 1 and 48");
            }
            DoaSendRaw(deviceAddr, (byte) (displayNum - 1), bits);
        }

        /// <summary>
        ///   Sends data to a DOA_7Seg daughtercard to control the 7-segment 
        ///   LCDs (or individual LEDs) wired to the card.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific
        ///   DOA_7Seg daughtercard to send data to.</param>
        /// <param name = "displayNum">The number of the 7-segment display 
        ///   (1-32), on the specified daughtercard, that 
        ///   the <paramref name = "bits" /> parameter will control.</param>
        /// <param name = "bits">A byte, whose bits correspond to individual 
        ///   segments of the specified 7-segment display (including the 
        ///   decimal point).  Each bit in this byte that is set 
        ///   to <see langref = "true" /> will result in a logic HIGH 
        ///   being sent to the corresponding segment (or LED).</param>
        public void DoaSend7Seg(byte deviceAddr, byte displayNum, byte bits)
        {
            if (displayNum == 0 || displayNum > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(displayNum), "must be between 1 and 32");
            }
            DoaSendRaw(deviceAddr, (byte) (displayNum - 1), bits);
        }

        /// <summary>
        ///   Produces a 7-segment bitmask with the appropriate bits set, to represent a specific Latin character
        /// </summary>
        /// <param name = "charToConvert">a Latin character to produce a seven-segment display bitmask from</param>
        /// <returns>a byte whose bits are set appropriately for sending to a seven-segment display</returns>
        public byte CharTo7Seg(char charToConvert)
        {
            return CharTo7SegConverter.ConvertCharTo7Seg(charToConvert);
        }

        /// <summary>
        ///   Sends data to a DOA_40DO digital output daughtercard.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   DOA_40DO daughtercard to send data to.</param>
        /// <param name = "connectorNum">The output connector number to 
        ///   send data to (3=CON3, 4=CON4, 5=CON5, 6=CON6, 7=CON7)</param>
        /// <param name = "bits">A byte, whose bits correspond to the pins on 
        ///   the specified connector.  Each bit in this byte that is set to 
        ///   TRUE will result in a logic HIGH on the corresponding output 
        ///   pin on the specified connector.</param>
        public void DoaSend40DO(byte deviceAddr, byte connectorNum, byte bits)
        {
            if (connectorNum < 3 || connectorNum > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(connectorNum), "must be between 3 and 7");
            }
            DoaSendRaw(deviceAddr, (byte) (connectorNum - 3), bits);
        }

        /// <summary>
        ///   Sends data to an air core motor daughtercard.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   air core motor daughtercard to send data to.</param>
        /// <param name = "motorNum">The motor number (1-4) of the motor to control.</param>
        /// <param name = "position">A value from 0-1023 indicating the position to move the motor to.  Zero is straight north and the scale proceeds clockwise.</param>
        public void DoaSendAirCoreMotor(byte deviceAddr, byte motorNum, int position)
        {
            if (motorNum < 1 || motorNum > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(motorNum), "must be between 1 and 4");
            }
            if (position < 0 || position > 1023)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "must be between 0 and 1023");
            }

            byte quadrant = 0;
            byte pos = 0;
            if (position >= 0 && position <= 255)
            {
                quadrant = 1;
                pos = (byte) (255 - position);
            }
            else if (position >= 256 && position <= 511)
            {
                quadrant = 3;
                pos = (byte) (position - 256);
            }
            else if (position >= 512 && position <= 767)
            {
                quadrant = 4;
                pos = (byte) (255 - (position - 512));
            }
            else if (position >= 768 && position <= 1023)
            {
                quadrant = 2;
                pos = (byte) (position - 768);
            }
            var motorNumMask = (byte) ((motorNum - 1) << 2);
            var subAddress = (byte) (((byte) (quadrant - 1)) | motorNumMask);

            DoaSendRaw(deviceAddr, subAddress, pos);
        }

        /// <summary>
        ///   Sends data to a stepper motor daughtercard.
        /// </summary>
        /// <param name = "deviceAddr">The device address of the specific 
        ///   stepper motor daughtercard to send data to.</param>
        /// <param name = "motorNum">The motor number (1-4) of the motor to control.</param>
        /// <param name = "direction">A value from the <see cref = "MotorDirections" /> enumeration, indicating the direction (clockwise or counterclockwise) to move the motor (this ultimately depends on how the motor is wired to the card).</param>
        /// <param name = "numSteps">A byte, whose value (0-127) represents the number of discrete steps to command the stepper motor to move, in the indicated direction.</param>
        /// <param name = "stepType">A value from the <see cref = "MotorStepTypes" /> enumeration, indicating whether to move the motor in full-steps or in half-steps.</param>
        public void DoaSendStepperMotor(byte deviceAddr, byte motorNum, MotorDirections direction, byte numSteps,
                                        MotorStepTypes stepType)
        {
            if (motorNum < 1 || motorNum > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(motorNum), "must be between 1 and 4");
            }
            if (numSteps < 0 || numSteps > 127)
            {
                throw new ArgumentOutOfRangeException(nameof(numSteps), "must be between 0 and 127");
            }
            if (stepType == MotorStepTypes.HalfStep)
            {
                motorNum += 4;
            }
            DoaSendRaw(deviceAddr, (byte) (motorNum - 1), ((byte) ((byte) direction | numSteps)));
        }


        /// <summary>
        ///   Sends data to a Digital Output Type A (DOA) peripheral attached to
        ///   the PHCC motherboard.
        /// </summary>
        /// <param name = "addr">The device address of the specific 
        ///   Digital Output Type A (DOA)
        ///   peripheral to send data to.</param>
        /// <param name = "subAddr">The sub-address of the 
        ///   Digital Output Type A (DOA) peripheral to send data to.</param>
        /// <param name = "data">The data to send to the specified 
        ///   Output Type A (DOA) peripheral.</param>
        public void DoaSendRaw(byte addr, byte subAddr, byte data)
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.DoaSend;
            _writeBuffer[1] = addr;
            _writeBuffer[2] = subAddr;
            _writeBuffer[3] = data;
            RS232Write(_writeBuffer, 0, 4);
        }

        /// <summary>
        ///   Sends data to a Digital Output Type B (DOB) peripheral attached 
        ///   to the PHCC motherboard.
        /// </summary>
        /// <param name = "addr">The address of the 
        ///   Digital Output Type B (DOB) peripheral to send data to.</param>
        /// <param name = "data">The data to send to the specified 
        ///   Digital Output Type B (DOB) peripheral.</param>
        public void DobSendRaw(byte addr, byte data)
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.DobSend;
            _writeBuffer[1] = addr;
            _writeBuffer[2] = data;
            RS232Write(_writeBuffer, 0, 3);
        }

        /// <summary>
        ///   Sends data to an I2C peripheral attached to the 
        ///   PHCC motherboard.
        /// </summary>
        /// <param name = "addr">The address of the I2C peripheral 
        ///   to send data to.</param>
        /// <param name = "subAddr">The sub-address of the I2C 
        ///   peripheral to send data to.</param>
        /// <param name = "data">The data to send to the specified 
        ///   I2C peripheral.</param>
        public void I2CSend(byte addr, byte subAddr, byte data)
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.I2CSend;
            _writeBuffer[1] = addr;
            _writeBuffer[2] = subAddr;
            _writeBuffer[3] = data;
            RS232Write(_writeBuffer, 0, 4);
        }

        /// <summary>
        ///   Informs the PHCC motherboard to stop sending change notification events
        /// </summary>
        public void StopTalking()
        {
            /*
            if (!_talking)
            {
                return;
            }
             */
            EnsurePortIsReady();
            var oldDontRead = _dontRead;
            try
            {
                WaitForInputBufferQuiesce();
                _writeBuffer[0] = (byte) Host2PhccCommands.StopTalking;
                RS232Write(_writeBuffer, 0, 1);
                WaitForInputBufferQuiesce();
                _talking = false;
            }
            finally
            {
                _dontRead = oldDontRead;
            }
        }

        private void Rs232DiscardInBuffer()
        {
            lock (_rs232lock)
            {
                _serialPort.DiscardInBuffer();
            }
        }

        private void WaitForInputBufferQuiesce()
        {
            EnsurePortIsReady();
            var oldDontRead = _dontRead;
            try
            {
                var done = false;
                while (!done)
                {
                    Rs232DiscardInBuffer(); //now discard whatever's in the buffer
                    if (Rs232BytesAvailable() == 0)
                        //if there's nothing in the buffer then we have quiesced, otherwise wait some more
                    {
                        done = true;
                    }
                    Thread.Sleep(0);
                }
                Rs232DiscardInBuffer(); //now discard whatever's left in the buffer
            }
            finally
            {
                _dontRead = oldDontRead;
            }
        }

        /// <summary>
        ///   Reads a packet from the RS232 serial port containing a report on 
        ///   a single byte of data received from an attached I2C peripheral
        /// </summary>
        private void ReadI2CDataReceivedPacket()
        {
            Rs232Read(_readBuffer, 1, 2);
            var addressLowOrderBits = _readBuffer[1];
            var addressHighOrderBits = (byte) (_readBuffer[0] | I2CDataReceivedAddressHighOrderBitsMask);

            ushort address = 0;
            if (BitConverter.IsLittleEndian)
            {
                address = BitConverter.ToUInt16(new[] {addressLowOrderBits, addressHighOrderBits}, 0);
            }
            else
            {
                address = BitConverter.ToUInt16(new[] {addressHighOrderBits, addressLowOrderBits}, 0);
            }

            var data = _readBuffer[2];
            if (I2CDataReceived != null)
            {
                I2CDataReceived(this,
                                new I2CDataReceivedEventArgs((short) address, data));
            }
        }

        /// <summary>
        ///   Reads a packet from the RS232 serial port containing a report on 
        ///   a single analog input value change event
        /// </summary>
        private void ReadAnalogInputUpdatePacket()
        {
            Rs232Read(_readBuffer, 1, 2);
            //ushort bits = ConvertBytesToUShort(_readBuffer, 0);
            ushort bits = _readBuffer[1];
            //byte index = (byte)((bits & AnalogInputUpdatedIndexMask)>>10);
            var index = (byte) (bits >> 2);
            bits = ConvertBytesToUShort(_readBuffer, 1);
            var newValue = (ushort) (bits & AnalogInputNewValueMask);
            if (AnalogInputChanged != null)
            {
                AnalogInputChanged(this,
                                   new AnalogInputChangedEventArgs(index, (short) newValue));
            }
        }

        /// <summary>
        ///   Reads a packet from the RS232 serial port containing a report on 
        ///   a single digital input value change event
        /// </summary>
        private void ReadDigitalInputUpdatePacket()
        {
            Rs232Read(_readBuffer, 1, 1);
            var bits = ConvertBytesToUShort(_readBuffer, 0);
            var index = (ushort) ((bits & DigitalInputUpdatedIndexMask) >> 1);
            var newVal = ((bits & DigitalInputNewValueMask) != 0);
            if (DigitalInputChanged != null)
            {
                DigitalInputChanged(this,
                                    new DigitalInputChangedEventArgs((short) index, newVal));
            }
        }

        ///<summary>
        ///  Reads a packet from the RS232 serial port containing a full update of
        ///  all digital input values.
        ///</summary>
        private void ReadDigitalInputFullDumpPacket()
        {
            var oldDontRead = _dontRead;
            try
            {
                _dontRead = true;
                Rs232Read(_currentDigitalInputValues, 0, 128);
            }
            finally
            {
                _dontRead = oldDontRead;
            }
        }

        /// <summary>
        ///   Reads a packet from the RS232 serial port containing a full update of
        ///   all analog input values.
        /// </summary>
        private void ReadAnalogInputFullDumpPacket()
        {
            var oldDontRead = _dontRead;
            try
            {
                _dontRead = true;
                /* 
                //this is the implementation that matches the PHCC2HostProtocol documentation, but this is not how it is implemented in Firmware18
                Rs232Read(_currentAnalogInputsRaw, 0, 45);
                */
                Rs232Read(_currentAnalogInputsRaw, 0, 70);
            }
            finally
            {
                _dontRead = oldDontRead;
            }
            ParseRawAnalogInputs(_currentAnalogInputsRaw, out _currentAnalogInputsParsed);
        }

        /// <summary>
        ///   Informs the PHCC motherboard to start sending automatic change notification events.
        /// </summary>
        public void StartTalking()
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.StartTalking;
            RS232Write(_writeBuffer, 0, 1);
            _talking = true;
        }

        /// <summary>
        ///   Event handler responsible for reading data from the serial port when it arrives
        /// </summary>
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_rs232lock)
            {
                try
                {
                    ProcessBufferContents();
                }
                catch 
                {
                }
            }
        }

        /// <summary>
        ///   Processes the contents of the UART buffer.
        /// </summary>
        private void ProcessBufferContents()
        {
            if (_dontRead) //if reading has been temporarily disabled here so 
                //that bytes don't disappear from the buffer 
                //when needed elsewhere, then yield before reading any
                //here
            {
                return;
            }
            _readBuffer.Initialize();
            try
            {
                while (_serialPort.BytesToRead > 0)
                {
                    Rs232Read(_readBuffer, 0, 1);
                    switch ((byte) (_readBuffer[0] & (byte) Phcc2HostPacketTypes.PacketTypeMask))
                    {
                        case (byte) Phcc2HostPacketTypes.I2CDataReceivedPacket:
                            ReadI2CDataReceivedPacket();
                            break;
                        case (byte) Phcc2HostPacketTypes.AnalogInputUpdatePacket:
                            ReadAnalogInputUpdatePacket();
                            break;
                        case (byte) Phcc2HostPacketTypes.DigitalInputUpdatePacket:
                            ReadDigitalInputUpdatePacket();
                            break;
                        case (byte) Phcc2HostPacketTypes.DigitalInputsFullDumpPacket:
                            ReadDigitalInputFullDumpPacket();
                            break;
                        case (byte) Phcc2HostPacketTypes.AnalogInputsFullDumpPacket:
                            ReadAnalogInputFullDumpPacket();
                            break;
                        case (byte) Phcc2HostPacketTypes.AllBitsOne:
                            break;
                        case (byte) Phcc2HostPacketTypes.AllBitsZero:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (TimeoutException)
            {
            }
        }

        /// <summary>
        ///   Commands the PHCC motherboard to send a full report of the current
        ///   digital input values.
        /// </summary>
        private void PollDigitalInputs()
        {
            var wasTalking = _talking;
            EnsurePortIsReady();
            if (_talking)
            {
                StopTalking();
            }
            _writeBuffer[0] = (byte) Host2PhccCommands.GetCurrentDigitalInputValues;
            RS232Write(_writeBuffer, 0, 1);
            ProcessBufferContents();
            if (wasTalking)
            {
                StartTalking();
            }
        }

        /// <summary>
        ///   Commands the PHCC motherboard to send a full report of the current
        ///   analog input values
        /// </summary>
        private void PollAnalogInputs()
        {
            var wasTalking = _talking;
            EnsurePortIsReady();
            if (_talking)
            {
                StopTalking();
            }
            _writeBuffer[0] = (byte) Host2PhccCommands.GetCurrentAnalogInputValues;
            RS232Write(_writeBuffer, 0, 1);
            ProcessBufferContents();
            if (wasTalking)
            {
                StartTalking();
            }
        }

        private void RS232Write(string toWrite)
        {
            EnsurePortIsReady();
            lock (_rs232lock)
            {
                _serialPort.Write(toWrite);
            }
        }

        private void RS232Write(byte[] buffer, int index, int count)
        {
            EnsurePortIsReady();
            lock (_rs232lock)
            {
                _serialPort.Write(buffer, index, count);
            }
        }

        /// <summary>
        ///   Parses the raw analog input values list that the P
        ///   HCC motherboard provides, by combining the low and 
        ///   high bytes into a single 16-bit value for 
        ///   each analog input value in the "raw" analog values list.
        /// </summary>
        /// <param name = "raw">A byte array containing 2 bytes for each 
        ///   analog input.</param>
        /// <param name = "processed">An array of 16-bit signed integers to 
        ///   hold the result of combining corresponding pairs of bytes
        ///   from the "raw" analog input values list.</param>
        private static void ParseRawAnalogInputs(byte[] raw, out short[] processed)
        {
            /* This is the algorithm for processing the analog inputs per the current PHCC documentation, which DOES NOT match the actual implementation in Firmware18
            int curInput = 1;
            for (int i = 0; i < raw.Length; i++)
            {
                byte curByte = raw[i];
                if ((i +1) % 5 == 0 && i > 0)
                {
                    //this byte contains the high-order 2 bits of the previous 4 axes values
                    for (int j = 1; j <= 4; j++) //for each of the previously-processed 4 axes,
                    {
                        if ((curInput - j) < processed.Length)
                        {
                            int mask = 0x3 << (2 * (j - 1)); // calculate a bitmask that will pass the 2 bits of the current byte that represent the high-order 2 bits of the axis but which will block the other bits
                            byte highOrderByte = (byte)(curByte & mask); //mask out any other bits in the current byte
                            byte lowOrderByte = (byte)processed[curInput - j]; //retrieve the low-order bits from the axes-values array where they've already been placed
                            ushort combined = 0;
                            if (BitConverter.IsLittleEndian)
                            {
                                combined = BitConverter.ToUInt16(new byte[] { lowOrderByte, highOrderByte }, 0);
                            }
                            else
                            {
                                combined = BitConverter.ToUInt16(new byte[] { highOrderByte, lowOrderByte }, 0);
                            }
                            processed[curInput - j] = (short)combined;
                        }
                    }
                    continue;
                }
                else
                {
                    //this byte contains the low-order 8 bits of the current axis
                    if (curInput <= processed.Length)
                    {
                        processed[curInput - 1] = (short)curByte;
                    }
                    curInput++;
                }
            }*/
            //this is the implementation for firmware18
            processed = new short[35];
            for (var i = 0; i < raw.Length; i += 2)
            {
                processed[i/2] = (short) ConvertBytesToUShort(raw, i);
            }
        }

        /// <summary>
        ///   Establishes a connection to the PHCC motherboard via RS232.
        /// </summary>
        private void EnsurePortIsReady()
        {
            lock (_rs232lock)
            {
                if (_serialPort == null || !_serialPort.IsOpen)
                {
                    InitializeSerialPort();
                }
            }
        }

        /// <summary>
        ///   Instructs the PHCC motherboard to enter the IDLE state.
        /// </summary>
        public void SetIdle()
        {
            EnsurePortIsReady();
            _writeBuffer[0] = (byte) Host2PhccCommands.Idle;
            RS232Write(_writeBuffer, 0, 1);
        }

        private int Rs232BytesAvailable()
        {
            lock (_rs232lock)
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    return _serialPort.BytesToRead;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        ///   Reads the speficied number of bytes synchronously, from the 
        ///   RS232 COM port interface, into the specified buffer, 
        ///   filling the buffer starting at the specified index.
        /// </summary>
        /// <param name = "buffer">a byte array that will store the 
        ///   results of the read operation.</param>
        /// <param name = "index">the index in 
        ///   the <paramref name = "buffer" /> where the bytes which are 
        ///   being read from the serial port, will be written.</param>
        /// <param name = "count">the number of bytes to read
        ///   from the serial port.</param>
        private void Rs232Read(byte[] buffer, int index, int count)
        {
            var oldDontRead = _dontRead;
            EnsurePortIsReady();
            lock (_rs232lock)
            {
                try
                {
                    _dontRead = true;
                    var startTime = DateTime.UtcNow;
                    var timeOut = 0;
                    lock (_rs232lock)
                    {
                        timeOut = _serialPort.ReadTimeout;
                    }

                    var bytesAvailable = Rs232BytesAvailable();
                    while (bytesAvailable < count)
                    {
                        bytesAvailable = Rs232BytesAvailable();
                        if (DateTime.UtcNow > startTime.AddMilliseconds(timeOut) && timeOut != Timeout.Infinite)
                        {
                            throw new TimeoutException();
                        }
                        Thread.Sleep(15);
                    }
                    _serialPort.Read(buffer, index, count);
                    if (Rs232BytesAvailable() > 0)
                    {
                    }
                }
                finally
                {
                    _dontRead = oldDontRead;
                }
            }
        }

        /// <summary>
        ///   Converts a pair of bytes to a 16-bit, unsigned integer
        /// </summary>
        /// <param name = "value">a byte array containing bytes to combine.</param>
        /// <param name = "startIndex">the index within the byte array 
        ///   indicating the first byte of the pair to combine.</param>
        /// <returns></returns>
        private static ushort ConvertBytesToUShort(byte[] value, int startIndex)
        {
            ushort toReturn = 0;
            if (BitConverter.IsLittleEndian)
            {
                var toSwap = new byte[2];
                toSwap[0] = value[startIndex];
                toSwap[1] = value[startIndex + 1];
                Array.Reverse(toSwap);
                toReturn = BitConverter.ToUInt16(toSwap, 0);
            }
            else
            {
                toReturn = BitConverter.ToUInt16(value, startIndex);
            }
            return toReturn;
        }

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
            if (!_isDisposed)
            {
                if (disposing)
                {
                    ClosePort(); //disconnect from the PHCC
                }
            }
            _isDisposed = true;
        }

        #endregion
    }
}