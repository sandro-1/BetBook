using BetBook.Models;
using BetBook.Services;
using BetBook.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    public class LoginViewModel : ViewModelEventHandler
    {
        public static UserData loggedUser { get; set; }

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
            //RefreshCommand = new Command(async() => await ExecuteRefreshCommand());

            DateTime nearestMinute = RoundUp(DateTime.Now, TimeSpan.FromMinutes(1));
            TimeSpan nearestMinuteDifference = nearestMinute - DateTime.Now;
            int formattedDifference = (int)nearestMinuteDifference.TotalMilliseconds;

            TimerCallback tmCallback = ExecuteRefreshCommand;
            Timer timer;

            if (nearestMinuteDifference < new TimeSpan(0, 0, 0, 10))
            {
                formattedDifference = formattedDifference + 50000;                
                timer = new Timer(tmCallback, null, formattedDifference, 60000);

            }
            else
            {
                timer = new Timer(tmCallback, null, formattedDifference, 60000);
            }
        }

        public ICommand RefreshCommand { get; }
        public async void ExecuteRefreshCommand(object objectInfo)
        {
            if (loggedUser != null)
            {
                loggedUser = await CosmoDBService.GetUser(loggedUser.Username);
                MessagingCenter.Send<object, bool>(Application.Current, "Refresh", true);

            }
        }

        public ICommand LoginCommand { get; }
        async Task ExecuteLoginCommand()
        {            
            var attemptedLogin = await CosmoDBService.Authenticate(User.Username, User.Password);
            
            if (attemptedLogin != null)
            {
                loggedUser = await CosmoDBService.GetUser(attemptedLogin.Username);
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
