using BetBook.Models;
using BetBook.Services;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    class CreateAccountViewModel : ViewModelEventHandler
    {
        UserCredentials credentials;
        public UserCredentials Credentials
        {
            get => credentials;
            set
            {
                credentials = value;
                OnPropertyChanged();
            }
        }

        public CreateAccountViewModel()
        {
            Credentials = new UserCredentials();
            CreateAccountCommand = new Command(async () => await ExecuteCreateAccountCommand());
        }
        public ICommand CreateAccountCommand { get; }
        async Task ExecuteCreateAccountCommand()
        {
            var checkUsername = await CosmoDBService.GetUser(Credentials.Username);

            if (checkUsername == null)
            {
                var id = Guid.NewGuid().ToString();
                UserData data = new UserData
                {
                    UserResults = new UserResults(),                    
                    BetList = new List<TermSheet>(),
                    Id = id,
                    Username = Credentials.Username
                };
                data.UserResults.CollectionRatio = 100;
                data.UserResults.CredibilityRatio = 100;
                credentials.Id = id;

                await CosmoDBService.InsertUser(credentials, data);
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Username taken, please choose another", null, "OK");
                Credentials.Username = "";
                Credentials.Password = "";
            }                        
        }
    }
}
