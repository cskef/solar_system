using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string exploreSceneName = "SampleScene";

    public void GoExplore()
    {
        SceneManager.LoadScene(exploreSceneName);
    }

    public void GoQuiz()
    {
        // Pour l’instant on va dans la scène AR (le quiz viendra après)
        SceneManager.LoadScene(exploreSceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
