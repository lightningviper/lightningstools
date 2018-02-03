using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MFDExtractor
{
    internal interface IPerformanceCounterInstaller
    {
        void CreatePerformanceCounters();
    }

    internal sealed class PerformanceCounterInstaller : IPerformanceCounterInstaller
    {
        private readonly IPerformanceCounterInstanceFactory _performanceCounterInstanceFactory;
        public PerformanceCounterInstaller(IPerformanceCounterInstanceFactory performanceCounterInstanceFactory=null)
        {
            _performanceCounterInstanceFactory = performanceCounterInstanceFactory ??
                                                 new PerformanceCounterInstanceInstanceFactory();
        }
        public void CreatePerformanceCounters()
        {
            try
            {
                if (PerformanceCounterCategory.Exists(Application.ProductName))
                {
                    try
                    {
                        PerformanceCounterCategory.Delete(Application.ProductName);
                    }
                    catch { }
                }
                var counters = new CounterCreationDataCollection();
                foreach (InstrumentType instrumentType in Enum.GetValues(typeof (InstrumentType)))
                {
                    counters.Add(new CounterCreationData(
                        string.Format("Rendered Frames per second - {0}", instrumentType),
                        string.Format("Rendered Frames per second - {0}", instrumentType), 
                        PerformanceCounterType.RateOfCountsPerSecond32));

                    counters.Add(new CounterCreationData(
                        string.Format("Skipped Frames per second - {0}", instrumentType),
                        string.Format("Skipped Frames per second - {0}", instrumentType),
                        PerformanceCounterType.RateOfCountsPerSecond32));

                    counters.Add(new CounterCreationData(
                        string.Format("Timeout Frames per second - {0}", instrumentType),
                        string.Format("Timeout Frames per second - {0}", instrumentType),
                        PerformanceCounterType.RateOfCountsPerSecond32));

                    counters.Add(new CounterCreationData(
                        string.Format("Total Frames per second - {0}", instrumentType),
                        string.Format("Total Frames per second - {0}", instrumentType),
                        PerformanceCounterType.RateOfCountsPerSecond32));

                }
                counters.Add(new CounterCreationData(
                    string.Format("Total Rendered Frames per second - All Instruments"),
                    string.Format("Total Rendered Frames per second - All Instruments"),
                    PerformanceCounterType.RateOfCountsPerSecond32));


                counters.Add(new CounterCreationData(
                    string.Format("Total Skipped Frames per second - All Instruments"),
                    string.Format("Total Skipped Frames per second - All Instruments"),
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    string.Format("Total Timeout Frames per second - All Instruments"),
                    string.Format("Total Timeout Frames per second - All Instruments"),
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    string.Format("Total Frames per second - All Instruments"),
                    string.Format("Total Frames per second - All Instruments"),
                    PerformanceCounterType.RateOfCountsPerSecond32));

                try
                {
                    PerformanceCounterCategory.Create(Application.ProductName, Application.ProductName,
                        PerformanceCounterCategoryType.SingleInstance, counters);
                }
                catch
                {
                }
                finally
                {
                    PerformanceCounter.CloseSharedResources();

                }

                Extractor.State.RenderedFramesCounter = _performanceCounterInstanceFactory.CreatePerformanceCounterInstance(Application.ProductName,
                    "Total Rendered Frames per second - All Instruments");
                Extractor.State.SkippedFramesCounter = _performanceCounterInstanceFactory.CreatePerformanceCounterInstance(Application.ProductName,
                    "Total Skipped Frames per second - All Instruments");
                Extractor.State.TotalFramesCounter = _performanceCounterInstanceFactory.CreatePerformanceCounterInstance(Application.ProductName,
                    "Total Frames per second - All Instruments");
                Extractor.State.TimeoutFramesCounter = _performanceCounterInstanceFactory.CreatePerformanceCounterInstance(Application.ProductName,
                    "Total Timeout Frames per second - All Instruments");

            }
            catch { }
        }
    }
}