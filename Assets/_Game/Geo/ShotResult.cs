namespace TerraToss.Geo
{
    /// <summary>
    /// Immutable, deterministic result of a shot calculation produced by
    /// <see cref="GeoShotCalculator"/>. Angles are in degrees and distances in kilometres.
    /// </summary>
    public readonly struct ShotResult
    {
        /// <summary>Geographic coordinate where the projectile landed.</summary>
        public GeoCoordinate ImpactCoordinate { get; }

        /// <summary>Distance travelled from origin to impact, in kilometres.</summary>
        public double DistanceTraveledKm { get; }

        /// <summary>Great-circle distance from the impact point to the target, in kilometres.</summary>
        public double DistanceToTargetKm { get; }

        /// <summary>Heading actually used, normalized to [0, 360) degrees.</summary>
        public double NormalizedHeadingDegrees { get; }

        /// <summary>Launch angle used, in degrees.</summary>
        public double LaunchAngleDegrees { get; }

        /// <summary>Normalized power used, within [0, 1].</summary>
        public double Power { get; }

        /// <summary>Impact classification derived from <see cref="DistanceToTargetKm"/>.</summary>
        public ImpactGrade Grade { get; }

        public ShotResult(
            GeoCoordinate impactCoordinate,
            double distanceTraveledKm,
            double distanceToTargetKm,
            double normalizedHeadingDegrees,
            double launchAngleDegrees,
            double power,
            ImpactGrade grade)
        {
            ImpactCoordinate = impactCoordinate;
            DistanceTraveledKm = distanceTraveledKm;
            DistanceToTargetKm = distanceToTargetKm;
            NormalizedHeadingDegrees = normalizedHeadingDegrees;
            LaunchAngleDegrees = launchAngleDegrees;
            Power = power;
            Grade = grade;
        }

        public override string ToString()
        {
            return $"Impact {ImpactCoordinate}, traveled {DistanceTraveledKm:0.###} km, " +
                   $"toTarget {DistanceToTargetKm:0.###} km, grade {Grade}";
        }
    }
}
