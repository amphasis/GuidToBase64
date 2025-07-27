using MongoConverter.Services.Converters;
using NHotkey;
using NHotkey.WindowsForms;

namespace MongoConverter.SystemTray;

/// <summary>
/// Entry point class for the MongoDB Converter system tray application.
/// </summary>
internal static class Program
{
	/// <summary>
	/// The main entry point for the application. Initializes the system tray icon, 
	/// sets up the context menu, and configures the global hotkey for clipboard conversion.
	/// </summary>
	[STAThread]
	public static void Main()
	{
		const Keys hotkey = Keys.Control | Keys.Alt | Keys.Z;

		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();

		using var icon = new NotifyIcon();
		var convertCommand = new ConvertClipboardCommand(icon, Converters.Get());

		icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		icon.Text = "MongoDB GUID/BinData Converter";

		icon.ContextMenuStrip = new ContextMenuStrip
		{
			Items =
			{
				// ReSharper disable once AccessToDisposedClosure
				new ToolStripMenuItem("Convert Clipboard") {Command = convertCommand, ShortcutKeys = hotkey},
				new ToolStripMenuItem("Exit", null, (_, _) => Application.Exit()),
			}
		};

		icon.Visible = true;

		var hotkeyName = $"ConvertCommand{Guid.NewGuid()}";

		try
		{
			HotkeyManager.Current.AddOrReplace(hotkeyName, hotkey, HotkeyHandler);
			Application.Run();
		}
		finally
		{
			HotkeyManager.Current.Remove(hotkeyName);
		}

		/// <summary>
		/// Handles the global hotkey event for clipboard conversion.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The hotkey event arguments containing information about the triggered hotkey.</param>
		void HotkeyHandler(object? sender, HotkeyEventArgs eventArgs)
		{
			if (eventArgs.Name == hotkeyName)
			{
				convertCommand.Execute(sender);
				eventArgs.Handled = true;
			}
		}
	}
}
