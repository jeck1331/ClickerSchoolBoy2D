using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer), typeof(SortingGroup))]
public class WhackMoleHole : MonoBehaviour
{
    [SerializeField] private Transform hiddenPoint;
    [SerializeField] private Transform visiblePoint;
    [SerializeField] private SpriteMask spriteMask;
    [Tooltip("Mole width relative to the hole sprite's world width.")]
    [SerializeField, Range(0.1f, 1.5f)] private float moleWidthFraction = 0.65f;

    public bool IsConfigured => hiddenPoint != null && visiblePoint != null && spriteMask != null &&
                                hiddenPoint.IsChildOf(transform) && visiblePoint.IsChildOf(transform) &&
                                spriteMask.transform.IsChildOf(transform) && GetComponent<SortingGroup>() != null;
    public Vector3 HiddenPosition => hiddenPoint.position;
    public Vector3 VisiblePosition => visiblePoint.position;
    public float MoleWidth => GetComponent<SpriteRenderer>().bounds.size.x * moleWidthFraction;

    private void Awake()
    {
        // The cell's SortingGroup limits this mask to its own mole instances.
        if (spriteMask != null) spriteMask.isCustomRangeActive = false;
    }
}
