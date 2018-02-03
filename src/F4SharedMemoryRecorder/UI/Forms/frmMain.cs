using System;
using System.Diagnostics;
using System.Windows.Forms;
using F4SharedMemoryRecorder.Properties;
using log4net;
using F4SharedMemoryRecorder.Runtime;

namespace F4SharedMemoryRecorder.UI.Forms
{
    public partial class frmMain : Form, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (frmMain));
        private ISharedMemoryRecording _recording;
        protected bool _isDisposed;

        public frmMain()
        {
            InitializeComponent();
        }

        public string[] CommandLineSwitches { get; set; }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings.Default.Reload();
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void btnRecord_Click(object sender, EventArgs e)
        {
            Record();
        }

        private void Record()
        {
            if (_recording != null)
            {
                _recording.Record();
            }

        }
        private void FileNew()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.AddExtension = true;
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.DefaultExt = "SMX";
                dialog.Filter = "Sharedmem Recording (*.SMX)|*.SMX|All files (*.*)|*.*";
                dialog.FilterIndex = 0;
                dialog.OverwritePrompt = true;
                dialog.RestoreDirectory = false;
                dialog.ShowHelp = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Create Sharedmem Recording";
                dialog.ValidateNames = true;
                dialog.FileOk += (s, e) =>
                {
                    if (e.Cancel)
                    {
                        return;
                    }
                    if (_recording != null)
                    {
                        Common.Util.DisposeObject(_recording);
                    }
                    _recording = new SharedMemoryRecording(dialog.FileName);
                    _recording.Stopped += _recording_Stopped;
                    _recording.PlaybackStarted += _recording_PlaybackStarted;
                    _recording.RecordingStarted += _recording_RecordingStarted;
                    _recording.PlaybackProgress += _recording_PlaybackProgress;
                    btnRecord.Enabled = true;
                    btnPlay.Enabled = false;
                    btnLoop.Enabled = false;
                    btnStop.Enabled = false;
                    statusLabel.Text = Common.Win32.Paths.Util.Compact(_recording.FileName, 48);
                };
                dialog.ShowDialog();
            }

        }
        private void FileOpen()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.AddExtension = true;
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.DefaultExt = "SMX";
                dialog.Filter = "Sharedmem Recording (*.SMX)|*.SMX|All files (*.*)|*.*";
                dialog.FilterIndex = 0;
                dialog.Multiselect = false;
                dialog.ReadOnlyChecked = false;
                dialog.RestoreDirectory = false;
                dialog.ShowHelp = false;
                dialog.ShowReadOnly = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Open Sharedmem Recording";
                dialog.ValidateNames = true;
                dialog.FileOk += (s, e) =>
                {
                    if (e.Cancel)
                    {
                        return;
                    }
                    if (_recording != null)
                    {
                        Common.Util.DisposeObject(_recording);
                    }
                    _recording = new SharedMemoryRecording(dialog.FileName);
                    _recording.Stopped += _recording_Stopped;
                    _recording.PlaybackStarted += _recording_PlaybackStarted;
                    _recording.RecordingStarted += _recording_RecordingStarted;
                    _recording.PlaybackProgress += _recording_PlaybackProgress;
                    btnRecord.Enabled = false;
                    btnPlay.Enabled = true;
                    btnLoop.Enabled = true;
                    btnStop.Enabled = false;
                    statusLabel.Text = Common.Win32.Paths.Util.Compact(_recording.FileName, 48);
                };
                dialog.ShowDialog();

            }
        }
        private void _recording_Stopped(object sender, StoppedEventArgs e)
        {
            slider.Value = 0;
            slider.Visible = true;
            btnRecord.Enabled = false;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            btnLoop.Enabled = true;
        }
        private void _recording_PlaybackStarted(object sender, PlaybackStartedEventArgs e)
        {
            slider.Value = 0;
            slider.Visible = true;
            btnRecord.Enabled = false;
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            btnLoop.Enabled = true;
        }
        private void _recording_RecordingStarted(object sender, RecordingStartedEventArgs e)
        {
            slider.Value = 0;
            slider.Visible = false;
            btnRecord.Enabled = false;
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            btnLoop.Enabled = false;
        }
        private void _recording_PlaybackProgress(object sender, PlaybackProgressEventArgs e)
        {
            slider.Value = (int)(e.ProgressPercentage * 100);
        }
        private void Play()
        {
            if (_recording != null)
            {
                _recording.Play();
            }
        }
        private void Stop()
        {
            if (_recording != null)
            {
                _recording.Stop();
            }
        }

        private void Quit()
        {
            Stop();
            LogManager.Shutdown();
            try
            {
                Dispose();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                Application.Exit();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Quit();
        }


        #region Destructors

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~frmMain()
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //dispose of managed resources here
                }
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;

        }

        #endregion

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void btnLoop_Click(object sender, EventArgs e)
        {
            if (_recording !=null)
            {
                _recording.LoopOnPlayback = btnLoop.Checked;
            }
        }
    }
}