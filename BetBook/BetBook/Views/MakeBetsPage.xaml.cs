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
    public partial class MakeBetsPage : ContentPage
    {
        MakeBetsViewModel makeBetsVM;
        public MakeBetsPage()
        {
            InitializeComponent();
            makeBetsVM = new MakeBetsViewModel();
            BindingContext = makeBetsVM;
            MessagingCenter.Subscribe<object, bool>(this, "RemoveOffer", (sender, arg) =>
            {
                makeBetsVM.RefreshCommand.Execute(null);                
            });            
        }

        public async void SendOffer(Object Sender, EventArgs args)
        {
            await makeBetsVM.ExecuteSendOfferCommand();

            if (makeBetsVM.BetTermSheet.BetPhase == "OfferSent")
            {
                var tab = this.Parent as TabbedPage;
                tab.CurrentPage = tab.Children[0];  
            }

            var difference = makeBetsVM.ExpiryDateTime - DateTime.Now;
            var betDifference = makeBetsVM.ExpiryDateTimeBet - DateTime.Now;

            if (makeBetsVM.IsExpirySet)
            {
                await makeBetsVM.ExecuteWithdrawal((long)difference.TotalMilliseconds, makeBetsVM.BetTermSheet.BetId, true);
                MessagingCenter.Send<object, bool>(this, "RemoveOffer", true);
            }

            if (makeBetsVM.IsExpirySetBet)
            {
                await makeBetsVM.ExecuteWithdrawal((long)betDifference.TotalMilliseconds, makeBetsVM.BetTermSheet.BetId, false);
                MessagingCenter.Send<object, bool>(this, "BetClosed", true);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            makeBetsVM.RefreshCommand.Execute(null);
        }

        void OnToggled(object sender, ToggledEventArgs e)
        {
            makeBetsVM.ChangeCommand.Execute(null);
        }

    }
}