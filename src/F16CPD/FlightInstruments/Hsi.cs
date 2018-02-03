using System;
using System.Collections.Generic;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;
using F16CPD.Properties;

namespace F16CPD.FlightInstruments
{
    public sealed class Hsi : IDisposable
    {
        private static Font _headingBoxFont = new Font("Lucida Console", 22, FontStyle.Bold);
        private static Font _textFont = new Font("Lucida Console", 12, FontStyle.Bold);
        private Bitmap _compassRose;
        private Bitmap _courseDeviationDiamond;
        private DateTime _currentNavModeDisplayedSince=DateTime.MinValue;
        private NavModes _currentNavMode;
        private bool _isDisposed;
        public F16CpdMfdManager Manager { get; set; }

        public void Render(Graphics g, Size renderSize)
        {
            if (Manager.FlightData.HsiOffFlag)
            {
                const string toDisplay = "NO HSI DATA";
                Brush greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
                var path = new GraphicsPath();
                var sf = new StringFormat(StringFormatFlags.NoWrap)
                             {
                                 Alignment = StringAlignment.Center,
                                 LineAlignment = StringAlignment.Center
                             };
                var layoutRectangle = new Rectangle(new Point(0, 0), renderSize);
                path.AddString(toDisplay, FontFamily.GenericMonospace, (int) FontStyle.Bold, 20, layoutRectangle, sf);
                g.FillPathFast(greenBrush, path);
            }
            else
            {
                //draw compass rose
                if (_compassRose == null)
                {
                    _compassRose = Resources.hsicompass;
                }

                var compassRose = (Bitmap) _compassRose.Clone();
                compassRose.MakeTransparent(Color.Black);
                var currentHeadingInDecimalDegrees = ((Manager.FlightData.MagneticHeadingInDecimalDegrees))%360;

                var whitePen = new Pen(Color.White) {Width = 3};
                Point pointA;
                Point pointB;
                Point pointC;
                Point pointD;
                Point[] points;
                Brush whiteBrush = new SolidBrush(Color.White);

                var airplaneSymbolPointList = new List<Point>
                                                  {
                                                      new Point(15, 3),
                                                      new Point(15, 13),
                                                      new Point(4, 13),
                                                      new Point(4, 17),
                                                      new Point(15, 17),
                                                      new Point(15, 30),
                                                      new Point(12, 30),
                                                      new Point(12, 34),
                                                      new Point(15, 34),
                                                      new Point(15, 36),
                                                      new Point(19, 36),
                                                      new Point(19, 34),
                                                      new Point(22, 34),
                                                      new Point(22, 30),
                                                      new Point(19, 30),
                                                      new Point(19, 17),
                                                      new Point(30, 17),
                                                      new Point(30, 13),
                                                      new Point(19, 13),
                                                      new Point(19, 3)
                                                  };
                //draw airplane symbol
                var blackPen = new Pen(Color.Black);
                var airplaneSymbolPoints = airplaneSymbolPointList.ToArray();
                g.TranslateTransform((renderSize.Width/2) - 24, (renderSize.Height/2) - 9);
                g.DrawPolygon(blackPen, airplaneSymbolPoints);
                g.FillPolygon(whiteBrush, airplaneSymbolPoints);
                g.TranslateTransform(-((renderSize.Width/2) - 24), -((renderSize.Height/2) - 9));
                //36x40
                var toFlag = false;
                var fromFlag = false;

                using (var crg = Graphics.FromImage(compassRose))
                {
                    crg.SmoothingMode = SmoothingMode.AntiAlias;

                    //compute course deviation and TO/FROM
                    var deviationLimitDecimalDegrees = Manager.FlightData.HsiCourseDeviationLimitInDecimalDegrees%180;
                    var courseDeviationDecimalDegrees = Manager.FlightData.HsiCourseDeviationInDecimalDegrees;
                    if (Math.Abs(courseDeviationDecimalDegrees) <= 90)
                    {
                        toFlag = true;
                    }
                    else
                    {
                        fromFlag = true;
                    }

                    if (courseDeviationDecimalDegrees < -90)
                    {
                        courseDeviationDecimalDegrees =
                            Common.Math.Util.AngleDelta(Math.Abs(courseDeviationDecimalDegrees), 180)%180;
                    }
                    else if (courseDeviationDecimalDegrees > 90)
                    {
                        courseDeviationDecimalDegrees =
                            -Common.Math.Util.AngleDelta(courseDeviationDecimalDegrees, 180)%180;
                    }
                    else
                    {
                        courseDeviationDecimalDegrees = -courseDeviationDecimalDegrees;
                    }
                    if (Math.Abs(courseDeviationDecimalDegrees) > deviationLimitDecimalDegrees)
                        courseDeviationDecimalDegrees = Math.Sign(courseDeviationDecimalDegrees)*
                                                        deviationLimitDecimalDegrees;


                    //draw course deviation ring
                    crg.TranslateTransform((float) compassRose.Width/2, (float) (compassRose.Height/2.0) - 1);
                    crg.RotateTransform((Manager.FlightData.HsiDesiredCourseInDegrees)%360);
                    crg.TranslateTransform(-((float) compassRose.Width/2), -((float) (compassRose.Height/2.0) - 1));

                    if (_courseDeviationDiamond == null)
                    {
                        _courseDeviationDiamond = Resources.hsicoursedeviationdiamond;
                        _courseDeviationDiamond.MakeTransparent(Color.Black);
                    }
                    var courseDeviationDiamond = (Bitmap) _courseDeviationDiamond.Clone();

                    //draw course deviation diamonds
                    crg.DrawImageFast(courseDeviationDiamond, new Point(101, 166));
                    crg.DrawImageFast(courseDeviationDiamond, new Point(134, 166));
                    crg.DrawImageFast(courseDeviationDiamond, new Point(200, 166));
                    crg.DrawImageFast(courseDeviationDiamond, new Point(233, 166));

                    //draw HSI TO/FROM flags
                    var toFromFlagSolidColorBrush = Brushes.White;
                    whitePen.Width = 1;

                    //draw TO flag
                    pointA = new Point((compassRose.Width/2) + 35, (compassRose.Height/2) - 40); //top
                    pointB = new Point((compassRose.Width/2) + 45, (compassRose.Height/2) - 20); //right
                    pointC = new Point((compassRose.Width/2) + 25, (compassRose.Height/2) - 20); //left
                    points = new[] {pointA, pointB, pointC};
                    if (toFlag && Manager.FlightData.HsiDisplayToFromFlag)
                    {
                        crg.FillPolygon(toFromFlagSolidColorBrush, points);
                    }

                    //draw FROM flag
                    pointA = new Point((compassRose.Width/2) + 25, (compassRose.Height/2) + 20); //left
                    pointB = new Point((compassRose.Width/2) + 45, (compassRose.Height/2) + 20); //right
                    pointC = new Point((compassRose.Width/2) + 35, (compassRose.Height/2) + 40); //bottom
                    points = new[] {pointA, pointB, pointC};

                    if (fromFlag && Manager.FlightData.HsiDisplayToFromFlag)
                    {
                        crg.FillPolygon(toFromFlagSolidColorBrush, points);
                    }

                    //draw HSI Course Deviation Invalid flag
                    var courseInvalidFlagBrush = Brushes.Red;
                    if (Manager.NightMode)
                    {
                        courseInvalidFlagBrush = Brushes.White;
                    }
                    pointA = new Point((compassRose.Width/2) - 50, (compassRose.Height/2) - 30); //left top
                    pointB = new Point((compassRose.Width/2) - 20, (compassRose.Height/2) - 30); //right top
                    pointC = new Point((compassRose.Width/2) - 20, (compassRose.Height/2) - 20); //right bottom
                    pointD = new Point((compassRose.Width/2) - 50, (compassRose.Height/2) - 20); //left bottom
                    points = new[] {pointA, pointB, pointC, pointD};

                    if (Manager.FlightData.HsiDeviationInvalidFlag)
                    {
                        crg.FillPolygon(courseInvalidFlagBrush, points);
                    }


                    whitePen.Width = 2;
                    //draw course indicator needle
                    crg.DrawLineFast(whitePen, new Point((compassRose.Width/2), 44), new Point((compassRose.Width/2), 126));
                    //draw reciprocal-of-course indicator needle
                    crg.DrawLineFast(whitePen, new Point((compassRose.Width/2), 207), new Point((compassRose.Width/2), 297));
                    //draw course pointer triangle
                    pointA = new Point((compassRose.Width/2), 74);
                    pointB = new Point((compassRose.Width/2) - 16, 94);
                    pointC = new Point((compassRose.Width/2) + 16, 94);
                    points = new[] {pointA, pointB, pointC};
                    crg.FillPolygon(whiteBrush, points);

                    //draw course deviation indicator needle
                    Pen cdiNeedlePen;


                    if (Manager.FlightData.HsiDeviationInvalidFlag)
                    {
                        cdiNeedlePen = Manager.NightMode ? new Pen(Color.White) : new Pen(Color.Yellow);
                        cdiNeedlePen.DashStyle = DashStyle.Dash;
                        cdiNeedlePen.DashCap = DashCap.Flat;
                        cdiNeedlePen.DashPattern = new float[] {1, 1};
                    }
                    else
                    {
                        cdiNeedlePen = new Pen(Color.White);
                    }
                    cdiNeedlePen.Width = 4;
                    var needleX = (compassRose.Width/2);
                    const int distanceBetweenDiamondCenters = 33;
                    if (deviationLimitDecimalDegrees != 0)
                    {
                        needleX +=
                            (int)
                            ((courseDeviationDecimalDegrees/deviationLimitDecimalDegrees)*
                             (distanceBetweenDiamondCenters*2));
                    }

                    crg.DrawLineFast(cdiNeedlePen, new Point(needleX, 132), new Point(needleX, 200));


                    //draw heading bug
                    crg.ResetTransform();
                    crg.TranslateTransform((float) compassRose.Width/2, (float) (compassRose.Height/2.0) - 1);
                    crg.RotateTransform(Manager.FlightData.HsiDesiredHeadingInDegrees%360);
                    crg.TranslateTransform(-(float) compassRose.Width/2, -(float) (compassRose.Height/2.0) - 1);
                    crg.TranslateTransform(154, 6);
                    var bugA = new Point(0, 1);
                    var bugB = new Point(0, 15);
                    var bugC = new Point(35, 15);
                    var bugD = new Point(35, 1);
                    var bugE = new Point(28, 1);
                    var bugF = new Point(19, 13);
                    var bugG = new Point(16, 13);
                    var bugH = new Point(6, 1);
                    var bugPoints = new[] {bugA, bugB, bugC, bugD, bugE, bugF, bugG, bugH};
                    var headingBugColor = Color.FromArgb(0, 255, 0);
                    Brush headingBugBrush = new SolidBrush(headingBugColor);
                    crg.FillPolygon(headingBugBrush, bugPoints);

                    //draw bearing to beacon indicator
                    var aquaPen = new Pen(Color.Aqua) {Width = 2};
                    crg.ResetTransform();
                    crg.TranslateTransform((float) compassRose.Width/2, (float) compassRose.Height/2);
                    crg.RotateTransform(Manager.FlightData.HsiBearingToBeaconInDecimalDegrees%360);
                    crg.TranslateTransform(-(float) compassRose.Width/2, -(float) compassRose.Height/2);
                    var triangleTop = new Point((compassRose.Width/2), 22);
                    var triangleLeft = new Point((compassRose.Width/2) - 10, triangleTop.Y + 17);
                    var triangleRight = new Point((compassRose.Width/2) + 10, triangleTop.Y + 17);
                    crg.FillPolygon(new SolidBrush(aquaPen.Color), new[] {triangleTop, triangleLeft, triangleRight});

                    //draw bearing to beacon tail on pointer head
                    aquaPen.Width = 5;
                    crg.DrawLineFast(aquaPen, new Point((compassRose.Width/2), 30), new Point((compassRose.Width/2), 50));

                    //draw reciprocal-of-bearing tail
                    aquaPen.Width = 5;
                    crg.DrawLineFast(aquaPen, new Point((compassRose.Width/2), compassRose.Height - 30),
                                 new Point((compassRose.Width/2), compassRose.Height - 15));
                    crg.ResetTransform();
                }
                compassRose = (Bitmap) Common.Imaging.Util.RotateBitmap(compassRose, currentHeadingInDecimalDegrees);
                g.DrawImageUnscaled(compassRose, new Point(120, 29));

                whitePen.Width = 3;
                //draw angle ticks
                g.DrawLineFast(whitePen, new Point(162, 70), new Point(177, 85));
                g.DrawLineFast(whitePen, new Point(108, 199), new Point(130, 199));
                g.DrawLineFast(whitePen, new Point(405, 86), new Point(421, 71));
                g.DrawLineFast(whitePen, new Point(453, 199), new Point(476, 199));
                g.DrawLineFast(whitePen, new Point(406, 313), new Point(421, 328));
                g.DrawLineFast(whitePen, new Point(291, 360), new Point(291, 382));
                g.DrawLineFast(whitePen, new Point(162, 328), new Point(177, 313));

                whitePen.Width = 2;
                //draw course box
                pointA = new Point(258, 4);
                pointB = new Point(258, 34);
                pointC = new Point(278, 34);
                pointD = new Point(290, 47);
                var pointD2 = new Point(291, 47);
                var pointE = new Point(303, 34);
                var pointF = new Point(325, 34);
                var pointG = new Point(325, 4);
                points = new[] {pointA, pointB, pointC, pointD, pointD2, pointE, pointF, pointG};
                whitePen.Width = 1;
                g.DrawPolygon(whitePen, points);


                //draw heading numeric value in heading box
                
                var headingBoxTextRectangle = new Rectangle(new Point(258, 6), new Size(68, 30));
                var headingBoxNumToDisplay = (int) (Math.Round(currentHeadingInDecimalDegrees, 0)%360);
                if (Settings.Default.DisplayNorthAsThreeSixZero)
                {
                    if (headingBoxNumToDisplay == 0) headingBoxNumToDisplay = 360;
                }
                else
                {
                    if (headingBoxNumToDisplay == 360) headingBoxNumToDisplay = 0;
                }

                var headingBoxText = string.Format("{0:000}", headingBoxNumToDisplay);
                g.DrawStringFast(headingBoxText, _headingBoxFont, whiteBrush, headingBoxTextRectangle);

                //draw text labels
                var selectedCourseTextRectangle = new Rectangle(new Point(449, 1), new Size(145, 18));
                var selectedHeadingTextRectangle = new Rectangle(new Point(421, 25), new Size(173, 20));
                var dmeTextRectangle = new Rectangle(new Point(1, 1), new Size(125, 18));

                var textFormat = new StringFormat
                                     {
                                         Alignment = StringAlignment.Far,
                                         LineAlignment = StringAlignment.Far
                                     };

                var selectedCourse = (Manager.FlightData.HsiDesiredCourseInDegrees);

                if (Settings.Default.DisplayNorthAsThreeSixZero)
                {
                    if (selectedCourse == 0) selectedCourse = 360;
                }
                else
                {
                    if (selectedCourse == 360) selectedCourse = 0;
                }

                var selectedCourseText = string.Format("{0:000}", selectedCourse);
                g.DrawStringFast("SEL CRS " + selectedCourseText, _textFont, whiteBrush, selectedCourseTextRectangle,
                             textFormat);

                var selectedHeading = (Manager.FlightData.HsiDesiredHeadingInDegrees);

                if (Settings.Default.DisplayNorthAsThreeSixZero)
                {
                    if (selectedHeading == 0) selectedHeading = 360;
                }
                else
                {
                    if (selectedHeading == 360) selectedHeading = 0;
                }
                var selectedHeadingText = string.Format("{0:000}", selectedHeading);
                g.DrawStringFast("SEL HDG " + selectedHeadingText, _textFont, whiteBrush, selectedHeadingTextRectangle,
                             textFormat);

                if (Manager.FlightData.HsiDisplayToFromFlag)
                {
                    var toFromFrequencyPen = Manager.FlightData.HsiDeviationInvalidFlag
                                                 ? new Pen(Color.Yellow)
                                                 : new Pen(Color.White);
                    toFromFrequencyPen.Width = 3;

                    /*
                    //draw TACAN frequency
                    Brush toFromFrequencyBrush = null;
                    toFromFrequencyBrush = new SolidBrush(toFromFrequencyPen.Color);
                    string tacanChannel = this.FlightData.TacanChannel;
                    StringFormat tacanChannelFormat = new StringFormat();
                    tacanChannelFormat.Alignment = StringAlignment.Center;
                    tacanChannelFormat.LineAlignment = StringAlignment.Center;
                    Rectangle tacanFrequencyRectangle = new Rectangle(toLineBottomPoint.X - 60, toLineBottomPoint.Y + 3, 120, 20);
                    g.DrawStringFast(tacanChannel, textFont, toFromFrequencyBrush, tacanFrequencyRectangle, tacanChannelFormat);
                    */
                    /*
                    //draw TO/FROM arrow near TACAN frequency

                    if (toFlag)
                    {
                        g.DrawLineFast(toFromArrowPen, toLineTopPoint, toLineBottomPoint);
                        g.DrawLineFast(toFromArrowPen, toLineTopPoint.X, toLineTopPoint.Y, toFromArrowLeftX, toLineTopPoint.Y + 10);
                        g.DrawLineFast(toFromArrowPen, toLineTopPoint.X, toLineTopPoint.Y,toFromArrowRightX, toLineTopPoint.Y + 10);
                    }
                    else if (fromFlag)
                    {
                        g.DrawLineFast(toFromArrowPen, fromLineTopPoint, fromLineBottomPoint);
                        g.DrawLineFast(toFromArrowPen, fromLineBottomPoint.X , fromLineBottomPoint.Y, toFromArrowLeftX, fromLineBottomPoint.Y - 10);
                        g.DrawLineFast(toFromArrowPen, fromLineBottomPoint.X, fromLineBottomPoint.Y, toFromArrowRightX, fromLineBottomPoint.Y - 10);
                    }
                    */
                }

                var dme = (Manager.FlightData.HsiDistanceToBeaconInNauticalMiles);
                var dmeText = string.Format("{0:000.0}", dme);
                textFormat.LineAlignment = StringAlignment.Near;
                textFormat.Alignment = StringAlignment.Near;
                g.DrawStringFast(dmeText + " MILES", _textFont, whiteBrush, dmeTextRectangle, textFormat);

                //draw HSI DME invalid flag if required
                if (Manager.FlightData.HsiDistanceInvalidFlag)
                {
                    var currentClip = g.Clip;
                    var clipRectangle = new Rectangle(dmeTextRectangle.X, dmeTextRectangle.Y + 4, dmeTextRectangle.Width,
                                                      7);
                    g.Clip = new Region(clipRectangle);
                    const int strokeWidth = 5;
                    var alternate = false;
                    for (var i = dmeTextRectangle.Left - 20; i <= dmeTextRectangle.Right; i += strokeWidth)
                    {
                        var thisColor = Color.Red;
                        if (alternate)
                        {
                            thisColor = Color.White;
                        }
                        var thisPen = new Pen(thisColor) {Width = strokeWidth};
                        g.DrawLineFast(thisPen, new Point(i, dmeTextRectangle.Bottom),
                                   new Point(i + (strokeWidth*2), dmeTextRectangle.Top));
                        alternate = !alternate;
                    }
                    g.Clip = currentClip;
                }

                if (Manager.FlightData.NavMode != _currentNavMode) 
                {
                    _currentNavModeDisplayedSince = DateTime.UtcNow;
                    _currentNavMode = Manager.FlightData.NavMode;
                }
                //draw HSI nav mode
                if (DateTime.UtcNow.Subtract(_currentNavModeDisplayedSince).TotalSeconds < 2)
                {
                    var navModeRectangle = new Rectangle(243, 165, 95, 22);
                    var navModeText = string.Format("{0}", _currentNavMode.ToString().ToUpper().Replace("ILS", "ILS/"));
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Center;

                    var path = new GraphicsPath();
                    path.AddString(navModeText, FontFamily.GenericSansSerif, (int)FontStyle.Bold, 20, navModeRectangle, textFormat);
                    g.FillPathFast(Brushes.Aqua, path);
                }
                
                
                
            
            
            }
        }

        #region Destructors

        /// <summary>
        ///   Public implementation of IDisposable.Dispose().  Cleans up managed
        ///   and unmanaged resources used by this object before allowing garbage collection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Standard finalizer, which will call Dispose() if this object is not
        ///   manually disposed.  Ordinarily called only by the garbage collector.
        /// </summary>
        ~Hsi()
        {
            Dispose();
        }

        /// <summary>
        ///   Private implementation of Dispose()
        /// </summary>
        /// <param name = "disposing">flag to indicate if we should actually perform disposal.  Distinguishes the private method signature from the public signature.</param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //dispose of managed resources here
                    Common.Util.DisposeObject(_compassRose);
                    Common.Util.DisposeObject(_courseDeviationDiamond);
                }
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;
        }

        #endregion
    }
}