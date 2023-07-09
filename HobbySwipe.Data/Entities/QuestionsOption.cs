using System;
using System.Collections.Generic;
using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Entities;

public partial class QuestionsOption
{
    public int Id { get; set; }

    public string QuestionId { get; set; } = null!;

    public string OptionText { get; set; } = null!;

    public string? NextQuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;
}
