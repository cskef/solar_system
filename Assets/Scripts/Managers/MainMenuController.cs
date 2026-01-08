using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string arSceneName = "SampleScene";

    // Explorer Tap 
    public void GoExplore()
    {
        LaunchState.StartInQuiz = false;
        LaunchState.UseVuforia = false;
        SceneManager.LoadScene(arSceneName);
    }

    // Explorer Vuforia (ImageTarget)
    public void GoExploreVuforia()
    {
        LaunchState.StartInQuiz = false;
        LaunchState.UseVuforia = true;
        SceneManager.LoadScene(arSceneName);
    }

    public void GoQuiz()
    {
        LaunchState.StartInQuiz = true;
        LaunchState.UseVuforia = false; // quiz sans vuforia
        SceneManager.LoadScene(arSceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
