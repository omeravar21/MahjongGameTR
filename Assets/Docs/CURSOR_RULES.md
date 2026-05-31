# CURSOR_RULES.md

# PURPOSE

This document defines mandatory development rules for Cursor.

Cursor must follow these rules before making any change to the project.

These rules exist to prevent architecture drift, duplicate systems, broken gameplay flows, temporary solutions becoming permanent systems, and accidental violation of project decisions.

---

# REQUIRED READING ORDER

Before making any code, scene, prefab, architecture, or gameplay change:

Cursor must read ALL markdown files inside Assets/Docs:

1. PROJECT_DECISIONS.md
2. PROJECT_BIBLE.md
3. PROJECT_ARCHITECTURE.md
4. CURSOR_RULES.md
5. DESIGN_BIBLE.md
6. PROJECT_CONTEXT.md
7. PROJECT_ROADMAP.md
8. README.md

Never rely on memory.
Never deviate from these documents.
If implementation and documentation conflict, documentation wins.
If documents conflict with each other, stop and report the conflict.

---

# PROJECT AUTHORITY

If any project documents conflict:

PROJECT_DECISIONS.md is correct.

Authority Order:

1. PROJECT_DECISIONS.md
2. PROJECT_BIBLE.md
3. PROJECT_ARCHITECTURE.md
4. CURSOR_RULES.md
5. DESIGN_BIBLE.md
6. PROJECT_CONTEXT.md
7. PROJECT_ROADMAP.md
8. README.md

---

# AUTONOMOUS EXECUTION EXTENSION

## Permanent Approval

Do not ask for:

- APPROVED. EXECUTE THIS PHASE
- CONTINUE NEXT PHASE

Those approvals are permanently granted.

You are authorized to read docs, plan, self-review, implement, test, fix errors, commit, push, and continue roadmap execution without waiting for user approval.

Stop only for:

1. Documentation conflicts.
2. Missing design decisions.
3. Required Unity Editor manual actions.
4. Compile errors that cannot be resolved from project files.
5. Git conflicts that require human intervention.
6. Framework installation requiring user login, license, payment, or manual Unity Asset Store action.

## Documentation Rule

Before every phase, re-read ALL markdown files inside Assets/Docs (listed above).

Never rely on memory.
Never deviate from these documents.
If implementation and documentation conflict, documentation wins.
If documents conflict with each other, stop and report the conflict.

## Continuous Execution Loop

1. Read all Assets/Docs markdown files.
2. Check Git status and current branch.
3. Check Unity/project error state.
4. If errors exist, fix errors first.
5. Find first incomplete roadmap phase.
6. Plan the phase.
7. Self-review the plan.
8. Implement the phase.
9. Self-review changed files.
10. Validate compile/project state where possible.
11. Fix all detected errors.
12. Update roadmap status only when acceptance criteria are met.
13. Run git status.
14. Commit with a clear phase/fix message.
15. Push to GitHub.
16. Continue to the next incomplete roadmap phase automatically.

Never skip: documentation read, error check, self-review, validation, commit, push.

## Error Control Rule

Before starting any new phase:

- Check current Unity compile state where possible.
- Check current known errors.
- Check broken serialized references.
- Check missing scripts.
- Check architecture violations.
- Check Git working tree.

If errors exist:

- Fix errors first.
- Do not start new feature work.
- Do not continue roadmap.
- Do not commit broken code.
- Do not create workaround systems.
- Fix the root cause.
- Validate again.
- Then commit and push the fix.

## Framework Rule

Use approved helper frameworks only when appropriate for the current roadmap phase.

Approved frameworks:

- DOTween / DOTween Pro for animation, UI transitions, tile movement, match effects, door animation.
- Unity Addressables for scalable assets, tile sets, themes, VFX, audio packs.
- Unity Localization for Turkish/English UI text and menu localization.
- Unity Test Framework for generation checks, validators, deadlock/opening-move tests.
- Google AdMob Unity Plugin for rewarded ads when booster ad flow is implemented.
- Firebase Analytics / Unity Gaming Services Analytics before external testing or soft launch.
- Firebase Crashlytics before external testing or soft launch.
- Unity Audio Mixer for music/SFX settings.
- Unity Particle System/VFX packs for match, shatter, joker, win/lose polish.
- Nice Vibrations or custom haptics during polish.
- Lean Pool or custom pooling when performance requires it.
- Odin Inspector only if editor tooling complexity justifies it.

Do not install or use:

