using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class GeoCoordinateTests
    {
        [Test]
        public void Constructor_ValidCoordinate_StoresLatitudeAndLongitude()
        {
            var coordinate = new GeoCoordinate(49.9929, 8.2473);

            Assert.AreEqual(49.9929, coordinate.LatitudeDegrees, 1e-9);
            Assert.AreEqual(8.2473, coordinate.LongitudeDegrees, 1e-9);
        }

        [Test]
        public void Constructor_LatitudeAtBounds_IsAccepted()
        {
            Assert.DoesNotThrow(() => new GeoCoordinate(90.0, 0.0));
            Assert.DoesNotThrow(() => new GeoCoordinate(-90.0, 0.0));
        }

        [TestCase(90.0001)]
        [TestCase(-90.0001)]
        [TestCase(120.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Constructor_InvalidLatitude_Throws(double latitude)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(latitude, 0.0));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Constructor_NonFiniteLongitude_Throws(double longitude)
        {
            Assert.Throws<ArgumentException>(() => new GeoCoordinate(0.0, longitude));
        }

        [Test]
        public void Constructor_OutOfRangeLongitude_IsNormalized()
        {
            var coordinate = new GeoCoordinate(0.0, 190.0);
            Assert.AreEqual(-170.0, coordinate.LongitudeDegrees, 1e-9);
        }

        [Test]
        public void TryCreate_ValidInput_ReturnsTrueAndNormalizes()
        {
            bool ok = GeoCoordinate.TryCreate(10.0, 200.0, out GeoCoordinate coordinate);

            Assert.IsTrue(ok);
            Assert.AreEqual(10.0, coordinate.LatitudeDegrees, 1e-9);
            Assert.AreEqual(-160.0, coordinate.LongitudeDegrees, 1e-9);
        }

        [TestCase(95.0, 0.0)]
        [TestCase(double.NaN, 0.0)]
        [TestCase(0.0, double.NaN)]
        [TestCase(0.0, double.PositiveInfinity)]
        public void TryCreate_InvalidInput_ReturnsFalse(double latitude, double longitude)
        {
            bool ok = GeoCoordinate.TryCreate(latitude, longitude, out _);
            Assert.IsFalse(ok);
        }

        [Test]
        public void Equality_SameNormalizedValues_AreEqual()
        {
            var a = new GeoCoordinate(45.0, 170.0);
            var b = new GeoCoordinate(45.0, 170.0 + 360.0);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void IsValidLatitude_ChecksFiniteRange()
        {
            Assert.IsTrue(GeoCoordinate.IsValidLatitude(0.0));
            Assert.IsTrue(GeoCoordinate.IsValidLatitude(90.0));
            Assert.IsFalse(GeoCoordinate.IsValidLatitude(90.5));
            Assert.IsFalse(GeoCoordinate.IsValidLatitude(double.NaN));
        }
    }
}
