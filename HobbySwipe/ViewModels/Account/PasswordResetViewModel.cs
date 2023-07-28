using System.ComponentModel.DataAnnotations;

namespace HobbySwipe.ViewModels.Account
{
    public class PasswordResetViewModel
    {
        public string UserID { get; set; }
        public string Token { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Must be at least 8 characters and include a lowercase letter, an uppercase letter, a number, and a special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        public PasswordResetViewModel()
        {

        }

        public PasswordResetViewModel(string userId, string token)
        {
            UserID = userId;
            Token = token;
        }
    }
}
