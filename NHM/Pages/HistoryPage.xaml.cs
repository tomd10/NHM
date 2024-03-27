using NHM.Model;

namespace NHM.Pages;

public partial class HistoryPage : ContentPage
{
	public HistoryPage()
	{
		InitializeComponent();
	}

	public void buttonShowClicked(object sender, EventArgs e)
	{
		if (DataStore.processed)
		{
			labelLog.Text = TextEdit.Serialize();
		}
        else
        {
            DisplayAlert("Error!", "No devices processed", "OK");
        }
	}

	public async void buttonLoadClicked(object sender, EventArgs e)
	{
		string choice = await DisplayPromptAsync("Enter name", "Enter name of log", "OK");
        string log = await SecureStorage.Default.GetAsync("nhmcfg-log" + choice);
        if (log != null)
        {
            labelLog.Text = log;
        }
        else
        {
            await DisplayAlert("Error!", "Log not found", "OK");
        }
    }

    public async void buttonSaveClicked(object sender, EventArgs e)
    {
        if (DataStore.processed)
        {
            string choice = await DisplayPromptAsync("Enter name", "Enter name of log", "OK");
            string log = TextEdit.Serialize();
            await SecureStorage.Default.SetAsync("nhmcfg-log" + choice, log);
        }
        else
        {
            await DisplayAlert("Error!", "No devices processed", "OK");
        }
    }

    public async void buttonEmailClicked(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            await DisplayAlert("Error!", "Emails not supported on Windows platform.", "OK");
            return;
        }
        if (DataStore.processed)
        {
            string log = TextEdit.Serialize();
            if (Email.Default.IsComposeSupported)
            {

                string subject = "NHM " + log.Split('\n')[0];
                string body = log;
                string[] recipients = new string[] {""};

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };

                await Email.Default.ComposeAsync(message);
            }
            else
            {
                await DisplayAlert("Error!", "No email client", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error!", "No devices processed", "OK");
        }
    }
}