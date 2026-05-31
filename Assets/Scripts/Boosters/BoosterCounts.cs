using MahjongGame.Core.Save;

namespace MahjongGame.Boosters
{
    public sealed class BoosterCounts
    {
        public int Shuffle { get; set; }

        public int Undo { get; set; }

        public int Hint { get; set; }

        public static BoosterCounts CreateDefault()
        {
            return new BoosterCounts
            {
                Shuffle = BoosterDefinition.StartingShuffleCount,
                Undo = BoosterDefinition.StartingUndoCount,
                Hint = BoosterDefinition.StartingHintCount
            };
        }

        public static BoosterCounts FromSave(BoosterCountsSaveData saveData)
        {
            BoosterCounts counts = CreateDefault();
            if (saveData == null)
            {
                return counts;
            }

            counts.Shuffle = saveData.shuffle;
            counts.Undo = saveData.undo;
            counts.Hint = saveData.hint;
            return counts;
        }

        public void WriteToSave(BoosterCountsSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.shuffle = Shuffle;
            saveData.undo = Undo;
            saveData.hint = Hint;
        }

        public int GetCount(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.Shuffle:
                    return Shuffle;
                case BoosterType.Undo:
                    return Undo;
                case BoosterType.Hint:
                    return Hint;
                default:
                    return 0;
            }
        }

        public void SetCount(BoosterType boosterType, int count)
        {
            switch (boosterType)
            {
                case BoosterType.Shuffle:
                    Shuffle = count;
                    break;
                case BoosterType.Undo:
                    Undo = count;
                    break;
                case BoosterType.Hint:
                    Hint = count;
                    break;
            }
        }

        public bool TryConsume(BoosterType boosterType)
        {
            int currentCount = GetCount(boosterType);
            if (currentCount <= 0)
            {
                return false;
            }

            SetCount(boosterType, currentCount - 1);
            return true;
        }

        public void Grant(BoosterType boosterType, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            SetCount(boosterType, GetCount(boosterType) + amount);
        }
    }
}
