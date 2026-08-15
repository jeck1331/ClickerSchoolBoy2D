using UnityEngine;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    private InputAction _clickAction;
    private InputAction _tapAction;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private ParticleSystem vfxStarParticle;
    [SerializeField] private ParticleSystem vfxCritStarParticle;
    [SerializeField] private ObserverGameObjSO circleObserver;
    [SerializeField] private CritConfigSO critConfig;
    [SerializeField] private string tapZoneTag = "TapZone";
    [SerializeField] private string miniGameTag = "ClickItemMG";

    private void OnEnable()
    {
        
        GameplayPauseService.OnPauseChanged += HandlePauseChanged;
        HandlePauseChanged(GameplayPauseService.IsGameplayInputPaused);
    }

    private void OnDisable()
    {
        GameplayPauseService.OnPauseChanged -= HandlePauseChanged;
    }

    private void Start()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _tapAction = InputSystem.actions.FindAction("Tap");
    }

    private void Update()
    {
        if (GameplayPauseService.IsGameplayInputPaused)
            return;

        if (!TryGetPointerPosition(out Vector3 pointerPosition))
            return;
        
        Vector2 screenPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
        int layerMask = 1 << LayerMask.NameToLayer("TapZone");
        Collider2D hitCollider = Physics2D.OverlapPoint(screenPosition, layerMask);
        
        if (hitCollider != null){
            if (hitCollider.CompareTag(tapZoneTag))
            {
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
            } else if (hitCollider.CompareTag(miniGameTag))
            {
                GameObject touchedObject = hitCollider.transform.gameObject;
                circleObserver.Changing(touchedObject);
                AudioManager.Instance?.PlaySfx(SfxType.MiniHit);
            }
        }
    }
    
    private bool TryGetPointerPosition(out Vector3 pointerPosition)
    {
        if (Mouse.current != null && _clickAction != null && _clickAction.WasPressedThisFrame())
        {
            pointerPosition = Mouse.current.position.ReadValue();
            return true;
        }

        if (Touchscreen.current != null && _tapAction != null && _tapAction.WasPressedThisFrame())
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        pointerPosition = new Vector3();
        return false;
    }

    private void HandlePauseChanged(bool isPaused)
    {
        
    }
}
