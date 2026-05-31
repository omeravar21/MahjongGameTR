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
