using HobbySwipe.Data.Entities.HobbySwipe;

namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class QuestionModel
    {
        public string Id { get; set; }

        public string QuestionText { get; set; }

        public int AnswerType { get; set; }

        public string NextQuestionId { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public virtual ICollection<QuestionsOption> QuestionsOptions { get; set; }
    }
}
