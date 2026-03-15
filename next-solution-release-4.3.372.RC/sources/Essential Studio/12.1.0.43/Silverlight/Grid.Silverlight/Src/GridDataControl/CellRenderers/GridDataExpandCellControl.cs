#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Linq;
    using System.Linq;
    using System.Collections.Generic;
    using Syncfusion.Windows.Data;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Media.Animation;

    /// <summary>
    /// GridDataExpandCellControl displays the Expand/Collapse button for GridDataControl.
    /// </summary>
    /// <remarks>
    /// It has Dependency properties that is used in a customized ControlTemplate for
    /// showing the default values.
    /// </remarks>


    public class GridDataExpandCellControl : Control
    {
        internal Canvas plusPath;
        internal Canvas minusPath;
        internal Rectangle rectangle;

        public GridDataExpandCellControl()
        {
            this.DefaultStyleKey = typeof(GridDataExpandCellControl);
            DependencyObjectExtensions.SetEnableMousePosition(this, true);
        }

        public static readonly DependencyProperty PlusMinusBackgroundProperty = DependencyProperty.Register(
         "PlusMinusBackground",
         typeof(Brush),
         typeof(GridDataExpandCellControl),
         new PropertyMetadata(null));

        public Brush PlusMinusBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataExpandCellControl.PlusMinusBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataExpandCellControl.PlusMinusBackgroundProperty, value);
            }
        }


        public static readonly DependencyProperty RectangleBackgroundProperty = DependencyProperty.Register(
      "RectangleBackground",
      typeof(Brush),
      typeof(GridDataExpandCellControl),
      new PropertyMetadata(null));

        public Brush RectangleBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataExpandCellControl.RectangleBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataExpandCellControl.RectangleBackgroundProperty, value);
            }
        }

        public Brush PlusMinusBorderBrush
        {
            get { return (Brush)GetValue(PlusMinusBorderBrushProperty); }
            set { SetValue(PlusMinusBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlusMinusBorderBrushProperty =
            DependencyProperty.Register("PlusMinusBorderBrush", typeof(Brush), typeof(GridDataExpandCellControl), new PropertyMetadata(null));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            plusPath = this.GetTemplateChild("plusPath") as Canvas;
            minusPath = this.GetTemplateChild("minusPath") as Canvas;
            rectangle = this.GetTemplateChild("rectangle") as Rectangle;
        }
    }

}
