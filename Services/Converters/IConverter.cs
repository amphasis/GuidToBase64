namespace MongoConverter.Services.Converters;

public interface IConverter
{
	int Order { get; }
	string InputTypeName { get; }
	string OutputTypeName { get; }
	string PreferredSplitter { get; }
	string? TryParseInput(string input);
}