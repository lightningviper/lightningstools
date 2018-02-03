using Common.Drawing;
using System.IO;
using System.Threading.Tasks;
using F4Utils.Process;
using F4Utils.Terrain.Structs;

namespace F4Utils.Terrain
{
    public interface ITerrainDBFactory
    {
        TerrainDB Create(bool loadAllLods);
    }
    public class TerrainDBFactory : ITerrainDBFactory
    {
        public TerrainDB Create(bool loadAllLods)
        {
            var bmsBaseDirectory = GetBmsBaseDirectory();
            if (bmsBaseDirectory == null) return null;
            return new TerrainDB(bmsBaseDirectory, loadAllLods);
        }

        private string GetBmsBaseDirectory()
        {
            var exePath = Util.GetFalconExePath();
            if (exePath == null) return null;
            var exePathFI = new FileInfo(exePath);
            return exePathFI.Directory.Parent.Parent.FullName + Path.DirectorySeparatorChar;
        }
    }
}
