---
paths:
  - "Assets/_Game/**/*.cs"
---

# Unity and C# rules

- Use namespaces beginning with `TerraToss`.
- Keep one primary public type per file.
- Match the file name to the primary type name.
- Prefer `private` fields with `[SerializeField]` over public mutable fields.
- Use constructor injection for pure C# classes where practical.
- Use serialized references or explicit composition for `MonoBehaviour` dependencies.
- Avoid scene-wide runtime searches such as `FindObjectOfType`.
- Avoid static mutable gameplay state.
- Avoid LINQ, allocations, and new collections in per-frame code.
- Avoid an `Update()` method when events, coroutines, or explicit calls are sufficient.
- Unsubscribe from events when a component is disabled or destroyed.
- Use `ScriptableObject` for configuration data, not mutable runtime game state.
- Keep UI, presentation, domain logic, and platform services separated.
- Do not suppress warnings without explaining why.