using System.Collections.Generic;
using MahjongGame.Session;
using MahjongGame.Tiles;
using UnityEngine;

namespace MahjongGame.ClosedTiles
{
    public sealed class ClosedTileController : MonoBehaviour
    {
        private readonly Dictionary<int, ClosedTileData> _registeredClosedTiles = new Dictionary<int, ClosedTileData>();

        public bool HasRevealedClosedTile
        {
            get
            {
                foreach (KeyValuePair<int, ClosedTileData> entry in _registeredClosedTiles)
                {
                    if (entry.Value.State == ClosedTileState.Revealed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void OnEnable()
        {
            SessionEvents.SessionStarted += HandleSessionStarted;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
        }

        public int GetRegisteredClosedTileCount()
        {
            return _registeredClosedTiles.Count;
        }

        public bool IsClosedTile(int tileId)
        {
            return _registeredClosedTiles.ContainsKey(tileId);
        }

        public bool TryGetClosedTileData(int tileId, out ClosedTileData closedTileData)
        {
            return _registeredClosedTiles.TryGetValue(tileId, out closedTileData);
        }

        public bool TryGetClosedTileState(int tileId, out ClosedTileState state)
        {
            if (_registeredClosedTiles.TryGetValue(tileId, out ClosedTileData closedTileData))
            {
                state = closedTileData.State;
                return true;
            }

            state = ClosedTileState.Closed;
            return false;
        }

        public bool TryRegisterClosedTile(Tile tile)
        {
            if (!ClosedTileData.TryCreate(tile, out ClosedTileData closedTileData))
            {
                return false;
            }

            return TryRegisterClosedTile(closedTileData);
        }

        public bool TryRegisterClosedTile(ClosedTileData closedTileData)
        {
            if (closedTileData == null || closedTileData.TileId < 0)
            {
                return false;
            }

            if (_registeredClosedTiles.ContainsKey(closedTileData.TileId))
            {
                return true;
            }

            _registeredClosedTiles[closedTileData.TileId] = closedTileData;
            ClosedTileEvents.RaiseClosedTileRegistered(new ClosedTileRegisteredContext(closedTileData));
            return true;
        }

        internal bool TrySetClosedTileStateForValidation(int tileId, ClosedTileState newState)
        {
            if (!_registeredClosedTiles.TryGetValue(tileId, out ClosedTileData existingData))
            {
                return false;
            }

            if (existingData.State == newState)
            {
                return true;
            }

            ClosedTileState previousState = existingData.State;
            ClosedTileData updatedData = existingData.WithState(newState);
            _registeredClosedTiles[tileId] = updatedData;
            ClosedTileEvents.RaiseClosedTileStateChanged(
                new ClosedTileStateChangedContext(tileId, previousState, newState));
            return true;
        }

        public void ResetRuntimeState()
        {
            if (_registeredClosedTiles.Count == 0)
            {
                return;
            }

            _registeredClosedTiles.Clear();
            ClosedTileEvents.RaiseClosedTileRuntimeReset();
        }

        private void HandleSessionStarted(SessionStartedContext context)
        {
            ResetRuntimeState();
        }
    }
}
