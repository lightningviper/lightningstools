using System;
using System.Threading;
using System.Windows.Forms;
using Common.Win32;
using SlimDX.DirectInput;
using log4net;
using MFDExtractor.EventSystem.Handlers;

namespace MFDExtractor.EventSystem
{
	internal interface IKeyboardWatcher
	{
		void Start();
        void Stop();
	}

	internal class KeyboardWatcher : IKeyboardWatcher
	{
        private Thread _thread;
        private bool _keepRunning;
		private readonly ILog _log;
		private readonly IKeyEventHandler _keyEventHandler;
		public KeyboardWatcher(IInputEvents inputEvents,ILog log = null)
		{
			_keyEventHandler = new KeyEventHandler(inputEvents);
			_log = log ??  LogManager.GetLogger(GetType());
		}

        public void Start()
        {
            if (_thread == null)
            {
                CreateThread();
            }
        }
        public void Stop()
        {
            _keepRunning = false;
        }
        private void CreateThread()
        {
            _thread = new Thread(() => KeyboardWatcherThreadWork());
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Priority = ThreadPriority.Highest;
            _thread.IsBackground = true;
            _thread.Name = "KeyboardWatcherThread";
            _thread.Start();
        }

        private void KeyboardWatcherThreadWork()
        { 
			_keepRunning = true;
			Keyboard keyboard = null;
            using (var directInput = new DirectInput())
            {
                try
                {
                    Control mainForm = null;// Application.OpenForms[0];

                    keyboard = new Keyboard(directInput);
                    keyboard.SetCooperativeLevel(mainForm, CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
                    keyboard.Properties.BufferSize = 255;
                    keyboard.Acquire();
                    var lastKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                    var currentKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                    while (_keepRunning)
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
                                var winFormsKey = (Keys)NativeMethods.MapVirtualKey((uint)thisKey, NativeMethods.MAPVK_VSC_TO_VK_EX);
                                if (isPressedNow && !wasPressedBefore)
                                {
                                    _keyEventHandler.ProcessKeyDownEvent(new KeyEventArgs(winFormsKey));
                                }
                                else if (wasPressedBefore && !isPressedNow)
                                {
                                    _keyEventHandler.ProcessKeyUpEvent(new KeyEventArgs(winFormsKey));
                                }
                                i++;
                            }
                            Array.Copy(currentKeyboardState, lastKeyboardState, currentKeyboardState.Length);
                        }
                        catch (Exception e)
                        {
                            _log.Error(e.Message, e);
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
                finally
                {
                    if (keyboard != null)
                    {
                        keyboard.Unacquire();
                    }
                    Common.Util.DisposeObject(keyboard);
                    Common.Util.DisposeObject(_thread);
                    _thread = null;
                }
            }
        }

    }
}
