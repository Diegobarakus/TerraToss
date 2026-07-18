---
name: debug-unity
description: Diagnose and fix a Unity error using evidence and the smallest safe change.
disable-model-invocation: true
---

Investigate this problem:

$ARGUMENTS

1. Capture the exact Unity console error and full stack trace.
2. Identify the first project-owned stack frame.
3. Inspect only relevant files and recent Git changes.
4. Form a testable root-cause hypothesis.
5. Reproduce the issue when practical.
6. Apply the smallest safe correction.
7. Add a regression test when practical.
8. Let Unity compile.
9. Re-read the Unity console.
10. Run relevant tests.
11. Avoid unrelated refactoring.
12. Update `Docs/CHANGELOG.md` and `Docs/PROJECT_STATE.md`.
13. Do not commit.

Report evidence, root cause, fix, tests, and remaining uncertainty.