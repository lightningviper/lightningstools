using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace TlkToolTest
{
    [TestClass]
    public class TlkToolTest
    {
        public TlkToolTest()
        {
            InitializeTest();
        }

        private void InitializeTest()
        {
            string targetFolder = @"c:\tlkTool";
            if (!File.Exists(Path.Combine(targetFolder, @"ST80W.DLL")))
            {
                File.Copy(@"C:\Microprose\Falcon4\ST80W.dll", Path.Combine(targetFolder, @"ST80W.DLL"));
            }
            DeleteFile(targetFolder, "commFile.bin");
            DeleteFile(targetFolder, "evalFile.bin");
            DeleteFile(targetFolder, "fragFile.bin");
            DeleteFile(targetFolder, "commFile.xml");
            DeleteFile(targetFolder, "evalFile.xml");
            DeleteFile(targetFolder, "fragFile.xml");
            DeleteFile(targetFolder, "falcon.tlk");
            DeleteFile(targetFolder, "falcon2.tlk");
            CopyFile(@"C:\Microprose\Falcon4\sounds\falcon.tlk", targetFolder);
            CopyFile(@"C:\Microprose\Falcon4\sounds\evalFile.bin", targetFolder);
            CopyFile(@"C:\Microprose\Falcon4\sounds\fragFile.bin", targetFolder);
            CopyFile(@"C:\Microprose\Falcon4\sounds\commFile.bin", targetFolder);
        }
        private void CopyFile(string sourceFile, string destinationFolder)
        {
            File.Copy(sourceFile, Path.Combine(destinationFolder, Path.GetFileName(sourceFile)));
        }
        private void DeleteFile(string path, string file)
        {
            string fullPath=Path.Combine(path, file);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestAllQuickly()
        {
            TestCommFileRoundTrip();
            TestEvalFileRoundTrip();
            TestFragFileRoundTrip();

            TestTlk2LhSingleFile();
            TestTlk2WavSingleFile();
            
            TestWav2LhSingleFile();
            TestWav2SpxSingleFile();

            TestLh2SpxSingleFile();
            TestLh2WavSingleFile();
            TestLh2TlkSingleFile();

            TestTlk2SpxSingleFile();
            TestSpx2LhSingleFile();
            TestSpx2WavSingleFile();
            TestSpx2TlkSingleFile();
        }
        
        [TestMethod]
        public void TestComm2Xml()
        {
            string commFilePath = @"c:\tlktool\commFile.bin";
            string xmlFilePath = @"c:\tlktool\commFile.xml";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_COMM2XML,
                commFilePath,
                xmlFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestEval2Xml()
        {
            string evalFilePath = @"c:\tlktool\evalFile.bin";
            string xmlFilePath = @"c:\tlktool\evalFile.xml";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_EVAL2XML,
                evalFilePath,
                xmlFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }

        [TestMethod]
        public void TestFrag2Xml()
        {
            string fragFilePath = @"c:\tlktool\fragFile.bin";
            string xmlFilePath = @"c:\tlktool\fragFile.xml";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_FRAG2XML,
                fragFilePath,
                xmlFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestXml2Comm()
        {
            string xmlFilePath = @"c:\tlktool\commFile.xml";
            string commFilePath = @"c:\tlktool\commFile.bin";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_XML2COMM,
                xmlFilePath,
                commFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestXml2Eval()
        {
            string xmlFilePath = @"c:\tlktool\evalFile.xml";
            string evalFilePath = @"c:\tlktool\evalFile.bin";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_XML2EVAL,
                xmlFilePath,
                evalFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestXml2Frag()
        {
            string xmlFilePath = @"c:\tlktool\fragFile.xml";
            string fragFilePath = @"c:\tlktool\fragFile.bin";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_XML2FRAG,
                xmlFilePath,
                fragFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        
        [TestMethod]
        public void TestTlk2WavFolder()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string wavFolder = @"c:\tlktool\wav\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2WAV,
                tlkFilePath,
                wavFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestTlk2LhFolder()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string lhFolder = @"c:\tlktool\lh\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2LH,
                tlkFilePath,
                lhFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestTlk2SpxFolder()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2SPX,
                tlkFilePath,
                spxFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2TlkFolder()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string lhFolder = @"c:\tlktool\lh\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2TLK,
                lhFolder,
                tlkFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2TlkFolder()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2TLK,
                spxFolder,
                tlkFilePath
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestTlk2LhFolderRoundTrip()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            TestTlk2LhFolder(); //export TLK to .LHs
            TestLh2TlkFolder(); //recreate TLK from .LHs
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(tlkFilePath).Name, hashOfOriginal, hashOfRecreated));
        }
        [TestMethod]
        public void TestTlk2SpxFolderRoundTrip()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            TestTlk2SpxFolder(); //export TLK to .SPXs
            TestSpx2TlkFolder(); //recreate TLK from .SPXs
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(tlkFilePath).Name, hashOfOriginal, hashOfRecreated));
        }
        [TestMethod]
        public void TestTlk2LhSingleFileRoundTrip()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            TestTlk2LhSingleFile(); //export one TLK to .LH
            TestLh2TlkSingleFile(); //import one TLK from .LH
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(tlkFilePath).Name, hashOfOriginal, hashOfRecreated));
        }
        [TestMethod]
        public void TestTlk2SpxSingleFileRoundTrip()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            TestTlk2SpxSingleFile(); //export one TLK to .SPX
            TestSpx2TlkSingleFile(); //import one TLK from .SPX
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(tlkFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(tlkFilePath).Name, hashOfOriginal, hashOfRecreated));
        }
        [TestMethod]
        public void TestWav2TlkFolderUsingSpxCodec()
        {
            string wavFolder = @"c:\tlktool\wav\";
            string tlkFilePath = @"c:\tlktool\falconspx2.tlk";
            string codec = "SPX";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2TLK,
                wavFolder,
                tlkFilePath, 
                codec
            };

            if (File.Exists(tlkFilePath)) File.Delete(tlkFilePath);
            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024);
        }

        [TestMethod]
        public void TestWav2TlkFolderUsingLhCodec()
        {
            string wavFolder = @"c:\tlktool\wav\";
            string tlkFilePath = @"c:\tlktool\falcon2.tlk";
            string codec = "LH";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2TLK,
                wavFolder,
                tlkFilePath, 
                codec
            };

            if (File.Exists(tlkFilePath)) File.Delete(tlkFilePath);
            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024); 
        }

        [TestMethod]
        public void TestWav2TlkSingleFileUsingLhCodec()
        {
            string wavFile = @"c:\tlktool\wav\300.wav";
            string tlkFilePath = @"c:\tlktool\falcon2.tlk";
            string codec = "LH";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2TLK,
                wavFile,
                tlkFilePath, 
                codec
            };

            if (File.Exists(tlkFilePath)) File.Delete(tlkFilePath);
            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024);
        }
        [TestMethod]
        public void TestWav2TlkSingleFileUsingSpxCodec()
        {
            string wavFile = @"c:\tlktool\wav\300.wav";
            string tlkFilePath = @"c:\tlktool\falconspx2.tlk";
            string codec = "SPX";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2TLK,
                wavFile,
                tlkFilePath, 
                codec
            };

            if (File.Exists(tlkFilePath)) File.Delete(tlkFilePath);
            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024);
        }

        [TestMethod]
        public void TestCommFileRoundTrip()
        {
            string commFilePath = @"c:\tlktool\commFile.bin";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(commFilePath));
            TestComm2Xml(); //export to XML file
            TestXml2Comm(); //recreate from XML file
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(commFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(commFilePath).Name, hashOfOriginal, hashOfRecreated));
        }

        [TestMethod]
        public void TestEvalFileRoundTrip()
        {
            string evalFilePath = @"c:\tlktool\evalFile.bin";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(evalFilePath));
            TestEval2Xml(); //export to XML file
            TestXml2Eval(); //recreate from XMLfile
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(evalFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(evalFilePath).Name, hashOfOriginal, hashOfRecreated));
        }

        [TestMethod]
        public void TestFragFileRoundTrip()
        {
            string fragFilePath = @"c:\tlktool\fragFile.bin";
            string hashOfOriginal = GetHexStringFromBytes(GetMD5HashOfFile(fragFilePath)); 
            TestFrag2Xml(); //export to XML file
            TestXml2Frag(); //recreate from XMLfile
            string hashOfRecreated = GetHexStringFromBytes(GetMD5HashOfFile(fragFilePath));
            Assert.AreEqual(hashOfRecreated, hashOfOriginal,
                string.Format("Original {0} file  had MD5 hash:{1} but recreated file had MD5 hash:{2}",
                    new FileInfo(fragFilePath).Name, hashOfOriginal, hashOfRecreated));
        }
        [TestMethod]
        public void TestTlk2WavSingleFile()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string wavFolder = @"c:\tlktool\wav\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2WAV,
                tlkFilePath,
                wavFolder, 
                "300"
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }

        [TestMethod]
        public void TestTlk2WavRoundTripViaLH()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            TestTlk2WavFolder(); //export TLK to .WAVs 
            TestWav2TlkFolderUsingLhCodec(); //recreate TLK from WAVs
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024);

        }
        [TestMethod]
        public void TestTlk2WavRoundTripViaSpx()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            TestTlk2WavFolder(); //export TLK to .WAVs 
            TestWav2TlkFolderUsingSpxCodec(); //recreate TLK from WAVs
            FileInfo fi = new FileInfo(tlkFilePath);
            Assert.IsTrue(fi.Exists && fi.Length > 25 * 1024 * 1024);
        }
        [TestMethod]
        public void TestTlk2SpxSingleFile()
        {
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string spxFolder= @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2SPX,
                tlkFilePath,
                spxFolder,
                "300"
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2TlkSingleFile()
        {
            string spxFolder = @"c:\tlktool\spx\";
            string tlkFilePath = @"c:\tlktool\falconspx.tlk";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2TLK,
                spxFolder,
                tlkFilePath,
                "300"
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2TlkSingleFile()
        {
            string lhFolder = @"c:\tlktool\lh\";
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2TLK,
                lhFolder,
                tlkFilePath,
                "300"
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestTlk2LhSingleFile()
        {
            string tlkFilePath = @"c:\tlktool\falcon.tlk";
            string lhFolder = @"c:\tlktool\lh\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_TLK2LH,
                tlkFilePath,
                lhFolder,
                "300"
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2LhSingleFile()
        {
            string spxFile = @"c:\tlktool\spx\300.spx";
            string lhFile = @"c:\tlktool\lh\300.lh";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2LH,
                spxFile,
                lhFile
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2LhFolder()
        {
            string spxFolder = @"c:\tlktool\spx\";
            string lhFolder = @"c:\tlktool\lh\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2LH,
                spxFolder,
                lhFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2SpxFolder()
        {
            string lhFolder = @"c:\tlktool\lh\";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2SPX,
                lhFolder,
                spxFolder
                
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2SpxSingleFile()
        {
            string lhFile = @"c:\tlktool\lh\300.lh";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2SPX,
                lhFile,
                spxFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2WavFolder()
        {
            string lhFolder = @"c:\tlktool\lh\";
            string wavFolder = @"c:\tlktool\wav\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2WAV,
                lhFolder,
                wavFolder
                
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestLh2WavSingleFile()
        {
            string lhFile = @"c:\tlktool\lh\300.lh";
            string wavFile = @"c:\tlktool\wav\300.wav";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_LH2WAV,
                lhFile,
                wavFile
                
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestWav2LhFolder()
        {
            string wavFolder = @"c:\tlktool\wav\";
            string lhFolder= @"c:\tlktool\lh\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2LH,
                wavFolder,
                lhFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestWav2LhSingleFile()
        {
            string wavFile = @"c:\tlktool\wav\300.wav";
            string lhFile = @"c:\tlktool\lh\300.lh";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2LH,
                wavFile,
                lhFile
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestWav2SpxFolder()
        {
            string wavFolder = @"c:\tlktool\wav\";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2SPX,
                wavFolder,
                spxFolder 
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }

        [TestMethod]
        public void TestWav2SpxSingleFile()
        {
            string wavFile = @"c:\tlktool\wav\300.wav";
            string spxFile = @"c:\tlktool\spx\300.spx";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_WAV2SPX,
                wavFile,
                spxFile 
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2WavFolder()
        {
            string wavFolder = @"c:\tlktool\wav\";
            string spxFolder = @"c:\tlktool\spx\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2WAV,
                spxFolder,
                wavFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        [TestMethod]
        public void TestSpx2WavSingleFile()
        {
            string spxFile = @"c:\tlktool\spx\300.spx";
            string wavFolder = @"c:\tlktool\wav\";
            string[] args = new string[] { 
                TlkTool.Program.ACTION_SPX2WAV,
                spxFile,
                wavFolder
            };

            int result = TlkTool.Program.Main(args);
            Assert.AreEqual(0, result, string.Format("TlkTool exited with status code {0}", result));
        }
        private static string GetHexStringFromBytes(byte[] bytes)
        {
            if (bytes == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x"));
            }
            return sb.ToString();
        }
        private static byte[] GetMD5HashOfFile(string filePath)
        {
            byte[] hash = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(fs);
                fs.Close();
            }
            return hash;
        }
      
    }
}
