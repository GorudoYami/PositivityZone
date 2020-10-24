using PositivityZone.API;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PositivityZone {
    public partial class App : Application {
        private API.WebAPI api;
        public App() {
            // Set language
            string lang = Preferences.Get("Language", "en");
            CultureInfo.CurrentUICulture = new CultureInfo(lang);
            api = new API.WebAPI();
            InitializeComponent();
            MainPage = new MainPage(ref api);
        }

        protected override async void OnStart() {
            Current.RequestedThemeChanged += (obj, args) => {
                Current.UserAppTheme = OSAppTheme.Light;
            };
            Current.UserAppTheme = OSAppTheme.Light;

            // Status check
            await CheckStatus();

            if (!Preferences.ContainsKey("UID"))
                Preferences.Set("UID", Guid.NewGuid().ToString());
            else
                api.UID = Preferences.Get("UID", string.Empty);

            api.PostUID();

            if (!Preferences.ContainsKey("Language"))
                Preferences.Set("Language", "en");
        }

        protected override void OnSleep() {

        }

        protected override async void OnResume() {
            await CheckStatus();
        }

        private async Task CheckStatus() {
            switch (await api.GetStatus()) {
                case Status.Ok:
                    break;
                case Status.Development:
                    await MainPage.DisplayAlert("Warning", "The application is in development mode!", "Sure thing.");
                    break;
                case Status.Maintenance:
                    await MainPage.DisplayAlert("Maintenance", "The application is in maintenance! Please come back later.", "OK");
                    Environment.Exit(0);
                    break;
                case Status.NoInternet:
                    await MainPage.DisplayAlert("Error", "There are no available connections to the internet. The app can't work without it.", "OK");
                    Environment.Exit(0);
                    break;
                case Status.ConnectionError:
                    await MainPage.DisplayAlert("Error", "Connection error. Your internet might be too slow or an unexpected error occured.", "OK");
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
