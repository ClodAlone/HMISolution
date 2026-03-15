using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  internal static class KeyboardUtils
  {
    internal static bool IsHoldingCtrl
    {
      get
      {
        return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
      }
    }

    internal static bool IsHoldingShift
    {
      get
      {
        return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
      }
    }
  }
}
