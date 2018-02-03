using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Timers;

namespace F4SharedMemoryRecorder.Runtime
{
    internal interface ISharedMemoryRecording: IDisposable
    {
        event EventHandler<RecordingStartedEventArgs> RecordingStarted;
        event EventHandler<PlaybackStartedEventArgs> PlaybackStarted;
        event EventHandler<StoppedEventArgs> Stopped;
        event EventHandler<PlaybackProgressEventArgs> PlaybackProgress;
        string FileName { get; }
        ushort SampleInterval { get; }
        ulong NumSamples { get; }
        ulong CurrentSample { get; }
        void Record();
        void Stop();
        void Play();
        bool LoopOnPlayback { get; set; }
    }
    internal class RecordingStartedEventArgs:EventArgs{}
    internal class PlaybackStartedEventArgs : EventArgs { }
    internal class StoppedEventArgs : EventArgs { }
    internal class PlaybackProgressEventArgs : EventArgs 
    {
        public float ProgressPercentage { get; internal set; }
    }
    internal class SharedMemoryRecording : ISharedMemoryRecording
    {
        public event EventHandler<RecordingStartedEventArgs> RecordingStarted;
        public event EventHandler<PlaybackStartedEventArgs> PlaybackStarted;
        public event EventHandler<StoppedEventArgs> Stopped;
        public event EventHandler<PlaybackProgressEventArgs> PlaybackProgress;

        private const int DEFAULT_SAMPLING_INTERVAL_MS = 20;
        private readonly Timer _recordingTimer = new Timer(DEFAULT_SAMPLING_INTERVAL_MS);
        private readonly Timer _playbackTimer = new Timer(DEFAULT_SAMPLING_INTERVAL_MS);
        private readonly F4SharedMem.Reader _sharedMemoryReader = new F4SharedMem.Reader();
        private readonly F4SharedMem.Writer _sharedMemoryWriter = new F4SharedMem.Writer();
        private BinaryWriter _gzipStreamBinaryWriter;
        private BinaryReader _gzipStreamBinaryReader;
        private FileStream _fileStream;
        private GZipStream _gzipStream;
        private object _playbackLock = new object();
        private object _recordingLock = new object();
        
        public SharedMemoryRecording(string fileName)
        {
            FileName = fileName;
            _recordingTimer.Elapsed += _recordingTimer_Elapsed;
            _playbackTimer.Elapsed += _playbackTimer_Elapsed;
        }


        public string FileName { get; private set; }
        public ushort SampleInterval { get; private set; }
        public ulong NumSamples { get; private set; }
        public ulong CurrentSample { get; private set; }
        public bool LoopOnPlayback { get; set; }
        public void Record()
        {
            lock (_recordingLock)
            {
                if (_playbackTimer.Enabled)
                {
                    _playbackTimer.Stop();
                }
                try
                {
                    _fileStream = new FileStream(path: FileName, mode: FileMode.Create, access: FileAccess.ReadWrite, share: FileShare.None, bufferSize: 4096, options: FileOptions.SequentialScan);
                    WriteHeader();
                    _gzipStream = new GZipStream(stream: _fileStream, compressionLevel: CompressionLevel.Optimal, leaveOpen: true);
                    _gzipStreamBinaryWriter = new BinaryWriter(output: _gzipStream, encoding: Encoding.UTF8, leaveOpen: true);
                }
                catch
                {
                    Stopped(this, null);
                    return;
                }
                if (RecordingStarted != null)
                {
                    RecordingStarted(this, null);
                }
                _recordingTimer.Start();
            }
        }

        public void Play()
        {
            lock (_playbackLock)
            {
                try
                {
                    _fileStream = new FileStream(path: FileName, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None, bufferSize: 4096, options: FileOptions.SequentialScan);
                    var header = ReadHeader();
                    NumSamples = header.NumSamples;
                    CurrentSample = 0;
                    SampleInterval = header.SampleInterval;
                    _gzipStream = new GZipStream(stream: _fileStream, mode: CompressionMode.Decompress, leaveOpen: true);
                    _gzipStreamBinaryReader = new BinaryReader(input: _gzipStream, encoding: Encoding.UTF8, leaveOpen: true);
                }
                catch (IOException)
                {
                    Stopped(this, null);
                    return;
                }
                if (PlaybackStarted != null)
                {
                    PlaybackStarted(this, null);
                }
                _playbackTimer.Start();
            }
        }

        public void Stop()
        {
            lock (_recordingLock)
            {
                if (_recordingTimer.Enabled)
                {
                    StopRecording();
                }
            }
            lock (_playbackLock)
            {
                if (_playbackTimer.Enabled)
                {
                    StopPlaying();
                }
            }
            if (Stopped != null)
            {
                Stopped(this, null);
            }
        }

