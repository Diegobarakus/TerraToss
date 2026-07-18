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
    time-driven with no physics; `Play`, `IsPlaying`, `Progress`. Owns no
    geographic rules and does not recompute impact or grade.
- Desktop aiming and interactive fire:
  - `ShotVisualizationDirector` (`MonoBehaviour`): orchestrates a shot by reusing
    `GeoShotCalculator` + `ShotTrajectorySampler` + `TrajectoryArcProjection`,
    updating the trajectory, projectile, and flight from an aim; exposes
    `Fire(heading, angle, power)` and `LastResult`. No new geographic rules.
  - `ShotAimController` (`MonoBehaviour`): desktop keyboard aiming via the Input
    System (polling `Keyboard.current`, no Input Action assets). A/D heading,
    W/S angle (clamped 0–90), Q/E power (clamped 0–1), Space to fire.
  - `ShotAimReadout` (`MonoBehaviour`): minimal IMGUI (`OnGUI`) readout of the
    current aim, the last impact grade / distance, and the match status.
- Camp-mode match rules (`TerraToss.Gameplay` assembly, pure C#,
  `noEngineReferences`, depends on `TerraToss.Geo`):
  - `MatchStatus` (enum: InProgress / Won / Lost) and `CampMatch`: a single valid
    hit (grade at least as good as a configurable threshold) destroys the camp;
    optional shot limit can cause a loss. Deterministic; resolves once.
  - `ShotVisualizationDirector` tracks a `CampMatch` (default threshold
    StrongHit, unlimited shots), counting only real fires; exposes `MatchStatus`
    and `ShotsTaken`, shown in the readout.
- Bootstrap.unity prototype hierarchy: `TerraToss_Prototype` (Earth sphere;
  Markers → Origin_Mainz, Target_Helsinki; ShotVisualization → Projectile,
  Trajectory, with `ShotFlightAnimator`, `ShotVisualizationDirector`,
  `ShotAimController`, `ShotAimReadout`; Environment → Directional Light) plus the
  reused Main Camera and Global Volume. `PrototypeSceneBuilder` idempotently wires
  the shot pipeline and populates an initial static trajectory (Mainz → Helsinki,
  centralized parameters). In Play Mode the player aims with the keyboard and
  presses Space to recompute and fly the shot.
- Tests: Edit Mode `TerraToss.Geo.EditMode.Tests` (119),
  `TerraToss.Presentation.EditMode.Tests` (41), and
  `TerraToss.Gameplay.EditMode.Tests` (17) — 177 total; Play Mode
  `TerraToss.Presentation.PlayMode.Tests` (7, flight + fire + match tracking).
  All passing.
- Only Camp mode exists; no multi-sector modes, animated camera, or formal UI yet.

## Current phase

Phase 1: local functional prototype.

## Next recommended task

Camp-mode match rules are complete. Candidate next steps (pick one and scope it):

1. Match feedback: reflect a Won/Lost outcome in the scene (e.g. change the
   target marker's material/state when the camp is destroyed) and add a restart,
   still driven by the idempotent builder.
2. Camera framing: keep both origin and impact in view after firing (still no
   animated/Cinemachine camera).
3. A first pass at a proper uGUI HUD to replace the IMGUI readout.
4. A second match mode from `GAME_DESIGN.md` (City mode: five sectors) as a pure
   layer with Edit Mode tests.

Prefer pure/testable layers first. Continue reproducing scene changes through the
idempotent Editor builder; never hand-edit `Bootstrap.unity` YAML.

## Known local issue

UnityMCP may need to be started manually through the Python environment documented in `Docs/LOCAL_SETUP.md`.