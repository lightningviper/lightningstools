using System.Drawing;
using System.Windows.Forms;
using Common.Win32;

namespace F16CPD.UI.Util
{
    /// <summary>
    ///   Contains helper code to assist in intercepting window messages associated with form events
    ///   to determine if a resizable border should be presented on the form or not, depending
    ///   on the cursor's location relative to the form's (invisible) border.  Also provides
    ///   the behavior for actually resizing the form if these conditions are true and the user
    ///   does a drag-type operation within the form's border area
    /// </summary>
    internal class ResizeHelper : IMessageFilter
    {
        private Form _theWindow;

        public ResizeHelper(Form form)
        {
            _theWindow = form;
            Application.AddMessageFilter(this);
        }

        public Form TheWindow
        {
            get { return _theWindow; }
            set { _theWindow = value; }
        }

        //[System.Diagnostics.DebuggerHidden()]

        #region IMessageFilter Members

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (!_theWindow.IsDisposed && _theWindow.Visible && 0 == _theWindow.OwnedForms.Length)
            {
                var pt = Cursor.Position;
                var formBounds = _theWindow.DesktopBounds;
                if (formBounds.Contains(pt))
                {
                    // Create a rectangular area which is 7 pix smaller than windows's size
                    // This is the area beyond which we need to simulate non client area
                    var bounds = new Rectangle(formBounds.X, formBounds.Y, formBounds.Width, formBounds.Height);
                    bounds.Inflate(-7, -7);

                    var htValue = NativeMethods.HT.HTNOWHERE;

                    // If the cursor is outside this inner rectangle, we need to
                    // check for resize
                    if (!bounds.Contains(pt))
                    {
                        // Mouse is moving, if it is around the border edge, then we need
                        // to set the cursor
                        if (pt.X < bounds.Left) // Cursor left side
                        {
                            if (pt.Y < bounds.Top)
                            {
                                // Cursor top left
                                TheWindow.Cursor = Cursors.SizeNWSE;
                                htValue = NativeMethods.HT.HTTOPLEFT;
                            }
                            else if (pt.Y > bounds.Bottom)
                            {
                                // Cursor bottom left
                                TheWindow.Cursor = Cursors.SizeNESW;
                                htValue = NativeMethods.HT.HTBOTTOMLEFT;
                            }
                            else
                            {
                                // cursor left
                                TheWindow.Cursor = Cursors.SizeWE;
                                htValue = NativeMethods.HT.HTLEFT;
                            }
                        }
                        else if (pt.X > bounds.Right) // Cursor right side
                        {
                            if (pt.Y < bounds.Top)
                            {
                                // Cursor top right
                                TheWindow.Cursor = Cursors.SizeNESW;
                                htValue = NativeMethods.HT.HTTOPRIGHT;
                            }
                            else if (pt.Y > bounds.Bottom)
                            {
                                // Cursor bottom right
                                TheWindow.Cursor = Cursors.SizeNWSE;
                                htValue = NativeMethods.HT.HTBOTTOMRIGHT;
                            }
                            else
                            {
                                // cursor right
                                TheWindow.Cursor = Cursors.SizeWE;
                                htValue = NativeMethods.HT.HTRIGHT;
                            }
                        }
                        else // cursor is in between the form
                        {
                            if (pt.Y < bounds.Top)
                            {
                                // cursor is top
                                TheWindow.Cursor = Cursors.SizeNS;
                                htValue = NativeMethods.HT.HTTOP;
                            }
                            else if (pt.Y > bounds.Bottom)
                            {
                                // Cursor is bottom
                                TheWindow.Cursor = Cursors.SizeNS;
                                htValue = NativeMethods.HT.HTBOTTOM;
                            }
                        }

                        if (m.Msg == NativeMethods.WM.WM_MOUSEMOVE)
                        {
                            // the cursor is already set, we have nothing to do
                            //this._TheWindow.Cursor = cursor;
                            return true; // The message is handled
                        }
                        if (m.Msg == NativeMethods.WM.WM_LBUTTONDOWN)
                        {
                            // Start resizing
                            NativeMethods.ReleaseCapture();
                            NativeMethods.SendMessage(_theWindow.Handle, NativeMethods.WM.WM_NCLBUTTONDOWN,
                                                      htValue, 0);
                            return true; // The message is handled					
                        }
                        _theWindow.Cursor = Cursors.Default;
                        return false; // The message is NOT handled					
                    }
                    bounds.Inflate(-50, -50);
                    _theWindow.Cursor = bounds.Contains(pt) ? Cursors.SizeAll : Cursors.Default;
                    return false;
                }
                _theWindow.Cursor = Cursors.Default;
                return false;
            }
            return false;
        }

        #endregion
    }
}