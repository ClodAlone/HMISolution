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
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;
using Syncfusion.Windows;
#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;

namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// A cell model for the <see cref="PivotGridHyperlinkCellRenderer"/>.
    /// </summary>
    public class PivotGridTemplateCellModel : GridCellModel<PivotGridTemplateCellRenderer>
    {
        /// <summary>
        /// Calculate the size of the cell
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="style">style information</param>
        /// <param name="queryBounds">Grid Query bounds</param>
        /// <returns>Size</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {           
            PivotGridStyleInfoIdentity styleInfoIdentity = style.CellIdentity as PivotGridStyleInfoIdentity;
            if (styleInfoIdentity != null && styleInfoIdentity.Style != null)
            {
                PivotGridTemplateCell cellControl = new PivotGridTemplateCell();
                cellControl.PivotCellInfo = styleInfoIdentity.PivotCellInfo;
                cellControl.DataContext = styleInfoIdentity.PivotCellInfo;
                cellControl.Style = styleInfoIdentity.Style;
                cellControl.Measure(new Size(double.MaxValue, double.MaxValue));
                return cellControl.DesiredSize;
            }
            return new Size(10, 10);
        }     
    }
    /// <summary>
    /// Implements the renderer part of a PivotGrid template cell.
    /// </summary>
    public class PivotGridTemplateCellRenderer : GridVirtualizingCellRenderer<PivotGridTemplateCell>
    {
#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(PivotGridTemplateCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.Style = identity.Style;
                uiElement.IsExpanded = identity.IsExpanded;
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.DataContext = identity.PivotCellInfo;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.CellIdentity = identity;
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
        public override void OnInitializeContent(PivotGridTemplateCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.Style = identity.Style;
                uiElement.IsExpanded = identity.IsExpanded;
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.DataContext = identity.PivotCellInfo;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.CellIdentity = identity;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Initialize"/> event.
        /// </summary>
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = GetControlValueFromEditor();
        }

        /// <summary>
        /// Raises the <see cref="E:SetFocus"/> event.
        /// </summary>
        
        protected override void OnSetFocus()
        {
            base.OnSetFocus();
        }
        /// <summary>
        /// method returns the control value of the cell
        /// </summary>
        /// <returns>The cell value</returns>
        protected override object GetControlValueFromEditor()
        {
            if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
            {
                var wrapperInstance = ((PivotGridTemplateCell)this.CurrentCellUIElement).DataContext as GridCellBoundWrapper; //uiElement.DataSource changed to uiElement.DataContext because WrapperInstance created for DataContext.
                if (wrapperInstance != null)
                {
                    return wrapperInstance.Style.CellValue;

                }
            }
            return base.GetControlValueFromEditor();
        }

        /// <summary>
        /// Raises the <see cref="E:Activate"/> event.
        /// </summary>
          
        protected override void OnActivated()
        {
            if ((PivotGridTemplateCell)this.CurrentCellUIElement != null && ((PivotGridTemplateCell)this.CurrentCellUIElement).GridControlBase.GridControl.EnableValueEditing)
            {
                var tb = this.CurrentCellUIElement.FindElementOfType<TextBox>();
                if (tb == null)
                {
                    return;
                }
                if (tb != null)
                {
                    tb.TextChanged -= new TextChangedEventHandler(tb_TextChanged);
                    tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                    tb.Focus();
                    tb.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tb.Select(0, tb.Text.Length);
                    }));
                }
            }                     
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        /// <summary>
        /// Raises the <see cref="E:Deactivate"/> event.
        /// </summary>
        protected override void OnDeactivated()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (GridControl is GridControl)
                GridControl.InvalidateVisual(); // Temporarily this code added here to avoid loding problem of CellItemTemplate. 
            else
                GridControl.InvalidateVisual(false);

        }

        /// <summary>
        /// Raises the <see cref="E:EnteredEditMode"/> event.
        /// </summary>
        
        protected override void OnEnteredEditMode()
        {
            base.OnEnteredEditMode();
        }

        /// <summary>
        /// Raises the <see cref="E:EditingComplete"/> event.
        /// </summary>
        protected override void OnEditingComplete()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (GridControl is GridControl)
                GridControl.InvalidateVisual(); // Temporarily this code added here to avoid loding problem of CellItemTemplate. 
            else
                GridControl.InvalidateVisual(false);
        }
        /// <summary>
        /// Indicates whether the CurrentCellUIElement should handle the key  and the grid should ignore it.
        /// </summary>
        /// <param name="e">The event argument</param>
        /// <returns>bool</returns>
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
#if SILVERLIGHT
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
#else
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
#endif
            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case Key.Tab:
                    return true;

                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    {
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !CurrentCell.IsEditing;
                    }

                case Key.End:
                case Key.Home:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }

                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }
                case Key.Enter:
                    {
                        CurrentCell.MoveRight();
                        e.Handled = true;
                        return false;
                    }

                case Key.Back:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
        /// <summary>
        /// Call when edit the current cell
        /// </summary>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
     
        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing)
            {

                RefreshContent();
                return;
            }
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
        }
        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tb = sender as TextBox;

            if (!(tb.Text.Equals((object)this.CurrentStyle.Text)) || IsModified)
            {
                if (!this.IsInArrange && IsCurrentCell(tb) && !CurrentCell.IsInEndEdit)
                {
                    this.CurrentStyle.Text = tb.Text;
                }
            }
}
        /// <summary>
        /// Calls when click on cell
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">an event argument</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {

            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0))
            {
                CurrentCell.BeginEdit(true);
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

    }
    /// <summary>
    /// A ContentControl derived class that aggregates the functionality of a PivotGridTemplateCell object into a
    /// ContentControl
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGridTemplateCell :ContentControl
    {
        #if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="PivotGridTemplateCell"/> class.
        /// </summary>
        static PivotGridTemplateCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotGridTemplateCell), new FrameworkPropertyMetadata(typeof(PivotGridTemplateCell)));
        }
