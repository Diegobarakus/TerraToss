# TerraToss — Privacy Rules

## Core rule

Opponents must never receive or display another player's exact real-world coordinates.

## MVP

Phase 1 uses fixed simulated coordinates only.

No real device location is collected.

## Future camp placement

When real location is introduced:
- Request location only while the user is actively placing or moving a camp.
- Do not use background location for the MVP.
- Reduce precision before storing gameplay coordinates.
- Display only a city, region, country, or broad area to opponents.
- Allow the user to keep the previous camp.
- Limit camp movement to prevent abuse.
- Explain clearly why location is requested.

## Data minimization

Store only what is required for gameplay.

Do not log exact coordinates in analytics, crash messages, or public match history.

Location handling must be reviewed before any backend is implemented.