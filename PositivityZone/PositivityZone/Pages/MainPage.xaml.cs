using PositivityZone.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace PositivityZone {
    public partial class MainPage : ContentPage {
        private API.WebAPI api;

        public MainPage(ref API.WebAPI _api) {
            InitializeComponent();

            api = _api;

            buttonVent.Clicked += ButtonVent_Clicked;
            buttonAnswer.Clicked += ButtonAnswer_Clicked;
            buttonUser.Clicked += ButtonUser_Clicked;
            buttonOptions.Clicked += ButtonOptions_Clicked;
            buttonModerate.Clicked += ButtonModerate_Clicked;

            // Doesn't seem to work.
            Application.Current.RequestedThemeChanged += (obj, args) => {
                Application.Current.UserAppTheme = OSAppTheme.Light;
            };
            Application.Current.UserAppTheme = OSAppTheme.Light;

            if (!Preferences.ContainsKey("FirstLaunch")) {
                Preferences.Set("FirstLaunch", false);
                DisplayAlert("Positivity Zone", Strings.textFirstLaunch, "OK");
            }

            RetranslateUI();
        }

        private async void ButtonVent_Clicked(object sender, EventArgs args) {
            await Navigation.PushModalAsync(new VentPage(ref api));
        }

        private async void ButtonAnswer_Clicked(object sender, EventArgs args) {
            await Navigation.PushModalAsync(new AnswerPage(ref api));
        }

        private async void ButtonModerate_Clicked(object sender, EventArgs args) {
            await Navigation.PushModalAsync(new ModeratePage(ref api));
        }
        private async void ButtonOptions_Clicked(object sender, EventArgs args) {
            await Navigation.PushModalAsync(new OptionsPage());
        }
        private async void ButtonUser_Clicked(object sender, EventArgs args) {
            await Navigation.PushModalAsync(new UserPage(ref api));
        }

        override protected void OnAppearing() {
            RetranslateUI();
        }

        private void RetranslateUI() {
            labelTitle.Text = Strings.labelTitle;
            buttonVent.Text = Strings.buttonVent;
            buttonAnswer.Text = Strings.buttonAnswer;
            buttonModerate.Text = Strings.buttonModerate;
        }
    }
}
