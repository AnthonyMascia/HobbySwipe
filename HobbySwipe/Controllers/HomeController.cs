using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HobbySwipe.Models;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels.RequestModels;

namespace HobbySwipe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var openAiService = new OpenAIService(new OpenAiOptions()
            //{
            //    ApiKey = "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"
            //});

            //var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            //{
            //    Messages = new List<ChatMessage>
            //    {
            //        ChatMessage.FromSystem("You are a helpful assistant."),
            //        ChatMessage.FromUser("Who won the world series in 2020?"),
            //        ChatMessage.FromAssistant("The Los Angeles Dodgers won the World Series in 2020."),
            //        ChatMessage.FromUser("Where was it p
            //        layed?")
            //    },
            //    Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo,
            //    MaxTokens = 50//optional
            //});
            //if (completionResult.Successful)
            //{
            //    Console.WriteLine(completionResult.Choices.First().Message.Content);
            //}

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}