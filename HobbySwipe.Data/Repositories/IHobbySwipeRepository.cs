using HobbySwipe.Data.Entities.HobbySwipe;
using Microsoft.EntityFrameworkCore;

namespace HobbySwipe.Data.Repositories
{
    public interface IHobbySwipeRepository
    {
        /******************************************************************************************************************/
        /*** QUESTIONS / ANSWERS                                                                                        ***/
        /******************************************************************************************************************/
        Task<IEnumerable<Question>> GetQuestionsAsync();



        /******************************************************************************************************************/
        /*** CATEGORIES / HOBBIES / ATTRIBUTES                                                                          ***/
        /******************************************************************************************************************/
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<Category>> GetCategoriesWithChildrenAsync();
        Task<IEnumerable<CategoriesHobby>> GetHobbiesAsync();
        Task<IEnumerable<CategoriesHobby>> GetHobbiesWithCategoryAsync();
        Task<IEnumerable<Entities.HobbySwipe.Attribute>> GetAttributesAsync();



        /******************************************************************************************************************/
        /*** USER RESULTS                                                                                               ***/
        /******************************************************************************************************************/




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



        /******************************************************************************************************************/
        /*** QUESTIONS / ANSWERS                                                                                        ***/
        /******************************************************************************************************************/

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            return await _ctx.Questions.Include(x => x.QuestionsOptions).ToListAsync();
        }



        /******************************************************************************************************************/
        /*** CATEGORIES / HOBBIES / ATTRIBUTES                                                                          ***/
        /******************************************************************************************************************/

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

        public async Task<IEnumerable<CategoriesHobby>> GetHobbiesWithCategoryAsync()
        {
            return await _ctx.CategoriesHobbies.Include(x => x.Category).ToListAsync();
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
