using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class UiToolkitManager : MonoBehaviour
{
    private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset uiShop;

    private Button _btnUpgrade; //BtnUpgrade
    private Button _btnShop; //BtnShop
    private Button _btnLeaderboard; //BtnLeaderboard
    private Button _btnSettings; //BtnSettings

    [CanBeNull] private TemplateContainer _shopInstance;
    [CanBeNull] private string _shopInstanceType;
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        
        var navButtons = _uiDocument.rootVisualElement.Q<VisualElement>("NavButtons");
        _btnUpgrade = navButtons.Q<Button>("BtnUpgrade");
        _btnShop = navButtons.Q<Button>("BtnShop");
        _btnLeaderboard = navButtons.Q<Button>("BtnLeaderboard");
        _btnSettings = navButtons.Q<Button>("BtnSettings");
        
        _btnUpgrade.clicked += BtnUpgradeHandleClick;
        _btnShop.clicked += BtnShopHandleClick;
        _btnLeaderboard.clicked += BtnLeaderboardHandleClick;
        _btnSettings.clicked += BtnSettingsHandleClick;
    }

    private void BtnUpgradeHandleClick()
    {
        HideOrVisibleShopInstance("Upgrades");
    }
    private void BtnShopHandleClick()
    {
        HideOrVisibleShopInstance("Shop");
    }
    private void BtnSettingsHandleClick()
    {
        
    }
    private void BtnLeaderboardHandleClick()
    {
        
    }

    private void HideOrVisibleShopInstance(string shopInstanceType)
    {
        void AddShopInstance()
        {
            _shopInstanceType = shopInstanceType;
            _shopInstance = uiShop.Instantiate();
            _shopInstance.Q<Label>("ShopTitle").text = shopInstanceType;
            _uiDocument.rootVisualElement.Q<VisualElement>("Body").Add(_shopInstance.Q<VisualElement>("ShopMenuContainer"));
        }
        if (_shopInstance != null && shopInstanceType == _shopInstanceType) {
            _uiDocument.rootVisualElement.Q<VisualElement>("Body").Clear();
            _shopInstance = null;
            _shopInstanceType = null;
        }
        else
        {
            if (_shopInstance != null && shopInstanceType != _shopInstanceType)
                _uiDocument.rootVisualElement.Q<VisualElement>("Body").Clear();
            
            AddShopInstance();
        }
    }
}
