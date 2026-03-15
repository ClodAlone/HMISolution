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
    using System.Windows.Media.Imaging;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;


    public class GridTreeExpanderCellModelExt : GridCellModel<GridTreeExpanderCellRendererExt>
    {
        public GridTreeExpanderCellModelExt()
        {
        }
    }

    public class GridTreeExpanderCellRendererExt : GridVirtualizingCellRenderer<GridTreeExpanderCellControlExt>
    {
        public GridTreeExpanderCellRendererExt()
        {
           
            this.AllowRecycle = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;            
            this.IsFocusable = true;
            this.IsEditable = true;           
        }

        public GridTreeExpanderCellModelExt ExpanderCellModel
        {
            get
            {
                return this.CellModel as GridTreeExpanderCellModelExt;
            }
        }

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of CustomPlusPathData of the GridTreeControl.
        /// </summary>
        public Path CustomPlusExpanderGlyphPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of CustomMinusPathData of the GridTreeControl.
        /// </summary>
        public Path CustomMinusExpanderGlyphPath
        {
            get;
            set;
        }


        /// <summary>
        /// While loading the GridTreeControl ExpanderCell Loads with TextBlock. 
        ///</summary>
        /// <param name="rendererElement"></param>
        /// <param name="style"></param>
        protected override void OnInitializeRendererElement(UIElement rendererElement, GridRenderStyleInfo style)
        {
           
            var ExpanderCell = rendererElement as Border;
            var uiElement = new GridTreeExpanderCellControlExt();
            ExpanderCell.Child = uiElement;
            var treeGrid = this.GridControl.FindParentElementOfType<GridTreeControl>();          
            TextBlock tb = new TextBlock();
            tb.Measure(new Size(double.MaxValue, double.MaxValue));
            var font = style.ReadOnlyFont;
            tb.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontStretch = font.FontStretch;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
            tb.Foreground = style.Foreground;
            tb.Padding = new Thickness(0);
            uiElement.Background = style.Background;
            tb.HorizontalAlignment = style.HorizontalAlignment;
          
           
            var width = treeGrid.InternalGrid.ColumnWidths[1];
            tb.Width = width;
            tb.VerticalAlignment = style.VerticalAlignment;
            uiElement.ChildElement = tb;
            var n = style.Tag as GridTreeNode;
            uiElement.Margin = new Thickness((n.Level + 1) * nodeColumnWidth, 0, 0, 0);
            uiElement.GridControl = style.GridControl;
            uiElement.PlusPath = treeGrid.InternalGrid.GetGridTreeExpanderPlusPath(VisualStyle);
            uiElement.MinusPath = treeGrid.InternalGrid.GetGridTreeExpanderMinusPath(VisualStyle);
            uiElement.PlusButtonBackground = treeGrid.InternalGrid.GetGridTreeExpanderBackground(VisualStyle);
            uiElement.MinusButtonBackground = treeGrid.InternalGrid.GetGridTreeExpanderExpandedBackground(VisualStyle);
            uiElement.PlusButtonBorderBrush = treeGrid.InternalGrid.GetGridTreeExpanderBorderBrush(VisualStyle);
            uiElement.MinusButtonBorderBrush = treeGrid.InternalGrid.GetGridTreeExpanderExpandedBorderBrush(VisualStyle);
            uiElement.ExpandGlyphType = this.ExpandGlyphType;
            uiElement.Node = style.Tag as GridTreeNode;
            uiElement.IsInSuspend = false;
            
            uiElement.Foreground = style.Foreground;
            uiElement.VerticalAlignment = style.VerticalAlignment;
           
           
            VisualContainer.SetWantsMouseInput(uiElement, false);          

            if (this.CustomPlusExpanderGlyphPath != null && this.CustomMinusExpanderGlyphPath != null)
            {
                (uiElement as GridTreeExpanderCellControlExt).CustomPlusPath = this.CustomPlusExpanderGlyphPath;
                (uiElement as GridTreeExpanderCellControlExt).CustomMinusPath = this.CustomMinusExpanderGlyphPath;
            }
        }

        /// <summary>
        /// Called when [element measured].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="size">The size.</param>
        protected override void OnElementMeasured(UIElement el, Size size)
        {
            var border = el as Border;
            {
                if (border != null && border.Child != null)
                {
                    var expander = border.Child as GridTreeExpanderCellControlExt;
                    if (expander != null && (expander.ChildElement as TextBlock) != null)
                    {
                        (expander.ChildElement as TextBlock).Width = border.Width; //TextBlock width set in OnInitialize RenderElement. If we resize the Expander Column, then Modified widhth doesnt affect in the TextBlock. Here we have set Textblock Width.
                    }
                }
            }
            base.OnElementMeasured(el, size);
        }

        /// <summary>
        /// While entering into Edit mode TextBox Should be load to Edit the values. In this method we have load the TextBox.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="style"></param>

        public override void OnInitializeContent(GridTreeExpanderCellControlExt uiElement, GridRenderStyleInfo style)
        {
            this.OnUnwireUIElement(uiElement);
            var treeGrid = this.GridControl.FindParentElementOfType<GridTreeControl>();
            uiElement.IsInSuspend = true;
            uiElement.IsContentInitialized = true;
            uiElement.Background = style.Background;
            base.OnInitializeContent(uiElement, style);

            //TextBox Properties
            TextBox tb = new TextBox();
            tb.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            var font = style.ReadOnlyFont;
            tb.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontStretch = font.FontStretch;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
            tb.Foreground = style.Foreground;
            tb.Background = style.Background;
            tb.VerticalAlignment = style.VerticalAlignment;
            tb.VerticalContentAlignment = style.VerticalAlignment;
            tb.HorizontalContentAlignment = style.HorizontalAlignment;
         
            tb.BorderThickness = new Thickness(0);
            //tb.BorderBrush = Brushes.Transparent;
            //if (treeGrid.InternalGrid.Model.Options.ShowCurrentCell)
            //{
            //    var value=treeGrid.InternalGrid.GetGridTreeCurrentCellBorderWidth(VisualStyle);
            //    tb.BorderThickness = new Thickness(value);
            //    tb.BorderBrush = treeGrid.InternalGrid.GetGridTreeCurrentCellBorderBrush(VisualStyle);
            //}
            

            var n = style.Tag as GridTreeNode;            
            uiElement.BorderMargin = new Thickness(0,0, (n.Level + 1) * nodeColumnWidth,0);
            uiElement.VerticalAlignment = style.VerticalAlignment;
            uiElement.VerticalContentAlignment = style.VerticalAlignment;
            uiElement.HorizontalAlignment = style.HorizontalAlignment;
            uiElement.HorizontalContentAlignment = style.HorizontalAlignment;
            uiElement.ChildElement = tb;
            uiElement.GridControl = style.GridControl;
            uiElement.PlusPath = treeGrid.InternalGrid.GetGridTreeExpanderPlusPath(VisualStyle);
            uiElement.MinusPath = treeGrid.InternalGrid.GetGridTreeExpanderMinusPath(VisualStyle);
            uiElement.PlusButtonBackground = treeGrid.InternalGrid.GetGridTreeExpanderBackground(VisualStyle);
            uiElement.MinusButtonBackground = treeGrid.InternalGrid.GetGridTreeExpanderExpandedBackground(VisualStyle);
            uiElement.MinusButtonBorderBrush = treeGrid.InternalGrid.GetGridTreeExpanderExpandedBorderBrush(VisualStyle);
            uiElement.PlusButtonBorderBrush = treeGrid.InternalGrid.GetGridTreeExpanderBorderBrush(VisualStyle);
            uiElement.ExpandGlyphType = this.ExpandGlyphType;
            uiElement.Node = style.Tag as GridTreeNode;
            uiElement.IsInSuspend = false;          
           // uiElement.ExpandGlyphType = this.ExpandGlyphType;
            uiElement.Foreground = style.Foreground;
            
           
            VisualContainer.SetWantsMouseInput(uiElement, false);           
            this.OnWireUIElement(uiElement);
            if (this.CustomPlusExpanderGlyphPath != null && this.CustomMinusExpanderGlyphPath != null)
            {
                (uiElement as GridTreeExpanderCellControlExt).CustomPlusPath = this.CustomPlusExpanderGlyphPath;
                (uiElement as GridTreeExpanderCellControlExt).CustomMinusPath = this.CustomMinusExpanderGlyphPath;
            }
        }

        private GridTreeExpandGlyph expandGlyphType = GridTreeExpandGlyph.Triangle;
        public GridTreeExpandGlyph ExpandGlyphType
        {
            get { return expandGlyphType; }
            set
            {
                expandGlyphType = value;
                switch (expandGlyphType)
                {
                    case GridTreeExpandGlyph.PlusMinus:
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Triangle:
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Custom:
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Themed:
                        NodeColumnWidth = 10;
                        break;
                    default:
                        this.ExpandGlyphType = GridTreeExpandGlyph.Triangle;
                        break;
                }
            }
        }


        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlText = this.GetControlText(this.CurrentStyle);
        }

        protected override string GetControlTextFromEditorCore(GridTreeExpanderCellControlExt uiElement)
        {
            TextBox textBox=uiElement.ChildElement as TextBox;
            if (textBox != null)
            {
                return textBox.Text;
            }
            return string.Empty;

        }

        protected override void OnWireUIElement(GridTreeExpanderCellControlExt uiElement)
        {
            base.OnWireUIElement(uiElement);
            TextBox textBox=uiElement.ChildElement as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                textBox.SelectionChanged += new RoutedEventHandler(textBox_SelectionChanged);                
            }

        }

        protected override void OnSetFocus()
        {
            var _textBox=this.CurrentCellUIElement.ChildElement as TextBox;
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
            {
                if (_textBox != null)
                {
                    _textBox.SelectAll();
                }
            }
            else
            {
                if(_textBox!=null)
                _textBox.Select(_textBox.Text.Length, 0);
                _textBox.Focus();
            }
        }

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
           GridControl.InvalidateCell(CellRowColumnIndex);
        }


        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing)
                return;

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            GridTreeExpanderCellControlExt cellControl = CurrentCellUIElement;
            if (cellControl != null)
            {
                TextBox tb = cellControl.ChildElement as TextBox;

                if (tb != null)
                {
                    /// tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\b]");
                    string str = regex.Replace(e.Text, "");
                    if (!str.Equals("\r") || GridControl.Model.EnableMultiline)
                    {
                        tb.Text = str;
                        tb.Select(tb.Text.Length, 1);
                    }
                    tb.Focus();
                }
            }
            
            
            e.Handled = true;
        }

       

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!this.IsInArrange && IsCurrentCell(textBox) && !CurrentCell.IsInEndEdit)
            {
                if (!SetControlText(textBox.Text))
                {
                    RefreshContent();
                }
            }
        }

        protected override void OnUnwireUIElement(GridTreeExpanderCellControlExt uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            TextBox textBox = uiElement.ChildElement as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                textBox.SelectionChanged += new RoutedEventHandler(textBox_SelectionChanged);
               
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.CurrentCell.IsEditing)
                    this.CurrentCell.EndEdit();
                this.CurrentCell.MoveRight();
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridTreeExpanderCellControlExt uiElement, GridRenderStyleInfo style)
        {
            var n = style.Tag as GridTreeNode;
            uiElement.Margin = new Thickness((n.Level + 1) * nodeColumnWidth, 0,0, 0);
            var TextBox = uiElement.ChildElement as TextBox;
            if (TextBox != null)
            {
                TextBox.Height = aca.CellRect.Height;                
            }
            base.ArrangeUIElement(aca, uiElement, style);  
        }

        private bool IsNodeLast(int rowIndex)
        {
            GridTreeControlImpl tree = this.GridControl as GridTreeControlImpl;
            GridTreeNode n = tree.GetNodeAtRowIndex(rowIndex);
            GridTreeNode nextNode = tree.GetNodeAtRowIndex(rowIndex + 1);
            return nextNode == null || n.Level != nextNode.Level ||
                (n.ParentNode != null && n.ParentNode.ChildNodes.IndexOf(n) == n.ParentNode.ChildNodes.Count - 1);
        }

        private double nodeColumnWidth = 10;

        public double NodeColumnWidth
        {
            get { return nodeColumnWidth; }
            set { nodeColumnWidth = value; }
        }

        private IGridTreeVisualStyle visualStyle = new GridTreeDefaultGridVisualStyle();

        public IGridTreeVisualStyle VisualStyle
        {
            get { return visualStyle; }
            set { visualStyle = value; }
        }
    }
}
