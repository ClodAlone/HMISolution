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
using System.Windows.Controls;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base;
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Input;

#if !SILVERLIGHT
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Media;

namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// A cell model for the <see cref="PivotGridHyperlinkCellRenderer"/>.
    /// </summary>
  
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
     public class PivotGridHyperlinkCellModel : GridCellModel<PivotGridHyperlinkCellRenderer>
    {
        /// <summary>
        ///  Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
                return Size.Empty;

            Thickness margins = style.TextMargins.ToThickness();

            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            size.Width += 20;
            return size;
        }
    }
    /// <summary>
    /// Implements the renderer part of a PivotGrid template cell.
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGridHyperlinkCellRenderer : GridVirtualizingCellRenderer<PivotGridHyperlinkCell>
    {

#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(PivotGridHyperlinkCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.Style = identity.Style;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.DataContext = identity.PivotCellInfo;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.IsEnabledOnMouseOver = uiElement.GridControlBase.EnableHyperlinkOnMouseOver;
#if !SILVERLIGHT
                if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                    uiElement.FlowDirection = style.FlowDirection;
                    double m11 = -1;
                    double m22 = 1;
                    double offsetX = uiElement.ActualWidth;
                    double offsetY = 0;
                    uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
                }
#endif
            }
        } 
#endif


        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(PivotGridHyperlinkCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.Style = identity.Style;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.DataContext = identity.PivotCellInfo;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;                
            }
        }

    }
    /// <summary>
    /// A ContentControl derived class that aggregates the functionality of a PivotGridHyperlinkCell object into a
    /// ContentControl
    /// </summary>
#if SyncfusionFramework4_0 
    [DesignTimeVisible(false)]
#endif
    public class PivotGridHyperlinkCell : ContentControl
    {
#if !SILVERLIGHT
        static PivotGridHyperlinkCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotGridHyperlinkCell), new FrameworkPropertyMetadata(typeof(PivotGridHyperlinkCell)));
        }
#else
        public PivotGridHyperlinkCell()
        {
            DefaultStyleKey = typeof(PivotGridHyperlinkCell);
        }
#endif
        /// <summary>
        /// Gets the Internal grid
        /// </summary>
        /// <remarks>
        /// This is for advance Appearance.
        /// </remarks>
        public PivotGridControlBase GridControlBase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PivotCellInfo PivotCellInfo
        {
            get { return (PivotCellInfo)GetValue(PivotCellInfoProperty); }
            set { SetValue(PivotCellInfoProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text block of the value cell
        /// </summary>
        public TextBlock ValueTextBlock { get; set; }
        /// <summary>
        /// Gets the Horixontal alignment of the cells
        /// </summary>
        public HorizontalAlignment HAlignment
        {
            get
            {
                if (this.PivotCellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()))                    
                {
                    return System.Windows.HorizontalAlignment.Right;
                }
                else
                    return System.Windows.HorizontalAlignment.Left;
            }
            set
            {
            }
        }
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridHyperlinkCell.PivotCellInfo"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridHyperlinkCell.PivotCellInfo"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotCellInfoProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("PivotCellInfo", typeof(PivotCellInfo), typeof(PivotGridHyperlinkCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("PivotCellInfo", typeof(PivotCellInfo), typeof(PivotGridHyperlinkCell), new PropertyMetadata(null));
#endif

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ValueTextBlock = GetTemplateChild("PART_CellValueTextBlock") as TextBlock;
            if (this.ValueTextBlock != null)
            {
#if !SILVERLIGHT
                this.ValueTextBlock.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ValueTextBlock_PreviewMouseLeftButtonDown);
#else
                this.ValueTextBlock.MouseEnter += new MouseEventHandler(ValueTextBlock_MouseEnter);
                this.ValueTextBlock.MouseLeave += new MouseEventHandler(ValueTextBlock_MouseLeave);
                this.ValueTextBlock.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ValueTextBlock_PreviewMouseLeftButtonDown);                
#endif
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);            
        }

        /// <summary>
        /// Handles the MouseEnter event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);            
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var cellrowcol = this.GridControlBase.PointToCellRowColumnIndex(e.GetPosition(this.GridControlBase));
            this.GridControlBase.GridControl.RaiseHyperlinkCellClick(new HyperlinkCellClickEventArgs(this.PivotCellInfo, cellrowcol, e));
        }

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridHyperlinkCell.IsEnabledOnMouseOver"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridHyperlinkCell.IsEnabledOnMouseOver"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsEnabledOnMouseOverProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("IsEnabledOnMouseOver", typeof(bool), typeof(PivotGridHyperlinkCell), new UIPropertyMetadata(true));
#else
            DependencyProperty.Register("IsEnabledOnMouseOver", typeof(bool), typeof(PivotGridHyperlinkCell), new PropertyMetadata(true));
#endif
        /// <summary>
        /// Gets or sets whether to enable the hyperlink in row pivot cell only on mouse over.
        /// </summary>
        public bool IsEnabledOnMouseOver
        {
            get { return (bool)GetValue(IsEnabledOnMouseOverProperty); }
            set { SetValue(IsEnabledOnMouseOverProperty, value); }
        }
    }
    
}
