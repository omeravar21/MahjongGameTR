using UnityEngine;

namespace MahjongGame.Tray
{
    [DefaultExecutionOrder(5)]
    public sealed class TrayRootController : MonoBehaviour
    {
        [SerializeField] private Transform trayRootTransform;

        public Transform TrayRootTransform => trayRootTransform != null ? trayRootTransform : transform;

        private void Awake()
        {
            if (trayRootTransform == null)
            {
                trayRootTransform = transform;
            }

            if (!HasRequiredTrayHierarchy(trayRootTransform))
            {
                BuildTrayHierarchy(trayRootTransform);
            }

            if (!TrayFrameVisualController.HasRequiredFrameVisual(trayRootTransform))
            {
                TrayFrameVisualController.BuildFrameVisual(trayRootTransform);
            }

            Transform trayContainer = EnsureTrayContainer(trayRootTransform);
            if (!TraySlotVisualController.HasAllSlotVisuals(trayContainer))
            {
                TraySlotVisualController.BuildAllSlotVisuals(trayContainer);
            }

            EnforceTrayHierarchyOrder(trayRootTransform);

            if (GetComponent<TrayCapacityController>() == null)
            {
                gameObject.AddComponent<TrayCapacityController>();
            }
        }

        public static bool HasRequiredTrayHierarchy(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                return false;
            }

            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer == null)
            {
                return false;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                if (trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex)) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static void BuildTrayHierarchy(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                Debug.LogWarning("[TrayRootController] TrayRoot transform is not available.");
                return;
            }

            Transform trayContainer = EnsureTrayContainer(trayRoot);
            EnsureSlotAnchors(trayContainer);
            TraySlotVisualController.BuildAllSlotVisuals(trayContainer);
        }

        public static Transform EnsureTrayContainer(Transform trayRoot)
        {
            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            if (trayContainer != null)
            {
                return trayContainer;
            }

            GameObject containerObject = new GameObject(TrayRootDefinition.TrayContainerName);
            trayContainer = containerObject.transform;
            trayContainer.SetParent(trayRoot, false);
            trayContainer.localPosition = TrayRootLayout.GetTrayContainerLocalPosition();
            trayContainer.localRotation = Quaternion.identity;
            trayContainer.localScale = Vector3.one;
            return trayContainer;
        }

        public static void EnsureSlotAnchors(Transform trayContainer)
        {
            if (trayContainer == null)
            {
                return;
            }

            for (int slotIndex = 0; slotIndex < TrayRootDefinition.SlotCount; slotIndex++)
            {
                string slotName = TrayRootDefinition.GetSlotName(slotIndex);
                Transform slotTransform = trayContainer.Find(slotName);
                if (slotTransform == null)
                {
                    GameObject slotObject = new GameObject(slotName);
                    slotTransform = slotObject.transform;
                    slotTransform.SetParent(trayContainer, false);
                }

                slotTransform.localPosition = TrayRootLayout.GetSlotLocalPosition(slotIndex);
                slotTransform.localRotation = Quaternion.identity;
                slotTransform.localScale = Vector3.one;
            }
        }

        public static void EnforceTrayHierarchyOrder(Transform trayRoot)
        {
            if (trayRoot == null)
            {
                return;
            }

            Transform frameRoot = trayRoot.Find(TrayRootDefinition.FrameRootName);
            Transform trayContainer = trayRoot.Find(TrayRootDefinition.TrayContainerName);
            int nextSiblingIndex = 0;

            if (frameRoot != null)
            {
                frameRoot.SetSiblingIndex(nextSiblingIndex);
                nextSiblingIndex++;
            }

            if (trayContainer != null)
            {
                trayContainer.SetSiblingIndex(nextSiblingIndex);
            }
        }

        public Transform GetTrayContainer()
        {
            return TrayRootTransform.Find(TrayRootDefinition.TrayContainerName);
        }

        public Transform GetSlotTransform(int slotIndex)
        {
            if (!TrayRootDefinition.IsValidSlotIndex(slotIndex))
            {
                Debug.LogWarning("[TrayRootController] Slot index out of range: " + slotIndex);
                return null;
            }

            Transform trayContainer = GetTrayContainer();
            if (trayContainer == null)
            {
                return null;
            }

            return trayContainer.Find(TrayRootDefinition.GetSlotName(slotIndex));
        }
    }
}
