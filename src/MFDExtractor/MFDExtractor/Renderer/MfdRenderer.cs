using System;
using Common.SimSupport;
using Common.Drawing;
using Common.Imaging;
using Common.Drawing.Imaging;

namespace MFDExtractor.Renderer
{
	public interface IMfdRenderer:IInstrumentRenderer
	{
		MfdRenderer.MfdRendererInstrumentState InstrumentState { get; set; }
		MfdRenderer.MfdRendererOptions Options { get; set; }
	}

	public class MfdRenderer:InstrumentRendererBase, IMfdRenderer
	{
		public MfdRenderer()
		{
			InstrumentState = new MfdRendererInstrumentState{SourceRectangle = Rectangle.Empty};
			Options = new MfdRendererOptions();
		}
		public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
		{
			if (InstrumentState.Blank)
			{
				if (Options.BlankImage != null)
				{
				    try
				    {
				        destinationGraphics.DrawImageFast(Options.BlankImage, destinationRectangle,
				            new Rectangle(new Point(0, 0), Options.BlankImage.Size), GraphicsUnit.Pixel);
				    }
                    catch { }
				}
			}
			else if (InstrumentState.TestMode)
			{
				if (Options.TestAlignmentImage != null)
				{
                    try
				    {
				        destinationGraphics.DrawImageFast(Options.TestAlignmentImage, destinationRectangle,
				            new Rectangle(new Point(0, 0), Options.TestAlignmentImage.Size), GraphicsUnit.Pixel);
				    }
                    catch {}
				}
			}
			else
			{
			    if (InstrumentState.SourceImage == null || InstrumentState.SourceRectangle == Rectangle.Empty)
			    {
			        try
			        {
			            destinationGraphics.DrawImageFast(Options.BlankImage, destinationRectangle,
			                new Rectangle(new Point(0, 0), Options.BlankImage.Size), GraphicsUnit.Pixel);
			        }
                    catch { }
			    }
                else
				{
				    try
				    {
				        RenderFromSharedmemSurface(destinationGraphics, destinationRectangle);
				    }
                    catch { }
				}
			}
		}
	    private void RenderFromSharedmemSurface(Graphics destinationGraphics, Rectangle destinationRectangle)
	    {
	        try
	        {
	            var mfdImage = InstrumentState.SourceImage;
                if (mfdImage !=null  && mfdImage.PixelFormat != PixelFormat.Undefined)
                {
	                destinationGraphics.DrawImageFast(mfdImage, destinationRectangle, InstrumentState.SourceRectangle,
	                    GraphicsUnit.Pixel);
	            }
	        }
	        catch (AccessViolationException){}
	        catch (InvalidOperationException){}
	    }

	    public MfdRendererInstrumentState InstrumentState { get; set; }
		public MfdRendererOptions Options { get; set; }
		public override void Dispose() {}
		~MfdRenderer()
        {
            Dispose(false);
        }

		[Serializable]
		public class MfdRendererOptions
		{
			public Image BlankImage { get; set; }
			public Image TestAlignmentImage { get; set; }
		}
		[Serializable]
		public class MfdRendererInstrumentState : InstrumentStateBase
		{
			public bool Blank { get; set; }
			public bool TestMode { get; set; }

		    [NonSerialized] 
            public Image SourceImage;

		    public int SourceImageHashCode
		    {
		        get { return SourceImage != null ? SourceImage.GetHashCode() : 0; }
		    }

		    public Rectangle SourceRectangle { get; set; }
            public override int GetHashCode()
            {
                return SourceImageHashCode;
            }
            public override string ToString()
            {
                return string.Format("Blank:{0}, OptionsFormIsShowing:{1}, SourceRectangle:{2}, SourceImageHashCode:{3}", Blank, TestMode, SourceRectangle, SourceImageHashCode);
            }
            public override bool Equals(object obj)
            {
                return (
                       obj != null &&
                       obj is MfdRendererInstrumentState &&
                       obj.GetHashCode() == GetHashCode()
                   );
            }
		  
		}
	}
}
