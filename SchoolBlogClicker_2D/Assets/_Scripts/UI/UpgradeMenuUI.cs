using _Scripts.Events;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour, ISubscribe
{
    [SerializeField]
    private GameObject canvasGame;
    [SerializeField]
    private GameObject tapZone;

    [SerializeField]
    private TMP_Text CountText;
    [SerializeField]
    private TMP_Text SubscribersText;
    [SerializeField]
    private TMP_Text CountPerSecText;

    private void OnEnable()
    {
    }

    public void OnExitUpgradeMenu()
    {
        canvasGame.SetActive(true);
        tapZone.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //gameState.UpdateState -= UpdateState;
    }
    private void OnDestroy()
    {
        //gameState.UpdateState -= UpdateState;
    }

    private void UpdateTextCoins(ulong coins, bool isAnimate = false)
    {
        CountText.text = coins.ToString();
    }

    public void Subscribes()
    {
        //CoinsEvents.OnCoinsChanged += UpdateTextCoins;
    }

    public void Unsubscribes()
    {
        //CoinsEvents.OnCoinsChanged += UpdateTextCoins;
    }
}
