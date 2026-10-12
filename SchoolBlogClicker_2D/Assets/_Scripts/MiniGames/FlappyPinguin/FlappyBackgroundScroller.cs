using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class FlappyBackgroundScroller : MonoBehaviour
{
    [SerializeField] private PinguinController player;
    [SerializeField] private RectTransform normalPrefab;
    [SerializeField] private RectTransform mirroredPrefab;
    [Tooltip("Speed in UI units per second, before Canvas scaling.")]
    [SerializeField, Min(0f)] private float scrollSpeed = 80f;
    private readonly RectTransform[] tiles = new RectTransform[4];
    private RectTransform viewport;
    private float phase;

    private void Start()
    {
        viewport = (RectTransform)transform;
        if (normalPrefab == null || mirroredPrefab == null || player == null)
        {
            Debug.LogError("FlappyBackgroundScroller: assign Player and both UI background prefabs.", this);
            enabled = false;
            return;
        }

        // Four tiles keep A/B alternation continuous, including across a full wrap.
        for (int i = 0; i < tiles.Length; i++)
        {
            RectTransform tile = Instantiate(i % 2 == 0 ? normalPrefab : mirroredPrefab, viewport);
            tiles[i] = tile;
            tile.anchorMin = tile.anchorMax = new Vector2(0f, 0.5f);
            tile.pivot = new Vector2(0.5f, 0.5f);
            // Preserve prefab rotation (the existing mirrored prefab uses Y = 180).
            Vector3 scale = tile.localScale;
            tile.localScale = new Vector3(Mathf.Sign(scale.x), Mathf.Sign(scale.y), 1f);
            LayoutElement layout = tile.GetComponent<LayoutElement>();
            if (layout == null) layout = tile.gameObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            foreach (Graphic graphic in tile.GetComponentsInChildren<Graphic>()) graphic.raycastTarget = false;
            Image image = tile.GetComponent<Image>();
            if (image != null) image.preserveAspect = false;
        }
        LayoutTiles();
    }

    private void LateUpdate()
    {
        if (tiles[0] == null) return;
        float width = viewport.rect.width;
        if (width <= 0f) return;
        if (!player.HasStarted) phase = 0f;
        if (player.IsRunning) phase = Mathf.Repeat(phase + scrollSpeed * Time.deltaTime / width, 2f);
        LayoutTiles();
    }

    private void LayoutTiles()
    {
        float width = viewport.rect.width;
        float height = viewport.rect.height;
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].sizeDelta = new Vector2(width, height);
            tiles[i].anchoredPosition3D = new Vector3((i + 0.5f - phase) * width, 0f, 0f);
        }
    }

    public void ResetScroll()
    {
        phase = 0f;
        if (tiles[0] != null) LayoutTiles();
    }

    private void OnDestroy()
    {
        foreach (RectTransform tile in tiles)
            if (tile != null) Destroy(tile.gameObject);
    }
}

