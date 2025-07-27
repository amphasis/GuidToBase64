using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

/// <summary>
/// Converts GUID strings to MongoDB binary data format.
/// Handles standard GUID format and converts to Base64-encoded binary representation.
/// </summary>
internal sealed class GuidToBinaryConverter : IConverter
{
	/// <inheritdoc/>
	public int Order => 1;
	
	/// <inheritdoc/>
	public string InputTypeName => "GUID";
	
	/// <inheritdoc/>
	public string OutputTypeName => "MongoDB binary";
	
	/// <inheritdoc/>
	public string PreferredSplitter => ",";

	/// <inheritdoc/>
	public string? TryParseInput(string input)
	{
		var match = _guidRegex.Match(input);

		return match.Success && Guid.TryParse(match.Value, out var guid)
			? $"BinData(3, '{Convert.ToBase64String(guid.ToByteArray())}')"
			: null;
	}

	/// <summary>
	/// Regular expression pattern for matching standard GUID format.
	/// Matches format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
	/// </summary>
	private readonly Regex _guidRegex = new (@"[0-9A-Fa-f]{8}-(?:[0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}");
}