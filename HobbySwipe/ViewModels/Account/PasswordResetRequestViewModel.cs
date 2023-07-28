using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class PasswordResetRequestViewModel
    {
        public string ReturnUrl { get; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public PasswordResetRequestViewModel()
        {

        }

        public PasswordResetRequestViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }
    }
}
