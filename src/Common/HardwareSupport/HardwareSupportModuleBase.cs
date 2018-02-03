using System.Drawing;
using System.Threading;
using Common.MacroProgramming;

namespace Common.HardwareSupport
{
    public abstract class HardwareSupportModuleBase : IHardwareSupportModule
    {
        public abstract AnalogSignal[] AnalogInputs { get; }
        public abstract AnalogSignal[] AnalogOutputs { get; }

        public abstract DigitalSignal[] DigitalInputs { get; }
        public abstract DigitalSignal[] DigitalOutputs { get; }
        public abstract string FriendlyName { get; }

        public virtual void Render(Graphics g, Rectangle destinationRectangle)
        {
        }

        public virtual void Synchronize()
        {
            Thread.Sleep(0);
        }
    }
}