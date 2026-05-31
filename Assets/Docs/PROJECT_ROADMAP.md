# PROJECT_ROADMAP.md

# PART 1 — GLOBAL ROADMAP RULES, PHASE 0, PHASE 1, PHASE 2, PHASE 3

---

# PROJECT ROADMAP PHILOSOPHY

The roadmap exists to build the project safely.

The roadmap defines implementation order.

The roadmap does not define gameplay rules.

Gameplay rules are defined in:

PROJECT_DECISIONS.md

PROJECT_BIBLE.md

Architecture rules are defined in:

PROJECT_ARCHITECTURE.md

CURSOR_RULES.md

---

# ROADMAP PROGRESSION RULE

No phase may start until the previous phase is marked Complete.

---

A phase is considered Complete only when:

Acceptance Criteria Passed

No Critical Errors

Manual Unity Steps Completed

GitHub Commit Created

---

# SYSTEM PROTECTION RULE

Completed systems are considered stable.

Do not modify completed systems unless the current roadmap phase explicitly requires it.

---

# REQUIRED MICRO PHASE STRUCTURE

Every micro phase must contain:

Goal

Scope

Do Not Touch

Acceptance Criteria

Manual Unity Steps

GitHub Commit Requirement

Status

Complexity

---

# STATUS TYPES

Not Started

In Progress

Blocked

Needs Review

Complete

---

# COMPLEXITY TYPES

Low

Medium

High

Critical

---

# PHASE 0 — DOCUMENTATION & PROJECT LOCK

---

# PHASE 0.1 — Documentation Creation

Complexity:

Low

Status:

Complete

Goal:

Create all required project documentation.

Scope:

PROJECT_BIBLE.md

PROJECT_ARCHITECTURE.md

PROJECT_DECISIONS.md

CURSOR_RULES.md

PROJECT_ROADMAP.md

PROJECT_CONTEXT.md

DESIGN_BIBLE.md

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

All documentation files created.

GitHub Commit:

PHASE 0.1 Complete - Documentation Created

---

# PHASE 0.2 — Project Decision Lock

Complexity:

Low

Status:

Complete

Goal:

Freeze gameplay and architecture decisions.

Scope:

Review all documentation.

Do Not Touch:

Runtime Systems

Acceptance Criteria:

All major design decisions documented.

GitHub Commit:

PHASE 0.2 Complete - Decisions Locked

---

# PHASE 1 — PROJECT FOUNDATION

---

# PHASE 1.1 — Unity Project Structure

Complexity:

Low

Status:

Complete

Goal:

Create production folder structure.

Scope:

Scripts

Prefabs

Scenes

SO Folders

Audio

VFX

UI

Do Not Touch:

Gameplay Logic

Acceptance Criteria:

Folder structure matches architecture document.

Manual Unity Steps:

Create required folders.

GitHub Commit:

PHASE 1.1 Complete - Folder Structure

---

# PHASE 1.2 — Scene Foundation

Complexity:

Low

Status:

Complete

Goal:

Create base scenes.

Scope:

BootScene

MainMenuScene

GameScene

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

All scenes load successfully.

Manual Unity Steps:

Create scenes.

Add scenes to Build Settings.

GitHub Commit:

PHASE 1.2 Complete - Scene Foundation

---

# PHASE 1.3 — Core Managers

Complexity:

Medium

Status:

Complete

Goal:

Create core project managers.

Scope:

GameBootstrapper

SceneLoadController

GameState

GameEvents

Do Not Touch:

Board Generation

Acceptance Criteria:

Core managers initialize correctly.

GitHub Commit:

PHASE 1.3 Complete - Core Managers

---

# PHASE 1.4 — Save System Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create official SaveSystem.

Scope:

SaveSystem

Save Data Models

Persistence Structure

Do Not Touch:

Gameplay Logic

Acceptance Criteria:

Data can save and load.

GitHub Commit:

PHASE 1.4 Complete - Save Foundation

---

# PHASE 1.5 — Player Progression Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create progression framework.

Scope:

PlayerProgressionDirector

PlayerProgressData

LevelProgressData

Do Not Touch:

Board Generation

Acceptance Criteria:

Current Level persists correctly.

GitHub Commit:

PHASE 1.5 Complete - Progression Foundation

---

# PHASE 2 — MAIN MENU FOUNDATION

---

# PHASE 2.1 — Main Menu Layout

Complexity:

Low

Status:

Complete

Goal:

Create menu structure.

Scope:

Profile Button

Theme Button

Settings Button

Level Button

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

UI hierarchy complete.

Manual Unity Steps:

Create Canvas structure.

GitHub Commit:

PHASE 2.1 Complete - Main Menu Layout

---

# PHASE 2.2 — Door Presentation

Complexity:

Medium

Status:

Complete

Goal:

Create closed door visual.

Scope:

Door Background

Door Visual Layer

Door Animation Preparation

Do Not Touch:

Board Systems

Acceptance Criteria:

Door presentation visible.

GitHub Commit:

PHASE 2.2 Complete - Door Presentation

---

# PHASE 2.3 — Current Level Button

Complexity:

Low

Status:

Complete

Goal:

Display current level.

Scope:

Level Button

Level Number Binding

Do Not Touch:

Progression Logic

Acceptance Criteria:

Button shows correct current level.

GitHub Commit:

PHASE 2.3 Complete - Current Level Button

---

# PHASE 2.4 — Main Menu Navigation

Complexity:

Medium

Status:

Complete

Goal:

Connect menu navigation.

Scope:

Profile

Theme

Settings

Level Start

Do Not Touch:

GameScene

Acceptance Criteria:

All buttons navigate correctly.

GitHub Commit:

PHASE 2.4 Complete - Main Menu Navigation

---

# PHASE 2.5 — Door To Gameplay Transition

Complexity:

Medium

Status:

Complete

Goal:

Connect door opening flow.

Scope:

Button Click

Door Animation

Scene Transition

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

Door opens and GameScene loads.

GitHub Commit:

PHASE 2.5 Complete - Door Transition

