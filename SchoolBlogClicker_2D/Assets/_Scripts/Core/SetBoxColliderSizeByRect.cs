using UnityEngine;

public class SetColliderSizeByRect : MonoBehaviour
{
    RectTransform _rectTransform;
    BoxCollider2D _boxCollider;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        if (_rectTransform.hasChanged) {
            _boxCollider.size = _rectTransform.rect.size;
            _rectTransform.hasChanged = false;
        }
    }
}
