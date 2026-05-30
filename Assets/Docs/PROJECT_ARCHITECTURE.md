# PROJECT_ARCHITECTURE.md

# PART 1 — HIGH LEVEL ARCHITECTURE, SCENES, HIERARCHY, FOLDERS

---

# 1. ARCHITECTURE PURPOSE

This document defines how the game must be built inside Unity.

PROJECT_BIBLE.md defines what the game is.

PROJECT_ARCHITECTURE.md defines how the game is implemented.

This document exists to prevent:

* Duplicate systems
* Conflicting managers
* Legacy runtime generators
* Broken scene structure
* Unclear script responsibilities
* Uncontrolled runtime spawning

---

# 2. ARCHITECTURE PHILOSOPHY

The project uses a clean director-based architecture.

Each major gameplay domain must have one owner.

Every system must have a clear responsibility.

No runtime system may secretly duplicate another system.

No temporary system may become part of production gameplay.

---

# 3. AUTHORITY HIERARCHY

If documents conflict, follow this order:

1. PROJECT_DECISIONS.md
2. PROJECT_BIBLE.md
3. PROJECT_ARCHITECTURE.md
4. CURSOR_RULES.md
5. DESIGN_BIBLE.md
6. PROJECT_ROADMAP.md

PROJECT_DECISIONS.md has the highest decision authority.

PROJECT_BIBLE.md has the highest gameplay and design authority.

PROJECT_ARCHITECTURE.md has the highest implementation authority.

---

# 4. SINGLE SOURCE OF TRUTH RULE

Every gameplay system must have exactly one owner.

Duplicate ownership is forbidden.

Example:

Tray state is owned only by TrayController.

Match logic is owned only by MatchController.

Board generation is owned only by BoardGenerationPipeline.

League state is owned only by LeagueDirector.

Booster economy is owned only by BoosterEconomyDirector.

---

# 5. DIRECTOR-BASED ARCHITECTURE

The project uses directors for high-level orchestration.

Directors coordinate systems.

Directors do not replace specialized controllers.

A director may request work from another system.

A director should not secretly perform another system's responsibility.

---

# 6. SCENE ARCHITECTURE

The project uses three major scenes:

BootScene

MainMenuScene

GameScene

---

# 7. BOOTSCENE

## Purpose

BootScene initializes global systems.

BootScene is lightweight.

BootScene should not contain gameplay board logic.

---

## Responsibilities

BootScene may initialize:

* GameBootstrapper
* SaveSystem
* SceneLoadController
* AudioManager
* Global configuration

---

## Restrictions

BootScene may not:

* Spawn gameplay tiles
* Generate boards
* Start levels directly
* Contain test board generators

---

# 8. MAINMENUSCENE

## Purpose

MainMenuScene presents the game entry point.

The player sees the closed-door style main menu.

The player can start the current level from here.

---

## Required Hierarchy

MainMenuScene

Canvas_MainMenu

DoorPanel

LevelButton

LeagueButton

SettingsButton

PlayerInfoPanel

MainMenuDirector

PlayerProgressionDirector

AudioManager

---

## Responsibilities

MainMenuScene handles:

* Door visual
* Level button
* League button
* Settings button
* Player progress display
* Entry into GameScene

---

## Restrictions

MainMenuScene may not:

* Generate runtime board tiles
* Own gameplay tray state
* Own match logic
* Own board generation logic

---

# 9. GAMESCENE

## Purpose

GameScene contains the active gameplay session.

All board, tray, timer, score, booster, and result systems live here.

---

## Required Hierarchy

GameScene

GameplayRoot

BoardRoot

TrayRoot

BoosterRoot

TimerRoot

VFXRoot

SessionDirector

BoardGenerationPipeline

TileSelectionController

TrayController

MatchController

ScoreController

ComboController

TimerController

BoosterController

LevelResultController

---

## Responsibilities

GameScene handles:

* Starting a level session
* Generating the board
* Spawning tiles
* Player tile selection
* Tray management
* Matching
* Timer
* Score
* Combo
* Boosters
* Win/Lose resolution
* Performance screen

---

# 10. GAMEPLAYROOT STRUCTURE

GameplayRoot is the parent object for all runtime gameplay visuals.

---

## Required Children

BoardRoot

TrayRoot

BoosterRoot

TimerRoot

VFXRoot

UIRoot

---

## BoardRoot

BoardRoot contains all spawned runtime board tiles.

Only BoardSpawner may instantiate children under BoardRoot.

---

## TrayRoot

TrayRoot contains tray slots and tray visuals.

TrayController owns tray state.

---

## BoosterRoot

BoosterRoot contains booster buttons and booster UI.

BoosterController coordinates booster usage.

BoosterEconomyDirector owns booster rights.

---

## TimerRoot

TimerRoot contains timer UI.

TimerController owns timer state.

---

## VFXRoot

VFXRoot contains runtime visual effects.

VFX should use pooling where possible.

---

# 11. FOLDER ARCHITECTURE

The Unity project must use a clean folder structure.

---

## Required Folder Structure

Assets/

Scripts/

Core/

Progression/

Session/

Board/

BoardGeneration/

Tiles/

Tray/

Matching/

ClosedTiles/

Rewards/

Boosters/

League/

UI/

Audio/

VFX/

Debug/

Prefabs/

Tiles/

Board/

UI/

VFX/

ScriptableObjects/

Levels/

Archetypes/

Variations/

HolePatterns/

TileSets/

Leagues/

Audio/

Scenes/

Materials/

Sprites/

---

# 12. SCRIPT FOLDER RESPONSIBILITIES

## Core

Global systems.

Examples:

