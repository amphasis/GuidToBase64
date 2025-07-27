using System.Text;
using System.Windows.Input;
using MongoConverter.Services.Converters;

namespace MongoConverter.SystemTray;

/// <summary>
/// Command that converts clipboard content using available converters and updates the clipboard with the converted result.
/// </summary>
internal sealed class ConvertClipboardCommand : ICommand
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConvertClipboardCommand"/> class.
	/// </summary>
	/// <param name="notifyIcon">The notify icon used to display conversion results to the user.</param>
	/// <param name="converters">The collection of converters available for clipboard content conversion.</param>
	public ConvertClipboardCommand(
		NotifyIcon notifyIcon,
		IReadOnlyCollection<IConverter> converters)
	{
		_notifyIcon = notifyIcon;
		_converters = converters;
	}

	/// <summary>
	/// Occurs when changes occur that affect whether or not the command should execute.
	/// </summary>
	public event EventHandler? CanExecuteChanged;

	/// <summary>
	/// Defines the method that determines whether the command can execute in its current state.
	/// </summary>
	/// <param name="parameter">Data used by the command. This parameter can be null.</param>
	/// <returns>Always returns <c>true</c> as the command can always be executed.</returns>
	public bool CanExecute(object? parameter)
	{
		return true;
	}

	/// <summary>
	/// Executes the clipboard conversion command by reading clipboard content, converting it using available converters, 
	/// and updating the clipboard with the converted result.
	/// </summary>
	/// <param name="parameter">Data used by the command. This parameter is not used.</param>
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

	/// <summary>
	/// Splits the input string into individual lines.
	/// </summary>
	/// <param name="source">The source string to split into lines.</param>
	/// <returns>A list of individual lines from the source string. Returns an empty list if the source is null.</returns>
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

	/// <summary>
	/// Generates a user-friendly tip text describing the conversion statistics.
	/// </summary>
	/// <param name="usageCountByConverter">Dictionary containing the usage count for each converter.</param>
	/// <returns>A formatted string describing which conversions were performed and how many lines were affected.</returns>
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

	/// <summary>
	/// Joins the converted lines using the appropriate splitter based on the converters that were used.
	/// </summary>
	/// <param name="usageCountByConverter">Dictionary containing the usage count for each converter.</param>
	/// <param name="convertedLines">The collection of converted lines to join.</param>
	/// <returns>A single string containing all converted lines joined with the appropriate splitter.</returns>
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

	/// <summary>
	/// The notify icon used to display conversion results to the user.
	/// </summary>
	private readonly NotifyIcon _notifyIcon;
	
	/// <summary>
	/// The collection of converters available for clipboard content conversion.
	/// </summary>
	private readonly IReadOnlyCollection<IConverter> _converters;

	/// <summary>
	/// The duration for which balloon tips are displayed to the user.
	/// </summary>
	private static readonly TimeSpan BalloonTimeout = TimeSpan.FromSeconds(5);
}