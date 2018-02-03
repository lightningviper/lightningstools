using Common.Drawing;

namespace F4Utils.SimSupport
{
    public class TexturesSharedMemoryImageCoordinates
    {
        public Rectangle HUD { get; set; } = Rectangle.Empty;
        public Rectangle LMFD { get; set; } = Rectangle.Empty;
        public Rectangle RMFD { get; set; } = Rectangle.Empty;
        public Rectangle MFD3 { get; set; } = Rectangle.Empty;
        public Rectangle MFD4 { get; set; } = Rectangle.Empty;
        public Rectangle DED { get; set; } = Rectangle.Empty;
        public Rectangle PFL { get; set; } = Rectangle.Empty;
        public Rectangle RWR { get; set; } = Rectangle.Empty;
        public Rectangle HMS { get; set; } = Rectangle.Empty;
    }
}