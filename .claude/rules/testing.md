---
paths:
  - "Assets/_Game/Tests/**/*.cs"
---

# Testing rules

- Use Edit Mode tests for pure C# and geographic calculations.
- Use Play Mode tests only for Unity lifecycle, scene integration, and runtime behavior.
- Tests must be deterministic and independent.
- Tests must not require internet access.
- Tests must not depend on real GPS, compass, gyroscope, or notifications.
- Use fakes for platform services.
- Cover boundary values, invalid inputs, and regression cases.
- A bug fix should include a regression test when practical.
- Do not weaken or delete a test merely to make the suite pass.