# PROJECT_BIBLE.md

# PART 1 — PROJECT VISION, GAME IDENTITY, DESIGN PHILOSOPHY

---

# 1. PROJECT VISION

## Vision Statement

The goal of this project is to create a premium casual mobile Mahjong-style puzzle game that feels relaxing, readable, fair, and highly replayable.

The game should provide short but satisfying gameplay sessions while remaining scalable for thousands of levels.

The project is designed as a production-quality commercial mobile game rather than a prototype or MVP.

The game must remain simple to understand, but difficult to master.

---

## Core Product Goals

The game should:

* Be easy to learn.
* Be difficult to fully master.
* Feel fair.
* Feel readable.
* Feel polished.
* Support long-term progression.
* Support thousands of levels.
* Encourage repeat play sessions.
* Reward skill and decision making.

The game should not rely on frustration, excessive randomness, or forced monetization.

---

# 2. GAME IDENTITY

## Genre

Single Player Mobile Puzzle Game

Mahjong-Style Layered Puzzle Game

Portrait Orientation

Premium Casual

---

## Core Gameplay Identity

The game is built around four systems:

1. Layered Mahjong Board
2. Four Slot Tray
3. Pair Matching
4. Board Clearing

The entire gameplay experience must remain centered around these systems.

---

## Permanent Gameplay Constants

The following rules may not be changed without explicit project approval.

### Board

Base Grid = 6x7

### Tray

Tray Capacity = 4

### Matching

Match Requirement = 2 identical tiles

### Orientation

Portrait Only

### Board Structure

Layered Tile System

### Win Condition

Remove all board tiles

### Lose Conditions

Tray overflow

Timer expiration

---

# 3. DESIGN PHILOSOPHY

## Primary Emotion

Control

The player should feel responsible for success and failure.

The player should understand why a move succeeds or fails.

The game should reward deliberate decision making.

---

## Secondary Emotion

Discovery

Players should continuously discover:

* New layouts
* New board silhouettes
* New closed tile arrangements
* New tile set combinations

---

## Tertiary Emotion

Pressure

The four-slot tray creates controlled pressure.

The timer creates controlled pressure.

Pressure should create excitement, not frustration.

---

## Emotions To Avoid

The game should avoid:

* Chaos
* Confusion
* Unfairness
* Random punishment
* Visual clutter
* Excessive stress

---

# 4. CORE GAMEPLAY PHILOSOPHY

## Main Rule

Gameplay must remain simple.

Depth should come from decision quality.

Depth should not come from complexity for its own sake.

---

## Key Principle

The game should become smarter, not larger.

Difficulty should come from:

* Better decisions
* More interesting layouts
* Stronger board reading

Difficulty should not come from:

* Massive tile counts
* Excessive timers
* Excessive closed tiles
* Booster dependency

---

## Board Solving Philosophy

Players should feel:

"I could have solved that better."

Players should never feel:

"That board was impossible."

---

## Booster Philosophy

Boosters exist to help.

Boosters do not exist to solve the game.

Every level must remain completable without boosters.

Boosters are convenience tools.

Boosters are not requirements.

---

# 5. PLAYER EXPERIENCE GOALS

## Session Length

A level should feel playable during a short mobile session.

The game should support:

* 2 minute sessions
* 5 minute sessions
* 10 minute sessions

---

## Readability

Players should always understand:

* What is selectable
* What is blocked
* What is matched
* Why they lost
* Why they won

---

## Fairness

The game should feel fair.

Failure should feel earned.

Success should feel deserved.

---

## Clarity

Visual clarity is more important than visual complexity.

Readability is more important than decoration.

Gameplay information is more important than visual effects.

---

# 6. PROGRESSION PHILOSOPHY

## Long-Term Progression

The game is designed for 9999 levels.

Progression must be scalable.

No system should rely on handcrafted levels.

---

## Content Strategy

New content should primarily come from:

* Archetypes
* Variations
* Hole Patterns
* Closed Tile Patterns
* Tile Sets

New content should not primarily come from:

