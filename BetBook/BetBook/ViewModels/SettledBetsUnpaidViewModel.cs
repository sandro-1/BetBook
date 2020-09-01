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
    class SettledBetsUnpaidViewModel : ViewModelEventHandler
    {
        ObservableCollection<SettledBetsUnpaidViewModel> settledBetsUnpaid;
        public ObservableCollection<SettledBetsUnpaidViewModel> SettledBetsUnpaid
        {
            get => settledBetsUnpaid;
            set
            {
                settledBetsUnpaid = value;
                OnPropertyChanged();
            }
        }

        string betBackgroundColor;
        public string BetBackgroundColor
        {
            get => betBackgroundColor;
            set
            {
                betBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        bool requestModeOpposite;
        public bool RequestModeOpposite
        {
            get => requestModeOpposite;
            set
            {
                requestModeOpposite = value;
                OnPropertyChanged();
            }
        }

        string postSettlementRequestText;
        public string PostSettlementRequestText
        {
            get => postSettlementRequestText;
            set
            {
                postSettlementRequestText = value;
                OnPropertyChanged();
            }
        }

        bool betPaidEnabled;
        public bool BetPaidEnabled
        {
            get => betPaidEnabled;
            set
            {
                betPaidEnabled = value;
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

        public UserData User { get; set; }

        public SettledBetsUnpaidViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());
        }

        public ICommand RefreshCommand { get; }
        void ExecuteRefreshCommand()
        {
            SettledBetsUnpaid = new ObservableCollection<SettledBetsUnpaidViewModel>();
            for (int i = 0; i < User.BetList.Count; i++)
            {
                if (User.BetList.ElementAt(i).BetPhase == "SettledUnpaid")
                {
                    SettledBetsUnpaidViewModel settledBet = JsonConvert.DeserializeObject<SettledBetsUnpaidViewModel>(JsonConvert.SerializeObject(User.BetList.ElementAt(i)));
                    settledBet.BetBackgroundColor = settledBet.BetWon == true ? "LightGreen" : "PeachPuff";
                    settledBet.BetBackgroundColor = settledBet.RequestMode == true ? "PaleTurquoise" : settledBet.BetBackgroundColor;
                    settledBet.RequestModeOpposite = !settledBet.RequestMode;
                    settledBet.PostSettlementRequestText = User.BetList.ElementAt(i).InitiatedRequest == true ? "Verifying with opponent" : "Opponent's requested settlement";
                    settledBet.BetPaidEnabled = User.BetList.ElementAt(i).InitiatedRequest == true ? false : true;
                    settledBet.CashOrNotText = settledBet.NonCashBet ?? settledBet.CashBetAmount;
                    SettledBetsUnpaid.Add(settledBet);
                }
            }
        }

        public async Task ExecuteRequestSettlementCommand(string betId)
        {
            SettledBetsUnpaidViewModel unpaidBetTermSheet = SettledBetsUnpaid.FirstOrDefault(offers => offers.BetId == betId);
            UserData opponent = await CosmoDBService.GetUser(unpaidBetTermSheet.OpponentsUsername);

            if (unpaidBetTermSheet.BetWon)
            {
                unpaidBetTermSheet.BetPaid = await Application.Current.MainPage.DisplayAlert("Did your opponent pay you?", null, "Yes", "No");
            }
            else
            {
                unpaidBetTermSheet.BetPaid = await Application.Current.MainPage.DisplayAlert("Did you pay your opponent?", null, "Yes", "No");
            }

            if (!unpaidBetTermSheet.BetPaid)
            {
                return;
            }

            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {
                    User.BetList.ElementAt(i).RequestMode = true;
                    User.BetList.ElementAt(i).InitiatedRequest = true;
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.ElementAt(i).RequestMode = true;
                    opponent.BetList.ElementAt(i).InitiatedRequest = false;
                }
            }

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();
        }

        public async Task ExecuteBetPaidCommand(string betId)
        {
            SettledBetsUnpaidViewModel unpaidBetTermSheet = SettledBetsUnpaid.FirstOrDefault(offers => offers.BetId == betId);
            UserData opponent = await CosmoDBService.GetUser(unpaidBetTermSheet.OpponentsUsername);

            bool betPaid;

            if (unpaidBetTermSheet.BetWon)
            {
                betPaid = await Application.Current.MainPage.DisplayAlert("Did your opponent pay you?", null, "Yes", "No");
            }
            else
            {
                betPaid = await Application.Current.MainPage.DisplayAlert("Did you pay your opponent?", null, "Yes", "No");
            }

            if (!betPaid)
            {
                return;
            }

            if (unpaidBetTermSheet.BetWon)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashWonCollected += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }                

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.CashLostPaid += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashLostPaid += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.CashWonCollected += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }

            User.UserResults.CredibilityRatio = User.UserResults.CashLost == User.UserResults.CashLostPaid ? 100 : User.UserResults.CashLostPaid / User.UserResults.CashLost * 100;
            User.UserResults.CollectionRatio = User.UserResults.CashWon == User.UserResults.CashWonCollected ? 100 : User.UserResults.CashWonCollected / User.UserResults.CashWon * 100;

            opponent.UserResults.CredibilityRatio = opponent.UserResults.CashLost == opponent.UserResults.CashLostPaid ? 100 : opponent.UserResults.CashLostPaid / opponent.UserResults.CashLost * 100;
            opponent.UserResults.CollectionRatio = opponent.UserResults.CashWon == opponent.UserResults.CashWonCollected ? 100 : opponent.UserResults.CashWonCollected / opponent.UserResults.CashWon * 100;

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();
        }
    }
}
