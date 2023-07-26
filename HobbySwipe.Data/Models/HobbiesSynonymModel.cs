using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Models
{
    public class HobbiesSynonymModel
    {
        public int Id { get; set; }

        public string HobbyId { get; set; }

        public string Synonym { get; set; }

        public virtual Hobby Hobby { get; set; }
    }
}
