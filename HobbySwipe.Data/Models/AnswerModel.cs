using HobbySwipe.Data.Entities;

namespace HobbySwipe.Data.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string QuestionId { get; set; }

        public string Response { get; set; }

        public virtual Question Question { get; set; }
    }
}
