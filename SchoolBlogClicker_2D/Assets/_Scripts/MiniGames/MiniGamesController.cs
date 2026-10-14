using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesController : MonoBehaviour
{
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
