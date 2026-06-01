using System;
using System.Collections.Generic;

namespace MahjongGame.DailyMissions
{
    public static class DailyMissionSetGenerator
    {
        private static readonly DailyMissionTier[] TierLayout =
        {
            DailyMissionTier.Easy,
            DailyMissionTier.Easy,
            DailyMissionTier.Medium,
            DailyMissionTier.Medium,
            DailyMissionTier.Hard
        };

        public static DailyMissionSet GenerateSet(int dayId, int missionSeed)
        {
            if (dayId <= 0 || missionSeed <= 0)
            {
                return DailyMissionSet.Empty;
            }

            DailyMissionType[] shuffledTypes = ShuffleMissionTypes(missionSeed);
            DailyMissionType[] assignedTypes = AssignTypesToTiers(shuffledTypes);
            DailyMissionEntry[] entries = new DailyMissionEntry[DailyMissionSet.SlotCount];

            for (int i = 0; i < DailyMissionSet.SlotCount; i++)
            {
                DailyMissionTier tier = TierLayout[i];
                DailyMissionType missionType = assignedTypes[i];
                int targetValue = DailyMissionDefinition.ResolveTargetValue(missionType, tier);
                DailyMissionSlot slot = new DailyMissionSlot(i, missionType, tier, targetValue);
                entries[i] = new DailyMissionEntry(slot, new DailyMissionProgress());
            }

            return new DailyMissionSet(dayId, missionSeed, entries);
        }

        public static DailyMissionType[] ExtractSlotTypes(DailyMissionSet missionSet)
        {
            if (missionSet == null || missionSet.Entries == null || missionSet.Entries.Length != DailyMissionSet.SlotCount)
            {
                return new DailyMissionType[DailyMissionSet.SlotCount];
            }

            DailyMissionType[] slotTypes = new DailyMissionType[DailyMissionSet.SlotCount];
            for (int i = 0; i < DailyMissionSet.SlotCount; i++)
            {
                slotTypes[i] = missionSet.Entries[i].Slot.MissionType;
            }

            return slotTypes;
        }

        private static DailyMissionType[] AssignTypesToTiers(DailyMissionType[] shuffledTypes)
        {
            DailyMissionType[] assigned = new DailyMissionType[DailyMissionSet.SlotCount];
            HashSet<DailyMissionType> usedTypes = new HashSet<DailyMissionType>();

            for (int slotIndex = 0; slotIndex < TierLayout.Length; slotIndex++)
            {
                DailyMissionTier tier = TierLayout[slotIndex];
                DailyMissionType selectedType = DailyMissionType.CompleteLevels;

                if (!TryPickType(shuffledTypes, tier, usedTypes, out selectedType)
                    && !TryPickType(DailyMissionDefinition.GetAllMissionTypes(), tier, usedTypes, out selectedType))
                {
                    selectedType = ResolveFallbackType(tier, usedTypes);
                }

                assigned[slotIndex] = selectedType;
                usedTypes.Add(selectedType);
            }

            return assigned;
        }

        private static bool TryPickType(
            DailyMissionType[] candidates,
            DailyMissionTier tier,
            HashSet<DailyMissionType> usedTypes,
            out DailyMissionType selectedType)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                DailyMissionType candidate = candidates[i];
                if (usedTypes.Contains(candidate))
                {
                    continue;
                }

                if (!DailyMissionDefinition.IsEligibleForTier(candidate, tier))
                {
                    continue;
                }

                selectedType = candidate;
                return true;
            }

            selectedType = DailyMissionType.CompleteLevels;
            return false;
        }

        private static DailyMissionType ResolveFallbackType(DailyMissionTier tier, HashSet<DailyMissionType> usedTypes)
        {
            DailyMissionType[] allTypes = DailyMissionDefinition.GetAllMissionTypes();
            for (int i = 0; i < allTypes.Length; i++)
            {
                DailyMissionType candidate = allTypes[i];
                if (usedTypes.Contains(candidate))
                {
                    continue;
                }

                if (DailyMissionDefinition.IsEligibleForTier(candidate, tier))
                {
                    return candidate;
                }
            }

            return DailyMissionType.CompleteLevels;
        }

        private static DailyMissionType[] ShuffleMissionTypes(int missionSeed)
        {
            DailyMissionType[] types = DailyMissionDefinition.GetAllMissionTypes();
            DailyMissionType[] shuffled = new DailyMissionType[types.Length];
            Array.Copy(types, shuffled, types.Length);

            for (int i = shuffled.Length - 1; i > 0; i--)
            {
                int swapIndex = PositiveMod(MixSeed(missionSeed, i + 17), i + 1);
                DailyMissionType temp = shuffled[i];
                shuffled[i] = shuffled[swapIndex];
                shuffled[swapIndex] = temp;
            }

            return shuffled;
        }

        private static int PositiveMod(int value, int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            int mod = value % count;
            return mod < 0 ? mod + count : mod;
        }

        private static int MixSeed(int seed, int salt)
        {
            unchecked
            {
                int hash = (seed * 1664525) + (salt * 1013904223);
                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
        }
    }
}
