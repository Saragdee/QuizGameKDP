using UnityEngine;

namespace manager
{
    public class AnswerFeedbackManager : MonoBehaviour
    {
        public Animator correctPanelAnimator;
        public Animator wrongPanelAnimator;

        public void ShowCorrectAnswer()
        {
            correctPanelAnimator.SetTrigger("ShowCorrect");
        }

        public void ShowWrongAnswer()
        {
            wrongPanelAnimator.SetTrigger("ShowWrong");
        }
    }
}