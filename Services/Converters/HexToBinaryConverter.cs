using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

internal sealed class HexToBinaryConverter : IConverter
{
	public int Order => 2;
	public string InputTypeName => "Hex";
	public string OutputTypeName => "MongoDB binary";
	public string PreferredSplitter => ",";

	public string? TryParseInput(string input)
	{
		var match = _guidRegex.Match(input);
		if (!match.Success) return null;

		var guid = new Guid(Convert.FromHexString(match.Value));
		return $"BinData(3, '{Convert.ToBase64String(guid.ToByteArray())}')";
	}

	private readonly Regex _guidRegex = new (@"[0-9A-Fa-f]{32}");
}