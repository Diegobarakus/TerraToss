using System;
using TerraToss.Geo;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Desktop keyboard aiming. Adjusts heading, launch angle, and power, and fires the shot through
    /// a <see cref="ShotVisualizationDirector"/>. Presentation/input only: it owns no geographic rules
    /// and performs no scene-wide search. Uses the Input System by polling the current keyboard, with
    /// no Input Action assets.
    ///
    /// Controls: A/D heading, W/S launch angle, Q/E power, Space to fire.
    /// </summary>
    public class ShotAimController : MonoBehaviour
    {
        [SerializeField] private ShotVisualizationDirector director;

        [Header("Initial aim")]
        [SerializeField] private double headingDegrees = 20.0;
        [SerializeField] private double launchAngleDegrees = 45.0;
        [SerializeField] private double power = 1.0;

        [Header("Adjustment rates (per second)")]
        [SerializeField] private double headingStepPerSecond = 45.0;
        [SerializeField] private double angleStepPerSecond = 30.0;
        [SerializeField] private double powerStepPerSecond = 0.5;

        public double HeadingDegrees => headingDegrees;
        public double LaunchAngleDegrees => launchAngleDegrees;
        public double Power => power;

        /// <summary>Assigns the director and the initial aim. Intended for the Editor builder and tests.</summary>
        public void Configure(ShotVisualizationDirector director, double headingDegrees,
            double launchAngleDegrees, double power)
        {
            this.director = director;
            this.headingDegrees = GeoMath.NormalizeHeading(headingDegrees);
            this.launchAngleDegrees = ClampAngle(launchAngleDegrees);
            this.power = ClampPower(power);
        }

        /// <summary>Fires the current aim through the director. Public for scripted triggering and tests.</summary>
        public void Fire()
        {
            if (director != null)
            {
                director.Fire(headingDegrees, launchAngleDegrees, power);
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            double delta = Time.deltaTime;

            if (keyboard.aKey.isPressed) headingDegrees -= headingStepPerSecond * delta;
            if (keyboard.dKey.isPressed) headingDegrees += headingStepPerSecond * delta;
            headingDegrees = GeoMath.NormalizeHeading(headingDegrees);

            if (keyboard.wKey.isPressed) launchAngleDegrees += angleStepPerSecond * delta;
            if (keyboard.sKey.isPressed) launchAngleDegrees -= angleStepPerSecond * delta;
            launchAngleDegrees = ClampAngle(launchAngleDegrees);

            if (keyboard.eKey.isPressed) power += powerStepPerSecond * delta;
            if (keyboard.qKey.isPressed) power -= powerStepPerSecond * delta;
            power = ClampPower(power);

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Fire();
            }
        }

        private static double ClampAngle(double value)
        {
            return Math.Max(0.0, Math.Min(90.0, value));
        }

        private static double ClampPower(double value)
        {
            return Math.Max(0.0, Math.Min(1.0, value));
        }
    }
}
