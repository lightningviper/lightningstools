using System;
using System.Linq;
using System.Threading;
using static F4SharedMemTester.TestFile;

namespace F4SharedMemTester
{
    internal class TestRunner:IDisposable
    {
        private readonly F4SharedMem.Writer _sharedMemWriter = new F4SharedMem.Writer();
        private bool _isDisposed;

        public static void LoadAndExecute(string inputFileSpec)
        {
            var testFile = Load(inputFileSpec);
            using (var testRunner = new TestRunner())
            {
                testRunner.Execute(testFile);
            }
        }
        public void Execute(TestFile testFile)
        {
            var testStartTime = DateTime.Now;

            var moments = testFile.Moments.OrderBy(x => x.StartTime).Where(x=>x.StartTime.HasValue).ToList();
            if (moments.Count == 0) return;

            WriteMomentToSharedMemory(moments[0]);
            Moment previousMoment = moments[0];

            for (var i = 1; i< moments.Count; i++)
            {
                var nextMoment = moments[i];
                var elapsedTime = DateTime.Now.Subtract(testStartTime);
                while (elapsedTime < nextMoment.StartTime.Value)
                {
                    var interpolatedMoment = InterpolationHelper.Interpolate(previousMoment, nextMoment, elapsedTime);
                    WriteMomentToSharedMemory(interpolatedMoment);
                    Thread.Sleep(1);
                    elapsedTime = DateTime.Now.Subtract(testStartTime);
                }
                WriteMomentToSharedMemory(nextMoment);
                previousMoment = nextMoment;
            }
            
        }
        private void WriteMomentToSharedMemory(Moment moment)
        {
            if (moment.FlightData.HasValue)
            {
                _sharedMemWriter.WritePrimaryFlightData(moment.FlightData.Value.Serialize());
            }
            if (moment.FlightData2.HasValue)
            {
                _sharedMemWriter.WriteFlightData2(moment.FlightData2.Value.Serialize());
            }
            if (moment.OSBData.HasValue)
            {
                _sharedMemWriter.WriteOSBData(moment.OSBData.Value.Serialize());
            }
            if (moment.DrawingData != null)
            {
                _sharedMemWriter.WriteDrawingData(moment.DrawingData.Serialize());
            }
            if (moment.IntellivibeData.HasValue)
            {
                _sharedMemWriter.WriteIntellivibeData(moment.IntellivibeData.Value.Serialize());
            }
            if (moment.StringData != null)
            {
                _sharedMemWriter.WriteStringData(moment.StringData.Serialize());
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _sharedMemWriter.Dispose();
                }

                _isDisposed = true;
            }
        }

        ~TestRunner()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
