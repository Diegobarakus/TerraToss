# TerraToss — Project Instructions

## Mission

Build a functional, family-friendly mobile MVP that proves the core mechanic:

A player uses direction, launch angle, and power to throw a virtual projectile from one real-world location toward another location on Earth.

The current priority is correct gameplay and reliable architecture, not final visuals.

## Current phase

Phase 1: local functional prototype.

Build now:
- Earth represented by a sphere.
- Placeholder objects made from Unity primitives.
- Origin near Mainz, Germany.
- Target near Helsinki, Finland.
- Desktop controls for heading, angle, and power.
- Geographic impact calculation.
- Flight animation compressed to a maximum of 10 seconds.
- Impact result and distance to target.

Do not build yet:
- Backend or online multiplayer.
- User accounts.
- GPS, compass, or gyroscope integration.
- Push notifications.
- Purchases or monetization.
- Final models, textures, audio, or visual effects.
- City and country modes beyond documented architecture.

## Technology

- Unity 6.3 LTS.
- Universal Render Pipeline.
- C#.
- Android is the active development target.
- iOS compatibility must be preserved.
- Unity Test Framework.
- UnityMCP for editor interaction.
- Git.

## Communication

- Communicate with the user in Spanish.
- Use English for code identifiers, namespaces, comments, and technical documentation.
- State assumptions and unresolved problems clearly.
- Never claim a test passed unless it was actually executed.
- Prefer a small correct implementation over a large speculative one.

## Source of truth

When instructions conflict, use this order:

1. The user's latest explicit instruction.
2. `Docs/MVP_SCOPE.md`.
3. `Docs/GAME_DESIGN.md`.
4. `Docs/DECISIONS.md`.
5. Existing tests.
6. Existing implementation.

Do not silently change an approved product decision.

## Architecture rules

- Pure domain and geographic logic must not depend on `MonoBehaviour`.
- Pure geographic logic should avoid `UnityEngine` where practical.
- `MonoBehaviour` classes coordinate Unity lifecycle and presentation only.
- UI must not calculate authoritative gameplay results.
- Platform-specific services must be hidden behind interfaces.
- Android and iOS must share the same gameplay logic.
- Random gameplay behavior must support an explicit deterministic seed.
- Avoid global mutable state.
- Avoid unnecessary `Update()` methods.
- Do not create duplicate systems for the same responsibility.

## Unity workflow

Use UnityMCP whenever editor state is required.

Before modifying:
1. Read this file.
2. Read only the relevant files under `Docs/`.
3. Inspect the existing implementation.
4. Run `git status`.
5. Inspect the Unity console through UnityMCP.
6. Confirm Unity is not importing assets or compiling.
7. Present a short plan and list the expected files to change.

During implementation:
- Work on one coherent feature at a time.
- Do not install packages without explicit approval.
- Do not modify unrelated files.
- Do not manually edit large `.unity`, `.prefab`, or `.asset` YAML files.
- Prefer UnityMCP or idempotent Editor scripts for scene changes.
- Use only Unity primitives and basic URP materials during the MVP.

After implementation:
1. Let Unity finish compiling.
2. Inspect the complete Unity console.
3. Run relevant Edit Mode tests.
4. Run relevant Play Mode tests.
5. Fix all new errors.
6. Review `git diff`.
7. Update relevant documentation.
8. Do not commit or push without explicit approval.

## Definition of done

A task is complete only when:
- Acceptance criteria are satisfied.
- Unity has no new compiler errors.
- Relevant tests pass.
- No serialized references are missing.
- Android compatibility remains intact.
- iOS compatibility remains intact.
- Documentation matches the implementation.
- The Git diff contains no unrelated changes.

## Safety boundaries

Ask before:
- Installing or updating a package.
- Changing Unity, Android, or iOS project settings.
- Changing an approved gameplay rule.
- Performing a large refactor.
- Deleting assets or scenes.
- Creating a Git commit or pushing changes.

Never expose exact player coordinates to opponents.
Never introduce realistic military presentation or realistic weapons.