#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
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
    using System.ComponentModel;


    public class GridTreeHeaderCellControl : Control
    {
        public new const double MinWidth = 25.0d;
        private System.Windows.Controls.Grid mainGrid;

        private Border mainBorder;

        private Border sortBorder;

        private Border textBorder;

        public GridTreeHeaderCellControl()
        {
            this.DefaultStyleKey = typeof(GridTreeHeaderCellControl);            
            this.MouseLeave += new MouseEventHandler(GridTreeHeaderCellControl_MouseLeave);
            DependencyObjectExtensions.SetEnableMousePosition(this, true);
        }

        #region Dependency Property

        #region Text
        
        /// <summary>
        /// DependencyProperty for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridTreeHeaderCellControl),
            new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(GridTreeHeaderCellControl.TextProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.TextProperty, value);
            }
        }

        private bool isTextPropertyChangedBeforeInitialization = false;
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeHeaderCellControl headerCell = d as GridTreeHeaderCellControl;
            if (headerCell.isTemplateApplied)
            {
                headerCell.TextBlockPart.Text = (string)args.NewValue;
            }
            else
            {
                headerCell.isTextPropertyChangedBeforeInitialization = true;
            }

        }
        #endregion

        #region SortVisibility
        /// <summary>
        /// DependencyProperty for SortVisibility.
        /// </summary>
        public static readonly DependencyProperty SortVisibilityProperty = DependencyProperty.Register(
            "SortVisibility",
            typeof(Visibility),
            typeof(GridTreeHeaderCellControl),
            new PropertyMetadata(Visibility.Collapsed, OnSortVisibilityChanged));

        /// <summary>
        /// Gets or sets whether if the Sort Widget has to be shown.
        /// </summary>
        public Visibility SortVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridTreeHeaderCellControl.SortVisibilityProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortVisibilityProperty, value);
            }
        }

        private bool isSortVisibiltyChangedBeforeInitialization = false;

        private static void OnSortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridTreeHeaderCellControl;
            if (headerCellControl.mainGrid != null)
            {
                headerCellControl.SetSortColumnWidth();
            }
            else
            {
                headerCellControl.isSortVisibiltyChangedBeforeInitialization = true;
            }
        }
        #endregion

        #region AscVisibility       

        public static readonly DependencyProperty AscVisibilityProperty = DependencyProperty.Register(
         "AscVisibility",
         typeof(Visibility),
         typeof(GridTreeHeaderCellControl),
         new PropertyMetadata(Visibility.Collapsed));


        public Visibility AscVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridTreeHeaderCellControl.AscVisibilityProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.AscVisibilityProperty, value);
            }
        }

        #endregion

        #region DescVisibility
        
        public static readonly DependencyProperty DescVisibilityProperty = DependencyProperty.Register(
           "DescVisibility",
           typeof(Visibility),
           typeof(GridTreeHeaderCellControl),
           new PropertyMetadata(Visibility.Collapsed));
      

        public Visibility DescVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridTreeHeaderCellControl.DescVisibilityProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.DescVisibilityProperty, value);
            }
        }

        #endregion

        #region SortDirection
        
        /// <summary>
        /// DependencyProperty for SortDirection.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(ListSortDirection),
            typeof(GridTreeHeaderCellControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Sort Direction for this instance.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get
            {
                return (ListSortDirection)this.GetValue(GridTreeHeaderCellControl.SortDirectionProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortDirectionProperty, value);
            }
        }
        #endregion    

        #region SortBrush       

        /// <summary>
        /// DependencyProperty for SortBrush.
        /// </summary>
        public static readonly DependencyProperty SortBrushProperty = DependencyProperty.Register(
            "SortBrush",
            typeof(Brush),
            typeof(GridTreeHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the brush value for the Sort Widget.
        /// </summary>
        public Brush SortBrush
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.SortBrushProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortBrushProperty, value);
            }
        }

        #endregion

        #region HoverBackground
       
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
          "HoverBackground",
          typeof(Brush),
          typeof(GridTreeHeaderCellControl),
          new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush HoverBackground
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.HoverBackgroundProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.HoverBackgroundProperty, value);
            }
        }

        #endregion

        #region HoverForeground

        public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register(
        "HoverForeground",
        typeof(Brush),
        typeof(GridTreeHeaderCellControl),
        new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush HoverForeground
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.HoverForegroundProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.HoverForegroundProperty, value);
            }
        }
