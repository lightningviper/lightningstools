using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using F4Utils.Resources;

namespace F4ResourceFileEditor
{
    public partial class frmMain : Form
    {
        private EditorState _editorState = new EditorState();
        private ResourceBundleReader _reader = new ResourceBundleReader();

        public frmMain()
        {
            var SaveFilter = SetUnhandledExceptionFilter(IntPtr.Zero);
            InitializeComponent();
            SetUnhandledExceptionFilter(SaveFilter);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.WorkerSupportsCancellation = true;
            UpdateMenus();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            FileOpen();
            UpdateView();
        }

        private void UpdateMenus()
        {
            if (_editorState.ChangesMade)
            {
                mnuFileSaveAs.Enabled = true;
                mnuFileSave.Enabled = true;
            }
            else
            {
                mnuFileSaveAs.Enabled = false;
                mnuFileSave.Enabled = false;
            }
        }

        private void UpdateView()
        {
            tvResources.Nodes.Clear();
            foreach (var thisStateRecord in _editorState.Resources)
            {
                var thisResourceID = thisStateRecord.Key;
                var thisResource = thisStateRecord.Value;
                var thisResourceType = thisResource.ResourceType.ToString();
                var newNode = tvResources.Nodes.Add(thisResourceID, thisResourceID + "(" + thisResourceType + ")");
                newNode.Tag = thisResource;
            }
            Text = Application.ProductName + " - ";
            if (_editorState.ChangesMade)
            {
                Text = Text + "*";
            }
            if (_editorState.FilePath != null)
            {
                Text = Text + new FileInfo(_editorState.FilePath).Name;
            }
        }

        private void FileOpen()
        {
            var dlgOpen = new OpenFileDialog();
            dlgOpen.AddExtension = true;
            dlgOpen.AutoUpgradeEnabled = true;
            dlgOpen.CheckFileExists = true;
            dlgOpen.CheckPathExists = true;
            dlgOpen.DefaultExt = ".rsc";
            dlgOpen.Filter = "Falcon 4 Resource Files (*.rsc)|*.rsc";
            dlgOpen.FilterIndex = 0;
            dlgOpen.DereferenceLinks = true;
            //dlgOpen.InitialDirectory = new FileInfo(Application.ExecutablePath).DirectoryName;
            dlgOpen.Multiselect = false;
            dlgOpen.ReadOnlyChecked = false;
            dlgOpen.RestoreDirectory = true;
            dlgOpen.ShowHelp = false;
            dlgOpen.ShowReadOnly = false;
            dlgOpen.SupportMultiDottedExtensions = true;
            dlgOpen.Title = "Open Resource File";
            dlgOpen.ValidateNames = true;
            var result = dlgOpen.ShowDialog(this);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            else if (result == DialogResult.OK)
            {
                LoadResourceFile(dlgOpen.FileName);
            }
        }

        private string GetFileNameWithoutExtension(string fileName)
        {
            var toReturn = fileName.Substring(0, fileName.Length - new FileInfo(fileName).Extension.Length);
            return toReturn;
        }

        private void LoadResourceFile(string resourceFilePath)
        {
            if (string.IsNullOrEmpty(resourceFilePath)) throw new ArgumentNullException("resourceFilePath");
            var resourceFileFI = new FileInfo(resourceFilePath);
            if (!resourceFileFI.Exists) throw new FileNotFoundException(resourceFilePath);
            var resourceIndexFileFI =
                new FileInfo(resourceFileFI.DirectoryName + Path.DirectorySeparatorChar +
                             GetFileNameWithoutExtension(resourceFileFI.Name) + ".idx");
            if (!resourceIndexFileFI.Exists) throw new FileNotFoundException(resourceIndexFileFI.FullName);

            var oldEditorState = (EditorState) _editorState.Clone();
            try
            {
                _editorState = new EditorState();
                _editorState.FilePath = resourceFilePath;
                _reader = new ResourceBundleReader();
                _reader.Load(resourceIndexFileFI.FullName);
                for (var i = 0; i < _reader.NumResources; i++)
                {
                    var thisResourceType = _reader.GetResourceType(i);
                    var thisResourceStateRecord = new EditorState.F4Resource();
                    thisResourceStateRecord.ResourceType = thisResourceType;
                    thisResourceStateRecord.ID = _reader.GetResourceID(i);
                    switch (thisResourceType)
                    {
                        case ResourceType.Unknown:
                            break;
                        case ResourceType.ImageResource:
                            var resourceData = _reader.GetImageResource(i);
                            using (var ms = new MemoryStream())
                            {
                                resourceData.Save(ms, ImageFormat.Bmp);
                                ms.Flush();
                                ms.Seek(0, SeekOrigin.Begin);
                                thisResourceStateRecord.Data = ms.ToArray();
                                ms.Close();
                            }
                            break;
                        case ResourceType.SoundResource:
                            thisResourceStateRecord.Data = _reader.GetSoundResource(thisResourceStateRecord.ID);
                            break;
                        case ResourceType.FlatResource:
                            thisResourceStateRecord.Data = _reader.GetFlatResource(thisResourceStateRecord.ID);
                            break;
                        default:
                            break;
                    }
                    _editorState.Resources.Add(thisResourceStateRecord.ID, thisResourceStateRecord);
                }
                _editorState.ChangesMade = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error occurred while loading the file.\n\n {0}", e.Message),
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1);
                _editorState = oldEditorState;
            }
        }

