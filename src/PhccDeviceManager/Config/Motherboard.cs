using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Phcc.DeviceManager.Config
{
    [Serializable]
    public class Motherboard : PhccConfigElement
    {
        public Motherboard()
        {
            Peripherals = new List<Peripheral>();
        }

        public string ComPort { get; set; }

        [XmlArray("Peripherals")]
        [XmlArrayItem("Peripheral")]
        public List<Peripheral> Peripherals { get; set; }
    }
}