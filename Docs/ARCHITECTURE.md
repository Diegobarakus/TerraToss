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