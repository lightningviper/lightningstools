using LightningGauges.Renderers.F16.AzimuthIndicator;
using System;
using System.Windows;
using System.Windows.Media;

namespace LightningGauges.Renderers.F16.RWR.CARAPACE
{
    public class RWRScopeCARAPACERenderer : RWRRenderer
    {
        private enum RWRSymbol { Hat, NewDetection, Diamond, MissileActivity, MissileLaunch, Lethal };

        private const int BASELINE_CYCLE_TIME_CARAPACE = 500;
        private const int CYCLE_TIME_PER_CONTACT_CARAPACE = 50;
        private int cycleduration = BASELINE_CYCLE_TIME_CARAPACE;
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

        public RWRScopeCARAPACERenderer()
        {

        }

        public override void Render(DrawingContext drawingContext)
        {
            string rwrInfoBuffer = string.Empty;
            for (int i = 0; i < InstrumentState.RwrInfo.Length; i++)
            {
                rwrInfoBuffer += (char)InstrumentState.RwrInfo[i];
            }

            rwrInfo = rwrInfoBuffer.Split('<');

            int processTimeNavDt = Environment.TickCount - processTimeNav;

            if (!RwrInfoContains("navfault"))
            {
                if (processTimeNavDt > 200)
                {
                    processTimeNav = Environment.TickCount;
                    platformYaw = InstrumentState.yaw;
                }
            }
            else
            {
                if (processTimeNavDt > 2000)
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
            if (flashFasterTimeDt >= 256)
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

            if (!RwrInfoContains("onbit"))
            {
                for (int i = 0; i < 12; i++)
                {
                    double y1 = 0.65 * Math.Sin(-(i * (45 * DTR)));
                    double x1 = 0.65 * Math.Cos(-(i * (45 * DTR)));
                    double y2 = 0.65 * Math.Sin(-((i * (45 * DTR)) + (22.5 * DTR)));
                    double x2 = 0.65 * Math.Cos(-((i * (45 * DTR)) + (22.5 * DTR)));
                    DrawLine(drawingContext, x1, y1, x2, y2);
                }

                for (int i = 0; i < 12; i++)
                {
                    double y1 = 0.3 * Math.Sin(-(i * (45 * DTR)));
                    double x1 = 0.3 * Math.Cos(-(i * (45 * DTR)));
                    double y2 = 0.3 * Math.Sin(-((i * (45 * DTR)) + (22.5 * DTR)));
                    double x2 = 0.3 * Math.Cos(-((i * (45 * DTR)) + (22.5 * DTR)));
                    DrawLine(drawingContext, x1, y1, x2, y2);
                }

                for (int i = 0; i < 12; i++)
                {
                    double y = 0.9 * Math.Sin(i * (30 * DTR));
                    double x = 0.9 * Math.Cos(i * (30 * DTR));
                    DrawSolidCircle(drawingContext, x, y, 0.02);
                }

                if (!RwrInfoContains("navfault"))
                {
                    DrawLine(drawingContext, 0.0, -0.05, 0.0, 0.05);
                    DrawLine(drawingContext, -0.05, 0.0, 0.05, 0.0);
                }

                for (int i = 0; i < InstrumentState.RwrObjectCount; i++)
                {
                    DrawContact(drawingContext, i);
                }

                if (RwrInfoContains("onpri"))
                {
                    DrawSolidBox(drawingContext, -TextWidth("000", false), -1 + (TextHeight(false) * 2), TextWidth("00", false), -1 + TextHeight(false), Color.FromRgb(7, 18, 8));
                    DrawLine(drawingContext, -TextWidth("000", false), -1 + (TextHeight(false) * 2), TextWidth("00", false), -1 + (TextHeight(false) * 2));
                    DrawLine(drawingContext, -TextWidth("000", false), -1 + TextHeight(false), TextWidth("00", false), -1 + TextHeight(false));
                    DrawLine(drawingContext, 0, -1 + TextHeight(false), 0, -1 + (TextHeight(false) * 2));
                    DrawLine(drawingContext, -TextWidth("000", false), -1 + TextHeight(false), -TextWidth("000", false), -1 + (TextHeight(false) * 2));
                    DrawLine(drawingContext, TextWidth("00", false), -1 + TextHeight(false), TextWidth("00", false), -1 + (TextHeight(false) * 2));

                    if (RwrInfoContains("prih"))
                    {
                        DrawTextRight(drawingContext, RwrInfoGetKeyContent("prih"), 0, -1 + (TextHeight(false) * 2), false);
                    }

                    if (RwrInfoContains("prir"))
                    {
                        DrawTextLeft(drawingContext, RwrInfoGetKeyContent("prir"), 0, -1 + (TextHeight(false) * 2), false);
                    }
                }
            }

            if (RwrInfoContains("on1bit"))
            {
                double x = -TextWidth("M", false) * 5;
                double y = TextHeight(false) * 3.2;
                y -= TextHeight(false) * 1.4;
                DrawTextLeft(drawingContext, "MTP ON 001", x, y, false);
                y -= TextHeight(false) * 1.4;
                DrawTextLeft(drawingContext, string.Format("C {0} F {1}", InstrumentState.ChaffCount, InstrumentState.FlareCount), x, y, false);
                y -= TextHeight(false) * 1.4;
                DrawTextLeft(drawingContext, "OFP 1298", x - TextWidth("M", false), y, false);
                y -= TextHeight(false) * 1.4;
                DrawTextLeft(drawingContext, "TRIT 0268", x - TextWidth("M", false) * 3, y, false);
            }

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
            cycleduration = BASELINE_CYCLE_TIME_CARAPACE + (InstrumentState.RwrObjectCount * CYCLE_TIME_PER_CONTACT_CARAPACE);
            if ((cycletimer == 0) || (Environment.TickCount > cycletimer))
            {
                cycletimeflipper = cycletimeflipper * -1.0F;
                cycletimer = Environment.TickCount + cycleduration;
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

            string trkList = new String('0', 40);
            if (RwrInfoContains("trklst"))
            {
                trkList = RwrInfoGetKeyContent("trklst").PadRight(40, '\0');
            }

            string csList = new String('\0', 40);
            if (RwrInfoContains("cslst"))
            {
                csList = RwrInfoGetKeyContent("cslst").PadRight(40, '\0');
            }

            if (InstrumentState.newDetection[contact] != 0)
            {
                DrawSymbol(drawingContext, RWRSymbol.Diamond, xpos, ypos, false);
                DrawSymbol(drawingContext, RWRSymbol.NewDetection, xpos, ypos, false);
            }
            else
            {
                if (InstrumentState.missileActivity[contact] == 0 && trkList[contact] == '1')
                {
                    DrawSymbol(drawingContext, RWRSymbol.MissileLaunch, xpos, ypos, false);
                }
                else if (InstrumentState.missileActivity[contact] != 0 || InstrumentState.missileLaunch[contact] != 0)
                {
                    DrawSymbol(drawingContext, RWRSymbol.MissileActivity, xpos, ypos, false);
                }
                else
                {
                    DrawSymbol(drawingContext, RWRSymbol.Diamond, xpos, ypos, false);
                }

                if (InstrumentState.selected[contact] != 0)
                {
                    DrawSymbol(drawingContext, RWRSymbol.Lethal, xpos, ypos, false);
                }
            }

            DrawEmitterSymbol(drawingContext, symbol, xpos, ypos, csList[contact], big);
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
                    DrawTextCenteredVertival(drawingContext, "A1", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_A2:
                    DrawTextCenteredVertival(drawingContext, "A2", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_A3:
                    DrawTextCenteredVertival(drawingContext, "A3", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_PDOT:
                    DrawTextCenteredVertival(drawingContext, "P1", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_PSLASH:
                    DrawTextCenteredVertival(drawingContext, "P2", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK1:
                    DrawTextCenteredVertival(drawingContext, "U1", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK2:
                    DrawTextCenteredVertival(drawingContext, "U2", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_UNK3:
                    DrawTextCenteredVertival(drawingContext, "U3", x, y, big);
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
                    DrawTextCenteredVertival(drawingContext, "A1", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_Ab:
                    DrawTextCenteredVertival(drawingContext, "A2", x, y, big);
                    break;
                case ThreatSymbols.RWRSYM_Ac:
                    DrawTextCenteredVertival(drawingContext, "A3", x, y, big);
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
                case RWRSymbol.Hat:
                    {
                        double[,] lines = {
		               {0.000F,0.086F},
		                {-0.066F,0.054F},
		                {0.000F,0.086F},
		                {0.066F,0.054F},
		                {-0.066F,0.054F},
		                {0.066F,0.054F}};
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
                case RWRSymbol.Lethal:
                    {
                        double size = 0.15;
                        SetDisplayOrigin(x, y);
                        DrawLine(drawingContext, size, size, -size, size);
                        DrawLine(drawingContext, size, -size, -size, -size);
                        DrawLine(drawingContext, size, size, size, -size);
                        DrawLine(drawingContext, -size, size, -size, -size);
                        ZeroDisplayOrigin();
                    }
                    break;
                case RWRSymbol.NewDetection:
                    {
                        double MARK_SIZE = 0.15 * scale;
                        double spotSize = MARK_SIZE * 0.166;
                        double position = MARK_SIZE - (spotSize / 2);
                        SetDisplayOrigin(x, y);
                        DrawSolidCircle(drawingContext, position, position, spotSize);
                        DrawSolidCircle(drawingContext, -position, position, spotSize);
                        DrawSolidCircle(drawingContext, position, -position, spotSize);
                        DrawSolidCircle(drawingContext, -position, -position, spotSize);
                        ZeroDisplayOrigin();
                    }
                    break;
                case RWRSymbol.Diamond:
                    {
                        double MARK_SIZE = 0.15 * scale;
                        SetDisplayOrigin(x, y);
                        DrawLine(drawingContext, 0.0, MARK_SIZE, MARK_SIZE, 0.0);
                        DrawLine(drawingContext, 0.0, MARK_SIZE, -MARK_SIZE, 0.0);
                        DrawLine(drawingContext, 0.0, -MARK_SIZE, MARK_SIZE, 0.0);
                        DrawLine(drawingContext, 0.0, -MARK_SIZE, -MARK_SIZE, 0.0);
                        ZeroDisplayOrigin();
                    }
                    break;
                case RWRSymbol.MissileActivity:
                    if (flashFaster)
                    {
                        scale = 1.5f;
                        SetDisplayOrigin(x, y);
                        double[,] lines = {
		                {-0.100F,-0.050F},
				        {-0.100F,0.050F},
				        {-0.050F,0.100F},
				        {0.050F,0.100F},
				        {0.050F,-0.100F},
				        {-0.050F,-0.100F},
				        {0.100F,0.050F},
				        {0.100F,-0.050F},
				        {-0.100F,0.050F},
				        {-0.050F,0.100F},
				        {0.050F,0.100F},
				        {0.100F,0.050F},
				        {-0.100F,-0.050F},
				        {-0.050F,-0.100F},
				        {0.100F,-0.050F},
				        {0.050F,-0.100F}};
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
                case RWRSymbol.MissileLaunch:
                    {
                        scale = 1.5f;
                        SetDisplayOrigin(x, y);
                        double[,] lines = {
		                {-0.100F,-0.050F},
				        {-0.100F,0.050F},
				        {-0.050F,0.100F},
				        {0.050F,0.100F},
				        {0.050F,-0.100F},
				        {-0.050F,-0.100F},
				        {0.100F,0.050F},
				        {0.100F,-0.050F},
				        {-0.100F,0.050F},
				        {-0.050F,0.100F},
				        {0.050F,0.100F},
				        {0.100F,0.050F},
				        {-0.100F,-0.050F},
				        {-0.050F,-0.100F},
				        {0.100F,-0.050F},
				        {0.050F,-0.100F}};
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
                    break;
            }
        }

    }
}
