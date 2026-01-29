using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour, ISubscribe
{
    [SerializeField] private GameObject canvasGameBtnsUI;
    [SerializeField] private GameObject tapZone;

    private void OnEnable()
    {
    }

    public void OnExitUpgradeMenu()
    {
        canvasGameBtnsUI.SetActive(true);
        tapZone.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
    }

    public void Subscribes()
    {
        
    }

    public void Unsubscribes()
    {
    }
}