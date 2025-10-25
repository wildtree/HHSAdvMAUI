// Properties for HHSAdvSDL

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace HHSAdvMAUI
{
    public enum ThemeType { Light, Dark, System }
    public partial class ZProperties : INotifyPropertyChanged
    {
        private int fontSize = 16;
        public int FontSize
        {
            get => Preferences.Get(nameof(FontSize), 16);
            set
            {
                if (fontSize != value)
                {
                    fontSize = value;
                    Preferences.Set(nameof(FontSize), value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
                }
            }
        }
        private ThemeType themeMode = ThemeType.System;
        public ThemeType ThemeMode
        {
            get
            {
                if (Enum.TryParse(typeof(ThemeType), Preferences.Get(nameof(ThemeMode), ThemeType.System.ToString()), out var value))
                {
                    return (ThemeType)value!;
                }
                return ThemeType.System;
            }
            set
            {
                if (themeMode != value)
                {
                    themeMode = value;
                    Preferences.Set(nameof(ThemeMode), value.ToString());
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeMode)));
                }
            }
        }
        private bool playSound = true;
        public bool PlaySound
        {
            get => Preferences.Get(nameof(PlaySound), true);
            set
            {
                if (playSound != value)
                {
                    playSound = value;
                    Preferences.Set(nameof(PlaySound), value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlaySound)));
                }
            }
        }
        private bool openingRoll = true;
        public bool OpeningRoll
        {
            get => Preferences.Get(nameof(OpeningRoll), true);
            set
            {
                if (openingRoll != value)
                {
                    openingRoll = value;
                    Preferences.Set(nameof(OpeningRoll), value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpeningRoll)));
                }
            }
        }
        private int width = 480;
        public int Width
        {
            get => Preferences.Get(nameof(Width), 640);
            set
            {
                if (width != value)
                {
                    width = value;
                    Preferences.Set(nameof(Width), value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        private int height = 640;
        public int Height
        {
            get => Preferences.Get(nameof(Height), 640);
            set
            {
                if (height != value)
                {
                    height = value;
                    Preferences.Set(nameof(Height), value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
 
        public ZProperties()
        {
            fontSize = Preferences.Get(nameof(FontSize), fontSize);
            openingRoll = Preferences.Get(nameof(OpeningRoll), openingRoll);
            playSound = Preferences.Get(nameof(PlaySound), playSound);
            width = Preferences.Get(nameof(Width), width);
            height = Preferences.Get(nameof(Height), height);
            if (Enum.TryParse(typeof(ThemeType), Preferences.Get(nameof(ThemeMode), themeMode.ToString()), out var value))
            {
                themeMode = (ThemeType)value;
            }
        }
    }

}
