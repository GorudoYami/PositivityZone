using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PositivityZone {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VentPage : ContentPage {
        private API.WebAPI api;
        public VentPage(ref API.WebAPI _api) {
            api =_api;
            InitializeComponent();
            buttonSend.Clicked += ButtonSend_Clicked;
            buttonReturn.Clicked += ButtonReturn_Clicked;

            RetranslateUI();
            if(!Preferences.ContainsKey("FirstVent")) {
                DisplayAlert(Strings.textRead, Strings.textVentInfo, "OK");
                Preferences.Set("FirstVent", false);
            }
        }

        public async void ButtonSend_Clicked(object sender, EventArgs args) {
            API.Models.Entry entry = new API.Models.Entry {
                Text = textBox.Text,
                UID = api.UID,
                Language = Preferences.Get("Language", "en")
            };

            if (api.PostEntry(entry))
                await DisplayAlert("Success", "Your entry is waiting on answers!", "OK");
            else
                await DisplayAlert("Failed", "Your entry couldn't be added!", "OK");
            await Navigation.PopModalAsync();
        }

        public async void ButtonReturn_Clicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        private void RetranslateUI() {
            labelVent.Text = Strings.labelVent;
            buttonSend.Text = Strings.buttonSend;
            buttonReturn.Text = Strings.buttonReturn;
        }
    }
}