# CURSOR_RULES.md

# PURPOSE

This document defines mandatory development rules for Cursor.

Cursor must follow these rules before making any change to the project.

These rules exist to prevent architecture drift, duplicate systems, broken gameplay flows, temporary solutions becoming permanent systems, and accidental violation of project decisions.

---

# REQUIRED READING ORDER

Before making any code, scene, prefab, architecture, or gameplay change:

Cursor must read:

1. PROJECT_DECISIONS.md
2. PROJECT_BIBLE.md
3. PROJECT_ARCHITECTURE.md
4. CURSOR_RULES.md

---

# PROJECT AUTHORITY

If any project documents conflict:

PROJECT_DECISIONS.md is correct.

Authority Order:

1. PROJECT_DECISIONS.md
2. PROJECT_BIBLE.md
3. PROJECT_ARCHITECTURE.md
4. PROJECT_ROADMAP.md
5. DESIGN_BIBLE.md

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
