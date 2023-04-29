using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace F4SharedMemTester
{
    public class Program
    {
        public const int ERR_LEVEL__ERROR_OCCURRED = 255;
        public const int ERR_LEVEL__SUCCESS = 0;
        

        [STAThread]
        public static int Main(string[] args)
        {
            Console.SetError(Console.Out);
            var successfullyParsedCommandLineArgs = ParseCommandLineOptions(args, out string inputFileSpec,
                                                             out string errorMessage);

            if (successfullyParsedCommandLineArgs)
            {
                try
                {
                    TestRunner.LoadAndExecute(inputFileSpec);
                }
                catch (Exception e)
                {
                    return WriteException(e);
                }
                Console.WriteLine("Done!");
                return ERR_LEVEL__SUCCESS;
            }
            return ShowUsageAndWriteError(errorMessage);
        }
        
        private static int WriteException(Exception e)
        {
            return WriteError(e.ToString());
        }

        private static int WriteError(string err)
        {
            Console.Error.WriteLine(err);
            return ERR_LEVEL__ERROR_OCCURRED;
        }

        private static bool ParseCommandLineOptions(string[] args, out string inputFileSpec, out string errorMessage)
        {
            //set default values for output parms
            {
                inputFileSpec = null;
                errorMessage = null;
            }

            if (args == null || args.Length == 0) //no args means no input file specified
            {
                errorMessage = "Please specify the input file.";
                return false;
            }

            if (args.Length > 1) //too many args
            {
                errorMessage = "Too many arguments.";
                return false;
            }

            inputFileSpec = args[0];
            if (!ValidateFileExists(ref inputFileSpec, ref errorMessage))
            {
                return false;
            }

            return true;
        }

        private static bool ValidateFileExists(ref string filespec, ref string errorMessage)
        {
            var isValid = File.Exists(filespec);
            if (!isValid)
            {
                errorMessage = string.Format("File does not exist: {0}", filespec);
                return false;
            }
            return true;
        }

        private static int ShowUsageAndWriteError(string errorMessage)
        {
            ShowUsage(Console.Out);
            return WriteError(errorMessage);
        }

        private static void ShowUsage(TextWriter o)
        {
            var appName = new FileInfo(Assembly.GetExecutingAssembly().Location).Name;
            o.WriteLine("Usage:");
            o.WriteLine();
            o.Write(appName);
            o.WriteLine(" [inputFileSpec]");
            o.WriteLine();
        }

    }
}