using System;
using TerraToss.Geo;
using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Converts geographic trajectory samples into visual points for a LineRenderer.
    /// Each sample is projected onto the sphere with <see cref="GeoSphereProjection"/> and lifted
    /// outward by a purely cosmetic arc height. This is presentation only: it never produces
    /// geographic results, distances, or gameplay classifications.
    /// </summary>
    public static class TrajectoryArcProjection
    {
        /// <summary>
        /// Projects samples to visual points. The arc height is
        /// <c>sin(PI * progress) * maximumArcHeight</c>, so it is zero at both ends and greatest near
        /// the middle. Every point lies on or outside the Earth radius.
        /// </summary>
        /// <param name="samples">Geographic samples from origin to impact; at least two.</param>
        /// <param name="earthRadius">Sphere radius in Unity units; must be finite and &gt; 0.</param>
        /// <param name="maximumArcHeight">Peak visual arc height in Unity units; must be finite and &gt;= 0.</param>
        /// <exception cref="ArgumentNullException">Samples is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Fewer than two samples, invalid radius, or negative/non-finite height.</exception>
        public static Vector3[] ToVisualPoints(GeoCoordinate[] samples, double earthRadius, double maximumArcHeight)
        {
            if (samples == null)
            {
                throw new ArgumentNullException(nameof(samples));
            }

            if (samples.Length < 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(samples), samples.Length, "At least two samples are required.");
            }

            if (double.IsNaN(earthRadius) || double.IsInfinity(earthRadius) || earthRadius <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(earthRadius), earthRadius, "Earth radius must be a finite number greater than zero.");
            }

            if (double.IsNaN(maximumArcHeight) || double.IsInfinity(maximumArcHeight) || maximumArcHeight < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumArcHeight), maximumArcHeight,
                    "Maximum arc height must be a finite, non-negative number.");
            }

            var points = new Vector3[samples.Length];
            int lastIndex = samples.Length - 1;

            for (int i = 0; i < samples.Length; i++)
            {
                double progress = (double)i / lastIndex;
                double arcOffset = Math.Sin(Math.PI * progress) * maximumArcHeight;

                Vector3 surface = GeoSphereProjection.ToLocalPosition(samples[i], earthRadius);
                points[i] = surface.normalized * (float)(earthRadius + arcOffset);
            }

            return points;
        }
    }
}
