using HobbySwipe.Data.Entities;
using HobbySwipe.Data.Repositories;
using HobbySwipe.Models;
using HobbySwipe.ViewModels.Discover;
using Microsoft.AspNetCore.Mvc;

namespace HobbySwipe.Controllers
{
    public class DiscoverController : Controller
    {
        // The QuestionManager instance is used to manage the flow of questions
        private static QuestionManager _questionManager;

        // A repository that allows the controller to interact with the database
        private readonly IQuestionsRepository _repos;

        // Constructor for the controller, initializes the repository
        public DiscoverController(IQuestionsRepository repos)
        {
            _repos = repos;
        }

        // An asynchronous method to initialize the QuestionManager with data from the repository
        private async Task InitializeQuestionManager()
        {
            var questions = (await _repos.GetQuestionsAsync()).ToList();
            _questionManager = new QuestionManager(questions);
        }

        // The initial HTTP GET action, displays the first question
        [HttpGet("Discover")]
        public async Task<IActionResult> Discover()
        {
            if (_questionManager == null)
            {
                // Initialize QuestionManager if it's not already done
                await InitializeQuestionManager();
            }

            var firstQuestion = _questionManager.GetFirstQuestion();

            return View(new QuestionAnswerViewModel
            {
                Question = firstQuestion,
                Answer = new Answer
                {
                    QuestionId = firstQuestion.Id,
                    UserId = Guid.NewGuid() // todo: grab user's real ID
                },
                IsFirstQuestion = true
            });
        }

        // The HTTP POST action for answering a question
        [HttpPost]
        public IActionResult Answer(Answer model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // If the model is not valid, return the current view with the model to display errors
            //    return View(model);
            //}

            // Move to the next question based on the answer
            _questionManager.MoveToNextQuestion(model);

            var nextQuestion = _questionManager.GetNextQuestion();

            // If there is a next question, return a partial view with the new question
            // If not, return a "Complete" view
            if (nextQuestion != null)
            {
                return PartialView("_Question.Partial", new QuestionAnswerViewModel
                {
                    Question = nextQuestion,
                    Answer = new Answer 
                    { 
                        QuestionId = nextQuestion.Id,
                        UserId = Guid.NewGuid() // todo: grab user's real ID
                    }
                });
            }
            else
            {
                return View("Complete");
            }
        }

        // The HTTP POST action for going back to the previous question
        [HttpPost]
        public IActionResult GoBack(Answer model)
        {
            _questionManager.MoveToPreviousQuestion(model);

            var previousQuestion = _questionManager.CurrentQuestion();
            var previousAnswer = _questionManager.CurrentAnswer();

            // If there is a previous question, return a partial view with the previous question
            // If not, redirect to the initial action
            return PartialView("_Question.Partial", new QuestionAnswerViewModel
            {
                Question = previousQuestion,
                Answer = previousAnswer
            });
        }
    }


}
