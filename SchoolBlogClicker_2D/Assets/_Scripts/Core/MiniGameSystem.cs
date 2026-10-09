using System.Collections;
using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject zoneGenerating;
    private RectTransform _zoneGeneratingTransform;
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
    private sealed class ActiveCircle
    {
        public GameObject Object;
        public float SpawnTime;
        public float Lifetime;
    }

    private readonly List<ActiveCircle> _activeCircles = new List<ActiveCircle>();
    private float _nextCircleLifetime;
    private int _roundHits;

    public static event Action<MiniGameRoundResult, int> OnRoundCompleted;

    private void Awake()
    {
        if (zoneGenerating != null)
            _zoneGeneratingTransform = zoneGenerating.GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        if (inMenuObserver != null)
            inMenuObserver.OnValueChanged += OnMenuValueChanged;

        GameplayPauseService.OnPauseChanged += OnGameplayPauseChanged;
        if (circleObserver != null)
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
        if (circleObserver != null)
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
        _nextCircleLifetime = miniGameConfig.GetFirstCircleLifetime();
        for (var i = 0; i < miniGameConfig.CircleCountPerRound; i++)
        {
            SpawnCircle();
            _nextCircleLifetime *= Random.Range(0.7f, 0.85f);
            if (i < miniGameConfig.CircleCountPerRound - 1)
                yield return new WaitForSeconds(miniGameConfig.DelayBetweenCircles);
        }

        yield return WaitForCircleInteractionCoroutine();
        _nextCircleLifetime = 0f;

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
        while (_activeCircles.Count > 0)
            yield return null;
    }

    private void Update()
    {
        for (var i = _activeCircles.Count - 1; i >= 0; i--)
        {
            var circle = _activeCircles[i];
            if (circle.Object != null && Time.time - circle.SpawnTime < circle.Lifetime)
                continue;

            if (circle.Object != null)
                Destroy(circle.Object);
            _activeCircles.RemoveAt(i);
            miniGameFailVfx?.Play();
            AudioManager.Instance?.PlaySfx(SfxType.MiniMiss);
        }
    }

    public void SpawnCircle()
    {
        if (_zoneGeneratingTransform == null || circlePrefab == null || miniGameConfig == null)
            return;

        var newCircle = Instantiate(circlePrefab, _zoneGeneratingTransform, false);
        var lifetime = _nextCircleLifetime > 0f ? _nextCircleLifetime : miniGameConfig.GetFirstCircleLifetime();
        lifetime = Mathf.Max(0.1f, lifetime);
        var lifecycle = newCircle.GetComponent<_Scripts.Prefabs.CircleLifeCycleMGS>();
        if (lifecycle != null)
            lifecycle.Configure(lifetime, miniGameConfig.WarningAnimationStartNormalized);

        var rect = newCircle.GetComponent<RectTransform>();
        if (rect == null)
        {
            Destroy(newCircle);
            return;
        }

        // Use the zone's local coordinates, independent of the Canvas and its scale.
        var size = rect.rect.size;
        rect.anchorMin = rect.anchorMax = _zoneGeneratingTransform.pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition3D = Vector3.zero;

        // Include child graphics: the prefab's root can have no visible image.
        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_zoneGeneratingTransform, rect);
        var zone = _zoneGeneratingTransform.rect;
        var minX = zone.xMin - bounds.min.x;
        var maxX = zone.xMax - bounds.max.x;
        var minY = zone.yMin - bounds.min.y;
        var maxY = zone.yMax - bounds.max.y;
        rect.anchoredPosition = new Vector2(
            minX <= maxX ? Random.Range(minX, maxX) : zone.center.x - bounds.center.x,
            minY <= maxY ? Random.Range(minY, maxY) : zone.center.y - bounds.center.y);
        _activeCircles.Add(new ActiveCircle { Object = newCircle, SpawnTime = Time.time, Lifetime = lifetime });
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
        if (_loopCoroutine != null || miniGameConfig == null || _zoneGeneratingTransform == null || circlePrefab == null) return;
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

        foreach (var circle in _activeCircles)
            if (circle.Object != null)
                Destroy(circle.Object);
        _activeCircles.Clear();
        _nextCircleLifetime = 0f;
    }

    private void HandleCircleClick(GameObject clickedObject)
    {
        if (IsGameplayPaused())
            return;

        var index = _activeCircles.FindIndex(circle => circle.Object == clickedObject);
        if (index < 0) return;
        var circle = _activeCircles[index];
        if (Time.time - circle.SpawnTime >= circle.Lifetime) return;
        _activeCircles.RemoveAt(index);

        _roundHits++;
        // Compare reaction speed against this circle's own, progressively shorter lifetime.
        var reactionTime = (Time.time - circle.SpawnTime) / circle.Lifetime * miniGameConfig.CircleLifetime;
        var reward = miniGameConfig.CalculateReward(clickPowerValue.Value, reactionTime);
        scoreValue.Value += reward;
        if (miniGameHitVfx != null)
        {
            var camera = Camera.main;
            if (camera != null)
            {
                var screenPosition = circleObserver.LastScreenPosition;
                var ray = camera.ScreenPointToRay(screenPosition);
                var plane = new Plane(Vector3.forward, Vector3.zero);
                if (plane.Raycast(ray, out var distance))
                    miniGameHitVfx.transform.position = ray.GetPoint(distance);
            }
            miniGameHitVfx.Play();
        }
        AudioManager.Instance?.PlaySfx(SfxType.MiniHit);
        GameEvents.ClickResolved(false, reward);
        Destroy(clickedObject);
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