---

# PHASE 3 — BOARD VISUAL FOUNDATION

---

# PHASE 3.1 — Board Root

Complexity:

Low

Status:

Complete

Goal:

Create board hierarchy foundation.

Scope:

BoardRoot

GameplayRoot

Board Containers

Do Not Touch:

Board Generation

Acceptance Criteria:

Board hierarchy exists.

Manual Unity Steps:

Create BoardRoot hierarchy.

GitHub Commit:

PHASE 3.1 Complete - Board Root

---

# PHASE 3.2 — Grid Foundation

Complexity:

Medium

Status:

Complete

Goal:

Implement visual 6x7 board grid.

Scope:

Grid Coordinates

Grid Cell Layout

Board Dimensions

Do Not Touch:

Board Generation Pipeline

Acceptance Criteria:

6x7 visual grid displayed correctly.

GitHub Commit:

PHASE 3.2 Complete - Grid Foundation

---

# PHASE 3.3 — Tile Prefab

Complexity:

Medium

Status:

Complete

Goal:

Create production tile prefab.

Scope:

Tile Visual

Tile Components

Tile States

Sorting Setup

Do Not Touch:

Match System

Acceptance Criteria:

Tile prefab ready for production use.

Manual Unity Steps:

Create prefab.

Assign references.

GitHub Commit:

PHASE 3.3 Complete - Tile Prefab

---

# PHASE 3.4 — Layer Visual Foundation

Complexity:

High

Status:

Complete

Goal:

Create visual layer system.

Scope:

Layer Offsets

Sorting Logic

Visual Stacking

Do Not Touch:

Tile Selection

Acceptance Criteria:

Layer readability is correct.

GitHub Commit:

PHASE 3.4 Complete - Layer Visuals

---

# PHASE 3.5 — Board Presentation

Complexity:

Medium

Status:

Complete

Goal:

Create final board presentation.

Scope:

Board Framing

Board Positioning

Board Readability

Camera Alignment

Do Not Touch:

Board Generation

Acceptance Criteria:

Board presentation matches design goals.

GitHub Commit:

PHASE 3.5 Complete - Board Presentation

---

END OF PROJECT_ROADMAP.md PART 1
# PROJECT_ROADMAP.md

# PART 2 — PHASE 4, PHASE 5, PHASE 6, PHASE 7, PHASE 8

---

# PHASE 4 — TILE INTERACTION FOUNDATION

---

# PHASE 4.1 — Tile Data Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create tile data architecture.

Scope:

TileData

TileType

TileState

Tile Identity

Do Not Touch:

Tray System

Match System

Acceptance Criteria:

Tiles contain required gameplay data.

GitHub Commit:

PHASE 4.1 Complete - Tile Data Foundation

---

# PHASE 4.2 — Tile Selection Controller

Complexity:

Medium

Status:

Complete

Goal:

Create tile selection entry point.

Scope:

TileSelectionController

Input Handling

Selection Requests

Do Not Touch:

Match Logic

Acceptance Criteria:

Valid tiles can receive selection requests.

GitHub Commit:

PHASE 4.2 Complete - Tile Selection Controller

---

# PHASE 4.3 — Tile Selectability Checker

Complexity:

High

Status:

Complete

Goal:

Implement selection validation.

Scope:

Upper Tile Blocking

Side Blocking

State Validation

Do Not Touch:

Tray System

Acceptance Criteria:

Only valid tiles are selectable.

GitHub Commit:

PHASE 4.3 Complete - Selectability Checker

---

# PHASE 4.4 — Tile Movement Controller

Complexity:

Medium

Status:

Complete

Goal:

Move selected tiles.

Scope:

Board → Tray Movement

Movement Events

Animation Hooks

Do Not Touch:

Match System

Acceptance Criteria:

Selected tiles move correctly.

GitHub Commit:

PHASE 4.4 Complete - Tile Movement

---

# PHASE 4.5 — Tile Interaction Validation

Complexity:

Medium

Status:

Complete

Goal:

Validate interaction system.

Scope:

Selection

Movement

Blocking Rules

Do Not Touch:

Tray Capacity

Acceptance Criteria:

Interaction system works reliably.

GitHub Commit:

PHASE 4.5 Complete - Tile Interaction Validation

---

# PHASE 5 — TRAY SYSTEM

---

# PHASE 5.1 — Tray Root

Complexity:

Low

Status:

Complete

Goal:

Create tray hierarchy.

Scope:

TrayRoot

Tray Container

Tray Layout

Do Not Touch:

Match System

Acceptance Criteria:

Tray structure exists.

GitHub Commit:

PHASE 5.1 Complete - Tray Root

---

# PHASE 5.2 — Tray Slot Creation

Complexity:

Low

Status:

Complete

Goal:

Create four tray slots.

Scope:

TraySlot_0

TraySlot_1

TraySlot_2

TraySlot_3

Do Not Touch:

Matching

Acceptance Criteria:

Four tray slots visible.

GitHub Commit:

PHASE 5.2 Complete - Tray Slots

---

# PHASE 5.3 — Tray Capacity Logic

Complexity:

Medium

Status:

Complete

Goal:

Implement capacity control.

Scope:

Capacity = 4

Overflow Detection

Slot Management

Do Not Touch:

Match System

Acceptance Criteria:

Tray respects capacity rule.

GitHub Commit:

PHASE 5.3 Complete - Tray Capacity

---

# PHASE 5.4 — Tray Controller

Complexity:

Medium

Status:

Complete

Goal:

Implement tray ownership.

Scope:

TrayController

Tray State

Tile Storage

Do Not Touch:

Match Logic

Acceptance Criteria:

TrayController owns tray state.

GitHub Commit:

PHASE 5.4 Complete - Tray Controller

---

# PHASE 5.5 — Tray Validation

Complexity:

Medium

Status:

Complete

Goal:

Validate tray system.

Scope:

Capacity

Storage

Overflow

Do Not Touch:

Match System

Acceptance Criteria:

Tray behaves correctly.

GitHub Commit:

