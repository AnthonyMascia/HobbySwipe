namespace HobbySwipe.ViewModels.Shared
{
    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }
        public string ReturnURL { get; set; }

        public ErrorViewModel(string errorMessage = null, string returnUrl = null)
        {
            ErrorMessage = errorMessage;
            ReturnURL = returnUrl ?? "/";
        }
    }
}
