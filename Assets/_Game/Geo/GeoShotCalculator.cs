using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Deterministic shot calculation. Validates a <see cref="ShotInput"/>, computes the fictional
    /// range, resolves the geographic impact coordinate, measures the distance to the target, and
    /// classifies the impact. Pure C#, no UnityEngine dependency. Reuses <see cref="GeoMath"/>,
    /// <see cref="ShotRangeCalculator"/> and <see cref="ImpactEvaluator"/> without duplicating logic.
    /// </summary>
    public static class GeoShotCalculator
    {
        /// <summary>Minimum valid launch angle in degrees.</summary>
        public const double MinLaunchAngleDegrees = 0.0;

        /// <summary>Maximum valid launch angle in degrees.</summary>
        public const double MaxLaunchAngleDegrees = 90.0;

        /// <summary>Minimum valid normalized power.</summary>
        public const double MinPower = 0.0;

        /// <summary>Maximum valid normalized power.</summary>
        public const double MaxPower = 1.0;

        /// <summary>
        /// Calculates the full result of a shot.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Power is outside [0, 1], launch angle is outside [0, 90], maximum range is not a finite
        /// positive number, or the heading is not finite.
        /// </exception>
        public static ShotResult Calculate(ShotInput input)
        {
            Validate(input);

            double normalizedHeading = GeoMath.NormalizeHeading(input.HeadingDegrees);
            double rangeKm = ShotRangeCalculator.ComputeRangeKm(
                input.MaximumRangeKm, input.Power, input.LaunchAngleDegrees);

            GeoCoordinate impact = GeoMath.DestinationPoint(input.Origin, normalizedHeading, rangeKm);
            double distanceToTargetKm = GeoMath.GreatCircleDistanceKm(impact, input.Target);
            ImpactGrade grade = ImpactEvaluator.Evaluate(distanceToTargetKm);

            return new ShotResult(
                impact,
                rangeKm,
                distanceToTargetKm,
                normalizedHeading,
                input.LaunchAngleDegrees,
                input.Power,
                grade);
        }

        private static void Validate(ShotInput input)
        {
            if (double.IsNaN(input.HeadingDegrees) || double.IsInfinity(input.HeadingDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.HeadingDegrees), input.HeadingDegrees, "Heading must be a finite number.");
            }

            if (double.IsNaN(input.Power) || double.IsInfinity(input.Power)
                || input.Power < MinPower || input.Power > MaxPower)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.Power), input.Power, "Power must be a finite number within [0, 1].");
            }

            if (double.IsNaN(input.LaunchAngleDegrees) || double.IsInfinity(input.LaunchAngleDegrees)
                || input.LaunchAngleDegrees < MinLaunchAngleDegrees
                || input.LaunchAngleDegrees > MaxLaunchAngleDegrees)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.LaunchAngleDegrees), input.LaunchAngleDegrees,
                    "Launch angle must be a finite number within [0, 90] degrees.");
            }

            if (double.IsNaN(input.MaximumRangeKm) || double.IsInfinity(input.MaximumRangeKm)
                || input.MaximumRangeKm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.MaximumRangeKm), input.MaximumRangeKm,
                    "Maximum range must be a finite number greater than zero.");
            }
        }
    }
}
