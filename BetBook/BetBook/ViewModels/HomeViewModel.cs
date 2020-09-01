using BetBook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    public class HomeViewModel : ViewModelEventHandler
    {
        public HomeViewModel()
        {
            LogoutCommand = new Command(async() => await ExecuteLogoutCommand());
        }
        public ICommand LogoutCommand { get; }
        async Task ExecuteLogoutCommand()
        {
            LoginViewModel.loggedUser = new UserData();
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
