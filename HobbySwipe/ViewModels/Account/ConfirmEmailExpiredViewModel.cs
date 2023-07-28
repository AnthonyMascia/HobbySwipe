using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class ConfirmEmailExpiredViewModel
    {
        [Required]
        public string UserID { get; set; }
        public string ReturnUrl { get; set; }

        public ConfirmEmailExpiredViewModel()
        {

        }

        public ConfirmEmailExpiredViewModel(string userId, string returnUrl)
        {
            UserID = userId;
            ReturnUrl = returnUrl;
        }
    }
}
