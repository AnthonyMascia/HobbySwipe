using HobbySwipe.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HobbySwipe.Data.Repositories
{
    public interface IQuestionsRepository
    {
        public IEnumerable<Question> GetQuestions();
    }

    public class QuestionsRepository : IQuestionsRepository
    {
        private HobbySwipeContext _ctx;

        public QuestionsRepository(HobbySwipeContext ctx) 
        {
            _ctx = ctx;
        }

        public IEnumerable<Question> GetQuestions()
        {
            return _ctx.Questions.Include(x => x.QuestionsOptions);
        }
    }
}
