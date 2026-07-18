# TerraToss — Geographic Engine

## Purpose

Convert a launch definition into:
- Travel distance.
- Geographic impact coordinate.
- Distance to target.
- Impact grade.
- Data required to reconstruct the visual trajectory.

## Core domain types

Planned types:
- `GeoCoordinate`.
- `ShotInput`.
- `ShotResult`.
- `ImpactGrade`.
- `GeoMath`.
- `GeoShotCalculator`.
- `ImpactEvaluator`.

## Coordinate rules

- Latitude valid range: `[-90, 90]`.
- Longitude must be normalized consistently.
- Heading normalized to `[0, 360)`.
- Public distance unit: kilometres.
- Public angle unit: degrees.

## Earth model

Use a spherical Earth model for the MVP.

Use:
- Great-circle distance for coordinate-to-coordinate distance.
- Spherical destination-point calculation for origin + heading + distance.

The MVP does not require an ellipsoidal geodesy library.

## Fictional range model

The projectile range is game physics, not real catapult physics.

Initial model:

```text
rangeKm =
    maximumRangeKm
    × power²
    × sin(2 × launchAngle)
```

Constraints:
- Power normalized to `[0, 1]`.
- Angle validated to an approved gameplay range.
- Invalid input must fail clearly.
- No random dispersion in the first calculation milestone.

## Impact classification

- `< 5 km`: DirectHit.
- `>= 5 km and < 15 km`: StrongHit.
- `>= 15 km and < 40 km`: LightHit.
- `>= 40 km and < 100 km`: NearMiss.
- `>= 100 km`: Miss.

## Required tests

- Mainz to Helsinki representative distance.
- North, east, south, and west destination calculations.
- Antimeridian crossing.
- Coordinates near both poles.
- Heading normalization.
- Invalid latitude and longitude.
- Zero and maximum power.
- Relevant angle boundaries.
- Every impact-grade boundary.