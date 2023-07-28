namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class Answer
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string QuestionId { get; set; }

    public string Response { get; set; }

    public virtual Question Question { get; set; }
}
