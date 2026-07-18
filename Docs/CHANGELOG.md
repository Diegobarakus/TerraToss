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
- Edit Mode test suite for the geographic foundation
  (`TerraToss.Geo.EditMode.Tests`): 64 tests covering valid/invalid coordinates,
  heading and longitude normalization, cardinal-direction destinations,
  antimeridian crossing, near-pole behavior, and all impact-grade boundaries.
  All 64 tests pass.

### Not implemented

- Shot calculation layer (range model, `ShotInput`, `ShotResult`, `GeoShotCalculator`).
- Gameplay orchestration.
- Visual launch prototype.
- Sensors.
- Backend.