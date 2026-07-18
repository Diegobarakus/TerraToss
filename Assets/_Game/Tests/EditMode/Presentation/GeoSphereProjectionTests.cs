using System;
using NUnit.Framework;
using TerraToss.Geo;
using TerraToss.Presentation;
using UnityEngine;

namespace TerraToss.Presentation.Tests.EditMode
{
    [TestFixture]
    public class GeoSphereProjectionTests
    {
        private const double Radius = 5.0;
        private const float Tolerance = 1e-3f;

        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        [Test]
        public void ToLocalPosition_MagnitudeEqualsRadius()
        {
            foreach (var c in new[]
            {
                new GeoCoordinate(0.0, 0.0),
                new GeoCoordinate(45.0, 45.0),
                new GeoCoordinate(-30.0, 120.0),
                Mainz,
                Helsinki,
            })
            {
                Vector3 v = GeoSphereProjection.ToLocalPosition(c, Radius);
                Assert.AreEqual((float)Radius, v.magnitude, Tolerance, $"coordinate {c}");
            }
        }

        [Test]
        public void ToLocalPosition_EquatorPrimeMeridian_IsPositiveZ()
        {
            Vector3 v = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(0.0, 0.0), Radius);
            Assert.AreEqual(0f, v.x, Tolerance);
            Assert.AreEqual(0f, v.y, Tolerance);
            Assert.AreEqual((float)Radius, v.z, Tolerance);
        }

        [Test]
        public void ToLocalPosition_East_IsPositiveX_West_IsNegativeX()
        {
            Vector3 east = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(0.0, 90.0), Radius);
            Vector3 west = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(0.0, -90.0), Radius);

            Assert.AreEqual((float)Radius, east.x, Tolerance);
            Assert.That(east.x, Is.GreaterThan(0f));
            Assert.AreEqual(-(float)Radius, west.x, Tolerance);
            Assert.That(west.x, Is.LessThan(0f));
        }

        [Test]
        public void ToLocalPosition_NorthPole_IsPositiveY()
        {
            Vector3 v = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(90.0, 0.0), Radius);
            Assert.AreEqual(0f, v.x, Tolerance);
            Assert.AreEqual((float)Radius, v.y, Tolerance);
            Assert.AreEqual(0f, v.z, Tolerance);
        }

        [Test]
        public void ToLocalPosition_SouthPole_IsNegativeY_OppositeToNorth()
        {
            Vector3 north = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(90.0, 0.0), Radius);
            Vector3 south = GeoSphereProjection.ToLocalPosition(new GeoCoordinate(-90.0, 0.0), Radius);

            Assert.AreEqual(-(float)Radius, south.y, Tolerance);
            // North and south poles are opposite extremes.
            Assert.AreEqual(-north.y, south.y, Tolerance);
            Assert.AreEqual((north + south).magnitude, 0f, Tolerance);
        }

        [Test]
        public void ToLocalPosition_Mainz_IsValidNorthernPosition()
        {
            Vector3 v = GeoSphereProjection.ToLocalPosition(Mainz, Radius);
            Assert.AreEqual((float)Radius, v.magnitude, Tolerance);
            Assert.That(v.y, Is.GreaterThan(0f), "Mainz is in the northern hemisphere.");
            Assert.IsFalse(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z));
        }

        [Test]
        public void ToLocalPosition_Helsinki_IsValidNorthernPosition()
        {
            Vector3 v = GeoSphereProjection.ToLocalPosition(Helsinki, Radius);
            Assert.AreEqual((float)Radius, v.magnitude, Tolerance);
            Assert.That(v.y, Is.GreaterThan(0f), "Helsinki is in the northern hemisphere.");
            // Helsinki is further north than Mainz.
            Vector3 mainz = GeoSphereProjection.ToLocalPosition(Mainz, Radius);
            Assert.That(v.y, Is.GreaterThan(mainz.y));
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ToLocalPosition_InvalidRadius_Throws(double radius)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => GeoSphereProjection.ToLocalPosition(new GeoCoordinate(0.0, 0.0), radius));
        }

        [Test]
        public void ToLocalPosition_IsDeterministic()
        {
            Vector3 a = GeoSphereProjection.ToLocalPosition(Mainz, Radius);
            Vector3 b = GeoSphereProjection.ToLocalPosition(Mainz, Radius);

            // Identical computation must be bit-for-bit equal; exact equality is intended here.
            Assert.AreEqual(a.x, b.x);
            Assert.AreEqual(a.y, b.y);
            Assert.AreEqual(a.z, b.z);
        }

        [Test]
        public void SurfaceNormal_IsUnitLength_AndRadial()
        {
            Vector3 normal = GeoSphereProjection.SurfaceNormal(Mainz);
            Assert.AreEqual(1f, normal.magnitude, Tolerance);

            Vector3 position = GeoSphereProjection.ToLocalPosition(Mainz, Radius);
            // The normal points in the same direction as the position vector.
            Assert.AreEqual(1f, Vector3.Dot(normal, position.normalized), Tolerance);
        }
    }
}
