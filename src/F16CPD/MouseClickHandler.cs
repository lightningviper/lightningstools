using F16CPD.Properties;
using System;
using System.Collections.Generic;
using Common.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F16CPD
{
    internal interface IMouseClickHandler
    {
        void Start(F16CpdMfdManager manager, Form applicationForm, Action whileMouseDown);
        void Stop();
    }
    class MouseClickHandler:IMouseClickHandler
    {
        private F16CpdMfdManager _manager;
        private Form _applicationForm;
        private bool _mouseDown;
        private DateTime? _mouseDownTime;
        private Action _whileMouseDown;

        public void Start(F16CpdMfdManager manager, Form applicationForm, Action whileMouseDown)
        {
            _manager = manager;
            _applicationForm = applicationForm;
            _applicationForm.MouseDown += MouseDown;
            _applicationForm.MouseUp += MouseUp;
            _whileMouseDown = whileMouseDown;
        }
        public void Stop()
        {
            _applicationForm.MouseDown -= MouseDown;
            _applicationForm.MouseUp -= MouseUp;
        }
        private void MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _mouseDownTime = DateTime.UtcNow;
            bool alreadyHandledOnce =false;
            while (_mouseDown && ((Form.MouseButtons & MouseButtons.Left) == MouseButtons.Left))
            {
                if (!alreadyHandledOnce || (alreadyHandledOnce && DateTime.UtcNow.Subtract(_mouseDownTime.Value).TotalMilliseconds > 500))
                {
                    HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, Cursor.Position.X, Cursor.Position.Y, 0));
                }
                alreadyHandledOnce = true;
                _whileMouseDown();
            }
            _mouseDown = false;
            _mouseDownTime = null;
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }
        private void HandleMouseClick(MouseEventArgs e)
        {
            //if there's no MFD manager set up yet, ignore this event
            if (_manager == null) return;

            //if the pressed mouse button was the left mouse button
            if (e.Button == MouseButtons.Left)
            {
                //if the mouse was single-clicked
                if (e.Clicks == 1)
                {
                    //get the current menu page
                    var currentPage = _manager.ActiveMenuPage;
                    //ask the current page which button was clicked
                    var x = e.Location.X - _applicationForm.Location.X;
                    var y = e.Location.Y - _applicationForm.Location.Y;

                    var xPrime = x;
                    var yPrime = y;

                    var rotation = Settings.Default.Rotation;
                    if (rotation == RotateFlipType.Rotate270FlipNone)
                    {
                        xPrime = _applicationForm.DisplayRectangle.Height - y;
                        yPrime = x;
                    }
                    else if (rotation == RotateFlipType.Rotate90FlipNone)
                    {
                        xPrime = y;
                        yPrime = _applicationForm.DisplayRectangle.Width - x;
                    }
                    else if (rotation == RotateFlipType.Rotate180FlipNone)
                    {
                        xPrime = _applicationForm.DisplayRectangle.Width - x;
                        yPrime = _applicationForm.DisplayRectangle.Height - y;
                    }

                    var clickedButton = currentPage.GetOptionSelectButtonByLocation(xPrime, yPrime);
                    if (clickedButton != null) //if a button was clicked
                    {
                        var whenPressed = _mouseDownTime.HasValue ? _mouseDownTime.Value : DateTime.UtcNow;
                        clickedButton.Press(whenPressed); //fire the button's press  event
                    }
                }
            }
        }

    }
}
