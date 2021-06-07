using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Server_Manager.Scripts
{
    public class ErrorItem : INotifyPropertyChanged
    {
        public string ErrorMessage 
        {
            get => EErrorMessage;
            set
            {
                EErrorMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EErrorMessage)));
            }
        }
        public Visibility ButtonVisibility 
        {
            get => BButtonVisibility;
            set
            {
                BButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BButtonVisibility)));
            }
        }
        public ICommand ButtonClickAction 
        {
            get => BButtonClickAction;
            set
            {
                BButtonClickAction = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BButtonClickAction)));
            }
        }
        public string ButtonText 
        {
            get => BButtonText;
            set
            {
                BButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BButtonText)));
            }
        }
        public Style ButtonStyle 
        {
            get => BButtonStyle;
            set
            {
                BButtonStyle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BButtonStyle)));
            }
        }
        public bool ButtonEnabled 
        {
            get => BButtonEnabled;
            set
            {
                BButtonEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BButtonEnabled)));
            }
        }

        public string EErrorMessage { get; private set; }
        public Visibility BButtonVisibility { get; private set; } = Visibility.Hidden;
        public ICommand BButtonClickAction { get; private set; }
        public string BButtonText { get; private set; }
        public Style BButtonStyle { get; private set; }
        public bool BButtonEnabled { get; private set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
