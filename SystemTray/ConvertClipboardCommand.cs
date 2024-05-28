using System.Text;
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

	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(object? parameter)
	{
		return true;
	}

	public void Execute(object? parameter)
	{
		var clipboardText = TextCopy.ClipboardService.GetText();
		var sourceLines = SplitLines(clipboardText);

		if (sourceLines.Count == 0)
		{
			return;
		}

		var usageCountByConverter = _converters.ToDictionary(x => x, _ => 0);
		var convertedLines = new List<string>(sourceLines.Count);

		foreach (var sourceLine in sourceLines)
		{
			var conversionResult = _converters
				.Select(converter => (Line: converter.TryParseInput(sourceLine), Converter: converter))
				.FirstOrDefault(result => result.Line != null);

			if (conversionResult.Line == null)
			{
				convertedLines.Add(sourceLine);
				continue;
			}

			convertedLines.Add(conversionResult.Line);
			usageCountByConverter[conversionResult.Converter] += 1;
		}

		var totalLinesConverted = usageCountByConverter.Sum(x => x.Value);

		if (totalLinesConverted == 0)
		{
			_notifyIcon.ShowBalloonTip(
				timeout: BalloonTimeout.Milliseconds,
				tipTitle: "Couldn't convert clipboard content",
				tipText: $"No convertible strings out of {sourceLines.Count} lines of text was found.",
				ToolTipIcon.Info);

			return;
		}

		var tipText = GetTipText(usageCountByConverter);

		_notifyIcon.ShowBalloonTip(
			timeout: BalloonTimeout.Milliseconds,
			tipTitle: $"Converted {totalLinesConverted} lines out of {sourceLines.Count}",
			tipText: tipText,
			ToolTipIcon.Info);

		var joinedConvertedLines = JoinConvertedLines(usageCountByConverter, convertedLines);

		TextCopy.ClipboardService.SetText(joinedConvertedLines);
	}

	private static List<string> SplitLines(string? source)
	{
		if (source == null)
		{
			return [];
		}

		using var reader = new StringReader(source);
		var lines = new List<string>();

		while (true)
		{
			var line = reader.ReadLine();

			if (line == null)
			{
				break;
			}

			lines.Add(line);
		}

		return lines;
	}

	private static string GetTipText(IReadOnlyDictionary<IConverter, int> usageCountByConverter)
	{
		var conversionStatistics = usageCountByConverter
			.Where(x => x.Value > 0)
			.OrderByDescending(x => x.Value)
			.ToArray();

		if (conversionStatistics.Length == 1)
		{
			var converter = conversionStatistics[0].Key;
			return $"{converter.InputTypeName} to {converter.OutputTypeName}";
		}

		var formattedStatisticsItems = conversionStatistics.Select(x => x.Value == 1
			? $"{x.Value} line {x.Key.InputTypeName} to {x.Key.OutputTypeName}"
			: $"{x.Value} lines {x.Key.InputTypeName} to {x.Key.OutputTypeName}");

		return string.Join(Environment.NewLine, formattedStatisticsItems);
	}

	private static string JoinConvertedLines(
		IReadOnlyDictionary<IConverter, int> usageCountByConverter,
		IEnumerable<string> convertedLines)
	{
		var preferredSplitters = usageCountByConverter
			.Where(countByConverter => countByConverter.Value > 0)
			.Select(countByConverter => countByConverter.Key.PreferredSplitter)
			.ToHashSet();

		var splitter = preferredSplitters.Count == 1
			? preferredSplitters.Single() + Environment.NewLine
			: Environment.NewLine;

		return new StringBuilder().AppendJoin(splitter, convertedLines).ToString();
	}

	private readonly NotifyIcon _notifyIcon;
	private readonly IReadOnlyCollection<IConverter> _converters;

	private static readonly TimeSpan BalloonTimeout = TimeSpan.FromSeconds(5);
}