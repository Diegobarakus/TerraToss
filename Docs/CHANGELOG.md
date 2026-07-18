# TerraToss — Changelog

## Unreleased

### Added

- Initial Unity 6.3 LTS URP project.
- Android build configuration.
- Successful Android test APK.
- Git repository.
- UnityMCP connection for Claude Code.
- Claude Code project instructions, rules, skills, agents, and documentation.
- Pure geographic foundation (`TerraToss.Geo` assembly, `noEngineReferences`):
  - `GeoCoordinate` immutable value type with latitude validation and longitude
    normalization.
  - `GeoMath` with heading/longitude normalization, haversine great-circle
    distance, and spherical destination-point calculation.
  - `ImpactGrade` and `ImpactEvaluator` distance-to-grade classification.
- Pure shot-calculation layer (`TerraToss.Geo` assembly):
  - `ShotInput` and `ShotResult` immutable value types.
  - `ShotRangeCalculator` fictional range model
    `rangeKm = maximumRangeKm × power² × sin(2 × launchAngle)` (maximum range is a
    parameter, not hardcoded).
  - `GeoShotCalculator` composing input validation, heading normalization,
    `ShotRangeCalculator`, `GeoMath`, and `ImpactEvaluator` into a `ShotResult`.
- Static spatial/visual base (`TerraToss.Presentation` and
  `TerraToss.Presentation.Editor` assemblies):
  - `GeoSphereProjection`: coordinate → local `Vector3` on a sphere, with a
    documented axis convention (Y polar, (0,0) = +Z, East = +X) and magnitude
    equal to the radius.
  - `PrototypeSceneReferences`: explicit scene references, no runtime search;
    holds the Earth radius constant.
  - `PrototypeSceneBuilder`: idempotent Editor builder (`TerraToss/Build
    Prototype Scene`) that constructs the prototype hierarchy into
    `Bootstrap.unity` from primitives with basic URP materials, reusing the
    existing camera and directional light, and saves the scene.
- `Bootstrap.unity` prototype hierarchy: `TerraToss_Prototype` (Earth sphere;
  Markers → Origin_Mainz, Target_Helsinki; Environment → Directional Light).
- Static shot-trajectory visualization:
  - `ShotTrajectorySampler` (pure C#, `TerraToss.Geo`): deterministic
    great-circle samples from origin to impact, reusing `GeoMath.DestinationPoint`
    (no linear lat/lon interpolation).
  - `TrajectoryArcProjection` (`TerraToss.Presentation`): projects samples via
    `GeoSphereProjection` and adds a cosmetic `sin(π·progress)·maxArcHeight`
    offset (zero at ends, peak near the middle; points on or outside the radius).
  - `ShotTrajectoryView` (`MonoBehaviour`): draws the trajectory with a
    `LineRenderer` in local space; show/hide/clear; no gameplay rules.
  - `PrototypeSceneBuilder` extended to build a `ShotVisualization` group
    (Projectile sphere on the origin + Trajectory LineRenderer) idempotently from
    a centralized deterministic demo shot (Mainz → Helsinki), and to keep a stable
    sibling order. `Bootstrap.unity` updated accordingly.
- Projectile flight animation:
  - `FlightPath` (pure, `TerraToss.Presentation`): deterministic position along a
    trajectory polyline for a normalized, clamped progress.
  - `ShotFlightAnimator` (`MonoBehaviour`): time-driven projectile movement along
    the trajectory over a configurable 6–10 s duration (default 8 s), no physics,
    `playOnStart` entry point; owns no geographic rules.
  - `PrototypeSceneBuilder` now adds and configures a single `ShotFlightAnimator`
    on `ShotVisualization` idempotently; entering Play Mode flies the projectile.
- Tests: Edit Mode `TerraToss.Geo.EditMode.Tests` (119) and
  `TerraToss.Presentation.EditMode.Tests` (41) — 160 total; new Play Mode
  assembly `TerraToss.Presentation.PlayMode.Tests` (3, flight flow). All passing.

### Not implemented

- Desktop aiming controls (heading/angle/power) and firing.
- User interface.
- Gameplay orchestration and match rules.
- Sensors.
- Backend.