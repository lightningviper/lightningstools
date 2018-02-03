using System;
using System.Collections.Generic;
using System.Linq;
using Common.Reflection;
using Common.SimSupport;
using log4net;

namespace SimLinkup.Runtime
{
    internal class SimSupportModuleLoader
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(SimSupportModuleLoader));

        public IEnumerable<SimSupportModule> LoadSimSupportModules(string pathToScan, bool recursiveScan)
        {
            var toReturn = new List<SimSupportModule>();
            try
            {
                var moduleTypes = AssemblyTypeScanner<SimSupportModule>.FindMatchingTypesInAssemblies(pathToScan, recursiveScan);

                foreach (var moduleType in moduleTypes.Distinct())
                    try
                    {
                        var module = Activator.CreateInstance(moduleType) as SimSupportModule;
                        toReturn.Add(module);
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