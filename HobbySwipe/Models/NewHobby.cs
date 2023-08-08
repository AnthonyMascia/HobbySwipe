using Newtonsoft.Json;

namespace HobbySwipe.Models
{
    public class NewHobby
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}
