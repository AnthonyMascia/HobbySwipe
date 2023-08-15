using AutoMapper;
using FuzzySharp;
using HobbySwipe.Data.Entities.HobbySwipe;
using HobbySwipe.Data.Models.HobbySwipe;
using HobbySwipe.Data.Repositories;
using HobbySwipe.Extensions;
using HobbySwipe.Models;
using HobbySwipe.ViewModels.Discover;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;

namespace HobbySwipe.Controllers
{
    public class DiscoverController : Controller
    {
        private static QuestionManager _questionManager;
        private readonly IHobbySwipeRepository _repos;
        private readonly IMapper _mapper;
        private readonly string _apiKey;
        private readonly string[] _prompt;
        private readonly string[] _newHobbyPrompt;
        private readonly ILogger<DiscoverController> _logger;

        public DiscoverController(IHobbySwipeRepository repos, IMapper mapper, ILogger<DiscoverController> logger)
        {
            _repos = repos;
            _mapper = mapper;
            _logger = logger;
            _apiKey = System.IO.File.ReadAllLines(@"C:\temp\HobbySwipe\api-key.txt").First();
            _prompt = System.IO.File.ReadAllLines(@"C:\temp\HobbySwipe\prompt.txt");
            _newHobbyPrompt = System.IO.File.ReadAllLines(@"C:\temp\HobbySwipe\prompt_new-hobby.txt");
            _logger.LogInformation("DiscoverController initialized");
        }



        /******************************************************************************************************************/
        /*** DISCOVER                                                                                                   ***/
        /******************************************************************************************************************/

        [HttpGet("Discover")]
        public async Task<IActionResult> Discover()
        {
            _logger.LogInformation("Discover action called");
            HttpContext.Session.Remove("ProcessedHobbies");

            if (_questionManager == null)
            {
                // Initialize QuestionManager if it's not already done
                _logger.LogInformation("Initializing QuestionManager");
                await InitializeQuestionManager();
            }

            var firstQuestion = _questionManager.GetFirstQuestion();
            _logger.LogInformation("First question retrieved from QuestionManager");

            return View(new QuestionAnswerViewModel
            {
                Question = firstQuestion,
                Answer = new AnswerModel
                {
                    QuestionId = firstQuestion.Id,
                    UserId = User.GetUserId()
                },
                IsFirstQuestion = true
            });
        }

        [HttpGet]
        public IActionResult Results()
        {
            _logger.LogInformation("Results action called");
            HttpContext.Session.Remove("ProcessedHobbies");

            if (TempData["Results"] == null)
            {
                return RedirectToAction("Discover");
            }

            var results = JsonConvert.DeserializeObject<ResultsRoot>((string)TempData["Results"]);
            TempData.Remove("Results");
            _logger.LogInformation("Results data created");

            return View(results);
        }


        [HttpGet("[controller]/Results/Review")]
        public async Task<IActionResult> Review()
        {
            _logger.LogInformation("Review action called");

            //var processedHobbies = HttpContext.Session.Get<Dictionary<string, int>>("ProcessedHobbies");

            var processedHobbies = new Dictionary<string, int>
            {
                { "singing", 1 },
                { "dance", 2 },
                { "running", 1 },
                { "sculpture", 0 },
                { "homebrewing", 0 },
            };

            if (processedHobbies == null || !processedHobbies.Any())
            {
                return RedirectToAction("Discover");
            }

            HttpContext.Session.Remove("ProcessedHobbies");
            var allHobbies = _mapper.Map<IEnumerable<CategoriesHobbyModel>>(await _repos.GetHobbiesWithCategoryAsync());
            var matchedHobbies = allHobbies.Where(x => processedHobbies.Keys.Contains(x.Id)).ToList();
            var likedHobbies = matchedHobbies.Where(x => processedHobbies[x.Id] == 1).ToList();
            var dislikedHobbies = matchedHobbies.Where(x => processedHobbies[x.Id] == 0).ToList();
            var favoritedHobbies = matchedHobbies.Where(x => processedHobbies[x.Id] == 2).ToList();

            return View(new ReviewViewModel(likedHobbies, dislikedHobbies,favoritedHobbies));
        }



        /******************************************************************************************************************/
        /*** AJAX METHODS                                                                                               ***/
        /******************************************************************************************************************/

