#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Styles;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicStyleInfoStore : StyleInfoStore, IDisposable
    {
        static StaticData sd = new StaticData(typeof(GraphicStyleInfoStore), typeof(GraphicStyleInfo), false);

        public GraphicStyleInfoStore()
        {
            if (sd.IsEmpty)
                new GraphicStyleInfo();
        }

        public readonly static StyleInfoProperty BackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "Background");

        public readonly static StyleInfoProperty ForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "Foreground");

        public readonly static StyleInfoProperty CellTypeProperty = sd.CreateStyleInfoProperty(typeof(string), "CellType");

        public readonly static StyleInfoProperty CellValueProperty = sd.CreateStyleInfoProperty(typeof(object), "CellValue");

        public readonly static StyleInfoProperty ReadOnlyProperty = sd.CreateStyleInfoProperty(typeof(bool), "ReadOnly", 1, true);

        public readonly static StyleInfoProperty EnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "Enabled", 1, true);

        public readonly static StyleInfoProperty BorderBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "Property");

        public readonly static StyleInfoProperty BorderThicknessProperty = sd.CreateStyleInfoProperty(typeof(Thickness), "BorderThickness");

        public readonly static StyleInfoProperty HorizontalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(HorizontalAlignment), "HorizontalAlignment", 3, true);

        public readonly static StyleInfoProperty VerticalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(VerticalAlignment), "VerticalAlignment", 3, true);

        public readonly static StyleInfoProperty GraphicCellControlProperty = sd.CreateStyleInfoProperty(typeof(GraphicCellControl), "GraphicCellControl");

        public readonly static StyleInfoProperty CellNameProperty = sd.CreateStyleInfoProperty(typeof(string), "CellName");

        public readonly static StyleInfoProperty TextProperty = sd.CreateStyleInfoProperty(typeof(string), "Text");

        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        public override object Clone()
        {
            StyleInfoStore target = new GraphicStyleInfoStore();
            CopyTo(target);
            return target;
        }

        void IDisposable.Dispose()
        {
            this.Clear();
        }
    }
}
