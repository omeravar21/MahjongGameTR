using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class BoosterController : MonoBehaviour
    {
        [SerializeField] private BoosterEconomyDirector economyDirector;
        [SerializeField] private ShuffleBooster shuffleBooster;
        [SerializeField] private UndoBooster undoBooster;

        private void Awake()
        {
            ResolveComponents();
        }

        public BoosterEconomyDirector GetEconomyDirector()
        {
            ResolveComponents();
            return economyDirector;
        }

        public ShuffleBooster GetShuffleBooster()
        {
            ResolveComponents();
            return shuffleBooster;
        }

        public UndoBooster GetUndoBooster()
        {
            ResolveComponents();
            return undoBooster;
        }

        public bool TryExecuteShuffle()
        {
            ResolveComponents();
            if (economyDirector == null || shuffleBooster == null)
            {
                return false;
            }

            if (!economyDirector.TryConsume(BoosterType.Shuffle))
            {
                return false;
            }

            if (shuffleBooster.TryExecuteShuffle(out _))
            {
                return true;
            }

            economyDirector.TryGrant(BoosterType.Shuffle, 1);
            return false;
        }

        public bool TryExecuteUndo()
        {
            ResolveComponents();
            if (economyDirector == null || undoBooster == null)
            {
                return false;
            }

            if (!economyDirector.TryConsume(BoosterType.Undo))
            {
                return false;
            }

            if (undoBooster.TryExecuteUndo())
            {
                return true;
            }

            economyDirector.TryGrant(BoosterType.Undo, 1);
            return false;
        }

        private void ResolveComponents()
        {
            if (economyDirector == null)
            {
                economyDirector = GetComponentInParent<BoosterEconomyDirector>();
            }

            if (shuffleBooster == null)
            {
                shuffleBooster = GetComponent<ShuffleBooster>();
            }

            if (undoBooster == null)
            {
                undoBooster = GetComponent<UndoBooster>();
            }
        }
    }
}
