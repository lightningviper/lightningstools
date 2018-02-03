using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PPJoy
{
    /// <summary>
    ///   A <see cref = "Device" /> <b> object</b> (an <i>instance</i> of the <see cref = "Device" /> <b> class</b>) 
    ///   represents a single PPJoy joystick <see cref = "Device" />.
    /// </summary>
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [SuppressUnmanagedCodeSecurity] //don't do security stack walks every time we call unmanaged (native) code
    public sealed class Device
    {
        #region Private variables

        /// <seealso cref = "JoystickTypes" />
        private readonly JoystickTypes _joyType;

        /// <seealso cref = "LptNum" />
        private readonly int _lptNum;

        /// <seealso cref = "ProductId" />
        private readonly int _productId;

        /// <seealso cref = "VendorId" />
        private readonly int _vendorId;

        /// <seealso cref = "JoystickSubTypes" />
        private JoystickSubTypes _subType = JoystickSubTypes.NotApplicable;

        /// <seealso cref = "UnitNum" />
        private int _unitNum;

        #endregion

        #region Constructors

        /// <summary>
        ///   Private default constructor and internal non-default constructor(s) makes creating new device objects impossible without using <see cref = "DeviceManager">DeviceManager</see>'s factory methods
        /// </summary>
        private Device()
        {
        }

        /// <summary>
        ///   Creates a new instance of a <see cref = "Device" /> object that can manage a single PPJoy virtual device.
        /// </summary>
        /// <param name = "lptNum">LPT number that the device is attached to.
        ///   A value of Zero specifies a virtual device.</param>
        /// <param name = "type"><see cref = "JoystickTypes">JoystickType</see> of the device.</param>
        /// <param name = "subType"><see cref = "JoystickSubTypes">JoystickSubType</see> of this device.</param>
        /// <param name = "productId">Product ID associated with the device.</param>
        /// <param name = "vendorId">Vendor ID associated with the device.</param>
        /// <param name = "unitNum">Unit number of the device.</param>
        internal Device(int lptNum, JoystickTypes type, JoystickSubTypes subType, int productId, int vendorId,
                        int unitNum)
        {
            if (_lptNum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lptNum));
            }
            _lptNum = lptNum;
            _joyType = type;
            SetSubtype(subType);
            _productId = productId;
            _vendorId = vendorId;
            SetUnitNumber(unitNum);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Gets a <see cref = "MappingCollection" /> representing the controls defined 
        ///   on this <see cref = "Device" />.
        /// </summary>
        /// <returns>A <see cref = "MappingCollection" /> object
        ///   representing the controls defined directly on this <see cref = "Device" />.</returns>
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "MappingCollection" />
        public MappingCollection GetMappings()
        {
            return new DeviceManager().GetDeviceMappings(LptNum, DeviceType, UnitNum, JoystickMapScope.Device);
        }

        /// <summary>
        ///   Gets a <see cref = "MappingCollection" /> representing the controls 
        ///   defined on this <see cref = "Device" />, or representing the controls
        ///   defined in this <see cref = "Device" />'s interface.
        /// </summary>
        /// <param name = "scope">Scope from which to retrive the <see cref = "MappingCollection" />.</param>
        /// <returns>If the <paramref name = "scope" /> argument is set to 
        ///   <see cref = "JoystickMapScope.Interface" />, then this method returns 
        ///   a <see cref = "MappingCollection" /> object 
        ///   representing the controls defined in this <see cref = "Device" />'s 
        ///   interface. 
        ///   <para />
        ///   If the <paramref name = "scope" /> argument is set to 
        ///   <see cref = "JoystickMapScope.Device" />, then this method returns 
        ///   a <see cref = "MappingCollection" /> object 
        ///   representing the controls defined directly on this <see cref = "Device" /> itself. 
        /// </returns>
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "JoystickMapScope" />
        public MappingCollection GetMappings(JoystickMapScope scope)
        {
            return new DeviceManager().GetDeviceMappings(LptNum, DeviceType, UnitNum, scope);
        }

        /// <summary>
        ///   Removes the custom-defined <see cref = "Mapping" />s from this <see cref = "Device" />, without affecting the <see cref = "Mapping" />s defined in the <see cref = "Device" />'s interface.
        /// </summary>
        /// <seealso cref = "JoystickMapScope" />
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "Mapping" />
        public void RemoveMappings()
        {
            RemoveMappings(JoystickMapScope.Device);
        }

        /// <summary>
        ///   Removes the custom-defined <see cref = "Mapping" />s from this <see cref = "Device" /> 
        ///   OR from its interface.
        /// </summary>
        /// <param name = "scope">The <see cref = "JoystickMapScope" /> from which to remove all custom-defined <see cref = "Mapping" />s.</param>
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "JoystickMapScope" />
        public void RemoveMappings(JoystickMapScope scope)
        {
            new DeviceManager().RemoveDeviceMappings(LptNum, DeviceType, UnitNum, scope);
        }

        /// <summary>
        ///   Associates a set of <see cref = "Mapping" />s (a <see cref = "MappingCollection" />) with a specific PPJoy <see cref = "Device" />.
        /// </summary>
        /// <param name = "newMappings">A <see cref = "MappingCollection" /> object 
        ///   containing the new <see cref = "Mapping" />s to associate with 
        ///   the <see cref = "Device" />.</param>
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "JoystickMapScope" />
        public void SetMappings(MappingCollection newMappings)
        {
            SetMappings(JoystickMapScope.Device, newMappings);
        }

        /// <summary>
        ///   Associates a set of <see cref = "Mapping" />s (a <see cref = "MappingCollection" />) with a specific PPJoy <see cref = "Device" /> in a specific <see cref = "JoystickMapScope" />.
        /// </summary>
        /// <param name = "scope">The <see cref = "JoystickMapScope" /> 
        ///   in which to store the new custom <see cref = "Mapping" />s.</param>
        /// <param name = "newMappings">A <see cref = "MappingCollection" /> object 
        ///   containing the new <see cref = "Mapping" />s to associate with 
        ///   the specified <paramref name = "scope" />.</param>
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "JoystickMapScope" />
        public void SetMappings(JoystickMapScope scope, MappingCollection newMappings)
        {
            new DeviceManager().SetDeviceMappings(LptNum, DeviceType, UnitNum, scope, newMappings);
        }

        /// <summary>
        ///   Deletes this <see cref = "Device" /> from PPJoy.
        /// </summary>
        /// <param name = "removeDirectInput">If <see langword = "true" />, 
        ///   this <see cref = "Device" />'s registration will be removed
        ///   from DirectInput.  If <see langword = "false" />, the <see cref = "Device" />'s 
        ///   DirectInput registration will not be removed.</param>
        /// <param name = "removeDriver">If <see langword = "true" />, the <see cref = "Device" />'s
        ///   drivers will be unregistered from the system.  If <see langword = "false" />, 
        ///   the <see cref = "Device" />'s drivers will not be unregistered from the system.</param>
        public void Delete(bool removeDirectInput, bool removeDriver)
        {
            new DeviceManager().DeleteDevice(this, removeDirectInput, removeDriver);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the <see cref = "JoystickSubTypes">JoystickSubType</see> of this <see cref = "Device" />.
        /// </summary>
        public JoystickSubTypes SubType
        {
            get { return _subType; }
        }

        /// <summary>
        ///   Gets the <see cref = "JoystickTypes">JoystickType</see> of this <see cref = "Device" />.
        /// </summary>
        public JoystickTypes DeviceType
        {
            get { return _joyType; }
        }

        /// <summary>
        ///   Gets the unit number of this <see cref = "Device" />.
        /// </summary>
        public int UnitNum
        {
            get { return _unitNum; }
        }

        ///<summary>
        ///  Gets the LPT number of this <see cref = "Device" />.
        ///</summary>
        ///<remarks>
        ///  Virtual joystick <see cref = "Device" />s will
        ///  have <see cref = "LptNum" /> = 0.
        ///</remarks>
        public int LptNum
        {
            get { return _lptNum; }
        }

        /// <summary>
        ///   Gets the Product ID associated with this <see cref = "Device" />.
        /// </summary>
        public int ProductId
        {
            get { return _productId; }
        }

        /// <summary>
        ///   Gets the Vendor ID associated with this <see cref = "Device" />.
        /// </summary>
        public int VendorId
        {
            get { return _vendorId; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Sets the subtype of this device.
        /// </summary>
        /// <param name = "subType">a value from the JoystickSubTypes enumeration, indicating the desired subtype that this device should have</param>
        private void SetSubtype(JoystickSubTypes subType)
        {
            var deviceType = DeviceType;

            //validate the supplied subtype value, with respect to the known JoystickType value of this device
            if (DeviceManager.IsSubTypeValidGivenJoystickType(deviceType, subType))
            {
                _subType = subType;
            }
            else
            {
                //the supplied subtype value is not valid, given the current type (JoystickType) of this device
                throw new ArgumentException("Invalid subtype: " + Enum.GetName(typeof (JoystickSubTypes), subType) +
                                            ", given known joystick type:" +
                                            Enum.GetName(typeof (JoystickTypes), deviceType));
            }
        }

        /// <summary>
        ///   Sets the Unit Number of this device.  The first device 
        ///   on a given LPT port should have Unit number = 0; the next
        ///   device on the same port would be Unit number = 1, and so on.
        /// </summary>
        /// <param name = "value">an integer specifying the unit number to associate with this device.</param>
        private void SetUnitNumber(int value)
        {
            //validate the supplied unit number (must be >=0 and 
            //<= the maximum possible unit number for this device's device type
            if (value < 0 || value > new DeviceManager().MaxValidUnitNumber(DeviceType))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid unit number:" + value);
            }
            _unitNum = value;
        }

        #endregion
    }
}