using System.Collections.Concurrent;
using System.Reflection;

namespace MFDExtractor
{
	internal interface ISettingWriter
	{
		void WriteSetting(string settingName, object value);
	}

	class SettingWriter : ISettingWriter
	{
		private readonly ConcurrentDictionary<string, MethodInfo> _setterMethods = new ConcurrentDictionary<string, MethodInfo>();
		public void WriteSetting(string settingName, object value)
		{
			var setterMethod = _setterMethods.GetOrAdd(settingName, PropertySetterMethod);
			if (setterMethod != null)
			{
				setterMethod.Invoke(Properties.Settings.Default, new [] {value});
			}
		}

		private static MethodInfo PropertySetterMethod(string settingName)
		{
			var settingsClass = Properties.Settings.Default.GetType();
            var propertyMetadata = settingsClass.GetProperty(settingName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
			return propertyMetadata != null ? propertyMetadata.GetSetMethod() : null;
		}
	}
}