* Bigger boards
* More tiles
* Longer timers

---

## Difficulty Philosophy

Difficulty should primarily come from:

* Archetype complexity
* Variation complexity
* Hole Pattern complexity
* Closed Tile Pattern complexity

Difficulty should not primarily come from:

* Excessive tile count
* Excessive timer pressure
* Excessive closed tile count
* Booster dependency

---

# 7. LONG-TERM CONTENT PHILOSOPHY

## Archetypes

Archetypes define board identity.

---

## Variations

Variations define board diversity.

---

## Content Expansion Rule

New content should primarily be added through variations rather than new archetypes.

---

## Tile Sets

Tile sets are cosmetic only.

Tile sets never affect:

* Gameplay
* Difficulty
* Score
* Combo
* Global Ranking
* Board Generation

Players may freely select their preferred tile set.

---

# 8. NON GOALS

The project is NOT:

* A Match-3 game
* A Triple Match game
* A Merge game
* A Gacha game
* An RPG
* A Base Building game
* A City Builder
* A Collection Game
* A Pay-To-Win game

---

## Monetization Non Goals

The game should not:

* Force ad watching
* Require purchases for progression
* Require boosters for completion

---

## Design Non Goals

The game should not:

* Become visually noisy
* Become overly complicated
* Introduce unnecessary mechanics
* Sacrifice readability for visuals

---

END OF PART 1
# PROJECT_BIBLE.md

# PART 2 — BOARD PHILOSOPHY, BOARD GENERATION, DIFFICULTY SYSTEM

---

# 9. BOARD PHILOSOPHY

## Board Purpose

The board is the core gameplay space.

All gameplay decisions originate from the board.

The board must remain:

* Readable
* Fair
* Structured
* Predictable
* Skill-driven

The board must never feel random or chaotic.

---

## Board Design Goals

Every generated board should:

* Feel solvable
* Feel intentional
* Provide meaningful choices
* Encourage planning
* Reward observation

A board should challenge the player's decisions, not their patience.

---

## Board Fairness Philosophy

A player should lose because of decisions.

A player should not lose because of:

* Hidden generation errors
* Impossible layouts
* Forced booster dependency
* Excessive randomness

---

## Board Complexity Philosophy

Board complexity should come from:

* Layer relationships
* Layout shape
* Tile accessibility
* Closed tile placement

Board complexity should not come from:

* Excessive tile count
* Visual clutter
* Artificial restrictions

---

# 10. GRID SYSTEM RULES

## Golden Rule

The gameplay board is always based on a fixed 6x7 grid.

This rule may not be changed.

---

## Base Grid

Columns: 6

Rows: 7

Total Cells: 42

Every runtime board begins from this grid.

---

## Coordinate Rules

All tile positions must originate from valid grid coordinates.

No tile may exist outside the grid coordinate system.

---

## Grid Integrity Rule

Grid coordinates never change.

Board generation may modify occupancy.

Board generation may not modify the coordinate system.

---

# 11. LAYER SYSTEM RULES

## Layer Philosophy

Layers create depth.

Layers do not create new coordinates.

---

## Layer Rule

Layering does not create new coordinates.

Layers only stack on top of existing grid coordinates.

---

## Alignment Rule

All layers must remain aligned to grid centers.

No horizontal offsets.

No vertical offsets.

No free-floating tiles.

---

## Maximum Layer Depth

Maximum Layer Depth = 4

Layer depth should increase gradually through progression.

---

## Layer Readability Rule

Layers must remain readable.

Layer complexity must never reduce board clarity.

---

# 12. ARCHETYPE SYSTEM

## Archetype Purpose

Archetypes define overall board identity.

Archetypes determine how occupied cells are distributed across the grid.

---

## Archetype Rule

Archetypes affect occupancy patterns only.

Archetypes never alter the underlying coordinate system.

---

## Launch Archetypes

Approved Launch Archetypes:

Diamond

Oval

Pyramid

Tower

Cross

Bridge

Island

Maze

---

## Future Archetypes