#endregion

        //#region HeaderInnerBorderBrush
        
        //public static readonly DependencyProperty HeaderInnerBorderBrushProperty = DependencyProperty.Register(
        //    "HeaderInnerBorderBrush",
        //    typeof(Brush),
        //    typeof(GridTreeHeaderCellControl),
        //    new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        //public Brush HeaderInnerBorderBrush
        //{
        //    get
        //    {
        //        return (Brush)this.GetValue(GridTreeHeaderCellControl.HeaderInnerBorderBrushProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridTreeHeaderCellControl.HeaderInnerBorderBrushProperty, value);
        //    }
        //}

        //#endregion

        //#region HeaderInnerBorderThickness
        
        //public static readonly DependencyProperty HeaderInnerBorderThicknessProperty = DependencyProperty.Register(
        //    "HeaderInnerBorderThickness",
        //    typeof(Thickness),
        //    typeof(GridTreeHeaderCellControl),
        //    new PropertyMetadata(null));

        //public Thickness HeaderInnerBorderThickness
        //{
        //    get
        //    {
        //        return (Thickness)this.GetValue(GridTreeHeaderCellControl.HeaderInnerBorderThicknessProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridTreeHeaderCellControl.HeaderInnerBorderThicknessProperty, value);
        //    }
        //}

        //#endregion

        #region SortWidgetBorderBrush

        
        public static readonly DependencyProperty SortWidgetBorderBrushProperty =
            DependencyProperty.Register("SortWidgetBorderBrush", typeof(Brush), typeof(GridTreeHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        public Brush SortWidgetBorderBrush
        {
            get { return (Brush)GetValue(SortWidgetBorderBrushProperty); }
            set { SetValue(SortWidgetBorderBrushProperty, value); }
        }      

        #endregion

        #region SortWidgetBorderHoverBackgroundBrush       

      
        public static readonly DependencyProperty SortWidgetBorderHoverBackgroundBrushProperty =
            DependencyProperty.Register("SortWidgetBorderHoverBackgroundBrush", typeof(Brush), typeof(GridTreeHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        public Brush SortWidgetBorderHoverBackgroundBrush
        {
            get { return (Brush)GetValue(SortWidgetBorderHoverBackgroundBrushProperty); }
            set { SetValue(SortWidgetBorderHoverBackgroundBrushProperty, value); }
        }       

        #endregion
        #endregion

        public bool IsInSuspend
        {
            get;
            internal set;
        }

        public GridRenderStyleInfo RenderStyle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the TextBlock UIElement associated with the Header Cell Control.
        /// </summary>
        public TextBlock TextBlockPart
        {
            get;
            private set;
        }



        void GridTreeHeaderCellControl_MouseLeave(object sender, MouseEventArgs e)
        {
            
            VisualStateManager.GoToState(this, "Normal", false);
        }

        void GridTreeHeaderCellControl_MouseEnter(object sender, MouseEventArgs e)
        {

            VisualStateManager.GoToState(this, "Normal", false);

            if (this.mainBorder != null)
            {
                var groups = VisualStateManager.GetVisualStateGroups(this.mainBorder);
                var commonStates = groups[0] as VisualStateGroup;
                var visualState = commonStates.States[1] as VisualState;
                var objectParentFrame = visualState.Storyboard.Children[0] as ObjectAnimationUsingKeyFrames;
                var objectBorderFrame = visualState.Storyboard.Children[1] as ObjectAnimationUsingKeyFrames;
                var TextBlockForegroundFrame = visualState.Storyboard.Children[2] as ObjectAnimationUsingKeyFrames;
                var discreteFrame = objectParentFrame.KeyFrames[0] as DiscreteObjectKeyFrame;
                var TextBlockForeground = TextBlockForegroundFrame.KeyFrames[0] as DiscreteObjectKeyFrame;
                var discreteBorderFrame = objectBorderFrame.KeyFrames[0] as DiscreteObjectKeyFrame;
                Storyboard.SetTargetName(objectParentFrame, mainBorder.Name);
                Storyboard.SetTargetName(objectBorderFrame, mainBorder.Name);

                if (e.OriginalSource == sortBorder)
                {
                    Storyboard.SetTargetName(objectParentFrame, sortBorder.Name);
                    Storyboard.SetTargetName(objectBorderFrame, sortBorder.Name);
                    discreteFrame.Value = this.SortWidgetBorderHoverBackgroundBrush;
                    discreteBorderFrame.Value = this.SortWidgetBorderBrush;
                    TextBlockForeground.Value = this.Foreground;
                }
                else
                {
                    discreteFrame.Value = this.HoverBackground;
                    TextBlockForeground.Value = this.HoverForeground;
                    
                }

            }

            VisualStateManager.GoToState(this, "MouseOver", false);
        }


        private void SetSortColumnWidth()
        {
            var sortColumn = this.mainGrid.ColumnDefinitions[1];
            if (this.SortVisibility == Visibility.Visible)
            {
                sortColumn.Width = new GridLength(GridTreeHeaderCellControl.MinWidth, GridUnitType.Pixel);
            }
            else
            {
                sortColumn.Width = new GridLength(0);
            }
        }

        private bool isTemplateApplied = false;

        public override void OnApplyTemplate()
        {
            if (textBorder != null)
            {
                textBorder.MouseEnter -= new MouseEventHandler(GridTreeHeaderCellControl_MouseEnter);
            }

            if (sortBorder != null)
            {
                sortBorder.MouseEnter -= new MouseEventHandler(GridTreeHeaderCellControl_MouseEnter);
            }
            base.OnApplyTemplate();
            this.mainBorder = this.GetTemplateChild("PART_MainBorder") as Border;
            this.mainGrid = this.GetTemplateChild("PART_MainGrid") as System.Windows.Controls.Grid;
            this.TextBlockPart = this.GetTemplateChild("PART_TextBlock") as TextBlock;
            this.sortBorder = this.GetTemplateChild("sortBorder") as Border;
            this.textBorder = this.GetTemplateChild("textBorder") as Border;

            if (TextBlockPart != null)
            {
                var tb = this.TextBlockPart;
                var font = this.RenderStyle.ReadOnlyFont;
                tb.FontFamily = font.FontFamily;
                tb.FontSize = font.FontSize;
                tb.FontStretch = font.FontStretch;
                tb.FontWeight = font.FontWeight;
                tb.FontStyle = font.FontStyle;
                //tb.Foreground = this.RenderStyle.Foreground;
                tb.HorizontalAlignment = this.RenderStyle.HorizontalAlignment;
                tb.Margin = this.RenderStyle.TextMargins.ToThickness();
                tb.Padding = this.RenderStyle.BorderMargins.ToThickness();
                tb.TextDecorations = font.TextDecorations;
                tb.TextWrapping = this.RenderStyle.TextWrapping;
                tb.TextTrimming = this.RenderStyle.TextTrimming;
                tb.VerticalAlignment = this.RenderStyle.VerticalAlignment;
            }

            if (textBorder != null)
            {
                textBorder.MouseEnter += new MouseEventHandler(GridTreeHeaderCellControl_MouseEnter);
            }

            if (sortBorder != null)
            {
                sortBorder.MouseEnter += new MouseEventHandler(GridTreeHeaderCellControl_MouseEnter);
            }

            this.isTemplateApplied = true;
            this.SetProperties();

        }

        private void SetProperties()
        {
            if (this.isTextPropertyChangedBeforeInitialization)
            {
                this.TextBlockPart.Text = this.Text;
            }

            if (this.isSortVisibiltyChangedBeforeInitialization)
            {
                this.SetSortColumnWidth();
            }
        }

    }
}
