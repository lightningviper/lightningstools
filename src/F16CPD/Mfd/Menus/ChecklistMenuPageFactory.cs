using F16CPD.Mfd.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD.Mfd.Menus
{
    internal interface IChecklistMenuPageFactory
    {
        MfdMenuPage BuildChecklistMenuPage();
    }
    class ChecklistMenuPageFactory:IChecklistMenuPageFactory
    {
        private F16CpdMfdManager _mfdManager;
        private IOptionSelectButtonFactory _optionSelectButtonFactory;
        public ChecklistMenuPageFactory(
            F16CpdMfdManager mfdManager,
            IOptionSelectButtonFactory optionSelectButtonFactory = null
        )
        {
            _mfdManager = mfdManager;
            _optionSelectButtonFactory = optionSelectButtonFactory ?? new OptionSelectButtonFactory();
        }
        public MfdMenuPage BuildChecklistMenuPage()
        {
            var thisPage = new MfdMenuPage(_mfdManager);
            var buttons = new List<OptionSelectButton>();

            var chartPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 2, "CHARTS", false);
            chartPageSelectButton.Pressed += (s,e)=>_mfdManager.SwitchToChartsPage();
            buttons.Add(chartPageSelectButton);

            var checklistPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 3, "CHKLST", true);
            checklistPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToChecklistsPage();
            buttons.Add(checklistPageSelectButton);

            var mapPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 4, "MAP", false);
            mapPageSelectButton.Pressed += (s, e) => _mfdManager.SwitchToTADPage();
            buttons.Add(mapPageSelectButton);

            var nextChecklistFileSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24, @"\/", false);
            nextChecklistFileSelectButton.Pressed += (s, e) => _mfdManager.NextChecklistFile();
            buttons.Add(nextChecklistFileSelectButton);

            var checklistListLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 24.5f, "L\nI\nS\nT",
                                                                     false);
            buttons.Add(checklistListLabel);

            var previousChecklistFileSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 25, "^", false);
            previousChecklistFileSelectButton.Pressed += (s, e) => _mfdManager.PrevChecklistFile();
            buttons.Add(previousChecklistFileSelectButton);

            var tacticalAwarenessDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 16, "TAD",
                                                                                    true);
            tacticalAwarenessDisplayPageSelectButton.Pressed += (s,e)=>_mfdManager.SwitchToTADPage();
            buttons.Add(tacticalAwarenessDisplayPageSelectButton);

            var targetingPodPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 17, "TGP", false);
            targetingPodPageSelectButton.Pressed += (s,e)=>_mfdManager.SwitchToTargetingPodPage();
            buttons.Add(targetingPodPageSelectButton);

            var headDownDisplayPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 18, "HDD", false);
            headDownDisplayPageSelectButton.Pressed += (s,e)=>_mfdManager.SwitchToInstrumentsPage();
            buttons.Add(headDownDisplayPageSelectButton);


            var nextChecklistPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 8, @"\/", false);
            nextChecklistPageSelectButton.Pressed += (s,e)=>_mfdManager.NextChecklistPage();
            buttons.Add(nextChecklistPageSelectButton);

            var checklistPageLabel = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7.5f, "P\nA\nG\nE", false);
            buttons.Add(checklistPageLabel);

            var prevChecklistPageSelectButton = _optionSelectButtonFactory.CreateOptionSelectButton(thisPage, 7, @"^", false);
            prevChecklistPageSelectButton.Pressed += (s,e)=>_mfdManager.PrevChecklistPage();
            buttons.Add(prevChecklistPageSelectButton);

            thisPage.OptionSelectButtons = buttons;
            thisPage.Name = "Checklists Page";
            return thisPage;
        }


    }
}
