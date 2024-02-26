namespace MongoConverter.Services.Converters
{
	public interface IConverter
	{
		string InputTypeName { get; }
		string OutputTypeName { get; }
		string? TryParseInput(string input);
	}
}