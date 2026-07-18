---
paths:
  - "Assets/_Game/Input/**/*.cs"
  - "Assets/_Game/Platform/**/*.cs"
  - "Assets/_Game/Services/**/*.cs"
---

# Android and iOS rules

- Keep platform-specific code behind interfaces.
- Do not use Android-only APIs in shared gameplay assemblies.
- Guard native platform code with the appropriate Unity platform directives.
- Provide a desktop or fake implementation for development and tests.
- Account for permissions being denied or unavailable.
- Account for sensors returning noisy or temporarily unavailable data.
- Do not assume a specific screen resolution.
- Respect portrait layout and mobile safe areas.
- Do not request background location during the MVP.