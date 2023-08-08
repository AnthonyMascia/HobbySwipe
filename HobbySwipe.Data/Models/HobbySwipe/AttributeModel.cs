namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class AttributeModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CategoriesHobbiesAttributeModel> CategoriesHobbiesAttributes { get; set; } = new List<CategoriesHobbiesAttributeModel>();
    }
}
