using System.Collections.Generic;
using WindowsInput;

namespace WpfKb.LogicalKeys
{
    public class NumLockSensitiveKey : MultiCharacterKey
    {
        public NumLockSensitiveKey(VirtualKeyCode keyCode, IList<string> keyDisplays)
            : base(keyCode, keyDisplays)
        {
        }

        public override void SynchroniseKeyState()
        {
            if (InputSimulator.IsTogglingKeyInEffect(VirtualKeyCode.NUMLOCK))
                SelectedIndex = 1;
        }
    }
}