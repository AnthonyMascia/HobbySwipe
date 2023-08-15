using HobbySwipe.Data.Entities.HobbySwipe;

namespace HobbySwipe.Data.Models.HobbySwipe
{
    public class AnswerModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string QuestionId { get; set; }

        public string Response { get; set; }

        public virtual Question Question { get; set; }
    }
}
