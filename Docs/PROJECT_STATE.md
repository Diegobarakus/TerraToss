# TerraToss — Project State

Last initialized: 2026-07-18

## Environment

- Unity 6.3 LTS.
- Universal Render Pipeline.
- Android build profile active.
- Android SDK, NDK, OpenJDK, Gradle, and CMake configured.
- Test APK generated successfully.
- iOS Build Support installed; final iOS build still requires macOS and Xcode.
- Git repository initialized.
- UnityMCP 10.1.0 connected to Claude Code.

## Current implementation

- Unity project and Bootstrap scene exist.
- Template URP assets and Input System actions exist.
- Placeholder-only development is approved.
- Pure geographic foundation implemented in the `TerraToss.Geo` assembly
  (`Assets/_Game/Geo/`), with no `UnityEngine` dependency (`noEngineReferences`):
  - `GeoCoordinate`: immutable value type, latitude validation `[-90, 90]`,
    longitude normalized to `[-180, 180)`, plus `TryCreate` / `IsValidLatitude`.
  - `GeoMath`: heading normalization `[0, 360)`, longitude normalization,
    haversine great-circle distance, and spherical destination-point calculation.
  - `ImpactGrade` + `ImpactEvaluator`: distance-to-grade classification that
    rejects negative / non-finite distances.
- Edit Mode tests in `TerraToss.Geo.EditMode.Tests`
  (`Assets/_Game/Tests/EditMode/Geo/`): 64 tests, all passing.
- No gameplay orchestration, presentation, UI, or scene wiring exists yet.

## Current phase

Phase 1: local functional prototype.

## Next recommended task

The pure geographic foundation is complete. The next step is the shot
calculation layer (still pure C#, no scene changes):

1. `ShotInput` (origin, heading, launch angle, normalized power).
2. Fictional range model: `rangeKm = maximumRangeKm × power² × sin(2 × launchAngle)`
   with explicit input validation.
3. `ShotResult` (impact coordinate, distance to target, impact grade,
   trajectory reconstruction data).
4. `GeoShotCalculator` that composes `GeoMath` + `ImpactEvaluator`.
5. Edit Mode tests, including the Mainz origin / Helsinki target scenario and
   the zero / maximum power and angle boundaries.

Do not modify scenes during this task.

## Known local issue

UnityMCP may need to be started manually through the Python environment documented in `Docs/LOCAL_SETUP.md`.