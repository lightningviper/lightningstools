using System.Diagnostics;

namespace MFDExtractor
{
    internal interface IPerformanceCounterInstanceFactory
    {
        PerformanceCounter CreatePerformanceCounterInstance(string categoryName, string counterName);
    }

    internal class PerformanceCounterInstanceInstanceFactory : IPerformanceCounterInstanceFactory
    {
        public PerformanceCounter CreatePerformanceCounterInstance(string categoryName, string counterName)
        {
            try
            {
                if ( PerformanceCounterCategory.Exists(categoryName))// && PerformanceCounterCategory.CounterExists(categoryName, counterName))
                {
                    PerformanceCounter.CloseSharedResources();
                    var perfCounter = new PerformanceCounter
                    {
                        ReadOnly = false,
                        CategoryName = categoryName,
                        CounterName = counterName,
                        MachineName = ".",
                        RawValue = 0
                    };
                    perfCounter.Increment();
                    PerformanceCounter.CloseSharedResources();
                    return perfCounter;
                };
            }
            catch { }
            return null;
        }
    }
}