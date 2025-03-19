namespace BiometricAttendanceApp.Views;

public partial class SigninPage : ContentPage
{
	public SigninPage()
	{
		InitializeComponent();
	}
    private async void LoginClicked(object sender, EventArgs e)
    {
        var email = Preferences.Get("UserEmail", "");
        var enteredEmail = EmailEntry.Text;

        if (email == enteredEmail)
            await Shell.Current.GoToAsync("///HomePage");
        else
            await DisplayAlert("Error", "Incorrect credentials", "OK");
    }

}