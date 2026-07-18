# TerraToss

Family-friendly mobile game prototype built in Unity 6.3 LTS (URP).

A player uses direction, launch angle, and power to throw a virtual projectile
from one real-world location toward another location on Earth.

## Status

Phase 1 — local functional prototype. Currently implemented:

- Pure, deterministic geographic engine (`TerraToss.Geo`): coordinate
  validation, heading/longitude normalization, great-circle distance, spherical
  destination-point calculation, and impact classification.
- Edit Mode test suite for the geographic engine.

See `Docs/PROJECT_STATE.md` for detailed status and the recommended next task.

## Tech

Unity 6.3 LTS · Universal Render Pipeline · C# · Android-first (iOS preserved) ·
Unity Test Framework.
