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
        public Task<IEnumerable<Question>> GetQuestionsAsync();
    }

    public class QuestionsRepository : IQuestionsRepository
    {
        private HobbySwipeContext _ctx;

        public QuestionsRepository(HobbySwipeContext ctx) 
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            return await _ctx.Questions.Include(x => x.QuestionsOptions).ToListAsync();
        }
    }
}
