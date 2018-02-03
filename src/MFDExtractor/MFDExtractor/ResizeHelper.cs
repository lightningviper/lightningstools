using System;
using System.Windows.Forms;
using Common.Win32;

namespace MFDExtractor
{
    /// <summary>
    ///     Contains helper code to assist in intercepting window messages associated with form events
    ///     to determine if a resizable border should be presented on the form or not, depending
    ///     on the cursor's location relative to the form's (invisible) border.  Also provides
    ///     the behavior for actually resizing the form if these conditions are true and the user
    ///     does a drag-type operation within the form's border area
    /// </summary>
    internal class ResizeHelper : IMessageFilter
    {
        private readonly Cursor _initialCursor;
        private Form _TheWindow;

        public ResizeHelper(Form form)
        {
            _TheWindow = form;
            _initialCursor = _TheWindow.Cursor;
            Application.AddMessageFilter(this);
        }

        #region IMessageFilter Members

        bool IMessageFilter.PreFilterMessage(ref Message message)
        {
            try
            {
                return PreFilterMessage(message);
            }
            catch { }
            return false;
        }

        private bool PreFilterMessage(Message message)
        {
            if (_TheWindow.IsDisposed || !_TheWindow.Visible || 0 != _TheWindow.OwnedForms.Length) return false;
            var cursorLocation = Control.MousePosition;
            var pt = _TheWindow.PointToClient(cursorLocation);
            var formBounds = _TheWindow.ClientRectangle;
            var p = new NativeMethods.POINT(cursorLocation.X, cursorLocation.Y);
            var windowFromPoint = NativeMethods.WindowFromPoint(p);
            Form formToCompare = null;
            if (windowFromPoint != IntPtr.Zero)
            {
                formToCompare = Control.FromHandle(windowFromPoint) as Form;
            }
            if (formBounds.Contains(pt) && formToCompare != null && formToCompare == _TheWindow)
            {
                // Create a rectangular area which is 5 pix smaller than windows's size
                // This is the area beyond which we need to simulate non client area
                var bounds = _TheWindow.ClientRectangle;
                bounds.Inflate(-7, -7);

                // If the cursor is outside this inner rectangle, we need to
                // check for resize
                if (!bounds.Contains(pt))
                {
                    // Mouse is moving, if it is around the border edge, then we need
                    // to set the cursor
                    Cursor cursor;
                    int htValue;
                    if (pt.X < bounds.Left) // Cursor left side
                    {
                        if (pt.Y < bounds.Top)
                        {
                            // Cursor top left
                            cursor = Cursors.SizeNWSE;
                            htValue = NativeMethods.HT.HTTOPLEFT;
                        }
                        else if (pt.Y > bounds.Bottom)
                        {
                            // Cursor bottom left
                            cursor = Cursors.SizeNESW;
                            htValue = NativeMethods.HT.HTBOTTOMLEFT;
                        }
                        else
                        {
                            // cursor left
                            cursor = Cursors.SizeWE;
                            htValue = NativeMethods.HT.HTLEFT;
                        }
                    }
                    else if (pt.X > bounds.Right) // Cursor right side
                    {
                        if (pt.Y < bounds.Top)
                        {
                            // Cursor top right
                            cursor = Cursors.SizeNESW;
                            htValue = NativeMethods.HT.HTTOPRIGHT;
                        }
                        else if (pt.Y > bounds.Bottom)
                        {
                            // Cursor bottom right
                            cursor = Cursors.SizeNWSE;
                            htValue = NativeMethods.HT.HTBOTTOMRIGHT;
                        }
                        else
                        {
                            // cursor right
                            cursor = Cursors.SizeWE;
                            htValue = NativeMethods.HT.HTRIGHT;
                        }
                    }
                    else // cursor is in between the form
                    {
                        if (pt.Y < bounds.Top)
                        {
                            // cursor is top
                            cursor = Cursors.SizeNS;
                            htValue = NativeMethods.HT.HTTOP;
                        }
                        else if (pt.Y > bounds.Bottom)
                        {
                            // Cursor is bottom
                            cursor = Cursors.SizeNS;
                            htValue = NativeMethods.HT.HTBOTTOM;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    switch (message.Msg)
                    {
                        case NativeMethods.WM.WM_MOUSEMOVE:
                            // the cursor is already set, we have nothing to do
                            _TheWindow.Cursor = cursor;
                            return true; // The message is handled
                        case NativeMethods.WM.WM_LBUTTONDOWN:
                            // Start resizing
                            NativeMethods.ReleaseCapture();
                            NativeMethods.SendMessage(_TheWindow.Handle, NativeMethods.WM.WM_NCLBUTTONDOWN,
                                htValue, 0);
                            return true; // The message is handled					
                        default:
                            return false; // The message is NOT handled					
                    }
                }
                _TheWindow.Cursor = Cursors.SizeAll;
                return false;
            }
            _TheWindow.Cursor = _initialCursor;
            return false;
        }

        #endregion

        public Form TheWindow
        {
            get { return _TheWindow; }
            set { _TheWindow = value; }
        }
    }
}