using System.Data;
using System.Net.Http.Json;

namespace MauiApp1;


public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
    }

    private async void FetchDataClicked(object sender, EventArgs e)
    {
        string apiUrl = "https://jsonplaceholder.typicode.com/todos";

        try
        {
            // Check internet connectivity
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("No Internet", "Please ensure you have internet access.", "OK");
                return;
            }

            // Fetch data from API
            var todos = await _httpClient.GetFromJsonAsync<List<Todo>>(apiUrl);

            if (todos == null || todos.Count == 0)
            {
                await DisplayAlert("No Data", "No data retrieved from the API.", "OK");
                return;
            }

            // Display data
            DataLabel.Text = string.Join("\n\n", todos.Select(t =>
                $"Title: {t.title}\nCompleted: {(t.completed ? "✅" : "❌")}"));
        }
        catch (HttpRequestException httpEx)
        {
            await DisplayAlert("HTTP Error", $"Error fetching data:\n{httpEx.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Unexpected Error", $"An unexpected error occurred:\n{ex.Message}", "OK");
        }
    }
}

// Class matching your JSON structure
public class Todo
{
    public int userId { get; set; }
    public int id { get; set; }
    public string title { get; set; }
    public bool completed { get; set; }
}
