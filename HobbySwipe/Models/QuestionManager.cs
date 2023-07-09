using Apache.Arrow;
using MathNet.Numerics.RootFinding;
using System.Collections.Generic;
using System;
using HobbySwipe.Data.Entities;

namespace HobbySwipe.Models
{
    public class QuestionManager
    {
        private readonly Dictionary<string, Question> _questions;
        private readonly string _firstQuestionId;
        private string _currentQuestionId;

        public QuestionManager(List<Question> questions)
        {
            _questions = questions.ToDictionary(q => q.Id);
            _firstQuestionId = questions.First().Id;
            _currentQuestionId = _firstQuestionId;
        }

        public Question CurrentQuestion()
        {
            return _questions[_currentQuestionId];
        }

        public void MoveToNextQuestion(Answer answer)
        {
            _currentQuestionId = answer.QuestionId;
            var currentQuestion = _questions[_currentQuestionId];

            string nextQuestionId = null;
            if (currentQuestion.AnswerType == 2 && currentQuestion.QuestionsOptions != null)
            {
                var option = currentQuestion.QuestionsOptions.FirstOrDefault(x => x.OptionText.ToLower() == answer.Response.ToLower());

                if (option != null && option.NextQuestionId != null)
                {
                    nextQuestionId = option.NextQuestionId;
                }
                else
                {
                    nextQuestionId = currentQuestion.NextQuestionId;
                }
            }
            else
            {
                nextQuestionId = currentQuestion.NextQuestionId;
            }

            _currentQuestionId = nextQuestionId;
        }

        public Question GetFirstQuestion()
        {
            _currentQuestionId = _firstQuestionId;
            return _questions[_firstQuestionId];
        }

        public Question GetNextQuestion()
        {
            return _currentQuestionId != null ? _questions[_currentQuestionId] : null;
        }
    }

}
