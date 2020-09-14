using BetBook.ViewModels;
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
    public partial class SettledBetsUnpaidPage : ContentPage
    {
        SettledBetsUnpaidViewModel settledBetsUnpaidVM;
        public SettledBetsUnpaidPage()
        {
            InitializeComponent();
            settledBetsUnpaidVM = new SettledBetsUnpaidViewModel();
            BindingContext = settledBetsUnpaidVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            settledBetsUnpaidVM.RefreshCommand.Execute(null);
        }

        public async void RequestSettlement(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await settledBetsUnpaidVM.ExecuteRequestSettlementCommand(betId);

            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);
            this.Parent.BindingContext = tabsVM;
        }

        public async void RespondToRequest(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await settledBetsUnpaidVM.ExecuteResponseToRequestCommand(betId);

            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);
            this.Parent.BindingContext = tabsVM;
        }


        public async void RespondToResponse(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await settledBetsUnpaidVM.ExecuteResponseToResponseCommand(betId);

            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);
            this.Parent.BindingContext = tabsVM;
        }
    }
}