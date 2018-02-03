using F16CPD.Mfd.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD.Mfd.Menus
{
    internal interface ITADMenuPageFactory
    {
        MfdMenuPage BuildTADMenuPage();
    }
    class TADMenuPageFactory:ITADMenuPageFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public TADMenuPageFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory = null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public MfdMenuPage BuildTADMenuPage()
        {
            var thisPage = new MfdMenuPage(_mfdManager);
            var buttons = new List<OptionSelectButton>();

            var chartPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 2, "CHARTS", false);
            chartPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToChartsPage();
            buttons.Add(chartPageSelectButton);

            var checklistPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 3, "CHKLST", false);
            checklistPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToChecklistsPage();
            buttons.Add(checklistPageSelectButton);

            var mapOnOffButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 4, "MAP", true);
            mapOnOffButton.Pressed += (s,e)=>_mfdManager.SwitchToTADPage();
            buttons.Add(mapOnOffButton);

            var scaleIncreaseButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7, "^", false);
            scaleIncreaseButton.Pressed += (s,e)=>_mfdManager.MapZoomIn();
            buttons.Add(scaleIncreaseButton);

            var scaleLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7.5f,
                                                      "ZOOM", false);
            buttons.Add(scaleLabel);

            var scaleDecreaseButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 8, @"\/", false);
            scaleDecreaseButton.Pressed += (s, e) => _mfdManager.MapZoomOut();
            buttons.Add(scaleDecreaseButton);


            var mapRotationModeLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 9,
                                          _mfdManager.GetMapRotationModeText(_mfdManager.MapRotationMode), false);
            mapRotationModeLabel.FunctionName = "MapRotationModeLabel";
            mapRotationModeLabel.Pressed += (s, e) => { 
                _mfdManager.MapRotationMode++; if ((int)_mfdManager.MapRotationMode > 1) _mfdManager.MapRotationMode = 0; 
            };
            buttons.Add(mapRotationModeLabel);

            
            var tacticalAwarenessDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 16, "TAD",
                                                                                    true);
            tacticalAwarenessDisplayPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToTADPage();
            buttons.Add(tacticalAwarenessDisplayPageSelectButton);

            var targetingPodPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 17, "TGP", false);
            targetingPodPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToTargetingPodPage();
            buttons.Add(targetingPodPageSelectButton);

            var headDownDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 18, "HDD", false);
            headDownDisplayPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToInstrumentsPage();
            buttons.Add(headDownDisplayPageSelectButton);

            var mapRangeIncrease = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 25, @"^", false);
            mapRangeIncrease.Pressed += (s,e)=>_mfdManager.IncreaseMapRange();
            buttons.Add(mapRangeIncrease);

            var mapRangeDecrease = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24, @"\/", false);
            mapRangeDecrease.Pressed += (s,e)=>_mfdManager.DecreaseMapRange();
            buttons.Add(mapRangeDecrease);

            var mapRangeLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24.5f,
                                                         _mfdManager.MapRangeRingsRadiusInNauticalMiles.ToString(),
                                                         false);
            mapRangeLabel.FunctionName = "MapRangeLabel";
            buttons.Add(mapRangeLabel);

            thisPage.OptionSelectButtons = buttons;
            thisPage.Name = "TAD Page";
            return thisPage;
        }
       

    }
}
