namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class CategoriesHobbyModel
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public string AddedBy { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public virtual ICollection<CategoriesHobbiesAttributeModel> CategoriesHobbiesAttributes { get; set; } = new List<CategoriesHobbiesAttributeModel>();

        public virtual ICollection<CategoriesHobbiesSynonymModel> CategoriesHobbiesSynonyms { get; set; } = new List<CategoriesHobbiesSynonymModel>();
    }
}
