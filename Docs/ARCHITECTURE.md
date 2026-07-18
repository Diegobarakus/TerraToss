# TerraToss — Architecture

## Main principle

Separate deterministic game logic from Unity presentation and platform integration.

## Planned project structure

```text
Assets/_Game/
├── Core/
├── Geo/
├── Gameplay/
│   ├── Aiming/
│   ├── Projectiles/
│   ├── Targets/
│   └── Matches/
├── Input/
├── Platform/
├── Presentation/
├── UI/
├── Editor/
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

## Layers

### Domain and geographic logic

Responsibilities:
- Coordinates.
- Distances.
- Heading.
- Range.
- Impact point.
- Impact grade.
- Match rules.

Requirements:
- Plain C#.
- Deterministic.
- Unit tested.
- Independent from scenes and UI.

### Application orchestration

Responsibilities:
- Create a shot request.
- Invoke calculation services.
- Produce a result.
- Coordinate match state.

Requirements:
- No direct UI rendering.
- No direct sensor APIs.
- Depend on interfaces.

### Unity presentation

Responsibilities:
- Globe.
- Markers.
- Projectile animation.
- Camera.
- UI.
- Audio and effects later.

Requirements:
- Consume calculated results.
- Do not own authoritative geographic rules.

Implemented so far (`TerraToss.Presentation` assembly, depends on `UnityEngine`
and `TerraToss.Geo`):
- `GeoSphereProjection` converts a `GeoCoordinate` to a local `Vector3` on a
  sphere. It may depend on `UnityEngine`; the pure geographic engine stays plain
  C# and is not duplicated.
- `TrajectoryArcProjection` converts geographic trajectory samples to visual
  `Vector3` points, adding a purely cosmetic arc height. See "Trajectory: visual
  arc vs. authoritative path" below.
- `ShotTrajectoryView` (`MonoBehaviour`) draws a trajectory through a
  `LineRenderer` in local space; it owns no geographic rules.
- `FlightPath` (pure) evaluates a deterministic position along the trajectory
  polyline for a normalized progress, and `ShotFlightAnimator` (`MonoBehaviour`)
  moves the projectile along it over a fixed duration (6–10 s), time-driven with
  no physics. Neither owns geographic rules nor recomputes the impact/grade.
- `ShotVisualizationDirector` (`MonoBehaviour`) is the single place that turns an
  aim into a shot: it calls `GeoShotCalculator`, `ShotTrajectorySampler`, and
  `TrajectoryArcProjection`, then updates the trajectory view, projectile, and
  flight. It exposes `Fire(heading, angle, power)` and the last `ShotResult` but
  adds no geographic rules of its own.
- `ShotAimController` (`MonoBehaviour`) reads desktop keyboard input through the
  Input System (polling `Keyboard.current`) to adjust heading/angle/power and to
  fire, and `ShotAimReadout` (`MonoBehaviour`) shows a minimal IMGUI readout.
  Input and UI stay in these components; they never compute authoritative results.
- `PrototypeSceneReferences` holds explicit scene references (no runtime search).
- `PrototypeSceneBuilder` (Editor assembly `TerraToss.Presentation.Editor`)
  builds the prototype hierarchy idempotently into `Bootstrap.unity`, including a
  deterministic demo shot and its trajectory.

The geographic path itself is domain logic: `ShotTrajectorySampler` lives in the
pure `TerraToss.Geo` assembly and produces `GeoCoordinate` samples along the
great-circle path by reusing `GeoMath.DestinationPoint`.

#### Trajectory: visual arc vs. authoritative path

The authoritative shot result (impact coordinate, distance, `ImpactGrade`) is
produced only by `GeoShotCalculator`, and the geographic path only by
`ShotTrajectorySampler` — both pure C#. `TrajectoryArcProjection` then applies a
cosmetic vertical offset `sin(π·progress)·maximumArcHeight` to the projected
points. This arc is presentation-only: it is zero at the endpoints, never feeds
back into any coordinate, distance, or grade, and does not represent real
physics. Presentation performs no authoritative gameplay calculation.

#### Sphere axis convention

Latitude/longitude map to a Unity local position (left-handed, Y up) as:

```text
x = R * cos(lat) * sin(lon)
y = R * sin(lat)
z = R * cos(lat) * cos(lon)
```

- Y is the polar axis: North pole = +Y, South pole = −Y.
- (lat 0, lon 0) = (0, 0, +R) → +Z (Unity forward), the reference meridian on
  the equator.
- East (lon > 0) is toward +X (right); West (lon < 0) toward −X (left).
- The magnitude of the result is exactly R.

### Match rules

Match rules are pure C# domain logic in the `TerraToss.Gameplay` assembly
(`noEngineReferences`, depends on `TerraToss.Geo`). `CampMatch` implements Camp
mode: a single valid hit (impact grade at least as good as a configurable
threshold) wins; an optional shot limit can cause a loss. It only inspects each
shot's `ImpactGrade` and computes no geographic result. Presentation
(`ShotVisualizationDirector`) owns a `CampMatch` instance and feeds real shots to
it, but never reimplements the rules.

### Platform services

Future interfaces:
- Location.
- Compass.
- Gyroscope.
- Notifications.
- Persistent storage.

Requirements:
- Platform-independent interfaces.
- Android and iOS implementations.
- Desktop fakes for development and tests.

## Scene strategy

Use `Bootstrap.unity` as the stable entry scene.

Prototype scenes should be reproducible through idempotent Editor builders where practical.

Avoid direct manual editing of Unity YAML.