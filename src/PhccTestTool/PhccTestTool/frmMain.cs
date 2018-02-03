using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Phcc;

namespace PhccTestTool
{
    public partial class frmMain : Form
    {
        private readonly bool[] _DOA40DOdigitalOutputs = new bool[40];
        private readonly bool[] _DOB74595TdigitalOutputs = new bool[16];

        private readonly Point[] _segmentAPoints = new[]
                                                       {
                                                           new Point(29, 14), new Point(26, 18), new Point(29, 21),
                                                           new Point(43, 21), new Point(47, 17), new Point(43, 14)
                                                       };

        private readonly Point[] _segmentBPoints = new[]
                                                       {
                                                           new Point(48, 19), new Point(45, 22), new Point(45, 33),
                                                           new Point(48, 37), new Point(51, 34), new Point(51, 22)
                                                       };

        private readonly Point[] _segmentCPoints = new[]
                                                       {
                                                           new Point(48, 40), new Point(45, 43), new Point(45, 54),
                                                           new Point(48, 57), new Point(51, 54), new Point(51, 43)
                                                       };

        private readonly Point[] _segmentDPoints = new[]
                                                       {
                                                           new Point(29, 55), new Point(25, 59), new Point(29, 62),
                                                           new Point(43, 62), new Point(47, 59), new Point(43, 55)
                                                       };

        private readonly Point[] _segmentEPoints = new[]
                                                       {
                                                           new Point(24, 40), new Point(21, 43), new Point(21, 55),
                                                           new Point(24, 57), new Point(27, 54), new Point(27, 43)
                                                       };

        private readonly Point[] _segmentFPoints = new[]
                                                       {
                                                           new Point(24, 19), new Point(21, 22), new Point(21, 33),
                                                           new Point(24, 36), new Point(27, 33), new Point(27, 22)
                                                       };

        private readonly Point[] _segmentGPoints = new[]
                                                       {
                                                           new Point(29, 35), new Point(26, 38), new Point(29, 41),
                                                           new Point(43, 41), new Point(46, 38), new Point(43, 35)
                                                       };

        private readonly byte[] _sevenSegBits = new byte[32];
        private bool _allowAnOut1Sending = true;
        private short[] _analogInputs;
        private bool[] _digitalInputs;
        private Device _phccDevice = new Device();
        private ReadOnlyCollection<string> _serialPorts;
        private Point _sevenSegDecimalPointCenter = new Point(57, 58);
        private Rectangle _sevenSegDecimalPointRectangle = Rectangle.Empty;
        private Thread _splashThread;
        private TextBox _txtDoa40doDevAddr;

        public frmMain()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public ReadOnlyCollection<string> SerialPorts
        {
            get { return _serialPorts; }
        }

