using System;

namespace SimLinkup.HardwareSupport.TeensyEWMU
{
    [Serializable]
    public class DXOutput
    {
        public DXOutput()
        {
        }

        public DXOutput(string ID, bool invert) : this()
        {
            this.ID = ID;
            this.Invert = invert;
        }
        public string ID { get; set; }
        public bool Invert { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DXOutput && string.Equals( (obj as DXOutput).ID, ID) && (obj as DXOutput).Invert == Invert;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(DXOutput)} ID:{ID} Invert:{Invert}";
        }
    }
}
