using Microsoft.AspNetCore.Mvc.Rendering;

namespace HobbySwipe.Models
{
    public class Question
    {
        public Guid ID { get; set; }
        public string QuestionText { get; set; }
        public AnswerType AnswerType { get; set; }
        public List<string> Options { get; set; } // For multiple choice questions
        public Dictionary<string, Guid?> ChoiceDependentChildQuestionId { get; set; } // Mapping of a multiple-choice answer to a child question ID
        public Guid? OpenEndedChildQuestionId { get; set; } // ID of a child question that's asked regardless of the open-ended answer
        public Guid? NextQuestionId { get; set; } // ID of the next root question
        public SelectList GetOptionsSelectList
        {
            get
            {

                if (AnswerType == AnswerType.MultipleChoice)
                {
                    return new SelectList(Options);
                }

                return null;
            }
        }
    }

    public enum AnswerType
    {
        OpenEnded,
        MultipleChoice
    }
}
