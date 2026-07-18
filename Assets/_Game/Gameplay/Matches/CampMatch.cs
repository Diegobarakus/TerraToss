using System;
using TerraToss.Geo;

namespace TerraToss.Gameplay
{
    /// <summary>
    /// Camp mode rules: a single valid hit destroys the camp. Pure C#, deterministic, no
    /// UnityEngine dependency. It only inspects the impact grade of each shot; it does not compute
    /// any geographic result itself.
    /// </summary>
    public sealed class CampMatch
    {
        /// <summary>The worst grade that still counts as a valid, camp-destroying hit.</summary>
        public ImpactGrade HitThreshold { get; }

        /// <summary>Maximum number of shots; 0 means unlimited (the match cannot be lost by running out).</summary>
        public int MaxShots { get; }

        /// <summary>Number of shots registered so far.</summary>
        public int ShotsTaken { get; private set; }

        /// <summary>Current resolution state.</summary>
        public MatchStatus Status { get; private set; }

        /// <param name="hitThreshold">Worst grade counting as a valid hit (default StrongHit).</param>
        /// <param name="maxShots">Maximum shots, or 0 for unlimited.</param>
        /// <exception cref="ArgumentOutOfRangeException">Maximum shots is negative.</exception>
        public CampMatch(ImpactGrade hitThreshold = ImpactGrade.StrongHit, int maxShots = 0)
        {
            if (maxShots < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxShots), maxShots, "Maximum shots cannot be negative.");
            }

            HitThreshold = hitThreshold;
            MaxShots = maxShots;
            Status = MatchStatus.InProgress;
        }

        /// <summary>Registers a shot from its full result. Returns the resulting status.</summary>
        public MatchStatus RegisterShot(ShotResult result)
        {
            return RegisterGrade(result.Grade);
        }

        /// <summary>Registers a shot from its impact grade. Returns the resulting status.</summary>
        public MatchStatus RegisterGrade(ImpactGrade grade)
        {
            if (Status != MatchStatus.InProgress)
            {
                // Once resolved, further shots do not change the outcome.
                return Status;
            }

            ShotsTaken++;

            if (IsValidHit(grade, HitThreshold))
            {
                Status = MatchStatus.Won;
            }
            else if (MaxShots > 0 && ShotsTaken >= MaxShots)
            {
                Status = MatchStatus.Lost;
            }

            return Status;
        }

        /// <summary>True when <paramref name="grade"/> is at least as good as <paramref name="threshold"/>.</summary>
        public static bool IsValidHit(ImpactGrade grade, ImpactGrade threshold)
        {
            // ImpactGrade is ordered best-to-worst (DirectHit = 0 ... Miss = 4).
            return (int)grade <= (int)threshold;
        }
    }
}
