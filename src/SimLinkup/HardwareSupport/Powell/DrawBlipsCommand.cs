using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.Powell
{
    internal class DrawBlipsCommand : RWRCommand
    {
        private const int MAX_RWR_SYMBOLS = 31;

        public DrawBlipsCommand()
        {
            Blips = new List<Blip>();
        }

        public IEnumerable<Blip> Blips { get; set; }

        public override byte[] ToBytes()
        {
            var blipsToWrite = Blips.Where(x => x.Symbol < Symbols.BlinkBit).Take(MAX_RWR_SYMBOLS);
            var toWrite = blipsToWrite as Blip[] ?? blipsToWrite.ToArray();
            if (toWrite.Length == 0)
            {
                return new byte[] {0x32};
            }
            var toReturn = new byte[3 * toWrite.Length + 1];
            toReturn[0] = (byte) toWrite.Length;
            for (var i = 0; i < toWrite.Length; i++)
            {
                var thisSymbol = toWrite.ElementAt(i);
                toReturn[i * 3 + 1] = thisSymbol.X;
                toReturn[i * 3 + 2] = thisSymbol.Y;
                toReturn[i * 3 + 3] = (byte) thisSymbol.Symbol;
            }
            return toReturn;
        }
    }
}