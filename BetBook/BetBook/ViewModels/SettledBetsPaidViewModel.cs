using BetBook.Models;
using BetBook.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    class SettledBetsPaidViewModel : ViewModelEventHandler
    {
        ObservableCollection<SettledBetsPaidViewModel> settledBetsPaid;
        public ObservableCollection<SettledBetsPaidViewModel> SettledBetsPaid
        {
            get => settledBetsPaid;
            set
            {
                settledBetsPaid = value;
                OnPropertyChanged();
            }
        }

        string cashOrNotText;
        public string CashOrNotText
        {
            get => cashOrNotText;
            set
            {
                cashOrNotText = value;
                OnPropertyChanged();
            }
        }

        string resultBackgroundColor;
        public string ResultBackgroundColor
        {
            get => resultBackgroundColor;
            set
            {
                resultBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        string result;
        public string Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }
        public UserData User { get; set; }

        public SettledBetsPaidViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());
        }

        public ICommand RefreshCommand { get; }
        void ExecuteRefreshCommand()
        {
            SettledBetsPaid = new ObservableCollection<SettledBetsPaidViewModel>();
            for (int i = 0; i < User.BetList.Count; i++)
            {
                if (User.BetList.ElementAt(i).BetPhase == "SettledPaid")
                {
                    SettledBetsPaidViewModel settledBet = JsonConvert.DeserializeObject<SettledBetsPaidViewModel>(JsonConvert.SerializeObject(User.BetList.ElementAt(i)));
                    settledBet.Result = settledBet.BetWon == true ? "Won" : "Lost";
                    settledBet.resultBackgroundColor = settledBet.BetWon == true ? "#FF4081" : "LightSlateGray";
                    settledBet.CashOrNotText = settledBet.NonCashBet ?? settledBet.CashBetAmount;
                    SettledBetsPaid.Add(settledBet);
                }
            }
        }
    }
}
