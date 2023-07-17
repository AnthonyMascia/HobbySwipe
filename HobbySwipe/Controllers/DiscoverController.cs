using HobbySwipe.Data.Entities;
using HobbySwipe.Data.Repositories;
using HobbySwipe.Models;
using HobbySwipe.ViewModels.Discover;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI;

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

        [HttpGet("Results")]
        public IActionResult Results()
        {
            if (TempData["Results"] == null)
            {
                return RedirectToAction("Discover");
            }

            var results = JsonConvert.DeserializeObject<ResultsViewModel>((string)TempData["Results"]);  // Deserialize the JSON string back into the complex object
            return View(results);
        } 

        // The HTTP POST action for answering a question
        [HttpPost]
        public IActionResult Answer(Answer model)
        {
            // todo: handle the answer model
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
                return PartialView("_Transition.Partial");
            }
        }

        [HttpPost] 
        public async Task<IActionResult> ProcessAnswers()
        {
            var answers = _questionManager.GetAnswers();

            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = ""
            });

            var messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a helpful assistant specialized in recommending hobbies based on a person's preferences."),
                ChatMessage.FromUser("I am going to provide you with a list of questions and their corresponding answers. These questions  are for you to help me discover new hobbies. The corresponding answers are my answers to these questions. Based on these questions and answers, please recommend me hobbies.")
            };

            for (var i = 0; i < answers.Count(); i++)
            {
                var answer = answers.ElementAt(i);
                var question = _questionManager.GetQuestion(answer.Value.QuestionId);
                var chatText = $"Question {i + 1}: {question.QuestionText} Answer {i + 1}: {answer.Value.Response}";

                messages.Add(ChatMessage.FromUser(chatText));
            }

            messages.Add(ChatMessage.FromUser("Based on my preferences, give me the top 5 hobbies you think I would enjoy. Please provide the results in a format where each recommended hobby is separated by '|'. "));

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
            });

            if (completionResult.Successful)
            {
                var resultContent = completionResult.Choices.First().Message.Content;
                var results = resultContent.Split('|');

                TempData["Results"] = JsonConvert.SerializeObject(new ResultsViewModel(results));

                return Json(new { url = Url.Action(nameof(Results), "Discover") });
            }


            // todo: error handling. error in json response
            return null;
        }

        // The HTTP POST action for going back to the previous question
        [HttpPost]
        public IActionResult GoBack(Answer model)
        {
            _questionManager.MoveToPreviousQuestion(model);

            var previousQuestion = _questionManager.GetCurrentQuestion();
            var previousAnswer = _questionManager.GetCurrentAnswer();

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
