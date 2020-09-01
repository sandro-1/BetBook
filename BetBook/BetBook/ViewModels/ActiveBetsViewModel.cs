using BetBook.Models;
using BetBook.Services;
using BetBook.Views;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

namespace BetBook.ViewModels
{
    class ActiveBetsViewModel : ViewModelEventHandler
    {
        ObservableCollection<ActiveBetsViewModel> activeBets;
        public ObservableCollection<ActiveBetsViewModel> ActiveBets
        {
            get => activeBets;
            set
            {
                activeBets = value;
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

        bool requestResponseVisible;
        public bool RequestResponseVisible
        {
            get => requestResponseVisible;
            set
            {
                requestResponseVisible = value;
                OnPropertyChanged();
            }
        }

        public UserData User { get; set; }

        public ActiveBetsViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());
        }

        public ICommand RefreshCommand { get; }
        void ExecuteRefreshCommand()
        {
            ActiveBets = new ObservableCollection<ActiveBetsViewModel>();
            User = LoginViewModel.loggedUser;
            
            for (int i = 0; i < User.BetList.Count; i++)
            {
                if (User.BetList.ElementAt(i).BetPhase == "ActiveBet")
                {
                    ActiveBetsViewModel settledBet = JsonConvert.DeserializeObject<ActiveBetsViewModel>(JsonConvert.SerializeObject(User.BetList.ElementAt(i)));

                    if (User.BetList.ElementAt(i).DateTimeBetClose.Contains(" (Now)"))
                    {
                        settledBet.BetBackgroundColor = "MediumPurple";
                    }

                    if (User.BetList.ElementAt(i).RequestResponse != null)
                    {
                        settledBet.RequestModeOpposite = false;
                        settledBet.requestResponseVisible = true;
                        ActiveBets.Add(settledBet);
                    }
                    else
                    {
                        settledBet.CashOrNotText = User.BetList.ElementAt(i).NonCashBet == null ? User.BetList.ElementAt(i).CashBetAmount : User.BetList.ElementAt(i).NonCashBet;
                        settledBet.RequestModeOpposite = !settledBet.RequestMode;
                        settledBet.PostSettlementRequestText = User.BetList.ElementAt(i).InitiatedRequest == true ? "Verifying with opponent" : "Opponent's requested settlement";                    
                        settledBet.BetPaidEnabled = User.BetList.ElementAt(i).InitiatedRequest == true ? false : true; //if user sent, disable. if user received, need to respond.
                        ActiveBets.Add(settledBet);
                    }
                    
                }
            }
        }

        public async Task ExecuteRequestResponseCommand(string betId)
        {
            string response = ActiveBets.FirstOrDefault(offers => offers.BetId == betId).RequestResponse;

            await Application.Current.MainPage.DisplayAlert(response, null, "Ok");

            User.BetList.FirstOrDefault(offers => offers.BetId == betId).RequestResponse = null;

            await CosmoDBService.UpdateUser(User);

            ExecuteRefreshCommand();
        }

        public async Task ExecuteRequestSettlementCommand(string betId)
        {
            ActiveBetsViewModel activeBetTermSheet = ActiveBets.FirstOrDefault(offers => offers.BetId == betId);
            UserData opponent = await CosmoDBService.GetUser(activeBetTermSheet.OpponentsUsername);

            if (opponent.BetList.FirstOrDefault(offers => offers.BetId == betId).RequestResponse != null)
            {
                await Application.Current.MainPage.DisplayAlert("Opponent has not read you response yet", null, "Ok");
                return;
            }

            bool betWon = await Application.Current.MainPage.DisplayAlert("Did you win this bet?", null, "Yes", "No");
            bool betPaid = false;

            await Task.Delay(200);

            if (betWon)
            {
                betPaid = await Application.Current.MainPage.DisplayAlert("Did your opponent pay you?", null, "Yes", "No");
            }
            else
            {
                betPaid = await Application.Current.MainPage.DisplayAlert("Did you pay you opponent?", null, "Yes", "No");
            }

            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {
                    User.BetList.ElementAt(i).RequestMode = true;
                    User.BetList.ElementAt(i).InitiatedRequest = true;
                    User.BetList.ElementAt(i).BetWon = betWon;
                    User.BetList.ElementAt(i).BetPaid = betPaid;
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.ElementAt(i).RequestMode = true;
                    opponent.BetList.ElementAt(i).InitiatedRequest = false;
                    opponent.BetList.ElementAt(i).BetWon = !betWon;
                    opponent.BetList.ElementAt(i).BetPaid = betPaid;
                }
            }

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();
        }

