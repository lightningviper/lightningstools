using System;
using Common.MacroProgramming;

namespace SimLinkup.Signals
{
    [Serializable]
    public class SignalMapping
    {
        public Signal Destination { get; set; }
        public Signal Source { get; set; }
    }
}