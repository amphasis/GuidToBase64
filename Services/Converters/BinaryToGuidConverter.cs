using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

/// <summary>
/// Converts Base64-encoded binary data to .NET GUID format.
/// Handles 22-character Base64 strings with '==' padding that represent binary GUID data.
/// </summary>
internal sealed class BinaryToGuidConverter : IConverter
{
	/// <inheritdoc/>
	public int Order => 0;
	
	/// <inheritdoc/>
	public string InputTypeName => "Base64";
	
	/// <inheritdoc/>
	public string OutputTypeName => ".NET GUID";
	
	/// <inheritdoc/>
	public string PreferredSplitter => "";

	/// <inheritdoc/>
	public string? TryParseInput(string input)
	{
		try
		{
			var match = _base64GuidRegex.Match(input);
			if (!match.Success) return null;

			input = match.Value;
			var binary = Convert.FromBase64String(input);
			var guid = new Guid(binary);
			return guid.ToString();
		}
		catch (Exception exception) when (exception is FormatException or ArgumentException)
		{
			return null;
		}
	}

	/// <summary>
	/// Regular expression pattern for matching Base64-encoded GUID strings.
	/// Matches 22 characters followed by '==' padding.
	/// </summary>
	private readonly Regex _base64GuidRegex = new(@"[0-9A-Za-z+\/]{22}==");
}