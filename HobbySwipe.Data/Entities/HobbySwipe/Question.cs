using System;
using System.Collections.Generic;

namespace HobbySwipe.Data.Entities.HobbySwipe;

public partial class Question
{
    public string Id { get; set; }

    public string QuestionText { get; set; }

    public int AnswerType { get; set; }

    public string NextQuestionId { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<QuestionsOption> QuestionsOptions { get; set; } = new List<QuestionsOption>();
}
