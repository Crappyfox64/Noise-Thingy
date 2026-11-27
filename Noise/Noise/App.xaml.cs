using Microsoft.Maui.Controls;

namespace Noise;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Catch ALL crashes and show them instead of just closing
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            try
            {
                string msg = "CRASH: " + e.ExceptionObject?.ToString();
                System.IO.File.WriteAllText("/storage/emulated/0/Download/noise_crash.txt", msg);
            }
            catch { }
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            try
            {
                System.IO.File.WriteAllText("/storage/emulated/0/Download/noise_crash.txt", "TASK CRASH: " + e.Exception.ToString());
            }
            catch { }
            e.SetObserved();
        };

        try
        {
            MainPage = new MainPage();
        }
        catch (Exception ex)
        {
            // If MainPage itself crashes, show a page with the error
            MainPage = new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text = "STARTUP CRASH:\n\n" + ex.ToString(),
                        FontSize = 14
                    }
                }
            };
        }
    }
}