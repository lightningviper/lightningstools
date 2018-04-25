using BMSVoiceGen.IO.BMSFiles;
using BMSVoiceGen.TextToSpeechProviders;
using F4Utils.Speech;
using log4net;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
namespace BMSVoiceGen
{
    internal interface ITextToSpeechBatchConverter
    {
        void GenerateWAVs(string fragFilePath, string csvFilePath, string outputFolder, ITextToSpeechProvider textToSpeechProvider, int maxDegreeOfParallelism = -1);
    }
    internal class TextToSpeechBatchConverter: ITextToSpeechBatchConverter
    {
        private static ILog Log = LogManager.GetLogger(typeof(TextToSpeechBatchConverter));
        public void GenerateWAVs(string fragFilePath, string csvFilePath, string outputFolder, ITextToSpeechProvider textToSpeechProvider, int maxDegreeOfParallelism=-1)
        {
            var startTime = DateTime.Now;
            EnsureOutputFolderExists(outputFolder);
            var fragFile = LoadFragFile(fragFilePath);
            var csvLines = ATCVoiceFileReader.ReadBMSVoiceStringCSVFile(csvFilePath);
            var expectedNumWavsToGenerate = fragFile.headers.Sum(x => x.totalSpeakers);
            Log.Info($"Generating {expectedNumWavsToGenerate} WAVs...");
            var numWavFilesGenerated = 0;
            var numWavFilesSkipped = 0;

            Parallel.ForEach(fragFile.headers, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, header =>
            {
                var fragNum = header.fragHdrNbr;
                Parallel.ForEach(header.data, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, data =>
                {
                    var fileName = Path.Combine(outputFolder, $"{data.fileNbr}.WAV");
                    var voice = data.speaker;
                    var text = csvLines.Where(x => x.FragNum == fragNum).First().SpeakerText[voice];
                    var itemsProcessed = numWavFilesGenerated + numWavFilesSkipped;
                    var pctComplete = Math.Round(((float)(itemsProcessed) / (float)expectedNumWavsToGenerate) * 100, 2);
                    var timeElapsed = DateTime.Now.Subtract(startTime);
                    var timePerItem = timeElapsed.Ticks / (numWavFilesGenerated + 1);
                    var itemsRemaining = expectedNumWavsToGenerate - itemsProcessed;
                    var timeRemaining = TimeSpan.FromTicks(itemsRemaining * timePerItem);
                    if (!File.Exists(fileName))
                    {
                        Log.Info($"[{itemsProcessed} / {expectedNumWavsToGenerate} ({pctComplete:0.00}%), Time Remaining: {timeRemaining:hh\\:mm\\:ss}] Generating WAV file for Frag #:{fragNum:00000}; Speaker #:{voice:00}; Text=\"{text.Replace("\"", "\\\"")}\" | File:\"{fileName}\" ...");
                        try
                        {
                            ConvertTextToSpeech(text, voice, textToSpeechProvider, fileName);
                            Interlocked.Add(ref numWavFilesGenerated, 1);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message, e);
                        }
                    }
                    else
                    {
                        Log.Info($"[{itemsProcessed} / {expectedNumWavsToGenerate} ({pctComplete:0.00}%), Time Remaining: {timeRemaining:hh\\:mm\\:ss}] Skippping WAV file for Frag #:{fragNum:00000}; Speaker #:{voice:00}; Text=\"{text.Replace("\"", "\\\"")}\" | File:\"{fileName}\" already exists.");
                        Interlocked.Add(ref numWavFilesSkipped, 1);
                    }
                });
            });
            var totalTime = DateTime.Now.Subtract(startTime);
            if (numWavFilesGenerated + numWavFilesSkipped == expectedNumWavsToGenerate)
            {
                Log.Info($"Complete.  Generated { numWavFilesGenerated } .WAV files, skipped generating {numWavFilesSkipped} already-existing files. Total time elapsed: {totalTime:hh\\:mm\\:ss}");
            }
            else
            {
                Log.Error($"INCOMPLETE.  Only generated { numWavFilesGenerated } .WAV files, and skipped {numWavFilesSkipped} already-existing files, for a total of {numWavFilesGenerated + numWavFilesSkipped}, but anticipated generating and/or skipping a total of {expectedNumWavsToGenerate} files. Total time elapsed: {totalTime:hh\\:mm\\:ss}");
            }
        }

        private static FragFile LoadFragFile(string fragFilePath)
        {
            return new FileInfo(fragFilePath).Extension.EndsWith("XML", System.StringComparison.InvariantCultureIgnoreCase)
                   ? FragFile.LoadFromXml(fragFilePath)
                   : FragFile.LoadFromBinary(fragFilePath);
        }

        private static void EnsureOutputFolderExists(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
        }

        private static void ConvertTextToSpeech(string text, ushort voice, ITextToSpeechProvider textToSpeechProvider, string fileName)
        {
            if (string.Equals("(Key Mike)", text, StringComparison.OrdinalIgnoreCase))
            {
                SaveKeyMikeFile(fileName);
            }
            else
            {
                textToSpeechProvider.GenerateWAV(text, voice, fileName);
            }
        }

        private static void SaveKeyMikeFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resourceStream = assembly.GetManifestResourceStream(@"BMSVoiceGen.media.KeyMike.wav"))
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
        }
    }
}
