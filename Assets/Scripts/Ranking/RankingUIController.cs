using UnityEngine;
using UnityEngine.UI;

namespace MahjongGame.Ranking
{
    public sealed class RankingUIController : MonoBehaviour
    {
        public const string SummaryTextName = "LeaderboardSummaryText";
        public const string EntryListRootName = "LeaderboardEntryListRoot";

        private static readonly Color TextColor = new Color(0.95f, 0.91f, 0.84f, 1f);
        private static readonly Color AccentTextColor = new Color(0.75f, 0.65f, 0.45f, 1f);
        private static readonly Color LocalPlayerColor = new Color(0.88f, 0.78f, 0.52f, 1f);

        private Text _summaryText;
        private Transform _entryListRoot;
        private bool _isVisible;

        private void OnEnable()
        {
            RankingEvents.GlobalPerformanceScoreChanged += HandleGlobalPerformanceScoreChanged;
            RankingEvents.GlobalRankChanged += HandleGlobalRankChanged;

            if (!HasRequiredLayout())
            {
                BuildLayout();
            }

            BindReferences();
        }

        private void OnDisable()
        {
            RankingEvents.GlobalPerformanceScoreChanged -= HandleGlobalPerformanceScoreChanged;
            RankingEvents.GlobalRankChanged -= HandleGlobalRankChanged;
        }

        public bool HasRequiredLayout()
        {
            Transform summaryTransform = transform.Find(SummaryTextName);
            Transform entryListTransform = transform.Find(EntryListRootName);
            return summaryTransform != null && entryListTransform != null;
        }

        public void BuildLayout()
        {
            ClearChildren();

            CreateSummaryText();
            CreateEntryListRoot();
            BindReferences();
        }

        public void RefreshFromDirector()
        {
            if (!RankingDirector.HasInstance)
            {
                SetSummaryText("Global Leaderboard", 0, 0);
                ClearEntryRows();
                return;
            }

            RankingDirector.Instance.RefreshGlobalRank();
            LeaderboardData leaderboardData = RankingDirector.Instance.GetLeaderboardData();
            SetSummaryText(
                "Global Leaderboard",
                leaderboardData.LocalPlayerRank,
                leaderboardData.LocalPlayerScore);
            RenderEntries(leaderboardData);
        }

        internal void SetVisibleForValidation(bool isVisible)
        {
            _isVisible = isVisible;
        }

        private void HandleGlobalPerformanceScoreChanged(GlobalPerformanceScoreChangedContext context)
        {
            if (!_isVisible)
            {
                return;
            }

            RefreshFromDirector();
        }

        private void HandleGlobalRankChanged(GlobalRankChangedContext context)
        {
            if (!_isVisible)
            {
                return;
            }

            RefreshFromDirector();
        }

        private void BindReferences()
        {
            _summaryText = transform.Find(SummaryTextName)?.GetComponent<Text>();
            _entryListRoot = transform.Find(EntryListRootName);
        }

        private void CreateSummaryText()
        {
            GameObject textObject = CreateRectObject(SummaryTextName, transform);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 1f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.pivot = new Vector2(0.5f, 1f);
            textRect.anchoredPosition = new Vector2(0f, -8f);
            textRect.sizeDelta = new Vector2(-32f, 120f);

            Text text = textObject.AddComponent<Text>();
            text.alignment = TextAnchor.UpperLeft;
            text.color = TextColor;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = "Global Leaderboard";
        }

        private void CreateEntryListRoot()
        {
            GameObject listObject = CreateRectObject(EntryListRootName, transform);
            RectTransform listRect = listObject.GetComponent<RectTransform>();
            listRect.anchorMin = Vector2.zero;
            listRect.anchorMax = Vector2.one;
            listRect.offsetMin = new Vector2(16f, 16f);
            listRect.offsetMax = new Vector2(-16f, -140f);
        }

        private void SetSummaryText(string title, int localRank, long localScore)
        {
            if (_summaryText == null)
            {
                return;
            }

            _summaryText.text = title
                + "\nYour Rank: "
                + (localRank > 0 ? localRank.ToString() : "-")
                + "\nYour Score: "
                + localScore;
        }

        private void RenderEntries(LeaderboardData leaderboardData)
        {
            ClearEntryRows();

            if (_entryListRoot == null || leaderboardData == null || leaderboardData.Entries == null)
            {
                return;
            }

            const float rowHeight = 44f;
            for (int index = 0; index < leaderboardData.Entries.Length; index++)
            {
                LeaderboardEntry entry = leaderboardData.Entries[index];
                CreateEntryRow(entry, index, rowHeight);
            }
        }

        private void CreateEntryRow(LeaderboardEntry entry, int index, float rowHeight)
        {
            GameObject rowObject = CreateRectObject("Entry_" + entry.RankPosition, _entryListRoot);
            RectTransform rowRect = rowObject.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = new Vector2(0f, -(index * rowHeight));
            rowRect.sizeDelta = new Vector2(0f, rowHeight - 4f);

            Text rowText = rowObject.AddComponent<Text>();
            rowText.text = "#"
                + entry.RankPosition
                + "  "
                + entry.DisplayName
                + "  "
                + entry.GlobalPerformanceScore;
            rowText.alignment = TextAnchor.MiddleLeft;
            rowText.color = entry.IsLocalPlayer ? LocalPlayerColor : TextColor;
            rowText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            rowText.fontSize = entry.IsLocalPlayer ? 26 : 22;
            rowText.horizontalOverflow = HorizontalWrapMode.Overflow;
            rowText.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private void ClearEntryRows()
        {
            if (_entryListRoot == null)
            {
                return;
            }

            for (int index = _entryListRoot.childCount - 1; index >= 0; index--)
            {
                Transform child = _entryListRoot.GetChild(index);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void ClearChildren()
        {
            for (int index = transform.childCount - 1; index >= 0; index--)
            {
                Transform child = transform.GetChild(index);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }
    }
}
