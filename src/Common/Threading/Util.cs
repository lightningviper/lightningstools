using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace Common.Threading
{
    public static class Util
    {
        public static void AbortThread(ref Thread t)
        {
            if (t == null) return;
            try
            {
                t.Abort();
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(Util)).Error(e.Message, e);
            }
            Common.Util.DisposeObject(t);
            t = null;
        }

        public static void WaitAllHandlesInListAndClearList(List<WaitHandle> toWait, int millisecondsTimeout)
        {
            if (toWait != null && toWait.Count > 0)
            {
                try
                {
                    var handles = toWait.ToArray();
                    if (handles.Length > 0)
                    {
                        WaitHandle.WaitAll(handles, millisecondsTimeout);
                    }
                }
                catch (TimeoutException te)
                {
                    LogManager.GetLogger(typeof(Util)).Error(te.Message, te);
                }
                catch (DuplicateWaitObjectException dwoe) //this can happen somehow if our list is not cleared
                {
                    LogManager.GetLogger(typeof(Util)).Error(dwoe.Message, dwoe);
                }
            }
            toWait?.Clear();
        }
        public static void SetMinAndMaxThreadPoolThreads(int minWorkerThreadsToConfigure, int maxWorkerThreadsToConfigure, int minIoThreadsToConfigure, int maxIoThreadsToConfigure)
        {
            ThreadPool.SetMaxThreads(maxWorkerThreadsToConfigure, maxIoThreadsToConfigure);
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIOThreads);

            int targetMinWorker = System.Math.Min(minWorkerThreadsToConfigure, maxWorkerThreads);
            int targetMinIO = System.Math.Min(minIoThreadsToConfigure, maxIOThreads);
            ThreadPool.SetMinThreads(targetMinWorker, targetMinIO);

            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIOThreads);

            // Print results
            Console.WriteLine($"[Max Limits] Worker Threads: {maxWorkerThreads:N0} | I/O Threads: {maxIOThreads:N0}");
            Console.WriteLine($"[Min Limits] Worker Threads: {minWorkerThreads:N0} | I/O Threads: {minIOThreads:N0}");
        }
    }
}