        private void StopPlaying()
        {
            lock (_playbackLock)
            {
                _playbackTimer.Stop();
                CloseFileOpenedForPlay();
            }
        }

        private void CloseFileOpenedForPlay()
        {
            Common.Util.DisposeObject(_gzipStreamBinaryReader);
            Common.Util.DisposeObject(_gzipStream);
            Common.Util.DisposeObject(_fileStream);
            _gzipStreamBinaryReader = null;
            _gzipStream = null;
            _fileStream = null;
        }

        private void StopRecording()
        {
            lock (_recordingLock)
            {
                _recordingTimer.Stop();
                _gzipStreamBinaryWriter.Flush();
                _gzipStream.Flush();
                _fileStream.Flush();
                Common.Util.DisposeObject(_gzipStreamBinaryWriter);
                Common.Util.DisposeObject(_gzipStream);
                WriteHeader();
                Common.Util.DisposeObject(_fileStream);
                _gzipStreamBinaryWriter = null;
                _gzipStream = null;
                _fileStream = null;
            }
        }
        private void ReportPlaybackProgress()
        {
            if (PlaybackProgress != null)
            {
                PlaybackProgress(this, new PlaybackProgressEventArgs { ProgressPercentage = (float)CurrentSample / (float)NumSamples });
            }
        }


