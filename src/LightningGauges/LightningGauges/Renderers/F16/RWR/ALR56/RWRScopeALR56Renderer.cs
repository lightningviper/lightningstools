using LightningGauges.Renderers.F16.AzimuthIndicator;
using System;
using System.Windows;
using System.Windows.Media;

namespace LightningGauges.Renderers.F16.RWR.ALR56
{
    public class RWRScopeALR56Renderer : RWRRenderer
    {
        private enum RWRSymbol { Diamond, Hat, MissileActivity, MissileLaunch };
        
        private const int BASELINE_CYCLE_TIME_ALR56 = 500;
        private const int CYCLE_TIME_PER_CONTACT_ALR56 = 50;
        private int cycleduration = BASELINE_CYCLE_TIME_ALR56;
        private double cycletimeflipper = -1.0F;
        private int cycletimer = 0;
        private int flipFactor = 1;
        private int processTimeNav = Environment.TickCount;
        private double platformYaw = 0;
        private int flashTime = Environment.TickCount;
        private bool flash = false;
        private int flashFasterTime = Environment.TickCount;
        private bool flashFaster = false;
        private int show = 0;
        private bool wasflashing = false;
        private bool processFlag = false;

        public RWRScopeALR56Renderer()
        {

        }

        public override void Render(DrawingContext drawingContext)
        {
            string rwrInfoBuffer = string.Empty;
            for (int i = 0; i <  InstrumentState.RwrInfo.Length; i++)
            {
                rwrInfoBuffer += (char)InstrumentState.RwrInfo[i];
            }

            rwrInfo = rwrInfoBuffer.Split('<');

            int processTimeNavDt = Environment.TickCount - processTimeNav;

            CycleTimeOrientation();

            if (!RwrInfoContains("navfault"))
            {
                if (processFlag)
                {
                    processTimeNav = Environment.TickCount;
                    platformYaw = InstrumentState.yaw;
                }
            }
            else
            {
                if (processTimeNavDt > 3000)
                {
                    processTimeNav = Environment.TickCount;
                    platformYaw = InstrumentState.yaw;
                }
            }

            int flashTimeDt = Environment.TickCount - flashTime;
            if (flashTimeDt >= 512)
            {
                flashTime = Environment.TickCount;
                flash = !flash;
            }

            int flashFasterTimeDt = Environment.TickCount - flashFasterTime;
            if (flashFasterTimeDt >= 128)
            {
                flashFasterTime = Environment.TickCount;
                flashFaster = !flashFaster;
            }

            if (flash && !wasflashing)
            {
                show++;
                wasflashing = true;
                if (show > 3)
                {
                    show = 0;
                }
            }
            else if (!flash && wasflashing)
            {
                show++;
                wasflashing = false;
            }

            if (RwrInfoContains("onflip"))
            {
                flipFactor = -1;
            }
            else
            {
                flipFactor = 1;
            }

            if (!RwrInfoContains("onpwr"))
            {
                return;
            }

            Pen pen = new Pen();
            pen.Thickness = 0;

            drawingContext.PushClip(new EllipseGeometry(new Point(ActualWidth / 2, ActualHeight / 2), ActualWidth / 2, ActualHeight / 2));
            
            if (!RwrInfoContains("onsys"))
            {
                for (int i = 0; i < InstrumentState.RwrObjectCount; i++)
                {
                    DrawContact(drawingContext, i);
                }

                DrawLine(drawingContext, 0.0, 0.1, 0.0, 0.2);
                DrawLine(drawingContext, 0.0, -0.1, 0.0, -0.2);
                DrawLine(drawingContext, 0.1, 0.0, 0.2, 0.0);
                DrawLine(drawingContext, -0.1, 0.0, -0.2, 0.0);

                DrawLine(drawingContext, 0.1, 0.0, 0.1, cycletimeflipper * 0.06);
            }

            if (RwrInfoContains("on1bit"))
            {
                DrawTextCenter(drawingContext, "RWR", 0, TextHeight(true) * 1.4, true);
                DrawTextCenter(drawingContext, "SYSTEM GO", 0, TextHeight(true) * 0.5, true);
            }

            if (RwrInfoContains("on2bit"))
            {
                double y = TextHeight(true) * 2.25;
                double x = -TextWidth("M", true) * 4;
                DrawTextLeft(drawingContext, "F16C", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "OFP", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "WO", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "PA", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "US", x, y, true);

                y = TextHeight(true) * 2.25;
                x = 0.1f;
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "0020", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "0040", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "0050", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "0060", x, y, true);

                y = TextHeight(true) * 1.7;
                x = -TextWidth("M", true) * 5.0;
                DrawTextLeft(drawingContext, "*", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "*", x, y, true);

                y = TextHeight(true) * 1.5;
                x = -TextWidth("M", true) * 6.0;
                DrawTextLeft(drawingContext, "1", x, y, true);
                y -= TextHeight(true) * 0.9;
                DrawTextLeft(drawingContext, "2", x, y, true);
            }

