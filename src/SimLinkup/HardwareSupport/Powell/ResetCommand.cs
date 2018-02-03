namespace SimLinkup.HardwareSupport.Powell
{
    internal class ResetCommand : RWRCommand
    {
        public override byte[] ToBytes()
        {
            return new byte[] {0x00};
        }
    }
}