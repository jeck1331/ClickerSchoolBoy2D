using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private FlappyGameZone gameZone;
    [SerializeField] private PinguinController player;
    [SerializeField] private GameObject topPipePrefab;
    [SerializeField] private GameObject bottomPipePrefab;
    [SerializeField, Min(0.01f)] private float moveSpeed = 2f;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(0f)] private float firstSpawnDelay = 1f;
    [SerializeField, Min(1.1f)] private float minGapInPlayerHeights = 3f;
    [SerializeField, Min(1.1f)] private float maxGapInPlayerHeights = 4f;
    [SerializeField, Min(0f)] private float edgePadding = 0.15f;
    [SerializeField, Min(0f)] private float offscreenMargin = 0.5f;
    [SerializeField] private float gameplayZ;

    private sealed class PipePair
    {
        public Rigidbody2D Body;
        public float RightOffset;
    }

    private readonly List<PipePair> pairs = new List<PipePair>();
    private float timer;
    private float playerHeight;
    private bool warnedAboutGap;

    private void Start()
    {
        timer = firstSpawnDelay;
        if (gameZone == null || player == null || topPipePrefab == null || bottomPipePrefab == null)
        {
            Debug.LogError("PipeSpawner: assign Game Zone, Player and both pipe prefabs.", this);
            enabled = false;
            return;
        }

        // Renderer bounds include the actual Transform scale and tiled sprite size.
        Bounds bounds = default;
        bool found = false;
        foreach (Renderer renderer in player.GetComponentsInChildren<Renderer>())
        {
            if (!renderer.enabled) continue;
            if (!found) bounds = renderer.bounds;
            else bounds.Encapsulate(renderer.bounds);
            found = true;
        }
        foreach (Collider2D collider in player.GetComponentsInChildren<Collider2D>())
        {
            if (!collider.enabled || collider.isTrigger) continue;
            if (!found) bounds = collider.bounds;
            else bounds.Encapsulate(collider.bounds);
            found = true;
        }
        playerHeight = bounds.size.y;
        if (!found || playerHeight <= 0f)
        {
            Debug.LogError("PipeSpawner: player must have a visible Renderer or an enabled Collider2D.", this);
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!player.HasStarted && (pairs.Count > 0 || timer != firstSpawnDelay)) ClearPipes();
        if (!player.IsRunning || !gameZone.TryGetBounds(out Bounds zone)) return;

        for (int i = pairs.Count - 1; i >= 0; i--)
        {
            PipePair pair = pairs[i];
            Vector2 next = pair.Body.position + Vector2.left * (moveSpeed * Time.fixedDeltaTime);
            if (next.x + pair.RightOffset < zone.min.x - offscreenMargin)
            {
                Destroy(pair.Body.gameObject);
                pairs.RemoveAt(i);
            }
            else pair.Body.MovePosition(next);
        }

        timer -= Time.fixedDeltaTime;
        if (timer > 0f) return;
        timer = Mathf.Max(0.1f, spawnInterval);
        Spawn(zone);
    }

    private void Spawn(Bounds zone)
    {
        float availableHeight = zone.size.y - 2f * edgePadding;
        float minimum = playerHeight * Mathf.Max(1.1f, minGapInPlayerHeights);
        float maximum = Mathf.Min(availableHeight,
            playerHeight * Mathf.Max(minGapInPlayerHeights, maxGapInPlayerHeights));
        if (maximum < minimum)
        {
            if (!warnedAboutGap)
                Debug.LogWarning("PipeSpawner: GameZoneUI is too short for the minimum gap. Reduce gap multipliers or player scale.", this);
            warnedAboutGap = true;
            return;
        }
        warnedAboutGap = false;
        float gap = Random.Range(minimum, maximum);
        float center = Random.Range(zone.min.y + edgePadding + gap * 0.5f,
            zone.max.y - edgePadding - gap * 0.5f);

        // Keep the physics root at world scale 1, independent of the spawner's parent.
        GameObject root = new GameObject("Pipe Pair");
        root.transform.position = new Vector3(zone.max.x + offscreenMargin, 0f, gameplayZ);
        GameObject top = Instantiate(topPipePrefab, root.transform);
        GameObject bottom = Instantiate(bottomPipePrefab, root.transform);
        Physics2D.SyncTransforms();
        if (!TryGetPipeBounds(top, out Bounds topBounds) || !TryGetPipeBounds(bottom, out Bounds bottomBounds))
        {
            Debug.LogError("PipeSpawner: each pipe needs an active, non-trigger BoxCollider2D and no Rigidbody2D.", this);
            Destroy(root);
            enabled = false;
            return;
        }

        // Align the opening using collider edges, not prefab pivots or pipe length.
        Vector3 topShift = new Vector3(root.transform.position.x - topBounds.min.x,
            center + gap * 0.5f - topBounds.min.y, gameplayZ - top.transform.position.z);
        Vector3 bottomShift = new Vector3(root.transform.position.x - bottomBounds.min.x,
            center - gap * 0.5f - bottomBounds.max.y, gameplayZ - bottom.transform.position.z);
        top.transform.position += topShift;
        bottom.transform.position += bottomShift;
        Rigidbody2D body = root.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        pairs.Add(new PipePair
        {
            Body = body,
            RightOffset = Mathf.Max(topBounds.size.x, bottomBounds.size.x)
        });
    }

    private static bool TryGetPipeBounds(GameObject pipe, out Bounds bounds)
    {
        bounds = default;
        if (pipe.GetComponentInChildren<Rigidbody2D>() != null) return false;
        bool found = false;
        foreach (BoxCollider2D collider in pipe.GetComponentsInChildren<BoxCollider2D>())
        {
            if (!collider.enabled || collider.isTrigger) continue;
            if (!found) bounds = collider.bounds;
            else bounds.Encapsulate(collider.bounds);
            found = true;
        }
        return found && bounds.size.y > 0f;
    }

    public void ClearPipes()
    {
        foreach (PipePair pair in pairs)
            if (pair.Body != null) Destroy(pair.Body.gameObject);
        pairs.Clear();
        timer = firstSpawnDelay;
    }

    private void OnDisable() => ClearPipes();
}

