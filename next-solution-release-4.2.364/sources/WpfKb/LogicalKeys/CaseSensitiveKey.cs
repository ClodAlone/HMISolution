using System.Collections.Generic;
using WindowsInput;

namespace WpfKb.LogicalKeys
{
    public class CaseSensitiveKey : MultiCharacterKey
    {
        public CaseSensitiveKey(VirtualKeyCode keyCode, IList<string> keyDisplays)
            : base(keyCode, keyDisplays)
        {
        }

        public override void SynchroniseKeyState()
        {
            if (InputSimulator.IsKeyDownAsync(VirtualKeyCode.SHIFT) ^ InputSimulator.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL))
                SelectedIndex = 1;
        }
    }
}