using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    public partial class SettingsPageModel : ObservableObject
    {
        private readonly ZProperties properties;

        public int FontSize
        {
            get => properties.FontSize;
            set
            {
                if (properties.FontSize != value)
                {
                    properties.FontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }
        }
        public ThemeType ThemeMode
        {
            get => properties.ThemeMode;
            set
            {
                if (properties.ThemeMode != value)
                {
                    ZSystem.Instance.DarkMode = ((value == ThemeType.System && ZSystem.IsSystemInDarkMode) || value == ThemeType.Dark);
                    properties.ThemeMode = value;
                    OnPropertyChanged(nameof(ThemeMode));
                }
            }
        }
        public bool PlaySound
        {
            get => properties.PlaySound;
            set
            {
                if (properties.PlaySound != value)
                {
                    properties.PlaySound = value;
                    OnPropertyChanged(nameof(PlaySound));
                }
            }
        }
        public bool OpeningRoll
        {
            get => properties.OpeningRoll;
            set
            {
                if (properties.OpeningRoll != value)
                {
                    properties.OpeningRoll = value;
                    OnPropertyChanged(nameof(OpeningRoll));
                }
            }
        }
        public SettingsPageModel(ZProperties properties)
        {
            this.properties = properties;
            this.properties.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ZProperties.FontSize))
                {
                    OnPropertyChanged(nameof(FontSize));
                }
                else if (e.PropertyName == nameof(ZProperties.ThemeMode))
                {
                    OnPropertyChanged(nameof(ThemeMode));
                }
                else if (e.PropertyName == nameof(ZProperties.PlaySound))
                {
                    OnPropertyChanged(nameof(PlaySound));
                }
                else if (e.PropertyName == nameof(ZProperties.OpeningRoll))
                {
                    OnPropertyChanged(nameof(OpeningRoll));
                }
            };
        }

        public IRelayCommand GoBackCommand => new RelayCommand(OnGoBackCommand);
        private async void OnGoBackCommand()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
