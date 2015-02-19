using System.Windows.Input;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Core;

namespace LeagueSharp.Loader.Model.Settings
{
    public class HotkeyEntry : ObservableObject
    {
        private string _description;
        private Key _hotkey;
        private string _name;
        public Key DefaultKey { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("DisplayDescription");
                }
            }
        }

        public string DisplayDescription
        {
            get { return Utility.GetMultiLanguageText(Description); }
        }

        public Key Hotkey
        {
            get { return _hotkey; }
            set
            {
                if (_hotkey != value)
                {
                    _hotkey = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("HotkeyInt");
                    RaisePropertyChanged("HotkeyString");
                }
            }
        }

        public byte HotkeyInt
        {
            get
            {
                if (Hotkey == Key.LeftShift || Hotkey == Key.RightShift)
                {
                    return 16;
                }
                if (Hotkey == Key.LeftAlt || Hotkey == Key.RightAlt)
                {
                    return 0x12;
                }
                if (Hotkey == Key.LeftCtrl || Hotkey == Key.RightCtrl)
                {
                    return 0x11;
                }
                return (byte) KeyInterop.VirtualKeyFromKey(Hotkey);
            }
        }

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }
    }
}