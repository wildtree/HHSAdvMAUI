using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    class ZRoll
    {
        public List<string> Credits { get; private set; } = new List<string>();
        public Grid? Overlay { get; set; }
        public VerticalStackLayout? Stack { get; set; }
        public AbsoluteLayout? Stage { get; set; }
        public double Duration { get; set; } = 20.0; // seconds
        private double scrollHeight = 0d;
        public ZRoll() { }
        public async Task LoadCredits(string fileName)
        {
            using (var fs = await FileSystem.OpenAppPackageFileAsync(fileName))
            {
                Credits.Clear();
                Stack!.Children.Clear();
                scrollHeight = 0d;
                using (var sr = new StreamReader(fs))
                {
                    string? line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        Credits.Add(line);
                        Stack!.Children.Add(new Label
                        {
                            Text = line,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            FontSize = 16,
                            TextColor = Colors.White,
                        });
                    }
                }
            }
            Stack.InvalidateMeasure();
        }
        public async Task ShowCredits(Action? onCompleted)
        {
            Stage!.Opacity = 0.0;
            Overlay!.IsVisible = true;
            Stage!.Dispatcher.Dispatch(async () =>
            {
                DeviceDisplay.Current.KeepScreenOn = true;
                // 子を追加した後に高さを測定
                var size = Stack!.Measure(double.PositiveInfinity, double.PositiveInfinity);
                while (size.Height <= 0 || Stage!.Height <= 0)
                {
                    await Task.Delay(16);
                    size = Stack.Measure(double.PositiveInfinity, double.PositiveInfinity);
                }
                // 初期位置を画面下に配置
                AbsoluteLayout.SetLayoutBounds(Stack, new Rect(0, 0, Stage.Width, size.Height));
                AbsoluteLayout.SetLayoutFlags(Stack, AbsoluteLayoutFlags.None);
                Stack.TranslationY = Stage.Height;
                Stage.Opacity = 1.0;
                Stack.Opacity = 1.0;

                // 上方向にアニメーション
                await Stack.TranslateTo(0, -size.Height, (uint)(Duration * 1000.0), Easing.Linear);

                DeviceDisplay.Current.KeepScreenOn = false;
                // スクロール完了後にコールバック実行
                onCompleted?.Invoke();

                Overlay.IsVisible = false;
                Stack.Children.Clear();
                Stage.SizeChanged -= null;
            });
        }
    }
}