GameBootstrapper

SceneLoadController

GameState

GameEvents

SaveSystem

---

## Progression

Player progression and persistent level data.

Examples:

PlayerProgressionDirector

PlayerProgressData

LevelProgressData

---

## Session

Active level session control.

Examples:

SessionDirector

SessionData

LevelStartController

LevelEndController

---

## BoardGeneration

All board generation pipeline systems.

Examples:

LevelRecipe

LevelRecipeGenerator

DifficultyDirector

VisualVarietyDirector

BoardGenerationPipeline

GridMaskGenerator

ArchetypeSelector

VariationSelector

HolePatternSelector

LayerBuilder

TilePairDistributor

ClosedTilePatternSelector

RewardJokerPatternSelector

OpeningMoveChecker

DeadlockRiskChecker

BoardQualityChecker

BoardSpawner

---

## Tiles

Tile state, tile data, tile visuals, tile selectability.

Examples:

Tile

TileData

TileView

TileState

TileType

TileSelectionController

TileSelectabilityChecker

TileMovementController

TileSortingController

---

## Tray

Tray slots and tray state.

Examples:

TrayController

TraySlot

TrayMatchChecker

TrayView

---

## Matching

Match execution and match feedback.

Examples:

MatchController

MatchResult

MatchVFXController

---

## ClosedTiles

Closed tile reveal and behavior systems.

Examples:

ClosedTileController

ClosedTileState

ClosedTileRevealController

---

## Rewards

Reward and scoring-related systems.

Examples:

RewardDirector

JokerTileController

JokerTimerController

ScoreController

TimeBonusController

ComboController

---

## Boosters

Booster behavior and booster economy systems.

Examples:

BoosterEconomyDirector

BoosterController

ShuffleBooster

UndoBooster

HintBooster

RewardedAdBoosterController

---

## League

League and leaderboard systems.

Examples:

LeagueDirector

LeagueData

LeagueRankEntry

LeagueUIController

LeagueTimerController

---

## UI

Menu, HUD, performance screen, settings.

Examples:

MainMenuUIController

GameHUDController

TimerUIController

BoosterUIController

PerformanceScreenController

SettingsUIController

---

## Debug

Development-only debug tools.

Debug tools must not be active in release builds.

---

# 13. RUNTIME ROOT OWNERSHIP

Runtime objects must be placed under the correct root.

Board tiles:

BoardRoot

Tray UI:

TrayRoot

Booster UI:

BoosterRoot

Timer UI:

TimerRoot

VFX:

VFXRoot

Debug UI:

DebugRoot

---

# 14. PERFORMANCE TARGETS

Target FPS:

60 FPS

---

Target Devices:

Mid-Range Android Devices and Above

---

Orientation:

Portrait Only

---

Optimization Strategy:

* Tile Pool
* VFX Pool
* Efficient Board Generation
* Minimal Runtime Allocations
* Object Reuse Wherever Possible

---

# 15. DEVELOPMENT BUILD DEBUG POLICY

Debug tools may exist only for development builds.

Debug tools must be disabled in release builds.

---

## Debug Panel Should Display

Current Level

Current Seed

Tile Count

Layer Count

Closed Tile Count

Current Archetype

Current Variation

Current Hole Pattern

Current Tile Set

Current League

Current FPS

---

# 16. PART 1 SUMMARY

This architecture establishes:

* One clear scene structure
* One clear folder structure
* Director-based ownership
* Single source of truth
* Runtime root ownership
* Debug policy
* Performance targets

All future systems must follow this structure.

END OF PART 1
# PROJECT_ARCHITECTURE.md

# PART 2 — DIRECTOR ARCHITECTURE, OWNERSHIP MATRIX, SAVE, ONLINE, SEED SYSTEM

---

# 17. DIRECTOR ARCHITECTURE

## Purpose

Directors coordinate major gameplay domains.

Directors are orchestration systems.

Directors should not secretly duplicate controller logic.

---

## Director Rule

A director may coordinate multiple systems.

A director may not replace a specialized controller.

Example:

SessionDirector may start a level.

SessionDirector may not directly manage tray slots.

Tray slots are owned by TrayController.

---

# 18. PLAYERPROGRESSIONDIRECTOR

## Responsibility

PlayerProgressionDirector owns long-term player progress.

---

## Owns

Current Level

Highest Level

Progression State

Player Statistics

Selected Tile Set

---

## Does Not Own

Runtime board state

Tray state

Match logic

Board generation logic

---

# 19. SESSIONDIRECTOR

## Responsibility

SessionDirector owns the active level session.

---

## Owns

Current session state

Level start flow

Level end flow

Win/Lose resolution

SessionData

---

## Coordinates

BoardGenerationPipeline

TimerController

ScoreController

ComboController

LevelResultController

---

## Does Not Own

Board generation internals

Tile spawning

Tray slots

League state

---

# 20. DIFFICULTYDIRECTOR

## Responsibility

DifficultyDirector calculates level difficulty.

---

## Owns

Difficulty profile

Tile count range

Layer depth

Closed tile count

Joker count

Timer recommendation

---

## Uses

Current Level

LevelRecipe rules

Difficulty curve

---

## Does Not Own

Tile spawning

Board visuals

Player progress

---

# 21. VISUALVARIETYDIRECTOR

## Responsibility

VisualVarietyDirector controls layout variety.

---

## Owns

Archetype selection

Variation selection

Hole Pattern selection

Closed Tile Pattern selection

---

## Goal

Avoid repetitive layouts.

Maintain production readability.

---

## Does Not Own

Difficulty math

Tile spawning

Match rules

---

# 22. LEAGUEDIRECTOR

