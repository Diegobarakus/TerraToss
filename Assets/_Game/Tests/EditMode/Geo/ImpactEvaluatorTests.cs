using System;
using NUnit.Framework;
using TerraToss.Geo;

namespace TerraToss.Geo.Tests.EditMode
{
    [TestFixture]
    public class ImpactEvaluatorTests
    {
        [Test]
        public void Evaluate_ZeroDistance_IsDirectHit()
        {
            Assert.AreEqual(ImpactGrade.DirectHit, ImpactEvaluator.Evaluate(0.0));
        }

        // Boundary matrix required by the spec: 4.999 / 5 / 14.999 / 15 / 39.999 / 40 / 99.999 / 100 km.
        [TestCase(4.999, ImpactGrade.DirectHit)]
        [TestCase(5.0, ImpactGrade.StrongHit)]
        [TestCase(14.999, ImpactGrade.StrongHit)]
        [TestCase(15.0, ImpactGrade.LightHit)]
        [TestCase(39.999, ImpactGrade.LightHit)]
        [TestCase(40.0, ImpactGrade.NearMiss)]
        [TestCase(99.999, ImpactGrade.NearMiss)]
        [TestCase(100.0, ImpactGrade.Miss)]
        public void Evaluate_BoundaryValues_ReturnExpectedGrade(double distanceKm, ImpactGrade expected)
        {
            Assert.AreEqual(expected, ImpactEvaluator.Evaluate(distanceKm));
        }

        [Test]
        public void Evaluate_LargeDistance_IsMiss()
        {
            Assert.AreEqual(ImpactGrade.Miss, ImpactEvaluator.Evaluate(5000.0));
        }

        [TestCase(-0.001)]
        [TestCase(-100.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Evaluate_InvalidDistance_Throws(double distanceKm)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ImpactEvaluator.Evaluate(distanceKm));
        }
    }
}
