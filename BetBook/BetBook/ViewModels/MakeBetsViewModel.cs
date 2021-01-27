using BetBook.Models;
using BetBook.Services;
using BetBook.Views;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.SystemFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Xamarin.Forms.Xaml;

namespace BetBook.ViewModels
{
    public class MakeBetsViewModel : ViewModelEventHandler
    {
        TermSheet betTermSheet;
        public TermSheet BetTermSheet
        {
            get => betTermSheet;
            set
            {
                betTermSheet = value;
                OnPropertyChanged();
            }
        }

        bool isCashToggle = true;
        public bool IsCashToggle
        {
            get => isCashToggle;
            set
            {
                isCashToggle = value;
                OnPropertyChanged();
            }
        }

        bool isExpirySet = false;
        public bool IsExpirySet
        {
            get => isExpirySet;
            set
            {
                isExpirySet = value;
                OnPropertyChanged();
            }
        }

        bool isExpirySetBet = false;
        public bool IsExpirySetBet
        {
            get => isExpirySetBet;
            set
            {
                isExpirySetBet = value;
                OnPropertyChanged();
            }
        }

        bool oppositeCashToggle = false;
        public bool OppositeCashToggle
        {
            get => oppositeCashToggle;
            set
            {
                oppositeCashToggle = value;
                OnPropertyChanged();
            }
        }

        string isCashToggleText;
        public string IsCashToggleText
        {
            get => isCashToggleText;
            set
            {
                isCashToggleText = value;
                OnPropertyChanged();
            }
        }

        DateTime expiryDate = DateTime.Now;
        public DateTime ExpiryDate
        {
            get => expiryDate;
            set
            {
                expiryDate = value;
                OnPropertyChanged();
            }
        }

        TimeSpan expiryTime = TimeSpan.Zero;
        public TimeSpan ExpiryTime
        {
            get => expiryTime;
            set
            {
                expiryTime = value;
                OnPropertyChanged();
            }
        }

        DateTime expiryDateBet = DateTime.Now;
        public DateTime ExpiryDateBet
        {
            get => expiryDateBet;
            set
            {
                expiryDateBet = value;
                OnPropertyChanged();
            }
        }

        TimeSpan expiryTimeBet = TimeSpan.Zero;
        public TimeSpan ExpiryTimeBet
        {
            get => expiryTimeBet;
            set
            {
                expiryTimeBet = value;
                OnPropertyChanged();
            }
        }

        DateTime expiryDateTime;
        public DateTime ExpiryDateTime
        {
            get => expiryDateTime;
            set
            {
                expiryDateTime = value;
                OnPropertyChanged();
            }
        }

        DateTime expiryDateTimeBet;
        public DateTime ExpiryDateTimeBet
        {
            get => expiryDateTimeBet;
            set
            {
                expiryDateTimeBet = value;
                OnPropertyChanged();
            }
        }

        UserData User { get; set; }

        public MakeBetsViewModel()
        {
            User = LoginViewModel.loggedUser;
            BetTermSheet = new TermSheet();
            BetTermSheet.MyUsername = User.Username;
            //SendOfferCommand = new Command(async() => await ExecuteSendOfferCommand());
            ChangeCommand = new Command(() => ExecuteChangeCommand());
            RefreshCommand = new Command(() => ExecuteRefreshCommand());
        }

        public ICommand RefreshCommand { get; }
        void ExecuteRefreshCommand()
        {
            User = LoginViewModel.loggedUser;
            BetTermSheet = new TermSheet();
            BetTermSheet.MyUsername = User.Username;
        }

        public ICommand ChangeCommand { get; }
        void ExecuteChangeCommand()
        {   
            if (IsCashToggle)
            {
                IsCashToggleText = "Cash Bet";
                OppositeCashToggle = false;
            }
            else
            {
                IsCashToggleText = "Non Cash Bet";
                OppositeCashToggle = true;
            }
        }

        string StandardTimeConversion(TimeSpan militaryTime)
        {
            string standardTime = "";
            string militaryTimeString = militaryTime.ToString();
            var firstColon = militaryTimeString.IndexOf(":");
            var hours = militaryTimeString.Substring(0, firstColon);
            var minutes = militaryTimeString.Substring(firstColon, 3);
            string amOrPm = Convert.ToInt32(hours) <= 12 ? " AM" : " PM";
            
            if (hours == "00")
            {
                hours = "12";
            }
            else if (Convert.ToInt32(hours) >= 13)
            {
                hours = (Convert.ToInt32(hours) - 12).ToString();
            }

            standardTime = hours + minutes + amOrPm;

            return standardTime;
        }
        //public ICommand SendOfferCommand { get; }
        public async Task ExecuteSendOfferCommand()
        {
            ExpiryDateTime = ExpiryDate.Date + ExpiryTime;
            TimeSpan offerExpiryCheck = ExpiryDateTime - DateTime.Now;

            ExpiryDateTimeBet = ExpiryDateBet.Date + ExpiryTimeBet;
            TimeSpan betCloseExpiryCheck = ExpiryDateTimeBet - DateTime.Now;

            int result;
            bool isNum = int.TryParse(BetTermSheet.CashBetAmount, out result);

            if (isNum == false && BetTermSheet.NonCashBet == null)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Invalid number", null, "Ok");
                return;
            }

            UserData opponent = await CosmoDBService.GetUser(BetTermSheet.OpponentsUsername);

            if (opponent == null || opponent.Username == User.Username)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Invalid opponent username", null, "Ok");
                return;
            }
            
