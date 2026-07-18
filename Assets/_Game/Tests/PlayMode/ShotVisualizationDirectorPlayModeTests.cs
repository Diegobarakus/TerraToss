using System.Collections;
using NUnit.Framework;
using TerraToss.Geo;
using TerraToss.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace TerraToss.Presentation.Tests.PlayMode
{
    public class ShotVisualizationDirectorPlayModeTests
    {
        private const int SampleCount = 24;

        private static readonly GeoCoordinate Mainz = new GeoCoordinate(49.9929, 8.2473);
        private static readonly GeoCoordinate Helsinki = new GeoCoordinate(60.1699, 24.9384);

        private static ShotVisualizationDirector BuildRig(out LineRenderer line, out Transform projectile,
            out ShotFlightAnimator animator, out GameObject host)
        {
            host = new GameObject("viz");
            projectile = new GameObject("proj").transform;
            projectile.SetParent(host.transform, false);

            var trajectory = new GameObject("traj");
            trajectory.transform.SetParent(host.transform, false);
            line = trajectory.AddComponent<LineRenderer>();
            var view = trajectory.AddComponent<ShotTrajectoryView>();
            view.SetLineRenderer(line);

            animator = host.AddComponent<ShotFlightAnimator>();
            var director = host.AddComponent<ShotVisualizationDirector>();
            director.Configure(view, animator, projectile, Mainz, Helsinki, 2000.0, 5.0, 1.75, SampleCount, 0.2f);
            return director;
        }

        [UnityTest]
        public IEnumerator Fire_UpdatesTrajectoryAndStartsFlight()
        {
            ShotVisualizationDirector director = BuildRig(out LineRenderer line, out Transform projectile,
                out ShotFlightAnimator animator, out GameObject host);

            ShotResult result = director.Fire(20.0, 45.0, 1.0);

            // Checked before advancing a frame: the flight has not moved the projectile yet.
            Assert.IsTrue(director.HasResult);
            Assert.AreEqual(ImpactEvaluator.Evaluate(result.DistanceToTargetKm), result.Grade);
            Assert.AreEqual(SampleCount, line.positionCount);
            Assert.IsTrue(animator.IsPlaying);
            Assert.That(Vector3.Distance(projectile.localPosition, line.GetPosition(0)), Is.LessThan(1e-3f),
                "The projectile starts at the first trajectory point.");

            yield return null;

            Object.Destroy(host);
        }

        [UnityTest]
        public IEnumerator Fire_DifferentAim_ProducesDifferentImpact()
        {
            ShotVisualizationDirector director = BuildRig(out LineRenderer _, out Transform _,
                out ShotFlightAnimator _, out GameObject host);

            ShotResult a = director.Fire(20.0, 45.0, 1.0, play: false);
            ShotResult b = director.Fire(200.0, 45.0, 1.0, play: false);

            bool differs =
                !Mathf.Approximately((float)a.ImpactCoordinate.LatitudeDegrees, (float)b.ImpactCoordinate.LatitudeDegrees) ||
                !Mathf.Approximately((float)a.ImpactCoordinate.LongitudeDegrees, (float)b.ImpactCoordinate.LongitudeDegrees);
            Assert.IsTrue(differs, "Different headings must produce different impact coordinates.");

            yield return null;
            Object.Destroy(host);
        }

        [UnityTest]
        public IEnumerator ControllerFire_DrivesDirector()
        {
            ShotVisualizationDirector director = BuildRig(out LineRenderer _, out Transform _,
                out ShotFlightAnimator _, out GameObject host);

            var controller = host.AddComponent<ShotAimController>();
            controller.Configure(director, 20.0, 45.0, 1.0);
            controller.Fire();

            Assert.IsTrue(director.HasResult);
            yield return null;
            Object.Destroy(host);
        }
    }
}