## Responsibility

LeagueDirector owns league and leaderboard state.

---

## Owns

Current League

League Score

Rank Position

Promotion State

Demotion State

League Timers

Online leaderboard sync

---

## Does Not Own

Gameplay score

Combo score

Board generation

---

# 23. REWARDDIRECTOR

## Responsibility

RewardDirector coordinates reward systems.

---

## Owns

Reward joker configuration

Reward joker session behavior

Reward timing rules

---

## Coordinates

JokerTileController

JokerTimerController

ScoreController

---

## Does Not Own

Board generation

Tile spawning

League score

---

# 24. BOOSTERECONOMYDIRECTOR

## Responsibility

BoosterEconomyDirector owns booster rights.

---

## Owns

Shuffle count

Undo count

Hint count

Booster reward progression

Rewarded ad booster grants

---

## Does Not Own

Booster execution logic

Tray state

Board generation

---

# 25. OWNERSHIP MATRIX

## Player Progress

Owner:

PlayerProgressionDirector

---

## Session State

Owner:

SessionDirector

---

## Difficulty

Owner:

DifficultyDirector

---

## Visual Variety

Owner:

VisualVarietyDirector

---

## Board Generation

Owner:

BoardGenerationPipeline

---

## Runtime Tile Spawning

Owner:

BoardSpawner

---

## Tile State

Owner:

Tile

---

## Tile Selectability

Owner:

TileSelectabilityChecker

---

## Tile Movement

Owner:

TileMovementController

---

## Tray State

Owner:

TrayController

---

## Matching

Owner:

MatchController

---

## Score

Owner:

ScoreController

---

## Combo

Owner:

ComboController

---

## Timer

Owner:

TimerController

---

## Closed Tiles

Owner:

ClosedTileController

---

## Reward Jokers

Owner:

RewardDirector

---

## Booster Rights

Owner:

BoosterEconomyDirector

---

## Booster Execution

Owner:

BoosterController

---

## League State

Owner:

LeagueDirector

---

## Save Data

Owner:

SaveSystem

---

# 26. SAVE SYSTEM ARCHITECTURE

## Purpose

SaveSystem persists player progress and recoverable gameplay state.

---

## Required Saved Data

Current Level

Highest Level

League

League Score

Booster Counts

Settings

Audio Settings

Statistics

Selected Tile Set

Active Level State

---

## Active Level State Rule

If the player closes the app during gameplay:

The player must return to the same level state.

---

## Active Level State Should Include

Current Level

Current Seed

Generated Board State

Remaining Timer

Tray State

Score

Combo State

Closed Tile Reveal State

Used Boosters

Matched Tiles

Remaining Tiles

---

## Save Restriction

SaveSystem stores data.

SaveSystem does not generate boards.

SaveSystem does not resolve matches.

SaveSystem does not modify league logic directly.

---

# 27. OFFLINE / ONLINE ARCHITECTURE

## Gameplay Mode

Gameplay is offline.

---

## Offline Systems

Board Generation

Player Progression

Level Play

Score

Combo

Boosters

Settings

Tile Set Selection

---

## Online Systems

League

Leaderboard

Cloud ranking sync

Promotion and demotion validation

---

## Hybrid Rule

The game must remain playable offline.

Online features may sync when connection is available.

---

# 28. SEED SYSTEM ARCHITECTURE

## Purpose

Levels must be deterministic.

The same level number must generate the same board family for every player.

---

## Seed Rule

Level Number

↓

Deterministic Seed

↓

LevelRecipe

↓

Board Generation

---

## Fairness Rule

Level 150 must represent the same challenge for all players.

This supports fair league and leaderboard comparison.

---

## Debug Benefit

Deterministic seeds make bugs easier to reproduce.

If Level 428 has a generation issue:

The same seed can be tested again.

---

# 29. JOKER COUNT ARCHITECTURE

## Rule

Joker count scales with level progression.

---

## Low Levels

1 Joker

---

## Mid Levels

1-2 Jokers

---

## High Levels

2-3 Jokers

---

## Joker Reward Rule

If a joker pair is matched within 1 minute:

Bonus score is awarded.

---

# 30. SCRIPTABLEOBJECT ARCHITECTURE

## Purpose

ScriptableObjects store configurable production data.

They keep the project editable without hardcoding every rule.

---

## Required ScriptableObjects

ArchetypeSO

VariationSO

HolePatternSO

TileSetSO

LeagueSO

LevelRecipeSO

---

## ArchetypeSO

Defines board identity and occupancy behavior.

---

## VariationSO

Defines variation behavior for an archetype.

---

## HolePatternSO

Defines controlled empty cell patterns.

---

## TileSetSO

Defines tile visuals and symbol set.

---

## LeagueSO

Defines league configuration.

---

## LevelRecipeSO

Defines generated or fixed level recipe data.

---

## ScriptableObject Rule

ScriptableObjects may define data.

ScriptableObjects may not secretly execute runtime gameplay ownership.

---

# 31. POOLING ARCHITECTURE

## Purpose

Pooling improves runtime performance.

---

## Required Pools

Tile Pool

VFX Pool

---

## Tile Pool

Used for runtime tile creation and reuse.

---

## VFX Pool

Used for match effects, combo effects, joker effects, and win/lose feedback.

---

## Pooling Rule

Pooling must not change gameplay behavior.

Pooling exists only for performance.

---

# 32. BOARD REGENERATION ARCHITECTURE

## Purpose

Board regeneration prevents bad boards from reaching the player.

---

## Flow

Generate Candidate Board

↓

BoardQualityChecker

↓

If Valid → BoardSpawner

