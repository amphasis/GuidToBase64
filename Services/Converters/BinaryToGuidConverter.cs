using System.Text.RegularExpressions;

namespace MongoConverter.Services.Converters;

internal sealed class BinaryToGuidConverter : IConverter
{
	public int Order => 0;
	public string InputTypeName => "Base64";
	public string OutputTypeName => ".NET GUID";
	public string PreferredSplitter => "";

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

	private readonly Regex _base64GuidRegex = new(@"[0-9A-Za-z+\/]{22}==");
}