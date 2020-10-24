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
    public partial class AnswerPage : ContentPage {
        private readonly API.WebAPI api;
        private List<API.Models.Entry> entryList;
        private int index;
        private readonly string lang;
        private bool noEntries;
        public AnswerPage(ref API.WebAPI _api) {
            api = _api;
            InitializeComponent();
            buttonReturn.Clicked += ButtonReturn_Clicked;
            buttonSkip.Clicked += ButtonSkip_Clicked;
            buttonAnswer.Clicked += ButtonAnswer_Clicked;

            if (!Preferences.ContainsKey("FirstAnswer")) {
                DisplayAlert(Strings.textRead, Strings.textAnswerInfo, "OK");
                Preferences.Set("FirstAnswer", false);
            }

            lang = Preferences.Get("Language", "en");
            RetranslateUI();
        }

        override async protected void OnAppearing() {
            entryList = await api.GetEntries(false, true, false, lang);
            if (entryList == null || entryList.Count == 0) {
                noEntries = true;
                textBoxVent.Text = Strings.textNoVents;
            }
            else {
                index = 0;
                textBoxVent.Text = entryList[index].Text;
            }
        }

        private async void ButtonAnswer_Clicked(object sender, EventArgs e) {
            if (noEntries)
                return;
            API.Models.Answer answer = new API.Models.Answer {
                Approved = false,
                EntryID = entryList[index].ID,
                Text = textBoxAnswer.Text,
                Language = lang
            };
            if (!await api.PostAnswer(answer))
                await DisplayAlert(Strings.textError, Strings.textAnswerError, "OK");
            else
                await DisplayAlert(Strings.textSuccess, Strings.textAnswerSuccess, "OK");
            entryList.RemoveAt(index);

            if (index == entryList.Count - 1)
                index = 0;

            if (entryList.Count == 0) {
                noEntries = true;
                textBoxVent.Text = Strings.textNoVents;
            }
            else
                textBoxVent.Text = entryList[index].Text;
        }

        private void ButtonSkip_Clicked(object sender, EventArgs e) {
            if (noEntries)
                return;

            if (index == entryList.Count - 1)
                index = 0;
            else
                index++;
            textBoxVent.Text = entryList[index].Text;
        }

        private async void ButtonReturn_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync();
        }

        private void RetranslateUI() {
            buttonAnswer.Text = Strings.buttonAnswer;
            buttonReturn.Text = Strings.buttonReturn;
            buttonSkip.Text = Strings.buttonSkip;
            labelAnswer.Text = Strings.labelAnswer;
            labelVent.Text = Strings.labelVent2;
        }
    }
}