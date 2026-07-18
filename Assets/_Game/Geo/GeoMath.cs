using System;

namespace TerraToss.Geo
{
    /// <summary>
    /// Deterministic spherical-Earth geographic math. Pure C#, no UnityEngine dependency.
    /// Public distances are in kilometres and public angles in degrees.
    /// </summary>
    public static class GeoMath
    {
        /// <summary>
        /// Mean Earth radius in kilometres (IUGG mean radius). Used for the spherical MVP model.
        /// </summary>
        public const double EarthRadiusKm = 6371.0088;

        private const double DegToRad = Math.PI / 180.0;
        private const double RadToDeg = 180.0 / Math.PI;

        /// <summary>
        /// Normalizes a heading in degrees to the range [0, 360).
        /// Example: -90 → 270, 370 → 10, 360 → 0, 720 → 0.
        /// </summary>
        public static double NormalizeHeading(double headingDegrees)
        {
            if (double.IsNaN(headingDegrees) || double.IsInfinity(headingDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(headingDegrees), headingDegrees, "Heading must be a finite number.");
            }

            double result = headingDegrees % 360.0;
            if (result < 0.0)
            {
                result += 360.0;
            }

            // Guard against a floating-point result of exactly 360 after the addition above.
            if (result >= 360.0)
            {
                result -= 360.0;
            }

            return result;
        }

        /// <summary>
        /// Normalizes a longitude in degrees to the range [-180, 180).
        /// Example: 190 → -170, -190 → 170, 180 → -180.
        /// </summary>
        public static double NormalizeLongitude(double longitudeDegrees)
        {
            if (double.IsNaN(longitudeDegrees) || double.IsInfinity(longitudeDegrees))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(longitudeDegrees), longitudeDegrees, "Longitude must be a finite number.");
            }

            double result = (longitudeDegrees + 180.0) % 360.0;
            if (result < 0.0)
            {
                result += 360.0;
            }

            result -= 180.0;

            // Keep the range half-open at [-180, 180): a computed +180 maps to -180.
            if (result >= 180.0)
            {
                result -= 360.0;
            }

            return result;
        }

        /// <summary>
        /// Great-circle distance between two coordinates in kilometres, using the haversine formula
        /// on a spherical Earth.
        /// </summary>
        public static double GreatCircleDistanceKm(GeoCoordinate a, GeoCoordinate b)
        {
            double lat1 = a.LatitudeDegrees * DegToRad;
            double lat2 = b.LatitudeDegrees * DegToRad;
            double deltaLat = (b.LatitudeDegrees - a.LatitudeDegrees) * DegToRad;
            double deltaLon = (b.LongitudeDegrees - a.LongitudeDegrees) * DegToRad;

            double sinHalfLat = Math.Sin(deltaLat * 0.5);
            double sinHalfLon = Math.Sin(deltaLon * 0.5);

            double h = (sinHalfLat * sinHalfLat)
                       + (Math.Cos(lat1) * Math.Cos(lat2) * sinHalfLon * sinHalfLon);

            // Clamp to guard against tiny floating-point overshoot above 1.0.
            double c = 2.0 * Math.Asin(Math.Min(1.0, Math.Sqrt(h)));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Computes the destination coordinate reached from <paramref name="origin"/> by travelling
        /// <paramref name="distanceKm"/> kilometres along the initial <paramref name="headingDegrees"/>
        /// (0 = north, 90 = east, 180 = south, 270 = west) over a spherical Earth.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Distance is negative, NaN, or infinite.</exception>
        public static GeoCoordinate DestinationPoint(GeoCoordinate origin, double headingDegrees, double distanceKm)
        {
            if (double.IsNaN(distanceKm) || double.IsInfinity(distanceKm) || distanceKm < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(distanceKm), distanceKm, "Distance must be a finite, non-negative number of kilometres.");
            }

            double heading = NormalizeHeading(headingDegrees) * DegToRad;
            double angularDistance = distanceKm / EarthRadiusKm;

            double lat1 = origin.LatitudeDegrees * DegToRad;
            double lon1 = origin.LongitudeDegrees * DegToRad;

            double sinLat1 = Math.Sin(lat1);
            double cosLat1 = Math.Cos(lat1);
            double sinAngular = Math.Sin(angularDistance);
            double cosAngular = Math.Cos(angularDistance);

            double sinLat2 = (sinLat1 * cosAngular) + (cosLat1 * sinAngular * Math.Cos(heading));

            // Clamp to the valid domain of Asin to keep latitude within [-90, 90] near the poles.
            sinLat2 = Math.Max(-1.0, Math.Min(1.0, sinLat2));
            double lat2 = Math.Asin(sinLat2);

            double y = Math.Sin(heading) * sinAngular * cosLat1;
            double x = cosAngular - (sinLat1 * sinLat2);
            double lon2 = lon1 + Math.Atan2(y, x);

            double latitudeDegrees = lat2 * RadToDeg;
            double longitudeDegrees = NormalizeLongitude(lon2 * RadToDeg);

            return new GeoCoordinate(latitudeDegrees, longitudeDegrees);
        }
    }
}
