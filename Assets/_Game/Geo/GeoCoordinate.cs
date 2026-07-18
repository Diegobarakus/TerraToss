using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Immutable geographic coordinate expressed in degrees.
    /// Latitude is constrained to [-90, 90]. Longitude is stored normalized to [-180, 180).
    /// Pure C# value type: no dependency on UnityEngine or MonoBehaviour.
    /// </summary>
    public readonly struct GeoCoordinate : IEquatable<GeoCoordinate>
    {
        /// <summary>Minimum valid latitude in degrees.</summary>
        public const double MinLatitude = -90.0;

        /// <summary>Maximum valid latitude in degrees.</summary>
        public const double MaxLatitude = 90.0;

        /// <summary>Latitude in degrees, always within [-90, 90].</summary>
        public double LatitudeDegrees { get; }

        /// <summary>Longitude in degrees, always normalized to [-180, 180).</summary>
        public double LongitudeDegrees { get; }

        /// <summary>
        /// Creates a coordinate. Latitude must be within [-90, 90] or an exception is thrown.
        /// Longitude is normalized to [-180, 180) so equivalent inputs produce equal coordinates.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Latitude is NaN, infinite, or outside [-90, 90].</exception>
        /// <exception cref="ArgumentException">Longitude is NaN or infinite.</exception>
        public GeoCoordinate(double latitudeDegrees, double longitudeDegrees)
        {
            if (double.IsNaN(latitudeDegrees) || double.IsInfinity(latitudeDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(latitudeDegrees), latitudeDegrees, "Latitude must be a finite number.");
            }

            if (latitudeDegrees < MinLatitude || latitudeDegrees > MaxLatitude)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(latitudeDegrees), latitudeDegrees,
                    "Latitude must be within [-90, 90] degrees.");
            }

            if (double.IsNaN(longitudeDegrees) || double.IsInfinity(longitudeDegrees))
            {
                throw new ArgumentException("Longitude must be a finite number.", nameof(longitudeDegrees));
            }

            LatitudeDegrees = latitudeDegrees;
            LongitudeDegrees = GeoMath.NormalizeLongitude(longitudeDegrees);
        }

        /// <summary>
        /// Returns true and outputs a coordinate when the inputs are valid; otherwise returns false.
        /// Does not throw. Longitude is normalized on success.
        /// </summary>
        public static bool TryCreate(double latitudeDegrees, double longitudeDegrees, out GeoCoordinate coordinate)
        {
            if (!IsValidLatitude(latitudeDegrees) ||
                double.IsNaN(longitudeDegrees) || double.IsInfinity(longitudeDegrees))
            {
                coordinate = default;
                return false;
            }

            coordinate = new GeoCoordinate(latitudeDegrees, longitudeDegrees);
            return true;
        }

        /// <summary>Returns true when the latitude is a finite value within [-90, 90].</summary>
        public static bool IsValidLatitude(double latitudeDegrees)
        {
            return !double.IsNaN(latitudeDegrees)
                   && !double.IsInfinity(latitudeDegrees)
                   && latitudeDegrees >= MinLatitude
                   && latitudeDegrees <= MaxLatitude;
        }

        public bool Equals(GeoCoordinate other)
        {
            return LatitudeDegrees.Equals(other.LatitudeDegrees)
                   && LongitudeDegrees.Equals(other.LongitudeDegrees);
        }

        public override bool Equals(object obj)
        {
            return obj is GeoCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LatitudeDegrees.GetHashCode() * 397) ^ LongitudeDegrees.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"({LatitudeDegrees:0.######}, {LongitudeDegrees:0.######})";
        }
    }
}
