using System;
using System.Threading;
using System.Windows.Forms;
using Common.Win32;
using Microsoft.DirectX.DirectInput;
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
			Device device = null;
			try
            {
                var eventWaitHandle = new AutoResetEvent(false);
                Control mainForm = null;// Application.OpenForms[0];
                device = new Device(SystemGuid.Keyboard);
                device.SetCooperativeLevel(mainForm, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                device.SetEventNotification(eventWaitHandle);
                device.Properties.BufferSize = 255;
                device.Acquire();
                var lastKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                var currentKeyboardState = new bool[Enum.GetValues(typeof(Key)).Length];
                while (_keepRunning)
                {
                    try
                    {
                        eventWaitHandle.WaitOne(1000);
                    }
                    catch (TimeoutException)
                    {
                        continue;
                    }

                    try
                    {
                        var curState = device.GetCurrentKeyboardState();
                        var possibleKeys = Enum.GetValues(typeof(Key));

                        var i = 0;
                        foreach (Key thisKey in possibleKeys)
                        {
                            currentKeyboardState[i] = curState[thisKey];
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
                }
            }
			catch (Exception e)
			{
				_log.Error(e.Message, e);
			}
			finally
			{
				if (device != null)
				{
					device.Unacquire();
				}
				Common.Util.DisposeObject(device);
                Common.Util.DisposeObject(_thread);
                _thread = null;
            }
        }

    }
}
