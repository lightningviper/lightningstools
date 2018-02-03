using System;
using System.Threading;
using System.Windows.Forms;

namespace Common.Win32
{
    public static class KeyAndMouseFunctions
    {
        public const int WheelDelta = 120;


        /// <summary>
        ///     Sends a left mouse button click at the current cursor position.
        /// </summary>
        public static void LeftClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }


        /// <summary>
        ///     Sends a left mouse button double click at the current cursor position.
        /// </summary>
        public static void LeftDoubleClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }

        /// <summary>
        ///     Sends a left mouse button down event at the current cursor position.
        /// </summary>
        public static void LeftDown()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        }


        /// <summary>
        ///     Sends a left mouse button up event at the current cursor position.
        /// </summary>
        public static void LeftUp()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///     Sends a middle mouse button click at the current cursor position.
        /// </summary>
        public static void MiddleClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }


        /// <summary>
        ///     Sends a middle mouse button double click at the current cursor position.
        /// </summary>
        public static void MiddleDoubleClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }


        /// <summary>
        ///     Sends a middle mouse button down event at the current cursor position.
        /// </summary>
        public static void MiddleDown()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
        }


        /// <summary>
        ///     Sends a middle mouse button up event at the current cursor position.
        /// </summary>
        public static void MiddleUp()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///     Moves the mouse to the given absolute (x,y) coordinates.
        /// </summary>
        public static void MouseMoveAbsolute(int x, int y)
        {
            x = x * 65535 / Screen.PrimaryScreen.Bounds.Width;
            y = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.ABSOLUTE | NativeMethods.MOUSEEVENTF.MOVE, (uint) x,
                (uint) y, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///     Moves the mouse to the given relative (x,y) coordinates.
        /// </summary>
        public static void MouseMoveRelative(int x, int y)
        {
            var cur_x = Cursor.Position.X;
            var cur_y = Cursor.Position.Y;

            var new_x = cur_x + x;
            var new_y = cur_y + y;
            MouseMoveAbsolute(new_x, new_y);
        }

        /// <summary>
        ///     Moves the mouse wheel by given amount.
        /// </summary>
        public static void MouseWheelMove(int amount)
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.WHEEL, 0, 0, (uint) amount, UIntPtr.Zero);
        }


        /// <summary>
        ///     Sends a right mouse button click at the current cursor position.
        /// </summary>
        public static void RightClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }


        /// <summary>
        ///     Sends a right mouse button double click at the current cursor position.
        /// </summary>
        public static void RightDoubleClick()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }

        /// <summary>
        ///     Sends a right mouse button down event at the current cursor position.
        /// </summary>
        public static void RightDown()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///     Sends a right mouse button up event at the current cursor position.
        /// </summary>
        public static void RightUp()
        {
            NativeMethods.SendMouseInput(NativeMethods.MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///     Sends a keystroke to the system input buffer (applies to all applications, including DirectInput applications).
        /// </summary>
        /// <param name="keycode">A value from the <see cref="System.Windows.Forms.Keys" /> enumeration indicating the key to send.</param>
        /// <param name="extendedKey">If <see langword="true" />, the extended-key version of this key will be sent.</param>
        /// <param name="press">If <see langword="true" />, a Press event will be sent.</param>
        /// <param name="release">If <see langword="true" />, a Release event will be sent.</param>
        public static void SendKey(Keys keycode, bool extendedKey, bool press, bool release)
        {
            NativeMethods.SendKeyInput(keycode, extendedKey, press, release);
        }

        /// <summary>
        ///     Sends a keystroke to the system input buffer (applies to all applications, including DirectInput applications).
        /// </summary>
        /// <param name="scanCode">A OEM scan code indicating the key to send.</param>
        /// <param name="press">If <see langword="true" />, a Press event will be sent.</param>
        /// <param name="release">If <see langword="true" />, a Release event will be sent.</param>
        public static void SendKey(ushort scanCode, bool press, bool release)
        {
            NativeMethods.SendKeyInput(scanCode, press, release);
        }
    }
}