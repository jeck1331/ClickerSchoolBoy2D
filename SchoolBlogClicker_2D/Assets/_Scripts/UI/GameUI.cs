using _Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour, ISubscribe
{
    [SerializeField] private GameObject canvasUpgradeMenu;
    [SerializeField] private GameObject tapZoneGM;

    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _subscribersText;

    [SerializeField] private Animator animator;


    private void Awake()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnEnable()
    {
        (this as ISubscribe).Subscribes();
    }

    private void OnDisable()
    {
        (this as ISubscribe).Unsubscribes();
    }
    private void OnDestroy()
    {
        (this as ISubscribe).Unsubscribes();
    }

    private void UpdateTextCoins(ulong coins, bool isAnimate)
    {
        _coinsText.text = coins / 1000 >= 1 ? $"{Mathf.Round(coins / 1000)}K" : coins.ToString();

        if (isAnimate)
            animator?.SetTrigger("increment");
    }

    public void ResetCount()
    {

    }

    public void Upgrade()
    {

    }

    public void Downgrade()
    {

    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenUpgradeMenu()
    {
        canvasUpgradeMenu.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(false);
        tapZoneGM.SetActive(false);
    }

    void ISubscribe.Subscribes()
    {
        //CoinsEvents.OnCoinsChanged += UpdateTextCoins;
    }

    void ISubscribe.Unsubscribes()
    {
        //CoinsEvents.OnCoinsChanged -= UpdateTextCoins;
    }
}
