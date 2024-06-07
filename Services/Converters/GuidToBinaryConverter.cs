using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

internal sealed class GuidToBinaryConverter : IConverter
{
	public int Order => 1;
	public string InputTypeName => "GUID";
	public string OutputTypeName => "MongoDB binary";
	public string PreferredSplitter => ",";

	public string? TryParseInput(string input)
	{
		var match = _guidRegex.Match(input);

		return match.Success && Guid.TryParse(match.Value, out var guid)
			? $"BinData(3, '{Convert.ToBase64String(guid.ToByteArray())}')"
			: null;
	}

	private readonly Regex _guidRegex = new (@"[0-9A-Fa-f]{8}-(?:[0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}");
}