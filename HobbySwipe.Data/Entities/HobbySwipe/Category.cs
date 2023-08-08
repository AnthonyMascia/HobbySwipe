namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class Category
{
    public string Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<CategoriesHobby> CategoriesHobbies { get; set; } = new List<CategoriesHobby>();
}