Reserved for future updates:

Snake

Spiral

Stairs

Fortress

---

## Archetype Expansion Rule

New content should primarily be added through variations rather than new archetypes.

---

# 13. VARIATION SYSTEM

## Variation Purpose

Variations provide long-term board diversity.

---

## Variation Philosophy

Archetypes define board identity.

Variations define board diversity.

---

## Variation Rule

Variations may:

* Modify occupied cells
* Modify layer distribution
* Modify density
* Modify shape details

Variations may not:

* Change board rules
* Change coordinate systems
* Change gameplay mechanics

---

# 14. HOLE PATTERN SYSTEM

## Purpose

Hole patterns create controlled empty spaces.

They improve visual diversity and board readability.

---

## Hole Pattern Rule

Hole patterns must remain intentional.

Random destruction of board structure is not allowed.

---

## Hole Pattern Goals

Create:

* Shape diversity
* Strategic routes
* Layout variation

Avoid:

* Confusion
* Visual imbalance
* Dead areas

---

# 15. TILE COUNT PROGRESSION

## Philosophy

Tile count is not the primary difficulty driver.

Difficulty should mainly come from:

* Archetypes
* Variations
* Hole Patterns
* Closed Tile Patterns

---

## Tile Count Curve

Level 1-20

80-88 Tiles

---

Level 21-100

88-100 Tiles

---

Level 101-300

100-112 Tiles

---

Level 301-1000

112-126 Tiles

---

Level 1001+

120-140 Tiles

---

## Hard Limit

Maximum Tile Count = 140

---

## Scaling Rule

Tile count should increase gradually.

Tile count should eventually stabilize.

The game should become smarter, not larger.

---

# 16. CLOSED TILE SYSTEM

## Purpose

Closed tiles are memory mechanics.

Closed tiles are not primary difficulty mechanics.

---

## Closed Tile Behavior

First Tap:

Reveal tile.

Second Tap:

Move tile to tray.

---

## Activation Rule

Closed tiles begin after Level 10.

---

## Closed Tile Curve

Level 1-9

0 Closed Tiles

---

Level 10-20

6-8 Closed Tiles

---

Level 21-100

8-10 Closed Tiles

---

Level 101-300

10-12 Closed Tiles

---

Level 301+

10-14 Closed Tiles

---

## Hard Limit

Maximum Closed Tiles = 14

---

## Closed Tile Fairness Rule

Closed tiles may create uncertainty.

Closed tiles may not block the opening game.

Closed tiles may not create impossible boards.

---

# 17. DIFFICULTY SYSTEM

## Core Philosophy

Difficulty should primarily come from:

* Archetype complexity
* Variation complexity
* Hole Pattern complexity
* Closed Tile Pattern complexity

---

## Difficulty Should Not Come From

* Excessive tile count
* Excessive timer pressure
* Excessive closed tile count
* Booster dependency

---

## Difficulty Progression Sources

DifficultyDirector may adjust:

* Layer depth
* Archetype complexity
* Variation complexity
* Hole Pattern complexity
* Closed Tile complexity
* Board density

---

## Difficulty Restrictions

DifficultyDirector may not:

* Change board size
* Change tray size
* Change matching rules

---

# 18. BOARD GENERATION PIPELINE

## Official Runtime Pipeline

This is the only valid runtime board generation flow.

Player Progress

↓

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

Grid Mask

↓

Archetype

↓

Variation

↓

Hole Pattern

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

## Runtime Generation Rule

No script, prefab, debug tool, editor helper, legacy generator, test generator, or temporary generator may spawn or modify runtime board tiles outside this flow.

---

## BoardSpawner Rule

BoardSpawner is the only system allowed to instantiate runtime tiles.

---

# 19. BOARD QUALITY CHECKER

## Authority Rule

BoardQualityChecker has authority over BoardGenerationPipeline.

Any board that fails validation must be discarded and regenerated.

---

## Quality Philosophy

A technically valid board is not necessarily a production-quality board.

---

## Validation Rules

### Grid Integrity Check

