using System;

namespace GuidToBase64.Converters
{
	public class GuidToBinaryConverter : IConverter
	{
		public string InputTypeName => "GUID";
		public string OutputTypeName => "MongoDB binary";
		public string? TryParseInput(string input)
		{
			if (!Guid.TryParse(input, out var guid)) return null;
			var guidBase64String = Convert.ToBase64String(guid.ToByteArray());
			return $"BinData(3, '{guidBase64String}')";
		}
	}
}