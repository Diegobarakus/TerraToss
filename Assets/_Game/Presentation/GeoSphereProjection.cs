using System;
using UnityEngine;
using TerraToss.Geo;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Projects a <see cref="GeoCoordinate"/> onto a local position on a sphere of a given radius.
    ///
    /// Axis convention (Unity, left-handed, Y up):
    ///   x = R * cos(lat) * sin(lon)
    ///   y = R * sin(lat)
    ///   z = R * cos(lat) * cos(lon)
    ///
    /// Which means:
    ///   - (lat 0, lon 0)  -> (0, 0, +R)  : +Z, Unity forward (reference meridian on the equator)
    ///   - North pole (+90) -> (0, +R, 0) : +Y
    ///   - South pole (-90) -> (0, -R, 0) : -Y  (opposite extreme)
    ///   - East  (lon +90) -> (+R, 0, 0)  : +X, right
    ///   - West  (lon -90) -> (-R, 0, 0)  : -X, left
    ///
    /// The magnitude of the returned vector is exactly R.
    /// This is a visual conversion only and may depend on UnityEngine; it does not duplicate or
    /// modify the pure geographic engine (<see cref="GeoMath"/>).
    /// </summary>
    public static class GeoSphereProjection
    {
        private const double DegToRad = Math.PI / 180.0;

        /// <summary>
        /// Converts a coordinate to a local position on a sphere of the given radius.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Radius is not a finite number greater than zero.</exception>
        public static Vector3 ToLocalPosition(GeoCoordinate coordinate, double sphereRadius)
        {
            if (double.IsNaN(sphereRadius) || double.IsInfinity(sphereRadius) || sphereRadius <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sphereRadius), sphereRadius,
                    "Sphere radius must be a finite number greater than zero.");
            }

            double latRad = coordinate.LatitudeDegrees * DegToRad;
            double lonRad = coordinate.LongitudeDegrees * DegToRad;
            double cosLat = Math.Cos(latRad);

            double x = sphereRadius * cosLat * Math.Sin(lonRad);
            double y = sphereRadius * Math.Sin(latRad);
            double z = sphereRadius * cosLat * Math.Cos(lonRad);

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Outward surface normal (unit vector) at the given coordinate, useful to orient markers.
        /// </summary>
        public static Vector3 SurfaceNormal(GeoCoordinate coordinate)
        {
            // On a unit sphere the position vector is already the outward normal.
            return ToLocalPosition(coordinate, 1.0).normalized;
        }
    }
}
