namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class CategoriesHobbiesSynonym
{
    public int Id { get; set; }

    public string HobbyId { get; set; }

    public string SynonymId { get; set; }

    public virtual CategoriesHobby Hobby { get; set; }
}
