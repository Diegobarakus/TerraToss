using System.Collections;
using NUnit.Framework;
using TerraToss.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace TerraToss.Presentation.Tests.PlayMode
{
    public class ShotFlightAnimatorPlayModeTests
    {
        private static Vector3[] MakePoints()
        {
            return new[]
            {
                new Vector3(0f, 0f, 5f),
                new Vector3(1f, 2f, 5f),
                new Vector3(2f, 3f, 4f),
                new Vector3(3f, 0f, 3f),
            };
        }

        [UnityTest]
        public IEnumerator Flight_MovesProjectileFromOriginToImpact()
        {
            var host = new GameObject("Animator");
            var projectile = new GameObject("Projectile").transform;
            Vector3[] points = MakePoints();

            var animator = host.AddComponent<ShotFlightAnimator>();
            animator.Configure(projectile, points, 0.2f, playOnStart: false);
            animator.Play();

            Assert.IsTrue(animator.IsPlaying);
            Assert.That(Vector3.Distance(projectile.localPosition, points[0]), Is.LessThan(1e-3f),
                "Flight should start at the origin point.");

            yield return new WaitForSeconds(0.4f);

            Assert.IsFalse(animator.IsPlaying, "Flight should have finished after the duration elapsed.");
            Assert.That(Vector3.Distance(projectile.localPosition, points[points.Length - 1]), Is.LessThan(1e-3f),
                "Flight should end at the impact point.");

            Object.Destroy(host);
            Object.Destroy(projectile.gameObject);
        }

        [UnityTest]
        public IEnumerator Flight_ProgressIsMonotonicAndReachesOne()
        {
            var host = new GameObject("Animator");
            var projectile = new GameObject("Projectile").transform;

            var animator = host.AddComponent<ShotFlightAnimator>();
            animator.Configure(projectile, MakePoints(), 0.2f, playOnStart: false);
            animator.Play();

            float previous = -1f;
            for (int i = 0; i < 4; i++)
            {
                Assert.That(animator.Progress, Is.GreaterThanOrEqualTo(previous));
                previous = animator.Progress;
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);
            Assert.That(animator.Progress, Is.EqualTo(1f).Within(1e-4f));

            Object.Destroy(host);
            Object.Destroy(projectile.gameObject);
        }

        [UnityTest]
        public IEnumerator Play_WithoutValidData_DoesNotPlay()
        {
            var host = new GameObject("Animator");
            var animator = host.AddComponent<ShotFlightAnimator>();
            animator.Configure(null, null, 0.2f, playOnStart: false);
            animator.Play();

            Assert.IsFalse(animator.IsPlaying);
            yield return null;

            Object.Destroy(host);
        }
    }
}
