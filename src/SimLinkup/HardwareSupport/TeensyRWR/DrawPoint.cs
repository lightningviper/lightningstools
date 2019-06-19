using System;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal struct DrawPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool BeamOn { get; set; }

        public static implicit operator uint(DrawPoint drawPoint)
        {
            var xBits = ((uint)drawPoint.X & 0xFFF) << 12;
            var yBits = ((uint)drawPoint.Y & 0xFFF);
            var zBit = (uint) (drawPoint.BeamOn ? 1 : 0) << 24;
            return xBits | yBits | zBit;
        }

        public static implicit operator byte[](DrawPoint drawPoint)
        {
            return BitConverter.GetBytes(drawPoint);
        }
    }
}
