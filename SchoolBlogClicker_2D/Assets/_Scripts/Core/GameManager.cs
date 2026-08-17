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

    [SerializeField] private ObserverSO scoreObserver;
    [SerializeField] private ObserverSO powerObserver;
    [SerializeField] private ObserverSO incomeObserver;

    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    
    [SerializeField] private UpgradeClickItemSO upgradeClickItem;
    [SerializeField] private UpgradeIncomeItemSO upgradeIncomeItem;
    
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private UpgradeManager upgradeManager;

    void IInitialize.Initialize()
    {
        _saveGameSystem = new SaveGameSystem();
        RuntimeSavedGameData rgd = _saveGameSystem.Load();
        scoreValue.Value = rgd.Coins;
        
        shopManager.FillDataFromSave(rgd.UpgradeIncomeTree);
        upgradeManager.FillDataFromSave(rgd.UpgradeClickTree);
        
        powerValue.Value = upgradeManager.GetPowerValue();
        incomeValue.Value = shopManager.GetIncomeValue();
    }

    private SavedGameData CurrentGameData() => new SavedGameData
    {
        Coins = scoreValue.Value,
        UpgradeClickTree = upgradeManager.GetDataToSave(),
        UpgradeIncomeTree = shopManager.GetDataToSave()
    };

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
        }
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

    public void ResetData()
    {
        scoreValue.Value = 0;
        shopManager.Reset();
        upgradeManager.Reset();
    }
}