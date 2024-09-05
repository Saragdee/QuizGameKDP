using System.Collections;
using System.IO;
using entity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace manager
{
    public class StudentModeManager : MonoBehaviour
    {
        public static StudentModeManager Instance;

        
        public Text quizTitleText;
        public Text questionText;
        public Text timerText;
        public Text livesText;

        public GameObject multipleChoiceHolder;
        public Button[] answerButtons; 
        public GameObject trueFalseHolder;
        public Button trueButton; 
        public Button falseButton; 

        public GameObject correctPanel;
        public GameObject incorrectPanel;

        public Button loadQuizButton;
        public Button mainMenuButton;
        public Button nextQuestionButton;

        private QuizData currentQuiz;
        private int currentQuestionIndex = 0;
        private int lives = 3;
        private float timeRemaining;
        private bool quizLoaded = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            loadQuizButton.onClick.AddListener(LoadQuiz);
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            nextQuestionButton.onClick.AddListener(NextQuestion);
            nextQuestionButton.interactable = false;

            correctPanel.SetActive(false);
            incorrectPanel.SetActive(false);
            
            SetAnswerHoldersActive(false);
        }

        public void LoadQuiz()
        {
            string path = Application.persistentDataPath + "/SavedQuizzes/quiz.json"; 

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                currentQuiz = JsonUtility.FromJson<QuizData>(json);
                quizLoaded = true;
                quizTitleText.text = currentQuiz.title;
                lives = 3;
                livesText.text = "Lives: " + lives;
                currentQuestionIndex = 0;
                DisplayNextQuestion();
            }
            else
            {
                Debug.LogError("Quiz file not found at " + path);
            }
        }

        private void DisplayNextQuestion()
        {
            if (currentQuestionIndex < currentQuiz.questions.Count)
            {
                QuestionEntity currentQuestion = currentQuiz.questions[currentQuestionIndex];
                questionText.text = currentQuestion.questionText;

                Debug.Log("Displaying question: " + currentQuestion.questionText);
                Debug.Log("Question Type: " + currentQuestion.type);

                if (currentQuestion.type == "Multiple Choice")
                {
                    SetAnswerHoldersActive(true, false);

                    for (int i = 0; i < answerButtons.Length; i++)
                    {
                        if (i < currentQuestion.answers.Count)
                        {
                            answerButtons[i].gameObject.SetActive(true);
                            answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
                            int index = i;
                            answerButtons[i].onClick.RemoveAllListeners();
                            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
                        }
                        else
                        {
                            answerButtons[i].gameObject.SetActive(false);
                        }
                    }
                }
                else if (currentQuestion.type == "True/False")
                {
                    Debug.Log("Activating True/False buttons");
                    SetAnswerHoldersActive(false, true);

                    trueButton.gameObject.SetActive(true);
                    falseButton.gameObject.SetActive(true);

                    trueButton.GetComponentInChildren<Text>().text = "True";
                    falseButton.GetComponentInChildren<Text>().text = "False";

                    trueButton.onClick.RemoveAllListeners();
                    trueButton.onClick.AddListener(() => CheckAnswer(0));

                    falseButton.onClick.RemoveAllListeners();
                    falseButton.onClick.AddListener(() => CheckAnswer(1));
                }

                timeRemaining = currentQuestion.timeLimit;
                StartCoroutine(StartTimer());
            }
            else
            {
                EndQuiz();
            }
        }

        private void SetAnswerHoldersActive(bool multipleChoiceActive, bool trueFalseActive = false)
        {
            Debug.Log("Setting MultipleChoiceHolder active: " + multipleChoiceActive);
            Debug.Log("Setting TrueFalseHolder active: " + trueFalseActive);

            multipleChoiceHolder.SetActive(multipleChoiceActive);
            trueFalseHolder.SetActive(trueFalseActive);

            if (!multipleChoiceActive)
            {
                foreach (var button in answerButtons)
                {
                    button.gameObject.SetActive(false);
                }
            }

            if (!trueFalseActive)
            {
                trueButton.gameObject.SetActive(false);
                falseButton.gameObject.SetActive(false);
            }
        }


        private IEnumerator StartTimer()
        {
            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
                yield return null;
            }

            CheckAnswer(-1);
        }

        private void CheckAnswer(int selectedAnswerIndex)
        {
            StopAllCoroutines();

            QuestionEntity currentQuestion = currentQuiz.questions[currentQuestionIndex];
            bool isCorrect = selectedAnswerIndex == currentQuestion.correctAnswerIndex;

            if (isCorrect)
            {
                correctPanel.SetActive(true);
                StartCoroutine(ShowCorrectAnswer());
            }
            else
            {
                incorrectPanel.SetActive(true);
                StartCoroutine(ShowWrongAnswer());
            }
        }

        private IEnumerator ShowCorrectAnswer()
        {
            yield return new WaitForSeconds(1.5f);
            correctPanel.SetActive(false);
            nextQuestionButton.interactable = true;
        }

        private IEnumerator ShowWrongAnswer()
        {
            yield return new WaitForSeconds(1.5f);
            incorrectPanel.SetActive(false);

            lives--;
            livesText.text = "Lives: " + lives;

            if (lives <= 0)
            {
                EndQuiz();
            }
            else
            {
                nextQuestionButton.interactable = true;
            }
        }

        private void NextQuestion()
        {
            currentQuestionIndex++;
            nextQuestionButton.interactable = false;
            DisplayNextQuestion();
        }

        private void EndQuiz()
        {
            questionText.text = "Quiz Over!";
            SetAnswerHoldersActive(false, false);
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
