README.md
PROJECT OVERVIEW

Premium Casual Mahjong-Style Puzzle Game

Built with:

Unity
Cursor
ChatGPT

Target Platform:

Android
Portrait Orientation
IMPORTANT

Before making any changes:

Read all documentation files.

Do not write code before understanding the project architecture.

Do not bypass documented systems.

REQUIRED READING ORDER
PROJECT_CONTEXT.md
PROJECT_BIBLE.md
PROJECT_ARCHITECTURE.md
PROJECT_DECISIONS.md
DESIGN_BIBLE.md
CURSOR_RULES.md
PROJECT_ROADMAP.md

All files must be understood before implementation begins.

PROJECT DOCUMENTATION
PROJECT_CONTEXT.md

Project vision, goals and design philosophy.

PROJECT_BIBLE.md

Core gameplay rules and game systems.

PROJECT_ARCHITECTURE.md

Technical architecture and system responsibilities.

PROJECT_DECISIONS.md

Locked project decisions and development constraints.

DESIGN_BIBLE.md

Visual design, UI, animation and audio philosophy.

CURSOR_RULES.md

Strict development rules and forbidden actions.

PROJECT_ROADMAP.md

Official development roadmap and phase order.

CORE PROJECT RULE

There must be only one active board generation pipeline.

All boards must be generated through:

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

No alternative board generation systems are allowed.

FORBIDDEN

Do not create:

Alternative Board Generators
Runtime Test Generators
Parallel Gameplay Systems
Duplicate Production Systems

Do not bypass:

BoardGenerationPipeline
BoardSpawner
Official Director Architecture
DEVELOPMENT PHILOSOPHY

Documentation Driven Development

Cursor First Development

Single Pipeline Architecture

Production Ready Systems Only

APPROVED FRAMEWORKS

Mandatory:

DOTween
Addressables
Firebase Analytics
Firebase Crashlytics
Google AdMob
Unity Localization
GitHub

Strongly Recommended:

Unity Test Framework

Optional:

Odin Inspector
Lean Pool
Nice Vibrations
STARTING POINT

Development begins at:

PROJECT_ROADMAP.md

↓

PHASE 0.1

Documentation Compliance Report

No gameplay implementation should begin before PHASE 0 is completed.

END OF README.md