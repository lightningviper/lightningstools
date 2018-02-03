using Common.UI.Layout;
using F16CPD.Mfd.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD.Mfd.Menus
{
    internal interface IOptionSelectButtonFactory
    {
        OptionSelectButton CreateOptionSelectButton(MfdMenuPage page, float positionNum, string labelText,
                                                           bool invertLabelText);
        OptionSelectButton CreateOptionSelectButton(MfdMenuPage page, float positionNum, string labelText,
                                                                   bool invertLabelText, int? triangleLegLengthPixels);
    }
    class OptionSelectButtonFactory:IOptionSelectButtonFactory
    {
        private IOSBLabelSizeCalculator _osbLabelSizeCalculator;
        public OptionSelectButtonFactory(IOSBLabelSizeCalculator osbLabelSizeCalculator=null) {
            _osbLabelSizeCalculator = osbLabelSizeCalculator ?? new OSBLabelSizeCalculator();
        }
        public OptionSelectButton CreateOptionSelectButton(MfdMenuPage page, float positionNum, string labelText,
                                                           bool invertLabelText)
        {
            return CreateOptionSelectButton(page, positionNum, labelText, invertLabelText, null);
        }

        public OptionSelectButton CreateOptionSelectButton(MfdMenuPage page, float positionNum, string labelText,
                                                                   bool invertLabelText, int? triangleLegLengthPixels)
        {
            var button = new OptionSelectButton(page)
            {
                PositionNumber = positionNum,
                LabelText = labelText,
                InvertLabelText = invertLabelText
            };
            var boundingRectangle = _osbLabelSizeCalculator.CalculateOSBLabelBitmapRectangle(positionNum);
            button.LabelLocation = boundingRectangle.Location;
            button.LabelSize = boundingRectangle.Size;
            if (triangleLegLengthPixels.HasValue)
            {
                button.TriangleLegLength = triangleLegLengthPixels.Value;
            }
            if (positionNum >= 1 && positionNum <= 5)
            {
                //TOP 
                button.TextVAlignment = VAlignment.Top;
                button.TextHAlignment = HAlignment.Center;
            }
            else if (positionNum >= 6 && positionNum <= 13)
            {
                //RIGHT
                button.TextVAlignment = VAlignment.Center;
                button.TextHAlignment = HAlignment.Right;
            }
            else if (positionNum >= 14 && positionNum <= 18)
            {
                //BOTTOM
                button.TextVAlignment = VAlignment.Bottom;
                button.TextHAlignment = HAlignment.Center;
            }
            else if (positionNum >= 19 && positionNum <= 26)
            {
                //LEFT
                button.TextVAlignment = VAlignment.Center;
                button.TextHAlignment = HAlignment.Left;
            }

            if (labelText.Trim() == "^")
            {
                button.TextVAlignment = VAlignment.Center;
            }
            else if (labelText.Trim() == @"\/")
            {
                button.TextVAlignment = VAlignment.Center;
            }
            return button;
        }


    }
}
