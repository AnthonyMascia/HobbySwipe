using HobbySwipe.Data.Entities.HobbySwipe;
using Microsoft.EntityFrameworkCore;

namespace HobbySwipe.Data.Repositories
{
    public interface IHobbySwipeRepository
    {
        public Task<IEnumerable<Question>> GetQuestionsAsync();
        public Task<IEnumerable<Category>> GetCategoriesAsync();
        public Task<IEnumerable<Category>> GetCategoriesWithChildrenAsync();
        public Task<IEnumerable<CategoriesHobby>> GetHobbiesAsync();
        public Task<IEnumerable<Entities.HobbySwipe.Attribute>> GetAttributesAsync();


        /******************************************************************************************************************/
        /*** CRUD OPERATIONS                                                                                            ***/
        /******************************************************************************************************************/

        void Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteRange<T>(IEnumerable<T> entity) where T : class;
        Task<bool> SaveAllAsync();
    }

    public class HobbySwipeRepository : IHobbySwipeRepository
    {
        private HobbySwipeContext _ctx;

        public HobbySwipeRepository(HobbySwipeContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            return await _ctx.Questions.Include(x => x.QuestionsOptions).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _ctx.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithChildrenAsync()
        {
            return await _ctx.Categories.Include(x => x.CategoriesHobbies).ToListAsync();
        }

        public async Task<IEnumerable<CategoriesHobby>> GetHobbiesAsync()
        {
            return await _ctx.CategoriesHobbies.ToListAsync();
        }

        public async Task<IEnumerable<Entities.HobbySwipe.Attribute>> GetAttributesAsync()
        {
            return await _ctx.Attributes.ToListAsync();
        }



        /******************************************************************************************************************/
        /*** CRUD OPERATIONS                                                                                            ***/
        /******************************************************************************************************************/

        public void Add<T>(T entity) where T : class
        {
            _ctx.Add(entity);
        }

        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            _ctx.AddRange(entities);
        }

        public void Delete<T>(T entity) where T : class
        {
            _ctx.Remove(entity);
        }

        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            _ctx.RemoveRange(entities);
        }

        public async Task<bool> SaveAllAsync()
        {
            try
            {
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
