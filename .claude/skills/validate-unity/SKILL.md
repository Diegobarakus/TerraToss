---
name: validate-unity
description: Validate current TerraToss changes without adding features.
disable-model-invocation: true
---

Validate the current working tree.

1. Read `CLAUDE.md`.
2. Inspect `git status` and the complete diff.
3. Wait until Unity finishes importing and compiling.
4. Read the complete Unity console through UnityMCP.
5. Run relevant Edit Mode tests.
6. Run relevant Play Mode tests.
7. Check for missing serialized references when possible.
8. Confirm Android remains the active build platform.
9. Verify no generated or unrelated files entered the diff.
10. Do not add features.
11. Do not commit.

Report:
- Validation result.
- Console errors and warnings.
- Tests run and results.
- Suspicious or unrelated changes.
- Remaining risks.