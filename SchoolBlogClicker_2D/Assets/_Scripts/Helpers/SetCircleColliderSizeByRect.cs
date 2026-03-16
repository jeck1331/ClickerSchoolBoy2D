using UnityEngine;

public class SetCircleColliderSizeByRect : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CircleCollider2D circleCollider;

    // void Awake() {
    //     _rectTransform = GetComponent<RectTransform>();
    //     _circleCollider = GetComponent<CircleCollider2D>();
    // }

    void Update() {
        if (rectTransform.hasChanged) {
            circleCollider.radius = rectTransform.rect.width / 2;
            rectTransform.hasChanged = false;
        }
    }
}
