using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
//using NHM.Model;
using Renci.SshNet;
using Button = Microsoft.Maui.Controls.Button;
using ShellItem = Microsoft.Maui.Controls.ShellItem;
using NHM.Model;
using NHM.Objects;
using NHM.Pages;

namespace NHM;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public async void buttonLocalClicked(object sender, EventArgs e)
	{
		string reply = await DisplayActionSheet("Select load position", "Cancel", null, "1", "2", "3", "4", "5", "6");
		if (reply == null || reply == "")
		{
			return;
		}
		
		string config = await SecureStorage.Default.GetAsync("nhmcfg" + reply);
		if (config != null)
		{
			DataStore.configured = true;
			DataStore.configuration = config;
			await DisplayAlert("Success", "Configuration loaded", "OK");
		}
		else
		{
			await DisplayAlert("Error!", "Configuration slot empty", "OK");
		}
		
    }

	public async void buttonLocalSaveClicked(object sender, EventArgs e)
	{
		if (DataStore.configured)
		{
			string reply = await DisplayActionSheet("Select save position", "Cancel", null, "1", "2", "3", "4", "5", "6");
			if (reply != "" && reply != null)
			{
                await SecureStorage.Default.SetAsync("nhmcfg" + reply, DataStore.configuration);
                await DisplayAlert("Success!", "Configuration saved", "OK");
            }
        }
		else
		{
            await DisplayAlert("Error!", "No configuration present", "OK");
        }
	}

	public async void buttonFileClicked(object sender, EventArgs e)
	{
        string config = await Filesystem.ReadFile();
        if (config != null)
        {
            DataStore.configured = true;
            DataStore.configuration = config;
            await DisplayAlert("Success", "Configuration saved", "OK");
        }
    }

	public async void buttonHttpClicked(object sender, EventArgs e)
	{
		string config = await Networking.GetResponse(entryURL.Text);
		if ( config == "HTTPErr")
		{
            await DisplayAlert("Error!", "HTTP error. Wrong URL, or the server is unreachable", "OK");
			return;
        }
        if (config != null)
        {
            DataStore.configured = true;
            DataStore.configuration = config;
            await DisplayAlert("Success", "Configuration saved", "OK");
        }

    }

	public void buttonMasterClicked(object sender, EventArgs e)
	{	if (DataStore.configured)
		{
            string keyString = entryMaster.Text;
            byte[] key = AesEncryption.StringToByteArr(keyString);
			byte[] ciphertext = null;

            try
			{
                ciphertext = AesEncryption.ArbitraryStringToByteArr(DataStore.configuration);
            }
			catch
			{
                DisplayAlert("Error!", "File format or structure incorrect", "OK");
				return;
            }
            
            string decrypted = AesEncryption.Decrypt(ciphertext, key, key);

            if (decrypted != null && decrypted.Replace("\r", "").Split("\n")[0] == "Config file for NHM v1.0")
            {
                DataStore.decrypted = true;
                DataStore.decryptedConfiguration = decrypted;

				int error = 0;
				if (ConfigParser.parseConfig(decrypted, out error))
				{
                    DisplayAlert("Success!", "Configuration decrypted and parsed", "OK");

					if (error != 0)
					{
						DisplayAlert("Warning!", "Defaults used on line " + error, "OK");
					}
                }
				else
				{
					if (error == -1)
					{
						DisplayAlert("Error!", "Wrong header format", "OK");
					}
					else if (error == -2)
					{
                        DisplayAlert("Error!", "No device for condition", "OK");
                    }
					else if (error != 0)
					{
						DisplayAlert("Error!", "Syntax error on line" + error.ToString(), "OK");
					}
				}
            }
            else
            {
                DisplayAlert("Error", "Wrong key or file", "OK");
            }
        }
		else
		{
            DisplayAlert("Error", "No configuration loaded", "OK");
        }	
	}

	public void buttonStartClicked(object sender, EventArgs e)
	{
		if (DataStore.decrypted)
		{
			//Test devices for availability
			string connected = "";
			string unreachable = "";
			string auth = "";


			foreach (NetDevice d in DataStore.devices)
			{
				bool? result = d.TestSsh();
				if (result == true)
				{
					connected = connected + d.getName() + " ";
					DataStore.availableDevices.Add(d);
				}
				else if (result == false)
				{
                    unreachable = unreachable + d.getName() + " ";
					DataStore.unreachableDevices.Add(d);
                }
				else
				{
                    auth = auth + d.getName() + " ";
					DataStore.authDevices.Add(d);
                }
			}
			if (unreachable != "" || auth != "")
			{
                DisplayAlert("Connections", "Connected devices: \r\n" + connected + "\r\nUnreachable devices: \r\n" + unreachable + "\r\nAuth failed devices: \r\n" + auth, "OK");
            }

			string processedDevices = "";
            foreach (NetDevice d in DataStore.availableDevices)
            {
                if (d.GetReplies())
				{
					DataStore.processedDevices.Add(d);
					processedDevices = processedDevices + d.getName() + " ";
				}
            }

			DataStore.processed = true;

			if (processedDevices != "")
			{
				DisplayAlert("Success!", "Successfully processed devices \r\n" + processedDevices, "OK");
			}
			else
			{
				DisplayAlert("Error!", "No devices processed", "OK");
			}

			foreach (NetDevice d in DataStore.processedDevices)
			{
				AppShell.shell.Items.Add(new DeviceShell(d));
			}

			buttonStart.IsEnabled = false;
        }

    }

	public void buttonDebugClicked(object sender, EventArgs e)
	{
		App.application.MainPage = new AppShell();

        DataStore.configured = false;
        DataStore.configuration = "";

        DataStore.decrypted = false;
        DataStore.decryptedConfiguration = "";

        DataStore.parsed = false;

        DataStore.processed = false;

        DataStore.devices = new List<NetDevice>();
        DataStore.availableDevices = new List<NetDevice>();
        DataStore.unreachableDevices = new List<NetDevice>();
        DataStore.authDevices = new List<NetDevice>();
        DataStore.processedDevices = new List<NetDevice>();
}
}

