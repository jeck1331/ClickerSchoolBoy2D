using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour, ISubscribe
{
    [SerializeField] private GameObject tapZone;
    [SerializeField] private UpgradeMenuState upgradeMenuState = UpgradeMenuState.ClickMenu;

    [SerializeField] private GameObject clickMenuBtn;
    [SerializeField] private GameObject passiveMenuBtn;
    
    [SerializeField] private GameObject clickUpgradeContent;
    [SerializeField] private GameObject passiveUpgradeContent;
    
    [SerializeField] private Material matSelectedBtn;
    
    [CanBeNull] private Image _imageClickMenuBtn;
    [CanBeNull] private Image _imagePassiveMenuBtn;

    private void OnEnable()
    {
        _imageClickMenuBtn = clickMenuBtn.GetComponent<Image>();
        _imagePassiveMenuBtn = passiveMenuBtn.GetComponent<Image>();
        
        UpdateColorsUpgradeMenu();
    }

    public void OnExitUpgradeMenu()
    {
        tapZone.SetActive(true);
        gameObject.SetActive(false);
    }
    
    public void OnClickUpgradeMenuState() => OnChangeUpgradeMenuState(UpgradeMenuState.ClickMenu);
    public void OnPassiveUpgradeMenuState() => OnChangeUpgradeMenuState(UpgradeMenuState.PassiveMenu);

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
    }
    
    private void OnChangeUpgradeMenuState(UpgradeMenuState state)
    {
        upgradeMenuState = state;
        UpdateColorsUpgradeMenu();
    }

    private void UpdateColorsUpgradeMenu()
    {
        if (_imagePassiveMenuBtn != null && _imageClickMenuBtn != null)
        {
            if (upgradeMenuState == UpgradeMenuState.ClickMenu)
            {
                _imageClickMenuBtn.material = matSelectedBtn;
                _imagePassiveMenuBtn.material = null;
            }
            else
            {
                _imageClickMenuBtn.material = null;
                _imagePassiveMenuBtn.material = matSelectedBtn;
            }
        
            clickUpgradeContent.SetActive(upgradeMenuState == UpgradeMenuState.ClickMenu);
            passiveUpgradeContent.SetActive(upgradeMenuState == UpgradeMenuState.PassiveMenu); 
        }
    }

    public void Subscribes()
    {
        
    }

    public void Unsubscribes()
    {
    }
}