PHASE 5.5 Complete - Tray Validation

---

# PHASE 6 — MATCH SYSTEM

---

# PHASE 6.1 — Match Detection

Complexity:

Medium

Status:

Complete

Goal:

Detect matching pairs.

Scope:

Pair Detection

Tile Comparison

Match Requests

Do Not Touch:

Score System

Acceptance Criteria:

Matching pairs detected.

GitHub Commit:

PHASE 6.1 Complete - Match Detection

---

# PHASE 6.2 — Match Delay

Complexity:

Low

Status:

Complete

Goal:

Add match timing.

Scope:

0.3 Second Delay

Match Queue

Do Not Touch:

Scoring

Acceptance Criteria:

Matches wait before removal.

GitHub Commit:

PHASE 6.2 Complete - Match Delay

---

# PHASE 6.3 — Match Execution

Complexity:

Medium

Status:

Complete

Goal:

Execute successful matches.

Scope:

Remove Tiles

State Updates

Events

Do Not Touch:

Combo System

Acceptance Criteria:

Matched tiles disappear correctly.

GitHub Commit:

PHASE 6.3 Complete - Match Execution

---

# PHASE 6.4 — Match Cleanup

Complexity:

Medium

Status:

Complete

Goal:

Cleanup match results.

Scope:

Slot Cleanup

References

Events

Do Not Touch:

Score System

Acceptance Criteria:

No leftover match data exists.

GitHub Commit:

PHASE 6.4 Complete - Match Cleanup

---

# PHASE 6.5 — Match Validation

Complexity:

Medium

Status:

Complete

Goal:

Validate complete match flow.

Scope:

Detection

Execution

Cleanup

Do Not Touch:

Timer

Acceptance Criteria:

Full match flow works correctly.

GitHub Commit:

PHASE 6.5 Complete - Match Validation

---

# PHASE 7 — WIN / LOSE / SESSION LOOP

---

# PHASE 7.1 — Session Director Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create session ownership.

Scope:

SessionDirector

Session State

Session Events

Do Not Touch:

Score

Acceptance Criteria:

Session state controlled centrally.

GitHub Commit:

PHASE 7.1 Complete - Session Foundation

---

# PHASE 7.2 — Win Detection

Complexity:

Medium

Status:

Complete

Goal:

Detect level completion.

Scope:

Board Cleared

Completion Validation

Do Not Touch:

Progression

Acceptance Criteria:

Win condition detected correctly.

GitHub Commit:

PHASE 7.2 Complete - Win Detection

---

# PHASE 7.3 — Tray Overflow Failure

Complexity:

Medium

Status:

Complete

Goal:

Detect tray failure.

Scope:

Overflow Check

Failure Trigger

Do Not Touch:

Timer

Acceptance Criteria:

Tray overflow causes fail.

GitHub Commit:

PHASE 7.3 Complete - Tray Failure

---

# PHASE 7.4 — Session Restart Flow

Complexity:

Medium

Status:

Complete

Goal:

Restart current level.

Scope:

Level Reset

Session Reset

Board Reset

Do Not Touch:

Progression

Acceptance Criteria:

Failed level restarts correctly.

GitHub Commit:

PHASE 7.4 Complete - Restart Flow

---

# PHASE 7.5 — Session Validation

Complexity:

Medium

Status:

Complete

Goal:

Validate gameplay loop.

Scope:

Start

Win

Fail

Restart

Do Not Touch:

Board Generation

Acceptance Criteria:

Gameplay loop is stable.

GitHub Commit:

PHASE 7.5 Complete - Session Validation

---

# PHASE 8 — TIMER, SCORE, COMBO FOUNDATION

---

# PHASE 8.1 — Timer Controller

Complexity:

Medium

Status:

Complete

Goal:

Create timer ownership.

Scope:

TimerController

Countdown

Expiration

Do Not Touch:

Scoring

Acceptance Criteria:

Timer functions correctly.

GitHub Commit:

PHASE 8.1 Complete - Timer Controller

---

# PHASE 8.2 — Score Controller

Complexity:

Medium

Status:

Complete

Goal:

Implement score ownership.

Scope:

Base Match Score

Score Tracking

Do Not Touch:

Global Ranking

Acceptance Criteria:

Score updates correctly.

GitHub Commit:

PHASE 8.2 Complete - Score Controller

---

# PHASE 8.3 — Combo Controller

Complexity:

Medium

Status:

Complete

Goal:

Implement combo system.

Scope:

Combo Tracking

Combo Window

Combo Rewards

Do Not Touch:

Ranking

Acceptance Criteria:

Combos function correctly.

GitHub Commit:

PHASE 8.3 Complete - Combo Controller

---

# PHASE 8.4 — Timer Failure Integration

Complexity:

Medium

Status:

Complete

Goal:

Connect timer to fail state.

Scope:

Timer Expiration

Session Fail

Penalty Events

Do Not Touch:

Progression

Acceptance Criteria:

Timer expiration causes fail.

GitHub Commit:

PHASE 8.4 Complete - Timer Fail Integration

---

# PHASE 8.5 — Performance Screen Foundation

Complexity:

High

Status:

Complete

Goal:

Create post-level results screen.

Scope:

Completion Time

Combo Count

Score

Next Level Button

Do Not Touch:

Global Ranking

Acceptance Criteria:

Performance screen displays correctly.

GitHub Commit:

PHASE 8.5 Complete - Performance Screen

---

END OF PROJECT_ROADMAP.md PART 2
# PROJECT_ROADMAP.md

# PART 3 — PHASE 9, PHASE 10, PHASE 11, PHASE 12, PHASE 13

---

# PHASE 9 — BOARD GENERATION PIPELINE

---

# PHASE 9.1 — DifficultyDirector

Complexity:

High

Status:

Complete

Goal:

Create difficulty ownership.

Scope:

Tile Count Scaling

Complexity Scaling

Closed Tile Scaling

Joker Scaling

Timer Recommendations

Do Not Touch:

Board Spawning

Acceptance Criteria:

Difficulty values generated correctly.

GitHub Commit:

PHASE 9.1 Complete - DifficultyDirector

---

# PHASE 9.2 — VisualVarietyDirector

Complexity:

Medium

Status:

Complete

Goal:

Control visual diversity.

Scope:

Archetype Rotation

Variation Rotation

Pattern Rotation

Do Not Touch:

Difficulty Logic

Acceptance Criteria:

Visual repetition reduced.

GitHub Commit:

PHASE 9.2 Complete - Visual Variety

---

# PHASE 9.3 — LevelRecipeGenerator

Complexity:

High

Status:

Complete

Goal:

Generate level recipes.

Scope:

Recipe Construction

Difficulty Inputs

Visual Inputs

Do Not Touch:

Board Spawning

Acceptance Criteria:

Recipes generated correctly.

GitHub Commit:

PHASE 9.3 Complete - LevelRecipeGenerator

---

# PHASE 9.4 — GridMask System

Complexity:

High

Status:

Complete

Goal:

Implement grid masking.

Scope:

6x7 Base Grid

Cell Activation

Occupancy Definition

Do Not Touch:

Archetypes

Acceptance Criteria:

Grid masks generated correctly.

GitHub Commit:

PHASE 9.4 Complete - GridMask

---

# PHASE 9.5 — Archetype System

Complexity:

High

Status:

Complete

Goal:

Implement archetype layouts.

Scope:

Launch Archetypes (Phase 9.5):

Pyramid

Tower

Oval

Maze

Diamond

Cross

Bridge

Island

Future Archetypes (deferred — not Phase 9.5 scope):

Snake

Spiral

Stairs

Fortress

Do Not Touch:

Variation Logic

Acceptance Criteria:

Launch archetypes generated correctly.

GitHub Commit:

PHASE 9.5 Complete - Archetypes

---

# PHASE 9.6 — Variation System

Complexity:

Critical

Status:

Complete

Goal:

Implement variation architecture.

Scope:

Archetype Variations

Visual Diversity

Layout Flavor

Do Not Touch:

Base Grid

Acceptance Criteria:

Variations work independently from archetypes.

GitHub Commit:

PHASE 9.6 Complete - Variations

---

# PHASE 9.7 — Hole Pattern System

Complexity:

High

Status:

Complete

Goal:

Generate controlled empty spaces.

Scope:

Hole Placement

Hole Validation

Pattern Selection

Do Not Touch:

Tile Distribution

Acceptance Criteria:

Hole patterns generated correctly.

GitHub Commit:

PHASE 9.7 Complete - Hole Patterns

---

# PHASE 9.8 — LayerBuilder

Complexity:

Critical

Status:

Complete

Goal:

Generate tile layers.

Scope:

Layer Construction

Layer Offsets

Stack Rules

Do Not Touch:

Tile Symbols

Acceptance Criteria:

Layer structure is valid and readable.

GitHub Commit:

PHASE 9.8 Complete - LayerBuilder

---

# PHASE 9.9 — TilePairDistributor

Complexity:

Critical

Status:

Complete

Goal:

Distribute symbols.

Scope:

Pair Assignment

Symbol Selection

Balance Logic

Do Not Touch:

Board Validation

Acceptance Criteria:

Pairs distributed fairly.

GitHub Commit:

PHASE 9.9 Complete - TilePairDistributor

---

# PHASE 9.10 — BoardSpawner Integration

Complexity:

Critical

Status:

Complete

Goal:

Connect generation pipeline to runtime.

Scope:

BoardSpawner

Tile Instantiation

Runtime Board Creation

Do Not Touch:

Validation Systems

Acceptance Criteria:

Pipeline successfully spawns runtime boards.

GitHub Commit:

PHASE 9.10 Complete - BoardSpawner Integration

---

# PHASE 10 — BOARD VALIDATION SYSTEMS

---

# PHASE 10.1 — OpeningMoveChecker

Complexity:

Critical

Status:

Complete

Goal:

Validate opening moves.

Scope:

Playable Start

Opening Pair Detection

Do Not Touch:

Tile Distribution

Acceptance Criteria:

All generated boards have valid opening moves.

GitHub Commit:

PHASE 10.1 Complete - OpeningMoveChecker

---

# PHASE 10.2 — DeadlockRiskChecker

Complexity:

Critical

Status:

Complete

Goal:

Detect deadlock risks.

Scope:

Playable State Analysis

Future Move Analysis

Do Not Touch:

Board Quality

Acceptance Criteria:

Deadlock risks identified correctly.

GitHub Commit:

PHASE 10.2 Complete - DeadlockRiskChecker

---

# PHASE 10.3 — BoardQualityChecker

Complexity:

Critical

Status:

Complete

Goal:

Validate final board quality.

Scope:

Readability

Fairness

Difficulty Targets

Progression Targets

Do Not Touch:

Spawner

Acceptance Criteria:

Boards meet quality standards.

GitHub Commit:

PHASE 10.3 Complete - BoardQualityChecker

---

# PHASE 10.4 — Regeneration Logic

Complexity:

High

Status:

Complete

Goal:

Regenerate invalid boards.

Scope:

Validation Failures

Regeneration Attempts

Retry System

Do Not Touch:

Fallback Logic

Acceptance Criteria:

Invalid boards regenerate correctly.

GitHub Commit:

PHASE 10.4 Complete - Regeneration Logic

---

# PHASE 10.5 — Emergency Fallback Recipe

Complexity:

High

Status:

Complete

Goal:

Guarantee playable board output.

Scope:

Fallback Recipes

Emergency Generation

Failure Recovery

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

Generation always produces a playable board.

GitHub Commit:

PHASE 10.5 Complete - Emergency Fallback

---

# PHASE 11 — CLOSED TILE SYSTEM

---

# PHASE 11.1 — Closed Tile Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create closed tile architecture.

Scope:

ClosedTileController

ClosedTileState

ClosedTileData

Do Not Touch:

Jokers

Acceptance Criteria:

Closed tile system initialized.

GitHub Commit:

PHASE 11.1 Complete - Closed Tile Foundation

---

# PHASE 11.2 — Reveal Logic

Complexity:

Medium

Status:

Complete

Goal:

Implement reveal behavior.

Scope:

First Tap Reveal

State Changes

Do Not Touch:

Tray System

Acceptance Criteria:

Closed tiles reveal correctly.

GitHub Commit:

PHASE 11.2 Complete - Reveal Logic

---

# PHASE 11.3 — Second Tap Logic

Complexity:

Medium

Status:

Complete

Goal:

Move revealed tiles.

Scope:

Second Tap

Tray Integration

Do Not Touch:

Match System

Acceptance Criteria:

Revealed tiles move correctly.

GitHub Commit:

PHASE 11.3 Complete - Second Tap Logic

---

# PHASE 11.4 — Reclose Logic

Complexity:

High

Status:

Complete

Goal:

Allow re-closing.

Scope:

Auto Close

State Restoration

Do Not Touch:

Selection Rules

Acceptance Criteria:

Closed tiles re-close correctly.

GitHub Commit:

PHASE 11.4 Complete - Reclose Logic

---

# PHASE 11.5 — Closed Tile Patterns

Complexity:

High

Status:

Complete

Goal:

Implement pattern selection.

Scope:

CornerSingle

CornerPair

CenterPair

CenterTriple

HorizontalLine3

HorizontalLine5

VerticalLine3

LShape

PlusShape

DiagonalPair

StackedPair

HiddenUnderLayer

SandwichClosed

MixedCornersAndCenter

Do Not Touch:

Board Validation

Acceptance Criteria:

Patterns spawn correctly.

GitHub Commit:

PHASE 11.5 Complete - Closed Tile Patterns

---

# PHASE 12 — REWARD JOKER SYSTEM

---

# PHASE 12.1 — Joker Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create joker architecture.

Scope:

JokerTileController

RewardDirector

Joker State

Do Not Touch:

Boosters

Acceptance Criteria:

Joker system initialized.

GitHub Commit:

PHASE 12.1 Complete - Joker Foundation

---

# PHASE 12.2 — Joker Placement

Complexity:

Medium

Status:

Complete

Goal:

Place jokers on boards.

Scope:

Joker Selection

Joker Distribution

Do Not Touch:

Scoring

Acceptance Criteria:

Jokers spawn correctly.

GitHub Commit:

PHASE 12.2 Complete - Joker Placement

---

# PHASE 12.3 — Early Match Detection

Complexity:

Medium

Status:

Complete

Goal:

Detect early joker clears.

Scope:

1 Minute Rule

Reward Tracking

Do Not Touch:

Combo

Acceptance Criteria:

Early joker matches detected correctly.

GitHub Commit:

PHASE 12.3 Complete - Early Joker Detection

---

# PHASE 12.4 — Joker Rewards

Complexity:

Medium

Status:

Complete

Goal:

Award joker bonuses.

Scope:

Score Bonus

Performance Bonus

Do Not Touch:

Ranking

Acceptance Criteria:

Bonuses granted correctly.

GitHub Commit:

PHASE 12.4 Complete - Joker Rewards

---

# PHASE 13 — BOOSTER SYSTEM

---

# PHASE 13.1 — Booster Economy Foundation

Complexity:

Medium

Status:

Complete

Goal:

Create booster economy.

Scope:

Booster Counts

Progression Rewards

Storage

Do Not Touch:

Ads

Acceptance Criteria:

Booster counts persist correctly.

GitHub Commit:

PHASE 13.1 Complete - Booster Economy

---

# PHASE 13.2 — Shuffle Booster

Complexity:

High

Status:

Complete

Goal:

Implement shuffle.

Scope:

Symbol Redistribution

Board Refresh

Do Not Touch:

Tile Positions

Acceptance Criteria:

Shuffle changes symbols only.

GitHub Commit:

PHASE 13.2 Complete - Shuffle Booster

---

# PHASE 13.3 — Undo Booster

Complexity:

High

Status:

Complete

Goal:

Implement undo.

Scope:

Last Move Tracking

State Restore

Do Not Touch:

Match Rules

Acceptance Criteria:

Undo restores previous state.

GitHub Commit:

PHASE 13.3 Complete - Undo Booster

---

# PHASE 13.4 — Hint Booster

Complexity:

Medium

Status:

Complete

Goal:

Suggest playable moves.

Scope:

Hint Detection

Hint Presentation

Do Not Touch:

Auto Solve

Acceptance Criteria:

Hints identify valid moves.

GitHub Commit:

PHASE 13.4 Complete - Hint Booster

---

# PHASE 13.5 — Booster Validation

Complexity:

High

Status:

Not Started

Goal:

Validate booster behavior.

Scope:

Shuffle

Undo

Hint

Persistence

Do Not Touch:

Ranking

Acceptance Criteria:

All boosters function correctly.

GitHub Commit:

PHASE 13.5 Complete - Booster Validation

---

END OF PROJECT_ROADMAP.md PART 3
# PROJECT_ROADMAP.md

# PART 4 — PHASE 14, PHASE 15, PHASE 16, PHASE 17, PHASE 18

---

# PHASE 14 — SAVE / RESUME SYSTEM

---

# PHASE 14.1 — Active Level Save

Complexity:

High

Status:

Not Started

Goal:

Save active level state.

Scope:

Current Board State

Current Level

Timer State

Score State

Do Not Touch:

Ranking

Acceptance Criteria:

Player resumes exact same level after restart.

GitHub Commit:

PHASE 14.1 Complete - Active Level Save

---

# PHASE 14.2 — Active Board Save

Complexity:

Critical

Status:

Not Started

Goal:

Save runtime board state.

Scope:

Tile States

Closed Tiles

Jokers

Tray State

Do Not Touch:

Generation Pipeline

Acceptance Criteria:

Board restores exactly.

GitHub Commit:

PHASE 14.2 Complete - Active Board Save

---

# PHASE 14.3 — Resume Session Logic

Complexity:

