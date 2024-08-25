using System;
using System.Collections.Generic;

namespace entity
{
    [Serializable]
    public class QuizData
    {
        public string title; 
        public List<QuestionEntity> questions = new List<QuestionEntity>();
    }
}