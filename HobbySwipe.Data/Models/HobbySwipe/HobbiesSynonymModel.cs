using HobbySwipe.Data.Entities.HobbySwipe;

namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class HobbiesSynonymModel
    {
        public int Id { get; set; }

        public string HobbyId { get; set; }

        public string Synonym { get; set; }

        public virtual Hobby Hobby { get; set; }
    }
}
