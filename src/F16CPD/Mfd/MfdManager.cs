using System;
using Common.Drawing;
using F16CPD.Mfd.Menus;
using F16CPD.Mfd.Controls;

namespace F16CPD.Mfd
{
    public abstract class MfdManager
    {
        public MfdManager()
        {
        }

        public MfdManager(Size screenBoundsPixels) : this()
        {
            ScreenBoundsPixels = screenBoundsPixels;
        }

        public Size ScreenBoundsPixels { get; set; }
        public MfdMenuPage[] MenuPages { get; set; }
        public MfdMenuPage ActiveMenuPage { get; set; }

        public virtual void Render(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}