            BetTermSheet.DateTimeOffered = DateTime.Now.ToString();
            
            if (IsExpirySet) //the below is commented out in order to test expiry timers more effectively
            {
                //if (offerExpiryCheck.TotalSeconds < 298)
                //{
                //    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Offer expiry must be at least 5 minutes from current time", null, "Ok");
                //    return;
                //}
                BetTermSheet.DateTimeOfferExpiration = ExpiryDateTime.ToString();
            }
            else
            {
                BetTermSheet.DateTimeOfferExpiration = "None";
            }

            if (IsExpirySetBet) //the below is commented out in order to test expiry timers more effectively
            {
                //if (betCloseExpiryCheck.TotalSeconds < 598)
                //{
                //    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Bet expiry must be at least 10 minutes from current time", null, "Ok");
                //    return;
                //}
                BetTermSheet.DateTimeBetClose = ExpiryDateTimeBet.ToString();
            }
            else
            {
                BetTermSheet.DateTimeBetClose = "None";
            }

            BetTermSheet.BetPhase = "OfferSent";
            BetTermSheet.BetId = Guid.NewGuid().ToString();

            TermSheet opponentsTermSheet = CreateOpponentsTermSheet(BetTermSheet);
            
            User.BetList.Add(BetTermSheet);
            opponent.BetList.Add(opponentsTermSheet);

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            LoginViewModel.loggedUser = User;
        }

        public async Task ExecuteBetResolutionReminder(long delay, string betId)
        {
            while (delay > 0)
            {
                var currentDelay = delay > int.MaxValue ? int.MaxValue : (int)delay;
                await Task.Delay(currentDelay);
                delay -= currentDelay;
            }

            User = CosmoDBService.GetUser(User.Username).Result;
            TermSheet termSheet = User.BetList.FirstOrDefault(offers => offers.BetId == betId);

            if (termSheet == null)
            {
                return;
            }

            UserData opponent = await CosmoDBService.GetUser(termSheet.OpponentsUsername);

            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {
                    User.BetList.ElementAt(i).BetCloseReminder = true;
                    User.BetList.ElementAt(i).DateTimeBetClose = User.BetList.ElementAt(i).DateTimeBetClose + " (Now)";
                    await CosmoDBService.UpdateUser(User);
                    break;
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.ElementAt(i).BetCloseReminder = true;
                    opponent.BetList.ElementAt(i).DateTimeBetClose = opponent.BetList.ElementAt(i).DateTimeBetClose + " (Now)";
                    await CosmoDBService.UpdateUser(opponent);
                    break;
                }
            }

            LoginViewModel.loggedUser = await CosmoDBService.GetUser(LoginViewModel.loggedUser.Username);
        }

        List<string> idTracker = new List<string>();
        
        public async Task ExecuteWithdrawal(long delay, string betId)
        {            
            while (delay > 0)
            {
                var currentDelay = delay > int.MaxValue ? int.MaxValue : (int)delay;
                await Task.Delay(currentDelay);
                delay -= currentDelay;
            }

            User = CosmoDBService.GetUser(User.Username).Result;
            TermSheet termSheet = User.BetList.FirstOrDefault(offers => offers.BetId == betId);

            if (termSheet.BetPhase == "ActiveBet")
            {
                return;
            }

            idTracker.Add(betId);
            
            UserData opponent = await CosmoDBService.GetUser(termSheet.OpponentsUsername);

            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {                    
                    User.BetList.RemoveAt(i);
                    await CosmoDBService.UpdateUser(User);
                    break;
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.RemoveAt(i);
                    await CosmoDBService.UpdateUser(opponent);
                    break;
                }
            }

            for (int j = 0; j < idTracker.Count; j++)
            {
                for (int i = 0; i < opponent.BetList.Count(); i++)
                {
                    if (opponent.BetList.ElementAt(i).BetId == idTracker.ElementAt(j))
                    {
                        opponent.BetList.RemoveAt(i);                        
                        break;
                    }
                }
                for (int i = 0; i < User.BetList.Count(); i++)
                {
                    if (User.BetList.ElementAt(i).BetId == idTracker.ElementAt(j))
                    {
                        User.BetList.RemoveAt(i);                        
                        break;
                    }
                }
            }

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);
            LoginViewModel.loggedUser = await CosmoDBService.GetUser(LoginViewModel.loggedUser.Username);            
        }

        public TermSheet CreateOpponentsTermSheet(TermSheet inputSheet)
        {
            TermSheet outputSheet = new TermSheet();

            outputSheet.BetId = inputSheet.BetId;
            outputSheet.MyUsername = inputSheet.OpponentsUsername;
            outputSheet.OpponentsUsername = User.Username;
            outputSheet.DateTimeOffered = inputSheet.DateTimeOffered;
            outputSheet.DateTimeAccepted = null; 
            outputSheet.DateTimeBetSettled = null; //next
            outputSheet.DateTimePaid = null;
            outputSheet.CashBetAmount = inputSheet.CashBetAmount;
            outputSheet.NonCashBet = inputSheet.NonCashBet;
            outputSheet.BetTerms = inputSheet.BetTerms;
            outputSheet.DateTimeOfferExpiration = inputSheet.DateTimeOfferExpiration;
            //outputSheet.SetBetCloseDate = inputSheet.SetBetCloseDate;
            outputSheet.DateTimeBetClose = inputSheet.DateTimeBetClose;
            outputSheet.BetPhase = "OfferReceived";

            return outputSheet;
        }

    }
}
