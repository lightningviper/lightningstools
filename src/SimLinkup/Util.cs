using System;
using System.IO;
using System.Reflection;

namespace SimLinkup
{
    internal static class Util
    {
        private static string _defaultProfile;
        public static string ApplicationDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;

        public static string ContentDirectory => Path.Combine(ApplicationDirectory, "Content");
        public static string CurrentMappingProfileDirectory => Path.Combine(MappingBaseDirectory, DefaultProfile);

        public static string DefaultProfile
        {
            get
            {
                if (_defaultProfile == null)
                {
                    using (var reader = File.OpenText(Path.Combine(MappingBaseDirectory, "default.profile")))
                    {
                        _defaultProfile = reader.ReadToEnd();
                    }
                }
                return _defaultProfile;
            }
        }

        public static string ExePath => Assembly.GetExecutingAssembly().Location;

        public static string MappingBaseDirectory => Path.Combine(ContentDirectory, "Mapping");
    }
}