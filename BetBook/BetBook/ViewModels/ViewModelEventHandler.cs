using BetBook.Models;
using BetBook.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BetBook.ViewModels
{
    public class ViewModelEventHandler : TermSheet, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> ShowPopup(string question)
        {
            PopupView popup = new PopupView();
            
            PopupViewModel popupVM = new PopupViewModel();                        
            popupVM.InputQuestion = question;

            if (question != "Opponent has not read your response yet." && question != "Opponent disagrees with the payment claim." 
                && question != "Opponent disagrees with the bet result.")
            {
                popupVM.YesNoVisible = true;
                popupVM.YesNoOpposite = false;
            }
            else
            {
                popupVM.YesNoVisible = false;
                popupVM.YesNoOpposite = true;
            }

            popup.BindingContext = popupVM;
            await PopupNavigation.Instance.PushAsync(popup);
            bool popupResponse = false;

            bool moveOn = false;
            while (!moveOn)
            {
                await Task.Delay(25);
                MessagingCenter.Subscribe<object, bool>(this, "DismissPopup", (sender, arg) =>
                {
                    popupResponse = arg;
                    moveOn = true;
                });
            }

            await PopupNavigation.Instance.PopAsync();

            return popupResponse;
        }
    }
}
