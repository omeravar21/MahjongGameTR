using System.Reflection;
using System.Text;
using MahjongGame.Board;
using MahjongGame.BoardGeneration;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Rewards
{
    public static class JokerSystemValidator
    {
        public static bool Validate(Transform gameplayRoot, StringBuilder reportBuilder = null)
        {
            if (reportBuilder == null)
            {
                reportBuilder = new StringBuilder();
            }

            bool passed = true;

            if (gameplayRoot == null)
            {
                AppendLine(reportBuilder, "[FAIL] GameplayRoot is missing for joker system validation.");
                return false;
            }

            RewardDirector rewardDirector = gameplayRoot.GetComponent<RewardDirector>();
            JokerTileController jokerTileController = gameplayRoot.GetComponent<JokerTileController>();
            JokerTimerController jokerTimerController = gameplayRoot.GetComponent<JokerTimerController>();

            passed &= ValidateComponents(rewardDirector, jokerTileController, jokerTimerController, reportBuilder);
            passed &= ValidateTypesAndEvents(reportBuilder);
            passed &= ValidateDefinition(reportBuilder);
            passed &= ValidateRegistryBehavior(jokerTileController, reportBuilder);
            passed &= ValidateRewardDirectorWiring(rewardDirector, jokerTileController, reportBuilder);
            passed &= ValidatePipelineJokerAssignments(reportBuilder);
            passed &= ValidateEarlyMatchDetection(rewardDirector, reportBuilder);

            AppendLine(reportBuilder, passed
                ? "[PASS] Joker system validation completed successfully."
                : "[FAIL] Joker system validation found issues.");

            return passed;
        }

        private static bool ValidateComponents(
            RewardDirector rewardDirector,
            JokerTileController jokerTileController,
            JokerTimerController jokerTimerController,
            StringBuilder reportBuilder)
        {
            bool passed = true;

            if (rewardDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] RewardDirector is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] RewardDirector is present on GameplayRoot.");
            }

            if (jokerTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] JokerTileController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] JokerTileController is present on GameplayRoot.");
            }

            if (jokerTimerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] JokerTimerController is missing on GameplayRoot.");
                passed = false;
            }
            else
            {
                AppendLine(reportBuilder, "[PASS] JokerTimerController is present on GameplayRoot.");
            }

            return passed;
        }

        private static bool ValidateTypesAndEvents(StringBuilder reportBuilder)
        {
            bool passed = true;

            AppendLine(reportBuilder, typeof(JokerTileData) != null
                ? "[PASS] JokerTileData type is present."
                : "[FAIL] JokerTileData type is missing.");

            AppendLine(reportBuilder, typeof(JokerTileState) != null
                ? "[PASS] JokerTileState type is present."
                : "[FAIL] JokerTileState type is missing.");

            AppendLine(reportBuilder, typeof(JokerDefinition) != null
                ? "[PASS] JokerDefinition type is present."
                : "[FAIL] JokerDefinition type is missing.");

            passed &= ValidateEventExists(
                typeof(JokerEvents),
                nameof(JokerEvents.JokerTileRegistered),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(JokerEvents),
                nameof(JokerEvents.JokerTileCleared),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(JokerEvents),
                nameof(JokerEvents.JokerRuntimeReset),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(JokerEvents),
                nameof(JokerEvents.JokerEarlyMatchDetected),
                reportBuilder);
            passed &= ValidateEventExists(
                typeof(JokerEvents),
                nameof(JokerEvents.JokerLateMatchDetected),
                reportBuilder);

            return passed;
        }

        private static bool ValidateDefinition(StringBuilder reportBuilder)
        {
            if (JokerDefinition.EarlyMatchWindowSeconds != 60f)
            {
                AppendLine(reportBuilder, "[FAIL] JokerDefinition early match window is not 60 seconds.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] JokerDefinition early match window is 60 seconds.");
            return true;
        }

        private static bool ValidateRegistryBehavior(
            JokerTileController jokerTileController,
            StringBuilder reportBuilder)
        {
            if (jokerTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] JokerTileController is unavailable for registry validation.");
                return false;
            }

            jokerTileController.ResetRuntimeState();

            if (jokerTileController.GetRegisteredJokerTileCount() != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Joker tile registry is not empty after reset.");
                return false;
            }

            TileBoardPosition boardPosition = new TileBoardPosition(new BoardGridCoordinate(1, 1), 0);
            if (!JokerTileData.TryCreate(600, 5, boardPosition, out JokerTileData jokerTileData))
            {
                AppendLine(reportBuilder, "[FAIL] Synthetic JokerTileData could not be created.");
                return false;
            }

            if (!jokerTileController.TryRegisterJokerTile(jokerTileData))
            {
                AppendLine(reportBuilder, "[FAIL] JokerTileController rejected synthetic registration.");
                return false;
            }

            if (jokerTileController.GetRegisteredJokerTileCount() != 1
                || !jokerTileController.IsJokerTile(600))
            {
                AppendLine(reportBuilder, "[FAIL] Joker tile registry count is incorrect after registration.");
                return false;
            }

            if (!jokerTileController.TryGetJokerTileData(600, out JokerTileData registeredData)
                || registeredData.State != JokerTileState.Registered)
            {
                AppendLine(reportBuilder, "[FAIL] Joker tile registry lookup failed after registration.");
                return false;
            }

            if (!jokerTileController.TryClearJokerTile(600)
                || jokerTileController.IsJokerTile(600))
            {
                AppendLine(reportBuilder, "[FAIL] Joker tile clear validation failed.");
                return false;
            }

            jokerTileController.ResetRuntimeState();

            if (jokerTileController.GetRegisteredJokerTileCount() != 0)
            {
                AppendLine(reportBuilder, "[FAIL] Joker tile registry did not reset cleanly.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Joker tile registry registration and reset behave correctly.");
            return true;
        }

        private static bool ValidateRewardDirectorWiring(
            RewardDirector rewardDirector,
            JokerTileController jokerTileController,
            StringBuilder reportBuilder)
        {
            if (rewardDirector == null || jokerTileController == null)
            {
                AppendLine(reportBuilder, "[FAIL] RewardDirector wiring cannot be validated without components.");
                return false;
            }

            if (rewardDirector.GetJokerTileController() != jokerTileController)
            {
                AppendLine(reportBuilder, "[FAIL] RewardDirector is not wired to JokerTileController.");
                return false;
            }

            JokerTimerController jokerTimerController = rewardDirector.GetJokerTimerController();
            if (jokerTimerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] RewardDirector is not wired to JokerTimerController.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] RewardDirector is wired to JokerTileController.");
            AppendLine(reportBuilder, "[PASS] RewardDirector is wired to JokerTimerController.");
            return true;
        }

        private static bool ValidateEarlyMatchDetection(
            RewardDirector rewardDirector,
            StringBuilder reportBuilder)
        {
            if (rewardDirector == null)
            {
                AppendLine(reportBuilder, "[FAIL] RewardDirector is unavailable for early match validation.");
                return false;
            }

            JokerTimerController jokerTimerController = rewardDirector.GetJokerTimerController();
            if (jokerTimerController == null)
            {
                AppendLine(reportBuilder, "[FAIL] JokerTimerController is unavailable for early match validation.");
                return false;
            }

            bool earlyDetected = false;
            bool lateDetected = false;
            JokerEvents.JokerEarlyMatchDetected += HandleEarlyDetected;
            JokerEvents.JokerLateMatchDetected += HandleLateDetected;

            try
            {
                rewardDirector.ResetJokerRuntimeState();
                jokerTimerController.StartSessionForValidation();
                jokerTimerController.AdvanceElapsedTimeForValidation(59f);
                rewardDirector.TryEvaluateJokerMatchForValidation(700, 59f);

                jokerTimerController.AdvanceElapsedTimeForValidation(2f);
                rewardDirector.TryEvaluateJokerMatchForValidation(701, 61f);
            }
            finally
            {
                JokerEvents.JokerEarlyMatchDetected -= HandleEarlyDetected;
                JokerEvents.JokerLateMatchDetected -= HandleLateDetected;
            }

            if (!earlyDetected || !lateDetected)
            {
                AppendLine(reportBuilder, "[FAIL] Joker early/late match detection validation failed.");
                return false;
            }

            if (rewardDirector.EarlyJokerMatchCount != 1 || rewardDirector.LateJokerMatchCount != 1)
            {
                AppendLine(reportBuilder, "[FAIL] Joker match counters are incorrect after validation.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Joker early and late match detection behave correctly.");
            return true;

            void HandleEarlyDetected(JokerEarlyMatchDetectedContext context)
            {
                earlyDetected = context != null && context.ElapsedSessionSeconds <= JokerDefinition.EarlyMatchWindowSeconds;
            }

            void HandleLateDetected(JokerLateMatchDetectedContext context)
            {
                lateDetected = context != null && context.ElapsedSessionSeconds > JokerDefinition.EarlyMatchWindowSeconds;
            }
        }

        private static bool ValidatePipelineJokerAssignments(StringBuilder reportBuilder)
        {
            BoardData boardData = BoardGenerationPipeline.GenerateCandidateBoardData(
                LevelRecipeDefinition.GenerateRecipe(1));

            if (boardData.JokerCount <= 0)
            {
                AppendLine(reportBuilder, "[FAIL] Pipeline candidate board for level 1 has no joker count.");
                return false;
            }

            int jokerAssignmentCount = 0;
            for (int index = 0; index < boardData.TileAssignments.Count; index++)
            {
                if (boardData.TileAssignments[index].IsJoker)
                {
                    jokerAssignmentCount++;
                }
            }

            if (jokerAssignmentCount != boardData.JokerCount)
            {
                AppendLine(
                    reportBuilder,
                    "[FAIL] Pipeline joker assignments do not match recipe joker count.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] Pipeline joker assignments match recipe joker count.");
            return true;
        }

        private static bool ValidateEventExists(
            System.Type eventsType,
            string eventName,
            StringBuilder reportBuilder)
        {
            EventInfo eventInfo = eventsType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
            if (eventInfo == null)
            {
                AppendLine(reportBuilder, "[FAIL] JokerEvents." + eventName + " event is missing.");
                return false;
            }

            AppendLine(reportBuilder, "[PASS] JokerEvents." + eventName + " event is present.");
            return true;
        }

        private static void AppendLine(StringBuilder reportBuilder, string line)
        {
            reportBuilder.AppendLine(line);
        }
    }
}
