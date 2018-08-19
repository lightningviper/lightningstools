using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using Common.Drawing;
using Common.InputSupport;
using Common.InputSupport.BetaInnovations;
using Common.InputSupport.DirectInput;
using Common.InputSupport.Phcc;
using Common.Strings;
using Common.UI;
using JoyMapper.Properties;
using log4net;
using SlimDX.DirectInput;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using PPJoy;
using Device = PPJoy.Device;

namespace JoyMapper
{
    internal sealed partial class frmMain : Form
    {
        private const int IMAGE_INDEX_LOCAL_DEVICES = 0;
        private const int IMAGE_INDEX_JOYSTICK = 1;
        private const int IMAGE_INDEX_AXIS = 2;
        private const int IMAGE_INDEX_BUTTON = 3;
        private const int IMAGE_INDEX_Pov = 4;
        private const int IMAGE_INDEX_BUTTON_PRESSED = 5;
        private const int IMAGE_INDEX_AXIS_MOVED = 6;
        private const int IMAGE_INDEX_POV_MOVED = 7;
        private const int IMAGE_INDEX_LOCAL_DEVICES_GREYED = 8;
        private const int IMAGE_INDEX_JOYSTICK_GREYED = 9;
        private const int IMAGE_INDEX_AXIS_GREYED = 10;
        private const int IMAGE_INDEX_BUTTON_GREYED = 11;
        private const int IMAGE_INDEX_POV_GREYED = 12;

        private static readonly ILog _log = LogManager.GetLogger(typeof(frmMain));
        private readonly OpenFileDialog _dlgOpen = new OpenFileDialog();
        private readonly SaveFileDialog _dlgSave = new SaveFileDialog();
        private readonly object _stateChanging = new object();
        private bool _autoHighlightingEnabled;
        private bool _exiting;
        private string _fileName;
        private bool _hasChanges;
        private TreeNode _lastNode;
        private bool _lastOpenCancelled;
        private bool _lastSaveCancelled;
        private TabPage _mappingsTab;
        private Mediator _mediator;
        private OutputMap _outputMap = new OutputMap();

        /// <summary>
        ///     Creates a new instance of this form.
        /// </summary>
        internal frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Event handler for PhysicalControlStateChanged events that are raised by the Mediator.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the PhysicalControlStateChanged event.</param>
        private void _mediator_PhysicalControlStateChanged(object sender, PhysicalControlStateChangedEventArgs e)
        {
            var highlight = false;
            lock (_stateChanging)
            {
                //This event handler is called whenever the Mediator detects that any axis, button, or POV control
                //on any device being managed by the Mediator, has changed state.  The event arguments
                //provides the current and previous state of the control.  Each control that
                //changes state causes a different event to fire.  This may result in a large
                //number of events firing, but allows for fine-grained handling of each event.

                var controlNode = findNodeForControl(e.Control);
                //find the TreeView node corresponding to the physical control whose state change is being signalled by the event
                if (controlNode != null && IsNodeEnabled(controlNode)) //if we found one
                {
                    if (e.Control.ControlType == ControlType.Button)
                        //if the control raising the change event is a Button control
                    {
                        if (e.CurrentState == 1) //if the button is now in the pressed state
                        {
                            highlight = true;
                            //update the corresponding TreeView node's icon to the highlighted state, and set the ToolTip text to indicate the button is pressed
                            controlNode.ToolTipText = "Current state=ON";
                            controlNode.ImageIndex = IMAGE_INDEX_BUTTON_PRESSED;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_BUTTON_PRESSED;
                        }
                        else //the button is now released
                        {
                            //update the corresponding TreeView node's icon to the normal (non-highlighted) state, and set the ToolTip text to indicate the button is in the released state
                            controlNode.ToolTipText = "Current state=OFF";
                            controlNode.ImageIndex = IMAGE_INDEX_BUTTON;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_BUTTON;
                        }
                    }
                    else if (e.Control.ControlType == ControlType.Axis)
                        //if the control raising the event is an Axis control
                    {
                        //based on the axis' raw value, calculate the percentage that this value represents of the maximum possible value
                        const int maxRange = VirtualJoystick.MaxAnalogDataSourceVal -
                                             VirtualJoystick.MinAnalogDataSourceVal + 1;
                        double pctOfRange = 0;
                        if (e.CurrentState > 1)
                        {
                            pctOfRange = e.CurrentState / (double) maxRange;
                        }
                        if ((e.CurrentState - (double) e.PreviousState) / maxRange < 0.01)
                        {
                            return;
                        }
                        highlight = true;
                        //update the axis' TreeView node's icon to the highlighted 
                        //state (it will be reset by the timer's Click event), 
                        //and set the ToolTip text to indicate the current 
                        //axis position (raw and percentage) 
                        controlNode.ToolTipText = "Current state=" + pctOfRange.ToString("P");
                        controlNode.ImageIndex = IMAGE_INDEX_AXIS_MOVED;
                        controlNode.SelectedImageIndex = IMAGE_INDEX_AXIS_MOVED;
                    }
                    else if (e.Control.ControlType == ControlType.Pov)
                        //if the control raising the event is a POV control
                    {
                        highlight = true;
                        if (e.CurrentState == -1) //if the POV is centered
                        {
                            //update the POV's TreeView node to set the icon to the normal (non-highlighted) state
                            //and update the ToolTip text to indicate this state
                            controlNode.ToolTipText = "Current state = CENTERED";
                            controlNode.ImageIndex = IMAGE_INDEX_Pov;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_Pov;
                        }
                        else //the POV is not centered (i.e., it's pressed in some direction)
                        {
                            //update the POV's TreeView node to set the icon to the Highlighted state
                            //and update the ToolTip text to indicate the exact state
                            var currentState = "UNKNOWN";
                            //start with the assumption that we can't determine the position of the POV

                            //calculate the scale length for raw axis values
                            var scaleLength = VirtualJoystick.MaxAnalogDataSourceVal -
                                              VirtualJoystick.MinAnalogDataSourceVal + 1;

                            //calculate the percentage that the current POV axis value represents of the total scale length
                            var pct = e.CurrentState / (double) scaleLength;

                            //translate that percentage into degrees, where zero degrees is due north
                            var degrees = pct * 360;

                            //now translate the degrees into a word describing the cardinal direction represented by the POV's position
                            if (degrees >= 0 && degrees < 22.5)
                            {
                                currentState = "UP";
                            }
                            else if (degrees >= 22.5 && degrees < 67.5)
                            {
                                currentState = "UP-RIGHT";
                            }
                            else if (degrees >= 67.5 && degrees < 112.5)
                            {
                                currentState = "RIGHT";
                            }
                            else if (degrees >= 112.5 && degrees < 157.5)
                            {
                                currentState = "DOWN-RIGHT";
                            }
                            else if (degrees >= 157.5 && degrees < 202.5)
                            {
                                currentState = "DOWN";
                            }
                            else if (degrees >= 202.5 && degrees < 247.5)
                            {
                                currentState = "DOWN-LEFT";
                            }
                            else if (degrees >= 247.5 && degrees < 292.5)
                            {
                                currentState = "LEFT";
                            }
                            else if (degrees >= 292.5 && degrees < 337.5)
                            {
                                currentState = "UP-LEFT";
                            }
                            else if (degrees >= 337.5)
                            {
                                currentState = "UP";
                            }

                            //update the POV's TreeView icon and tooltip text (these will be cleared by the clearing Timer's click event)
                            controlNode.ToolTipText = "Current state = " + currentState + ":" + e.CurrentState;
                            controlNode.ImageIndex = IMAGE_INDEX_POV_MOVED;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_POV_MOVED;
                        }
                    }
                    if (!_autoHighlightingEnabled)
                    {
                    }

                    else if (highlight)
                    {
                        //if a control's state has changed, make that control the currently-selected control
                        controlNode.EnsureVisible();
                        treeMain.SelectedNode = controlNode;
                    }
                }
            }
        }

        /// <summary>
        ///     Adds greyed-out versions of the main color images in the TreeView's ImageList control, to the ImageList control.
        /// </summary>
        private void AddGreyedOutImages()
        {
            var numImages = imglstImages.Images.Count;

            //for each image originally in the ImageList control
            for (var i = 0; i < numImages; i++)
            {
                Image img = imglstImages.Images[i]; //get a reference to the current Image from the control
                var greyedOut = Common.Imaging.Util.ConvertImageToGreyscale((Bitmap) img);
                //convert this image to greyscale
                imglstImages.Images.Add(greyedOut); //add the greyscale version back into the control
            }
        }

        private bool AreAnyChildNodesDisabled(TreeNode node)
        {
            if (node == null)
            {
                return false;
            }

            //obtain the node's .Tag object
            var tag = node.Tag;

            if (tag is PhysicalControlInfo) //if the tag represents a Physical Control
            {
                return false;
            }
            //this node is a container node, so we'll have to
            //recursively call the IsNodeEnabled() function for each of the children
            //of this node, and if any of them are disabled, then we can return true
            foreach (TreeNode child in node.Nodes)
                if (!IsNodeEnabled(child))
                {
                    return true;
                }
            return false; //all children were enabled
        }

        /// <summary>
        ///     Applies a set of maximum device capabilities to all detected PPJoy virtual joystick devices.
        ///     These capabilities represent the maximum number of axes, buttons, hats, etc., as
        ///     well as the configurations for where each of those controls read their data from
        ///     (i.e. which PPJoy data source drives the control's values).
        /// </summary>
        private void AssignMaximumCapabilitiesToAllPPJoyVirtualDevices()
        {
            StopMediatingAndDisposeMediator();
            var devices = new DeviceManager().GetAllDevices();
            foreach (var device in devices)
            {
                Application.DoEvents();
                if (device != null && device.DeviceType == JoystickTypes.Virtual_Joystick)
                {
                    try
                    {
                        device.SetMappings(JoystickMapScope.Device, new DeviceManager().IdealMappings);
                    }
                    catch (Exception e)
                    {
                        _log.Debug(e.Message, e);
                    }
                }
            }
            CreateMediatorAndUpdateMediatorMap();
        }

        //TODO: test HasChanges functionality
        //TODO: document all classes and methods
        //TODO: add help manual
        //TODO: fix menu capitalization on PPJoy menu
        //TODO: prompt user when removing virtual joysticks which ones to remove

        /// <summary>
        ///     Event handler for the Auto Highlighting button's Click event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnAutoHighlighting_Click(object sender, EventArgs e)
        {
            //toggle the state of the auto-highlighting flag
            _autoHighlightingEnabled = !_autoHighlightingEnabled;
            //toggle the "checked" state of the auto-highlighting button
            btnAutoHighlighting.Checked = _autoHighlightingEnabled;


            if (!_autoHighlightingEnabled) //if auto highlighting is disabled
            {
                //set the button's text appropriately
                btnAutoHighlighting.Text = @"Auto-highlighting (off)";
                //reset all highlighted controls and images to their non-highlighted state
                ClearAllActiveButtonStateImages();
                //call the axis/pov clearing timer to do the same thing for axis/pov controls
                tmrAxisAndPovMovedStateClearingTimer_Tick(this, null);
                //reset all control's tooltip texts to empty strings
                ClearAllTreeViewItemToolTipTexts();
            }
            else
            {
                btnAutoHighlighting.Text = @"Auto-highlighting (on)";
            }
            var set = Settings.Default;
            set.EnableAutoHighlighting = _autoHighlightingEnabled;
            set.Save();
        }

        /// <summary>
        ///     Event handler for the toolbar's "New" button's press event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        /// <summary>
        ///     Event handler for the toolbar's "Open" button's press event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        /// <summary>
        ///     Event handler for the toolbar's "Options" button's press event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnOptions_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }

        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            RefreshDeviceList();
        }

