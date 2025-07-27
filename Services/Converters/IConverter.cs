namespace MongoConverter.Services.Converters;

/// <summary>
/// Interface for data type converters.
/// Provides a unified contract for converting data between different formats.
/// </summary>
public interface IConverter
{
	/// <summary>
	/// Order of converter application when attempting to parse input data.
	/// Converters with lower values will be called first.
	/// </summary>
	int Order { get; }
	
	/// <summary>
	/// Name of the input data type that this converter handles.
	/// Used for display in the user interface.
	/// </summary>
	string InputTypeName { get; }
	
	/// <summary>
	/// Name of the output data type that this converter transforms input data into.
	/// Used for display in the user interface.
	/// </summary>
	string OutputTypeName { get; }
	
	/// <summary>
	/// Preferred separator for joining conversion results of multiple lines.
	/// Used when processing multi-line input.
	/// </summary>
	string PreferredSplitter { get; }
	
	/// <summary>
	/// Attempts to parse and convert the input string to the output format.
	/// </summary>
	/// <param name="input">Input string for conversion</param>
	/// <remarks>This method should be applied to a single line of input.</remarks>
	/// <returns>Converted string on success, null if conversion is not possible</returns>
	string? TryParseInput(string input);
}