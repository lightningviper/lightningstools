using LightningGauges.Renderers.F16.AzimuthIndicator;
using System;
using System.Windows;
using System.Windows.Media;

namespace LightningGauges.Renderers.F16.RWR.ALR67
{
    public class RWRScopeALR67Renderer : RWRRenderer
    {
        private enum RWRSymbol { NewDetection, Diamond, Hat, MissileActivity, MissileLaunch, Lethal };

        private const int BASELINE_CYCLE_TIME_ALR67 = 500;
        private const int CYCLE_TIME_PER_CONTACT_ALR67 = 100;
        private int cycleduration = BASELINE_CYCLE_TIME_ALR67;
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

        public RWRScopeALR67Renderer()
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
                if (processTimeNavDt > 5000)
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

            for (int i = 0; i < InstrumentState.RwrObjectCount; i++)
            {
                DrawContact(drawingContext, i);
            }

            if (RwrInfoContains("nlbl"))
            {
                DrawTextCenteredVertival(drawingContext, "N", -TextWidth("M", false), TextHeight(false) * 0.5, false);
            }

            if (RwrInfoContains("llbl"))
            {
                DrawTextCenteredVertival(drawingContext, "L", TextWidth("L", false), TextHeight(false) * 0.5, false);
            }

            if (RwrInfoContains("blbl"))
            {
                DrawTextCenteredVertival(drawingContext, "B", 0, -TextHeight(false) * 0.5, false);
            }
        }

        private bool RwrInfoContains(string key)
        {
            for (int i = 0; i < rwrInfo.Length; i++)
            {
                if (rwrInfo[i].StartsWith(key + ">")) return true;
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
            cycleduration = BASELINE_CYCLE_TIME_ALR67 + (InstrumentState.RwrObjectCount * CYCLE_TIME_PER_CONTACT_ALR67);
            if ((cycletimer == 0) || (Environment.TickCount > cycletimer))
            {
                cycletimeflipper = cycletimeflipper * -1.0F;
                cycletimer = Environment.TickCount + cycleduration;
            }
            return cycletimeflipper;
        }

        private void DrawContact(DrawingContext drawingContext, int contact)
        {
            string lckList = new String('0', 40);
            if (RwrInfoContains("lcklst"))
            {
                lckList = RwrInfoGetKeyContent("lcklst").PadRight(40, '\0');
            }

            string bndList = new String('\0', 40);
            if (RwrInfoContains("bndlst"))
            {
                bndList = RwrInfoGetKeyContent("bndlst").PadRight(40, '\0');
            }

            string csList = new String('\0', 40);
            if (RwrInfoContains("cslst"))
            {
                csList = RwrInfoGetKeyContent("cslst").PadRight(40, '\0');
            }

            if ((InstrumentState.missileActivity[contact] == 1 || InstrumentState.missileLaunch[contact] == 1 || lckList[contact] == '1') && flash)
            {
                return;
            }

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

            DrawEmitterSymbol(drawingContext, symbol, xpos, ypos, bndList[contact], csList[contact], big);

            if (InstrumentState.selected[contact] != 0)
            {
                DrawSymbol(drawingContext, RWRSymbol.Lethal, xpos, ypos, false);
            }
        }

        private void DrawEmitterSymbol(DrawingContext drawingContext, int symbol, double x, double y, char band, char customSymbol, bool big)
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
		                    {-0.078F,-0.046F},
		                    {-0.052F,-0.076F},
		                    {0.076F,-0.046F},
		                    {0.052F,-0.076F},
		                    {-0.052F,-0.076F},
		                    {0.052F,-0.076F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2; )
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        string buffer = string.Empty;

		                switch(band)
		                {
			                case 'C':
				                buffer = "1";
				                break;
			                case 'D':
				                buffer = "2";
				                break;
			                case 'E':
				                buffer = "3";
				                break;
			                case 'F':
				                buffer = "4";
				                break;
			                case 'G':
				                buffer = "5";
				                break;
			                case 'H':
				                buffer = "6";
				                break;
			                case 'I':
				                buffer = "7";
				                break;
			                case 'J':
				                buffer = "8";
				                break;
			                case 'K':
				                buffer = "9";
				                break;
			                default:
				                buffer = "*";
				                break;
		                }

                        DrawTextCenteredVertival(drawingContext, buffer, x, y, big);
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
                case RWRSymbol.Lethal:
                    double size = 0.13;
                    SetDisplayOrigin(x, y);
                    DrawLine(drawingContext, size, size, -size, size);
                    DrawLine(drawingContext, size, -size, -size, -size);
                    DrawLine(drawingContext, size, size, size, -size);
                    DrawLine(drawingContext, -size, size, -size, -size);
                    ZeroDisplayOrigin();
                    break;
                default:
                    break;
            }
        }

    }
}
