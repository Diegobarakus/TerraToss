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
- Edit Mode test suite: `TerraToss.Geo.EditMode.Tests` (106) covering the
  geographic and shot-calculation layers, and
  `TerraToss.Presentation.EditMode.Tests` (14) covering the sphere projection and
  the builder's idempotency. 120 tests total, all passing.

### Not implemented

- Shot visualization (projectile, trajectory, flight animation).
- Desktop input controls.
- Gameplay orchestration and match rules.
- Sensors.
- Backend.