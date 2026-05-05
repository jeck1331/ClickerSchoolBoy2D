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
    private BoxCollider2D _boxColliderZg;
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
        _boxColliderZg = zoneGenerating.GetComponent<BoxCollider2D>();
        _canvasTransform = inheritGameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        inMenuObserver.OnValueChanged += OnMenuValueChanged;
        circleObserver.OnValueChanged += HandleCircleClick;
        if (!valueInMenu.Value) 
        {
            StartLoop();
        }
    }

    private void OnDisable()
    {
        inMenuObserver.OnValueChanged -= OnMenuValueChanged;
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
        if (_boxColliderZg == null || circlePrefab == null || _canvasTransform == null) return;

        var bounds = _boxColliderZg.bounds;
        
        var circleRadius = miniGameConfig.CircleRadius;
        var randomX = Random.Range(bounds.min.x + circleRadius * 0.01f, bounds.max.x - circleRadius * 0.01f);
        var randomY = Random.Range(bounds.min.y + circleRadius * 0.01f, bounds.max.y - circleRadius * 0.01f);
        
        var worldPos = new Vector3(randomX, randomY, 1);

        var newCircle = Instantiate(circlePrefab, _canvasTransform);
        var lifecycle = newCircle.GetComponent<_Scripts.Prefabs.CircleLifeCycleMGS>();
        if (lifecycle != null)
            lifecycle.Configure(miniGameConfig.CircleLifetime, miniGameConfig.WarningAnimationStartNormalized);

        var rect = newCircle.GetComponent<RectTransform>();
        var mainCamera = Camera.main;
        if (rect == null || mainCamera == null)
        {
            Destroy(newCircle);
            return;
        }
        var screenPoint = mainCamera.WorldToScreenPoint(worldPos);
        
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform, 
            screenPoint, 
            mainCamera, 
            out localPos
        );

        rect.anchoredPosition = localPos;
        _currentCircle = newCircle;
        _currentCircleSpawnTime = Time.time;
    }

    private void OnMenuValueChanged()
    {
        if (!valueInMenu.Value)
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
}
