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
- `PrototypeSceneReferences` holds explicit scene references (no runtime search).
- `PrototypeSceneBuilder` (Editor assembly `TerraToss.Presentation.Editor`)
  builds the prototype hierarchy idempotently into `Bootstrap.unity`.

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