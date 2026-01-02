using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string arSceneName = "SampleScene";

    public void GoExplore()
    {
        PlayerPrefs.SetString(AppLaunch.MODE_KEY, AppLaunch.MODE_EXPLORE);
        SceneManager.LoadScene(arSceneName);
    }

    public void GoQuiz()
    {
        PlayerPrefs.SetString(AppLaunch.MODE_KEY, AppLaunch.MODE_QUIZ);
        SceneManager.LoadScene(arSceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
