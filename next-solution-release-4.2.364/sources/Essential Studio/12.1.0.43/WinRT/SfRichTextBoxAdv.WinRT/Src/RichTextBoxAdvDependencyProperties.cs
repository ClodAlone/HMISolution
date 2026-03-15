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
using System.Windows.Input;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
#if WPF
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public partial class SfRichTextBoxAdv
    {

        #region Event Handlers
        /// <summary>
        /// Event handler for file load failed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="FileLoadingFailedEventArgs"/> instance containing the event data.</param>
        public delegate void FileLoadingFailedEventHandler(object obj, FileLoadingFailedEventArgs args);
        /// <summary>
        /// Event handler for print completed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="PrintCompletedEventArgs"/> instance containing the event data.</param>
        public delegate void PrintCompletedEventHandler(object obj, PrintCompletedEventArgs args);
        /// <summary>
        /// Event handler for content changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="ContentChangedEventArgs"/> instance containing the event data.</param>
        public delegate void ContentChangedEventHandler(object obj, ContentChangedEventArgs args);
        /// <summary>
        /// Event handler for request URL navigation changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
        public delegate void RequestNavigateEventHandler(object obj, RequestNavigateEventArgs args);

        public delegate void SelectionChangedEventHandler(object obj, SelectionChangedEventArgs args);
#if DEBUG
        public delegate void PerformanceInfoChangedEventHandler(object obj);
#endif
        #endregion



        internal string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the fill color
        /// </summary>
        internal Brush SelectionBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBrushProperty);
            }
            set
            {
                SetValue(SelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the FillColorProperty dependency property
        /// </summary>
        internal static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(SfRichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)), OnSelectionFillColorChanged));

        /// <summary>
        /// Gets or Sets the Selection Opacity.
        /// </summary>
        internal double SelectionOpacity
        {
            get
            {
                return (double)GetValue(SelectionOpacityProperty);
            }
            set
            {
                SetValue(SelectionOpacityProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the SelectionOpacity dependency property
        /// </summary>
        internal static readonly DependencyProperty SelectionOpacityProperty = DependencyProperty.Register("SelectionOpacity", typeof(double), typeof(SfRichTextBoxAdv), new PropertyMetadata(0.5d, OnSelectionOpacityChanged));

        private static void OnSelectionOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        internal bool DefferedScrolling
        {
            get
            {
                return (bool)GetValue(DefferedScrollingProperty);
            }
            set
            {
                SetValue(DefferedScrollingProperty, value);
            }
        }

        internal static readonly DependencyProperty DefferedScrollingProperty = DependencyProperty.Register("DefferedScrolling", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(false));

#if !WPF
        #region Character Format commands
        /// <summary>
        /// Gets or Sets the Bold Command
        /// </summary>
        public BoldCommand BoldCommand
        {
            get
            {
                return (BoldCommand)GetValue(BoldCommandProperty);
            }
            set
            {
                SetValue(BoldCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the BoldCommandProperty dependency property
        /// </summary>
        public static readonly DependencyProperty BoldCommandProperty = DependencyProperty.Register("BoldCommand", typeof(BoldCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the italic command.
        /// </summary>
        /// <value>The italic command.</value>
        public ItalicCommand ItalicCommand
        {
            get
            {
                return (ItalicCommand)GetValue(ItalicCommandProperty);
            }
            set
            {
                SetValue(ItalicCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ItalicCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItalicCommandProperty =
            DependencyProperty.Register("ItalicCommand", typeof(ItalicCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the change text color command.
        /// </summary>
        /// <value>The change text color command.</value>
        public FontColorCommand FontColorCommand
        {
            get
            {
                return (FontColorCommand)GetValue(FontColorCommandProperty);
            }
            set
            {
                SetValue(FontColorCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for FontColorCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontColorCommandProperty =
            DependencyProperty.Register("FontColorCommand", typeof(FontColorCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the change font family command.
        /// </summary>
        /// <value>The change font family command.</value>
        public FontFamilyCommand FontFamilyCommand
        {
            get { return (FontFamilyCommand)GetValue(FontFamilyCommandProperty); }
            set { SetValue(FontFamilyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamilyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyCommandProperty =
            DependencyProperty.Register("FontFamilyCommand", typeof(FontFamilyCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the change font size command.
        /// </summary>
        /// <value>The change font size command.</value>
        public FontSizeCommand FontSizeCommand
        {
            get { return (FontSizeCommand)GetValue(FontSizeCommandProperty); }
            set { SetValue(FontSizeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSizeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeCommandProperty =
            DependencyProperty.Register("FontSizeCommand", typeof(FontSizeCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the highlight color command.
        /// </summary>
        /// <value>The highlight color command.</value>
        public HighlightColorCommand HighlightColorCommand
        {
            get
            {
                return (HighlightColorCommand)GetValue(HighlightColorCommandProperty);
            }
            set
            {
                SetValue(HighlightColorCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightColorCommandProperty =
            DependencyProperty.Register("HighlightColorCommand", typeof(HighlightColorCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the BaselineAlignment command.
        /// </summary>
        /// <value>The BaselineAlignment command.</value>
        public BaselineAlignmentCommand BaselineAlignmentCommand
        {
            get { return (BaselineAlignmentCommand)GetValue(BaselineAlignmentCommandProperty); }
            set { SetValue(BaselineAlignmentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BaselineAlignmentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaselineAlignmentCommandProperty =
            DependencyProperty.Register("BaselineAlignmentCommand", typeof(BaselineAlignmentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the strike through command.
        /// </summary>
        /// <value>The strike through command.</value>
        public StrikeThroughCommand StrikeThroughCommand
        {
            get
            {
                return (StrikeThroughCommand)GetValue(StrikeThroughCommandProperty);
            }
            set
            {
                SetValue(StrikeThroughCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for DoubleStrikeThroughCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrikeThroughCommandProperty =
            DependencyProperty.Register("StrikeThroughCommand", typeof(StrikeThroughCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the underline command.
        /// </summary>
        /// <value>The underline command.</value>
        public UnderlineCommand UnderlineCommand
        {
            get { return (UnderlineCommand)GetValue(UnderlineCommandProperty); }
            set { SetValue(UnderlineCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnderlineCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderlineCommandProperty =
            DependencyProperty.Register("UnderlineCommand", typeof(UnderlineCommand), typeof(SfRichTextBoxAdv), null);
        #endregion

        #region Paragraph Format Commands
        /// <summary>
        /// Gets or sets the text alignment command.
        /// </summary>
        /// <value>
        /// The text alignment command.
        /// </value>
        public TextAlignmentCommand TextAlignmentCommand
        {
            get
            {
                return (TextAlignmentCommand)GetValue(TextAlignmentCommandProperty);
            }
            set
            {
                SetValue(TextAlignmentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for TextAlignementCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentCommandProperty =
            DependencyProperty.Register("TextAlignmentCommand", typeof(TextAlignmentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the after spacing command.
        /// </summary>
        /// <value>
        /// The after spacing command.
        /// </value>
        public AfterSpacingCommand AfterSpacingCommand
        {
            get
            {
                return (AfterSpacingCommand)GetValue(AfterSpacingCommandProperty);
            }
            set
            {
                SetValue(AfterSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for AfterSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AfterSpacingCommandProperty =
            DependencyProperty.Register("AfterSpacingCommand", typeof(AfterSpacingCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the before spacing command.
        /// </summary>
        /// <value>
        /// The before spacing command.
        /// </value>
        public BeforeSpacingCommand BeforeSpacingCommand
        {
            get
            {
                return (BeforeSpacingCommand)GetValue(BeforeSpacingCommandProperty);
            }
            set
            {
                SetValue(BeforeSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for BeforeSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeforeSpacingCommandProperty =
            DependencyProperty.Register("BeforeSpacingCommand", typeof(BeforeSpacingCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the line spacing type command.
        /// </summary>
        /// <value>
        /// The line spacing type command.
        /// </value>
        public LineSpacingTypeCommand LineSpacingTypeCommand
        {
            get
            {
                return (LineSpacingTypeCommand)GetValue(LineSpacingTypeCommandProperty);
            }
            set
            {
                SetValue(LineSpacingTypeCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for LineSpacingTypeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSpacingTypeCommandProperty =
            DependencyProperty.Register("LineSpacingTypeCommand", typeof(LineSpacingCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the line spacing command.
        /// </summary>
        /// <value>
        /// The line spacing command.
        /// </value>
        public LineSpacingCommand LineSpacingCommand
        {
            get
            {
                return (LineSpacingCommand)GetValue(LineSpacingCommandProperty);
            }
            set
            {
                SetValue(LineSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for LineSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSpacingCommandProperty =
            DependencyProperty.Register("LineSpacingCommand", typeof(LineSpacingCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the left indent command.
        /// </summary>
        /// <value>
        /// The left indent command.
        /// </value>
        public LeftIndentCommand LeftIndentCommand
        {
            get
            {
                return (LeftIndentCommand)GetValue(LeftIndentCommandProperty);
            }
            set
            {
                SetValue(LeftIndentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for LeftIndentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIndentCommandProperty =
            DependencyProperty.Register("LeftIndentCommand", typeof(LeftIndentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the right indent command.
        /// </summary>
        /// <value>
        /// The right indent command.
        /// </value>
        public RightIndentCommand RightIndentCommand
        {
            get
            {
                return (RightIndentCommand)GetValue(RightIndentCommandProperty);
            }
            set
            {
                SetValue(RightIndentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RightIndentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightIndentCommandProperty =
            DependencyProperty.Register("RightIndentCommand", typeof(RightIndentCommand), typeof(SfRichTextBoxAdv), null);
        #endregion

        #region Clipboard Commands
        /// <summary>
        /// Gets or sets the copy command
        /// </summary>
        /// <value>The copy command.</value>
        public CopyCommand CopyCommand
        {
            get
            {
                return (CopyCommand)GetValue(CopyCommandProperty);
            }
            set
            {
                SetValue(CopyCommandProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for CopyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(CopyCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the cut command.
        /// </summary>
        /// <value>The cut command.</value>
        public CutCommand CutCommand
        {
            get
            {
                return (CutCommand)GetValue(CutCommandProperty);
            }
            set
            {
                SetValue(CutCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CutCommandProperty =
            DependencyProperty.Register("CutCommand", typeof(CutCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the paste command.
        /// </summary>
        /// <value>The paste command.</value>
        public PasteCommand PasteCommand
        {
            get
            {
                return (PasteCommand)GetValue(PasteCommandProperty);
            }
            set
            {
                SetValue(PasteCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PasteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(PasteCommand), typeof(SfRichTextBoxAdv), null);
        #endregion

        #region Undo Redo Commands
        /// <summary>
        /// Gets or sets the undo command.
        /// </summary>
        /// <value>The single strikethrough command.</value>
        public UndoCommand UndoCommand
        {
            get { return (UndoCommand)GetValue(UndoCommandProperty); }
            set { SetValue(UndoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Single.
        public static readonly DependencyProperty UndoCommandProperty =
            DependencyProperty.Register("UndoCommand", typeof(UndoCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the redo command.
        /// </summary>
        /// <value>The single strikethrough command.</value>
        public RedoCommand RedoCommand
        {
            get { return (RedoCommand)GetValue(RedoCommandProperty); }
            set { SetValue(RedoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Single.
        public static readonly DependencyProperty RedoCommandProperty =
            DependencyProperty.Register("RedoCommand", typeof(RedoCommand), typeof(SfRichTextBoxAdv), null);
        #endregion

        #region Document Commands
        /// <summary>
        /// Gets or sets the open document command.
        /// </summary>
        /// <value>The open document command.</value>
        public OpenDocumentCommand OpenDocumentCommand
        {
            get { return (OpenDocumentCommand)GetValue(OpenDocumentCommandProperty); }
            set { SetValue(OpenDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenDocumentCommandProperty =
            DependencyProperty.Register("OpenDocumentCommand", typeof(OpenDocumentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the print document command.
        /// <para>Ensure no other handler for the PrintTaskRequested event is registered before invoking this method, since SfRichTextBoxAdv internally registers it.</para>
        /// </summary>
        /// <value>
        /// The print document command.
        /// </value>
        public PrintDocumentCommand PrintDocumentCommand
        {
            get { return (PrintDocumentCommand)GetValue(PrintDocumentCommandProperty); }
            set { SetValue(PrintDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrintDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintDocumentCommandProperty =
            DependencyProperty.Register("PrintDocumentCommand", typeof(PrintDocumentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the new document command.
        /// </summary>
        /// <value>
        /// The new document command.
        /// </value>
        public NewDocumentCommand NewDocumentCommand
        {
            get { return (NewDocumentCommand)GetValue(NewDocumentCommandProperty); }
            set { SetValue(NewDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewDocumentCommandProperty =
            DependencyProperty.Register("NewDocumentCommand", typeof(NewDocumentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the save document command.
        /// </summary>
        /// <value>The save document command.</value>
        public SaveDocumentCommand SaveDocumentCommand
        {
            get { return (SaveDocumentCommand)GetValue(SaveDocumentCommandProperty); }
            set { SetValue(SaveDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveDocumentCommandProperty =
            DependencyProperty.Register("SaveDocumentCommand", typeof(SaveDocumentCommand), typeof(SfRichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the save as document command.
        /// </summary>
        /// <value>
        /// The save as document command.
        /// </value>
        public SaveAsDocumentCommand SaveAsDocumentCommand
        {
            get { return (SaveAsDocumentCommand)GetValue(SaveAsDocumentCommandProperty); }
            set { SetValue(SaveAsDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveAsDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveAsDocumentCommandProperty =
            DependencyProperty.Register("SaveAsDocumentCommand", typeof(SaveAsDocumentCommand), typeof(SfRichTextBoxAdv), new PropertyMetadata(null));
        #endregion

        #region Insert Commands
        /// <summary>
        /// Gets or sets the insert picture command.
        /// </summary>
        /// <value>
        /// The insert picture command.
        /// </value>
        public InsertPictureCommand InsertPictureCommand
        {
            get { return (InsertPictureCommand)GetValue(InsertPictureCommandProperty); }
            set { SetValue(InsertPictureCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertPictureCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertPictureCommandProperty =
            DependencyProperty.Register("InsertPictureCommand", typeof(InsertPictureCommand), typeof(SfRichTextBoxAdv), null);

        public InsertTableCommand InsertTableCommand
        {
            get { return (InsertTableCommand)GetValue(InsertTableCommandProperty); }
            set { SetValue(InsertTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertTableCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertTableCommandProperty =
            DependencyProperty.Register("InsertTableCommand", typeof(InsertTableCommand), typeof(SfRichTextBoxAdv), null);
        
        /// <summary>
        /// Gets or sets the insert hyperlink command.
        /// </summary>
        /// <value>
        /// The insert hyperlink command.
        /// </value>
        internal InsertHyperlinkCommand InsertHyperlinkCommand
        {
            get { return (InsertHyperlinkCommand)GetValue(InsertHyperlinkCommandProperty); }
            set { SetValue(InsertHyperlinkCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HyperlinkDialogCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InsertHyperlinkCommandProperty =
            DependencyProperty.Register("InsertHyperlinkCommand", typeof(InsertHyperlinkCommand), typeof(SfRichTextBoxAdv), null);
        #endregion

        #region LayoutType Command
        public LayoutTypeCommand LayoutTypeCommand
        {
            get { return (LayoutTypeCommand)GetValue(LayoutTypeCommandProperty); }
            set { SetValue(LayoutTypeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangePageLayoutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutTypeCommandProperty =
            DependencyProperty.Register("LayoutTypeCommand", typeof(LayoutTypeCommand), typeof(SfRichTextBoxAdv), new PropertyMetadata(null));
        #endregion
#endif

        /// <summary>
        /// Gets or sets the document title.
        /// </summary>
        /// <value>
        /// The document title.
        /// </value>
        public string DocumentTitle
        {
            get { return (string)GetValue(DocumentTitleProperty); }
            set { SetValue(DocumentTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DocumentTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentTitleProperty =
            DependencyProperty.Register("DocumentTitle", typeof(string), typeof(SfRichTextBoxAdv), new PropertyMetadata(string.Empty));

        internal bool EnableCursorOnReadOnly
        {
            get { return (bool)GetValue(EnableCursorOnReadOnlyProperty); }
            set { SetValue(EnableCursorOnReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableCursorOnReadOnly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EnableCursorOnReadOnlyProperty =
            DependencyProperty.Register("EnableCursorOnReadOnly", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(false,new PropertyChangedCallback(OnEnableCursorOnReadOnlyChanged)));
        
        internal string XAMLText
        {
            get
            {
                //if (this.Document != null)
                //    return XAMLExporting.ConvertToXAML(this.Document);
                return null;
            }
            set
            {
                //SetValue(XAMLTextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for XAMLText.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty XAMLTextProperty =
            DependencyProperty.Register("XAMLText", typeof(string), typeof(SfRichTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnXAMLTextChanged)));

        /// <summary>
        /// Gets or Sets the layout type.
        /// </summary>
        public LayoutType LayoutType
        {
            get
            {
                return (LayoutType)GetValue(LayoutTypeProperty);
            }
            set
            {
                SetValue(LayoutTypeProperty, value);
            }
        }

        /// <summary>
        /// Registers the PageLayout Dependency property
        /// </summary>
        public static readonly DependencyProperty LayoutTypeProperty = DependencyProperty.Register("LayoutType", typeof(LayoutType), typeof(SfRichTextBoxAdv), new PropertyMetadata(LayoutType.Pages, OnLayoutTypeChanged));

        /// <summary>
        /// Gets or sets the font names.
        /// </summary>
        /// <value>
        /// The font names.
        /// </value>
        public ObservableCollection<string> FontNames
        {
            get { return (ObservableCollection<string>)GetValue(FontNamesProperty); }
            set { SetValue(FontNamesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fonts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontNamesProperty =
            DependencyProperty.Register("FontNames", typeof(ObservableCollection<string>), typeof(SfRichTextBoxAdv), new PropertyMetadata(null));

        internal int CurrentPageNumber
        {
            get
            {
                return (int)GetValue(CurrentPageNumberProperty);
            }
            set
            {
                SetValue(CurrentPageNumberProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CurrentPageNumber.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentPageNumberProperty =
            DependencyProperty.Register("CurrentPageNumber", typeof(int), typeof(LayoutViewer), new PropertyMetadata(null));

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (Viewer == null)
                    return 0;
                return (int)Viewer.Pages.Count;
            }
        }
        /// <summary>
        /// Gets or Sets the zoom factor in percent [Zoom factor should be between 10 to 500].
        /// </summary>
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }
            set
            {
                SetValue(ZoomFactorProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(SfRichTextBoxAdv), new PropertyMetadata(100d, new PropertyChangedCallback(OnZoomFactorChanged)));

#if !WPF

        internal ParagraphDialogCommand ParagraphDialogCommand
        {
            get { return (ParagraphDialogCommand)GetValue(ParagraphDialogCommandProperty); }
            set { SetValue(ParagraphDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParagraphPropertiesDialogCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParagraphDialogCommandProperty =
            DependencyProperty.Register("ParagraphDialogCommand", typeof(ParagraphDialogCommand), typeof(SfRichTextBoxAdv), null);




        internal InsertTableDialogCommand InsertTableDialogCommand
        {
            get { return (InsertTableDialogCommand)GetValue(InsertTableDialogCommandProperty); }
            set { SetValue(InsertTableDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertTableDialogCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InsertTableDialogCommandProperty =
            DependencyProperty.Register("InsertTableDialogCommand", typeof(InsertTableDialogCommand), typeof(SfRichTextBoxAdv), null);



        internal FontDialogCommand FontDialogCommand
        {
            get { return (FontDialogCommand)GetValue(FontDialogCommandProperty); }
            set { SetValue(FontDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontDialogCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FontDialogCommandProperty =
            DependencyProperty.Register("FontDialogCommand", typeof(FontDialogCommand), typeof(SfRichTextBoxAdv), null);


        internal ChangeListTypeCommand ChangeListTypeCommand
        {
            get { return (ChangeListTypeCommand)GetValue(ChangeListTypeCommandProperty); }
            set { SetValue(ChangeListTypeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeListTypeCommmand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ChangeListTypeCommandProperty =
            DependencyProperty.Register("ChangeListTypeCommand", typeof(ChangeListTypeCommand), typeof(SfRichTextBoxAdv), new PropertyMetadata(null));
        
        internal InsertRowCommand InsertRowCommand
        {
            get { return (InsertRowCommand)GetValue(InsertRowCommandProperty); }
            set { SetValue(InsertRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertRowCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InsertRowCommandProperty =
            DependencyProperty.Register("InsertRowCommand", typeof(InsertRowCommand), typeof(SfRichTextBoxAdv), null);




        internal InsertColumnCommand InsertColumnCommand
        {
            get { return (InsertColumnCommand)GetValue(InsertColumnCommandProperty); }
            set { SetValue(InsertColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertColumnCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InsertColumnCommandProperty =
            DependencyProperty.Register("InsertColumnCommand", typeof(InsertColumnCommand), typeof(SfRichTextBoxAdv), null);


        internal DeleteRowCommand DeleteRowCommand
        {
            get { return (DeleteRowCommand)GetValue(DeleteRowCommandProperty); }
            set { SetValue(DeleteRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteRowCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DeleteRowCommandProperty =
            DependencyProperty.Register("DeleteRowCommand", typeof(DeleteRowCommand), typeof(SfRichTextBoxAdv), null);


        internal DeleteColumnCommand DeleteColumnCommand
        {
            get { return (DeleteColumnCommand)GetValue(DeleteColumnCommandProperty); }
            set { SetValue(DeleteColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteColumnCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DeleteColumnCommandProperty =
            DependencyProperty.Register("DeleteColumnCommand", typeof(DeleteColumnCommand), typeof(SfRichTextBoxAdv), null);


        internal DeleteTableCommand DeleteTableCommand
        {
            get { return (DeleteTableCommand)GetValue(DeleteTableCommandProperty); }
            set { SetValue(DeleteTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteTableCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DeleteTableCommandProperty =
            DependencyProperty.Register("DeleteTableCommand", typeof(DeleteTableCommand), typeof(SfRichTextBoxAdv), null);



        internal SelectCellCommand SelectCellCommand
        {
            get { return (SelectCellCommand)GetValue(SelectCellCommandProperty); }
            set { SetValue(SelectCellCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectCellCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectCellCommandProperty =
            DependencyProperty.Register("SelectCellCommand", typeof(SelectCellCommand), typeof(SfRichTextBoxAdv), null);




        internal SelectRowCommand SelectRowCommand
        {
            get { return (SelectRowCommand)GetValue(SelectRowCommandProperty); }
            set { SetValue(SelectRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectRowCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectRowCommandProperty =
            DependencyProperty.Register("SelectRowCommand", typeof(SelectRowCommand), typeof(SfRichTextBoxAdv), null);




        internal SelectColumnCommand SelectColumnCommand
        {
            get { return (SelectColumnCommand)GetValue(SelectColumnCommandProperty); }
            set { SetValue(SelectColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectColumnCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectColumnCommandProperty =
            DependencyProperty.Register("SelectColumnCommand", typeof(SelectColumnCommand), typeof(SfRichTextBoxAdv), null);




        internal SelectTableCommand SelectTableCommand
        {
            get { return (SelectTableCommand)GetValue(SelectTableCommandProperty); }
            set { SetValue(SelectTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectTableCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectTableCommandProperty =
            DependencyProperty.Register("SelectTableCommand", typeof(SelectTableCommand), typeof(SfRichTextBoxAdv), null);

        internal MergeSelectedCellsCommand MergeSelectedCellsCommand
        {
            get { return (MergeSelectedCellsCommand)GetValue(MergeSelectedCellsCommandProperty); }
            set { SetValue(MergeSelectedCellsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MergeSelectedCellsCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MergeSelectedCellsCommandProperty =
            DependencyProperty.Register("MergeSelectedCellsCommand", typeof(MergeSelectedCellsCommand), typeof(SfRichTextBoxAdv), null);

        internal ChangeTableCellStyleCommand ChangeTableCellStyleCommand
        {
            get { return (ChangeTableCellStyleCommand)GetValue(ChangeTableCellStyleCommandProperty); }
            set { SetValue(ChangeTableCellStyleCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeTableCellStyleCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ChangeTableCellStyleCommandProperty =
            DependencyProperty.Register("ChangeTableCellStyleCommand", typeof(ChangeTableCellStyleCommand), typeof(SfRichTextBoxAdv), null);


        internal ChangeTableBorderColorCommand ChangeTableBorderColorCommand
        {
            get { return (ChangeTableBorderColorCommand)GetValue(ChangeTableBorderColorCommandProperty); }
            set { SetValue(ChangeTableBorderColorCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeTableStyleCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ChangeTableBorderColorCommandProperty =
            DependencyProperty.Register("ChangeTableBorderColorCommand", typeof(ChangeTableBorderColorCommand), typeof(SfRichTextBoxAdv), null);
#endif

        internal bool IsContextMenuVisible
        {
            get { return (bool)GetValue(IsContextMenuVisibleProperty); }
            set { SetValue(IsContextMenuVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextMenuVisible.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsContextMenuVisibleProperty =
            DependencyProperty.Register("IsContextMenuVisible", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsContextMenuVisibleChanged)));

        public bool VerticalScrollBarVisibility
        {
            get { return (bool)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnVerticalScrollBarVisibilityChanged)));

        public bool HorizontalScrollBarVisibility
        {
            get { return (bool)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnHorizontalScrollBarVisibilityChanged)));


        #region Event
#if DEBUG
        public event PerformanceInfoChangedEventHandler PerformanceInfoChanged;
#endif


        internal event EventHandler StyleChanged;

        internal event RoutedEventHandler Printing;

        /// <summary>
        /// Occurs when file load failed.
        /// </summary>
        public event FileLoadingFailedEventHandler FileLoadingFailed;
        /// <summary>
        /// Occurs when print completed.
        /// </summary>
        public event PrintCompletedEventHandler PrintCompleted;
        /// <summary>
        /// Occurs when content changed.
        /// </summary>
        public event ContentChangedEventHandler ContentChanged;
        /// <summary>
        /// Occurs when request navigate.
        /// </summary>
        public event RequestNavigateEventHandler RequestNavigate;

        public event SelectionChangedEventHandler SelectionChanged;

        internal event DependencyPropertyChangedEventHandler IsReadOnlyChanged;

        public event DependencyPropertyChangedEventHandler ZoomFactorChanged;
        internal event EventHandler LoadCompleted;
        #endregion

        /// <summary>
        /// Gets or Sets the stroke value
        /// </summary>
        internal Brush SelectionStrokeColor
        {
            get
            {
                return (Brush)GetValue(SelectionStrokeColorProperty);
            }
            set
            {
                SetValue(SelectionStrokeColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the FillColorProperty dependency property
        /// </summary>
        internal static readonly DependencyProperty SelectionStrokeColorProperty = DependencyProperty.Register("SelectionStrokeColor", typeof(Brush), typeof(SfRichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 142, 239)), OnSelectionStrokeColorChanged));



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnSelectionFillColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnSelectionFillColorChanged(args);
        }

        /// <summary>
        /// Gets or Sets the document
        /// </summary>
        public DocumentAdv Document
        {
            get
            {
                return (DocumentAdv)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }

        /// <summary>
        /// Registers the dependency property Document
        /// </summary>
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(DocumentAdv), typeof(SfRichTextBoxAdv), new PropertyMetadata(null, OnDocumentChanged));

        /// <summary>
        /// Gets or Sets a value indicating whether to record Undo/Redo
        /// </summary>
        internal bool DisableHistory
        {
            get
            {
                return (bool)GetValue(DisableHistoryProperty);
            }
            set
            {
                SetValue(DisableHistoryProperty, value);
            }
        }

        /// <summary>
        /// Registers the dependency property DisableHistory
        /// </summary>
        internal static readonly DependencyProperty DisableHistoryProperty = DependencyProperty.Register("DisableHistoryProperty", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(false, OnDisableHistoryChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnDisableHistoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnDisableHistoryChanged(args);
        }

        protected virtual void OnDisableHistoryChanged(DependencyPropertyChangedEventArgs e)
        {
            //if ((bool)e.NewValue)
            //{
            //    History.ClearHistory();
            //}
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can change the content in the RichTextBoxAdv.
        /// </summary>
        /// <value>
        /// <c>true</c> if the RichTextBoxAdv is read-only; otherwise, <c>false</c> The default is false.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged)));

        /// <summary>
        /// Get or Sets a value indicating the control can be zoomed
        /// </summary>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)GetValue(IsZoomEnabledProperty);
            }
            set
            {
                SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// Registers the IsReadOnly dependency property
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(true));
#if !WPF
        /// <summary>
        /// Get or Sets the scrollbar indicator mode
        /// </summary>
        public ScrollingIndicatorMode ScrollBarIndicatorMode
        {
            get
            {
                return (ScrollingIndicatorMode)GetValue(ScrollBarIndicatorModeProperty);
            }
            set
            {
                SetValue(ScrollBarIndicatorModeProperty, value);
            }
        }

        /// <summary>
        /// Registers the scrollbar indicator mode dependency property
        /// </summary>
        public static readonly DependencyProperty ScrollBarIndicatorModeProperty = DependencyProperty.Register("ScrollBarIndicatorMode", typeof(ScrollingIndicatorMode), typeof(SfRichTextBoxAdv), new PropertyMetadata(ScrollingIndicatorMode.TouchIndicator, new PropertyChangedCallback(OnScrollBarIndicatorModeChanged)));

        private static void OnScrollBarIndicatorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfRichTextBoxAdv rte = (SfRichTextBoxAdv)d;
            if (rte.verticalScrollBar != null)
                rte.verticalScrollBar.IndicatorMode = (ScrollingIndicatorMode)e.NewValue;
            if (rte.horizontalScrollBar != null)
                rte.horizontalScrollBar.IndicatorMode = (ScrollingIndicatorMode)e.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Radial menu needs to be enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable radial menu]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRadialMenu
        {
            set
            {
                SetValue(EnableRadialMenuProperty , value);
            }
            get
            {
               return (bool)GetValue(EnableRadialMenuProperty);
            }
        }

        // Using a DependencyProperty as the backing store for EnableRadialMenu.  This enables binding...
        public static readonly DependencyProperty EnableRadialMenuProperty =
            DependencyProperty.Register("RadialMenuVisibility", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableRadialMenuPropertyChanged)));

        private static void OnEnableRadialMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfRichTextBoxAdv rte= d as SfRichTextBoxAdv;
            if ((bool)e.NewValue)
                rte.viewer.Children.Add(rte.radialMenu);
            else
                rte.Viewer.Children.Remove(rte.radialMenu);
        }
#endif
#if WPF && SyncfusionFramework4_0
        /// <summary>
        /// Get or Sets a value that indicates whether manipulation events are enabled on the RichTextBoxAdv.
        /// </summary>
        public new bool IsManipulationEnabled
        {
            get
            {
                return (bool)GetValue(IsManipulationEnabledProperty);
            }
            set
            {
                SetValue(IsManipulationEnabledProperty, value);
                if (this.Viewer != null)
                    this.Viewer.IsManipulationEnabled = value;
            }
        }
#endif

        internal bool IsPositionInsideTable
        {
            get { return (bool)GetValue(IsPositionInsideTableProperty); }
            set { SetValue(IsPositionInsideTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPositionInsideTable.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsPositionInsideTableProperty =
            DependencyProperty.Register("IsPositionInsideTable", typeof(bool), typeof(SfRichTextBoxAdv), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a brush that provides the background of the RichTextBoxAdv.
        /// </summary>
        /// <returns>The brush that provides the background of the RichTextBoxAdv. The default value is a SolidColorBrush with color of Black.</returns>
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Background dependency property.
        /// </summary>
        /// <returns>The identifier for the Background dependency property.</returns>
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(SfRichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBackgroundChanged));

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        /// <returns>The brush that paints the foreground of the RichTextBoxAdv. The default value is a SolidColorBrush with color of White.</returns>
        public new Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Foreground dependency property.
        /// </summary>
        /// <returns>The identifier for the Foreground dependency property.</returns>
        public new static DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(SfRichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Colors.White), OnForegroundChanged));

        /// <summary>
        /// Gets or sets the padding inside the RichTextBoxAdv.
        /// </summary>
        /// <returns>The amount of space between the content of RichTextBoxAdv and its Margin or Border. The default is a Thickness with values of 0 on all four sides.</returns>
        public new Thickness Padding
        {
            get
            {
                return (Thickness)GetValue(PaddingProperty);
            }
            set
            {
                SetValue(PaddingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Padding dependency property.
        /// </summary>
        /// <returns>The identifier for the Padding dependency property.</returns>
        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(SfRichTextBoxAdv), new PropertyMetadata(new Thickness(0), OnPaddingChanged));

        /// <summary>
        /// Called when background changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnBackgroundChanged(args);
        }
        /// <summary>
        /// Raises the <see cref="E:BackgroundChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDocumentLoaded && LayoutType == LayoutType.Block && Viewer != null)
                Viewer.RenderVisiblePages();
        }
        /// <summary>
        /// Called when foreground changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnForegroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnForegroundChanged(args);
        }
        /// <summary>
        /// Raises the <see cref="E:ForegroundChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDocumentLoaded && LayoutType == LayoutType.Block && Viewer != null)
                Viewer.RenderVisiblePages();
        }
        /// <summary>
        /// Called when padding changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPaddingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnPaddingChanged(args);
        }
        /// <summary>
        /// Raises the <see cref="E:PaddingChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDocumentLoaded && Viewer is FlowLayoutViewer)
                OnViewerChanged(Viewer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnSelectionStrokeColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnSelectionStrokeColorChanged(args);
        }

        protected virtual void OnSelectionFillColorChanged(DependencyPropertyChangedEventArgs e)
        {
            //Selection.SelectionFillColor = SelectionFillColor;
        }
        protected static void OnXAMLTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SfRichTextBoxAdv richtextbox = (SfRichTextBoxAdv)obj;
            if (e.NewValue != null)
            {
                richtextbox.Document = XAMLImporting.ConvertToDocumentAdv(e.NewValue.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnLayoutTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv document = (SfRichTextBoxAdv)dependencyObject;
            document.OnLayoutTypeChanged(args);
        }

        internal void OnLayoutTypeChanged(DependencyPropertyChangedEventArgs args)
        {
            switch ((LayoutType)args.NewValue)
            {
                case LayoutType.Pages:
                    if (Viewer is FlowLayoutViewer)
                    {
                        Viewer.Dispose();
                        Viewer = new PageLayoutViewer(this);
                    }
                    break;
                case LayoutType.Continuous:
                    Viewer.Dispose();
                    Viewer = new FlowLayoutViewer(this);
                    break;
                case LayoutType.Block:
                    ResetZooming();
                    Viewer.ResetScrollBars();
                    Viewer.Dispose();
                    Viewer = new FlowLayoutViewer(this);
                    break;
            }
        }

        protected static void OnVerticalScrollBarVisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnVerticalScrollBarVisibilityChanged(args);
        }

        private void OnVerticalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            if (verticalScrollBar != null && !(bool)args.NewValue)
                verticalScrollBar.Visibility = Visibility.Collapsed;
        }

        protected static void OnHorizontalScrollBarVisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnHorizontalScrollBarVisibilityChanged(args);
        }

        private void OnHorizontalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            if (horizontalScrollBar != null && !(bool)args.NewValue)
                horizontalScrollBar.Visibility = Visibility.Collapsed;
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnZoomFactorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv document = (SfRichTextBoxAdv)dependencyObject;
            document.OnZoomFactorChanged(args);
        }
        /// <summary>
        /// Raises the <see cref="E:ZoomFactorChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnZoomFactorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Viewer != null && (Viewer.IsLoadingPreloadPages() || LayoutType == LayoutType.Block))
            {
                if (!m_zoomFlag)
                {
                    m_zoomFlag = true;
                    ZoomFactor = Math.Round(Convert.ToDouble(args.OldValue));
                }
                m_zoomFlag = false;
                return;
            }
            if (IsZoomEnabled && LayoutType != LayoutType.Block)
            {
                double newvalue = Math.Round(Convert.ToDouble(args.NewValue));
                if (newvalue > 500)
                    newvalue = 500;
                else if (newvalue < 10)
                    newvalue = 10;
                if (!m_zoomFlag && Viewer != null)
                {
                    Viewer.ScaleFactor = newvalue / 100;
                    Viewer.Zoom();
                }

                if (ZoomFactorChanged != null)
                    ZoomFactorChanged(this, args);
            }
            m_zoomFlag = false;
        }

        internal LayoutViewer ChangePageLayoutForPrinting(bool change)
        {
            PageLayoutViewer viewer = null;
            //if (Viewer != null)
            //    Viewer.RemoveViewer();
            if (change)
            {
                viewer = new PageLayoutViewer(this);
                viewer.Measure(new Size(817, 1020));
            }
            else
            {
                UpdateEditorLayout();
            }

            return viewer;
        }


        protected static void OnIsContextMenuVisibleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnIsContextMenuVisibleChanged(args);
        }

        private void OnIsContextMenuVisibleChanged(DependencyPropertyChangedEventArgs args)
        {
            //if (this.ContextMenu != null && (bool)args.NewValue)
            //{
            //    this.ContextMenu.Visibility = Visibility.Visible;
            //}
            //else if (ContextMenu != null)
            //{
            //    this.ContextMenu.Visibility = Visibility.Collapsed;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnDocumentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnDocumentChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (History != null)
                History.ClearHistory();
            if (Selection != null)
                Selection.ClearSelection();
            if (e.NewValue != null)
            {
                DocumentAdv document = ((DocumentAdv)e.NewValue);
                document.OwnerControl = this;
            }
            if (Viewer != null && Viewer.HorizontalScrollBar != null)
                Viewer.HorizontalScrollBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnEnableCursorOnReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnEnableCursorOnReadOnlyChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEnableCursorOnReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (viewer != null)
                    viewer.ShowCaret(false);
            }
            else
            {
                if (viewer != null && IsReadOnlyMode)
                        viewer.HideCaret(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnIsReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SfRichTextBoxAdv richTextBox = (SfRichTextBoxAdv)dependencyObject;
            richTextBox.OnIsReadOnlyChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            //if ((bool)e.NewValue)
            //{
            //    if (CurrentPage != null)
            //    {
            //        if (EnableCursorOnReadOnly)
            //        {
            //            CurrentPage.ShowCaret(false);
            //        }
            //        else
            //            CurrentPage.HideCaret(false);
            //    }
            //}
            //else
            //{
            //    if(CurrentPage != null)
            //    {
            //        CurrentPage.ShowCaret(false);
            //    }
            //}

            if (IsReadOnlyChanged != null)
            {
                IsReadOnlyChanged(this, e);
            }
        }

        protected virtual void OnSelectionStrokeColorChanged(DependencyPropertyChangedEventArgs e)
        {
            //Selection.SelectionStrokeColor = SelectionStrokeColor;
        }
    }
}