↓

If Invalid → Regenerate

---

## Maximum Attempts

Max Regeneration Attempts = 50

---

## Emergency Fallback

If no valid board is found after 50 attempts:

Use Emergency Fallback Recipe.

---

## Fallback Purpose

Emergency Fallback Recipe must generate a safer, simpler, solvable board.

---

# 33. PART 2 SUMMARY

This section establishes:

* Director ownership
* Single responsibility per system
* Save architecture
* Offline/online architecture
* Deterministic seed system
* ScriptableObject strategy
* Pooling strategy
* Board regeneration safety

END OF PART 2
# PROJECT_ARCHITECTURE.md

# PART 3 — BOARD GENERATION ARCHITECTURE

---

# 34. BOARD GENERATION PURPOSE

Board generation is the most important technical system in the project.

Its purpose is to create playable, readable, fair, layered Mahjong-style boards using the official runtime pipeline.

Board generation must always respect:

* Base Grid = 6x7
* Maximum Tile Count = 140
* Maximum Layer Depth = 4
* Single Runtime Pipeline
* BoardSpawner-only tile instantiation
* BoardQualityChecker validation

---

# 35. OFFICIAL BOARD GENERATION PIPELINE

This is the only valid runtime board generation flow.

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

Base Grid

↓

GridMask

↓

Archetype

↓

Variation

↓

HolePattern

↓

LayerBuilder

↓

TilePairDistributor

↓

ClosedTilePatternSelector

↓

RewardJokerPatternSelector

↓

RewardDirector

↓

BoosterEconomyDirector

↓

OpeningMoveChecker

↓

DeadlockRiskChecker

↓

BoardQualityChecker

↓

If Bad → Regenerate

↓

BoardSpawner

---

# 36. PIPELINE PROTECTION RULE

No script, prefab, debug tool, editor helper, legacy generator, test generator, temporary generator, or third-party framework may spawn or modify runtime board tiles outside this flow.

BoardSpawner is the only system allowed to instantiate runtime board tiles.

---

# 37. BOARDGENERATIONPIPELINE

## Responsibility

BoardGenerationPipeline owns the complete board generation process.

It coordinates the generation steps.

It does not directly decide every detail.

It calls specialized systems in the correct order.

---

## Owns

Generation flow

Generation attempts

Validation loop

Emergency fallback use

Final BoardData output

---

## Does Not Own

Player progress

Runtime session state

Tile movement

Tray state

Match logic

Score logic

---

## Required Flow

1. Receive LevelRecipe request from SessionDirector.
2. Ask LevelRecipeGenerator for level recipe.
3. Generate candidate board.
4. Validate candidate board.
5. Regenerate if invalid.
6. Use emergency fallback if needed.
7. Send final valid BoardData to BoardSpawner.

---

## Regeneration Rule

Maximum attempts:

50

If no valid board is produced after 50 attempts:

Use Emergency Fallback Recipe.

---

# 38. LEVELRECIPEGENERATOR

## Responsibility

LevelRecipeGenerator creates the recipe for a level.

A recipe defines what should be generated.

It does not spawn tiles.

---

## Inputs

Current Level

Deterministic Seed

Difficulty Profile

Visual Variety Rules

Player Progression Data

---

## Outputs

LevelRecipe

---

## LevelRecipe Must Include

Level Number

Seed

Tile Count

Layer Depth

Archetype

Variation

Hole Pattern

Closed Tile Count

Closed Tile Pattern

Joker Count

Reward Joker Pattern

Timer Recommendation

Difficulty Rating

Max Regeneration Attempts

---

## Deterministic Rule

The same level number must generate the same recipe for every player.

---

# 39. BASE GRID

## Responsibility

Base Grid defines the coordinate system.

---

## Rule

Base Grid is always 6x7.

Columns = 6

Rows = 7

Total Cells = 42

---

## Restrictions

Base Grid may not be changed by:

Archetype

Variation

HolePattern

LayerBuilder

DifficultyDirector

Any runtime system

---

# 40. GRIDMASK

## Responsibility

GridMask defines which base grid cells are eligible for occupation.

---

## Works On

6x7 Base Grid only.

---

## May Do

Enable cells

Disable cells

Create controlled playable area

---

## May Not Do

Change grid size

Create new coordinates

Move cells

Offset cells

Spawn tiles

---

# 41. ARCHETYPE

## Responsibility

Archetype defines board identity.

It describes the high-level occupancy style.

---

## Examples

Diamond

Oval

Pyramid

Tower

Cross

Bridge

Island

Maze

---

## Rule

Archetype changes occupancy.

Archetype does not change the coordinate system.

---

## May Do

Choose cell groups

Influence layer distribution

Define silhouette logic

Define shape identity

---

## May Not Do

Spawn tiles

Assign tile symbols

Change match rules

Change tray rules

Move grid coordinates

---

# 42. VARIATION

## Responsibility

Variation creates diversity inside an archetype.

---

## Rule

Archetype defines identity.

Variation defines diversity.

---

## May Do

Modify density

Modify occupied cells

Modify layer spread

Modify shape details

Change layout flavor

---

## May Not Do

Change gameplay rules

Change board size

Change coordinate system

Spawn tiles

Assign symbols

---

# 43. HOLEPATTERN

## Responsibility

HolePattern creates controlled empty spaces.

---

## Purpose

Improve variety.

Improve readability.

Create strategic openings.

---

## May Do

Remove selected occupied cells

Create gaps

Shape board flow

---

## May Not Do

Destroy board readability

Create impossible layouts

Remove all opening choices

Spawn tiles

Change tile symbols

---

# 44. LAYERBUILDER

