using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using F4Utils.Speech;

namespace TlkTool
{
    public class Program
    {
        public const int ERR_LEVEL__ERROR_OCCURRED = 255;
        public const int ERR_LEVEL__SUCCESS = 0;
        public const string ACTION_TLK2WAV = "TLK2WAV";
        public const string ACTION_WAV2TLK = "WAV2TLK";
        public const string ACTION_TLK2LH = "TLK2LH";
        public const string ACTION_LH2TLK = "LH2TLK";
        public const string ACTION_TLK2SPX = "TLK2SPX";
        public const string ACTION_SPX2TLK = "SPX2TLK";
        public const string ACTION_XML2COMM = "XML2COMM";
        public const string ACTION_COMM2XML = "COMM2XML";
        public const string ACTION_XML2FRAG = "XML2FRAG";
        public const string ACTION_FRAG2XML = "FRAG2XML";
        public const string ACTION_XML2EVAL = "XML2EVAL";
        public const string ACTION_EVAL2XML = "EVAL2XML";

        public const string ACTION_WAV2SPX = "WAV2SPX";
        public const string ACTION_SPX2WAV = "SPX2WAV";
        public const string ACTION_WAV2LH = "WAV2LH";
        public const string ACTION_LH2WAV = "LH2WAV";
        public const string ACTION_LH2SPX = "LH2SPX";
        public const string ACTION_SPX2LH = "SPX2LH";

