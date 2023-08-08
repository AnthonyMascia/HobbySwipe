namespace HobbySwipe.Data.Models.HobbySwipe;

public partial class UserHobbyPreferenceModel
{
    public int PreferenceId { get; set; }

    public string UserId { get; set; }

    public string HobbyId { get; set; }

    public int? ActionId { get; set; }

    public DateTime DateProcessed { get; set; }

    public virtual Action Action { get; set; }

    public virtual ICollection<UserHobbyPreferencesHistoryModel> UserHobbyPreferencesHistories { get; set; } = new List<UserHobbyPreferencesHistoryModel>();
}
