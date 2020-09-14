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
    public partial class PopupView
    {
        public PopupView()
        {
            InitializeComponent();
        }

        public void AnswerPopupQuestion(Object Sender, EventArgs args)
        {
            Button button = (Button)Sender;
            bool popupResponse = Convert.ToBoolean(button.CommandParameter);
            MessagingCenter.Send<object, bool>(this, "DismissPopup", popupResponse);
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}