            if (RwrInfoContains("on3bit"))
            {
                if (RwrInfoContains("edf4lbl"))
                {
                    DrawTextCenter(drawingContext, "DF4", -0.4, 0.4, true);
                }

                if (RwrInfoContains("edf1lbl"))
                {
                    DrawTextCenter(drawingContext, "DF1", 0.4, 0.4, true);
                }

                if (RwrInfoContains("edf3lbl"))
                {
                    DrawTextCenter(drawingContext, "DF3", -0.4, -0.4, true);
                }

                if (RwrInfoContains("edf2lbl"))
                {
                    DrawTextCenter(drawingContext, "DF2", 0.4, -0.4, true);
                }

                if (RwrInfoContains("eaplbl"))
                {
                    DrawTextRight(drawingContext, "AP", 0.8, TextHeight(true) / 2, true);
                }
            }

            if (RwrInfoContains("wolbl"))
            {
                DrawTextCenteredVertival(drawingContext, "WO", 0, 0, true);
            }

            if (RwrInfoContains("ilbl"))
            {
                DrawTextCenteredVertival(drawingContext, "I", 0, 0, true);
            }
            
            if (RwrInfoContains("flbl"))
            {
                DrawTextCenteredVertival(drawingContext, "F", 0, 0, true);
            }
            
            if (RwrInfoContains("sllbl"))
            {
                DrawTextCenteredVertival(drawingContext, "SL", 0, 0, true);
            }
            else if (RwrInfoContains("slbl"))
            {
                DrawTextCenteredVertival(drawingContext, "S", 0, 0, true);
            }
            
            if (RwrInfoContains("llbl"))
            {
                DrawTextCenteredVertival(drawingContext, "L", 0, 0, true);
            }

            processFlag = false;

        }

        private bool RwrInfoContains(string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if(rwrInfo[i].StartsWith(key + ">")) return true;
            }

