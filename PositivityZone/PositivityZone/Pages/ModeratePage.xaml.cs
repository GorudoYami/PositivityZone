using PositivityZone.Pages;
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
    public partial class ModeratePage : ContentPage {
        private API.WebAPI api;
        private List<API.Models.Merged> mergedList;
        private string lang;
        private int index;
        private bool noEntries;
        private bool noAnswers;
        public ModeratePage(ref API.WebAPI _api) {
            api = _api;
            InitializeComponent();

            buttonNo.Clicked += ButtonNo_Clicked;
            buttonYes.Clicked += ButtonYes_Clicked;
            buttonSkip.Clicked += ButtonSkip_Clicked;
            buttonReturn.Clicked += ButtonReturn_Clicked;

            if (!Preferences.ContainsKey("FirstModerate")) {
                DisplayAlert(Strings.textRead, Strings.textModerateInfo, "OK");
                Preferences.Set("FirstModerate", false);
            }

            noEntries = false;
            noAnswers = false;
            index = 0;
            mergedList = new List<API.Models.Merged>();
            lang = Preferences.Get("Language", "en");
            RetranslateUI();
        }

        private async void ButtonReturn_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync();
        }

        private void ButtonSkip_Clicked(object sender, EventArgs e) {
            if (noEntries && noAnswers)
                return;

            if (index == mergedList.Count - 1)
                index = 0;
            else index++;

            if (mergedList[index].Answer != null) {
                textBox.Text = Strings.textAnswer + "\n" + mergedList[index].Answer.Text;
            }
            else {
                textBox.Text = Strings.textEntry + "\n" + mergedList[index].Entry.Text;
            }
        }

        private async void ButtonYes_Clicked(object sender, EventArgs e) {
            if (noEntries && noAnswers)
                return;

            bool result;
            if (mergedList[index].Answer != null)
                result = await api.Approve(mergedList[index].Answer);
            else
                result = await api.Approve(mergedList[index].Entry);

            if (result) {
                await DisplayAlert(Strings.textSuccess, Strings.textApproved, "OK");
                mergedList.RemoveAt(index);
                if (mergedList.Count == 0) {
                    textBox.Text = Strings.textModerate;
                    noEntries = true;
                    noAnswers = true;
                }
                else {
                    if (index == mergedList.Count - 1)
                        index = 0;
                    else index++;
                    if (mergedList[index].Answer != null) {
                        textBox.Text = Strings.textAnswer + "\n" + mergedList[index].Answer.Text;
                    }
                    else {
                        textBox.Text = Strings.textEntry + "\n" + mergedList[index].Entry.Text;
                    }
                }
            }
            else
                await DisplayAlert(Strings.textError, Strings.textModError, "OK");
        }

        private async void ButtonNo_Clicked(object sender, EventArgs e) {
            if (noEntries && noAnswers)
                return;

            bool result;
            if (mergedList[index].Answer != null)
                result = await api.Disapprove(mergedList[index].Answer);
            else
                result = await api.Disapprove(mergedList[index].Entry);

            if (result) {
                await DisplayAlert(Strings.textSuccess, Strings.textDisapproved, "OK");
                mergedList.RemoveAt(index);
                if (mergedList.Count == 0) {
                    textBox.Text = Strings.textModerate;
                    noEntries = true;
                    noAnswers = true;
                }
                else {
                    if (index == mergedList.Count - 1)
                        index = 0;
                    else index++;
                    if (mergedList[index].Answer != null)
                        textBox.Text = Strings.textAnswer + "\n" + mergedList[index].Answer.Text;
                    else
                        textBox.Text = Strings.textEntry + "\n" + mergedList[index].Entry.Text;
                }
            }
            else
                await DisplayAlert(Strings.textError, Strings.textModError, "OK");
        }

        override async protected void OnAppearing() {
            List<API.Models.Entry> entryList = await api.GetEntries(null, false, false, lang);
            List<API.Models.Answer> answerList = await api.GetAnswers(false, false, lang);

            if (entryList == null)
                noEntries = true;
            if (answerList == null)
                noAnswers = true;

            if (noAnswers && noEntries)
                textBox.Text = Strings.textModerate;

            if (!noEntries) {
                foreach (API.Models.Entry e in entryList)
                    mergedList.Add(new API.Models.Merged(e));
            }

            if (!noAnswers) {
                foreach (API.Models.Answer a in answerList)
                    mergedList.Add(new API.Models.Merged(a));
            }

            if (!noEntries || !noAnswers) {
                if (mergedList[index].Answer != null)
                    textBox.Text = Strings.textAnswer + "\n" + mergedList[index].Answer.Text;
                else
                    textBox.Text = Strings.textEntry + "\n" + mergedList[index].Entry.Text;
            }
        }

        private void RetranslateUI() {
            labelModerate.Text = Strings.labelModerate;
            buttonYes.Text = Strings.buttonYes;
            buttonNo.Text = Strings.buttonNo;
            buttonSkip.Text = Strings.buttonSkip;
            buttonReturn.Text = Strings.buttonReturn;
        }
    }
}