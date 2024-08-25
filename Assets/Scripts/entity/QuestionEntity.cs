using System;
using System.Collections.Generic;
using manager;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace entity
{
    [Serializable]
    public class QuestionEntity
    {
        public string questionText;
        public string type;
        public List<string> answers = new List<string>();
        public int correctAnswerIndex;
        public int timeLimit;

        private GameObject questionCard; 
        public int index; 
        private Image borderImage; 

        public QuestionEntity(GameObject cardPrefab, Transform parentTransform, int index)
        {
            this.index = index;
            questionCard = Object.Instantiate(cardPrefab, parentTransform);
            borderImage = questionCard.transform.Find("Border").GetComponent<Image>(); 
            UpdateCardText();
            AttachCardListeners();
        }

        private void AttachCardListeners()
        {
            Button cardButton = questionCard.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => QuestionManager.Instance.SelectQuestion(index));
            }

            Button removeButton = questionCard.transform.Find("RemoveButton").GetComponent<Button>();
            if (removeButton != null)
            {
                removeButton.onClick.AddListener(() => QuestionManager.Instance.RemoveQuestion(index));
            }
        }

        public void UpdateCardText()
        {
            Text cardText = questionCard.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = "Question " + (index + 1);
            }
        }

        public void UpdateIndex(int newIndex)
        {
            this.index = newIndex;
            UpdateCardText();
        }

        public void SetSelected(bool isSelected)
        {
            if (borderImage != null)
            {
                borderImage.color = isSelected ? new Color(0f, 1f, 0f, 0.5f) : Color.clear;
            }
        }

        public void DestroyCard()
        {
            Object.Destroy(questionCard);
        }
    }
}