All tiles must occupy valid 6x7 coordinates.

---

### Layer Integrity Check

All layers must remain aligned to grid centers.

---

### Tile Pair Check

All tile types must exist in valid pairs.

---

### Opening Move Check

Every generated board must provide at least two meaningful opening choices.

---

### Accessibility Check

Boards should provide sufficient selectable tiles at level start.

Recommended minimum:

8 selectable tiles.

---

### Closed Tile Fairness Check

Closed tiles may not block the opening game.

---

### Joker Accessibility Check

Reward jokers must remain realistically reachable.

---

### Deadlock Risk Check

Boards must not have excessive early deadlock risk.

---

### Visual Silhouette Check

Board silhouettes must remain intentional, balanced, and visually readable.

---

### Density Check

Boards may not be excessively empty or excessively crowded.

---

## Regeneration Rule

If any validation fails:

Board is rejected.

Board is regenerated.

No exceptions.

---

END OF PART 2
# PROJECT_BIBLE.md

# PART 3 — SCORE, COMBO, TIMER, GLOBAL RANKING, BOOSTERS, ECONOMY

---

# 20. SCORE SYSTEM

## Score Philosophy

Score exists to reward skillful play.

Score does not determine board difficulty.

Score does not affect board generation.

---

## Base Score Rule

Every successful tile pair match awards:

+1000 Score

---

## Match Score Rule

Every pair removed from the tray grants score.

Score is awarded immediately after a successful match.

---

## Time Performance Bonus

Time performance is the most valuable scoring factor.

Finish within 40% of allocated level time:

+25,000

---

Finish within 60% of allocated level time:

+15,000

---

Finish within 80% of allocated level time:

+8,000

---

Finish within 100% of allocated level time:

+3,000

---

## Perfect Clear Bonus

Perfect Clear:

+10,000

---

Perfect Clear Definition:

No Fail

No Mistakes Leading To Loss

Successful Board Completion

---

## No Booster Bonus

Complete level without using boosters:

+5,000

---

## Joker Bonus

Each early joker pair matched within 1 minute:

+2,500

---

## Score Independence Rule

Level score components feed Global Performance Score.

Score does not determine board difficulty.

Score does not affect board generation.

---

# 21. COMBO SYSTEM

## Combo Philosophy

Combo rewards efficient and consecutive matching.

Combo should feel rewarding but should not dominate scoring.

---

## Combo Window

Combo Window = 3 Seconds

After a successful match:

A 3-second combo timer begins.

If another match occurs within this window:

Combo increases.

If no match occurs within 3 seconds:

Combo resets.

---

## Combo Rewards

Combo x2

+200

---

Combo x3

+400

---

Combo x4

+600

---

Combo x5

+800

---

Combo x6+

+1,200

---

## Combo Philosophy Rule

Combo rewards good play.

Combo should never become the primary score source.

Time performance remains the dominant scoring factor.

---

## Combo Ranking Rule

Combo bonuses contribute to Global Performance Score.

Combo should not dominate total level score.

---

## Performance Screen Metrics

Performance screen must display:

* Total Combo Count
* Highest Combo Achieved

---

# 22. TIMER SYSTEM

## Timer Philosophy

Timer creates controlled pressure.

Timer must never create unfair pressure.

---

## Difficulty Driven Timer

Timer is calculated by DifficultyDirector.

There is no globally fixed timer.

---

## Timer Calculation Sources

Timer may be influenced by:

* Tile Count
* Layer Depth
* Closed Tile Count
* Archetype Complexity
* Variation Complexity
* Hole Pattern Complexity

---

## Timer Scaling Rule

Harder levels may receive additional time.

Easier levels may receive less time.

---

## Timer Expiration Rule

If timer reaches zero:

* Level fails
* Same level restarts

---

# 23. GLOBAL RANKING SYSTEM

## Global Ranking Philosophy

Global Ranking is the single competitive motivation system.

Global Ranking rewards performance quality.

Global Ranking is not intended to function as esports competition.

---

## Global Ranking Structure

