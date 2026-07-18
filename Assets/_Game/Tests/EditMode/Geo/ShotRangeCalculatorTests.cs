using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class ShotRangeCalculatorTests
    {
        private const double MaxRangeKm = 1000.0;

        [Test]
        public void ComputeRange_ZeroPower_IsZero()
        {
            Assert.AreEqual(0.0, ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 0.0, 45.0), 1e-12);
        }

        [Test]
        public void ComputeRange_FullPowerAt45Degrees_EqualsMaximumRange()
        {
            Assert.AreEqual(MaxRangeKm, ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 45.0), 1e-9);
        }

        [TestCase(0.0)]
        [TestCase(90.0)]
        public void ComputeRange_AngleAtExtremes_IsZero(double angle)
        {
            Assert.AreEqual(0.0, ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, angle), 1e-9);
        }

        [Test]
        public void ComputeRange_FullPower_UsesAngleCapacity()
        {
            // At full power the range is maxRange * sin(2 * angle). At 30 deg that is sin(60 deg).
            double expected = MaxRangeKm * Math.Sin(60.0 * Math.PI / 180.0);
            Assert.AreEqual(expected, ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 30.0), 1e-9);
        }

        [Test]
        public void ComputeRange_45Degrees_IsMaximumOverAngles()
        {
            double at45 = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 45.0);
            double at30 = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 30.0);
            double at60 = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 60.0);

            Assert.That(at45, Is.GreaterThan(at30));
            Assert.That(at45, Is.GreaterThan(at60));
        }

        [Test]
        public void ComputeRange_PowerScalesQuadratically()
        {
            double full = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, 45.0);
            double half = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 0.5, 45.0);

            // Halving power quarters the range (power squared).
            Assert.AreEqual(full * 0.25, half, 1e-9);
            Assert.AreEqual(4.0, full / half, 1e-9);
        }

        [Test]
        public void ComputeRange_IsDeterministic()
        {
            double a = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 0.73, 37.0);
            double b = ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 0.73, 37.0);
            Assert.AreEqual(a, b);
        }

        [TestCase(0.0)]
        [TestCase(-100.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ComputeRange_InvalidMaximumRange_Throws(double maxRange)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ShotRangeCalculator.ComputeRangeKm(maxRange, 1.0, 45.0));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ComputeRange_NonFinitePower_Throws(double power)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, power, 45.0));
        }

        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        public void ComputeRange_NonFiniteAngle_Throws(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ShotRangeCalculator.ComputeRangeKm(MaxRangeKm, 1.0, angle));
        }
    }
}
