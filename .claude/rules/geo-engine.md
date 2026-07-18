---
paths:
  - "Assets/_Game/Geo/**/*.cs"
  - "Assets/_Game/Tests/EditMode/Geo/**/*.cs"
---

# Geographic engine rules

- Geographic calculations must be deterministic.
- Geographic domain types must be plain C#.
- Avoid `UnityEngine` dependencies in calculation classes.
- Store and document units explicitly.
- Public angles use degrees.
- Public distances use kilometres unless the type name states otherwise.
- Validate latitude in `[-90, 90]`.
- Normalize longitude consistently.
- Normalize heading to `[0, 360)`.
- Handle the antimeridian correctly.
- Include behavior near both poles.
- Never compare floating-point results using exact equality.
- Any random dispersion must accept an explicit seed.
- Every public calculation requires Edit Mode tests for normal, boundary, and invalid inputs.