using UnityEngine;
using System.Collections;

public class ARSceneStarter : MonoBehaviour
{
    public QuizManager quizManager;

    private IEnumerator Start()
    {
        // Attendre 1 frame pour que tous les scripts/UI soient initialisés
        yield return null;

        if (LaunchState.StartInQuiz)
        {
            Debug.Log("AR: StartInQuiz = True (launching quiz)");

            if (quizManager == null)
            {
                Debug.LogError("ARSceneStarter: quizManager is NULL. Assign QuizManagerObject in inspector.");
            }
            else
            {
                quizManager.StartQuiz();
            }
        }

        // Reset pour éviter que ça relance le quiz après
        LaunchState.StartInQuiz = false;
    }
}
