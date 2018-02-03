using System;
using System.Windows.Forms;
using Common.UI.Screen;
using MFDExtractor.Configuration;
using MFDExtractor.UI;

namespace MFDExtractor
{
    public interface IInstrumentFormDataChangedHandler
    {
        void HandleDataChangedEvent(object sender, EventArgs e);
    }

    internal class InstrumentFormDataChangedHandler : IInstrumentFormDataChangedHandler
    {
	    private readonly InstrumentType _instrumentType;
        private readonly  InstrumentForm _instrumentForm;
	    private readonly IInstrumentFormSettingsWriter _instrumentFormSettingsWriter;
        public InstrumentFormDataChangedHandler(
            InstrumentType instrumentTYpe,
            InstrumentForm instrumentForm, 
			IInstrumentFormSettingsWriter instrumentFormSettingsWriter = null
            )
        {
	        _instrumentType = instrumentTYpe;
            _instrumentForm = instrumentForm;
	        _instrumentFormSettingsWriter = instrumentFormSettingsWriter ?? new InstrumentFormSettingsWriter();
        }
        public void HandleDataChangedEvent(object sender, EventArgs e)
        {
            var location = _instrumentForm.DesktopLocation;
            var screen = Screen.FromRectangle(_instrumentForm.DesktopBounds);
	        var settings = _instrumentForm.Settings;
	        settings.OutputDisplay = Util.CleanDeviceName(screen.DeviceName);
            if (!_instrumentForm.StretchToFill)
            {
                var size = _instrumentForm.Size;
	            settings.ULX = location.X - screen.Bounds.Location.X;
	            settings.ULY = location.Y - screen.Bounds.Location.Y;
	            settings.LRX = (location.X - screen.Bounds.Location.X) + size.Width;
                settings.LRY = (location.Y - screen.Bounds.Location.Y) + size.Height;
            }
			settings.Enabled = _instrumentForm.Visible;
			_instrumentFormSettingsWriter.Write(_instrumentType.ToString(), settings);
        }
    }
}
