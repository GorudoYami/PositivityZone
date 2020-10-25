using PositivityZone.CustomControls;
using PositivityZone.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace PositivityZone.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage {
        private API.WebAPI api;
        private List<API.Models.Entry> entryList;
        public UserPage(ref API.WebAPI _api) {
            api = _api;
            InitializeComponent();
            buttonBack.Clicked += ButtonBack_Clicked;
            buttonInfo.Clicked += ButtonInfo_Clicked;
            buttonRecover.Clicked += ButtonRecover_Clicked;
            buttonPassword.Clicked += ButtonPassword_Clicked;
            labelUID.Text = "UID: " + Preferences.Get("UID", "N/A");
            RetranslateUI();
        }

        private async void ButtonPassword_Clicked(object sender, EventArgs e) {
            await DisplayAlert("Sorry!", "This feature is not yet impemented!", "OK");
        }

        override protected async void OnAppearing() {
            stackLayout.Children.Clear();
            entryList = await api.GetUserEntries();
            if (entryList != null) {
                int index = 0;
                foreach (API.Models.Entry e in entryList) {
                    CustomButton button = new CustomButton() {
                        Text = Strings.textEntry + e.ID + " " + Strings.textAnswered + (e.Answered ? Strings.textYes : Strings.textNo),
                        EntryID = index++
                    };
                    button.Clicked += Button_Clicked;
                    stackLayout.Children.Add(button);
                }
            }
            else {
                stackLayout.Children.Add(new Label() {
                    Text = Strings.labelNoUserEntries
                });
            }
        }

        private async void Button_Clicked(object sender, EventArgs e) {
            CustomButton button = (CustomButton)sender;
            API.Models.Answer answer = await api.GetAnswer(entryList[button.EntryID].ID);
            await Navigation.PushModalAsync(new UserEntryPage(entryList[button.EntryID].Text, answer.Text));
        }

        private async void ButtonInfo_Clicked(object sender, EventArgs e) {
            await DisplayAlert("Positivity Zone", Strings.textRules, "OK");
        }

        private async void ButtonRecover_Clicked(object sender, EventArgs e) {
            await DisplayAlert("Sorry!", "This feature is not yet impemented!", "OK");
            //await Navigation.PushModalAsync(new RecoveryDialog(ref api));
        }

        private async void ButtonBack_Clicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        private void RetranslateUI() {
            labelUser.Text = Strings.labelUser;
            labelUIDInfo.Text = Strings.labelUIDInfo;
            buttonPassword.Text = Strings.buttonPassword1;
            buttonRecover.Text = Strings.buttonRecover;
            labelUID.Text = api.UID;
        }
    }
}