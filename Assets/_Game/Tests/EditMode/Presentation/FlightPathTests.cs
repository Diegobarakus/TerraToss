using System;
using NUnit.Framework;
using TerraToss.Presentation;
using UnityEngine;

namespace TerraToss.Presentation.Tests.EditMode
{
    [TestFixture]
    public class FlightPathTests
    {
        private static Vector3[] MakePoints()
        {
            return new[]
            {
                new Vector3(0f, 0f, 5f),
                new Vector3(2f, 2f, 5f),
                new Vector3(4f, 0f, 3f),
            };
        }

        [Test]
        public void Evaluate_ProgressZero_IsFirstPoint()
        {
            Vector3[] points = MakePoints();
            Assert.AreEqual(points[0], FlightPath.Evaluate(points, 0f));
        }

        [Test]
        public void Evaluate_ProgressOne_IsLastPoint()
        {
            Vector3[] points = MakePoints();
            Assert.AreEqual(points[points.Length - 1], FlightPath.Evaluate(points, 1f));
        }

        [Test]
        public void Evaluate_ProgressHalf_IsMiddlePoint()
        {
            // Three points: progress 0.5 lands exactly on the middle sample.
            Vector3[] points = MakePoints();
            Assert.AreEqual(points[1], FlightPath.Evaluate(points, 0.5f));
        }

        [Test]
        public void Evaluate_ProgressQuarter_InterpolatesFirstSegment()
        {
            Vector3[] points = MakePoints();
            Vector3 expected = Vector3.Lerp(points[0], points[1], 0.5f); // 0.25 * (n-1=2) = 0.5 within first segment
            Assert.AreEqual(expected, FlightPath.Evaluate(points, 0.25f));
        }

        [TestCase(-0.5f)]
        [TestCase(-100f)]
        public void Evaluate_ProgressBelowZero_ClampsToFirstPoint(float progress)
        {
            Vector3[] points = MakePoints();
            Assert.AreEqual(points[0], FlightPath.Evaluate(points, progress));
        }

        [TestCase(1.5f)]
        [TestCase(100f)]
        public void Evaluate_ProgressAboveOne_ClampsToLastPoint(float progress)
        {
            Vector3[] points = MakePoints();
            Assert.AreEqual(points[points.Length - 1], FlightPath.Evaluate(points, progress));
        }

        [Test]
        public void Evaluate_IsDeterministic()
        {
            Vector3[] points = MakePoints();
            Assert.AreEqual(FlightPath.Evaluate(points, 0.37f), FlightPath.Evaluate(points, 0.37f));
        }

        [Test]
        public void Evaluate_NullPoints_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => FlightPath.Evaluate(null, 0.5f));
        }

        [Test]
        public void Evaluate_FewerThanTwoPoints_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => FlightPath.Evaluate(new[] { Vector3.zero }, 0.5f));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void Evaluate_NonFiniteProgress_Throws(float progress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FlightPath.Evaluate(MakePoints(), progress));
        }
    }
}
