using Common.Drawing;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AoaTapeRenderer
    {

        internal static void DrawAoaTape(Graphics g, int centerYPfd, float angleOfAttackInDegrees, bool aoaOffFlag, bool nightMode)
        {
            //*************************
            //AOA TAPE
            //*************************

            var blackPen = new Pen(Color.Black);
            var whitePen = new Pen(Color.White);
            //draw bounding box for AOA tape
            var aoaStripBoundingBox = new Rectangle(new Point(87, 168), new Size(42, 144));

            //draw aoa tape Bitmap
            var aoa = angleOfAttackInDegrees;

            if (aoaOffFlag) aoa = 0.00f;

            var aoaBitmap = AOATapeBitmapFactory.GetAoATapeBitmap(aoa, aoaStripBoundingBox.Width, aoaStripBoundingBox.Height);

            g.DrawImageFast(
                aoaBitmap,
                aoaStripBoundingBox.Left,
                aoaStripBoundingBox.Top,
                new Rectangle(
                    new Point(0, 0),
                    new Size(aoaBitmap.Width, aoaStripBoundingBox.Height)
                    ), GraphicsUnit.Pixel
                );


            //trace the outline of the AOA tape with black
            g.DrawRectangleFast(blackPen, aoaStripBoundingBox);

            //draw white line at top of AOA tape
            whitePen.Width = 2;
            g.DrawLineFast(whitePen, aoaStripBoundingBox.Left, aoaStripBoundingBox.Top, aoaStripBoundingBox.Right,
                aoaStripBoundingBox.Top);

            //draw white line at bottom of AOA tape
            whitePen.Width = 2;
            g.DrawLineFast(whitePen, aoaStripBoundingBox.Left, aoaStripBoundingBox.Bottom, aoaStripBoundingBox.Right,
                aoaStripBoundingBox.Bottom);

            //draw left AOA indicator triangle
            var aoaTriangleLeftPoints = new Point[3];
            aoaTriangleLeftPoints[0] = new Point(aoaStripBoundingBox.Left, centerYPfd);
            //right-most point on left triangle
            aoaTriangleLeftPoints[1] = new Point(aoaStripBoundingBox.Left - 16, centerYPfd - 10);
            //upper point on left triangle
            aoaTriangleLeftPoints[2] = new Point(aoaStripBoundingBox.Left - 16, centerYPfd + 10);
            //lower point on left triangle
            g.FillPolygon(Brushes.Black, aoaTriangleLeftPoints);
            whitePen.Width = 1;
            g.DrawPolygon(whitePen, aoaTriangleLeftPoints);

            //draw right AOA indicator triangle
            var aoaTriangleRightPoints = new Point[3];
            aoaTriangleRightPoints[0] = new Point(aoaStripBoundingBox.Right, centerYPfd);
            //left-most point on right triangle
            aoaTriangleRightPoints[1] = new Point(aoaStripBoundingBox.Right + 16, centerYPfd - 10);
            //upper point on left triangle
            aoaTriangleRightPoints[2] = new Point(aoaStripBoundingBox.Right + 16, centerYPfd + 10);
            //lower point on left triangle
            g.FillPolygon(Brushes.Black, aoaTriangleRightPoints);
            whitePen.Width = 1;
            g.DrawPolygon(whitePen, aoaTriangleRightPoints);

            //draw line between indicator triangle points
            g.DrawLineFast(whitePen, aoaTriangleLeftPoints[0], aoaTriangleRightPoints[0]);

            var location = new Point(aoaStripBoundingBox.Left - 17,
                aoaStripBoundingBox.Top + (aoaStripBoundingBox.Height/2) - 12);
            if (aoaOffFlag)
            {
                AOAOffFlagRenderer.DrawAoaOffFlag(g, location, aoaOffFlag, nightMode);
            }
        }
    }
}