        private void tvResources_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node;
            var thisResource = (EditorState.F4Resource) e.Node.Tag;
            splitContainer1.Panel2.Controls.Clear();

            switch (thisResource.ResourceType)
            {
                case ResourceType.Unknown:
                    break;
                case ResourceType.ImageResource:
                    RenderImageResource(thisResource.ID);
                    break;
                case ResourceType.SoundResource:
                    RenderSoundResource(thisResource.ID);
                    break;
                case ResourceType.FlatResource:
                    break;
                default:
                    break;
            }
        }

        private void RenderImageResource(string resourceId)
        {
            splitContainer1.Panel2.Controls.Clear();
            Bitmap bmp = null;
            using (var ms = new MemoryStream(_editorState.Resources[resourceId].Data))
            {
                bmp = (Bitmap) Image.FromStream(ms);
            }

            var pb = new PictureBox();
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Width = bmp.Width;
            pb.Height = bmp.Height;
            pb.Image = bmp;
            splitContainer1.Panel2.Controls.Add(pb);
        }

        private void RenderSoundResource(string resourceId)
        {
            splitContainer1.Panel2.Controls.Clear();
            var toPlay = new Button();
            toPlay.Text = "Play Sound...";
            toPlay.Click += toPlay_Click;
            toPlay.Tag = _editorState.Resources[resourceId].Data;
            splitContainer1.Panel2.Controls.Add(toPlay);
        }

        private void toPlay_Click(object sender, EventArgs e)
        {
            while (backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
            }
            backgroundWorker1.RunWorkerAsync(sender);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var soundData = (byte[]) ((Button) e.Argument).Tag;

            ((Button) e.Argument).Invoke(new InvokeWithParm(DisablePlayButton), e.Argument);
            BackgroundPlay(soundData);
            ((Button) e.Argument).Invoke(new InvokeWithParm(EnablePlayButton), e.Argument);
        }

        private void EnablePlayButton(object argument)
        {
            ((Button) argument).Enabled = true;
        }

        private void DisablePlayButton(object argument)
        {
            ((Button) argument).Enabled = false;
        }

        private void BackgroundPlay(object soundBytes)
        {
            var soundData = (byte[]) soundBytes;
            var tempFile = Path.GetTempFileName();
            try
            {
                using (var fs = new FileStream(tempFile, FileMode.Create))
                {
                    fs.Write(soundData, 0, soundData.Length);
                    fs.Flush();
                    fs.Close();
                }
                PlaySound(tempFile, IntPtr.Zero, SoundFlags.SND_FILENAME);
            }
            finally
            {
                try
                {
                    new FileInfo(tempFile).Delete();
                }
                catch (IOException)
                {
                }
            }
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void FileSave()
        {
            //   _editorState.FilePath
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            FileExit();
        }

        private void FileExit()
        {
            Application.Exit();
        }

        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode,
            ThrowOnUnmappableChar = true)]
        private static extern bool PlaySound(string szSound, IntPtr hMod, SoundFlags flags);

        [DllImport("kernel32.dll")]
        private static extern IntPtr SetUnhandledExceptionFilter(IntPtr lpFilter);

        #region Nested type: EditorState

        [Serializable]
        internal class EditorState
        {
            public bool ChangesMade;
            [NonSerialized] public string FilePath;
            public Dictionary<string, F4Resource> Resources = new Dictionary<string, F4Resource>();


            public object Clone()
            {
                EditorState cloned = null;
                using (var ms = new MemoryStream(1000))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(ms, this);
                    ms.Seek(0, SeekOrigin.Begin);
                    cloned = (EditorState) bf.Deserialize(ms);
                    ms.Close();
                }
                return cloned;
            }

            #region Nested type: F4Resource

            [Serializable]
            public class F4Resource
            {
                public byte[] Data;
                public string ID;
                public ResourceType ResourceType;
            }

            #endregion
        }

        #endregion

        #region Nested type: InvokeWithParm

        private delegate void InvokeWithParm(object parms);

        #endregion

        #region Nested type: InvokeWithParms

        private delegate void InvokeWithParms(object[] parms);

        #endregion
    }

    [Flags]
    public enum SoundFlags
    {
        SND_SYNC = 0x0000, // play synchronously (default) 
        SND_ASYNC = 0x0001, // play asynchronously 
        SND_NODEFAULT = 0x0002, // silence (!default) if sound not found 
        SND_MEMORY = 0x0004, // pszSound points to a memory file
        SND_LOOP = 0x0008, // loop the sound until next sndPlaySound 
        SND_NOSTOP = 0x0010, // don't stop any currently playing sound 
        SND_NOWAIT = 0x00002000, // don't wait if the driver is busy 
        SND_ALIAS = 0x00010000, // name is a registry alias 
        SND_ALIAS_ID = 0x00110000, // alias is a predefined ID
        SND_FILENAME = 0x00020000, // name is file name 
        SND_RESOURCE = 0x00040004 // name is resource name or atom 
    }
}