using Newtonsoft.Json;

namespace HobbySwipe.Models
{
    public class ResultsRoot
    {
        [JsonProperty("Results")]
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("Category")]
        public string Category { get; set; }
        [JsonProperty("Hobby")]
        public string Hobby { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Attributes")]
        public string[] Attributes { get; set; }
    }
}
