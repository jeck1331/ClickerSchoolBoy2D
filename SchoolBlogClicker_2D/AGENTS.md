# AGENTS.md — Unity Development

## Core Principles

- Follow **First Make it Work → Then Make it Right → Follow Practice**.
- Prefer simple, readable, maintainable code over clever solutions.
- Make the smallest change that fully solves the task.
- Avoid overengineering, unnecessary abstractions, and premature optimization.
- Reuse existing systems, utilities, and project conventions.
- Fix root causes instead of hiding symptoms.
- Do not change unrelated code.

## Skills & Unity Plugin

Use relevant installed skills and plugins when their purpose matches the task.

### General Skills

- **ub-mode** — General development principles, minimal changes, clean architecture, and efficient execution.
- **scenario-unity-gameplay** — Gameplay systems such as AI, physics, pooling, multiplayer, and audio when the task matches the skill's scope.
- **unity-performance** — Runtime performance, memory allocations, garbage collection, and optimization.
- **unity-editor-tools** — Custom inspectors, EditorWindows, PropertyDrawers, Gizmos, Handles, and editor extensions.
- **debugging-code** — Debugger-driven investigation when runtime values or control flow are needed to establish the cause.
- **refactoring-code** — Semantic refactoring of declarations and IDE-resolved usages, including symbol renames and signature changes.

### Official Unity Plugin

Use the Unity plugin for Codex (`unity@unity-agent-plugin`) when available in the current session.

- Use relevant `/unity:` skills for Unity-specific tasks when available.
- Prefer official Unity plugin guidance for Unity APIs, engine features, and supported workflows.
- Automatically select relevant Unity skills when the task clearly matches their descriptions.
- Resolve `/unity:<skill-name>` to an available skill and read its `SKILL.md` before applying it; `/unity:` alone does not identify a skill or a task.
- Read skill reference files only when necessary for the task.
- Do not load unrelated Unity skills or documentation.
- Reuse existing project architecture and conventions.
- Do not assume that the plugin provides direct Unity Editor control unless the installed tools explicitly support it.
- When using Unity MCP tools, load project context with `Unity_GetUserGuidelines` before inspecting or modifying the Unity project.
- Do not create automated tests, test files, or test infrastructure unless explicitly requested.
- Do not run test suites unless explicitly requested.
- Prefer minimal, focused changes over broad rewrites.

### Skill Coordination

- **ub-mode** defines the general development workflow.
- **Official Unity skills** provide specialized Unity knowledge and workflows.
- **Project instructions** define architecture, coding conventions, and safety requirements.
- Use multiple skills together when they provide complementary guidance.
- Avoid redundant skills that add no value to the current task.
- Resolve conflicting instructions according to their applicable priority and the user's explicit requirements.

## Code Quality

- Follow existing C# naming conventions and formatting.
- Keep classes and methods focused on one responsibility.
- Prefer composition over unnecessary inheritance.
- Use interfaces and abstractions only when they solve a real problem.
- Avoid duplicate logic.
- Keep public APIs stable unless changes are necessary.
- Use `[SerializeField] private` instead of public fields when Inspector access is needed without exposing a public API.
- Prefer early returns over deeply nested conditions.
- Do not introduce dependencies without a clear reason.

## Unity Architecture

- Respect Unity's component-based architecture.
- Use `MonoBehaviour` for Unity lifecycle and scene integration.
- Use `ScriptableObject` for suitable shared configuration and data assets.
- Keep domain and gameplay logic independent of `MonoBehaviour` when practical.
- Use `Update` only for frame-dependent logic.
- Use `FixedUpdate` for physics-related operations.
- Use `LateUpdate` when execution order requires it.
- Cache component references when repeated lookup is expensive.
- Avoid unnecessary global managers and singletons.
- Subscribe and unsubscribe from events according to their intended lifecycle.
- Do not introduce new architectural patterns without justification.

## Performance

- Avoid unnecessary allocations in frequently executed code.
- Avoid LINQ in performance-critical paths when it creates measurable overhead.
- Avoid repeated expensive operations inside `Update` and `FixedUpdate`.
- Use object pooling when repeated instantiation and destruction cause a real performance problem.
- Do not optimize code without a reasonable performance justification.
- Prefer profiling evidence over assumptions.
- Keep performance improvements readable and maintainable.

## Unity Asset Safety

- Preserve `.meta` files and asset GUIDs.
- Never regenerate GUIDs unnecessarily.
- Do not modify scenes, prefabs, or serialized assets unless the task requires it.
- Preserve serialized field names when existing scene or prefab data depends on them.
- Consider `[FormerlySerializedAs]` when renaming serialized fields.
- Do not manually rewrite Unity YAML files when a safer method is available.
- Do not delete assets without checking references.
- Preserve existing prefab relationships and overrides.

## Testing Policy

- Do not create unit, integration, or automated tests unless explicitly requested.
- Do not create test files, mocks, fixtures, or testing infrastructure.
- Do not modify existing tests unless explicitly requested.
- Do not install testing frameworks.
- Do not run automated test suites unless explicitly requested.
- Prefer lightweight verification through compilation, relevant static checks, and code inspection.
- Never claim that Unity compilation or runtime behavior was verified unless it actually was.
- Clearly report verification limitations.

## Debugging

1. Identify the observable problem.
2. Inspect the relevant code and error messages.
3. Determine the most likely root cause.
4. Apply the smallest reliable fix.
5. Check affected code paths.
6. Avoid speculative changes and unrelated refactoring.

Do not add excessive logging or defensive checks without a concrete reason.

## Token Efficiency

- Read only files relevant to the current task.
- Use targeted searches instead of dumping entire directories.
- Do not repeatedly read unchanged files.
- Avoid unnecessary exploration of `Library`, `Temp`, `Logs`, `Obj`, and other generated directories.
- Keep plans short for simple tasks.
- Avoid repeating information already established.
- Prefer concise explanations and actionable results.
- Do not create documentation files unless requested.
- Do not create helper scripts for trivial one-time operations.
- Never sacrifice correctness or safety merely to save tokens.
- Load only the skills relevant to the current task.
- Avoid reading entire skill reference directories unnecessarily.
- Prefer targeted code inspection and focused modifications.
- Do not repeat documentation already available in context.
- Keep tool calls and explanations concise without sacrificing correctness.

## Workflow

1. Understand the request and identify the affected systems.
2. Inspect relevant files and existing patterns.
3. Choose the smallest suitable implementation.
4. Make focused changes.
5. Perform available lightweight verification.
6. Summarize the result concisely.

For straightforward tasks, proceed directly without unnecessary planning.

For complex tasks, divide the implementation into small, verifiable steps.

## Git Safety

- Never discard uncommitted user changes.
- Do not reset or clean the worktree without explicit permission.
- Do not commit unless requested or authorized.
- Never push without explicit permission.
- Keep unrelated files unchanged.

## Final Response

After completing a task, briefly report:

- What changed.
- Which files were affected.
- What verification was performed.
- Any important limitations or remaining issues.

Avoid long explanations unless the user requests them.
