using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly ZProperties properties;
        private static MainPageModel? instance = null;
        public static MainPageModel Instance
        {
            get
            {                 
                if (instance == null)
                {
                    instance = new MainPageModel();
                }
                return instance;
            }
        }

        private MainPageModel() : base()
        {
            properties = ZSystem.Instance.Properties;
            properties.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ZProperties.FontSize))
                {
                    OnPropertyChanged(nameof(FontSize));
                }
                else if (e.PropertyName == nameof(ZProperties.Width))
                {
                    OnPropertyChanged(nameof(Width));
                }
                else if (e.PropertyName == nameof(ZProperties.Height))
                {
                    OnPropertyChanged(nameof(Height));
                }
            };
        }

        public int FontSize
        {
            get => properties.FontSize;
        }

        public int Width
        {
            get => properties.Width;
        }
        public int Height
        {
            get => properties.Height;
        }
        private StringBuilder log = new StringBuilder();
        public string Log
        {
            get => log.ToString();
        }

        public StringBuilder AppendLog(string s)
        {
            log.Append(s);
            OnPropertyChanged(nameof(Log));
            return log;
        }
        public StringBuilder AppendLogLine(string s)
        {
            log.Append(s).Append("\r\n");
            OnPropertyChanged(nameof(Log));
            return log;
        }
        public void ClearLog()
        {
            log.Clear();
            OnPropertyChanged(nameof(Log));
        }
        public IRelayCommand OpenSettingsCommand => new RelayCommand(OpenSettings);
        public IRelayCommand OpenAboutCommand => new RelayCommand(OpenAbout);

        private async void OpenSettings()
        {
            // Implement settings opening logic here
            SaveGameState();
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private async void OpenAbout()
        {
            SaveGameState();
            await Shell.Current.GoToAsync(nameof(AboutPage));
        }

        private byte[] coreState = Array.Empty<byte>();
        private byte[] userState = Array.Empty<byte>();
        private ZSystem.GameStatus status = ZSystem.GameStatus.Title;
        public void SaveGameState()
        {
            ZCore core = ZCore.Instance;
            ZUserData userData = ZUserData.Instance;
            coreState = new byte[core.packedSize];
            userState = new byte[userData.packedSize];
            Array.Copy(core.pack(), 0, coreState, 0, core.packedSize);
            Array.Copy(userData.pack(), 0, userState, 0, userData.packedSize);
            //coreState = core.pack();
            //userState = userData.pack();
            status = ZSystem.Instance.Status;
        }
        public void LoadGameState()
        {
            if (coreState.Count() == 0 || userState.Count() == 0)
            {
                return;
            }
            ZCore core = ZCore.Instance;
            ZUserData userData = ZUserData.Instance;
            core.unpack(coreState);
            userData.unpack(userState);
            ZSystem.Instance.Map.Cursor = core.MapId;
            ZSystem.Instance.Status = status;
            coreState = Array.Empty<byte>();
            userState = Array.Empty<byte>();
        }
    }
}
