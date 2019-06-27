using LightningGauges.Renderers.F16.AzimuthIndicator;
using System;
using System.Windows;
using System.Windows.Media;

namespace LightningGauges.Renderers.F16.RWR.ALR93
{
    public class RWRScopeALR93Renderer : RWRRenderer
    {
        private enum RWRSymbol { Hat, Diamond, Lethal, MissileActivity, MissileLaunch };

        private const int BASELINE_CYCLE_TIME_ALR93 = 500;
        private const int CYCLE_TIME_PER_CONTACT_ALR93 = 50;
        private int flipFactor = 1;
        private int processTimeNav = Environment.TickCount;
        private double platformYaw = 0;
        private int flashTime = Environment.TickCount;
        private bool flash = false;
        private int flashFasterTime = Environment.TickCount;
        private bool flashFaster = false;
        private int show = 0;
        private bool wasflashing = false;

        public RWRScopeALR93Renderer()
        {

        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
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

            for (int i = 0; i < InstrumentState.RwrObjectCount; i++)
            {
                DrawContact(drawingContext, i);
            }

            if (RwrInfoContains("nnlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "NN", 0, -TextHeight(false) * 2, false);
            }
            else if (RwrInfoContains("onlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "ON", 0, -TextHeight(false) * 2, false);
            }

            if (RwrInfoContains("albl"))
            {
                DrawTextCenteredVertical(drawingContext, "A", -TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("slbl"))
            {
                DrawTextCenteredVertical(drawingContext, "S", -TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("clbl"))
            {
                DrawTextCenteredVertical(drawingContext, "C", -TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("dlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "D", -TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("mlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "M", -TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("elbl"))
            {
                DrawTextCenteredVertical(drawingContext, "E", TextWidth("M", false), 1 - TextHeight(false), false);
            }

            if (RwrInfoContains("mltlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "MLT", 0, -1 + TextHeight(false), false);
            }

            if (RwrInfoContains("fcrlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "FCR", 0, -1 + TextHeight(false), false);
            }

            if (RwrInfoContains("tfrlbl"))
            {
                DrawTextCenteredVertical(drawingContext, "TFR", 0, -1 + TextHeight(false), false);
            }

            if (RwrInfoContains("falbl"))
            {
                DrawTextCenteredVertical(drawingContext, "F", -0.99 + TextWidth("M", false), 0 - (TextHeight(false) / 2), false);
                DrawTextCenteredVertical(drawingContext, "A", -0.99 + TextWidth("M", false), 0 + (TextHeight(false) / 2), false);
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

            string lthList = new String('0', 40);
            if (RwrInfoContains("lthlst"))
            {
                lthList = RwrInfoGetKeyContent("lthlst").PadRight(40, '\0');
            }

            string mslList = new String('0', 40);
            if (RwrInfoContains("msllst"))
            {
                mslList = RwrInfoGetKeyContent("msllst").PadRight(40, '\0');
            }

            string lckList = new String('0', 40);
            if (RwrInfoContains("lcklst"))
            {
                lckList = RwrInfoGetKeyContent("lcklst").PadRight(40, '\0');
            }

            string trkList = new String('0', 40);
            if (RwrInfoContains("trklst"))
            {
                trkList = RwrInfoGetKeyContent("trklst").PadRight(40, '\0');
            }

            string bndList = new String('\0', 40);
            if (RwrInfoContains("bndlst"))
            {
                bndList = RwrInfoGetKeyContent("bndlst").PadRight(40, '\0');
            }

            string typList = new String('\0', 40);
            if (RwrInfoContains("typlst"))
            {
                typList = RwrInfoGetKeyContent("typlst").PadRight(40, '\0');
            }

            string csList = new String('\0', 40);
            if (RwrInfoContains("cslst"))
            {
                csList = RwrInfoGetKeyContent("cslst").PadRight(40, '\0');
            }

            string priList = new String('\0', 40);
            if (RwrInfoContains("prilst"))
            {
                priList = RwrInfoGetKeyContent("prilst").PadRight(40, '\0');
            }

            DrawEmitterSymbol(drawingContext, symbol, xpos, ypos, lckList[contact], trkList[contact], bndList[contact], typList[contact], csList[contact], priList[contact], big);

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

            if (lthList[contact] == '1')
            {
                DrawSymbol(drawingContext, RWRSymbol.Lethal, xpos, ypos, false);
            }

            if (mslList[contact] == '1')
            {
                DrawSymbol(drawingContext, RWRSymbol.MissileLaunch, xpos, ypos, false);
            }
        }

        private void DrawEmitterSymbol(DrawingContext drawingContext, int symbol, double x, double y, char locked, char tracking, char band, char type, char customSymbol, char pri, bool big)
        {
            double scale = big ? 1.5 : 1;

            double[,] basicAir = {
                {0.090F,-0.030F},
                {0.000F,0.060F},
                {0.000F,0.010F},
                {0.090F,-0.030F},
                {-0.090F,-0.030F},
                {0.000F,0.010F},
                {0.000F,0.060F},
                {-0.090F,-0.030F}};

            double[,] advAir =  {
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

            double[,] theBoat = {
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

            double[,] theSearch = {
                {0.000F,-0.050F},
                {0.000F,0.038F},
                {-0.024F,-0.050F},
                {0.024F,-0.050F},
                {0.014F,0.054F},
                {-0.016F,0.020F},
                {0.014F,0.054F},
                {0.024F,0.090F},
                {-0.054F,0.010F},
                {-0.016F,0.020F}};

            double[,] theSam = {
                {-0.056F,0.060F},
                {-0.096F,-0.074F},
                {-0.056F,0.060F},
                {-0.078F,0.028F},
                {-0.056F,0.060F},
                {-0.054F,0.020F}};

            double[,] theAAA = {
                {-0.098F,0.038F},
                {-0.098F,-0.100F},
                {-0.098F,0.038F},
                {0.098F,0.038F},
                {0.098F,-0.100F},
                {0.098F,0.038F},
                {-0.098F,0.038F},
                {0.098F,0.098F},
                {0.098F,-0.100F},
                {-0.098F,-0.100F}};

            if (symbol >= 0)
            {
                ThreatSymbols knownSymbol = (ThreatSymbols)symbol;

                switch (knownSymbol)
                {
                    //case ThreatSymbols.RWRSYM_NONE:
                    //    break;
                    /*
                    case ThreatSymbols.RWRSYM_ADVANCED_INTERCEPTOR:
                        break;
                    case ThreatSymbols.RWRSYM_BASIC_INTERCEPTOR:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_ACTIVE_MISSILE:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        DrawTextCenteredVertical(drawingContext, "AM", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_HAWK:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        DrawTextCenteredVertical(drawingContext, " MQ", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_PATRIOT:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " PT", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "P", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA2:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 2T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "2", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA3:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 3T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "3", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA4:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 4T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "4", x, y, big);
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_SA5:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_SA6:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 6T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "6", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA8:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 8T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "8", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA9:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " 9T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "9", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA10:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        DrawTextCenteredVertical(drawingContext, " 10", x, y, big);
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_SA13:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_AAA:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        if (tracking == '1')
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3T", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4T", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        else
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        break;
                    case ThreatSymbols.RWRSYM_SEARCH:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSearch.Length / 2;)
                        {
                            DrawLine(drawingContext, theSearch[i, 0] * scale, theSearch[i, 1] * scale, theSearch[i - 1, 0] * scale, theSearch[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        switch (band)
                        {
                            case 'E':
                                DrawTextCenteredVertical(drawingContext, "1", x + 0.07, y + 0.02f, big);
                                break;
                            case 'F':
                                DrawTextCenteredVertical(drawingContext, "1", x + 0.07, y + 0.02f, big);
                                break;
                            case 'G':
                                DrawTextCenteredVertical(drawingContext, "2", x + 0.07, y + 0.02f, big);
                                break;
                            case 'H':
                                DrawTextCenteredVertical(drawingContext, "2", x + 0.07, y + 0.02f, big);
                                break;
                            case 'I':
                                DrawTextCenteredVertical(drawingContext, "3", x + 0.07, y + 0.02f, big);
                                break;
                            case 'J':
                                DrawTextCenteredVertical(drawingContext, "4", x + 0.07, y + 0.02f, big);
                                break;
                            default:
                                DrawTextCenteredVertical(drawingContext, "1", x + 0.07, y + 0.02f, big);
                                break;
                        }
                        break;
                    case ThreatSymbols.RWRSYM_NAVAL:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " FT", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "F", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_CHAPARAL:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " CT", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "C", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_SA15:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (locked == '1')
                            DrawTextCenteredVertical(drawingContext, " 5T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "5", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_NIKE:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " NT", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "N", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_A1:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        if (tracking == '1')
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3T", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4T", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        else
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        break;
                    case ThreatSymbols.RWRSYM_A2:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        if (tracking == '1')
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3T", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4T", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        else
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        break;
                    case ThreatSymbols.RWRSYM_A3:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();

                        if (tracking == '1')
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3T", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4T", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        else
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04f, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "3", x, y - 0.04f, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "4", x, y - 0.04f, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04f, big);
                                    break;
                            }
                        }
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_PDOT:
                        break;
                    case ThreatSymbols.RWRSYM_PSLASH:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_UNK1:
                        switch (band)
                        {
                            case 'E':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'F':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'G':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'H':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'I':
                                DrawTextCenteredVertical(drawingContext, "13", x, y, big);
                                break;
                            case 'J':
                                DrawTextCenteredVertical(drawingContext, "14", x, y, big);
                                break;
                            default:
                                DrawTextCenteredVertical(drawingContext, "1", x, y, big);
                                break;
                        }
                        break;
                    case ThreatSymbols.RWRSYM_UNK2:
                        switch (band)
                        {
                            case 'E':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'F':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'G':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'H':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'I':
                                DrawTextCenteredVertical(drawingContext, "13", x, y, big);
                                break;
                            case 'J':
                                DrawTextCenteredVertical(drawingContext, "14", x, y, big);
                                break;
                            default:
                                DrawTextCenteredVertical(drawingContext, "1", x, y, big);
                                break;
                        }
                        break;
                    case ThreatSymbols.RWRSYM_UNK3:
                        switch (band)
                        {
                            case 'E':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'F':
                                DrawTextCenteredVertical(drawingContext, "11", x, y, big);
                                break;
                            case 'G':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'H':
                                DrawTextCenteredVertical(drawingContext, "12", x, y, big);
                                break;
                            case 'I':
                                DrawTextCenteredVertical(drawingContext, "13", x, y, big);
                                break;
                            case 'J':
                                DrawTextCenteredVertical(drawingContext, "14", x, y, big);
                                break;
                            default:
                                DrawTextCenteredVertical(drawingContext, "1", x, y, big);
                                break;
                        }
                        break;
                    case ThreatSymbols.RWRSYM_KSAM:
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " CT", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "C", x, y, big);
                        break;
                    case ThreatSymbols.RWRSYM_V4:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "4T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "4", x, y - 0.04, big);
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_V5:
                        break;
                    case ThreatSymbols.RWRSYM_V6:
                        break;
                    case ThreatSymbols.RWRSYM_V14:
                        break;
                    case ThreatSymbols.RWRSYM_V15:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_V16:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "6T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "6", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V18:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "8T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "8", x, y - 0.04, big);
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_V19:
                        break;
                    case ThreatSymbols.RWRSYM_V20:
                        break;
                     */
                    case ThreatSymbols.RWRSYM_V21:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "1T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "1", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V22:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "2T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "2", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V23:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "3T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "3", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V25:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "5T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "5", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V27:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "7T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "7", x, y - 0.04, big);
                        break;
                    case ThreatSymbols.RWRSYM_V29:
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, "9T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, "9", x, y - 0.04, big);
                        break;
                    /*
                    case ThreatSymbols.RWRSYM_V30:
                        break;
                    case ThreatSymbols.RWRSYM_V31:
                        break;
                    case ThreatSymbols.RWRSYM_VP:
                        break;
                    case ThreatSymbols.RWRSYM_VPD:
                        break;
                    case ThreatSymbols.RWRSYM_VA:
                        break;
                    case ThreatSymbols.RWRSYM_VB:
                        break;
                    case ThreatSymbols.RWRSYM_VS:
                        break;
                    case ThreatSymbols.RWRSYM_Aa:
                        break;
                    case ThreatSymbols.RWRSYM_Ab:
                        break;
                    case ThreatSymbols.RWRSYM_Ac:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_F_S:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_F_A:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_F_M:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_F_U:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_F_BW:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_BW_S:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_BW_A:
                        break;
                    case ThreatSymbols.RWRSYM_MIB_BW_M:
                        break;
                     */
                    default:
                        if (symbol >= 65 && symbol < 100)
                        {
                            if (customSymbol == '1')
                            {
                                DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                                if (tracking == '1')
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y - 0.04, big);
                                else
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y - 0.04, big);
                            }
                            else if (customSymbol == '2')
                            {
                                SetDisplayOrigin(x, y);
                                for (int i = 1; i < theSam.Length / 2;)
                                {
                                    DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                                    i++;
                                    i++;
                                }
                                ZeroDisplayOrigin();
                                if (tracking == '1')
                                    DrawTextCenteredVertical(drawingContext, " " + ((char)symbol).ToString() + "T", x, y, big);
                                else
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y, big);
                            }
                            else if (customSymbol == '3')
                            {
                                SetDisplayOrigin(x, y);
                                for (int i = 1; i < theAAA.Length / 2;)
                                {
                                    DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                                    i++;
                                    i++;
                                }
                                ZeroDisplayOrigin();
                                if (tracking == '1')
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y - 0.04, big);
                                else
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y - 0.04, big);
                            }
                            else if (customSymbol == '4')
                            {
                                SetDisplayOrigin(x, y);
                                for (int i = 1; i < theSearch.Length / 2;)
                                {
                                    DrawLine(drawingContext, theSearch[i, 0] * scale, theSearch[i, 1] * scale, theSearch[i - 1, 0] * scale, theSearch[i - 1, 1] * scale);
                                    i++;
                                    i++;
                                }
                                ZeroDisplayOrigin();
                                DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x + 0.07, y + 0.02f, big);
                            }
                            else
                            {
                                if (tracking == '1')
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y, big);
                                else
                                    DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y, big);
                            }
                        }
                        else if (type == 'C')
                        {
                            switch (band)
                            {
                                case 'E':
                                    DrawTextCenteredVertical(drawingContext, "CW1", x, y, big);
                                    break;
                                case 'F':
                                    DrawTextCenteredVertical(drawingContext, "CW1", x, y, big);
                                    break;
                                case 'G':
                                    DrawTextCenteredVertical(drawingContext, "CW2", x, y, big);
                                    break;
                                case 'H':
                                    DrawTextCenteredVertical(drawingContext, "CW2", x, y, big);
                                    break;
                                case 'I':
                                    DrawTextCenteredVertical(drawingContext, "CW3", x, y, big);
                                    break;
                                case 'J':
                                    DrawTextCenteredVertical(drawingContext, "CW4", x, y, big);
                                    break;
                                default:
                                    DrawTextCenteredVertical(drawingContext, "CW", x, y, big);
                                    break;
                            }
                        }
                        else
                        {
                            string buffer = pri.ToString();

                            switch (band)
                            {
                                case 'E':
                                    buffer += "1";
                                    break;
                                case 'F':
                                    buffer += "1";
                                    break;
                                case 'G':
                                    buffer += "2";
                                    break;
                                case 'H':
                                    buffer += "2";
                                    break;
                                case 'I':
                                    buffer += "3";
                                    break;
                                case 'J':
                                    buffer += "4";
                                    break;
                                default:
                                    buffer += "";
                                    break;
                            }

                            DrawTextCenteredVertical(drawingContext, buffer, x, y, big);
                        }
                        break;
                }
            }
            else
            {
                symbol = -symbol;

                if (symbol >= 100)
                {
                    if (customSymbol == '1')
                    {
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1' && symbol < 110)
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString() + "T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString(), x, y - 0.04, big);
                    }
                    else if (customSymbol == '2')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1' && symbol < 110)
                            DrawTextCenteredVertical(drawingContext, " " + (symbol - 100).ToString() + "T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString(), x, y, big);
                    }
                    else if (customSymbol == '3')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1' && symbol < 110)
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString() + "T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString(), x, y - 0.04, big);
                    }
                    else if (customSymbol == '4')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSearch.Length / 2;)
                        {
                            DrawLine(drawingContext, theSearch[i, 0] * scale, theSearch[i, 1] * scale, theSearch[i - 1, 0] * scale, theSearch[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString(), x + 0.07, y + 0.02f, big);
                    }
                    else
                    {
                        if (tracking == '1' && symbol < 110)
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString() + "T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, (symbol - 100).ToString(), x, y, big);
                    }
                }
                else if (symbol >= 48)
                {
                    if (customSymbol == '1')
                    {
                        DrawSymbol(drawingContext, RWRSymbol.Hat, x, y, big);
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y - 0.04, big);
                    }
                    else if (customSymbol == '2')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSam.Length / 2;)
                        {
                            DrawLine(drawingContext, theSam[i, 0] * scale, theSam[i, 1] * scale, theSam[i - 1, 0] * scale, theSam[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, " " + ((char)symbol).ToString() + "T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y, big);
                    }
                    else if (customSymbol == '3')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theAAA.Length / 2;)
                        {
                            DrawLine(drawingContext, theAAA[i, 0] * scale, theAAA[i, 1] * scale, theAAA[i - 1, 0] * scale, theAAA[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y - 0.04, big);
                        else
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y - 0.04, big);
                    }
                    else if (customSymbol == '4')
                    {
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < theSearch.Length / 2;)
                        {
                            DrawLine(drawingContext, theSearch[i, 0] * scale, theSearch[i, 1] * scale, theSearch[i - 1, 0] * scale, theSearch[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                        DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x + 0.07, y + 0.02f, big);
                    }
                    else
                    {
                        if (tracking == '1')
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant() + "T", x, y, big);
                        else
                            DrawTextCenteredVertical(drawingContext, ((char)symbol).ToString().ToUpperInvariant(), x, y, big);
                    }
                }
                else
                {
                    string buffer = pri.ToString();

                    switch (band)
                    {
                        case 'E':
                            buffer += "1";
                            break;
                        case 'F':
                            buffer += "1";
                            break;
                        case 'G':
                            buffer += "2";
                            break;
                        case 'H':
                            buffer += "2";
                            break;
                        case 'I':
                            buffer += "3";
                            break;
                        case 'J':
                            buffer += "4";
                            break;
                        default:
                            buffer += "";
                            break;
                    }

                    DrawTextCenteredVertical(drawingContext, buffer, x, y, big);
                }
            }
        }

        private void DrawSymbol(DrawingContext drawingContext, RWRSymbol symbol, double x, double y, bool big)
        {
            double scale = big ? 1.5 : 1;

            switch (symbol)
            {
                case RWRSymbol.Diamond:
                    double MARK_SIZE = 0.25;
                    SetDisplayOrigin(x, y);
                    DrawLine(drawingContext, 0.0, MARK_SIZE, MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, MARK_SIZE, -MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, -MARK_SIZE, MARK_SIZE, 0.0);
                    DrawLine(drawingContext, 0.0, -MARK_SIZE, -MARK_SIZE, 0.0);
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
                case RWRSymbol.Hat:
                    {
                        double[,] lines = {
                        {0.000F,0.082F},
                        {-0.084F,0.050F},
                        {0.000F,0.082F},
                        {0.084F,0.050F}};
                        SetDisplayOrigin(x, y);
                        for (int i = 1; i < lines.Length / 2;)
                        {
                            DrawLine(drawingContext, lines[i, 0] * scale, lines[i, 1] * scale, lines[i - 1, 0] * scale, lines[i - 1, 1] * scale);
                            i++;
                            i++;
                        }
                        ZeroDisplayOrigin();
                    }
                    break;
                case RWRSymbol.MissileActivity:
                    if (flashFaster)
                    {
                        scale = 1.3;
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
                        for (int i = 1; i < lines.Length / 2;)
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
                        scale = 1.3;
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
                        for (int i = 1; i < lines.Length / 2;)
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
