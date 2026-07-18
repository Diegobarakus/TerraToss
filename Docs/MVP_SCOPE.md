# TerraToss — MVP Scope

## Objective

Validate whether choosing direction, angle, and power to launch a projectile toward a geographic target is understandable, satisfying, and worth repeating.

## Included in Phase 1

- One local player.
- One simulated target.
- Origin near Mainz.
- Target near Helsinki.
- Desktop controls for heading, angle, and power.
- Pure geographic calculations.
- Fictional range model.
- Projectile path over a spherical Earth.
- Flight animation with a maximum duration of 10 seconds.
- Distance from impact to target.
- Impact classification.
- Bootstrap or prototype scene generated with primitives.
- Edit Mode tests for geographic logic.
- Basic Play Mode tests for the visual launch flow where practical.

## Excluded from Phase 1

- Backend.
- Authentication.
- Friends.
- Online turns.
- Notifications.
- GPS.
- Compass.
- Gyroscope.
- Background location.
- Real map service.
- City mode implementation.
- Country mode implementation.
- Inventory.
- Progression.
- Purchases.
- Final art, audio, particles, or animation.
- App-store publication.

## Phase 1 acceptance criteria

1. The project compiles without errors.
2. Android remains the active build platform.
3. A player can set heading, angle, and power on desktop.
4. A launch produces a deterministic geographic impact coordinate.
5. A projectile visibly travels over the globe.
6. The animation lasts no more than 10 seconds.
7. The result shows distance to the target and impact grade.
8. Geographic tests pass for normal and boundary cases.
9. The scene contains only approved placeholder assets.
10. No backend or sensor code is introduced.