using HobbySwipe.Data.Entities.HobbySwipe;

namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class QuestionsOptionModel
    {
        public int Id { get; set; }

        public string QuestionId { get; set; }

        public string OptionText { get; set; }

        public string NextQuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}