            return false;
        }

        private string RwrInfoGetKeyContent(string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return rwrInfo[i].Replace(key + ">", string.Empty).Replace("\0", string.Empty);
            }

            return string.Empty;
        }


        private double CycleTimeOrientation()
        {
            cycleduration = BASELINE_CYCLE_TIME_ALR56 + (InstrumentState.RwrObjectCount * CYCLE_TIME_PER_CONTACT_ALR56);
            if ((cycletimer == 0) || (Environment.TickCount > cycletimer))
            {
                cycletimeflipper = cycletimeflipper * -1.0F;
                cycletimer = Environment.TickCount + cycleduration;
                processFlag = true;
            }
            return cycletimeflipper;
        }

        private void DrawContact(DrawingContext drawingContext, int contact)
        {
            int symbol = InstrumentState.RWRsymbol[contact];

            if (symbol == -1) return;

            double angle = InstrumentState.bearing[contact];
            double range = InstrumentState.lethality[contact];

            angle -= platformYaw;
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);
            double radius = Math.Max(0.25f, Math.Min(0.80f, range));

            double xpos = radius * angleSin;
            xpos *= flipFactor;
            double ypos = radius * angleCos;

            bool big = false;

            if (InstrumentState.newDetection[contact] != 0 && flash)
            {
                big = true;
            }

            string csList = new String('\0', 40);
            if (RwrInfoContains("cslst"))
            {
                csList = RwrInfoGetKeyContent("cslst").PadRight(40, '\0');
            }

            DrawEmitterSymbol(drawingContext, symbol, xpos, ypos, csList[contact], big);

            if (InstrumentState.selected[contact] != 0)
            {
                DrawSymbol(drawingContext, RWRSymbol.Diamond, xpos, ypos, false);
            }

            if (InstrumentState.missileActivity[contact] != 0)
            {
                if (InstrumentState.missileLaunch[contact] != 0)
                    DrawSymbol(drawingContext, RWRSymbol.MissileLaunch, xpos, ypos, false);
                else
                    DrawSymbol(drawingContext, RWRSymbol.MissileActivity, xpos, ypos, false);
            }
        }

        private void DrawEmitterSymbol(DrawingContext drawingContext, int symbol, double x, double y, char customSymbol, bool big)
        {
            double scale = big ? 1.5 : 1;
            
            ThreatSymbols knownSymbol = (ThreatSymbols)symbol;

            switch (knownSymbol)
            {
                case ThreatSymbols.RWRSYM_ADVANCED_INTERCEPTOR:
                    {
                        double[,] lines = {
                            {0.000F,0.090F},
		                    {0.090F,-0.010F},
		                    {0.090F,-0.010F},
		                    {0.030F,0.010F},
		                    {0.030F,0.010F},
		                    {0.000F,-0.060F},
		                    {0.000F,-0.060F},
		                    {-0.030F,0.010F},
		                    {-0.030F,0.010F},
		                    {-0.090F,-0.010F},
		                    {-0.090F,-0.010F},
		                    {0.000F,0.090F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_BASIC_INTERCEPTOR:
                    {
                        double[,] lines = {
		                    {0.090F,-0.030F},
                            {0.000F,0.060F},
                            {0.000F,0.010F},
                            {0.090F,-0.030F},
                            {-0.090F,-0.030F},
                            {0.000F,0.010F},
                            {0.000F,0.060F},
                            {-0.090F,-0.030F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_ACTIVE_MISSILE:
                    DrawTextCenteredVertival(drawingContext, "M", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_HAWK:
                    DrawTextCenteredVertival(drawingContext, "H", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_PATRIOT:
                    DrawTextCenteredVertival(drawingContext, "P", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA2:
                    DrawTextCenteredVertival(drawingContext, "2", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA3:
                    DrawTextCenteredVertival(drawingContext, "3", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA4:
                    DrawTextCenteredVertival(drawingContext, "4", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA5:
                    DrawTextCenteredVertival(drawingContext, "5", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA6:
                    DrawTextCenteredVertival(drawingContext, "6", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA8:
                    DrawTextCenteredVertival(drawingContext, "8", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA9:
                    DrawTextCenteredVertival(drawingContext, "9", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA10:
                    DrawTextCenteredVertival(drawingContext, "10", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA13:
                    DrawTextCenteredVertival(drawingContext, "13", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_AAA:
                    if (flash)
                        DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "S", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SEARCH:
                    DrawTextCenteredVertival(drawingContext, "S", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_NAVAL:
                    {
                        double[,] lines = {
		                    {-0.060F,-0.040F},
		                    {-0.080F,0.000F},
		                    {-0.080F,0.000F},
		                    {-0.030F,0.000F},
		                    {-0.030F,0.000F},
		                    {-0.030F,0.040F},
		                    {-0.030F,0.040F},
		                    {0.030F,0.040F},
		                    {0.030F,0.040F},
		                    {0.030F,0.000F},
		                    {0.030F,0.000F},
		                    {0.060F,0.000F},
		                    {0.060F,0.000F},
		                    {0.060F,-0.040F},
		                    {0.060F,-0.040F},
		                    {-0.060F,-0.040F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_CHAPARAL:
                    DrawTextCenteredVertival(drawingContext, "C", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_SA15:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "15", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "M", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_NIKE:
                    DrawTextCenteredVertival(drawingContext, "N", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_A1:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_A2:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x + 0.02, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x - 0.02, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_A3:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x + 0.03, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x - 0.03, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_PDOT:
                    DrawTextCenteredVertival(drawingContext, "P", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_PSLASH:
                    DrawTextCenteredVertival(drawingContext, "P", x, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x + 0.01, y, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK1:
                    DrawTextCenteredVertival(drawingContext, "U", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK2:
                    DrawTextCenteredVertival(drawingContext, "U", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x + 0.02, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x - 0.02, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK3:
                    DrawTextCenteredVertival(drawingContext, "U", x, y, big);
                    DrawTextCenteredVertival(drawingContext, ".", x + 0.03, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x, y - 0.04, big);
                    DrawTextCenteredVertival(drawingContext, ".", x - 0.03, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_KSAM:
                    DrawTextCenteredVertival(drawingContext, "C", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_V4:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "4", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V5:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "5", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V6:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "6", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V14:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "14", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V15:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "15", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V16:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "16", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V18:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "18", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V19:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "19", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V20:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "20", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V21:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "21", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V22:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "22", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V23:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "23", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V25:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "25", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V27:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "27", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V29:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "29", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V30:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "30", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_V31:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "31", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_VP:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "P", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_VPD:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "PD", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_VA:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "A", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_VB:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "B", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_VS:
                    DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                    DrawTextCenteredVertival(drawingContext, "S", x, y - 0.04, big);
                    break;
                case ThreatSymbols.RWRSYM_Aa:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x + 0.01, y, big);
                    break;
                case ThreatSymbols.RWRSYM_Ab:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x + 0.02, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_Ac:
                    DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x - 0.01, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x + 0.1, y, big);
                    DrawTextCenteredVertival(drawingContext, "|", x + 0.03, y, big);
                    break;
                case ThreatSymbols.RWRSYM_MIB_F_S:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "S", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_MIB_F_A:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_MIB_F_M:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "M", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_MIB_F_U:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                    else
                        DrawTextCenteredVertival(drawingContext, "U", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_MIB_F_BW:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                    else
                    {
                        double[,] lines = {
		                    {0.090F,-0.030F},
                            {0.000F,0.060F},
                            {0.000F,0.010F},
                            {0.090F,-0.030F},
                            {-0.090F,-0.030F},
                            {0.000F,0.010F},
                            {0.000F,0.060F},
                            {-0.090F,-0.030F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_MIB_BW_S:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "S", x, y, big);
                    else
                    {
                        double[,] lines = {
		                    {0.090F,-0.030F},
                            {0.000F,0.060F},
                            {0.000F,0.010F},
                            {0.090F,-0.030F},
                            {-0.090F,-0.030F},
                            {0.000F,0.010F},
                            {0.000F,0.060F},
                            {-0.090F,-0.030F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_MIB_BW_A:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    else
                    {
                        double[,] lines = {
		                    {0.090F,-0.030F},
                            {0.000F,0.060F},
                            {0.000F,0.010F},
                            {0.090F,-0.030F},
                            {-0.090F,-0.030F},
                            {0.000F,0.010F},
                            {0.000F,0.060F},
                            {-0.090F,-0.030F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case ThreatSymbols.RWRSYM_MIB_BW_M:
                    if (show != 0)
                        DrawTextCenteredVertival(drawingContext, "M", x, y, big);
                    else
                    {
                        double[,] lines = {
		                    {0.090F,-0.030F},
                            {0.000F,0.060F},
                            {0.000F,0.010F},
                            {0.090F,-0.030F},
                            {-0.090F,-0.030F},
                            {0.000F,0.010F},
                            {0.000F,0.060F},
                            {-0.090F,-0.030F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                default:
                    if (symbol == 66)
                    {
                        if (show != 0)
                            DrawTextCenteredVertival(drawingContext, "F", x, y, big);
                        else
                            DrawTextCenteredVertival(drawingContext, "A", x, y, big);
                    }
                    else if (symbol >= 100)
                    {
                        if (customSymbol == '1')
                        {
                            DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                            DrawTextCenteredVertival(drawingContext, (symbol - 100).ToString(), x, y - 0.04, big);
                        }
                        else
                        {
                            DrawTextCenteredVertival(drawingContext, (symbol - 100).ToString(), x, y, big);
                        }
                    }
                    else if (symbol >= 65)
                    {
                        if (customSymbol == '1')
                        {
                            DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                            DrawTextCenteredVertival(drawingContext, ((char)symbol).ToString(), x, y - 0.04, big);
                        }
                        else
                        {
                            DrawTextCenteredVertival(drawingContext, ((char)symbol).ToString(), x, y, big);
                        }
                    }
                    else
                    {
                        DrawTextCenteredVertival(drawingContext, "U", x, y, big);
                    }
                    break;
            }
        }

        private void DrawSymbol(DrawingContext drawingContext, RWRSymbol symbol, double x, double y, bool big)
        {
            double scale = big ? 1.5 : 1;

            switch (symbol)
            {
                case RWRSymbol.Diamond:
                    double MARK_SIZE = 0.15 * scale;
                    SetDisplayOrigin(x, y);
                    DrawLine(drawingContext, 0.0, MARK_SIZE, MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, MARK_SIZE, -MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, -MARK_SIZE, MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, -MARK_SIZE, -MARK_SIZE, 0.0);
                    ZeroDisplayOrigin();
                    break;
                case RWRSymbol.Hat:
                    double[,] lines = {
		                {-0.050F,0.032F},
		                {0.000F,0.082F},
		                {0.000F,0.082F},
		                {0.050F,0.032F}};
                    SetDisplayOrigin(x, y);
                    for (int i = 1; i < lines.Length / 2; )
                    {
                        DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                        i++;
                        i++;
                    }
                    ZeroDisplayOrigin();
                    break;
                case RWRSymbol.MissileActivity:
                    if (flashFaster)
                    {
                        SetDisplayOrigin(x, y);
                        DrawCircle(drawingContext, 0, 0, 0.10);
                        ZeroDisplayOrigin();
                    }
                    break;
                case RWRSymbol.MissileLaunch:
                    SetDisplayOrigin(x, y);
                    DrawCircle(drawingContext, 0, 0, 0.10);
                    ZeroDisplayOrigin();
                    break;
                default:
                    break;
            }
        }

    }
}