        void _recordingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_recordingLock)
            {
                var sample = ReadSampleFromSharedmem();
                NumSamples++;
                CurrentSample++;
                WriteSampleToFile(sample);
            }
        }
        void _playbackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_playbackLock)
            {
                if (_recordingTimer.Enabled)
                {
                    _playbackTimer.Stop();
                    return;
                }
                CurrentSample++;
                if (CurrentSample > NumSamples)
                {
                    if (LoopOnPlayback)
                    {
                        StopPlaying();
                        Play();
                        return;
                    }
                    else
                    {
                        Stop();
                        return;
                    }
                }
                var sample = ReadNextSampleFromFile();
                WriteSampleToSharedMemory(sample);
                ReportPlaybackProgress();
            }
        }

        public SharedMemoryRecordingHeader ReadHeader()
        {
            var header = new SharedMemoryRecordingHeader();
            using (var reader = new BinaryReader(input: _fileStream, encoding: Encoding.UTF8, leaveOpen: true))
            {
                header.Magic = reader.ReadBytes(4);
                header.NumSamples = reader.ReadUInt64();
                header.SampleInterval = reader.ReadUInt16();
            }
            return header;
        }
        public void WriteHeader()
        {
            _fileStream.Seek(0, SeekOrigin.Begin);
            using (var writer = new BinaryWriter(output: _fileStream, encoding: Encoding.UTF8, leaveOpen: true))
            {
                var header = new SharedMemoryRecordingHeader();
                header.Magic = new byte[4] { (byte)'S', (byte)'M', (byte)'X', (byte)'1' };
                header.NumSamples = NumSamples;
                header.SampleInterval = DEFAULT_SAMPLING_INTERVAL_MS;
                writer.Write(header.Magic, 0, header.Magic.Length);
                writer.Write(header.NumSamples);
                writer.Write(header.SampleInterval);
                writer.Flush();
            }

        }
        private SharedMemorySample ReadNextSampleFromFile()
        {
            var sample = new SharedMemorySample();
            sample.PrimaryFlightDataLength = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.PrimaryFlightDataLength > 0)
            {
                sample.PrimaryFlightData = _gzipStreamBinaryReader.ReadBytes(sample.PrimaryFlightDataLength);
            }
            sample.FlightData2Length = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.FlightData2Length > 0)
            {
                sample.FlightData2 = _gzipStreamBinaryReader.ReadBytes(sample.FlightData2Length);
            }
            sample.OSBDataLength = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.OSBDataLength > 0)
            {
                sample.OSBData = _gzipStreamBinaryReader.ReadBytes(sample.OSBDataLength);
            }
            sample.IntellivibeDataLength = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.IntellivibeDataLength > 0)
            {
                sample.IntellivibeData = _gzipStreamBinaryReader.ReadBytes(sample.IntellivibeDataLength);
            }
            sample.RadioClientStatusDataLength = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.RadioClientStatusDataLength > 0)
            {
                sample.RadioClientStatusData = _gzipStreamBinaryReader.ReadBytes(sample.RadioClientStatusDataLength);
            }
            sample.RadioClientControlDataLength = _gzipStreamBinaryReader.ReadUInt16();
            if (sample.RadioClientControlDataLength > 0)
            {
                sample.RadioClientControlData = _gzipStreamBinaryReader.ReadBytes(sample.RadioClientControlDataLength);
            }
            return sample;
        }
        private void WriteSampleToFile(SharedMemorySample sample)
        {
            if (_gzipStreamBinaryWriter == null)
            {
                return;
            }
            _gzipStreamBinaryWriter.Write(sample.PrimaryFlightDataLength);
            if (sample.PrimaryFlightDataLength > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.PrimaryFlightData);
            }
            _gzipStreamBinaryWriter.Write(sample.FlightData2Length);
            if (sample.FlightData2Length > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.FlightData2);
            }
            _gzipStreamBinaryWriter.Write(sample.OSBDataLength);
            if (sample.OSBDataLength > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.OSBData);
            }
            _gzipStreamBinaryWriter.Write(sample.IntellivibeDataLength);
            if (sample.IntellivibeDataLength > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.IntellivibeData);
            }
            _gzipStreamBinaryWriter.Write(sample.RadioClientStatusDataLength);
            if (sample.RadioClientStatusDataLength > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.RadioClientStatusData);
            }
            _gzipStreamBinaryWriter.Write(sample.RadioClientControlDataLength);
            if (sample.RadioClientControlDataLength > 0)
            {
                _gzipStreamBinaryWriter.Write(sample.RadioClientControlData);
            }
            _gzipStreamBinaryWriter.Flush();
            _gzipStream.Flush();
            _fileStream.Flush();
        }
        private SharedMemorySample ReadSampleFromSharedmem()
        {
            var sample = new SharedMemorySample();
            sample.PrimaryFlightData = _sharedMemoryReader.GetRawPrimaryFlightData();
            sample.PrimaryFlightDataLength = (ushort)(sample.PrimaryFlightData != null ? sample.PrimaryFlightData.Length : 0);
            sample.FlightData2 = _sharedMemoryReader.GetRawFlightData2();
            sample.FlightData2Length = (ushort)(sample.FlightData2 != null ? sample.FlightData2.Length : 0);
            sample.OSBData = _sharedMemoryReader.GetRawOSBData();
            sample.OSBDataLength = (ushort)(sample.OSBData != null ? sample.OSBData.Length : 0);
            sample.IntellivibeData = _sharedMemoryReader.GetRawIntelliVibeData();
            sample.IntellivibeDataLength = (ushort)(sample.IntellivibeData != null ? sample.IntellivibeData.Length : 0);
            sample.RadioClientControlData = _sharedMemoryReader.GetRawRadioClientControlData();
            sample.RadioClientControlDataLength = (ushort)(sample.RadioClientControlData != null ? sample.RadioClientControlData.Length : 0);
            sample.RadioClientStatusData = _sharedMemoryReader.GetRawRadioClientStatusData();
            sample.RadioClientStatusDataLength = (ushort)(sample.RadioClientStatusData != null ? sample.RadioClientStatusData.Length : 0);
            return sample;

        }
        private void WriteSampleToSharedMemory(SharedMemorySample sample)
        {
            if (sample.PrimaryFlightData != null)
            {
                try
                {
                    _sharedMemoryWriter.WritePrimaryFlightData(sample.PrimaryFlightData);
                }
                catch { }
            }
            if (sample.FlightData2 != null)
            {
                try
                {
                    _sharedMemoryWriter.WriteFlightData2(sample.FlightData2);
                }
                catch { }
            }
            if (sample.OSBData != null)
            {
                try
                {
                    _sharedMemoryWriter.WriteOSBData(sample.OSBData);
                }
                catch { }
            }
            if (sample.IntellivibeData != null)
            {
                try
                {
                    _sharedMemoryWriter.WriteIntellivibeData(sample.IntellivibeData);
                }
                catch { }
            }
            if (sample.RadioClientStatusData != null)
            {
                try
                {
                    _sharedMemoryWriter.WriteRadioClientStatusData(sample.RadioClientStatusData);
                }
                catch { }
            }
            if (sample.RadioClientControlData != null)
            {
                try
                {
                    _sharedMemoryWriter.WriteRadioClientControlData(sample.RadioClientControlData);
                }
                catch { }
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_recordingTimer != null)
                {
                    _recordingTimer.Stop();
                }
                if (_playbackTimer != null)
                {
                    _playbackTimer.Stop();
                }
                Common.Util.DisposeObject(_recordingTimer);
                Common.Util.DisposeObject(_playbackTimer);
                Common.Util.DisposeObject(_gzipStreamBinaryWriter);
                Common.Util.DisposeObject(_gzipStreamBinaryReader);
                Common.Util.DisposeObject(_gzipStream);
                Common.Util.DisposeObject(_fileStream);
                Common.Util.DisposeObject(_sharedMemoryReader);
                Common.Util.DisposeObject(_sharedMemoryWriter);
            }
        }
        ~SharedMemoryRecording()
        {
            Dispose(false);
        }
    }
}
