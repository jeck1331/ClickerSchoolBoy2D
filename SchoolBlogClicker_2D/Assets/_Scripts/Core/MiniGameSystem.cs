using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MiniGameRoundResult
{
    Fail,
    Partial,
    Success
}

public class MiniGameSystem : MonoBehaviour
{
    [SerializeField] private GameObject inheritGameObject;
    [SerializeField] private GameObject zoneGenerating;
    private RectTransform _zoneGeneratingTransform;
    private RectTransform _canvasTransform;
    [SerializeField] private BoolValue valueInMenu;
    [SerializeField] private ObserverSO inMenuObserver;
    [SerializeField] private ObserverGameObjSO circleObserver;
    
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue clickPowerValue;
    
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private MiniGameConfigSO miniGameConfig;
    [SerializeField] private ParticleSystem miniGameHitVfx;
    [SerializeField] private ParticleSystem miniGameFailVfx;

    private Coroutine _loopCoroutine;
    private Coroutine _roundCoroutine;
    private GameObject _currentCircle;
    private float _currentCircleSpawnTime;
    private bool _currentCircleWasHit;
    private int _roundHits;

    public static event Action<MiniGameRoundResult, int> OnRoundCompleted;

    private void Awake()
    {
        if (zoneGenerating != null)
            _zoneGeneratingTransform = zoneGenerating.GetComponent<RectTransform>();

        if (inheritGameObject != null)
            _canvasTransform = inheritGameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (inMenuObserver != null)
            inMenuObserver.OnValueChanged += OnMenuValueChanged;

        GameplayPauseService.OnPauseChanged += OnGameplayPauseChanged;
        circleObserver.OnValueChanged += HandleCircleClick;
        if (!IsGameplayPaused()) 
        {
            StartLoop();
        }
    }

    private void OnDisable()
    {
        if (inMenuObserver != null)
            inMenuObserver.OnValueChanged -= OnMenuValueChanged;

        GameplayPauseService.OnPauseChanged -= OnGameplayPauseChanged;
        circleObserver.OnValueChanged -= HandleCircleClick;
        StopLoop();
    }

    private IEnumerator MiniGamePlayCoroutineStart()
    {
        while (true)
        {
            var randomTime = Random.Range(miniGameConfig.RoundStartDelayMin, miniGameConfig.RoundStartDelayMax);
            yield return new WaitForSeconds(randomTime);
            _roundCoroutine = StartCoroutine(PlayRoundCoroutine());
            yield return _roundCoroutine;
            _roundCoroutine = null;
        }
    }

    private IEnumerator PlayRoundCoroutine()
    {
        _roundHits = 0;
        for (var i = 0; i < miniGameConfig.CircleCountPerRound; i++)
        {
            SpawnCircle();
            yield return WaitForCircleInteractionCoroutine();
            if (i < miniGameConfig.CircleCountPerRound - 1)
                yield return new WaitForSeconds(miniGameConfig.DelayBetweenCircles);
        }

        var result = _roundHits >= miniGameConfig.CircleCountPerRound
            ? MiniGameRoundResult.Success
            : _roundHits > 0
                ? MiniGameRoundResult.Partial
                : MiniGameRoundResult.Fail;
        OnRoundCompleted?.Invoke(result, _roundHits);
        GameEvents.MiniGameRoundCompleted(result, _roundHits);
    }

    private IEnumerator WaitForCircleInteractionCoroutine()
    {
        var timeout = miniGameConfig.CircleLifetime;
        var elapsed = 0f;
        _currentCircleWasHit = false;
        while (_currentCircle != null && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!_currentCircleWasHit)
        {
            miniGameFailVfx?.Play();
            AudioManager.Instance?.PlaySfx(SfxType.MiniMiss);
        }
        _currentCircle = null;
    }

    public void SpawnCircle()
    {
        if (_zoneGeneratingTransform == null || circlePrefab == null || _canvasTransform == null || miniGameConfig == null)
            return;

        var newCircle = Instantiate(circlePrefab, _canvasTransform);
        var lifecycle = newCircle.GetComponent<_Scripts.Prefabs.CircleLifeCycleMGS>();
        if (lifecycle != null)
            lifecycle.Configure(miniGameConfig.CircleLifetime, miniGameConfig.WarningAnimationStartNormalized);

        var rect = newCircle.GetComponent<RectTransform>();
        if (rect == null)
        {
            Destroy(newCircle);
            return;
        }

        rect.anchoredPosition = GetRandomPositionInsideZone();
        _currentCircle = newCircle;
        _currentCircleSpawnTime = Time.time;
    }

    private Vector2 GetRandomPositionInsideZone()
    {
        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_canvasTransform, _zoneGeneratingTransform);
        var circlePadding = miniGameConfig.CircleRadius * 0.5f;

        var minX = bounds.min.x + circlePadding;
        var maxX = bounds.max.x - circlePadding;
        var minY = bounds.min.y + circlePadding;
        var maxY = bounds.max.y - circlePadding;

        if (minX > maxX)
        {
            minX = bounds.center.x;
            maxX = bounds.center.x;
        }

        if (minY > maxY)
        {
            minY = bounds.center.y;
            maxY = bounds.center.y;
        }

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    private void OnMenuValueChanged()
    {
        if (!IsGameplayPaused())
        {
            StartLoop();
        }
        else
        {
            StopLoop();
        }
    }

    private void StartLoop()
    {
        if (_loopCoroutine != null || miniGameConfig == null) return;
        _loopCoroutine = StartCoroutine(MiniGamePlayCoroutineStart());
    }

    private void StopLoop()
    {
        if (_roundCoroutine != null)
        {
            StopCoroutine(_roundCoroutine);
            _roundCoroutine = null;
        }

        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        if (_currentCircle != null)
        {
            Destroy(_currentCircle);
            _currentCircle = null;
        }
    }

    private void HandleCircleClick(GameObject clickedObject)
    {
        if (IsGameplayPaused())
            return;

        if (_currentCircle == null || clickedObject != _currentCircle) return;

        _currentCircleWasHit = true;
        _roundHits++;
        var reactionTime = Time.time - _currentCircleSpawnTime;
        var reward = miniGameConfig.CalculateReward(clickPowerValue.Value, reactionTime);
        scoreValue.Value += reward;
        miniGameHitVfx?.Play();
        AudioManager.Instance?.PlaySfx(SfxType.MiniHit);
        GameEvents.ClickResolved(false, reward);
        _currentCircle = null;
    }

    private void OnGameplayPauseChanged(bool _)
    {
        OnMenuValueChanged();
    }

    private bool IsGameplayPaused()
    {
        return GameplayPauseService.IsGameplayInputPaused || valueInMenu != null && valueInMenu.Value;
    }
}
