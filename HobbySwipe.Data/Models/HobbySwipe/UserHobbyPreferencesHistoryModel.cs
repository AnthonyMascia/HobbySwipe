namespace HobbySwipe.Data.Models.HobbySwipe;

public partial class UserHobbyPreferencesHistoryModel
{
    public int HistoryId { get; set; }

    public int? PreferenceId { get; set; }

    public string UserId { get; set; }

    public string HobbyId { get; set; }

    public int? ActionId { get; set; }

    public DateTime DateProcessed { get; set; }

    public DateTime DateMovedToHistory { get; set; }

    public virtual ActionModel Action { get; set; }
}
