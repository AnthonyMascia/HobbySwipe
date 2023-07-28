using System;
using System.Collections.Generic;

namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class QuestionsOption
{
    public int Id { get; set; }

    public string QuestionId { get; set; }

    public string OptionText { get; set; }

    public string NextQuestionId { get; set; }

    public virtual Question Question { get; set; }
}
