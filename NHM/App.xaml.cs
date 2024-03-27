using System.Runtime.CompilerServices;

namespace NHM;

public partial class App : Application
{
	public static App application;
	public App()
	{
		InitializeComponent();

        MainPage = new AppShell();
		application = this;
	}
}
