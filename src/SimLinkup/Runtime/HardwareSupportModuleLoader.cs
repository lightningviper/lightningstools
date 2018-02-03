using System;
using System.Collections.Generic;
using System.Linq;
using Common.HardwareSupport;
using Common.Reflection;
using log4net;

namespace SimLinkup.Runtime
{
    internal class HardwareSupportModuleLoader
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(HardwareSupportModuleLoader));

        public IEnumerable<IHardwareSupportModule> LoadHardwareSupportModules(string pathToScan, bool recursiveScan)
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var moduleTypes = AssemblyTypeScanner<IHardwareSupportModule>.FindMatchingTypesInAssemblies(pathToScan,
                        recursiveScan);

                foreach (var moduleType in moduleTypes.Distinct())
                    try
                    {
                        var instances =
                            moduleType.GetMethod("GetInstances").Invoke(null, null) as IHardwareSupportModule[];
                        if (instances != null) toReturn.AddRange(instances);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return toReturn;
        }
    }
}