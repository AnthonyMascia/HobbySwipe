using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class LoginTwoFactorViewModel
    {
        public string EmailAddress { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        [Required, Display(Name = "One-time Code")]
        public string OneTimeCode { get; set; }

        public LoginTwoFactorViewModel()
        {

        }

        public LoginTwoFactorViewModel(string emailAddress, bool rememberMe, string returnUrl)
        {
            EmailAddress = emailAddress;
            RememberMe = rememberMe;
            ReturnUrl = returnUrl;
        }
    }
}
