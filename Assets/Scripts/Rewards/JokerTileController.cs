using System.Collections.Generic;
using MahjongGame.Session;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.Rewards
{
    public sealed class JokerTileController : MonoBehaviour
    {
        private readonly Dictionary<int, JokerTileData> _registeredJokerTiles = new Dictionary<int, JokerTileData>();

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
        }

        public int GetRegisteredJokerTileCount()
        {
            return _registeredJokerTiles.Count;
        }

        public bool IsJokerTile(int tileId)
        {
            return _registeredJokerTiles.ContainsKey(tileId);
        }

        public bool TryGetJokerTileData(int tileId, out JokerTileData jokerTileData)
        {
            return _registeredJokerTiles.TryGetValue(tileId, out jokerTileData);
        }

        public bool TryGetJokerTileState(int tileId, out JokerTileState state)
        {
            if (_registeredJokerTiles.TryGetValue(tileId, out JokerTileData data))
            {
                state = data.State;
                return true;
            }

            state = JokerTileState.Cleared;
            return false;
        }

        public bool TryRegisterJokerTile(Tile tile)
        {
            if (!JokerTileData.TryCreate(tile, out JokerTileData jokerTileData))
            {
                return false;
            }

            return TryRegisterJokerTile(jokerTileData);
        }

        public bool TryRegisterJokerTile(JokerTileData jokerTileData)
        {
            if (jokerTileData == null || jokerTileData.TileId < 0)
            {
                return false;
            }

            if (_registeredJokerTiles.ContainsKey(jokerTileData.TileId))
            {
                return true;
            }

            _registeredJokerTiles[jokerTileData.TileId] = jokerTileData;
            JokerEvents.RaiseJokerTileRegistered(new JokerTileRegisteredContext(jokerTileData));
            return true;
        }

        public bool TryClearJokerTile(int tileId)
        {
            if (!_registeredJokerTiles.TryGetValue(tileId, out JokerTileData existingData))
            {
                return false;
            }

            if (existingData.State == JokerTileState.Cleared)
            {
                return true;
            }

            _registeredJokerTiles[tileId] = existingData.WithState(JokerTileState.Cleared);
            _registeredJokerTiles.Remove(tileId);
            JokerEvents.RaiseJokerTileCleared(new JokerTileClearedContext(tileId));
            return true;
        }

        internal bool TrySetJokerTileStateForValidation(int tileId, JokerTileState newState)
        {
            if (!_registeredJokerTiles.TryGetValue(tileId, out JokerTileData existingData))
            {
                return false;
            }

            if (existingData.State == newState)
            {
                return true;
            }

            if (newState == JokerTileState.Cleared)
            {
                return TryClearJokerTile(tileId);
            }

            _registeredJokerTiles[tileId] = existingData.WithState(newState);
            return true;
        }

        public void ResetRuntimeState()
        {
            if (_registeredJokerTiles.Count == 0)
            {
                return;
            }

            _registeredJokerTiles.Clear();
            JokerEvents.RaiseJokerRuntimeReset();
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            ResetRuntimeState();
        }
    }
}
