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
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class LineItem : FrameworkElement
    {
        #region Dependency Properties

        /// <summary>
        /// DependencyProperty for IsExpanded
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(LineItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnExpandedChanged)));

        /// <summary>
        /// DependencyProperty for ContainsLines
        /// </summary>
        public static readonly DependencyProperty ContainsLinesProperty = DependencyProperty.Register("ContainsLines", typeof(bool), typeof(LineItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnContainsLinesChanged)));

        /// <summary>
        /// DependencyProperty for Text
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LineItem), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// DependencyProperty for IsChildrenExpanded
        /// </summary>
        internal static readonly DependencyProperty IsChildrenExpandedProperty = DependencyProperty.Register("IsChildrenExpanded", typeof(bool), typeof(LineItem), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for LineNumber
        /// </summary>
        public static readonly DependencyProperty LineNumberProperty =
            DependencyProperty.Register("LineNumber", typeof(int), typeof(EditControl), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets the SelectionPath
        /// </summary>
        private static readonly DependencyProperty SelectionPathProperty = DependencyProperty.Register("SelectionPath", typeof(RectangleGeometry), typeof(LineItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectionPathChanged)));

        /// <summary>
        /// Gets or sets the IsSelected
        /// </summary>
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LineItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSelectionPathChanged)));

        /// <summary>
        /// Gets or sets EndLine property
        /// </summary>
        public static readonly DependencyProperty EndLineProperty =
            DependencyProperty.Register("EndLine", typeof(int), typeof(LineItem), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets StartLine property
        /// </summary>
        public static readonly DependencyProperty StartLineProperty =
            DependencyProperty.Register("StartLine", typeof(int), typeof(LineItem), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets ParentLineNumber property
        /// </summary>
        public static readonly DependencyProperty ParentLineNumberProperty =
            DependencyProperty.Register("ParentLineNumber", typeof(int), typeof(LineItem), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets IsEndLine Property
        /// </summary>
        public static readonly DependencyProperty IsEndLineProperty =
            DependencyProperty.Register("IsEndLine", typeof(bool), typeof(EditControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        ///  // Using a DependencyProperty as the backing store for LineState.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineStateProperty =
            DependencyProperty.Register("LineState", typeof(LineModificationState), typeof(LineItem), new FrameworkPropertyMetadata(LineModificationState.Unchanged));

        #endregion Dependency Properties

        #region Delegates and Events

        /// <summary>
        /// Occurs when <see cref="LineItem"/> is Changed.
        /// </summary>
        [Description("Occurs when <see cref='LineItem'> is Changed")]
        internal event ItemTextChanged OnItemTextChanged;

        /// <summary>
        /// Handles <see cref="LineItem"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> value that contains the event data.</param>
        internal delegate void ItemTextChanged(DependencyPropertyChangedEventArgs e);

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "Text")
                RaiseTextChanged(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        private void RaiseTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OnItemTextChanged != null)
                OnItemTextChanged(e);
        }

        #endregion Delegates and Events

        #region local variables

        /// <summary>
        /// instance for TextWidth property.
        /// </summary>
        /// <value>
        /// Type: System.Double
        /// </value>
        private double textwidth;

        /// <summary>
        /// instance for SelectionStartIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int selectionstartindex;

        /// <summary>
        /// instance for SelectionEndIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int selectionendindex;

        /// <summary>
        /// instance for LineStartBlock property.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.BlockCodes
        /// </value>
        private BlockCodes linestartblock;

        /// <summary>
        /// instance for SetCursoronLoad property.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        private bool setcursoronload;

        /// <summary>
        /// instance for SetCursorIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int setcursorindex;

        /// <summary>
        /// instance for WordDetails property.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.WordDetails
        /// </value>
        internal IList<WordDetails> wordlist;

        /// <summary>
        /// Object of VisualCollection.
        /// </summary>
        private VisualCollection collection;

        /// <summary>
        /// Object of EditControl.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditControl
        /// </value>
        private EditControl parentcontrol;

        #endregion local variables

        #region properties

        /// <summary>
        /// Gets or sets the Text in a line
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets the line number of the text
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int LineNumber
        {
            get
            {
                return GetLineNumber();
            }
        }

        /// <summary>
        /// Gets or sets width of the text in the line
        /// </summary>
        /// <value>
        /// Type: System.Double
        /// </value>
        public double TextWidth
        {
            get
            {
                return textwidth;
            }

            set
            {
                textwidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the line is selected.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Start index of the text selection in the line
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int SelectionStartIndex
        {
            set
            {
                selectionstartindex = value;
            }

            get
            {
                return selectionstartindex;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the line is expanded or not
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ContainsLines contain child item.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool ContainsLines
        {
            get
            {
                return (bool)GetValue(ContainsLinesProperty);
            }

            set
            {
                SetValue(ContainsLinesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the StartLine
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int StartLine
        {
            get { return (int)GetValue(StartLineProperty); }
            set { SetValue(StartLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the EndLine
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int EndLine
        {
            get { return (int)GetValue(EndLineProperty); }
            set { SetValue(EndLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the End index of the text selection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int SelectionEndIndex
        {
            set
            {
                selectionendindex = value;
            }

            get
            {
                return selectionendindex;
            }
        }

        /// <summary>
        /// Gets parent editcontrol
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditControl
        /// </value>
        public EditControl ParentControl
        {
            get
            {
                if (parentcontrol == null)
                {
                    parentcontrol = GetEditControl();
                }

                return parentcontrol;
            }
        }

        /// <summary>
        /// Gets or sets the ParentLineNumber
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int ParentLineNumber
        {
            get { return (int)GetValue(ParentLineNumberProperty); }
            set { SetValue(ParentLineNumberProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LineStartBlock
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.BlockCodes
        /// </value>
        internal BlockCodes LineStartBlock
        {
            get
            {
                return linestartblock;
            }

            set
            {
                linestartblock = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SetCursorOnLoad true or false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool SetCursorOnLoad
        {
            get
            {
                return setcursoronload;
            }

            set
            {
                setcursoronload = value;
            }
        }

        /// <summary>
        /// Gets or sets the SetCursorIndex
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int SetCursorIndex
        {
            get
            {
                return setcursorindex;
            }

            set
            {
                setcursorindex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsChildrenExpanded is true or false
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        internal bool IsChildrenExpanded
        {
            get
            {
                return (bool)GetValue(IsChildrenExpandedProperty);
            }

            set
            {
                SetValue(IsChildrenExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets the WordsCollection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public IList<WordDetails> WordsCollection
        {
            get
            {
                return wordlist;
            }

            internal set
            {
                wordlist = value;
            }
        }

        internal IFormat LineStartFormat
        {
            get;
            set;
        }

        internal bool ContainsPreprocessor
        {
            get;
            set;
        }

        internal string PreprocessorText
        {
            get;
            set;
        }

        internal Point EllipsisPosition
        {
            get;
            set;
        }

        internal BlockListener LineStartBlockListener
        {
            get;
            set;
        }

        internal Stack<BlockListener> ParentListeners
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsEndLine
        {
            get { return (bool)GetValue(IsEndLineProperty); }
            set { SetValue(IsEndLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectionPath
        /// </summary>
        public RectangleGeometry SelectionPath
        {
            get
            {
                return (RectangleGeometry)GetValue(SelectionPathProperty);
            }

            set
            {
                SetValue(SelectionPathProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public LineModificationState LineState
        {
            get { return (LineModificationState)GetValue(LineStateProperty); }
            set { SetValue(LineStateProperty, value); }
        }

        #endregion properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        public LineItem()
        {
            InitializeValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        static LineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineItem), new FrameworkPropertyMetadata(typeof(LineItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        public LineItem(string text)
        {
            Text = text;
            InitializeValues();
        }

        /// <summary>
        /// Remove CursorLayer object from LineItem and set SetCursorOnLoad and SetCursorIndex
        /// </summary>
        /// <param name="sender">represents the LineItem object</param>
        /// <param name="e">represents RoutedEventArgs</param>
        private void LineItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.ParentControl != null && this.ParentControl.LineNumber == this.LineNumber)
            {
                this.SetCursorOnLoad = true;
                this.SetCursorIndex = this.parentcontrol.ScrollControl.CaretIndex;
            }
        }

        /// <summary>
        /// Applying a DrawingVisual to collection.
        /// </summary>
        /// <param name="sender">Gets the sender object from the reporting source</param>
        /// <param name="e">Gets the RoutedEventArgs object from the reporting source</param>
        private void LineItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ParentControl != null)
            {
                this.TextWidth = Utils.GetWidth(this.Text, ParentControl.FontFamily, parentcontrol.FontSize, parentcontrol.Foreground);
                this.ParentControl.SetValue(EditControl.PreferredWidthProperty, Math.Max(this.TextWidth + 10, this.ParentControl.PreferredWidth));
                this.ParentControl.LineHeight = Math.Round(Math.Max(this.ParentControl.LineHeight, this.CalculateItemHeight(this.Text)));
                if (this.collection == null)
                {
                    this.collection = new VisualCollection(this);
                    this.collection.Add(new DrawingVisual());
                }

                this.Refresh();
                if (ParentControl.scrollControl != null)
                {
                    if (this.IsSelected && this.ParentControl.ScrollControl.TextSelectionPointer != null)
                    {
                        this.ParentControl.ScrollControl.UpdateSelection(this.ParentControl.ScrollControl.TextSelectionPointer, this.LineNumber - 1);
                    }

                    if (this.SetCursorOnLoad)
                    {
                        this.ParentControl.ScrollControl.MoveCursorToLineItem(this.LineNumber - 1);
                        if (ParentControl.ScrollControl.Caret != null)
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(this.SetCursorIndex);
                    }
                }
            }
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Helper method to get the LineNumber of LineItem from Lines collection EditControl
        /// </summary>
        /// <returns></returns>
        private int GetLineNumber()
        {
            if (ParentControl != null)
            {
                return ParentControl.Lines.IndexOf(this) + 1;
            }

            return -1;
        }

        /// <summary>
        /// Helper method to get reference of parent EditControl in VisualTree
        /// </summary>
        /// <returns></returns>
        private EditControl GetEditControl()
        {
            return VisualUtils.FindAncestor(this, typeof(EditControl)) as EditControl;
        }

        /// <summary>
        /// Helper method to initialize events for LineItem object
        /// </summary>
        private void InitializeValues()
        {
            this.collection = new VisualCollection(this);
            this.Loaded += new RoutedEventHandler(LineItem_Loaded);
            this.Unloaded += new RoutedEventHandler(LineItem_Unloaded);
        }

        /// <summary>
        /// Helper method used to render the text in the line using a DrawingVisual
        /// </summary>
        /// <remarks>
        /// DrawTest method draws the text based on the lineheight, preferred width. The
        /// text should be formatted with fontfamily, fontsize, foreground properties.
        /// </remarks>
        /// <param name="drawingVisual"></param>
        private void DrawText(DrawingVisual drawingVisual)
        {
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                if (this.IsSelected)
                {
                    if (SelectionPath != null)
                    {
                        //Brush brush = (Brush)new BrushConverter().ConvertFromString("#77A7A7A7");
                        if (this.ParentControl.IsSelectionForegroundEnabled)
                        {
                            if (this.ParentControl.CurrentLanguage != null)
                            {
                                this.ParentControl.CurrentLanguage.OnLineItemRender(this, context);
                            }
                            RectangleGeometry selPath = SelectionPath;
                            string collapsedText = this.ParentControl.CurrentLanguage.GetCollapsedItemText(this);
                            int selStartIndex = Math.Min(this.SelectionStartIndex, collapsedText.Length);
                            int selLength = Math.Max(this.SelectionEndIndex - this.SelectionStartIndex, 0);
                            BlockListener block = this.GetPreprocessorType();
                            if (block != null && block.IsPreprocessor && !this.IsExpanded)
                            {
                                double textWidth = Utils.GetWidth(collapsedText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);
                                if (selPath.Rect.Right > textWidth)
                                {
                                    double selWidth = (selPath.Rect.Width - (selPath.Rect.Right - textWidth));
                                    selWidth = selWidth > 0 ? selWidth : 0;
                                    Rect selPathRec = new Rect(selPath.Rect.X, selPath.Rect.Y, selWidth, selPath.Rect.Height);
                                    selPath = new RectangleGeometry(selPathRec);
                                }
                                if (selLength > collapsedText.Length - selStartIndex)
                                {
                                    selLength = collapsedText.Length - selStartIndex;
                                }
                            }
                            if (selPath.Rect.Width > 0)
                            {
                                context.DrawGeometry(this.ParentControl.Background, new Pen(this.ParentControl.Background, 0d), selPath);
                                context.DrawGeometry(this.ParentControl.SelectionBackground, new Pen(this.ParentControl.SelectionBackground, 0d), selPath);
                            }

                            try
                            {
                                if (this.IsExpanded)
                                {
                                    context.DrawText(Utils.GetFormattedText(this.Text.Substring(Math.Min(this.SelectionStartIndex, this.Text.Length), Math.Max(this.SelectionEndIndex - this.SelectionStartIndex, 0)), this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.SelectionForeground), SelectionPath.Rect.TopLeft);
                                }
                                else
                                {
                                    string printText = collapsedText.Substring(selStartIndex, selLength);
                                    int ellipsisIndex = printText.IndexOf(this.ParentControl.CurrentLanguage.EllipsisText);
                                    string ellipsisText = this.ParentControl.CurrentLanguage.EllipsisText;
                                    if (this.ContainsPreprocessor)
                                    {
                                        context.DrawText(Utils.GetFormattedText(printText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.SelectionForeground), SelectionPath.Rect.TopLeft);
                                        ellipsisIndex = printText.IndexOf(this.PreprocessorText);
                                        ellipsisText = this.PreprocessorText;
                                    }
                                    else
                                    {
                                        context.DrawText(Utils.GetFormattedText(printText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.SelectionForeground), SelectionPath.Rect.TopLeft);
                                    }

                                    if (ellipsisIndex >= 0)
                                    {
                                        FormattedText ellipsis = Utils.GetFormattedText(ellipsisText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.SelectionForeground);
                                        context.DrawRoundedRectangle(Brushes.Transparent, new Pen(Brushes.Black, 0.5d), new Rect(this.EllipsisPosition, new Size(ellipsis.WidthIncludingTrailingWhitespace, this.ParentControl.LineHeight)), 1, 1);
                                    }
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            context.DrawGeometry(this.ParentControl.SelectionBackground, new Pen(this.ParentControl.SelectionBackground, 0d), SelectionPath);
                            if (this.ParentControl.CurrentLanguage != null)
                            {
                                this.ParentControl.CurrentLanguage.OnLineItemRender(this, context);
                            }
                        }
                    }
                }
                else
                {
                    if (this.ParentControl.CurrentLanguage != null)
                    {
                        this.ParentControl.CurrentLanguage.OnLineItemRender(this, context);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to get the Preprocessor or Block that it represents.
        /// </summary>
        /// <returns>a BlockListener object indicating the block details</returns>
        internal BlockListener GetPreprocessorType()
        {
            if (parentcontrol == null)
                return null;

            var block = this.ParentControl.CurrentLanguage.DocumentBlocks.Where(blockItem => blockItem.ParentLineNumber == this.ParentControl.Lines.IndexOf(this) + 1);

            if (block.Count() > 0)
            {
                return block.ElementAt(0);
            }
            return null;
        }

        /// <summary>
        /// Helper method to Refresh the LineItem's properties
        /// </summary>
        internal void Refresh()
        {
            if (ParentControl == null)
            {
                return;
            }

            if (this.collection.Count > 0)
            {
                this.collection.Clear();
            }

            this.ParentControl.LineHeight = Math.Max(this.ParentControl.LineHeight, !double.IsNaN(this.Height) ? this.Height : 0);
            DrawingVisual visual = new DrawingVisual();
            this.collection.Add(visual);
            if (this.WordsCollection == null)
            {
                this.ParentControl.CurrentLanguage.ResetLine(this);
            }

            this.DrawText(this.collection[0] as DrawingVisual);
        }

        /// <summary>
        /// Helper method to calculate the width of the lineitem
        /// </summary>
        /// <param name="text">Gets the text from the reporting source</param>
        /// <returns>Returns the width of the text</returns>
        /// <remarks>CalculateItemWidth is used to calculate the item width based on the specified text.</remarks>
        internal double CalculateItemWidth(string text)
        {
            if (ParentControl != null)
            {
                return Utils.GetWidth(text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
            }
            else
            {
                return Utils.GetWidth(text, SystemFonts.MessageFontFamily, SystemFonts.MessageFontSize, Brushes.Black);
            }
        }

        /// <summary>
        /// Helper method to calculate the height of the lineitem
        /// </summary>
        /// <param name="text">Gets the text from the reporting source</param>
        /// <returns>Return the formatted text height</returns>
        /// <remarks>CalculateItemHeight is used to calculate the item height based on the specified text.</remarks>
        internal double CalculateItemHeight(string text)
        {
            if (text == string.Empty)
            {
                text = @"ABCDEFGHIJKLIMOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,/;{}()1234567890_+|\~!@#$%^&*=[]:";
            }

            double tempheight = 0;
            if (ParentControl != null)
            {
                tempheight = Utils.GetFormattedText(text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground).Height;
            }
            else
            {
                tempheight = Utils.GetFormattedText(text, SystemFonts.MessageFontFamily, SystemFonts.MessageFontSize, Brushes.Black).Height;
            }

            if (double.IsNaN(tempheight))
            {
                return 0;
            }

            return tempheight;
        }

        /// <summary>
        /// Helper method to update the cursor position and selection when a line item object is collapsed or expanded.
        /// </summary>
        internal void UpdateCursorPosition()
        {
            if (this.IsExpanded)
            {
                if (!this.IsSelected)
                {
                    if (this.parentcontrol.LineNumber == this.LineNumber)
                    {
                        BlockListener block = this.GetPreprocessorType();
                        var prefixindex = this.ContainsPreprocessor ? this.Text.IndexOf(block.BlockStart) : this.Text.Length;

                        if (this.ParentControl.CursorIndex <= prefixindex && this.ParentControl.CurrentLanguage.GetCollapsedItemSelectionEndIndex(this, prefixindex) != this.ParentControl.CursorIndex)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(this.ParentControl.CursorIndex);
                        }
                        else
                        {
                            this.ParentControl.ScrollControl.ScrollRows.ScrollInView(this.EndLine);
                            this.ParentControl.ScrollControl.MoveCursorToLineItem(this.EndLine - 1);
                            if (this.ParentControl.ScrollControl.Caret != null)
                            {
                                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToEnd();
                            }
                            else
                            {
                                this.ParentControl.ScrollControl.LineNumber = this.EndLine;
                                this.ParentControl.Lines[this.EndLine - 1].SetCursorOnLoad = true;
                                this.ParentControl.Lines[this.EndLine - 1].SetCursorIndex = this.ParentControl.Lines[this.EndLine - 1].Text.Length;
                            }
                        }
                    }
                }
                else
                {
                    this.ParentControl.ScrollControl.RemoveSelectionLayer(this.LineNumber - 1);
                    this.ParentControl.ScrollControl.AddSelectionLayer(this.LineNumber - 1, this.SelectionStartIndex, this.SelectionEndIndex);
                    this.ParentControl.ScrollControl.ScrollRows.ScrollInView(this.ParentControl.ScrollControl.TextSelectionPointer.EndLine);
                    this.ParentControl.ScrollControl.MoveCursorToLineItem(this.ParentControl.ScrollControl.TextSelectionPointer.EndLine);
                    if (this.ParentControl.ScrollControl.Caret != null)
                    {
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(this.ParentControl.ScrollControl.TextSelectionPointer.EndIndex);
                    }
                    else
                    {
                        this.ParentControl.Lines[this.EndLine - 1].SetCursorOnLoad = true;
                        this.ParentControl.Lines[this.EndLine - 1].SetCursorIndex = this.ParentControl.ScrollControl.TextSelectionPointer.EndIndex;
                    }
                }
            }
            else
            {
                string collapseText = this.ParentControl.CurrentLanguage.GetCollapsedItemText(this);
                string ellipsis = this.ContainsPreprocessor ? this.PreprocessorText : this.ParentControl.CurrentLanguage.EllipsisText;
                var ellipsisEndIndex = collapseText.IndexOf(ellipsis) + ellipsis.Length;
                if (!this.IsSelected)
                {
                    if (this.ParentControl.LineNumber == this.LineNumber && this.parentcontrol.CursorIndex <= collapseText.IndexOf(ellipsis))
                    {
                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.Caret.MoveTo(0);
                        }
                        else
                        {
                            this.ParentControl.Lines[this.EndLine - 1].SetCursorOnLoad = true;
                            this.ParentControl.Lines[this.EndLine - 1].SetCursorIndex = this.ParentControl.CursorIndex;
                        }
                    }
                    else if (this.ParentControl.LineNumber <= this.EndLine + 1 && this.ParentControl.LineNumber >= this.LineNumber)
                    {
                        this.ParentControl.ScrollControl.MoveCursorToLineItem(this.LineNumber - 1);
                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(ellipsisEndIndex);
                        }
                        else
                        {
                            this.ParentControl.Lines[this.EndLine - 1].SetCursorOnLoad = true;
                            this.ParentControl.Lines[this.EndLine - 1].SetCursorIndex = 0;
                        }
                    }
                }
                else
                {
                    if (this.ParentControl.ScrollControl.TextSelectionPointer != null && this.ParentControl.ScrollControl.TextSelectionPointer.StartLine <= this.StartLine - 1 && this.ParentControl.ScrollControl.TextSelectionPointer.EndLine >= this.EndLine - 1)
                    {
                        if (this.ParentControl.ScrollControl.isEllipsisSelected)
                        {
                            this.ParentControl.ScrollControl.AddSelectionLayer(this.LineNumber - 1, collapseText.IndexOf(ellipsis), ellipsisEndIndex);
                            this.ParentControl.ScrollControl.isEllipsisSelected = true;
                            ellipsisEndIndex = SelectionEndIndex;
                        }
                        else if ((this.ParentControl.ScrollControl.TextSelectionPointer.EndLine > this.EndLine - 1) || (this.ParentControl.ScrollControl.TextSelectionPointer.EndLine == this.EndLine - 1 && this.ParentControl.ScrollControl.TextSelectionPointer.EndIndex >= this.ParentControl.CurrentLanguage.GetSelectionEndIndex(this)))
                        {
                            this.ParentControl.ScrollControl.AddSelectionLayer(this.LineNumber - 1, SelectionStartIndex, collapseText.Length);
                        }
                        else
                        {
                            // ellipsisEndIndex = SelectionEndIndex;
                            this.ParentControl.ScrollControl.AddSelectionLayer(this.LineNumber - 1, SelectionStartIndex, SelectionEndIndex);// collapseText.IndexOf(ellipsis), ellipsis.Length + collapseText.IndexOf(ellipsis));
                        }

                        if (this.parentcontrol.LineNumber == this.LineNumber)
                        {
                            this.ParentControl.ScrollControl.MoveCursorToLineItem(this.LineNumber - 1);
                            if (this.ParentControl.ScrollControl.Caret != null)
                            {
                                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(ellipsisEndIndex);
                            }
                            else
                            {
                                this.ParentControl.Lines[this.EndLine - 1].SetCursorOnLoad = true;
                                this.ParentControl.Lines[this.EndLine - 1].SetCursorIndex = ellipsisEndIndex;
                            }
                        }
                        //this.ParentControl.ScrollControl.ShowToolTipWhenOverEllipsis(this, this.EllipsisPosition);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public LineItemExpandInformation GetLineItemExpandDetails()
        {
            return new LineItemExpandInformation()
            {
                Text = this.Text,
                StartLine = this.StartLine,
                EndLine = this.EndLine,
                ContainsLines = this.ContainsLines,
                IsExpanded = this.IsExpanded,
                IsSelected = this.IsSelected,
                LineStartBlock = this.LineStartBlock,
                LineStartFormat = this.LineStartFormat,
                LineStartBlockListener = this.LineStartBlockListener,
                ParentLineNumber = this.ParentLineNumber
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void CopyExpandDetails(LineItem item)
        {
            StartLine = item.StartLine;
            EndLine = item.EndLine;
            ContainsLines = item.ContainsLines;
            IsExpanded = item.IsExpanded;
            IsSelected = item.IsSelected;
            LineStartBlock = item.LineStartBlock;
            LineStartFormat = item.LineStartFormat;
            LineStartBlockListener = item.LineStartBlockListener;
            ParentLineNumber = item.ParentLineNumber;
            ContainsPreprocessor = item.ContainsPreprocessor;
            PreprocessorText = item.PreprocessorText;
            IsEndLine = item.IsEndLine;
        }

        #endregion Implementation

        #region Events

        /// <summary>
        /// Gets called when SelectionPath property of the SelectionLayer gets changed. Also it triggers
        /// the SelectionPathChanged Event of the SelectionLayer.
        /// </summary>
        /// <param name="d">DepdendencyObject, returns SelectionLayer</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, returns old and new
        /// value</param>
        private static void OnSelectionPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineItem item = d as LineItem;
            if (item.ParentControl != null)
            {
                item.collection.Clear();
                DrawingVisual visual = new DrawingVisual();
                item.collection.Add(visual);
                item.DrawText(visual);
            }
        }

        /// <summary>
        /// Called when ContainsLinesChanged.
        /// </summary>
        /// <param name="d">DepdendencyObject from the requested source</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContainsLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when Text Changed.
        /// </summary>
        /// <param name="d">DepdendencyObject from the requested source</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineItem item = (LineItem)d;
            //item.Width = Math.Min(item.CalculateItemWidth(item.Text), 10);
            item.TextWidth = Math.Min(item.CalculateItemWidth(item.Text), 10);
            item.WordsCollection = null;
            item.IsSelected = false;
            item.SelectionStartIndex = 0;
            item.SelectionEndIndex = 0;
            if (item.ParentControl != null)
            {
                item.ParentControl.CurrentLanguage.CalculatePreferredWidth();
                item.ParentControl.CurrentLanguage.ResetLine(item);
                item.parentcontrol.isReinitializeLines = false;
                item.parentcontrol.Text = item.parentcontrol.GetText();
                item.parentcontrol.isReinitializeLines = true;
                item.LineState = LineModificationState.Modified;
            }
        }

        /// <summary>
        /// Called when ExpandedChanged.
        /// </summary>
        /// <param name="d">DepdendencyObject from the requested source</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineItem item = d as LineItem;
            if (item.ParentControl != null)
            {
                item.collection.Clear();

                if (item.IsExpanded)
                    item.SelectionEndIndex -= item.ParentControl.CurrentLanguage.EllipsisText.Length;

                DrawingVisual visual = new DrawingVisual();
                item.collection.Add(visual);
                item.DrawText(visual);
            }
        }

        #endregion Events

        #region Overrides

        /// <summary>
        /// Gets the number of visual child elements.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        /// <remarks>VisualChildrenCount is the Override method used to get the count of the visual child from the visual collection.</remarks>
        protected override int VisualChildrenCount
        {
            get { return collection.Count; }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        /// <remarks>GetVisualChild is the Override method used to get the visual child from the visual collection based on the index value.</remarks>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return collection[index];
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
        }

        #endregion Overrides
    }
}