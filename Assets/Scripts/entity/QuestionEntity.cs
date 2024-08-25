using System.Collections.Generic;
using manager;
using UnityEngine;
using UnityEngine.UI;

public class QuestionEntity
{
    public List<string> answers = new();
    private readonly Image borderImage; // Reference to the border image
    public int correctAnswerIndex;
    private int index; // Index of the question in the list

    private readonly GameObject questionCard; // The UI element associated with this question
    public string questionText;
    public int timeLimit;
    public string type; // "Multiple Choice" or "True/False"

    public QuestionEntity(GameObject cardPrefab, Transform parentTransform, int index)
    {
        this.index = index;
        questionCard = Object.Instantiate(cardPrefab, parentTransform);
        borderImage =
            questionCard.transform.Find("Border")
                .GetComponent<Image>(); // Assuming the border is a child named "Border"
        UpdateCardText();
        AttachCardListeners();
    }

    private void AttachCardListeners()
    {
        var cardButton = questionCard.GetComponent<Button>();
        if (cardButton != null) cardButton.onClick.AddListener(() => QuestionManager.Instance.SelectQuestion(index));

        var removeButton = questionCard.transform.Find("RemoveButton").GetComponent<Button>();
        if (removeButton != null)
            removeButton.onClick.AddListener(() => QuestionManager.Instance.RemoveQuestion(index));
    }

    public void UpdateCardText()
    {
        var cardText = questionCard.GetComponentInChildren<Text>();
        if (cardText != null) cardText.text = "Question " + (index + 1);
    }

    public void UpdateIndex(int newIndex)
    {
        index = newIndex;
        UpdateCardText();
    }

    public void SetSelected(bool isSelected)
    {
        if (borderImage != null)
            // Set the border to green if selected, otherwise make it transparent
            borderImage.color = isSelected ? new Color(0f, 1f, 0f, 0.5f) : Color.clear;
    }

    public void DestroyCard()
    {
        Object.Destroy(questionCard);
    }
}