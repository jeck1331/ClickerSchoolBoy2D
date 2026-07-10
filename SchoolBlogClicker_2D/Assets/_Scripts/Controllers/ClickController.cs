using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickController : MonoBehaviour
{
    private InputAction _clickAction;
    private InputAction _tapAction;
    private TapZonePointerInput _tapZonePointerInput;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    [SerializeField] private ParticleSystem vfxStarParticle;
    [SerializeField] private ParticleSystem vfxCritStarParticle;
    [SerializeField] private ObserverGameObjSO circleObserver;
    [SerializeField] private CritConfigSO critConfig;
    [SerializeField] private string tapZoneTag = "TapZone";
    [SerializeField] private Button tapZoneButton;
    [SerializeField] private string miniGameTag = "ClickItemMG";

    private void Awake()
    {
        if (tapZoneButton == null)
            tapZoneButton = GetComponent<Button>();

        if (tapZoneButton == null && !string.IsNullOrWhiteSpace(tapZoneTag))
        {
            var tapZoneObject = GameObject.FindGameObjectWithTag(tapZoneTag);
            if (tapZoneObject != null)
                tapZoneButton = tapZoneObject.GetComponent<Button>();
        }

        if (tapZoneButton != null)
        {
            _tapZonePointerInput = tapZoneButton.GetComponent<TapZonePointerInput>();
            if (_tapZonePointerInput == null)
                _tapZonePointerInput = tapZoneButton.gameObject.AddComponent<TapZonePointerInput>();

            _tapZonePointerInput.Initialize(HandleTapZonePointerDown);
        }
    }

    private void OnEnable()
    {
        if (_tapZonePointerInput != null)
            _tapZonePointerInput.Initialize(HandleTapZonePointerDown);

        GameplayPauseService.OnPauseChanged += HandlePauseChanged;
        HandlePauseChanged(GameplayPauseService.IsGameplayInputPaused);
    }

    private void OnDisable()
    {
        if (_tapZonePointerInput != null)
            _tapZonePointerInput.Initialize(null);

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

        if (!TryGetPointerPosition(out var pointerPosition))
            return;

        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(pointerPosition);
        var hitCollider = Physics2D.OverlapPoint(worldPosition);

        if (hitCollider == null || !hitCollider.CompareTag(miniGameTag))
            return;

        var touchedObject = hitCollider.transform.gameObject;
        circleObserver.Changing(touchedObject);
        AudioManager.Instance?.PlaySfx(SfxType.MiniHit);
    }

    public void OnTapZoneClicked()
    {
        TryResolveTapZoneClickFromCurrentPointer();
    }

    private void HandleTapZonePointerDown(Vector2 screenPosition)
    {
        TryResolveTapZoneClick(screenPosition);
    }

    private void TryResolveTapZoneClickFromCurrentPointer()
    {
        if (TryGetPointerScreenPosition(out var screenPosition))
        {
            TryResolveTapZoneClick(screenPosition);
            return;
        }

        TryResolveTapZoneClick(null);
    }

    private void TryResolveTapZoneClick(Vector2? screenPosition)
    {
        if (GameplayPauseService.IsGameplayInputPaused)
            return;

        var hitReward = CritCalculator.CalculateHitReward(clickPowerValue!.Value, scoreValue.Value, critConfig, out var isCrit);
        scoreValue.Value += hitReward;

        if (TryGetWorldPosition(screenPosition, out var worldPosition))
            PlayClickVfx(isCrit, worldPosition);

        AudioManager.Instance?.PlaySfx(isCrit ? SfxType.Crit : SfxType.Click);
        GameEvents.ClickResolved(isCrit, hitReward);
    }

    private void PlayClickVfx(bool isCrit, Vector3 worldPosition)
    {
        var particle = isCrit ? vfxCritStarParticle : vfxStarParticle;
        if (particle == null)
            return;

        particle.transform.position = worldPosition;
        particle.Play();
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

    private bool TryGetWorldPosition(Vector2? screenPosition, out Vector3 worldPosition)
    {
        if (screenPosition.HasValue)
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                worldPosition = mainCamera.ScreenToWorldPoint(screenPosition.Value);
                return true;
            }
        }

        if (tapZoneButton != null)
        {
            worldPosition = tapZoneButton.transform.position;
            return true;
        }

        worldPosition = transform.position;
        return false;
    }

    private bool TryGetPointerScreenPosition(out Vector3 pointerPosition)
    {
        if (Mouse.current != null)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            return true;
        }

        if (Touchscreen.current != null)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        pointerPosition = new Vector3();
        return false;
    }

    private void HandlePauseChanged(bool isPaused)
    {
        if (tapZoneButton != null)
            tapZoneButton.interactable = !isPaused;
    }
}

public class TapZonePointerInput : MonoBehaviour, IPointerDownHandler
{
    private System.Action<Vector2> _onPointerDown;

    public void Initialize(System.Action<Vector2> onPointerDown)
    {
        _onPointerDown = onPointerDown;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _onPointerDown?.Invoke(eventData.position);
    }
}
