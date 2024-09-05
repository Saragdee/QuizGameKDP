using System.Collections.Generic;
using System.IO;
using entity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace manager
{
    public class QuestionManager : MonoBehaviour
    {
        public static QuestionManager Instance;

        public GameObject questionCardPrefab;
        public Transform questionListTransform;
        public Button addQuestionButton;
        public int maxQuestions = 15;

        private List<QuestionEntity> questions = new List<QuestionEntity>();
        private int selectedQuestionIndex = -1;
        public Button saveQuizButton;

        public QuestionEditor questionEditor;

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
            addQuestionButton.onClick.RemoveAllListeners();
            addQuestionButton.onClick.AddListener(AddQuestion);
            UpdateAddButtonState();
            saveQuizButton.onClick.AddListener(SaveQuiz);
        }

        public void AddQuestion()
        {
            if (questions.Count >= maxQuestions)
            {
                return;
            }

            QuestionEntity newQuestion = new QuestionEntity(questionCardPrefab, questionListTransform, questions.Count);
            questions.Add(newQuestion);
            
            UpdateAddButtonState();
        }

        public void RemoveQuestion(int index)
        {
            if (index >= 0 && index < questions.Count)
            {
                // Destroy the UI card first
                questions[index].DestroyCard();

                // Remove the question from the list
                questions.RemoveAt(index);

                // Update the indices of the remaining questions
                for (int i = index; i < questions.Count; i++)
                {
                    questions[i].UpdateIndex(i);
                }

                // Handle the selected question logic
                if (selectedQuestionIndex == index)
                {
                    ClearQuestionEditor();
                    selectedQuestionIndex = -1; // No selection after removal
                }
                else if (selectedQuestionIndex > index)
                {
                    selectedQuestionIndex--; // Adjust selection index if necessary
                }

                Debug.Log($"Question removed. Current number of questions: {questions.Count}");

                UpdateAddButtonState();
            }
        }

        public void SaveQuiz()
        {
            QuizData quizData = new QuizData();
            quizData.title = "My Quiz"; // Optionally, you can add a field for the title in your UI

            // Gather all questions
            foreach (var question in questions)
            {
                quizData.questions.Add(question);
            }

            // Serialize to JSON
            string json = JsonUtility.ToJson(quizData, true);

            // Open file explorer and save the JSON file
            string path = Application.persistentDataPath + "/SavedQuizzes";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = path + "/quiz.json"; // You can modify this to let the user choose the filename
            File.WriteAllText(filePath, json);

            Debug.Log("Quiz saved to " + filePath);

            // Optionally, open the file explorer at the save location
            Application.OpenURL("file://" + path);
        }

        public void LoadQuiz()
        {
            // Define the path where the quiz file is saved
            string filePath = Application.persistentDataPath + "/SavedQuizzes/quiz.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                QuizData loadedQuiz = JsonUtility.FromJson<QuizData>(json);

                // Clear current questions
                foreach (var question in questions)
                {
                    question.DestroyCard(); // Assuming you have this method to clean up UI
                }

                questions.Clear();

                // Load the quiz data
                foreach (var question in loadedQuiz.questions)
                {
                    QuestionEntity newQuestion =
                        new QuestionEntity(questionCardPrefab, questionListTransform, questions.Count);
                    newQuestion.questionText = question.questionText;
                    newQuestion.type = question.type;
                    newQuestion.answers = question.answers;
                    newQuestion.correctAnswerIndex = question.correctAnswerIndex;
                    newQuestion.timeLimit = question.timeLimit;

                    questions.Add(newQuestion);
                    newQuestion.UpdateCardText(); // Assuming this updates the UI with the question data
                }

                Debug.Log("Quiz loaded from " + filePath);
            }
            else
            {
                Debug.LogError("Quiz file not found at " + filePath);
            }
        }

        private void UpdateAddButtonState()
        {
            addQuestionButton.interactable = questions.Count < maxQuestions;
        }

        private void ClearQuestionEditor()
        {
            Debug.Log("Clearing question editor UI.");
            questionEditor.questionTitleField.text = "";
            questionEditor.questionTypeDropdown.value = 0;
            foreach (var field in questionEditor.multipleChoiceAnswerFields)
            {
                field.text = "";
            }

            foreach (var toggle in questionEditor.correctAnswerToggles)
            {
                toggle.isOn = false;
            }

            questionEditor.trueToggle.isOn = false;
            questionEditor.falseToggle.isOn = false;
            questionEditor.timeLimitField.text = "";
        }

        public void SelectQuestion(int index)
        {
            if (selectedQuestionIndex >= 0 && selectedQuestionIndex < questions.Count)
            {
                questions[selectedQuestionIndex].SetSelected(false);
            }

            selectedQuestionIndex = index;
            questions[selectedQuestionIndex].SetSelected(true);

            if (questionEditor != null)
            {
                questionEditor.LoadQuestion(questions[selectedQuestionIndex]);
            }
            else
            {
                Debug.LogError("QuestionEditor is not assigned in QuestionManager.");
            }
        }

        public QuestionEntity GetQuestion(int index)
        {
            return questions[index];
        }

        public void UpdateQuestion(QuestionEntity updatedQuestion)
        {
            questions[updatedQuestion.index] = updatedQuestion;
        }
        
        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}