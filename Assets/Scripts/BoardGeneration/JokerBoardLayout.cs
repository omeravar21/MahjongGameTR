using System.Collections.Generic;

namespace MahjongGame.BoardGeneration
{
    public sealed class JokerBoardLayout
    {
        public HolePatternId HolePatternId { get; }

        public BoardArchetypeId ArchetypeId { get; }

        public int VariationIndex { get; }

        public int LevelNumber { get; }

        public int Seed { get; }

        public int LayerDepth { get; }

        public int RequestedTileCount { get; }

        public int EffectiveTileCount { get; }

        public int PairCount { get; }

        public int DistinctSymbolCount { get; }

        public ClosedTilePatternId ClosedTilePatternId { get; }

        public int AppliedClosedTileCount { get; }

        public RewardJokerPatternId RewardJokerPatternId { get; }

        public int AppliedJokerCount { get; }

        public IReadOnlyList<TileSymbolAssignment> Assignments { get; }

        public JokerBoardLayout(
            HolePatternId holePatternId,
            BoardArchetypeId archetypeId,
            int variationIndex,
            int levelNumber,
            int seed,
            int layerDepth,
            int requestedTileCount,
            int effectiveTileCount,
            int pairCount,
            int distinctSymbolCount,
            ClosedTilePatternId closedTilePatternId,
            int appliedClosedTileCount,
            RewardJokerPatternId rewardJokerPatternId,
            int appliedJokerCount,
            IReadOnlyList<TileSymbolAssignment> assignments)
        {
            HolePatternId = holePatternId;
            ArchetypeId = archetypeId;
            VariationIndex = variationIndex;
            LevelNumber = levelNumber;
            Seed = seed;
            LayerDepth = layerDepth;
            RequestedTileCount = requestedTileCount;
            EffectiveTileCount = effectiveTileCount;
            PairCount = pairCount;
            DistinctSymbolCount = distinctSymbolCount;
            ClosedTilePatternId = closedTilePatternId;
            AppliedClosedTileCount = appliedClosedTileCount;
            RewardJokerPatternId = rewardJokerPatternId;
            AppliedJokerCount = appliedJokerCount;
            Assignments = assignments ?? new TileSymbolAssignment[0];
        }

        public static JokerBoardLayout FromClosedBoardLayout(
            ClosedBoardLayout layout,
            RewardJokerPatternId rewardJokerPatternId,
            int appliedJokerCount,
            IReadOnlyList<TileSymbolAssignment> assignments)
        {
            if (layout == null)
            {
                return new JokerBoardLayout(
                    HolePatternId.SingleCenter,
                    BoardArchetypeId.Diamond,
                    0,
                    0,
                    0,
                    1,
                    0,
                    0,
                    0,
                    0,
                    ClosedTilePatternId.CornerSingle,
                    0,
                    rewardJokerPatternId,
                    appliedJokerCount,
                    assignments);
            }

            return new JokerBoardLayout(
                layout.HolePatternId,
                layout.ArchetypeId,
                layout.VariationIndex,
                layout.LevelNumber,
                layout.Seed,
                layout.LayerDepth,
                layout.RequestedTileCount,
                layout.EffectiveTileCount,
                layout.PairCount,
                layout.DistinctSymbolCount,
                layout.ClosedTilePatternId,
                layout.AppliedClosedTileCount,
                rewardJokerPatternId,
                appliedJokerCount,
                assignments);
        }
    }
}
