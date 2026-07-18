---
name: prepare-commit
description: Review TerraToss changes and prepare a clean commit without executing it.
disable-model-invocation: true
---

Prepare the current work for a possible Git commit.

1. Show `git status --short`.
2. Review the full staged and unstaged diff.
3. Identify generated, accidental, or unrelated files.
4. Confirm relevant tests were executed.
5. Confirm the Unity console has no new errors.
6. Confirm documentation is current.
7. Propose one conventional commit message.
8. Do not run `git commit`, `git push`, or `git tag`.

Return:
- Files that should be included.
- Files that should be excluded.
- Validation status.
- Proposed commit message.