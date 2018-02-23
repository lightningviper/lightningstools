using System.Windows.Media;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal static class DrawingGroupExtensions
    {
        public static Geometry GetGeometry(this DrawingGroup drawingGroup)
        {
            var geometry = new GeometryGroup();

            foreach (var drawing in drawingGroup.Children)
            {
                if (drawing is GeometryDrawing gd)
                {
                    geometry.Children.Add(gd.Geometry);
                }
                else if (drawing is GlyphRunDrawing grd)
                {
                    geometry.Children.Add(grd.GlyphRun.BuildGeometry());
                }
                else if (drawing is DrawingGroup dg)
                {
                    geometry.Children.Add(GetGeometry(dg));
                }
            }

            geometry.Transform = drawingGroup.Transform;
            return geometry;
        }
    }
}
