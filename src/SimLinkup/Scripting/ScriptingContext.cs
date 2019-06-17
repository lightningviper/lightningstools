using System;
using System.Collections.Generic;
using System.Linq;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.SimSupport;

namespace SimLinkup.Scripting
{
    public class ScriptingContext : Dictionary<object, object>
    {
        private IHardwareSupportModule[] _hsms;
        private SimSupportModule[] _ssms;

        public SignalList<Signal> AllSignals
        {
            get
            {
                var toReturn = new SignalList<Signal>();
                toReturn.AddRange(Values.OfType<Signal>());
                return toReturn;
            }
        }
        public SignalList<AnalogSignal> AnalogSignals
        {
            get
            {
                var toReturn = new SignalList<AnalogSignal>();
                toReturn.AddRange(Values.OfType<AnalogSignal>());
                return toReturn;
            }
        }
        public SignalList<DigitalSignal> DigitalSignals
        {
            get
            {
                var toReturn = new SignalList<DigitalSignal>();
                toReturn.AddRange(Values.OfType<DigitalSignal>());
                return toReturn;
            }
        }
        public SignalList<TextSignal> TextSignals
        {
            get
            {
                var toReturn = new SignalList<TextSignal>();
                toReturn.AddRange(Values.OfType<TextSignal>());
                return toReturn;
            }
        }


        public IHardwareSupportModule[] HardwareSupportModules
        {
            get => _hsms;
            set
            {
                _hsms = value;
                AddSimAndHardwareInsAndOuts();
            }
        }

        public SimSupportModule[] SimSupportModules
        {
            get => _ssms;
            set
            {
                _ssms = value;
                AddSimAndHardwareInsAndOuts();
            }
        }

        private void AddHardwareInputsAndOutputs()
        {
            if (HardwareSupportModules == null || HardwareSupportModules.Length == 0) return;

            foreach (var hsm in HardwareSupportModules)
            {
                if (hsm == null) continue;
                if (hsm.AnalogInputs != null && hsm.AnalogInputs.Length > 0)
                {
                    foreach (var analogInput in hsm.AnalogInputs)
                        this[analogInput.Id] = analogInput;
                }
                if (hsm.AnalogOutputs != null && hsm.AnalogOutputs.Length > 0)
                {
                    foreach (var analogOutput in hsm.AnalogOutputs)
                        this[analogOutput.Id] = analogOutput;
                }
                if (hsm.DigitalInputs != null && hsm.DigitalInputs.Length > 0)
                {
                    foreach (var digitalInput in hsm.DigitalInputs)
                        this[digitalInput.Id] = digitalInput;
                }
                if (hsm.DigitalOutputs != null && hsm.DigitalOutputs.Length > 0)
                {
                    foreach (var digitalOutput in hsm.DigitalOutputs)
                        this[digitalOutput.Id] = digitalOutput;
                }
                if (hsm.TextInputs != null && hsm.TextInputs.Length > 0)
                {
                    foreach (var textInput in hsm.TextInputs)
                        this[textInput.Id] = textInput;
                }
                if (hsm.TextOutputs != null && hsm.TextOutputs.Length > 0)
                {
                    foreach (var textOutput in hsm.TextOutputs)
                        this[textOutput.Id] = textOutput;
                }
            }
        }

        private void AddSimAndHardwareInsAndOuts()
        {
            Clear();
            AddSimInputsAndOutputs();
            AddHardwareInputsAndOutputs();
        }

        private void AddSimInputsAndOutputs()
        {
            if (SimSupportModules == null || SimSupportModules.Length == 0) return;
            foreach (var ssm in SimSupportModules.Where(ssm => ssm != null))
            {
                if (ssm.SimOutputs != null)
                {
                    foreach (var output in ssm.SimOutputs.Values)
                        this[output.Id] = output;
                }
                if (ssm.SimCommands != null)
                {
                    foreach (var command in ssm.SimCommands.Values)
                        this[command.In.Id] = command.In;
                }
            }
        }
    }
}