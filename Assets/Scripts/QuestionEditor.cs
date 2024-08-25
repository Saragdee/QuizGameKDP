using entity;
using manager;
using UnityEngine;
using UnityEngine.UI;

public class QuestionEditor : MonoBehaviour
{
    public InputField questionTitleField;
    public Dropdown questionTypeDropdown;
    public InputField[] multipleChoiceAnswerFields;
    public Toggle[] correctAnswerToggles;
    public Toggle trueToggle;
    public Toggle falseToggle;
    public InputField timeLimitField;
    public Button saveButton;
    public GameObject multipleChoicePanel;
    public GameObject trueFalsePanel;

    private int currentQuestionIndex = -1;
    private const int DefaultTimeLimit = 15;

    private void Start()
    {
        saveButton.onClick.AddListener(SaveQuestion);
        questionTypeDropdown.onValueChanged.AddListener(OnQuestionTypeChanged);

        // Set default value for time limit
        timeLimitField.text = DefaultTimeLimit.ToString();
        timeLimitField.onEndEdit.AddListener(ValidateTimeLimit);

        // Initially hide all editor elements until a question is selected
        ClearEditor();
    }

    private void ValidateTimeLimit(string input)
    {
        // Check if the input is empty or non-numeric
        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out _))
            timeLimitField.text = DefaultTimeLimit.ToString();
    }

    public void LoadQuestion(QuestionEntity question)
    {
        currentQuestionIndex = question.index;
        questionTitleField.text = question.questionText;
        questionTypeDropdown.value = question.type == "Multiple Choice" ? 0 : 1;
        timeLimitField.text = question.timeLimit.ToString();

        OnQuestionTypeChanged(questionTypeDropdown.value);

        if (question.type == "Multiple Choice")
        {
            for (var i = 0; i < multipleChoiceAnswerFields.Length; i++)
            {
                multipleChoiceAnswerFields[i].text = i < question.answers.Count ? question.answers[i] : string.Empty;
                correctAnswerToggles[i].isOn = i == question.correctAnswerIndex;
            }
        }
        else if (question.type == "True/False")
        {
            trueToggle.isOn = question.correctAnswerIndex == 0;
            falseToggle.isOn = question.correctAnswerIndex == 1;
        }

        EnableEditor(true);
    }

    private void OnQuestionTypeChanged(int newTypeIndex)
    {
        var isMultipleChoice = newTypeIndex == 0;

        multipleChoicePanel.SetActive(isMultipleChoice);
        trueFalsePanel.SetActive(!isMultipleChoice);
    }

    public void SaveQuestion()
    {
        if (currentQuestionIndex == -1) return;

        var question = QuestionManager.Instance.GetQuestion(currentQuestionIndex);
        question.questionText = questionTitleField.text;
        question.type = questionTypeDropdown.value == 0 ? "Multiple Choice" : "True/False";

        // Ensure time limit is valid before saving
        ValidateTimeLimit(timeLimitField.text);
        question.timeLimit = int.Parse(timeLimitField.text);

        question.answers.Clear();
        if (question.type == "Multiple Choice")
        {
            for (var i = 0; i < multipleChoiceAnswerFields.Length; i++)
                if (!string.IsNullOrEmpty(multipleChoiceAnswerFields[i].text))
                    question.answers.Add(multipleChoiceAnswerFields[i].text);

            for (var i = 0; i < correctAnswerToggles.Length; i++)
                if (correctAnswerToggles[i].isOn)
                {
                    question.correctAnswerIndex = i;
                    break;
                }
        }
        else if (question.type == "True/False")
        {
            question.answers.Add("True");
            question.answers.Add("False");

            question.correctAnswerIndex = trueToggle.isOn ? 0 : 1;
        }

        QuestionManager.Instance.UpdateQuestion(question);
    }

    public void ClearEditor()
    {
        questionTitleField.text = "";
        questionTypeDropdown.value = 0;

        foreach (var field in multipleChoiceAnswerFields) field.text = "";

        foreach (var toggle in correctAnswerToggles) toggle.isOn = false;

        trueToggle.isOn = false;
        falseToggle.isOn = false;

        timeLimitField.text = DefaultTimeLimit.ToString();

        EnableEditor(false);
    }

    private void EnableEditor(bool enable)
    {
        questionTitleField.gameObject.SetActive(enable);
        questionTypeDropdown.gameObject.SetActive(enable);
        multipleChoicePanel.SetActive(enable && questionTypeDropdown.value == 0);
        trueFalsePanel.SetActive(enable && questionTypeDropdown.value == 1);
        timeLimitField.gameObject.SetActive(enable);
        saveButton.gameObject.SetActive(enable);
    }
}