using BetBook.ViewModels;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BetBook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OffersReceivedPage : ContentPage
    {
        OffersReceivedViewModel offersReceivedVM;
        public OffersReceivedPage()
        {
            InitializeComponent();
            offersReceivedVM = new OffersReceivedViewModel();
            BindingContext = offersReceivedVM;

            //MessagingCenter.Subscribe<object, bool>(this, "RemoveOffer", (sender, arg) =>
            //{
            //    offersReceivedVM.RefreshCommand.Execute(null);
            //    counter++;
            //});

            MessagingCenter.Subscribe<object, bool>(Application.Current, "Refresh", (sender, arg) =>
            {
                offersReceivedVM.RefreshCommand.Execute(null);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            offersReceivedVM.RefreshCommand.Execute(null);
            offersReceivedVM.CheckEffectExpiry(null);
        }

        public void AcceptOffer(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            offersReceivedVM.AcceptOfferCommand.Execute(betId);            
        }

        public void DenyOffer(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            offersReceivedVM.DenyOfferCommand.Execute(betId);
        }
    }
}