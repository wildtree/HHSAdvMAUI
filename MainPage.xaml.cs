using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Text;
using static HHSAdvMAUI.ZSystem;

namespace HHSAdvMAUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private Canvas canvas;
        private ZSystem zsystem;

        private const string CRLF = "\r\n";

        private static string[] title = new string[]
        {
            @"ハイハイスクールアドベンチャー",
            @"Copyright(c)1995-2025",
            @"ZOBplus",
            @"hiro",
        };
        public MainPage()
        {
            InitializeComponent();
            canvas = new Canvas();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object? sender, EventArgs e)
        {
            await ZSystem.CopyAssetFilesAsync();
            zsystem = ZSystem.Instance;
            zsystem.Init();
            zsystem.LoadPreferences();
            LogArea.SizeChanged += (s, e) =>
            {
                ScrollArea.ScrollToAsync(0, ScrollArea.ContentSize.Height, false);
            };
            MainPageModel.Instance.LoadGameState(); // restore if necessary
            BindingContext = MainPageModel.Instance;
            switch (zsystem.Status)
            {
                case GameStatus.Title:
                    TitleScreen();
                    break;
                case GameStatus.Play:
                    DrawScreen(false);
                    GameScreen();
                    break;
                case GameStatus.GameOver:
                    DrawScreen(false);
                    GameOver();
                    break;
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var image = e.Surface.Canvas;
            image.ResetMatrix();
            image.Clear(SKColors.Black);
            float scaleX = (float)e.Info.Width / (float)canvas.Bitmap.Width;
            float scaleY = (float)e.Info.Height / (float)canvas.Bitmap.Height;
            float scale = Math.Min(scaleX, scaleY);
            float offsetX = (e.Info.Width - canvas.Bitmap.Width * scale) / 2f;
            float offsetY = (e.Info.Height - canvas.Bitmap.Height * scale) / 2f;
            float w = canvas.Bitmap.Width * scale;
            float h = canvas.Bitmap.Height * scale;
            var imgRect = new SKRect(left: offsetX, top: offsetY, right: w + offsetX, bottom: h + offsetY);
            image.DrawBitmap(canvas.Bitmap, imgRect);
        }

        private void GameOver()
        {
            zsystem.Status = ZSystem.GameStatus.GameOver;
            InputTextBox.IsVisible = false;
            GameStartButton.IsVisible = true;
            GameStartButton.IsEnabled = true;
            GameStartButton.Text = "タイトルへ戻る";
        }

        private bool IsDark()
        {
            bool dim = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            switch (core.MapId)
            {
                case 47:
                case 48:
                case 49:
                case 61:
                case 64:
                case 65:
                case 67:
                case 68:
                case 69:
                case 71:
                case 74:
                case 75:
                case 77:
                    if (user.getFact(7) != 0)
                    {
                        if (user.getFact(6) != 0)
                        {
                            // dark mode (blue)
                            dim = true;
                        }
                    }
                    else
                    {
                        // blackout
                        core.MapViewId = core.MapId;
                        zsystem.Map.Cursor = 84;
                    }
                    break;
                default:
                    if (user.getFact(6) != 0)
                    {
                        dim = false;
                    }
                    break;
            }
            return dim;
        }
        public void TimeElapsed()
        {
            ZUserData user = ZUserData.Instance;
            ZMessage messages = zsystem.Messages;

            if (user.getFact(3) > 0 && user.getFact(7) == 1)
            {
                // Light is ON
                user.setFact(3, (byte)(user.getFact(3) - 1));
                if (user.getFact(3) < 8 && user.getFact(3) > 0)
                {
                    // battery LOW
                    user.setFact(6, 1); // dim mode
                    MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xd9));
                }
                else if (user.getFact(3) == 0)
                {
                    // battery ware out
                    user.setFact(7, 0); // light off
                    MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xc0));
                }
            }
            if (user.getFact(11) > 0)
            {
                user.setFact(11, (byte)(user.getFact(11) - 1));
                if (user.getFact(11) == 0)
                {
                    MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xd8));
                    if (user.getPlace(7) == 48)
                    {
                        user.getLink(75 - 1).N = 77;
                        user.getLink(68 - 1).W = 77;
                        MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xda));
                    }
                    else if (user.getPlace(7) == 255 || user.getPlace(7) == zsystem.Map.Cursor)
                    {
                        // suicide explosion
                        // set screen color to red
                        canvas!.ColorFilterType = Canvas.FilterType.Red;
                        MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xcf));
                        MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xcb));
                        GameOver();
                    }
                    else
                    {
                        user.setPlace(7, 0);
                    }
                }
            }
        }
        private void CheckTeacher()
        {
            ZUserData user = ZUserData.Instance;
            ZCore core = ZCore.Instance;
            if (zsystem.Status == GameStatus.GameOver || user.getFact(1) == core.MapId)
                return;
            int rd = 100 + core.MapId + ((user.getFact(1) > 0) ? 1000 : 0);
            int rz = new Random().Next(3000);
            user.setFact(1, (byte)((rd < rz) ? 0 : core.MapId));
            switch (core.MapId)
            {
                case 1:
                case 48:
                case 50:
                case 51:
                case 52:
                case 53:
                case 61:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 83:
                case 86:
                    user.setFact(1, 0);
                    break;
            }
        }
        private void SaveGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = System.IO.Path.Combine(zsystem.dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var br = new BinaryWriter(fs))
                {
                    br.Write(core.pack());
                    br.Write(user.pack());
                }
            }
        }
        private void LoadGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = System.IO.Path.Combine(zsystem.dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    core.unpack(br.ReadBytes(core.packedSize));
                    user.unpack(br.ReadBytes(user.packedSize));
                }
            }
            zsystem.Map.Cursor = core.MapId;
        }

        private async Task ExecuteRules()
        {
            bool okay = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            ZMessage messages = zsystem.Messages;
            ZMap map = zsystem.Map;
            ZProperties properties = zsystem.Properties;
            foreach (var rule in zsystem.Rules.Rules)
            {
                if (rule.Evaluate())
                {
                    map.Cursor = core.MapId;
                    ZCore.ZCommand c = new ZCore.ZCommand();
                    ZAudio audio = zsystem.Audio;
                    while ((c = core.pop()).Cmd != ZCore.ZCommand.Command.Nop)
                    {
                        byte o = c.Operand;
                        switch (c.Cmd)
                        {
                            case ZCore.ZCommand.Command.Nop:
                                break;
                            case ZCore.ZCommand.Command.Message:
                                string s = messages.GetMessage(o);
                                if ((o & 0x80) == 0)
                                {
                                    s = map.Find(core.CmdId, core.ObjId);
                                }
                                MainPageModel.Instance.AppendLogLine(s);
                                break;
                            case ZCore.ZCommand.Command.Sound:
                                if (properties.PlaySound)
                                    await audio!.Play(o);
                                break;
                            case ZCore.ZCommand.Command.Dialog:
                                switch (o)
                                {
                                    case 0: // boy or girl
                                        user.setFact(0, 1); // boy
                                        {
                                            string gender = await DisplayActionSheet(messages.GetMessage(0xe7), null, null, "男子", "女子");
                                            byte selected = gender switch
                                            {
                                                "男子" => 1,
                                                "女子" => 2,
                                                _ => 0,
                                            };
                                            user.setFact(0, selected);
                                            map!.Cursor = 3;
                                        }
                                        break;
                                    case 1:
                                        List<string> list = new List<string>();
                                        list.Add("1");
                                        list.Add("2");
                                        list.Add("3");
                                        string title = "セーブ";
                                        if (core.CmdId != 0x0f)
                                        {
                                            list.Clear();
                                            title = "ロード";
                                            if (File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "1.dat")))
                                            {
                                                list.Add("1");
                                            }
                                            if (File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "2.dat")))
                                            {
                                                list.Add("2");
                                            }
                                            if (File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "3.dat")))
                                            {
                                                list.Add("3");
                                            }
                                            if (list.Count == 0)
                                            {
                                                await DisplayAlert("情報", "セーブデータが存在していません。", "了解");
                                                break;
                                            }
                                        }
                                        {
                                            string fileNo = await DisplayActionSheet(title, null, null, list.ToArray());
                                            if (core.CmdId != 0x0f)
                                            {
                                                if (string.IsNullOrEmpty(fileNo) || !int.TryParse(fileNo, out int n))
                                                {
                                                    break;
                                                }
                                                LoadGame(n);
                                                break;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(fileNo) || !int.TryParse(fileNo, out int n))
                                                {
                                                    break;
                                                }
                                                SaveGame(n);
                                                MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xef));
                                                break;
                                            }
                                        }
                                    case 2:
                                        await DisplayAlert("持物", user.getItemList().Length == 0 ? "持物はありません。" : user.getItemList(), "了解");
                                        break;
                                    case 3:
                                        {
                                            string cable = await DisplayActionSheet(messages.GetMessage(0xe9), null, null, "黄", "赤");
                                            byte selected = cable switch
                                            {
                                                "黄" => 1,
                                                "赤" => 2,
                                                _ => 0,
                                            };
                                            if (user.getPlace(11) != 0xff)
                                            {
                                                MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xe0));
                                            }
                                            if (selected == 1 || user.getPlace(11) != 0xff)
                                            {
                                                canvas.ColorFilterType = Canvas.FilterType.Red;
                                                MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xc7));
                                                MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xee));
                                                GameOver();
                                                break;
                                            }
                                            user.setPlace(11, 0);
                                            map.Cursor = 74;
                                        }
                                        break;
                                }
                                break;
                            case ZCore.ZCommand.Command.GameOver:
                                switch (o)
                                {
                                    case 1: // teacher
                                        canvas!.ColorFilterType = Canvas.FilterType.Sepia;
                                        DrawScreen(false);
                                        break;
                                    case 2: // explosion
                                        canvas!.ColorFilterType = Canvas.FilterType.Red;
                                        DrawScreen(false);
                                        break;
                                    case 3: // clear
                                        if (properties.PlaySound)
                                            audio!.Play(0);
                                        ZRoll endroll = new ZRoll()
                                        {
                                            Duration = 35.0,
                                            Overlay = CreditOverlay,
                                            Stage = CreditStage,
                                            Stack = CreditStack
                                        };
                                        await endroll.LoadCredits("credits.txt");
                                        Dispatcher.Dispatch(async () =>
                                        {
                                            await Task.Yield();
                                            await endroll.ShowCredits(() =>
                                            {
                                                DrawScreen(true);
                                            });
                                        });
                                        break;
                                }
                                GameOver();
                                break;
                        }
                    }
                    if (zsystem.Status == GameStatus.GameOver) return;
                    MainPageModel.Instance.AppendLogLine(messages.GetMessage(0xed)); // Ｏ．Ｋ．
                    okay = true;
                    break;
                }
            }
            map!.Cursor = core.MapId;
            if (!okay)
            {
                string s = map.Find(core.CmdId, core.ObjId);
                if (string.IsNullOrEmpty(s))
                {
                    s = messages.GetMessage(0xec); // ダメ
                }
                MainPageModel.Instance.AppendLogLine(s);
            }
            if (map.Cursor == 74)
            {
                int msg_id = 0;
                user.setFact(13, (byte)(user.getFact(13) + 1));
                msg_id = user.getFact(13) switch
                {
                    1 => 0xe1,
                    2 => 0xe5,
                    3 => 0xe6,
                    _ => msg_id,
                };
                if (msg_id != 0)
                {
                    MainPageModel.Instance.AppendLogLine(messages.GetMessage(msg_id));
                }
            }
        }

        private void DrawScreen(bool drawMessage = true)
        {
            if (canvas != null)
            {
                ZMap map = zsystem.Map;
                if (canvas.ColorFilterType == Canvas.FilterType.Blue)
                {
                    canvas.ColorFilterType = Canvas.FilterType.None;
                }
                if (IsDark())
                {
                    canvas.ColorFilterType = Canvas.FilterType.Blue;
                }
                map.Draw(canvas);
                if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                {
                    string s = map.Message;
                    if (map.IsBlank)
                    {
                        MainPageModel.Instance.AppendLogLine(zsystem.Messages.GetMessage(0xcc));
                    }
                    if (!string.IsNullOrEmpty(s))
                    {
                        MainPageModel.Instance.AppendLogLine(s);
                    }
                }
                ZUserData user = ZUserData.Instance;
                ZObjects obj = zsystem.Objects;
                for (int i = 0; i < ZUserData.Items; i++)
                {
                    if (user.getPlace(i) == map.Cursor)
                    {
                        bool shift = (i == 1 && user.getFact(0) != 1);

                        obj.Id = i;
                        obj.Draw(canvas, shift);
                        if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                        {
                            MainPageModel.Instance.AppendLogLine(zsystem.Messages.GetMessage(0x96 + i));
                        }
                    }
                }
                if (user.getFact(1) == map.Cursor)
                {
                    obj.Id = 14;
                    obj.Draw(canvas);
                    if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                    {
                        MainPageModel.Instance.AppendLogLine(zsystem.Messages.GetMessage(0xb4));
                    }

                }
                canvas.colorFilter();
                Image.InvalidateSurface();
            }
        }

        private void TitleScreen()
        {
            //System.Windows.Application.Current.MainWindow.PreviewKeyDown += Application_PreviewKeyDown;
            InputTextBox.IsVisible = false;
            InputTextBox.IsEnabled = false;
            GameStartButton.IsVisible = true;
            GameStartButton.IsEnabled = true;
            GameStartButton.Text = "ゲーム開始";
            zsystem.Init();
            MainPageModel.Instance.ClearLog(); // clear log
            canvas.ColorFilterType = Canvas.FilterType.None;
            foreach (var s in title)
            {
                MainPageModel.Instance.AppendLogLine(s);
            }
            DrawScreen(false);
        }
        private void GameScreen()
        {
            InputTextBox.IsVisible = true;
            InputTextBox.IsEnabled = true;
            GameStartButton.IsVisible = false;
            GameStartButton.IsEnabled = false;
            InputTextBox.Focus();
        }

        private async void OnInputCompleted(object sender, EventArgs e)
        {
            string inputText = InputTextBox.Text!.Trim();
            if (string.IsNullOrEmpty(inputText)) return;

            string[] args = inputText.Split(new char[] { ' ' });
            var cmd = args[0].Trim();
            var obj = (args.Length > 1) ? args[1].Trim() : string.Empty;
            ZCore.Instance.CmdId = (byte)zsystem.Dict.findVerb(cmd);
            ZCore.Instance.ObjId = (byte)zsystem.Dict.findObj(obj);
            // call game interpreter
            InputTextBox.Text = string.Empty;
            MainPageModel.Instance.AppendLog(">> ");
            MainPageModel.Instance.AppendLogLine(inputText);
            //
            TimeElapsed();
            if (zsystem.Status == ZSystem.GameStatus.GameOver) return;
            await ExecuteRules();
            if (zsystem.Status == ZSystem.GameStatus.GameOver) return;
            CheckTeacher();
            DrawScreen(true);
            InputTextBox.Focus();
        }

        private async void GameStartButton_Clicked(object sender, EventArgs e)
        {
            if (zsystem.Status == ZSystem.GameStatus.GameOver)
            {
                GameStartButton.Text = "ゲーム開始";
                zsystem.Status = ZSystem.GameStatus.Title;
                TitleScreen();
                return;
            }
            EventHandler h = (s, e) =>
            {
                zsystem.Map.Cursor = 1;
                MainPageModel.Instance.ClearLog();
                zsystem.Status = ZSystem.GameStatus.Play;
                DrawScreen(true);
                GameStartButton.IsVisible = false;
                InputTextBox.IsVisible = true;
                InputTextBox.IsEnabled = true;
                InputTextBox.Focus();
            };
            if (zsystem.Properties.OpeningRoll)
            {
                ZRoll openingRoll = new ZRoll()
                {
                    Duration = 20.0,
                    Overlay = CreditOverlay,
                    Stage = CreditStage,
                    Stack = CreditStack
                };
                await openingRoll.LoadCredits("opening.txt");
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Yield();
                    await openingRoll.ShowCredits(() =>
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            h.Invoke(null, EventArgs.Empty);
                        });
                    });
                });
            }
            else
            {
                h.Invoke(null, EventArgs.Empty);
            }
        }
    }
}


