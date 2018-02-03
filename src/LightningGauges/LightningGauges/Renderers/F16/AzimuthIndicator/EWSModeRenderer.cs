using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeRenderer
    {
        internal static void DrawEWSMode(
            Color severeColor, Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Color warnColor, Color okColor, InstrumentState instrumentState, Font font)
        {
            //Added Falcas 10-11-2012.
            instrumentState.EWMSMode = EWSModeRetriever.GetEWMSMode(instrumentState.cmdsMode);
            switch (instrumentState.EWMSMode)
            {
                case EWMSMode.Off:
                {
                    EWSModeOffRenderer.DrawEWSModeOff(severeColor, gfx, ewmsModeRectangle, miscTextStringFormat, font);
                }
                    break;
                case EWMSMode.Standby:
                {
                    EWSModeStandbyRenderer.DrawEWSModeStandby(gfx, ewmsModeRectangle, miscTextStringFormat, warnColor, font);
                }
                    break;
                case EWMSMode.Manual:
                {
                    EWSModeManualRenderer.DrawEWSModeManual(gfx, ewmsModeRectangle, miscTextStringFormat, okColor, font);
                }
                    break;
                case EWMSMode.Semiautomatic:
                {
                    EWSModeSemiautomaticRenderer.DrawEWSModeSemiautomatic(gfx, ewmsModeRectangle, miscTextStringFormat, okColor, font);
                }
                    break;
                case EWMSMode.Automatic:
                {
                    EWSModeAutomaticRenderer.DrawEWSModeAutomatic(gfx, ewmsModeRectangle, miscTextStringFormat, okColor, font);
                }
                    break;
                case EWMSMode.Bypass:
                {
                    EWSModeBypassRenderer.DrawEWSModeBypass(severeColor, gfx, ewmsModeRectangle, miscTextStringFormat, font);
                }
                    break;
            }
        }
    }
}