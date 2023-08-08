namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class UserHobbyPreference
{
    public int PreferenceId { get; set; }

    public string UserId { get; set; }

    public string HobbyId { get; set; }

    public int? ActionId { get; set; }

    public DateTime DateProcessed { get; set; }

    public virtual Action Action { get; set; }

    public virtual CategoriesHobby Hobby { get; set; }

    public virtual ICollection<UserHobbyPreferencesHistory> UserHobbyPreferencesHistories { get; set; } = new List<UserHobbyPreferencesHistory>();
}
