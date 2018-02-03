using System.Collections.Generic;
using System.Windows.Forms;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.SimSupport;

namespace SimLinkup.UI.UserControls
{
    public partial class ModuleList : UserControl
    {
        private IEnumerable<IHardwareSupportModule> _hardwareSupportModules;
        private IEnumerable<SimSupportModule> _simSupportModules;

        public ModuleList()
        {
            InitializeComponent();
        }

        public IEnumerable<IHardwareSupportModule> HardwareSupportModules
        {
            get => _hardwareSupportModules;
            set
            {
                panel.Controls.Clear();
                _hardwareSupportModules = value;
                if (_hardwareSupportModules == null)
                {
                    return;
                }
                foreach (var hsm in _hardwareSupportModules)
                {
                    var visualizer = new ModuleBasicVisualizer
                    {
                        ModuleName = hsm.FriendlyName
                    };
                    visualizer.ShowSignals += (s, e) =>
                    {
                        var signalsView = new frmSignalsViewer();
                        var signalList = new SignalList<Signal>();
                        if (hsm.AnalogInputs != null)
                        {
                            signalList.AddRange(hsm.AnalogInputs);
                        }
                        if (hsm.AnalogOutputs != null)
                        {
                            signalList.AddRange(hsm.AnalogOutputs);
                        }
                        if (hsm.DigitalInputs != null)
                        {
                            signalList.AddRange(hsm.DigitalInputs);
                        }
                        if (hsm.DigitalOutputs != null)
                        {
                            signalList.AddRange(hsm.DigitalOutputs);
                        }
                        signalsView.Signals = signalList;
                        signalsView.ShowDialog(ParentForm);
                    };
                    panel.Controls.Add(visualizer);
                }
            }
        }

        public IEnumerable<SimSupportModule> SimSupportModules
        {
            get => _simSupportModules;
            set
            {
                panel.Controls.Clear();
                _simSupportModules = value;
                if (_simSupportModules == null)
                {
                    return;
                }
                foreach (var ssm in _simSupportModules)
                {
                    var visualizer = new ModuleBasicVisualizer
                    {
                        ModuleName = ssm.FriendlyName
                    };
                    visualizer.ShowSignals += (s, e) => { };
                    panel.Controls.Add(visualizer);
                }
            }
        }
    }
}