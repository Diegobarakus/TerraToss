using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Presentation-only view that draws a shot trajectory through a <see cref="LineRenderer"/>.
    /// It never computes geographic results, distances, or impact grades and never performs a
    /// scene-wide search: it uses the LineRenderer on its own GameObject.
    ///
    /// Positions are interpreted in the LineRenderer's local space (<c>useWorldSpace = false</c>).
    /// The Trajectory object sits at the prototype root origin, which is the sphere centre, so the
    /// local-space points produced by <see cref="TrajectoryArcProjection"/> map directly.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ShotTrajectoryView : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        /// <summary>Assigns the LineRenderer reference. Intended for the Editor builder.</summary>
        public void SetLineRenderer(LineRenderer target)
        {
            lineRenderer = target;
        }

        /// <summary>Replaces the drawn trajectory with the given local-space points.</summary>
        public void SetTrajectory(Vector3[] localPoints)
        {
            LineRenderer line = EnsureLineRenderer();
            Clear();

            if (localPoints == null || localPoints.Length == 0)
            {
                return;
            }

            line.useWorldSpace = false;
            line.positionCount = localPoints.Length;
            line.SetPositions(localPoints);
        }

        /// <summary>Removes all previously drawn points.</summary>
        public void Clear()
        {
            EnsureLineRenderer().positionCount = 0;
        }

        /// <summary>Shows the trajectory.</summary>
        public void Show()
        {
            EnsureLineRenderer().enabled = true;
        }

        /// <summary>Hides the trajectory without discarding its points.</summary>
        public void Hide()
        {
            EnsureLineRenderer().enabled = false;
        }

        private LineRenderer EnsureLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            return lineRenderer;
        }
    }
}
