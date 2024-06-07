using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

internal sealed class ObjectIdConverter : IConverter
{
	public int Order => 3;
	public string InputTypeName => "ObjectId string";
	public string OutputTypeName => "ObjectId";
	public string PreferredSplitter => ",";

	public string? TryParseInput(string input)
	{
		var match = _objectIdRegex.Match(input);
		if (!match.Success) return null;

		var objectId = match.Value.ToLowerInvariant();
		return $"ObjectId('{objectId}')";
	}

	private readonly Regex _objectIdRegex = new (@"[0-9A-Fa-f]{24}");
}