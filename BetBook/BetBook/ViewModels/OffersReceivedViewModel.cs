using BetBook.Models;
using BetBook.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    class OffersReceivedViewModel : ViewModelEventHandler
    {
        ObservableCollection<OffersReceivedViewModel> offersReceived;
        public ObservableCollection<OffersReceivedViewModel> OffersReceived
        {
            get => offersReceived;
            set
            {
                offersReceived = value;
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

        public OffersReceivedViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());

            AcceptOfferCommand = new Command<string>(async (betId) => await ExecuteAcceptOfferCommand(betId));
            DenyOfferCommand = new Command<string>(async (betId) => await ExecuteDenyOfferCommand(betId));

            DateTime nearestMinute = RoundUp(DateTime.Now, TimeSpan.FromMinutes(1));
            TimeSpan nearestMinuteDifference = nearestMinute - DateTime.Now;
            int formattedDifference = (int)nearestMinuteDifference.TotalMilliseconds;

            TimerCallback tmCallback = CheckEffectExpiry;
            Timer timer = new Timer(tmCallback, "", formattedDifference, 60000);
        }

        public async void CheckEffectExpiry(object objectInfo) 
        {
            //bool refresh = true;
            if (OffersReceived != null)
            {
                for (int i = 0; i < OffersReceived.Count; i++)
                {
                    bool isExpired = false;
                    string offerExpiry = OffersReceived.ElementAt(i).DateTimeOfferExpiration;
                    if (offerExpiry != "None")
                    {
                        DateTime offerExpiryDt = DateTime.Parse(offerExpiry);
                        TimeSpan timeLeftExpiry = offerExpiryDt - DateTime.Now;
                        isExpired = timeLeftExpiry.TotalMilliseconds <= 0;
                    }

                    if (isExpired)
                    {
                        //refresh = false;
                        UserData opponent = await CosmoDBService.GetUser(OffersReceived.ElementAt(i).OpponentsUsername);
                        for (int a = 0; a < User.BetList.Count(); a++)
                        {
                            if (User.BetList.ElementAt(a).BetId == OffersReceived.ElementAt(i).BetId)
                            {
                                User.BetList.RemoveAt(a);
                            }
                        }

                        for (int b = 0; b < opponent.BetList.Count(); b++)
                        {
                            if (opponent.BetList.ElementAt(b).BetId == OffersReceived.ElementAt(i).BetId)
                            {
                                opponent.BetList.RemoveAt(b);
                            }
                        }
                        await CosmoDBService.UpdateUser(opponent);
                        await CosmoDBService.UpdateUser(User);
                        OffersReceived.RemoveAt(i);
                        i--;
                    }
                }
            }
            //if (refresh)
            //{
            //    ExecuteRefreshCommand();
            //}
        }

        public ICommand RefreshCommand { get; }
        void ExecuteRefreshCommand() 
        {
            User = LoginViewModel.loggedUser;
            OffersReceived = new ObservableCollection<OffersReceivedViewModel>();
            var offerList = User.BetList.Where(terms => terms.BetPhase == "OfferReceived");
            for (int i = 0; i < offerList.Count(); i++)
            {
                OffersReceivedViewModel offer = JsonConvert.DeserializeObject<OffersReceivedViewModel>(JsonConvert.SerializeObject(offerList.ElementAt(i)));
                offer.CashOrNotText = offer.NonCashBet ?? offer.CashBetAmount;
                offersReceived.Add(offer);                
            }
        }

        public ICommand DenyOfferCommand { get; }

        async Task ExecuteDenyOfferCommand(string betId)
        {
            OffersReceivedViewModel termSheet = OffersReceived.FirstOrDefault(offers => offers.BetId == betId);
            UserData opponent = await CosmoDBService.GetUser(termSheet.OpponentsUsername);

            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {
                    User.BetList.RemoveAt(i);                    
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.RemoveAt(i);
                }
            }

            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();
        }
        public ICommand AcceptOfferCommand { get; }
        async Task ExecuteAcceptOfferCommand(string betId)
        {
            OffersReceivedViewModel termSheet = OffersReceived.FirstOrDefault(offers => offers.BetId == betId);
            UserData opponent = await CosmoDBService.GetUser(termSheet.OpponentsUsername);
            
            for (int i = 0; i < User.BetList.Count(); i++)
            {
                if (User.BetList.ElementAt(i).BetId == betId)
                {
                    User.BetList.ElementAt(i).BetPhase = "ActiveBet";                    
                }
            }

            for (int i = 0; i < opponent.BetList.Count(); i++)
            {
                if (opponent.BetList.ElementAt(i).BetId == betId)
                {
                    opponent.BetList.ElementAt(i).BetPhase = "ActiveBet";
                }
            }
            
            await CosmoDBService.UpdateUser(User);
            await CosmoDBService.UpdateUser(opponent);

            ExecuteRefreshCommand();
        }        
    }
}
