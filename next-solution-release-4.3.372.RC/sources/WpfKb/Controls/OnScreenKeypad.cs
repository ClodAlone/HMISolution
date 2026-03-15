using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using WindowsInput;
using WpfKb.LogicalKeys;

namespace WpfKb.Controls
{
    public class OnScreenKeypad : UniformOnScreenKeyboard
    {
        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public OnScreenKeypad(int fontSize)
        {
            var digitSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            var scanCode = VkKeyScan(digitSeparator[0]);

            var scanCodeDigitSeparator = (VirtualKeyCode)(scanCode & 0xff);
            int modifiers = scanCode >> 8;
            bool bNeedShiftDown = false;
            if ((modifiers & 1) != 0) bNeedShiftDown = true;
            //if ((modifiers & 2) != 0) retval |= Keys.Control;
            //if ((modifiers & 4) != 0) retval |= Keys.Alt;

            var key0 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D0);
            var key1 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D1);
            var key2 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D2);
            var key3 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D3);
            var key4 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D4);
            var key5 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D5);
            var key6 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D6);
            var key7 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D7);
            var key8 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D8);
            var key9 = OnScreenKeyboard.GetCharFromKey(System.Windows.Input.Key.D9);
            bool bNeedShiftDown0 = key0 != "0";
            bool bNeedShiftDown1 = key1 != "1";
            bool bNeedShiftDown2 = key2 != "2";
            bool bNeedShiftDown3 = key3 != "3";
            bool bNeedShiftDown4 = key4 != "4";
            bool bNeedShiftDown5 = key5 != "5";
            bool bNeedShiftDown6 = key6 != "6";
            bool bNeedShiftDown7 = key7 != "7";
            bool bNeedShiftDown8 = key8 != "8";
            bool bNeedShiftDown9 = key9 != "9";

            Keys = new ObservableCollection<OnScreenKey>
                       {
                           new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 0, Key = new VirtualKey(VirtualKeyCode.VK_7, "7") { NeedShiftDown = bNeedShiftDown7 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 1, Key = new VirtualKey(VirtualKeyCode.VK_8, "8") { NeedShiftDown = bNeedShiftDown8 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 2, Key = new VirtualKey(VirtualKeyCode.VK_9, "9") { NeedShiftDown = bNeedShiftDown9 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 0, Key = new VirtualKey(VirtualKeyCode.VK_4, "4") { NeedShiftDown = bNeedShiftDown4 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 1, Key = new VirtualKey(VirtualKeyCode.VK_5, "5") { NeedShiftDown = bNeedShiftDown5 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 2, Key = new VirtualKey(VirtualKeyCode.VK_6, "6") { NeedShiftDown = bNeedShiftDown6 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 0, Key = new VirtualKey(VirtualKeyCode.VK_1, "1") { NeedShiftDown = bNeedShiftDown1 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 1, Key = new VirtualKey(VirtualKeyCode.VK_2, "2") { NeedShiftDown = bNeedShiftDown2 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 2, Key = new VirtualKey(VirtualKeyCode.VK_3, "3") { NeedShiftDown = bNeedShiftDown3 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 0, Key = new VirtualKey(VirtualKeyCode.CLEAR, "Clear") },
                           new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 1, Key = new VirtualKey(VirtualKeyCode.VK_0, "0") { NeedShiftDown = bNeedShiftDown0 } },
                           new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 2, Key = new VirtualKey(VirtualKeyCode.BACK, "Del") },
                           new OnScreenKey { FontSize = fontSize, GridRow = 4, GridColumn = 0, Key = new VirtualKey(scanCodeDigitSeparator, digitSeparator) { NeedShiftDown = bNeedShiftDown } },
                       };
        }
    }
}