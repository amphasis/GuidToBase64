using System.Collections.Generic;
using System.Linq;
using MongoConverter.Services.Converters;

namespace MongoConverter.Console
{
	/// <summary>
	/// Main class for the data conversion console application.
	/// Provides an interactive command-line interface for converting various data types.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Entry point for the converter console application.
		/// Starts an interactive loop for data input and conversion with optional quiet mode.
		/// </summary>
		/// <param name="args">Command line arguments. Supports "-q" flag for quiet mode</param>
		static void Main(string[] args)
		{
			var isQuietMode = args.Any(x => x == "-q");

			var converters = Converters.Get();
			var promptString = ComposePromptString(converters.Select(x => x.InputTypeName));

			while (true)
			{
				if (!isQuietMode)
				{
					System.Console.Write(promptString);
				}

				var input = System.Console.ReadLine();
				if (string.IsNullOrEmpty(input)) break;

				var (result, usedConverter) = converters
					.Select(converter => (Result: converter.TryParseInput(input), Converter: converter))
					.FirstOrDefault(x => x.Result != null);

				if (result == null)
				{
					System.Console.WriteLine("Could not convert entered data! Enter empty string to quit.");
					continue;
				}

				if (isQuietMode)
				{
					System.Console.WriteLine(result);
				}
				else
				{
					TextCopy.ClipboardService.SetText(result);
					System.Console.WriteLine($"{usedConverter.OutputTypeName}: {result} (copied to clipboard)");
				}
			}
		}

		private static object ComposePromptString(IEnumerable<string> inputTypeNames)
		{
			var typeNames = inputTypeNames.ToArray();
			var left = string.Join(", ", typeNames.Take(typeNames.Length - 1));
			var right = typeNames.Length > 1 ? typeNames[^1] : null;
			var typeNamesString = string.Join(" or ", new[] { left, right }.Where(x => !string.IsNullOrEmpty(x)));

			return $"Enter {typeNamesString} value: ";
		}
	}
}
