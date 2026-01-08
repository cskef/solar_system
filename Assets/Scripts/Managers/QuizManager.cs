using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("Scene")]
    public string menuSceneName = "MenuScene"; 

    [Header("UI References (assign in Inspector)")]
    public GameObject quizPanel;

    public TextMeshProUGUI questionText;
    public Button[] answerButtons;                 // 4 boutons
    public TextMeshProUGUI[] answerButtonTexts;    // auto si vide

    public TextMeshProUGUI feedbackText;           // bonne/mauvaise + bonne réponse
    public TextMeshProUGUI scoreText;              // Score: x/y

    public Button replayButton;                    // visible seulement à la fin
    public Button closeButton;                     // Fermer -> menu

    [Header("Timing")]
    public float feedbackDelay = 0.7f;

    [Header("Quiz Data")]
    public List<QuestionData> questions = new List<QuestionData>();

    private List<QuestionData> workingQuestions = new List<QuestionData>();
    private int currentIndex = 0;
    private int score = 0;
    private bool locked = false;

    private void Awake()
    {
        // Auto-fill textes depuis les boutons si pas assigné
        if (answerButtons != null && answerButtons.Length > 0)
        {
            if (answerButtonTexts == null || answerButtonTexts.Length != answerButtons.Length)
            {
                answerButtonTexts = new TextMeshProUGUI[answerButtons.Length];
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if (answerButtons[i] != null)
                        answerButtonTexts[i] = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>(true);
                }
            }
        }

        // Close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseToMenu);
        }

        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(Replay);
            replayButton.gameObject.SetActive(false);
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (LaunchState.StartInQuiz)
            StartQuiz();
    }

    public void StartQuiz()
    {
        if (quizPanel != null) quizPanel.SetActive(true);

        // Reset
        score = 0;
        currentIndex = 0;
        locked = false;

        if (feedbackText != null) feedbackText.gameObject.SetActive(false);
        if (replayButton != null) replayButton.gameObject.SetActive(false);

        // Copie + mélange des questions
        workingQuestions = new List<QuestionData>(questions);
        Shuffle(workingQuestions);

        // Brancher les callbacks réponses
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            if (answerButtons[i] != null)
            {
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
            }
        }

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        locked = false;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (workingQuestions == null || workingQuestions.Count == 0)
        {
            if (questionText != null) questionText.text = "Aucune question n'a été configurée.";
            SetAnswersVisible(false);
            return;
        }

        if (currentIndex >= workingQuestions.Count)
        {
            ShowFinal();
            return;
        }

        var q = workingQuestions[currentIndex];

        if (questionText != null)
            questionText.text = q.question;

        // Afficher réponses
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = (q.answers != null && i < q.answers.Length);

            if (answerButtons[i] != null)
            {
                answerButtons[i].gameObject.SetActive(hasAnswer);
                answerButtons[i].interactable = true;
            }

            if (hasAnswer && answerButtonTexts[i] != null)
                answerButtonTexts[i].text = q.answers[i];
        }

        if (scoreText != null)
            scoreText.text = $"Score: {score}/{workingQuestions.Count}";
    }

    private void OnAnswerSelected(int answerIndex)
    {
        if (locked) return;
        locked = true;

        var q = workingQuestions[currentIndex];

        bool correct = (answerIndex == q.correctIndex);
        if (correct) score++;

        // Bloquer les boutons pendant le feedback
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].interactable = false;
        }

        // Feedback
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);

            if (correct)
            {
                feedbackText.text = "Bonne réponse !";
            }
            else
            {
                string good = GetCorrectAnswerText(q);
                feedbackText.text = $"Mauvaise réponse.\nLa bonne réponse était : {good}";
            }
        }

        StartCoroutine(NextAfterDelay());
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDelay);
        currentIndex++;
        ShowQuestion();
    }

    private void ShowFinal()
    {
        SetAnswersVisible(false);

        if (replayButton != null)
            replayButton.gameObject.SetActive(true);

        float ratio = (workingQuestions.Count == 0) ? 0f : (float)score / workingQuestions.Count;
        string finalMsg = GetFinalFeedback(ratio, score, workingQuestions.Count);

        if (questionText != null)
            questionText.text = $"Quiz terminé \nScore final : {score}/{workingQuestions.Count}\n\n{finalMsg}";

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (scoreText != null)
            scoreText.text = "";
    }

    public void Replay()
    {
        // Rejouer = relancer + mélanger
        StartQuiz();
    }

    public void CloseToMenu()
    {
        LaunchState.StartInQuiz = false;
        LaunchState.UseVuforia = false;
        SceneManager.LoadScene(menuSceneName);
    }

    // ---------------- Helpers ----------------

    private void SetAnswersVisible(bool visible)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(visible);
        }
    }

    private string GetCorrectAnswerText(QuestionData q)
    {
        if (q == null || q.answers == null) return "(inconnue)";
        int idx = Mathf.Clamp(q.correctIndex, 0, q.answers.Length - 1);
        return q.answers[idx];
    }

    private string GetFinalFeedback(float ratio, int sc, int total)
    {
        // Tu peux ajuster les seuils
        if (ratio >= 0.85f) return " Excellent ! Bravo, tu maîtrises \ntrès bien le système solaire.";
        if (ratio >= 0.65f) return " Très bien ! Tu as de bonnes connaissances, \ncontinue comme ça.";
        if (ratio >= 0.45f) return " Pas mal, mais tu peux faire mieux.\n Relis les infos des planètes et réessaie.";
        return " Il faut faire plus d’efforts. Explore les planètes,\n écoute les audios, puis refais le quiz.";
    }

    private void Shuffle(List<QuestionData> list)
    {
        // Fisher-Yates
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
