using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    public class PreprocessorOptions
    {
        public bool EqualizeBrightness { get; set; } = true;
    }
    internal static class VectorPreprocessor
    {
        public static IEnumerable<(Point Point, bool BeamOn)> PreprocessVectors(IEnumerable<(Point Point, bool BeamOn)> vectors, PreprocessorOptions options)
        {
            return options.EqualizeBrightness
                ? EqualizeBrightness(vectors)
                : vectors;
        }
        private static IEnumerable<(Point Point, bool BeamOn)> EqualizeBrightness(IEnumerable<(Point Point, bool BeamOn)> vectors)
        {
            var sumOfOriginalVectorLengths_beamOn = 0.0;
            var sumOfOriginalVectorLengths_beamOff = 0.0;

            for (var i = 1; i < vectors.Count(); i++)
            {
                var previousVector = vectors.ElementAt(i - 1);
                var thisVector = vectors.ElementAt(i);
                var thisVectorLength = Math.Sqrt(Math.Pow(thisVector.Point.X - previousVector.Point.X, 2) + Math.Pow(thisVector.Point.Y - previousVector.Point.Y, 2));
                if (thisVector.BeamOn)
                {
                    sumOfOriginalVectorLengths_beamOn += thisVectorLength;
                }
                else
                {
                    sumOfOriginalVectorLengths_beamOff += thisVectorLength;
                }
            }
            List<(Point, bool)> newVectors = new List<(Point Point, bool BeamOn)>();
            for (var i = 1; i < vectors.Count(); i++)
            {
                var previousVector = vectors.ElementAt(i - 1);
                newVectors.Add(previousVector);
                var thisVector = vectors.ElementAt(i);
                var thisVectorLength = Math.Sqrt(Math.Pow(thisVector.Point.X - previousVector.Point.X, 2) + Math.Pow(thisVector.Point.Y - previousVector.Point.Y, 2));
                if (!thisVector.BeamOn)
                {
                    newVectors.Add(thisVector);
                }
                else
                {
                    var steps = (thisVectorLength / (sumOfOriginalVectorLengths_beamOn)) * 500;
                    for (var v = 1; v <= steps; v++)
                    {
                        newVectors.Add((new Point(
                                x: (previousVector.Point.X + ((v * (thisVector.Point.X - previousVector.Point.X)) / steps)),
                                y: (previousVector.Point.Y + ((v * (thisVector.Point.Y - previousVector.Point.Y)) / steps))
                            ), thisVector.BeamOn));
                    }
                }
            }
            return newVectors;
        }
    }
}
