using System;
using NUnit.Framework;
using TerraToss.Gameplay;
using TerraToss.Geo;

namespace TerraToss.Gameplay.Tests.EditMode
{
    [TestFixture]
    public class CampMatchTests
    {
        [Test]
        public void NewMatch_IsInProgress_WithZeroShots()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            Assert.AreEqual(MatchStatus.InProgress, match.Status);
            Assert.AreEqual(0, match.ShotsTaken);
        }

        [Test]
        public void RegisterGrade_ValidHitWithinThreshold_Wins()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            Assert.AreEqual(MatchStatus.Won, match.RegisterGrade(ImpactGrade.DirectHit));
            Assert.AreEqual(MatchStatus.Won, match.Status);
        }

        [Test]
        public void RegisterGrade_GradeEqualToThreshold_Wins()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            Assert.AreEqual(MatchStatus.Won, match.RegisterGrade(ImpactGrade.StrongHit));
        }

        [Test]
        public void RegisterGrade_GradeWorseThanThreshold_StaysInProgress()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            Assert.AreEqual(MatchStatus.InProgress, match.RegisterGrade(ImpactGrade.LightHit));
            Assert.AreEqual(1, match.ShotsTaken);
        }

        [Test]
        public void RegisterShot_UsesResultGrade()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            var origin = new GeoCoordinate(0.0, 0.0);
            var result = new ShotResult(origin, 10.0, 3.0, 90.0, 45.0, 1.0, ImpactGrade.DirectHit);

            Assert.AreEqual(MatchStatus.Won, match.RegisterShot(result));
        }

        [Test]
        public void RegisterGrade_AfterWon_IsNoOp()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            match.RegisterGrade(ImpactGrade.DirectHit);

            int shotsAtWin = match.ShotsTaken;
            Assert.AreEqual(MatchStatus.Won, match.RegisterGrade(ImpactGrade.Miss));
            Assert.AreEqual(shotsAtWin, match.ShotsTaken, "Shots must not increase after the match resolves.");
        }

        [Test]
        public void ShotsTaken_IncrementsPerMiss()
        {
            var match = new CampMatch(ImpactGrade.StrongHit);
            match.RegisterGrade(ImpactGrade.Miss);
            match.RegisterGrade(ImpactGrade.NearMiss);
            Assert.AreEqual(2, match.ShotsTaken);
            Assert.AreEqual(MatchStatus.InProgress, match.Status);
        }

        [Test]
        public void MaxShots_ExhaustedWithoutHit_IsLost()
        {
            var match = new CampMatch(ImpactGrade.DirectHit, maxShots: 3);
            match.RegisterGrade(ImpactGrade.Miss);
            match.RegisterGrade(ImpactGrade.NearMiss);
            Assert.AreEqual(MatchStatus.InProgress, match.Status);
            Assert.AreEqual(MatchStatus.Lost, match.RegisterGrade(ImpactGrade.LightHit));
            Assert.AreEqual(3, match.ShotsTaken);
        }

        [Test]
        public void MaxShots_HitOnLastShot_Wins()
        {
            var match = new CampMatch(ImpactGrade.StrongHit, maxShots: 2);
            match.RegisterGrade(ImpactGrade.Miss);
            Assert.AreEqual(MatchStatus.Won, match.RegisterGrade(ImpactGrade.StrongHit));
        }

        [Test]
        public void Unlimited_NeverLoses()
        {
            var match = new CampMatch(ImpactGrade.DirectHit, maxShots: 0);
            for (int i = 0; i < 50; i++)
            {
                match.RegisterGrade(ImpactGrade.Miss);
            }
            Assert.AreEqual(MatchStatus.InProgress, match.Status);
        }

        [Test]
        public void NegativeMaxShots_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CampMatch(ImpactGrade.StrongHit, -1));
        }

        [TestCase(ImpactGrade.DirectHit, ImpactGrade.StrongHit, true)]
        [TestCase(ImpactGrade.StrongHit, ImpactGrade.StrongHit, true)]
        [TestCase(ImpactGrade.LightHit, ImpactGrade.StrongHit, false)]
        [TestCase(ImpactGrade.Miss, ImpactGrade.NearMiss, false)]
        [TestCase(ImpactGrade.NearMiss, ImpactGrade.NearMiss, true)]
        public void IsValidHit_RespectsOrdering(ImpactGrade grade, ImpactGrade threshold, bool expected)
        {
            Assert.AreEqual(expected, CampMatch.IsValidHit(grade, threshold));
        }

        [Test]
        public void RegisterGrade_IsDeterministicForSameSequence()
        {
            MatchStatus Run()
            {
                var m = new CampMatch(ImpactGrade.StrongHit, maxShots: 4);
                m.RegisterGrade(ImpactGrade.Miss);
                m.RegisterGrade(ImpactGrade.LightHit);
                return m.RegisterGrade(ImpactGrade.StrongHit);
            }

            Assert.AreEqual(Run(), Run());
        }
    }
}
