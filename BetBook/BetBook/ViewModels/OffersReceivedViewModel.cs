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
                offer.CashOrNotText = offer.NonCashBet ?? offer.CashBetAmount; //assign noncashbet if it is not null else cashbetamount
                OffersReceived.Add(offer);
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
