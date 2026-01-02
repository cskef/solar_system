using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string arSceneName = "SampleScene";

    public void GoExplore()
    {
        LaunchState.StartInQuiz = false;
        SceneManager.LoadScene(arSceneName);
    }

    public void GoQuiz()
    {
        LaunchState.StartInQuiz = true;
        SceneManager.LoadScene(arSceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
