using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    [Header("Scene")]
    public string menuSceneName = "MenuScene"; 

    public bool resetLaunchState = true;

    public void GoToMenu()
    {
        if (resetLaunchState)
        {
            LaunchState.StartInQuiz = false;
            LaunchState.UseVuforia = false;
        }

        SceneManager.LoadScene(menuSceneName);
    }
}
