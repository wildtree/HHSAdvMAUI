using HHSAdvMAUI.Resources.Styles;
using Microsoft.Win32;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static HHSAdvMAUI.ZProperties;

namespace HHSAdvMAUI
{
    internal class ZSystem
    {
        public enum GameStatus { Title = 0, Play = 1, GameOver = 2, }
        public GameStatus Status { get; set; } = GameStatus.Title;

        private static ZSystem? instance = null;
        public string dataFolder { get; private set; } = string.Empty;
        private ZMap? map = null;
        private ZRules? rules = null;
        private ZWords? dict = null;
        private ZObjects? objects = null;
        private ZMessage? messages = null;
        private ZAudio? audio = null;
        private ZProperties? properties = null;

        public static string DataFolder => FileSystem.AppDataDirectory;

        private class VersionAttributes
        {
            public int version { get; set; } = 0;
        }
        private VersionAttributes version = new VersionAttributes();
        public ZMap Map
        {
            get
            {
                return map!;
            }
        }
        public ZRules Rules
        {
            get
            {
                return rules!;
            }
        }
        public ZWords Dict
        {
            get
            {
                return dict!;
            }
        }
        public ZObjects Objects
        {
            get
            {
                return objects!;
            }
        }
        public ZMessage Messages
        {
            get
            {
                return messages!;
            }
        }
        public ZAudio Audio
        {
            get { return audio!; }
        }
        public ZProperties Properties
        {
            get
            {
                return properties!;
            }
        }
        public static ZSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ZSystem();
                }
                return instance;
            }
        }

        private bool darkMode = false;
        public bool DarkMode
        {
            get { return darkMode; }
            set
            {
                darkMode = value;
                Application.Current!.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(darkMode ? new DarkTheme() : new LightTheme());
            }
        }
        public static bool IsSystemInDarkMode
        {
            get
            {
                var theme = Application.Current!.RequestedTheme;
                return theme == AppTheme.Dark;
            }
        }
        private ZSystem()
        {
            dataFolder = FileSystem.AppDataDirectory;
            map = new ZMap("map.dat");
            rules = new ZRules("rule.dat");
            dict = new ZWords("highds.com");
            objects = new ZObjects("thin.dat");
            messages = new ZMessage("msg.dat");
            audio = new ZAudio(AudioManager.Current);
            properties = new ZProperties();
        }

        public void Init()
        {
            //Properties.Load(System.IO.Path.Combine(dataFolder, "HHSAdvMAUI.json"));
            bool dm = ((Properties.ThemeMode == ThemeType.System && IsSystemInDarkMode) || Properties.ThemeMode == ThemeType.Dark);
            DarkMode = dm;
            Status = GameStatus.Title;
            ZUserData.Instance.load("data.dat");
            map!.Cursor = 76;
        }
        public void Quit()
        {
            SavePreferences();
        }
        public void SavePreferences()
        {
            //Properties.Save(System.IO.Path.Combine(dataFolder, "HHSAdvWin.json"));
        }
        public void LoadPreferences()
        {
            //Properties.Load(System.IO.Path.Combine(dataFolder, "HHSAdvMAUI.json"));
        }
        private static async Task CopyAssetToAppDataAsync(string filename)
        {
            // アセットを開く
            using Stream input = await FileSystem.Current.OpenAppPackageFileAsync(filename);

            // 保存先パスを決定
            string targetFile = Path.Combine(DataFolder, filename);

            // コピー
            using FileStream output = File.Create(targetFile);
            await input.CopyToAsync(output, bufferSize: 65536);
            await output.FlushAsync();
        }

        private static async Task<string> ReadTextFileAsync(string filename)
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filename);
            using StreamReader reader = new StreamReader(fileStream);
            return await reader.ReadToEndAsync();
        }


        private static int GetFileVersion(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return 0;
            var deserializedAttributes = JsonSerializer.Deserialize<VersionAttributes>(jsonString);
            if (deserializedAttributes == null)
            {
                return 0;
            }
            var version = deserializedAttributes;
            return version.version;
        }
        public static async Task CopyAssetFilesAsync()
        {
            bool forceCopy = true;
            string destVersionFile = Path.Combine(ZSystem.DataFolder, "version.json");
            if (File.Exists(destVersionFile))
            {
                int destVersion = GetFileVersion(File.ReadAllText(destVersionFile));
                int srcVersion = GetFileVersion(await ReadTextFileAsync("version.json"));
                forceCopy = (destVersion < srcVersion);
            }

            foreach (var filename in new string[] { "map.dat", "rule.dat", "highds.com", "thin.dat", "msg.dat", "data.dat", "version.json" })
            {
                string targetFile = Path.Combine(ZSystem.DataFolder, filename);
                if (forceCopy || !File.Exists(targetFile))
                {
                    await ZSystem.CopyAssetToAppDataAsync(filename);
                }
            }
        }
    }
}

