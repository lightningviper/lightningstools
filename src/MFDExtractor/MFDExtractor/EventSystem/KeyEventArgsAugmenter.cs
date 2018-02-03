using System.Windows.Forms;
using Common.Win32;

namespace MFDExtractor.EventSystem
{
	internal interface IKeyEventArgsAugmenter
	{
		Keys UpdateKeyEventArgsWithExtendedKeyInfo(Keys keys);
	}

	class KeyEventArgsAugmenter : IKeyEventArgsAugmenter
	{
		public Keys UpdateKeyEventArgsWithExtendedKeyInfo(Keys keys)
		{
			if ((NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0)
			{
				keys |= Keys.Shift;
				//SHIFT is pressed
			}
			if ((NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0)
			{
				keys |= Keys.Control;
				//CONTROL is pressed
			}
			if ((NativeMethods.GetKeyState(NativeMethods.VK_MENU) & 0x8000) != 0)
			{
				keys |= Keys.Alt;
				//ALT is pressed
			}
			return keys;
		}

	}
}
