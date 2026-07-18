using System;
using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Deterministic evaluation of a position along a polyline trajectory. Pure: no time and no state.
    /// A <c>progress</c> value in [0, 1] maps 0 to the first point and 1 to the last point via
    /// fractional-index interpolation. This is presentation only and never computes gameplay results.
    /// </summary>
    public static class FlightPath
    {
        /// <summary>
        /// Returns the point at the given normalized progress along <paramref name="points"/>.
        /// Progress is clamped to [0, 1].
        /// </summary>
        /// <exception cref="ArgumentNullException">Points is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Fewer than two points, or non-finite progress.</exception>
        public static Vector3 Evaluate(Vector3[] points, float progress)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            if (points.Length < 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(points), points.Length, "At least two points are required.");
            }

            if (float.IsNaN(progress) || float.IsInfinity(progress))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progress), progress, "Progress must be a finite number.");
            }

            float clamped = Mathf.Clamp01(progress);
            int lastIndex = points.Length - 1;
            float scaled = clamped * lastIndex;
            int index = Mathf.FloorToInt(scaled);

            if (index >= lastIndex)
            {
                return points[lastIndex];
            }

            float fraction = scaled - index;
            return Vector3.Lerp(points[index], points[index + 1], fraction);
        }
    }
}