High

Status:

Not Started

Goal:

Resume interrupted sessions.

Scope:

Game Restart

State Recovery

Session Recovery

Do Not Touch:

Progression

Acceptance Criteria:

Interrupted session resumes correctly.

GitHub Commit:

PHASE 14.3 Complete - Resume Session

---

# PHASE 14.4 — Save Validation

Complexity:

Critical

Status:

Not Started

Goal:

Validate save system.

Scope:

Save

Load

Resume

Recovery

Do Not Touch:

Gameplay Rules

Acceptance Criteria:

No data loss.

GitHub Commit:

PHASE 14.4 Complete - Save Validation

---

# PHASE 15 — PROGRESSION SYSTEM

---

# PHASE 15.1 — Level Progression Rules

Complexity:

Medium

Status:

Not Started

Goal:

Implement level advancement.

Scope:

Current Level

Next Level

Completion Flow

Do Not Touch:

Ranking

Acceptance Criteria:

Players progress correctly.

GitHub Commit:

PHASE 15.1 Complete - Progression Rules

---

# PHASE 15.2 — Level Completion Flow

Complexity:

Medium

Status:

Not Started

Goal:

Connect win to progression.

Scope:

Performance Screen

Next Level Button

Progress Update

Do Not Touch:

Global Ranking

Acceptance Criteria:

Players advance correctly.

GitHub Commit:

PHASE 15.2 Complete - Completion Flow

---

# PHASE 15.3 — Difficulty Scaling Integration

Complexity:

High

Status:

Not Started

Goal:

Connect progression to difficulty.

Scope:

DifficultyDirector

Tile Scaling

Closed Tile Scaling

Joker Scaling

Do Not Touch:

Generation Architecture

Acceptance Criteria:

Difficulty increases correctly.

GitHub Commit:

PHASE 15.3 Complete - Difficulty Integration

---

# PHASE 15.4 — Progression Validation

Complexity:

High

Status:

Not Started

Goal:

Validate long-term progression.

Scope:

Level Flow

Difficulty Flow

Save Flow

Do Not Touch:

Ranking

Acceptance Criteria:

Progression remains stable.

GitHub Commit:

PHASE 15.4 Complete - Progression Validation

---

# PHASE 16 — GLOBAL RANKING SYSTEM

---

# PHASE 16.1 — Ranking Architecture

Complexity:

High

Status:

Not Started

Goal:

Create ranking ownership.

Scope:

RankingDirector

Ranking Models

Leaderboard Models

Do Not Touch:

Daily Board

Acceptance Criteria:

Ranking architecture created.

GitHub Commit:

PHASE 16.1 Complete - Ranking Architecture

---

# PHASE 16.2 — Global Performance Score

Complexity:

High

Status:

Not Started

Goal:

Implement score accumulation.

Scope:

Performance Score

Lifetime Score

Score Tracking

Do Not Touch:

Leaderboard Sync

Acceptance Criteria:

Performance score calculated correctly.

GitHub Commit:

PHASE 16.2 Complete - Performance Score

---

# PHASE 16.3 — Global Leaderboard

Complexity:

Critical

Status:

Not Started

Goal:

Create global leaderboard.

Scope:

Player Rank

Player Score

Global Position

Do Not Touch:

Daily Missions

Acceptance Criteria:

Leaderboard displays correctly.

GitHub Commit:

PHASE 16.3 Complete - Global Leaderboard

---

# PHASE 16.4 — Leaderboard Sync

Complexity:

Critical

Status:

Not Started

Goal:

Synchronize ranking data.

Scope:

Upload Scores

Download Rank

Refresh Data

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

Ranking sync works correctly.

GitHub Commit:

PHASE 16.4 Complete - Leaderboard Sync

---

# PHASE 16.5 — Ranking Validation

Complexity:

Critical

Status:

Not Started

Goal:

Validate ranking system.

Scope:

Score

Rank

Sync

Display

Do Not Touch:

Gameplay

Acceptance Criteria:

Ranking system stable.

GitHub Commit:

PHASE 16.5 Complete - Ranking Validation

---

# PHASE 17 — DAILY BOARD & DAILY MISSIONS

---

# PHASE 17.1 — Daily Board Architecture

Complexity:

High

Status:

Not Started

Goal:

Create Daily Board system.

Scope:

Daily Board Model

Daily Seed

Daily Availability

Do Not Touch:

Normal Progression

Acceptance Criteria:

Daily Board generated correctly.

GitHub Commit:

PHASE 17.1 Complete - Daily Board Architecture

---

# PHASE 17.2 — Daily Board Generation

Complexity:

High

Status:

Not Started

Goal:

Generate daily content.

Scope:

Board Generation

Daily Rules

Daily Rewards

Do Not Touch:

Normal Levels

Acceptance Criteria:

Daily Board playable.

GitHub Commit:

PHASE 17.2 Complete - Daily Board Generation

---

# PHASE 17.3 — Daily Mission Architecture

Complexity:

Medium

Status:

Not Started

Goal:

Create mission framework.

Scope:

Mission Types

Mission Tracking

Mission Completion

Do Not Touch:

Ranking

Acceptance Criteria:

Mission system created.

GitHub Commit:

PHASE 17.3 Complete - Daily Missions Architecture

---

# PHASE 17.4 — Daily Rewards

Complexity:

Medium

Status:

Not Started

Goal:

Reward daily participation.

Scope:

Performance Score Rewards

Booster Rewards

Mission Rewards

Do Not Touch:

Progression

Acceptance Criteria:

Rewards granted correctly.

GitHub Commit:

PHASE 17.4 Complete - Daily Rewards

---

# PHASE 17.5 — Daily Content Validation

Complexity:

High

Status:

Not Started

Goal:

Validate daily systems.

Scope:

Daily Board

Daily Missions

Daily Rewards

Do Not Touch:

Global Ranking

Acceptance Criteria:

Daily content stable.

GitHub Commit:

PHASE 17.5 Complete - Daily Validation

---

# PHASE 18 — THEMES & SYMBOL LIBRARY

