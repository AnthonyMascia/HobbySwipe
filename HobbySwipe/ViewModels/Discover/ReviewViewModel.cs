using HobbySwipe.Data.Entities.HobbySwipe;
using HobbySwipe.Data.Models.HobbySwipe;

namespace HobbySwipe.ViewModels.Discover
{
    public class ReviewViewModel
    {
        public List<CategoriesHobbyModel> HobbiesLiked { get; set; }
        public List<CategoriesHobbyModel> HobbiesDisliked { get; set; }
        public List<CategoriesHobbyModel> HobbiesFavorited { get; set; }

        public ReviewViewModel(List<CategoriesHobbyModel> hobbiesLiked, List<CategoriesHobbyModel> hobbiesDisliked, List<CategoriesHobbyModel> hobbiesFavorited)
        {
            HobbiesLiked = hobbiesLiked;
            HobbiesDisliked = hobbiesDisliked;
            HobbiesFavorited = hobbiesFavorited;
        }
    }
}
