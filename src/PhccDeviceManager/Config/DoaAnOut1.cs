using System;

namespace Phcc.DeviceManager.Config
{
    [Serializable]
    public class DoaAnOut1 : Peripheral
    {
        public byte GainAllChannels { get; set; }
    }
}