using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGameSystem : MonoBehaviour
{
    [SerializeField] private GameObject inheritGameObject;
    [SerializeField] private GameObject zoneGenerating;
    private BoxCollider2D _boxColliderZg;
    private RectTransform _canvasTransform;
    [SerializeField] private BoolValue valueInMenu;
    [SerializeField] private ObserverSO inMenuObserver;
    
    [SerializeField] private GameObject circlePrefab;
    
    [Header("Settings")]
    [SerializeField] private float circleRadius = 187f;

    private void Awake()
    {
        _boxColliderZg = zoneGenerating.GetComponent<BoxCollider2D>();
        _canvasTransform = inheritGameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        inMenuObserver.OnValueChanged += OnMenuValueChanged;
        if (!valueInMenu.Value) 
        {
            StartCoroutine(MiniGamePlayCoroutineStart());
        }
    }

    private void OnDisable()
    {
        inMenuObserver.OnValueChanged -= OnMenuValueChanged;
        StopCoroutine(MiniGamePlayCoroutineStart());
    }

    private IEnumerator MiniGamePlayCoroutineStart()
    {
        while (true)
        {
            var randomTime = Random.Range(5, 15);
            yield return new WaitForSeconds(randomTime);
            Debug.Log(randomTime);
            SpawnCircle();
        }
    }
    
    public void SpawnCircle()
    {
        if (_boxColliderZg == null || circlePrefab == null || _canvasTransform == null) return;

        // 1. Получаем случайную точку внутри BoxCollider2D в мировых координатах
        Bounds bounds = _boxColliderZg.bounds;
        
        // Учитываем радиус, чтобы центр круга не спавнился слишком близко к краю
        float randomX = Random.Range(bounds.min.x + circleRadius * 0.01f, bounds.max.x - circleRadius * 0.01f);
        float randomY = Random.Range(bounds.min.y + circleRadius * 0.01f, bounds.max.y - circleRadius * 0.01f);
        
        Vector3 worldPos = new Vector3(randomX, randomY, 1);

        // 2. Переводим мировую позицию в локальную позицию внутри Canvas
        GameObject newCircle = Instantiate(circlePrefab, _canvasTransform);
        RectTransform rect = newCircle.GetComponent<RectTransform>();

        // Используем ScreenPoint для корректного наложения (подходит для Overlay и Camera Canvas)
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
        
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform, 
            screenPoint, 
            Camera.main, 
            out localPos
        );

        rect.anchoredPosition = localPos;
    }
    
    // private IEnumerator MiniGameGenerateCircle_Coroutine()
    // {
    //     // while (true)
    //     // {
    //     //     var randomTime = Random.Range(20, 60);
    //     //     yield return new WaitForSeconds(randomTime);
    //     //     Debug.Log(randomTime);
    //     // }
    // }

    private void OnMenuValueChanged()
    {
        if (!valueInMenu.Value)
        {
            StartCoroutine(MiniGamePlayCoroutineStart());
        }
        else
        {
            StopCoroutine(MiniGamePlayCoroutineStart());
        }
    }
}
