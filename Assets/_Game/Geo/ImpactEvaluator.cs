using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Maps a distance-to-target in kilometres to an <see cref="ImpactGrade"/>.
    /// Pure C#, deterministic, no UnityEngine dependency.
    /// </summary>
    public static class ImpactEvaluator
    {
        /// <summary>Upper bound (exclusive) in km for a DirectHit.</summary>
        public const double DirectHitMaxKm = 5.0;

        /// <summary>Upper bound (exclusive) in km for a StrongHit.</summary>
        public const double StrongHitMaxKm = 15.0;

        /// <summary>Upper bound (exclusive) in km for a LightHit.</summary>
        public const double LightHitMaxKm = 40.0;

        /// <summary>Upper bound (exclusive) in km for a NearMiss; at or beyond this is a Miss.</summary>
        public const double NearMissMaxKm = 100.0;

        /// <summary>
        /// Returns the impact grade for a given distance to target in kilometres.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Distance is negative, NaN, or infinite.</exception>
        public static ImpactGrade Evaluate(double distanceKm)
        {
            if (double.IsNaN(distanceKm) || double.IsInfinity(distanceKm) || distanceKm < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(distanceKm), distanceKm,
                    "Distance must be a finite, non-negative number of kilometres.");
            }

            if (distanceKm < DirectHitMaxKm)
            {
                return ImpactGrade.DirectHit;
            }

            if (distanceKm < StrongHitMaxKm)
            {
                return ImpactGrade.StrongHit;
            }

            if (distanceKm < LightHitMaxKm)
            {
                return ImpactGrade.LightHit;
            }

            if (distanceKm < NearMissMaxKm)
            {
                return ImpactGrade.NearMiss;
            }

            return ImpactGrade.Miss;
        }
    }
}
