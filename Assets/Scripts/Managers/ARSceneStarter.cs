using UnityEngine;

public class ARSceneStarter : MonoBehaviour
{
    public QuizManager quizManager;

    private void Start()
    {
        string mode = PlayerPrefs.GetString(AppLaunch.MODE_KEY, AppLaunch.MODE_EXPLORE);

        if (mode == AppLaunch.MODE_QUIZ && quizManager != null)
        {
            quizManager.StartQuiz();
        }
    }
}
