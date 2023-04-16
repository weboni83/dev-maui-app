using Microsoft.Maui.Controls;
using System;

namespace AlertApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        string userName;
        string password;
        string errorText;
        bool hasError;
        bool isAuthInProcess;

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked, ValidateLogin);
            PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }


        public string UserName
        {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }

        public string Password
        {
            get => this.password;
            set => SetProperty(ref this.password, value);
        }

        public string ErrorText
        {
            get => errorText;
            set => SetProperty(ref errorText, value);
        }

        public bool HasError
        {
            get => hasError;
            set => SetProperty(ref hasError, value);
        }

        public bool IsAuthInProcess
        {
            get => isAuthInProcess;
            set
            {
                SetProperty(ref isAuthInProcess, value);
                OnPropertyChanged(nameof(AllowNewAuthRequests));
            }
        }

        public bool AllowNewAuthRequests
        {
            get { return !IsAuthInProcess; }
        }

        public Command LoginCommand { get; }
        public Command SignUpCommand { get; }


        async void OnLoginClicked()
        {
            IsAuthInProcess = true;
            string response = await DataStore.Authenticate(userName, password);
            IsAuthInProcess = false;
            if (!string.IsNullOrEmpty(response))
            {
                ErrorText = response;
                HasError = true;
                return;
            }
            HasError = false;
            await Navigation.NavigateToAsync<AboutViewModel>(true);
        }

        bool ValidateLogin()
        {
            return !String.IsNullOrWhiteSpace(UserName)
                && !String.IsNullOrWhiteSpace(Password);
        }
    }
}