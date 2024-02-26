using MongoConverter.Services.Converters;

namespace MongoConverter.SystemTray;

internal static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	public static void Main()
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

	private static void ConvertClipboardContent(object? sender, EventArgs eventArgs)
	{
		var clipboardText = TextCopy.ClipboardService.GetText();

		if (clipboardText == null)
		{
			return;
		}

		var convertedText = clipboardText
			.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries)
			.Select(TryConvertLine)
			.JoinLines();

		TextCopy.ClipboardService.SetText(convertedText);
	}

	private static string TryConvertLine(string line)
	{
		var convertedLine = Converters
			.Select(converter => converter.TryParseInput(line))
			.FirstOrDefault(converted => converted != null);

		return convertedLine ?? line;
	}

	private static string JoinLines(this IEnumerable<string> lines)
	{
		return string.Join(Environment.NewLine, lines);
	}

	private static readonly char[] LineSeparators = ['\r', '\n'];
	private static readonly IReadOnlyCollection<IConverter> Converters = Services.Converters.Converters.Get();
}
