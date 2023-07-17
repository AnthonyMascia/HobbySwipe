using Apache.Arrow;
using MathNet.Numerics.RootFinding;
using System.Collections.Generic;
using System;
using HobbySwipe.Data.Entities;

namespace HobbySwipe.Models
{
    /// <summary>
    ///     This class manages a set of Questions.
    /// </summary>
    public class QuestionManager
    {
        private readonly Dictionary<string, Question> _questions;
        private readonly string _firstQuestionId;
        private string _currentQuestionId;
        private readonly Stack<string> _questionHistory;
        private readonly Dictionary<string, Answer> _answers; // To store user's answers

        public QuestionManager(List<Question> questions)
        {
            _questions = questions.ToDictionary(q => q.Id);
            _firstQuestionId = questions.First().Id;
            _currentQuestionId = _firstQuestionId;
            _questionHistory = new Stack<string>();
            _answers = new Dictionary<string, Answer>();
        }

        public Question GetCurrentQuestion()
        {
            return _questions.TryGetValue(_currentQuestionId, out var currentQuestion) ? currentQuestion : null;
        }
        public Question GetFirstQuestion()
        {
            _currentQuestionId = _firstQuestionId;
            return _questions[_firstQuestionId];
        }

        public Question GetNextQuestion()
        {
            return _currentQuestionId != null && _questions.ContainsKey(_currentQuestionId) ? _questions[_currentQuestionId] : null;
        }

        public Answer GetCurrentAnswer()
        {
            return _answers.TryGetValue(_currentQuestionId, out var currentAnswer) ? currentAnswer : new Answer { QuestionId = _currentQuestionId };
        }

        public Dictionary<string, Answer> GetAnswers()
        {
            return _answers;
        }

        public Question GetQuestion(string questionId)
        {
            return _questions[questionId];
        }

        public void MoveToNextQuestion(Answer answer)
        {
            if (!_questions.ContainsKey(answer.QuestionId)) return;

            // Save the user's answer
            _answers[answer.QuestionId] = answer;

            _currentQuestionId = answer.QuestionId;
            var currentQuestion = _questions[_currentQuestionId];
            _questionHistory.Push(_currentQuestionId);

            string nextQuestionId = null;
            if (currentQuestion.AnswerType == 2 && currentQuestion.QuestionsOptions != null)
            {
                var option = currentQuestion.QuestionsOptions.FirstOrDefault(x => x.OptionText.Equals(answer.Response, StringComparison.OrdinalIgnoreCase));
                nextQuestionId = option != null && option.NextQuestionId != null ? option.NextQuestionId : currentQuestion.NextQuestionId;
            }
            else
            {
                nextQuestionId = currentQuestion.NextQuestionId;
            }

            _currentQuestionId = nextQuestionId;
        }

        public void MoveToPreviousQuestion(Answer answer)
        {
            // Delete the answer for the current question the user is moving back from.
            // This makes it so that we don't save any answers from potential question
            // chains that were broken from the user navigating back
            _answers.Remove(answer.QuestionId);

            if (_questionHistory.Count > 0)
            {
                _currentQuestionId = _questionHistory.Peek() != null ? _questionHistory.Pop() : null;
            }
        }

    }

}
