﻿using HobbySwipe.Data.Models;

namespace HobbySwipe.ViewModels.Discover
{
    public class QuestionAnswerViewModel
    {
        public QuestionModel Question { get; set; }
        public AnswerModel Answer { get; set; }
        public bool IsFirstQuestion { get; set; }
    }
}
