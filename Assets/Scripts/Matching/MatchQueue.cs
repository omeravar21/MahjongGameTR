using System.Collections.Generic;
using MahjongGame.Tiles;

namespace MahjongGame.Matching
{
    public sealed class MatchQueue
    {
        private readonly Queue<MatchRequest> _queue = new Queue<MatchRequest>();

        public int Count => _queue.Count;

        public bool Enqueue(MatchRequest request)
        {
            if (request == null)
            {
                return false;
            }

            if (ContainsTiles(request.FirstTile, request.SecondTile))
            {
                return false;
            }

            _queue.Enqueue(request);
            return true;
        }

        public bool TryDequeue(out MatchRequest request)
        {
            if (_queue.Count == 0)
            {
                request = null;
                return false;
            }

            request = _queue.Dequeue();
            return true;
        }

        public bool ContainsTiles(Tile firstTile, Tile secondTile)
        {
            if (firstTile == null || secondTile == null)
            {
                return false;
            }

            foreach (MatchRequest pendingRequest in _queue)
            {
                if (ReferencesSamePair(pendingRequest, firstTile, secondTile))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            _queue.Clear();
        }

        internal static bool ReferencesSamePair(MatchRequest request, Tile firstTile, Tile secondTile)
        {
            if (request == null || firstTile == null || secondTile == null)
            {
                return false;
            }

            Tile requestFirstTile = request.FirstTile;
            Tile requestSecondTile = request.SecondTile;

            return (requestFirstTile == firstTile && requestSecondTile == secondTile)
                || (requestFirstTile == secondTile && requestSecondTile == firstTile);
        }
    }
}
