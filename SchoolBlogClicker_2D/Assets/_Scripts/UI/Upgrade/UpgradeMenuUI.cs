using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour, ISubscribe
{
    [SerializeField] private GameObject canvasGameBtnsUI;
    [SerializeField] private GameObject tapZone;
    [SerializeField] private UpgradeMenuState upgradeMenuState = UpgradeMenuState.ClickMenu;

    [SerializeField] private GameObject clickMenuBtn;
    [SerializeField] private GameObject passiveMenuBtn;
    
    [SerializeField] private GameObject clickUpgradeContent;
    [SerializeField] private GameObject passiveUpgradeContent;

    private void OnEnable()
    {
        UpdateColorsUpgradeMenu();
    }

    public void OnExitUpgradeMenu()
    {
        canvasGameBtnsUI.SetActive(true);
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
        if (upgradeMenuState == UpgradeMenuState.ClickMenu) clickMenuBtn.GetComponent<Button>().Select();
        else passiveMenuBtn.GetComponent<Button>().Select();
        
        clickUpgradeContent.SetActive(upgradeMenuState == UpgradeMenuState.ClickMenu);
        passiveUpgradeContent.SetActive(upgradeMenuState == UpgradeMenuState.PassiveMenu);
    }

    public void Subscribes()
    {
        
    }

    public void Unsubscribes()
    {
    }
}