#else
        /// <summary>
        /// Initializes the <see cref="PivotGridTemplateCell"/> class.
        /// </summary>
        public PivotGridTemplateCell()
        {
            DefaultStyleKey = typeof(PivotGridTemplateCell);
        }
#endif
        /// <summary>
        /// Gets or sets the custom expander.
        /// </summary>
        /// <value>The custom expander.</value>
        public UIElement CustomExpander { get; set; }

        /// <summary>
        /// Gets or sets the grid control base.
        /// </summary>
        /// <value>The grid control base.</value>
        public PivotGridControlBase GridControlBase { get; set; }

        /// <summary>
        /// Gets or sets the pivot cell info.
        /// </summary>
        /// <value>The pivot cell info.</value>
        public PivotCellInfo PivotCellInfo
        {
            get { return (PivotCellInfo)GetValue(PivotCellInfoProperty); }
            set { SetValue(PivotCellInfoProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                if (this.PivotCellInfo.Tag == null && this.PivotCellInfo.CellType.ToString().Contains(PivotCellType.ExpanderCell.ToString()))
                    return true;
                else 
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                if (this.PivotCellInfo.Tag == null)
                    return this.PivotCellInfo.FormattedText;
                else
                {
                    GridStyleInfo styleInfo = this.PivotCellInfo.Tag as GridStyleInfo;
                    return ((PivotGridStyleInfoIdentity)this.GridControlBase.Model[styleInfo.RowIndex, styleInfo.ColumnIndex].CellIdentity).PivotCellInfo.FormattedText;
                }
            }
        }
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridTemplateCell.PivotCellInfo"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridTemplateCell.PivotCellInfo"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotCellInfoProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("PivotCellInfo", typeof(PivotCellInfo), typeof(PivotGridTemplateCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("PivotCellInfo", typeof(PivotCellInfo), typeof(PivotGridTemplateCell), new PropertyMetadata(null));
#endif
        /// <summary>
        /// Gets or sets the style info.
        /// </summary>
        /// <value>The style info.</value>
        public GridStyleInfo StyleInfo { get; set; }

        /// <summary>
        /// Gets or sets the cell identity.
        /// </summary>
        /// <value>The cell identity.</value>
        public PivotGridStyleInfoIdentity CellIdentity { get; set; }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CustomExpander = GetTemplateChild("PART_Expander") as UIElement;
                if (CustomExpander != null)
                {
#if !SILVERLIGHT

                    CustomExpander.PreviewMouseLeftButtonDown +=
                        new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown);
#else
                //CustomExpander.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown);
                    CustomExpander.AddHandler(UIElement.MouseLeftButtonDownEvent, new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown), true);
#endif
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the CustomExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CustomExpander_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.StyleInfo != null && this.PivotCellInfo != null)
            {
                if (this.PivotCellInfo.Tag == null)
                {
                    this.GridControlBase.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(this.PivotCellInfo), this.StyleInfo);
                }
                else
                {
                    PivotGridStyleInfoIdentity cellIdentity = ((GridStyleInfo)this.PivotCellInfo.Tag).Identity as PivotGridStyleInfoIdentity;
                    this.GridControlBase.CurrentCell.Deactivate();
                    this.GridControlBase.GridControl.RaiseOnExpanding(new ExpandingEventArgs(this.PivotCellInfo),
                        this.GridControlBase.GridControl.PivotEngine[cellIdentity.RowIndex, cellIdentity.ColumnIndex]);
                }
            }
        }
    }
}