- Ready-made Mahjong generators.
- Ready match-game templates.
- Ready level generators.
- Third-party systems replacing BoardGenerationPipeline.
- Systems replacing Tile, Tray, Match, Booster, League, or core gameplay architecture.

Framework installation rules:

- Do not install a framework just because it exists.
- Install only when the current phase requires it.
- If a framework can be installed via Unity Package Manager or manifest safely, do it.
- If installation requires Asset Store, login, license, or manual action, stop and give exact manual instructions.
- After installation, commit and push a separate framework checkpoint.

## MCP/Tool Rule

Use available MCP/tools only when they support the current phase and do not violate documentation.

Allowed MCP/tool usage:

- Unity MCP/tools for scene/object setup, project inspection, compile checks, and safe editor automation.
- Git/GitHub terminal commands for status, commits, pushes, branch checks.
- UI/UX MCP or design tools only for menu, HUD, visual hierarchy, layout, and usability planning.
- Documentation/search tools only to read project docs and verify consistency.
- Test/validation tools only to confirm implementation quality.

Do not use MCP/tools to:

- Bypass project documentation.
- Create alternative systems.
- Replace the custom BoardGenerationPipeline.
- Auto-generate gameplay architecture not defined in docs.
- Install unknown frameworks.
- Introduce template/game-kit dependencies.

Superpowers workflow:

Use available Superpowers skills when useful:

- brainstorming
- writing-plans
- executing-plans
- systematic-debugging
- test-driven-development
- verification-before-completion

---

# NO ASSUMPTION RULE

Cursor may not invent gameplay systems.

Cursor may not assume missing requirements.

Cursor may not create alternative interpretations of documented systems.

If documentation exists:

Follow documentation.

Do not redesign the project.

---

# LOCKED GAMEPLAY RULES

The following decisions are locked.

Do not change them.

Base Grid = 6x7

Tray Capacity = 4

Match Requirement = 2

Portrait Orientation

Single Player

Linear Level Progression

Level 1 → Level 9999

Global Ranking System

Deterministic Levels

Maximum Layer Depth = 4

Maximum Tile Count = 140

Closed Tiles After Level 10

Performance-Based Competition

---

# SINGLE PIPELINE RULE

There must be only one active runtime board generation pipeline.

Official Runtime Flow:

PlayerProgressionDirector

↓

SessionDirector

↓

DifficultyDirector

↓

VisualVarietyDirector

↓

LevelRecipeGenerator

↓

BoardGenerationPipeline

↓

BoardSpawner

No alternative runtime generation path is allowed.

---

# BOARD SPAWNER RULE

BoardSpawner is the only system allowed to instantiate runtime board tiles.

No other runtime system may create board tiles.

No exceptions.

---

# NO ALTERNATIVE GENERATORS

Never create:

Alternative Board Generators

Temporary Generators

Debug Generators

Prototype Generators

Legacy Generators

Experimental Runtime Generators

Random Board Generators

Test Board Generators

All runtime boards must use the official pipeline.

---

# NO PIPELINE BYPASSING

Never bypass:

DifficultyDirector

VisualVarietyDirector

LevelRecipeGenerator

BoardGenerationPipeline

OpeningMoveChecker

DeadlockRiskChecker

BoardQualityChecker

BoardSpawner

Every generated board must pass through the entire pipeline.

---

# BOARD QUALITY RULE

Every runtime board must pass:

OpeningMoveChecker

DeadlockRiskChecker

BoardQualityChecker

If validation fails:

Regenerate.

Never skip validation.

---

# NO DUPLICATE SYSTEMS

Before creating any new system:

Search for an existing system.

Modify existing systems whenever possible.

Never create duplicate systems.

---

# DUPLICATE SYSTEMS FORBIDDEN

Never create duplicate versions of:

BoardGenerationPipeline

BoardSpawner

TrayController

MatchController

ScoreController

ComboController

TimerController

RankingDirector

SaveSystem

RewardDirector

BoosterController

AudioManager

SessionDirector

PlayerProgressionDirector

---

# SINGLE OWNERSHIP RULE

Every gameplay responsibility must have exactly one owner.

Examples:

Tray State → TrayController

Match Logic → MatchController

Score Logic → ScoreController

Ranking Logic → RankingDirector

Board Generation → BoardGenerationPipeline

Do not split ownership.

Do not duplicate ownership.

---

# MODIFY BEFORE REPLACE

When improving systems:

Prefer modification.

