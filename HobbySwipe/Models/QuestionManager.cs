using Apache.Arrow;
using MathNet.Numerics.RootFinding;
using System.Collections.Generic;
using System;

namespace HobbySwipe.Models
{
    public class QuestionManager
    {
        private readonly Dictionary<Guid, Question> _questions;
        private readonly Guid _firstQuestionId;
        private Guid? _currentQuestionId;

        public QuestionManager(List<Question> questions)
        {
            _questions = questions.ToDictionary(q => q.ID);
            _firstQuestionId = questions.First().ID;
            _currentQuestionId = _firstQuestionId;
        }

        public Question CurrentQuestion()
        {
            return _questions[_currentQuestionId.Value];
        }

        public void MoveToNextQuestion(Answer answer)
        {
            var currentQuestion = _questions[_currentQuestionId.Value];

            Guid? nextQuestionId = null;
            if (currentQuestion.AnswerType == AnswerType.MultipleChoice && currentQuestion.ChoiceDependentChildQuestionId.ContainsKey(answer.Response))
            {
                nextQuestionId = currentQuestion.ChoiceDependentChildQuestionId[answer.Response];
            }
            else if (currentQuestion.AnswerType == AnswerType.OpenEnded && currentQuestion.OpenEndedChildQuestionId.HasValue)
            {
                nextQuestionId = currentQuestion.OpenEndedChildQuestionId.Value;
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
            return _currentQuestionId.HasValue ? _questions[_currentQuestionId.Value] : null;
        }
    }

}
