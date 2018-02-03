using F4Utils.Process;
using System.Linq;
namespace F4Utils.Terrain
{
    internal interface ITileSetConfigValueReader
    {
        string TileSetConfigValue { get; }
    }

    internal class TileSetConfigValueReader : ITileSetConfigValueReader
    {
        private readonly IBMSConfigFileReader _bmsConfigFileReader;
        private readonly ITokenJoiner _tokenJoiner;

        public TileSetConfigValueReader(IBMSConfigFileReader bmsConfigFileReader = null, ITokenJoiner tokenJoiner=null)
        {
            _bmsConfigFileReader = bmsConfigFileReader ?? new BMSConfigFileReader();
            _tokenJoiner = tokenJoiner ?? new TokenJoiner();
        }
        public string TileSetConfigValue
        {
            get
            {
                var matchingConfigLine= _bmsConfigFileReader
                    .ConfigLines
                    .Select(Common.Strings.Util.Tokenize)
                    .Where(tokens =>
                        tokens.Count > 2 &&
                        tokens[0].ToLowerInvariant() == "set" &&
                        tokens[1].ToLowerInvariant() == "g_sTileSet".ToLowerInvariant())
                    .FirstOrDefault();

                if (matchingConfigLine ==null) return null;
                            
                return matchingConfigLine
                    .Skip(2)
                    .Take(1)
                    .Select(x=>x.TrimStart('\"').TrimEnd('\"').Trim())
                    .FirstOrDefault();
                    
            }
        }
    }
}
