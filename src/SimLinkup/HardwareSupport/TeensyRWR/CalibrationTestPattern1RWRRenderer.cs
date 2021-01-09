using LightningGauges.Renderers.F16.RWR;
using System.Windows;
using System.Windows.Media;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    public class CalibrationTestPattern1RWRRenderer : RWRRenderer
    {
        public CalibrationTestPattern1RWRRenderer(double actualWidth, double actualHeight){
            ActualWidth = actualWidth;
            ActualHeight = actualHeight;
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
            DrawLine(drawingContext, -1, 0, 1, 0);
            DrawLine(drawingContext, 0, -1, 0, 1);
            DrawLine(drawingContext, -1, -1, 1, 1);
            DrawLine(drawingContext, -1, 1, 1, -1);

            DrawCircle(drawingContext, 0, 0, 0.22);
            DrawCircle(drawingContext, 0, 0, 0.64);
            DrawCircle(drawingContext, 0, 0, 1.00);

        }

    }
}
