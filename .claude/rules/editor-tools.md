---
paths:
  - "Assets/_Game/Editor/**/*.cs"
---

# Unity Editor tooling rules

- Editor-only code must remain inside an `Editor` folder.
- Editor tools must not be included in player assemblies.
- Scene builders must be idempotent.
- Running a scene builder twice must not duplicate objects.
- Generated GameObjects must have stable descriptive names.
- Generated assets must use predictable project-relative paths.
- Save modified scenes explicitly.
- Report missing references as errors instead of silently continuing.
- Prefer an Editor builder over direct manual YAML editing.