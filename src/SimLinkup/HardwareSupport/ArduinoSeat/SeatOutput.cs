using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.ArduinoSeat
{
    [Serializable]
    public class SeatOutput
    {
        public SeatOutput()
        {
        }

        public SeatOutput(string ID) : this()
        {
            this.ID = ID;
        }
        [XmlElement("ID")]
        public string ID { get; set; }
        [XmlElement("FORCE")]
        public MotorForce FORCE { get; set; } = MotorForce.Manual;
        public PulseType TYPE { get; set; } = PulseType.Fixed;
        
        public bool MOTOR_1 { get; set; }
        public bool MOTOR_2 { get; set; }
        public bool MOTOR_3 { get; set; }
        public bool MOTOR_4 { get; set; }
        
        public byte MOTOR_1_SPEED { get; set; }
        public byte MOTOR_2_SPEED { get; set; }
        public byte MOTOR_3_SPEED { get; set; }
        public byte MOTOR_4_SPEED { get; set; }
        
        public double MIN { get; set; }
        public double MAX { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SeatOutput && string.Equals((obj as SeatOutput).ID, ID);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(SeatOutput)} ID:{ID}";
        }
    }

    public enum MotorForce
    {
        Manual,
        Off,
        Rumble,
        Medium,
        Hard
    }

    public enum PulseType
    {
        Fixed,
        Progressive,
        CenterPeak
    }
}
