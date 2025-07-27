using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

/// <summary>
/// Converts ObjectId string representations to ObjectId format.
/// Handles 24-character hexadecimal strings and formats them as ObjectId expressions.
/// </summary>
internal sealed class ObjectIdConverter : IConverter
{
	/// <inheritdoc/>
	public int Order => 3;
	
	/// <inheritdoc/>
	public string InputTypeName => "ObjectId string";
	
	/// <inheritdoc/>
	public string OutputTypeName => "ObjectId";
	
	/// <inheritdoc/>
	public string PreferredSplitter => ",";

	/// <inheritdoc/>
	public string? TryParseInput(string input)
	{
		var match = _objectIdRegex.Match(input);
		if (!match.Success) return null;

		var objectId = match.Value.ToLowerInvariant();
		return $"ObjectId('{objectId}')";
	}

	/// <summary>
	/// Regular expression pattern for matching 24-character hexadecimal ObjectId strings.
	/// </summary>
	private readonly Regex _objectIdRegex = new (@"[0-9A-Fa-f]{24}");
}