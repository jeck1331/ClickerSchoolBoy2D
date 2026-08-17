using UnityEngine;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    private InputAction _pointerAction;

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
    
    private void Awake()
    {
        _pointerAction = InputSystem.actions.FindAction("Pointer");
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
        if (ctx.control.device is not Pointer pointer)
            return;

        Vector2 screenPosition = pointer.position.ReadValue();
        
        if (!IsInside(screenPosition)) return;

        //MiniGame Hit
        if (false)
        {
            //Подумать мб генерить не gameObject Sprite, а RectTransform?
            // GameObject touchedObject = hitCollider.transform.gameObject;
            // circleObserver.Changing(touchedObject);
            // AudioManager.Instance?.PlaySfx(SfxType.MiniHit);
        }

        var hitReward = CritCalculator.CalculateHitReward(clickPowerValue!.Value, scoreValue.Value, critConfig, out var isCrit);
        scoreValue.Value += hitReward;
        if (isCrit)
        {
            if (vfxCritStarParticle != null)
            {
                vfxCritStarParticle.transform.position = screenPosition;
                vfxCritStarParticle.Play();
            }
            AudioManager.Instance?.PlaySfx(SfxType.Crit);
        }
        else
        {
            if (vfxStarParticle != null)
            {
                vfxStarParticle.transform.position = screenPosition;
                vfxStarParticle.Play();
            }
            AudioManager.Instance?.PlaySfx(SfxType.Click);
        }

        GameEvents.ClickResolved(isCrit, hitReward);
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
