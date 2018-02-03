using System;
using System.Threading;
using log4net;
using Microsoft.DirectX.DirectInput;

namespace Common.InputSupport.DirectInput
{
    public class Util
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Util));

        /// <summary>
        ///     Gets the DirectInput Device associated with a specific
        ///     Device Instance Guid.  Safer than DirectInput's Device
        ///     constructor and allows for retrying within a default
        ///     timeout period.  If the device cannot be obtained
        ///     within 1 second, then a null Device object is returned.
        /// </summary>
        /// <param name="instanceGuid">
        ///     the DirectInput Device Instance GUID
        ///     of the Device to retrieve.
        /// </param>
        /// <returns></returns>
        public static Device GetDIDevice(Guid instanceGuid)
        {
            Device device = null;
            var startTime = DateTime.UtcNow;
            var endTime = startTime.Add(TimeSpan.FromMilliseconds(1000));

            while (device == null && DateTime.UtcNow.CompareTo(endTime) < 0)
            {
                try
                {
                    device = new Device(instanceGuid);
                }
                catch (ApplicationException e)
                {
                    _log.Debug(e.Message, e);
                }
                catch (NullReferenceException)
                {
                }
                Thread.Sleep(20);
            }
            return device;
        }
    }
}