There is only one ranking system:

Global Ranking

---

## Global Ranking Characteristics

Permanent

Global

Fair

Performance-based

Never resets

---

## Forbidden Ranking Systems

No league tiers at launch.

No Bronze, Silver, Gold, Diamond, Elite, Premium, or Ultra Premium leagues.

No seasons.

No rank wipes.

No score resets.

---

## Ranking Ownership

Global Ranking is owned by RankingDirector.

---

# 24. GLOBAL PERFORMANCE SCORE

## Global Performance Score Purpose

Global Performance Score measures overall player performance quality.

Global Performance Score is the basis for Global Ranking.

---

## Global Performance Score Sources

Global Performance Score is accumulated from:

* Match score (+1000 per pair)
* Time performance bonuses
* Combo bonuses
* Joker bonuses
* Perfect clear bonus
* No-booster bonus

---

## Performance Priority Rule

Completion time is the most valuable scoring factor.

Combo performance supports score but does not replace time performance importance.

---

# 25. BOOSTER SYSTEM

## Booster Philosophy

Boosters assist players.

Boosters do not solve levels.

Levels must remain completable without boosters.

---

## Approved Boosters

Shuffle

Undo

Hint

---

## Booster Ownership

Players own booster rights.

Booster rights are stored permanently.

---

## Booster Progression

Every 10 completed levels:

Shuffle +1

Undo +1

Hint +1

---

# 26. SHUFFLE BOOSTER

## Purpose

Redistribute symbols.

---

## Restrictions

Shuffle may:

* Change symbol assignments

Shuffle may not:

* Move tile positions
* Change board structure
* Change layers

---

# 27. UNDO BOOSTER

## Purpose

Reverse the previous tray action.

---

## Behavior

Undo returns the most recently selected tile.

Returned tile goes back to its original board position.

---

# 28. HINT BOOSTER

## Purpose

Reveal a useful move.

---

## Behavior

Hint should identify a meaningful playable option.

Hint should not solve the board automatically.

---

## Visual Rule

Hint indicators must remain subtle.

Premium casual presentation must be maintained.

---

# 29. REWARDED AD SYSTEM

## Philosophy

Rewarded ads are optional.

Players choose whether to watch them.

---

## Approved Rewarded Ads

Booster Recovery Ads

Lose Screen Ads

---

## Booster Recovery

If booster count reaches zero:

Player may watch a rewarded ad.

Reward:

+1 booster right

---

## Lose Screen Ad

After failure:

Player may watch a rewarded ad.

Additional implementation details may be defined later.

---

## Ad Restrictions

Ads may not:

* Force progression
* Gate levels
* Gate board generation
* Gate gameplay systems

---

# 30. SYMBOL LIBRARY SYSTEM

## Symbol Library Philosophy

The game owns a global symbol library.

Symbol collections are selected automatically per level by the generation system.

Symbol libraries do not affect gameplay rules.

---

## Automatic Selection Rule

Players do not manually choose symbol collections.

The game selects symbols automatically per level.

---

## Launch Symbol Categories

Animals

Fruits

Objects

Symbols

Classic Mahjong

Gem Stones

---

## Gameplay Independence

Symbol libraries must never affect:

* Score
* Difficulty
* Combo
* Board Generation
* Global Performance Score
* Progression

---

# 31. PERFORMANCE SCREEN

## Purpose

Provide completion feedback.

Celebrate player performance.

---

## Required Information

Completion Time

Gameplay Score

Global Performance Score Earned

Joker Bonus

Time Bonus

Total Combos

Highest Combo

---

## Navigation

Performance screen must contain:

Next Level Button

---

# 32. LONG TERM ECONOMY PHILOSOPHY

## Economy Rule

Economy exists to support gameplay.

Economy does not control gameplay.

---

## Progression Rule

Players should feel rewarded for activity.

Players should not feel punished for absence.

---

## Monetization Rule

Monetization must support the player experience.

Monetization must never become a progression requirement.

---

## Core Principle

Frameworks serve the game.

