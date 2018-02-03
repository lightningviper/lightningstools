using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Strings;
using F4Utils.Campaign;
using F4Utils.Campaign.F4Structs;
using F4Utils.Process;
using F4Utils.Terrain;
using log4net;

namespace F4Utils.SimSupport
{
    public interface IThreeDeeCockpitFileFinder2
    {
        FileInfo FindThreeDeeCockpitFile(int vehicleACD);
    }

    public class ThreeDeeCockpitFileFinder : IThreeDeeCockpitFileFinder2
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ThreeDeeCockpitFileFinder));
        private readonly ICurrentTheaterDotTdfLoader _currentTheaterDotTdfLoader;
        private readonly IBMSRunningExecutableLocator _bmsRunningExecutableLocator;

        public ThreeDeeCockpitFileFinder(IBMSRunningExecutableLocator bmsRunningExecutableLocator = null, ICurrentTheaterDotTdfLoader currentTheaterDotTdfLoader = null)
        {
            _bmsRunningExecutableLocator = bmsRunningExecutableLocator ?? new BMSRunningExecutableLocator();
            _currentTheaterDotTdfLoader = currentTheaterDotTdfLoader ?? new CurrentTheaterDotTdfLoader();
        }

        public FileInfo FindThreeDeeCockpitFile(int vehicleACD)
        {
            if (vehicleACD == -1) return FindThreeDeeCockpitFileUsingOldMethod();

            var file = string.Empty;
            try
            {
                var exePath = _bmsRunningExecutableLocator.BMSExePath;
                if (exePath == null) return null;

                exePath += Path.DirectorySeparatorChar;
                var bmsBaseDirectory = new DirectoryInfo(exePath).Parent.Parent.FullName + Path.DirectorySeparatorChar;
                var currentTheaterTdf = _currentTheaterDotTdfLoader.GetCurrentTheaterDotTdf(bmsBaseDirectory);
                var dataDir = Path.Combine(bmsBaseDirectory, "data");

                var artDir = Path.Combine(dataDir, currentTheaterTdf != null ? currentTheaterTdf.artDir ?? "art" : "art");
                var mainCkptArtFolder = Path.Combine(artDir, "ckptart");
                if (!Directory.Exists(mainCkptArtFolder)) { mainCkptArtFolder = Path.Combine(dataDir, @"art\ckptart"); }

                var objectDir = Path.Combine(dataDir, currentTheaterTdf != null ? currentTheaterTdf.objectDir ?? @"terrdata\objects" : @"terrdata\objects");
                if (!Directory.Exists(objectDir)) { objectDir = Path.Combine(dataDir, @"terrdata\objects"); }
                var classTable = ClassTable.ReadClassTable(Path.Combine(objectDir, "FALCON4.CT"));
                var vehicleDataTable = new VcdFile(Path.Combine(objectDir, "FALCON4.VCD")).VehicleDataTable;
                var vehicleClass = classTable.FirstOrDefault(
                        x => x.dataType == (byte) Data_Types.DTYPE_VEHICLE
                                                          && x.vuClassData.classInfo_[(int) VuClassHierarchy.VU_DOMAIN] == (byte) Classtable_Domains.DOMAIN_AIR
                                                          && x.vuClassData.classInfo_[(int) VuClassHierarchy.VU_TYPE] == (byte) Classtable_Types.TYPE_AIRPLANE
                                                          && x.vehicleDataIndex == vehicleACD)
;

                var vehicleData = vehicleDataTable[vehicleClass.dataPtr];
                var vehicleName = Encoding.ASCII.GetString(vehicleData.Name).TrimAtNull().Replace("*", "");
                var vehicleNCTR = Encoding.ASCII.GetString(vehicleData.NCTR).TrimAtNull().Replace("*", "");
                var visType = vehicleClass.visType[0];

                const string threeDeeCockpitDatFile = "3dckpit.dat";
                if (visType == (short) Vis_Types.VIS_F16C) { file = Path.Combine(mainCkptArtFolder, threeDeeCockpitDatFile); }
                else
                {
                    file = Path.Combine(mainCkptArtFolder, visType.ToString(), threeDeeCockpitDatFile);
                    if (!FileExists(file))
                    {
                        file = Path.Combine(mainCkptArtFolder, vehicleName, threeDeeCockpitDatFile);
                        if (!FileExists(file))
                        {
                            file = Path.Combine(mainCkptArtFolder, vehicleNCTR, threeDeeCockpitDatFile);
                            if (!FileExists(file)) { file = Path.Combine(mainCkptArtFolder, threeDeeCockpitDatFile); }
                        }
                    }
                }
            }
            catch (Exception e) { _log.Error(e.Message, e); }

            var fi = !string.IsNullOrEmpty(file) ? new FileInfo(file) : null;
            return FileExists(file) ? fi : FindThreeDeeCockpitFileUsingOldMethod();
        }

        private static bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            try
            {
                var invalidPathChars = Path.GetInvalidPathChars();
                var invalidFilenameChars = Path.GetInvalidFileNameChars();
                var pathPart = Path.GetDirectoryName(fileName) ?? string.Empty;
                var fileNamePart = Path.GetFileName(fileName);
                if (invalidPathChars.Intersect(pathPart.ToCharArray()).Any() 
                    || invalidFilenameChars.Intersect(fileNamePart.ToCharArray()).Any()) { return false; }

                return File.Exists(fileName);
            }
            catch { }

            return false;
        }

        private FileInfo FindThreeDeeCockpitFileUsingOldMethod()
        {
            var basePath = _bmsRunningExecutableLocator.BMSExePath;
            return basePath == null ? null : Paths(basePath).FirstOrDefault(x => x != null);
        }

        private static IEnumerable<FileInfo> Paths(string basePath)
        {
            yield return SearchIn(Path.Combine(basePath, @"..\..\Data\art\ckptartn"), "3dckpit.dat");
            yield return SearchIn(Path.Combine(basePath, @"..\..\Data\art\ckptart"), "3dckpit.dat");
        }

        private static FileInfo SearchIn(string searchPath, string fileName)
        {
            var dir = new DirectoryInfo(searchPath);
            if (!dir.Exists) return null;

            var subdirectories = dir.GetDirectories();

            var cockpitFilesInUse = subdirectories.Concat(new[] {dir})
                .Select(x => new FileInfo(Path.Combine(x.FullName, fileName)))
                .Where(FileExistsAndIsInUse)
                .OrderByDescending(x => x.LastAccessTime)
                .ToList();

            return cockpitFilesInUse.FirstOrDefault();
        }

        private static bool FileExistsAndIsInUse(FileInfo file)
        {
            if (!FileExists(file.FullName)) return false;

            try { using (var filestream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { filestream.Close(); } }
            catch (IOException) { return true; }

            return false;
        }
    }
}