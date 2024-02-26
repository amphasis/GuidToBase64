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
				new ToolStripMenuItem("Convert clipboard content", null, ConvertClipboardContent),
				new ToolStripMenuItem("Exit", null, (_, _) => { Application.Exit(); }),
			}
		};

		icon.Visible = true;

		Application.Run();
	}

	private static void ConvertClipboardContent(object? s, EventArgs e)
	{
	}
}
