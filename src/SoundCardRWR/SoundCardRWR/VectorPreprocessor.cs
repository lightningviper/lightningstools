using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SoundCardRWR
{
    public class PreprocessorOptions
    {
        public uint NumSamplesPerVectorEndpoint {get;set;}=1;
        public bool EqualizeBrightness { get; set; } = true;
    }
    internal static class VectorPreprocessor
    {
        public static IEnumerable<(Point Point, uint Intensity)> PreprocessVectors(IEnumerable<(Point Point, uint Intensity)> vectors, PreprocessorOptions options) 
        {
            var vects = options.EqualizeBrightness
                ? EqualizeBrightness(vectors)
                : vectors;
            vects = RepeatEndpointVectors(vects, options.NumSamplesPerVectorEndpoint);
            return vects;

        }
        private static IEnumerable<(Point Point, uint Intensity)> RepeatEndpointVectors(IEnumerable<(Point Point, uint Intensity)> vectors, uint numTimes)
        {
            List<(Point, uint)> newVectors = new List<(Point Point, uint Intensity)>();
            foreach (var vector in vectors)
            {
                for (var n=1;n<=numTimes;n++)
                {
                    newVectors.Add(vector);
                }
            }
            return newVectors;
        }
        private static IEnumerable<(Point Point, uint Intensity)> EqualizeBrightness(IEnumerable<(Point Point, uint Intensity)> vectors)
        {
            var sumOfOriginalVectorLengths_beamOn = 0.0;
            var sumOfOriginalVectorLengths_beamOff = 0.0;

            for (var i = 1; i < vectors.Count(); i++)
            {
                var previousVector = vectors.ElementAt(i - 1);
                var thisVector = vectors.ElementAt(i);
                var thisVectorLength = Math.Sqrt(Math.Pow(thisVector.Point.X - previousVector.Point.X, 2) + Math.Pow(thisVector.Point.Y - previousVector.Point.Y, 2));
                if (thisVector.Intensity == 0)
                {
                    sumOfOriginalVectorLengths_beamOff += thisVectorLength;
                }
                else
                {
                    sumOfOriginalVectorLengths_beamOn += thisVectorLength;
                }
            }
            List<(Point, uint)> newVectors = new List<(Point Point, uint Intensity)>();
            for (var i = 1; i < vectors.Count(); i++)
            {
                var previousVector = vectors.ElementAt(i - 1);
                newVectors.Add(previousVector);
                var thisVector = vectors.ElementAt(i);
                var thisVectorLength = Math.Sqrt(Math.Pow(thisVector.Point.X - previousVector.Point.X, 2) + Math.Pow(thisVector.Point.Y - previousVector.Point.Y, 2));
                if (thisVector.Intensity == 0)
                {
                    newVectors.Add(thisVector);
                }
                else
                {
                    var steps = thisVectorLength/(sumOfOriginalVectorLengths_beamOn);
                    for (var v=1;v<=steps;v++)
                    {
                        newVectors.Add((new Point(
                                x: (previousVector.Point.X + ((v*(thisVector.Point.X - previousVector.Point.X)) / steps)),
                                y: (previousVector.Point.Y + ((v*(thisVector.Point.Y - previousVector.Point.Y)) / steps))
                            ), thisVector.Intensity));
                    }
                }
            }
            return newVectors;
        }
    }
}
