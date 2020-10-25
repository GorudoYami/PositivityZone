using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PositivityZone.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionsPage : ContentPage {

        public OptionsPage() {
            InitializeComponent();
            buttonBack.Clicked += ButtonBack_Clicked;
            buttonLangEN.Clicked += ButtonLangEN_Clicked;
            buttonLangPL.Clicked += ButtonLangPL_Clicked;

            string lang = Preferences.Get("Language", "en");
            if (lang == "pl") {
                buttonLangPL.BackgroundColor = Color.FromRgba(0, 0, 0, 0.25);
            }
            else {
                buttonLangEN.BackgroundColor = Color.FromRgba(0, 0, 0, 0.25);
            }
            RetranslateUI();
        }

        private void ButtonLangPL_Clicked(object sender, EventArgs e) {
            buttonLangEN.BackgroundColor = Color.FromRgba(255, 255, 255, 0);
            buttonLangPL.BackgroundColor = Color.FromRgba(0, 0, 0, 0.25);
            CultureInfo.CurrentUICulture = new CultureInfo("pl");
            Preferences.Set("Language", "pl");
            RetranslateUI();
        }

        private void ButtonLangEN_Clicked(object sender, EventArgs e) {
            buttonLangPL.BackgroundColor = Color.FromRgba(255, 255, 255, 0);
            buttonLangEN.BackgroundColor = Color.FromRgba(0, 0, 0, 0.25);
            CultureInfo.CurrentUICulture = new CultureInfo("en");
            Preferences.Set("Language", "en");
            RetranslateUI();
        }

        private async void ButtonBack_Clicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        private void RetranslateUI() {
            labelOptions.Text = Strings.labelOptions;
            labelLanguage.Text = Strings.labelLanguage;
            labelVersion.Text = Strings.labelVersion + "0.0.1";
            labelBG.Text = "Background and icon:\nhttps://pinterest.com/pin/726486983632169506/\nOnly for educational purpouses.";
        }
    }
}