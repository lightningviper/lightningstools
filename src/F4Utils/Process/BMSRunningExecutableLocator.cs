using System.IO;

namespace F4Utils.Process
{
	public interface IBMSRunningExecutableLocator
	{
		string BMSExePath { get; }
	}

	public class BMSRunningExecutableLocator : IBMSRunningExecutableLocator
	{
		public string BMSExePath 
		{
			get
			{
				var exePath = F4Utils.Process.Util.GetFalconExePath();
				if (string.IsNullOrEmpty(exePath)) return null;
				var directoryInfo = new FileInfo(exePath).Directory;
				return directoryInfo != null ? directoryInfo.FullName : null;
			}
		}
	}
}
