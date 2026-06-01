using MahjongGame.DailyRewards;
using UnityEngine;

namespace MahjongGame.DailyMissions
{
    public sealed class DailyMissionRewardController : MonoBehaviour
    {
        private void OnEnable()
        {
            DailyMissionEvents.DailyMissionCompleted += HandleDailyMissionCompleted;
        }

        private void OnDisable()
        {
            DailyMissionEvents.DailyMissionCompleted -= HandleDailyMissionCompleted;
        }

        private void HandleDailyMissionCompleted(DailyMissionCompletedContext context)
        {
            if (context == null || !DailyMissionDirector.HasInstance || !DailyRewardDirector.HasInstance)
            {
                return;
            }

            DailyMissionDirector missionDirector = DailyMissionDirector.Instance;
            if (missionDirector.IsSlotRewardClaimed(context.SlotIndex))
            {
                return;
            }

            if (!DailyRewardDirector.Instance.TryGrantMissionCompletionRewards(context.Tier))
            {
                Debug.LogWarning(
                    "[DailyMissionRewardController] Failed to grant mission rewards for slot "
                    + context.SlotIndex
                    + ".");
                return;
            }

            if (missionDirector.TryMarkSlotRewardClaimed(context.SlotIndex))
            {
                Debug.Log(
                    "[DailyMissionRewardController] Mission rewards granted for slot "
                    + context.SlotIndex
                    + " ("
                    + context.MissionType
                    + ", "
                    + context.Tier
                    + ").");
            }
        }
    }
}
