using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Phcc.DeviceManager.Config
{
    [Serializable]
    public class Doa8Servo : Peripheral
    {
        [XmlArray("Calibrations")]
        [XmlArrayItem("Calibration")]
        public List<ServoCalibration> ServoCalibrations { get; set; }
    }
}