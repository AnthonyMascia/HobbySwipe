namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class CategoriesHobbiesAttribute
{
    public int Id { get; set; }

    public string HobbyId { get; set; }

    public string AttributeId { get; set; }

    public virtual Attribute Attribute { get; set; }

    public virtual CategoriesHobby Hobby { get; set; }
}
