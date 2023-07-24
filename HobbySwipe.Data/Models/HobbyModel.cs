using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Models
{
    public class HobbyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public virtual ICollection<HobbiesSynonym> HobbiesSynonyms { get; set; }
    }
}
