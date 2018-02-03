using System.Collections.Generic;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;
using Common.Imaging;
using F16CPD.Properties;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class VVITapeRenderer
    {

        internal static void DrawVVITape(Graphics g, int centerYPfd, float verticalVelocityInDecimalFeetPerSecond, bool vviOffFlag, bool nightMode)
        {
            //*************************
            //VVI TAPE
            //*************************
            var vviTotalFont = new Font("Lucida Console", 14, FontStyle.Bold);

            var whitePen = new Pen(Color.White);
            var blackPen = new Pen(Color.Black);

            var vviBoundingBox = new Rectangle(new Point(473, 111), new Size(27, 256));
            var vviUpperBoundingBox = new Rectangle(new Point(vviBoundingBox.Left, vviBoundingBox.Top),
                new Size(vviBoundingBox.Width, centerYPfd - vviBoundingBox.Top));

            //draw vertical velocity tape Bitmap
            var verticalVelocityFpm = (verticalVelocityInDecimalFeetPerSecond * 60);
            if (vviOffFlag)
            {
                verticalVelocityFpm = 0.00f;
            }

            var verticalVelocityKFpm = (verticalVelocityFpm/1000.0f);
            var verticalVelocityKFpmNormalized = verticalVelocityKFpm;
            if (verticalVelocityKFpmNormalized < -6.5f) verticalVelocityKFpmNormalized = -6.5f;
            if (verticalVelocityKFpmNormalized > 6.5f) verticalVelocityKFpmNormalized = 6.5f;
            var path = new GraphicsPath();
            var pointList = new List<Point>();

            var verticalVelocityTapeBitmap = VerticalVelocityTapeBitmapFactory.GetVerticalVelocityTapeBitmap(vviBoundingBox.Width,
                vviBoundingBox.Height);

            g.DrawImageFast(
                verticalVelocityTapeBitmap,
                vviBoundingBox.Left,
                vviUpperBoundingBox.Top,
                new Rectangle(
                    new Point(0, 0),
                    new Size(verticalVelocityTapeBitmap.Width, vviBoundingBox.Height)
                    ), GraphicsUnit.Pixel
                );


            //draw bounding box 
            pointList.Clear();
            pointList.Add(new Point(vviBoundingBox.Left, vviBoundingBox.Top));
            pointList.Add(new Point(vviBoundingBox.Left, vviBoundingBox.Bottom));
            pointList.Add(new Point(vviBoundingBox.Right, vviBoundingBox.Bottom));
            pointList.Add(new Point(vviBoundingBox.Right, vviBoundingBox.Top + (vviBoundingBox.Height/2) + 20));
            pointList.Add(new Point(vviBoundingBox.Left + 1, vviBoundingBox.Top + (vviBoundingBox.Height/2)));
            pointList.Add(new Point(vviBoundingBox.Right, vviBoundingBox.Top + (vviBoundingBox.Height/2) - 20));
            pointList.Add(new Point(vviBoundingBox.Right, vviBoundingBox.Top));
            path.Reset();
            path.AddPolygon(pointList.ToArray());
            whitePen.Width = 2;
            g.DrawPathFast(whitePen, path);
            whitePen.Width = 1;

            if (!vviOffFlag)
            {
                //draw VVI quantity box
                pointList.Clear();
                pointList.Add(new Point(vviUpperBoundingBox.Left, vviUpperBoundingBox.Bottom - 2));
                pointList.Add(new Point(vviUpperBoundingBox.Left + (vviUpperBoundingBox.Width/2),
                    vviUpperBoundingBox.Bottom + 7));
                pointList.Add(new Point(vviUpperBoundingBox.Right + 26, vviUpperBoundingBox.Bottom + 7));
                pointList.Add(new Point(vviUpperBoundingBox.Right + 26, vviUpperBoundingBox.Bottom - 10));
                pointList.Add(new Point(vviUpperBoundingBox.Left + (vviUpperBoundingBox.Width/2),
                    vviUpperBoundingBox.Bottom - 10));
                path.Reset();
                path.AddPolygon(pointList.ToArray());
                g.TranslateTransform(3, 0 - (verticalVelocityKFpmNormalized*20) + 2);
                g.FillPathFast(Brushes.Gray, path);
                blackPen.Width = 1;
                g.DrawPathFast(blackPen, path);

                //draw VVI quantity text
                var vviQuantity = Settings.Default.DisplayVerticalVelocityInDecimalThousands
                    ? string.Format("{0:0.0}", verticalVelocityFpm/1000.0f)
                    : string.Format("{0:0}", verticalVelocityFpm);
                var vviQuantityFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Near
                };
                var fontSize = vviTotalFont.Size;
                if (vviQuantity.Length > 4)
                {
                    vviTotalFont = new Font(vviTotalFont.FontFamily, fontSize - 1, vviTotalFont.Style);
                }
                vviTotalFont = new Font(vviTotalFont.FontFamily, fontSize, vviTotalFont.Style);
                path.Reset();
                path.AddString(vviQuantity, vviTotalFont.FontFamily, (int) vviTotalFont.Style, vviTotalFont.SizeInPoints,
                    new Point(vviUpperBoundingBox.Right + 26, vviUpperBoundingBox.Bottom - 8),
                    vviQuantityFormat);
                var hint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);
                g.TextRenderingHint = hint;
            }

            if (vviOffFlag)
            {
                VVIOffFlagRenderer.DrawVVIOffFlag(g,
                    new Point(vviBoundingBox.Left - 15, vviBoundingBox.Top + (vviBoundingBox.Height/2) - 12), vviOffFlag, nightMode);
            }
        }
    }
}