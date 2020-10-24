using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace PositivityZone.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecoveryDialog : ContentPage
    {
        private API.WebAPI api;
        private bool isPassworded = false;

        public RecoveryDialog(ref API.WebAPI _api)
        {
            api = _api;
            InitializeComponent();
            CheckIfPassworded();
            confirmSetPassword.Clicked += ConfirmSetPassword_Clicked;
            confirmRecoveryDataButton.Clicked += ConfirmRecoveryDataButton_Clicked;
        }

        private async void ConfirmRecoveryDataButton_Clicked(object sender, EventArgs e)
        {
            if (!api.Recover(inputUID.Text, inputPassword.Text))
                await DisplayAlert("Error", "Wrong login credentials!", "OK");
            await Navigation.PopModalAsync();
        }

        private void ConfirmSetPassword_Clicked(object sender, EventArgs e)
        {
            if (isPassworded)
            {
                if (firstPass.Text == secondPass.Text)
                    if (api.ChangePass(previousPass.Text, firstPass.Text))
                    {
                        labelSetPassNotification.TextColor = Color.FromHex("#00FF00");
                        labelSetPassNotification.Text = "Password Changed Succesfully!";
                        confirmSetPassword.IsEnabled = false;
                    }
                    else
                    {
                        labelSetPassNotification.Text = "Wrong Previous Password!";
                    }
                else {
                    labelSetPassNotification.Text = "Passwords don't match!";
                }
            }
            else
            {
                if (firstPass.Text == secondPass.Text)
                    if (api.SetPass(firstPass.Text))
                    {
                        labelSetPassNotification.TextColor = Color.FromHex("#00FF00");
                        labelSetPassNotification.Text = "Password Set Succesfully!";
                        confirmSetPassword.IsEnabled = false;
                    }
                    else {
                        labelSetPassNotification.TextColor = Color.FromHex("#FC1B1B");
                        labelSetPassNotification.Text = "Couldn't set password. Try again later.";
                        confirmSetPassword.IsEnabled = false;
                    }
                else
                {
                    labelSetPassNotification.TextColor = Color.FromHex("#FC1B1B");
                    labelSetPassNotification.Text = "Passwords don't match!";
                }
            }
        }

        private void CheckIfPassworded()
        {
            if (api.HasPass()){
                isPassworded = true;
                labelSetPassNotification.Text = "You have already assigned a password to this UID";
                labelSetPassNotification.TextColor = Color.FromHex("#FC1B1B");
                previousPassLabel.IsVisible = true;
                previousPass.IsVisible = true;
                Grid.SetRow(firstPassLabel, Grid.GetRow(firstPassLabel)+1);
                Grid.SetRow(firstPass, Grid.GetRow(firstPass) + 1);
                Grid.SetRow(secondPassLabel, Grid.GetRow(secondPassLabel) + 1);
                Grid.SetRow(secondPass, Grid.GetRow(secondPass) + 1);
                Grid.SetRow(confirmSetPassword, Grid.GetRow(confirmSetPassword) + 1);
            }
        }
    }
}