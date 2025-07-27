using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

/// <summary>
/// Converts hexadecimal .NET GUID representation to MongoDB binary data format.
/// Handles 32-character hexadecimal strings representing GUID data.
/// </summary>
internal sealed class HexToBinaryConverter : IConverter
{
	/// <inheritdoc/>
	public int Order => 2;
	
	/// <inheritdoc/>
	public string InputTypeName => "Hex";
	
	/// <inheritdoc/>
	public string OutputTypeName => "MongoDB binary";
	
	/// <inheritdoc/>
	public string PreferredSplitter => ",";

	/// <inheritdoc/>
	public string? TryParseInput(string input)
	{
		var match = _guidRegex.Match(input);
		if (!match.Success) return null;

		var guid = new Guid(Convert.FromHexString(match.Value));
		return $"BinData(3, '{Convert.ToBase64String(guid.ToByteArray())}')";
	}

	/// <summary>
	/// Regular expression pattern for matching 32-character hexadecimal strings.
	/// </summary>
	private readonly Regex _guidRegex = new (@"[0-9A-Fa-f]{32}");
}