---

# PHASE 18.1 — Theme Architecture

Complexity:

Medium

Status:

Not Started

Goal:

Create theme framework.

Scope:

Theme Manager

Theme Data

Theme Selection

Do Not Touch:

Gameplay

Acceptance Criteria:

Themes switch correctly.

GitHub Commit:

PHASE 18.1 Complete - Theme Architecture

---

# PHASE 18.2 — Luxury Wood Theme

Complexity:

Medium

Status:

Not Started

Goal:

Implement Luxury Wood theme.

Scope:

Background

Board

UI Styling

Do Not Touch:

Gameplay

Acceptance Criteria:

Theme visually complete.

GitHub Commit:

PHASE 18.2 Complete - Luxury Wood Theme

---

# PHASE 18.3 — Bamboo Zen Theme

Complexity:

Medium

Status:

Not Started

Goal:

Implement Bamboo Zen theme.

Scope:

Visual Style

Board Style

UI Style

Do Not Touch:

Gameplay

Acceptance Criteria:

Theme visually complete.

GitHub Commit:

PHASE 18.3 Complete - Bamboo Zen Theme

---

# PHASE 18.4 — Premium Evening Theme

Complexity:

Medium

Status:

Not Started

Goal:

Implement Premium Evening theme.

Scope:

Visual Style

Board Style

UI Style

Do Not Touch:

Gameplay

Acceptance Criteria:

Theme visually complete.

GitHub Commit:

PHASE 18.4 Complete - Premium Evening Theme

---

# PHASE 18.5 — Symbol Library System

Complexity:

High

Status:

Not Started

Goal:

Create expandable symbol architecture.

Scope:

Animals

Fruits

Objects

Symbols

Symbol Selection Rules

Do Not Touch:

Tile Matching Rules

Acceptance Criteria:

Symbols selected automatically per level.

GitHub Commit:

PHASE 18.5 Complete - Symbol Library

---

# PHASE 18.6 — Theme & Symbol Validation

Complexity:

High

Status:

Not Started

Goal:

Validate visual systems.

Scope:

Themes

Symbols

Theme Switching

Automatic Symbol Selection

Do Not Touch:

Gameplay

Acceptance Criteria:

All visual systems stable.

GitHub Commit:

PHASE 18.6 Complete - Theme Validation

---

END OF PROJECT_ROADMAP.md PART 4
# PROJECT_ROADMAP.md

# PART 5 — PHASE 19, PHASE 20, PHASE 21, PHASE 22

---

# PHASE 19 — AUDIO, VFX, ANIMATION POLISH

---

# PHASE 19.1 — Audio Architecture

Complexity:

Medium

Status:

Not Started

Goal:

Create audio framework.

Scope:

AudioManager

Music Channels

SFX Channels

Audio Mixer Integration

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

Audio architecture functional.

GitHub Commit:

PHASE 19.1 Complete - Audio Architecture

---

# PHASE 19.2 — Menu Audio

Complexity:

Low

Status:

Not Started

Goal:

Create menu atmosphere.

Scope:

Main Menu Music

Button Sounds

Door Sounds

Do Not Touch:

Gameplay Audio

Acceptance Criteria:

Menu audio complete.

GitHub Commit:

PHASE 19.2 Complete - Menu Audio

---

# PHASE 19.3 — Gameplay Audio

Complexity:

Medium

Status:

Not Started

Goal:

Create gameplay soundscape.

Scope:

Tile Select

Match

Combo

Win

Lose

Timer Warning

Do Not Touch:

Board Generation

Acceptance Criteria:

Gameplay sounds functional.

GitHub Commit:

PHASE 19.3 Complete - Gameplay Audio

---

# PHASE 19.4 — VFX Foundation

Complexity:

Medium

Status:

Not Started

Goal:

Create visual effects framework.

Scope:

Match VFX

Combo VFX

Joker VFX

Win VFX

Lose VFX

Do Not Touch:

Gameplay Rules

Acceptance Criteria:

VFX framework functional.

GitHub Commit:

PHASE 19.4 Complete - VFX Foundation

---

# PHASE 19.5 — Tile Animation System

Complexity:

High

Status:

Not Started

Goal:

Implement DOTween animations.

Scope:

Tile Movement

Match Animation

Reveal Animation

Button Animation

Do Not Touch:

Game Logic

Acceptance Criteria:

Animations function correctly.

GitHub Commit:

PHASE 19.5 Complete - Tile Animation System

---

# PHASE 19.6 — Haptics Integration

Complexity:

Medium

Status:

Not Started

Goal:

Create feedback layer.

Scope:

Match Haptics

Combo Haptics

Win Haptics

Lose Haptics

Do Not Touch:

Scoring

Acceptance Criteria:

Haptics work correctly.

GitHub Commit:

PHASE 19.6 Complete - Haptics Integration

---

# PHASE 19.7 — Visual Polish Pass

Complexity:

High

Status:

Not Started

Goal:

Achieve premium casual feel.

Scope:

UI Polish

Board Polish

Transitions

Readability

Atmosphere

Do Not Touch:

Gameplay Systems

Acceptance Criteria:

Visual presentation meets design goals.

GitHub Commit:

PHASE 19.7 Complete - Visual Polish

---

# PHASE 20 — ANALYTICS, ADS, BACKEND SERVICES

---

# PHASE 20.1 — Firebase Analytics

Complexity:

Medium

Status:

Not Started

Goal:

Integrate analytics.

Scope:

Level Events

Performance Events

Booster Events

Daily Events

Do Not Touch:

Gameplay

Acceptance Criteria:

Analytics events received correctly.

GitHub Commit:

PHASE 20.1 Complete - Firebase Analytics

---

# PHASE 20.2 — Firebase Crashlytics

Complexity:

Medium

Status:

Not Started

Goal:

Integrate crash reporting.

Scope:

Crash Tracking

Exception Tracking

Error Reporting

Do Not Touch:

Gameplay

Acceptance Criteria:

Crash reports received.

GitHub Commit:

