using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace PPJoy
{
    /// <summary>
    ///   A <see cref = "VirtualJoystick" /> provides an easy-to-use interface for setting the PPJoy data source states for a single PPJoy Virtual Joystick <see cref = "Device" />.
    /// </summary>
    [
        SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), ComVisible(true),
        //allow this class to be visible in the COM-callable wrapper
        SuppressUnmanagedCodeSecurity //don't do security stack walks every time we call unmanaged (native) code
    ]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class VirtualJoystick : IDisposable
    {
        #region Instance Variables

        private readonly int[] _analogDataSourceVals = new int[MaxAnalogDataSources]; //analog axis states
        private readonly byte[] _digitalDataSourceVals = new byte[MaxDigitalDataSources]; //digital button states
        private SafeFileHandle _hFileHandle; //safe managed file handle wrapper for IOCTL interface
        private bool _isDisposed; //IDisposable state flag
        private int _virtualStickNumber; //PPJoy Virtual Joystick # (1-16)

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructs a new <see cref = "VirtualJoystick" /> instance.
        /// </summary>
        /// <param name = "virtualStickNumber">The one-based PPJoy virtual <see cfef = "Device" /> number 
        ///   that will be managed by this <see cref = "VirtualJoystick" /> instance.</param>
        public VirtualJoystick(int virtualStickNumber)
        {
            _virtualStickNumber = virtualStickNumber;
        }

        /// <summary>
        ///   Creates a new <see cref = "VirtualJoystick" /> instance.
        /// </summary>
        /// <remarks>
        ///   If you use this "default" constructor, you will need to set 
        ///   the <see cref = "VirtualJoystick.VirtualStickNumber" />
        ///   property manually.  The default constructor is supplied so that non-.NET 
        ///   (COM) clients can use this wrapper class, since COM requires
        ///   classes to have a default constructor.  If you don't set 
        ///   the <see cref = "VirtualJoystick.VirtualStickNumber" /> property manually,
        ///   the virtual stick number defaults to PPJoy Virtual Joystick #1.
        /// </remarks>
        public VirtualJoystick()
        {
            _virtualStickNumber = 1;
        }

        #endregion

        #region Constant Declarations

        /// <summary>
        ///   The value that should be set on an analog data source when that data source is assigned to a <see cref = "PovMapping" /> and when the <see cref = "PovMapping" /> should be <b>centered</b>.
        /// </summary>
        public const int PovCentered = -1;

        ///<summary>
        ///  The minimum value that can be applied to an analog data source (except <see cref = "VirtualJoystick.PovCentered" />).
        ///</summary>
        public const int MinAnalogDataSourceVal = 1;

        ///<summary>
        ///  The maximum value that can be applied to an analog data source.
        ///</summary>
        public const int MaxAnalogDataSourceVal = 32767;

        ///<summary>
        ///  The maximum number of analog data sources supported by PPJoy on a single <see cref = "Device" />.
        ///</summary>
        public const int MaxAnalogDataSources = 63;

        ///<summary>
        ///  The maximum number of digital data sources supported by PPJoy on a single <see cref = "Device" />.
        ///</summary>
        public const int MaxDigitalDataSources = 128;

        ///<summary>
        ///  The maximum number of virtual joystick <see cref = "Device" />s supported by PPJoy.
        ///</summary>
        public const int MaxVirtualDevices = 16;

        ///<summary>
        ///  The maximum number of buttons that can be created on a PPJoy virtual joystick <see cref = "Device" />.
        ///</summary>
        public const int MaxVisibleButtons = 32;

        ///<summary>
        ///  The maximum number of axes that can be created on a PPJoy virtual joystick <see cref = "Device" />.
        ///</summary>
        public const int MaxVisibleAxes = 8;

        ///<summary>
        ///  The maximum number of POVs that can be created on a PPJoy virtual joystick <see cref = "Device" />.
        ///</summary>
        public const int MaxVisiblePovs = 2;

        #endregion

        #region Property Getters/Setters

        /// <summary>
        ///   Gets/sets the PPJoy virtual <see cref = "Device" /> number that this <see cref = "VirtualJoystick" /> instance is managing.
        /// </summary>
        public int VirtualStickNumber
        {
            get { return _virtualStickNumber; }
            set
            {
                CloseFileHandle();
                _hFileHandle = null;
                _virtualStickNumber = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Sets an individual Analog data source value.
        /// </summary>
        /// <param name = "dataSourceNum">Zero-based index of the Analog data source to update.</param>
        /// <param name = "newValue">A value between 
        ///   <see cref = "VirtualJoystick.MinAnalogDataSourceVal" /> and <see cref = "VirtualJoystick.MaxAnalogDataSourceVal" />, 
        ///   that will be assigned to the Analog data source.</param>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown if the <paramref name = "dataSourceNum" /> param &lt; 1 or &gt; <see cref = "VirtualJoystick.MaxAnalogDataSources" />; also thrown if the <paramref name = "newValue" /> argument is &lt; <see cref = "VirtualJoystick.MinAnalogDataSourceVal" /> or &gt; <see cref = "VirtualJoystick.MaxAnalogDataSourceVal" /></exception>
        /// <remarks>
        ///   Axis data source values that are set by calling the <see cref = "VirtualJoystick.SetAnalogDataSourceValue" /> method 
        ///   do not get passed to PPJoy until the <see cref = "VirtualJoystick.SendUpdates" /> method is called.  This
        ///   allows multiple data source value updates to be passed to the PPJoy driver in
        ///   a single pass.
        /// </remarks>
        public void SetAnalogDataSourceValue(int dataSourceNum, int newValue)
        {
            if (dataSourceNum < 0 || dataSourceNum > MaxAnalogDataSources - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dataSourceNum));
            }
            if (newValue < MinAnalogDataSourceVal && newValue != -1)
            {
                newValue = MinAnalogDataSourceVal;
            }
            else if (newValue > MaxAnalogDataSourceVal)
            {
                newValue = MaxAnalogDataSourceVal;
            }

            _analogDataSourceVals[dataSourceNum] = newValue;
        }


        /// <summary>
        ///   Sets an individual Digital data source state.
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown if the <paramref name = "dataSourceNum" /> argument &lt; 1 or &gt; <see cref = "VirtualJoystick.MaxDigitalDataSources" /></exception>
        /// <param name = "dataSourceNum">Zero-based index of the digital data source to update.</param>
        /// <param name = "newValue">A new value to apply to the specified digital data source.</param>
        /// <remarks>
        ///   This value does not get passed to PPJoy until the 
        ///   <see cref = "VirtualJoystick.SendUpdates" /> method gets called.  This
        ///   allows multiple digital data source state updates to be passed 
        ///   to the PPJoy driver in a single pass.
        /// </remarks>
        public void SetDigitalDataSourceState(int dataSourceNum, bool newValue)
        {
            if (dataSourceNum < 0 || dataSourceNum > MaxDigitalDataSources - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dataSourceNum));
            }
            if (newValue)
            {
                _digitalDataSourceVals[dataSourceNum] = 1;
            }
            else
            {
                _digitalDataSourceVals[dataSourceNum] = 0;
            }
            ;
        }

        /// <summary>
        ///   Sends all pending updates to PPJoy.
        /// </summary>
        public void SendUpdates()
        {
            var JoyState = new JoystickState();

            /* Initialise the IOCTL data structure */
            JoyState.Signature = (uint) MessageVersions.JoystickStateV1;
            JoyState.NumAnalog = MaxAnalogDataSources; /* Number of analog values */
            JoyState.NumDigital = MaxDigitalDataSources; /* Number of digital values */
            JoyState.Analog = (int[]) _analogDataSourceVals.Clone();
            JoyState.Digital = (byte[]) _digitalDataSourceVals.Clone();

            SendUpdate(JoyState);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Sends all pending updates to PPJoy using the IOCTL interface.
        /// </summary>
        /// <param name = "JoyState">A populated <see cref = "JoystickState" /> structure containing data to pass to PPJoy</param>
        private void SendUpdate(JoystickState JoyState)
        {
            GetFileHandle();

            var bytesReturned = new uint();
            var outBuffer = new byte[0];

            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(JoyState));
            Marshal.StructureToPtr(JoyState, pinnedMessage, true);
            var outBufferHandle = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
            try
            {
                NativeMethods.DeviceIoControlSynchronous(_hFileHandle,
                                                         Headers.IoCtlSetPPJoyDeviceState,
                                                         pinnedMessage, (uint) Marshal.SizeOf(JoyState),
                                                         outBufferHandle.AddrOfPinnedObject(), 0, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                outBufferHandle.Free();
            }
        }

        /// <summary>
        ///   Gets a handle to the PPJoy IOCTL interface for the virtual 
        ///   joystick device being managed by this instance.  
        ///   Overwrites any previous handles stored at the instance level.
        /// </summary>
        /// <returns>A managed <see cref = "SafeFileHandle" /> wrapper around a Win32 API handle.</returns>
        private SafeFileHandle GetFileHandle()
        {
            if (_hFileHandle != null && _hFileHandle.IsInvalid)
            {
                CloseFileHandle();
            }
            if (_hFileHandle != null && _hFileHandle.IsClosed == false && _hFileHandle.IsInvalid == false)
            {
                return _hFileHandle;
            }

            var devName = @"\\.\PPJoyIOCTL" + _virtualStickNumber;
            //_hFileHandle = NativeMethods.CreateFile(devName,NativeMethods.EFileAccess.GenericWrite, NativeMethods.EFileShare.Write, IntPtr.Zero, NativeMethods.ECreationDisposition.OpenExisting, 0,new SafeFileHandle(IntPtr.Zero,true));
            _hFileHandle = NativeMethods.CreateFile(devName, NativeMethods.EFileAccess.GenericAll,
                                                    NativeMethods.EFileShare.Read | NativeMethods.EFileShare.Write,
                                                    IntPtr.Zero, NativeMethods.ECreationDisposition.OpenAlways,
                                                    NativeMethods.EFileAttributes.Overlapped,
                                                    //0,
                                                    new SafeFileHandle(IntPtr.Zero, true));
            var lastError = Marshal.GetLastWin32Error();

            if (_hFileHandle.IsInvalid && lastError != 0)
            {
                Exception e = new Win32Exception(lastError);
                throw new OperationFailedException(e.Message, e);
            }
            //ThreadPool.BindHandle(_hFileHandle);  
            return _hFileHandle;
        }

        /// <summary>
        ///   Closes the current IOCTL file handle.
        /// </summary>
        private void CloseFileHandle()
        {
            if (_hFileHandle != null)
            {
                try
                {
                    _hFileHandle.Close();
                    _hFileHandle.Dispose();
                }
                catch (ApplicationException)
                {
                }
            }
            _hFileHandle = null;
        }

        #endregion

        #region Destructors

        /// <summary>
        ///   Public implementation of the <see cref = "IDisposable.Dispose" /> method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Standard destructor
        /// </summary>
        ~VirtualJoystick()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Private implementation of the <see cref = "IDisposable.Dispose" /> method.
        /// </summary>
        /// <param name = "disposing"></param>
        internal void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Code to dispose the managed resources of the class
                }
            }
            // Code to dispose the un-managed resources of the class
            CloseFileHandle();
            _isDisposed = true;
        }

        #endregion
    }

//class
}

//namespace