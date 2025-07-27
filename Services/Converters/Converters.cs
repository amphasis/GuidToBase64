using System.Reflection;

namespace MongoConverter.Services.Converters;

/// <summary>
/// Factory for creating and obtaining all available data converters.
/// Automatically discovers and creates instances of all classes implementing the <see cref="IConverter"/> interface.
/// </summary>
public static class Converters
{
	/// <summary>
	/// Gets a collection of all available converters ordered by priority (Order).
	/// Converters are created through reflection based on IConverter interface implementation.
	/// </summary>
	/// <returns>Read-only collection of converters sorted by application order</returns>
	public static IReadOnlyCollection<IConverter> Get()
	{
		var allTypes = Assembly.GetExecutingAssembly().DefinedTypes
			.Where(typeInfo => typeInfo.ImplementedInterfaces.Contains(typeof(IConverter)))
			.Select(typeInfo => Activator.CreateInstance(typeInfo.AsType()))
			.Cast<IConverter>()
			.OrderBy(converter => converter.Order)
			.ToArray();

		return allTypes;
	}
}