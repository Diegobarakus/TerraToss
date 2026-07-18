using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Moves a projectile transform along a precomputed trajectory over a fixed duration.
    /// Deterministic and time-driven (no physics). It owns no geographic rules, never recomputes the
    /// impact or grade, and performs no scene-wide search: all references are explicit.
    /// </summary>
    public class ShotFlightAnimator : MonoBehaviour
    {
        [SerializeField] private Transform projectile;
        [SerializeField] private Vector3[] points;
        [SerializeField, Range(6f, 10f)] private float flightDurationSeconds = 8f;
        [SerializeField] private bool playOnStart = true;

        private float elapsedSeconds;
        private bool isPlaying;

        /// <summary>True while the flight is in progress.</summary>
        public bool IsPlaying => isPlaying;

        /// <summary>Normalized flight progress in [0, 1].</summary>
        public float Progress => flightDurationSeconds > 0f
            ? Mathf.Clamp01(elapsedSeconds / flightDurationSeconds)
            : 1f;

        /// <summary>Assigns the animation data. Intended for the Editor builder and for tests.</summary>
        public void Configure(Transform projectile, Vector3[] points, float flightDurationSeconds, bool playOnStart)
        {
            this.projectile = projectile;
            this.points = points;
            this.flightDurationSeconds = flightDurationSeconds;
            this.playOnStart = playOnStart;
        }

        /// <summary>Starts (or restarts) the flight from the origin.</summary>
        public void Play()
        {
            if (points == null || points.Length < 2 || projectile == null)
            {
                isPlaying = false;
                return;
            }

            elapsedSeconds = 0f;
            isPlaying = true;
            Apply(0f);
        }

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            elapsedSeconds += Time.deltaTime;
            float progress = Progress;
            Apply(progress);

            if (progress >= 1f)
            {
                isPlaying = false;
            }
        }

        private void Apply(float progress)
        {
            projectile.localPosition = FlightPath.Evaluate(points, progress);
        }
    }
}