The game does not serve the framework.

---

END OF PART 3
# PROJECT_BIBLE.md

# PART 4 — DEVELOPMENT PHILOSOPHY, AI WORKFLOW, ARCHITECTURE PROTECTION, PROJECT GOVERNANCE

---

# 33. DEVELOPMENT PHILOSOPHY

## Project Philosophy

This project is designed to be developed primarily through AI-assisted workflows.

The majority of software engineering work should be performed through:

* Cursor
* ChatGPT
* Approved MCP tools
* Approved Frameworks

Human involvement should be minimized whenever possible.

---

## Human Responsibilities

Human involvement should primarily be limited to:

* Unity Hierarchy assignments
* Inspector references
* Package installation approval
* Android build generation
* Google Play configuration
* Store publishing tasks
* External service account setup
* Platform-specific deployment tasks

---

## AI Responsibilities

AI systems should perform:

* Architecture implementation
* Code generation
* Refactoring
* Documentation generation
* Debugging assistance
* Testing assistance
* Project structure management
* Pipeline implementation
* Gameplay implementation

---

## Development Goal

Whenever possible:

Development should be automated.

Development should be AI-assisted.

Manual repetitive work should be avoided.

---

# 34. FRAMEWORK PHILOSOPHY

## Core Principle

Build around the game.

Do not build the game around the framework.

---

## Authority Rule

Frameworks serve the game.

The game does not serve the framework.

---

## Conflict Rule

If a framework conflicts with PROJECT_BIBLE:

PROJECT_BIBLE always wins.

---

## Framework Restrictions

Frameworks may assist development.

Frameworks may not replace core gameplay systems.

---

## Core Systems That Must Remain Custom

BoardGenerationPipeline

LevelRecipeGenerator

Tile System

Tray System

Match System

Closed Tile System

DifficultyDirector

BoardQualityChecker

Global Ranking System

Booster System

Reward System

---

## Forbidden Replacements

Ready Mahjong Generators

Ready Puzzle Generators

Ready Match Templates

Ready Level Generators

Third-Party Core Gameplay Systems

---

# 35. APPROVED FRAMEWORKS

## Required Frameworks

DOTween

Addressables

Unity Localization

Google AdMob

Firebase Analytics

Firebase Crashlytics

Unity Test Framework

---

## Optional Frameworks

Odin Inspector

Lean Pool

Nice Vibrations

Easy Save

---

## Framework Usage Intent

DOTween

Animation

---

Addressables

Content Delivery

---

Localization

Multi-Language Support

---

AdMob

Rewarded Ads

---

Firebase Analytics

Player Analytics

---

Firebase Crashlytics

Crash Monitoring

---

Unity Test Framework

Automated Validation

---

# 36. MCP PHILOSOPHY

## MCP Purpose

MCP tools exist to assist development.

MCP tools do not define architecture.

---

## Approved MCP Categories

Unity MCP

Filesystem MCP

GitHub MCP

Documentation MCP

Firebase MCP

---

## Allowed MCP Activities

Analyze Scenes

Analyze Prefabs

Analyze Project Structure

Generate Documentation

Assist Debugging

Automate Workflows

Support Build Pipelines

---

## Forbidden MCP Activities

Bypassing PROJECT_BIBLE

Bypassing BoardGenerationPipeline

Replacing Core Gameplay Systems

Modifying Locked Design Decisions

Changing Project Architecture Without Approval

---

# 37. VERSION CONTROL PHILOSOPHY

## GitHub Requirement

GitHub is mandatory.

---

## Purpose

GitHub exists to:

* Protect progress
* Track architecture changes
* Enable rollback
* Prevent accidental loss

---

## Commit Philosophy

Major gameplay milestones should be committed.

Examples:

Board Generation Stable

Tray System Stable

Match System Stable

Closed Tile System Stable

Global Ranking System Stable

---

## Rollback Rule

If a new change damages a stable system:

Rollback is preferred over emergency patching.

---

# 38. SINGLE SOURCE OF TRUTH

## Purpose

