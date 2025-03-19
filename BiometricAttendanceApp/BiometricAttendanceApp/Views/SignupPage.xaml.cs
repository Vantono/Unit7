using System.Text.RegularExpressions;

namespace BiometricAttendanceApp.Views;

public partial class SignupPage : ContentPage
{
    public SignupPage()
    {
        InitializeComponent();
    }

    private async void SignupClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var name = NameEntry.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Name and Email are required.", "OK");
            return;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        Preferences.Set("UserName", name);
        Preferences.Set("UserEmail", email);

        await Shell.Current.GoToAsync("//Signin");

    }
}
