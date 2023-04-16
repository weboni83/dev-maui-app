using System.Windows.Input;

namespace AlertApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public const string ViewName = "AboutPage";

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://ironpot.netlify.app"));
        }

        public string Header { get; set; }
        public string Info { get; set; }

        public ICommand OpenWebCommand { get; }
    }
}