using BetBook.Models;
using BetBook.Services;
using Microsoft.Azure.Documents;
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

        UserData User { get; set; }

        public OffersSentViewModel()
        {
            User = LoginViewModel.loggedUser;
            RefreshCommand = new Command(() => ExecuteRefreshCommand());
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
