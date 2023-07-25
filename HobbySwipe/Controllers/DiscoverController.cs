using HobbySwipe.Data.Repositories;
using HobbySwipe.Models;
using HobbySwipe.ViewModels.Discover;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI;
using AutoMapper;
using HobbySwipe.Data.Models;

namespace HobbySwipe.Controllers
{
    public class DiscoverController : Controller
    {
        // The QuestionManager instance is used to manage the flow of questions
        private static QuestionManager _questionManager;

        // A repository that allows the controller to interact with the database
        private readonly IQuestionsRepository _repos;
        private readonly IMapper _mapper;

        // Constructor for the controller, initializes the repository
        public DiscoverController(IQuestionsRepository repos, IMapper mapper)
        {
            _repos = repos;
            _mapper = mapper;
        }

        // An asynchronous method to initialize the QuestionManager with data from the repository
        private async Task InitializeQuestionManager()
        {
            var questions = _mapper.Map<List<QuestionModel>>((await _repos.GetQuestionsAsync()).ToList());
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
                Answer = new AnswerModel
                {
                    QuestionId = firstQuestion.Id,
                    UserId = Guid.NewGuid() // todo: grab user's real ID
                },
                IsFirstQuestion = true
            });
        }

        [HttpGet]
        public IActionResult Results()
        {
            //if (TempData["Results"] == null)
            //{
            //    return RedirectToAction("Discover");
            //}

            //var results = JsonConvert.DeserializeObject<Models.ResultsRoot>((string)TempData["Results"]);
            var results = new ResultsRoot
            {
                Results = new List<Result>
                {
                    new Result { Hobby = "Puzzle solving", Reasoning = "Based on your preference for thinking hard and enjoying indoor activities, puzzle solving can be a perfect fit for you. With your skills and talent for problem-solving, you can delve into various types of puzzles like crosswords, sudoku, riddles, and brain teasers. It's a mentally stimulating hobby that challenges your intellect, enhances your cognitive abilities, and provides a sense of accomplishment." },
                    new Result { Hobby = "Writing", Reasoning = "For a solo player like you who enjoys thinking critically, writing can be a fulfilling hobby. Whether it's journaling, creative writing, or even blogging, writing allows you to express your thoughts, unleash your creativity, and delve into self-reflection. You can explore various genres, improve your writing skills, and even consider publishing your work in the future, aligning with your interest in making money from your hobby." },
                    new Result { Hobby = "Learning a musical instrument", Reasoning = "If you're looking for a hobby that combines mental engagement with creativity and self-expression, learning a musical instrument can offer a great avenue. Whether it's the piano, guitar, or any instrument that resonates with you, playing music can be a beautiful journey. You can start with online tutorials, join virtual communities, and gradually progress to playing your favorite tunes, providing relaxation and a sense of accomplishment." },
                    new Result { Hobby = "Photography", Reasoning = "As someone who enjoys indoor activities but seeks to meet new people, photography can be an excellent choice. With your interest in capturing meaningful moments, you can immerse yourself in the world of photography. Explore different techniques, master composition, and seek out local photography groups or workshops to connect with like-minded individuals. Photography allows you to express your creativity, document your experiences, and capture the beauty around you." },
                    new Result { Hobby = "Coding/Programming", Reasoning = "Given your penchant for thinking hard and your interest in utilizing your skills and talents, coding or programming can be an exciting hobby to consider. Engaging with coding challenges, learning different programming languages, and developing your own projects can provide both a mental workout and a sense of accomplishment. It also opens up opportunities for you to create useful tools or applications that align with your values, be it sustainability or community impact." }
                }
            };

            return View(results);
        } 

        // The HTTP POST action for answering a question
        [HttpPost]
        public IActionResult Answer(AnswerModel model)
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
                    Answer = new AnswerModel 
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
               
            });

            var messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are Hobbix, an innovative and imaginative assistant with a deep understanding of various hobbies and interests. Your primary purpose is to provide personalized hobby recommendations tailored to each individual's preferences. Your recommendations will be both unique and creative, combining your knowledge of different hobbies with a keen understanding of the user's interests. Whether it's discovering new passions, honing existing skills, or exploring uncharted territories, you are here to guide and inspire users on their hobby journey. With your wealth of expertise and a touch of creativity, you'll help users uncover exciting hobbies they never knew they would love."),
                ChatMessage.FromUser("I am excited to embark on a journey of discovering new hobbies with your guidance, Hobbix. To start our quest, I will provide you with a series of questions along with my answers to them. These questions are designed to help you understand my interests and preferences better. Based on this information, I kindly request your expertise in recommending me some exciting hobbies that align with my passions. I trust your ability to think outside the box and offer unique suggestions that will ignite my curiosity and bring joy to my leisure time. Let's dive into this hobby-seeking adventure together!")
            };

            for (var i = 0; i < answers.Count(); i++)
            {
                var answer = answers.ElementAt(i);
                var question = _questionManager.GetQuestion(answer.Value.QuestionId);
                var chatText = $"Question {i + 1}: {question.QuestionText} Answer {i + 1}: {answer.Value.Response}";

                messages.Add(ChatMessage.FromUser(chatText));
            }

            messages.Add(ChatMessage.FromUser(
                "Based on my preferences, I kindly request your expert opinion, Hobbix, to provide me with the top 5 hobbies you believe I would thoroughly enjoy. Along with the results, I would greatly appreciate a descriptive yet concise blurb about each recommended hobby, highlighting why it might resonate with me. Feel free to reference any of my previous answers if they influenced your decision-making process. To ensure convenience and readability, please present the results in a JSON format where the list of hobbies will contain two fields: 'Hobby' and 'Reasoning.' Your insightful recommendations and personalized explanations will help me make an informed choice and embark on exciting new hobby adventures. I eagerly await your suggestions! Remember to please only suggest 5 hobbies in one singular JSON structure. The root of the structure should be called 'Results'."));

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
            });

            if (completionResult.Successful)
            {
                var results = completionResult.Choices.First().Message.Content;
                TempData["Results"] = results;

                return Json(new { url = Url.Action(nameof(Results), "Discover") });
            }


            // todo: error handling. error in json response
            return null;
        }

        // The HTTP POST action for going back to the previous question
        [HttpPost]
        public IActionResult GoBack(AnswerModel model)
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
