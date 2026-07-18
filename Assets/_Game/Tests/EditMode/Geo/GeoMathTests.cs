using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class GeoMathTests
    {
        // Representative coordinates used across the MVP.
        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        // ---- Heading normalization ----

        [TestCase(0.0, 0.0)]
        [TestCase(90.0, 90.0)]
        [TestCase(359.999, 359.999)]
        [TestCase(360.0, 0.0)]
        [TestCase(370.0, 10.0)]
        [TestCase(720.0, 0.0)]
        [TestCase(-90.0, 270.0)]
        [TestCase(-360.0, 0.0)]
        [TestCase(-370.0, 350.0)]
        public void NormalizeHeading_ReturnsValueInZeroTo360(double input, double expected)
        {
            double result = GeoMath.NormalizeHeading(input);

            Assert.AreEqual(expected, result, 1e-9);
            Assert.That(result, Is.GreaterThanOrEqualTo(0.0).And.LessThan(360.0));
        }

        [Test]
        public void NormalizeHeading_NonFinite_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GeoMath.NormalizeHeading(double.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => GeoMath.NormalizeHeading(double.PositiveInfinity));
        }

        // ---- Longitude normalization ----

        [TestCase(0.0, 0.0)]
        [TestCase(179.999, 179.999)]
        [TestCase(180.0, -180.0)]
        [TestCase(190.0, -170.0)]
        [TestCase(-190.0, 170.0)]
        [TestCase(360.0, 0.0)]
        [TestCase(540.0, -180.0)]
        [TestCase(-540.0, -180.0)]
        public void NormalizeLongitude_ReturnsValueInMinus180To180(double input, double expected)
        {
            double result = GeoMath.NormalizeLongitude(input);

            Assert.AreEqual(expected, result, 1e-9);
            Assert.That(result, Is.GreaterThanOrEqualTo(-180.0).And.LessThan(180.0));
        }

        // ---- Great-circle distance ----

        [Test]
        public void GreatCircleDistance_SamePoint_IsZero()
        {
            double distance = GeoMath.GreatCircleDistanceKm(Mainz, Mainz);
            Assert.AreEqual(0.0, distance, 1e-6);
        }

        [Test]
        public void GreatCircleDistance_MainzToHelsinki_IsRepresentative()
        {
            double distance = GeoMath.GreatCircleDistanceKm(Mainz, Helsinki);

            // Spherical great-circle distance is approximately 1544 km.
            Assert.That(distance, Is.InRange(1500.0, 1590.0));
        }

        [Test]
        public void GreatCircleDistance_IsSymmetric()
        {
            double ab = GeoMath.GreatCircleDistanceKm(Mainz, Helsinki);
            double ba = GeoMath.GreatCircleDistanceKm(Helsinki, Mainz);
            Assert.AreEqual(ab, ba, 1e-9);
        }

        [Test]
        public void GreatCircleDistance_Antipodal_IsHalfCircumference()
        {
            var north = new GeoCoordinate(45.0, 0.0);
            var south = new GeoCoordinate(-45.0, 180.0);

            double distance = GeoMath.GreatCircleDistanceKm(north, south);
            double expected = Math.PI * GeoMath.EarthRadiusKm;

            Assert.AreEqual(expected, distance, 1.0);
        }

        // ---- Destination point: cardinal directions ----

        [Test]
        public void DestinationPoint_North_IncreasesLatitude()
        {
            var origin = new GeoCoordinate(0.0, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 0.0, 111.0);

            Assert.That(destination.LatitudeDegrees, Is.GreaterThan(0.0));
            Assert.AreEqual(0.0, destination.LongitudeDegrees, 1e-6);
            Assert.AreEqual(1.0, destination.LatitudeDegrees, 0.05);
        }

        [Test]
        public void DestinationPoint_South_DecreasesLatitude()
        {
            var origin = new GeoCoordinate(0.0, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 180.0, 111.0);

            Assert.That(destination.LatitudeDegrees, Is.LessThan(0.0));
            Assert.AreEqual(0.0, destination.LongitudeDegrees, 1e-6);
            Assert.AreEqual(-1.0, destination.LatitudeDegrees, 0.05);
        }

        [Test]
        public void DestinationPoint_East_IncreasesLongitude()
        {
            var origin = new GeoCoordinate(0.0, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 90.0, 111.0);

            Assert.That(destination.LongitudeDegrees, Is.GreaterThan(0.0));
            Assert.AreEqual(0.0, destination.LatitudeDegrees, 1e-6);
        }

        [Test]
        public void DestinationPoint_West_DecreasesLongitude()
        {
            var origin = new GeoCoordinate(0.0, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 270.0, 111.0);

            Assert.That(destination.LongitudeDegrees, Is.LessThan(0.0));
            Assert.AreEqual(0.0, destination.LatitudeDegrees, 1e-6);
        }

        [Test]
        public void DestinationPoint_RoundTripDistance_MatchesInput()
        {
            var origin = new GeoCoordinate(49.9929, 8.2473);
            const double distanceKm = 500.0;

            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 42.0, distanceKm);
            double measured = GeoMath.GreatCircleDistanceKm(origin, destination);

            Assert.AreEqual(distanceKm, measured, 0.5);
        }

        // ---- Destination point: antimeridian crossing ----

        [Test]
        public void DestinationPoint_CrossingAntimeridianEastward_WrapsToNegativeLongitude()
        {
            var origin = new GeoCoordinate(0.0, 179.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 90.0, 300.0);

            Assert.That(destination.LongitudeDegrees, Is.GreaterThanOrEqualTo(-180.0).And.LessThan(180.0));
            Assert.That(destination.LongitudeDegrees, Is.LessThan(0.0),
                "Travelling east past +180 should normalize the longitude to a negative value.");
        }

        [Test]
        public void DestinationPoint_CrossingAntimeridianWestward_WrapsToPositiveLongitude()
        {
            var origin = new GeoCoordinate(0.0, -179.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 270.0, 300.0);

            Assert.That(destination.LongitudeDegrees, Is.GreaterThanOrEqualTo(-180.0).And.LessThan(180.0));
            Assert.That(destination.LongitudeDegrees, Is.GreaterThan(0.0),
                "Travelling west past -180 should normalize the longitude to a positive value.");
        }

        // ---- Destination point: near the poles ----

        [Test]
        public void DestinationPoint_NearNorthPole_ProducesValidLatitude()
        {
            var origin = new GeoCoordinate(89.9, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 0.0, 50.0);

            Assert.IsTrue(GeoCoordinate.IsValidLatitude(destination.LatitudeDegrees));
            Assert.That(destination.LatitudeDegrees, Is.LessThanOrEqualTo(90.0));
        }

        [Test]
        public void DestinationPoint_NearSouthPole_ProducesValidLatitude()
        {
            var origin = new GeoCoordinate(-89.9, 0.0);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 180.0, 50.0);

            Assert.IsTrue(GeoCoordinate.IsValidLatitude(destination.LatitudeDegrees));
            Assert.That(destination.LatitudeDegrees, Is.GreaterThanOrEqualTo(-90.0));
        }

        [Test]
        public void DestinationPoint_ZeroDistance_ReturnsOrigin()
        {
            var origin = new GeoCoordinate(49.9929, 8.2473);
            GeoCoordinate destination = GeoMath.DestinationPoint(origin, 123.0, 0.0);

            Assert.AreEqual(origin.LatitudeDegrees, destination.LatitudeDegrees, 1e-9);
            Assert.AreEqual(origin.LongitudeDegrees, destination.LongitudeDegrees, 1e-9);
        }

        [Test]
        public void DestinationPoint_NegativeDistance_Throws()
        {
            var origin = new GeoCoordinate(0.0, 0.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => GeoMath.DestinationPoint(origin, 0.0, -1.0));
        }
    }
}
