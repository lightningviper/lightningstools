using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Henkie.HSI.DeviceSimulation
{
    public class Program
    {
        private static Board1.SimulatedDevice _simulatedHsiBoard1Device;
        private static Board2.SimulatedDevice _simulatedHsiBoard2Device;
        public static void Main(string[] args)
        {
            var board1ComPort = args[0];
            var board2ComPort = args[1];
            _simulatedHsiBoard1Device = new Board1.SimulatedDevice(board1ComPort);
            _simulatedHsiBoard2Device = new Board2.SimulatedDevice(board2ComPort);

            var headingDatumDegrees = -90.000d;
            while (true)
            {
                _simulatedHsiBoard1Device.ProcessIncomingData();
                _simulatedHsiBoard2Device.ProcessIncomingData();
                var hsiBoard1Props = SerializePublicPropertiesToString(_simulatedHsiBoard1Device);
                var hsiBoard2Props = SerializePublicPropertiesToString(_simulatedHsiBoard2Device);
                _simulatedHsiBoard2Device.SetHeadingDatum(headingDatumDegrees:headingDatumDegrees);
                headingDatumDegrees += 0.1;
                if (headingDatumDegrees > 90.000d) headingDatumDegrees = -90.000d;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(hsiBoard1Props);
                Console.WriteLine(hsiBoard2Props);
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
                Thread.Sleep(10);
            }
        }
        private static string SerializePublicPropertiesToString(object obj)
        {
            if (obj == null) return string.Empty;

            var publicProperties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var lines = publicProperties.Select(p =>
            {
                object value = p.GetValue(obj, null);
                return $"{p.Name}={value ?? "null"}";
            });

            return string.Join("; ", lines) + ";";
        }
    }
}
