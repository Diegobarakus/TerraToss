using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Produces a deterministic sequence of coordinates along the geographic path a shot follows,
    /// from origin to impact. Pure C#, no UnityEngine dependency. Reuses <see cref="GeoMath"/>
    /// (great-circle destination points along the shot's initial heading) rather than interpolating
    /// latitude/longitude linearly.
    /// </summary>
    public static class ShotTrajectorySampler
    {
        /// <summary>
        /// Samples <paramref name="sampleCount"/> coordinates along the shot path. The first sample is
        /// the origin and the last sample is the impact coordinate (identical to
        /// <see cref="ShotResult.ImpactCoordinate"/>).
        /// </summary>
        /// <param name="input">The shot definition; its origin is used.</param>
        /// <param name="result">The computed result; its normalized heading and travelled distance are used.</param>
        /// <param name="sampleCount">Number of samples to produce; must be at least 2.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Fewer than two samples are requested, or the travelled distance or heading is not finite.
        /// </exception>
        public static GeoCoordinate[] Sample(ShotInput input, ShotResult result, int sampleCount)
        {
            if (sampleCount < 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sampleCount), sampleCount, "At least two samples are required.");
            }

            double traveledKm = result.DistanceTraveledKm;
            double headingDegrees = result.NormalizedHeadingDegrees;

            if (double.IsNaN(traveledKm) || double.IsInfinity(traveledKm) || traveledKm < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(result), traveledKm, "Travelled distance must be a finite, non-negative number.");
            }

            if (double.IsNaN(headingDegrees) || double.IsInfinity(headingDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(result), headingDegrees, "Normalized heading must be a finite number.");
            }

            var samples = new GeoCoordinate[sampleCount];
            int lastIndex = sampleCount - 1;

            // Pin the exact endpoints: the first sample is the origin and the last is the impact
            // coordinate. Recomputing them via the destination formula would introduce tiny
            // floating-point drift, so they are assigned directly.
            samples[0] = input.Origin;
            samples[lastIndex] = result.ImpactCoordinate;

            // Interior samples follow the great-circle path (no linear lat/lon interpolation).
            for (int i = 1; i < lastIndex; i++)
            {
                double progress = (double)i / lastIndex;
                double distanceKm = traveledKm * progress;
                samples[i] = GeoMath.DestinationPoint(input.Origin, headingDegrees, distanceKm);
            }

            return samples;
        }
    }
}
