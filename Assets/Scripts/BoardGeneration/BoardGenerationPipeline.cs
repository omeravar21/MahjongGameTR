using System.Collections.Generic;
using MahjongGame.Progression;

namespace MahjongGame.BoardGeneration
{
    public static class BoardGenerationPipeline
    {
        public static BoardData GenerateBoardData(int levelNumber)
        {
            LevelRecipe baseRecipe = ResolveBaseRecipe(levelNumber);
            if (baseRecipe == null)
            {
                return CreateEmptyBoardData();
            }

            return GenerateValidatedBoardData(baseRecipe);
        }

        public static BoardData GenerateBoardData(LevelRecipe recipe)
        {
            if (recipe == null)
            {
                return CreateEmptyBoardData();
            }

            return GenerateValidatedBoardData(recipe);
        }

        private static BoardData GenerateValidatedBoardData(LevelRecipe baseRecipe)
        {
            BoardData lastCandidate = null;
            int maxAttempts = baseRecipe.MaxRegenerationAttempts;
            for (int attemptIndex = 0; attemptIndex < maxAttempts; attemptIndex++)
            {
                int attemptSeed = BoardRegenerationDefinition.ComputeAttemptSeed(baseRecipe.Seed, attemptIndex);
                LevelRecipe attemptRecipe = LevelRecipeDefinition.CreateWithSeed(baseRecipe, attemptSeed);
                BoardData candidate = GenerateCandidateBoardData(attemptRecipe);
                lastCandidate = candidate;

                if (BoardQualityChecker.Check(candidate).IsValid)
                {
                    return candidate.WithValidationFlag(true);
                }
            }

            return EmergencyFallbackRecipeDefinition.GenerateFallbackBoardData(baseRecipe);
        }

        public static BoardData GenerateCandidateBoardData(LevelRecipe recipe)
        {
            if (recipe == null)
            {
                return CreateEmptyBoardData();
            }

            GridMask baseMask = GridMaskDefinition.GenerateFromRecipe(recipe);
            ArchetypeLayout archetypeLayout = ArchetypeSelector.Apply(baseMask, recipe);
            VariationLayout variationLayout = VariationSelector.Apply(archetypeLayout, recipe);
            HolePatternLayout holePatternLayout = HolePatternSelector.Apply(variationLayout, recipe);
            LayeredBoardLayout layeredBoardLayout = LayerBuilder.Build(holePatternLayout, recipe);
            DistributedBoardLayout distributedBoardLayout = TilePairDistributor.Distribute(
                layeredBoardLayout,
                recipe);

            ClosedBoardLayout closedBoardLayout = ClosedTilePatternSelector.Apply(distributedBoardLayout, recipe);

            JokerBoardLayout jokerBoardLayout = RewardJokerPatternSelector.Apply(closedBoardLayout, recipe);

            return CreateBoardData(recipe, jokerBoardLayout);
        }

        private static LevelRecipe ResolveBaseRecipe(int levelNumber)
        {
            if (LevelRecipeGenerator.HasInstance)
            {
                return LevelRecipeGenerator.Instance.GenerateRecipe(levelNumber);
            }

            return LevelRecipeDefinition.GenerateRecipe(levelNumber);
        }

        private static BoardData CreateBoardData(LevelRecipe recipe, JokerBoardLayout jokerBoardLayout)
        {
            if (recipe == null || jokerBoardLayout == null)
            {
                return CreateEmptyBoardData();
            }

            return new BoardData(
                recipe.LevelNumber,
                recipe.Seed,
                jokerBoardLayout.ArchetypeId,
                jokerBoardLayout.VariationIndex,
                jokerBoardLayout.HolePatternId,
                jokerBoardLayout.LayerDepth,
                jokerBoardLayout.EffectiveTileCount,
                recipe.ClosedTileCount,
                recipe.JokerCount,
                false,
                jokerBoardLayout.Assignments);
        }

        private static BoardData CreateBoardData(LevelRecipe recipe, ClosedBoardLayout closedBoardLayout)
        {
            JokerBoardLayout jokerBoardLayout = RewardJokerPatternSelector.Apply(closedBoardLayout, recipe);
            return CreateBoardData(recipe, jokerBoardLayout);
        }

        private static BoardData CreateEmptyBoardData()
        {
            return new BoardData(
                LevelProgressData.MinLevel,
                0,
                BoardArchetypeId.Diamond,
                0,
                HolePatternId.SingleCenter,
                1,
                0,
                0,
                0,
                false,
                new TileSymbolAssignment[0]);
        }
    }
}
