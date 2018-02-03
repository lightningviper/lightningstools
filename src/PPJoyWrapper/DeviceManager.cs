using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace PPJoy
{
    /// <summary>
    ///   Provides methods for creating, retrieving, deleting, and managing details of PPJoy <see cref = "Device">Device</see> objects.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [SuppressUnmanagedCodeSecurity] //don't do security stack walks every time we call unmanaged (native) code
    public class DeviceManager
    {
        #region Constant and static variable declarations

        //apparently, the guy who wrote PPJoy decided it would be cute
        //to use the words DEAD BEEF as the hexadecimal bases for the
        //PPJoy vendor ID and for the product ID sequence base, respectively.
        private const int PPJOY_VENDOR_ID = 0xDEAD;
        private const int PPJOY_BASE_PRODUCT_ID = 0xBEEF;
        private const string PPJOY_IOCTL_BASE_DEVICE = @"\\?\root#media#0000#{64c3b4c4-cdcc-49aa-99a0-5b4ae4b5b1bb}";
        private const int MAP_BYTES_PER_AXIS = 4;
        private const int MAP_BYTES_PER_BUTTON = 3;
        private const int MAP_BYTES_PER_POV = 6;
        private static readonly string PPJOY_VENDOR_ID_STRING = Convert.ToString(PPJOY_VENDOR_ID, 16);

        #endregion

        /// <summary>
        ///   Gets a custom <see cref = "MappingCollection" /> that defines
        ///   the broadest possible set of controls that can 
        ///   be assigned to a PPJoy <see cref = "Device" />.  The controls are
        ///   pre-set to expose the maximum capabilities that a
        ///   virtual joystick <see cref = "Device" /> can express.
        /// </summary>
        /// <remarks>
        ///   The <see cref = "MappingCollection" /> that will be returned will 
        ///   define a control set that includes 8 axes, 32 buttons, and 2 POVs.
        ///   <para />Each <see cref = "ButtonMapping" /> will have 
        ///   its <see cref = "ButtonMapping.DataSource" /> property pre-set to
        ///   a <see cref = "ButtonDataSources">ButtonDataSource</see> that 
        ///   corresponds with the <see cref = "ButtonMapping" />'s 
        ///   <see cref = "Mapping.ControlNumber">ControlNumber</see> property 
        ///   value, such that the #1 button in the collection will source 
        ///   its data from <see cref = "ButtonDataSources.Digital0" />; 
        ///   the #2 button will source its data 
        ///   from <see cref = "ButtonDataSources.Digital1" />; and
        ///   so on.
        ///   <para />Each <see cref = "PovMapping" /> will be 
        ///   a <see cref = "ContinuousPovMapping" />, and will
        ///   have its <see cref = "ContinuousPovMapping.DataSource" /> property set 
        ///   to <see cref = "ContinuousPovDataSources.Analog8" /> for 
        ///   Pov #1, and <see cref = "ContinuousPovDataSources.Analog9" /> 
        ///   for Pov #2.
        ///   <para />
        ///   Each <see cref = "AxisMapping" /> will have its 
        ///   <see cref = "AxisMapping.MinDataSource" /> property set to
        ///   an <see cref = "AxisDataSources">AxisDataSource</see> that 
        ///   corresponds with the <see cref = "AxisMapping" />'s 
        ///   <see cref = "Mapping.ControlNumber">ControlNumber</see> property value, 
        ///   such that the #1 axis will source its data from 
        ///   <see cref = "AxisDataSources.Analog0" />; 
        ///   the #2 button will source its data 
        ///   from <see cref = "AxisDataSources.Analog1" />; and
        ///   so on.  Additionally, each <see cref = "AxisMapping" /> will have 
        ///   its <see cref = "AxisMapping.AxisType" /> 
        ///   property set to an <see cref = "AxisTypes">AxisType</see> in such a way 
        ///   as to ensure that the defined <see cref = "AxisMapping" />s 
        ///   will include a member of each of the 
        ///   following <see cref = "AxisTypes" />:
        ///   <list type = "bullet">
        ///     <item><see cref = "AxisTypes.X" /></item> 
        ///     <item><see cref = "AxisTypes.Y" /></item> 
        ///     <item><see cref = "AxisTypes.Z" /></item> 
        ///     <item><see cref = "AxisTypes.XRotation" /></item> 
        ///     <item><see cref = "AxisTypes.YRotation" /></item> 
        ///     <item><see cref = "AxisTypes.ZRotation" /></item> 
        ///     <item><see cref = "AxisTypes.Slider" /> - #1</item> 
        ///     <item><see cref = "AxisTypes.Slider" /> - #2</item> 
        ///   </list>
        /// </remarks>
        /// <returns>A fully-loaded <see cref = "MappingCollection" /> object 
        ///   that can be assigned to a <see cref = "Device" /> using 
        ///   the <see cref = "Device.SetMappings(PPJoy.MappingCollection)" /> 
        ///   method.</returns>
        /// <seealso cref = "Device" />
        /// <seealso cref = "MappingCollection" />
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "AxisTypes" />
        /// <seealso cref = "AxisDataSources" />
        /// <seealso cref = "AxisMapping" />
        /// <seealso cref = "ButtonMapping" />
        /// <seealso cref = "ButtonDataSources" />
        /// <seealso cref = "PovMapping" />
        /// <seealso cref = "ContinuousPovMapping" />
        /// <seealso cref = "ContinuousPovDataSources" />
        public MappingCollection IdealMappings
        {
            get
            {
                //create a new mapping collection to store the results
                var mappings = new MappingCollection();

                //create a set of 32 button mappings, with the data
                //sources set to Digital0 for the first button, Digital1 for
                //the next button, and so forth.
                for (var i = 0; i < 32; i++)
                {
                    var b = new ButtonMapping(i + 1);
                    b.DataSource = (ButtonDataSources) i;
                    mappings.Add(b); //add the mapping to the collecion
                }

                //create a set of 8 axis mappings, with the data
                //sources set to Analog0 through Analog7, and 
                //where the axis types are set to the standard
                //axis types for that axis number so that
                //the axes reported to Windows by a device with 
                //these mappings applied will have 8 axes defined
                //in the correct order for maximum utility.
                for (var i = 0; i < 8; i++)
                {
                    var a = new AxisMapping(i);
                    switch (i)
                    {
                        case 0:
                            a.AxisType = AxisTypes.X;
                            a.MinDataSource = AxisDataSources.Analog0;
                            break;
                        case 1:
                            a.AxisType = AxisTypes.Y;
                            a.MinDataSource = AxisDataSources.Analog1;
                            break;
                        case 2:
                            a.AxisType = AxisTypes.Z;
                            a.MinDataSource = AxisDataSources.Analog2;
                            break;
                        case 3:
                            a.AxisType = AxisTypes.XRotation;
                            a.MinDataSource = AxisDataSources.Analog3;
                            break;
                        case 4:
                            a.AxisType = AxisTypes.YRotation;
                            a.MinDataSource = AxisDataSources.Analog4;
                            break;
                        case 5:
                            a.AxisType = AxisTypes.ZRotation;
                            a.MinDataSource = AxisDataSources.Analog5;
                            break;
                        case 6:
                            a.AxisType = AxisTypes.Slider;
                            a.MinDataSource = AxisDataSources.Analog6;
                            break;
                        case 7:
                            a.AxisType = AxisTypes.Slider;
                            a.MinDataSource = AxisDataSources.Analog7;
                            break;
                        default:
                            break;
                    }
                    a.MaxDataSource = AxisDataSources.None; //these are pure analog-driven axes,
                    //so we don't need to define a Max data source value
                    mappings.Add(a); //add the mapping to the collection
                }

                //create two continuous POV mappings, deriving their 
                //values from Analog8 and Analog9.
                var p = new ContinuousPovMapping(0);
                p.DataSource = ContinuousPovDataSources.Analog8;
                mappings.Add(p); //add the mapping to the collection

                p = new ContinuousPovMapping(1);
                p.DataSource = ContinuousPovDataSources.Analog9;
                mappings.Add(p); //add the mapping to the collection

                return mappings; //return the collection of idealized mappings
            }
        }

        /// <summary>
        ///   Creates and registers a new joystick <see cref = "Device" /> with PPJoy.
        /// </summary>
        /// <param name = "lptNum">LPT number of the <see cref = "Device" /> to create/register.</param>
        /// <param name = "joystickType"><see cref = "JoystickTypes">JoystickType</see> of the <see cref = "Device" /> to create/register.</param>
        /// <param name = "subType"><see cref = "JoystickSubTypes">JoystickSubType</see> of the <see cref = "Device" /> to create/register.</param>
        /// <param name = "unitNum">Unit number of the <see cref = "Device" /> to create/register.</param>
        public void CreateDevice(int lptNum, JoystickTypes joystickType, JoystickSubTypes subType, int unitNum)
        {
            //determine if a device with the specified unit number already
            //exists on the specified parallel port
            var dev = new DeviceManager().GetDevice(lptNum, unitNum);

            //if a device with those values already exists, we can't 
            //create another device "on top" of it, so throw an exception
            if (dev != null)
            {
                throw new DeviceAlreadyExistsException(
                    "Could not create new device -- device already exists with LPTNum:" + lptNum + ", UnitNumber:" +
                    unitNum);
            }

            //if the provided subtype is not valid given the type of
            //device requested, then throw an exception
            if (!IsSubTypeValidGivenJoystickType(joystickType, subType))
            {
                throw new ArgumentException("Could not create new device with DeviceType:" + joystickType +
                                            " and SubType:" + subType + " because that is not a valid combination.");
            }

            //if the unit number is out of range, throw an exception
            var maxUnitNumber = MaxValidUnitNumber(joystickType);
            if (unitNum > maxUnitNumber)
            {
                throw new ArgumentOutOfRangeException("Could not create new device with DeviceType:" + joystickType +
                                                      " and UnitNumber:" + unitNum +
                                                      " because the maximum unit number for devices of that type is " +
                                                      maxUnitNumber);
            }

            //if the unit number is less than zero, it's invalid, so throw an exception
            if (unitNum < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid unit number:" + unitNum);
            }

            //if the unit number is not the first (zeroth) unit on the specified parallel port,
            //then the joystick type of the new device must match the joystick type of the root device
            //on the same parallel port (i.e. the zeroth device on that port)
            if (unitNum > 0)
            {
                dev = new DeviceManager().GetDevice(lptNum, 0);
                //check for the existance of the root device on this port
                if (dev != null)
                {
                    //a root device exists, so compare its type to the specified type
                    if (joystickType != dev.DeviceType)
                    {
                        //the types don't match, so throw an exception
                        throw new ArgumentException("Could not create new device with unit number:" + unitNum +
                                                    " and DeviceType:" + joystickType +
                                                    " because the first device on the same LPT number (" + lptNum +
                                                    ") is of a different DeviceType.");
                    }
                    //we've already checked for the existance of the root device,
                    //so now we need to verify that all other lower-numbered
                    //devices on the specified port exist (we can't have any gaps
                    //in unit numbers, so we can't, for example, 
                    //create unit number 8 when unit number 7 doesn't exist.
                    if (unitNum > 1)
                    {
                        for (var i = 2; i < unitNum; i++)
                        {
                            dev = new DeviceManager().GetDevice(lptNum, i);
                            if (dev == null)
                            {
                                //a required lower-numbered device does 
                                //not already exist,so this means the 
                                //supplied unit number parameter is invalid.
                                //Throw an exception.
                                throw new ArgumentException("Could not create new device with unit number:" + unitNum +
                                                            " and DeviceType:" + joystickType +
                                                            " because the next lower unit number on the same LPT number (" +
                                                            lptNum + ") does not already exist.");
                            }
                        } //end for
                    } //end if
                } //end if
                else
                {
                    //the root device was missing, so there's no way to create
                    //a higher-numbered device on the specified port.
                    //Throw an exception.
                    throw new ArgumentException("Could not create new device with unit number: " + unitNum +
                                                " when no device with unit number 0 exists.");
                }
            }

            //create a message payload to send to PPJoy's IOCTL interface
            //for creating a new device.
            var joyData = new DeviceInfo();
            joyData.VendorID = PPJOY_VENDOR_ID;
            joyData.JoyType = (byte) joystickType;
            joyData.JoySubType = GetPassableSubtype(subType);
            //get the subtype value to pass to PPJoy (different than the values represented by the JoystickSubTypes enumeration)
            joyData.LPTNumber = (byte) lptNum;
            var productId = GetNextFreeProductId();
            //find the next free product ID starting with the lowest-possible ProductID value
            if (productId == 0)
            {
                //if we couldn't find a free Product Id for whatever reason,
                //we can't create the new Device.  Throw an exception.
                throw new OperationFailedException(
                    "Could not create new device -- no available ProductIDs were found because the maximum number of virtual devices has already been created.");
            }
            joyData.ProductID = productId;
            joyData.UnitNumber = (byte) unitNum;
            joyData.PortAddress = 0; //we don't need to set the Port Address at this time.
            joyData.Size = (uint) Marshal.SizeOf(joyData);

            //create a new "Add Device" message  
            //to send to PPJoy's IOCTL interface
            var message = new AddDeviceMessage();
            message.Persistent = 1;
            message.JoyData = joyData; //add the payload to the message

            //now send the message to PPJoy via the IOCTL interface
            var hFileHandle = GetFileHandle(PPJOY_IOCTL_BASE_DEVICE);
            var bytesReturned = new uint();
            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(message));
            Marshal.StructureToPtr(message, pinnedMessage, true);

            try
            {
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlCreatePPJoyDevice,
                                                         pinnedMessage,
                                                         (uint) Marshal.SizeOf(message),
                                                         IntPtr.Zero, 0, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                CloseFileHandle(hFileHandle);
            }
        }

        /// <summary>
        ///   Gets the maximum valid unit number for a given joystick type.
        /// </summary>
        /// <param name = "joystickType">Joystick type to determine the maximum valid unit number for.</param>
        /// <returns>The maximum valid unit number for the specified joystick type.</returns>
        public int MaxValidUnitNumber(JoystickTypes joystickType)
        {
            var retVal = 0;
            switch (joystickType)
            {
                case JoystickTypes.Joystick_TheMaze:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_IanHerries:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_TurboGraFX:
                    retVal = 6;
                    break;
                case JoystickTypes.Joystick_Linux_v0802:
                    retVal = 1;
                    break;
                case JoystickTypes.Joystick_Linux_DB9c:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_TorMod:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_DirectPad_Pro:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_TurboGraFX_SwappedButtons:
                    retVal = 6;
                    break;
                case JoystickTypes.Joystick_LPT_JoyStick:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_CHAMPgames:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_STFormat:
                    retVal = 1;
                    break;
                case JoystickTypes.Joystick_SNESKey2600:
                    retVal = 0;
                    break;
                case JoystickTypes.Joystick_Amiga_4_Player:
                    retVal = 1;
                    break;
                case JoystickTypes.Joystick_PCAE:
                    retVal = 1;
                    break;
                case JoystickTypes.Genesis_Pad_Linux:
                    retVal = 0;
                    break;
                case JoystickTypes.Genesis_Pad_DirectPad_Pro:
                    retVal = 0;
                    break;
                case JoystickTypes.Genesis_Pad_NTPad_XP:
                    retVal = 1;
                    break;
                case JoystickTypes.SNES_or_NESPad_Linux:
                    retVal = 4;
                    break;
                case JoystickTypes.SNES_or_NESPad_DirectPadPro_Or_SNESKey:
                    retVal = 4;
                    break;
                case JoystickTypes.Genesis_Pad_ConsoleCable:
                    retVal = 1;
                    break;
                case JoystickTypes.Genesis_Pad_SNESKey:
                    retVal = 0;
                    break;
                case JoystickTypes.Playstation_Pad_PSXPBLib:
                    retVal = 7;
                    break;
                case JoystickTypes.Playstation_Pad_DirectPad_Pro:
                    retVal = 1;
                    break;
                case JoystickTypes.Playstation_Pad_Linux:
                    retVal = 4;
                    break;
                case JoystickTypes.Playstation_Pad_NTPad_XP:
                    retVal = 5;
                    break;
                case JoystickTypes.Playstation_Pad_Megatap:
                    retVal = 4;
                    break;
                case JoystickTypes.Virtual_Joystick:
                    retVal = 15;
                    break;
                case JoystickTypes.Joystick_Linux_gamecon:
                    retVal = 4;
                    break;
                case JoystickTypes.Joystick_LPTswitch:
                    retVal = 1;
                    break;
                case JoystickTypes.Radio_Control_TX:
                    retVal = 0;
                    break;
                case JoystickTypes.SNES_or_NESPad_PowerPad:
                    retVal = 1;
                    break;
                case JoystickTypes.Genesis_Pad_DirectPad_Pro_V6:
                    retVal = 0;
                    break;
                default:
                    retVal = 0;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Retrieves a <see cref = "Device" /> object that matches the 
        ///   specified query parameters.
        /// </summary>
        /// <param name = "lptNum">LPT Port number of the <see cref = "Device" /> to retrieve; set to 0 for Virtual Joystick <see cref = "Device" />s.</param>
        /// <param name = "unitNum">Unit number of the <see cref = "Device" /> to retrieve.</param>
        /// <returns>A <see cref = "Device" /> object matching the search criteria, or <see langword = "null" />, if no matching <see cref = "Device" /> is found.</returns>
        public Device GetDevice(int lptNum, int unitNum)
        {
            var devices = GetAllDevices();
            Device toReturn = null;
            foreach (var device in devices)
            {
                if (device.UnitNum == unitNum && device.LptNum == lptNum)
                {
                    toReturn = device;
                    break;
                }
            }
            return toReturn;
        }

        /// <summary>
        ///   Deletes all registered PPJoy joystick <see cref = "Device" />s.
        /// </summary>
        /// <param name = "removeDirectInput">If <see langword = "true" />, 
        ///   each <see cref = "Device" />'s registration will be removed
        ///   from DirectInput.  If <see langword = "false" />, no <see cref = "Device" />'s 
        ///   DirectInput registration will be removed.</param>
        /// <param name = "removeDriver">If <see langword = "true" />, each <see cref = "Device" />'s
        ///   drivers will be unregistered from the system.  If <see langword = "false" />, 
        ///   no <see cref = "Device" />'s drivers will be unregistered from the system.</param>
        public void DeleteAllDevices(bool removeDirectInput, bool removeDriver)
        {
            var devices = GetAllDevices();
            foreach (var dev in devices)
            {
                dev.Delete(removeDirectInput, removeDriver);
            }
        }

        /// <summary>
        ///   Enumerates all defined PPJoy <see cref = "Device" />s.
        /// </summary>
        /// <returns>An array of <see cref = "Device" /> objects, where each element 
        ///   in the array represents a single defined PPJoy <see cref = "Device" />.</returns>
        public Device[] GetAllDevices()
        {
            //send an "Enumerate Devices" message to PPJoy via the IOCTL interface
            var hFileHandle = GetFileHandle(PPJOY_IOCTL_BASE_DEVICE);
            var outBuffer = new byte[512];
            var bytesReturned = new uint();
            var outBufferHandle = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
            try
            {
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlEnumeratePPJoyDevices,
                                                         IntPtr.Zero, 0,
                                                         Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer, 0),
                                                         (uint) outBuffer.Length, out bytesReturned);
            }
            finally
            {
                outBufferHandle.Free();
                CloseFileHandle(hFileHandle);
            }
            //convert the data returned from the IOCTL call to an EnumerateDevicesMessage data object
            var joysticksData =
                (EnumerateDevicesMessage) Util.RawDataToObject(ref outBuffer, typeof (EnumerateDevicesMessage));

            //create a List object to hold the list of devices that were returned
            var devicesList = new List<Device>();

            //for each unique joystick discovered, create a new Device object
            //to represent that device, and add it to the list of devices
            //to return
            for (var i = 0; i < joysticksData.Count; i++)
            {
                var thisDeviceInfo = joysticksData.Joysticks[i];
                var device = GetDeviceFromDeviceInfo(thisDeviceInfo);
                devicesList.Add(device);
            }

            //return the list of devices to the caller
            return devicesList.ToArray();
        }

        /// <summary>
        ///   Deletes a <see cref = "Device" /> from PPJoy.
        /// </summary>
        /// <param name = "device">a <see cref = "Device">Device</see> to delete from PPJoy.</param>
        /// <param name = "removeDirectInput">If <see langword = "true" />, 
        ///   the <see cref = "Device" />'s registration will be removed
        ///   from DirectInput.  If <see langword = "false" />, the <see cref = "Device" />'s 
        ///   DirectInput registration will not be removed.</param>
        /// <param name = "removeDriver">If <see langword = "true" />, the <see cref = "Device" />'s
        ///   drivers will be unregistered from the system.  If <see langword = "false" />, 
        ///   the <see cref = "Device" />'s drivers will not be unregistered from the system.</param>
        public void DeleteDevice(Device device, bool removeDirectInput, bool removeDriver)
        {
            int currentRemoveDirectInputSetting;
            //stores the current value of the removeDirectInput registry setting used by PPJoy
            int currentRemoveDriverSetting; //holds the current value of the removeDriver registry setting used by PPJoy

            //read from the registry to determine how PPJoy is currently
            //configured as far as defaults for deleting devices
            //from DirectInput when they are removed from the PPJoy
            //control panel.  We need to read this value so that we can 
            //revert the registry later to its current state after
            //we're done with this entire operation
            try
            {
                try
                {
                    Registry.CurrentUser.CreateSubKey(@"Control Panel\PPJoy");
                }
                catch (Exception)
                {
                }
                currentRemoveDirectInputSetting =
                    (int)
                    Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy").GetValue("RemoveDirectInput",
                                                                                     removeDirectInput ? 1 : 0);
            }
            catch (IOException)
            {
                //if there's no current value readable, we'll use a default assumption
                currentRemoveDirectInputSetting = removeDirectInput ? 1 : 0;
            }

            //read from the registry to determine how PPJoy is currently
            //configured as far as defaults for removing device drivers
            //from Windows when they are removed from the PPJoy
            //control panel.  We need to read this value so that we can 
            //revert the registry later to its current state after
            //we're done with this entire operation           
            try
            {
                try
                {
                    Registry.CurrentUser.CreateSubKey(@"Control Panel\PPJoy");
                }
                catch 
                {
                }
                currentRemoveDriverSetting =
                    (int)
                    Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy").GetValue("RemoveDriver",
                                                                                     removeDriver ? 1 : 0);
            }
            catch (IOException)
            {
                //if there's no current value readable, we'll use a default assumption
                currentRemoveDriverSetting = removeDriver ? 1 : 0;
            }

            //set the registry values used by PPJoy to control its behavior
            //during remove operations
            try
            {
                Registry.CurrentUser.CreateSubKey(@"Control Panel\PPJoy");
            }
            catch 
            {
            }
            Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy", true).SetValue("RemoveDirectInput",
                                                                                   removeDirectInput);
            Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy", true).SetValue("RemoveDriver", removeDriver);

            //create an IOCTL "Remove Device" message to send to PPJoy
            var message = new RemoveDeviceMessage();
            message.JoyData = GetDeviceInfoFromDevice(device);
            message.JoyData.JoySubType = 0; //doesn't need to be filled
            message.JoyData.PortAddress = 0; //doesn't need to be filled
            message.JoyData.ProductID = 0; //doesn't need to be filled
            message.JoyData.VendorID = 0; //doesn't need to be filled

            //send the "Remove Device" message to PPJoy via cmb interface
            var hFileHandle = GetFileHandle(PPJOY_IOCTL_BASE_DEVICE);
            var bytesReturned = new uint();
            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(message));
            Marshal.StructureToPtr(message, pinnedMessage, true);

            try
            {
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlDeletePPJoyDevice,
                                                         pinnedMessage,
                                                         (uint) Marshal.SizeOf(message),
                                                         IntPtr.Zero, 0, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                CloseFileHandle(hFileHandle);
            }
            //set registry settings back to the way their values were before
            //this method was called
            try
            {
                Registry.CurrentUser.CreateSubKey(@"Control Panel\PPJoy");
            }
            catch 
            {
            }

            try
            {
                Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy", true).SetValue("RemoveDirectInput",
                                                                                       currentRemoveDirectInputSetting);
            }
            catch (IOException)
            {
            }

            try
            {
                Registry.CurrentUser.OpenSubKey(@"Control Panel\PPJoy", true).SetValue("RemoveDriver",
                                                                                       currentRemoveDriverSetting);
            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        ///   Checks whether a given integer containing a VendorID/ProductID
        ///   combination (typically obtained from DirectInput) refers to a virtual 
        ///   <see cref = "Device" /> or a physical <see cref = "Device" />.
        /// </summary>
        /// <param name = "vendorIdentityProductId">A 32-bit integer containing
        ///   a VendorID (in the high 16 bits) and a Product ID 
        ///   (in the low 16 bits), indicating a particular <see cref = "Device" /> on the system.
        /// </param>
        /// <returns><see langword = "true" />, if the <see cref = "Device" /> matching
        ///   the specified <paramref name = "vendorIdentityProductId" /> 
        ///   is a PPJoy virtual <see cref = "Device" />, or <see langword = "false" /> 
        ///   if it is a physical <see cref = "Device" />.</returns>
        public bool IsVirtualDevice(int vendorIdentityProductId)
        {
            //convert the Vendor ID/Product ID integer to a string in Base 16 (hexadecimal)
            //and ensure that string is at least 8 characters in length or pad 
            //it with zeroes on the left until it is 8 characters long
            var vipidHex = Convert.ToString(vendorIdentityProductId, 16).PadLeft(8, '0');

            //the first 4 hex chars represent the high 16 bits 
            //of the vendor ID/product ID integer
            var prod = vipidHex.Substring(0, 4);

            //the next 4 hex chars represent the low 16 bits
            //of the vendor ID/product ID integer
            var vendor = vipidHex.Substring(4, 4);

            //if the vendor ID is the vendor ID used by PPJoy, then we
            //still can't be sure this is a virtual device because
            //it could be a physical device hanging off of LPT1 and
            //using PPJoy as its driver, so we need to dig deeper
            //and load the corresponding PPJoy device object
            //to see if it's a virtual joystick or a LPT1 joystick (physical)
            if (vendor.Equals(PPJOY_VENDOR_ID_STRING, StringComparison.OrdinalIgnoreCase))
            {
                //get an array containing all registered PPJoy devices
                var devices = GetAllDevices();

                //for each element of the array, check to see if that
                //device's Product ID matches the specified Product ID,
                //and if so, then check if it's a virtual joystick or not
                for (var i = 0; i < devices.Length; i++)
                {
                    var thisDevice = devices[i];
                    var thisProdId = thisDevice.ProductId;
                    var thisProdIdString = Convert.ToString(thisProdId, 16);

                    //if the product ID string matches the supplied Product ID,
                    //check if it's a virtual device
                    if (thisProdIdString.Equals(prod, StringComparison.OrdinalIgnoreCase))
                    {
                        if (thisDevice.DeviceType == JoystickTypes.Virtual_Joystick)
                        {
                            //if it's virtual, then we have our answer,
                            //so we can return it now
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                //if we reach here, then we haven't found a matching Product ID
                //in the set of registered PPJoy devices, so 
                //we have to throw an exception
                throw new DeviceNotFoundException("Could not find matching PPJoy joystick with Vendor ID:" + vendor +
                                                  " and ProductID:" + prod);
            }
            else
            {
                return false; //the device can't be virtual 
                //because it's not a PPJoy device 
                //which we know because the Vendor ID
                //is not set to the PPJoy Vendor ID
            }
        }

        /// <summary>
        ///   Gets a <see cref = "Device" /> object representing the PPJoy device 
        ///   whose product ID matches the supplied Product ID.
        /// </summary>
        /// <param name = "productId">The Product ID of the <see cref = "Device" /> to return.</param>
        /// <returns>a <see cref = "Device" /> object representing the PPJoy device whose Product ID matches the value supplied in the <paramref name = "productId" /> argument.</returns>
        public Device GetDeviceByProductId(int productId)
        {
            Device toReturn = null;
            var devices = new DeviceManager().GetAllDevices();
            foreach (var device in devices)
            {
                if (device.ProductId == productId)
                {
                    toReturn = device;
                    break;
                }
            }
            return toReturn;
        }

        /// <summary>
        ///   Gets a new managed <see cref = "SafeFileHandle" /> object, 
        ///   wrapping a Win32 API file handle, using the standard access 
        ///   methods and modes required by PPJoy's IOCTL interface.
        /// </summary>
        /// <param name = "devName">The Win32 device name to read from/write to.</param>
        /// <returns>a managed <see cref = "SafeFileHandle" /> object 
        ///   representing a Win3 API file handle.</returns>
        private static SafeFileHandle GetFileHandle(String devName)
        {
            SafeFileHandle hFileHandle = null;
            hFileHandle = NativeMethods.CreateFile(
                devName,
                (
                    NativeMethods.EFileAccess.GenericRead
                    |
                    NativeMethods.EFileAccess.GenericWrite
                ),
                (
                    NativeMethods.EFileShare.Read
                    |
                    NativeMethods.EFileShare.Write
                ),
                IntPtr.Zero,
                NativeMethods.ECreationDisposition.OpenExisting,
                //NativeMethods.EFileAttributes.Overlapped,
                0,
                new SafeFileHandle(IntPtr.Zero, true)
                );
            //ThreadPool.BindHandle(hFileHandle);
            return hFileHandle;
        }

        /// <summary>
        ///   Closes a <see cref = "SafeFileHandle" /> object.
        /// </summary>
        /// <param name = "hFileHandle">a <see cref = "SafeFileHandle" /> object to close.</param>
        private static void CloseFileHandle(SafeFileHandle hFileHandle)
        {
            if (hFileHandle != null)
            {
                try
                {
                    hFileHandle.Close();
                    hFileHandle.Dispose();
                }
                catch (ApplicationException)
                {
                }
            }
        }

        /// <summary>
        ///   Checks whether a given <see cref = "JoystickSubTypes" /> value is valid 
        ///   in combination with a given <see cref = "JoystickTypes" /> value.
        /// </summary>
        /// <param name = "joystickType">A <see cref = "JoystickTypes">JoystickType</see> 
        ///   to validate in combination with the <paramref name = "subType" /> 
        ///   argument.</param>
        /// <param name = "subType">A <see cref = "JoystickSubTypes">JoystickSubType</see> 
        ///   to validate in combination with the <paramref name = "joystickType" /> argument.</param>
        /// <returns><see langword = "true" />, if the combination is valid, or <see langword = "false" /> 
        ///   if it is not a valid combination.</returns>
        /// <seealso cref = "JoystickTypes" />
        /// <seealso cref = "JoystickSubTypes" />
        internal static bool IsSubTypeValidGivenJoystickType(JoystickTypes joystickType, JoystickSubTypes subType)
        {
            var retVal = false;
            switch (subType)
            {
                case JoystickSubTypes.NotApplicable:
                    retVal = true;
                    break;
                case JoystickSubTypes.Genesis_Pad_A_B_C_Start:
                    switch (joystickType)
                    {
                        case JoystickTypes.Genesis_Pad_ConsoleCable:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_DirectPad_Pro:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_DirectPad_Pro_V6:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_Linux:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_NTPad_XP:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_SNESKey:
                            retVal = true;
                            break;
                        default:
                            retVal = false;
                            break;
                    }
                    break;
                case JoystickSubTypes.Genesis_Pad_A_B_C_X_Y_Z_Start_Mode:
                    switch (joystickType)
                    {
                        case JoystickTypes.Genesis_Pad_ConsoleCable:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_DirectPad_Pro:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_DirectPad_Pro_V6:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_Linux:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_NTPad_XP:
                            retVal = true;
                            break;
                        case JoystickTypes.Genesis_Pad_SNESKey:
                            retVal = true;
                            break;
                        default:
                            retVal = false;
                            break;
                    }
                    break;
                case JoystickSubTypes.NES:
                    switch (joystickType)
                    {
                        case JoystickTypes.SNES_or_NESPad_DirectPadPro_Or_SNESKey:
                            retVal = true;
                            break;
                        case JoystickTypes.SNES_or_NESPad_Linux:
                            retVal = true;
                            break;
                        case JoystickTypes.SNES_or_NESPad_PowerPad:
                            retVal = true;
                            break;
                        default:
                            retVal = false;
                            break;
                    }
                    break;
                case JoystickSubTypes.SNES_or_Virtual_Gameboy:
                    switch (joystickType)
                    {
                        case JoystickTypes.SNES_or_NESPad_DirectPadPro_Or_SNESKey:
                            retVal = true;
                            break;
                        case JoystickTypes.SNES_or_NESPad_Linux:
                            retVal = true;
                            break;
                        case JoystickTypes.SNES_or_NESPad_PowerPad:
                            retVal = true;
                            break;
                        default:
                            retVal = false;
                            break;
                    }
                    break;
                default:
                    retVal = false;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Gets a MappingCollection representing the controls mappings associated with a defined PPJoy device.
        /// </summary>
        /// <param name = "lptNum">LPT number of the joystick whose mappings will be returned.</param>
        /// <param name = "joystickType">Type of joystick whose mappings will be returned.</param>
        /// <param name = "unitNum">Unit number of the device of the given type whose mappings will be returned.</param>
        /// <param name = "scope">Scope to return mappings from.</param>
        /// <returns>a MappingCollection object, where each element in the collection represents a single control mapping.</returns>
        public MappingCollection GetDeviceMappings(int lptNum, JoystickTypes joystickType, int unitNum,
                                                   JoystickMapScope scope)
        {
            if (unitNum > MaxValidUnitNumber(joystickType))
            {
                throw new ArgumentOutOfRangeException(nameof(unitNum), "Invalid unit number:" + unitNum);
            }
            //get the raw mappings byte array from the specified PPJoy device
            var map = GetRawMappings(lptNum, joystickType, unitNum, scope);

            //convert the raw mappings to a MappingsCollection and return the collection
            return ReadMapData(map);
        }

        /// <summary>
        ///   Sets the mappings for a specific PPJoy device.
        /// </summary>
        /// <param name = "lptNum">The LPT number of the device whose mappings will be set to the newly-supplied mappings.</param>
        /// <param name = "joystickType">The type of device whose mappings will be set to the newly-supplied mappings.</param>
        /// <param name = "unitNum">The unit number of the device whose mappings will be set.</param>
        /// <param name = "header">A JoystickMapHeader object containing the new mappings to associate with this device or interface.</param>
        private static void SetDeviceMappings(int lptNum, JoystickTypes joystickType, int unitNum,
                                              JoystickMapHeader header)
        {
            //pass the JoystickMapHeader structure to PPJoy via its IOCTL interface
            var hFileHandle = GetFileHandle(@"\\.\PPJoyCtl" + lptNum + ":" + (unitNum + 1));
            var bytesReturned = new uint();
            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(header));
            Marshal.StructureToPtr(header, pinnedMessage, true);

            try
            {
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlSetPPJoyDeviceMappings,
                                                         pinnedMessage,
                                                         (uint) Marshal.SizeOf(header),
                                                         IntPtr.Zero, 0, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                CloseFileHandle(hFileHandle);
            }
        }

        /// <summary>
        ///   Sets the mappings for a specific PPJoy device.
        /// </summary>
        /// <param name = "lptNum">The LPT number of the device whose mappings will be set to the newly-supplied mappings.</param>
        /// <param name = "joystickType">The type of device whose mappings will be set to the newly-supplied mappings.</param>
        /// <param name = "unitNum">The unit number of the device whose mappings will be set.</param>
        /// <param name = "scope">The scope in which to set the new mappings -- either for the device instance itself, or for the device's interface defaults for all devices of the same type that do not override those defaults.</param>
        /// <param name = "newMappings">A MappingCollection object containing the new mappings to associate with this device or interface.</param>
        public void SetDeviceMappings(int lptNum, JoystickTypes joystickType, int unitNum, JoystickMapScope scope,
                                      MappingCollection newMappings)
        {
            if (unitNum > MaxValidUnitNumber(joystickType))
            {
                throw new ArgumentOutOfRangeException(nameof(unitNum));
            }
            //create and populate a JoystickMapPayload structure to pass to PPJoy's
            //IOCTL interface
            var toSet = new JoystickMapPayload();
            var rawPayload = BuildMapData(newMappings);
            toSet.Data = rawPayload;
            toSet.NumMaps = 1;
            toSet.NumAxes = (byte) newMappings.AxisMappings.Count;
            toSet.NumButtons = (byte) newMappings.ButtonMappings.Count;
            toSet.NumHats = (byte) newMappings.PovMappings.Count;

            //create and populate a JoystickMapHeader structure to pass to PPJoy's
            //IOCTL interface
            var header = new JoystickMapHeader();
            header.Version = (uint) MessageVersions.JoystickMapV1;
            header.JoyType = (byte) joystickType;
            header.MapScope = (byte) scope;
            header.MapData = toSet;
            header.MapSize =
                (uint)
                (4 +
                 ((MAP_BYTES_PER_AXIS*toSet.NumAxes) + (MAP_BYTES_PER_BUTTON*toSet.NumButtons) +
                  (MAP_BYTES_PER_POV*toSet.NumHats)));
            //remove old mappings
            try
            {
                RemoveDeviceMappings(lptNum, joystickType, unitNum, scope);
            }
            catch (OperationFailedException)
            {
            }

            //create new mappings
            SetDeviceMappings(lptNum, joystickType, unitNum, header);

            if (scope == JoystickMapScope.Device)
            {
                //delete the current instance of this virtual stick from DirectInput's configuration
                var d = GetDevice(lptNum, unitNum);
                d.Delete(true, false);
                //recreate the current instance of this virtual stick with DirectInput
                CreateDevice(lptNum, d.DeviceType, d.SubType, d.UnitNum);
            }
            else if (scope == JoystickMapScope.Interface)
            {
                var devices = GetAllDevices();
                for (var i = 0; i < devices.Length; i++)
                {
                    var thisDevice = devices[i];
                    try
                    {
                        var thisDeviceMappings = thisDevice.GetMappings(JoystickMapScope.Device);
                        //if this succeeds, then that means this device has its own mappings and is not
                        //dependent on the interface mappings.
                    }
                    catch (OperationFailedException)
                    {
                        //this device is missing its mappings so it is reliant on the interface mappings.  
                        //Since those have changed, we have to remove and recreate this device 
                        //so it will re-inherit its DirectInput config from the interface mappings.
                        thisDevice.Delete(true, false);
                        CreateDevice(thisDevice.LptNum, thisDevice.DeviceType, thisDevice.SubType, thisDevice.UnitNum);
                    }
                }
            }
        }

        /// <summary>
        ///   Removes the mappings from a specific PPJoy device or the default mappings from its interface.
        /// </summary>
        /// <param name = "lptNum">The LPT number of the device whose mappings or whose interface's mappings will be removed.</param>
        /// <param name = "joystickType">The type of the device whose mappings or whose interface's mappings will be removed.</param>
        /// <param name = "unitNum">The unit number of the device whose mappings or whose interface's mappings will be removed.</param>
        /// <param name = "scope">the scope of the mappings to remove (the device's, or the device's interface's default mappings.</param>
        public void RemoveDeviceMappings(int lptNum, JoystickTypes joystickType, int unitNum, JoystickMapScope scope)
        {
            if (unitNum > MaxValidUnitNumber(joystickType))
            {
                throw new ArgumentOutOfRangeException(nameof(unitNum));
            }

            //create a new JoystickMapHeader structure to pass to PPJoy's IOCTL
            //interface
            var header = new JoystickMapHeader();
            header.Version = (uint) MessageVersions.JoystickMapV1;
            header.MapScope = (byte) scope;
            header.JoyType = (byte) joystickType;

            //pass the JoystickMapHeader structure to PPJoy via its IOCTL interface
            var hFileHandle = GetFileHandle(@"\\.\PPJoyCtl" + lptNum + ":" + (unitNum + 1));
            var bytesReturned = new uint();
            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(header));
            Marshal.StructureToPtr(header, pinnedMessage, true);

            try
            {
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlDeletePPJoyDeviceMappings,
                                                         pinnedMessage,
                                                         (uint) Marshal.SizeOf(header),
                                                         IntPtr.Zero, 0, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                CloseFileHandle(hFileHandle);
            }
        }

        /// <summary>
        ///   Reads the current mappings from a specific PPJoy device.
        /// </summary>
        /// <param name = "lptNum">the LPT number of the joystick to read from.</param>
        /// <param name = "joystickType">a value from the JoystickTypes enumeration, 
        ///   describing the type of joystick to read from.</param>
        /// <param name = "unitNum">an integer specifying the unit number of the 
        ///   joystick to read from.</param>
        /// <param name = "scope">a value from the JoystickMapScope enumeration 
        ///   indicating the scope of the mappings to return.</param>
        /// <returns>a JoystickMapPayload structure, containing raw mapping
        ///   data, as returned from a direct call to PPJoy's IOCTL interface.</returns>
        private static JoystickMapPayload GetRawMappings(int lptNum, JoystickTypes joystickType, int unitNum,
                                                         JoystickMapScope scope)
        {
            //create and populate a JoystickMapHeader structure to pass to PPJoy's
            //IOCTL interface
            var header = new JoystickMapHeader();
            header.Version = (uint) MessageVersions.JoystickMapV1;
            header.MapScope = (byte) scope;
            header.JoyType = (byte) joystickType;

            //get a handle to the PPJoy IOCTL interface to the requested device
            var hFileHandle = GetFileHandle(@"\\.\PPJoyCtl" + lptNum + ":" + (unitNum + 1));
            var bytesReturned = new uint();
            var outBuffer = new byte[512];
            var outBufferHandle = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
            var pinnedMessage = Marshal.AllocHGlobal(Marshal.SizeOf(header));
            Marshal.StructureToPtr(header, pinnedMessage, true);
            try
            {
                //send the JoystickMapHeader structure to PPJoy, requesting
                //the desired mappings for the specified device
                NativeMethods.DeviceIoControlSynchronous(hFileHandle,
                                                         Headers.IoCtlGetPPJoyDeviceMappings, pinnedMessage,
                                                         (uint) Marshal.SizeOf(header),
                                                         Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer, 0),
                                                         (uint) outBuffer.Length, out bytesReturned);
            }
            finally
            {
                Marshal.FreeHGlobal(pinnedMessage);
                outBufferHandle.Free();
                CloseFileHandle(hFileHandle);
            }

            //convert the output buffer's contents to a JoystickMapHeader structure
            var mapHeader = (JoystickMapHeader) Util.RawDataToObject(ref outBuffer, typeof (JoystickMapHeader));

            //read the raw map data from the returned JoystickMapHeader structure
            var map = mapHeader.MapData;

            //return the raw map data as a JoystickMapPayload structure
            return map;
        }

        /// <summary>
        ///   Gets the next available Product ID on the PPJoy bus,
        ///   by examining the currently-used Product IDs, in order,
        ///   until a free one is found.
        /// </summary>
        /// <returns>the next available Product ID on the PPJoy bus, or zero, 
        ///   if an available product ID cannot be found.</returns>
        /// <remarks>
        ///   <b>Note:</b> The first free
        ///   ID can be in the middle between two used IDs.
        /// </remarks>
        private ushort GetNextFreeProductId()
        {
            var startingProductId = PPJOY_BASE_PRODUCT_ID + 1;
            ushort toReturn = 0;
            for (var i = startingProductId; i <= startingProductId + VirtualJoystick.MaxVirtualDevices; i++)
            {
                var device = GetDeviceByProductId(i);
                if (device == null)
                {
                    toReturn = (ushort) i;
                    break;
                }
            }
            return toReturn;
        }

        /// <summary>
        ///   Converts a <see cref = "MappingCollection" /> to an array of bytes 
        ///   that can be passed into PPJoy's IOCTL interface in a 
        ///   "Set Mapping" operation.
        /// </summary>
        /// <param name = "mappings">A populated <see cref = "MappingCollection" />
        ///   containing a complete set of <see cref = "Mapping" />s to convert 
        ///   to raw bytes.</param>
        /// <returns>an array of bytes that can be passed to PPJoy's IOCTL 
        ///   interface in a "Set Mapping" operation.  This byte array is the PPJoy 
        ///   "native" equivalent of the <see cref = "Mapping" />s supplied in 
        ///   the <paramref name = "mappings" /> argument.</returns>
        /// <seealso cref = "Mapping" />
        /// <seealso cref = "MappingCollection" />
        private static byte[] BuildMapData(MappingCollection mappings)
        {
            //obtain individual sub-collections of mappings for POVs, Buttons, and Axes
            var povMappings = mappings.PovMappings;
            var buttonMappings = mappings.ButtonMappings;
            var axisMappings = mappings.AxisMappings;

            //create a List to hold the bytes we'll be returning (we'll convert
            //this list to an array later)
            var toReturn = new List<byte>();

            //sort the mappings sub-collections by their control numbers, so that
            //low-numbered controls of that type appear first when we iterate
            //over the collections.  This is required by PPJoy.
            axisMappings.Sort();
            povMappings.Sort();
            buttonMappings.Sort();

            //write out the declarations bytes for all axes in the axes collection
            foreach (AxisMapping mapping in axisMappings)
            {
                if (mapping.AxisType != AxisTypes.Throttle && mapping.AxisType != AxisTypes.Wheel)
                {
                    //write a byte that declares "an ordinary axis or POV axis"
                    toReturn.Add((byte) DeviceCapabilitiesPrefixes.OrdinaryAxisOrPOV);
                }
                else
                {
                    //write a byte that declares a "wheel or throttle" axis
                    toReturn.Add((byte) DeviceCapabilitiesPrefixes.WheelOrThrottleAxis);
                }
                //write a byte that describes the exact type of axis 
                //being declared (x,y,z,xr,yr,zr, etc. or POV)
                toReturn.Add((byte) mapping.AxisType);
            }
            //write out the declarations bytes for all button mappings 
            //in the button mappings collection
            foreach (ButtonMapping mapping in buttonMappings)
            {
                //write a byte that declares a new button control 
                toReturn.Add((byte) DeviceCapabilitiesPrefixes.Button);
                //write a byte that indicates the button's control number (button 1, button 2, etc.)
                toReturn.Add((byte) mapping.ControlNumber);
            }
            //write out the declarations bytes for all POV mappings
            //in the POV mappings collection
            foreach (PovMapping mapping in povMappings)
            {
                //write a byte that declares a new axis control
                toReturn.Add((byte) DeviceCapabilitiesPrefixes.OrdinaryAxisOrPOV);
                //write a byte that describes the axis control as being a POV axis
                toReturn.Add((byte) AxisTypes.Pov);
            }
            //write out the specific data sources for each axis in the axis mappings
            //collection
            foreach (AxisMapping mapping in axisMappings)
            {
                //write out a byte specifying the MIN data source for this axis
                toReturn.Add((byte) mapping.MinDataSource);
                //write out a byte specifying the MAX data source for this axis
                toReturn.Add((byte) mapping.MaxDataSource);
            }
            //write out the specific data sources for each button in the button mappings
            //collection
            foreach (ButtonMapping mapping in buttonMappings)
            {
                //write a byte that specifies the data source of this button
                toReturn.Add((byte) mapping.DataSource);
            }
            //write out the specific data sources for each POV in the POV mappings
            //collection
            foreach (PovMapping mapping in povMappings)
            {
                //4 bytes are written per each POV control declared.  The first
                //byte describes the NORTH data source, the second byte
                //describes the SOUTH data source, the 3rd byte describes the WEST
                //data source, and the 4th byte describes the EAST data source.
                if (mapping is ContinuousPovMapping)
                {
                    //for Continuous POV mappings, we only need 
                    //to write out the NORTH data source specifically;
                    //all other data sources will be set to None.
                    var asContinuous = (ContinuousPovMapping) mapping;
                    toReturn.Add((byte) asContinuous.DataSource);
                    toReturn.Add((byte) ContinuousPovDataSources.None);
                    toReturn.Add((byte) ContinuousPovDataSources.None);
                    toReturn.Add((byte) ContinuousPovDataSources.None);
                }
                else if (mapping is DirectionalPovMapping)
                {
                    //for Directional POV mappings, write out the bytes 
                    //describing all four data sources (NORTH, SOUTH, WEST, EAST)
                    var asDirectional = (DirectionalPovMapping) mapping;
                    toReturn.Add((byte) asDirectional.NorthDataSource);
                    toReturn.Add((byte) asDirectional.SouthDataSource);
                    toReturn.Add((byte) asDirectional.WestDataSource);
                    toReturn.Add((byte) asDirectional.EastDataSource);
                }
            } //end foreach POV mapping

            //create an empty array whose size is set
            //to the maximum valid size for a mapping byte array
            var vals = new byte[Headers.MaxMappingPayloadLength];

            //copy the values from the List of bytes to the new byte array
            toReturn.CopyTo(vals);

            //return the byte array
            return vals;
        }

        /// <summary>
        ///   Parses the contents of a PPJoy IOCTL <see cref = "JoystickMapPayload" /> 
        ///   structure, converting them to a managed
        ///   <see cref = "MappingCollection" />, which can be easily worked with
        ///   programmatically.
        /// </summary>
        /// <param name = "mapData">A populated <see cref = "JoystickMapPayload" /> structure,
        ///   typically obtained by calling PPJoy's IOCTL interface using
        ///   the "Get Mappings" message.</param>
        /// <returns>A <see cref = "MappingCollection" /> object, representing the <see cref = "Mapping" />s
        ///   contained in the supplied <see cref = "JoystickMapPayload" /> structure.</returns>
        private static MappingCollection ReadMapData(JoystickMapPayload mapData)
        {
            //check the supplied JoystickMapPayload structure's data length 
            //to ensure that it is at least long enough to contain
            //a valid map as obtained from a PPJoy IOCTL call.
            if (mapData.Data.Length <
                (MAP_BYTES_PER_AXIS*mapData.NumAxes) + (MAP_BYTES_PER_BUTTON*mapData.NumButtons) +
                (MAP_BYTES_PER_POV*mapData.NumHats))
            {
                throw new ArgumentException("Invalid map data.", nameof(mapData));
            }

            //create a new MappingCollection collection to hold the results
            //of parsing
            var toReturn = new MappingCollection();

            var curPos = 0; //current position in the JoystickMapData data array
            var lastAxis = 0; //the last axis number discovered
            var lastButton = 0; //the last button number discovered
            var lastPov = 0; //the last POV number discovered

            //for each Axis declared by the JoystickMapData structure's axis count,
            //examine the corresponding raw bytes in the map data
            //and produce a AxisMapping object that represents
            //that specific mapping
            for (var i = 0; i < mapData.NumAxes; i++)
            {
                var axis = new AxisMapping(lastAxis);
                var capability = mapData.Data[curPos];

                //the  byte at the current position in the map data 
                //should indicate that an axis declaration follows. If that byte
                //does not match one of the possible values for an axis
                //declaration byte, then we know we're looking at bad data in the map, so 
                //throw an exception.  Otherwise, we don't really need that byte...
                //so we'll use it here for validation purposes only.
                if (capability != (byte) DeviceCapabilitiesPrefixes.OrdinaryAxisOrPOV
                    &&
                    capability != (byte) DeviceCapabilitiesPrefixes.WheelOrThrottleAxis)
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }

                //advance the position counter to the next byte
                curPos++;

                //now, the byte at the current position represents the type of
                //axis mapping, which, conveniently, corresponds to a value from the
                //AxisTypes enumeration, so we can just cast the byte to an AxisTypes value

                axis.AxisType = (AxisTypes) mapData.Data[curPos];
                //set the axis type in our AxisMapping object to this value

                //advance the position counter
                curPos++;

                //add the new AxisMapping object to the MappingCollection we'll return later
                toReturn.Add(axis);

                //increase the axis counter so the next axis will be numbered one higher than this one
                lastAxis++;
            }

            //now, we undertake a similar process for reading buttons from the map data.
            //For each button declared in the map data's button count, iterate over
            //the map data and create a new ButtonMapping object to represent each
            //unique mapping discovered in the underlying data
            for (var i = 0; i < mapData.NumButtons; i++)
            {
                //create a new ButtonMapping object to hold the results
                var button = new ButtonMapping(lastButton);

                //examine the byte at the current position in the map.  This byte should
                //be a value that specifies that a Button mapping descriptor byte will follow.
                //If the value at the current position does *not* declare 
                //that a button mapping descriptor byte follows, we know
                //we're looking at bad map data, so we'll throw an exception.
                var capability = mapData.Data[curPos];
                if (capability != (byte) DeviceCapabilitiesPrefixes.Button)
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }


                curPos++; //advance the position counter

                //now, the current byte should declare a button number
                var buttonNum = mapData.Data[curPos]; //get the current byte from the map data

                //buttons should be declared sequentially in a map, so if the  
                //value we just read from the map doesn't match what we expect the next
                //button number to be, we know we're reading from bad map data, 
                //so throw an exception.  NOTE: the first button number in the 
                //sequence can be either zero, or one.
                if (buttonNum != lastButton && (lastButton != 0 && buttonNum == 1))
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }

                //add the ButtonMapping object to the MappingCollection we'll return later
                toReturn.Add(button);

                //advance the position counter
                curPos++;

                //increase the button counter
                lastButton++;
            } //end for

            //now, undertake a similar process for evaluating all the POV Hat declarations
            //in the map data.  For each hat declared in the map data's hat count, 
            //iterate over the map data and create a new PovMapping object to represent each
            //unique hat mapping discovered in the underlying data
            for (var i = 0; i < mapData.NumHats; i++)
            {
                //the first byte of the pair should declare that a POV mapping follows.
                //if it doesn't, we know we're looking at bad map data, so throw an exception.
                //However, a POV mapping is just a special kind of Axis mapping, so the 
                //first byte actually declares an axis mapping
                var capability = mapData.Data[curPos];
                if (capability != (byte) DeviceCapabilitiesPrefixes.OrdinaryAxisOrPOV
                    &&
                    capability != (byte) DeviceCapabilitiesPrefixes.WheelOrThrottleAxis)
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }

                curPos++; //advance the position counter

                //the second byte of the pair should declare that this kind of axis declaration
                //is really a POV declaration.  NOTE: the first part of the map data
                //only contains control declarations; actual control MAPPINGS come later
                var axisType = mapData.Data[curPos];
                if (axisType != (byte) AxisTypes.Pov)
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }

                curPos++; //advance the position counter
                lastPov++; //advance the POV counter
            }


            //RECAP: at this point, we've read through the first section of the map data.
            //The first section contains control declarations (e.g., it defines the type
            //and number of controls that this virtual device will represent to Windows.
            //The next section of the map data describes the MAPPINGS associated
            //with each of these controls (e.g., where each virtual control gets its data from).

            //for each axis declared in the map, we now need to discover that axis' data
            //sources.  There can be multiple data sources for an axis, and if so,
            //then it's a Digital axis.
            lastAxis = 0;
            for (var i = 0; i < mapData.NumAxes; i++)
            {
                //the next two bytes in the data stream represent the MIN and MAX
                //data sources for the axis being evaluated.  Read those bytes now,
                //and store them in variables
                var minDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter
                var maxDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter

                //now retrieve the descriptive name of the corresponding AxisDataSources enum member
                //for both the MIN and MAX axis data sources that were declared
                var minDataSourceName = Enum.GetName(typeof (AxisDataSources), minDataSource);
                var maxDataSourceName = Enum.GetName(typeof (AxisDataSources), maxDataSource);

                //if the MIN data source is a digital data source, then the MAX data source must be declared
                //and it must also be Digital.  If those corrolary conditions are not true, throw
                //an exception.
                //On the other hand, if the MIN data source is an Analog (or Reversed) data source,
                //then the MAX data source should not be defined at all.  If it is, throw an exception.
                if (minDataSourceName.StartsWith("Digital") &&
                    !(maxDataSourceName.StartsWith("Digital") || maxDataSourceName.StartsWith("None")))
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }
                else if ((minDataSourceName.StartsWith("Analog") || minDataSourceName.StartsWith("Reversed")) &&
                         maxDataSource != (byte) AxisDataSources.None)
                {
                    throw new ArgumentException("Invalid map data.", nameof(mapData));
                }

                //Go back into the MappingCollection and retrieve the appropriate AxisMapping object
                //that we put in there earlier, and set its MinDataSource and MaxDataSource properties
                //according to the data we just read from the map.
                ((AxisMapping) toReturn.AxisMappings[lastAxis]).MinDataSource = (AxisDataSources) minDataSource;
                ((AxisMapping) toReturn.AxisMappings[lastAxis]).MaxDataSource = (AxisDataSources) maxDataSource;

                lastAxis++; //increase the axis counter
            }

            //for each button that was declared in the map's declarations,
            //discover the data source for that button.
            lastButton = 0; //reset the button counter
            for (var i = 0; i < mapData.NumButtons; i++)
            {
                //the next byte in the map should describe the current button's data source
                var dataSource = mapData.Data[curPos]; //read the current byte from the map

                curPos++; //advance the position counter

                //Go back into the MappingCollection and retrieve the appropriate ButtonMapping object
                //that we put in there earlier, and set its DataSource property
                //according to the data we just read from the map.
                ((ButtonMapping) toReturn.ButtonMappings[lastButton]).DataSource = (ButtonDataSources) dataSource;

                lastButton++; //advance the button counter
            }
            //for each POV that was declared in the map's declarations,
            //discover the data sources for that POV

            lastPov = 0; //reset the POV counter
            for (var i = 0; i < mapData.NumHats; i++)
            {
                //the next 4 bytes in the map describe the 4 data sources
                //for each POV.  If a POV gets its data from an analog
                //data source, thenonly the North data source will be set,
                //the remaining bytes will be set to a data source of None.
                var northDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter
                var southDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter
                var westDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter
                var eastDataSource = mapData.Data[curPos];
                curPos++; //advance the position counter

                var northDataSourceName = Enum.GetName(typeof (ContinuousPovDataSources), northDataSource);
                var isContinuousPOV = false;
                if (
                    //if the North data source is *not* None, and the other
                    //data sources *are* None, and the North data source
                    //is Analog or Reversed, then this is a Continuous POV.
                    northDataSource != (byte) ContinuousPovDataSources.None &&
                    (
                        southDataSource == (byte) ContinuousPovDataSources.None
                        &&
                        westDataSource == (byte) ContinuousPovDataSources.None
                        &&
                        eastDataSource == (byte) ContinuousPovDataSources.None
                    )
                    &&
                    (
                        northDataSourceName.StartsWith("Analog")
                        ||
                        northDataSourceName.StartsWith("Reversed")
                    )
                    )
                {
                    isContinuousPOV = true;
                }

                //create a new POV mapping object to represent this POV, 
                //and set its data sources as per the map data
                PovMapping pov;
                if (isContinuousPOV)
                {
                    pov = new ContinuousPovMapping(lastPov);
                    ((ContinuousPovMapping) pov).DataSource = (ContinuousPovDataSources) northDataSource;
                }
                else
                {
                    pov = new DirectionalPovMapping(lastPov);
                    ((DirectionalPovMapping) pov).NorthDataSource = (DirectionalPovDataSources) northDataSource;
                    ((DirectionalPovMapping) pov).SouthDataSource = (DirectionalPovDataSources) southDataSource;
                    ((DirectionalPovMapping) pov).WestDataSource = (DirectionalPovDataSources) westDataSource;
                    ((DirectionalPovMapping) pov).EastDataSource = (DirectionalPovDataSources) eastDataSource;
                }

                //add the POV Mapping object to the MappingCollection that we'll return
                toReturn.Add(pov);
                lastPov++; //increment the POV counter
            }

            //return the MappingCollection we built
            return toReturn;
        }

        /// <summary>
        ///   Returns a value from the <see cref = "JoystickSubTypes" />  enumeration 
        ///   that correpsonds to a given PPJoy IOCTL "subtype" byte,
        ///   as returned from certain calls to the PPJoy IOCTL interface.
        ///   In addition to the SubType byte, a value from the <see cref = "JoystickTypes" />
        ///   enumeration is required, in order to uniquely qualify the 
        ///   value to return from the <see cref = "JoystickSubTypes" /> enumeration.
        /// </summary>
        /// <param name = "joystickType">A value from the <see cref = "JoystickTypes" /> enumeration,
        ///   indicating the type of joystick to which the subtype byte applies.</param>
        /// <param name = "nativeSubType">A byte containing the <see cref = "Device" />'s "subtype",
        ///   as returned from certain calls to PPJoy's IOCTL interface.</param>
        /// <returns>A value from the <see cref = "JoystickSubTypes" /> enumeration 
        ///   that correpsonds to the specified PPJoy IOCTL "subtype" byte
        ///   and specified value from the <see cref = "JoystickTypes" /> enumeration.</returns>
        private static JoystickSubTypes GetEnumSubtype(JoystickTypes joystickType, byte nativeSubType)
        {
            var retVal = JoystickSubTypes.NotApplicable; //assume we can't figure it out

            var joystickTypeName = Enum.GetName(typeof (JoystickTypes), joystickType);
            if (joystickTypeName.StartsWith("Genesis_Pad"))
            {
                if (nativeSubType == 0)
                {
                    retVal = JoystickSubTypes.Genesis_Pad_A_B_C_Start;
                }
                else
                {
                    retVal = JoystickSubTypes.Genesis_Pad_A_B_C_X_Y_Z_Start_Mode;
                }
            }
            else if (joystickTypeName.StartsWith("SNES_or_NESPad"))
            {
                if (nativeSubType == 0)
                {
                    retVal = JoystickSubTypes.SNES_or_Virtual_Gameboy;
                }
                else
                {
                    retVal = JoystickSubTypes.NES;
                }
            }
            return retVal;
        }

        /// <summary>
        ///   Converts a value from the <see cref = "JoystickSubTypes" /> 
        ///   enumeration to a similar value that is required 
        ///   when making certain calls to PPJoy's IOCTL 
        ///   interface.
        /// </summary>
        /// <param name = "subType">a value from the <see cref = "JoystickSubTypes" /> 
        ///   enumeration, whose corresponding "passable" value 
        ///   (as expected by PPJoy's IOCTL interface)
        ///   will be returned.</param>
        /// <returns>a byte containing the value expected by PPJoy's 
        ///   IOCTL interface, which corresponds to the value passed in the <paramref name = "subType" /> 
        ///   parameter.</returns>
        /// <remarks>
        ///   <b>Note:</b> The value from the <see cref = "JoystickSubTypes" /> 
        ///   enumeration is not the same as the value that
        ///   is expected by PPJoy; hence, the need for this method.
        /// </remarks>
        internal static byte GetPassableSubtype(JoystickSubTypes subType)
        {
            byte retVal = 0;
            switch (subType)
            {
                case JoystickSubTypes.Genesis_Pad_A_B_C_Start:
                    retVal = 0;
                    break;
                case JoystickSubTypes.Genesis_Pad_A_B_C_X_Y_Z_Start_Mode:
                    retVal = 1;
                    break;
                case JoystickSubTypes.SNES_or_Virtual_Gameboy:
                    retVal = 0;
                    break;
                case JoystickSubTypes.NES:
                    retVal = 1;
                    break;
                case JoystickSubTypes.NotApplicable:
                    retVal = 0;
                    break;
                default:
                    retVal = 0;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Gets a PPJoy IOCTL <see cref = "DeviceInfo" /> structure from a <see cref = "Device" /> object.
        /// </summary>
        /// <param name = "device">a <see cref = "Device" /> object to return a PPJoy IOCTL <see cref = "DeviceInfo" /> structure from.</param>
        /// <returns>a PPJoy IOCTL <see cref = "DeviceInfo" /> structure, populated from the supplied <see cref = "Device" /> object.</returns>
        private static DeviceInfo GetDeviceInfoFromDevice(Device device)
        {
            var toReturn = new DeviceInfo();
            toReturn.JoyType = (byte) device.DeviceType;
            toReturn.JoySubType = GetPassableSubtype(device.SubType);
            toReturn.ProductID = (ushort) device.ProductId;
            toReturn.VendorID = (ushort) device.VendorId;
            toReturn.UnitNumber = (byte) device.UnitNum;
            toReturn.LPTNumber = (byte) device.LptNum;
            toReturn.Size = (uint) Marshal.SizeOf(toReturn);
            return toReturn;
        }

        /// <summary>
        ///   Creates a <see cref = "Device" /> object from a PPJoy IOCTL <see cref = "DeviceInfo" /> structure.
        /// </summary>
        /// <param name = "deviceInfo">a populated PPJoy IOCTL <see cref = "DeviceInfo" /> structure.</param>
        /// <returns>a <see cref = "Device" /> object, initialized with the values from the supplied <see cref = "DeviceInfo" /> structure.</returns>
        private static Device GetDeviceFromDeviceInfo(DeviceInfo deviceInfo)
        {
            var type = (JoystickTypes) deviceInfo.JoyType;
            var subType = GetEnumSubtype(type, deviceInfo.JoySubType);
            int productId = deviceInfo.ProductID;
            int unitNum = deviceInfo.UnitNumber;
            int vendorId = deviceInfo.VendorID;
            int lptNum = deviceInfo.LPTNumber;
            return new Device(lptNum, type, subType, productId, vendorId, unitNum);
        }
    }
}