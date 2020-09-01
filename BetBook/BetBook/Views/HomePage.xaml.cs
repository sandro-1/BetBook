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
    public partial class HomePage : MasterDetailPage
    {
        public HomePage()
        {
            InitializeComponent();
            TabbedPage tabsHome = new TabsHomePage();
            tabsHome.Children.Add(new ActiveBetsPage());
            tabsHome.Children.Add(new MakeBetsPage());
            tabsHome.Children.Add(new OffersReceivedPage());
            tabsHome.Children.Add(new OffersSentPage());
            tabsHome.Children.Add(new SettledBetsUnpaidPage());
            tabsHome.Children.Add(new SettledBetsPaidPage());
            Detail = new NavBasePage(tabsHome);            
        }
    }
}