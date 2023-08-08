namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class CategoryModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CategoriesHobbyModel> CategoriesHobbies { get; set; } = new List<CategoriesHobbyModel>();
    }
}
