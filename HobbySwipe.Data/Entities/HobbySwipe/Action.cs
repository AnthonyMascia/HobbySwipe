namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class Action
{
    public int ActionId { get; set; }

    public string ActionName { get; set; }

    public virtual ICollection<UserHobbyPreference> UserHobbyPreferences { get; set; } = new List<UserHobbyPreference>();

    public virtual ICollection<UserHobbyPreferencesHistory> UserHobbyPreferencesHistories { get; set; } = new List<UserHobbyPreferencesHistory>();
}
