using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeFieldPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button actionButton;

    private UpgradeFieldModel _data;

    public void Initialize(UpgradeFieldModel data)
    {
        _data = data;

        titleText.text = data.Title;
        valueText.text = data.Value;
        descriptionText.text = data.Description;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Debug.Log($"Clicked: {_data.Title}");
    }
}
