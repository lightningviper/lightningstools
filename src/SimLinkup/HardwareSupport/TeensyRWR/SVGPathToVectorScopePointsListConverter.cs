using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class SVGPathToVectorScopePointsListConverter
    {
        //DAC settings
        private readonly byte _dacPrecisionBits;

        private readonly long _stepSize; 

        //degree of detail for Bezier curve interpolation
        private readonly long _bezierCurveInterpolationSteps;

        //SVG parser state
        private double _currentX;
        private double _currentY;
        private double _figureStartX;
        private double _figureStartY;
        private char _previousCommand = 'z';
        private double _previousCommandControlPointX;
        private double _previousCommandControlPointY;

        private readonly IList<DrawPoint> _drawPoints=new List<DrawPoint>();

        public SVGPathToVectorScopePointsListConverter(byte dacPrecisionBits = 12, long bezierCurveInterpolationSteps = 25, long stepSize=10)
        {
            _dacPrecisionBits = dacPrecisionBits;
            _bezierCurveInterpolationSteps = bezierCurveInterpolationSteps;
            _stepSize = stepSize;
        }


        public IEnumerable<DrawPoint> ConvertToDrawPoints(string svgPathString)
        {
            if (string.IsNullOrWhiteSpace(svgPathString))
            {
                return Enumerable.Empty<DrawPoint>();
            }

            var chars = svgPathString.ToCharArray();
            _drawPoints.Clear();

            long offset = 0;
            while (offset < chars.Length - 1)
            {
                SkipSeparators(chars, ref offset);
                if (offset > chars.Length - 1) break;
                var nextChar = chars[offset++];
                if (nextChar > 0) ProcessPathChar(nextChar, chars, ref offset);
            }

            return _drawPoints.ToArray();
        }

        private void ProcessPathChar(char pathChar, char[] chars, ref long offset)
        {
            double[] args;
            switch (pathChar)
            {
                case 'M': //move to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    MoveTo(args[0], args[1]);
                    _previousCommand =
                        'L'; //subsequent coordinates will be treated as implicit absolute LineTo commands
                    _previousCommandControlPointX = _currentX;
                    _previousCommandControlPointY = _currentY;
                    break;

                case 'm': //move to, relative
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    MoveTo(args[0] + _currentX, args[1] + _currentY);
                    _previousCommand =
                        'l'; //subsequent coordinates will be treated as implicit relative lineTo commands
                    break;

                case 'L': //line to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    LineTo(args[0], args[1]);
                    _previousCommand = 'L';
                    break;

                case 'l': //line to, relative
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    LineTo(args[0] + _currentX, args[1] + _currentY);
                    _previousCommand = 'l';
                    break;

                case 'H': //horizontal line to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 1, out args);
                    HorizontalLineTo(args[0]);
                    _previousCommand = 'H';
                    break;

                case 'h': //horizontal line to, relative
                    ReadSVGCommandArgs(chars, ref offset, 1, out args);
                    HorizontalLineTo(args[0] + _currentX);
                    _previousCommand = 'h';
                    break;

                case 'V': //vertical line to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 1, out args);
                    VerticalLineTo(args[0]);
                    _previousCommand = 'V';
                    break;

                case 'v': //vertical line to, relative
                    ReadSVGCommandArgs(chars, ref offset, 1, out args);
                    VerticalLineTo(args[0] + _currentY);
                    _previousCommand = 'v';
                    break;

                case 'C': //cubic bezier curve to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 6, out args);
                    CubicBezierCurveTo(args[0], args[1], args[2], args[3], args[4], args[5]);
                    _previousCommand = 'C';
                    break;

                case 'c': //cubic bezier curve to, relative
                    ReadSVGCommandArgs(chars, ref offset, 6, out args);
                    CubicBezierCurveTo(args[0] + _currentX, args[1] + _currentY, args[2] + _currentX,
                        args[3] + _currentY, args[4] + _currentX, args[5] + _currentY);
                    _previousCommand = 'c';
                    break;

                case 'S': //smooth cubic bezier curve to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 4, out args);
                    if (_previousCommand == 'S' || _previousCommand == 's' || _previousCommand == 'C' ||
                        _previousCommand == 'c')
                        CubicBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX),
                            _currentY - (_previousCommandControlPointY - _currentY), args[0], args[1], args[2],
                            args[3]);
                    else
                        CubicBezierCurveTo(_currentX, _currentY, args[0], args[1], args[2], args[3]);
                    _previousCommand = 'S';
                    break;

                case 's': //smooth cubic bezier curve to, relative
                    ReadSVGCommandArgs(chars, ref offset, 4, out args);
                    if (_previousCommand == 'S' || _previousCommand == 's' || _previousCommand == 'C' ||
                        _previousCommand == 'c')
                        CubicBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX),
                            _currentY - (_previousCommandControlPointY - _currentY), args[0] + _currentX,
                            args[1] + _currentY, args[2] + _currentX, args[3] + _currentY);
                    else
                        CubicBezierCurveTo(_currentX, _currentY, args[0] + _currentX, args[1] + _currentY,
                            args[2] + _currentX, args[3] + _currentY);
                    _previousCommand = 's';
                    break;

                case 'Q': //quadratic bezier curve to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 4, out args);
                    QuadraticBezierCurveTo(args[0], args[1], args[2], args[3]);
                    _previousCommand = 'Q';
                    break;

                case 'q': //quadratic bezier curve to, relative
                    ReadSVGCommandArgs(chars, ref offset, 4, out args);
                    QuadraticBezierCurveTo(args[0] + _currentX, args[1] + _currentY, args[2] + _currentX,
                        args[3] + _currentY);
                    _previousCommand = 'q';
                    break;

                case 'T': //smooth quadratic bezier curve to, absolute
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    if (_previousCommand == 'Q' || _previousCommand == 'q' || _previousCommand == 'T' ||
                        _previousCommand == 't')
                        QuadraticBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX),
                            _currentY - (_previousCommandControlPointY - _currentY), args[0], args[1]);
                    else
                        QuadraticBezierCurveTo(_currentX, _currentY, args[0], args[1]);
                    _previousCommand = 'T';
                    break;

                case 't': //smooth quadratic bezier curve to, relative
                    ReadSVGCommandArgs(chars, ref offset, 2, out args);
                    if (_previousCommand == 'Q' || _previousCommand == 'q' || _previousCommand == 'T' ||
                        _previousCommand == 't')
                        QuadraticBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX),
                            _currentY - (_previousCommandControlPointY - _currentY), args[0] + _currentX,
                            args[1] + _currentY);
                    else
                        QuadraticBezierCurveTo(_currentX, _currentY, args[0] + _currentX, args[1] + _currentY);
                    _previousCommand = 't';
                    break;

                case 'A': //elliptical arc, absolute
                    ReadSVGCommandArgs(chars, ref offset, 7, out args);
                    EllipticalArc(args[0], args[1], args[2], args[3] > 0.5, args[4] > 0.5, args[5], args[6]);
                    _previousCommand = 'A';
                    break;

                case 'a': //elliptical arc, relative
                    ReadSVGCommandArgs(chars, ref offset, 7, out args);
                    EllipticalArc(args[0], args[1], args[2], args[3] > 0.5, args[4] > 0.5, args[5] + _currentX,
                        args[6] + _currentY);
                    _previousCommand = 'a';
                    break;

                case 'Z': //close path
                    ClosePath();
                    _previousCommand = 'Z';
                    break;

                case 'z': //close path
                    ClosePath();
                    _previousCommand = 'z';
                    break;
            }

            if (NextCharIsNumeric(chars, ref offset))
                ProcessPathChar(_previousCommand, chars, ref offset);
        }

        private static bool NextCharIsNumeric(char[] buffer, ref long offset)
        {
            SkipSeparators(buffer, ref offset);
            if (offset > buffer.Length - 1) return false;
            var nextChar = buffer[offset];
            return char.IsDigit(nextChar) || nextChar == '.' || nextChar == '-';
        }

        private static void ReadSVGCommandArgs(char[] buffer, ref long offset, long howMany, out double[] args)
        {
            args = new double[7];

            for (var i = 0; i < howMany; i++)
            {
                SkipSeparators(buffer, ref offset);
                args[i] = ParseSVGArgFloat(buffer, ref offset);
            }
        }

        private static float ParseSVGArgFloat(char[] buffer, ref long offset)
        {
            var toParse = "";
            while (offset <= buffer.Length - 1)
            {
                var thisChar = buffer[offset];
                if (char.IsDigit(thisChar) || thisChar == '.' || thisChar == '-')
                {
                    toParse += thisChar;
                    offset++;
                }
                else
                {
                    break;
                }
            }

            var parsed = float.TryParse(toParse, out var toReturn);
            return parsed ? toReturn : 0;
        }

        private static void SkipSeparators(char[] buffer, ref long offset)
        {
            while (offset <= buffer.Length - 1)
            {
                var thisChar = buffer[offset];
                if (
                    char.IsDigit(thisChar)
                    || thisChar == '.'
                    || thisChar == '-'
                    || thisChar == 'M'
                    || thisChar == 'm'
                    || thisChar == 'L'
                    || thisChar == 'l'
                    || thisChar == 'H'
                    || thisChar == 'h'
                    || thisChar == 'V'
                    || thisChar == 'v'
                    || thisChar == 'C'
                    || thisChar == 'c'
                    || thisChar == 'Q'
                    || thisChar == 'q'
                    || thisChar == 'S'
                    || thisChar == 's'
                    || thisChar == 'T'
                    || thisChar == 't'
                    || thisChar == 'A'
                    || thisChar == 'a'
                    || thisChar == 'Z'
                    || thisChar == 'z'
                )
                    return;
                offset++;
            }
        }


        private void MoveTo(double x, double y)
        {
            _figureStartX = x;
            _figureStartY = y;
            DrawPointInterpolationHelper.InsertInterpolatedDrawPoints(_drawPoints, x, y, beamOn: false, dacPrecisionBits:_dacPrecisionBits, stepSize: _stepSize);
            _currentX = x;
            _currentY = y;
        }

        private void LineTo(double x, double y)
        {
            DrawPointInterpolationHelper.InsertInterpolatedDrawPoints(_drawPoints, x, y, beamOn:true, dacPrecisionBits: _dacPrecisionBits,stepSize: _stepSize);
            _currentX = x;
            _currentY = y;
        }

        private void HorizontalLineTo(double x)
        {
            LineTo(x, _currentY);
        }

        private void VerticalLineTo(double y)
        {
            LineTo(_currentX, y);
        }

        private void CubicBezierCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            var startX = _currentX;
            var startY = _currentY;
            for (var i = 0; i < _bezierCurveInterpolationSteps; i++)
            {
                BezierCurveHelper.PointOnCubicBezierCurve(startX, startY, x1, y1, x2, y2, x, y, (double)i / (_bezierCurveInterpolationSteps - 1),
                    out var nextPointX, out var nextPointY);
                LineTo(nextPointX, nextPointY);
            }

            LineTo(x, y);
            _previousCommandControlPointX = x2;
            _previousCommandControlPointY = y2;
        }

        private void QuadraticBezierCurveTo(double x1, double y1, double x, double y)
        {
            var startX = _currentX;
            var startY = _currentY;
            for (var i = 0; i < _bezierCurveInterpolationSteps; i++)
            {
                BezierCurveHelper.PointOnQuadraticBezierCurve(startX, startY, x1, y1, x, y, (double)i / (_bezierCurveInterpolationSteps - 1),
                    out var nextPointX, out var nextPointY);
                LineTo(nextPointX, nextPointY);
            }

            LineTo(x, y);
            _previousCommandControlPointX = x1;
            _previousCommandControlPointY = y1;
        }

        private void EllipticalArc(double rx, double ry, double xAxisRotation, bool largeArcFlag,
            bool sweepDirectionFlag, double x, double y)
        {
            var startX = _currentX;
            var startY = _currentY;
            for (var i = 0; i < _bezierCurveInterpolationSteps; i++)
            {
                BezierCurveHelper.PointOnEllipticalArc(startX, startY, rx, ry, xAxisRotation, largeArcFlag, sweepDirectionFlag, x, y,
                    (double)i / (_bezierCurveInterpolationSteps - 1), out var nextPointX, out var nextPointY);
                LineTo(nextPointX, nextPointY);
            }

            LineTo(x, y);
        }

        private void ClosePath()
        {
            LineTo(_figureStartX, _figureStartY);
            _figureStartX = 0;
            _figureStartY = 0;
        }





    }
}