        [HttpPost]
        public IActionResult Answer(AnswerModel model)
        {
            _logger.LogInformation("Answer action called");

            // TODO: handle the answer model
            //if (!ModelState.IsValid)
            //{
            //    // If the model is not valid, return the current view with the model to display errors
            //    return View(model);
            //}

            // Move to the next question based on the answer
            _questionManager.MoveToNextQuestion(model);
            _logger.LogInformation("Moved to the next question");

            var nextQuestion = _questionManager.GetNextQuestion();

            if (nextQuestion != null)
            {
                return PartialView("_Question.Partial", new QuestionAnswerViewModel
                {
                    Question = nextQuestion,
                    Answer = new AnswerModel
                    {
                        QuestionId = nextQuestion.Id,
                        UserId = User.GetUserId()
                    }
                });
            }
            else
            {
                _logger.LogInformation("No next question, transitioning to completion");
                return PartialView("_Transition.Partial");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAnswers()
        {
            _logger.LogInformation("ProcessAnswers action called");
            var answers = _questionManager.GetAnswers();
            var hobbies = _mapper.Map<IEnumerable<CategoriesHobbyModel>>(await _repos.GetHobbiesAsync());

            // If the user is not authenticated, return 5 randomly selected hobbies
            if (!User.Identity.IsAuthenticated)
            {
                var randomHobbies = hobbies.OrderBy(h => Guid.NewGuid()).Take(5).ToList();
                var resultsObj = new ResultsRoot
                {
                    Results = new List<Models.Result>()
                };

                foreach (var hobby in randomHobbies)
                {
                    resultsObj.Results.Add(new Models.Result
                    {
                        HobbyId = hobby.Id,
                        Hobby = hobby.Name,
                        Description = hobby.Description,
                        Category = hobby.CategoryId
                    });
                }

                // Save the results to TempData to be passed to the Results view
                TempData["Results"] = JsonConvert.SerializeObject(resultsObj);

                // Simulate API processing
                Thread.Sleep(5000);

                // Redirect to the Results action
                _logger.LogInformation("Redirecting to the Results action");
                return Json(new { url = Url.Action(nameof(Results), "Discover") });
            }
            // Else, call the API and get tailored hobbies specific to the user
            else
            {
                var categories = _mapper.Map<IEnumerable<CategoryModel>>(await _repos.GetCategoriesAsync());
                var attributes = _mapper.Map<IEnumerable<AttributeModel>>(await _repos.GetAttributesAsync());
                var openAiService = InitializeOpenAIService();
                var messages = ConstructHobbyRecPrompt(categories, answers);
                var completionResult = await SendToOpenAI(messages, openAiService);

                if (completionResult.Successful)
                {
                    // Extract the AI-generated response and store it in an object to be parsed.
                    _logger.LogInformation("AI response received. Parsing the results");
                    var results = completionResult.Choices.First().Message.Content;
                    var resultsObj = JsonConvert.DeserializeObject<ResultsRoot>(results);

                    // Create a set of existing attribute IDs for efficient lookup
                    var existingAttributeIds = new HashSet<string>(attributes.Select(a => a.Id));

                    // Lists to batch database operations
                    var attributesToAdd = new List<Data.Entities.HobbySwipe.Attribute>();
                    var hobbiesToAdd = new List<CategoriesHobby>();

                    foreach (var item in resultsObj.Results)
                    {
                        // Check and add new attributes
                        foreach (var attr in item.Attributes)
                        {
                            var attributeId = GenerateAttributeId(attr);

                            if (!existingAttributeIds.Contains(attributeId))
                            {
                                var newAttribute = new AttributeModel
                                {
                                    Id = attributeId,
                                    Name = attr
                                };

                                attributesToAdd.Add(_mapper.Map<Data.Entities.HobbySwipe.Attribute>(newAttribute));
                            }
                        }

                        var existingHobby = GetClosestMatch(item.Hobby, hobbies.ToList());

                        if (existingHobby == null)
                        {
                            var newHobby = await CreateNewHobby(item, openAiService);
                            if (newHobby != null) { hobbiesToAdd.Add(_mapper.Map<CategoriesHobby>(newHobby)); }
                            item.HobbyId = newHobby.Id;
                        }
                        else
                        {
                            item.HobbyId = existingHobby.Id;
                        }
                    }

                    // Batch save Attributes to the database
                    if (attributesToAdd.Any())
                    {
                        _repos.AddRange(attributesToAdd);
                        await _repos.SaveAllAsync();
                    }

                    // Batch save Hobbies to the database
                    if (hobbiesToAdd.Any())
                    {
                        _repos.AddRange(hobbiesToAdd);
                        await _repos.SaveAllAsync();
                    }

                    // Save the results to TempData to be passed to the Results view
                    TempData["Results"] = JsonConvert.SerializeObject(resultsObj);

                    // Redirect to the Results action
                    _logger.LogInformation("Redirecting to the Results action");
                    return Json(new { url = Url.Action(nameof(Results), "Discover") });
                }

                // If the call to the OpenAI API is not successful, log an error and return null
                // TODO: Implement proper error handling
                _logger.LogError("Error in AI response");
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> ProcessResult(int actionId, string hobbyId)
        {
            _logger.LogInformation("ProcessResult action called");

            // Save the results to session storage for tracking purposes (both for logged in and anonymous users)
            try
            {
                var preferences = HttpContext.Session.Get<Dictionary<string, int>>("ProcessedHobbies") ?? new Dictionary<string, int>();
                preferences[hobbyId] = actionId;
                HttpContext.Session.Set("ProcessedHobbies", preferences);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Error saving to session: " + ex.Message });
            }

            // If the user is signed in, save to the database
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var preference = new UserHobbyPreferenceModel
                    {
                        UserId = User.GetUserId(),
                        HobbyId = hobbyId,
                        ActionId = actionId,
                        DateProcessed = DateTime.Now
                    };

                    _repos.Add(_mapper.Map<UserHobbyPreference>(preference));
                    await _repos.SaveAllAsync();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMessage = "Error saving to database: " + ex.Message });
                }
            }

            // For non-authenticated users, just return success since we've already saved their preference in session
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult GoBack(AnswerModel model)
        {
            _logger.LogInformation("GoBack action called");
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



        /******************************************************************************************************************/
        /*** CONTROLLER HELPER METHODS                                                                                  ***/
        /******************************************************************************************************************/

        private async Task InitializeQuestionManager()
        {
            _logger.LogInformation("Initializing QuestionManager in helper method");
            var questions = _mapper.Map<List<QuestionModel>>((await _repos.GetQuestionsAsync()).ToList());
            _questionManager = new QuestionManager(questions);
            _logger.LogInformation("QuestionManager initialized in helper method");
        }

        private OpenAIService InitializeOpenAIService()
        {
            return new OpenAIService(new OpenAiOptions() { ApiKey = _apiKey });
        }

        private async Task<ChatCompletionCreateResponse> SendToOpenAI(List<ChatMessage> messages, OpenAIService openAiService)
        {
            return await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
            });
        }

