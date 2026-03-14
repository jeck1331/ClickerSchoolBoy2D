using UnityEngine;

public class SetCircleColliderSizeByRect : MonoBehaviour
{
    RectTransform _rectTransform;
    CircleCollider2D _circleCollider;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update() {
        if (_rectTransform.hasChanged) {
            _circleCollider.radius = _rectTransform.rect.width;
            _rectTransform.hasChanged = false;
        }
    }
}
