using System;
using System.Windows.Forms;
using PhccTestTool.Properties;

namespace PhccTestTool
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            var bmp = Resources.testTool;
            //bmp.MakeTransparent(Color.White);
            Size = bmp.Size;
            BackgroundImage = bmp;
        }
    }
}