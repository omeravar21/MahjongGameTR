#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MahjongGame.Editor
{
    /// <summary>
    /// Forces reimport of scripts whose .meta GUIDs were repaired outside Unity.
    /// Runs once per editor session until all listed scripts compile into Assembly-CSharp.
    /// </summary>
    [InitializeOnLoad]
    public static class ScriptImportRecoveryBuilder
    {
        private const string RecoverySessionKey = "MahjongGame.ScriptImportRecovery.pending";

        private static readonly string[] RecoveredScriptPaths =
        {
            "Assets/Editor/GameSceneBoardBuilder.cs",
            "Assets/Editor/MainMenuLayoutAutoBuilder.cs",
            "Assets/Editor/MainMenuLayoutBuilder.cs",
            "Assets/Editor/MainMenuLayoutUtility.cs",
            "Assets/Scripts/Board/BoardGridCoordinate.cs",
            "Assets/Scripts/Board/BoardGridDefinition.cs",
            "Assets/Scripts/Board/BoardGridLayout.cs",
            "Assets/Scripts/Board/BoardGridVisualController.cs",
            "Assets/Scripts/Board/BoardRootController.cs",
            "Assets/Scripts/Core/Save/ActiveLevelStateSaveData.cs",
            "Assets/Scripts/Core/Save/AudioSettingsSaveData.cs",
            "Assets/Scripts/Core/Save/BoosterCountsSaveData.cs",
            "Assets/Scripts/Core/Save/PlayerSaveData.cs",
            "Assets/Scripts/Core/Save/StatisticsSaveData.cs",
            "Assets/Scripts/Core/SaveSystem.cs",
            "Assets/Scripts/Progression/PlayerProgressionDirector.cs",
            "Assets/Scripts/BoardGeneration/ComplexityTier.cs",
            "Assets/Scripts/BoardGeneration/DifficultyDefinition.cs",
            "Assets/Scripts/BoardGeneration/DifficultyDirector.cs",
            "Assets/Scripts/BoardGeneration/DifficultyProfile.cs",
            "Assets/Scripts/BoardGeneration/DifficultySystemValidator.cs",
            "Assets/Scripts/BoardGeneration/BoardArchetypeId.cs",
            "Assets/Scripts/BoardGeneration/ClosedTilePatternId.cs",
            "Assets/Scripts/BoardGeneration/HolePatternId.cs",
            "Assets/Scripts/BoardGeneration/VisualVarietyDefinition.cs",
            "Assets/Scripts/BoardGeneration/VisualVarietyDirector.cs",
            "Assets/Scripts/BoardGeneration/VisualVarietyProfile.cs",
            "Assets/Scripts/BoardGeneration/VisualVarietySystemValidator.cs",
            "Assets/Scripts/BoardGeneration/RewardJokerPatternId.cs",
            "Assets/Scripts/BoardGeneration/LevelRecipe.cs",
            "Assets/Scripts/BoardGeneration/LevelRecipeDefinition.cs",
            "Assets/Scripts/BoardGeneration/LevelRecipeGenerator.cs",
            "Assets/Scripts/BoardGeneration/LevelRecipeSystemValidator.cs",
            "Assets/Scripts/BoardGeneration/GridCellOccupancy.cs",
            "Assets/Scripts/BoardGeneration/GridMask.cs",
            "Assets/Scripts/BoardGeneration/GridMaskDefinition.cs",
            "Assets/Scripts/BoardGeneration/GridMaskGenerator.cs",
            "Assets/Scripts/BoardGeneration/GridMaskSystemValidator.cs",
            "Assets/Scripts/BoardGeneration/ArchetypeLayout.cs",
            "Assets/Scripts/BoardGeneration/ArchetypePatternDefinition.cs",
            "Assets/Scripts/BoardGeneration/ArchetypeSelector.cs",
            "Assets/Scripts/BoardGeneration/ArchetypeSystemValidator.cs",
            "Assets/Scripts/BoardGeneration/VariationLayout.cs",
            "Assets/Scripts/BoardGeneration/VariationPatternDefinition.cs",
            "Assets/Scripts/BoardGeneration/VariationSelector.cs",
            "Assets/Scripts/BoardGeneration/VariationSystemValidator.cs",
            "Assets/Scripts/BoardGeneration/HolePatternLayout.cs",
            "Assets/Scripts/BoardGeneration/HolePatternDefinition.cs",
            "Assets/Scripts/BoardGeneration/HolePatternSelector.cs",
            "Assets/Scripts/BoardGeneration/HolePatternSystemValidator.cs",
            "Assets/Scripts/BoardGeneration/LayeredBoardLayout.cs",
            "Assets/Scripts/BoardGeneration/LayerBuildDefinition.cs",
            "Assets/Scripts/BoardGeneration/LayerBuilder.cs",
            "Assets/Scripts/BoardGeneration/LayerBuilderSystemValidator.cs",
            "Assets/Editor/VariationValidationBuilder.cs",
            "Assets/Editor/HolePatternValidationBuilder.cs",
            "Assets/Editor/LayerBuilderValidationBuilder.cs",
            "Assets/Scripts/UI/DoorTransitionController.cs",
            "Assets/Scripts/UI/MainMenuNavigationController.cs",
        };

        static ScriptImportRecoveryBuilder()
        {
            EditorApplication.delayCall += TryRecoverIgnoredScripts;
        }

        [MenuItem("MahjongGame/Recover Ignored Scripts")]
        public static void RecoverIgnoredScriptsMenu()
        {
            SessionState.SetBool(RecoverySessionKey, true);
            TryRecoverIgnoredScripts();
        }

        private static void TryRecoverIgnoredScripts()
        {
            if (!SessionState.GetBool(RecoverySessionKey, true))
            {
                return;
            }

            bool anyMissing = false;
            foreach (string scriptPath in RecoveredScriptPaths)
            {
                if (AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath) == null)
                {
                    anyMissing = true;
                    AssetDatabase.ImportAsset(scriptPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
                }
            }

            if (anyMissing)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Debug.Log("[ScriptImportRecoveryBuilder] Reimported recovered scripts. Waiting for compile.");
                return;
            }

            SessionState.SetBool(RecoverySessionKey, false);
            Debug.Log("[ScriptImportRecoveryBuilder] All recovered scripts registered.");
        }
    }
}
#endif
