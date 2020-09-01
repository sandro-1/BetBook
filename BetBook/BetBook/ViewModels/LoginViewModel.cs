using BetBook.Models;
using BetBook.Services;
using BetBook.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    class LoginViewModel : ViewModelEventHandler
    {
        public static UserData loggedUser;

        UserCredentials user;
        public UserCredentials User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel()
        {
            User = new UserCredentials();
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            CreateAccountCommand = new Command(async () => await ExecuteCreateAccountCommand());
        }

        public ICommand LoginCommand { get; }
        async Task ExecuteLoginCommand()
        {            
            var attemptedLogin = await CosmoDBService.Authenticate(User.Username, User.Password);
            
            if (attemptedLogin != null)
            {
                loggedUser = CosmoDBService.GetUser(attemptedLogin.Username).Result;
                HomePage home = new HomePage();
                await Application.Current.MainPage.Navigation.PushModalAsync(home);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Username or password incorrect", "OK");
            }
        }

        public ICommand CreateAccountCommand { get; }
        async Task ExecuteCreateAccountCommand()
        {
            CreateAccountPage createAccountPage = new CreateAccountPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(createAccountPage);
        }
    }
}
