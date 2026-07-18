using System;
using NUnit.Framework;
using TerraToss.Geo;
using TerraToss.Presentation;
using UnityEngine;

namespace TerraToss.Presentation.Tests.EditMode
{
    [TestFixture]
    public class TrajectoryArcProjectionTests
    {
        private const double Radius = 5.0;
        private const double MaxArc = 2.0;
        private const float Tolerance = 1e-3f;

        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        private static GeoCoordinate[] MakeSamples(int count)
        {
            var input = new ShotInput(Mainz, 20.0, 45.0, 1.0, 2000.0, Helsinki);
            ShotResult result = GeoShotCalculator.Calculate(input);
            return ShotTrajectorySampler.Sample(input, result, count);
        }

        [Test]
        public void ToVisualPoints_HeightIsZeroAtBothEnds()
        {
            Vector3[] points = TrajectoryArcProjection.ToVisualPoints(MakeSamples(9), Radius, MaxArc);

            Assert.AreEqual((float)Radius, points[0].magnitude, Tolerance);
            Assert.AreEqual((float)Radius, points[points.Length - 1].magnitude, Tolerance);
        }

        [Test]
        public void ToVisualPoints_MaxHeightIsNearCentre()
        {
            // Odd count => exact centre at progress 0.5 => full arc height.
            Vector3[] points = TrajectoryArcProjection.ToVisualPoints(MakeSamples(9), Radius, MaxArc);
            int centre = points.Length / 2;

            Assert.AreEqual((float)(Radius + MaxArc), points[centre].magnitude, Tolerance);

            for (int i = 0; i < points.Length; i++)
            {
                if (i != centre)
                {
                    Assert.That(points[centre].magnitude, Is.GreaterThanOrEqualTo(points[i].magnitude - Tolerance));
                }
            }
        }

        [Test]
        public void ToVisualPoints_AllPointsOnOrOutsideRadius()
        {
            Vector3[] points = TrajectoryArcProjection.ToVisualPoints(MakeSamples(24), Radius, MaxArc);

            foreach (Vector3 p in points)
            {
                Assert.That(p.magnitude, Is.GreaterThanOrEqualTo((float)Radius - Tolerance));
            }
        }

        [Test]
        public void ToVisualPoints_ZeroArcHeight_KeepsPointsOnSurface()
        {
            Vector3[] points = TrajectoryArcProjection.ToVisualPoints(MakeSamples(6), Radius, 0.0);

            foreach (Vector3 p in points)
            {
                Assert.AreEqual((float)Radius, p.magnitude, Tolerance);
            }
        }

        [Test]
        public void ToVisualPoints_IsDeterministic()
        {
            GeoCoordinate[] samples = MakeSamples(16);
            Vector3[] a = TrajectoryArcProjection.ToVisualPoints(samples, Radius, MaxArc);
            Vector3[] b = TrajectoryArcProjection.ToVisualPoints(samples, Radius, MaxArc);

            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i].x, b[i].x);
                Assert.AreEqual(a[i].y, b[i].y);
                Assert.AreEqual(a[i].z, b[i].z);
            }
        }

        [Test]
        public void ToVisualPoints_NullSamples_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => TrajectoryArcProjection.ToVisualPoints(null, Radius, MaxArc));
        }

        [Test]
        public void ToVisualPoints_FewerThanTwoSamples_Throws()
        {
            var single = new[] { Mainz };
            Assert.Throws<ArgumentOutOfRangeException>(
                () => TrajectoryArcProjection.ToVisualPoints(single, Radius, MaxArc));
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ToVisualPoints_InvalidRadius_Throws(double radius)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => TrajectoryArcProjection.ToVisualPoints(MakeSamples(4), radius, MaxArc));
        }

        [TestCase(-0.5)]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        public void ToVisualPoints_InvalidArcHeight_Throws(double height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => TrajectoryArcProjection.ToVisualPoints(MakeSamples(4), Radius, height));
        }
    }
}