## Responsibility

LayerBuilder creates stacked tile positions.

---

## Input

GridMask

Archetype

Variation

HolePattern

Layer Depth

Tile Count Target

---

## Output

Layered Tile Positions

---

## Rules

Layers stack on existing grid coordinates.

Layers never create new coordinates.

All layers remain aligned to grid centers.

Maximum Layer Depth = 4.

---

## May Do

Create layer stacks

Assign layer index

Control vertical depth

Control tile position list

---

## May Not Do

Assign tile symbols

Spawn runtime tiles

Break grid alignment

Move tiles outside valid coordinates

---

# 45. TILEPAIRDISTRIBUTOR

## Responsibility

TilePairDistributor assigns tile symbols to generated positions.

---

## Input

Layered Tile Positions

Tile Set Data

Tile Count

Seed

---

## Output

Tile symbol assignments

---

## Rules

All tile symbols must appear in valid pairs.

No unmatched symbol count is allowed.

Tile count must remain even.

---

## May Do

Assign symbols

Distribute pairs

Reduce bad clustering

Support selected TileSetSO

---

## May Not Do

Move positions

Change layer structure

Spawn tiles

Change board shape

---

# 46. CLOSEDTILEPATTERNSELECTOR

## Responsibility

ClosedTilePatternSelector selects which tiles become closed tiles.

---

## Input

LevelRecipe

Layered BoardData

Closed Tile Count

Closed Tile Pattern Rules

---

## Output

Closed Tile Assignments

---

## Rules

Closed tiles begin after Level 10.

Closed tile count follows progression curve.

Closed tiles may create uncertainty.

Closed tiles may not block opening play.

---

## May Do

Select tiles for closed state

Apply closed tile patterns

Balance closed tile placement

---

## May Not Do

Create impossible boards

Block all meaningful opening choices

Change tile symbols

Spawn tiles

---

# 47. REWARDJOKERPATTERNSELECTOR

## Responsibility

RewardJokerPatternSelector selects reward joker placements.

---

## Input

LevelRecipe

BoardData

Joker Count

Reward Rules

---

## Output

Reward Joker Assignments

---

## Joker Count Scaling

Low Levels:

1 Joker

Mid Levels:

1-2 Jokers

High Levels:

2-3 Jokers

---

## Rules

Jokers must be realistically reachable.

Jokers must not be placed in unfairly inaccessible positions.

---

## May Do

Mark tiles as reward jokers

Balance joker accessibility

Support 1-minute bonus logic

---

## May Not Do

Spawn tiles

Change board shape

Change tile pair rules

Guarantee free rewards

---

# 48. OPENINGMOVECHECKER

## Responsibility

OpeningMoveChecker validates opening playability.

---

## Required Condition

Every board must provide at least two meaningful opening choices.

---

## Checks

Selectable tiles at start

Available matching opportunities

Closed tile opening fairness

First move variety

---

## Failure Result

Reject board.

Regenerate.

---

# 49. DEADLOCKRISKCHECKER

## Responsibility

DeadlockRiskChecker estimates early deadlock risk.

---

## Checks

Selectable tile count

Pair accessibility

Layer blockage

Closed tile pressure

Board density

---

## Purpose

Prevent boards that can become unfairly stuck too early.

---

## Failure Result

Reject board.

Regenerate.

---

# 50. BOARDQUALITYCHECKER

## Responsibility

BoardQualityChecker is the final board authority.

It decides if a candidate board is production-quality.

---

## Authority Rule

BoardQualityChecker has authority over BoardGenerationPipeline.

A technically valid board is not necessarily a production-quality board.

---

## Required Checks

Grid Integrity

Layer Integrity

Tile Pair Validity

Opening Move Validity

Selectable Tile Count

Closed Tile Fairness

Joker Accessibility

Deadlock Risk

Visual Silhouette

Density

---

## Recommended Minimums

Starting selectable tiles:

At least 8

Opening meaningful choices:

At least 2

---

## Failure Result

Reject board.

Regenerate.

No exceptions.

---

# 51. BOARDSPAWNER

## Responsibility

BoardSpawner instantiates runtime tiles into the scene.

---

## Input

Validated BoardData

Tile Prefab

BoardRoot

Tile Pool

TileSetSO

---

## Rules

BoardSpawner may only spawn validated boards.

BoardSpawner must not generate board logic.

BoardSpawner must not decide layout.

BoardSpawner must not assign symbols independently.

---

## Only Allowed Runtime Tile Spawner

BoardSpawner is the only system allowed to instantiate runtime board tiles.

---

## Output

Runtime Tile GameObjects

---

# 52. BOARDDATA STRUCTURE

## Purpose

BoardData is the final generated board model before spawning.

---

## Must Contain

Level Number

Seed

Tile Positions

Layer Indexes

Tile Symbols

Closed Tile Flags

Reward Joker Flags

Archetype Id

Variation Id

Hole Pattern Id

Tile Count

Closed Tile Count

Joker Count

Validation Result

---

## Rule

BoardData is data.

BoardData does not execute gameplay logic.

---

# 53. EMERGENCY FALLBACK RECIPE

## Purpose

Emergency Fallback Recipe prevents generation failure loops.

---

## Trigger

Used only when BoardGenerationPipeline fails to create a valid board after 50 attempts.

---

## Fallback Board Characteristics

Simpler archetype

Lower complexity

Safe opening choices

Reduced deadlock risk

Readable silhouette

Valid tile pairs

---

## Restriction

Emergency fallback must still obey:

6x7 grid

Max 140 tiles

Max 4 layers

BoardQualityChecker validation

---

# 54. PART 3 SUMMARY