        private List<ChatMessage> ConstructHobbyRecPrompt(IEnumerable<CategoryModel> categories, Dictionary<string, AnswerModel> answers)
        {
            var categoriesStr = string.Join(", ", categories.Select(c => c.Id));

            var messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(_prompt[0]),
                ChatMessage.FromUser(_prompt[1])
            };

            _logger.LogInformation("Processing answers");
            for (var i = 0; i < answers.Count(); i++)
            {
                var answer = answers.ElementAt(i);
                var question = _questionManager.GetQuestion(answer.Value.QuestionId);
                var chatText = $"Question {i + 1}: {question.QuestionText} Answer {i + 1}: {answer.Value.Response}";

                messages.Add(ChatMessage.FromUser(chatText));
            }

            messages.Add(ChatMessage.FromUser(_prompt[2]));
            messages.Add(ChatMessage.FromUser(categoriesStr));
            messages.Add(ChatMessage.FromUser(_prompt[3]));

            return messages;
        }

        private string GenerateAttributeId(string attribute)
        {
            return attribute.ToLower().Replace(" ", "-");
        }

        private async Task<CategoriesHobbyModel> CreateNewHobby(Models.Result item, OpenAIService openAiService)
        {
            var newHobbyChatMessages = new List<ChatMessage>
            {
                ChatMessage.FromUser(_newHobbyPrompt[0]),
                ChatMessage.FromUser(item.Hobby)
            };

            // Send the conversation to the OpenAI API and get the AI-generated response
            _logger.LogInformation("Sending conversation to OpenAI API");
            var newHobbyCompletionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = newHobbyChatMessages,
                Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
            });

            // The API call was successful. Return the new hobby
            if (newHobbyCompletionResult.Successful)
            {
                _logger.LogInformation("AI response received. Parsing the new hobby");
                var newHobbyResults = newHobbyCompletionResult.Choices.First().Message.Content;
                var newHobbyResultsObj = JsonConvert.DeserializeObject<NewHobby>(newHobbyResults);
                var newAttributes = new List<CategoriesHobbiesAttributeModel>();

                foreach (var attr in item.Attributes)
                {
                    newAttributes.Add(new CategoriesHobbiesAttributeModel
                    {
                        HobbyId = newHobbyResultsObj.Id,
                        AttributeId = attr.ToLower().Replace(" ", "-")
                    });
                }

                return new CategoriesHobbyModel
                {
                    Id = newHobbyResultsObj.Id,
                    CategoryId = item.Category,
                    Name = item.Hobby,
                    Description = newHobbyResultsObj.Description,
                    IsActive = true,
                    AddedBy = User.GetUserId(),
                    AddedDate = DateTime.Now,
                    CategoriesHobbiesAttributes = newAttributes
                };
            }

            return null;
        }

        private CategoriesHobbyModel GetClosestMatch(string input, List<CategoriesHobbyModel> potentialMatches)
        {
            var scores = potentialMatches.Select(hobby => Fuzz.PartialRatio(input, hobby.Name)).ToList();
            int maxScore = scores.Max();

            // Check if the maximum score meets the threshold
            if (maxScore >= 80)
            {
                // If it does, return the first hobby that had the highest score
                return potentialMatches[scores.IndexOf(maxScore)];
            }

            // If not, return null or any appropriate value indicating no match found
            return null;
        }
    }
}
