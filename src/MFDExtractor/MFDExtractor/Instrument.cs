using System;
using System.Diagnostics;
using Common.SimSupport;
using MFDExtractor.Properties;
using MFDExtractor.UI;
using log4net;
using System.Windows.Forms;

namespace MFDExtractor
{
    internal interface IInstrument:IDisposable
    {
        InstrumentType Type { get; }
        IInstrumentRenderer Renderer { get; }
        InstrumentForm OutputForm { get;}
        void Render();
        void Show();
        void Hide();
    }

    internal class Instrument : IInstrument
    {
        private bool _disposed;
        private readonly IInstrumentRenderHelper _instrumentRenderHelper;
        private readonly IInstrumentStateSnapshotCache _instrumentStateSnapshotCache;
        private readonly IInstrumentFormFactory _instrumentFormFactory;
        private readonly ILog _log = LogManager.GetLogger(typeof (Instrument));
        private readonly PerformanceCounter _renderedFramesCounter;
        private readonly PerformanceCounter _skippedFramesCounter;
        private readonly PerformanceCounter _totalFramesCounter;
        public InstrumentForm OutputForm { get; private set; }
        internal Instrument(
            InstrumentType instrumentType,
            IInstrumentRenderer renderer,
            IInstrumentStateSnapshotCache instrumentStateSnapshotCache = null,
            IPerformanceCounterInstanceFactory performanceCounterInstanceFactory = null,
            IInstrumentRenderHelper instrumentRenderHelper = null,
            IInstrumentFormFactory instrumentFormFactory = null
            )
        {
            Type = instrumentType;
            Renderer = renderer;
            _instrumentStateSnapshotCache = instrumentStateSnapshotCache ?? new InstrumentStateSnapshotCache();
            _instrumentRenderHelper = instrumentRenderHelper ?? new InstrumentRenderHelper();
            _instrumentFormFactory = instrumentFormFactory ?? new InstrumentFormFactory();
            var performanceCounterInstanceFactory1 = performanceCounterInstanceFactory ?? new PerformanceCounterInstanceInstanceFactory();
            _renderedFramesCounter = performanceCounterInstanceFactory1.CreatePerformanceCounterInstance(Application.ProductName,
                $"Rendered Frames per second - {instrumentType}");
            _skippedFramesCounter = performanceCounterInstanceFactory1.CreatePerformanceCounterInstance(Application.ProductName,
                $"Skipped Frames per second - {instrumentType}");
            _totalFramesCounter = performanceCounterInstanceFactory1.CreatePerformanceCounterInstance(Application.ProductName,
                $"Total Frames per second - {instrumentType}");
        }
        public InstrumentType Type { get; internal set; }
        public IInstrumentRenderer Renderer { get; internal set; }
        public void Show()
        {
            OutputForm = _instrumentFormFactory.Create(Type,Renderer
            );
            if (OutputForm != null && !OutputForm.Visible) { OutputForm.Show(); }
        }

        public void Hide()
        {
            OutputForm?.Hide();
            OutputForm?.Close();
            Common.Util.DisposeObject(OutputForm);
            OutputForm = null;
        }

        private static bool HighlightingBorderShouldBeDisplayedOnTargetForm(InstrumentForm targetForm)
        {
            return targetForm != null && targetForm.SizingOrMovingCursorsAreDisplayed && Settings.Default.HighlightOutputWindows;
        }

        public void Render()
        {
            if (!Extractor.State.Running || !Extractor.State.KeepRunning) return;
            try
            {
                if (!ShouldRenderNow())
                {
                    IncrementPerformanceCounter(_skippedFramesCounter);
                    IncrementPerformanceCounter(Extractor.State.SkippedFramesCounter);
                }
                else
                {
                    try { RenderInternal(); }
                    catch (Exception e) { _log.Error(e.Message, e); }
                }
                IncrementPerformanceCounter(_totalFramesCounter);
                IncrementPerformanceCounter(Extractor.State.TotalFramesCounter);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private bool ShouldRenderNow()
        {
            if (!Extractor.State.Running || !Extractor.State.KeepRunning || OutputForm == null || OutputForm.IsDisposed) return false;
            var stateIsStale = _instrumentStateSnapshotCache.CaptureInstrumentStateSnapshotAndCheckIfStale(Renderer, OutputForm);
            var shouldRenderNow = OutputForm != null && stateIsStale && OutputForm.Settings != null && OutputForm.Settings.Enabled;
            return shouldRenderNow;
        }
        private void RenderInternal()
        {
            try
            {
                if (OutputForm == null) return;
                _instrumentRenderHelper.Render(Renderer, OutputForm, OutputForm.Rotation, OutputForm.Monochrome,
                    HighlightingBorderShouldBeDisplayedOnTargetForm(OutputForm), Extractor.State.NightMode);
                IncrementPerformanceCounter(_renderedFramesCounter);
                IncrementPerformanceCounter(Extractor.State.RenderedFramesCounter);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private static void IncrementPerformanceCounter(PerformanceCounter performanceCounter)
        {
            if (performanceCounter == null) return;
            try
            {
                performanceCounter.Increment();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Hide();
                Common.Util.DisposeObject(OutputForm);
                Common.Util.DisposeObject(Renderer);
                Renderer = null;

                Common.Util.DisposeObject(_renderedFramesCounter);
                Common.Util.DisposeObject(_skippedFramesCounter);
                Common.Util.DisposeObject(_totalFramesCounter);
            }
            _disposed = true;
        }

        ~Instrument()
        {
            Dispose(false);
        }

    }
}
