using System;
using System.Windows.Forms;
using Common.InputSupport;
using Common.InputSupport.UI;

namespace MFDExtractor.Configuration
{
	public interface IInputControlSelectionSettingReader
	{
		InputControlSelection Read(string xmlSerializedSetting);
	}

	internal class InputControlSelectionSettingReader : IInputControlSelectionSettingReader
	{
		public InputControlSelection Read(string xmlSerializedSetting)
		{
			return (!string.IsNullOrEmpty(xmlSerializedSetting))
				? Deserialize(xmlSerializedSetting) ?? Default
				: Default;
		}

		private static InputControlSelection Default
		{
			get
			{
				return new InputControlSelection { ControlType = ControlType.Unknown,Keys = Keys.None};
			}
		}

		private static InputControlSelection Deserialize(string xmlSerializedSetting)
		{
			try
			{
				return (InputControlSelection)
					Common.Serialization.Util.DeserializeFromXml(xmlSerializedSetting,
						typeof (InputControlSelection));
			}
			catch (Exception) {}
			return null;
		}
	}
}
