PROJECT_CONTEXT.md
PROJECT OVERVIEW
PROJECT NAME

Temporary Working Title:

Premium Mahjong Puzzle

(Project name can change later.)

PROJECT PURPOSE

Create a premium casual mobile puzzle game that combines:

Relaxing gameplay
Long-term progression
Performance-based competition
High replayability
Fair global ranking

The game should feel premium, polished, and timeless.

PROJECT VISION

The goal is not to create a fast-paced arcade game.

The goal is not to create a stressful competitive game.

The goal is to create a puzzle game that feels:

Relaxing
Readable
Rewarding
Competitive in a healthy way

The player should feel:

Calm while playing
Proud when improving
Motivated to continue
TARGET EXPERIENCE

The game should create:

Calm Competition

Meaning:

Low stress
High satisfaction
Continuous self-improvement
Meaningful progression

Competition should encourage mastery.

Competition should never create frustration.

TARGET DAILY PLAYTIME

Target:

30+ Minutes Per Day

The game is designed for:

Multiple sessions per day
Long-term retention
Daily return behavior
SESSION PHILOSOPHY

Typical Session Length:

5–15 Minutes

Target Daily Total:

30+ Minutes

Example:

Morning Session

↓

Lunch Session

↓

Evening Session

The game should fit naturally into a mobile lifestyle.

TARGET AUDIENCE

Age:

18–65

Gender:

All

Player Types:

Casual Players
Puzzle Players
Relaxation Players
Long-Term Progression Players
Competitive Improvement Players
GAME IDENTITY

This game is NOT:

Match-3
Triple Match
Candy Crush
Hardcore Mahjong Simulator
Speed Runner Puzzle

This game IS:

Tray-Based Pair Matching Puzzle

Core Identity:

Easy to Learn
Difficult to Master
CORE DESIGN PILLARS
Pillar 1 — Readability First

Players must always understand the board.

Visual clarity is more important than visual complexity.

Pillar 2 — Performance Competition

Players compete through performance quality.

Not through spending.

Not through grinding.

Pillar 3 — Meditative Experience

The game should feel:

Calm
Smooth
Relaxing
Comfortable
Pillar 4 — Long-Term Progression

The game is designed for years of progression.

Level progression is effectively endless.

Pillar 5 — The Game Should Become Smarter, Not Larger

Difficulty should come from:

Better board design
Better variations
Better patterns

Not from:

Bigger grids
More slots
More complexity for its own sake
CORE COMPETITION PHILOSOPHY

Competition is based on:

Score
Time Performance
Combo Performance
Efficient Play

Competition is NOT based on:

Spending
Luck
Special advantages
GLOBAL RANKING PHILOSOPHY

There is only one ranking system:

Global Ranking

Characteristics:

Permanent
Global
Fair
Performance-based

No:

Seasons
League Tiers
Rank Resets
FAIRNESS PHILOSOPHY

Every player should receive equivalent level conditions.

A level should be fundamentally the same for all players.

Global ranking fairness is a core requirement.

DEVELOPMENT PHILOSOPHY

The project follows:

Documentation Driven Development
Cursor First Development
Single Pipeline Architecture
Production Ready Systems Only
GitHub Version Control

All major decisions must be documented before implementation.

TECHNOLOGY PHILOSOPHY

Mandatory Technologies:

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
CORE PRINCIPLES
Readability First
Calm Competition
Performance Over Grinding
Long-Term Progression
The Game Should Become Smarter, Not Larger
Global Ranking Must Remain Fair
Single Pipeline Architecture
Premium Casual Experience
Production Ready Systems Only
Every Player Should Receive Equivalent Level Conditions

END OF PROJECT_CONTEXT.md PART 1

DAILY CONTENT PHILOSOPHY

The game should encourage players to return every day.

Daily content exists to increase engagement and long-term retention.

Daily content must never replace core progression.

Daily content complements progression.

DAILY BOARD PHILOSOPHY

The game contains a Daily Board system.

Characteristics:

