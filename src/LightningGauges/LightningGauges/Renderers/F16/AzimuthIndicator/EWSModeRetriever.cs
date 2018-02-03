namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeRetriever
    {
        //Added Falcas 10-11-2012

        internal static EWMSMode GetEWMSMode(int cmdsMode)
        {
            EWMSMode mode;
            switch (cmdsMode)
            {
                case (int) EWMSMode.Off:
                    mode = EWMSMode.Off;
                    break;
                case (int) EWMSMode.Standby:
                    mode = EWMSMode.Standby;
                    break;
                case (int) EWMSMode.Manual:
                    mode = EWMSMode.Manual;
                    break;
                case (int) EWMSMode.Semiautomatic:
                    mode = EWMSMode.Semiautomatic;
                    break;
                case (int) EWMSMode.Automatic:
                    mode = EWMSMode.Automatic;
                    break;
                case (int) EWMSMode.Bypass:
                    mode = EWMSMode.Bypass;
                    break;
                default:
                    mode = EWMSMode.Off;
                    break;
            }

            return mode;
        }
    }
}