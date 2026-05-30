using UnityEngine;

namespace MahjongGame.Board
{
    public sealed class BoardRootController : MonoBehaviour
    {
        public const int MaxLayerCount = 4;

        [SerializeField] private Transform boardRootTransform;

        public Transform BoardRootTransform => boardRootTransform != null ? boardRootTransform : transform;

        private void Awake()
        {
            if (boardRootTransform == null)
            {
                boardRootTransform = transform;
            }

            if (!HasRequiredBoardHierarchy(boardRootTransform))
            {
                BuildBoardContainers(boardRootTransform);
            }
        }

        public static bool HasRequiredBoardHierarchy(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                return false;
            }

            for (int layerIndex = 0; layerIndex < MaxLayerCount; layerIndex++)
            {
                if (boardRoot.Find(GetLayerContainerName(layerIndex)) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static void BuildBoardContainers(Transform boardRoot)
        {
            if (boardRoot == null)
            {
                Debug.LogWarning("[BoardRootController] BoardRoot transform is not available.");
                return;
            }

            for (int layerIndex = 0; layerIndex < MaxLayerCount; layerIndex++)
            {
                string containerName = GetLayerContainerName(layerIndex);
                Transform existingContainer = boardRoot.Find(containerName);
                if (existingContainer != null)
                {
                    continue;
                }

                GameObject containerObject = new GameObject(containerName);
                containerObject.transform.SetParent(boardRoot, false);
                containerObject.transform.localPosition = Vector3.zero;
                containerObject.transform.localRotation = Quaternion.identity;
                containerObject.transform.localScale = Vector3.one;
            }
        }

        public static string GetLayerContainerName(int layerIndex)
        {
            return "LayerContainer_" + layerIndex;
        }

        public Transform GetLayerContainer(int layerIndex)
        {
            if (layerIndex < 0 || layerIndex >= MaxLayerCount)
            {
                Debug.LogWarning("[BoardRootController] Layer index out of range: " + layerIndex);
                return null;
            }

            Transform container = BoardRootTransform.Find(GetLayerContainerName(layerIndex));
            if (container == null)
            {
                Debug.LogWarning("[BoardRootController] Missing layer container: " + GetLayerContainerName(layerIndex));
            }

            return container;
        }
    }
}
