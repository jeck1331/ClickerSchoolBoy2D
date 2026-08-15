using System.Collections;
using _Scripts.Models.Save;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IInitialize
{
    private SaveGameSystem _saveGameSystem;
    private bool _isIncomeStarted = false;
    [CanBeNull] private Coroutine _autoSaveCoroutine;
    [CanBeNull] private Coroutine _incomeCoroutine;

    [SerializeField] private GameObject gmTapZone;
    [SerializeField] private RectTransform tapZoneRectTransform;

    [SerializeField] private ObserverSO scoreObserver;
    [SerializeField] private ObserverSO powerObserver;
    [SerializeField] private ObserverSO incomeObserver;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeIncomeItemSO upgradeIncomeItem;
    
    [SerializeField] private Camera canvasCamera;
    
    private readonly Vector3[] worldCorners = new Vector3[4];
    
    // [SerializeField] private ShopManager shopManager;
    // [SerializeField] private UpgradeManager upgradeManager;

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSavedGameData rgd = _saveGameSystem.Load();
        scoreValue.Value = rgd.Coins;
        
        // shopManager.FillDataFromSave(rgd.UpgradeIncomeTree);
        // upgradeManager.FillDataFromSave(rgd.UpgradeClickTree);
        //
        // powerValue.Value = upgradeManager.GetPowerValue();
        // incomeValue.Value = shopManager.GetIncomeValue();
    }

    private SavedGameData CurrentGameData()
    {
        var sgd = new SavedGameData
        {
            Coins = scoreValue.Value,
            // UpgradeClickTree = upgradeManager.GetDataToSave(),
            // UpgradeIncomeTree = upgradeManager.GetDataToSave()
        };
        return sgd;
    }

    private IEnumerator AutoSave_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);

            _saveGameSystem.Save(CurrentGameData());

            Debug.Log("AutoSave executed");
        }
    }
    
    private IEnumerator Income_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
        
            scoreValue.Value += incomeValue.Value;
        
            // Debug.Log("INCOME executed");
        }
    }
    
    IEnumerator Start() 
    {
        // Wait until the end of the frame so UI layout calculations finish
        yield return new WaitForEndOfFrame();
        
        // Transform TapZoneUI RectTransform size and position to boxCollider2D size
        var boxColliderTapZone = gmTapZone.GetComponent<BoxCollider2D>();
        tapZoneRectTransform.GetWorldCorners(worldCorners);
         
        Vector2 bottomLeft = canvasCamera.WorldToScreenPoint(worldCorners[0]);
        Vector2 topRight = canvasCamera.WorldToScreenPoint(worldCorners[2]);

        Vector3 localBottomLeft = transform.InverseTransformPoint(canvasCamera.ScreenToWorldPoint(new Vector3(bottomLeft.x, bottomLeft.y, canvasCamera.nearClipPlane)));
        Vector3 localTopRight = transform.InverseTransformPoint(canvasCamera.ScreenToWorldPoint(new Vector3(topRight.x, topRight.y, canvasCamera.nearClipPlane)));

        // 3. Compute size and center offset in local coordinates
        Vector2 size = new Vector2(Mathf.Abs(localTopRight.x - localBottomLeft.x), Mathf.Abs(localTopRight.y - localBottomLeft.y));
        Vector2 center = (Vector2)(localBottomLeft + localTopRight) * 0.5f;

        // 4. Apply to BoxCollider2D
        boxColliderTapZone.size = size;
        boxColliderTapZone.offset = center;
    }

    private void OnEnable()
    {
        incomeObserver.OnValueChanged += IncomeUpdate;
        
        scoreObserver.Changing();
        powerObserver.Changing();

        IncomeUpdate();

        _autoSaveCoroutine = StartCoroutine(AutoSave_Coroutine());
    }

    private void IncomeUpdate()
    {
        if (!_isIncomeStarted && incomeValue.Value > 0)
        {
            _isIncomeStarted = true;
            _incomeCoroutine = StartCoroutine(Income_Coroutine());
        }
        else if (_isIncomeStarted && incomeValue.Value == 0 && _incomeCoroutine != null)
        {
            StopCoroutine(_incomeCoroutine);
            _incomeCoroutine = null;
            _isIncomeStarted = false;
        }
    }
    
    private void OnDisable()
    {
        incomeObserver.OnValueChanged -= IncomeUpdate;
        
        if (_autoSaveCoroutine != null)
            StopCoroutine(_autoSaveCoroutine);
        if (_incomeCoroutine != null)
            StopCoroutine(_incomeCoroutine);
        _isIncomeStarted = false;
        
        _saveGameSystem.Save(CurrentGameData());
    }
}