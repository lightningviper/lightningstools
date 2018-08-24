using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.HardwareSupport;
using Common.InputSupport;
using Common.InputSupport.DirectInput;
using Common.MacroProgramming;
using log4net;

namespace SimLinkup.HardwareSupport.DirectInput
{
    public class DirectInputHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DirectInputHardwareSupportModule));
        private readonly AnalogSignal[] _axes;

        private readonly DigitalSignal[] _buttons;
        private readonly DIDeviceMonitor _deviceMonitor;
        private readonly Mediator _mediator;
        private readonly AnalogSignal[] _povs;
        private bool _isDisposed;

        private DirectInputHardwareSupportModule()
        {
        }

        private DirectInputHardwareSupportModule(Mediator mediator, DIDeviceMonitor deviceMonitor)
            : this()
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _deviceMonitor = deviceMonitor ?? throw new ArgumentNullException(nameof(deviceMonitor));

            CreateSignals(out _buttons, out _axes, out _povs);
            _mediator.PhysicalControlStateChanged += _mediator_PhysicalControlStateChanged;
        }

        public override AnalogSignal[] AnalogInputs => null;

        public override AnalogSignal[] AnalogOutputs => _axes.Union(_povs).ToArray();

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => _buttons.ToArray();

        public override string FriendlyName
        {
            get
            {
                var alias = _deviceMonitor.DeviceInfo.Alias;
                var guid = _deviceMonitor.DeviceInfo.Guid;
                var deviceNum = _deviceMonitor.DeviceInfo.DeviceNum;
                return $"DirectInput Device {deviceNum} - {alias} [GUID:{guid}]";
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DirectInputHardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var mediator = new Mediator(Application.OpenForms.Count == 0 ? null : Application.OpenForms[0])
                {
                    RaiseEvents = true
                };
                foreach (var deviceMonitor in mediator.DeviceMonitors)
                {
                    IHardwareSupportModule thisHsm =
                        new DirectInputHardwareSupportModule(mediator, deviceMonitor.Value);
                    toReturn.Add(thisHsm);
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return toReturn.ToArray();
        }


        private void _mediator_PhysicalControlStateChanged(object sender, PhysicalControlStateChangedEventArgs e)
        {
            if (e.Control.Parent.Key != _deviceMonitor.DeviceInfo.Key) return;
            switch (e.Control.ControlType)
            {
                case ControlType.Axis:
                    var thisAxis = _axes.SingleOrDefault(x => x.Id == e.Control.ToString());
                    if (thisAxis != null) thisAxis.State = e.CurrentState;
                    break;
                case ControlType.Button:
                    var thisButton = _buttons.SingleOrDefault(x => x.Id == e.Control.ToString());
                    if (thisButton != null) thisButton.State = e.CurrentState == 1;
                    break;
                case ControlType.Pov:
                    var thisPov = _povs.SingleOrDefault(x => x.Id == e.Control.ToString());
                    if (thisPov != null) thisPov.State = e.CurrentState;
                    break;
            }
        }

        private void CreateSignals(out DigitalSignal[] buttons, out AnalogSignal[] axes, out AnalogSignal[] povs)
        {
            var device = _deviceMonitor.DeviceInfo;
            var buttonsToReturn = new List<DigitalSignal>();
            var axesToReturn = new List<AnalogSignal>();
            var povsToReturn = new List<AnalogSignal>();

            foreach (var button in device.Buttons)
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Controls",
                    CollectionName = "Buttons",
                    FriendlyName = button.Alias,
                    Id = button.ToString(),
                    Index = button.ControlNum,
                    PublisherObject = this,
                    Source = device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = device.Guid.ToString(),
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null
                };
                var currentState = _mediator.GetPhysicalControlValue(button, StateType.Current);
                thisSignal.State = currentState.HasValue && currentState.Value == 1;
                buttonsToReturn.Add(thisSignal);
            }

            foreach (var axis in device.Axes)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Controls",
                    CollectionName = "Axes",
                    FriendlyName = axis.Alias,
                    Id = axis.ToString(),
                    Index = axis.ControlNum,
                    PublisherObject = this,
                    Source = device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = device.Guid.ToString(),
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null
                };
                var currentState = _mediator.GetPhysicalControlValue(axis, StateType.Current);
                thisSignal.State = currentState ?? _deviceMonitor.AxisRangeMin;
                thisSignal.MinValue = _deviceMonitor.AxisRangeMin;
                thisSignal.MaxValue = _deviceMonitor.AxisRangeMax;

                axesToReturn.Add(thisSignal);
            }

            foreach (var pov in device.Povs)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Controls",
                    CollectionName = "POVs",
                    FriendlyName = pov.Alias,
                    Id = pov.ToString(),
                    Index = pov.ControlNum,
                    PublisherObject = this,
                    Source = device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = device.Guid.ToString(),
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0
                };
                var currentState = _mediator.GetPhysicalControlValue(pov, StateType.Current);
                thisSignal.State = currentState ?? -1;
                thisSignal.MinValue = -1;
                thisSignal.MaxValue = _deviceMonitor.AxisRangeMax;
                povsToReturn.Add(thisSignal);
            }

            buttons = buttonsToReturn.ToArray();
            axes = axesToReturn.ToArray();
            povs = povsToReturn.ToArray();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                }
            }
            _isDisposed = true;
        }
    }
}