using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string LoginProvider { get; set; }
    }
}
