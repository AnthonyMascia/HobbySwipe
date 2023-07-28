using HobbySwipe.Data.Entities.HobbySwipe;
using Microsoft.EntityFrameworkCore;

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
