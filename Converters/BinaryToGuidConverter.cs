using System;

namespace GuidToBase64.Converters
{
	public class BinaryToGuidConverter : IConverter
	{
		public string InputTypeName => "Base64";
		public string OutputTypeName => ".NET GUID";
		public string? TryParseInput(string input)
		{
			try
			{
				var binary = Convert.FromBase64String(input);
				var guid = new Guid(binary);
				return guid.ToString();
			}
			catch (Exception exception) when (exception is FormatException or ArgumentException)
			{
				return null;
			}
		}
	}
}