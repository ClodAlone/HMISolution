using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Core;

namespace ScreenManager
{
    public class DockLayoutControlEx : DockLayoutControl
    {
        public event EventHandler MeasureChanged;
        protected override Size OnMeasure(Size availableSize)
        {
            MeasureChanged?.Invoke(this, EventArgs.Empty);
            return base.OnMeasure(availableSize);
        }
    }
}
