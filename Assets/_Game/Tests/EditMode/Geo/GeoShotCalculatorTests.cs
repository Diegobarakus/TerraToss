using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class GeoShotCalculatorTests
    {
        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        private static ShotInput MakeInput(
            double heading = 20.0,
            double angle = 45.0,
            double power = 1.0,
            double maxRange = 2000.0)
        {
            return new ShotInput(Mainz, heading, angle, power, maxRange, Helsinki);
        }

        [Test]
        public void Calculate_ZeroPower_ImpactAtOrigin()
        {
            var input = MakeInput(power: 0.0);
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(0.0, result.DistanceTraveledKm, 1e-9);
            Assert.AreEqual(Mainz.LatitudeDegrees, result.ImpactCoordinate.LatitudeDegrees, 1e-9);
            Assert.AreEqual(Mainz.LongitudeDegrees, result.ImpactCoordinate.LongitudeDegrees, 1e-9);
            // With no travel, distance to target is the origin-to-target distance.
            Assert.AreEqual(
                GeoMath.GreatCircleDistanceKm(Mainz, Helsinki), result.DistanceToTargetKm, 1e-9);
        }

        [TestCase(0.0)]
        [TestCase(90.0)]
        public void Calculate_AngleExtremes_ProduceZeroRange(double angle)
        {
            var input = MakeInput(angle: angle);
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(0.0, result.DistanceTraveledKm, 1e-9);
            Assert.AreEqual(Mainz.LatitudeDegrees, result.ImpactCoordinate.LatitudeDegrees, 1e-9);
            Assert.AreEqual(Mainz.LongitudeDegrees, result.ImpactCoordinate.LongitudeDegrees, 1e-9);
        }

        [Test]
        public void Calculate_FullPowerAt45_TravelsMaximumRange()
        {
            var input = MakeInput(angle: 45.0, power: 1.0, maxRange: 2000.0);
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(2000.0, result.DistanceTraveledKm, 1e-6);
        }

        [Test]
        public void Calculate_NegativeHeading_IsNormalized()
        {
            ShotResult result = GeoShotCalculator.Calculate(MakeInput(heading: -90.0));

            Assert.AreEqual(GeoMath.NormalizeHeading(-90.0), result.NormalizedHeadingDegrees, 1e-9);
            Assert.AreEqual(270.0, result.NormalizedHeadingDegrees, 1e-9);
        }

        [Test]
        public void Calculate_HeadingAbove360_IsNormalized()
        {
            ShotResult result = GeoShotCalculator.Calculate(MakeInput(heading: 450.0));

            Assert.AreEqual(90.0, result.NormalizedHeadingDegrees, 1e-9);
        }

        [Test]
        public void Calculate_SameInput_ProducesIdenticalResult()
        {
            var input = MakeInput(heading: 33.0, angle: 42.0, power: 0.8, maxRange: 1500.0);
            ShotResult a = GeoShotCalculator.Calculate(input);
            ShotResult b = GeoShotCalculator.Calculate(input);

            // Determinism: identical computation must be bit-for-bit equal, so exact equality is intended here.
            Assert.AreEqual(a.ImpactCoordinate.LatitudeDegrees, b.ImpactCoordinate.LatitudeDegrees);
            Assert.AreEqual(a.ImpactCoordinate.LongitudeDegrees, b.ImpactCoordinate.LongitudeDegrees);
            Assert.AreEqual(a.DistanceTraveledKm, b.DistanceTraveledKm);
            Assert.AreEqual(a.DistanceToTargetKm, b.DistanceToTargetKm);
            Assert.AreEqual(a.NormalizedHeadingDegrees, b.NormalizedHeadingDegrees);
            Assert.AreEqual(a.Grade, b.Grade);
        }

        [Test]
        public void Calculate_ProducesValidImpactCoordinate()
        {
            ShotResult result = GeoShotCalculator.Calculate(MakeInput());

            Assert.IsTrue(GeoCoordinate.IsValidLatitude(result.ImpactCoordinate.LatitudeDegrees));
            Assert.That(result.ImpactCoordinate.LongitudeDegrees,
                Is.GreaterThanOrEqualTo(-180.0).And.LessThan(180.0));
        }

        [Test]
        public void Calculate_WhenTargetIsImpactPoint_DistanceIsZeroDirectHit()
        {
            // First compute where the shot lands, then aim at exactly that point.
            ShotResult first = GeoShotCalculator.Calculate(MakeInput(heading: 90.0, maxRange: 100.0));
            var aimed = new ShotInput(Mainz, 90.0, 45.0, 1.0, 100.0, first.ImpactCoordinate);

            ShotResult result = GeoShotCalculator.Calculate(aimed);

            Assert.AreEqual(0.0, result.DistanceToTargetKm, 1e-6);
            Assert.AreEqual(ImpactGrade.DirectHit, result.Grade);
        }

        [Test]
        public void Calculate_GradeMatchesEvaluatorForMeasuredDistance()
        {
            ShotResult result = GeoShotCalculator.Calculate(MakeInput());
            Assert.AreEqual(ImpactEvaluator.Evaluate(result.DistanceToTargetKm), result.Grade);
        }

        [Test]
        public void Calculate_CarriesPowerAndAngleIntoResult()
        {
            var input = MakeInput(angle: 37.0, power: 0.6);
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(37.0, result.LaunchAngleDegrees, 1e-12);
            Assert.AreEqual(0.6, result.Power, 1e-12);
        }

        // ---- Validation ----

        [TestCase(-0.001)]
        [TestCase(1.001)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Calculate_InvalidPower_Throws(double power)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => GeoShotCalculator.Calculate(MakeInput(power: power)));
        }

        [TestCase(-0.001)]
        [TestCase(90.001)]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        public void Calculate_InvalidAngle_Throws(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => GeoShotCalculator.Calculate(MakeInput(angle: angle)));
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Calculate_InvalidMaximumRange_Throws(double maxRange)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => GeoShotCalculator.Calculate(MakeInput(maxRange: maxRange)));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Calculate_NonFiniteHeading_Throws(double heading)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => GeoShotCalculator.Calculate(MakeInput(heading: heading)));
        }

        [Test]
        public void Calculate_MainzToHelsinki_RepresentativeShot()
        {
            var input = new ShotInput(Mainz, 20.0, 45.0, 1.0, 2000.0, Helsinki);
            ShotResult result = GeoShotCalculator.Calculate(input);

            Assert.AreEqual(2000.0, result.DistanceTraveledKm, 1.0);
            Assert.IsTrue(GeoCoordinate.IsValidLatitude(result.ImpactCoordinate.LatitudeDegrees));
            Assert.That(result.DistanceToTargetKm, Is.GreaterThanOrEqualTo(0.0));
            Assert.AreEqual(ImpactEvaluator.Evaluate(result.DistanceToTargetKm), result.Grade);
        }
    }
}
