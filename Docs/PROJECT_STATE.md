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
- Pure shot-calculation layer, also in the `TerraToss.Geo` assembly:
  - `ShotInput`: immutable shot definition (origin, heading, launch angle,
    normalized power, maximum range, target).
  - `ShotResult`: immutable result (impact coordinate, distance travelled,
    distance to target, normalized heading, angle, power, impact grade).
  - `ShotRangeCalculator`: fictional range model
    `rangeKm = maximumRangeKm × power² × sin(2 × launchAngle)` (maximum range is a
    parameter, never hardcoded).
  - `GeoShotCalculator`: validates the input (power `[0, 1]`, angle `[0, 90]`,
    maximum range `> 0`, finite heading), normalizes heading, and composes
    `ShotRangeCalculator` + `GeoMath` + `ImpactEvaluator` into a `ShotResult`.
- Static spatial/visual base in the `TerraToss.Presentation` assembly
  (`Assets/_Game/Presentation/`, depends on `UnityEngine` and `TerraToss.Geo`):
  - `GeoSphereProjection`: `GeoCoordinate` → local `Vector3` on a sphere of a
    given radius. Axis convention: Y = polar axis (N = +Y, S = −Y), (lat 0, lon 0)
    = +Z, East = +X, West = −X; magnitude equals the radius. Rejects radius ≤ 0
    and non-finite radius. Does not duplicate `GeoMath`.
  - `PrototypeSceneReferences`: `MonoBehaviour` holding explicit references
    (Earth, origin marker, target marker, camera); no runtime scene search.
    Holds the Earth radius constant (`EarthRadiusUnits = 5`).
  - `PrototypeSceneBuilder` (Editor, `TerraToss.Presentation.Editor`): idempotent
    `[MenuItem("TerraToss/Build Prototype Scene")]` that builds/updates the
    prototype hierarchy into `Bootstrap.unity` (primitives, basic URP materials,
    reuses the existing camera and directional light) and saves the scene.
- Static shot-trajectory visualization:
  - `ShotTrajectorySampler` (pure C#, `TerraToss.Geo`): converts a `ShotInput` +
    `ShotResult` into a deterministic sequence of `GeoCoordinate` samples along
    the great-circle path (first = origin, last = impact), reusing
    `GeoMath.DestinationPoint` (no linear lat/lon interpolation).
  - `TrajectoryArcProjection` (`TerraToss.Presentation`): projects samples via
    `GeoSphereProjection` and adds a cosmetic `sin(π·progress)·maxArcHeight`
    offset (zero at both ends, peak near the middle; every point on or outside
    the radius). Purely visual — never affects geographic results or grades.
  - `ShotTrajectoryView` (`MonoBehaviour`): drives a `LineRenderer` in local
    space; no gameplay rules, no runtime scene search; supports show/hide/clear.
- Projectile flight animation:
  - `FlightPath` (`TerraToss.Presentation`, pure): deterministic position along a
    polyline for a normalized progress in [0, 1] (clamped), fractional-index
    interpolation; no time or state.
  - `ShotFlightAnimator` (`MonoBehaviour`): moves the projectile along the
    trajectory points over a configurable duration (`[Range(6,10)]` s, default 8),
    time-driven with no physics; `Play`, `IsPlaying`, `Progress`; `playOnStart`
    triggers it on Play Mode entry. Owns no geographic rules and does not recompute
    impact or grade.
- Bootstrap.unity prototype hierarchy: `TerraToss_Prototype` (Earth sphere;
  Markers → Origin_Mainz, Target_Helsinki; ShotVisualization → Projectile,
  Trajectory, with a `ShotFlightAnimator`; Environment → Directional Light) plus
  the reused Main Camera and Global Volume. `PrototypeSceneBuilder` builds a
  deterministic demo shot (Mainz → Helsinki, centralized parameters), its
  trajectory, and the flight animator idempotently. Entering Play Mode flies the
  projectile along the trajectory over 8 seconds.
- Tests: Edit Mode `TerraToss.Geo.EditMode.Tests` (119) and
  `TerraToss.Presentation.EditMode.Tests` (41) — 160 total; Play Mode
  `TerraToss.Presentation.PlayMode.Tests` (3, flight flow). All passing.
- No desktop controls, UI, animated camera, or gameplay orchestration yet.

## Current phase

Phase 1: local functional prototype.

## Next recommended task

The projectile flight animation is complete. The next step is desktop aiming
controls that drive the demo shot instead of hardcoded parameters:

1. A presentation/input component that lets the user set heading, launch angle,
   and power on desktop (keyboard/mouse via the Input System), feeding a
   `ShotInput`.
2. Recompute the `ShotResult`, trajectory, and flight on demand (fire action),
   reusing `GeoShotCalculator`, `ShotTrajectorySampler`, `TrajectoryArcProjection`,
   and `ShotFlightAnimator` — no new geographic rules.
3. Minimal on-screen readout of heading/angle/power and the impact grade (a later
   UI task may formalize this).
4. Play Mode test(s) for the aim → fire → flight flow where practical.

Continue reproducing scene changes through the idempotent Editor builder; never
hand-edit `Bootstrap.unity` YAML.

## Known local issue

UnityMCP may need to be started manually through the Python environment documented in `Docs/LOCAL_SETUP.md`.