namespace UstbBox.Wpf.Services
{
    public class SettingService
    {
        private SettingService()
        {
        }

        public static SettingService Instance { get; } = new SettingService();

        public string AccentColor
        {
            get
            {
                return Settings.Default.AccentColor;
            }

            set
            {
                Settings.Default.AccentColor = value;
                Settings.Default.Save();
            }
        }

        public bool IsDark
        {
            get
            {
                return Settings.Default.IsDark;
            }

            set
            {
                Settings.Default.IsDark = value;
                Settings.Default.Save();
            }
        }

        public string PrimaryColor
        {
            get
            {
                return Settings.Default.PrimaryColor;
            }

            set
            {
                Settings.Default.PrimaryColor = value;
                Settings.Default.Save();
            }
        }

        public void ResetSettings()
        {
            Settings.Default.Reset();
            Settings.Default.Reload();
        }
    }
}