        private void txtCharLcdDeviceAddress_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtCharLcdDeviceAddress, out val);
        }

        private bool ValidateHexTextControl(TextBox textControl, out byte val)
        {
            // First create an instance of the call stack   
            var callStack = new StackTrace();
            var frames = callStack.GetFrames();
            for (var i = 1; i < frames.Length; i++)
            {
                var frame = frames[i];
                var method = frame.GetMethod();
                // Get the declaring type and method names    
                var declaringType = method.DeclaringType.Name;
                var methodName = method.Name;
                if (methodName != null && methodName.Contains("ValidateHex"))
                {
                    val = 0x00;
                    return true;
                }
            }

            ResetErrors();
            var text = textControl.Text.Trim().ToUpperInvariant();
            textControl.Text = text;
            var parsed = false;
            if (text.StartsWith("0X"))
            {
                text = text.Substring(2, text.Length - 2);
                textControl.Text = "0x" + text;
                parsed = Byte.TryParse(text, NumberStyles.HexNumber, null, out val);
            }
            else
            {
                parsed = Byte.TryParse(text, out val);
            }
            if (!parsed)
            {
                epErrorProvider.SetError(textControl,
                                         "Invalid hexadecimal or decimal byte value.\nHex values should be preceded by the\ncharacters '0x' (zero, x) and\nshould be in the range 0x00-0xFF.\nDecimal values should be in the range 0-255.");
            }
            return parsed;
        }

        private void txtLcdDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtCharLcdDataByte, out val);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte devAddr = 0;
            byte dataByte = 0;
            var valid = true;
            valid = ValidateHexTextControl(txtCharLcdDeviceAddress, out devAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtCharLcdDataByte, out dataByte);
            if (!valid) return;

            var displayNum = (int) nudCharLcdDisplayNumber.Value;
            var dataMode = LcdDataModes.DisplayData;

            if (cbCharLcdDataMode.SelectedText.ToLowerInvariant().Contains("control"))
            {
                dataMode = LcdDataModes.ControlData;
            }
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendCharLcd(devAddr, (byte) displayNum, dataMode, dataByte);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void EnumerateSerialPorts()
        {
            var ports = new Ports();
            _serialPorts = ports.SerialPortNames;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _splashThread = new Thread(DoSplash);
            _splashThread.Start();
            Application.DoEvents();
            epErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            Text = Application.ProductName + " v" + Application.ProductVersion;
            _sevenSegDecimalPointRectangle = new Rectangle(_sevenSegDecimalPointCenter.X - 3,
                                                           _sevenSegDecimalPointCenter.Y - 3, 6, 6);
            keyMatrixCardTabs.TabPages.Add("tabDigitalInputs", "Digital Inputs");
            var digitalInputsOuterFlowLayoutPanel = new FlowLayoutPanel();
            digitalInputsOuterFlowLayoutPanel.Name = "digitalInputsOuterFlowLayoutPanel";
            digitalInputsOuterFlowLayoutPanel.Dock = DockStyle.Fill;
            digitalInputsOuterFlowLayoutPanel.Margin = new Padding(0);
            digitalInputsOuterFlowLayoutPanel.Padding = new Padding(0);
            digitalInputsOuterFlowLayoutPanel.AutoScroll = true;
            keyMatrixCardTabs.TabPages["tabDigitalInputs"].Controls.Add(digitalInputsOuterFlowLayoutPanel);

            for (var i = 1; i <= 16; i++)
            {
                var gbDigitalInputs = new GroupBox();
                gbDigitalInputs.Name = "gbDigitalInputs" + i;
                gbDigitalInputs.Text = "KEY" + i;
                gbDigitalInputs.Size = new Size(410, 180);
                digitalInputsOuterFlowLayoutPanel.Controls.Add(gbDigitalInputs);
                digitalInputsOuterFlowLayoutPanel.SetFlowBreak(gbDigitalInputs, false);
                var panel = new FlowLayoutPanel();
                panel.Dock = DockStyle.Fill;
                panel.Margin = new Padding(0);
                panel.Padding = new Padding(0);
                panel.Name = "digitalInputsKey" + i + "FlowLayoutPanel";
                gbDigitalInputs.Controls.Add(panel);
            }

            for (var i = 1; i <= 1024; i++)
            {
                var flowPanelName = "digitalInputsKey" + (((i - 1)/64) + 1) + "FlowLayoutPanel";
                var groupBoxName = "gbDigitalInputs" + (((i - 1)/64) + 1);
                var button = new RadioButton();
                button.Name = "rdoDigitalInput" + i;
                button.Checked = false;
                button.Text = (((i - 1)%64) + 1).ToString();
                button.AutoSize = false;
                button.Width = 50;
                button.Height = 20;
                button.Padding = new Padding(0);
                button.Margin = new Padding(0);
                button.AutoCheck = false;
                keyMatrixCardTabs.TabPages["tabDigitalInputs"].Controls["digitalInputsOuterFlowLayoutPanel"].Controls[
                    groupBoxName].Controls[flowPanelName].Controls.Add(button);

                if (i%16 == 0)
                {
                    ((FlowLayoutPanel)
                     keyMatrixCardTabs.TabPages["tabDigitalInputs"].Controls["digitalInputsOuterFlowLayoutPanel"].
                         Controls[groupBoxName].Controls[flowPanelName]).SetFlowBreak(button, true);
                }
            }

            {
                var digitalOutputsOuterFlowPanel = new FlowLayoutPanel();
                digitalOutputsOuterFlowPanel.Dock = DockStyle.Fill;
                digitalOutputsOuterFlowPanel.Name = "DigitalOutputsOuterFlowLayoutPanel";
                tabDigitalOutput.Controls.Add(digitalOutputsOuterFlowPanel);

                var gbDoa40do = new GroupBox();
                gbDoa40do.Text = "DOA_40DO";

                var doa40doFlowPanel = new FlowLayoutPanel();
                doa40doFlowPanel.Dock = DockStyle.Fill;
                doa40doFlowPanel.Margin = new Padding(0);
                doa40doFlowPanel.Padding = new Padding(0);
                doa40doFlowPanel.Name = "DOA40DOFlowLayoutPanel";

                var lblDoa40doDevAddr = new Label();
                lblDoa40doDevAddr.Name = "lblDoa40doDevAddr";
                lblDoa40doDevAddr.Text = "Device &Address:";
                var padding = new Padding();
                padding.Top = 5;
                lblDoa40doDevAddr.Padding = padding;

                lblDoa40doDevAddr.AutoSize = true;
                doa40doFlowPanel.Controls.Add(lblDoa40doDevAddr);

                _txtDoa40doDevAddr = new TextBox();
                _txtDoa40doDevAddr.Name = "txtDoa40doDevAddr";
                _txtDoa40doDevAddr.Width = txtAnOut1DevAddr.Width;
                _txtDoa40doDevAddr.Height = txtAnOut1DevAddr.Height;
                _txtDoa40doDevAddr.Leave += txtDoa40doDevAddr_Leave;

                doa40doFlowPanel.Controls.Add(_txtDoa40doDevAddr);
                doa40doFlowPanel.SetFlowBreak(_txtDoa40doDevAddr, true);

                var cmdDoa40doTurnAllOutputsOn = new Button();
                cmdDoa40doTurnAllOutputsOn.AutoSize = true;
                cmdDoa40doTurnAllOutputsOn.AutoSizeMode = AutoSizeMode.GrowOnly;
                cmdDoa40doTurnAllOutputsOn.Text = "All Outputs &ON";
                cmdDoa40doTurnAllOutputsOn.Name = "cmdDoa40doTurnAllOutputsOn";
                cmdDoa40doTurnAllOutputsOn.Click += cmdDoa40doTurnAllOutputsOn_Click;
                doa40doFlowPanel.Controls.Add(cmdDoa40doTurnAllOutputsOn);
                doa40doFlowPanel.SetFlowBreak(cmdDoa40doTurnAllOutputsOn, false);

                var cmdDoa40doTurnAllOutputsOff = new Button();
                cmdDoa40doTurnAllOutputsOff.AutoSize = true;
                cmdDoa40doTurnAllOutputsOff.AutoSizeMode = AutoSizeMode.GrowOnly;
                cmdDoa40doTurnAllOutputsOff.Text = "All Outputs O&FF";
                cmdDoa40doTurnAllOutputsOff.Name = "cmdDoa40doTurnAllOutputsOff";
                cmdDoa40doTurnAllOutputsOff.Click += cmdDoa40doTurnAllOutputsOff_Click;
                doa40doFlowPanel.Controls.Add(cmdDoa40doTurnAllOutputsOff);
                doa40doFlowPanel.SetFlowBreak(cmdDoa40doTurnAllOutputsOff, true);


                for (var i = 1; i <= 40; i++)
                {
                    var doa40doOutputButton = new RadioButton();
                    doa40doOutputButton.Name = "rdoDoa40DoDigitalOutput" + i;
                    doa40doOutputButton.Checked = false;
                    doa40doOutputButton.Text = i.ToString();
                    doa40doOutputButton.AutoSize = false;
                    doa40doOutputButton.Width = 50;
                    doa40doOutputButton.Height = 20;
                    doa40doOutputButton.Padding = new Padding(0);
                    doa40doOutputButton.Margin = new Padding(0);
                    doa40doOutputButton.AutoCheck = false;
                    doa40doOutputButton.CheckedChanged += doa40doOutputButton_CheckChanged;
                    doa40doOutputButton.Tag = i;
                    doa40doFlowPanel.Controls.Add(doa40doOutputButton);
                    doa40doOutputButton.Click += doa40doOutputButton_Click;

                    if (i%8 == 0)
                    {
                        doa40doFlowPanel.SetFlowBreak(doa40doOutputButton, true);
                    }
                }
                gbDoa40do.Size = new Size(420, 200);
                gbDoa40do.Padding = new Padding(10);
                gbDoa40do.Controls.Add(doa40doFlowPanel);
                digitalOutputsOuterFlowPanel.Controls.Add(gbDoa40do);
                digitalOutputsOuterFlowPanel.SetFlowBreak(gbDoa40do, true);

                var gbDob74595t = new GroupBox();
                gbDob74595t.Size = new Size(420, 170);
                gbDob74595t.Padding = new Padding(10);
                gbDob74595t.Text = "DOB_74595+T";
                gbDob74595t.Enabled = false;

                var dob74595tFlowPanel = new FlowLayoutPanel();
                dob74595tFlowPanel.Dock = DockStyle.Fill;
                dob74595tFlowPanel.Margin = new Padding(0);
                dob74595tFlowPanel.Padding = new Padding(0);
                dob74595tFlowPanel.Name = "dob74595tFlowPanel ";

                var lbldob74595tDevAddr = new Label();
                lbldob74595tDevAddr.Name = "lbldob74595tDevAddr";
                lbldob74595tDevAddr.Text = "Device &Address:";
                padding = new Padding();
                padding.Top = 5;
                lbldob74595tDevAddr.Padding = padding;

                lbldob74595tDevAddr.AutoSize = true;
                dob74595tFlowPanel.Controls.Add(lbldob74595tDevAddr);

                var txtDob74595tDevAddr = new TextBox();
                txtDob74595tDevAddr.Name = "txtDob74595tDevAddr";
                txtDob74595tDevAddr.Width = txtAnOut1DevAddr.Width;
                txtDob74595tDevAddr.Height = txtAnOut1DevAddr.Height;
                txtDob74595tDevAddr.Leave += txtDob74595tDevAddr_Leave;

                dob74595tFlowPanel.Controls.Add(txtDob74595tDevAddr);
                dob74595tFlowPanel.SetFlowBreak(txtDob74595tDevAddr, true);

                for (var i = 1; i <= 16; i++)
                {
                    var dob74595tOutputButton = new RadioButton();
                    dob74595tOutputButton.Name = "rdodob74595tDigitalOutput" + i;
                    dob74595tOutputButton.Checked = false;
                    dob74595tOutputButton.Text = i.ToString();
                    dob74595tOutputButton.AutoSize = false;
                    dob74595tOutputButton.Width = 50;
                    dob74595tOutputButton.Height = 20;
                    dob74595tOutputButton.Padding = new Padding(0);
                    dob74595tOutputButton.Margin = new Padding(0);
                    dob74595tOutputButton.AutoCheck = false;
                    dob74595tOutputButton.CheckedChanged += dob74595tOutputButton_CheckedChanged;
                    dob74595tOutputButton.Tag = i;
                    dob74595tFlowPanel.Controls.Add(dob74595tOutputButton);

                    if (i%8 == 0)
                    {
                        dob74595tFlowPanel.SetFlowBreak(dob74595tOutputButton, true);
                    }
                }
                gbDob74595t.Controls.Add(dob74595tFlowPanel);
                digitalOutputsOuterFlowPanel.Controls.Add(gbDob74595t);
            }

            RenderNeedleGauge(pbAirCore1, 0, 0, 1023, 360);
            RenderNeedleGauge(pbAirCore2, 0, 0, 1023, 360);
            RenderNeedleGauge(pbAirCore3, 0, 0, 1023, 360);
            RenderNeedleGauge(pbAirCore4, 0, 0, 1023, 360);

            RenderNeedleGauge(pbServo1, (int) nudServo1Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo2, (int) nudServo2Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo3, (int) nudServo3Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo4, (int) nudServo4Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo5, (int) nudServo5Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo6, (int) nudServo6Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo7, (int) nudServo7Position.Value, 0, 255, 180);
            RenderNeedleGauge(pbServo8, (int) nudServo8Position.Value, 0, 255, 180);

            RenderAll7Segs();

            EnumerateSerialPorts();
            cbSerialPort.Sorted = true;
            foreach (var port in _serialPorts)
            {
                cbSerialPort.Items.Add(port);
                cbSerialPort.Text = port;
                Application.DoEvents();
            }
            ResetErrors();
            cbCharLcdDataMode.SelectedItem = "Display Data";
            txtI2CDataReceived.AppendText("Address:");
            txtI2CDataReceived.AppendText(Environment.NewLine + "Data:");
            try
            {
                _splashThread.Abort();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            Activate();
        }

        private void cmdDoa40doTurnAllOutputsOff_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < 40; i++)
            {
                var button = (RadioButton) Controls.Find("rdoDoa40DoDigitalOutput" + (i + 1), true)[0];

                button.Checked = false;
            }
        }

        private void cmdDoa40doTurnAllOutputsOn_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < 40; i++)
            {
                var button = (RadioButton) Controls.Find("rdoDoa40DoDigitalOutput" + (i + 1), true)[0];
                button.Checked = true;
            }
        }

        private void RenderAll7Segs()
        {
            render7Seg(pbSevenSegDisplay1, _sevenSegBits[0]);
            render7Seg(pbSevenSegDisplay2, _sevenSegBits[1]);
            render7Seg(pbSevenSegDisplay3, _sevenSegBits[2]);
            render7Seg(pbSevenSegDisplay4, _sevenSegBits[3]);
            render7Seg(pbSevenSegDisplay5, _sevenSegBits[4]);
            render7Seg(pbSevenSegDisplay6, _sevenSegBits[5]);
            render7Seg(pbSevenSegDisplay7, _sevenSegBits[6]);
            render7Seg(pbSevenSegDisplay8, _sevenSegBits[7]);
            render7Seg(pbSevenSegDisplay9, _sevenSegBits[8]);
            render7Seg(pbSevenSegDisplay10, _sevenSegBits[9]);
            render7Seg(pbSevenSegDisplay11, _sevenSegBits[10]);
            render7Seg(pbSevenSegDisplay12, _sevenSegBits[11]);
            render7Seg(pbSevenSegDisplay13, _sevenSegBits[12]);
            render7Seg(pbSevenSegDisplay14, _sevenSegBits[13]);
            render7Seg(pbSevenSegDisplay15, _sevenSegBits[14]);
            render7Seg(pbSevenSegDisplay16, _sevenSegBits[15]);
            render7Seg(pbSevenSegDisplay17, _sevenSegBits[16]);
            render7Seg(pbSevenSegDisplay18, _sevenSegBits[17]);
            render7Seg(pbSevenSegDisplay19, _sevenSegBits[18]);
            render7Seg(pbSevenSegDisplay20, _sevenSegBits[19]);
            render7Seg(pbSevenSegDisplay21, _sevenSegBits[20]);
            render7Seg(pbSevenSegDisplay22, _sevenSegBits[21]);
            render7Seg(pbSevenSegDisplay23, _sevenSegBits[22]);
            render7Seg(pbSevenSegDisplay24, _sevenSegBits[23]);
            render7Seg(pbSevenSegDisplay25, _sevenSegBits[24]);
            render7Seg(pbSevenSegDisplay26, _sevenSegBits[25]);
            render7Seg(pbSevenSegDisplay27, _sevenSegBits[26]);
            render7Seg(pbSevenSegDisplay28, _sevenSegBits[27]);
            render7Seg(pbSevenSegDisplay29, _sevenSegBits[28]);
            render7Seg(pbSevenSegDisplay30, _sevenSegBits[29]);
            render7Seg(pbSevenSegDisplay31, _sevenSegBits[30]);
            render7Seg(pbSevenSegDisplay32, _sevenSegBits[31]);
        }

        private void doa40doOutputButton_Click(object sender, EventArgs e)
        {
            var digitalOutputButton = ((RadioButton) sender);
            digitalOutputButton.Checked = !digitalOutputButton.Checked;
        }

        private void txtDob74595tDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl((TextBox) sender, out val);
        }

        private void txtDoa40doDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl((TextBox) sender, out val);
            if (valid) SendDoa40DOOutputs();
        }

        private void dob74595tOutputButton_CheckedChanged(object sender, EventArgs e)
        {
            var digitalOutputButton = ((RadioButton) sender);
            var buttonNumber = Int32.Parse(digitalOutputButton.Tag.ToString());
            _DOB74595TdigitalOutputs[buttonNumber] = digitalOutputButton.Checked;
        }

        private void doa40doOutputButton_CheckChanged(object sender, EventArgs e)
        {
            var digitalOutputButton = ((RadioButton) sender);
            var buttonNumber = Int32.Parse(digitalOutputButton.Tag.ToString()) - 1;
            _DOA40DOdigitalOutputs[buttonNumber] = digitalOutputButton.Checked;
            SendDoa40DOOutputs();
        }

        private void SendDoa40DOOutputs()
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(_txtDoa40doDevAddr, out devAddr);
            if (valid)
            {
                for (var i = 3; i <= 7; i++)
                {
                    byte toSend = 0;
                    for (var j = 0; j < 8; j++)
                    {
                        if (_DOA40DOdigitalOutputs[((i - 3)*8) + j])
                        {
                            toSend |= (byte) (1 << j);
                        }
                    }
                    if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                    {
                        try
                        {
                            _phccDevice.DoaSend40DO(devAddr, (byte) i, toSend);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    Application.DoEvents();
                }
            }
        }


        private void cmdAnOut1ZeroAllSliders_Click(object sender, EventArgs e)
        {
            _allowAnOut1Sending = false;
            //tbAnOut1Gain.Value = 0;
            tbAnOut1Chan1PulseWidth.Value = 0;
            tbAnOut1Chan2PulseWidth.Value = 0;
            tbAnOut1Chan3PulseWidth.Value = 0;
            tbAnOut1Chan4PulseWidth.Value = 0;
            tbAnOut1Chan5PulseWidth.Value = 0;
            tbAnOut1Chan6PulseWidth.Value = 0;
            tbAnOut1Chan7PulseWidth.Value = 0;
            tbAnOut1Chan8PulseWidth.Value = 0;
            tbAnOut1Chan9PulseWidth.Value = 0;
            tbAnOut1Chan10PulseWidth.Value = 0;
            tbAnOut1Chan11PulseWidth.Value = 0;
            tbAnOut1Chan12PulseWidth.Value = 0;
            tbAnOut1Chan13PulseWidth.Value = 0;
            tbAnOut1Chan14PulseWidth.Value = 0;
            tbAnOut1Chan15PulseWidth.Value = 0;
            tbAnOut1Chan16PulseWidth.Value = 0;
            _allowAnOut1Sending = true;
            SendAnOut1Updates();
        }

        private void SendAnOut1Updates()
        {
            SendAnOut1Gain();
            SendAnOut1Channel1();
            SendAnOut1Channel2();
            SendAnOut1Channel3();
            SendAnOut1Channel4();
            SendAnOut1Channel5();
            SendAnOut1Channel6();
            SendAnOut1Channel7();
            SendAnOut1Channel8();
            SendAnOut1Channel9();
            SendAnOut1Channel10();
            SendAnOut1Channel11();
            SendAnOut1Channel12();
            SendAnOut1Channel13();
            SendAnOut1Channel14();
            SendAnOut1Channel15();
            SendAnOut1Channel16();
        }

        private void cbSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeSerialPort();
        }

        private bool ChangeSerialPort()
        {
            var isPhcc = false;

            var selectedPort = cbSerialPort.Text;
            if (String.IsNullOrEmpty(selectedPort)) return false;
            try
            {
                if (_phccDevice != null) DisposePhccDevice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            _phccDevice = null;
            try
            {
                _phccDevice = new Device(selectedPort);
                GC.SuppressFinalize(_phccDevice.SerialPort.BaseStream);
                var firmwareVersion = _phccDevice.FirmwareVersion;
                if (firmwareVersion != null && firmwareVersion.ToLowerInvariant().Trim().StartsWith("phcc"))
                {
                    isPhcc = true;
                    lblFirmwareVersion.Text = "Firmware Version:" + firmwareVersion;
                }
            }
            catch (Exception g)
            {
                lblFirmwareVersion.Text = "Firmware Version:";
                Debug.Write(g);
            }

            try
            {
                if (_phccDevice != null && isPhcc)
                {
                    _analogInputs = _phccDevice.AnalogInputs;
                    _digitalInputs = _phccDevice.DigitalInputs;
                    _analogInputs = _phccDevice.AnalogInputs;
                    _digitalInputs = _phccDevice.DigitalInputs;
                    _analogInputs = _phccDevice.AnalogInputs;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (_analogInputs != null)
            {
                for (var i = 0; i <= 2; i++)
                {
                    var controls = Controls.Find(string.Format("pbPrioAnalogInput{0:0}", i + 1), true);
                    if (controls != null)
                    {
                        var control = ((ProgressBar) controls[0]);
                        if (control != null)
                        {
                            control.Value = _analogInputs[i];
                        }
                    }
                }
                for (var i = 3; i < _analogInputs.Length; i++)
                {
                    var controls = Controls.Find(string.Format("pbAnalogInput{0:0}", i - 2), true);
                    if (controls != null)
                    {
                        var control = ((ProgressBar) controls[0]);
                        if (control != null)
                        {
                            control.Value = _analogInputs[i];
                        }
                    }
                }
            }
            if (_digitalInputs != null)
            {
                for (var i = 0; i < _digitalInputs.Length; i++)
                {
                    var controls = Controls.Find("rdoDigitalInput" + ((i + 1)), true);
                    if (controls != null)
                    {
                        var control = (RadioButton) controls[0];
                        if (control != null) control.Checked = _digitalInputs[i];
                    }
                }
            }
            if (_phccDevice != null && isPhcc)
            {
                _phccDevice.AnalogInputChanged += _phccDevice_AnalogInputChanged;
                _phccDevice.DigitalInputChanged += _phccDevice_DigitalInputChanged;
                _phccDevice.I2CDataReceived += _phccDevice_I2CDataReceived;
            }
            try
            {
                if (_phccDevice != null && isPhcc)
                {
                    _phccDevice.StartTalking();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            ResetErrors();
            return isPhcc;
        }

        private void _phccDevice_I2CDataReceived(object sender, I2CDataReceivedEventArgs e)
        {
            var address = e.Address.ToString("X").PadLeft(4, '0');
            var data = e.Data.ToString("X").PadLeft(4, '0');
            txtI2CDataReceived.AppendText(Environment.NewLine);
            txtI2CDataReceived.AppendText("Address:" + address);
            txtI2CDataReceived.AppendText(Environment.NewLine);
            txtI2CDataReceived.AppendText("Data:" + data);
        }

        private void _phccDevice_DigitalInputChanged(object sender, DigitalInputChangedEventArgs e)
        {
            if (_digitalInputs != null)
            {
                _digitalInputs[e.Index] = e.NewValue;
                var controls = Controls.Find("rdoDigitalInput" + ((e.Index + 1)), true);
                if (controls != null)
                {
                    var control = (RadioButton) controls[0];
                    if (control != null) control.Checked = _digitalInputs[e.Index];
                }
            }
        }

        private void _phccDevice_AnalogInputChanged(object sender, AnalogInputChangedEventArgs e)
        {
            if (_analogInputs != null)
            {
                _analogInputs[e.Index] = e.NewValue;
            }
            Control[] controls = null;
            if (e.Index <= 2)
            {
                controls = tabAnalogInputs.Controls.Find(string.Format("pbPrioAnalogInput{0:0}", e.Index + 1), true);
            }
            else
            {
                Debug.WriteLine("Analog input changed on axis:" + e.Index);
                Debug.WriteLine("New value is :" + e.NewValue);
                controls = tabAnalogInputs.Controls.Find(string.Format("pbAnalogInput{0:0}", e.Index - 2), true);
            }
            if (controls != null)
            {
                ((ProgressBar) controls[0]).Value = e.NewValue;
            }
        }

        private void tbAirCore1_Scroll(object sender, EventArgs e)
        {
            nudAirCore1.Value = tbAirCore1.Value;
            RenderNeedleGauge(pbAirCore1, tbAirCore1.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void tbAirCore2_Scroll(object sender, EventArgs e)
        {
            nudAirCore2.Value = tbAirCore2.Value;
            RenderNeedleGauge(pbAirCore2, tbAirCore2.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void tbAirCore3_Scroll(object sender, EventArgs e)
        {
            nudAirCore3.Value = tbAirCore3.Value;
            RenderNeedleGauge(pbAirCore3, tbAirCore3.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void tbAirCore4_Scroll(object sender, EventArgs e)
        {
            nudAirCore4.Value = tbAirCore4.Value;
            RenderNeedleGauge(pbAirCore4, tbAirCore4.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void SendAirCore()
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtAirCoreDevAddr, out devAddr);
            if (!valid) return;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendAirCoreMotor(devAddr, 1, tbAirCore1.Value);
                    _phccDevice.DoaSendAirCoreMotor(devAddr, 2, tbAirCore2.Value);
                    _phccDevice.DoaSendAirCoreMotor(devAddr, 3, tbAirCore3.Value);
                    _phccDevice.DoaSendAirCoreMotor(devAddr, 4, tbAirCore4.Value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void RenderNeedleGauge(PictureBox pb, int value, int scaleMin, int scaleMax, int degrees)
        {
            if (pb.Image == null) pb.Image = new Bitmap(pb.Width, pb.Height);
            using (var g = Graphics.FromImage(pb.Image))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                var blackPen = new Pen(Color.Black);
                for (var i = 0; i <= degrees; i += 5)
                {
                    var lineLength = 0;
                    if (i%15 == 0)
                    {
                        blackPen.Width = 1;
                        lineLength = 15;
                    }
                    else
                    {
                        blackPen.Width = 1;
                        lineLength = 8;
                    }
                    g.ResetTransform();
                    g.TranslateTransform(pb.Width/2, pb.Height/2);
                    g.RotateTransform(i);
                    g.TranslateTransform(-pb.Width/2, -pb.Height/2);
                    g.DrawLine(blackPen, new Point(pb.Width/2, 0), new Point(pb.Width/2, lineLength));
                }

                var pct = (value/(float) scaleMax);
                var angle = pct*degrees;
                g.ResetTransform();
                g.TranslateTransform(pb.Width/2, pb.Height/2);
                g.RotateTransform(angle);
                g.TranslateTransform(-pb.Width/2, -pb.Height/2);
                g.DrawLine(blackPen, new Point(pb.Width/2, 0), new Point(pb.Width/2, pb.Height/2));
                pb.Update();
            }
            pb.Refresh();
        }

        private void txtAirCoreDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtAirCoreDevAddr, out val);
            SendAirCore();
        }

        private void render7Seg(PictureBox pb, byte bits)
        {
            if (pb.Image == null) pb.Image = new Bitmap(pb.Width, pb.Height);
            using (var g = Graphics.FromImage(pb.Image))
            {
                var greenPen = new Pen(Color.FromArgb(0, 255, 0));
                Brush greenBrush = new SolidBrush(greenPen.Color);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                g.DrawPolygon(greenPen, _segmentAPoints);
                g.DrawPolygon(greenPen, _segmentBPoints);
                g.DrawPolygon(greenPen, _segmentCPoints);
                g.DrawPolygon(greenPen, _segmentDPoints);
                g.DrawPolygon(greenPen, _segmentEPoints);
                g.DrawPolygon(greenPen, _segmentFPoints);
                g.DrawPolygon(greenPen, _segmentGPoints);
                g.DrawEllipse(greenPen, _sevenSegDecimalPointRectangle);

                if ((bits & (byte) SevenSegmentBits.SegmentDP) == (byte) SevenSegmentBits.SegmentDP)
                {
                    g.FillEllipse(greenBrush, _sevenSegDecimalPointRectangle);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentA) == (byte) SevenSegmentBits.SegmentA)
                {
                    g.FillPolygon(greenBrush, _segmentAPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentB) == (byte) SevenSegmentBits.SegmentB)
                {
                    g.FillPolygon(greenBrush, _segmentBPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentC) == (byte) SevenSegmentBits.SegmentC)
                {
                    g.FillPolygon(greenBrush, _segmentCPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentD) == (byte) SevenSegmentBits.SegmentD)
                {
                    g.FillPolygon(greenBrush, _segmentDPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentE) == (byte) SevenSegmentBits.SegmentE)
                {
                    g.FillPolygon(greenBrush, _segmentEPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentF) == (byte) SevenSegmentBits.SegmentF)
                {
                    g.FillPolygon(greenBrush, _segmentFPoints);
                }
                if ((bits & (byte) SevenSegmentBits.SegmentG) == (byte) SevenSegmentBits.SegmentG)
                {
                    g.FillPolygon(greenBrush, _segmentGPoints);
                }

                pb.Update();
            }

            pb.Refresh();
        }

        private void pbSevenSegDisplay1_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay1, new Point(args.X, args.Y), 1);
        }

        private void Evaluate7SegPicBoxClick(PictureBox pb, Point clickLocation, int displayNum)
        {
            var currentBits = _sevenSegBits[displayNum - 1];
            var bits = new BitArray(new[] {currentBits});
            if (PolygonContainsPoint(_segmentGPoints, clickLocation))
            {
                bits[0] = !bits[0];
            }
            else if (PolygonContainsPoint(_segmentFPoints, clickLocation))
            {
                bits[1] = !bits[1];
            }
            else if (PolygonContainsPoint(_segmentEPoints, clickLocation))
            {
                bits[2] = !bits[2];
            }
            else if (PolygonContainsPoint(_segmentDPoints, clickLocation))
            {
                bits[3] = !bits[3];
            }
            else if (PolygonContainsPoint(_segmentCPoints, clickLocation))
            {
                bits[5] = !bits[5];
            }
            else if (PolygonContainsPoint(_segmentBPoints, clickLocation))
            {
                bits[6] = !bits[6];
            }
            else if (PolygonContainsPoint(_segmentAPoints, clickLocation))
            {
                bits[7] = !bits[7];
            }
            else if (RectangleContainsPoint(_sevenSegDecimalPointRectangle, clickLocation))
            {
                bits[4] = !bits[4];
            }
            byte newBits = 0;
            for (var i = 0; i < 8; i++)
            {
                if (bits[i]) newBits |= (byte) (1 << i);
            }
            _sevenSegBits[displayNum - 1] = newBits;
            render7Seg(pb, newBits);
            SendDoaSevenSegOutputs();
        }

        private void SendDoaSevenSegOutputs()
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txt7SegDevAddr, out devAddr);
            if (valid)
            {
                for (var i = 0; i < 32; i++)
                {
                    if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                    {
                        try
                        {
                            _phccDevice.DoaSend7Seg(devAddr, (byte) (i + 1), _sevenSegBits[i]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    Application.DoEvents();
                }
            }
        }

        private bool RectangleContainsPoint(Rectangle rect, Point point)
        {
            if (
                point.X >= rect.X
                && point.X <= (rect.X + rect.Width)
                && point.Y >= rect.Y
                && point.Y <= (rect.Y + rect.Height)
                )
            {
                return true;
            }
            return false;
        }

        private bool PolygonContainsPoint(Point[] polygon, Point point)
        {
            int i, j;
            var c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X <
                     (polygon[j].X - polygon[i].X)*(point.Y - polygon[i].Y)/(polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }
            return c;
        }

        private void pbSevenSegDisplay2_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay2, new Point(args.X, args.Y), 2);
        }

        private void pbSevenSegDisplay3_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay3, new Point(args.X, args.Y), 3);
        }

        private void pbSevenSegDisplay4_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay4, new Point(args.X, args.Y), 4);
        }

        private void pbSevenSegDisplay5_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay5, new Point(args.X, args.Y), 5);
        }

        private void pbSevenSegDisplay6_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay6, new Point(args.X, args.Y), 6);
        }

        private void pbSevenSegDisplay7_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay7, new Point(args.X, args.Y), 7);
        }

        private void pbSevenSegDisplay8_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay8, new Point(args.X, args.Y), 8);
        }

        private void pbSevenSegDisplay9_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay9, new Point(args.X, args.Y), 9);
        }

        private void pbSevenSegDisplay10_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay10, new Point(args.X, args.Y), 10);
        }

        private void pbSevenSegDisplay11_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay11, new Point(args.X, args.Y), 11);
        }

        private void pbSevenSegDisplay12_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay12, new Point(args.X, args.Y), 12);
        }

        private void pbSevenSegDisplay13_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay13, new Point(args.X, args.Y), 13);
        }

        private void pbSevenSegDisplay14_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay14, new Point(args.X, args.Y), 14);
        }

        private void pbSevenSegDisplay15_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay15, new Point(args.X, args.Y), 15);
        }

        private void pbSevenSegDisplay16_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay16, new Point(args.X, args.Y), 16);
        }

        private void pbSevenSegDisplay17_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay17, new Point(args.X, args.Y), 17);
        }

        private void pbSevenSegDisplay18_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay18, new Point(args.X, args.Y), 18);
        }

        private void pbSevenSegDisplay19_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay19, new Point(args.X, args.Y), 19);
        }

        private void pbSevenSegDisplay20_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay20, new Point(args.X, args.Y), 20);
        }

        private void pbSevenSegDisplay21_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay21, new Point(args.X, args.Y), 21);
        }

        private void pbSevenSegDisplay22_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay22, new Point(args.X, args.Y), 22);
        }

        private void pbSevenSegDisplay23_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay23, new Point(args.X, args.Y), 23);
        }

        private void pbSevenSegDisplay24_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay24, new Point(args.X, args.Y), 24);
        }

        private void pbSevenSegDisplay25_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay25, new Point(args.X, args.Y), 25);
        }

        private void pbSevenSegDisplay26_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay26, new Point(args.X, args.Y), 26);
        }

        private void pbSevenSegDisplay27_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay27, new Point(args.X, args.Y), 27);
        }

        private void pbSevenSegDisplay28_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay28, new Point(args.X, args.Y), 28);
        }

        private void pbSevenSegDisplay29_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay29, new Point(args.X, args.Y), 29);
        }

        private void pbSevenSegDisplay30_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay30, new Point(args.X, args.Y), 30);
        }

        private void pbSevenSegDisplay31_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay31, new Point(args.X, args.Y), 31);
        }

        private void pbSevenSegDisplay32_Click(object sender, EventArgs e)
        {
            var args = ((MouseEventArgs) e);
            Evaluate7SegPicBoxClick(pbSevenSegDisplay32, new Point(args.X, args.Y), 32);
        }

        private void cmdMoveStepper1_Click(object sender, EventArgs e)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtStepperDevAddr, out devAddr);
            if (!valid) return;
            var direction = MotorDirections.Clockwise;
            if (rdoStepper1DirectionCounterClockwise.Checked) direction = MotorDirections.Counterclockwise;
            var stepType = MotorStepTypes.FullStep;
            if (rdoStepper1StepTypeHalfStep.Checked) stepType = MotorStepTypes.HalfStep;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendStepperMotor(devAddr, 1, direction, (byte) (nudStepper1NumSteps.Value - 1),
                                                    stepType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void cmdMoveStepper2_Click(object sender, EventArgs e)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtStepperDevAddr, out devAddr);
            if (!valid) return;
            var direction = MotorDirections.Clockwise;
            if (rdoStepper2DirectionCounterClockwise.Checked) direction = MotorDirections.Counterclockwise;
            var stepType = MotorStepTypes.FullStep;
            if (rdoStepper2StepTypeHalfStep.Checked) stepType = MotorStepTypes.HalfStep;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendStepperMotor(devAddr, 2, direction, (byte) (nudStepper2NumSteps.Value - 1),
                                                    stepType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void cmdMoveStepper3_Click(object sender, EventArgs e)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtStepperDevAddr, out devAddr);
            if (!valid) return;
            var direction = MotorDirections.Clockwise;
            if (rdoStepper3DirectionCounterClockwise.Checked) direction = MotorDirections.Counterclockwise;
            var stepType = MotorStepTypes.FullStep;
            if (rdoStepper3StepTypeHalfStep.Checked) stepType = MotorStepTypes.HalfStep;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendStepperMotor(devAddr, 3, direction, (byte) (nudStepper3NumSteps.Value - 1),
                                                    stepType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void cmdMoveStepper4_Click(object sender, EventArgs e)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtStepperDevAddr, out devAddr);
            if (!valid) return;
            var direction = MotorDirections.Clockwise;
            if (rdoStepper4DirectionCounterClockwise.Checked) direction = MotorDirections.Counterclockwise;
            var stepType = MotorStepTypes.FullStep;
            if (rdoStepper4StepTypeHalfStep.Checked) stepType = MotorStepTypes.HalfStep;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendStepperMotor(devAddr, 4, direction, (byte) (nudStepper4NumSteps.Value - 1),
                                                    stepType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void txtStepperDevAddr_Leave(object sender, EventArgs e)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txtStepperDevAddr, out devAddr);
        }

        private void tbAnOut1Chan1PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel1();
        }

        private void SendAnOut1Channel1()
        {
            SendAnOut1ChannelValue(1, (byte) (255 - tbAnOut1Chan1PulseWidth.Value));
        }

        private void SendAnOut1Channel2()
        {
            SendAnOut1ChannelValue(2, (byte) (255 - tbAnOut1Chan2PulseWidth.Value));
        }

        private void SendAnOut1Channel3()
        {
            SendAnOut1ChannelValue(3, (byte) (255 - tbAnOut1Chan3PulseWidth.Value));
        }

        private void SendAnOut1Channel4()
        {
            SendAnOut1ChannelValue(4, (byte) (255 - tbAnOut1Chan4PulseWidth.Value));
        }

        private void SendAnOut1Channel5()
        {
            SendAnOut1ChannelValue(5, (byte) (255 - tbAnOut1Chan5PulseWidth.Value));
        }

        private void SendAnOut1Channel6()
        {
            SendAnOut1ChannelValue(6, (byte) (255 - tbAnOut1Chan6PulseWidth.Value));
        }

        private void SendAnOut1Channel7()
        {
            SendAnOut1ChannelValue(7, (byte) (255 - tbAnOut1Chan7PulseWidth.Value));
        }

        private void SendAnOut1Channel8()
        {
            SendAnOut1ChannelValue(8, (byte) (255 - tbAnOut1Chan8PulseWidth.Value));
        }

        private void SendAnOut1Channel9()
        {
            SendAnOut1ChannelValue(9, (byte) (255 - tbAnOut1Chan9PulseWidth.Value));
        }

        private void SendAnOut1Channel10()
        {
            SendAnOut1ChannelValue(10, (byte) (255 - tbAnOut1Chan10PulseWidth.Value));
        }

        private void SendAnOut1Channel11()
        {
            SendAnOut1ChannelValue(11, (byte) (255 - tbAnOut1Chan11PulseWidth.Value));
        }

        private void SendAnOut1Channel12()
        {
            SendAnOut1ChannelValue(12, (byte) (255 - tbAnOut1Chan12PulseWidth.Value));
        }

        private void SendAnOut1Channel13()
        {
            SendAnOut1ChannelValue(13, (byte) (255 - tbAnOut1Chan13PulseWidth.Value));
        }

        private void SendAnOut1Channel14()
        {
            SendAnOut1ChannelValue(14, (byte) (255 - tbAnOut1Chan14PulseWidth.Value));
        }

        private void SendAnOut1Channel15()
        {
            SendAnOut1ChannelValue(15, (byte) (255 - tbAnOut1Chan15PulseWidth.Value));
        }

        private void SendAnOut1Channel16()
        {
            SendAnOut1ChannelValue(16, (byte) (255 - tbAnOut1Chan16PulseWidth.Value));
        }

        private void SendAnOut1ChannelValue(byte channel, byte value)
        {
            byte devAddr = 0;
            var valid = true;
            valid = ValidateHexTextControl(txtAnOut1DevAddr, out devAddr);
            if (!valid) return;
            if (!_allowAnOut1Sending) return;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendAnOut1(devAddr, channel, value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void SendAnOut1Gain()
        {
            byte devAddr = 0;
            var valid = true;
            valid = ValidateHexTextControl(txtAnOut1DevAddr, out devAddr);
            if (!valid) return;
            if (!_allowAnOut1Sending) return;
            if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
            {
                try
                {
                    _phccDevice.DoaSendAnOut1GainAllChannels(devAddr, (byte) (255 - tbAnOut1Gain.Value));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void tbAnOut1Chan2PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel2();
        }

        private void tbAnOut1Chan3PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel3();
        }

        private void tbAnOut1Chan4PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel4();
        }

        private void tbAnOut1Chan5PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel5();
        }

        private void tbAnOut1Chan6PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel6();
        }

        private void tbAnOut1Chan7PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel7();
        }

        private void tbAnOut1Chan8PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel8();
        }

        private void tbAnOut1Chan9PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel9();
        }

        private void tbAnOut1Chan10PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel10();
        }

        private void tbAnOut1Chan11PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel11();
        }

        private void tbAnOut1Chan12PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel12();
        }

        private void tbAnOut1Chan13PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel13();
        }

        private void tbAnOut1Chan14PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel14();
        }

        private void tbAnOut1Chan15PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel15();
        }

        private void tbAnOut1Chan16PulseWidth_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Channel16();
        }

        private void tbAnOut1Gain_Scroll(object sender, EventArgs e)
        {
            SendAnOut1Gain();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposePhccDevice();
        }

        private void DisposePhccDevice()
        {
            if (_phccDevice != null)
            {
                try
                {
                    _phccDevice.I2CDataReceived -= _phccDevice_I2CDataReceived;
                    _phccDevice.AnalogInputChanged -= _phccDevice_AnalogInputChanged;
                    _phccDevice.DigitalInputChanged -= _phccDevice_DigitalInputChanged;
                    if (_phccDevice.SerialPort != null && _phccDevice.SerialPort.IsOpen) _phccDevice.SerialPort.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                try
                {
                    _phccDevice.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void txt7Seg1thru8String_TextChanged(object sender, EventArgs e)
        {
            var contents = txt7Seg1thru8String.Text;
            var bits = new byte[8];
            if (contents != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (contents.Length >= (i + 1))
                    {
                        _sevenSegBits[i] = new Device().CharTo7Seg(contents[i]);
                    }
                    else
                    {
                        _sevenSegBits[i] = (byte) SevenSegmentBits.None;
                    }
                }
            }
            render7Seg(pbSevenSegDisplay1, _sevenSegBits[0]);
            render7Seg(pbSevenSegDisplay2, _sevenSegBits[1]);
            render7Seg(pbSevenSegDisplay3, _sevenSegBits[2]);
            render7Seg(pbSevenSegDisplay4, _sevenSegBits[3]);
            render7Seg(pbSevenSegDisplay5, _sevenSegBits[4]);
            render7Seg(pbSevenSegDisplay6, _sevenSegBits[5]);
            render7Seg(pbSevenSegDisplay7, _sevenSegBits[6]);
            render7Seg(pbSevenSegDisplay8, _sevenSegBits[7]);
            SendDoaSevenSegOutputs();
        }

        private void txt7Seg9thru16String_TextChanged(object sender, EventArgs e)
        {
            var contents = txt7Seg9thru16String.Text;
            var bits = new byte[8];
            if (contents != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (contents.Length >= (i + 1))
                    {
                        _sevenSegBits[i + 8] = new Device().CharTo7Seg(contents[i]);
                    }
                    else
                    {
                        _sevenSegBits[i + 8] = (byte) SevenSegmentBits.None;
                    }
                }
            }
            render7Seg(pbSevenSegDisplay9, _sevenSegBits[8]);
            render7Seg(pbSevenSegDisplay10, _sevenSegBits[9]);
            render7Seg(pbSevenSegDisplay11, _sevenSegBits[10]);
            render7Seg(pbSevenSegDisplay12, _sevenSegBits[11]);
            render7Seg(pbSevenSegDisplay13, _sevenSegBits[12]);
            render7Seg(pbSevenSegDisplay14, _sevenSegBits[13]);
            render7Seg(pbSevenSegDisplay15, _sevenSegBits[14]);
            render7Seg(pbSevenSegDisplay16, _sevenSegBits[15]);
            SendDoaSevenSegOutputs();
        }

        private void txt7Seg17thru24String_TextChanged(object sender, EventArgs e)
        {
            var contents = txt7Seg17thru24String.Text;
            var bits = new byte[8];
            if (contents != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (contents.Length >= (i + 1))
                    {
                        _sevenSegBits[i + 16] = new Device().CharTo7Seg(contents[i]);
                    }
                    else
                    {
                        _sevenSegBits[i + 16] = (byte) SevenSegmentBits.None;
                    }
                }
            }
            render7Seg(pbSevenSegDisplay17, _sevenSegBits[16]);
            render7Seg(pbSevenSegDisplay18, _sevenSegBits[17]);
            render7Seg(pbSevenSegDisplay19, _sevenSegBits[18]);
            render7Seg(pbSevenSegDisplay20, _sevenSegBits[19]);
            render7Seg(pbSevenSegDisplay21, _sevenSegBits[20]);
            render7Seg(pbSevenSegDisplay22, _sevenSegBits[21]);
            render7Seg(pbSevenSegDisplay23, _sevenSegBits[22]);
            render7Seg(pbSevenSegDisplay24, _sevenSegBits[23]);
            SendDoaSevenSegOutputs();
        }

        private void txt7Seg25thru32String_TextChanged(object sender, EventArgs e)
        {
            var contents = txt7Seg25thru32String.Text;
            var bits = new byte[8];
            if (contents != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (contents.Length >= (i + 1))
                    {
                        _sevenSegBits[i + 24] = new Device().CharTo7Seg(contents[i]);
                    }
                    else
                    {
                        _sevenSegBits[i + 24] = (byte) SevenSegmentBits.None;
                    }
                }
            }
            render7Seg(pbSevenSegDisplay25, _sevenSegBits[24]);
            render7Seg(pbSevenSegDisplay26, _sevenSegBits[25]);
            render7Seg(pbSevenSegDisplay27, _sevenSegBits[26]);
            render7Seg(pbSevenSegDisplay28, _sevenSegBits[27]);
            render7Seg(pbSevenSegDisplay29, _sevenSegBits[28]);
            render7Seg(pbSevenSegDisplay30, _sevenSegBits[29]);
            render7Seg(pbSevenSegDisplay31, _sevenSegBits[30]);
            render7Seg(pbSevenSegDisplay32, _sevenSegBits[31]);
            SendDoaSevenSegOutputs();
        }

        private void nudAirCore1_ValueChanged(object sender, EventArgs e)
        {
            tbAirCore1.Value = (int) nudAirCore1.Value;
            RenderNeedleGauge(pbAirCore1, tbAirCore1.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void nudAirCore2_ValueChanged(object sender, EventArgs e)
        {
            tbAirCore2.Value = (int) nudAirCore2.Value;
            RenderNeedleGauge(pbAirCore2, tbAirCore2.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void nudAirCore3_ValueChanged(object sender, EventArgs e)
        {
            tbAirCore3.Value = (int) nudAirCore3.Value;
            RenderNeedleGauge(pbAirCore3, tbAirCore3.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void nudAirCore4_ValueChanged(object sender, EventArgs e)
        {
            tbAirCore4.Value = (int) nudAirCore4.Value;
            RenderNeedleGauge(pbAirCore4, tbAirCore4.Value, 0, 1023, 360);
            SendAirCore();
        }

        private void ResetErrors()
        {
            epErrorProvider.Clear();
            if (String.IsNullOrEmpty(cbSerialPort.Text) || lblFirmwareVersion.Text == "Firmware Version:")
            {
                epErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                epErrorProvider.SetError(cbSerialPort,
                                         "No serial port is selected, or no PHCC device is detected on the selected serial port.");
            }
        }

        private void tcTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetErrors();
        }

        private void tbServo1Position_Scroll(object sender, EventArgs e)
        {
            nudServo1Position.Value = tbServo1Position.Value;
            RenderNeedleGauge(pbServo1, tbServo1Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void SendDoa8ServoCalibrationChannel1()
        {
            SendDoa8ServoCalibration(1, (ushort) nudServo1Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel2()
        {
            SendDoa8ServoCalibration(2, (ushort) nudServo2Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel3()
        {
            SendDoa8ServoCalibration(3, (ushort) nudServo3Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel4()
        {
            SendDoa8ServoCalibration(4, (ushort) nudServo4Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel5()
        {
            SendDoa8ServoCalibration(5, (ushort) nudServo5Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel6()
        {
            SendDoa8ServoCalibration(6, (ushort) nudServo6Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel7()
        {
            SendDoa8ServoCalibration(7, (ushort) nudServo7Calibration.Value);
        }

        private void SendDoa8ServoCalibrationChannel8()
        {
            SendDoa8ServoCalibration(8, (ushort) nudServo8Calibration.Value);
        }

        private void SendDoa8ServoGainChannel1()
        {
            SendDoa8ServoGain(1, (byte) nudServo1Gain.Value);
        }

        private void SendDoa8ServoGainChannel2()
        {
            SendDoa8ServoGain(2, (byte) nudServo2Gain.Value);
        }

        private void SendDoa8ServoGainChannel3()
        {
            SendDoa8ServoGain(3, (byte) nudServo3Gain.Value);
        }

        private void SendDoa8ServoGainChannel4()
        {
            SendDoa8ServoGain(4, (byte) nudServo4Gain.Value);
        }

        private void SendDoa8ServoGainChannel5()
        {
            SendDoa8ServoGain(5, (byte) nudServo5Gain.Value);
        }

        private void SendDoa8ServoGainChannel6()
        {
            SendDoa8ServoGain(6, (byte) nudServo6Gain.Value);
        }

        private void SendDoa8ServoGainChannel7()
        {
            SendDoa8ServoGain(7, (byte) nudServo7Gain.Value);
        }

        private void SendDoa8ServoGainChannel8()
        {
            SendDoa8ServoGain(8, (byte) nudServo8Gain.Value);
        }

        private void SendDoa8ServoGain(byte channel, byte value)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txt8ServoDevAddr, out devAddr);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        _phccDevice.DoaSend8ServoGain(devAddr, channel, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void SendDoa8ServoCalibration(byte channel, ushort value)
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txt8ServoDevAddr, out devAddr);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        unchecked
                        {
                            var x = (short) value;
                            _phccDevice.DoaSend8ServoCalibration(devAddr, channel, x);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void SendDoa8ServoValues()
        {
            byte devAddr = 0;
            var valid = ValidateHexTextControl(txt8ServoDevAddr, out devAddr);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        _phccDevice.DoaSend8ServoPosition(devAddr, 1, (byte) nudServo1Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 2, (byte) nudServo2Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 3, (byte) nudServo3Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 4, (byte) nudServo4Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 5, (byte) nudServo5Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 6, (byte) nudServo6Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 7, (byte) nudServo7Position.Value);
                        _phccDevice.DoaSend8ServoPosition(devAddr, 8, (byte) nudServo8Position.Value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void tbServo2Position_Scroll(object sender, EventArgs e)
        {
            nudServo2Position.Value = tbServo2Position.Value;
            RenderNeedleGauge(pbServo2, tbServo2Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo3Position_Scroll(object sender, EventArgs e)
        {
            nudServo3Position.Value = tbServo3Position.Value;
            RenderNeedleGauge(pbServo3, tbServo3Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo4Position_Scroll(object sender, EventArgs e)
        {
            nudServo4Position.Value = tbServo4Position.Value;
            RenderNeedleGauge(pbServo4, tbServo4Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo5Position_Scroll(object sender, EventArgs e)
        {
            nudServo5Position.Value = tbServo5Position.Value;
            RenderNeedleGauge(pbServo5, tbServo5Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo6Position_Scroll(object sender, EventArgs e)
        {
            nudServo6Position.Value = tbServo6Position.Value;
            RenderNeedleGauge(pbServo6, tbServo6Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo7Position_Scroll(object sender, EventArgs e)
        {
            nudServo7Position.Value = tbServo7Position.Value;
            RenderNeedleGauge(pbServo7, tbServo7Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void tbServo8Position_Scroll(object sender, EventArgs e)
        {
            nudServo8Position.Value = tbServo8Position.Value;
            RenderNeedleGauge(pbServo8, tbServo8Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo1Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo1Position.Value = (int) nudServo1Position.Value;
            RenderNeedleGauge(pbServo1, tbServo1Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo2Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo2Position.Value = (int) nudServo2Position.Value;
            RenderNeedleGauge(pbServo2, tbServo2Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo3Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo3Position.Value = (int) nudServo3Position.Value;
            RenderNeedleGauge(pbServo3, tbServo3Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo4Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo4Position.Value = (int) nudServo4Position.Value;
            RenderNeedleGauge(pbServo4, tbServo4Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo5Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo5Position.Value = (int) nudServo5Position.Value;
            RenderNeedleGauge(pbServo5, tbServo5Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo6Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo6Position.Value = (int) nudServo6Position.Value;
            RenderNeedleGauge(pbServo6, tbServo6Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo7Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo7Position.Value = (int) nudServo7Position.Value;
            RenderNeedleGauge(pbServo7, tbServo7Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo8Position_ValueChanged(object sender, EventArgs e)
        {
            tbServo8Position.Value = (int) nudServo8Position.Value;
            RenderNeedleGauge(pbServo8, tbServo8Position.Value, 0, 255, 180);
            SendDoa8ServoValues();
        }

        private void nudServo1Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel1();
        }

        private void nudServo1Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel1();
        }


        private void nudServo2Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel2();
        }

        private void nudServo2Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel2();
        }

        private void nudServo3Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel3();
        }

        private void nudServo3Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel3();
        }

        private void nudServo4Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel4();
        }

        private void nudServo4Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel4();
        }

        private void nudServo5Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel5();
        }

        private void nudServo5Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel5();
        }

        private void nudServo6Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel6();
        }

        private void nudServo6Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel6();
        }

        private void nudServo7Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel7();
        }

        private void nudServo7Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel7();
        }

        private void nudServo8Calibration_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoCalibrationChannel8();
        }

        private void nudServo8Gain_ValueChanged(object sender, EventArgs e)
        {
            SendDoa8ServoGainChannel8();
        }

        private void sendDoa()
        {
            byte devAddr = 0;
            byte subAddr = 0;
            byte data = 0;
            var valid = ValidateHexTextControl(txtDoaDevAddr, out devAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtDoaSubAddr, out subAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtDoaDataByte, out data);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        _phccDevice.DoaSendRaw(devAddr, subAddr, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void btnSendDoa_Click(object sender, EventArgs e)
        {
            sendDoa();
        }

        private void txtDoaDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtDoaDevAddr, out val);
        }

        private void txtDoaSubAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtDoaSubAddr, out val);
        }

        private void txtDoaDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtDoaDataByte, out val);
        }

        private void txtDobDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtDobDevAddr, out val);
        }

        private void txtDobDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtDobDataByte, out val);
        }

        private void cmdSendDob_Click(object sender, EventArgs e)
        {
            SendDob();
        }

        private void SendDob()
        {
            byte devAddr = 0;
            byte data = 0;
            var valid = ValidateHexTextControl(txtDobDevAddr, out devAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtDobDataByte, out data);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        _phccDevice.DobSendRaw(devAddr, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void btn7SegAllSegsOn_Click(object sender, EventArgs e)
        {
            txt7Seg1thru8String.Text = "";
            txt7Seg9thru16String.Text = "";
            txt7Seg17thru24String.Text = "";
            txt7Seg25thru32String.Text = "";
            for (var i = 0; i < _sevenSegBits.Length; i++)
            {
                _sevenSegBits[i] = 0xFF;
            }
            RenderAll7Segs();
            SendDoaSevenSegOutputs();
        }

        private void btn7SegAllSegsOff_Click(object sender, EventArgs e)
        {
            txt7Seg1thru8String.Text = "";
            txt7Seg9thru16String.Text = "";
            txt7Seg17thru24String.Text = "";
            txt7Seg25thru32String.Text = "";
            for (var i = 0; i < _sevenSegBits.Length; i++)
            {
                _sevenSegBits[i] = 0x00;
            }
            RenderAll7Segs();
            SendDoaSevenSegOutputs();
        }

        private void btnAirCoreZeroAll_Click(object sender, EventArgs e)
        {
            nudAirCore1.Value = 0;
            nudAirCore2.Value = 0;
            nudAirCore3.Value = 0;
            nudAirCore4.Value = 0;
        }

        private void btnZeroAllServos_Click(object sender, EventArgs e)
        {
            nudServo1Position.Value = 128;
            nudServo2Position.Value = 128;
            nudServo3Position.Value = 128;
            nudServo4Position.Value = 128;
            nudServo5Position.Value = 128;
            nudServo6Position.Value = 128;
            nudServo7Position.Value = 128;
            nudServo8Position.Value = 128;
        }

        private void txtI2CDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtI2CDevAddr, out val);
        }

        private void txtI2CSubAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtI2CSubAddr, out val);
        }

        private void txtI2CDataByte_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtI2CDataByte, out val);
        }

        private void SendI2C()
        {
            byte devAddr = 0;
            byte subAddr = 0;
            byte data = 0;
            var valid = ValidateHexTextControl(txtI2CDevAddr, out devAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtI2CSubAddr, out subAddr);
            if (!valid) return;
            valid = ValidateHexTextControl(txtI2CDataByte, out data);
            if (valid)
            {
                if (_phccDevice != null && !String.IsNullOrEmpty(_phccDevice.PortName))
                {
                    try
                    {
                        _phccDevice.DoaSendRaw(devAddr, subAddr, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void btnI2CSend_Click(object sender, EventArgs e)
        {
            SendI2C();
        }

        private void txtAnOut1DevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txtAnOut1DevAddr, out val);
            SendAnOut1Updates();
        }

        private void txt7SegDevAddr_Leave(object sender, EventArgs e)
        {
            byte val = 0;
            var valid = ValidateHexTextControl(txt7SegDevAddr, out val);
            SendDoaSevenSegOutputs();
        }

        private void DoSplash()
        {
            var sp = new Splash();
            try
            {
                sp.ShowDialog();
                Thread.Sleep(1000);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            if (sp != null)
            {
                sp.BackgroundImage = null;
                sp.Update();
                sp.Refresh();
                sp.Visible = false;
                sp.Hide();
                sp.Close();
            }
        }
    }
}