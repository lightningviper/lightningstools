using System;
using System.Xml.Serialization;

namespace Phcc.DeviceManager.Config
{
    [Serializable]
    [XmlInclude(typeof (Motherboard))]
    [XmlInclude(typeof (Peripheral))]
    public abstract class PhccConfigElement
    {
    }
}