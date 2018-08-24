using Common.Win32;
using log4net;
using SlimDX.DirectInput;
using System;
using System.Threading;
using System.Windows.Forms;

namespace F16CPD
{
    internal interface IKeyboardWatcher
    {
        void Start(IKeyDownEventHandler keyDownEventHandler);
        void Stop();
    }
    class KeyboardWatcher:IKeyboardWatcher
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(KeyboardWatcher));
        private IKeyDownEventHandler _keyDownEventHandler;
        private Thread _keyboardWatcherThread;

        public void Start(IKeyDownEventHandler keyDownEventHandler)
        {
            Stop();
            _keyDownEventHandler = keyDownEventHandler;
            _keyboardWatcherThread = new Thread(KeyboardWatcherThreadWork);
            _keyboardWatcherThread.SetApartmentState(ApartmentState.STA);
            _keyboardWatcherThread.Priority = ThreadPriority.Highest;
            _keyboardWatcherThread.IsBackground = true;
            _keyboardWatcherThread.Name = "KeyboardWatcherThread";
            _keyboardWatcherThread.Start();
        }
        public void Stop()
        {
            Common.Threading.Util.AbortThread(ref _keyboardWatcherThread);
        }
        private void KeyboardWatcherThreadWork()
        {
            if (Application.OpenForms.Count == 0) return;
            Keyboard keyboard = null;
            using (var directInput = new DirectInput())
            {
                try
                {
                    keyboard = new Keyboard(directInput);
                    keyboard.SetCooperativeLevel(Application.OpenForms.Count ==0 ? null : Application.OpenForms[0], CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
                    keyboard.Properties.BufferSize = 255;
                    keyboard.Acquire();
                    var lastKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                    var currentKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                    while (true)
                    {
                        try
                        {
                            var curState = keyboard.GetCurrentState();
                            var possibleKeys = Enum.GetValues(typeof(Key));

                            var i = 0;
                            foreach (Key thisKey in possibleKeys)
                            {
                                currentKeyboardState[i] = curState.IsPressed(thisKey);
                                i++;
                            }

                            i = 0;
                            foreach (Key thisKey in possibleKeys)
                            {
                                var isPressedNow = currentKeyboardState[i];
                                var wasPressedBefore = lastKeyboardState[i];
                                if (isPressedNow && !wasPressedBefore)
                                {
                                    var winFormsKey =
                                        (Keys)NativeMethods.MapVirtualKey((uint)thisKey, NativeMethods.MAPVK_VSC_TO_VK_EX);
                                    AddExtendedKeyInfo(winFormsKey);
                                    var eventArgs = new KeyEventArgs(winFormsKey);
                                    _keyDownEventHandler.HandleKeyDownEvent(this, eventArgs);
                                }
                                i++;
                            }
                            Array.Copy(currentKeyboardState, lastKeyboardState, currentKeyboardState.Length);
                        }
                        catch (Exception e)
                        {
                            _log.Debug(e.Message, e);
                        }
                        Thread.Sleep(50);

                    }
                }
                catch (ThreadInterruptedException)
                {
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
                finally
                {
                    if (keyboard != null)
                    {
                        try
                        {
                            keyboard.Unacquire();
                        }
                        catch { }

                        try
                        {
                            Common.Util.DisposeObject(keyboard);
                        }
                        catch { }

                        keyboard = null;
                    }
                }
            }
        }
        private Keys AddExtendedKeyInfo(Keys keys)
        {
            if ((NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0)
            {
                keys |= Keys.Shift;
                //SHIFT is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0)
            {
                keys |= Keys.Control;
                //CONTROL is pressed
            }
            if ((NativeMethods.GetKeyState(NativeMethods.VK_MENU) & 0x8000) != 0)
            {
                keys |= Keys.Alt;
                //ALT is pressed
            }
            return keys;
        }


    }
}
