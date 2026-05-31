using MahjongGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MahjongGame.Tiles
{
    public sealed class TileSelectionController : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private TileSelectabilityChecker selectabilityChecker;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            if (selectabilityChecker == null)
            {
                selectabilityChecker = GetComponent<TileSelectabilityChecker>();
            }
        }

        private void Update()
        {
            if (!ShouldProcessInput())
            {
                return;
            }

            if (!TryGetPointerPressPosition(out Vector2 screenPosition))
            {
                return;
            }

            if (!TryRaycastTile(screenPosition, out Tile tile))
            {
                return;
            }

            RequestSelection(tile);
        }

        public bool RequestSelection(Tile tile)
        {
            if (!CanReceiveSelectionRequest(tile))
            {
                return false;
            }

            if (!IsSelectable(tile))
            {
                return false;
            }

            TileSelectionRequest request = new TileSelectionRequest(
                tile,
                tile.GetIdentity(),
                tile.State);
            TileSelectionEvents.RaiseTileSelectionRequested(request);
            return true;
        }

        public static bool CanReceiveSelectionRequest(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            switch (tile.State)
            {
                case TileState.OnBoard:
                case TileState.Closed:
                case TileState.Revealed:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsSelectable(Tile tile)
        {
            if (selectabilityChecker != null)
            {
                return selectabilityChecker.TryValidate(tile, out _);
            }

            return TileSelectabilityChecker.TryValidate(null, tile, out _);
        }

        private bool ShouldProcessInput()
        {
            if (GameState.IsInitialized && GameState.Current == AppGameState.Gameplay)
            {
                return true;
            }

            return SceneLoadController.HasInstance
                && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneLoadController.GameSceneName;
        }

        private bool TryGetPointerPressPosition(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null)
            {
                for (int touchIndex = 0; touchIndex < Touchscreen.current.touches.Count; touchIndex++)
                {
                    TouchControl touch = Touchscreen.current.touches[touchIndex];
                    if (touch.press.wasPressedThisFrame)
                    {
                        screenPosition = touch.position.ReadValue();
                        return true;
                    }
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                return true;
            }

            screenPosition = default;
            return false;
        }

        private bool TryRaycastTile(Vector2 screenPosition, out Tile tile)
        {
            tile = null;

            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            if (gameplayCamera == null)
            {
                return false;
            }

            Vector3 worldPoint = gameplayCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, -gameplayCamera.transform.position.z));
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider == null)
            {
                return false;
            }

            tile = hit.collider.GetComponentInParent<Tile>();
            return tile != null;
        }
    }
}
