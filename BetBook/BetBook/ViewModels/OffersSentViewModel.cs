using BetBook.Models;
using BetBook.Services;
using Microsoft.Azure.Documents;
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
    class OffersSentViewModel : ViewModelEventHandler
    {
        ObservableCollection<OffersSentViewModel> offersSent;
        public ObservableCollection<OffersSentViewModel> OffersSent
        {
            get => offersSent;
            set
            {
                offersSent = value;
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

        public OffersSentViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());    

            DateTime nearestMinute = RoundUp(DateTime.Now, TimeSpan.FromMinutes(1));
            TimeSpan nearestMinuteDifference = nearestMinute - DateTime.Now;
            int formattedDifference = (int)nearestMinuteDifference.TotalMilliseconds;

            TimerCallback tmCallback = CheckEffectExpiry;
            Timer timer = new Timer(tmCallback, "", formattedDifference, 60000);
        }

        public async void CheckEffectExpiry(object objectInfo)
        {
            //bool refresh = true;
            if (OffersSent != null)
            {
                for (int i = 0; i < OffersSent.Count; i++)
                {
                    bool isExpired = false;
                    string offerExpiry = OffersSent.ElementAt(i).DateTimeOfferExpiration;
                    if (offerExpiry != "None")
                    {
                        DateTime offerExpiryDt = DateTime.Parse(offerExpiry);
                        TimeSpan timeLeftExpiry = offerExpiryDt - DateTime.Now;
                        isExpired = timeLeftExpiry.TotalMilliseconds <= 0;
                    }

                    if (isExpired)
                    {
                        //refresh = false;
                        UserData opponent = await CosmoDBService.GetUser(OffersSent.ElementAt(i).OpponentsUsername);
                        for (int a = 0; a < User.BetList.Count(); a++)
                        {
                            if (User.BetList.ElementAt(a).BetId == OffersSent.ElementAt(i).BetId)
                            {
                                User.BetList.RemoveAt(a);
                            }
                        }

                        for (int b = 0; b < opponent.BetList.Count(); b++)
                        {
                            if (opponent.BetList.ElementAt(b).BetId == OffersSent.ElementAt(i).BetId)
                            {
                                opponent.BetList.RemoveAt(b);
                            }
                        }
                        await CosmoDBService.UpdateUser(opponent);
                        await CosmoDBService.UpdateUser(User);
                        OffersSent.RemoveAt(i);
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
            OffersSent = new ObservableCollection<OffersSentViewModel>();
            var offerList = User.BetList.Where(terms => terms.BetPhase == "OfferSent");

            for (int i = 0; i < offerList.Count(); i++)
            {
                OffersSentViewModel offerSent = JsonConvert.DeserializeObject<OffersSentViewModel>(JsonConvert.SerializeObject(offerList.ElementAt(i)));
                offerSent.CashOrNotText = offerSent.NonCashBet ?? offerSent.CashBetAmount;
                OffersSent.Add(offerSent);
            }
        }
    }
}
