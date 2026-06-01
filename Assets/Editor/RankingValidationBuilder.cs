#if UNITY_EDITOR
using System.Text;
using MahjongGame.Ranking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MahjongGame.Editor
{
    public static class RankingValidationBuilder
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenuScene.unity";

        [MenuItem("MahjongGame/Validate Ranking System")]
        public static void ValidateRankingSystem()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = RankingSystemValidator.Validate(reportBuilder);
            passed &= ValidateMainMenuDisplayScene(reportBuilder);

            if (passed)
            {
                Debug.Log("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }

        [MenuItem("MahjongGame/Validate Ranking Architecture")]
        public static void ValidateRankingArchitecture()
        {
            StringBuilder reportBuilder = new StringBuilder();
            bool passed = RankingSystemValidator.Validate(reportBuilder);

            if (passed)
            {
                Debug.Log("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
            else
            {
                Debug.LogWarning("[RankingValidationBuilder] " + reportBuilder.ToString().TrimEnd());
            }
        }

        private static bool ValidateMainMenuDisplayScene(StringBuilder reportBuilder)
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            Transform canvasTransform = FindCanvasTransform(scene);
            if (canvasTransform == null)
            {
                AppendLine(reportBuilder, "[FAIL] Main menu canvas is missing for ranking display validation.");
                return false;
            }

            bool passed = true;

            Transform rankingButton = canvasTransform.Find("TopBar/RankingButton");
            if (rankingButton == null)
            {
                AppendLine(reportBuilder, "[FAIL] Main menu RankingButton is missing.");
                passed = false;
            }
            else if (rankingButton.GetComponent<Button>() == null)
            {
                AppendLine(reportBuilder, "[FAIL] Main menu RankingButton is missing a Button component.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Main menu RankingButton is present.");
            }

            Transform overlayRoot = canvasTransform.Find("MenuOverlayRoot");
            Transform leaderboardPanel = overlayRoot != null ? overlayRoot.Find("LeaderboardPanel") : null;
            Transform contentTransform = leaderboardPanel != null ? leaderboardPanel.Find("Content") : null;
            RankingUIController rankingUiController = contentTransform != null
                ? contentTransform.GetComponent<RankingUIController>()
                : null;

            if (leaderboardPanel == null)
            {
                AppendLine(reportBuilder, "[FAIL] Main menu LeaderboardPanel is missing.");
                passed = false;
            }
            else if (rankingUiController == null)
            {
                AppendLine(reportBuilder, "[FAIL] Main menu LeaderboardPanel Content is missing RankingUIController.");
                passed = false;
            }
            else if (!rankingUiController.HasRequiredLayout())
            {
                AppendLine(reportBuilder, "[FAIL] Main menu RankingUIController is missing required layout nodes.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] Main menu LeaderboardPanel display wiring is present.");
            }

            return passed;
        }

        private static Transform FindCanvasTransform(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == "Canvas_MainMenu")
                {
                    return rootObject.transform;
                }

                Transform canvasTransform = rootObject.transform.Find("Canvas_MainMenu");
                if (canvasTransform != null)
                {
                    return canvasTransform;
                }
            }

            return null;
        }

        private static void AppendLine(StringBuilder reportBuilder, string message)
        {
            reportBuilder.AppendLine(message);
        }
    }
}
#endif
