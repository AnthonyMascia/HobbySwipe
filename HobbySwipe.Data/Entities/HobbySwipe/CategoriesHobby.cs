namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class CategoriesHobby
{
    public string Id { get; set; }

    public string CategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool? IsActive { get; set; }

    public string AddedBy { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public virtual ICollection<CategoriesHobbiesAttribute> CategoriesHobbiesAttributes { get; set; } = new List<CategoriesHobbiesAttribute>();

    public virtual ICollection<CategoriesHobbiesSynonym> CategoriesHobbiesSynonyms { get; set; } = new List<CategoriesHobbiesSynonym>();

    public virtual Category Category { get; set; }

    public virtual ICollection<UserHobbyPreference> UserHobbyPreferences { get; set; } = new List<UserHobbyPreference>();

    public virtual ICollection<UserHobbyPreferencesHistory> UserHobbyPreferencesHistories { get; set; } = new List<UserHobbyPreferencesHistory>();
}
