namespace NHM;

public partial class AppShell : Shell
{
	public static AppShell shell;
	public AppShell()
	{
		InitializeComponent();
		shell = this;
	}
}
