using MongoConverter.Services.Converters;
using NHotkey;
using NHotkey.WindowsForms;

namespace MongoConverter.SystemTray;

internal static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	public static void Main()
	{
		var hotkey = Keys.Z | Keys.Control | Keys.Alt | Keys.Shift;

		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();

		using var icon = new NotifyIcon();
		var convertCommand = new ConvertClipboardCommand(icon, Converters.Get());

		icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		icon.Text = "Convert MongoDB GUID/BinData";

		icon.ContextMenuStrip = new ContextMenuStrip
		{
			Items =
			{
				// ReSharper disable once AccessToDisposedClosure
				new ToolStripMenuItem("Convert clipboard content") {Command = convertCommand, ShortcutKeys = hotkey},
				new ToolStripMenuItem("Exit", null, (_, _) => { Application.Exit(); }),
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
