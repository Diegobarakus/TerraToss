namespace TerraToss.Geo
{
    /// <summary>
    /// Immutable definition of a single shot. Pure data: gameplay-rule validation is
    /// performed by <see cref="GeoShotCalculator"/>, not by this type. Origin and target
    /// are validated at <see cref="GeoCoordinate"/> construction time.
    /// Angles are in degrees, distances in kilometres, and power is normalized to [0, 1].
    /// </summary>
    public readonly struct ShotInput
    {
        /// <summary>Launch origin coordinate.</summary>
        public GeoCoordinate Origin { get; }

        /// <summary>Launch heading in degrees (0 = north, 90 = east). Normalized during calculation.</summary>
        public double HeadingDegrees { get; }

        /// <summary>Launch angle in degrees, expected within [0, 90].</summary>
        public double LaunchAngleDegrees { get; }

        /// <summary>Normalized launch power, expected within [0, 1].</summary>
        public double Power { get; }

        /// <summary>Maximum range in kilometres for full power at the optimal angle. Must be &gt; 0.</summary>
        public double MaximumRangeKm { get; }

        /// <summary>Coordinate of the active target.</summary>
        public GeoCoordinate Target { get; }

        public ShotInput(
            GeoCoordinate origin,
            double headingDegrees,
            double launchAngleDegrees,
            double power,
            double maximumRangeKm,
            GeoCoordinate target)
        {
            Origin = origin;
            HeadingDegrees = headingDegrees;
            LaunchAngleDegrees = launchAngleDegrees;
            Power = power;
            MaximumRangeKm = maximumRangeKm;
            Target = target;
        }
    }
}
