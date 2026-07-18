---
name: start-session
description: Inspect TerraToss project state and propose the next safe task. Use at the beginning of a development session.
disable-model-invocation: true
---

Start a TerraToss development session.

1. Read `CLAUDE.md`.
2. Read `Docs/PROJECT_STATE.md`.
3. Read `Docs/MVP_SCOPE.md`.
4. Run `git status --short`.
5. Inspect the Unity console through UnityMCP.
6. Confirm the active scene and build platform.
7. Do not modify anything.
8. Report:
   - Current phase.
   - Current Git state.
   - Unity console state.
   - Active scene and platform.
   - The single recommended next task.
   - Any blocker that must be solved first.