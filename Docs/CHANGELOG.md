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
- Desktop aiming and interactive fire:
  - `ShotVisualizationDirector` (`MonoBehaviour`): orchestrates a shot by reusing
    the existing pure calculators; `Fire(heading, angle, power)` and `LastResult`.
  - `ShotAimController` (`MonoBehaviour`): keyboard aiming via the Input System
    (polling, no Input Action assets) — A/D heading, W/S angle, Q/E power, Space
    to fire.
  - `ShotAimReadout` (`MonoBehaviour`): minimal IMGUI readout of aim and result.
  - `PrototypeSceneBuilder` refactored to wire the director/controller/readout on
    `ShotVisualization` idempotently and delegate the demo shot to the director;
    `TerraToss.Presentation` now references `Unity.InputSystem`.
- Tests: Edit Mode `TerraToss.Geo.EditMode.Tests` (119) and
  `TerraToss.Presentation.EditMode.Tests` (41) — 160 total; Play Mode
  `TerraToss.Presentation.PlayMode.Tests` (6, flight + director/controller fire).
  All passing.

### Not implemented

- Match rules and scoring / gameplay orchestration.
- Formal user interface (beyond the debug IMGUI readout).
- Sensors.
- Backend.