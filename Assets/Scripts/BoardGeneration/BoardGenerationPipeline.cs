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

            return CreateBoardData(recipe, closedBoardLayout);
        }

        private static LevelRecipe ResolveBaseRecipe(int levelNumber)
        {
            if (LevelRecipeGenerator.HasInstance)
            {
                return LevelRecipeGenerator.Instance.GenerateRecipe(levelNumber);
            }

            return LevelRecipeDefinition.GenerateRecipe(levelNumber);
        }

        private static BoardData CreateBoardData(LevelRecipe recipe, ClosedBoardLayout closedBoardLayout)
        {
            if (recipe == null || closedBoardLayout == null)
            {
                return CreateEmptyBoardData();
            }

            return new BoardData(
                recipe.LevelNumber,
                recipe.Seed,
                closedBoardLayout.ArchetypeId,
                closedBoardLayout.VariationIndex,
                closedBoardLayout.HolePatternId,
                closedBoardLayout.LayerDepth,
                closedBoardLayout.EffectiveTileCount,
                recipe.ClosedTileCount,
                recipe.JokerCount,
                false,
                closedBoardLayout.Assignments);
        }

        private static BoardData CreateBoardData(LevelRecipe recipe, DistributedBoardLayout distributedBoardLayout)
        {
            ClosedBoardLayout closedBoardLayout = ClosedTilePatternSelector.Apply(distributedBoardLayout, recipe);
            return CreateBoardData(recipe, closedBoardLayout);
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
