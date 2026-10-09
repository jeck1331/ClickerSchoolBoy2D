using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    private InputAction _pointerAction;
    private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private ParticleSystem vfxStarParticle;
    [SerializeField] private ParticleSystem vfxCritStarParticle;
    [SerializeField] private ObserverGameObjSO circleObserver;
    [SerializeField] private CritConfigSO critConfig;
    [SerializeField] private string miniGameTag = "ClickItemMG";
    
    [SerializeField] private RectTransform hitArea;
    [SerializeField] private Canvas canvas;
    private Camera UICamera => canvas.worldCamera;
    private Camera _worldCamera;

    private void Awake()
    {
        _pointerAction = InputSystem.actions.FindAction("Pointer");
        _worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        _pointerAction.performed += OnClick;
        GameplayPauseService.OnPauseChanged += HandlePauseChanged;
        HandlePauseChanged(GameplayPauseService.IsGameplayInputPaused);
    }

    private void OnDisable()
    {
        _pointerAction.performed -= OnClick;
        GameplayPauseService.OnPauseChanged -= HandlePauseChanged;
    }
    
    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (GameplayPauseService.IsGameplayInputPaused)
            return;

        if (ctx.control.device is not Pointer pointer)
            return;

        Vector2 screenPosition = pointer.position.ReadValue();
        
        if (TryHitMiniGameCircle(screenPosition)) return;
        if (!IsInside(screenPosition)) return;

        var hitReward = CritCalculator.CalculateHitReward(clickPowerValue!.Value, scoreValue.Value, critConfig, out var isCrit);
        Debug.Log($"Hit Reward: {hitReward}, isCrit: {isCrit}");
        scoreValue.Value += hitReward;
        if (isCrit)
        {
            if (vfxCritStarParticle != null)
            {
                vfxCritStarParticle.transform.position = ScreenToWorldPosition(screenPosition);
                vfxCritStarParticle.Play();
            }
            AudioManager.Instance?.PlaySfx(SfxType.Crit);
        }
        else
        {
            if (vfxStarParticle != null)
            {
                vfxStarParticle.transform.position = ScreenToWorldPosition(screenPosition);
                vfxStarParticle.Play();
            }
            AudioManager.Instance?.PlaySfx(SfxType.Click);
        }

        GameEvents.ClickResolved(isCrit, hitReward);
    }

    private bool TryHitMiniGameCircle(Vector2 screenPosition)
    {
        if (EventSystem.current == null || circleObserver == null)
            return false;

        _uiHits.Clear();
        var pointerData = new PointerEventData(EventSystem.current) { position = screenPosition };
        EventSystem.current.RaycastAll(pointerData, _uiHits);
        if (_uiHits.Count == 0) return false;

        // Only the foremost UI element receives the hit; overlays can block circles.
        var circle = _uiHits[0].gameObject.GetComponentInParent<_Scripts.Prefabs.CircleLifeCycleMGS>();
        if (circle == null || !circle.CompareTag(miniGameTag)) return false;

        circleObserver.ChangingAtScreenPosition(circle.gameObject, screenPosition);
        return true;
    }
    
    private Vector3 ScreenToWorldPosition(Vector2 screenPosition)
    {
        if (_worldCamera == null)
            return screenPosition;

        Vector3 worldPosition = _worldCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, -_worldCamera.transform.position.z));
        worldPosition.z = 0f;
        return worldPosition;
    }

    private bool IsInside(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            hitArea,
            screenPosition,
            UICamera
        );
    }

    private void HandlePauseChanged(bool isPaused)
    {
        
    }
}
