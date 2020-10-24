using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PositivityZone.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserEntryPage : ContentPage {
        public UserEntryPage(string vent, string answer) {
            InitializeComponent();
            buttonBack.Clicked += ButtonBack_Clicked;
            textBoxAnswer.Text = answer;
            textBoxVent.Text = vent;
            labelVent.Text = Strings.labelVent2;
            labelAnswer.Text = Strings.textAnswer;
        }

        private async void ButtonBack_Clicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }
    }
}