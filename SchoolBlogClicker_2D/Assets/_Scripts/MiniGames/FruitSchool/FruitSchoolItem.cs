using UnityEngine;

public class FruitSchoolItem : MonoBehaviour
{
    public enum ItemKind { Fruit, LargeBomb, SmallBomb }

    [SerializeField] private ItemKind kind;
    [SerializeField, Min(1)] private int points = 10;
    [Tooltip("SpriteRenderer of the whole object, on the prefab root or a child. Only the renderer is hidden after a tap.")]
    [SerializeField] private SpriteRenderer intact;
    [Tooltip("Optional child containing the two fruit halves; shown after a tap.")]
    [SerializeField] private Transform cutForm;
    [SerializeField] private Transform leftHalf;
    [SerializeField] private Transform rightHalf;
    [Header("Cut motion")]
    [Tooltip("Speed of each half away from the cut, in world units per second.")]
    [SerializeField, Min(0f)] private float separationSpeed = 1.5f;
    [SerializeField, Range(0f, 1f)] private float inheritedVelocity = 0.45f;
    [SerializeField, Min(0f)] private float cutLift = 0.6f;
    [SerializeField, Min(0f)] private float halfSpinSpeed = 90f;
    [SerializeField, Min(0.1f)] private float fragmentsLifetime = 2.5f;

    private float age;
    private bool popped;
    private Rigidbody2D body;
    private Rigidbody2D leftBody;
    private Rigidbody2D rightBody;

    public ItemKind Kind => kind;
    public int Points => points;
    public bool IsConfigured => intact != null && intact.sprite != null &&
                                (intact.transform == transform || intact.transform.IsChildOf(transform)) &&
                                (cutForm == null || (cutForm != transform && cutForm.IsChildOf(transform) &&
                                 intact.transform != cutForm && !intact.transform.IsChildOf(cutForm)));

    public void Launch(Vector2 launchVelocity, float angularSpeed, int sortingOrder, float gravity = 12f)
    {
        age = 0f;
        popped = false;
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>(true))
            renderer.sortingOrder = sortingOrder;
        intact.gameObject.SetActive(true);
        intact.enabled = true;
        if (cutForm != null) cutForm.gameObject.SetActive(false);

        body = PrepareBody(transform, gravity);
        leftBody = PrepareBody(leftHalf, gravity);
        rightBody = PrepareBody(rightHalf, gravity);
        ApplyImpulse(body, launchVelocity, angularSpeed);
    }

    public bool Contains(Vector3 worldPoint)
    {
        if (popped || intact == null || intact.sprite == null) return false;
        Vector3 local = intact.transform.InverseTransformPoint(worldPoint);
        Bounds bounds = intact.sprite.bounds;
        if (intact.drawMode != SpriteDrawMode.Simple) bounds.size = intact.size;
        return Mathf.Abs(local.x - bounds.center.x) <= bounds.extents.x &&
               Mathf.Abs(local.y - bounds.center.y) <= bounds.extents.y;
    }

    public void Pop()
    {
        if (popped) return;
        popped = true;
        age = 0f;
        Vector2 momentum = body.linearVelocity * inheritedVelocity;
        body.simulated = false;
        intact.enabled = false;
        if (cutForm != null) cutForm.gameObject.SetActive(true);

        // Keep some of the fruit's momentum, with a small impulse along the cut.
        Vector2 direction = transform.right;
        ApplyImpulse(leftBody,
            momentum - direction * (separationSpeed * Random.Range(0.9f, 1.1f)) + Vector2.up * cutLift,
            -halfSpinSpeed * Random.Range(0.85f, 1.15f));
        ApplyImpulse(rightBody,
            momentum + direction * (separationSpeed * Random.Range(0.9f, 1.1f)) + Vector2.up * (cutLift * 0.8f),
            halfSpinSpeed * Random.Range(0.85f, 1.15f));
    }

    // Physics owns movement; the session only tracks lifetime and removes fallen objects.
    public bool Tick(float deltaTime, float gravity, float bottom)
    {
        age += deltaTime;
        if (popped)
        {
            bool leftFinished = HasFallen(leftBody, bottom);
            bool rightFinished = HasFallen(rightBody, bottom);
            return (leftFinished && rightFinished) || age >= fragmentsLifetime;
        }
        return HasFallen(body, bottom) || age > 15f;
    }

    public void SetPaused(bool paused)
    {
        if (body != null) body.simulated = !paused && !popped;
        if (leftBody != null) leftBody.simulated = !paused && popped;
        if (rightBody != null) rightBody.simulated = !paused && popped;
    }

    private void OnDisable() => SetPaused(true);

    private static Rigidbody2D PrepareBody(Transform target, float gravity)
    {
        if (target == null) return null;
        if (!target.TryGetComponent(out Rigidbody2D rb)) rb = target.gameObject.AddComponent<Rigidbody2D>();
        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.gravityScale = gravity / -Physics2D.gravity.y;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        return rb;
    }

    private static void ApplyImpulse(Rigidbody2D rb, Vector2 launchVelocity, float angularSpeed)
    {
        if (rb == null) return;
        // Inactive halves have followed their parent visually; sync before enabling physics.
        rb.position = rb.transform.position;
        rb.rotation = rb.transform.eulerAngles.z;
        rb.simulated = true;
        // J = mass * deltaVelocity: authored mass does not change the target flight height.
        rb.AddForce(launchVelocity * rb.mass, ForceMode2D.Impulse);
        rb.angularVelocity = angularSpeed;
    }

    private static bool HasFallen(Rigidbody2D rb, float bottom)
    {
        return rb == null || (rb.linearVelocity.y < 0f && rb.position.y < bottom);
    }
}
