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

            MessagingCenter.Subscribe<object, bool>(this, "RemoveOffer", (sender, arg) =>
            {
                offersSentVM.RefreshCommand.Execute(null);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            offersSentVM.RefreshCommand.Execute(null);
        }
    }
}