using System.Text;
using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Minimal on-screen readout (IMGUI) of the current aim and the last shot result.
    /// Presentation only; it reads state from the controller and director and computes nothing.
    /// </summary>
    public class ShotAimReadout : MonoBehaviour
    {
        [SerializeField] private ShotAimController aimController;
        [SerializeField] private ShotVisualizationDirector director;

        /// <summary>Assigns the sources. Intended for the Editor builder and tests.</summary>
        public void Configure(ShotAimController aimController, ShotVisualizationDirector director)
        {
            this.aimController = aimController;
            this.director = director;
        }

        private void OnGUI()
        {
            if (aimController == null)
            {
                return;
            }

            var style = new GUIStyle(GUI.skin.label) { fontSize = 14 };

            var text = new StringBuilder();
            text.AppendLine("TerraToss prototype (desktop)");
            text.AppendLine($"Heading: {aimController.HeadingDegrees:0.0}   (A / D)");
            text.AppendLine($"Angle:   {aimController.LaunchAngleDegrees:0.0}   (W / S)");
            text.AppendLine($"Power:   {aimController.Power:0.00}   (Q / E)");
            text.AppendLine("Fire: Space");

            if (director != null && director.HasResult)
            {
                var result = director.LastResult;
                text.AppendLine($"Impact grade: {result.Grade}");
                text.AppendLine($"Distance to target: {result.DistanceToTargetKm:0.0} km");
            }

            GUI.Label(new Rect(12f, 12f, 440f, 170f), text.ToString(), style);
        }
    }
}
