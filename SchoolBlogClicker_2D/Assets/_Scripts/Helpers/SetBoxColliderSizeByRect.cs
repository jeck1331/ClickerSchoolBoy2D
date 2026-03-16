using UnityEngine;

public class SetBoxColliderSizeByRect : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private BoxCollider2D boxCollider;

    // void Awake() {
    //     _rectTransform = GetComponent<RectTransform>();
    //     _boxCollider = GetComponent<BoxCollider2D>();
    // }

    void Update() {
        if (rectTransform.hasChanged) {
            boxCollider.size = rectTransform.rect.size;
            rectTransform.hasChanged = false;
        }
    }
}