        /// <summary>
        ///     Event handler for the toolbar's "Save" button's press event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        ///     Event handler for the toolbar's "Start" button's press event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartMapping();
        }

        /// <summary>
        ///     Event handler for the toolbar's "Stop" button's press event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMapping();
        }

        /// <summary>
        ///     Event handler for the Virtual Control combo box's SelectionChangeCommmitted event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the SelectionChangeCommmitted event.</param>
        private void cboVirtualControl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //update the output map with the mappings that the user has selected for
            //the currently-selected physical control node 
            CheckAndStoreCurrentMapping();
        }

        /// <summary>
        ///     Event handler for the Virtual Device combo box's SelectionChangeCommitted event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the SelectionChangeCommitted event.</param>
        private void cboVirtualDevice_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //this event fires when the Virtual Device's combo bos selection changes
            //and the user clicks out of that input control.  This event handler
            //validates the selection of the Virtual Device combo box and
            //determines whether the Virtual Control combo box should be enabled.
            //If so, it populates the Virtual Control combo box with
            //the list of valid selections for that control.
            if (cboVirtualDevice.Text == @"<unmapped>")
            {
                cboVirtualControl.Enabled = false;
            }
            else
            {
                //determine which virtual joystick device was selected in the Virtual Device combobox
                var selectedVirtualDevice = cboVirtualDevice.Text;
                var virtualDeviceNumber = selectedVirtualDevice.Replace(@"PPJoy Virtual joystick ", "").Trim();
                var ppJoyDeviceNum = int.Parse(virtualDeviceNumber);
                var virtualDevice = new VirtualDeviceInfo(ppJoyDeviceNum);

                //populate the Virtual Control combobox based on the type of control node selected in the main TreeView
                PopulateSelectableOutputControls((PhysicalControlInfo) _lastNode.Tag, virtualDevice);

                //set the initial selection of the Virtual Control combobox to the "<unmapped>" entry.
                var unmappedIndex = cboVirtualControl.FindStringExact(@"<unmapped>");
                cboVirtualControl.SelectedIndex = unmappedIndex;

                //enable the Virtual Control combobox.
                cboVirtualControl.Enabled = true;
            }

            //this event would fire even if the user just selected a different control
            //node, or upon any action would cause the Virtual Device combobox to lose
            //focus.  So these cases would require checking the current values
            //of the comboboxes and description textbox and storing those in the map.
            CheckAndStoreCurrentMapping();
        }

        /// <summary>
        ///     Reads the last-edited node's values from the controls on the Mappings tab, and updates
        ///     the output map accordingly
        /// </summary>
        private void CheckAndStoreCurrentMapping()
        {
            if (_lastNode == null)
            {
                //if this is the first time this method's been called, then it's possible
                //that the _lastNode variable has not yet been set, so this would be a good time
                //to set it.  
                _lastNode = treeMain.SelectedNode;
            }
            if (_lastNode == null)
            {
                //if it's still NULL here, then no node is currently selected, so we'll set the
                //_lastNode variable to the root node of the control tree
                _lastNode = treeMain.Nodes[0];
            }

            //obtain the parent node of the last-edited node so we can determine what kind of node 
            //it is -- note that you can't edit mappings for the root node or container nodes, only
            //for physical control nodes, so there's no need to handle the other cases here
            var parent = _lastNode.Parent;
            if (parent != null)
            {
                //if the parent node is a container node for physical controls, then we can do something
                //about reading the control's new mappings values
                if (parent.Text == @"Axes" || parent.Text == @"Buttons" || parent.Text == @"Povs")
                {
                    //get a reference to the actual physical control that was edited (from the
                    //TreeNode's .Tag property
                    var inputControl = (PhysicalControlInfo) _lastNode.Tag;

                    //now call the method that will actually capture the new mapping for this item
                    StoreCurrentMapping(inputControl);
                }
            }
        }

        private void ClearAllActiveButtonStateImages()
        {
            foreach (TreeNode deviceNode in treeMain.Nodes[0].Nodes)
            foreach (TreeNode containerNode in deviceNode.Nodes)
            foreach (TreeNode controlNode in containerNode.Nodes)
                if (controlNode != null && IsNodeEnabled(controlNode)) //if we found one
                {
                    var control = (PhysicalControlInfo) controlNode.Tag;
                    if (control != null)
                    {
                        if (control.ControlType == ControlType.Button) //if the control is a Button control
                        {
                            //update the corresponding TreeView node's icon to the normal (non-highlighted) state, and set the ToolTip text to indicate the button is in the released state
                            controlNode.ToolTipText = @"Current state=OFF";
                            controlNode.ImageIndex = IMAGE_INDEX_BUTTON;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_BUTTON;
                        }
                        else if (control.ControlType == ControlType.Pov) //if the control is a POV control
                        {
                            //update the POV's TreeView node to set the icon to the normal (non-highlighted) state
                            //and update the ToolTip text to indicate this state
                            controlNode.ToolTipText = @"Current state = CENTERED";
                            controlNode.ImageIndex = IMAGE_INDEX_Pov;
                            controlNode.SelectedImageIndex = IMAGE_INDEX_Pov;
                        }
                    }
                }
        }

        /// <summary>
        ///     Clears all the ToolTip texts on all TreeView nodes
        /// </summary>
        private void ClearAllTreeViewItemToolTipTexts()
        {
            foreach (TreeNode deviceNode in treeMain.Nodes[0].Nodes)
            {
                foreach (TreeNode containerNode in deviceNode.Nodes)
                {
                    foreach (TreeNode controlNode in containerNode.Nodes)
                        controlNode.ToolTipText = string.Empty;
                    containerNode.ToolTipText = string.Empty;
                }
                deviceNode.ToolTipText = string.Empty;
            }
            treeMain.Nodes[0].ToolTipText = string.Empty;
        }

        private static bool CompareAxisTypes(AxisMapping virtualControl, PhysicalControlInfo physicalControl)
        {
            bool areSame;
            if (virtualControl.AxisType == AxisTypes.X && physicalControl.AxisType == AxisType.X)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.Y && physicalControl.AxisType == AxisType.Y)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.Z && physicalControl.AxisType == AxisType.Z)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.XRotation && physicalControl.AxisType == AxisType.XR)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.YRotation && physicalControl.AxisType == AxisType.YR)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.ZRotation && physicalControl.AxisType == AxisType.ZR)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.Slider && physicalControl.AxisType == AxisType.Slider)
            {
                areSame = true;
            }
            else if (virtualControl.AxisType == AxisTypes.Pov && physicalControl.AxisType == AxisType.Pov)
            {
                areSame = true;
            }
            else if (physicalControl.AxisType == AxisType.Unknown &&
                     (physicalControl.ControlType == ControlType.Axis ||
                      physicalControl.ControlType == ControlType.Pov))
            {
                areSame = true;
            }
            else
            {
                areSame = false;
            }
            return areSame;
        }

        /// <summary>
        ///     Creates a set of default mappings where each physical button, Pov, and axis gets mapped
        ///     to a corresponding virtual control.
        /// </summary>
        private void CreateDefaultMappings()
        {
            var shouldContinue =
                PromptForSaveChanges(
                    @"The requested operation cannot be undone.  There are currently unsaved changes.  Would you like to save all unsaved changes before continuing?");
            if (!shouldContinue)
            {
                return;
            }
            var oldMap = (OutputMap) ((ICloneable) _outputMap).Clone();

            //Create a new output map from scratch (similar to File-New), containing all the detected input devices
            CreateNewOutputMap();

            //iterate over the controls tree to find each physical device and each control on that device,
            //then map each physical control to the next available like-kind virtual control
            foreach (TreeNode topLevelCategoryNode in treeMain.Nodes)
            foreach (TreeNode deviceNode in topLevelCategoryNode.Nodes)
                if (deviceNode.Tag is PhysicalDeviceInfo)
                {
                    //under each device node, there are "container" nodes that break up the
                    //device's controls by category (button, Pov, axis).  Iterate over these node's
                    //children to get the physical controls themselves
                    foreach (TreeNode controlCategoryNode in deviceNode.Nodes)
                    foreach (TreeNode controlNode in controlCategoryNode.Nodes)
                        try
                        {
                            if (!(controlNode.Tag is PhysicalControlInfo)) continue;
                            //the current node is a physical control so obtain the physical control info object
                            //from the node's .Tag property
                            var physicalControl = (PhysicalControlInfo) controlNode.Tag;

                            VirtualControlInfo virtualControl = null;
                            // holds the virtual control we'll map this physical control to

                            switch (physicalControl.ControlType)
                            {
                                case ControlType.Axis:
                                case ControlType.Pov:
                                    //if this node represents an axis or Pov control, map to the next available virtual axis data source of the same type
                                    virtualControl = GetNextUnassignedPPJoyAxisControl(physicalControl);
                                    break;
                                case ControlType.Button:
                                    //if this node represents a button control, map to the next available virtual button data source 
                                    virtualControl = GetNextUnassignedPPJoyButtonControl();
                                    break;
                                case ControlType.Unknown:
                                    break;
                                case ControlType.Key:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            //save this mapping to the output map
                            _outputMap.SetMapping(physicalControl, virtualControl);
                        }
                        catch (ApplicationException e)
                        {
                            _log.Debug(e.Message, e);
                        }
                }
            //merge original mappings into the new output map
            MergeMaps(oldMap, _outputMap, true);

            //return true if any changes were made to the output map during this process
        }

        private void CreateMediator()
        {
            //create a new Mediator that can raise input-device state-changed events to the event handler in this form's code
            _mediator = new Mediator(this);
            _mediator.PhysicalControlStateChanged += _mediator_PhysicalControlStateChanged;
        }

        private void CreateMediatorAndUpdateMediatorMap()
        {
            CreateMediator();
            UpdateMediatorMap();
        }

        /// <summary>
        ///     Creates a new Output Map and initializes it with all of the currently-detected devices
        ///     known to DirectInput, and all of their controls, with no actual mappings established
        ///     for any given control.  NOTE:Controls, even unmapped controls, must appear in the
        ///     output map (as unmapped, in that case) in order to appear in the control tree
        ///     displayed on this form.
        /// </summary>
        private void CreateNewOutputMap()
        {
            //create a new, blank Output Map and set that as the form's Output Map
            _outputMap = new OutputMap();

            //get a list of joysticks that DirectInput can currently detect
            IList<DeviceInstance> detectedJoysticks=null;
            using (var directInput = new DirectInput())
            {
                detectedJoysticks = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AllDevices);
            }
            if (detectedJoysticks != null)
            {
                //for each detected joystick, determine if it's physical or a virtual PPJoy stick,
                //and if it's physical, then it should appear in the new output map.
                foreach (DeviceInstance instance in detectedJoysticks)
                {
                    var deviceInfo = new DIPhysicalDeviceInfo(instance.InstanceGuid, instance.InstanceName);
                    //Get the DIDeviceMonitor object from the monitor pool that represents the input
                    //device being evaluated.   If no monitor exists in the pool yet for that device,
                    //one will be created and added to the pool.  This avoids having multiple objects
                    //taking control of a device at the same time -- all communication with
                    //the device itself will occur via the monitor object, not via DirectInput directly.
                    var dev = DIDeviceMonitor.GetInstance(deviceInfo, this,
                        VirtualJoystick.MinAnalogDataSourceVal,
                        VirtualJoystick.MaxAnalogDataSourceVal);

                    //get the vendor ID and product ID from the current device
                    var productId = dev.VendorIdentityProductId;

                    //check with PPJoy to determine if this device is actually a virtual joystick --
                    //PPJoy will check the vendor identity and product ID to determine this
                    if (!productId.HasValue) continue;
                    var isVirtual = false;
                    try
                    {
                        isVirtual = new DeviceManager().IsVirtualDevice(productId.Value);
                    }
                    catch (DeviceNotFoundException e)
                    {
                        _log.Debug(e.Message, e);
                    }
                    if (isVirtual)
                    {
                        dev.Dispose();
                    }
                    else
                    {
                        //if it's not a PPJoy virtual device, then we can add it to the map.

                        //create a PhysicalDeviceInfo object to represent the current device, allowing
                        //us to get a list of its controls
                        var thisDev = new DIPhysicalDeviceInfo(instance.InstanceGuid, instance.InstanceName);

                        //add all the device's controls to the map, mapping them to nothing (null).
                        //This will ensure that they appear in the control tree on the form, without
                        //having any explicit mappings defined at this point in time.
                        foreach (var pci in thisDev.Controls)
                            _outputMap.SetMapping(pci, null);
                    }
                }
            }

            //get a list of BetaInnovations input devices
            var biDeviceManager = BIDeviceManager.GetInstance();
            BIPhysicalDeviceInfo[] biDevices = null;
            try
            {
                biDevices = biDeviceManager.GetDevices(false);
            }
            catch (OperationFailedException e)
            {
                _log.Debug(e.Message, e);
            }

            if (biDevices != null)
            {
                //for each detected BetaInnovations input device, add it to the new output map.
                foreach (var biDevice in biDevices)
                {
                    //Get the BIDeviceMonitor object from the monitor pool that represents the input
                    //device being evaluated.   If no monitor exists in the pool yet for that device,
                    //one will be created and added to the pool.  This avoids having multiple objects
                    //taking control of a device at the same time -- all communication with
                    //the device itself will occur via the monitor object, not via the BetaInnovations SDK directly.
                    BIDeviceMonitor.GetInstance(biDevice);

                    //add all the device's controls to the map, mapping them to nothing (null).
                    //This will ensure that they appear in the control tree on the form, without
                    //having any explicit mappings defined at this point in time.
                    foreach (var pci in biDevice.Controls)
                        _outputMap.SetMapping(pci, null);
                }
            }

            //get a list of PHCC devices
            PHCCPhysicalDeviceInfo[] phccDevices = null;
            try
            {
                phccDevices = PHCCDeviceManager.GetDevices();
            }
            catch (ApplicationException e)
            {
                _log.Debug(e.Message, e);
            }

            if (phccDevices != null)
            {
                //for each detected PHCC device, add it to the new output map.
                foreach (var phccDevice in phccDevices)
                {
                    //Get the PHCCDeviceMonitor object from the monitor pool that represents the PHCC
                    //device being evaluated.   If no monitor exists in the pool yet for that device,
                    //one will be created and added to the pool.  This avoids having multiple objects
                    //taking control of a device at the same time -- all communication with
                    //the device itself will occur via the monitor object, not via the PHCC Interface Library directly.
                    var dev = PHCCDeviceMonitor.GetInstance(phccDevice,
                        VirtualJoystick.MinAnalogDataSourceVal,
                        VirtualJoystick.MaxAnalogDataSourceVal);

                    //add all the device's controls to the map, mapping them to nothing (null).
                    //This will ensure that they appear in the control tree on the form, without
                    //having any explicit mappings defined at this point in time.
                    if (phccDevice.Controls == null) continue;
                    foreach (var pci in phccDevice.Controls)
                        _outputMap.SetMapping(pci, null);
                }
            }

            //since we're creating a new output map here, it's assumed that this operation will
            //reset the changes flag, since change tracking *starts* from this point -- the 
            //creation of a new map, and works forward.  Outer methods calling this method
            //can override that behavior from the outside, if needed in special cases.
            _hasChanges = false;
        }


        /// <summary>
        ///     Defines new PPJoy virtual joystick devices
        /// </summary>
        private void CreateNewPPJoyVirtualJoystickDevices()
        {
            Util.CountPPJoyVirtualDevices();

            //prompt the user to see how many additional devices to create
            var promptForm = new frmPPJoyDevicesToCreate();
            var result = promptForm.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                StopMediatingAndDisposeMediator();

                var manager = new DeviceManager();
                var devicesCreated = 0;
                var currentDeviceUnitNum = 0;
                var unitNumsToSetIdealMappingsOn = new List<int>();
                while (devicesCreated < promptForm.NumDevicesToCreate)
                {
                    Application.DoEvents();
                    try
                    {
                        manager.CreateDevice(0, JoystickTypes.Virtual_Joystick, JoystickSubTypes.NotApplicable,
                            currentDeviceUnitNum);
                        unitNumsToSetIdealMappingsOn.Add(currentDeviceUnitNum);
                        devicesCreated++; //if successful, increment count of devices created
                    }
                    catch (DeviceAlreadyExistsException e)
                    {
                        _log.Debug(e.Message, e);
                    }
                    catch (OperationFailedException e)
                    {
                        _log.Debug(e.Message, e);
                    }
                    currentDeviceUnitNum++; //increment the device unit number 
                    if (currentDeviceUnitNum > manager.MaxValidUnitNumber(JoystickTypes.Virtual_Joystick))
                    {
                        break;
                    }
                }
                //foreach (int unitNum in unitNumsToSetIdealMappingsOn)
                //{
                //    try
                //    {
                //        manager.SetDeviceMappings(0, JoystickTypes.Virtual_Joystick, unitNum, JoystickMapScope.Device, manager.IdealMappings);
                //    }
                //    catch (ApplicationException)
                //    {
                //                _log.Debug(e.Message, e);
                //    }
                //}
            }
            CreateMediatorAndUpdateMediatorMap();
        }

        private void DisableAutoHighlightingFunctionality()
        {
            btnAutoHighlighting.Enabled = false;
        }

        /// <summary>
        ///     Disables a node in the Treeview whose only purpose is to serve as a container for other kinds of nodes.
        /// </summary>
        /// <param name="node">The TreeNode representing a control container, whose children will be disable.</param>
        private void DisableContainerNode(TreeNode node)
        {
            //TODO: comment this
            if (node != null)
            {
                if (node.Text.StartsWith("Axes", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_AXIS_GREYED;
                    node.SelectedImageIndex = IMAGE_INDEX_AXIS_GREYED;
                }
                else if (node.Text.StartsWith("Buttons", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_BUTTON_GREYED;
                    node.SelectedImageIndex = IMAGE_INDEX_BUTTON_GREYED;
                }
                else if (node.Text.StartsWith("Povs", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_POV_GREYED;
                    node.SelectedImageIndex = IMAGE_INDEX_POV_GREYED;
                }
                else if (node.Text.StartsWith("Local Devices", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_LOCAL_DEVICES_GREYED;
                    node.SelectedImageIndex = IMAGE_INDEX_LOCAL_DEVICES_GREYED;
                }
                node.ForeColor = BackColor;
            }
        }

        /// <summary>
        ///     Disables mapping for the physical control represented by a specific TreeNode.
        /// </summary>
        /// <param name="controlNode">A TreeNode representing a physical control to disable mapping for.</param>
        private void DisableControlNode(TreeNode controlNode)
        {
            //get an object representing the actual physical control that the TreeNode represents
            var currentControl = controlNode.Tag as PhysicalControlInfo;

            if (currentControl != null) //if we found such an object, then this node is truly a Control node
            {
                //disable mapping for this physical control
                _outputMap.DisableMapping(currentControl);

                //set the icon for this control to the greyed-out icon
                //for the specific type of control that this is
                if (currentControl.ControlType == ControlType.Axis)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_AXIS_GREYED;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_AXIS_GREYED;
                }
                else if (currentControl.ControlType == ControlType.Button)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_BUTTON_GREYED;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_BUTTON_GREYED;
                }
                else if (currentControl.ControlType == ControlType.Pov)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_POV_GREYED;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_POV_GREYED;
                }
                //grey-out the control's text description in the TreeView
                controlNode.ForeColor = BackColor;
            }
        }

        /// <summary>
        ///     Disables the "Create Default Mapping" menu item.
        /// </summary>
        private void DisableCreateDefaultMapping()
        {
            mnuActionsCreateDefaultMapping.Enabled = false;
        }

        /// <summary>
        ///     Greys out a TreeNode representing a Physical Device.
        /// </summary>
        /// <param name="deviceNode">a TreeNode (representing a Physical Device) to grey out</param>
        private void DisableDeviceNode(TreeNode deviceNode)
        {
            //get an object representing the associated Physical Device itself
            var currentDevice = deviceNode.Tag as PhysicalDeviceInfo;
            if (currentDevice != null) //if this is not null, then we have found such an object
            {
                //out the node's descriptive text and set its icon to the
                //Greyed-state icon
                deviceNode.ImageIndex = IMAGE_INDEX_JOYSTICK_GREYED;
                deviceNode.SelectedImageIndex = IMAGE_INDEX_JOYSTICK_GREYED;
                deviceNode.ForeColor = BackColor;
                deviceNode.Collapse(true); //collapse the node if it's expanded, hiding its children nodes
            }
        }

        /// <summary>
        ///     Disables menu items/toolbar items associated with the Disable functionality for a treeview item
        /// </summary>
        private void DisableDisableItemFunctionality()
        {
            mnuCtxTreeItemsDisable.Enabled = false;
        }

        /// <summary>
        ///     Disables menu items/toolbar items associated with the Enable functionality for a treeview item
        /// </summary>
        private void DisableEnableItemFunctionality()
        {
            mnuCtxTreeItemsEnable.Enabled = false;
        }

        /// <summary>
        ///     Disables the File-Import menu item
        /// </summary>
        private void DisableFileImportFunctionality()
        {
            mnuFileImport.Enabled = false;
        }

        /// <summary>
        ///     Disables the File-New menu item and corresponding toolbar buttons
        /// </summary>
        private void DisableFileNewFunctionality()
        {
            mnuFileNew.Enabled = false;
            btnNew.Enabled = false;
        }

        /// <summary>
        ///     Disables the File-Open menu item and corresponding toolbar button
        /// </summary>
        private void DisableFileOpenFunctionality()
        {
            mnuFileOpen.Enabled = false;
            btnOpen.Enabled = false;
        }

        private void DisableMappingsTab()
        {
            tabMain.Enabled = false;
        }

        /// <summary>
        ///     Disables menu items that can't/shouldn't be used during
        ///     active mediation (i.e. when data is actively being sent to PPJoy)
        /// </summary>
        private void DisableMediationSensitiveMenuFunctionality()
        {
            tabMain.Enabled = false;
            mnuViewRefreshDeviceList.Enabled = false; //bad idea to change the output map during active mediation
            mnuPPJoyOpenPPJoyControlPanel.Enabled = false; //shouldn't change the underlying PPJoy stuff either!
            btnRefreshDevices.Enabled = false;
        }

        private void DisableNode(TreeNode node)
        {
            //TODO: comment this
            if (node == null)
            {
                return;
            }
            var tag = node.Tag;
            foreach (TreeNode child in node.Nodes)
                DisableNode(child);
            if (tag is PhysicalControlInfo)
            {
                DisableControlNode(node); //disable the control node itself
                if (!IsNodeEnabled(node.Parent))
                {
                    DisableContainerNode(node.Parent); //disable the control container node
                }
                if (node.Parent != null && !IsNodeEnabled(node.Parent.Parent))
                {
                    DisableContainerNode(node.Parent.Parent); //disable the device node
                }
                if (node.Parent != null && node.Parent.Parent != null && !IsNodeEnabled(node.Parent.Parent.Parent))
                {
                    DisableContainerNode(node.Parent.Parent.Parent); //disable the local devices node
                }
            }
            else if (tag is PhysicalDeviceInfo)
            {
                DisableDeviceNode(node);
                if (!IsNodeEnabled(node.Parent))
                {
                    DisableContainerNode(node.Parent); //disable the device node
                }
                if (node.Parent != null && !IsNodeEnabled(node.Parent.Parent))
                {
                    DisableContainerNode(node.Parent.Parent); //disable the local devices node
                }
            }
            else
            {
                DisableContainerNode(node); //the control container node itself
                if (!IsNodeEnabled(node.Parent))
                {
                    DisableContainerNode(node.Parent); //the device node
                }
                if (node.Parent != null && !IsNodeEnabled(node.Parent.Parent))
                {
                    DisableContainerNode(node.Parent.Parent); //the the local devices node
                }
            }
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Disables menu items on the PPJoy menu
        /// </summary>
        private void DisablePPJoyMenuFunctionality()
        {
            mnuPPJoy.Enabled = false;
        }

        /// <summary>
        ///     Disables the "Remove" menu item functionality
        /// </summary>
        private void DisableRemoveItemFunctionality()
        {
            mnuCtxTreeItemsRemove.Enabled = false;
        }

        /// <summary>
        ///     Disables all menu items and toolbar buttons that would allow starting mediation (presumably
        ///     because mediation is already started)
        /// </summary>
        private void DisableStartMappingFunctionality()
        {
            mnuActionsStartMapping.Enabled = false;
            mnuTrayCtxStartMapping.Enabled = false;
            btnStart.Enabled = false;
        }

        /// <summary>
        ///     Disables all menu items and toolbar buttons that would allow stopping mediation (presumably
        ///     because mediation is already stopped)
        /// </summary>
        private void DisableStopMappingFunctionality()
        {
            mnuActionsStopMapping.Enabled = false;
            mnuTrayCtxStopMapping.Enabled = false;
            btnStop.Enabled = false;
        }

        /// <summary>
        ///     Updates the controls on the Mappings tab to display the current mapping for a
        ///     given physical control.
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object representing the
        ///     physical control whose mappings will be displayed
        /// </param>
        private void DisplayCurrentMapping(PhysicalControlInfo inputControl)
        {
            PopulateVirtualDevicesList();

            //get the current virtual (output) control associated with the specified
            //physical (input) control in the Output Map
            var outputControl = _outputMap.GetMapping(inputControl);

            //if the specified physical (input) control is not currently mapped to any 
            //virtual (output) control, then display the <unmapped> states in the
            //Mapping tab's controls
            if (outputControl == null)
            {
                //find the index of the item in the Virtual Device combobox 
                //representing the <unmapped> virtual device
                var unmappedIndex = cboVirtualDevice.FindStringExact("<unmapped>");

                //set the current item in the Virtual Device combo box to the index of the item 
                //bearing that <unmapped> string
                cboVirtualDevice.SelectedIndex = unmappedIndex;

                //find the index of the item in the Virtual Control combobox
                //representing the <unmapped> virtual control
                unmappedIndex = cboVirtualControl.FindStringExact("<unmapped>");

                //set the current item in the Virtual Control combo box to the index of the item
                //bearing that <unmapped> string
                cboVirtualControl.SelectedIndex = unmappedIndex;

                //disable the Virtual Control combo box so that you have to select a virtual device
                //first.  Selecting a valid virtual device will allow the Virtual Control combobox
                //to be enabled.
                cboVirtualControl.Enabled = false;

                //we've rendered the unmapped state for the specified physical control, 
                //so we're done.
            }
            else // if the specified virtual control *is* mapped to some virtual control, then display that mapping now
            {
                string expectedText = null; //reference to strings we'll use to search comboboxes with

                if (inputControl.ControlType == ControlType.Axis || inputControl.ControlType == ControlType.Pov)
                {
                    //if the specified physical control is an axis or a Pov, then we know we're
                    //looking for an Analog string in the Virtual Control combobox, and we know
                    //exactly which Analog data source we're looking for too, so we can
                    //assemble a search string here such as Analog0.
                    expectedText = "Analog" + outputControl.ControlNum;
                }
                else if (inputControl.ControlType == ControlType.Button)
                {
                    //otherwise, if the specified physical control is a Button, then we know
                    //we're looking for a Digital string in the Virtual Control combobox, and
                    //we know exactly which Digital string we're looking for (i.e. Digital0, etc.)
                    expectedText = "Digital" + outputControl.ControlNum;
                }

                //Now populate the comboboxes with the set of items they can realistically contain,
                //given the type of physical control we're dealing with and the particular
                //virtual device we're currently mapped to (the virtual Device is given by the virtual
                //Control's .Parent property -- this is important because the method we're calling
                //wants to know what data sources on this device are already being mapped to,
                //so that it won't add those to the list of selectable mappings.  If you were
                //to *unmap* some of those first, then they'd be selectable here)
                PopulateSelectableOutputControls(inputControl, outputControl.Parent);

                //now that our comboboxes are populated, we can set the currently-selected
                //value in the Virtual Control combobox, based on the search strings we created earlier
                cboVirtualControl.SelectedIndex = cboVirtualControl.FindStringExact(expectedText);

                cboVirtualControl.Enabled = true; //and we can enable the Virtual Control combobox for editing as well

                //the next thing to do is to set the currently-selected Virtual Device in the Virtual Device
                //combobox.  We can just build a simple search string based on the virtual device
                //number of the device we're mapping to
                expectedText = "PPJoy Virtual joystick " + outputControl.Parent.VirtualDeviceNum;

                //and now we set the selected item to the item matching that search string
                cboVirtualDevice.SelectedIndex = cboVirtualDevice.FindStringExact(expectedText);
            }
        }

        /// <summary>
        ///     Event handler for the FileOpen common dialog's dismissal (OK/Cancel) events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the common dialog's Ok/Cancel events.</param>
        private void dlgOpen_FileOk(object sender, CancelEventArgs e)
        {
            //check if the dialog was cancelled
            if (e.Cancel)
            {
                _lastOpenCancelled = true;
                return;
            }
            //get the full file path from the dialog
            var fileName = _dlgOpen.FileName;

            //attempt to load the mapping file specified by the user
            var file = new FileInfo(fileName);
            try
            {
                LoadMappingsFile(file);
            }
            catch (Exception ex)
            {
                _log.Debug(ex.Message, ex);
                _lastOpenCancelled = true;
                throw;
            }
        }

        /// <summary>
        ///     Event handler for the FileSave common dialog's dismissal (OK/Cancel) events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the common dialog's Ok events.</param>
        private void dlgSave_FileOk(object sender, CancelEventArgs e)
        {
            //check if the dialog was cancelled
            if (e.Cancel)
            {
                //store a flag indicating that the last save operation was cancelled
                _lastSaveCancelled = true;
                return; //and return control back to the calling method
            }
            //get the full file path from the dialog
            _fileName = _dlgSave.FileName;

            //attempt to save the current mappings file to the location specified by the user
            var file = new FileInfo(_fileName);
            SaveMappingsFile(file);
        }

        private void EnableAutoHighlightingFunctionality()
        {
            btnAutoHighlighting.Enabled = true;
        }

        private void EnableContainerNode(TreeNode node)
        {
            //TODO: comment this
            if (node != null)
            {
                if (node.Text.StartsWith("Axes", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_AXIS;
                    node.SelectedImageIndex = IMAGE_INDEX_AXIS;
                }
                else if (node.Text.StartsWith("Buttons", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_BUTTON;
                    node.SelectedImageIndex = IMAGE_INDEX_BUTTON;
                }
                else if (node.Text.StartsWith("Povs", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_Pov;
                    node.SelectedImageIndex = IMAGE_INDEX_Pov;
                }
                else if (node.Text.StartsWith("Local Devices", StringComparison.OrdinalIgnoreCase))
                {
                    node.ImageIndex = IMAGE_INDEX_LOCAL_DEVICES;
                    node.SelectedImageIndex = IMAGE_INDEX_LOCAL_DEVICES;
                }
                node.ForeColor = treeMain.ForeColor;
            }
        }

        /// <summary>
        ///     Enables mapping for the physical control represented by a specific TreeNode.
        /// </summary>
        /// <param name="controlNode">A TreeNode representing a physical control to enable mapping for.</param>
        private void EnableControlNode(TreeNode controlNode)
        {
            //get an object representing the actual physical control that the TreeNode represents
            var currentControl = controlNode.Tag as PhysicalControlInfo;
            if (currentControl != null) //if we found such an object, then this node is truly a Control node
            {
                //enable mapping for this physical control
                _outputMap.EnableMapping(currentControl);

                //set the icon for this control to the Normal-state icon
                //for the specific type of control that this is
                if (currentControl.ControlType == ControlType.Axis)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_AXIS;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_AXIS;
                }
                else if (currentControl.ControlType == ControlType.Button)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_BUTTON;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_BUTTON;
                }
                else if (currentControl.ControlType == ControlType.Pov)
                {
                    controlNode.ImageIndex = IMAGE_INDEX_Pov;
                    controlNode.SelectedImageIndex = IMAGE_INDEX_Pov;
                }
                //un-greyout the control's text description in the TreeView
                controlNode.ForeColor = treeMain.ForeColor;
            }
        }

        /// <summary>
        ///     Enables the "Create Default Mapping" menu item on the Actions menu
        /// </summary>
        private void EnableCreateDefaultMapping()
        {
            var numSticksDefined = 0;
            try
            {
                numSticksDefined = Util.CountPPJoyVirtualDevices();
            }
            catch (ApplicationException e)
            {
                _log.Debug(e.Message, e);
            }
            if (numSticksDefined > 0 && _outputMap.PhysicalDevices.Length > 0)
            {
                mnuActionsCreateDefaultMapping.Enabled = true;
            }
            else
            {
                mnuActionsCreateDefaultMapping.Enabled = false;
            }
        }

        /// <summary>
        ///     Un-greys out a node representing a Physical Device.
        /// </summary>
        /// <param name="deviceNode">a TreeNode (representing a Physical Device) to un-greyout</param>
        private void EnableDeviceNode(TreeNode deviceNode)
        {
            //get an object representing the associated Physical Device itself
            var currentDevice = deviceNode.Tag as PhysicalDeviceInfo;
            if (currentDevice != null) //if this is not null, then we have found such an object
            {
                //ungrey out the node's descriptive text and set its icon to the
                //Normal-state icon
                deviceNode.ForeColor = treeMain.ForeColor;
                deviceNode.ImageIndex = IMAGE_INDEX_JOYSTICK;
                deviceNode.SelectedImageIndex = IMAGE_INDEX_JOYSTICK;
                deviceNode.Expand(); //expand the device node to show the container nodes below it
            }
        }

        /// <summary>
        ///     Enables menu items/toolbar items associated with the Disable functionality for a treeview item
        /// </summary>
        private void EnableDisableItemFunctionality()
        {
            mnuCtxTreeItemsDisable.Enabled = true;
        }

        /// <summary>
        ///     Enables menu items/toolbar items associated with the Enable functionality for a treeview item
        /// </summary>
        private void EnableEnableItemFunctionality()
        {
            mnuCtxTreeItemsEnable.Enabled = true;
        }

        /// <summary>
        ///     Enables the File-Import menu item
        /// </summary>
        private void EnableFileImportFunctionality()
        {
            mnuFileImport.Enabled = true;
        }

        /// <summary>
        ///     Enables the File-New menu item and corresponding toolbar items
        /// </summary>
        private void EnableFileNewFunctionality()
        {
            mnuFileNew.Enabled = true;
            btnNew.Enabled = true;
        }

        /// <summary>
        ///     Enables the File-Open menu item and the corresponding toolbar item
        /// </summary>
        private void EnableFileOpenFunctionality()
        {
            mnuFileOpen.Enabled = true;
            btnOpen.Enabled = true;
        }

        private void EnableMappingsTab()
        {
            tabMain.Enabled = true;
        }

        /// <summary>
        ///     Enables menu items that can't/shouldn't be used during
        ///     active mediation (i.e. when data is actively being sent to PPJoy)
        /// </summary>
        private void EnableMediationSensitiveMenuFunctionality()
        {
            tabMain.Enabled = true;
            mnuViewRefreshDeviceList.Enabled = true; //bad idea to change the output map during active mediation
            mnuPPJoyOpenPPJoyControlPanel.Enabled = true; //shouldn't change the underlying PPJoy stuff either!
            btnRefreshDevices.Enabled = true;
        }

        /// <summary>
        ///     Enables a node in the TreeView, and its child nodes.
        /// </summary>
        /// <param name="node"></param>
        private void EnableNode(TreeNode node)
        {
            if (node == null)
            {
                return;
            }
            //enable all children nodes of the specified node
            foreach (TreeNode child in node.Nodes)
                EnableNode(child);
            var tag = node.Tag;
            if (tag is PhysicalControlInfo)
            {
                EnableControlNode(node); //control node itself
                EnableContainerNode(node.Parent); //control container node
                EnableContainerNode(node.Parent.Parent); //device node
                EnableContainerNode(node.Parent.Parent.Parent); //local devices node
            }
            else if (tag is PhysicalDeviceInfo)
            {
                EnableDeviceNode(node); //device node itself
                EnableContainerNode(node.Parent); //local devices node
            }
            else
            {
                EnableContainerNode(node); //container node itself
                EnableContainerNode(node.Parent); //device node
                EnableContainerNode(node.Parent); //local devices node
            }

            //update the menu & toolbar enabled/disabled states
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Enables menu items on the PPJoy menu
        /// </summary>
        private void EnablePPJoyMenuFunctionality()
        {
            mnuPPJoy.Enabled = true;
        }

        /// <summary>
        ///     Enables the "Remove" menu item functionality
        /// </summary>
        private void EnableRemoveItemFunctionality()
        {
            mnuCtxTreeItemsRemove.Enabled = true;
        }

        /// <summary>
        ///     Enables the Actions-StartMapping menu item, the corresponding context-menu item, and
        ///     the corresponding toolbar button (presumably, because mediation is now stopped)
        /// </summary>
        private void EnableStartMappingFunctionality()
        {
            mnuActionsStartMapping.Enabled = true;
            mnuTrayCtxStartMapping.Enabled = true;
            btnStart.Enabled = true;
        }

        /// <summary>
        ///     Enables the Actions-StopMapping menu item, the corresponding context-menu item, and
        ///     the corresponding toolbar button (presumably, because mediation is now active)
        /// </summary>
        private void EnableStopMappingFunctionality()
        {
            mnuActionsStopMapping.Enabled = true;
            mnuTrayCtxStopMapping.Enabled = true;
            btnStop.Enabled = true;
        }

        /// <summary>
        ///     Enables menu items on the Tools menu, and their corresonding toolbar buttons
        /// </summary>
        private void EnableToolsMenuFunctionality()
        {
            mnuPPJoyOpenPPJoyControlPanel.Enabled = true;
            mnuToolsOpenWindowsJoystickControlPanel.Enabled = true;
            mnuTools.Enabled = true;
            mnuToolsOptions.Enabled = true;
            mnuViewRefreshDeviceList.Enabled = true;
            btnOptions.Enabled = true;
            btnRefreshDevices.Enabled = true;
        }

        /// <summary>
        ///     Implements the behavior for the File-Exit menu command
        /// </summary>
        /// <returns>
        ///     false if the user cancelled the operation, and true if the user
        ///     is exiting the program.  Probably never actually returns true because
        ///     the app's shutdown is invoked during this method call, and a hard-terminate
        ///     condition exists in that logic, preventing control flow from ever reaching
        ///     the return true condition herein.
        /// </returns>
        private bool FileExit()
        {
            //prompt the user to exit and give them the opportunity to cancel
            if (!UserWantsToExit()) return false;
            //shut down the mediator and quit gracefully if the user wants to exit
            StopRunning();
            return true; //probably never reached!  required by the compiler
            //or just do nothing if the user cancelled the exit operation
        }

        /// <summary>
        ///     Finds the TreeNode that corresponds to a specific physical input control.
        /// </summary>
        /// <param name="control">
        ///     a PhysicalControlInfo object representing the physical input control whose TreeView node will be
        ///     returned.
        /// </param>
        /// <returns>
        ///     a TreeNode object representing the node in the TeeView that is associated with the specified physical input
        ///     control.
        /// </returns>
        private TreeNode findNodeForControl(PhysicalControlInfo control)
        {
            if (treeMain.Nodes.Count > 0)
            {
                foreach (TreeNode deviceNode in treeMain.Nodes[0].Nodes)
                foreach (TreeNode containerNode in deviceNode.Nodes)
                foreach (TreeNode controlNode in containerNode.Nodes)
                {
                    var controlInNode = (PhysicalControlInfo) controlNode.Tag;

                    if (controlInNode != null && controlInNode.Equals(control))
                    {
                        return controlNode;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Event handler for the FormClosing event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the FormClosing event.</param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if the user attempts to close the form using the form's close button or
            //the form's control menu, then we can ask the user to save any unsaved
            //changes, and if the user cancels that operation, then we can cancel
            //the FormClosing event.
            e.Cancel |= !FileExit();
        }

        /// <summary>
        ///     Event handler for the form's Load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Load event.</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            //generate greyed-out images to complement the color images in the TreeView's ImageList control
            AddGreyedOutImages();

            CreateMediator();
            //set the form's context menu
            treeMain.ContextMenuStrip = mnuCtxTreeItems;

            //disable runtime checking for illegal cross-thread calls to the UI thread.
            //We have to do this or DirectInput will throw a fit because its callback
            //methods do exactly that -- make cross-thread UI-thread calls.
            CheckForIllegalCrossThreadCalls = false;

            //configure the tray icon and set it to invisible
            nfyTrayIcon.Text = Application.ProductName;
            nfyTrayIcon.Visible = false;
            nfyTrayIcon.ContextMenuStrip = mnuCtxTrayIcon;

            //update the application's title bar to indicate an untitled map is loaded
            Text = Application.ProductName + @" - Untitled";

            //register event handlers for the common dialog events
            _dlgSave.FileOk += dlgSave_FileOk;
            _dlgOpen.FileOk += dlgOpen_FileOk;

            //initialize form-level variables that need to be initialized during the Load event
            _mappingsTab = tabMain.TabPages[nameof(tabMappings)];

            //remove the "Mappings" tab from the tab control
            tabMain.TabPages.Remove(_mappingsTab);

            //configure data binding for the comboboxes in the mappings tab
            cboVirtualDevice.DisplayMember = "InstanceName";
            cboVirtualDevice.ValueMember = "InstanceGuid";

            //check if PPJoy is installed
            if (!IsPPJoyInstalled())
            {
                MessageBox.Show(
                    @"WARNING: " + Application.ProductName + @" requires PPJoy, which was not found on this machine.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                StopRunning();
            }
            /*
            //check for presence of at least one PPJoy virtual device and prompt
            //user to auto-create these devices if they're not found
            if (!(new PPJoy.DeviceManager().GetAllDevices().Length > 0))
            {
                DialogResult dr = MessageBox.Show("PPJoy is installed on this system, but no PPJoy virtual joystick devices have been configured.  Would you like JoyMapper to configure these for you now?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    CreateNewPPJoyVirtualJoystickDevices();
                }
            }
            */

            //check for presence of at least one PPJoy virtual device and prompt
            //user to auto-create these devices if they're not found
            if (!(new DeviceManager().GetAllDevices().Length > 0))
            {
                var dr =
                    MessageBox.Show(
                        @"PPJoy is installed on this system, but no PPJoy virtual joystick devices have been configured.  Would you like to configure these devices now?",
                        Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    //CreateNewPPJoyVirtualJoystickDevices();
                    OpenPPJoyControlPanel();
                }
            }


            //initialize menu items states (set whether they're enabled or disabled)
            SetMenuAndToolbarEnabledDisabledStates();

            //populate the combo boxes in the mappings tab with their initial values
            PopulateVirtualDevicesList();

            //retrieve the user's custom settings
            var set = Settings.Default;
            if (set.EnableAutoHighlighting)
            {
                _autoHighlightingEnabled = true;
                btnAutoHighlighting.Text = @"Auto-highlighting (on)";
                EnableAutoHighlightingFunctionality();
            }
            else
            {
                _autoHighlightingEnabled = false;
                btnAutoHighlighting.Text = @"Auto-highlighting (off)";
                DisableAutoHighlightingFunctionality();
            }
            if (set.LoadDefaultMappingFile)
            {
                //if the user's settings call 
                //for a certain mapping file to be 
                //loaded on application startup, 
                //load that file now

                var fileName = set.DefaultMappingFile;
                var file = new FileInfo(fileName);
                var success = LoadMappingsFile(file);

                if (success) //if we successfully loaded the file specified in the settings
                {
                    if (!set.StartMappingOnLaunch) return;
                    //update the toolbar and menu item enabled/diabled states
                    SetMenuAndToolbarEnabledDisabledStates();

                    //if starting mapping is allowed via the toolbar or menu,
                    //then we're ok to start mapping here.  
                    if (mnuActionsStartMapping.Enabled)
                    {
                        StartMapping();
                    }
                }
                else //the file specieid in the settings didn't load successfully,
                    //so we need to initialize the application using the NewFile()
                    //method.
                {
                    NewFile();
                }
            }
            else
            {
                //if there's no default mapping file specified, then we have
                //to initialize the application using the NewFile() method.
                NewFile();
            }
        }

        /// <summary>
        ///     Event handler for the form's SizeChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the SizeChanged event.</param>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            //NOTE: this event is fired *after* the window already has changed size

            if (WindowState == FormWindowState.Minimized)
            {
                //if the user minimizes the application's main form, then add the app
                //icon to the system tray, hide the app's main form, and remove the app's
                //button from the task tray
                MinimizeToSystemTray();
            }
            else
            {
                //if the main form is in some state other than the minimized state,
                //then we should not have the tray icon visible, but teh app's window
                //should be visible.

                RestoreFromSystemTray();
            }
        }

        /// <summary>
        ///     Gets the text to display next to a particular control's node in the TreeView
        /// </summary>
        /// <param name="pci">
        ///     a PhysicalControlInfo object representing the input control whose descriptive text representation
        ///     will be returned.
        /// </param>
        /// <returns>a String containing the text to display next to the control's node in the TreeView</returns>
        private static string GetControlDisplayText(PhysicalControlInfo pci)
        {
            string controlName = null;
            switch (pci.ControlType)
            {
                case ControlType.Axis:
                    controlName = pci.AxisType == AxisType.Slider
                        ? pci.AxisType + " " + (pci.ControlNum + 1) + " Axis"
                        : pci.AxisType + " Axis";
                    break;
                case ControlType.Pov:
                    controlName = pci.AxisType + " " + (pci.ControlNum + 1);
                    break;
                case ControlType.Button:
                    controlName = pci.ControlType + " " + (pci.ControlNum + 1);
                    break;
                case ControlType.Unknown:
                    break;
                case ControlType.Key:
                    break;
                default:
                    controlName = "Unknown control " + pci.ControlNum;
                    break;
            }
            if (controlName != null && (pci.Alias == null || pci.Alias.Trim() == controlName.Trim())) return controlName;
            if (pci.AxisType != AxisType.Unknown)
            {
                controlName += pci.Alias != null && !string.Equals(pci.Alias, string.Empty)
                    ? " - " + pci.Alias
                    : "";
            }
            else
            {
                controlName = pci.Alias; //so it doesn't read "Unknown Axis - xxx", just "Axis xxx".
            }
            return controlName;
        }

        /// <summary>
        ///     Gets the next-available unassigned PPJoy axis data source, starting at
        ///     Virtual Joystick 1, Analog0 and
        ///     working up to the highest-numbered virtual joystick and highest-numbered analog data source,
        ///     until an unmapped data source is located.  If no unmapped data source is located, this method
        ///     returns NULL.
        /// </summary>
        /// <param name="physicalControl">
        ///     A PhysicalControlInfo object representing the physical control
        ///     that will be mapped to the next-available corresponding virtual axis data source
        /// </param>
        /// <returns>
        ///     a VirtualControlInfo object representing the next-available unmapped
        ///     analog data source across all virtual joysticks, or NULL if no available
        ///     unmapped corresponding virtual analog data source can be found.
        /// </returns>
        private VirtualControlInfo GetNextUnassignedPPJoyAxisControl(PhysicalControlInfo physicalControl)
        {
            //axis data sources must be of the correct type -- while you can map any 
            //physical axis to any virtual axis data source, it's best if you know that
            //the axis data source you're mapping to is actually mapped internally by PPJoy to
            //an exposed virtual axis of the same type (i.e. mapping a physical X axis to a virtual
            //axis data source exposed by a virtual X axis is ok, but other permutations are not).

            var ppjoyDevices = new DeviceManager().GetAllDevices();
            foreach (var thisDevice in ppjoyDevices)
            {
                if (thisDevice.DeviceType != JoystickTypes.Virtual_Joystick)
                {
                    continue;
                }
                var mappings = thisDevice.GetMappings();
                foreach (Mapping mapping in mappings)
                    if (mapping is PovMapping)
                    {
                        if (!(mapping is ContinuousPovMapping)) continue;
                        var datasource = ((ContinuousPovMapping) mapping).DataSource;
                        if (datasource == ContinuousPovDataSources.None) continue;
                        var dataSourceName = Enum.GetName(typeof(ContinuousPovDataSources), datasource);
                        if (dataSourceName == null) continue;
                        var dataSourceNumber =
                            dataSourceName.Replace("Analog", string.Empty).Replace("Reversed", string.Empty);
                        var dsNum = int.Parse(dataSourceNumber);

                        var virtualDevice = new VirtualDeviceInfo(thisDevice.UnitNum + 1);
                        var virtualControl = new VirtualControlInfo(virtualDevice, ControlType.Axis, dsNum);
                        if (!_outputMap.ContainsMappingTo(virtualControl))
                        {
                            return virtualControl;
                        }
                        //TODO: implement directional POV auto-mapping
                    }
                    else if (mapping is AxisMapping)
                    {
                        if (CompareAxisTypes((AxisMapping) mapping, physicalControl))
                        {
                            var virtualDevice = new VirtualDeviceInfo(thisDevice.UnitNum + 1);
                            var virtualControl = new VirtualControlInfo(virtualDevice, physicalControl.ControlType,
                                mapping.ControlNumber);
                            if (!_outputMap.ContainsMappingTo(virtualControl))
                            {
                                return virtualControl;
                            }
                        }
                    }
            }
            return null;
        }

        /// <summary>
        ///     Gets the next-available unassigned PPJoy button data source, starting at
        ///     Virtual Joystick 1, Digital0 and
        ///     working up to the highest-numbered virtual joystick and highest-numbered digital data source,
        ///     until an unmapped data source is located.  If no unmapped data source is located, this method
        ///     returns NULL.
        /// </summary>
        /// <returns>
        ///     a VirtualControlInfo object representing the next-available unmapped
        ///     digital data source across all virtual joysticks, or NULL if no available
        ///     unmapped virtual digital data source can be found.
        /// </returns>
        private VirtualControlInfo GetNextUnassignedPPJoyButtonControl()
        {
            var ppjoyDevices = new DeviceManager().GetAllDevices();
            foreach (var thisDevice in ppjoyDevices)
            {
                if (thisDevice.DeviceType != JoystickTypes.Virtual_Joystick)
                {
                    continue;
                }
                var mappings = thisDevice.GetMappings();
                foreach (ButtonMapping mapping in mappings.ButtonMappings)
                {
                    var virtualDevice = new VirtualDeviceInfo(thisDevice.UnitNum + 1);
                    var virtualControl =
                        new VirtualControlInfo(virtualDevice, ControlType.Button, mapping.ControlNumber);
                    if (!_outputMap.ContainsMappingTo(virtualControl))
                    {
                        return virtualControl;
                    }
                }
            }
            return null;
        }


        private static string GetPPJoyControlPanelFilePath()
        {
            var c = new Computer();
            using (
                var uninstallKey =
                    c.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"))
            {
                if (uninstallKey == null) return null;
                var subkeys = uninstallKey.GetSubKeyNames();
                foreach (var subkeyName in subkeys)
                    if (subkeyName.Equals("Parallel Port Joystick", StringComparison.InvariantCultureIgnoreCase) ||
                        subkeyName.StartsWith("PPJoy Joystick Driver", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var ppjoyUninstallerSubkey = uninstallKey.OpenSubKey(subkeyName);
                        if (ppjoyUninstallerSubkey == null) continue;
                        var cplPath =
                            (string) ppjoyUninstallerSubkey.GetValue("DisplayIcon", null, RegistryValueOptions.None);
                        if (string.IsNullOrEmpty(cplPath)) return null;
                        var fi = new FileInfo(cplPath);
                        if (fi.Directory != null)
                            return fi.Exists ? cplPath : Path.Combine(fi.Directory.FullName, "pportjoy.cpl");
                    }
            }
            return null;
        }

        /// <summary>
        ///     Gets a value that indicates if the specified node's mappings are enabled.  If the specified node is not a leaf
        ///     node, then the value returned indicates if *all* of the children of the supplied container node have their mappings
        ///     enabled.  If any child node does not have its mappings enabled, the return value is false.
        /// </summary>
        /// <param name="node">
        ///     a TreeNode from the main control tree, representing a physical control to check whether its output
        ///     mappings are enabled.
        /// </param>
        /// <returns>
        ///     a boolean indicating if the physical control represented by the supplied TreeNode has its output mappings
        ///     enabled.
        /// </returns>
        private bool IsNodeEnabled(TreeNode node)
        {
            if (node == null) return false;

            //obtain the node's .Tag object
            var tag = node.Tag;

            var info = tag as PhysicalControlInfo;
            if (info != null) //if the tag represents a Physical Control
            {
                return _outputMap.IsMappingEnabled(info);
            }
            var device = tag as PhysicalDeviceInfo;
            if (device != null) //if the tag represents a Phyaical Device
            {
                return _outputMap.IsMappingEnabled(device);
            }
            return node.Nodes.Cast<TreeNode>().Any(IsNodeEnabled);
        }

        private static bool IsPPJoyInstalled()
        {
            var c = new Computer();
            using (
                var uninstallKey =
                    c.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"))
            {
                if (uninstallKey == null) return false;
                var subkeys = uninstallKey.GetSubKeyNames();
                if (subkeys.Any(subkeyName => subkeyName.Equals("Parallel Port Joystick", StringComparison.InvariantCultureIgnoreCase) ||
                                              subkeyName.StartsWith("PPJoy Joystick Driver", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Loads an output map from a file and sets the current output map to that newly-loaded map
        /// </summary>
        /// <param name="file">a FileInfo object describing the file containing an output map to load</param>
        /// <returns>a boolean indicating true if the operation was successful or false if it was not</returns>
        private bool LoadMappingsFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            try
            {
                _outputMap = OutputMap.Load(file); //try to load the output map from the file
                _fileName = file.FullName; //store the file name of this file so it can appear in the application title
                _lastNode = null; //clear the last-selected-node reference
            }
            catch (IOException e)
            {
                //if the operation wasn't successful, then alert the user to that fact and quit out of 
                //this method call
                MessageBox.Show(
                    @"An error occurred while loading the file:" + file.FullName + @"\n\nMessage:\n" + e.Message,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (SerializationException se)
            {
                //if the operation wasn't successful, then alert the user to that fact and quit out of 
                //this method call
                MessageBox.Show(
                    @"The file:" + file.FullName + @" does not appear to be a valid " + Application.ProductName +
                    @" file." +
                    @"\n\nMessage:\n" + se.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //assuming we've successfully loaded the file, we need to render the controls it contains
            RenderOutputMap();

            //we also need to render the Mappings tab for the currently-selected node, if it's
            //an editable item
            if (treeMain.SelectedNode?.Tag is PhysicalControlInfo)
            {
                DisplayCurrentMapping((PhysicalControlInfo) treeMain.SelectedNode.Tag);
            }
            //set the application's title to include the file name of the newly-loaded file
            Text = Application.ProductName + @" - " + _fileName;

            //since we just loaded the file, there can't have been any unsaved changes yet
            _hasChanges = false;

            //reset the enabled/disabled menu items states based on what is possible given the 
            //new output map and currently-selected item 
            SetMenuAndToolbarEnabledDisabledStates();


            return true; // that's all folks!  if we're here, all went well!  return TRUE to indicate that
        }

        /// <summary>
        ///     Merges two OutputMap objects together.
        /// </summary>
        /// <param name="sourceMap">The map to merge mappings from.</param>
        /// <param name="targetMap">The map to merge mappings into.</param>
        /// <param name="overrideExistingMappings">
        ///     If true, then existing
        ///     mappings in the target map will be overridden by mappings
        ///     from the source map.  If false, then mappings that already
        ///     exist in the target map will be preserved as-is.
        /// </param>
        private static void MergeMaps(OutputMap sourceMap, OutputMap targetMap, bool overrideExistingMappings)
        {
            if (targetMap == null)
            {
                throw new ArgumentNullException(nameof(targetMap));
            }
            if (sourceMap == null)
            {
                throw new ArgumentNullException(nameof(sourceMap));
            }
            foreach (var physicalControl in sourceMap.PhysicalControls)
                if (targetMap.ContainsMappingFrom(physicalControl)) //target map already contains this mapping
                {
                    if (overrideExistingMappings) //if we're allowed to override the existing mapping in the target map
                    {
                        var toAssign = sourceMap.GetMapping(physicalControl);
                        if (toAssign != null)
                        {
                            targetMap.SetMapping(physicalControl, toAssign); //override the existing mapping


                            //set the enabled/disabled state of this mapping in the target map
                            if (sourceMap.IsMappingEnabled(physicalControl))
                            {
                                targetMap.EnableMapping(physicalControl);
                            }
                            else
                            {
                                targetMap.DisableMapping(physicalControl);
                            }
                        }
                    }
                }
                else //target map does not already contain this mapping
                {
                    var toAssign = sourceMap.GetMapping(physicalControl);
                    if (toAssign != null)
                    {
                        targetMap.SetMapping(physicalControl, toAssign); //override the existing mapping
                        //set the enabled/disabled state of this mapping in the target map
                        if (sourceMap.IsMappingEnabled(physicalControl))
                        {
                            targetMap.EnableMapping(physicalControl);
                        }
                        else
                        {
                            targetMap.DisableMapping(physicalControl);
                        }
                    }
                }
        }

        /// <summary>
        ///     Minimizes the application's primary form to the system tray
        /// </summary>
        private void MinimizeToSystemTray()
        {
            WindowState = FormWindowState.Minimized; //minimize the primary window 
            ShowInTaskbar = false; //make sure it won't show in the taskbar since we want it only in the system tray
            nfyTrayIcon.Visible = true; //show the system tray icon
        }

        /// <summary>
        ///     Event handler for the "Actions" menu's "Create Default Mappings" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuActionsCreateDefaultMappings_Click(object sender, EventArgs e)
        {
            treeMain.SelectedNode = treeMain.Nodes[0];
            //select the root node in the tree (forces pending updates to the output map to commit)
            var hadChanges = _hasChanges; //store the current state of the HasChanges flag
            //get a copy of the current output map
            var mapBefore = (OutputMap) ((ICloneable) _outputMap).Clone();
            //call the logic that creates the default mappings in the output map
            CreateDefaultMappings();
            //get a copy of the output map that exists after creating default mappings
            var mapAfter = (OutputMap) ((ICloneable) _outputMap).Clone();

            //compare the two maps (before and after), and if they're different,
            //raise the HasChanges flag if it isn't raised already.
            _hasChanges = hadChanges | !mapBefore.Equals(mapAfter);

            //render the new output map
            RenderOutputMap();

            //select the root node of the tree
            treeMain.SelectedNode = treeMain.Nodes[0];

            //update menu and toolbar enabled/disabled states
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Event handler for the "Actions" menu's "Start Mapping" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuActionsStartMapping_Click(object sender, EventArgs e)
        {
            StartMapping();
        }

        /// <summary>
        ///     Event handler for the "Actions" menu's "Stop Mapping" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuActionsStopMapping_Click(object sender, EventArgs e)
        {
            StopMapping();
        }

        /// <summary>
        ///     Event handler for the tray icon's context menu's "Exit" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxExit_Click(object sender, EventArgs e)
        {
            FileExit();
        }

        /// <summary>
        ///     Event handler for the tray icon's context menu's "Restore" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxRestore_Click(object sender, EventArgs e)
        {
            //set the main form to the Normal size state.  This will cause
            //the SizeChanged event handler to run, which, in turn, will call
            //RestoreFromSystemTray().
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        ///     Event handler for the tray icon's context menu's "Start Mapping" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxStartMapping_Click(object sender, EventArgs e)
        {
            StartMapping();
        }

        /// <summary>
        ///     Event handler for the tray icon's context menu's "Stop Mapping" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxStopMapping_Click(object sender, EventArgs e)
        {
            StopMapping();
        }

        /// <summary>
        ///     Event handler for the TreeView's context menu's "Disable" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxTreeItemsDisable_Click(object sender, EventArgs e)
        {
            DisableNode(treeMain.SelectedNode);
        }

        /// <summary>
        ///     Event handler for the TreeView's context menu's "Enable" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxTreeItemsEnable_Click(object sender, EventArgs e)
        {
            EnableNode(treeMain.SelectedNode);
        }

        /// <summary>
        ///     Event handler for the TreeView's context menu's "Remove" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuCtxTreeItemsRemove_Click(object sender, EventArgs e)
        {
            RemoveCurrentItem();
        }

        /// <summary>
        ///     Event handler for the "File" menu's "Exit" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            FileExit();
        }

        private void mnuFileImport_Click(object sender, EventArgs e)
        {
            //prompt user to save any unsaved changes to the current output map
            var shouldContinue = PromptForSaveChanges();
            if (!shouldContinue)
            {
                return; //user cancelled the operaton
            }

            //store flag indicating if current output map has unsaved changes
            var currentlyHasChanges = _hasChanges;

            //store the current output map
            var oldMap = (OutputMap) ((ICloneable) _outputMap).Clone();

            _hasChanges = false;
            //detect and store output map of currently-detected devices
            shouldContinue = NewFile();
            if (!shouldContinue)
            {
                return;
            }
            var currentDevicesAndControls = _outputMap;

            //prompt the user for which file to import and load that file
            shouldContinue = OpenFile();
            if (!shouldContinue)
            {
                return;
            }
            if (_lastOpenCancelled)
            {
                return;
            }

            //merge the newly-loaded output map with original output map
            MergeMaps(oldMap, _outputMap, true);

            //disable any devices that aren't attached
            foreach (var device in _outputMap.EnabledPhysicalDevices)
            {
                var isAttached = false;
                foreach (var attachedDevice in currentDevicesAndControls.EnabledPhysicalDevices)
                    if (attachedDevice.Equals(device))
                    {
                        isAttached = true;
                        break;
                    }
                if (!isAttached)
                {
                    _outputMap.DisableMapping(device);
                }
            }

            _hasChanges = !oldMap.Equals(_outputMap) || currentlyHasChanges;

            //render the output map
            RenderOutputMap();
        }

        /// <summary>
        ///     Event handler for the "File" menu's "New" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        /// <summary>
        ///     Event handler for the "File" menu's "Open" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        /// <summary>
        ///     Event handler for the "File" menu's "Save" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        ///     Event handler for the "File" menu's "Save As" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        /// <summary>
        ///     Event handler for the "Help" menu's "About" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            ShowHelpAbout();
        }

        private void mnuPPJoy_DropDownOpening(object sender, EventArgs e)
        {
            var numSticksDefined = Util.CountPPJoyVirtualDevices();
            var maxAllowed = Util.GetMaxPPJoyVirtualDevicesAllowed();

            mnuPPJoyCreateNewVirtualDevices.Enabled = numSticksDefined < maxAllowed;

            if (numSticksDefined == 0)
            {
                mnuPPJoyRemoveAllPPJoyVirtualSticks.Enabled = false;
                mnuPPJoyAssignMaximumCapabilities.Enabled = false;
            }
            else
            {
                mnuPPJoyRemoveAllPPJoyVirtualSticks.Enabled = true;
                mnuPPJoyAssignMaximumCapabilities.Enabled = true;
            }
        }

        /// <summary>
        ///     Event handler for the "PPJoy" menu's "Assign maximum capabilities to all PPJoy Virtual Joystick Devices" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuPPJoyAssignMaximumCapabilities_Click(object sender, EventArgs e)
        {
            AssignMaximumCapabilitiesToAllPPJoyVirtualDevices();
        }

        /// <summary>
        ///     Event handler for the "PPJoy" menu's "Create new PPJoy Virtual Joystick Devices" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuPPJoyCreateNewVirtualJoystickDevices_Click(object sender, EventArgs e)
        {
            CreateNewPPJoyVirtualJoystickDevices();
        }

        /// <summary>
        ///     Event handler for the "Tools" menu's "Open PPJoy Control Panel" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private static void mnuPPJoyOpenPPJoyControlPanel_Click(object sender, EventArgs e)
        {
            //display the PPJoy control panel
            OpenPPJoyControlPanel();
        }

        /// <summary>
        ///     Event handler for the "PPJoy" menu's "Remove All PPJoy Virtual Joysticks" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuPPJoyRemoveAllPPJoyVirtualSticks_Click(object sender, EventArgs e)
        {
            RemoveAllPPJoyVirtualSticks();
        }

        /// <summary>
        ///     Event handler for the "Tools" menu's "Open Windows Joystick Control Panel" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private static void mnuToolsOpenWindowsJoystickControlPanel_Click(object sender, EventArgs e)
        {
            //display the Windows Gaming Devices control panel
            OpenWindowsJoystickControlPanel();
        }

        /// <summary>
        ///     Event handler for the "Tools" menu's "Options" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }

        /// <summary>
        ///     Event handler for the "View" menu's "Refresh Device List" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Click event.</param>
        private void mnuViewRefreshDeviceList_Click(object sender, EventArgs e)
        {
            RefreshDeviceList();
        }

        /// <summary>
        ///     Implements the behavior for the File-New menu command
        /// </summary>
        /// <returns>a boolean indicating if the operation completed successfully.</returns>
        private bool NewFile()
        {
            var shouldContinue = PromptForSaveChanges();
            if (!shouldContinue)
            {
                return false;
            }

            _lastNode = null; //clear the last-selected-node reference
            CreateNewOutputMap(); //create a new Output Map at the form level
            _fileName = null; //set the file name for this new output map to NULL (Untitled)
            Text = Application.ProductName + @" - " + @"Untitled"; //set the form's window title appropriately
            RenderOutputMap(); //render the new output map to the control tree
            treeMain.SelectedNode = treeMain.Nodes[0]; //set the current node in the control tree to the root node
            _hasChanges = false; //reset the unsaved-changes flag since no changes can possibly have occurred yet

            //update the possible menu enabled/disabled states based on what's possible now, given that our
            //new output map is in place
            SetMenuAndToolbarEnabledDisabledStates();
            return true;
        }

        private void nfyTrayIcon_Click(object sender, EventArgs e)
        {
            var args = e as MouseEventArgs;
            if (args?.Button == MouseButtons.Left)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        ///     Opens a saved mapping file
        /// </summary>
        /// <returns>a boolean indicating if the operation completed successfully.</returns>
        private bool OpenFile()
        {
            _lastOpenCancelled = false;
            //prompt the user to save unsaved changes
            var shouldContinue = PromptForSaveChanges();
            if (!shouldContinue)
            {
                _lastOpenCancelled = true;
                return false;
            }
            //if the user didn't cancel the save operation, then we can
            //go ahead and display the FileOpen dialog
            ShowOpenFileDialog();
            return true;
        }

        /// <summary>
        ///     Displays the PPJoy control panel
        /// </summary>
        private static void OpenPPJoyControlPanel()
        {
            var proc = new Process();
            proc.EnableRaisingEvents = false;
            var toStart = GetPPJoyControlPanelFilePath() ?? "PPortJoy.cpl";
            var fi = new FileInfo(toStart);
            if (!fi.Exists && !toStart.Equals("PPortJoy.cpl", StringComparison.InvariantCultureIgnoreCase)) return;
            try
            {
                proc.StartInfo.FileName = toStart;
                proc.Start();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    @"An error occurred while attempting to launch the PPJoy control panel:\n The path {toStart} could not be found.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        ///     Displays the Windows "Gaming Devices" control panel.
        /// </summary>
        private static void OpenWindowsJoystickControlPanel()
        {
            var proc = new Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "joy.cpl";
            proc.Start();
        }

        /// <summary>
        ///     Populates the combobox of selectable Virtual Controls that a given physical control can be
        ///     mapped to, taking into account mappings that are already being used
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object representing the physical control whose possible mappings will
        ///     be populated in the Mappings tab's controls
        /// </param>
        /// <param name="virtualDevice">
        ///     a VirtualDeviceInfo object representing the currently-selected Virtual Device in the
        ///     Mappings tab's virtual device selector combobox
        /// </param>
        private void PopulateSelectableOutputControls(PhysicalControlInfo inputControl, VirtualDeviceInfo virtualDevice)
        {
            //populate the combo boxes in the mappings tab with their initial values
            var virtualControls = new List<string>();
            //list of strings that will be associated with the output control combobox
            virtualControls.Add("<unmapped>"); //string representing the unmapped state

            //if the input control is an axis or Pov control, then the set of
            //selectable output controls consists of the set of unmapped axes data sources on the supplied virtual
            //joystick device.
            if (inputControl.ControlType == ControlType.Axis || inputControl.ControlType == ControlType.Pov)
            {
                for (var i = 0; i < VirtualJoystick.MaxAnalogDataSources; i++)
                {
                    var virtualControl = new VirtualControlInfo(virtualDevice, ControlType.Axis, i);
                    var axisIsMapped = false;

                    //for the current axis being evaluated, check to see if a mapping already exists to that axis.
                    //If it does, then we can't add it to the list of selectable mappings, as that would
                    //allow duplicate mappings to exist.
                    if (_outputMap.ContainsMappingTo(virtualControl))
                    {
                        if (_outputMap.ContainsMappingTo(virtualControl))
                        {
                            var thisInputControlMapping = _outputMap.GetMapping(inputControl);
                            if (thisInputControlMapping == null || !thisInputControlMapping.Equals(virtualControl))
                            {
                                axisIsMapped = true;
                            }
                        }
                    }

                    //if the axis is not already mapped, we can add it to the list of selectable mappings
                    if (!axisIsMapped)
                    {
                        virtualControls.Add("Analog" + i);
                    }
                } //end for
            } //end if
            //otherwise, if the supplied input control is a Button control, then the 
            //set of allowable mappings consists of the set of unmapped button data sources on the
            //supplied virtual device.
            else if (inputControl.ControlType == ControlType.Button)
            {
                //for each button data source on the supplied virtual device, check for
                //an existing mapping to that data source, and if none exists, add
                //that button data source to the list of selectable output mappings
                for (var i = 0; i < VirtualJoystick.MaxDigitalDataSources; i++)
                {
                    var virtualControl = new VirtualControlInfo(virtualDevice, ControlType.Button, i);
                    var buttonIsMapped = false;
                    //check for an existing mapping
                    if (_outputMap.ContainsMappingTo(virtualControl) && _outputMap.ContainsMappingTo(virtualControl))
                    {
                        var thisInputControlMapping = _outputMap.GetMapping(inputControl);
                        if (thisInputControlMapping == null || !thisInputControlMapping.Equals(virtualControl))
                        {
                            buttonIsMapped = true;
                        }
                    }

                    //if no existing mapping is found, then this virtual data source is selectable as an output mapping
                    if (!buttonIsMapped)
                    {
                        virtualControls.Add("Digital" + i);
                    }
                } //end for
            }

            //sort the list of selectable mappings
            virtualControls.Sort(new NumericComparer().Compare);

            //now bind the combobox's internal list to this list of selectable mappings
            cboVirtualControl.DataSource = virtualControls;
        }

        /// <summary>
        ///     Populates the Virtual Device combobox
        /// </summary>
        private void PopulateVirtualDevicesList()
        {
            var ppJoyDevices = new DeviceManager().GetAllDevices();

            var virtualDeviceNames = (from t in ppJoyDevices where t.DeviceType == JoystickTypes.Virtual_Joystick select "PPJoy Virtual joystick " + (t.UnitNum + 1)).ToList();
            virtualDeviceNames.Add("<unmapped>");
            virtualDeviceNames.Sort(new NumericComparer().Compare);
            cboVirtualDevice.DataSource = virtualDeviceNames;
        }

        private bool PromptForSaveChanges()
        {
            return PromptForSaveChanges("Do you want to save changes?");
        }

        /// <summary>
        ///     Prompts the user to save pending changes, if there are any.  If the user responds "Yes", then
        ///     the Save File dialog is presented.
        /// </summary>
        /// <returns>
        ///     True if the operation requesting the prompt should continue, or false if the user cancelled the save
        ///     operation.
        /// </returns>
        private bool PromptForSaveChanges(string promptText)
        {
            var shouldContinue = false; //flag for holding the results of the user prompt to save changes

            //set the selected node to the root of the tree (forces any pending mapping changes to be captured
            //because capturing and storing changes occurs whenever the selected node changes)
            treeMain.SelectedNode = treeMain.Nodes[0];

            if (_hasChanges) //if there are unsaved changes in the output map
            {
                //prompt the user to save those unsaved changes
                var promptForSave = MessageBox.Show(promptText, Application.ProductName,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                switch (promptForSave)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.No:
                        //if the user said no to saving the unsaved changes, it's still ok to proceed
                        shouldContinue = true; //set the continue flag accordingly
                        break;
                    case DialogResult.Yes:
                        //if the user said yes to saving the changes, then we need to 
                        //display the appropriate File-Save Common Dialog.
                        SaveFile();

                        shouldContinue |= !_lastSaveCancelled;
                        break;
                }
            } //end if (_hasChanges)
            else
            {
                //if we're here, there must not have been any unsaved changes, so there was
                //no need to prompt the user before disarding any information during the File-New operation
                //and hence, we're ok to continue
                shouldContinue = true; //set the flag accordingly
            }
            return shouldContinue;
        }

        /// <summary>
        ///     Re-detects known devices on this system, and refreshes the list of devices visible in the treeview.
        /// </summary>
        private void RefreshDeviceList()
        {
            var changesDetected = false; //flag which is raised if changes to the current device set are detected
            treeMain.SelectedNode = treeMain.Nodes[0];
            var oldMap = (OutputMap) ((ICloneable) _outputMap).Clone();

            //create a list to store all detected controls across all known input devices
            var detectedInputControls = new List<PhysicalControlInfo>();

            //detect all known DirectInput joysticks on this system
            IList<DeviceInstance> detectedJoysticks = null;
            using (var directInput = new DirectInput())
            {
                detectedJoysticks = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AllDevices);
            }
            if (detectedJoysticks != null)
            {
                //check each known joystick device to see if it's a virtual joystick or a 
                //physical joystick.  If it's physical, enumerate its controls.
                foreach (DeviceInstance instance in detectedJoysticks)
                {
                    var deviceInfo = new DIPhysicalDeviceInfo(instance.InstanceGuid, instance.InstanceName);
                    var device = DIDeviceMonitor.GetInstance(deviceInfo, this,
                        VirtualJoystick.MinAnalogDataSourceVal,
                        VirtualJoystick.MaxAnalogDataSourceVal);


                    //get the vendor ID and product ID of the current device
                    var vendorIdentityProductId = device.VendorIdentityProductId.HasValue ? device.VendorIdentityProductId.Value: 0;
                    //use the vendor ID/product ID to determine if the device is virtual or physical
                    var isVirtual = false;
                    try
                    {
                        isVirtual = new DeviceManager().IsVirtualDevice(vendorIdentityProductId);
                    }
                    catch (DeviceNotFoundException e)
                    {
                        _log.Debug(e.Message, e);
                    }
                    if (!isVirtual)
                    {
                        //create a PhysicalDeviceInfo object to represent the current device
                        var thisDev = new DIPhysicalDeviceInfo(instance.InstanceGuid, instance.InstanceName);
                        //if the device is physical, then create a monitor for it if it's not already created
                        DIDeviceMonitor.GetInstance(thisDev, this,
                            VirtualJoystick.MinAnalogDataSourceVal,
                            VirtualJoystick.MaxAnalogDataSourceVal);
                        //obtain a list of the device's controls
                        foreach (var pci in thisDev.Controls)
                        {
                            //add the current control to the master list of controls
                            detectedInputControls.Add(pci);
                            if (!_outputMap.ContainsMappingFrom(pci))
                            {
                                //the output map does not contain a mapping from the current control, then add
                                //the control to the output map but don't associate it with any particular output.
                                //This, in and of itself, represents a change to the output map, as we've added an unknown control
                                //to the map.
                                changesDetected = true;
                                _outputMap.SetMapping(pci, null);
                                _outputMap.EnableMapping(pci);
                            }
                            else
                            {
                                if (oldMap.ContainsMappingFrom(pci))
                                {
                                    if (oldMap.IsMappingEnabled(pci))
                                    {
                                        _outputMap.EnableMapping(pci);
                                    }
                                    else
                                    {
                                        _outputMap.DisableMapping(pci);
                                    }
                                }
                            } //end if
                        } //end foreach
                    }
                    else
                    {
                        device.Dispose();
                    } //end if
                } //end foreach
            }

            //detect all BetaInnovations HID input devices on this system
            var manager = BIDeviceManager.GetInstance();
            BIPhysicalDeviceInfo[] biDevices = null;
            try
            {
                biDevices = manager.GetDevices(false);
            }
            catch (OperationFailedException e)
            {
                _log.Debug(e.Message, e);
            }

            if (biDevices != null)
            {
                //for each BetaInnovations device detected, enumerate its controls.
                foreach (var biDevice in biDevices)
                {
                    //create a monitor for the device if one has not already been created
                    BIDeviceMonitor.GetInstance(biDevice);
                    foreach (var pci in biDevice.Controls)
                    {
                        //add the current control to the master list of controls
                        detectedInputControls.Add(pci);
                        if (!_outputMap.ContainsMappingFrom(pci))
                        {
                            //the output map does not contain a mapping from the current control, then add
                            //the control to the output map but don't associate it with any particular output.
                            //This, in and of itself, represents a change to the output map, as we've added an unknown control
                            //to the map.
                            changesDetected = true;
                            _outputMap.SetMapping(pci, null);
                            _outputMap.EnableMapping(pci);
                        }
                        else
                        {
                            if (!oldMap.ContainsMappingFrom(pci)) continue;
                            if (oldMap.IsMappingEnabled(pci))
                            {
                                _outputMap.EnableMapping(pci);
                            }
                            else
                            {
                                _outputMap.DisableMapping(pci);
                            }
                        } //end if
                    } //end foreach
                } //end foreach
            }
            Mediator.ResetMonitors();
            PHCCPhysicalDeviceInfo[] phccDevices = null;
            try
            {
                phccDevices = PHCCDeviceManager.GetDevices();
            }
            catch (ApplicationException e)
            {
                _log.Debug(e.Message, e);
            }

            if (phccDevices != null)
            {
                //for each PHCC device detected, enumerate its controls.
                foreach (var phccDevice in phccDevices)
                {
                    //create a monitor for the device if one has not already been created
                    PHCCDeviceMonitor.GetInstance(phccDevice,
                        VirtualJoystick.MinAnalogDataSourceVal,
                        VirtualJoystick.MaxAnalogDataSourceVal);
                    foreach (var pci in phccDevice.Controls)
                    {
                        //add the current control to the master list of controls
                        detectedInputControls.Add(pci);
                        if (!_outputMap.ContainsMappingFrom(pci))
                        {
                            //the output map does not contain a mapping from the current control, then add
                            //the control to the output map but don't associate it with any particular output.
                            //This, in and of itself, represents a change to the output map, as we've added an unknown control
                            //to the map.
                            changesDetected = true;
                            _outputMap.SetMapping(pci, null);
                            _outputMap.EnableMapping(pci);
                        }
                        else
                        {
                            if (!oldMap.ContainsMappingFrom(pci)) continue;
                            if (oldMap.IsMappingEnabled(pci))
                            {
                                _outputMap.EnableMapping(pci);
                            }
                            else
                            {
                                _outputMap.DisableMapping(pci);
                            }
                        } //end if
                    } //end foreach
                } //end foreach
            }


            //now, compare the list of controls in the current output map to
            //the list of controls we just detected.  If there's an item
            //in the old list that doesn't appear in the new list,
            //we need to disable it in the output map
            foreach (var pci in _outputMap.PhysicalControls)
                //if this control from the output map isn't in the list of
                //detected controls, then it's no longer detected
                //and should be disabled in output map
                if (!detectedInputControls.Contains(pci))
                {
                    changesDetected = true; //this represents a change to the output map
                    _outputMap.DisableMapping(pci);
                }
            //update the HasChanges flag so we can keep the form's titlebar's Asterisk indicator in sync
            _hasChanges |= changesDetected;

            //the output map has changed, so we'll re-render it
            RenderOutputMap();

            //since the output map has changed, we should now update the selectable menu items/toolbar states as well
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Removes all PPJoy virtual joysticks from PPJoy.
        /// </summary>
        private void RemoveAllPPJoyVirtualSticks()
        {
            StopMediatingAndDisposeMediator();

            try
            {
                var devices = new DeviceManager().GetAllDevices();
                foreach (var dev in devices) //for each device in the array
                {
                    Application.DoEvents();
                    var device = dev;

                    if (device.DeviceType == JoystickTypes.Virtual_Joystick)
                        //if the device is a Virtual Joystick device, then
                    {
                        var unitNum = device.UnitNum;
                        var numTries = 0;

                        //enter a loop wherein we try to delete the device until we're successful
                        //or until we fail a certain number of times.  
                        while (device != null && numTries < 50)
                        {
                            Application.DoEvents();

                            try
                            {
                                //try to delete the device
                                device.Delete(false, true);
                            }
                            catch (PPJoyException e)
                            {
                                _log.Debug(e.Message, e);
                            }

                            try
                            {
                                //see if the device is still there
                                device = new DeviceManager().GetDevice(0, unitNum);
                            }
                            catch (PPJoyException e)
                            {
                                _log.Debug(e.Message, e);
                            }

                            numTries++; //increment the attempts counter
                        } //end while

                        try
                        {
                            //try to delete the device
                            device?.Delete(true, false);
                            for (var i = 0; i < 300; i++)
                            {
                                Thread.Sleep(10);
                                Application.DoEvents();
                            }
                        }
                        catch (Exception e)
                        {
                            _log.Debug(e.Message, e);
                        }
                    } //end if
                } //end foreach
            } //end try
            catch (PPJoyException e) //over-arching catch statement to prevent errors herein from bubbling up
            {
                _log.Debug(e.Message, e);
            }
            CreateMediatorAndUpdateMediatorMap();
        }

        /// <summary>
        ///     Removes the currently-selected node from the treeview, and removes
        ///     the corresponding mappings from the output map.
        /// </summary>
        private void RemoveCurrentItem()
        {
            var curNode = treeMain.SelectedNode;
            var parentNode = treeMain.SelectedNode.Parent;
            treeMain.SelectedNode = treeMain.Nodes[0];

            //get a copy of the output map as it exists before we make any changes to it
            var mapBefore = (OutputMap) ((ICloneable) _outputMap).Clone();

            if (parentNode == null) parentNode = curNode;

            if (curNode != null)
            {
                //if the current node is the "Axes" container node, then remove all mappings from
                //all child nodes 
                if (curNode.Text.StartsWith("Axes", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (TreeNode controlNode in curNode.Nodes)
                        _outputMap.RemoveMapping((PhysicalControlInfo) controlNode.Tag);
                }
                //if the current node is the "Buttons" container node, then remove all mappings from
                //all child nodes 
                else if (curNode.Text.StartsWith("Buttons", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (TreeNode controlNode in curNode.Nodes)
                        _outputMap.RemoveMapping((PhysicalControlInfo) controlNode.Tag);
                }
                //if the current node is the "Povs" container node, then remove all mappings from
                //all child nodes 
                else if (curNode.Text.StartsWith("Povs", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (TreeNode controlNode in curNode.Nodes)
                        _outputMap.RemoveMapping((PhysicalControlInfo) controlNode.Tag);
                }
                //if the current node is the "Local Devices" container node, then remove all mappings from
                //all child nodes 
                else if (curNode.Text.StartsWith("Local Devices", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (TreeNode deviceNode in curNode.Nodes)
                        _outputMap.RemoveMapping((PhysicalDeviceInfo) deviceNode.Tag);
                }
                //if the current node is a leaf node, then convert the node's .Tag object
                //to the appropriate object type and remove the mapping from the output map
                else if (curNode.Tag != null)
                {
                    var tag = curNode.Tag as PhysicalDeviceInfo;
                    if (tag != null)
                    {
                        _outputMap.RemoveMapping(tag);
                    }
                    else if (curNode.Tag is PhysicalControlInfo)
                    {
                        _outputMap.RemoveMapping((PhysicalControlInfo) curNode.Tag);
                    }
                }
            }

            //make a copy of the output map after the changes have been made
            var mapAfter = (OutputMap) ((ICloneable) _outputMap).Clone();

            //compare the output map after changes to the output map before changes, and if the two are different,
            //then update the global HasChanges flag 
            _hasChanges |= !mapBefore.Equals(mapAfter);

            //render the new output map
            RenderOutputMap();

            //select the node's parent in the treeview
            parentNode.EnsureVisible();
            treeMain.SelectedNode = parentNode;

            //update menu and toolbar states since the output map has changed
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Renders the currently loaded output map as a set of nodes in a treeview
        /// </summary>
        private void RenderOutputMap()
        {
            UpdateMediatorMap();
            //update the mediator with the current output map so that mediation behavior matches what's being displayed

            //clear all device nodes and their children from the treeview 
            var nodeLocalDevs = treeMain.Nodes["nodeLocalDevices"];
            nodeLocalDevs.Nodes.Clear();

            //add each physical device and its constituent controls to the treeview
            foreach (var pdi in _outputMap.PhysicalDevices)
            {
                var numAxes = 0;
                var numButtons = 0;
                var numPovs = 0;

                //add a node for the current physical device
                var thisInstanceNode = nodeLocalDevs.Nodes.Add(pdi.Key.ToString(), pdi.Alias, IMAGE_INDEX_JOYSTICK,
                    IMAGE_INDEX_JOYSTICK);
                //store the PhysicalDeviceInfo object representing the current physical device, in the .Tag property of the treenode so we can easily retrieve it later
                thisInstanceNode.Tag = pdi;

                //create nodes for axes, buttons, and Povs that may exist on this physical device
                var thisInstanceAxesNode = thisInstanceNode.Nodes.Add("AXES", "Axes", IMAGE_INDEX_AXIS,
                    IMAGE_INDEX_AXIS);
                var thisInstanceButtonsNode = thisInstanceNode.Nodes.Add("BUTTONS", "Buttons", IMAGE_INDEX_BUTTON,
                    IMAGE_INDEX_BUTTON);
                var thisInstancePovsNode = thisInstanceNode.Nodes.Add("Povs", "Povs", IMAGE_INDEX_Pov,
                    IMAGE_INDEX_Pov);

                //for each control on this physical device, add a corresponding treeview node representing that control
                foreach (var pci in _outputMap.GetDeviceSpecificMappings(pdi).Keys)
                {
                    TreeNode newNode;
                    switch (pci.ControlType)
                    {
                        case ControlType.Axis:
                            numAxes++;
                            var axisName = GetControlDisplayText(pci);
                            //create a new treeview node to represent the current axis control, using the 
                            //appropriate axis image
                            newNode = thisInstanceAxesNode.Nodes.Add(pci.ToString(),
                                axisName, IMAGE_INDEX_AXIS, IMAGE_INDEX_AXIS);
                            //store a reference to the PhysicalControlInfo object that represents the current axis control, in the .Tag property of the treenode, for easy retrieval later
                            newNode.Tag = pci;
                            break;
                        case ControlType.Pov:
                            numPovs++;
                            var povName = GetControlDisplayText(pci);
                            //create a new Pov node for the current control, using a one-based counter for the Pov name, and using the appropriate Pov image
                            newNode = thisInstancePovsNode.Nodes.Add(pci.ToString(), povName, IMAGE_INDEX_Pov,
                                IMAGE_INDEX_Pov);
                            //store a reference to the PhysicalControlInfo object that represents the current Pov control, in the .Tag property of the treenode, for easy retrieval later
                            newNode.Tag = pci;
                            break;
                        default:
                            numButtons++;
                            var buttonName = GetControlDisplayText(pci);
                            //create a new Button node for the current control, using a one-based counter for the Button name, and using the appropriate Button image
                            newNode = thisInstanceButtonsNode.Nodes.Add(pci.ToString(), buttonName, IMAGE_INDEX_BUTTON,
                                IMAGE_INDEX_BUTTON);
                            //store a reference to the PhysicalControlInfo object that represents the current Button control, in the .Tag property of the treenode, for easy retrieval later
                            newNode.Tag = pci;
                            break;
                    }

                    //if mapping has been disabled for the current control, then update
                    //the images and greyout state for the current control node
                    if (!_outputMap.IsMappingEnabled(pci))
                    {
                        DisableControlNode(newNode);
                    }
                } //end foreach physicalcontrolinfo
                //if mapping has been disabled for the current input device, then update
                //the images and greyout state for the current device node
                if (!_outputMap.IsMappingEnabled(pdi))
                {
                    DisableDeviceNode(thisInstanceNode);
                }
                if (!IsNodeEnabled(thisInstanceAxesNode))
                {
                    DisableContainerNode(thisInstanceAxesNode);
                }
                if (!IsNodeEnabled(thisInstanceButtonsNode))
                {
                    DisableContainerNode(thisInstanceButtonsNode);
                }
                if (!IsNodeEnabled(thisInstancePovsNode))
                {
                    DisableContainerNode(thisInstancePovsNode);
                }

                if (numAxes == 0)
                {
                    thisInstanceAxesNode.Remove();
                }
                if (numPovs == 0)
                {
                    thisInstancePovsNode.Remove();
                }
                if (numButtons == 0)
                {
                    thisInstanceButtonsNode.Remove();
                }
            } //end foreach physicaldeviceinfo
            //sort the treeview nodes
            treeMain.TreeViewNodeSorter = new TreeNodeNumericComparer();
            treeMain.Sort();

            //ensure the root node is selected and only the device nodes are visible initially
            treeMain.SelectedNode = treeMain.Nodes[0];
            treeMain.CollapseAll();
            treeMain.SelectedNode = treeMain.Nodes[0];
            treeMain.SelectedNode.Expand();
            treeMain.Select();
        }

        /// <summary>
        ///     Restores the main editor window from its minimized-to-tray state
        /// </summary>
        private void RestoreFromSystemTray()
        {
            nfyTrayIcon.Visible = false; //hide the tray icon when we're in this state
            ShowInTaskbar = true; //show a button in the main taskbar when this form is in the Normal size state
            Activate(); //make the main form the current window
        }

        /// <summary>
        ///     Implements the File-Save and File-SaveAs menu item behavior
        /// </summary>
        private void SaveFile()
        {
            //if the current output map is unnamed, then this is the first
            //time we've saved the file, so use the SaveAs funtionality, which
            //will prompt the user for a file name and path.
            if (_fileName == null)
            {
                SaveFileAs();
            }
            else //otherwise, we already know what filepath this map should be saved to
            {
                //Convert the stored mapping file name to a FileInfo object
                var file = new FileInfo(_fileName);

                //save the current output map to that file
                SaveMappingsFile(file);
            }
        }

        /// <summary>
        ///     Displays the File-SaveAs dialog box
        /// </summary>
        private void SaveFileAs()
        {
            //capture any uncommitted changes
            CheckAndStoreCurrentMapping();

            _lastSaveCancelled = false; //clear the last-save-was-cancelled flag 

            //prepare the SaveAs common dialog
            _dlgSave.Reset(); //reset the common dialog to its defaults
            _dlgSave.OverwritePrompt = true; //prompt the user to overwrite existing files
            _dlgSave.ValidateNames = true; //make sure the user provides a valid file name

            //get the application's path to use as a default path
            var appPath = Path.GetFullPath(Application.ExecutablePath);

            _dlgSave.InitialDirectory = appPath;
            //set the initial directory for the save dialog to the application's directory
            _dlgSave.CheckPathExists = true; //make sure the user chooses a path that actually exists
            _dlgSave.AddExtension = true;
            //automatically add the .map file extension if the user doesn't explicitly give it
            _dlgSave.CreatePrompt = false;
            //don't prompt the user to create a file that doesn't exist -- that's the whole point here
            _dlgSave.DefaultExt = ".map"; //set the default file extension to use
            _dlgSave.DereferenceLinks = true;
            //allows the SaveAs dialog to navigate softlinks in the file system by providing the full path back to the application instead of the path via softlink
            _dlgSave.Filter = @"JoyMapper files (*.map)|*.map|All files(*.*)|*.*"; //set the possible filter values
            _dlgSave.FilterIndex = 0; //use *.map as the default filter
            _dlgSave.ShowHelp = false; //don't show the help functionality since we haven't implemented any
            _dlgSave.SupportMultiDottedExtensions = true; //allow files with names like a.b.c.d.map
            _dlgSave.Title = @"Save As"; //set the dialog's title
            _dlgSave.ShowDialog(this);
            //show the dialog -- an event handler callback will be called when the user dismisses the dialog box either through cancelling or though success
        }

        /// <summary>
        ///     Saves the current output map to a file
        /// </summary>
        /// <param name="file">a FileInfo object representing the file path where the current output map will be saved</param>
        private void SaveMappingsFile(FileInfo file)
        {
            //store a reference to the currently-selected node so we can re-select it after saving
            var currentNode = treeMain.SelectedNode;

            //select the root node of the treeview, causing any pending updates 
            //to be committed to the underlying Output Map
            treeMain.SelectedNode = treeMain.Nodes[0];

            //try to save the output map to the specified file
            try
            {
                OutputMap.Save(file, _outputMap);
            }
            catch (IOException e)
            {
                //if anything went wrong during the save process, notify the user, and return a value of false, indicating an error occurred.
                MessageBox.Show(@"An error occurred while saving the mappings file:\n" + e.Message,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //if the save was successful, update the titlebar with the new file name.  
            //Note that the changes-pending asterisk will not be required,
            //since we've just committed any pending changes to disk.
            Text = Application.ProductName + @" - " + file.FullName;

            //reset the changes-pending flag since we've just committed all pending changes
            _hasChanges = false;

            //re-select the node that was selected before we saved the file
            treeMain.SelectedNode = currentNode;

            //update the states of menu and toolbar buttons 
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Determines the valid states for all menu and toolbar items and sets their .Enabled
        ///     property to allow/prevent selection of each such item
        /// </summary>
        private void SetMenuAndToolbarEnabledDisabledStates()
        {
            if (_outputMap != null && _outputMap.ContainsValidMappings())
            {
                //there's a valid mapping set and it contains items,
                //so mapping is possible to start
                if (_mediator != null && _mediator.SendOutput)
                {
                    //there's already a mediator and it's running, 
                    //so we can't enable starting the mediator
                    //so in this case, we'll disable starting and enable stopping
                    DisableStartMappingFunctionality();
                    EnableStopMappingFunctionality();
                }
                else
                {
                    //there's either no mediator, or there is a mediator,
                    //but it's not running, so we can enable starting the mediator
                    EnableStartMappingFunctionality();
                    DisableStopMappingFunctionality();
                }
            }
            else
            {
                //There's no valid output mappings, 
                //so no mediation is possible.  
                DisableStartMappingFunctionality();

                if (_mediator != null && _mediator.SendOutput)
                {
                    //there is a mediator and the mediator is running, so we have to 
                    //enable stopping it
                    EnableStopMappingFunctionality();
                }
                else
                {
                    //if there's no mediator, or there's a non-running mediator,
                    //we can disable stopping it (it's already stopped)
                    DisableStopMappingFunctionality();
                }
            }
            //if the mediator is currently sending output to PPJoy, then
            if (_mediator != null && _mediator.SendOutput)
            {
                //don't ask the mediator to raise "change notification" events -- we don't
                //want to sacrifice performance by processing those while active mediation is occuring
                try
                {
                    _mediator.RaiseEvents = false;
                }
                catch (NullReferenceException e)
                {
                    _log.Debug(e.Message, e);
                }
                //disable menu items that should not be enabled when active mediation is occurring
                DisableMediationSensitiveMenuFunctionality();
                DisablePPJoyMenuFunctionality(); //we shouldn't change PPJoy stuff while mediation's running
                DisableFileNewFunctionality(); //we don't want to lose the existing output map while mediation's running
                DisableFileImportFunctionality();
                DisableFileOpenFunctionality(); //we don't want to open a new output map while mediation's running
                DisableCreateDefaultMapping();
                //we don't want to allow changes to the output map while mediation's running
                treeMain.Enabled = false;
                //again, we don't want to allow changes to the output map while mediation's running

                DisableAutoHighlightingFunctionality();
            }
            else //the mediator is not currently sending output to PPJoy, so
            {
                if (_mediator != null)
                {
                    try
                    {
                        _mediator.RaiseEvents = true; //we can track change events
                    }
                    catch (NullReferenceException e)
                    {
                        _log.Debug(e.Message, e);
                    }
                }
                EnableMediationSensitiveMenuFunctionality();
                EnableToolsMenuFunctionality();
                EnablePPJoyMenuFunctionality(); //are basically re-enabled here
                EnableFileNewFunctionality();
                EnableFileImportFunctionality();
                EnableFileOpenFunctionality();
                EnableCreateDefaultMapping();
                treeMain.Enabled = true;
                EnableAutoHighlightingFunctionality();
            }

            if (treeMain.SelectedNode != null) //if there's a selected node in the treeview
            {
                if (IsNodeEnabled(treeMain.SelectedNode)) //if the node is currently enabled
                {
                    if (!AreAnyChildNodesDisabled(treeMain.SelectedNode)) //no child nodes are disabled
                    {
                        DisableEnableItemFunctionality(); //disable the "Enable" menu items
                    }
                    else //at least one child node is disabled
                    {
                        EnableEnableItemFunctionality();
                    }
                    EnableDisableItemFunctionality(); //enable the "Disable" menu items
                }
                else //(the node is (or all of its children are) currently Disabled)
                {
                    EnableEnableItemFunctionality(); //enable the "Enable" menu items
                    DisableDisableItemFunctionality(); //disable the "Disable" menu items
                } //end if
                DisableRemoveItemFunctionality();
                if (treeMain.SelectedNode.Nodes.Count > 0) //if the node has children
                {
                    if (treeMain.SelectedNode.Tag is PhysicalDeviceInfo)
                    {
                        EnableRemoveItemFunctionality(); //enable the "Remove" menu items
                    }
                }
            }
            else //(treeMain.SelectedNode == null)
            {
                DisableEnableItemFunctionality(); //disable the "Enable" menu items
                DisableDisableItemFunctionality(); //disable the "Disable" menu items
                DisableRemoveItemFunctionality(); //disable the "Remove" menu items
            }

            ShowChangesAsteriskIfNeeded();
            //update the application's title bar to indicate if unsaved changes exist in the output map
        }

        /// <summary>
        ///     Updates the application's title bar with an asterisk if unsaved changes exist in the output map
        /// </summary>
        private void ShowChangesAsteriskIfNeeded()
        {
            if (!_hasChanges) return;
            if (!Text.EndsWith(" *", StringComparison.Ordinal))
            {
                Text = Text + @" *";
            }
        }

        /// <summary>
        ///     Shows the Help-About dialog
        /// </summary>
        private void ShowHelpAbout()
        {
            using (var dialog = new frmHelpAbout())
            {
                dialog.ShowDialog(this);
            }
        }

        /// <summary>
        ///     Displays the Open File common dialog
        /// </summary>
        private void ShowOpenFileDialog()
        {
            _dlgOpen.Reset(); //reset the dialog's properties to the default set of properties
            _dlgOpen.ValidateNames = true; //make sure the supplied file name is a valid Win32 file name
            _dlgOpen.Multiselect = false; //only allow selecting one file
            _dlgOpen.ShowReadOnly = false; //don't show the "Read-Only" checkbox 
            var appPath = Path.GetFullPath(Application.ExecutablePath);
            _dlgOpen.InitialDirectory = appPath; //set the dialog to search from the application's path first
            _dlgOpen.CheckPathExists = true; //make sure any user-supplied paths exist
            _dlgOpen.AddExtension = true;
            //automatically add an extension to user-supplied file names if the raw name doesn't exist
            _dlgOpen.DefaultExt = ".map"; //this is the default extension to add 
            _dlgOpen.DereferenceLinks = true;
            //follow shortcuts to their targets if the user selects a shortcut instead of the file it points to
            _dlgOpen.Filter = @"Joymapper files (*.map)|*.map|All files(*.*)|*.*";
            //set up the basic filter types for this dialog
            _dlgOpen.FilterIndex = 0; //choose the default filter
            _dlgOpen.ShowHelp = false; //disable context-sensitive help within the FileOpen dialog
            _dlgOpen.SupportMultiDottedExtensions = true; //allow file names with multiple "dots" in the filename
            _dlgOpen.Title = @"Open"; //set the dialog's titlebar text
            _dlgOpen.ShowDialog(this); //show the dialog
        }

        /// <summary>
        ///     Displays the Options dialog box.
        /// </summary>
        private void ShowOptionsDialog()
        {
            using (var opt = new frmOptions())
            {
                opt.ShowDialog(this);
            }
            var set = Settings.Default;
            if (set.EnableAutoHighlighting)
            {
                _autoHighlightingEnabled = true;
                btnAutoHighlighting.Text = @"Auto-highlighting (on)";
                EnableAutoHighlightingFunctionality();
            }
            else
            {
                _autoHighlightingEnabled = false;
                btnAutoHighlighting.Text = @"Auto-highlighting (off)";
                DisableAutoHighlightingFunctionality();
            }
        }

        /// <summary>
        ///     Informs the mediator to start sending output through to PPJoy. Also performs minimization
        ///     of the main window if the user preference is configured to do so.
        /// </summary>
        private void StartMapping()
        {
            //can't start mapping if there's no valid mappings in the output map
            if (_outputMap == null || !_outputMap.ContainsValidMappings())
            {
                return;
            }
            //read user preferences to see if we should minimize the main window to the tray when
            //we start mapping
            var set = Settings.Default;
            if (set.MinimizeOnMappingStart)
            {
                MinimizeToSystemTray();
            }

            UpdateMediatorMap();
            try
            {
                if (_mediator != null)
                {
                    //inform the mediator to start sending output to PPJoy
                    _mediator.SendOutput = true;
                }
            }
            catch (NullReferenceException e)
            {
                _log.Debug(e.Message, e);
            }
            //update menu/toolbar items enabled/disabled states
            SetMenuAndToolbarEnabledDisabledStates();
            DisableMappingsTab();
        }

        /// <summary>
        ///     Informs the mediator to stop sending output to PPJoy
        /// </summary>
        private void StopMapping()
        {
            if (_mediator != null)
            {
                //inform the mediator to stop sending output to PPJoy
                _mediator.SendOutput = false;
            }
            //update menu/toolbar items enabled/disabled states
            SetMenuAndToolbarEnabledDisabledStates();
            EnableMappingsTab();
        }

        private void StopMediatingAndDisposeMediator()
        {
            if (_mediator != null)
            {
                try
                {
                    _mediator.RaiseEvents = false; //tell the mediator to stop sending us events
                    _mediator.StopMediating(); //shut down the mediator 
                    _mediator.Dispose(); //manually dispose of the mediator
                }
                catch (NullReferenceException e)
                {
                    _log.Debug(e.Message, e);
                }
            }
            _mediator = null;
        }

        /// <summary>
        ///     Shuts down the application cleanly.
        /// </summary>
        private void StopRunning()
        {
            if (!_exiting) //ignore simultaneous calls to this method
            {
                _exiting = true; //set a flag indicating we're currently exiting the application
                StopMapping(); //tell the mediator not to send any more updates to PPJoy
                StopMediatingAndDisposeMediator();
                nfyTrayIcon.Visible = false;
                Application.Exit(); //hard-exit the application
            }
        }

        /// <summary>
        ///     Updates the output map with the mapping values supplied by the user for a specific physical control
        /// </summary>
        /// <param name="inputControl">
        ///     a PhysicalControlInfo object representing the physical control to which the mappings
        ///     specified by the user will apply
        /// </param>
        private void StoreCurrentMapping(PhysicalControlInfo inputControl)
        {
            var changed = false; //flag to signal if anything's changed as a result of these new mappings
            var selectedVirtualDeviceName = cboVirtualDevice.Text; //read the value of the "Virtual Device" combo box
            var selectedVirtualControlName = cboVirtualControl.Text;
            //read the value of the "Virtual Control" combo box
            var alias = txtDescription.Text; //read the contents of the "Description" textbox

            if (txtDescription.Text != null && string.Equals(txtDescription.Text, string.Empty))
            {
                //if an empty string was supplied for a description, treat that as if no description
                //was supplied
                alias = null;
            }
            //check for changes as a result of the description changing and update the map accordingly
            if (inputControl.Alias != alias)
            {
                inputControl.Alias = alias;
                changed = true;
            }

            string newDisplayText = null;
            if (_lastNode.Tag != null)
            {
                var tag = _lastNode.Tag as PhysicalControlInfo;
                if (tag != null)
                    //if the node the user just edited is a node that represents a physical control
                {
                    //figure out what to display next to the control's node in the TreeView 
                    newDisplayText = GetControlDisplayText(tag);
                }
                else if (_lastNode.Tag is PhysicalDeviceInfo) //else if the edited node is a Device node
                {
                    //Device's descriptions are simple -- they always match the Device's alias
                    newDisplayText = ((PhysicalDeviceInfo) _lastNode.Tag).Alias;
                }

                //update the text displayed next to the treeview node for this control
                if (newDisplayText != null)
                {
                    _lastNode.Text = newDisplayText;
                }
            }

            //if the user selected any of the '<unmapped>' combo box entries, then we need to remove the 
            //current mapping from the output map
            if (selectedVirtualDeviceName == "<unmapped>" || selectedVirtualControlName == "<unmapped>" ||
                selectedVirtualDeviceName == "" || selectedVirtualControlName == "")
            {
                changed = changed | _outputMap.SetMapping(inputControl, null);
            }
            else //the user must have selected an actual, valid mapping
            {
                var controlNum = -1;

                //if the physical control is a Button, then we know to expect that the mapping must be to
                //a Digital data source
                if (inputControl.ControlType == ControlType.Button)
                {
                    //determine *which* digital data source the user selected by removing the word "Digital"
                    //from the text of the selected entry in the Virtual Control combobox
                    var virtualControlNumber = selectedVirtualControlName.Replace("Digital", "").Trim();
                    controlNum = int.Parse(virtualControlNumber);
                }
                //if the physical control is an Axis, then we know to expect that the mapping must be to
                //an Analog data source
                else if (inputControl.ControlType == ControlType.Axis)
                {
                    //determine *which* analog data source the user selected by removing the word "Analog"
                    //from the text of the selected entry in the Virtual Control combobox
                    var virtualControlNumber = selectedVirtualControlName.Replace("Analog", "").Trim();
                    controlNum = int.Parse(virtualControlNumber);
                }
                //if the physical control is a POV, then we know to expect that the mapping must be to
                //an Analog data source
                else if (inputControl.ControlType == ControlType.Pov)
                {
                    //determine *which* analog data source the user selected by removing the word "Analog"
                    //from the text of the selected entry in the Virtual Control combobox
                    var virtualControlNumber = selectedVirtualControlName.Replace("Analog", "").Trim();
                    controlNum = int.Parse(virtualControlNumber);
                }

                //determine which output device the user selected by removing the words "PPJoy Virtual joystick "
                //from the text of the selected entry in the Virtual Device combobox
                var virtualDeviceNumber = selectedVirtualDeviceName.Replace("PPJoy Virtual joystick ", "").Trim();
                var PPJoyDeviceNumber = int.Parse(virtualDeviceNumber);

                //create the appropriate objects to represent the user's choice of virtual device
                //and virtual control 
                var virtualDevice = new VirtualDeviceInfo(PPJoyDeviceNumber);
                VirtualControlInfo outputControl = null;
                if (inputControl.ControlType == ControlType.Axis || inputControl.ControlType == ControlType.Pov)
                {
                    outputControl = new VirtualControlInfo(virtualDevice, ControlType.Axis, controlNum);
                }
                else if (inputControl.ControlType == ControlType.Button)
                {
                    outputControl = new VirtualControlInfo(virtualDevice, ControlType.Button, controlNum);
                }
                //save the user's choices in the output map, detecting any changes that are made as a result
                changed = changed | _outputMap.SetMapping(inputControl, outputControl);
            }
            if (changed)
            {
                _hasChanges = true;
            }
            //SetMenuAndToolbarEnabledDisabledStates();
        }

        private void tblMappingsLayoutTable_Resize(object sender, EventArgs e)
        {
            txtDescription.Width = (int) tblMappingsLayoutTable.ColumnStyles[1].Width;
            cboVirtualDevice.Width = (int) tblMappingsLayoutTable.ColumnStyles[1].Width;
            cboVirtualDevice.DropDownWidth = Math.Max(cboVirtualDevice.Width - 10, 10);
            cboVirtualControl.Width = (int) tblMappingsLayoutTable.ColumnStyles[1].Width;
            cboVirtualControl.DropDownWidth = Math.Max(cboVirtualControl.Width - 10, 10);
        }

        /// <summary>
        ///     Event handler for the form's axis and POV state-clearing timer's Tick event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the Tick event.</param>
        private void tmrAxisAndPovMovedStateClearingTimer_Tick(object sender, EventArgs e)
        {
            //The form has a timer on it that fires at rapid intervals so that
            //the state of axis controls and POV that have moved (and which have
            //consequently had their icons highlighted in the TreeView) can be reset
            //to the default image state.  Without this mechanism, these icons, 
            //once highlighted, would never return to the unhighlighted (normal) state.
            //NOTE: if an axis is still undergoing active movement, the icon will
            //reset briefly to Normal state, but will then be set to the Highlighted
            //state as the next incoming state-change event is raised for that axis.
            foreach (TreeNode deviceNode in treeMain.Nodes[0].Nodes)
            foreach (TreeNode containerNode in deviceNode.Nodes)
            foreach (TreeNode controlNode in containerNode.Nodes)
            {
                var control = (PhysicalControlInfo) controlNode.Tag;
                if (control?.ControlType != ControlType.Axis) continue;
                if (!IsNodeEnabled(controlNode)) continue;
                controlNode.ImageIndex = IMAGE_INDEX_AXIS;
                controlNode.SelectedImageIndex = IMAGE_INDEX_AXIS;
            }
        }

        /// <summary>
        ///     Event handler for the TreeView control's AfterSelect event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the AfterSelect event.</param>
        private void treeMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //this method gets called whenever a different node is selected in the TreeView.
            //By the time this method gets called, the current node in the treeview
            //is the newly-selected node.

            _lastNode = e.Node;
            var parent = e.Node.Parent;
            if (parent != null)
            {
                switch (parent.Text)
                {
                    case "Axes":
                    case "Buttons":
                    case "Povs":
                        //a physical control node is selected in the treeview,
                        //so enable editing tab and populate and make visible the relevant
                        //controls on that tab
                        tabMain.Enabled = true;
                        txtDescription.Text = ((PhysicalControlInfo) e.Node.Tag).Alias;
                        txtDescription.ReadOnly = false;
                        cboVirtualControl.Show();
                        cboVirtualDevice.Show();
                        lblVirtualControl.Show();
                        lblVirtualDevice.Show();
                        if (_mappingsTab != null)
                        {
                            if (!tabMain.TabPages.Contains(_mappingsTab))
                            {
                                tabMain.TabPages.Add(_mappingsTab);
                            }
                            _mappingsTab.Show();
                            _mappingsTab.BringToFront();
                        }
                        DisplayCurrentMapping((PhysicalControlInfo) e.Node.Tag);
                        break;
                    case @"Local Devices":
                        //an input *device* node is selected in the treeview,
                        //so enable editing tab, but hide controls on 
                        //that tab that aren't relevant to input device editing
                        tabMain.Enabled = true;
                        txtDescription.Text = ((PhysicalDeviceInfo) e.Node.Tag).Alias;
                        txtDescription.ReadOnly = true;
                        cboVirtualControl.Hide();
                        cboVirtualDevice.Hide();
                        lblVirtualControl.Hide();
                        lblVirtualDevice.Hide();
                        if (_mappingsTab != null)
                        {
                            if (!tabMain.TabPages.Contains(_mappingsTab))
                            {
                                tabMain.TabPages.Add(_mappingsTab);
                            }
                            _mappingsTab.Show();
                            _mappingsTab.BringToFront();
                        }
                        break;
                    default:
                        tabMain.Enabled = false; //nothing to edit here, so remove
                        //the Mappings tab from the Tab control
                        if (_mappingsTab != null)
                        {
                            tabMain.TabPages.Remove(_mappingsTab);
                        }
                        break;
                }
            }
            else //the Local Devices node is selected, nothing to edit here, so remove
                //the Mappings tab from the Tab control
            {
                tabMain.Enabled = false;
                if (_mappingsTab != null)
                {
                    tabMain.TabPages.Remove(_mappingsTab);
                }
            }

            //update the menu and toolbar enabled/disabled states
            SetMenuAndToolbarEnabledDisabledStates();
        }

        /// <summary>
        ///     Event handler for the form's TreeView's BeforeSelect event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the BeforeSelect event.</param>
        private void treeMain_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //This event handler gets called whenever the user attempts to select
            //a different node in the TreeView.  We handle the event so we can 
            //save the edits they made to the current node before we move to the
            //new node.

            //call the common method for validating and saving a node's edits
            CheckAndStoreCurrentMapping();
        }

        /// <summary>
        ///     Updates the Mediator object's copy of the output map.  The Mediator uses
        ///     a separate copy of the Output Map, which allows for making changes to the
        ///     displayed output map without those changes having an immediate effect
        ///     upon the mediator's behavior.  This allows the mediator to be stopped cleanly
        ///     and restarted using the newly-updated output map without causing havoc with
        ///     DirectInput, etc.
        /// </summary>
        private void UpdateMediatorMap()
        {
            //copy this form's instance of the output map 
            var toUse = (OutputMap) ((ICloneable) _outputMap).Clone();

            if (_mediator != null)
            {
                try
                {
                    //supply the new map to the mediator
                    _mediator.OutputMap = toUse;

                    if (!_mediator.IsRunning)
                    {
                        //start the mediator if it's not already started
                        _mediator.StartMediating();
                    }
                }
                catch (NullReferenceException e)
                {
                    _log.Debug(e.Message, e);
                }
            }
        }

        /// <summary>
        ///     Determines if the user wants to exit the application even if there are unsaved changes.
        ///     Presents the user with the opportunity to save changes, and parses their answer
        ///     so that if the user cancels the save operation or chooses not to save, the
        ///     value returned from this method will indicate the appropriate action to take (exit or don't exit)
        /// </summary>
        /// <returns>a boolean indicating true if the user wants to proceed with exiting the application, or false, if not.</returns>
        private bool UserWantsToExit()
        {
            if (_exiting)
            {
                return true; //we're already exiting the application, so we must have already gone through this process
            }
            return PromptForSaveChanges();
        }
    }
}