One Daily Board per day
Same Daily Board for all players
Generated from a daily seed
Optional content
Separate from normal progression

Daily Board exists to provide:

Variety
Daily motivation
Additional challenge

Daily Board does not block normal progression.

DAILY MISSION PHILOSOPHY

The game contains Daily Missions.

Mission Categories:

Easy Missions
Medium Missions
Hard Missions

Recommended Daily Structure:

2 Easy Missions
2 Medium Missions
1 Hard Mission

Total:

5 Daily Missions Per Day

Daily Missions should encourage interaction with different game systems.

PROGRESSION PHILOSOPHY

Progression is linear.

Level Structure:

Level 1

↓

Level 2

↓

Level 3

↓

...

↓

Level 9999+

There are:

No Chapters
No Worlds
No Episodes

The player always knows the next objective.

DIFFICULTY PHILOSOPHY

Difficulty should feel fair.

Difficulty should not rely on frustration.

Difficulty should come from:

Archetypes
Variations
Hole Patterns
Closed Tile Patterns

Difficulty should not come from:

Random punishment
Hidden mechanics
Unfair board states

The game should become smarter, not larger.

BOARD GENERATION PHILOSOPHY

Board generation is one of the core pillars of the project.

Every runtime board must pass through:

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

No alternative generation path is allowed.

BOARD QUALITY PHILOSOPHY

Every generated board must satisfy quality requirements.

Board quality is more important than board quantity.

Required Quality Checks:

OpeningMoveChecker
DeadlockRiskChecker
BoardQualityChecker

Board Quality Rules:

Minimum Opening Moves = 2 meaningful opening choices
Selectable Tile Availability controlled by DifficultyDirector
Maximum Blocked Tile Ratio = 70%
Maximum Closed Tile Cluster = 4
Minimum Readability Score = 75 (design target)
Minimum Layer Balance Score = 70 (design target)

Invalid boards must regenerate.

ARCHETYPE PHILOSOPHY

Launch Archetypes:

Diamond

Oval

Pyramid

Tower

Cross

Bridge

Island

Maze

Future Archetypes (post-launch expansion):

Snake

Spiral

Stairs

Fortress

Launch archetypes are available at launch.

Future archetypes are reserved for later updates.

Archetypes modify board shape and occupancy.

Archetypes never modify the 6x7 base grid.

VARIATION PHILOSOPHY

Variations are responsible for most long-term content diversity.

Variations should create new board experiences without changing gameplay rules.

Goals:

Reduce repetition
Increase variety
Preserve readability

The project prioritizes variation quality over archetype quantity.

THEME PHILOSOPHY

Themes are visual only.

Themes never affect:

Difficulty
Progression
Ranking
Gameplay Rules

Launch Themes:

Luxury Wood
Bamboo Zen
Premium Evening

All themes are available from the beginning.

Players may switch themes freely.

SYMBOL PHILOSOPHY

Players do not manually choose symbol libraries.

The game automatically selects symbols per level.

Launch Symbol Categories:

Animals
Fruits
Objects
Symbols

The symbol library is designed for long-term expansion.

New symbols may be added at any time.

COMPETITION PHILOSOPHY

Competition should feel motivating.

Competition should not feel stressful.

Players compete through:

Score
Combo Quality
Time Performance
Efficient Play

Players do not compete through:

Spending
Random advantages
Exclusive gameplay benefits
FUTURE EXPANSION PHILOSOPHY

Future updates should expand content variety.

Future updates should not alter core gameplay.

Allowed Future Expansions:

New Themes
New Symbols
New Variations
New Archetypes
New Daily Content
New Statistics
Additional Languages

Not Allowed:

Changing Tray Capacity
Changing Match Rules
Changing Base Grid
Changing Core Gameplay Loop
LONG-TERM GOAL

Create a premium casual puzzle game that remains enjoyable for years.

The game should provide:

Relaxation
Mastery
Progression
Fair Competition
Daily Motivation

The project prioritizes quality, readability, and longevity over feature quantity.

END OF PROJECT_CONTEXT.md PART 2