Board generation must be deterministic, validated, readable, and fair.

Only BoardGenerationPipeline may coordinate generation.

Only BoardSpawner may instantiate runtime tiles.

All boards must pass BoardQualityChecker before appearing in the game.

END OF PART 3
# PROJECT_ARCHITECTURE.md

# PART 4 — GAMEPLAY SYSTEM ARCHITECTURE

---

# 55. GAMEPLAY ARCHITECTURE PURPOSE

Gameplay systems define how the player interacts with the generated board.

BoardGenerationPipeline creates a valid board.

Gameplay systems make that board playable.

Gameplay systems must not modify board generation rules.

---

# 56. TILE SYSTEM

## Responsibility

Tile system owns individual tile state, visuals, and interaction readiness.

---

## Core Scripts

Tile

TileData

TileView

TileState

TileType

TileSelectionController

TileSelectabilityChecker

TileMovementController

TileSortingController

---

## Tile Must Know

Tile Id

Grid Coordinate

Layer Index

Tile Type

Closed State

Reward Joker State

Current State

Original Board Position

---

## Tile States

OnBoard

MovingToTray

InTray

Matched

Closed

Revealed

Disabled

---

# 57. TILE SELECTABILITY SYSTEM

## Responsibility

TileSelectabilityChecker decides if a tile can be selected.

---

## Selection Rules

A tile may be selected only if:

* It is on the board
* It is not already selected
* It is not in tray
* It is not matched
* It is not blocked by an upper tile
* Side/lower blocking rules allow selection

---

## Visual Rule

Selectable and unselectable tiles must not visually change color, glow, or opacity.

---

# 58. TILE SELECTION FLOW

Player taps tile

↓

TileSelectionController checks tile state

↓

TileSelectabilityChecker validates selection

↓

ClosedTileController handles reveal if closed

↓

TileMovementController moves tile to tray if allowed

↓

TrayController receives tile

---

# 59. TRAY SYSTEM

## Responsibility

TrayController owns tray state.

---

## Tray Rule

Tray Capacity = 4

Match Requirement = 2 identical tiles

Matching occurs only inside tray.

---

## TrayController Owns

Tray slots

Tray tile list

Tray capacity

Tray overflow detection

---

## TrayController Does Not Own

Board generation

Tile spawning

Score calculation

League score

---

# 60. MATCH SYSTEM

## Responsibility

MatchController owns match execution.

---

## Match Rule

If two identical tile types exist in tray:

Wait approximately 0.3 seconds.

Then remove matched pair.

Play VFX/SFX.

Notify score and combo systems.

---

## MatchController Owns

Match detection

Match delay

Match execution

Match result dispatch

---

## MatchController Does Not Own

Tray capacity

Board generation

League score

Booster economy

---

# 61. LOSE CONDITIONS

Level fails if:

* Tray reaches 4 tiles without a valid match
* Timer reaches zero

Lose flow is handled by SessionDirector.

---

# 62. WIN CONDITION

Level completes if:

* All board tiles are matched and removed
* No pending tray match is unresolved

Win flow is handled by SessionDirector.

---

# 63. CLOSED TILE SYSTEM

## Responsibility

ClosedTileController owns closed tile behavior.

---

## Rules

Closed tiles activate after Level 10.

First tap reveals the symbol.

Second valid tap sends tile to tray.

If another tile is selected before the revealed closed tile is moved:

The revealed closed tile may close again.

---

## ClosedTileController Does Not Own

Board generation

Closed tile placement

Score

League score

---

# 64. REWARD JOKER SYSTEM

## Responsibility

RewardDirector coordinates reward joker logic.

---

## Rule

Reward joker bonus is granted if matched within 1 minute.

---

## Joker Count

Low levels:

1 Joker

Mid levels:

1-2 Jokers

High levels:

2-3 Jokers

---

## RewardDirector Does Not Own

Board generation

Tile spawning

League score

---

# 65. SCORE SYSTEM

## Responsibility

ScoreController owns gameplay score.

---

## Base Score

Every successful pair match:

+100 points

---

## ScoreController Owns

Gameplay score

Match score

Joker score bonus

Score display events

---

## ScoreController Does Not Own

League score

Board generation

Timer

---

# 66. COMBO SYSTEM

## Responsibility

ComboController owns combo state.

---

## Combo Window

3 seconds

---

## Combo Rewards

Combo x1 = 100

Combo x2 = 120

Combo x3 = 140

Combo x4 = 160

Combo x5 = 180

Combo x6+ = 200

---

## Combo Cap

Maximum match value:

200

---

## ComboController Owns

Current combo

Highest combo

Total combo count

Combo timer

---

## ComboController Does Not Own

League score

Board generation

Tray capacity

---

# 67. TIMER SYSTEM

## Responsibility

TimerController owns active level timer.

---

## Timer Source

DifficultyDirector provides recommended level time.

---

## TimerController Owns

Remaining time

Timer start

Timer pause

Timer resume

Timer expiration event

---

## Timer Expiration

If timer reaches zero:

SessionDirector triggers level fail.

LeagueDirector applies failure penalty.

Same level restarts.

---

# 68. LEAGUE SCORE INTEGRATION

## Responsibility

LeagueDirector owns league score.

---

## League Score Rule

League score is based on completion percentage of allocated level time.

---

## Rewards

Complete within 40% of level time:

+5 league points

---

Complete within 60% of level time:

+3 league points

---

Complete within 80% of level time:

+2 league points

---

Complete level:

+1 league point

---

Timer fail:

-5 league points

---

## League Score Independence

Gameplay score and league score are separate.

Combo does not affect league score.