        public async Task ExecuteSettleBetCommand(string betId)
        {
            var termSheet = ActiveBets.FirstOrDefault(offers => offers.BetId == betId);            
            var opponentUsername = termSheet.OpponentsUsername;
            var opponent = await CosmoDBService.GetUser(opponentUsername);
            
            bool agreeToBetResultClaim = false;
            bool agreeToBetPaymentClaim = false;

            if (termSheet.BetWon)
            {
                agreeToBetResultClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming that you have won the bet. Is this true?", null, "Yes", "No");                
            }
            else
            {
                agreeToBetResultClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming to have won the bet. Is this true?", null, "Yes", "No");
            }
            
            if (agreeToBetResultClaim == false)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).RequestMode = false;
                        User.BetList.ElementAt(i).BetWon = false;
                        User.BetList.ElementAt(i).BetPaid = false;
                        User.BetList.ElementAt(i).InitiatedRequest = false;                        
                    }
                }

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).RequestMode = false;
                        opponent.BetList.ElementAt(i).BetWon = false;
                        opponent.BetList.ElementAt(i).BetPaid = false;
                        opponent.BetList.ElementAt(i).InitiatedRequest = false;
                        opponent.BetList.ElementAt(i).RequestResponse = "Opponent disagrees with the bet result";                        
                    }
                }

                await CosmoDBService.UpdateUser(User);
                await CosmoDBService.UpdateUser(opponent);
                ExecuteRefreshCommand();
                return;
            }

            await Task.Delay(200);
            
            if (termSheet.BetPaid)
            {
                if (termSheet.BetWon)
                {
                    agreeToBetPaymentClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming to have paid you. Is this true?", null, "Yes", "No");
                }
                else
                {
                    agreeToBetPaymentClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming that you have paid them. Is this true?", null, "Yes", "No");
                }
            }
            else if (!termSheet.BetPaid)
            {
                if (termSheet.BetWon)
                {
                    agreeToBetPaymentClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming that they have not paid you. Is this true?", null, "Yes", "No");
                }
                else
                {
                    agreeToBetPaymentClaim = await Application.Current.MainPage.DisplayAlert("Your opponent is claiming that you have not paid them. Is this true?", null, "Yes", "No");
                }
            }

            termSheet.BetPaid = agreeToBetPaymentClaim == false ? false : termSheet.BetPaid;

            if (termSheet.BetWon && termSheet.BetPaid)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        User.BetList.ElementAt(i).RequestMode = false;
                        User.BetList.ElementAt(i).InitiatedRequest = false;              
                        User.UserResults.BetsWon++;
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashWon += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                            User.UserResults.CashWonCollected += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
                
                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        opponent.BetList.ElementAt(i).RequestMode = false;
                        opponent.BetList.ElementAt(i).InitiatedRequest = false;
                        opponent.BetList.ElementAt(i).RequestResponse = "Opponent accepts request";
                        opponent.UserResults.BetsLost++;
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.CashLost += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                            opponent.UserResults.CashLostPaid += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }
            else if (termSheet.BetWon && !termSheet.BetPaid)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledUnpaid";
                        User.BetList.ElementAt(i).RequestMode = false;
                        User.BetList.ElementAt(i).InitiatedRequest = false;                        
                        User.BetList.ElementAt(i).BetPaid = false;
                        User.UserResults.BetsWon++;
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashWon += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledUnpaid";
                        opponent.BetList.ElementAt(i).BetPaid = false;
                        opponent.BetList.ElementAt(i).RequestMode = false;
                        opponent.BetList.ElementAt(i).InitiatedRequest = false;
                        opponent.BetList.ElementAt(i).RequestResponse = agreeToBetPaymentClaim == false ?
                            "Your opponent denies that bet payment has been settled. This bet will be moved to Settled Bets - Unpaid" : "Opponent accepts request";
                        opponent.UserResults.BetsLost += 1;
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.CashLost += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }
            else if (!termSheet.BetWon && termSheet.BetPaid)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        User.BetList.ElementAt(i).RequestMode = false;
                        User.BetList.ElementAt(i).InitiatedRequest = false;
                        User.UserResults.BetsLost++;
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashLost += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                            User.UserResults.CashLostPaid += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledPaid";
                        opponent.BetList.ElementAt(i).RequestMode = false;
                        opponent.BetList.ElementAt(i).InitiatedRequest = false;
                        opponent.BetList.ElementAt(i).RequestResponse = "Opponent accepts request";
                        opponent.UserResults.BetsWon++;
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.CashWon += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                            opponent.UserResults.CashWonCollected += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }
            else if (!termSheet.BetWon && !termSheet.BetPaid)
            {
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == betId)
                    {
                        User.BetList.ElementAt(i).BetPhase = "SettledUnpaid";
                        User.BetList.ElementAt(i).RequestMode = false;
                        User.BetList.ElementAt(i).InitiatedRequest = false;
                        User.UserResults.BetsLost += 1;
                        if (User.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            User.UserResults.CashLost += Convert.ToInt32(User.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }

                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == betId)
                    {
                        opponent.BetList.ElementAt(i).BetPhase = "SettledUnpaid";
                        opponent.BetList.ElementAt(i).RequestMode = false;
                        opponent.BetList.ElementAt(i).InitiatedRequest = false;
                        opponent.BetList.ElementAt(i).RequestResponse = agreeToBetPaymentClaim == false ?
                            "Your opponent denies that bet payment has been settled. This bet will be moved to Settled Bets - Unpaid" : "Opponent accepts request";
                        if (opponent.BetList.ElementAt(i).CashBetAmount != null)
                        {
                            opponent.UserResults.BetsWon++;
                            opponent.UserResults.CashWon += Convert.ToInt32(opponent.BetList.ElementAt(i).CashBetAmount);
                        }
                    }
                }
            }

            User.UserResults.CredibilityRatio = User.UserResults.CashLost == User.UserResults.CashLostPaid ? 100 : User.UserResults.CashLostPaid / User.UserResults.CashLost * 100;
            User.UserResults.CollectionRatio = User.UserResults.CashWon == User.UserResults.CashWonCollected ? 100 : User.UserResults.CashWonCollected / User.UserResults.CashWon * 100;

            opponent.UserResults.CredibilityRatio = opponent.UserResults.CashLost == opponent.UserResults.CashLostPaid ? 100 : opponent.UserResults.CashLostPaid / opponent.UserResults.CashLost * 100;
            opponent.UserResults.CollectionRatio = opponent.UserResults.CashWon == opponent.UserResults.CashWonCollected? 100 : opponent.UserResults.CashWonCollected/ opponent.UserResults.CashWon * 100;

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();            
        }

    }
}
