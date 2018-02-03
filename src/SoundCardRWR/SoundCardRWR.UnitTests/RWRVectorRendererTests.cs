using NUnit.Framework;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualBasic.Devices;

namespace SoundCardRWR.UnitTests
{
    [TestFixture]
    public class RWRVectorRendererTests
    {
        private static Audio Audio = new Audio();
        [Test, Apartment(System.Threading.ApartmentState.STA), Repeat(3000)]
        public void TestSharedMemRenderToAudioStreamAndPlay()
        {
            var renderer = new BMSRWRRenderer();
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Append();
            drawingContext.PushTransform(new ScaleTransform(1, -1));
            renderer.Render(drawingContext);
            drawingContext.Close();
            Rect bounds = new Rect(0, -500, 500, 500);
            var ms = new MemoryStream();
            AudioEncoder.Serialize(drawingGroup, bounds, ms);
            ms.Seek(0, SeekOrigin.Begin);
            Audio.Stop();
            Audio.Play(ms, Microsoft.VisualBasic.AudioPlayMode.BackgroundLoop);
        }
    }

}
