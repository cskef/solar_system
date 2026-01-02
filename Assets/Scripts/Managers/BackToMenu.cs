using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public string menuSceneName = "MenuScene";

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
