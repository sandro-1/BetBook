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
    public partial class OffersSentPage : ContentPage
    {
        OffersSentViewModel offersSentVM;
        public OffersSentPage()
        {
            InitializeComponent();
            offersSentVM = new OffersSentViewModel();
            BindingContext = offersSentVM;

            //MessagingCenter.Subscribe<object, bool>(this, "RemoveOffer", (sender, arg) =>
            //{
            //    offersSentVM.RefreshCommand.Execute(null);
            //});
            MessagingCenter.Subscribe<object, bool>(Application.Current, "Refresh", (sender, arg) =>
            {
                offersSentVM.RefreshCommand.Execute(null);                
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            offersSentVM.RefreshCommand.Execute(null);
            offersSentVM.CheckEffectExpiry(null);
        }

        //public async void RemoveExpiredOffer(Object Sender, EventArgs args)
        //{
        //    Button button = (Button)Sender;
        //    string betId = button.CommandParameter.ToString();
        //    await offersSentVM.ExecuteExpiredOfferRemoval(betId);

        //    TabsHomeViewModel tabsVM = new TabsHomeViewModel();
        //    tabsVM.RefreshCommand.Execute(null);
        //    this.Parent.BindingContext = tabsVM;
        //}
    }
}