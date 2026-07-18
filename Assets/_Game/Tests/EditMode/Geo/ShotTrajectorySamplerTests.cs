using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class ShotTrajectorySamplerTests
    {
        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        private static ShotInput MakeInput()
        {
            return new ShotInput(Mainz, 20.0, 45.0, 1.0, 2000.0, Helsinki);
        }

        [Test]
        public void Sample_TwoSamples_AreOriginAndImpact()
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            GeoCoordinate[] samples = ShotTrajectorySampler.Sample(input, result, 2);

            Assert.AreEqual(2, samples.Length);
            Assert.AreEqual(input.Origin, samples[0]);
            Assert.AreEqual(result.ImpactCoordinate, samples[1]);
        }

        [Test]
        public void Sample_FirstSample_IsOrigin()
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            GeoCoordinate[] samples = ShotTrajectorySampler.Sample(input, result, 12);

            Assert.AreEqual(input.Origin, samples[0]);
        }

        [Test]
        public void Sample_LastSample_IsImpact()
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            GeoCoordinate[] samples = ShotTrajectorySampler.Sample(input, result, 12);

            Assert.AreEqual(result.ImpactCoordinate, samples[samples.Length - 1]);
        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(48)]
        public void Sample_ProducesRequestedCount(int count)
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(count, ShotTrajectorySampler.Sample(input, result, count).Length);
        }

        [Test]
        public void Sample_FollowsGeodesic_NotLatLonLerp()
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            GeoCoordinate[] samples = ShotTrajectorySampler.Sample(input, result, 5);

            // The midpoint must match the great-circle destination at half the travelled distance,
            // proving the sampler follows the geodesic instead of interpolating lat/lon linearly.
            GeoCoordinate expectedMid = GeoMath.DestinationPoint(
                input.Origin, result.NormalizedHeadingDegrees, result.DistanceTraveledKm * 0.5);
            Assert.AreEqual(expectedMid, samples[2]);

            double lerpLat = (input.Origin.LatitudeDegrees + result.ImpactCoordinate.LatitudeDegrees) * 0.5;
            Assert.That(Math.Abs(samples[2].LatitudeDegrees - lerpLat), Is.GreaterThan(1e-6),
                "A geodesic midpoint should not equal the linear latitude average.");
        }

        [Test]
        public void Sample_IsDeterministic()
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            GeoCoordinate[] a = ShotTrajectorySampler.Sample(input, result, 20);
            GeoCoordinate[] b = ShotTrajectorySampler.Sample(input, result, 20);

            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i].LatitudeDegrees, b[i].LatitudeDegrees);
                Assert.AreEqual(a[i].LongitudeDegrees, b[i].LongitudeDegrees);
            }
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-3)]
        public void Sample_FewerThanTwoSamples_Throws(int count)
        {
            var input = MakeInput();
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.Throws<ArgumentOutOfRangeException>(() => ShotTrajectorySampler.Sample(input, result, count));
        }

        [Test]
        public void Sample_NonFiniteTravelledDistance_Throws()
        {
            var input = MakeInput();
            var badResult = new ShotResult(Mainz, double.NaN, 0.0, 20.0, 45.0, 1.0, ImpactGrade.Miss);

            Assert.Throws<ArgumentOutOfRangeException>(() => ShotTrajectorySampler.Sample(input, badResult, 5));
        }

        [Test]
        public void Sample_NonFiniteHeading_Throws()
        {
            var input = MakeInput();
            var badResult = new ShotResult(Mainz, 1000.0, 0.0, double.PositiveInfinity, 45.0, 1.0, ImpactGrade.Miss);

            Assert.Throws<ArgumentOutOfRangeException>(() => ShotTrajectorySampler.Sample(input, badResult, 5));
        }
    }
}
