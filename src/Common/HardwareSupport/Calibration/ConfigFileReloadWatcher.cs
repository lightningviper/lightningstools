using System;
using System.IO;
using System.Threading;
using log4net;

namespace Common.HardwareSupport.Calibration
{
    // Resilient file watcher for editor-authored gauge config files.
    //
    // Replaces the per-HSM FileSystemWatcher boilerplate with three layers
    // of defence against the well-known Windows file-watching failure
    // modes:
    //
    //   1. InternalBufferSize bumped to 64 KB (max safe value before the
    //      kernel starts dropping events). Avoids transient buffer
    //      overflows during bursty multi-gauge saves.
    //
    //   2. Error event handler. When the watcher reports an internal
    //      error (most commonly buffer overflow), dispose the watcher
    //      and create a fresh one on a background timer beat.
    //
    //   3. Periodic resubscribe (every 30 s). Several Windows scenarios
    //      silently orphan a FileSystemWatcher without firing an Error
    //      event — antivirus / OneDrive / SMB filter drivers can drop
    //      the IOCP completion notifications mid-flight, leaving the
    //      watcher object alive but deaf. Periodic recreation guarantees
    //      the watcher is fresh at most every 30 s. Belt-and-braces.
    //
    //   4. Mtime poll fallback (every 5 s). Even if all three watcher
    //      layers fail, a polling timer reads the file's
    //      LastWriteTime and triggers a reload when it changes. This
    //      guarantees worst-case ~5 s latency between editor save and
    //      gauge reload, even when the watcher is completely broken.
    //
    // Per-HSM usage:
    //
    //     // In the HSM's field section
    //     private ConfigFileReloadWatcher _configWatcher;
    //
    //     // In StartConfigWatcher (or equivalent)
    //     _configWatcher = new ConfigFileReloadWatcher(
    //         _config.FilePath, ReloadConfig);
    //
    //     // In Dispose
    //     if (_configWatcher != null) {
    //         _configWatcher.Dispose();
    //         _configWatcher = null;
    //     }
    //
    //     // The HSM's existing reload method:
    //     private void ReloadConfig() {
    //         var reloaded = MyHardwareSupportModuleConfig.Load(_config.FilePath);
    //         if (reloaded == null) return;
    //         reloaded.FilePath = _config.FilePath;
    //         _config = reloaded;
    //         ResolveAllChannels(reloaded);
    //     }
    //
    // The helper handles all dedup internally; the HSM's onChange callback
    // can assume the file actually changed since the last call.
    public sealed class ConfigFileReloadWatcher : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigFileReloadWatcher));

        // 30 s resubscribe and 5 s poll cadence picked as a compromise:
        // fast enough that a "watcher silently died" scenario heals in
        // about as long as it takes the user to retry a save, slow
        // enough to cost ~negligible CPU when nothing is happening.
        private const int RESUBSCRIBE_INTERVAL_MS = 30 * 1000;
        private const int POLL_INTERVAL_MS = 5 * 1000;
        private const int INTERNAL_BUFFER_SIZE = 64 * 1024;  // 64 KB max safe

        private readonly string _filePath;
        private readonly Action _onChange;
        private readonly object _lock = new object();

        private FileSystemWatcher _watcher;
        private Timer _resubscribeTimer;
        private Timer _pollTimer;
        private DateTime _lastSeenWriteTime = DateTime.MinValue;
        private bool _isDisposed;

        public ConfigFileReloadWatcher(string filePath, Action onChange)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            if (onChange == null) throw new ArgumentNullException("onChange");
            _filePath = filePath;
            _onChange = onChange;
            try
            {
                if (File.Exists(_filePath))
                {
                    _lastSeenWriteTime = File.GetLastWriteTimeUtc(_filePath);
                }
            }
            catch (Exception e)
            {
                _log.Warn("Could not read initial mtime for " + _filePath + ": " + e.Message);
            }
            CreateWatcher();
            // Resubscribe periodically to recover from silent orphaning.
            _resubscribeTimer = new Timer(
                _ => RecreateWatcher(reason: "periodic"),
                state: null,
                dueTime: RESUBSCRIBE_INTERVAL_MS,
                period: RESUBSCRIBE_INTERVAL_MS);
            // Poll mtime as a worst-case fallback.
            _pollTimer = new Timer(
                _ => PollOnce(),
                state: null,
                dueTime: POLL_INTERVAL_MS,
                period: POLL_INTERVAL_MS);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_isDisposed) return;
                _isDisposed = true;
                if (_resubscribeTimer != null)
                {
                    try { _resubscribeTimer.Dispose(); } catch { }
                    _resubscribeTimer = null;
                }
                if (_pollTimer != null)
                {
                    try { _pollTimer.Dispose(); } catch { }
                    _pollTimer = null;
                }
                DisposeWatcher();
            }
        }

        // Called from constructor and RecreateWatcher. Caller must hold
        // the lock when entering from RecreateWatcher; constructor is
        // single-threaded so it doesn't need to.
        private void CreateWatcher()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                var name = Path.GetFileName(_filePath);
                if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(name)) return;
                if (!Directory.Exists(dir)) return;
                var w = new FileSystemWatcher(dir, name)
                {
                    InternalBufferSize = INTERNAL_BUFFER_SIZE,
                    NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.Size
                                 | NotifyFilters.FileName
                                 | NotifyFilters.CreationTime,
                };
                // Watch all the events that can signal a save: Changed for
                // in-place truncate-write (Node fs.writeFileSync), Created
                // for atomic-replace patterns (rename-into-place), and
                // Renamed for editors that use a temp file then rename.
                w.Changed += OnFsEvent;
                w.Created += OnFsEvent;
                w.Renamed += OnFsRenamed;
                w.Error += OnFsError;
                w.EnableRaisingEvents = true;
                _watcher = w;
            }
            catch (Exception e)
            {
                _log.Error("Could not create FileSystemWatcher for " + _filePath + ": " + e.Message, e);
            }
        }

        private void DisposeWatcher()
        {
            var w = _watcher;
            _watcher = null;
            if (w == null) return;
            try { w.EnableRaisingEvents = false; } catch { }
            try { w.Changed -= OnFsEvent; } catch { }
            try { w.Created -= OnFsEvent; } catch { }
            try { w.Renamed -= OnFsRenamed; } catch { }
            try { w.Error -= OnFsError; } catch { }
            try { w.Dispose(); } catch { }
        }

        private void RecreateWatcher(string reason)
        {
            lock (_lock)
            {
                if (_isDisposed) return;
                DisposeWatcher();
                CreateWatcher();
            }
            // After recreating, also poll once in case the file changed
            // during the brief window where we had no watcher armed.
            PollOnce();
        }

        private void OnFsEvent(object sender, FileSystemEventArgs e)
        {
            MaybeFireReload();
        }

        private void OnFsRenamed(object sender, RenamedEventArgs e)
        {
            // Atomic-replace patterns rename the new file INTO the watched
            // name, so the rename event signals a fresh file is in place.
            MaybeFireReload();
        }

        private void OnFsError(object sender, ErrorEventArgs e)
        {
            // Most common cause: internal buffer overflow. Dispose and
            // recreate the watcher; the poll timer will catch any
            // mtime change that happened between events.
            var ex = e.GetException();
            _log.Warn("FileSystemWatcher error for " + _filePath +
                ": " + (ex != null ? ex.Message : "unknown") +
                " — recreating watcher");
            RecreateWatcher(reason: "error");
        }

        private void PollOnce()
        {
            if (_isDisposed) return;
            MaybeFireReload();
        }

        // Single source of truth for "should we tell the HSM to reload?"
        // Compares the file's current LastWriteTimeUtc against the last
        // value we successfully reloaded for. Returns early when nothing
        // has changed. Synchronised so multiple watcher events firing
        // close together (or a watcher event arriving the same instant
        // the poll timer ticks) collapse into a single reload.
        private void MaybeFireReload()
        {
            DateTime now;
            lock (_lock)
            {
                if (_isDisposed) return;
                try
                {
                    if (!File.Exists(_filePath)) return;
                    now = File.GetLastWriteTimeUtc(_filePath);
                }
                catch
                {
                    return;
                }
                if (now == _lastSeenWriteTime) return;
                _lastSeenWriteTime = now;
            }
            // Invoke the HSM's reload callback OUTSIDE the lock so a slow
            // reload (XML parse + channel re-resolve) doesn't block other
            // threads that want to fire events while we're working.
            try
            {
                _onChange();
            }
            catch (Exception e)
            {
                _log.Error("Reload callback failed for " + _filePath + ": " + e.Message, e);
            }
        }
    }
}
