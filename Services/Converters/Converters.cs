using System.Reflection;

namespace MongoConverter.Services.Converters;

public static class Converters
{
	public static IReadOnlyCollection<IConverter> Get()
	{
		var allTypes = Assembly.GetExecutingAssembly().DefinedTypes
			.Where(typeInfo => typeInfo.ImplementedInterfaces.Contains(typeof(IConverter)))
			.Select(typeInfo => Activator.CreateInstance(typeInfo.AsType()))
			.Cast<IConverter>()
			.ToArray();

		return allTypes;
	}
}