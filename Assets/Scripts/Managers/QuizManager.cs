using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Data")]
    public QuestionData[] questions;

    [Header("UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons; // 4 boutons
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject restartButton;


    private int currentIndex = 0;
    private int score = 0;
    private bool isFinished = false;
    private ClickRaycaster clickRaycaster;


    private void Awake()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
        if (scoreText != null) scoreText.text = "";

        clickRaycaster = FindObjectOfType<ClickRaycaster>();

        if (restartButton != null)
            restartButton.SetActive(false);


    }

    public void StartQuiz()
    {
        if (restartButton != null)
            restartButton.SetActive(false);


        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("QuizManager: No questions set.");
            return;
        }

        isFinished = false;
        score = 0;
        currentIndex = 0;
        ShuffleQuestions();


        if (quizPanel != null) quizPanel.SetActive(true);
        if (feedbackText != null) feedbackText.text = "";

        if (clickRaycaster != null) clickRaycaster.enabled = false;


        ShowQuestion();
    }

    public void CloseQuiz()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
        if (scoreText != null) scoreText.text = "";

        if (clickRaycaster != null) clickRaycaster.enabled = true;

    }

    private void ShowQuestion()
    {
        if (currentIndex >= questions.Length)
        {
            FinishQuiz();
            return;
        }

        QuestionData q = questions[currentIndex];
        if (questionText != null) questionText.text = q.question;

        // Afficher score en direct
        if (scoreText != null) scoreText.text = $"Score : {score}/{questions.Length}";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int btnIndex = i;
            answerButtons[i].onClick.RemoveAllListeners();

            if (i < q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                var txt = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = q.answers[i];

                answerButtons[i].onClick.AddListener(() => OnAnswer(btnIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnAnswer(int chosenIndex)
    {
        if (isFinished) return;

        QuestionData q = questions[currentIndex];

        if (chosenIndex == q.correctIndex)
        {
            score++;
            if (feedbackText != null) feedbackText.text = " Bonne réponse !";
        }
        else
        {
            string good = (q.correctIndex >= 0 && q.correctIndex < q.answers.Length) ? q.answers[q.correctIndex] : "—";
            if (feedbackText != null) feedbackText.text = $" Faux. Bonne réponse : {good}";
        }

        currentIndex++;
        Invoke(nameof(ShowQuestion), 0.3f); // petit délai pour lire feedback
    }

    private void FinishQuiz()
    {
        isFinished = true;

        if (questionText != null) questionText.text = "Quiz terminé !";
        if (scoreText != null) scoreText.text = $"Score final : {score}/{questions.Length}";
        if (feedbackText != null)
        {
            feedbackText.text = (score >= questions.Length / 2) ? " Bravo !" : " Encore un effort !";
        }

        if (restartButton != null)
            restartButton.SetActive(true);

        // Désactiver boutons réponses
        foreach (var b in answerButtons)
            b.gameObject.SetActive(false);
    }

    private void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Length; i++)
        {
            int r = Random.Range(i, questions.Length);
            var tmp = questions[i];
            questions[i] = questions[r];
            questions[r] = tmp;
        }
    }

}
