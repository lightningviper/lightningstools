#region Using statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Common.InputSupport;

#endregion

namespace JoyMapper
{
    [Serializable]
    public sealed class OutputMap : ICloneable
    {
        /// <summary>
        ///     A list of PhysicalControlInfo objects representing the set of
        ///     "disabled" controls -- controls that appear in the mappings but
        ///     which should be ignored by Mediators
        /// </summary>
        private readonly List<PhysicalControlInfo> _disabledMappings = new List<PhysicalControlInfo>();

        /// <summary>
        ///     A Dictionary of mappings between PhysicalControlInfo objects
        ///     (the keys in the dictionary) and corresponding
        ///     VirtualControlInfo objects (the values in the dictionary)
        /// </summary>
        private readonly Dictionary<PhysicalControlInfo, VirtualControlInfo> _mappings =
            new Dictionary<PhysicalControlInfo, VirtualControlInfo>();

        /// <summary>
        ///     Gets an array of PhysicalControlInfo objects, where each object in the array
        ///     represents a single physical input control registered with (known to)
        ///     the output map *and* where mapping is *disabled for that physical control
        /// </summary>
        public PhysicalControlInfo[] DisabledPhysicalControls
        {
            get
            {
                var physicalControls = new List<PhysicalControlInfo>();
                foreach (var pci in _mappings.Keys)
                    if (!physicalControls.Contains(pci) && !IsMappingEnabled(pci))
                    {
                        physicalControls.Add(pci);
                    }
                return physicalControls.ToArray();
            }
        }

        /// Returns an array of PhysicalDeviceInfo objects, where each element in the array
        /// represents a unique physical device referenced in the full set of mappings, where
        /// all of the physical controls on the physical device are marked as 'disabled' in
        /// the mappings set
        public PhysicalDeviceInfo[] DisabledPhysicalDevices
        {
            get
            {
                var disabledPhysicalDevices = new List<PhysicalDeviceInfo>();
                var physicalDevices = PhysicalDevices;
                foreach (var device in physicalDevices)
                    if (!IsMappingEnabled(device))
                    {
                        disabledPhysicalDevices.Add(device);
                    }
                return disabledPhysicalDevices.ToArray();
            }
        }

        /// <summary>
        ///     Gets an array of PhysicalControlInfo objects, where each object in the array
        ///     represents a single physical input control registered with (known to)
        ///     the output map *and* where mapping is *enabled for that physical control
        /// </summary>
        public PhysicalControlInfo[] EnabledPhysicalControls
        {
            get
            {
                var physicalControls = new List<PhysicalControlInfo>();
                foreach (var pci in _mappings.Keys)
                    if (!physicalControls.Contains(pci) && IsMappingEnabled(pci))
                    {
                        physicalControls.Add(pci);
                    }
                return physicalControls.ToArray();
            }
        }

        /// Returns an array of PhysicalDeviceInfo objects, where each element in the array
        /// represents a unique physical device referenced in the set of enabled mappings
        public PhysicalDeviceInfo[] EnabledPhysicalDevices
        {
            get
            {
                var enabledPhysicalDevices = new List<PhysicalDeviceInfo>();
                var physicalDevices = PhysicalDevices;
                foreach (var device in physicalDevices)
                    if (IsMappingEnabled(device))
                    {
                        enabledPhysicalDevices.Add(device);
                    }
                return enabledPhysicalDevices.ToArray();
            }
        }

        /// <summary>
        ///     Gets an array of PhysicalControlInfo objects, where each object
        ///     in the array represents a single physical input control
        ///     registered with (known to) the output map, regardless of whether
        ///     that control has mapping enabled or disabled.
        /// </summary>
        public PhysicalControlInfo[] PhysicalControls
        {
            get
            {
                var physicalControls = new List<PhysicalControlInfo>();
                foreach (var pci in _mappings.Keys)
                    if (!physicalControls.Contains(pci))
                    {
                        physicalControls.Add(pci);
                    }
                return physicalControls.ToArray();
            }
        }

        /// <summary>
        ///     Returns an array of PhysicalDeviceInfo objects, where each element in the array
        ///     represents a unique physical device referenced in the full set of defined mappings
        ///     regardless of whether any mappings are enabled for any controls on that physical device
        /// </summary>
        public PhysicalDeviceInfo[] PhysicalDevices
        {
            get
            {
                var uniquePhysicalDevices = new List<PhysicalDeviceInfo>();
                foreach (var pci in _mappings.Keys)
                    if (!uniquePhysicalDevices.Contains(pci.Parent))
                    {
                        uniquePhysicalDevices.Add(pci.Parent);
                    }
                return uniquePhysicalDevices.ToArray();
            }
        }

        /// <summary>
        ///     Returns a deep copy of this Output Map
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Common.Serialization.Util.DeepClone(this);
        }

