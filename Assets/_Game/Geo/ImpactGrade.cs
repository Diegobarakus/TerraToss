namespace TerraToss.Geo
{
    /// <summary>
    /// Classification of how close a shot landed to its target, based on distance in kilometres.
    /// Boundaries are defined by <see cref="ImpactEvaluator"/>.
    /// </summary>
    public enum ImpactGrade
    {
        /// <summary>Distance &lt; 5 km.</summary>
        DirectHit,

        /// <summary>Distance in [5, 15) km.</summary>
        StrongHit,

        /// <summary>Distance in [15, 40) km.</summary>
        LightHit,

        /// <summary>Distance in [40, 100) km.</summary>
        NearMiss,

        /// <summary>Distance &gt;= 100 km.</summary>
        Miss
    }
}
