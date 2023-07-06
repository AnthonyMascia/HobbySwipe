using HobbySwipe.Models;
using HobbySwipe.ViewModels.Discover;
using Microsoft.AspNetCore.Mvc;

namespace HobbySwipe.Controllers
{
    [Route("[controller]")]
    public class DiscoverController : Controller
    {
        private static QuestionManager _questionManager;

        public DiscoverController()
        {
            if (_questionManager == null)
            {
                // Construct your questions
                var question1 = new Question
                {
                    ID = Guid.NewGuid(),
                    QuestionText = "What is your age?",
                    AnswerType = AnswerType.OpenEnded,
                    NextQuestionId = Guid.NewGuid() // This will be the ID of question2
                };

                var question2 = new Question
                {
                    ID = question1.NextQuestionId.Value,
                    QuestionText = "Do you prefer social or solitary activities?",
                    AnswerType = AnswerType.MultipleChoice,
                    Options = new List<string> { "Social", "Solitary" },
                    ChoiceDependentChildQuestionId = new Dictionary<string, Guid?>
                    {
                        { "Solitary", Guid.NewGuid() } // This will be the ID of question2a
                    },
                    NextQuestionId = Guid.NewGuid() // This will be the ID of question3
                };


                var question3 = new Question
                {
                    ID = question2.NextQuestionId.Value,
                    QuestionText = "Where do you live?",
                    AnswerType = AnswerType.OpenEnded
                    // No NextQuestionId since this is the last question in the example
                };

                var question2a = new Question
                {
                    ID = question2.ChoiceDependentChildQuestionId["Solitary"].Value,
                    QuestionText = "What solitary activities do you enjoy?",
                    AnswerType = AnswerType.OpenEnded,
                    NextQuestionId = question3.ID // After question2a, we proceed to question3
                };

                // Construct your question list
                _questionManager = new QuestionManager(new List<Question> { question1, question2, question2a, question3 });
            }
        }

        [HttpGet]
        public IActionResult Discover()
        {
            var firstQuestion = _questionManager.GetFirstQuestion();

            return View(new QuestionAnswerViewModel
            {
                Question = firstQuestion,
                Answer = new Answer { QuestionID = firstQuestion.ID }
            });
        }

        [HttpPost]
        public IActionResult Answer(QuestionAnswerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _questionManager.MoveToNextQuestion(model.Answer);

            var nextQuestion = _questionManager.GetNextQuestion();

            if (nextQuestion != null)
            {
                return View("Discover", new QuestionAnswerViewModel
                {
                    Question = nextQuestion,
                    Answer = new Answer { QuestionID = nextQuestion.ID }
                });
            }
            else
            {
                return View("Complete");
            }
        }
    }
}
