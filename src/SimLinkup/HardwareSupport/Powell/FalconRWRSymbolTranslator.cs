using System;
using System.Collections.Generic;
using System.Linq;
using Common.Math;

namespace SimLinkup.HardwareSupport.Powell
{
    internal interface IFalconRWRSymbolTranslator
    {
        IEnumerable<Blip> Translate(FalconRWRSymbol falconRWRSymbol, double magneticHeadingDegrees,
            bool usePrimarySymbol = true, bool inverted = false);
    }

    internal class FalconRWRSymbolTranslator : IFalconRWRSymbolTranslator
    {
        public IEnumerable<Blip> Translate(FalconRWRSymbol falconRWRSymbol, double magneticHeadingDegrees,
            bool primarySymbol = true, bool inverted = false)
        {
            var lethality = falconRWRSymbol.Lethality;

            var radius = lethality > 1
                ? -((2.0f - lethality) * 100.0f) * 0.95f
                : -((1.0f - lethality) * 100.0f) * 0.95f;

            var angleDegrees = //-magneticHeadingDegrees +
                falconRWRSymbol.BearingDegrees;
            if (inverted)
            {
                angleDegrees = -angleDegrees;
            }
            var angleRadians = angleDegrees * Constants.RADIANS_PER_DEGREE;

            var blips = new List<Blip>();
            var x = (byte) (radius * Math.Cos(angleRadians) * 255);
            var y = (byte) (radius * Math.Sin(angleRadians) * 255);
            const byte lineSpacingYOffset = (byte) 15;
            const byte charSpacingXOffset = (byte) 15;
            const byte charSpacingHalfXOffset = (byte) 8;
            switch (falconRWRSymbol.SymbolId)
            {
                case 0:
                    break;
                case 1: //U
                    blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    break;
                case 2: //advanced interceptor
                    blips.Add(new Blip {Symbol = Symbols.AdvancedInterceptor, X = x, Y = y});
                    break;
                case 3: //basic interceptor
                    blips.Add(new Blip {Symbol = Symbols.BasicInterceptor, X = x, Y = y});
                    break;
                case 4: //M
                    blips.Add(new Blip {Symbol = Symbols.M, X = x, Y = y});
                    break;
                case 5: //H
                    blips.Add(new Blip {Symbol = Symbols.H, X = x, Y = y});
                    break;
                case 6: //P
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = y});
                    break;
                case 7: //2
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = y});
                    break;
                case 8: //3
                    blips.Add(new Blip {Symbol = Symbols.Three, X = x, Y = y});
                    break;
                case 9: //4
                    blips.Add(new Blip {Symbol = Symbols.Four, X = x, Y = y});
                    break;
                case 10: //5
                    blips.Add(new Blip {Symbol = Symbols.Five, X = x, Y = y});
                    break;
                case 11: //6
                    blips.Add(new Blip {Symbol = Symbols.Six, X = x, Y = y});
                    break;
                case 12: //8
                    blips.Add(new Blip {Symbol = Symbols.Eight, X = x, Y = y});
                    break;
                case 13: //9
                    blips.Add(new Blip {Symbol = Symbols.Nine, X = x, Y = y});
                    break;
                case 14: //10
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Zero, X = (byte) (x + charSpacingXOffset), Y = y});
                    break;
                case 15: //13
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Three, X = (byte) (x + charSpacingXOffset), Y = y});
                    break;
                case 16: //alternating A or S
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.A, X = x, Y = y}
                        : new Blip {Symbol = Symbols.S, X = x, Y = y});
                    break;
                case 17: //S
                    blips.Add(new Blip {Symbol = Symbols.S, X = x, Y = y});
                    break;
                case 18: //ship
                    blips.Add(new Blip {Symbol = Symbols.Ship, X = x, Y = y});
                    break;
                case 19: //C
                    blips.Add(new Blip {Symbol = Symbols.C, X = x, Y = y});
                    break;
                case 20: //alternating 15 or M
                    if (primarySymbol)
                    {
                        blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = y});
                        blips.Add(new Blip {Symbol = Symbols.Five, X = (byte) (x + charSpacingXOffset), Y = y});
                    }
                    else
                    {
                        blips.Add(new Blip {Symbol = Symbols.M, X = x, Y = y});
                    }
                    break;
                case 21: //N
                    blips.Add(new Blip {Symbol = Symbols.N, X = x, Y = y});
                    break;
                case 22: //A with . underneath
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Dot, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 23: //A with .. underneath
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x - charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x + charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 24: //A with ... underneath
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Dot, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x - charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x + charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 25: //P with . underneath
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Dot, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 26: //P|
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x + charSpacingXOffset), Y = y});
                    break;
                case 27: //U with . underneath
                    blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Dot, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 28: //U with .. underneath
                    blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x - charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x + charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 29: //U with ... underneath
                    blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Dot, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x - charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Dot,
                        X = (byte) (x + charSpacingHalfXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 30: //C
                    blips.Add(new Blip {Symbol = Symbols.C, X = x, Y = y});
                    break;
                case 31: //airborne threat symbol with 1 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 32: //airborne threat symbol with 4 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Four, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 33: //airborne threat symbol with 5 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Five, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 34: //airborne threat symbol with 6 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Six, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 35: //airborne threat symbol with 14 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Four,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 36: //airborne threat symbol with 15 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Five,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 37: //airborne threat symbol with 16 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Six,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 38: //airborne threat symbol with 18 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Eight,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 39: //airborne threat symbol with 19 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.One, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Nine,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 40: //airborne threat symbol with 20 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Zero,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 41: //airborne threat symbol with 21 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.One,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 42: //airborne threat symbol with 22 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Two,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 43: //airborne threat symbol with 23 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Three,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 44: //airborne threat symbol with 25 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Five,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 45: //airborne threat symbol with 27 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Seven,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 46: //airborne threat symbol with 29 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Two, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Nine,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 47: //airborne threat symbol with 30 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Three, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.Zero,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 48: //airborne threat symbol with 31 inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.Three, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.One,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 49: //airborne threat symbol with P inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 50: //airborne threat symbol with PD inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    blips.Add(new Blip
                    {
                        Symbol = Symbols.D,
                        X = (byte) (x + charSpacingXOffset),
                        Y = (byte) (y + lineSpacingYOffset)
                    });
                    break;
                case 51: //airborne threat symbol with A inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 52: //airborne threat symbol with B inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.B, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 53: //airborne threat symbol with S inside
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.S, X = x, Y = (byte) (y + lineSpacingYOffset)});
                    break;
                case 54: //A|
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x + charSpacingXOffset), Y = y});
                    break;
                case 55: //|A|
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x - charSpacingXOffset), Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x + charSpacingXOffset), Y = y});
                    break;
                case 56: //||| with A overlaid
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x - charSpacingXOffset), Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = (byte) (x + charSpacingXOffset), Y = y});
                    blips.Add(new Blip {Symbol = Symbols.VerticalLine, X = x, Y = y});
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    break;
                case 57: // alternating F and S
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.F, X = x, Y = y}
                        : new Blip {Symbol = Symbols.S, X = x, Y = y});
                    break;
                case 58: //alternating F and A
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.F, X = x, Y = y}
                        : new Blip {Symbol = Symbols.A, X = x, Y = y});
                    break;
                case 59: //alternating F and M
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.F, X = x, Y = y}
                        : new Blip {Symbol = Symbols.M, X = x, Y = y});
                    break;
                case 60: //alternating F and U
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.F, X = x, Y = y}
                        : new Blip {Symbol = Symbols.U, X = x, Y = y});
                    break;
                case 61: //alternating F and basic interceptor symbol
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.F, X = x, Y = y}
                        : new Blip {Symbol = Symbols.BasicInterceptor, X = x, Y = y});
                    break;
                case 62: //alternating S and basic interceptor symbol
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.S, X = x, Y = y}
                        : new Blip {Symbol = Symbols.BasicInterceptor, X = x, Y = y});
                    break;
                case 63: //alternating A and basic interceptor symbol
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.A, X = x, Y = y}
                        : new Blip {Symbol = Symbols.BasicInterceptor, X = x, Y = y});
                    break;
                case 64: //alternating M and basic interceptor symbol
                    blips.Add(primarySymbol
                        ? new Blip {Symbol = Symbols.M, X = x, Y = y}
                        : new Blip {Symbol = Symbols.BasicInterceptor, X = x, Y = y});
                    break;
                case 65: //A
                    blips.Add(new Blip {Symbol = Symbols.A, X = x, Y = y});
                    break;
                case 66: //B
                    blips.Add(new Blip {Symbol = Symbols.B, X = x, Y = y});
                    break;
                case 67: //C
                    blips.Add(new Blip {Symbol = Symbols.C, X = x, Y = y});
                    break;
                case 68: //D
                    blips.Add(new Blip {Symbol = Symbols.D, X = x, Y = y});
                    break;
                case 69: //E
                    blips.Add(new Blip {Symbol = Symbols.E, X = x, Y = y});
                    break;
                case 70: //F
                    blips.Add(new Blip {Symbol = Symbols.F, X = x, Y = y});
                    break;
                case 71: //G
                    blips.Add(new Blip {Symbol = Symbols.G, X = x, Y = y});
                    break;
                case 72: //H
                    blips.Add(new Blip {Symbol = Symbols.H, X = x, Y = y});
                    break;
                case 73: //I
                    blips.Add(new Blip {Symbol = Symbols.I, X = x, Y = y});
                    break;
                case 74: //J
                    blips.Add(new Blip {Symbol = Symbols.J, X = x, Y = y});
                    break;
                case 75: //K
                    blips.Add(new Blip {Symbol = Symbols.K, X = x, Y = y});
                    break;
                case 76: //L
                    blips.Add(new Blip {Symbol = Symbols.L, X = x, Y = y});
                    break;
                case 77: //M
                    blips.Add(new Blip {Symbol = Symbols.M, X = x, Y = y});
                    break;
                case 78: //N
                    blips.Add(new Blip {Symbol = Symbols.N, X = x, Y = y});
                    break;
                case 79: //O
                    blips.Add(new Blip {Symbol = Symbols.O, X = x, Y = y});
                    break;
                case 80: //P
                    blips.Add(new Blip {Symbol = Symbols.P, X = x, Y = y});
                    break;
                case 81: //Q
                    blips.Add(new Blip {Symbol = Symbols.Q, X = x, Y = y});
                    break;
                case 82: //R
                    blips.Add(new Blip {Symbol = Symbols.R, X = x, Y = y});
                    break;
                case 83: //S
                    blips.Add(new Blip {Symbol = Symbols.S, X = x, Y = y});
                    break;
                case 84: //T
                    blips.Add(new Blip {Symbol = Symbols.T, X = x, Y = y});
                    break;
                case 85: //U
                    blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    break;
                case 86: //V
                    blips.Add(new Blip {Symbol = Symbols.V, X = x, Y = y});
                    break;
                case 87: //W
                    blips.Add(new Blip {Symbol = Symbols.W, X = x, Y = y});
                    break;
                case 88: //X
                    blips.Add(new Blip {Symbol = Symbols.X, X = x, Y = y});
                    break;
                case 89: //Y
                    blips.Add(new Blip {Symbol = Symbols.Y, X = x, Y = y});
                    break;
                case 90: //Z
                    blips.Add(new Blip {Symbol = Symbols.Z, X = x, Y = y});
                    break;
                case 91: //LBRACKET [
                    blips.Add(new Blip {Symbol = Symbols.LeftBracket, X = x, Y = y});
                    break;
                case 92: //BACKSLASH \
                    blips.Add(new Blip {Symbol = Symbols.Backslash, X = x, Y = y});
                    break;
                case 93: //RBRAKCET ]
                    blips.Add(new Blip {Symbol = Symbols.RightBracket, X = x, Y = y});
                    break;
                case 94: //HAT
                    blips.Add(new Blip {Symbol = Symbols.Hat, X = x, Y = y});
                    break;
                case 95: //UNDERSCORE _
                    blips.Add(new Blip {Symbol = Symbols.Underscore, X = x, Y = y});
                    break;
                case 96: //BACKTICK `
                    blips.Add(new Blip {Symbol = Symbols.BackTick, X = x, Y = y});
                    break;
                case 97: //a
                    blips.Add(new Blip {Symbol = Symbols.LowerCaseA, X = x, Y = y});
                    break;
                case 98: //b
                    blips.Add(new Blip {Symbol = Symbols.LowerCaseB, X = x, Y = y});
                    break;
                case 99: //c
                    blips.Add(new Blip {Symbol = Symbols.LowerCaseC, X = x, Y = y});
                    break;

                default:
                    if (falconRWRSymbol.SymbolId < 0) //show U for Unknown Threat
                    {
                        blips.Add(new Blip {Symbol = Symbols.U, X = x, Y = y});
                    }
                    else if (falconRWRSymbol.SymbolId >= 100)
                    {
                        //subtract 100 from the number, and that's the Number to display as digits
                        var numberToDisplay = falconRWRSymbol.SymbolId - 100;
                        var digits = numberToDisplay.ToString();
                        blips.AddRange(digits.Select((t, i) =>
                                (byte) int.Parse(digits.Substring(i, 1)))
                            .Select(
                                (thisDigit, i) =>
                                    new Blip
                                    {
                                        Symbol = (Symbols) thisDigit,
                                        X = (byte) (x + i * charSpacingXOffset),
                                        Y = y
                                    }));
                    }

                    break;
            }
            return blips;
        }
    }
}