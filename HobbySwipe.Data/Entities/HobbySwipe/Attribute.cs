namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class Attribute
{
    public string Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<CategoriesHobbiesAttribute> CategoriesHobbiesAttributes { get; set; } = new List<CategoriesHobbiesAttribute>();
}
