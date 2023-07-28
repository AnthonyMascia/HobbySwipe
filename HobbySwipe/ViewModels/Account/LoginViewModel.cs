using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme> ExternalLogins { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public LoginViewModel()
        {

        }

        public LoginViewModel(string returnUrl, List<AuthenticationScheme> externalLogins)
        {
            try
            {
                ReturnUrl = returnUrl;
                ExternalLogins = externalLogins;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
