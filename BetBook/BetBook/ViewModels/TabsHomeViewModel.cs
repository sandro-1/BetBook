using BetBook.Models;
using BetBook.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    public class TabsHomeViewModel : ViewModelEventHandler
    {
        UserData user;
        public UserData User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        string betsWonLost;
        public string BetsWonLost
        {
            get => betsWonLost;
            set
            {
                betsWonLost = value;
                OnPropertyChanged();
            }
        }

        string cashWonLost;
        public string CashWonLost
        {
            get => cashWonLost;
            set
            {
                cashWonLost = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public TabsHomeViewModel()
        {
            User = new UserData();
            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());            
        }

        async Task ExecuteRefreshCommand()
        {
            User = await CosmoDBService.GetUser(LoginViewModel.loggedUser.Username);
        }
    }
}
