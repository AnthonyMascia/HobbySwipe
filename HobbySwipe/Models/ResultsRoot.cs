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
        [JsonProperty("Hobby")]
        public string Hobby { get; set; }
        [JsonProperty("Reasoning")]
        public string Reasoning { get; set; }
    }
}
