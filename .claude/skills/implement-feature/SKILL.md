---
name: implement-feature
description: Implement one approved TerraToss feature from planning through Unity validation.
disable-model-invocation: true
---

Implement this feature:

$ARGUMENTS

Required workflow:

1. Read `CLAUDE.md`.
2. Read `Docs/MVP_SCOPE.md`.
3. Read only the domain documentation relevant to the feature.
4. Inspect the existing implementation and relevant tests.
5. Inspect `git status`.
6. Inspect the Unity console through UnityMCP.
7. Define measurable acceptance criteria.
8. Present a concise plan and expected files to change.
9. Implement the smallest complete solution.
10. Let Unity compile.
11. Inspect the Unity console.
12. Run relevant Edit Mode and Play Mode tests.
13. Review the complete Git diff for unrelated changes.
14. Update `Docs/PROJECT_STATE.md`, relevant domain documentation, and `Docs/CHANGELOG.md`.
15. Do not commit.

Conclude with:
- Acceptance criteria and result.
- Tests actually executed.
- Unity console status.
- Files changed.
- Known limitations.
- Recommended next task.