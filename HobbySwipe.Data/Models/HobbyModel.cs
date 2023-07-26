using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Models
{
    public class HobbyModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<HobbiesSynonym> HobbiesSynonyms { get; set; }
    }
}
