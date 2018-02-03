
using System.Windows.Forms;
using Common.InputSupport;
using Common.InputSupport.UI;

namespace MFDExtractor.EventSystem
{
    internal interface IKeyInputEventHotkeyFilter
    {
        bool CheckIfMatches(InputControlSelection hotkey, Keys keyPressed);
    }

    internal class KeyInputEventHotkeyFilter : IKeyInputEventHotkeyFilter
    {
        public bool CheckIfMatches(InputControlSelection hotkey, Keys keyPressed)
        {
            return (hotkey != null) && hotkey.ControlType == ControlType.Key
                   && (
                       (hotkey.Keys & Keys.KeyCode) == (keyPressed & Keys.KeyCode)
                       &&
                       (hotkey.Keys & Keys.Modifiers) == (keyPressed & Keys.Modifiers)
                       );

        }
    }
}
