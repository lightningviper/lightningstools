using F16CPD.Mfd.Controls;
using F16CPD.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD.Mfd.Menus.InstrumentsDisplay
{
    internal interface IInstrumentsDisplayMenuPageFactory
    {
        MfdMenuPage BuildInstrumentsDisplayMenuPage();
    }
    class InstrumentsDisplayMenuPageFactory:IInstrumentsDisplayMenuPageFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public InstrumentsDisplayMenuPageFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory= null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public MfdMenuPage BuildInstrumentsDisplayMenuPage()
        {
            var thisPage = new MfdMenuPage(_mfdManager);
            var buttons = new List<OptionSelectButton>();
            const int triangleLegLengthPixels = 25;

            var altimeterModeButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 3, "PNEU", false);
            altimeterModeButton.FunctionName = "ToggleAltimeterModeElecPneu";
            altimeterModeButton.Pressed += pneuElecButton_Press;

            var altitudeIndexUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7, "^", false,
                                                                 triangleLegLengthPixels);
            altitudeIndexUpButton.FunctionName = "AltitudeIndexIncrease";
            altitudeIndexUpButton.Pressed += altitudeIndexUpButton_Press;

            var altitudeIndexLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7.5f, "ALT", false);
            
            var altitudeIndexDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 8, @"\/", false,
                                                                   triangleLegLengthPixels);
            altitudeIndexDownButton.FunctionName = "AltitudeIndexDecrease";
            altitudeIndexDownButton.Pressed += altitudeIndexDownButton_Press;
            
            var barometricPressureUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 9, "^", false,
                                                                      triangleLegLengthPixels);
            barometricPressureUpButton.FunctionName = "BarometricPressureSettingIncrease";
            barometricPressureUpButton.Pressed += barometricPressureUpButton_Press;
            
            var barometricPressureLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 9.5f, "BARO", false);
            
            var barometricPressureDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 10, @"\/", false,
                                                                        triangleLegLengthPixels);
            barometricPressureDownButton.FunctionName = "BarometricPressureSettingDecrease";
            barometricPressureDownButton.Pressed += barometricPressureDownButton_Press;
            
            var courseSelectUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 11, "^", false,
                                                                triangleLegLengthPixels);
            courseSelectUpButton.FunctionName = "CourseSelectIncrease";
            courseSelectUpButton.Pressed += courseSelectUpButton_Press;
            
            var courseSelectLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 11.5f, "CRS", false);
            
            var courseSelectDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 12, @"\/", false,
                                                                  triangleLegLengthPixels);
            courseSelectDownButton.FunctionName = "CourseSelectDecrease";
            courseSelectDownButton.Pressed += courseSelectDownButton_Press;
            
            var tacticalAwarenessDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 16, "TAD",
                                                                                    false);
            tacticalAwarenessDisplayPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToTADPage();
            
            var targetingPodPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 17, "TGP", false);
            targetingPodPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToTargetingPodPage();
            
            var headDownDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 18, "HDD", true);
            headDownDisplayPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToInstrumentsPage();
            
            var headingSelectDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 20, @"\/", false,
                                                                   triangleLegLengthPixels);
            headingSelectDownButton.FunctionName = "HeadingSelectDecrease";
            headingSelectDownButton.Pressed += headingSelectDownButton_Press;
            
            var headingSelectLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 20.5f, "HDG", false);
            
            var headingSelectUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 21, "^", false,
                                                                 triangleLegLengthPixels);
            headingSelectUpButton.FunctionName = "HeadingSelectIncrease";
            headingSelectUpButton.Pressed += headingSelectUpButton_Press;
            
            var lowAltitudeThresholdSelectDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 22, @"\/",
                                                                                false,
                                                                                triangleLegLengthPixels);
            lowAltitudeThresholdSelectDownButton.FunctionName = "LowAltitudeWarningThresholdDecrease";
            lowAltitudeThresholdSelectDownButton.Pressed += lowAltitudeThresholdSelectDownButton_Press;
            
            var lowAltitudeThresholdLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 22.5f, "ALOW", false);
            
            var lowAltitudeThresholdSelectUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 23, "^", false,
                                                                              triangleLegLengthPixels);
            lowAltitudeThresholdSelectUpButton.Pressed += lowAltitudeThresholdSelectUpButton_Press;
            lowAltitudeThresholdSelectUpButton.FunctionName = "LowAltitudeWarningThresholdIncrease";
            
            var airspeedIndexSelectDownButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24, @"\/", false,
                                                                         triangleLegLengthPixels);
            airspeedIndexSelectDownButton.FunctionName = "AirspeedIndexDecrease";
            airspeedIndexSelectDownButton.Pressed += airspeedIndexSelectDownButton_Press;
            
            var airspeedIndexLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24.5f, "ASPD", false);
            
            var airspeedIndexSelectUpButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 25, @"^", false,
                                                                       triangleLegLengthPixels);
            airspeedIndexSelectUpButton.FunctionName = "AirspeedIndexIncrease";
            airspeedIndexSelectUpButton.Pressed += airspeedIndexSelectUpButton_Press;

            buttons.Add(altimeterModeButton);
            buttons.Add(altitudeIndexUpButton);
            buttons.Add(altitudeIndexLabel);
            buttons.Add(altitudeIndexDownButton);
            buttons.Add(barometricPressureUpButton);
            buttons.Add(barometricPressureLabel);
            buttons.Add(barometricPressureDownButton);
            buttons.Add(courseSelectUpButton);
            buttons.Add(courseSelectLabel);
            buttons.Add(courseSelectDownButton);
            buttons.Add(tacticalAwarenessDisplayPageSelectButton);
            buttons.Add(targetingPodPageSelectButton);
            buttons.Add(headDownDisplayPageSelectButton);
            buttons.Add(headingSelectDownButton);
            buttons.Add(headingSelectLabel);
            buttons.Add(headingSelectUpButton);
            buttons.Add(lowAltitudeThresholdSelectDownButton);
            buttons.Add(lowAltitudeThresholdLabel);
            buttons.Add(lowAltitudeThresholdSelectUpButton);
            buttons.Add(airspeedIndexSelectDownButton);
            buttons.Add(airspeedIndexLabel);
            buttons.Add(airspeedIndexSelectUpButton);
            thisPage.OptionSelectButtons = buttons;
            thisPage.Name = "Instruments Display Page";
            return thisPage;
        }
        private void pneuElecButton_Press(object sender, EventArgs args)
        {
            if (_mfdManager.SimSupportModule != null)
                _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton3, (OptionSelectButton)sender);
        }
        
        private void altitudeIndexUpButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var secondsPressed = (howLongPressed / 1000.0f);
            var valueDelta = 20;

            if (howLongPressed >= 200) valueDelta = 100;
            if (secondsPressed >= 1) valueDelta = 500;
            if (secondsPressed >= 2) valueDelta = 1000;

            var diff = valueDelta + ((((_mfdManager.AltitudeIndexInFeet / valueDelta)) * valueDelta) - _mfdManager.AltitudeIndexInFeet);
            _mfdManager.AltitudeIndexInFeet += diff;
        }
        private void altitudeIndexDownButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var secondsPressed = (howLongPressed / 1000.0f);
            var valueDelta = 20;

            if (howLongPressed >= 200) valueDelta = 100;
            if (secondsPressed >= 1) valueDelta = 500;
            if (secondsPressed >= 2) valueDelta = 1000;

            var diff = valueDelta - ((((_mfdManager.AltitudeIndexInFeet / valueDelta)) * valueDelta) - _mfdManager.AltitudeIndexInFeet);
            _mfdManager.AltitudeIndexInFeet -= diff;
        }

        private void barometricPressureUpButton_Press(object sender, EventArgs e)
        {
            _mfdManager.FlightData.BarometricPressure += 0.01f;
            if (_mfdManager.SimSupportModule != null)
                _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton9, (OptionSelectButton)sender);
        }
        private void barometricPressureDownButton_Press(object sender, EventArgs e)
        {
            _mfdManager.FlightData.BarometricPressure -= 0.01f;
            if (_mfdManager.SimSupportModule != null)
                _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton10, (OptionSelectButton)sender);
        }

        private void courseSelectUpButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var numTimes = 1;
            if (howLongPressed > 300) numTimes = Settings.Default.FastCourseAndHeadingAdjustSpeed;

            for (var i = 0; i < numTimes; i++)
            {
                _mfdManager.FlightData.HsiDesiredCourseInDegrees += 1;
                if (_mfdManager.SimSupportModule != null)
                    _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton11, (OptionSelectButton)sender);
            }
        }

        private void courseSelectDownButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var numTimes = 1;
            if (howLongPressed > 300) numTimes = Settings.Default.FastCourseAndHeadingAdjustSpeed;

            for (var i = 0; i < numTimes; i++)
            {
                _mfdManager.FlightData.HsiDesiredCourseInDegrees -= 1;
                if (_mfdManager.SimSupportModule != null)
                    _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton12, (OptionSelectButton)sender);
            }
        }

        private void headingSelectUpButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var numTimes = 1;
            if (howLongPressed > 300) numTimes = Settings.Default.FastCourseAndHeadingAdjustSpeed;

            for (var i = 0; i < numTimes; i++)
            {
                _mfdManager.FlightData.HsiDesiredHeadingInDegrees += 1;
                if (_mfdManager.SimSupportModule != null)
                    _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton21, (OptionSelectButton)sender);
            }
        }

        private void headingSelectDownButton_Press(object sender, MomentaryButtonPressedEventArgs e)
        {
            var whenPressed = e.WhenPressed;
            var howLongPressed = (int)DateTime.UtcNow.Subtract(whenPressed).TotalMilliseconds;
            var numTimes = 1;
            if (howLongPressed > 300) numTimes = Settings.Default.FastCourseAndHeadingAdjustSpeed;

            for (var i = 0; i < numTimes; i++)
            {
                _mfdManager.FlightData.HsiDesiredHeadingInDegrees -= 1;
                if (_mfdManager.SimSupportModule != null)
                    _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton20, (OptionSelectButton)sender);
            }
        }
        private void airspeedIndexSelectUpButton_Press(object sender, EventArgs e)
        {
            _mfdManager.AirspeedIndexInKnots += 20;
        }

        private void airspeedIndexSelectDownButton_Press(object sender, EventArgs e)
        {
            _mfdManager.AirspeedIndexInKnots -= 20;
        }

        private void lowAltitudeThresholdSelectUpButton_Press(object sender, EventArgs e)
        {
            var button = (OptionSelectButton)sender;
            if (_mfdManager.SimSupportModule != null)
                _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton23, button);
        }

        private void lowAltitudeThresholdSelectDownButton_Press(object sender, EventArgs e)
        {
            var button = (OptionSelectButton)sender;
            if (_mfdManager.SimSupportModule != null)
                _mfdManager.SimSupportModule.HandleInputControlEvent(CpdInputControls.OsbButton22, button);
        }





    }
}
