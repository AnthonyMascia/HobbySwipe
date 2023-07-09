using System;
using System.Collections.Generic;

namespace HobbySwipe.Data.Entities;

public partial class Answer
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string QuestionId { get; set; } = null!;

    public string Response { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
