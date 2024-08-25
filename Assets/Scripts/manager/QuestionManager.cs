using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace manager
{
    public class QuestionManager : MonoBehaviour
    {
        public static QuestionManager Instance;

        public GameObject questionCardPrefab;
        public Transform questionListTransform; // This should be the Content object in ScrollView
        public Button addQuestionButton;
        public int maxQuestions = 15;

        private readonly List<QuestionEntity> questions = new();
        private int selectedQuestionIndex = -1;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            addQuestionButton.onClick.RemoveAllListeners();
            addQuestionButton.onClick.AddListener(AddQuestion);
            UpdateAddButtonState();
        }

        public void AddQuestion()
        {
            if (questions.Count >= maxQuestions)
            {
                Debug.Log("Maximum number of questions reached.");
                return;
            }

            var newQuestion = new QuestionEntity(questionCardPrefab, questionListTransform, questions.Count);
            questions.Add(newQuestion);
            Debug.Log("Adding question. Total questions: " + questions.Count);

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
                for (var i = index; i < questions.Count; i++) questions[i].UpdateIndex(i);

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

        private void UpdateAddButtonState()
        {
            addQuestionButton.interactable = questions.Count < maxQuestions;
        }

        private void ClearQuestionEditor()
        {
            Debug.Log("Clearing question editor UI.");
            // Implement this method to clear the question editor UI
        }

        public void SelectQuestion(int index)
        {
            if (selectedQuestionIndex >= 0 && selectedQuestionIndex < questions.Count)
                questions[selectedQuestionIndex].SetSelected(false);

            selectedQuestionIndex = index;
            questions[selectedQuestionIndex].SetSelected(true);

            // Implement logic to display the selected question in the right-side editor
        }
    }
}