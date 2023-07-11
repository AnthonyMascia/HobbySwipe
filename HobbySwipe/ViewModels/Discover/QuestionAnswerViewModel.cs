﻿using HobbySwipe.Data.Entities;
using HobbySwipe.Models;

namespace HobbySwipe.ViewModels.Discover
{
    public class QuestionAnswerViewModel
    {
        public Question Question { get; set; }
        public Answer Answer { get; set; }
        public bool IsFirstQuestion { get; set; }
    }
}
