using BetBook.Models;
using BetBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Azure.WebJobs;
using BetBook.Services;

namespace BetBook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveBetsPage : ContentPage
    {
        ActiveBetsViewModel activeBetsVM;
        
        public ActiveBetsPage()
        {
            InitializeComponent();
            activeBetsVM = new ActiveBetsViewModel();
            BindingContext = activeBetsVM;

            //MessagingCenter.Subscribe<object, bool>(Application.Current, "Refresh", (sender, arg) =>
            //{
            //    activeBetsVM.RefreshCommand.Execute(null);
            //});
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();            
            activeBetsVM.RefreshCommand.Execute(null);
            //activeBetsVM.CheckEffectExpiry(null);
        }
        public async void RequestSettlement(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await activeBetsVM.ExecuteRequestSettlementCommand(betId);

            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);
            this.Parent.BindingContext = tabsVM;
        }

        public async void RespondToRequest(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await activeBetsVM.ExecuteResponseToRequestCommand(betId);
            
            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);            
            this.Parent.BindingContext = tabsVM;
        }

        public async void RespondToResponse(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            string betId = button.CommandParameter.ToString();
            await activeBetsVM.ExecuteResponseToResponseCommand(betId);

            TabsHomeViewModel tabsVM = new TabsHomeViewModel();
            tabsVM.RefreshCommand.Execute(null);
            this.Parent.BindingContext = tabsVM;
        }
    }
}