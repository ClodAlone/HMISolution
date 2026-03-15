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
    using System.Xml;    
    using System.Windows.Automation.Provider;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Cells;
    using System.Text;
    using System.Reflection;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Markup;
    using Syncfusion.Windows.Controls.Grid.GridUtils;
    using System.Windows.Media.Imaging;


    /// <summary>
    ///  This Class derives the RichTextBox Control. 
    /// </summary>
    public class RichTextBoxAdv : RichTextBox
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Grid.RichTextBoxAdv">RichTextBoxAdv</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public RichTextBoxAdv()
        {
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            

        }
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" />
        /// event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = false;
        }



    }

  

    /// <summary>
    ///  This Class defines the Model of RichTextBoxRenderer
    /// </summary>
    public class GridCellRichTextBoxModel : GridCellModel<GridCellRichTextBoxCellRenderer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Grid.GridCellRichTextBoxModel">GridCellRichTextBoxModel</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public GridCellRichTextBoxModel()
        {

        }

        /// <summary>
        /// Returns the Plain text of the Cell Value. 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        /// <param name="textInfo"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            StringBuilder FormatText = new StringBuilder();
            Paragraph para = value as Paragraph;             
            foreach (var run in para.Inlines)
            {
                if (run is Run)
                {
                    Run rn = run as Run;
                    FormatText.Append(rn.Text);
                }
              
            }
            return FormatText.ToString();
        }


      

    }


    public class GridCellRichTextBoxCellRenderer : GridVirtualizingCellRenderer<RichTextBoxAdv>
    {

      
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Grid.GridCellRichTextBoxCellRenderer">GridCellRichTextBoxCellRenderer</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public GridCellRichTextBoxCellRenderer()
        {
            this.AllowRecycle = false;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = false;
            this.IsFocusable = true;
            this.AllowTransparentBackground = false;
            IsControlTextShown = false;
       
            
        }


        /// <summary>
        /// Initilize the Control
        /// </summary>
        /// <remarks></remarks>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// OnInitilizeContent defines vhte styles and content of the RichTextBox
        /// </summary>
        /// <param name="richtextBox"></param>
        /// <param name="style"></param>
        /// <remarks></remarks>
        public override void OnInitializeContent(RichTextBoxAdv richtextBox, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(richtextBox, style);
            this.OnUnwireUIElement(richtextBox);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 4);
            margins.Right = Math.Max(0, margins.Right - 2);
            var font = style.ReadOnlyFont;
            var rtb = richtextBox;
            rtb.AcceptsReturn = true;
            rtb.Margin = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            rtb.Padding = rtb.Margin;
            rtb.TextWrapping = style.TextWrapping;
            rtb.VerticalAlignment = style.VerticalAlignment;
            richtextBox.BorderThickness = new Thickness(0);
            richtextBox.UseLayoutRounding = true;
            this.AllowCancelMouseCapture = true;
            VisualContainer.SetWantsMouseInput(richtextBox, true);
            richtextBox.Padding = new Thickness(0);
            richtextBox.IsReadOnly = style.ReadOnly;
            richtextBox.Background = style.Background;
            richtextBox.HorizontalAlignment = style.HorizontalAlignment;
            richtextBox.VerticalAlignment = style.VerticalAlignment;
            SetRichTextControlValue(style, richtextBox);

            this.GridControl.InvalidateCell(new RowColumnIndex(style.CellRowColumnIndex.RowIndex, style.CellRowColumnIndex.ColumnIndex));

            this.OnWireUIElement(richtextBox);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, RichTextBoxAdv uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            uiElement.Padding = margins;
            base.ArrangeUIElement(aca, uiElement, style);
        }

        /// <summary>
        /// SetRichTextControlValue() method set the value for RichTextBox. 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="rtb"></param>
        /// <remarks></remarks>
        public void SetRichTextControlValue(GridRenderStyleInfo style, RichTextBoxAdv rtb)
        {
            string xamlstring = string.Empty;
            if (style.CellValue is Paragraph)
            {
                try
                {
                    //Add for the current cell selection issue
                    if (rtb.Blocks.Count == 0)
                    {
                        xamlstring = GridUtility.ConvertParagraphToXaml(style.CellValue as Paragraph);
                        rtb.Blocks.Add(XamlReader.Load(xamlstring) as Block);
                        //style.CellValue = XamlReader.Load(xamlstring) as Paragraph;
                        //this.GridControl.Model[style.RowIndex, style.ColumnIndex].CellValue = style.CellValue;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Cell value is unidentified");
                }
            }
            else if (style.CellValue is List<Paragraph>)
            {
                try
                {                  
                    List<Paragraph> Paragraphs = style.CellValue as List<Paragraph>;
                    int Previousblocks = rtb.Blocks.Count;
                    foreach (Paragraph paragraph in Paragraphs)
                    {
                        xamlstring = GridUtility.ConvertParagraphToXaml(paragraph);
                        rtb.Blocks.Add(XamlReader.Load(xamlstring) as Paragraph);
                        xamlstring = string.Empty;
                    }
                    for (int count = 0; count < Previousblocks; count++)
                        rtb.Blocks.RemoveAt(count);
                    style.CellValue = XamlReader.Load(xamlstring) as List<Paragraph>;
                    this.GridControl.Model[style.RowIndex, style.ColumnIndex].CellValue = style.CellValue;
                }
                catch (Exception)
                {
                    throw new Exception("Cell value is unidentified");
                }
            }
            else
            {
                throw new ArgumentNullException("CellValue", "CellValue is not supported. CellValue must be Paragraph or List<Paragraph>");
            }     
        }

        /// <summary>
        /// Wires All the Events for UIElement - RichTextBoxAdv
        /// </summary>
        /// <param name="uiElement"></param>
       protected override void OnWireUIElement(RichTextBoxAdv uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
            uiElement.ContentChanged += new ContentChangedEventHandler(uiElement_ContentChanged);         
            uiElement.MouseLeftButtonDown += new MouseButtonEventHandler(uiElement_MouseLeftButtonDown);
            uiElement.MouseEnter += new MouseEventHandler(uiElement_MouseEnter);
            uiElement.MouseLeave += new MouseEventHandler(uiElement_MouseLeave);
            uiElement.MouseMove += new MouseEventHandler(uiElement_MouseMove);        
        }

       /// <summary>
       /// Unwires all the Event for UIElement. 
       /// </summary>
       /// <param name="uiElement"></param>
       /// <remarks></remarks>
       protected override void OnUnwireUIElement(RichTextBoxAdv uiElement)
       {
           base.OnUnwireUIElement(uiElement);
           uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
           uiElement.ContentChanged -= new ContentChangedEventHandler(uiElement_ContentChanged);
           uiElement.MouseLeftButtonDown -= new MouseButtonEventHandler(uiElement_MouseLeftButtonDown);
           uiElement.MouseEnter -= new MouseEventHandler(uiElement_MouseEnter);
           uiElement.MouseLeave -= new MouseEventHandler(uiElement_MouseLeave);
           uiElement.MouseMove -= new MouseEventHandler(uiElement_MouseMove);
       }
       void uiElement_MouseMove(object sender, MouseEventArgs e)
       {
           UpdateRichTextBox(sender as RichTextBox, e);
       }

       void uiElement_MouseLeave(object sender, MouseEventArgs e)
       {
           UpdateRichTextBox(sender as RichTextBox, e);
       }

       void uiElement_MouseEnter(object sender, MouseEventArgs e)
       {
           UpdateRichTextBox(sender as RichTextBox, e);
       }
       public void UpdateRichTextBox(RichTextBox Richtextbox, MouseEventArgs e)
       {
           Point pt = e.GetPosition(this.GridControl);
           RowColumnIndex rci = this.GridControl.PointToCellRowColumnIndex(pt);
           CoveredCellInfo Coveredcell = this.GridControl.CoveredCells.GetCoveredCell(rci.RowIndex, rci.ColumnIndex);
           if (Coveredcell != null)
           {
               rci.RowIndex = Coveredcell.Top;
               rci.ColumnIndex = Coveredcell.Left;
           }
           this.GridControl.InvalidateCell(new RowColumnIndex(rci.RowIndex,rci.ColumnIndex));
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs">MouseButtonEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        void uiElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CurrentCell != null && this.GridControl != null)
            {
                Point pt = e.GetPosition(this.GridControl);
                RowColumnIndex rci = this.GridControl.PointToCellRowColumnIndex(pt);
                this.GridControl.CurrentCell.MoveTo(rci);

                if (!(sender as RichTextBox).IsReadOnly)
                {
                    if (!this.CurrentCell.IsEditing && !this.CurrentCellUIElement.IsReadOnly)
                        this.CurrentCell.BeginEdit();
                    RichTextBox Richtextbox = sender as RichTextBox;
                    if (Richtextbox.Blocks.Count == 1)
                    {
                        Paragraph cellvalue = Richtextbox.Blocks[0] as Paragraph;
                        this.GridControl.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex].CellValue = cellvalue;
                    }
                }
            }
            UpdateRichTextBox(sender as RichTextBox, e);
            e.Handled = true;
        }

        /// <summary>
        /// ShouldGridTryToHandlePreviewKeyDown verifies the key down event should be handles by Grid or Cell. 
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.KeyEventArgs">KeyEventArgs</see> that contains the event data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            bool isAltKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (isShiftKey && this.GridControl.Model.EnableMultiline)
                        {
                            return false;
                        }
                    }
                    break;
                case Key.Tab:
                    return true;
                case Key.F2:
                    {
                        if (!CurrentCell.IsEditing)
                            CurrentCell.BeginEdit();
                        else
                            CurrentCell.EndEdit();
                        return false;
                    }                    

                case Key.End:
                    return true;
                case Key.Home:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }

                case Key.Delete:
                    {
                        if (!CurrentCell.IsEditing)
                        {
                            CurrentCell.BeginEdit(true);
                            if (CurrentCellUIElement != null)
                            {
                                RichTextBoxAdv Rtb = this.CurrentCellUIElement as RichTextBoxAdv;
                                if (Rtb.Blocks.Count > 0)
                                    Rtb.Blocks.Clear();
                            }
                        }
                        return false;
                    }

            }
            return false;
        }
      
        void uiElement_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            RichTextBox Richtextbox = sender as RichTextBox;
            if(!Richtextbox.IsReadOnly)
            {
                if (Richtextbox.Blocks.Count == 1)
                {
                    try
                    {
                        Paragraph cellvalue = Richtextbox.Blocks[0] as Paragraph;
                        this.GridControl.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex].CellValue = cellvalue;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

                this.GridControl.InvalidateCell(new RowColumnIndex(this.GridControl.CurrentCell.RowIndex, this.GridControl.CurrentCell.ColumnIndex));
            }
            
        }

        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
            this.GridControl.InvalidateCell( new RowColumnIndex(this.GridControl.CurrentCell.RowIndex, this.GridControl.CurrentCell.ColumnIndex));
        }

       
    }
    
}
