using System;
using System.Collections.Generic;
using System.Text;

namespace BetBook.ViewModels
{
    class PopupViewModel : ViewModelEventHandler
    {
        string inputQuestion;
        public string InputQuestion
        {
            get => inputQuestion;
            set
            {
                inputQuestion = value;
                OnPropertyChanged();
            }
        }

        bool yesNoVisible;
        public bool YesNoVisible
        {
            get => yesNoVisible;
            set
            {
                yesNoVisible = value;
                OnPropertyChanged();
            }
        }

        bool yesNoOpposite;
        public bool YesNoOpposite
        {
            get => yesNoOpposite;
            set
            {
                yesNoOpposite = value;
                OnPropertyChanged();
            }
        }
    }
}
