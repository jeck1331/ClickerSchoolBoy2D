using UnityEngine;

public class FlappyGameZone : MonoBehaviour
{
    [SerializeField] private RectTransform gameZoneUI;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float gameplayZ;
    private readonly Vector3[] corners = new Vector3[4];

    private Camera UICamera
    {
        get
        {
            Canvas canvas = gameZoneUI.GetComponentInParent<Canvas>();
            return canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.rootCanvas.worldCamera : null;
        }
    }

    public bool ContainsScreenPoint(Vector2 point)
    {
        return gameZoneUI != null &&
               RectTransformUtility.RectangleContainsScreenPoint(gameZoneUI, point, UICamera);
    }

    public bool TryGetBounds(out Bounds bounds)
    {
        bounds = default;
        if (gameZoneUI == null) return false;
        if (worldCamera == null) worldCamera = Camera.main;
        if (worldCamera == null || gameZoneUI.rect.width <= 0f || gameZoneUI.rect.height <= 0f)
            return false;

        gameZoneUI.GetWorldCorners(corners);
        Camera uiCamera = UICamera;
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, gameplayZ));
        for (int i = 0; i < corners.Length; i++)
        {
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]);
            Ray ray = worldCamera.ScreenPointToRay(screen);
            if (!plane.Raycast(ray, out float distance)) return false;
            Vector3 point = ray.GetPoint(distance);
            if (i == 0) bounds = new Bounds(point, Vector3.zero);
            else bounds.Encapsulate(point);
        }
        return bounds.size.x > 0f && bounds.size.y > 0f;
    }
}
