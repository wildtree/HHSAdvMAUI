using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HHSAdvMAUI
{

    internal class AboutPageModel : ObservableObject
    {
        private string aboutText = string.Empty;
        public string AboutText
        {
            get => aboutText;
            set
            {
                if (aboutText != value)
                {
                    aboutText = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }
        private string icon;
        public string Icon
        {
            get => icon;
            set
            {
                if (icon != value)
                {
                    icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }
        public AboutPageModel()
        {
            Icon = "isako.png";
            var appVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
            AboutText = $@"High High School Adventure
{appVersion}

PalmOS version: hiro © 2002-2004
Android version: hiro © 2011-2025
Web version: hiro © 2012-2024
M5 version: hiro © 2023-2025
Qt version: hiro © 2024-2025
PicoCalc version: hiro © 2025
SDL version: hiro © 2025
Windows version: hiro © 2025
AvaloniaUI version: hiro © 2025
.NET MAUI version: hiro © 2025

- Project ZOBPlus -
Hayami <hayami@zob.jp>
Exit <exit@zob.jp>
ezumi <ezumi@zob.jp>
Ogu <ogu@zob.jp>
neopara <neopara@zob.jp>
hiro <hiro@zob.jp>

--- Original Staff ---
Directed By HIRONOBU NAKAGUCHI

- Graphic Designers -

NOBUKO YANAGITA, YUMIKO HOSONO, HIRONOBU NAKAGUCHI, TOSHIHIKO YANAGITA, TOHRU OHYAMA
MASANORI ISHII, YASUSHI SHIGEHARA, HIDETOSHI SUZUKI, TATSUYA UCHIBORI, MASAKI NOZAWA
TOMOKO OHKAWA, FUMIKAZU SHIRATSUCHI, YASUNORI YAMADA, MUNENORI TAKIMOTO

- Message Converters -
TATSUYA UCHIBORI, HIDETOSHI SUZUKI, YASUSHI SHIGEHARA, YASUNORI YAMADA

- Floppy Disk Converters -
HIRONOBU NAKAGUCHI

- Music -
MASAO MIZOBE

- Special Thanks To -
HIROSHI YAMAMOTO, TAKAYOSHI KASHIWAGI

- Cooperate with -
Furniture KASHIWAGI

ZAMA HIGH SCHOOL MICRO COMPUTER CIRCLE";
        }
    }
}
