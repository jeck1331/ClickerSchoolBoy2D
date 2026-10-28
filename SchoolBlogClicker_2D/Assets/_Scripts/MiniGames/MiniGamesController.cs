using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGamesController : MonoBehaviour
{
    [SerializeField] private Button btnFlappyPinguin;
    [SerializeField] private Button btnWhackMole;
    [SerializeField] private Button btnPopIt;
    [SerializeField] private Button btnFruitSchool;
    
    private void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "WhackMole":
                btnWhackMole.interactable = false;
                break;
            case "FruitSchool":
                btnFruitSchool.interactable = false;
                break;
            case "PopIt":
                btnPopIt.interactable = false;
                break;
            case "FlappyPinguin":
                btnFlappyPinguin.interactable = false;
                break;
            default: break;
        }
    }

    public void RunFlappyPinguin()
    {
        SceneManager.LoadScene("FlappyPinguin");
    }
    public void RunWhackMole()
    {
        SceneManager.LoadScene("WhackMole");
    }
    public void RunPopIt()
    {
        SceneManager.LoadScene("PopIt");
    }
    public void RunFruitSchool()
    {
        SceneManager.LoadScene("FruitSchool");
    }
}