---

# 69. BOOSTER SYSTEM

## Responsibility

BoosterController executes booster behavior.

BoosterEconomyDirector owns booster rights.

---

## Approved Boosters

Shuffle

Undo

Hint

---

## Booster Rights

Every 10 completed levels:

Shuffle +1

Undo +1

Hint +1

---

# 70. SHUFFLE BOOSTER ARCHITECTURE

## Purpose

Shuffle redistributes symbols among active board tiles.

---

## Rule

Shuffle may change tile symbols.

Shuffle may not move tile positions.

Shuffle may not change layers.

Shuffle may not regenerate the board.

---

# 71. UNDO BOOSTER ARCHITECTURE

## Purpose

Undo reverses the last valid tray move.

---

## Rule

Undo returns the most recently selected tile from tray to its original board position.

Undo must restore original tile state correctly.

---

# 72. HINT BOOSTER ARCHITECTURE

## Purpose

Hint suggests a playable match or useful move.

---

## Rule

Hint must not solve the board automatically.

Hint visual must remain subtle.

---

# 73. REWARDED AD BOOSTER FLOW

If booster count is zero:

Player taps booster

↓

RewardedAdBoosterController offers rewarded ad

↓

Player watches ad

↓

BoosterEconomyDirector grants +1 right

↓

BoosterController may execute booster

---

# 74. PERFORMANCE SCREEN ARCHITECTURE

## Responsibility

LevelResultController and PerformanceScreenController display post-level results.

---

## Required Data

Completion Time

Gameplay Score

League Score Earned

Joker Bonus

Time Bonus

Total Combo Count

Highest Combo

Next Level Button

---

# 75. GAMEPLAY EVENT FLOW

Player selects tile

↓

TileSelectionController

↓

TileSelectabilityChecker

↓

TileMovementController

↓

TrayController

↓

MatchController

↓

ScoreController

↓

ComboController

↓

SessionDirector

↓

LevelResultController

---

# 76. GAMEPLAY RESTRICTIONS

Gameplay systems may not:

* Generate boards
* Spawn runtime board tiles
* Modify grid rules
* Modify level recipes
* Change archetype logic
* Change variation logic
* Change hole pattern logic
* Bypass BoardQualityChecker

---

# 77. PART 4 SUMMARY

Gameplay systems must consume validated boards.

Gameplay systems must not generate or alter board architecture.

Tray, match, score, combo, timer, league, and booster systems must each have one clear owner.

END OF PART 4
# PROJECT_ARCHITECTURE.md

# PART 5 — SERVICES, INTEGRATIONS, TESTING, OPERATIONS

---

# 78. PURPOSE

This section defines external services, supporting systems, testing strategy, debugging tools, deployment rules, and production integrations.

These systems support the game.

These systems do not define gameplay.

Gameplay authority remains inside:

* PROJECT_BIBLE.md
* PROJECT_DECISIONS.md
* Core Runtime Systems

---

# 79. ANALYTICS ARCHITECTURE

## Purpose

Analytics exists to understand player behavior.

Analytics exists to improve future balancing.

Analytics does not affect gameplay.

---

## Approved Service

Firebase Analytics

---

## Required Events

Level Start

Level Complete

Level Fail

Timer Fail

Combo Average

Highest Combo

Booster Usage

League Promotion

League Demotion

Rewarded Ad Watched

Tile Set Selected

Session Length

Daily Active User

Retention Events

---

## Analytics Restrictions

Analytics may not:

* Change gameplay
* Modify difficulty
* Modify board generation
* Modify player progression

Analytics is observational only.

---

# 80. CRASH REPORTING ARCHITECTURE

## Purpose

Track production errors and crashes.

---

## Approved Service

Firebase Crashlytics

---

## Required Data

Crash Logs

Exception Logs

Fatal Errors

Performance Issues

Device Information

Build Version

---

## Crashlytics Restrictions

Crash reporting must never expose player-sensitive data.

---

# 81. ADVERTISEMENT ARCHITECTURE

## Purpose

Support monetization without affecting gameplay fairness.

---

## Approved Service

Google AdMob

---

## Approved Rewarded Ads

Booster Recovery Ads

Lose Screen Ads

---

## Booster Recovery Flow

Player has no booster rights

↓

Player requests booster

↓

Rewarded Ad

↓

BoosterEconomyDirector grants +1 right

---

## Lose Screen Flow

Player fails level

↓

Optional rewarded ad offered

↓

Reward handled by RewardedAdBoosterController

---

## Ad Restrictions

Ads may not:

* Gate levels
* Gate progression
* Gate board generation
* Gate difficulty
* Force gameplay interruption

---

# 82. ADDRESSABLES ARCHITECTURE

## Purpose

Manage scalable game content.

---

## Approved Addressable Content

Tile Sets

Themes

Audio Packs

VFX Packs

Future Kids Mode Assets

Future Seasonal Content

---

## Restrictions

Addressables may manage content.

Addressables may not manage gameplay logic.

---

# 83. LOCALIZATION ARCHITECTURE

## Purpose

Support multiple languages.

---

## Approved System

Unity Localization

---

## Launch Languages

Turkish

English

---

## Future Languages

May be added later.

---

## Localization Restrictions

Localization may not affect gameplay rules.

---

# 84. AUDIO ARCHITECTURE

## Purpose

Provide relaxing premium-casual audio experience.

---

## Audio Direction

Relaxing

Meditative

Premium Casual

Modern Evening Luxury Atmosphere

---

## Audio Categories

Music

UI SFX

Tile SFX

Match SFX

Combo SFX

Joker SFX

Win SFX

Lose SFX

