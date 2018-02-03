using System.Collections.Concurrent;
using System.Reflection;

namespace MFDExtractor
{
	internal interface ISettingReader
	{
		object ReadSetting(string settingName, object defaultValue);
	}

	class SettingReader : ISettingReader
	{
		private readonly ConcurrentDictionary<string, MethodInfo> _getterMethods= new ConcurrentDictionary<string, MethodInfo>();
		public object ReadSetting(string settingName, object defaultValue)
		{
			var getterMethod = _getterMethods.GetOrAdd(settingName, PropertyGetterMethod);
			return getterMethod != null ? getterMethod.Invoke(Properties.Settings.Default, null) : defaultValue;
		}

		private static MethodInfo PropertyGetterMethod(string settingName)
		{
			var settingsClass = Properties.Settings.Default.GetType();
			var propertyMetadata = settingsClass.GetProperty(settingName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return propertyMetadata != null ? propertyMetadata.GetGetMethod() : null;
		}
	}
}