PHASE 20.2 Complete - Crashlytics

---

# PHASE 20.3 — Rewarded Ads

Complexity:

High

Status:

Not Started

Goal:

Implement rewarded ad flow.

Scope:

Shuffle Ads

Undo Ads

Hint Ads

Do Not Touch:

Booster Economy

Acceptance Criteria:

Rewarded ads function correctly.

GitHub Commit:

PHASE 20.3 Complete - Rewarded Ads

---

# PHASE 20.4 — Lose Screen Ads

Complexity:

Medium

Status:

Not Started

Goal:

Implement optional fail ads.

Scope:

Lose Screen Ad Offer

Reward Flow

Ad Validation

Do Not Touch:

Ranking

Acceptance Criteria:

Lose ads work correctly.

GitHub Commit:

PHASE 20.4 Complete - Lose Ads

---

# PHASE 20.5 — Global Ranking Backend

Complexity:

Critical

Status:

Not Started

Goal:

Finalize ranking infrastructure.

Scope:

Score Upload

Leaderboard Requests

Player Ranking

Do Not Touch:

Gameplay

Acceptance Criteria:

Ranking backend stable.

GitHub Commit:

PHASE 20.5 Complete - Ranking Backend

---

# PHASE 20.6 — Service Validation

Complexity:

Critical

Status:

Not Started

Goal:

Validate service integrations.

Scope:

Analytics

Crashlytics

Ads

Leaderboard

Do Not Touch:

Gameplay

Acceptance Criteria:

All services stable.

GitHub Commit:

PHASE 20.6 Complete - Service Validation

---

# PHASE 21 — TESTING & OPTIMIZATION

---

# PHASE 21.1 — Gameplay Testing

Complexity:

High

Status:

Not Started

Goal:

Test complete gameplay loop.

Scope:

Selection

Tray

Matching

Scoring

Progression

Do Not Touch:

Architecture

Acceptance Criteria:

Core gameplay stable.

GitHub Commit:

PHASE 21.1 Complete - Gameplay Testing

---

# PHASE 21.2 — Board Generation Testing

Complexity:

Critical

Status:

Not Started

Goal:

Stress test generation systems.

Scope:

Generation

Validation

Regeneration

Difficulty

Do Not Touch:

Scoring

Acceptance Criteria:

Generated boards meet standards.

GitHub Commit:

PHASE 21.2 Complete - Generation Testing

---

# PHASE 21.3 — Save System Testing

Complexity:

Critical

Status:

Not Started

Goal:

Validate persistence.

Scope:

Save

Load

Resume

Recovery

Do Not Touch:

Gameplay

Acceptance Criteria:

No progress loss.

GitHub Commit:

PHASE 21.3 Complete - Save Testing

---

# PHASE 21.4 — Performance Optimization

Complexity:

Critical

Status:

Not Started

Goal:

Reach production performance targets.

Scope:

FPS

Memory

Pooling

GC Reduction

Do Not Touch:

Gameplay Rules

Acceptance Criteria:

Stable 60 FPS on target devices.

GitHub Commit:

PHASE 21.4 Complete - Optimization

---

# PHASE 21.5 — Device Testing

Complexity:

High

Status:

Not Started

Goal:

Validate Android devices.

Scope:

Resolution Testing

Performance Testing

UI Testing

Touch Testing

Do Not Touch:

Architecture

Acceptance Criteria:

Game stable across supported devices.

GitHub Commit:

PHASE 21.5 Complete - Device Testing

---

# PHASE 21.6 — Final QA Pass

Complexity:

Critical

Status:

Not Started

Goal:

Production readiness validation.

Scope:

Gameplay

UI

Audio

Progression

Services

Do Not Touch:

Major Systems

Acceptance Criteria:

No critical blockers remain.

GitHub Commit:

PHASE 21.6 Complete - Final QA

---

# PHASE 22 — GOOGLE PLAY RELEASE PREPARATION

---

# PHASE 22.1 — Build Pipeline

Complexity:

Medium

Status:

Not Started

Goal:

Create release build pipeline.

Scope:

Android Build Settings

Release Configuration

Versioning

Do Not Touch:

Gameplay

Acceptance Criteria:

Release APK/AAB generated.

GitHub Commit:

PHASE 22.1 Complete - Build Pipeline

---

# PHASE 22.2 — Store Assets

Complexity:

Medium

Status:

Not Started

Goal:

Prepare store content.

Scope:

App Icon

Feature Graphic

Screenshots

Store Description

Do Not Touch:

Game Code

Acceptance Criteria:

Store assets complete.

GitHub Commit:

PHASE 22.2 Complete - Store Assets

---

# PHASE 22.3 — Privacy & Compliance

Complexity:

High

Status:

Not Started

Goal:

Meet platform requirements.

Scope:

Privacy Policy

Ads Disclosure

Analytics Disclosure

Permissions Review

Do Not Touch:

Gameplay

Acceptance Criteria:

Compliance requirements satisfied.

GitHub Commit:

PHASE 22.3 Complete - Compliance

---

# PHASE 22.4 — Release Candidate Build

Complexity:

Critical

Status:

Not Started

Goal:

Create final candidate build.

Scope:

Production Build

Final Validation

Release Checklist

Do Not Touch:

Core Systems

Acceptance Criteria:

Release candidate approved.

GitHub Commit:

PHASE 22.4 Complete - Release Candidate

---

# PHASE 22.5 — Google Play Launch

Complexity:

Critical

Status:

Not Started

Goal:

Publish the game.

Scope:

Store Submission

Review Process

Launch Monitoring

Post Launch Validation

Do Not Touch:

Architecture

Acceptance Criteria:

Game successfully published.

GitHub Commit:

PHASE 22.5 Complete - Google Play Launch

---

# ROADMAP COMPLETION CONDITION

The roadmap is considered complete only when:

All phases marked Complete

All acceptance criteria passed

No critical blockers remain

Release build approved

Google Play launch completed

---

END OF PROJECT_ROADMAP.md
END OF PART 5
