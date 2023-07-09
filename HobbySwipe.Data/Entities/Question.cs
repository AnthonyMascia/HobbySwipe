using System;
using System.Collections.Generic;
using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Entities;

public partial class Question
{
    public string Id { get; set; } = null!;

    public string QuestionText { get; set; } = null!;

    public int AnswerType { get; set; }

    public string? NextQuestionId { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<QuestionsOption> QuestionsOptions { get; set; } = new List<QuestionsOption>();
}
