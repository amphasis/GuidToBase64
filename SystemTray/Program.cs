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
		icon.Text = "Convert MongoDB GUID/BinData";

		icon.ContextMenuStrip = new ContextMenuStrip
		{
			Items =
			{
				// ReSharper disable once AccessToDisposedClosure
				new ToolStripMenuItem("Convert clipboard content", null, (_, _) => ConvertClipboardContent(icon)),
				new ToolStripMenuItem("Exit", null, (_, _) => { Application.Exit(); }),
			}
		};

		icon.Visible = true;

		Application.Run();
	}

	private static void ConvertClipboardContent(NotifyIcon icon)
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

		var tipText = convertedText.Length == 0
			? "(Empty content)"
			: convertedText.Length > MaxToolTipLength
				? string.Concat(convertedText.AsSpan(0, MaxToolTipLength), "...")
				: convertedText;

		icon.ShowBalloonTip(
			timeout: BalloonTimeout.Milliseconds,
			tipTitle: "Clipboard content converted",
			tipText: tipText,
			ToolTipIcon.Info);
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
	private static readonly TimeSpan BalloonTimeout = TimeSpan.FromSeconds(5);

	private const int MaxToolTipLength = 128;
}