Prefer extension.

Prefer refactoring.

Avoid replacement.

Do not rewrite systems unless absolutely necessary.

---

# NO TEMPORARY FIXES

Do not create:

Temporary Scripts

Temporary Managers

Temporary Gameplay Systems

Temporary Runtime Workarounds

Temporary Production Solutions

Every implementation should be production-ready.

---

# GAMEPLAY PROTECTION RULE

Visual requests must not change gameplay.

Gameplay requests must not change visual systems unless requested.

Board generation requests must not modify UI systems.

UI requests must not modify board generation.

Keep responsibilities separated.

---

# SAVE SYSTEM RULES

Never create alternative save systems.

Use the official SaveSystem.

All player progress must flow through the official save architecture.

---

# GLOBAL RANKING RULES

There is only one ranking system.

Global Ranking.

Do not create:

League Systems

Season Systems

Tier Systems

Alternative Ranking Systems

Temporary Ranking Systems

Global Ranking never resets.

---

# THEME SYSTEM RULES

Themes are visual only.

Themes may never affect:

Difficulty

Board Generation

Score

Combo

Progression

Ranking

Gameplay Rules

---

# SYMBOL SYSTEM RULES

Players do not manually choose symbol libraries.

The game selects symbols automatically per level.

Do not create manual symbol selection systems.

---

# PERFORMANCE RULES

Target FPS:

60 FPS

Target Platform:

Mid-Range Android+

Optimize for production performance.

Avoid unnecessary allocations.

Prefer pooling.

---

# POOLING RULES

Use Tile Pool.

Use VFX Pool.

Avoid runtime Instantiate/Destroy loops whenever possible.

---

# FRAMEWORK RULES

Approved Frameworks:

DOTween

Addressables

Unity Localization

Firebase Analytics

Firebase Crashlytics

Google AdMob

Unity Test Framework

Optional:

Odin Inspector

Lean Pool

Nice Vibrations

Easy Save

Do not replace gameplay architecture with third-party systems.

---

# FORBIDDEN FRAMEWORK USAGE

Never import:

Ready Mahjong Generators

Ready Puzzle Generators

Ready Match Systems

Ready Level Generators

Ready Board Generators

Template-Based Core Gameplay Systems

Core gameplay must remain custom.

---

# UNITY ASSISTANCE RULE

Assume the project owner has limited Unity experience.

Whenever manual Unity work is required:

Explain:

What to do

Why it is needed

Where to do it

How to do it

Step-by-step

---

# HIERARCHY RULE

When creating required GameObjects:

Provide exact hierarchy paths.

Example:

GameplayRoot
└── BoardRoot
└── TrayRoot
└── BoosterRoot

Do not assume manual setup knowledge.

---

# INSPECTOR RULE

Whenever references must be assigned:

Explain exactly:

Which component

Which field

Which object

Must be connected.

---

# DEBUG RULES

Debug systems are allowed only for development builds.

Debug systems must not affect production gameplay.

Debug systems must not bypass architecture.

---

# ANALYTICS RULES

Analytics is observational.

Analytics may never:

Change difficulty

Change progression

Change board generation

Change score

Analytics records data only.

---

# ADVERTISEMENT RULES

Ads may never create pay-to-win advantages.

Ads may only provide approved rewards.

Ads may not block progression.

---

# CODE QUALITY RULES

Prefer readable code.

Prefer maintainable code.

Prefer modular code.

Avoid unnecessary complexity.

Avoid hidden dependencies.

Avoid magic numbers.

Use documented architecture.

---

# FUTURE EXPANSION RULE

Allowed Future Expansions:

Kids Mode

Additional Themes

Additional Symbols

Additional Variations

Additional Archetypes

Additional Daily Content

Additional Statistics

Additional Languages

Do not modify locked gameplay foundations.

---

# FINAL PRINCIPLES

Principle 1

There must be only one BoardGenerationPipeline.

---

Principle 2

BoardSpawner is the only runtime tile spawner.

---

Principle 3

Every gameplay responsibility has one owner.

---

Principle 4

The game should become smarter, not larger.

---

Principle 5

Performance competition is more important than speed competition.

---

Principle 6

Global Ranking never resets.

---

Principle 7

Readability is more important than visual complexity.

---

Principle 8

Production architecture is more important than temporary convenience.

---

Principle 9

Follow documentation before creating solutions.

---

Principle 10

Never violate locked project decisions.

---

END OF CURSOR_RULES.md
