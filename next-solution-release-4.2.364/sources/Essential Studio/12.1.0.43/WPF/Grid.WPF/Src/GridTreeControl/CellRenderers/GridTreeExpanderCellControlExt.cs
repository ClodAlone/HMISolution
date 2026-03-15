#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.GridCommon;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GridTreeExpanderCellControlExt : Control
    {
        public const double MinimumWidth = 10.0d;
        public GridTreeExpanderCellControlExt()
        {
            this.DefaultStyleKey = typeof(GridTreeExpanderCellControlExt);
        }

    

        #region Text

        /// <summary>
        /// DependencyProperty for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridTreeExpanderCellControlExt),
            new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(GridTreeExpanderCellControlExt.TextProperty);
            }

            set
            {
                this.SetValue(GridTreeExpanderCellControlExt.TextProperty, value);
            }
        }

        private bool isTextPropertyChangedBeforeInitialization = false;
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeExpanderCellControlExt expanderCell = d as GridTreeExpanderCellControlExt;
            if (expanderCell.isTemplateApplied)
            {
                expanderCell.TextBoxPart.Text = (string)args.NewValue;
            }
            else
            {
                expanderCell.isTextPropertyChangedBeforeInitialization = true;
            }

        }
        #endregion

       

        #region TextBoxBorderBrush
        public static readonly DependencyProperty TextBoxBorderBrushProperty = DependencyProperty.Register(
          "TextBoxBorderBrush",
          typeof(Brush),
          typeof(GridTreeExpanderCellControlExt),
          new PropertyMetadata(null));

        public Brush TextBoxBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(GridTreeExpanderCellControlExt.TextBoxBorderBrushProperty);
            }

            set
            {
                this.SetValue(GridTreeExpanderCellControlExt.TextBoxBorderBrushProperty, value);
            }
        }
        #endregion

        

        #region TextBoxBorderThickness
        public static readonly DependencyProperty TextBoxBorderThicknessProperty = DependencyProperty.Register(
          "TextBoxBorderThickness",
          typeof(Thickness),
          typeof(GridTreeExpanderCellControlExt),
          new PropertyMetadata(null));

        public Thickness TextBoxBorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(GridTreeExpanderCellControlExt.TextBoxBorderThicknessProperty);
            }

            set
            {
                this.SetValue(GridTreeExpanderCellControlExt.TextBoxBorderThicknessProperty, value);
            }
        }
        #endregion

        #region TextBoxMargin

        public static readonly DependencyProperty TextBoxMarginProperty = DependencyProperty.Register(
          "TextBoxMargin",
          typeof(Thickness),
          typeof(GridTreeExpanderCellControlExt),
          new PropertyMetadata(null));

        public Thickness TextBoxMargin
        {
            get
            {
                return (Thickness)this.GetValue(GridTreeExpanderCellControlExt.TextBoxMarginProperty);
            }

            set
            {
                this.SetValue(GridTreeExpanderCellControlExt.TextBoxMarginProperty, value);
            }
        }
        #endregion

        
        private GridControlBase gridControl;
        
        public bool IsInSuspend
        {
            get;
            internal set;
        }

        public bool IsContentInitialized
        {
            get;
            internal set;
        }      

       
        public GridControlBase GridControl
        {
            get
            {
                return gridControl;
            }
            set { gridControl = value; }
        }
       
        /// <summary>
        /// Gets the TextBox UIElement associated with the Expander Cell Control.
        /// </summary>
        public TextBox TextBoxPart
        {
            get;
            private set;
        }
       

        public GridTreeNode Node
        {
            get;
            set;
        }       

        private bool isTemplateApplied = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
            this.TextBoxPart = this.GetTemplateChild("PART_TextBox") as TextBox;            
         
                       
            this.SetProperties();
            this.isTemplateApplied = true; 
        }
               

        public void SetProperties()
        {
            if (this.isTextPropertyChangedBeforeInitialization && this.TextBoxPart!=null)
            {
                this.TextBoxPart.Text = this.Text;
            }            
        }

        
    }

    public class GridTreeExpanderMinWidthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return GridTreeExpanderCellControlExt.MinimumWidth;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
