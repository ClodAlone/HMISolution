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
using System.Text;
using System.IO;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class RichTextBoxAdv
    {

        #region Event Handlers

        public delegate void FileOpeningEventHandler(object obj, FileOpeningEventArgs args);

        public delegate void FileSavingEventHandler(object obj, FileSavingEventArgs args);

        public delegate void TextChangedEventHandler(object obj, TextChangedEventArgs args);

        public delegate void SelectionChangedEventHandler(object obj, SelectionChangedEventArgs args);

        public delegate void OpenFailedEventHandler(object obj, OpenFailedEventArgs args);

        public delegate void SaveFailedEventHandler(object obj, SaveFailedEventArgs args);
        #endregion


#if !WPF

        public ContextMenuAdv ContextMenu
        {
            get
            {
                if (contextMenu != null)
                    return contextMenu;

                return null;
            }
        }
#endif

        internal string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the fill color
        /// </summary>
        public Brush SelectionFillColor
        {
            get
            {
                return (Brush)GetValue(SelectionFillColorProperty);
            }
            set
            {
                SetValue(SelectionFillColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the FillColorProperty dependency property
        /// </summary>
        public static readonly DependencyProperty SelectionFillColorProperty = DependencyProperty.Register("SelectionFillColor", typeof(Brush), typeof(RichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 142, 239)), OnSelectionFillColorChanged));

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

        public static readonly DependencyProperty DefferedScrollingProperty = DependencyProperty.Register("DefferedScrolling", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(false));

#if !WPF
        /// <summary>
        /// Gets or Sets the Bold Command
        /// </summary>
        public BoldCommand BoldCommand
        {
            get
            {
                return (BoldCommand)GetValue(BoldCommandProperty);
            }
            internal set
            {
                SetValue(BoldCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the BoldCommandProperty dependency property
        /// </summary>
        public static readonly DependencyProperty BoldCommandProperty = DependencyProperty.Register("BoldCommand", typeof(BoldCommand), typeof(RichTextBoxAdv), null);


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
            internal set
            {
                SetValue(ItalicCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ItalicCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItalicCommandProperty =
            DependencyProperty.Register("ItalicCommand", typeof(ItalicCommand), typeof(RichTextBoxAdv), null);


        public TextAlignmentCommand TextAlignmentCommand
        {
            get
            {
                return (TextAlignmentCommand)GetValue(TextAlignmentCommandProperty);
            }
            internal set
            {
                SetValue(TextAlignmentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for TextAlignementCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentCommandProperty =
            DependencyProperty.Register("TextAlignmentCommand", typeof(TextAlignmentCommand), typeof(RichTextBoxAdv), null);

        public BeforeSpacingCommand BeforeSpacingCommand
        {
            get
            {
                return (BeforeSpacingCommand)GetValue(BeforeSpacingCommandProperty);
            }
            internal set
            {
                SetValue(BeforeSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for BeforeSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeforeSpacingCommandProperty =
            DependencyProperty.Register("BeforeSpacingCommand", typeof(BeforeSpacingCommand), typeof(RichTextBoxAdv), null);

#endif
        
        public string DocumentTitle
        {
            get { return (string)GetValue(DocumentTitleProperty); }
            set { SetValue(DocumentTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DocumentTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentTitleProperty =
            DependencyProperty.Register("DocumentTitle", typeof(string), typeof(RichTextBoxAdv), new PropertyMetadata(string.Empty));


        public bool HideCursorOnLostFocus
        {
            get { return (bool)GetValue(HideCursorOnLostFocusProperty); }
            set { SetValue(HideCursorOnLostFocusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideCursorOnLostFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideCursorOnLostFocusProperty =
            DependencyProperty.Register("HideCursorOnLostFocus", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnHideCursorOnLostFocusChanged)));
        
        public bool EnableCursorOnReadOnly
        {
            get { return (bool)GetValue(EnableCursorOnReadOnlyProperty); }
            set { SetValue(EnableCursorOnReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableCursorOnReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableCursorOnReadOnlyProperty =
            DependencyProperty.Register("EnableCursorOnReadOnly", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(false,new PropertyChangedCallback(OnEnableCursorOnReadOnlyChanged)));


        public string HTMLText
        {
            get
            {
                if (this.Document != null)
                    return HTMLExporting.ConvertToHtml(this.Document);
                return null;
            }
            set
            {
                SetValue(HTMLTextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for HTMLText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HTMLTextProperty =
            DependencyProperty.Register("HTMLText", typeof(string), typeof(RichTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnHTMLTextChanged)));


        public string XAMLText
        {
            get
            {
                if (this.Document != null)
                    return XAMLExporting.ConvertToXAML(this.Document);
                return null;
            }
            set
            {
                SetValue(XAMLTextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for XAMLText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAMLTextProperty =
            DependencyProperty.Register("XAMLText", typeof(string), typeof(RichTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnXAMLTextChanged)));

        /// <summary>
        /// Gets or Sets the page layout.
        /// </summary>
        public PageLayout PageLayout
        {
            get
            {
                return (PageLayout)GetValue(PageLayoutProperty);
            }
            set
            {
                SetValue(PageLayoutProperty, value);
            }
        }

        public LayoutViewer Viewer
        {
            get { return (LayoutViewer)GetValue(ViewerProperty); }
            set { SetValue(ViewerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Viewer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewerProperty =
            DependencyProperty.Register("Viewer", typeof(LayoutViewer), typeof(RichTextBoxAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnViewerChanged)));



        /// <summary>
        /// Registers the PageLayout Dependency property
        /// </summary>
        public static readonly DependencyProperty PageLayoutProperty = DependencyProperty.Register("PageLayout", typeof(PageLayout), typeof(RichTextBoxAdv), new PropertyMetadata(PageLayout.Continuous, OnPageLayoutChanged));

#if !WPF
        public AfterSpacingCommand AfterSpacingCommand
        {
            get
            {
                return (AfterSpacingCommand)GetValue(AfterSpacingCommandProperty);
            }
            internal set
            {
                SetValue(AfterSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for AfterSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AfterSpacingCommandProperty =
            DependencyProperty.Register("AfterSpacingCommand", typeof(AfterSpacingCommand), typeof(RichTextBoxAdv), null);


        public LineSpacingCommand LineSpacingCommand
        {
            get
            {
                return (LineSpacingCommand)GetValue(LineSpacingCommandProperty);
            }
            internal set
            {
                SetValue(LineSpacingCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for LineSpacingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSpacingCommandProperty =
            DependencyProperty.Register("LineSpacingCommand", typeof(LineSpacingCommand), typeof(RichTextBoxAdv), null);



        public LeftIndentCommand LeftIndentCommand
        {
            get
            {
                return (LeftIndentCommand)GetValue(LeftIndentCommandProperty);
            }
            internal set
            {
                SetValue(LeftIndentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for LeftIndentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIndentCommandProperty =
            DependencyProperty.Register("LeftIndentCommand", typeof(LeftIndentCommand), typeof(RichTextBoxAdv), null);
#endif
        /// <summary>
        /// Zoom factor value define the Zooming [Enter a value between 0 to 1]
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(RichTextBoxAdv), new PropertyMetadata(0.0, new PropertyChangedCallback(OnZoomFactorChanged)));

        public bool IsEmpty
        {
            get
            {
                if (Document != null && Document.Sections.Count == 1)
                {
                    if (Document.Sections[0].Blocks.Count == 1)
                    {
                        ParagraphAdv paragraph = Document.Sections[0].Blocks[0] as ParagraphAdv;
                        if (paragraph.Inlines.Count == 0)
                        {
                            return paragraph.Inlines.Count == 0;
                        }
                        else
                        {
                            if (paragraph.Inlines.Count == 1)
                            {
                                if (paragraph.Inlines[0] is SpanAdv)
                                {
                                    return (paragraph.Inlines[0] as SpanAdv).Text == string.Empty;
                                }
                                else if (paragraph.Inlines[0] is HyperlinkAdv)
                                {
                                    return (paragraph.Inlines[0] as HyperlinkAdv).Text == string.Empty;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

#if !WPF
        public RightIndentCommand RightIndentCommand
        {
            get
            {
                return (RightIndentCommand)GetValue(RightIndentCommandProperty);
            }
            internal set
            {
                SetValue(RightIndentCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RightIndentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightIndentCommandProperty =
            DependencyProperty.Register("RightIndentCommand", typeof(RightIndentCommand), typeof(RichTextBoxAdv), null);



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
            internal set
            {
                SetValue(CutCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CutCommandProperty =
            DependencyProperty.Register("CutCommand", typeof(CutCommand), typeof(RichTextBoxAdv), null);

        public ParagraphDialogCommand ParagraphDialogCommand
        {
            get { return (ParagraphDialogCommand)GetValue(ParagraphDialogCommandProperty); }
            internal set { SetValue(ParagraphDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParagraphPropertiesDialogCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParagraphDialogCommandProperty =
            DependencyProperty.Register("ParagraphDialogCommand", typeof(ParagraphDialogCommand), typeof(RichTextBoxAdv), null);




        public InsertTableDialogCommand InsertTableDialogCommand
        {
            get { return (InsertTableDialogCommand)GetValue(InsertTableDialogCommandProperty); }
            set { SetValue(InsertTableDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertTableDialogCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertTableDialogCommandProperty =
            DependencyProperty.Register("InsertTableDialogCommand", typeof(InsertTableDialogCommand), typeof(RichTextBoxAdv), null);



        public FontDialogCommand FontDialogCommand
        {
            get { return (FontDialogCommand)GetValue(FontDialogCommandProperty); }
            set { SetValue(FontDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontDialogCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontDialogCommandProperty =
            DependencyProperty.Register("FontDialogCommand", typeof(FontDialogCommand), typeof(RichTextBoxAdv), null);




        public HyperlinkDialogCommand HyperlinkDialogCommand
        {
            get { return (HyperlinkDialogCommand)GetValue(HyperlinkDialogCommandProperty); }
            set { SetValue(HyperlinkDialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HyperlinkDialogCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HyperlinkDialogCommandProperty =
            DependencyProperty.Register("HyperlinkDialogCommand", typeof(HyperlinkDialogCommand), typeof(RichTextBoxAdv), null);


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
            internal set
            {
                SetValue(CopyCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CopyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(CopyCommand), typeof(RichTextBoxAdv), null);



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
            internal set
            {
                SetValue(PasteCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PasteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(PasteCommand), typeof(RichTextBoxAdv), null);




        /// <summary>
        /// Gets or sets the change highlight color command.
        /// </summary>
        /// <value>The change highlight color command.</value>
        public ChangeHighlightColorCommand ChangeHighlightColorCommand
        {
            get
            {
                return (ChangeHighlightColorCommand)GetValue(ChangeHighlightColorCommandProperty);
            }
            internal set
            {
                SetValue(ChangeHighlightColorCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeHighlightColorCommandProperty =
            DependencyProperty.Register("ChangeHighlightColorCommand", typeof(ChangeHighlightColorCommand), typeof(RichTextBoxAdv), null);



        /// <summary>
        /// Gets or sets the double strike through command.
        /// </summary>
        /// <value>The double strike through command.</value>
        public DoubleStrikeThroughCommand DoubleStrikeThroughCommand
        {
            get
            {
                return (DoubleStrikeThroughCommand)GetValue(DoubleStrikeThroughCommandProperty);
            }
            internal set
            {
                SetValue(DoubleStrikeThroughCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for DoubleStrikeThroughCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleStrikeThroughCommandProperty =
            DependencyProperty.Register("DoubleStrikeThroughCommand", typeof(DoubleStrikeThroughCommand), typeof(RichTextBoxAdv), null);




        /// <summary>
        /// Gets or sets the change text color command.
        /// </summary>
        /// <value>The change text color command.</value>
        public ChangeTextColorCommand ChangeTextColorCommand
        {
            get
            {
                return (ChangeTextColorCommand)GetValue(ChangeTextColorCommandProperty);
            }
            internal set
            {
                SetValue(ChangeTextColorCommandProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ChangeTextColorCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeTextColorCommandProperty =
            DependencyProperty.Register("ChangeTextColorCommand", typeof(ChangeTextColorCommand), typeof(RichTextBoxAdv), null);



        /// <summary>
        /// Gets or sets the change font size command.
        /// </summary>
        /// <value>The change font size command.</value>
        public ChangeFontSizeCommand ChangeFontSizeCommand
        {
            get { return (ChangeFontSizeCommand)GetValue(ChangeFontSizeCommandProperty); }
            internal set { SetValue(ChangeFontSizeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeFontSizeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeFontSizeCommandProperty =
            DependencyProperty.Register("ChangeFontSizeCommand", typeof(ChangeFontSizeCommand), typeof(RichTextBoxAdv), null);



        /// <summary>
        /// Gets or sets the change font family command.
        /// </summary>
        /// <value>The change font family command.</value>
        public ChangeFontFamilyCommand ChangeFontFamilyCommand
        {
            get { return (ChangeFontFamilyCommand)GetValue(ChangeFontFamilyCommandProperty); }
            internal set { SetValue(ChangeFontFamilyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeFontFamilyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeFontFamilyCommandProperty =
            DependencyProperty.Register("ChangeFontFamilyCommand", typeof(ChangeFontFamilyCommand), typeof(RichTextBoxAdv), null);



        public SaveAsDocumentCommand SaveAsDocumentCommand
        {
            get { return (SaveAsDocumentCommand)GetValue(SaveAsDocumentCommandProperty); }
            set { SetValue(SaveAsDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveAsDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveAsDocumentCommandProperty =
            DependencyProperty.Register("SaveAsDocumentCommand", typeof(SaveAsDocumentCommand), typeof(RichTextBoxAdv), new PropertyMetadata(null));




        public ChangeListTypeCommand ChangeListTypeCommand
        {
            get { return (ChangeListTypeCommand)GetValue(ChangeListTypeCommandProperty); }
            set { SetValue(ChangeListTypeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeListTypeCommmand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeListTypeCommandProperty =
            DependencyProperty.Register("ChangeListTypeCommand", typeof(ChangeListTypeCommand), typeof(RichTextBoxAdv), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the underline command.
        /// </summary>
        /// <value>The underline command.</value>
        public UnderlineCommand UnderlineCommand
        {
            get { return (UnderlineCommand)GetValue(UnderlineCommandProperty); }
            internal set { SetValue(UnderlineCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnderlineCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderlineCommandProperty =
            DependencyProperty.Register("UnderlineCommand", typeof(UnderlineCommand), typeof(RichTextBoxAdv), null);



        /// <summary>
        /// Gets or sets the single strikethrough command.
        /// </summary>
        /// <value>The single strikethrough command.</value>
        public SingleStrikeThroughCommand SingleStrikethroughCommand
        {
            get { return (SingleStrikeThroughCommand)GetValue(SingleStrikethroughCommandProperty); }
            internal set { SetValue(SingleStrikethroughCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Single.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleStrikethroughCommandProperty =
            DependencyProperty.Register("SingleStrikethroughCommand", typeof(SingleStrikeThroughCommand), typeof(RichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the undo command.
        /// </summary>
        /// <value>The single strikethrough command.</value>
        public UndoCommand UndoCommand
        {
            get { return (UndoCommand)GetValue(UndoCommandProperty); }
            internal set { SetValue(UndoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Single.
        public static readonly DependencyProperty UndoCommandProperty =
            DependencyProperty.Register("UndoCommand", typeof(UndoCommand), typeof(RichTextBoxAdv), null);

        /// <summary>
        /// Gets or sets the redo command.
        /// </summary>
        /// <value>The single strikethrough command.</value>
        public RedoCommand RedoCommand
        {
            get { return (RedoCommand)GetValue(RedoCommandProperty); }
            internal set { SetValue(RedoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Single.
        public static readonly DependencyProperty RedoCommandProperty =
            DependencyProperty.Register("RedoCommand", typeof(RedoCommand), typeof(RichTextBoxAdv), null);


        /// <summary>
        /// Gets or sets the superscript command.
        /// </summary>
        /// <value>The superscript command.</value>
        public SuperscriptCommand SuperscriptCommand
        {
            get { return (SuperscriptCommand)GetValue(SuperscriptCommandProperty); }
            internal set { SetValue(SuperscriptCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SuperscriptCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuperscriptCommandProperty =
            DependencyProperty.Register("SuperscriptCommand", typeof(SuperscriptCommand), typeof(RichTextBoxAdv), null);



        /// <summary>
        /// Gets or sets the subscript command.
        /// </summary>
        /// <value>The subscript command.</value>
        public SubscriptCommand SubscriptCommand
        {
            get { return (SubscriptCommand)GetValue(SubscriptCommandProperty); }
            internal set { SetValue(SubscriptCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubscriptCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubscriptCommandProperty =
            DependencyProperty.Register("SubscriptCommand", typeof(SubscriptCommand), typeof(RichTextBoxAdv), null);




        /// <summary>
        /// Gets or sets the open document command.
        /// </summary>
        /// <value>The open document command.</value>
        public OpenDocumentCommand OpenDocumentCommand
        {
            get { return (OpenDocumentCommand)GetValue(OpenDocumentCommandProperty); }
            internal set { SetValue(OpenDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenDocumentCommandProperty =
            DependencyProperty.Register("OpenDocumentCommand", typeof(OpenDocumentCommand), typeof(RichTextBoxAdv), null);


        public PrintDocumentCommand PrintDocumentCommand
        {
            get { return (PrintDocumentCommand)GetValue(PrintDocumentCommandProperty); }
            set { SetValue(PrintDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrintDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintDocumentCommandProperty =
            DependencyProperty.Register("PrintDocumentCommand", typeof(PrintDocumentCommand), typeof(RichTextBoxAdv), null);



        public NewDocumentCommand NewDocumentCommand
        {
            get { return (NewDocumentCommand)GetValue(NewDocumentCommandProperty); }
            set { SetValue(NewDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewDocumentCommandProperty =
            DependencyProperty.Register("NewDocumentCommand", typeof(NewDocumentCommand), typeof(RichTextBoxAdv), null);




        public InsertRowCommand InsertRowCommand
        {
            get { return (InsertRowCommand)GetValue(InsertRowCommandProperty); }
            set { SetValue(InsertRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertRowCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertRowCommandProperty =
            DependencyProperty.Register("InsertRowCommand", typeof(InsertRowCommand), typeof(RichTextBoxAdv), null);




        public InsertColumnCommand InsertColumnCommand
        {
            get { return (InsertColumnCommand)GetValue(InsertColumnCommandProperty); }
            set { SetValue(InsertColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertColumnCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertColumnCommandProperty =
            DependencyProperty.Register("InsertColumnCommand", typeof(InsertColumnCommand), typeof(RichTextBoxAdv), null);



        public InsertTableCommand InsertTableCommand
        {
            get { return (InsertTableCommand)GetValue(InsertTableCommandProperty); }
            set { SetValue(InsertTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertTableCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertTableCommandProperty =
            DependencyProperty.Register("InsertTableCommand", typeof(InsertTableCommand), typeof(RichTextBoxAdv), null);

        public DeleteRowCommand DeleteRowCommand
        {
            get { return (DeleteRowCommand)GetValue(DeleteRowCommandProperty); }
            set { SetValue(DeleteRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteRowCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteRowCommandProperty =
            DependencyProperty.Register("DeleteRowCommand", typeof(DeleteRowCommand), typeof(RichTextBoxAdv), null);


        public DeleteColumnCommand DeleteColumnCommand
        {
            get { return (DeleteColumnCommand)GetValue(DeleteColumnCommandProperty); }
            set { SetValue(DeleteColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteColumnCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteColumnCommandProperty =
            DependencyProperty.Register("DeleteColumnCommand", typeof(DeleteColumnCommand), typeof(RichTextBoxAdv), null);


        public DeleteTableCommand DeleteTableCommand
        {
            get { return (DeleteTableCommand)GetValue(DeleteTableCommandProperty); }
            set { SetValue(DeleteTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteTableCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteTableCommandProperty =
            DependencyProperty.Register("DeleteTableCommand", typeof(DeleteTableCommand), typeof(RichTextBoxAdv), null);



        public SelectCellCommand SelectCellCommand
        {
            get { return (SelectCellCommand)GetValue(SelectCellCommandProperty); }
            set { SetValue(SelectCellCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectCellCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectCellCommandProperty =
            DependencyProperty.Register("SelectCellCommand", typeof(SelectCellCommand), typeof(RichTextBoxAdv), null);




        public SelectRowCommand SelectRowCommand
        {
            get { return (SelectRowCommand)GetValue(SelectRowCommandProperty); }
            set { SetValue(SelectRowCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectRowCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectRowCommandProperty =
            DependencyProperty.Register("SelectRowCommand", typeof(SelectRowCommand), typeof(RichTextBoxAdv), null);




        public SelectColumnCommand SelectColumnCommand
        {
            get { return (SelectColumnCommand)GetValue(SelectColumnCommandProperty); }
            set { SetValue(SelectColumnCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectColumnCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectColumnCommandProperty =
            DependencyProperty.Register("SelectColumnCommand", typeof(SelectColumnCommand), typeof(RichTextBoxAdv), null);




        public SelectTableCommand SelectTableCommand
        {
            get { return (SelectTableCommand)GetValue(SelectTableCommandProperty); }
            set { SetValue(SelectTableCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectTableCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectTableCommandProperty =
            DependencyProperty.Register("SelectTableCommand", typeof(SelectTableCommand), typeof(RichTextBoxAdv), null);


        /// <summary>
        /// Gets or sets the save document command.
        /// </summary>
        /// <value>The save document command.</value>
        public SaveDocumentCommand SaveDocumentCommand
        {
            get { return (SaveDocumentCommand)GetValue(SaveDocumentCommandProperty); }
            internal set { SetValue(SaveDocumentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveDocumentCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveDocumentCommandProperty =
            DependencyProperty.Register("SaveDocumentCommand", typeof(SaveDocumentCommand), typeof(RichTextBoxAdv), null);
        
        public ChangePageLayoutCommand ChangePageLayoutCommand
        {
            get { return (ChangePageLayoutCommand)GetValue(ChangePageLayoutCommandProperty); }
            set { SetValue(ChangePageLayoutCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangePageLayoutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangePageLayoutCommandProperty =
            DependencyProperty.Register("ChangePageLayoutCommand", typeof(ChangePageLayoutCommand), typeof(RichTextBoxAdv), new PropertyMetadata(null));

        public MergeSelectedCellsCommand MergeSelectedCellsCommand
        {
            get { return (MergeSelectedCellsCommand)GetValue(MergeSelectedCellsCommandProperty); }
            set { SetValue(MergeSelectedCellsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MergeSelectedCellsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MergeSelectedCellsCommandProperty =
            DependencyProperty.Register("MergeSelectedCellsCommand", typeof(MergeSelectedCellsCommand), typeof(RichTextBoxAdv), null);


        public InsertPictureCommand InsertPictureCommand
        {
            get { return (InsertPictureCommand)GetValue(InsertPictureCommandProperty); }
            set { SetValue(InsertPictureCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertPictureCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertPictureCommandProperty =
            DependencyProperty.Register("InsertPictureCommand", typeof(InsertPictureCommand), typeof(RichTextBoxAdv), null);

        public ChangeTableCellStyleCommand ChangeTableCellStyleCommand
        {
            get { return (ChangeTableCellStyleCommand)GetValue(ChangeTableCellStyleCommandProperty); }
            set { SetValue(ChangeTableCellStyleCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeTableCellStyleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeTableCellStyleCommandProperty =
            DependencyProperty.Register("ChangeTableCellStyleCommand", typeof(ChangeTableCellStyleCommand), typeof(RichTextBoxAdv), null);


        public ChangeTableBorderColorCommand ChangeTableBorderColorCommand
        {
            get { return (ChangeTableBorderColorCommand)GetValue(ChangeTableBorderColorCommandProperty); }
            set { SetValue(ChangeTableBorderColorCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeTableStyleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeTableBorderColorCommandProperty =
            DependencyProperty.Register("ChangeTableBorderColorCommand", typeof(ChangeTableBorderColorCommand), typeof(RichTextBoxAdv), null);
#endif

        public bool IsContextMenuVisible
        {
            get { return (bool)GetValue(IsContextMenuVisibleProperty); }
            set { SetValue(IsContextMenuVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextMenuVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContextMenuVisibleProperty =
            DependencyProperty.Register("IsContextMenuVisible", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsContextMenuVisibleChanged)));

        public SelectionAdv Selection
        {
            get { return (SelectionAdv)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Selection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register("Selection", typeof(SelectionAdv), typeof(RichTextBoxAdv), null);

        public bool VerticalScrollBarVisibility
        {
            get { return (bool)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnVerticalScrollBarVisibilityChanged)));



        public FontCollection Fonts
        {
            get { return (FontCollection)GetValue(FontsProperty); }
            set { SetValue(FontsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fonts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontsProperty =
            DependencyProperty.Register("Fonts", typeof(FontCollection), typeof(RichTextBoxAdv), new PropertyMetadata(null));



        public bool HorizontalScrollBarVisibility
        {
            get { return (bool)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnHorizontalScrollBarVisibilityChanged)));


        #region Event

        public event FileOpeningEventHandler FileOpening;

        public event FileSavingEventHandler FileSaving;

        public event EventHandler StyleChanged;

        public event RoutedEventHandler Printing;

        public event EventHandler PrintCompleted;

        public event TextChangedEventHandler TextChanged;

        public event SelectionChangedEventHandler SelectionChanged;

        public event EventHandler HyperlinkClicked;

        public event DependencyPropertyChangedEventHandler IsReadOnlyChanged;

        public event DependencyPropertyChangedEventHandler ZoomFactorChanged;

        /// <summary>
        /// Occurs when open failed.
        /// </summary>
        public event OpenFailedEventHandler OpenFailed;
        /// <summary>
        /// Occurs when save failed.
        /// </summary>
        public event SaveFailedEventHandler SaveFailed;
        #endregion

        /// <summary>
        /// Gets or Sets the stroke value
        /// </summary>
        public Brush SelectionStrokeColor
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
        public static readonly DependencyProperty SelectionStrokeColorProperty = DependencyProperty.Register("SelectionStrokeColor", typeof(Brush), typeof(RichTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 44, 142, 239)), OnSelectionStrokeColorChanged));



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnSelectionFillColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
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
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(DocumentAdv), typeof(RichTextBoxAdv), new PropertyMetadata(OnDocumentChanged));

        /// <summary>
        /// Gets or Sets a value indicating whether to record Undo/Redo
        /// </summary>
        public bool DisableHistory
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
        public static readonly DependencyProperty DisableHistoryProperty = DependencyProperty.Register("DisableHistoryProperty", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(OnDisableHistoryChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnDisableHistoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnDisableHistoryChanged(args);
        }

        protected virtual void OnDisableHistoryChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                History.ClearHistory();
            }
        }

        /// <summary>
        /// Get or Sets a value indicating the control can be enablled
        /// </summary>
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
        /// Registers the IsReadOnly dependency property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(false,new PropertyChangedCallback(OnIsReadOnlyChanged)));

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
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(true));

#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
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

        public bool IsPositionInsideTable
        {
            get { return (bool)GetValue(IsPositionInsideTableProperty); }
            internal set { SetValue(IsPositionInsideTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPositionInsideTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPositionInsideTableProperty =
            DependencyProperty.Register("IsPositionInsideTable", typeof(bool), typeof(RichTextBoxAdv), new PropertyMetadata(false));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnSelectionStrokeColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnSelectionStrokeColorChanged(args);
        }

        protected virtual void OnSelectionFillColorChanged(DependencyPropertyChangedEventArgs e)
        {
            Selection.SelectionFillColor = SelectionFillColor;
        }

        protected static void OnHTMLTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            RichTextBoxAdv richtextbox = (RichTextBoxAdv)obj;
            if (e.NewValue != null)
            {
                richtextbox.Document = HTMLImporting.ConvertToDocumentAdv(e.NewValue.ToString());
            }
        }

        protected static void OnXAMLTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            RichTextBoxAdv richtextbox = (RichTextBoxAdv)obj;
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
        protected static void OnPageLayoutChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv document = (RichTextBoxAdv)dependencyObject;
            document.OnPageLayoutChanged(args);
        }

        internal void OnPageLayoutChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Document != null)
                Document.ClearLines();
            if (Viewer != null)
                Viewer.RemoveViewer();
            if ((PageLayout)args.NewValue == PageLayout.Pages)
            {
                PageLayoutViewer viewer = new PageLayoutViewer(this);
                viewer.Document = Document;
                if (Viewer != null)
                    viewer.SelectedImage = Viewer.SelectedImage;
                Viewer = viewer;
            }
            else if ((PageLayout)args.NewValue == PageLayout.Continuous)
            {
                FlowLayoutViewer viewer = new FlowLayoutViewer(this);
                viewer.Document = Document;
                if (Viewer != null)
                    viewer.SelectedImage = Viewer.SelectedImage;
                Viewer = viewer;
            }

            Viewer.OnLoading = true;
            UpdateEditorLayout(false);
#if !WPF
            ChangePageLayoutCommand.ExecuteChanged();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnViewerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv document = (RichTextBoxAdv)dependencyObject;
            document.OnViewerChanged(args);
        }

        internal void OnViewerChanged(DependencyPropertyChangedEventArgs args)
        {
            LayoutViewer viewer = (LayoutViewer)args.NewValue;
            if (Selection != null)
                Selection.LayoutViewer = viewer;
            if (contentPresenter != null)
                contentPresenter.Content = viewer;

            viewer.HorizontalScrollBar = horizontalScrollBar;
            viewer.VerticalScrollBar = verticalScrollBar;
            viewer.UpdateCurrentPageNumber();
        }

        protected static void OnVerticalScrollBarVisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnVerticalScrollBarVisibilityChanged(args);
        }

        private void OnVerticalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            if (verticalScrollBar != null && !(bool)args.NewValue)
                verticalScrollBar.Visibility = Visibility.Collapsed;
            else if (Viewer != null && !Viewer.NeedRecreateLayout)
                UpdateEditorLayout(false);

        }

        protected static void OnHorizontalScrollBarVisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnHorizontalScrollBarVisibilityChanged(args);
        }

        private void OnHorizontalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            if (horizontalScrollBar != null && !(bool)args.NewValue)
                horizontalScrollBar.Visibility = Visibility.Collapsed;
            else if (Viewer != null && !Viewer.NeedRecreateLayout)
                UpdateEditorLayout(false);

        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnZoomFactorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv document = (RichTextBoxAdv)dependencyObject;
            document.OnZoomFactorChanged(args);
        }

        internal void OnZoomFactorChanged(DependencyPropertyChangedEventArgs args)
        {
            double newvalue = Math.Round(Convert.ToDouble(args.NewValue),2);
            double oldvalue = Math.Round(Convert.ToDouble(args.OldValue),2);
            const double Factor = 4;

            if (newvalue >= 0 && newvalue <= 1 && IsZoomEnabled)
            {
                if (!m_zoomFlag)
                {
                    if (Viewer != null)
                    {
                        if (newvalue == 0)
                        {
                            newvalue = 0.1;
                        }
                        Viewer.transX = newvalue * Factor;
                        Viewer.transY = newvalue * Factor;
                        Viewer.Zoom();
                    }
                }

                if (ZoomFactorChanged != null)
                {
                    ZoomFactorChanged(this, args);
                }
            }
            m_zoomFlag = false;
        }

        internal LayoutViewer ChangePageLayoutForPrinting(bool change)
        {
            PageLayoutViewer viewer = null;
            if (Document != null)
                Document.ClearLines();
            //if (Viewer != null)
            //    Viewer.RemoveViewer();
            if (change)
            {
                viewer = new PageLayoutViewer(this);
                viewer.IsPrinting = true;
                viewer.Document = Document;
                viewer.OnLoading = true;
                viewer.NeedRecreateLayout = true;
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
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnIsContextMenuVisibleChanged(args);
        }

        private void OnIsContextMenuVisibleChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ContextMenu != null && (bool)args.NewValue)
            {
                this.ContextMenu.Visibility = Visibility.Visible;
            }
            else if (ContextMenu != null)
            {
                this.ContextMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnDocumentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnDocumentChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentLoaded = true;

                if (Selection != null)
                {
                    Selection.Start = null;
                    Selection.End = null;
                }

                ((DocumentAdv)e.NewValue).OwnerControl = this;

                DocumentAdv document = ((DocumentAdv)e.NewValue);

                if (Viewer != null && Viewer.HorizontalScrollBar != null)
                {
                    Viewer.HorizontalScrollBar.Visibility = Visibility.Collapsed;
                }

                //document.PageLayout = PageLayout.Pages;

                if (document.Sections.Count == 0)
                {
                    SectionAdv section = new SectionAdv();
                    document.Sections.Add(section);
                }

                if (document.Sections[0].Blocks.Count == 0)
                {
                    ParagraphAdv paragraph = new ParagraphAdv();
                    document.Sections[0].Blocks.Add(paragraph);
                }

                PositionHandler = new DocumentPositionHandler(document);
                PositionHandler.OwnerControl = this;

                //if (Viewer is PageLayoutViewer && document.PageLayout != PageLayout.Pages)
                //{
                //    Viewer.Document = document;
                //    Viewer = new FlowLayoutViewer(this);
                //    Selection.LayoutViewer = Viewer;
                //    Viewer.Document = document;
                //}
                //else if (Viewer is FlowLayoutViewer && document.PageLayout != PageLayout.Continuous)
                //{
                //    Viewer.Document = document;
                //    Viewer = new PageLayoutViewer(this);
                //    Selection.LayoutViewer = Viewer;
                //    Viewer.Document = document;
                //    this.contentPresenter.Content = Viewer;
                //    Viewer.HorizontalScrollBar = horizontalScrollBar;
                //    Viewer.VerticalScrollBar = verticalScrollBar;
                //}
                //else 
                if (Viewer != null)
                {
                    //Type type = Viewer.GetType();
                    //Viewer = (LayoutViewer)Activator.CreateInstance(type, this);
                    Viewer.Document = document;
                }
                if (Selection != null)
                {
                    Selection.Document = document;
                }
                if (History != null)
                {
                    History.Document = document;
                }
                if (DocumentLoaded)
                {
                    History.ClearHistory();
                    Viewer.NeedRecreateLayout = true;
                    UpdateEditorLayout();
                    DocumentLoaded = false;
                }
            }
            else
            {
                if (Viewer != null)
                    Viewer.Document = null;
                if (Selection != null)
                    Selection.Document = null;
                if (History != null)
                {
                    History.Document = null;
                    History.ClearHistory();
                }
                CreateEmptyDocument();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnEnableCursorOnReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
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
                if (CurrentPage != null)
                {
                    CurrentPage.ShowCaret(false);
                }
            }
            else
            {
                if (CurrentPage != null)
                {
                    if(IsReadOnly)
                        CurrentPage.HideCaret(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnHideCursorOnLostFocusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnHideCursorOnLostFocusChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHideCursorOnLostFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                if (CurrentPage != null)
                {
                    CurrentPage.ShowCaret(false);
                }
            }
            else
            {
                if (CurrentPage != null)
                {
                    CurrentPage.HideCaret(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnIsReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextBoxAdv richTextBox = (RichTextBoxAdv)dependencyObject;
            richTextBox.OnIsReadOnlyChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (CurrentPage != null)
                {
                    if (EnableCursorOnReadOnly)
                    {
                        CurrentPage.ShowCaret(false);
                    }
                    else
                        CurrentPage.HideCaret(false);
                }
            }
            else
            {
                if(CurrentPage != null)
                {
                    CurrentPage.ShowCaret(false);
                }
            }

            if (IsReadOnlyChanged != null)
            {
                IsReadOnlyChanged(this, e);
            }
        }

        protected virtual void OnSelectionStrokeColorChanged(DependencyPropertyChangedEventArgs e)
        {
            Selection.SelectionStrokeColor = SelectionStrokeColor;
        }
    }
}
