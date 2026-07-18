using TerraToss.Gameplay;
using TerraToss.Geo;
using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Orchestrates a single shot's visualization: computes the authoritative geographic result,
    /// samples the trajectory, projects the visual arc, updates the trajectory view, positions the
    /// projectile, and starts the flight animation. It only reuses the existing pure calculators
    /// (<see cref="GeoShotCalculator"/>, <see cref="ShotTrajectorySampler"/>,
    /// <see cref="TrajectoryArcProjection"/>) and owns no new geographic rules. All references are
    /// explicit; it performs no scene-wide search.
    /// </summary>
    public class ShotVisualizationDirector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShotTrajectoryView trajectoryView;
        [SerializeField] private ShotFlightAnimator flightAnimator;
        [SerializeField] private Transform projectile;

        [Header("Geography")]
        [SerializeField] private double originLatitude = 49.9929;
        [SerializeField] private double originLongitude = 8.2473;
        [SerializeField] private double targetLatitude = 60.1699;
        [SerializeField] private double targetLongitude = 24.9384;
        [SerializeField] private double maximumRangeKm = 2000.0;

        [Header("Visual")]
        [SerializeField] private double earthRadius = 5.0;
        [SerializeField] private double arcHeight = 1.75;
        [SerializeField] private int sampleCount = 48;
        [SerializeField] private float flightDurationSeconds = 8f;

        [Header("Match (Camp mode)")]
        [SerializeField] private ImpactGrade campHitThreshold = ImpactGrade.StrongHit;
        [SerializeField] private int campMaxShots = 0;

        private bool hasResult;
        private ShotResult lastResult;
        private CampMatch match;

        /// <summary>True once a shot has been fired at least once.</summary>
        public bool HasResult => hasResult;

        /// <summary>The most recently computed shot result.</summary>
        public ShotResult LastResult => lastResult;

        /// <summary>Current Camp-mode match status.</summary>
        public MatchStatus MatchStatus => match != null ? match.Status : MatchStatus.InProgress;

        /// <summary>Number of shots counted towards the current match.</summary>
        public int ShotsTaken => match != null ? match.ShotsTaken : 0;

        /// <summary>Assigns references and parameters. Intended for the Editor builder and tests.</summary>
        public void Configure(
            ShotTrajectoryView trajectoryView, ShotFlightAnimator flightAnimator, Transform projectile,
            GeoCoordinate origin, GeoCoordinate target, double maximumRangeKm,
            double earthRadius, double arcHeight, int sampleCount, float flightDurationSeconds,
            ImpactGrade campHitThreshold, int campMaxShots)
        {
            this.trajectoryView = trajectoryView;
            this.flightAnimator = flightAnimator;
            this.projectile = projectile;
            this.originLatitude = origin.LatitudeDegrees;
            this.originLongitude = origin.LongitudeDegrees;
            this.targetLatitude = target.LatitudeDegrees;
            this.targetLongitude = target.LongitudeDegrees;
            this.maximumRangeKm = maximumRangeKm;
            this.earthRadius = earthRadius;
            this.arcHeight = arcHeight;
            this.sampleCount = sampleCount;
            this.flightDurationSeconds = flightDurationSeconds;
            this.campHitThreshold = campHitThreshold;
            this.campMaxShots = campMaxShots;
            this.match = null; // start a fresh match with the new configuration
        }

        /// <summary>
        /// Recomputes the shot for the given aim and updates the trajectory, projectile, and flight.
        /// The flight is started unless <paramref name="play"/> is false. Returns the computed result.
        /// </summary>
        public ShotResult Fire(double headingDegrees, double launchAngleDegrees, double power,
            bool play = true, bool countShot = true)
        {
            var origin = new GeoCoordinate(originLatitude, originLongitude);
            var target = new GeoCoordinate(targetLatitude, targetLongitude);
            var input = new ShotInput(origin, headingDegrees, launchAngleDegrees, power, maximumRangeKm, target);

            ShotResult result = GeoShotCalculator.Calculate(input);
            GeoCoordinate[] samples = ShotTrajectorySampler.Sample(input, result, sampleCount);
            Vector3[] points = TrajectoryArcProjection.ToVisualPoints(samples, earthRadius, arcHeight);

            if (trajectoryView != null)
            {
                trajectoryView.SetTrajectory(points);
                trajectoryView.Show();
            }

            if (projectile != null)
            {
                projectile.localPosition = points[0];
            }

            if (flightAnimator != null)
            {
                flightAnimator.Configure(projectile, points, flightDurationSeconds, playOnStart: false);
                if (play)
                {
                    flightAnimator.Play();
                }
            }

            lastResult = result;
            hasResult = true;

            if (countShot)
            {
                match ??= new CampMatch(campHitThreshold, campMaxShots);
                match.RegisterShot(result);
            }

            return result;
        }
    }
}