        /// <summary>
        ///     Checks whether the output map contains a mapping from a given physical input control
        /// </summary>
        /// <param name="physicalControl">
        ///     a PhysicalControlInfo object representing the physical input control to check for known
        ///     mappings
        /// </param>
        /// <returns>
        ///     true if the output map contains any mappings from the specified physical control, or false if it does not
        ///     contain any mappings from that input control
        /// </returns>
        public bool ContainsMappingFrom(PhysicalControlInfo physicalControl)
        {
            return _mappings.ContainsKey(physicalControl);
        }

        /// <summary>
        ///     Checks whether the output map contains a mapping to a specific virtual data source
        /// </summary>
        /// <param name="virtualControl">a VirtualControlInfo object representing a virtual data source to check</param>
        /// <returns>
        ///     true, if the output map contains any mappings to the specified virtual data source, or false if the output map
        ///     does not contain any mappings to the specified virtual data source
        /// </returns>
        public bool ContainsMappingTo(VirtualControlInfo virtualControl)
        {
            return _mappings.ContainsValue(virtualControl);
        }

        /// <summary>
        ///     Checks to see if this output map contains any actual mappings, or if all its mappings
        ///     are undefined.  If even a single valid output mapping exists,
        ///     this method will return true.
        /// </summary>
        /// <returns>
        ///     a boolean indicating if this output map contains any actual valid output
        ///     mappings or just input controls with no associated output controls.  A value
        ///     of true indicates that at least one output mapping is associated
        ///     with an input; a value of false indicates that no output
        ///     controls (virtual controls) currently appear in the map.
        /// </returns>
        public bool ContainsValidMappings()
        {
            var result = false;
            if (_mappings != null && _mappings.Count > 0)
            {
                foreach (var virtualControl in _mappings.Values)
                    if (virtualControl != null)
                    {
                        result = true;
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        ///     Marks a specific physical control in the map as 'disabled', preventing it
        ///     from being seen by Mediators
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object to mark as disabled
        ///     in the output map
        /// </param>
        public void DisableMapping(PhysicalControlInfo inputControl)
        {
            if (!_disabledMappings.Contains(inputControl))
            {
                _disabledMappings.Add(inputControl);
            }
        }

        /// <summary>
        ///     Marks all PhysicalControlInfo objects associated with the specified
        ///     PhysicalDeviceInfo object, as disabled.
        /// </summary>
        /// <param name="device">
        ///     PhysicalDeviceInfo object representing the physical
        ///     device whose PhysicalControlInfo objects (representing the device's controls)
        ///     will be marked as 'disabled' in the output map
        /// </param>
        public void DisableMapping(PhysicalDeviceInfo device)
        {
            foreach (var control in device.Controls)
                DisableMapping(control);
        }

        /// <summary>
        ///     Marks a specific PhysicalControlInfo in the Output Map as 'enabled'
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object to mark as 'enabled'
        ///     in the output map
        /// </param>
        public void EnableMapping(PhysicalControlInfo inputControl)
        {
            if (_disabledMappings.Contains(inputControl))
            {
                _disabledMappings.Remove(inputControl);
            }
        }

        /// <summary>
        ///     Marks all PhysicalControlInfo objects in the Output Map, which belong to the
        ///     specified PhysicalDeviceInfo object, as 'enabled' in the Output Map
        /// </summary>
        /// <param name="device">
        ///     PhysicalDeviceInfo object representing the physical
        ///     device whose PhysicalControlInfo objects (representing the device's controls)
        ///     will be marked as 'enabled' in the output map
        /// </param>
        public void EnableMapping(PhysicalDeviceInfo device)
        {
            foreach (var control in device.Controls)
                EnableMapping(control);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            // safe because of the GetType check
            var map = (OutputMap) obj;

            return Common.Serialization.Util.DeepEquals(this, map);
        }

        /// <summary>
        ///     Retrieves all output mappings associated with a specific physical input device
        /// </summary>
        /// <param name="device">
        ///     a PhysicalDeviceInfo object representing the physical device
        ///     whose mappings set should be retrieved
        /// </param>
        /// <returns>
        ///     a Dictionary&lt;PhysicalControlInfo, VirtualControlInfo&gt;
        ///         , where the keys
        ///         in the dictionary are physical controls on the specified physical device,
        ///         and the values in the dictionary are the corresponding VirtualControlInfo objects
        ///         representing the virtual (output) controls
        /// </returns>
        public Dictionary<PhysicalControlInfo, VirtualControlInfo> GetDeviceSpecificMappings(PhysicalDeviceInfo device)
        {
            var toReturn = new Dictionary<PhysicalControlInfo, VirtualControlInfo>();
            foreach (var control in _mappings.Keys)
            {
                var controlDevice = control.Parent;
                if (controlDevice.Key == device.Key)
                {
                    toReturn.Add(control, _mappings[control]);
                }
            }
            return toReturn;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        ///     Returns the VirtualControlInfo object currently mapped to the specified
        ///     PhysicalControlInfo object, if any.  If no mapping is defined for the
        ///     specified PhysicalControlInfo, then null is returned.
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object whose
        ///     corresponding VirtualControlInfo object (if any), will be returned
        ///     from the defined mappings set
        /// </param>
        /// <returns>
        ///     a VirtualControlInfo object representing the virtual control
        ///     which is currently mapped to receive data from the specified
        ///     physical control
        /// </returns>
        public VirtualControlInfo GetMapping(PhysicalControlInfo inputControl)
        {
            if (_mappings.ContainsKey(inputControl))
            {
                var vc = _mappings[inputControl];
                return vc;
            }
            return null;
        }

        /// <summary>
        ///     Checks to see if a given physical device is enabled in the output map for being
        ///     read from by Mediators.
        /// </summary>
        /// <param name="device">
        ///     a PhysicalDeviceInfo object representing the physical device
        ///     to check
        /// </param>
        /// <returns>
        ///     a boolean indicating true if any of the physical controls on the specified
        ///     device are enabled in the output map, or false if none of the physical controls
        ///     on the specified device are enabled in the output map
        /// </returns>
        public bool IsMappingEnabled(PhysicalDeviceInfo device)
        {
            foreach (var pci in device.Controls)
                if (IsMappingEnabled(pci))
                {
                    return true;
                }
            return false;
        }

        /// <summary>
        ///     Checks whether a specific physical control is enabled in the output map
        ///     for being read from by Mediators.
        /// </summary>
        /// <param name="control">
        ///     a PhysicalControlInfo object representing the
        ///     physical control to check
        /// </param>
        /// <returns>
        ///     a boolean indicating true if the specified physical control is
        ///     enabled in the output map, or false if the specified physical control is
        ///     not enabled (disabled) in the map
        /// </returns>
        public bool IsMappingEnabled(PhysicalControlInfo control)
        {
            return !_disabledMappings.Contains(control);
        }

        /// <summary>
        ///     Loads an OutputMap object from a file where it has previously been saved to
        /// </summary>
        /// <param name="mappingFile">
        ///     a FileInfo object pointing to a file containing
        ///     a serialized OutputMap object
        /// </param>
        /// <returns>an OutputMap object deserialized from the specified file</returns>
        public static OutputMap Load(FileInfo mappingFile)
        {
            OutputMap map;
            using (var fs = mappingFile.OpenRead())
            {
                var serializer = new BinaryFormatter();
                map = (OutputMap) serializer.Deserialize(fs);
                fs.Close();
            }
            return map;
        }

        /// <summary>
        ///     Removes all mappings from the output map which are associated with a given physical input device
        /// </summary>
        /// <param name="device">
        ///     a PhysicalDeviceInfo object representing a phsyical input device whose control's mappings will be
        ///     removed from the output map
        /// </param>
        public void RemoveMapping(PhysicalDeviceInfo device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }
            foreach (var control in device.Controls)
                RemoveMapping(control);
        }

        /// <summary>
        ///     Removes a mapping from the output map
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object representing the physical control whose mappings will be
        ///     removed from the output map
        /// </param>
        /// <returns>a bool indicating if any changes were actually made to the output map as a result of this call</returns>
        public bool RemoveMapping(PhysicalControlInfo inputControl)
        {
            var toReturn = false;
            if (_mappings.ContainsKey(inputControl))
            {
                toReturn = true;
                _mappings.Remove(inputControl);
            }
            if (_disabledMappings.Contains(inputControl))
            {
                toReturn = true;
                _disabledMappings.Remove(inputControl);
            }
            return toReturn;
        }

        /// <summary>
        ///     Saves an OutputMap to a file.
        /// </summary>
        /// <param name="outputFile">
        ///     a FileInfo object pointing to a file that
        ///     the output map should be saved to
        /// </param>
        /// <param name="map">an OutputMap object to serialize</param>
        public static void Save(FileInfo outputFile, OutputMap map)
        {
            using (var fs = outputFile.OpenWrite())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(fs, map);
                fs.Close();
            }
        }

        /// <summary>
        ///     Defines or removes a mapping in the output map.
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object
        ///     representing the source of data (input)
        /// </param>
        /// <param name="outputControl">
        ///     a VirtualControlInfo object representing the target
        ///     (output) for data coming from the specified physical control
        /// </param>
        /// <returns>
        ///     a boolean indicating if the output map has changed as a result
        ///     of this operation. Allows editors to call this method without
        ///     being aware of all of the current mappings, and yet still be able to
        ///     detect whether changes have occurred as a result of an attempted
        ///     modification of the map.
        /// </returns>
        public bool SetMapping(PhysicalControlInfo inputControl, VirtualControlInfo outputControl)
        {
            VirtualControlInfo currentMapping = null;

            if (_mappings.ContainsKey(inputControl))
            {
                currentMapping = _mappings[inputControl];
                _mappings.Remove(inputControl);
            }
            _mappings.Add(inputControl, outputControl);
            if (currentMapping != null)
            {
                if (outputControl == null)
                {
                    return true;
                }
                return !currentMapping.Equals(outputControl);
            }
            if (outputControl == null)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "OutputMap:" + Common.Serialization.Util.ToRawBytes(this);
        }
    }
}