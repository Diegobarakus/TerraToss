using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Deterministic fictional projectile range model. Pure C#, no UnityEngine dependency.
    /// The maximum range is always supplied by the caller; no maximum-range value is hardcoded.
    /// </summary>
    public static class ShotRangeCalculator
    {
        private const double DegToRad = Math.PI / 180.0;

        /// <summary>
        /// Computes the projectile range in kilometres using the documented model:
        /// <c>rangeKm = maximumRangeKm × power² × sin(2 × launchAngle)</c>.
        /// The launch angle is provided in degrees and converted to radians internally.
        /// </summary>
        /// <param name="maximumRangeKm">Maximum range in kilometres; must be finite and &gt; 0.</param>
        /// <param name="power">Launch power; must be finite (gameplay range [0, 1] is enforced by
        /// <see cref="GeoShotCalculator"/>).</param>
        /// <param name="launchAngleDegrees">Launch angle in degrees; must be finite (gameplay range
        /// [0, 90] is enforced by <see cref="GeoShotCalculator"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException">A precondition is violated.</exception>
        public static double ComputeRangeKm(double maximumRangeKm, double power, double launchAngleDegrees)
        {
            if (double.IsNaN(maximumRangeKm) || double.IsInfinity(maximumRangeKm) || maximumRangeKm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumRangeKm), maximumRangeKm,
                    "Maximum range must be a finite number greater than zero.");
            }

            if (double.IsNaN(power) || double.IsInfinity(power))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(power), power, "Power must be a finite number.");
            }

            if (double.IsNaN(launchAngleDegrees) || double.IsInfinity(launchAngleDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(launchAngleDegrees), launchAngleDegrees, "Launch angle must be a finite number.");
            }

            return maximumRangeKm * (power * power) * Math.Sin(2.0 * launchAngleDegrees * DegToRad);
        }
    }
}