Booster SFX

---

## Audio Ownership

AudioManager owns runtime audio playback.

---

## Audio Settings

Music Volume

SFX Volume

Mute State

Must be saved by SaveSystem.

---

# 85. VFX ARCHITECTURE

## Purpose

Provide feedback and polish.

---

## Required VFX Categories

Tile Match

Tile Shatter

Combo

Joker

Win

Lose

Booster

Hint

---

## VFX Restrictions

VFX may not reduce gameplay readability.

Gameplay information is more important than visual effects.

---

## Pooling Rule

VFX must use VFX Pool whenever possible.

---

# 86. TILE SET ARCHITECTURE

## Purpose

Allow cosmetic personalization.

---

## Launch Tile Sets

Classic Mahjong

Gem Stones

Animals

Fruits

Objects

Symbols

---

## Tile Set Rule

Tile sets are cosmetic only.

---

## Tile Set Restrictions

Tile sets may not affect:

* Difficulty
* Score
* Combo
* Board Generation
* League Score
* Progression

---

## Runtime Change Rule

Changing tile set must not regenerate the board.

Only visuals should update.

---

# 87. DEBUG ARCHITECTURE

## Purpose

Assist development and testing.

---

## Debug Availability

Development Build Only

---

## Release Build Rule

All debug tools must be disabled in release builds.

---

## Required Debug Information

Current Level

Current Seed

Tile Count

Layer Count

Closed Tile Count

Joker Count

Current Archetype

Current Variation

Current Hole Pattern

Current Tile Set

Current League

Current FPS

---

## Debug Restrictions

Debug tools may not affect release gameplay.

---

# 88. TESTING ARCHITECTURE

## Purpose

Ensure production stability.

---

## Approved Framework

Unity Test Framework

---

## Required Test Categories

Board Generation Tests

Gameplay Tests

Booster Tests

Save System Tests

League Tests

Performance Tests

---

# 89. BOARD GENERATION TESTS

## Required Coverage

Grid Integrity

Layer Integrity

Tile Pair Validity

OpeningMoveChecker

DeadlockRiskChecker

BoardQualityChecker

Regeneration System

Fallback Recipe

---

## Goal

Generated boards must remain fair and playable.

---

# 90. GAMEPLAY TESTS

## Required Coverage

Tile Selection

Tray Capacity

Match Detection

Closed Tile Flow

Joker Logic

Score Calculation

Combo Calculation

Timer Flow

Win Conditions

Lose Conditions

---

# 91. BOOSTER TESTS

## Required Coverage

Shuffle

Undo

Hint

Rewarded Booster Recovery

Booster Count Persistence

---

# 92. SAVE SYSTEM TESTS

## Required Coverage

Level Persistence

Active Level State

Settings

Audio Settings

League Data

Booster Counts

Tile Set Selection

---

## Recovery Rule

Player must resume the same active level after closing the application.

---

# 93. LEAGUE TESTS

## Required Coverage

Promotion

Demotion

League Score Gain

League Score Loss

Leaderboard Sync

---

# 94. PERFORMANCE TESTING

## Performance Targets

60 FPS

Mid-Range Android Devices and Above

Portrait Only

---

## Required Validation

Tile Pool Stability

VFX Pool Stability

Memory Stability

Board Generation Performance

Load Time Performance

---

# 95. BUILD ARCHITECTURE

## Build Types

Development Build

Release Build

---

# 96. DEVELOPMENT BUILD

## Purpose

Internal testing and debugging.

---

## Enabled

Debug Panel

Debug Commands

Verbose Logging

Testing Utilities

---

## Optional

Analytics

Ads

Crashlytics

---

# 97. RELEASE BUILD

## Purpose

Production player build.

---

## Enabled

Analytics

AdMob

Crashlytics

Save System

Leaderboard

League Services

---

## Disabled

Debug Panels

Debug Commands

Development Utilities

Verbose Logs

---

# 98. GITHUB ARCHITECTURE

## Purpose

Protect project progress.

---

## Required Usage

GitHub is mandatory.

---

## Commit Strategy

Commit after major milestones.

Examples:

Board Generation Stable

Tray Stable

Match Stable

Closed Tiles Stable

League Stable

Booster Stable

---

## Rollback Rule

Rollback is preferred over emergency patching.

---

# 99. MCP INTEGRATION ARCHITECTURE

## Approved MCP Categories

Unity MCP

Filesystem MCP

GitHub MCP

Documentation MCP

Firebase MCP

---

## Allowed Activities

Scene Analysis

Prefab Analysis

Project Structure Analysis

Documentation Generation

Workflow Automation

Debug Assistance

Build Support

---

## Forbidden Activities

Changing Locked Design Decisions

Bypassing BoardGenerationPipeline

Replacing Core Gameplay Systems

Creating Alternative Runtime Architectures

---

# 100. FINAL ARCHITECTURE PRINCIPLES

## Principle 1

There must be only one active BoardGenerationPipeline.

---

## Principle 2

BoardSpawner is the only runtime tile spawner.

---

## Principle 3

Every gameplay responsibility must have exactly one owner.

---

## Principle 4

Frameworks serve the game.

The game does not serve the framework.

---

## Principle 5

Gameplay must remain deterministic and reproducible.

---

## Principle 6

The game should remain playable offline.

---

## Principle 7

League and leaderboard systems operate online.

---

## Principle 8

Debug systems exist only for development builds.

---

## Principle 9

All runtime boards must pass BoardQualityChecker.

---

## Principle 10

PROJECT_ARCHITECTURE.md defines the official implementation architecture of the project.

---

END OF PROJECT_ARCHITECTURE.md
