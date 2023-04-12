using AlertApp.ViewModels;
using System.Runtime.CompilerServices;

namespace AlertApp.Views
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
}