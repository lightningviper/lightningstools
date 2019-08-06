using F16CPD.Networking;
using F4Utils.Resources;
using F4Utils.Terrain;
using Common.Drawing;
using Common.Drawing.Imaging;
using System.IO;

namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface ITheaterMapRetriever 
    {
        Bitmap GetTheaterMapImage(ref float mapWidthInFeet);
    }
    internal class TheaterMapRetriever:ITheaterMapRetriever
    {
        private readonly TerrainDB _terrainDB;
        private readonly IResourceBundleReader _resourceBundleReader;
        private readonly IF16CPDClient _client;
        private Bitmap _theaterMap;
        private float _mapWidthInFeet;
       
        public TheaterMapRetriever(TerrainDB terrainDB, IF16CPDClient client, IResourceBundleReader resourceBundleReader=null)
        {
            _terrainDB = terrainDB;
            _client = client;
            _resourceBundleReader = resourceBundleReader ?? new ResourceBundleReader();
        }
        public Bitmap GetTheaterMapImage(ref float mapWidthInFeet)
        {
            if (_theaterMap != null)
            {
                mapWidthInFeet = _mapWidthInFeet;
                return _theaterMap;
            }
            if (Properties.Settings.Default.RunAsClient)
            {
                _theaterMap = GetLatestMapFromServer(out _mapWidthInFeet);
                mapWidthInFeet = _mapWidthInFeet;
                return _theaterMap;
            }
            if (_terrainDB == null || _terrainDB.TerrainBasePath == null) return null;
            var mapWidthInL2Segments = _terrainDB.TheaterDotMap.LODMapWidths[2];
            var mapWidthInL2Posts = mapWidthInL2Segments * F4Utils.Terrain.Constants.NUM_ELEVATION_POSTS_ACROSS_SINGLE_LOD_SEGMENT;
            mapWidthInFeet = mapWidthInL2Posts * (_terrainDB.TheaterDotMap.FeetBetweenL0Posts * 4);
            Image theaterMap = null;
            if (_resourceBundleReader.NumResources <= 0)
            {
                try
                {
                    var campMapResourceBundleIndexPath =
                                                         _terrainDB.DataPath + Path.DirectorySeparatorChar
                                                        + _terrainDB.TheaterDotTdf.artDir + Path.DirectorySeparatorChar
                                                        + "resource" + Path.DirectorySeparatorChar
                                                        + "campmap.idx";
                    if (!new FileInfo(campMapResourceBundleIndexPath).Exists)
                    {
                        campMapResourceBundleIndexPath =
                                                         _terrainDB.DataPath + Path.DirectorySeparatorChar
                                                        + _terrainDB.TheaterDotTdf.artDir + Path.DirectorySeparatorChar
                                                        + "art" + Path.DirectorySeparatorChar 
                                                        + "resource" + Path.DirectorySeparatorChar
                                                        + "campmap.idx";
                    }
                    if (!new FileInfo(campMapResourceBundleIndexPath).Exists)
                    {
                        campMapResourceBundleIndexPath =
                                                         _terrainDB.DataPath + Path.DirectorySeparatorChar
                                                        + "art" + Path.DirectorySeparatorChar
                                                        + "resource" + Path.DirectorySeparatorChar
                                                        + "campmap.idx";
                    }
                    _resourceBundleReader.Load(campMapResourceBundleIndexPath);
                }
                catch { }
            }
            if (_resourceBundleReader.NumResources > 0)
            {
                theaterMap = (Image)_resourceBundleReader.GetImageResource("BIG_MAP_ID");
                Common.Imaging.Util.ConvertPixelFormat(ref theaterMap, PixelFormat.Format16bppRgb555);
            }
            else
            {
                return null;
            }
            _theaterMap = (Bitmap)theaterMap;
            _mapWidthInFeet = mapWidthInFeet;
            if (Properties.Settings.Default.RunAsServer)
            {
                MakeTheaterMapAvailableToClient();
            }
            return _theaterMap;
        }
        private void MakeTheaterMapAvailableToClient()
        {
            if (_theaterMap == null) return;
            var theaterMap = (Image)_theaterMap;
            var rawBytes = Common.Imaging.Util.BytesFromBitmap(theaterMap, null, "BMP");
            F16CPDServer.SetSimProperty("CurrentMapImage", rawBytes);
            F16CPDServer.SetSimProperty("CurrentMapWidthInFeet", _mapWidthInFeet);

        }
        private Bitmap GetLatestMapFromServer(out float mapWidthInFeet)
        {
            var mapBytes = (byte[])_client.GetSimProperty("CurrentMapImage");
            var mapWidth = _client.GetSimProperty("CurrentMapWidthInFeet");
            if (mapBytes == null || mapWidth == null)
            {
                mapWidthInFeet = float.NaN;
                return null;
            }
            mapWidthInFeet = (float)mapWidth;
           return (Bitmap)Common.Imaging.Util.BitmapFromBytes(mapBytes);
        }
    }
}
