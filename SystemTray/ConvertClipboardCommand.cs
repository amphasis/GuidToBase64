using System.Windows.Input;
using MongoConverter.Services.Converters;

namespace MongoConverter.SystemTray;

internal sealed class ConvertClipboardCommand : ICommand
{
	public ConvertClipboardCommand(
		NotifyIcon notifyIcon,
		IReadOnlyCollection<IConverter> converters)
	{
		_notifyIcon = notifyIcon;
		_converters = converters;
	}

	public bool CanExecute(object? parameter)
	{
		return true;
	}

	public void Execute(object? parameter)
	{
		var clipboardText = TextCopy.ClipboardService.GetText();

		if (string.IsNullOrWhiteSpace(clipboardText))
		{
			return;
		}

		var convertedText = ConvertLines(clipboardText);
		TextCopy.ClipboardService.SetText(convertedText);
		ShowTooltip(convertedText);
	}

	public event EventHandler? CanExecuteChanged;

	private string ConvertLines(string source)
	{
		var sourceLines = source.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
		var convertedLines = sourceLines.Select(TryConvertLine);

		return string.Join(Environment.NewLine, convertedLines);

		string TryConvertLine(string line)
		{
			var convertedLine = _converters
				.Select(converter => converter.TryParseInput(line))
				.FirstOrDefault(converted => converted != null);

			return convertedLine ?? line;
		}
	}

	private void ShowTooltip(string convertedText)
	{
		var tipText = convertedText.Length == 0
			? "(Empty content)"
			: convertedText.Length > MaxToolTipLength
				? string.Concat(convertedText.AsSpan(0, MaxToolTipLength), "...")
				: convertedText;

		_notifyIcon.ShowBalloonTip(
			timeout: BalloonTimeout.Milliseconds,
			tipTitle: "Clipboard content converted",
			tipText: tipText,
			ToolTipIcon.Info);
	}

	private readonly NotifyIcon _notifyIcon;
	private readonly IReadOnlyCollection<IConverter> _converters;

	private static readonly char[] LineSeparators = ['\r', '\n'];
	private static readonly TimeSpan BalloonTimeout = TimeSpan.FromSeconds(5);

	private const int MaxToolTipLength = 128;
}