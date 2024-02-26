namespace MongoConverter.SystemTray;

internal static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();

		using var icon = new NotifyIcon();
		icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

		icon.ContextMenuStrip = new ContextMenuStrip
		{
			Items =
			{
				new ToolStripMenuItem("Show form", null, (s, e) => { new Form1().Show(); }),
				new ToolStripMenuItem("Exit", null, (s, e) => { Application.Exit(); }),
			}
		};

		icon.Visible = true;

		Application.Run(new Form1());

		icon.Visible = false;
	}
}
