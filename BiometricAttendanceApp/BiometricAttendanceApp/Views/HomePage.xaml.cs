using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Microsoft.Maui.Devices.Sensors;

namespace BiometricAttendanceApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void CheckInClicked(object sender, EventArgs e)
    {
        await HandleCheckInOut("Check-In");
    }

    private async void CheckOutClicked(object sender, EventArgs e)
    {
        await HandleCheckInOut("Check-Out");
    }

    private async void LogoutClicked(object sender, EventArgs e)
    {
        Preferences.Remove("UserName");
        Preferences.Remove("UserEmail");
        Preferences.Remove("BiometricRegistered");
        Preferences.Remove("Check-InDate");
        Preferences.Remove("Check-OutDate");

        await DisplayAlert("Logged Out", "Your data was cleared. Please restart the app.", "OK");
    }

    private async Task HandleCheckInOut(string actionType)
    {
        bool biometricRegistered = Preferences.Get("BiometricRegistered", false);

        // Case 1: Biometric Registration
        if (!biometricRegistered)
        {
            var regResult = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration("Register Biometric", "Register your biometric to continue."));

            if (regResult.Authenticated)
            {
                Preferences.Set("BiometricRegistered", true);
                await DisplayAlert("Success", "Biometric registered successfully.", "OK");
                // Save to database if necessary
            }
            else
            {
                await DisplayAlert("Error", "Biometric registration failed.", "OK");
                return;
            }
        }

        // Case 2: Biometric Authentication
        var authResult = await CrossFingerprint.Current.AuthenticateAsync(
            new AuthenticationRequestConfiguration("Authentication", $"Authenticate to {actionType}."));

        if (!authResult.Authenticated)
        {
            // Case 2b: Biometric authentication failed
            await DisplayAlert("Error", "Biometric authentication failed.", "OK");
            return;
        }

        // Case 2a: GPS Location Check
        var locationResult = await CheckUserLocationAsync();
        if (!locationResult)
            return;

        // Case 3: Check if already checked-in/out today
        var todayDate = DateTime.Now.ToString("yyyy-MM-dd");
        var lastActionDate = Preferences.Get($"{actionType}Date", "");

        if (lastActionDate == todayDate)
        {
            await DisplayAlert("Already Done", $"You have already completed {actionType} today.", "OK");
            return;
        }

        Preferences.Set($"{actionType}Date", todayDate);
        Preferences.Set($"{actionType}Time", DateTime.Now.ToString("HH:mm:ss"));

        await DisplayAlert("Success", $"{actionType} successful at {DateTime.Now:T}.", "OK");
        // Optionally save to database here
    }

    private async Task<bool> CheckUserLocationAsync()
    {
        var location = await Geolocation.GetLastKnownLocationAsync();

        if (location == null)
        {
            await DisplayAlert("Error", "Unable to get location.", "OK");
            return false;
        }

        var officeLatitude = 37.9715; // Replace with your office coordinates
        var officeLongitude = 23.7267;

        var distance = Location.CalculateDistance(location.Latitude, location.Longitude, officeLatitude, officeLongitude, DistanceUnits.Kilometers);

        if (distance > 100)
        {
            await DisplayAlert("Location Error", "You're not at the office premises.", "OK");
            return false;
        }

        return true;
    }
}
