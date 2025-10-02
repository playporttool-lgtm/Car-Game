using UnityEngine;
using UnityEngine.UI;

public class scroll_button : MonoBehaviour
{
    [Header("UI References")]
    public ScrollRect scrollRect;         // Your ScrollRect
    public Button leftButton;             // Left button
    public Button rightButton;            // Right button
    public RectTransform content;         // The Content inside the ScrollView
    public RectTransform[] items;         // All the buttons/items inside content

    [Header("Scaling Settings")]
    public float focusedScale = 1.0f;     // Scale of the focused item
    public float unfocusedScale = 0.7f;   // Scale of non-focused items
    public float scaleSmooth = 10f;       // Speed of scale transition

    [Header("Scroll Settings")]
    public float scrollSmooth = 10f;      // Speed of scroll snapping

    [Header("Focus Point Settings")]
    [Tooltip("Normalized 0 = left, 0.5 = center, 1 = right of viewport")]
    [Range(0f, 1f)]
    public float focusPoint = 0.5f;       // Where the focus line is inside the viewport

    private int currentIndex = 0;         // Index of the focused item
    private RectTransform viewport;

    private void Awake()
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (content == null && scrollRect != null) content = scrollRect.content;
    }

    private void Start()
    {
        // ✅ Fix: assign the viewport
        viewport = (scrollRect != null && scrollRect.viewport != null)
            ? scrollRect.viewport
            : (RectTransform)scrollRect.transform; // fallback

        if (leftButton)  leftButton.onClick.AddListener(ScrollLeft);
        if (rightButton) rightButton.onClick.AddListener(ScrollRight);

        ScrollToIndex(0);
    }

    private void Update()
    {
        UpdateItemScales();
    }

    private void UpdateItemScales()
    {
        if (items == null || items.Length == 0 || viewport == null) return;

        // Local position of the focus line inside the viewport
        float focusLocalX = (focusPoint - 0.5f) * viewport.rect.width;
        Vector3 focusLocal = new Vector3(focusLocalX, 0f, 0f);

        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i];
            if (item == null) continue;

            // Item position expressed in viewport local space
            Vector3 itemLocal = viewport.InverseTransformPoint(item.position);
            float distance = Mathf.Abs(focusLocal.x - itemLocal.x);

            float t = Mathf.Clamp01(distance / (viewport.rect.width * 0.5f));
            float targetScale = Mathf.Lerp(focusedScale, unfocusedScale, t);

            item.localScale = Vector3.Lerp(
                item.localScale,
                Vector3.one * targetScale,
                Time.deltaTime * scaleSmooth
            );
        }
    }

    private void ScrollLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ScrollToIndex(currentIndex);
        }
    }

    private void ScrollRight()
    {
        if (currentIndex < items.Length - 1)
        {
            currentIndex++;
            ScrollToIndex(currentIndex);
        }
    }

    private void ScrollToIndex(int index)
    {
        if (items == null || items.Length <= 1) return;

        currentIndex = Mathf.Clamp(index, 0, items.Length - 1);

        // Simple normalized mapping by index (you can upgrade this to align the item to focusPoint later)
        float targetPos = (float)currentIndex / (items.Length - 1);
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(targetPos);
    }

#if UNITY_EDITOR
    // Optional: auto-fill items from content to avoid forgetting to assign them
    private void OnValidate()
    {
        if (content != null && (items == null || items.Length == 0))
        {
            int n = content.childCount;
            RectTransform[] arr = new RectTransform[n];
            for (int i = 0; i < n; i++) arr[i] = content.GetChild(i) as RectTransform;
            items = arr;
        }
    }
#endif
}
