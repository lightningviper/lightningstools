using System;
using System.Collections.Generic;
using Common.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F16CPD.Mfd.Controls
{
    internal interface IOSBLabelSizeCalculator
    {
        Rectangle CalculateOSBLabelBitmapRectangle(float positionNum);
    }
    class OSBLabelSizeCalculator:IOSBLabelSizeCalculator
    {
        public Rectangle CalculateOSBLabelBitmapRectangle(float positionNum)
        {
            const float pixelsPerInch = Constants.F_NATIVE_RES_HEIGHT / 8.32f;
            const float bezelButtonRevealWidthInches = 0.83376676384839650145772594752187f;
            var bezelButtonRevealWidthPixels = (int)Math.Floor((bezelButtonRevealWidthInches * pixelsPerInch));

            const float bezelButtonRevealHeightInches = 0.83695842450765864332603938730853f;
            var bezelButtonRevealHeightPixels = (int)Math.Floor((bezelButtonRevealHeightInches * pixelsPerInch));

            var maxTextWidthPixels = (int)(bezelButtonRevealWidthPixels * 1.5f);
            const float bezelButtonSeparatorWidthInches = 0.14500291545189504373177842565598f;
            var bezelButtonSeparatorWidthPixels = (int)(Math.Ceiling(bezelButtonSeparatorWidthInches * pixelsPerInch));
            const float bezelButtonSeparatorHeightInches = bezelButtonSeparatorWidthInches;
            //0.14555798687089715536105032822757f;
            var bezelButtonSeparatorHeightPixels = (int)(Math.Ceiling(bezelButtonSeparatorHeightInches * pixelsPerInch));
            var leftMarginPixels =
                (int)
                (((Constants.I_NATIVE_RES_WIDTH -
                   ((5 * bezelButtonRevealWidthPixels) + (4 * bezelButtonSeparatorWidthPixels))) / 2.0f));
            var topMarginPixels =
                (int)
                (((Constants.I_NATIVE_RES_HEIGHT -
                   ((8 * bezelButtonRevealHeightPixels) + (7 * bezelButtonSeparatorHeightPixels))) / 2.0f));
            var boundingRectangle = new Rectangle();
            if (positionNum >= 1 && positionNum <= 5)
            {
                //TOP ROW OF BUTTONS
                var x = (int)(((positionNum - 1) * (bezelButtonRevealWidthPixels + bezelButtonSeparatorWidthPixels))) +
                        leftMarginPixels;
                boundingRectangle.Location = new Point(x, 0);
                var width = bezelButtonRevealWidthPixels;
                var height = bezelButtonRevealHeightPixels;
                boundingRectangle.Size = new Size(width, height);
            }
            else if (positionNum >= 6 && positionNum <= 13)
            {
                //RIGHT HAND SIDE BUTTONS
                var y = (int)(((positionNum - 6) * (bezelButtonRevealHeightPixels + bezelButtonSeparatorHeightPixels))) +
                        topMarginPixels;
                var width = maxTextWidthPixels;
                var height = bezelButtonRevealHeightPixels;
                boundingRectangle.Size = new Size(width, height);
                boundingRectangle.Location = new Point(Constants.I_NATIVE_RES_WIDTH - width, y);
            }
            else if (positionNum >= 14 && positionNum <= 18)
            {
                //BOTTOM ROW OF BUTTONS
                var x = (int)(((18 - positionNum) * (bezelButtonRevealWidthPixels + bezelButtonSeparatorWidthPixels))) +
                        leftMarginPixels;
                var y = Constants.I_NATIVE_RES_HEIGHT - bezelButtonRevealHeightPixels;
                boundingRectangle.Location = new Point(x, y);
                var width = bezelButtonRevealWidthPixels;
                var height = bezelButtonRevealHeightPixels;
                boundingRectangle.Size = new Size(width, height);
            }
            else if (positionNum >= 19 && positionNum <= 26)
            {
                //LEFT HAND SIDE BUTTONS
                var y =
                    (int)(((26 - positionNum) * (bezelButtonRevealHeightPixels + bezelButtonSeparatorHeightPixels))) +
                    topMarginPixels;
                var width = maxTextWidthPixels;
                var height = bezelButtonRevealHeightPixels;
                boundingRectangle.Size = new Size(width, height);
                boundingRectangle.Location = new Point(0, y);
            }
            return boundingRectangle;
        }
    }
}
