using System.Collections.Generic;
using Common.Drawing;
using System.Linq;

namespace F16CPD.Mfd.Controls
{
    public class MfdMenuPage
    {
        protected MfdManager _manager;

        protected MfdMenuPage()
        {
        }

        public MfdMenuPage(MfdManager manager) : this()
        {
            _manager = manager;
        }

        public List<OptionSelectButton> OptionSelectButtons { get; set; }
        public string Name { get; set; }

        public MfdManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        ///   Checks a set of x/y coordinates to see if that corresponds 
        ///   to a screen position occupied by an Option Select Buttonlabel.  If so, 
        ///   returns the corresponding <see cref = "OptionSelectButton" /> object.  
        ///   If not, returns <see langword = "null" />.
        /// </summary>
        /// <param name = "x"></param>
        /// <param name = "y"></param>
        /// <returns></returns>
        public OptionSelectButton GetOptionSelectButtonByLocation(int x, int y)
        {
            return (from button in OptionSelectButtons
                    where button.Visible
                    let origLabelRectangle = new Rectangle(button.LabelLocation, button.LabelSize)
                    let labelX =
                        (int) (((Manager.ScreenBoundsPixels.Width)/Constants.F_NATIVE_RES_WIDTH)*origLabelRectangle.X)
                    let labelY =
                        (int) (((Manager.ScreenBoundsPixels.Height)/Constants.F_NATIVE_RES_HEIGHT)*origLabelRectangle.Y)
                    let labelWidth =
                        (int)
                        (((Manager.ScreenBoundsPixels.Width)/Constants.F_NATIVE_RES_WIDTH)*origLabelRectangle.Width)
                    let labelHeight =
                        (int)
                        (((Manager.ScreenBoundsPixels.Height)/Constants.F_NATIVE_RES_HEIGHT)*origLabelRectangle.Height)
                    let labelRectangle = new Rectangle(labelX, labelY, labelWidth, labelHeight)
                    where
                        x >= labelRectangle.X && y >= labelRectangle.Y && x <= (labelRectangle.X + labelRectangle.Width) &&
                        y <= (labelRectangle.Y + labelRectangle.Height)
                    select button).FirstOrDefault();
        }

        public OptionSelectButton FindOptionSelectButtonByPositionNumber(float positionNumber)
        {
            return OptionSelectButtons.FirstOrDefault(button => button.PositionNumber == positionNumber);
        }

        public OptionSelectButton FindOptionSelectButtonByFunctionName(string functionName)
        {
            return OptionSelectButtons.FirstOrDefault(button => button.FunctionName == functionName);
        }

        public OptionSelectButton FindOptionSelectButtonByLabelText(string labelText)
        {
            return OptionSelectButtons.FirstOrDefault(button => button.LabelText == labelText);
        }
    }
}