using System;
using MahjongGame.DailyBoard;

namespace MahjongGame.DailyMissions
{
    /// <summary>
    /// Static daily mission rules. UTC calendar day drives global identity so all players share the same daily mission set.
    /// </summary>
    public static class DailyMissionDefinition
    {
        public const int EasyMissionCount = 2;
        public const int MediumMissionCount = 2;
        public const int HardMissionCount = 1;

        public static int GetUtcDayId(DateTime utcNow)
        {
            return DailyBoardDefinition.GetUtcDayId(utcNow);
        }

        public static int ComputeMissionSeed(int dayId)
        {
            if (dayId <= 0)
            {
                return 0;
            }

            unchecked
            {
                int hash = (dayId * 314159265) ^ 271828182;
                hash = (hash * 161803399) ^ 141421356;
                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
        }

        public static int ResolveTargetValue(DailyMissionType missionType, DailyMissionTier tier)
        {
            switch (missionType)
            {
                case DailyMissionType.CompleteLevels:
                    return tier switch
                    {
                        DailyMissionTier.Easy => 1,
                        DailyMissionTier.Medium => 2,
                        DailyMissionTier.Hard => 3,
                        _ => 1
                    };

                case DailyMissionType.CreateCombos:
                    return tier switch
                    {
                        DailyMissionTier.Easy => 3,
                        DailyMissionTier.Medium => 5,
                        DailyMissionTier.Hard => 8,
                        _ => 1
                    };

                case DailyMissionType.FinishWithoutBoosters:
                    return tier switch
                    {
                        DailyMissionTier.Easy => 1,
                        DailyMissionTier.Medium => 2,
                        DailyMissionTier.Hard => 1,
                        _ => 1
                    };

                case DailyMissionType.CompleteDailyBoard:
                    return 1;

                case DailyMissionType.MatchRewardJokers:
                    return tier switch
                    {
                        DailyMissionTier.Easy => 1,
                        DailyMissionTier.Medium => 2,
                        DailyMissionTier.Hard => 3,
                        _ => 1
                    };

                case DailyMissionType.FinishUnderTargetTime:
                    return tier switch
                    {
                        DailyMissionTier.Medium => 1,
                        DailyMissionTier.Hard => 2,
                        _ => 1
                    };

                default:
                    return 1;
            }
        }

        public static bool IsEligibleForTier(DailyMissionType missionType, DailyMissionTier tier)
        {
            switch (missionType)
            {
                case DailyMissionType.CompleteDailyBoard:
                    return tier == DailyMissionTier.Hard;

                case DailyMissionType.FinishUnderTargetTime:
                    return tier == DailyMissionTier.Medium || tier == DailyMissionTier.Hard;

                default:
                    return true;
            }
        }

        public static DailyMissionType[] GetAllMissionTypes()
        {
            return new[]
            {
                DailyMissionType.CompleteLevels,
                DailyMissionType.CreateCombos,
                DailyMissionType.FinishWithoutBoosters,
                DailyMissionType.CompleteDailyBoard,
                DailyMissionType.MatchRewardJokers,
                DailyMissionType.FinishUnderTargetTime
            };
        }
    }
}
