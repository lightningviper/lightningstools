namespace SimLinkup.HardwareSupport.Powell
{
    internal struct FalconRWRSymbol
    {
        public int SymbolId { get; set; }
        public double BearingDegrees { get; set; }
        public double Lethality { get; set; }
        public bool MissileActivity { get; set; }
        public bool MissileLaunch { get; set; }
        public bool Selected { get; set; }
        public bool NewDetection { get; set; }
    }
}