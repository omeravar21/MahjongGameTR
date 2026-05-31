using UnityEngine;

namespace MahjongGame.Boosters
{
    public sealed class BoosterController : MonoBehaviour
    {
        [SerializeField] private BoosterEconomyDirector economyDirector;
        [SerializeField] private ShuffleBooster shuffleBooster;

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
        }
    }
}
