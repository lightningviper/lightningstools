using System;
using Common.Drawing;

namespace MFDExtractor.Configuration
{
	internal interface IInstrumentFormSettingsReader
	{
		InstrumentFormSettings Read(string instrumentName);
	}

	internal class InstrumentFormSettingsReader : IInstrumentFormSettingsReader
	{
		private readonly ISettingReader _settingReader;

		public InstrumentFormSettingsReader(ISettingReader settingReader = null)
		{
			_settingReader = settingReader ?? new SettingReader();
		}

		public InstrumentFormSettings Read(string instrumentName)
		{

		    var toReturn = new InstrumentFormSettings();
		    toReturn.Enabled =(bool)_settingReader.ReadSetting(defaultValue: false,settingName: String.Format("Enable{0}Output", instrumentName));
		    toReturn.OutputDisplay =(string)_settingReader.ReadSetting(defaultValue: string.Empty,settingName: String.Format("{0}_OutputDisplay", instrumentName));
		    toReturn.StretchToFit =(bool)_settingReader.ReadSetting(defaultValue: false,settingName: String.Format("{0}_StretchToFit", instrumentName));
		    toReturn.ULX =(int) _settingReader.ReadSetting(defaultValue: 0, settingName: String.Format("{0}_OutULX", instrumentName));
		    toReturn.ULY =(int) _settingReader.ReadSetting(defaultValue: 0, settingName: String.Format("{0}_OutULY", instrumentName));
		    toReturn.LRX =(int) _settingReader.ReadSetting(defaultValue: 200, settingName: String.Format("{0}_OutLRX", instrumentName));
		    toReturn.LRY = (int) _settingReader.ReadSetting(defaultValue: 200, settingName: String.Format("{0}_OutLRY", instrumentName));
		    toReturn.AlwaysOnTop = (bool) _settingReader.ReadSetting(defaultValue: true,settingName: String.Format("{0}_AlwaysOnTop", instrumentName));
		    toReturn.Monochrome = (bool) _settingReader.ReadSetting(defaultValue: false,settingName: String.Format("{0}_Monochrome", instrumentName));
		    toReturn.RotateFlipType =(RotateFlipType)_settingReader.ReadSetting(defaultValue: RotateFlipType.RotateNoneFlipNone,settingName: String.Format("{0}_RotateFlipType", instrumentName));
		    return toReturn;
		}
	}
}
