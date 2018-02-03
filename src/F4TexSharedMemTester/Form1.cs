using System;
using System.Drawing;
using System.Windows.Forms;
using F4TexSharedMem;

namespace F4TexSharedMemTester
{
    public partial class Form1 : Form
    {
        private readonly Reader _reader = new Reader();
        private readonly Timer _timer = new Timer() { Interval = 20 };
        private bool _closing;

        public Form1()
        {
            InitializeComponent();
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _timer.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;
            _timer.Enabled = false;
            if (_reader != null)
            {
                try
                {
                    _reader.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            TopMost = true;

        }
        private void RefreshView()
        {
            if (!_reader.IsDataAvailable || _closing)
            {
                return;
            }
            pictureBox1.Image = _reader.FullImage;
            pictureBox1.Width = pictureBox1.Image.Width;
            pictureBox1.Height = pictureBox1.Image.Height;
            ClientSize = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);
            pictureBox1.Refresh();
        }
    }
}