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
            TileMovementEvents.TileMovementStarted += HandleTileMovementStarted;
        }

        private void OnDisable()
        {
            SessionEvents.SessionStarted -= HandleSessionStarted;
            TileMovementEvents.TileMovementStarted -= HandleTileMovementStarted;
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

        public bool TryRevealClosedTile(Tile tile)
        {
            if (tile == null || tile.TileId < 0)
            {
                return false;
            }

            if (!IsClosedTile(tile.TileId))
            {
                return false;
            }

            if (!TryGetClosedTileState(tile.TileId, out ClosedTileState state)
                || state != ClosedTileState.Closed)
            {
                return false;
            }

            return TryTransitionClosedTileState(tile.TileId, ClosedTileState.Revealed, tile);
        }

        public bool RequiresTrayMove(Tile tile)
        {
            if (tile == null || tile.TileId < 0)
            {
                return false;
            }

            return IsClosedTile(tile.TileId)
                && TryGetClosedTileState(tile.TileId, out ClosedTileState state)
                && state == ClosedTileState.Revealed;
        }

        internal bool TrySetClosedTileStateForValidation(int tileId, ClosedTileState newState)
        {
            return TryTransitionClosedTileState(tileId, newState);
        }

        private bool TryTransitionClosedTileState(
            int tileId,
            ClosedTileState newState,
            Tile tile = null)
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

            if (tile != null && newState == ClosedTileState.Revealed)
            {
                tile.SetState(TileState.Revealed);
            }

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

        private void HandleTileMovementStarted(TileMovementRequest request)
        {
            if (request == null || request.Tile == null)
            {
                return;
            }

            int tileId = request.Tile.TileId;
            if (!IsClosedTile(tileId))
            {
                return;
            }

            if (TryGetClosedTileState(tileId, out ClosedTileState state)
                && state == ClosedTileState.Revealed)
            {
                TryUnregisterClosedTileLeavingBoard(tileId);
            }
        }

        private void TryUnregisterClosedTileLeavingBoard(int tileId)
        {
            _registeredClosedTiles.Remove(tileId);
        }
    }
}
