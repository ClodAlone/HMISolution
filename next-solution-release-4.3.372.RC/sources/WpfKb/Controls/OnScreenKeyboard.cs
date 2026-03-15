using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using WindowsInput;
using WpfKb.LogicalKeys;
using System.Runtime.InteropServices;
using System.Text;

namespace WpfKb.Controls
{
    public class OnScreenKeyboard : Grid
    {

        public static readonly DependencyProperty AreAnimationsEnabledProperty = DependencyProperty.Register("AreAnimationsEnabled", typeof(bool), typeof(OnScreenKeyboard), new UIPropertyMetadata(true, OnAreAnimationsEnabledPropertyChanged));

        private ObservableCollection<OnScreenKeyboardSection> _sections;
        private List<ModifierKeyBase> _modifierKeys;
        private List<ILogicalKey> _allLogicalKeys;
        private List<OnScreenKey> _allOnScreenKeys;
        private int fontSize;

        public OnScreenKeyboard(int fontSize)
        {
            this.fontSize = fontSize;
        }

        public bool AreAnimationsEnabled
        {
            get { return (bool)GetValue(AreAnimationsEnabledProperty); }
            set { SetValue(AreAnimationsEnabledProperty, value); }
        }

        private static void OnAreAnimationsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyboard = (OnScreenKeyboard)d;
            keyboard._allOnScreenKeys.ToList().ForEach(x => x.AreAnimationsEnabled = (bool)e.NewValue);
        }

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);
        [DllImport("user32.dll")]
        public static extern int ToUnicode(
         uint wVirtKey,
         uint wScanCode,
         byte[] lpKeyState,
         [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
        StringBuilder pwszBuff,
         int cchBuff,
         uint wFlags);

        public static String GetCharFromKey(Key key, bool bShift = false)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            // VK_SHIFT 0x10, VK_CONTROL 0x11, VK_CAPITAL 0x14
            keyboardState[0x10] = keyboardState[0x11] = keyboardState[0x14] = 0x00;
            if (bShift)
            {
                // VK_SHIFT 0x10
                keyboardState[0x10] = 0x80;
            }

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        return stringBuilder.ToString();
                    }
            }
            return ch.ToString();
        }

        public override void BeginInit()
        {
            SetValue(FocusManager.IsFocusScopeProperty, true);
            _modifierKeys = new List<ModifierKeyBase>();
            _allLogicalKeys = new List<ILogicalKey>();
            _allOnScreenKeys = new List<OnScreenKey>();

            _sections = new ObservableCollection<OnScreenKeyboardSection>();

            var mainSection = new OnScreenKeyboardSection();
            var mainKeys = new ObservableCollection<OnScreenKey>
                               {
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 0, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_3, new List<string> { GetCharFromKey(Key.Oem3), GetCharFromKey(Key.Oem3, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 1, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_1, new List<string> { GetCharFromKey(Key.D1), GetCharFromKey(Key.D1, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 2, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_2, new List<string> { GetCharFromKey(Key.D2), GetCharFromKey(Key.D2, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 3, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_3, new List<string> { GetCharFromKey(Key.D3), GetCharFromKey(Key.D3, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 4, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_4, new List<string> { GetCharFromKey(Key.D4), GetCharFromKey(Key.D4, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 5, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_5, new List<string> { GetCharFromKey(Key.D5), GetCharFromKey(Key.D5, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 6, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_6, new List<string> { GetCharFromKey(Key.D6), GetCharFromKey(Key.D6, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 7, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_7, new List<string> { GetCharFromKey(Key.D7), GetCharFromKey(Key.D7, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 8, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_8, new List<string> { GetCharFromKey(Key.D8), GetCharFromKey(Key.D8, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 9, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_9, new List<string> { GetCharFromKey(Key.D9), GetCharFromKey(Key.D9, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 10, Key =  new ShiftSensitiveKey(VirtualKeyCode.VK_0, new List<string> { GetCharFromKey(Key.D0), GetCharFromKey(Key.D0, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 11, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_MINUS, new List<string> { GetCharFromKey(Key.OemMinus), GetCharFromKey(Key.OemMinus, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 12, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_PLUS, new List<string> { GetCharFromKey(Key.OemPlus), GetCharFromKey(Key.OemPlus, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 0, GridColumn = 13, Key =  new VirtualKey(VirtualKeyCode.BACK, "Bksp"), GridWidth = new GridLength(2, GridUnitType.Star)},

                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 0, Key =  new VirtualKey(VirtualKeyCode.TAB, "Tab"), GridWidth = new GridLength(1.5, GridUnitType.Star)},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 1, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_Q, new List<string> { GetCharFromKey(Key.Q), GetCharFromKey(Key.Q, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 2, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_W, new List<string> { GetCharFromKey(Key.W), GetCharFromKey(Key.W, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 3, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_E, new List<string> { GetCharFromKey(Key.E), GetCharFromKey(Key.E, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 4, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_R, new List<string> { GetCharFromKey(Key.R), GetCharFromKey(Key.R, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 5, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_T, new List<string> { GetCharFromKey(Key.T), GetCharFromKey(Key.T, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 6, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_Y, new List<string> { GetCharFromKey(Key.Y), GetCharFromKey(Key.Y, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 7, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_U, new List<string> { GetCharFromKey(Key.U), GetCharFromKey(Key.U, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 8, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_I, new List<string> { GetCharFromKey(Key.I), GetCharFromKey(Key.I, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 9, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_O, new List<string> { GetCharFromKey(Key.O), GetCharFromKey(Key.O, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 10, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_P, new List<string> { GetCharFromKey(Key.P), GetCharFromKey(Key.P, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 11, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_4, new List<string> { GetCharFromKey(Key.Oem4), GetCharFromKey(Key.Oem4, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 12, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_6, new List<string> { GetCharFromKey(Key.Oem6), GetCharFromKey(Key.Oem6, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 13, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_5, new List<string> { GetCharFromKey(Key.Oem5), GetCharFromKey(Key.Oem5, true) }), GridWidth = new GridLength(1.3, GridUnitType.Star)},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 14, Key =  new StringKey("[", "[" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 15, Key =  new StringKey("]", "]" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 16, Key =  new StringKey("{", "{" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 1, GridColumn = 17, Key =  new StringKey("}", "}" )},

                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 0, Key =  new TogglingModifierKey("Caps", VirtualKeyCode.CAPITAL), GridWidth = new GridLength(1.7, GridUnitType.Star)},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 1, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_A, new List<string> { GetCharFromKey(Key.A), GetCharFromKey(Key.A, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 2, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_S, new List<string> { GetCharFromKey(Key.S), GetCharFromKey(Key.S, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 3, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_D, new List<string> { GetCharFromKey(Key.D), GetCharFromKey(Key.D, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 4, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_F, new List<string> { GetCharFromKey(Key.F), GetCharFromKey(Key.F, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 5, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_G, new List<string> { GetCharFromKey(Key.G), GetCharFromKey(Key.G, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 6, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_H, new List<string> { GetCharFromKey(Key.H), GetCharFromKey(Key.H, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 7, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_J, new List<string> { GetCharFromKey(Key.J), GetCharFromKey(Key.J, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 8, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_K, new List<string> { GetCharFromKey(Key.K), GetCharFromKey(Key.K, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 9, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_L, new List<string> { GetCharFromKey(Key.L), GetCharFromKey(Key.L, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 10, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_1, new List<string> { GetCharFromKey(Key.Oem1), GetCharFromKey(Key.Oem1, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 11, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_7, new List<string> { GetCharFromKey(Key.Oem7), GetCharFromKey(Key.Oem7, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 12, Key =  new StringKey("@", "@" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 13, Key =  new StringKey("#", "#" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 2, GridColumn = 14, Key =  new VirtualKey(VirtualKeyCode.RETURN, "Enter"), GridWidth = new GridLength(1.8, GridUnitType.Star)},

                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 0, Key =  new InstantaneousModifierKey("Shift", VirtualKeyCode.SHIFT), GridWidth = new GridLength(2.4, GridUnitType.Star)},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 1, Key =  new StringKey("<", "<" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 2, Key =  new StringKey(">", ">" )},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 3, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_Z, new List<string> { GetCharFromKey(Key.Z), GetCharFromKey(Key.Z, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 4, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_X, new List<string> { GetCharFromKey(Key.X), GetCharFromKey(Key.X, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 5, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_C, new List<string> { GetCharFromKey(Key.C), GetCharFromKey(Key.C, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 6, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_V, new List<string> { GetCharFromKey(Key.V), GetCharFromKey(Key.V, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 7, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_B, new List<string> { GetCharFromKey(Key.B), GetCharFromKey(Key.B, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 8, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_N, new List<string> { GetCharFromKey(Key.N), GetCharFromKey(Key.N, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 9, Key =  new CaseSensitiveKey(VirtualKeyCode.VK_M, new List<string> { GetCharFromKey(Key.M), GetCharFromKey(Key.M, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 10, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_COMMA, new List<string> { GetCharFromKey(Key.OemComma), GetCharFromKey(Key.OemComma, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 11, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_PERIOD, new List<string> { GetCharFromKey(Key.OemPeriod), GetCharFromKey(Key.OemPeriod, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 12, Key =  new ShiftSensitiveKey(VirtualKeyCode.OEM_2, new List<string> { GetCharFromKey(Key.Oem2), GetCharFromKey(Key.Oem2, true) })},
                                   new OnScreenKey { FontSize = fontSize, GridRow = 3, GridColumn = 13, Key =  new InstantaneousModifierKey("Shift", VirtualKeyCode.SHIFT), GridWidth = new GridLength(2.4, GridUnitType.Star)},

                                   new OnScreenKey { FontSize = fontSize, GridRow = 4, GridColumn = 0, Key =  new VirtualKey(VirtualKeyCode.SPACE, " "), GridWidth = new GridLength(5, GridUnitType.Star)},
                               };

            mainSection.Keys = mainKeys;
            mainSection.SetValue(ColumnProperty, 0);
            _sections.Add(mainSection);
            ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(3, GridUnitType.Star)});
            Children.Add(mainSection);

            _allLogicalKeys.AddRange(mainKeys.Select(x => x.Key));
            _allOnScreenKeys.AddRange(mainSection.Keys);







            _modifierKeys.AddRange(_allLogicalKeys.OfType<ModifierKeyBase>());
            _allOnScreenKeys.ForEach(x => x.OnScreenKeyPress += OnScreenKeyPress);

            SynchroniseModifierKeyState();
            SynchroniseMultiCharacterKeyState();

            base.BeginInit();
        }

        void OnScreenKeyPress(DependencyObject sender, OnScreenKeyEventArgs e)
        {
            if (e.OnScreenKey.Key is ModifierKeyBase)
            {
                var modifierKey = (ModifierKeyBase)e.OnScreenKey.Key;
                if (modifierKey.KeyCode == VirtualKeyCode.SHIFT)
                {
                    HandleShiftKeyPressed(modifierKey);
                }
                else if (modifierKey.KeyCode == VirtualKeyCode.CAPITAL)
                {
                    HandleCapsLockKeyPressed(modifierKey);
                }
                else if (modifierKey.KeyCode == VirtualKeyCode.NUMLOCK)
                {
                    HandleNumLockKeyPressed(modifierKey);
                }
            }
            else
            {
                ResetInstantaneousModifierKeys();
            }
            _modifierKeys.OfType<InstantaneousModifierKey>().ToList().ForEach(x => x.SynchroniseKeyState());
        }

        private void SynchroniseModifierKeyState()
        {
            _modifierKeys.ToList().ForEach(x => x.SynchroniseKeyState());
        }

        private void SynchroniseMultiCharacterKeyState()
        {
            _allLogicalKeys.OfType<MultiCharacterKey>().ToList().ForEach(x => x.SynchroniseKeyState());
        }

        private void ResetInstantaneousModifierKeys()
        {
            _modifierKeys.OfType<InstantaneousModifierKey>().ToList().ForEach(x => { if (x.IsInEffect) x.Press(); });
        }

        void HandleShiftKeyPressed(ModifierKeyBase shiftKey)
        {
            _allLogicalKeys.OfType<CaseSensitiveKey>().ToList().ForEach(x => x.SelectedIndex =
                                                                             InputSimulator.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL) ^ shiftKey.IsInEffect ? 1 : 0);
            _allLogicalKeys.OfType<ShiftSensitiveKey>().ToList().ForEach(x => x.SelectedIndex = shiftKey.IsInEffect ? 1 : 0);
        }

        void HandleCapsLockKeyPressed(ModifierKeyBase capsLockKey)
        {
            _allLogicalKeys.OfType<CaseSensitiveKey>().ToList().ForEach(x => x.SelectedIndex =
                                                                             capsLockKey.IsInEffect ^ InputSimulator.IsKeyDownAsync(VirtualKeyCode.SHIFT) ? 1 : 0);
        }

        void HandleNumLockKeyPressed(ModifierKeyBase numLockKey)
        {
            _allLogicalKeys.OfType<NumLockSensitiveKey>().ToList().ForEach(x => x.SelectedIndex = numLockKey.IsInEffect? 1 : 0);
        }
    }
}