# TerraToss — Game Design

## Core fantasy

The Earth is the game board.

Players aim physically or virtually toward opponents in other locations and launch humorous fantasy projectiles across the globe.

The tone is playful, family-friendly, and non-military.

## Core input

A launch is defined by:
- Origin coordinate.
- Heading.
- Launch angle.
- Power.
- Launcher configuration.
- Projectile configuration.
- Optional deterministic dispersion seed.

## Impact grading

Distance from the impact point to the active target:

| Distance | Result |
|---:|---|
| Less than 5 km | Direct hit |
| 5–15 km | Strong hit |
| 15–40 km | Light hit |
| 40–100 km | Near miss |
| More than 100 km | Miss |

Boundary convention:
- Exactly 5 km begins Strong hit.
- Exactly 15 km begins Light hit.
- Exactly 40 km begins Near miss.
- Exactly 100 km begins Miss.

## Game modes

### Camp mode

- One small target sector.
- Hardest target.
- One valid hit destroys the camp.
- Intended for short, precise matches.

### City mode

- The player selects a city.
- The city contains five distinct target sectors:
  - Centre.
  - North.
  - South.
  - East.
  - West.
- Each sector must be hit once.
- A sector already completed cannot count again.

### Country mode

- The player selects a country.
- The country contains ten normalized target sectors.
- All ten sectors must be hit.
- Sector design must avoid making large countries automatically easier than small countries.

## Presentation

During the MVP:
- Earth is a sphere.
- Projectile is a sphere.
- Camp is a cube.
- Targets are cylinders or simple markers.
- Trajectory uses a LineRenderer or generated curve.
- Flight duration is compressed to 6–10 seconds.

## Long-term projectile tone

Examples:
- Stones.
- Water balloons.
- Rubber ducks.
- Flying sheep.
- Slime.
- Pies.
- A comedic pig riding a fantasy rocket.

Avoid realistic missiles, real military systems, killing, or human targets.