Every major system must have a single owner.

Duplicate ownership is forbidden.

---

## Ownership Map

Player Progress

Owner:

PlayerProgressionDirector

---

Session State

Owner:

SessionDirector

---

Board Generation

Owner:

BoardGenerationPipeline

---

Tile State

Owner:

Tile System

---

Tray State

Owner:

TrayController

---

Match State

Owner:

MatchController

---

Score

Owner:

ScoreController

---

Combo

Owner:

ComboController

---

Timer

Owner:

TimerController

---

Global Ranking

Owner:

RankingDirector

---

Booster Economy

Owner:

BoosterEconomyDirector

---

## Ownership Rule

The same responsibility may not be owned by multiple systems.

---

# 39. ARCHITECTURE PROTECTION RULES

## Purpose

Protect long-term project stability.

---

## Core Rule

There must be only one active board generation pipeline.

---

## Official Runtime Flow

Player Progress

↓

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

Grid Mask

↓

Archetype

↓

Variation

↓

Hole Pattern

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

## Runtime Protection Rule

No script, prefab, debug tool, editor helper, legacy generator, test generator, or temporary generator may spawn or modify runtime board tiles outside this flow.

---

## BoardSpawner Rule

BoardSpawner is the only system allowed to instantiate runtime board tiles.

---

# 40. FORBIDDEN SYSTEMS

## Forbidden Generators

Alternative Board Generators

Runtime Test Generators

Legacy Runtime Generators

Temporary Runtime Generators

---

## Forbidden Gameplay Systems

Duplicate Tray Systems

Duplicate Match Systems

Duplicate Booster Systems

Duplicate Global Ranking Systems

Duplicate Save Systems

Duplicate Progression Systems

---

## Forbidden Architecture Patterns

Multiple Sources Of Truth

Runtime Rule Overrides

Untracked Gameplay Changes

Hidden Architecture Dependencies

---

# 41. CURSOR OPERATION RULES

## Mandatory Reading Order

Before making any change:

Read PROJECT_BIBLE.md

Read PROJECT_ARCHITECTURE.md

Read PROJECT_DECISIONS.md

Read CURSOR_RULES.md

---

## Conflict Resolution

If documents conflict:

PROJECT_DECISIONS.md overrides all other documents.

---

## Unity User Experience Rule

Assume the project owner has limited Unity editor experience.

Whenever a manual Unity action is required:

1. Explain why it is required.
2. Explain exactly where to click.
3. Explain exactly what object must be assigned.
4. Minimize manual steps.

---

## Refactor Rule

Before creating a new system:

Search for an existing system.

Reuse existing systems whenever possible.

---

## Duplicate System Rule

Never create a second system that performs the same responsibility.

---

# 42. FUTURE EXPANSION RULES

## Expansion Philosophy

Future content should expand the game without changing its identity.

---

## Approved Future Expansions

Additional Variations

Additional Tile Sets

Additional Closed Tile Patterns

Additional Hole Patterns

Additional Archetypes

Kids Mode

Additional Languages

Additional Themes

---

## Expansion Restrictions

Future expansions may not change:

Board Size

Tray Size

Match Requirement

Portrait Orientation

Core Gameplay Loop

Board Generation Philosophy

Single Pipeline Rule

---

# 43. FINAL PROJECT PRINCIPLES

## Principle 1

The game should become smarter, not larger.

---

## Principle 2

A technically valid board is not necessarily a production-quality board.

---

## Principle 3

Players should lose because of decisions, not because of generation errors.

---

## Principle 4

Every level must remain completable without boosters.

---

## Principle 5

Readability is more important than visual complexity.

---

## Principle 6

Frameworks serve the game.

The game does not serve the framework.

---

## Principle 7

There must be only one active board generation pipeline.

---

## Principle 8

Long-term content should primarily come from variations rather than new archetypes.

---

## Principle 9

Tile sets are cosmetic only.

---

## Principle 10

PROJECT_BIBLE is the highest gameplay and design authority of the project.

---

END OF PROJECT_BIBLE
