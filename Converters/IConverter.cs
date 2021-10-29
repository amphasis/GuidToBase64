namespace GuidToBase64.Converters
{
	public interface IConverter
	{
		string InputTypeName { get; }
		string OutputTypeName { get; }
		string? TryParseInput(string input);
	}
}