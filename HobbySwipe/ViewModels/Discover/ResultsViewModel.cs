using HobbySwipe.Data.Entities;

namespace HobbySwipe.ViewModels.Discover
{
    public class ResultsViewModel
    {
        public string[] Results  { get; set; }

        public ResultsViewModel()
        {
            
        }

        public ResultsViewModel(string[] Results)
        {
            this.Results = Results;
        }
    }
}
