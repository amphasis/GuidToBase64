using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GuidToBase64.Converters;

namespace GuidToBase64
{
	class Program
	{
		static void Main(string[] args)
		{
			var converters = GetConverters();
			var promptString = ComposePromptString(converters.Select(x => x.InputTypeName));

			while (true)
			{
				Console.Write(promptString);
				var input = Console.ReadLine();
				if (input == null) break;

				var (result, usedConverter) = converters
					.Select(converter => (Result: converter.TryParseInput(input), Converter: converter))
					.FirstOrDefault(x => x.Result != null);

				if (result == null) break;

				Console.WriteLine($"{usedConverter.OutputTypeName}: {result}");
			}
		}

		private static IReadOnlyCollection<IConverter> GetConverters()
		{
			var allTypes = Assembly.GetExecutingAssembly().DefinedTypes
				.Where(typeInfo => typeInfo.ImplementedInterfaces.Contains(typeof(IConverter)))
				.Select(typeInfo => Activator.CreateInstance(typeInfo.AsType()))
				.Cast<IConverter>()
				.ToArray();

			return allTypes;
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
