using MFDExtractor.Properties;

namespace MFDExtractor.Configuration
{
	public interface IKeySettingsReader
	{
		KeySettings Read();
	}

	public class KeySettingsReader : IKeySettingsReader
	{
		private readonly IInputControlSelectionSettingReader _inputControlSelectionSettingReader;
	    private KeySettings _keySettings;
		public KeySettingsReader(IInputControlSelectionSettingReader inputControlSelectionSettingReader = null)
		{
			_inputControlSelectionSettingReader = inputControlSelectionSettingReader ?? new InputControlSelectionSettingReader();
		    Settings.Default.SettingChanging += (s, e) => _keySettings = null;
            Settings.Default.SettingsSaving += (s, e) => _keySettings = null;
            Settings.Default.SettingsLoaded += (s, e) => _keySettings = null;
		}

        
		public KeySettings Read()
		{
			return _keySettings ?? (_keySettings = new KeySettings
			{
				NVISKey = _inputControlSelectionSettingReader.Read(Settings.Default.NVISKey),
				AirspeedIndexIncreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.AirspeedIndexIncreaseKey),
				AirspeedIndexDecreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.AirspeedIndexDecreaseKey),
				EHSIHeadingIncreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSIHeadingIncreaseKey),
				EHSIHeadingDecreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSIHeadingDecreaseKey),
				EHSICourseDecreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSICourseDecreaseKey),
				EHSICourseDepressedKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSICourseKnobDepressedKey),
				EHSIMenuButtonDepressedKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSIMenuButtonKey),
				EHSICourseIncreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.EHSICourseIncreaseKey),
				ISISBrightButtonKey = _inputControlSelectionSettingReader.Read(Settings.Default.ISISBrightButtonKey),
				ISISStandardButtonKey = _inputControlSelectionSettingReader.Read(Settings.Default.ISISStandardButtonKey),
				AzimuthIndicatorBrightnessIncreaseKey = _inputControlSelectionSettingReader.Read(Settings.Default.AzimuthIndicatorBrightnessIncreaseKey),
				AzimuthIndicatorBrightnessDecreaseKey =_inputControlSelectionSettingReader.Read(Settings.Default.AzimuthIndicatorBrightnessDecreaseKey),
				AccelerometerResetKey = _inputControlSelectionSettingReader.Read(Settings.Default.AccelerometerResetKey)
			});
		}

	}
}