        [STAThread]
        public static int Main(string[] args)
        {
            Console.SetError(Console.Out);
            string errorMessage;
            string action;
            string inputFileOrFolderSpec;
            string outputFileOrFolderSpec;
            CodecType codecType;
            uint? fileNum;
            var successfullyParsed = ParseCommandLineOptions(args, out action, out inputFileOrFolderSpec,
                                                             out outputFileOrFolderSpec, out codecType, out fileNum,
                                                             out errorMessage);

            if (successfullyParsed)
            {
                try
                {
                    switch (action.ToUpper().Trim())
                    {
                        case ACTION_TLK2WAV:
                            TLK2WAV(inputFileOrFolderSpec, outputFileOrFolderSpec, fileNum);
                            break;
                        case ACTION_WAV2TLK:
                            WAV2TLK(inputFileOrFolderSpec, outputFileOrFolderSpec, codecType, fileNum);
                            break;
                        case ACTION_TLK2LH:
                            return TLK2COMPRESSED(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.LH, fileNum);
                        case ACTION_LH2TLK:
                            COMPRESSED2TLK(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.LH, fileNum);
                            break;
                        case ACTION_TLK2SPX:
                            return TLK2COMPRESSED(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.SPX, fileNum);
                        case ACTION_SPX2TLK:
                            COMPRESSED2TLK(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.SPX, fileNum);
                            break;
                        case ACTION_XML2COMM:
                            XML2COMM(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_XML2FRAG:
                            XML2FRAG(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_XML2EVAL:
                            XML2EVAL(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_EVAL2XML:
                            EVAL2XML(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_FRAG2XML:
                            FRAG2XML(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_COMM2XML:
                            COMM2XML(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_WAV2SPX:
                            WAV2SPX(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_SPX2WAV:
                            SPX2WAV(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_WAV2LH:
                            WAV2LH(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_LH2WAV:
                            LH2WAV(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_LH2SPX:
                            LH2SPX(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;
                        case ACTION_SPX2LH:
                            SPX2LH(inputFileOrFolderSpec, outputFileOrFolderSpec);
                            break;

                        default:
                            return ERR_LEVEL__ERROR_OCCURRED;
                    }
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

        private static bool ParseCommandLineOptions(string[] args, out string action, out string inputFileOrFolderSpec,
                                                    out string outputFileOrFolderSpec, out CodecType codecType,
                                                    out uint? fileNum, out string errorMessage)
        {
            //set default values for output parms
            {
                inputFileOrFolderSpec = null;
                outputFileOrFolderSpec = null;
                action = null;
                errorMessage = null;
                fileNum = null;
                codecType = CodecType.Unknown;
            }

            if (args == null || args.Length == 0)
                //no args means no "action" specified -- what, we're mind-readers, now?
            {
                errorMessage = "Please specify the input file (or folder) and output file (or folder).";
                return false;
            }

            //one arg means action but no parms; that's ok... 

            if (args.Length == 2)
            {
                //two args would be an action and either an input file or an output file specified...but which (input or output file/directory) 
                //does it represent?  Fuck it...this is an error for now.
                errorMessage =
                    "Can't tell if the second argument is an input file or an output file...please specify both.";
                return false;
            }

            if (args.Length >= 3 && args.Length <= 5)
                //3 or 4 args = an action + an input file + an output file, plus optionally a codec in one case
            {
                //always wanna know the action, then the input, then the output
                inputFileOrFolderSpec = args[1];
                outputFileOrFolderSpec = args[2];
                if (string.IsNullOrEmpty(inputFileOrFolderSpec) || string.IsNullOrEmpty(inputFileOrFolderSpec.Trim()))
                {
                    inputFileOrFolderSpec = Path.GetDirectoryName(Application.ExecutablePath);
                }
                if (string.IsNullOrEmpty(outputFileOrFolderSpec) || string.IsNullOrEmpty(outputFileOrFolderSpec.Trim()))
                {
                    outputFileOrFolderSpec = Path.GetDirectoryName(Application.ExecutablePath);
                }
            }
            if (args.Length > 5) //way too many args
            {
                errorMessage = "Too many arguments.";
                return false;
            }

            action = args[0];
            if (!string.IsNullOrEmpty(action)) action = action.ToUpper().Trim();

            if (string.IsNullOrEmpty(action) ||
                (
                    action != ACTION_COMM2XML &&
                    action != ACTION_EVAL2XML &&
                    action != ACTION_FRAG2XML &&
                    action != ACTION_LH2TLK &&
                    action != ACTION_SPX2TLK &&
                    action != ACTION_TLK2LH &&
                    action != ACTION_TLK2SPX &&
                    action != ACTION_TLK2WAV &&
                    action != ACTION_WAV2TLK &&
                    action != ACTION_XML2COMM &&
                    action != ACTION_XML2EVAL &&
                    action != ACTION_XML2FRAG &&
                    action != ACTION_WAV2SPX &&
                    action != ACTION_SPX2WAV &&
                    action != ACTION_WAV2LH &&
                    action != ACTION_LH2WAV &&
                    action != ACTION_LH2SPX &&
                    action != ACTION_SPX2LH
                )
                )
            {
                errorMessage = string.Format("Invalid action: {0}", action);
                return false;
            }

            if (action == ACTION_LH2TLK || action == ACTION_TLK2LH)
            {
                codecType = CodecType.LH;
            }
            else if (action == ACTION_SPX2TLK || action == ACTION_TLK2SPX)
            {
                codecType = CodecType.SPX;
            }
            string codec = null;
            if (args.Length >= 4)
            {
                if (action == ACTION_WAV2TLK)
                {
                    codec = args[3];
                    if (!string.IsNullOrEmpty(codec)) codec = codec.ToUpper().Trim().Replace(".", "");
                }
                if (action == ACTION_TLK2LH || action == ACTION_TLK2SPX || action == ACTION_LH2TLK ||
                    action == ACTION_SPX2TLK || action == ACTION_TLK2WAV || action == ACTION_WAV2TLK)
                {
                    string fileNumString = null;
                    if (action == ACTION_TLK2LH || action == ACTION_TLK2SPX || action == ACTION_LH2TLK ||
                        action == ACTION_SPX2TLK || action == ACTION_TLK2WAV)
                    {
                        fileNumString = args[3];
                    }
                    else if (action == ACTION_WAV2TLK && args.Length > 4)
                    {
                        fileNumString = args[4];
                    }

                    if (!string.IsNullOrEmpty(fileNumString))
                    {
                        int fileNumParsed;
                        var parsed = int.TryParse(fileNumString, out fileNumParsed);
                        if (parsed && fileNumParsed > 0)
                        {
                            fileNum = (uint) fileNumParsed;
                        }
                        else
                        {
                            errorMessage = string.Format("Invalid fileNum: {0}", fileNumString);
                            return false;
                        }
                    }
                }
                else
                {
                    errorMessage = string.Format("Cannot specify <codec> when the <action> is not in {0}",
                                                 ACTION_WAV2TLK);
                    return false;
                }
            }
            if (action == ACTION_WAV2TLK)
            {
                if (codec == "SPX")
                {
                    codecType = CodecType.SPX;
                }
                else if (codec == "LH")
                {
                    codecType = CodecType.LH;
                }
                else
                {
                    errorMessage = string.Format("Invalid <codec>:{0}",
                                                 string.IsNullOrEmpty(codec) ? "(missing)" : codec);
                    return false;
                }
            }
            if (action == ACTION_WAV2TLK || action == ACTION_LH2TLK || action == ACTION_SPX2TLK)
            {
                if (fileNum.HasValue)
                {
                    if (!ValidateFileExists(ref inputFileOrFolderSpec, (fileNum + "." + codecType), ref errorMessage))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ValidateDirectoryExists(inputFileOrFolderSpec, ref errorMessage))
                    {
                        return false;
                    }
                }
                if (fileNum.HasValue)
                {
                    if (!ValidateFileExists(ref outputFileOrFolderSpec, "falcon.tlk", ref errorMessage))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ValidateFileExists(ref outputFileOrFolderSpec, "falcon.tlk", ref errorMessage))
                    {
                        if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "falcon.tlk", ref errorMessage))
                        {
                            return false;
                        }
                    }
                }
            }
            else if (action == ACTION_TLK2SPX || action == ACTION_TLK2LH)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "falcon.tlk", ref errorMessage))
                {
                    return false;
                }
                if (fileNum.HasValue)
                {
                    if (
                        !ValidateCanCreateFile(ref outputFileOrFolderSpec, (fileNum + "." + codecType), ref errorMessage))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ValidateDirectoryExistsOrCanCreate(outputFileOrFolderSpec, ref errorMessage))
                    {
                        return false;
                    }
                }
            }
            else if (action == ACTION_TLK2WAV)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "falcon.tlk", ref errorMessage))
                {
                    return false;
                }
                if (fileNum.HasValue)
                {
                    if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, (fileNum + ".WAV"), ref errorMessage))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ValidateDirectoryExistsOrCanCreate(outputFileOrFolderSpec, ref errorMessage))
                    {
                        return false;
                    }
                }
            }
            else if (action == ACTION_COMM2XML)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "commFile.bin", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "commFile.xml", ref errorMessage))
                {
                    return false;
                }
            }
            else if (action == ACTION_FRAG2XML)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "fragFile.bin", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "fragFile.xml", ref errorMessage))
                {
                    return false;
                }
            }
            else if (action == ACTION_EVAL2XML)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "evalFile.bin", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "evalFile.xml", ref errorMessage))
                {
                    return false;
                }
            }

            else if (action == ACTION_XML2COMM)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "commFile.xml", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "commFile.bin", ref errorMessage))
                {
                    return false;
                }
            }
            else if (action == ACTION_XML2FRAG)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "fragFile.xml", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "fragFile.bin", ref errorMessage))
                {
                    return false;
                }
            }
            else if (action == ACTION_XML2EVAL)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, "evalFile.xml", ref errorMessage))
                {
                    return false;
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, "evalFile.bin", ref errorMessage))
                {
                    return false;
                }
            }
            else if (action == ACTION_WAV2SPX || action == ACTION_SPX2WAV || action == ACTION_WAV2LH ||
                     action == ACTION_LH2WAV || action == ACTION_LH2SPX || action == ACTION_SPX2LH)
            {
                if (!ValidateFileExists(ref inputFileOrFolderSpec, string.Empty, ref errorMessage))
                {
                    if (!ValidateDirectoryExists(inputFileOrFolderSpec, ref errorMessage))
                    {
                        return false;
                    }
                }
                if (!ValidateCanCreateFile(ref outputFileOrFolderSpec, string.Empty, ref errorMessage))
                {
                    if (!ValidateDirectoryExistsOrCanCreate(outputFileOrFolderSpec, ref errorMessage))
                    {
                        return false;
                    }
                }
            }
            else
            {
                errorMessage = string.Format("Invalid action: {0}", action);
                return false;
            }

            if (codecType == CodecType.LH &&
                (action == ACTION_LH2SPX || action == ACTION_LH2WAV || action == ACTION_WAV2LH ||
                 action == ACTION_SPX2LH || action == ACTION_TLK2WAV || action == ACTION_WAV2TLK))
            {
                if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ST80W.dll")))
                {
                    errorMessage =
                        string.Format("Missing ST80W.dll; please copy it from your F4 installation folder, to {0}",
                                      Path.GetDirectoryName(Application.ExecutablePath));
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateFileExists(ref string filespec, string defaultFileName, ref string errorMessage)
        {
            if (Directory.Exists(filespec))
            {
                filespec = Path.Combine(filespec, defaultFileName);
            }
            var isValid = File.Exists(filespec);
            if (!isValid)
            {
                errorMessage = string.Format("File does not exist: {0}", filespec);
                return false;
            }
            return true;
        }

        private static bool ValidateDirectoryExistsOrCanCreate(string dirSpec, ref string errorMessage)
        {
            var isValid = true;
            try
            {
                if (!Directory.Exists(dirSpec))
                {
                    Directory.CreateDirectory(dirSpec);
                }
            }
            catch (Exception e)
            {
                isValid = false;
                errorMessage = string.Format("Can't create directory: {0}:{1}", dirSpec, e.Message);
            }
            return isValid;
        }

        private static bool ValidateCanCreateFile(ref string filespec, string defaultFileName, ref string errorMessage)
        {
            var isValid = true;
            if (Directory.Exists(filespec)) //filespec is a directory, not a file
            {
                filespec = Path.Combine(filespec, defaultFileName);
            }

            try
            {
                var di = new DirectoryInfo(new FileInfo(filespec).DirectoryName);

                if (!di.Exists)
                {
                    di.Create();
                    if (Directory.Exists(filespec)) //filespec is a directory, not a file
                    {
                        filespec = Path.Combine(filespec, defaultFileName);
                    }
                }
                using (File.Open(filespec, FileMode.Create, FileAccess.Write))
                {
                }
            }
            catch (Exception e)
            {
                isValid = false;
                errorMessage = string.Format("Can't create file : {0}:{1}", filespec, e.Message);
            }
            finally
            {
                if (File.Exists(filespec))
                {
                    try
                    {
                        File.Delete(filespec);
                    }
                    catch
                    {
                    }
                }
            }

            return isValid;
        }

        private static bool ValidateDirectoryExists(string folderspec, ref string errorMessage)
        {
            var audioFilesFolderPathIsValid = Directory.Exists(folderspec);
            if (!audioFilesFolderPathIsValid)
            {
                errorMessage = string.Format("Invalid folder specification: {0}", folderspec);
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
            var appName = new FileInfo(Application.ExecutablePath).Name;
            o.WriteLine("Usage:");
            o.WriteLine();
            o.Write(appName);
            o.WriteLine("  [action] [inputFileOrFolder] [outputFileOrFolder] [codec] [tlkId]");
            o.WriteLine();
            o.WriteLine("    <action> ::={ TLK2WAV | WAV2TLK   | ");
            o.WriteLine("                  TLK2LH  | LH2TLK    | ");
            o.WriteLine("                  TLK2SPX | SPX2TLK   | ");
            o.WriteLine("                  WAV2SPX | SPX2WAV   | ");
            o.WriteLine("                   WAV2LH | LH2WAV   | ");
            o.WriteLine("                   LH2SPX | SPX2LH   | ");
            o.WriteLine("                 XML2COMM | COMM2XML  | ");
            o.WriteLine("                 XML2FRAG | FRAG2XML  | ");
            o.WriteLine("                 XML2EVAL | EVAL2XML  }");
            o.WriteLine();
            o.WriteLine("    <codec> ::={ SPX | LH }   (required when action= WAV2TLK)");
            o.WriteLine();
            o.WriteLine("    <tlkId> ::={ 0, 1, 2, ...} (Tlk ID of the TLK file to extract;");
            o.WriteLine(
                "                                 required when outputFileOrFolder refers to an individual file)");
            o.WriteLine();
            o.WriteLine("Examples:");

            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\lh\ ", appName, ACTION_TLK2LH));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\lh\{2}.lh {2}", appName, ACTION_TLK2LH, 300));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\lh\ {2}", appName, ACTION_TLK2LH, 300));

            o.WriteLine(string.Format(@"{0} {1} .\lh\ .\falcon.tlk", appName, ACTION_LH2TLK));
            o.WriteLine(string.Format(@"{0} {1} .\lh\{2}.lh .\falcon.tlk {2}", appName, ACTION_LH2TLK, 300));
            o.WriteLine(string.Format(@"{0} {1} .\lh\ .\falcon.tlk {2}", appName, ACTION_LH2TLK, 300));

            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\falcon.tlk {2}", appName, ACTION_WAV2TLK, CodecType.LH));
            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\falcon.tlk {2} {3}", appName, ACTION_WAV2TLK, CodecType.LH, 300));
            o.WriteLine(string.Format(@"{0} {1} .\wav\{3}.wav .\falcon.tlk {2} {3}", appName, ACTION_WAV2TLK,
                                      CodecType.LH, 300));


            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\spx\ ", appName, ACTION_TLK2SPX));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\spx\{2}.spx {2}", appName, ACTION_TLK2SPX, 300));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\spx\ {2}", appName, ACTION_TLK2SPX, 300));

            o.WriteLine(string.Format(@"{0} {1} .\spx\ .\falcon.tlk", appName, ACTION_SPX2TLK));
            o.WriteLine(string.Format(@"{0} {1} .\spx\{2}.lh .\falcon.tlk {2}", appName, ACTION_SPX2TLK, 300));
            o.WriteLine(string.Format(@"{0} {1} .\spx\ .\falcon.tlk {2}", appName, ACTION_SPX2TLK, 300));

            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\falcon.tlk {2}", appName, ACTION_WAV2TLK, CodecType.SPX));
            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\falcon.tlk {2} {3}", appName, ACTION_WAV2TLK, CodecType.SPX,
                                      300));
            o.WriteLine(string.Format(@"{0} {1} .\wav\{3}.wav .\falcon.tlk {2} {3}", appName, ACTION_WAV2TLK,
                                      CodecType.SPX, 300));


            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\wav\ ", appName, ACTION_TLK2WAV));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\wav\ {2}", appName, ACTION_TLK2WAV, 300));
            o.WriteLine(string.Format(@"{0} {1} .\falcon.tlk .\wav\{2}.wav {2}", appName, ACTION_TLK2WAV, 300));

            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\spx\", appName, ACTION_WAV2SPX));
            o.WriteLine(string.Format(@"{0} {1} .\wav\300.wav .\spx\", appName, ACTION_WAV2SPX));
            o.WriteLine(string.Format(@"{0} {1} .\wav\300.wav .\spx\300.spx", appName, ACTION_WAV2SPX));

            o.WriteLine(string.Format(@"{0} {1} .\spx\ .\wav\", appName, ACTION_SPX2WAV));
            o.WriteLine(string.Format(@"{0} {1} .\spx\300.spx .\wav\", appName, ACTION_SPX2WAV));
            o.WriteLine(string.Format(@"{0} {1} .\spx\300.spx .\wav\300.wav", appName, ACTION_SPX2WAV));

            o.WriteLine(string.Format(@"{0} {1} .\wav\ .\lh\", appName, ACTION_WAV2LH));
            o.WriteLine(string.Format(@"{0} {1} .\wav\300.wav .\lh\", appName, ACTION_WAV2LH));
            o.WriteLine(string.Format(@"{0} {1} .\wav\300.wav .\lh\300.lh", appName, ACTION_WAV2LH));

            o.WriteLine(string.Format(@"{0} {1} .\lh\ .\wav\", appName, ACTION_LH2WAV));
            o.WriteLine(string.Format(@"{0} {1} .\lh\300.lh .\wav\", appName, ACTION_LH2WAV));
            o.WriteLine(string.Format(@"{0} {1} .\lh\300.lh .\wav\300.wav", appName, ACTION_LH2WAV));

            o.WriteLine(string.Format(@"{0} {1} .\lh\ .\spx\", appName, ACTION_LH2SPX));
            o.WriteLine(string.Format(@"{0} {1} .\lh\300.lh .\spx\", appName, ACTION_LH2SPX));
            o.WriteLine(string.Format(@"{0} {1} .\lh\300.lh .\spx\300.spx", appName, ACTION_LH2SPX));

            o.WriteLine(string.Format(@"{0} {1} .\spx\ .\lh\", appName, ACTION_SPX2LH));
            o.WriteLine(string.Format(@"{0} {1} .\spx\300.spx .\lh\", appName, ACTION_SPX2LH));
            o.WriteLine(string.Format(@"{0} {1} .\spx\300.spx .\lh\300.lh", appName, ACTION_SPX2LH));

            o.WriteLine(string.Format(@"{0} {1} .\commFile.xml .\commFile.bin", appName, ACTION_XML2COMM));
            o.WriteLine(string.Format(@"{0} {1} .\commFile.bin .\commFile.xml ", appName, ACTION_COMM2XML));

            o.WriteLine(string.Format(@"{0} {1} .\fragFile.xml .\fragFile.bin", appName, ACTION_XML2FRAG));
            o.WriteLine(string.Format(@"{0} {1} .\fragFile.bin .\fragFile.xml ", appName, ACTION_FRAG2XML));

            o.WriteLine(string.Format(@"{0} {1} .\evalFile.xml .\evalFile.bin", appName, ACTION_XML2EVAL));
            o.WriteLine(string.Format(@"{0} {1} .\evalFile.bin .\evalFile.xml ", appName, ACTION_EVAL2XML));

            o.WriteLine("-------------------------------------------------------------------------------");
            o.WriteLine();
            o.WriteLine();
            o.WriteLine("ACTIONS:");
            o.WriteLine("-------------------------------------------------------------------------------");
            o.WriteLine("    TLK2WAV  exports .WAV files from the .TLK file");
            o.WriteLine("    WAV2TLK  imports .WAV files into the .TLK file");
            o.WriteLine();
            o.WriteLine("    TLK2LH  exports LH-compressed audio files from the .TLK file");
            o.WriteLine("    LH2TLK  imports LH-compressed audio files into the .TLK file");
            o.WriteLine();
            o.WriteLine("    TLK2SPX  exports SPX-compressed audio files from the .TLK file");
            o.WriteLine("    SPX2TLK  imports SPX-compressed audio files into the .TLK file");
            o.WriteLine();
            o.WriteLine("    XML2COMM  builds a commFile.bin file from an XML file");
            o.WriteLine("    COMM2XML  exports the contents of a commFile.bin file to an XML file");
            o.WriteLine();
            o.WriteLine("    XML2FRAG  builds a fragFile.bin file from an XML file");
            o.WriteLine("    FRAG2XML  exports the contents of a fragFile.bin file to an XML file");
            o.WriteLine();
            o.WriteLine("    XML2EVAL  builds a evalFile.bin file from an XML file");
            o.WriteLine("    EVAL2XML  exports the contents of a evalFile.bin file to an XML file");
            o.WriteLine();
            o.WriteLine("    WAV2SPX converts a .WAV file (or a folder of .WAV files) to .SPX file(s)");
            o.WriteLine("    SPX2WAV converts a .SPX file (or a folder of .SPX files) to .WAV file(s)");
            o.WriteLine();
            o.WriteLine("    WAV2LH converts a .WAV file (or a folder of .WAV files) to .LH file(s)");
            o.WriteLine("    LH2WAV converts a .LH file (or a folder of .LH files) to .WAV file(s)");
            o.WriteLine();
            o.WriteLine("    LH2SPX converts a .LH file (or a folder of .LH files) to .SPX file(s)");
            o.WriteLine("    SPX2LH converts a .SPX file (or a folder of .SPX files) to .LH file(s)");
            o.WriteLine();
            o.WriteLine("-------------------------------------------------------------------------------");
        }

        private static void XML2FRAG(string fragXmlFile, string outputBinFile)
        {
            var fragFile = FragFile.LoadFromXml(fragXmlFile);
            fragFile.SaveAsBinary(outputBinFile);
        }

        private static void FRAG2XML(string fragBinFile, string outputXmlFile)
        {
            var fragFile = FragFile.LoadFromBinary(fragBinFile);
            fragFile.SaveAsXml(outputXmlFile);
        }

        private static void COMM2XML(string commBinFile, string outputXmlFile)
        {
            var commFile = CommFile.LoadFromBinary(commBinFile);
            commFile.SaveAsXml(outputXmlFile);
        }

        private static void XML2COMM(string commXmlFile, string outputBinFile)
        {
            var commFile = CommFile.LoadFromXml(commXmlFile);
            commFile.SaveAsBinary(outputBinFile);
        }

        private static void EVAL2XML(string evalBinFile, string outputXmlFile)
        {
            var evalFile = EvalFile.LoadFromBinary(evalBinFile);
            evalFile.SaveAsXml(outputXmlFile);
        }

        private static void XML2EVAL(string evalXmlFile, string outputBinFile)
        {
            var evalFile = EvalFile.LoadFromXml(evalXmlFile);
            evalFile.SaveAsBinary(outputBinFile);
        }

        private static int TLK2COMPRESSED(string tlkFilePath, string outputFileOrFolder, CodecType codecType,
                                          uint? fileNum)
        {
            var extension = "." + codecType;
            var tlkFile = TlkFile.Load(tlkFilePath);

            var tlkFileCodecType = tlkFile.DetectTlkFileCodecType();
            if (tlkFileCodecType != codecType)
            {
                if (tlkFileCodecType != CodecType.Unknown)
                {
                    Console.Error.WriteLine(
                        string.Format(
                            "Invalid <codec> argument: {0} was built using files that were encoded using the {1} codec, not the {2} codec.",
                            tlkFilePath, tlkFileCodecType, codecType));
                    return ERR_LEVEL__ERROR_OCCURRED;
                }
            }
            Console.WriteLine(string.Format("Extracting {0} file(s) from {1}...", extension, tlkFilePath));
            var successful = 0;
            var failed = 0;
            for (var i = 0; i < tlkFile.Records.Length; i++)
            {
                if (fileNum.HasValue && fileNum != i) continue;
                try
                {
                    var dataItem = tlkFile.Records[i];
                    string thisFileName;
                    if (fileNum.HasValue && !Directory.Exists(outputFileOrFolder))
                    {
                        thisFileName = outputFileOrFolder;
                    }
                    else
                    {
                        thisFileName = Path.Combine(outputFileOrFolder, i + extension);
                    }
                    Console.Write("Extracting " + thisFileName + "...");
                    tlkFile.WriteRecordToFile(dataItem, thisFileName);
                    Console.Write("Completed.");
                    successful++;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(string.Format("Error: {0}", ex));
                    failed++;
                }
                Console.WriteLine();
            }
            if (failed == 0 && successful == 0) failed = 1;
            Console.WriteLine(string.Format("Finished extracting {0} successful / {1} failed {2} file(s) from {3}.",
                                            successful, failed, extension, tlkFilePath));
            return failed == 0 ? ERR_LEVEL__SUCCESS : ERR_LEVEL__ERROR_OCCURRED;
        }

        private static void TLK2WAV(string tlkFilePath, string outputFileOrFolder, uint? fileNum)
        {
            const string extension = ".WAV";
            var tlkFile = TlkFile.Load(tlkFilePath);

            var tlkFileCodec = tlkFile.DetectTlkFileCodecType();
            Console.WriteLine(string.Format("Extracting {0} file(s) from {1} using {2} codec...", extension, tlkFilePath,
                                            tlkFileCodec));
            var successful = 0;
            var failed = 0;
            for (var i = 0; i < tlkFile.Records.Length; i++)
            {
                if (fileNum.HasValue && fileNum != i) continue;
                try
                {
                    var dataItem = tlkFile.Records[i];
                    string thisFileName;
                    if (fileNum.HasValue && !Directory.Exists(outputFileOrFolder))
                    {
                        thisFileName = outputFileOrFolder;
                    }
                    else
                    {
                        thisFileName = Path.Combine(outputFileOrFolder, i + extension);
                    }
                    Console.Write("Extracting " + thisFileName + "...");
                    tlkFile.DecompressRecordAndWriteToFile(tlkFileCodec, dataItem, thisFileName);
                    Console.Write("Completed.");
                    successful++;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(string.Format("Error: {0}", ex));
                    failed++;
                }
                Console.WriteLine();
            }
            if (failed == 0 && successful == 0) failed = 1;
            Console.WriteLine(string.Format("Finished extracting {0} successful / {1} failed {2} file(s) from {3}.",
                                            successful, failed, extension, tlkFilePath));
        }

        private static void WAV2TLK(string wavsFileOrFolder, string tlkFilePath, CodecType codecType, uint? fileNum)
        {
            var codecTypeDesc = codecType == CodecType.LH ? "LH" : codecType == CodecType.SPX ? "SPX" : "Unknown";

            var wavsToInclude = new Dictionary<uint, string>();
            if (fileNum.HasValue)
            {
                wavsToInclude.Add(fileNum.Value, wavsFileOrFolder);
            }
            else
            {
                var files = Directory.GetFiles(wavsFileOrFolder, "*.wav");
                Array.Sort(files, new NumericComparer());
                foreach (var file in files)
                {
                    uint rslt;
                    var parsed = UInt32.TryParse(Path.GetFileNameWithoutExtension(file), out rslt);
                    if (parsed)
                    {
                        wavsToInclude.Add(rslt, file);
                    }
                }
            }
            if (fileNum.HasValue || new FileInfo(tlkFilePath).Exists)
            {
                Console.WriteLine(string.Format("Importing {0} .WAV(s) (using {1} codec) into {2}...",
                                                wavsToInclude.Count, codecTypeDesc, tlkFilePath));
                TlkFile.ImportWAVFiles(codecType, wavsToInclude, tlkFilePath, Console.Out);
                Console.WriteLine(string.Format("Finished importing {0} .WAV(s) (using {1} codec) into {2}.",
                                                wavsToInclude.Count, codecTypeDesc, tlkFilePath));
            }
            else
            {
                Console.WriteLine(string.Format("Creating new .TLK file {0} from .WAV(s) (using {1} codec)...",
                                                tlkFilePath, codecTypeDesc));
                TlkFile.BuildFromWAVFiles(codecType, new List<string>(wavsToInclude.Values).ToArray(), tlkFilePath,
                                          Console.Out);
                Console.WriteLine(string.Format("Finished creating new .TLK file {0} from .WAV(s) (using {1} codec).",
                                                tlkFilePath, codecTypeDesc));
            }
        }

        private static void COMPRESSED2TLK(string compressedFileOrFolder, string tlkFilePath, CodecType codecType,
                                           uint? fileNum)
        {
            var extension = "." + codecType;
            var compressedFilesToInclude = new Dictionary<uint, string>();
            if (!extension.StartsWith(".")) extension = "." + extension;
            if (fileNum.HasValue)
            {
                compressedFilesToInclude.Add(fileNum.Value, compressedFileOrFolder);
            }
            else
            {
                var files = Directory.GetFiles(compressedFileOrFolder, "*" + extension);
                Array.Sort(files, new NumericComparer());
                foreach (var file in files)
                {
                    uint rslt;
                    var parsed = UInt32.TryParse(Path.GetFileNameWithoutExtension(file), out rslt);
                    if (parsed)
                    {
                        compressedFilesToInclude.Add(rslt, file);
                    }
                }
            }
            if (fileNum.HasValue || new FileInfo(tlkFilePath).Exists)
            {
                Console.WriteLine(string.Format("Importing {0} .{1} file(s) into {2}...", compressedFilesToInclude.Count,
                                                codecType, tlkFilePath));
                TlkFile.ImportCompressedFiles(compressedFilesToInclude, tlkFilePath, codecType, Console.Out);
                Console.WriteLine(string.Format("Finished importing {0} .{1} file(s) into {2}.",
                                                compressedFilesToInclude.Count, codecType, tlkFilePath));
            }
            else
            {
                Console.WriteLine(string.Format("Creating new .TLK file {0} from {1} file(s)...", tlkFilePath, extension));
                TlkFile.BuildFromCompressedAudioFiles(new List<string>(compressedFilesToInclude.Values).ToArray(),
                                                      tlkFilePath, Console.Out);
                Console.WriteLine(string.Format("Finished creating new .TLK file {0} from {1} file(s).", tlkFilePath,
                                                extension));
            }
        }

        private static void SPX2LH(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.SPX, CodecType.LH);
        }

        private static void LH2SPX(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.LH, CodecType.SPX);
        }

        private static void LH2WAV(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.LH, CodecType.Unknown);
        }

        private static void WAV2LH(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.Unknown, CodecType.LH);
        }

        private static void SPX2WAV(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.SPX, CodecType.Unknown);
        }

        private static void WAV2SPX(string inputFileOrFolderSpec, string outputFileOrFolderSpec)
        {
            ConvertCompressedFileFormats(inputFileOrFolderSpec, outputFileOrFolderSpec, CodecType.Unknown, CodecType.SPX);
        }


        private static void ConvertCompressedFileFormats(string sourceFileOrFolderSpec, string outputFileOrFolderSpec,
                                                         CodecType sourceCodec, CodecType targetCodec)
        {
            string[] inputFiles;
            var sourceCodecString = sourceCodec.ToString();
            if (sourceCodecString == "Unknown") sourceCodecString = "WAV";
            var targetCodecString = targetCodec.ToString();
            if (targetCodecString == "Unknown") targetCodecString = "WAV";

            if (Directory.Exists(sourceFileOrFolderSpec))
            {
                inputFiles = Directory.GetFiles(sourceFileOrFolderSpec, "*." + sourceCodecString);
            }
            else
            {
                inputFiles = new[] {sourceFileOrFolderSpec};
            }
            Array.Sort(inputFiles, new NumericComparer());
            if (inputFiles.Length == 0)
            {
                Console.WriteLine(string.Format("No {0} file(s) were found to convert!", "." + sourceCodecString));
                return;
            }
            Console.WriteLine(string.Format("Converting {0} .{1} file(s) to .{2} format...", inputFiles.Length,
                                            sourceCodecString, targetCodecString));
            foreach (var inputFile in inputFiles)
            {
                var targetDirectory = Directory.Exists(outputFileOrFolderSpec)
                                          ? outputFileOrFolderSpec
                                          : Path.GetDirectoryName(outputFileOrFolderSpec);
                var targetFileName = Path.Combine(targetDirectory,
                                                  Path.GetFileNameWithoutExtension(Path.GetFileName(inputFile)) + "." +
                                                  targetCodecString);

                Console.Write(string.Format("Converting {0} to {1}...", inputFile, targetFileName));
                using (var sourceFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    const int WAV_HEADER_SIZE = 44;

                    uint uncompressedLength = 0;
                    uint compressedLength = 0;
                    if (sourceCodec == CodecType.Unknown)
                    {
                        uncompressedLength = (uint) new FileInfo(inputFile).Length - WAV_HEADER_SIZE;
                    }
                    else
                    {
                        var fieldBytes = new byte[4];
                        sourceFileStream.Read(fieldBytes, 0, 4);
                        uncompressedLength = BitConverter.ToUInt32(fieldBytes, 0);
                        sourceFileStream.Read(fieldBytes, 0, 4);
                        compressedLength = BitConverter.ToUInt32(fieldBytes, 0);
                    }
                    using (var sourceFilePcmStream = new MemoryStream((int) uncompressedLength))
                    {
                        if (sourceCodec == CodecType.Unknown)
                        {
                            sourceFileStream.Seek(WAV_HEADER_SIZE, SeekOrigin.Begin);
                            var sourceFilePcmBytes = new byte[uncompressedLength];
                            sourceFileStream.Read(sourceFilePcmBytes, 0, (int) uncompressedLength);
                            sourceFilePcmStream.Write(sourceFilePcmBytes, 0, (int) uncompressedLength);
                            sourceFilePcmStream.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            TlkFile.DecompressAudioDataFromStream(sourceCodec, sourceFileStream, (int) compressedLength,
                                                                  (int) uncompressedLength, sourceFilePcmStream);
                            sourceFilePcmStream.Seek(0, SeekOrigin.Begin);
                        }
                        using (
                            var targetFileStream = new FileStream(targetFileName, FileMode.Create, FileAccess.ReadWrite)
                            )
                        {
                            if (targetCodec == CodecType.Unknown)
                            {
                                var pcmBuffer = new byte[uncompressedLength];
                                sourceFilePcmStream.Read(pcmBuffer, 0, pcmBuffer.Length);
                                TlkFile.WritePCMBitsAsWAVToStream(pcmBuffer, 0, pcmBuffer.Length, targetFileStream);
                            }
                            else
                            {
                                targetFileStream.Write(BitConverter.GetBytes(uncompressedLength), 0, 4);
                                targetFileStream.Seek(4, SeekOrigin.Current);
                                //leave room for writing the compressed file length
                                compressedLength =
                                    (uint)
                                    TlkFile.CompressAudioToStream(targetCodec, sourceFilePcmStream,
                                                                  (int) uncompressedLength, targetFileStream);
                                targetFileStream.Flush();
                                targetFileStream.Seek(4, SeekOrigin.Begin);
                                targetFileStream.Write(BitConverter.GetBytes(compressedLength), 0, 4);
                            }
                            targetFileStream.Flush();
                            targetFileStream.Close();
                        }
                    }
                }
                Console.WriteLine("Completed.");
            }
            Console.WriteLine(string.Format("Finished converting {0} .{1} file(s) to .{2} format.", inputFiles.Length,
                                            sourceCodecString, targetCodecString));
        }
    }
}