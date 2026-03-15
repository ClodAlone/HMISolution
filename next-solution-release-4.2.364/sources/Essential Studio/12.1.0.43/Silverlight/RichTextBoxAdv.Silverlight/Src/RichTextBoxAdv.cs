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
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;
using System.IO;
using System.Text;
using Syncfusion.Windows.Shared;

#if !WPF
using Syncfusion.Windows.Controls.Theming;
#endif

using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

#if WPF
using Microsoft.Win32;
using Syncfusion.Licensing;
using System.Collections.ObjectModel;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Document")]

#if !WPF
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/RichTextBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.Theming.Blend;component/RichTextBoxAdv.xaml")]
#endif

#if !WPF
    public partial class RichTextBoxAdv : Control, ISkinStylePropagator
#endif
#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle=Skin.Blend,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
   Type = typeof(RichTextBoxAdv), XamlResource = "/Syncfusion.RichTextBoxAdv.WPF;component/Themes/TransparentStyle.xaml")] 
    public partial class RichTextBoxAdv:Control,IDisposable,ISkinStylePropagator
#endif
    {
        #region fields
        /// <summary>
        /// Gets or Sets the content of RichTextboxAdv
        /// </summary>
        internal ContentControl contentPresenter;

        /// <summary>
        /// 
        /// </summary>
        private InlineStyle currentInlineStyle;

        private ParagraphStyle currentParagraphStyle;

        /// <summary>
        /// Gets or Sets the horizontal scroll bar
        /// </summary>
        internal ScrollBar horizontalScrollBar;

        /// <summary>
        /// Gets or Sets the vertical scroll bar
        /// </summary>
        internal ScrollBar verticalScrollBar;

        /// <summary>
        /// Gets or Sets the page layout canvas
        /// </summary>
        internal Canvas containerLayout;

        /// <summary>
        /// Gets or Sets the current page
        /// </summary>
        internal PageAdv CurrentPage;
        
        internal RenderingManager RenderingManager;

        /// <summary>
        /// Gets or Sets the current paragraph
        /// </summary>
        internal ParagraphAdv CurrentParagraph;
        internal bool DocumentLoaded = false;
        internal History History;
        private bool isHandled = false;
        private DocumentPositionHandler positionHandler;
        private string documentTitle = string.Empty;
        internal bool m_zoomFlag = false;
        private bool m_contextmenuflag = false;

#if !WPF

        private ContextMenuItemAdv cutItem;
        private ContextMenuItemAdv copyItem;
        private ContextMenuItemAdv pasteItem;
        private ContextMenuAdv contextMenu;
        private ContextMenuItemAdv tableitem;
#endif
#if WPF
        MenuItem cutitem = null;
        MenuItem copyitem = null;
        MenuItem pasteitem = null;
        Separator pastesplitter = null;
        MenuItem insertmenuitem = null;
        MenuItem deletemenuitem = null;
        MenuItem selectmenuitem = null;
        MenuItem mergemenuitem = null;
        Separator mergeseparator = null;
        MenuItem insertTable = null;
        MenuItem fontitem = null;
        MenuItem paragraphitem = null;
        MenuItem bulletitem = null;
        MenuItem numberitem = null;
        Separator hyperlinkseparator = null;
        MenuItem hyperlinkitem = null;

#endif
        private Dictionary<int, object> m_removeditems = new Dictionary<int, object>();
              
        internal DocumentPositionHandler PositionHandler
        {
            get
            {
                if (positionHandler == null)
                {
                    positionHandler = new DocumentPositionHandler(Document);
                    positionHandler.OwnerControl = this;
                }
                if (positionHandler.OwnerControl == null)
                {
                    positionHandler.OwnerControl = this;
                }

                return positionHandler;
            }
            set
            {
                positionHandler = value;
            }
        }

#if !WPF
        private Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get;
            set;
        }
#endif

        #endregion

        /// <summary>
        /// Initializes the new instance of RichTextBoxAdv class
        /// </summary>
        public RichTextBoxAdv()
        {
            this.DefaultStyleKey = typeof(RichTextBoxAdv);
            this.containerLayout = new Canvas();
            Selection = new SelectionAdv(this); 
            InitCommands();
            RenderingManager = new RenderingManager(this);
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(RichTextBoxAdv));
            }
            RegisterCommands();

            Fonts = new FontCollection();
            foreach (FontFamily font in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                Fonts.Add(font);
            }

            Loaded += (sender, e) =>
                {
                    if (IsContextMenuVisible)
                    {
                        ContextMenu.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ContextMenu.Visibility = Visibility.Collapsed;
                    }
                };
#else
            Fonts = new FontCollection();
            List<FontFamily> fontfamilies = new List<FontFamily>
                                   {
                                       new FontFamily("Arial"),
                                       new FontFamily("Arial Black"),
                                       new FontFamily("Calibri(Body)"),
                                       new FontFamily("Comic Sans MS"),
                                       new FontFamily("Courier New"),
                                       new FontFamily("Georgia"),
                                       new FontFamily("Lucida Sans Unicode"),
                                       new FontFamily("Portable User Interface"),
                                       new FontFamily("Times New Roman"),
                                       new FontFamily("Trebuchet MS"),
                                       new FontFamily("Verdana"),
                                       new FontFamily("Webdings")
                                   };

            foreach (FontFamily font in fontfamilies)
            {
                Fonts.Add(font);
            }
#endif
        }

        private void InitCommands()
        {
            Viewer = new FlowLayoutViewer(this);
            Selection.LayoutViewer = Viewer;
#if !WPF
            BoldCommand = new BoldCommand(this);
            ItalicCommand = new ItalicCommand(this);
            CutCommand = new CutCommand(this);
            CopyCommand = new CopyCommand(this);
            PasteCommand = new PasteCommand(this);
            UnderlineCommand = new UnderlineCommand(this);
            OpenDocumentCommand = new OpenDocumentCommand(this);
            ChangeFontSizeCommand = new ChangeFontSizeCommand(this);
            ChangeFontFamilyCommand = new ChangeFontFamilyCommand(this);
            SingleStrikethroughCommand = new SingleStrikeThroughCommand(this);
            DoubleStrikeThroughCommand = new DoubleStrikeThroughCommand(this);
            ChangeListTypeCommand = new ChangeListTypeCommand(this);
            ChangePageLayoutCommand = new ChangePageLayoutCommand(this);
            SuperscriptCommand = new SuperscriptCommand(this);
            SubscriptCommand = new SubscriptCommand(this);
            SaveDocumentCommand = new SaveDocumentCommand(this);
            ChangeTextColorCommand = new ChangeTextColorCommand(this);
            ChangeHighlightColorCommand = new ChangeHighlightColorCommand(this);
            LineSpacingCommand = new LineSpacingCommand(this);
            TextAlignmentCommand = new TextAlignmentCommand(this);
            BeforeSpacingCommand = new BeforeSpacingCommand(this);
            AfterSpacingCommand = new AfterSpacingCommand(this);
            LeftIndentCommand = new LeftIndentCommand(this);
            RightIndentCommand = new RightIndentCommand(this);
            SaveAsDocumentCommand = new SaveAsDocumentCommand(this);
            ParagraphDialogCommand = new ParagraphDialogCommand(this);
            FontDialogCommand = new FontDialogCommand(this);
            HyperlinkDialogCommand = new HyperlinkDialogCommand(this);
            PrintDocumentCommand = new PrintDocumentCommand(this);
            NewDocumentCommand = new NewDocumentCommand(this);
            InsertRowCommand = new InsertRowCommand(this);
            InsertColumnCommand = new InsertColumnCommand(this);
            DeleteRowCommand = new DeleteRowCommand(this);
            DeleteColumnCommand = new DeleteColumnCommand(this);
            DeleteTableCommand = new DeleteTableCommand(this);
            SelectCellCommand = new SelectCellCommand(this);
            SelectRowCommand = new SelectRowCommand(this);
            SelectColumnCommand = new SelectColumnCommand(this);
            SelectTableCommand = new SelectTableCommand(this);
            MergeSelectedCellsCommand = new MergeSelectedCellsCommand(this);
            InsertTableDialogCommand = new InsertTableDialogCommand(this);
            ChangeTableCellStyleCommand = new ChangeTableCellStyleCommand(this);
            ChangeTableBorderColorCommand = new ChangeTableBorderColorCommand(this);
            InsertPictureCommand = new InsertPictureCommand(this);
            InsertTableCommand = new InsertTableCommand(this);
            UndoCommand = new UndoCommand(this);
            RedoCommand = new RedoCommand(this);
#endif
            History = new History(this);
        }

        #region public fields

        /// <summary>
        /// Gets the current inline style
        /// </summary>
        public InlineStyle CurrentInlineStyle
        {
            get
            {
                if (currentInlineStyle == null)
                {
                    currentInlineStyle = new InlineStyle(this);
                }

                return currentInlineStyle;
            }
        }

        /// <summary>
        /// Gets the current inline style
        /// </summary>
        public ParagraphStyle CurrentParagraphStyle
        {
            get
            {
                if (currentParagraphStyle == null)
                {
                    currentParagraphStyle = new ParagraphStyle(this);
                }

                return currentParagraphStyle;
            }
        }

        #endregion

        #region methods
        /// <summary>
        /// Applies the template
        /// </summary>
        public override void OnApplyTemplate()
        {

#if !WPF

            if (cutItem == null)
                cutItem = GetTemplateChild("PART_Cut") as ContextMenuItemAdv;

            if (copyItem == null)
                copyItem = GetTemplateChild("PART_Copy") as ContextMenuItemAdv;

            if (contextMenu == null)
            {
                contextMenu =(ContextMenuAdv)GetTemplateChild("PART_ContextMenu");
            }

            if (pasteItem == null)
                pasteItem = GetTemplateChild("PART_Paste") as ContextMenuItemAdv;

            if (tableitem != null)
                tableitem = GetTemplateChild("PART_Table") as ContextMenuItemAdv;

            
            if (contextMenu != null)
            {
                contextMenu.Opened += new RoutedEventHandler(contextMenu_Opened);
                contextMenu.Closed += new RoutedEventHandler(contextMenu_Closed);
            }


#endif
            base.OnApplyTemplate();
            if (contentPresenter != null)
                this.contentPresenter.Content = null;
            this.contentPresenter = GetTemplateChild("content") as ContentControl;
#if WPF
            //Handled specifically to avoid focus, on Tab navigation.
            this.contentPresenter.Focusable = false;
#endif
            this.horizontalScrollBar = GetTemplateChild("HorizontalScrollBar") as ScrollBar;
            this.verticalScrollBar = GetTemplateChild("VerticalScrollBar") as ScrollBar;

            if (verticalScrollBar != null && !VerticalScrollBarVisibility)
                verticalScrollBar.Visibility = Visibility.Collapsed;

            Viewer.HorizontalScrollBar = horizontalScrollBar;
            Viewer.VerticalScrollBar = verticalScrollBar;

            if (this.contentPresenter != null && (Viewer.Parent == null || this.contentPresenter.Content == null))
            {
                this.contentPresenter.Content = Viewer;
            }
        }

#if WPF

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenu parent = this.ContextMenu as ContextMenu;
            parent.DataContext = this;
            for (int i = 0; i < parent.Items.Count;i++ )
            {
                MenuItem item = parent.Items[i] as MenuItem;
                Separator separator = parent.Items[i] as Separator;
                if (item != null && !m_contextmenuflag)
                {
                    if (item.Header.ToString() == "Cut")
                    {
                        cutitem = item;
                    }
                    else if (item.Header.ToString() == "Copy")
                    {
                        copyitem = item;
                    }
                    else if (item.Header.ToString() == "Paste")
                    {
                        pasteitem = item;
                    }
                    else if (item.Header.ToString() == "Insert")
                    {
                        insertmenuitem = item;
                    }
                    else if (item.Header.ToString() == "Delete")
                    {
                        deletemenuitem = item;
                    }
                    else if (item.Header.ToString() == "Select")
                    {
                        selectmenuitem = item;
                    }
                    else if (item.Header.ToString() == "Merge Cells")
                    {
                        mergemenuitem = item;
                    }
                    else if (item.Header.ToString() == "Insert Table")
                    {
                        insertTable = item;
                    }
                    else if (item.Header.ToString() == "Font...")
                    {
                        fontitem = item;
                    }
                    else if (item.Header.ToString() == "Paragraph...")
                    {
                        paragraphitem = item;
                    }
                    else if (item.Header.ToString() == "Bullets")
                    {
                        bulletitem = item;
                    }
                    else if (item.Header.ToString() == "Numbering")
                    {
                        numberitem = item;
                    }
                    else if (item.Header.ToString() == "Hyperlink")
                    {
                        hyperlinkitem = item;
                    }
                }
                else if (!m_contextmenuflag && item == null && separator != null)
                {
                    if (parent.Items[i - 1] == pasteitem)
                    {
                        pastesplitter = separator;
                    }
                    else if (parent.Items[i - 1] == mergemenuitem)
                    {
                        mergeseparator = separator;
                    }
                    else if (parent.Items[i - 1] == numberitem)
                    {
                        hyperlinkseparator = separator;
                    }
                }
            }

            m_contextmenuflag = true;

            DisableOnReadOnly();

            if (IsReadOnly)
                return;

            SetVisibility(insertTable);
            SetVisibility(fontitem);
            SetVisibility(paragraphitem);
            SetVisibility(bulletitem);
            SetVisibility(numberitem);
            SetVisibility(mergeseparator);
            SetVisibility(hyperlinkitem);
            SetVisibility(hyperlinkseparator);

            if (Viewer.TextPosition.IsInsideTable)
            {
                SetVisibility(insertmenuitem);
                SetVisibility(deletemenuitem);
                SetVisibility(mergemenuitem);
                SetVisibility(selectmenuitem);
                SetVisibility(mergeseparator);
            }
            else
            {
                CollapseVisibility(insertmenuitem);
                CollapseVisibility(deletemenuitem);
                CollapseVisibility(mergemenuitem);
                CollapseVisibility(selectmenuitem);
                CollapseVisibility(mergeseparator);
            }

            bool canopen = false;

            if (Viewer != null && Viewer.IsSelected)
            {
                if (Selection.Start != null && Selection.End != null)
                {
                    canopen = Selection.Start.GetRootBlock() == Selection.End.GetRootBlock() && Selection.Start.GetRootBlock().IsTable;
                }
            }
            else
            {
                canopen = PositionHandler.Paragraph.IsInsideTable;
            }

            if (canopen)
            {
                if (Viewer.IsSelected)
                {
                    if (Selection.SelectedCellsInTable().Count > 0)
                    {
                        CollapseVisibility(insertTable);
                        CollapseVisibility(fontitem);
                        CollapseVisibility(paragraphitem);
                        CollapseVisibility(bulletitem);
                        CollapseVisibility(numberitem);
                        CollapseVisibility(mergeseparator);
                        CollapseVisibility(hyperlinkseparator);
                        CollapseVisibility(hyperlinkitem);
                    }
                }
            }
        }

        private void DisableOnReadOnly()
        {
            if (IsReadOnly)
            {
                CollapseVisibility(cutitem);
                CollapseVisibility(pasteitem);
                CollapseVisibility(pastesplitter);
                CollapseVisibility(insertmenuitem);
                CollapseVisibility(deletemenuitem);
                CollapseVisibility(selectmenuitem);
                CollapseVisibility(mergemenuitem);
                CollapseVisibility(mergeseparator);
                CollapseVisibility(fontitem);
                CollapseVisibility(paragraphitem);
                CollapseVisibility(insertTable);
                CollapseVisibility(hyperlinkitem);
                CollapseVisibility(hyperlinkseparator);
                CollapseVisibility(bulletitem);
                CollapseVisibility(numberitem);
            }
            else
            {
                SetVisibility(cutitem);
                SetVisibility(pasteitem);
                SetVisibility(pastesplitter);
                SetVisibility(insertmenuitem);
                SetVisibility(deletemenuitem);
                SetVisibility(selectmenuitem);
                SetVisibility(mergemenuitem);
                SetVisibility(mergeseparator);
                SetVisibility(fontitem);
                SetVisibility(paragraphitem);
                SetVisibility(insertTable);
                SetVisibility(hyperlinkitem);
                SetVisibility(hyperlinkseparator);
                SetVisibility(bulletitem);
                SetVisibility(numberitem);
            }
        }

        private void SetVisibility(UIElement item)
        {
            if (item != null)
                item.Visibility = Visibility.Visible;
        }

        private void CollapseVisibility(UIElement item)
        {
            if (item != null)
                item.Visibility = Visibility.Collapsed;
        }

#endif

        void contextMenu_Closed(object sender, RoutedEventArgs e)
        {

#if !WPF

            ContextMenuAdv parent = sender as ContextMenuAdv;

            if (PositionHandler.Paragraph.IsInsideTable)
            {
                if (parent.Items.Count > 0)
                {
                    int i = 0;
                    while (i < parent.Items.Count)
                    {
                        object item = parent.Items[i];
                        if (item is ContextMenuItemAdv)
                        {
                            ContextMenuItemAdv menuitem = item as ContextMenuItemAdv;
                            if (menuitem.Header.ToString() == "Insert" || menuitem.Header.ToString() == "Delete" || menuitem.Header.ToString() == "Select" || menuitem.Header.ToString()=="Merge Cells")
                            {
                                if (menuitem.Header.ToString() == "Insert")
                                {
                                    i = i - 1;
                                    for (int j = 0; j < 2; j++)
                                    {
                                        parent.Items.RemoveAt(i);
                                    }
                                }
                                else
                                {
                                    parent.Items.RemoveAt(i);
                                }
                            }
                            else
                                i++;
                        }
                        else
                            i++;
                    }
                }
            }

            this.Focus();
#endif
        }
        
        /// <summary>
        /// Handles the open event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void contextMenu_Opened(object sender, RoutedEventArgs e)
        {

#if !WPF

            ContextMenuAdv parent = sender as ContextMenuAdv;
            if (parent != null)
            {
                if (parent.Items.Count > 0)
                {
                    if (tableitem == null)
                    {
                        List<ContextMenuItemAdv> list = new List<ContextMenuItemAdv>();

                        foreach (object item in parent.Items)
                        {
                            if (item is ContextMenuItemAdv)
                            {
                                if ((item as ContextMenuItemAdv).Header.ToString() == "Table")
                                {
                                    list.Add(item as ContextMenuItemAdv);
                                    break;
                                }
                            }
                        }

                        if (list.Count > 0)
                        {
                            tableitem = list[0];
                        }
                    }

                    if (m_removeditems.Count > 0)
                    {
                        foreach (int key in m_removeditems.Keys)
                        {
                            if (!parent.Items.Contains(m_removeditems[key]))
                            {
                                parent.Items.Insert(key, m_removeditems[key]);
                            }
                        }
                    }


                    bool canopen = false;

                    if (Viewer != null && Viewer.IsSelected)
                    {
                        if (Selection.Start != null && Selection.End != null)
                        {
                            canopen = Selection.Start.GetRootBlock() == Selection.End.GetRootBlock() && Selection.Start.GetRootBlock().IsTable;
                        }
                    }
                    else
                    {
                        canopen = PositionHandler.Paragraph.IsInsideTable;
                    }

                    if (canopen)
                    {
                        if (Viewer.IsSelected)
                        {
                            if (Selection.SelectedCellsInTable().Count > 0)
                            {
                                int i = 0;
                                SeparatorAdv seperator=null;
                                ContextMenuItemAdv item=null;
                                List<object> itemstoremove = new List<object>();
                                bool started = false;
                                while (i < parent.Items.Count)
                                {
                                    object obj=parent.Items[i];

                                    if (obj != null)
                                    {
                                        if (obj is ContextMenuItemAdv)
                                        {
                                            item = obj as ContextMenuItemAdv;
                                        }
                                        else if (obj is SeparatorAdv)
                                        {
                                            seperator = obj as SeparatorAdv;
                                        }

                                        if (seperator != null)
                                        {
                                            if (started)
                                            {
                                                itemstoremove.Add(seperator);
                                                if (!m_removeditems.ContainsKey(i))
                                                {
                                                    m_removeditems.Add(i, seperator);
                                                }
                                            }
                                            seperator = null;
                                        }
                                        else if (item != null)
                                        {
                                            if (item.Header.ToString() == "Table" || item.Header.ToString() == "Font..." || item.Header.ToString() == "Paragraph..." 
                                                || item.Header.ToString() == "Bullet" || item.Header.ToString() == "Numbering" || item.Header.ToString()=="Hyperlink")
                                            {
                                                if ((item.Header.ToString() == "Table" || item.Header.ToString() == "Font...") && !started)
                                                {
                                                    started = true;
                                                }

                                                if (started)
                                                {
                                                    itemstoremove.Add(item);
                                                    if (!m_removeditems.ContainsKey(i))
                                                    {
                                                        m_removeditems.Add(i, item);
                                                    }
                                                    if (item.Header.ToString() == "Hyperlink")
                                                    {
                                                        started = false;
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    i++;
                                }

                                foreach (object obj in itemstoremove)
                                {
                                    parent.Items.Remove(obj);
                                }
                            }
                        }

                        ContextMenuItemAdv insertmenuitem = null;
                        ContextMenuItemAdv deletemenuitem = null;
                        ContextMenuItemAdv selectmenuitem = null;
                        ContextMenuItemAdv mergecellsmenuitem = null;

                        if (parent.Items.Count >=4)
                        {
                            insertmenuitem = new ContextMenuItemAdv();
                            insertmenuitem.Header = "Insert";
                            AddInsertCommands(insertmenuitem);
                            Add(parent, insertmenuitem, 4);
                            
                            deletemenuitem = new ContextMenuItemAdv();
                            deletemenuitem.Header = "Delete";
                            AddDeleteCommands(deletemenuitem);
                            Add(parent, deletemenuitem, 5);

                            selectmenuitem = new ContextMenuItemAdv();
                            selectmenuitem.Header = "Select";
                            AddSelectCommands(selectmenuitem);
                            Add(parent, selectmenuitem, 6);

                            mergecellsmenuitem = new ContextMenuItemAdv();
                            mergecellsmenuitem.Header = "Merge Cells";
                            mergecellsmenuitem.Command = MergeSelectedCellsCommand;
                            mergecellsmenuitem.Icon = new Image { Source = GenerateBitmapImage("MergeCells.png"),Width=16,Height=16,Margin=new Thickness(3,0,0,0) };
                            Add(parent, mergecellsmenuitem, 7);

                            Add(parent, new SeparatorAdv(), 8);
                        }
                    }
                }
            }
#endif
        }

#if !WPF

        internal void Add(ContextMenuAdv parent, object item,int index)
        {
            if (index < parent.Items.Count)
            {
                parent.Items.Insert(index, item);
            }
            else if (index == parent.Items.Count)
            {
                parent.Items.Add(item);
            }
        }

        internal void AddInsertCommands(ContextMenuItemAdv menuitem)
        {
            menuitem.Items.Add(CreateContextMenuItem("Insert Column to the Left", "InsertColumnLeft.png", InsertColumnCommand, ColumnPlacement.Left));
            menuitem.Items.Add(CreateContextMenuItem("Insert Column To the Right", "InsertColumnRight.png", InsertColumnCommand, ColumnPlacement.Right));
            menuitem.Items.Add(CreateContextMenuItem("Insert Row Above", "InsertRowAbove.png", InsertRowCommand, RowPlacement.Above));
            menuitem.Items.Add(CreateContextMenuItem("Insert Row Below", "InsertRowBelow.png", InsertRowCommand, RowPlacement.Below));
        }

        internal void AddDeleteCommands(ContextMenuItemAdv menuitem)
        {
            menuitem.Items.Add(CreateContextMenuItem("Delete Row", "DeleteRow.png", DeleteRowCommand, null));
            menuitem.Items.Add(CreateContextMenuItem("Delete Column", "DeleteColumn.png", DeleteColumnCommand, null));
            menuitem.Items.Add(CreateContextMenuItem("Delete Table", "DeleteTable.png", DeleteTableCommand, null));
        }

        internal void AddSelectCommands(ContextMenuItemAdv menutitem)
        {
            menutitem.Items.Add(CreateContextMenuItem("Select Cell", "SelectCell.png", SelectCellCommand, null));
            menutitem.Items.Add(CreateContextMenuItem("Select Row", "SelectRow.png", SelectRowCommand, null));
            menutitem.Items.Add(CreateContextMenuItem("Select Column", "SelectColumn.png", SelectColumnCommand, null));
            menutitem.Items.Add(CreateContextMenuItem("Select Table", "SelectTable.png", SelectTableCommand, null));
        }

        internal ContextMenuItemAdv CreateContextMenuItem(string header, object icon, CommandBase icommand, object commandparameter)
        {
            ContextMenuItemAdv subcontextMenuitem = new ContextMenuItemAdv();
            subcontextMenuitem.Header = header;
            subcontextMenuitem.Icon = new Image { Source = GenerateBitmapImage(icon),Width=16,Height=16,Margin=new Thickness(3,0,0,0) };
            subcontextMenuitem.Command = icommand;
            subcontextMenuitem.CommandParameter = commandparameter;
            return subcontextMenuitem;
        }
#endif

        private BitmapImage GenerateBitmapImage(object icon)
        {
            BitmapImage bitmapimage = new BitmapImage();
#if !WPF     
            bitmapimage.UriSource = new System.Uri("/Syncfusion.RichTextBoxAdv.Silverlight;component/Images/" + icon.ToString(), UriKind.RelativeOrAbsolute);
#else
            bitmapimage.BeginInit();
            bitmapimage.UriSource = new Uri("pack://application:,,,/Syncfusion.RichTextBoxAdv.WPF;component/Images/" + icon.ToString(), UriKind.RelativeOrAbsolute);
            bitmapimage.EndInit();
#endif
            return bitmapimage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Document == null)
            {
                Document = new DocumentAdv();
                if (Document.Sections.Count == 0)
                {
                    SectionAdv section = new SectionAdv();
                    Document.Sections.Add(section);
                }
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (CurrentPage != null)
            {
                if (Viewer != null)
                {
                    //this.CurrentPage.ShowCaret();
                    Viewer.CheckForCursorVisibility(false);
                }
            }
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Raised when the control lost focus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (CurrentPage != null)
            {
                if (HideCursorOnLostFocus)
                {
                    this.CurrentPage.HideCaret(false);
                }
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void OnStyleChanged()
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, EventArgs.Empty);
            }
        }

        internal void FirePrinting(RoutedEventArgs args)
        {
            if (Printing != null)
            {
                Printing(this, args);
            }
        }

        internal void FirePrinted()
        {
            if (PrintCompleted != null)
            {
                PrintCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when mouse left button is clicked
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }
#if WPF
        /// <summary>
        /// Raised when mouse button (Down) is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }
#endif
        /// <summary>
        /// Handles the text input event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (Document == null)
            {
                Document = new DocumentAdv();
                SectionAdv section = new SectionAdv();
                Document.Sections.Add(section);
            }
            if (CurrentParagraph == null)
            {
                CurrentParagraph = new ParagraphAdv();
                CurrentParagraph.LayoutViewer = Viewer;
                this.CurrentPage.CurrentParagraph = CurrentParagraph;
                Document.Sections[0].Blocks.Add(CurrentParagraph);
            }
            if (e.Text != "\r" && e.Text != "\b" && !isHandled && !IsReadOnly && Keyboard.Modifiers != ModifierKeys.Control)
            {
                Dispatcher.BeginInvoke(new Action(delegate() 
                    { 
                        this.Viewer.HandleTextInput(e.Text);
                        TextChangedEventArgs args = new TextChangedEventArgs();
                        args.Text = e.Text;
                        FireTextChanged(args);
                    }
                    ));
            }

            isHandled = false;

            base.OnTextInput(e);
        }

        internal void FireTextChanged(TextChangedEventArgs args)
        {
            if (TextChanged != null)
            {
                TextChanged(this, args);
            }
        }

        internal void FireHyperlinkClicked()
        {
            if (HyperlinkClicked != null)
            {
                HyperlinkClicked(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Updates the entire editor layout
        /// </summary>
        public void UpdateEditorLayout()
        {
            if (Viewer != null)
            {
                Viewer.NeedRecreateLayout = true;
                Viewer.InvalidateMeasure();
                Viewer.InvalidateArrange();
                Viewer.UpdateLayout();
            }
        }

        internal ListType ToggleBullet(ListType listType)
        {
            ListType list = CurrentParagraphStyle.ListType;
            if (Viewer.IsSelected && Selection.Start != null && Selection.Start.Paragraph != null)
            {
                list = Selection.Start.Paragraph.ListType;
            }

            listType = list != listType ? listType : Syncfusion.Windows.Tools.Controls.ListType.None;

            return listType;
        }
        
        internal void ChangeContextMenuOnReadOnly()
        {

#if !WPF

            if (IsReadOnly)
            {
                if (ContextMenu.Items.Count > 0)
                {
                    int i = 0;
                    while (i < ContextMenu.Items.Count)
                    {
                        object item = ContextMenu.Items[i];
                        if (item is ContextMenuItemAdv)
                        {
                            if ((item as ContextMenuItemAdv).Header.ToString() == "Copy")
                            {
                                i++;
                                continue;
                            }
                            else
                                ContextMenu.Items.Remove(item);
                        }
                        else
                            ContextMenu.Items.Remove(item);
                    }
                }
                else
                {
                    if (copyItem != null)
                    {
                        ContextMenu.Items.Add(copyItem);
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Updates the entire editor layout using the boolean specified
        /// </summary>
        /// <param name="parseDoc"></param>
        public void UpdateEditorLayout(bool parseDoc)
        {
            if (Viewer != null)
            {
                Viewer.NeedRecreateLayout = parseDoc;
                Viewer.InvalidateMeasure();
                Viewer.InvalidateArrange();
                Viewer.UpdateLayout();
            }
        }
        /// <summary>
        /// Sorts the UI elements to list.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="uiElementList">The UI element list.</param>
        private void SortUIElementsToList(Panel panel, List<UIElement> uiElementList)
        {
            Dictionary<UIElement, int> uiElementDict = new Dictionary<UIElement, int>();
            foreach (UIElement uiElement in panel.Children)
            {
                int zIndex = 0;
#if WPF
                zIndex = Panel.GetZIndex(uiElement);
#else
                if (panel is Canvas)
                    zIndex = Canvas.GetZIndex(uiElement);
#endif
                uiElementDict.Add(uiElement, zIndex);
            }
            List<int> zOrders = uiElementDict.Values.ToList();
            zOrders.Sort();
            //Sorts the UI element based on their Z orders.
            for (int i = 0; i < zOrders.Count; i++)
            {
                int zOrder = zOrders[i];
                List<UIElement> keys = uiElementDict.Keys.ToList();
                for (int j = 0; j < uiElementDict.Count; j++)
                {
                    if (uiElementDict[keys[j]] == zOrder)
                    {
                        uiElementList.Add(keys[j]);
                        uiElementDict.Remove(keys[j]);
                        keys.RemoveAt(j);
                        j--;
                        zOrders.Remove(zOrder);
                    }
                }
                if (!zOrders.Contains(zOrder))
                    i--;
                keys.Clear();
            }
        }
        /// <summary>
        /// Sorts the index of the by tab.
        /// </summary>
        /// <param name="controls">The controls.</param>
        private void SortByTabIndex(List<Control> controls)
        {
            Dictionary<Control, int> controlDict = new Dictionary<Control, int>();
            foreach (Control control in controls)
            {
                controlDict.Add(control, control.TabIndex);
            }
            controls.Clear();
            List<int> tabIndex = controlDict.Values.ToList();
            tabIndex.Sort();
            //Sorts the control based on their tab index.
            for (int i = 0; i < tabIndex.Count; i++)
            {
                int index = tabIndex[i];
                List<Control> keys = controlDict.Keys.ToList();
                for (int j = 0; j < controlDict.Count; j++)
                {
                    if (controlDict[keys[j]] == index)
                    {
                        controls.Add(keys[j]);
                        controlDict.Remove(keys[j]);
                        keys.RemoveAt(j);
                        j--;
                        tabIndex.Remove(index);
                    }
                }
                if (!tabIndex.Contains(index))
                    i--;
                keys.Clear();
            }
        }
        /// <summary>
        /// Gets the adjacent controls.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <returns></returns>
        private List<Control> GetAdjacentControls(Panel panel)
        {
            List<Control> controls = new List<Control>();
            List<UIElement> uiElements = new List<UIElement>();
            //Sorts the panel and controls in the main window.
            SortUIElementsToList(panel, uiElements);
            //Updates all the child controls within the nested panel.
            UpdateControls(uiElements, controls);
            uiElements.Clear();
            //Sorts all the controls based on tab index.
            SortByTabIndex(controls);
            return controls;
        }
        /// <summary>
        /// Updates the controls.
        /// </summary>
        /// <param name="uiElementList">The UI element list.</param>
        /// <param name="controls">The controls.</param>
        private void UpdateControls(List<UIElement> uiElementList, List<Control> controls)
        {
            foreach (UIElement uiElement in uiElementList)
            {
                if (uiElement is Control)
                    controls.Add(uiElement as Control);
                else if (uiElement is Panel)
                {
                    List<UIElement> uiElements = new List<UIElement>();
                    SortUIElementsToList(uiElement as Panel, uiElements);
                    //Updates all the child controls within the nested panel.
                    UpdateControls(uiElements, controls);
                    uiElements.Clear();
                }
            }
        }
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns></returns>
        private Panel GetParent()
        {
            Panel panel = this.Parent as Panel;
            while (panel.Parent is Panel)
            {
                panel = panel.Parent as Panel;
            }
            return panel;
        }
        /// <summary>
        /// Navigates to adjacent control.
        /// </summary>
        private void NavigateToAdjacentControl()
        {
            if (Viewer != null)
            {
                if (Viewer.isPageActive)
                {
                    Viewer.isPageActive = false;
                    if (CurrentPage != null)
                    {
                        if (HideCursorOnLostFocus)
                            CurrentPage.HideCaret(false);
                        if (this.Parent is Panel)
                        {
                            //Gets the parent panel.
                            Panel parentPanel = GetParent();
                            //Gets the adjacent controls in the main window.
                            List<Control> controls = GetAdjacentControls(parentPanel);
                            int index = controls.IndexOf(this);
                            if (index == -1)
                                this.Focus();
                            else if (Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                //Reverse navigation - Moves the focus to previous control.
                                if (index == 0)
                                    controls[controls.Count - 1].Focus();
                                else
                                    controls[index - 1].Focus();
                            }
                            else
                            {
                                //Moves the focus to next control.
                                if (index == controls.Count - 1)
                                    controls[0].Focus();
                                else
                                    controls[index + 1].Focus();
                            }
                            controls.Clear();
                        }
                    }
                }
                else
                {
                    Viewer.isPageActive = true;
                    Viewer.CheckForCursorVisibility(false);
                }
            }
        }
        /// <summary>
        /// Raised when a key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsTabStop && e.Key == Key.Tab)
            {
                NavigateToAdjacentControl();
                e.Handled = true;
                base.OnKeyDown(e);
                return;
            }
            bool isModifierKeyPressed = Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift || Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift);
            bool clearRedo = false;

            if (IsReadOnly && !(Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.C || e.Key == Key.A)))
                return;

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.PageUp)
            {
                e.Handled = true;
                Viewer.HandlePageUp();
            }
            else if (!isModifierKeyPressed && e.Key == Key.PageDown)
            {
                e.Handled = true;
                Viewer.HandlePageDown();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.PageUp)
            {
                e.Handled = true;
                Viewer.HandleControlPageUp();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.PageDown)
            {
                e.Handled = true;
                Viewer.HandleControlPageDown();
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.Tab)
            {
                e.Handled = true;
                if (PositionHandler.TextPosition.IsInsideTable
                    && Keyboard.Modifiers != ModifierKeys.Control)
                {
                    Viewer.HandleTabKey();
                }
                else
                {
                    Viewer.HandleTextInput("\t");
                }

                clearRedo = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Tab)
            {
                e.Handled = true;
                if (PositionHandler.TextPosition.IsInsideTable)
                {
                    Viewer.HandleShiftTabKey();
                }
                else
                {
                    Viewer.HandleTextInput("\t");
                }

                clearRedo = true;
            }
            if (!isModifierKeyPressed && e.Key == Key.Enter)
            {
                Viewer.HandleEnterKey();
                e.Handled = true;
                clearRedo = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Back)
            {
                Viewer.HandleBackKey();
                clearRedo = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Left)
            {
                Viewer.HandleLeftKey();
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Right)
            {
                Viewer.HandleRightKey();
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Delete)
            {
                Viewer.HandleDeleteKey();
                clearRedo = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Up)
            {
                Viewer.HandleUpKey();
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Down)
            {
                Viewer.HandleDownKey();
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.Home)
            {
                Viewer.HandleHomeKey();
                e.Handled = true;
            }
            else if (!isModifierKeyPressed && e.Key == Key.End)
            {
                Viewer.HandleEndKey();
                e.Handled = true;
            }
#if !WPF
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                e.Handled = true;
                Selection.Copy();
                isHandled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                e.Handled = true;
                Selection.Paste();
                isHandled = true;
                clearRedo = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X)
            {
                e.Handled = true;
                Selection.Cut();
                isHandled = true;
                clearRedo = true;
            }
#endif
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                e.Handled = true;
                Selection.SelectAll();
                isHandled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Home)
            {
                e.Handled = true;
                Viewer.HandleControlHomeKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            {
                e.Handled = true;
                Viewer.HandleControlDownKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Up)
            {
                e.Handled = true;
                Viewer.HandleControlUpKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.End)
            {
                e.Handled = true;
                Viewer.HandleControlEndKey();
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Left)
            {
                e.Handled = true;
                Viewer.HandleCtrlShiftLeft();
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Right)
            {
                e.Handled = true;
                Viewer.HandleCtrlShiftRight();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Up)
            {
                e.Handled = true;
                Viewer.HandleShiftUpArrow();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Down)
            {
                e.Handled = true;
                Viewer.HandleShiftDownArrow();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Left)
            {
                e.Handled = true;
                Viewer.HandleShiftLeftArrow();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Right)
            {
                e.Handled = true;
                Viewer.HandleShiftRightKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Home)
            {
                e.Handled = true;
                Viewer.HandleShiftHomeKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.End)
            {
                e.Handled = true;
                Viewer.HandleShiftEndKey();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Insert)
            {
                e.Handled = true;
                Selection.Paste();
                clearRedo = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Left)
            {
                e.Handled = true;
                Viewer.HandleControlLeft();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Right)
            {
                e.Handled = true;
                Viewer.HandleControlRight();
            }
#if !WPF
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                History.Undo();
                isHandled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
            {
                e.Handled = true;
                History.Redo();
                isHandled = true;
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.S)
            {
                e.Handled = true;
                SaveDocument(string.Empty);
            }
#endif
            if (clearRedo)
                History.ClearRedo();
#if !WPF
            CanExecuteCommands();
#endif
            base.OnKeyDown(e);
        }

        public void ResetZooming()
        {
            if (Viewer != null)
            {
                Viewer.ResetZooming();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHandled = false;
        }

        #region Export Functions

        public void ExportToHTML(Stream htmlStream)
        {
            HTMLExporting.ConvertToHtml(this.Document, htmlStream);
        }

        public void ExportToText(Stream textStream)
        {
            TextExporting.ConvertToText(this.Document, textStream);
        }

        public void ExportToXAML(Stream xamlStream)
        {
            XAMLExporting.ConvertToXAML(this.Document, xamlStream);
        }
        #endregion

        #region Import functions

        public void ImportHTML(Stream htmlStream)
        {
            DocumentAdv document = HTMLImporting.ConvertToDocumentAdv(htmlStream);
            if (document != null)
            {
                DocumentLoaded = true;
                Document = document;
            }
        }

        public void ImportXAML(Stream xamlStream)
        {
            DocumentAdv document = XAMLImporting.ConvertToDocumentAdv(xamlStream);
            if (document != null)
            {
                DocumentLoaded = true;
                Document = document;
            }
        }

        public void ImportText(Stream textStream)
        {
            DocumentAdv document = new TextImporting().ConvertToDocumentAdv(textStream);
            if (document != null)
            {
                DocumentLoaded = true;
                Document = document;
            }
        }

        #endregion

        #region TextFormatting Commands
        public void ChangeSingleStrikeThrough()
        {
            if (IsReadOnly)
                return;
            Selection.ChangeSingleStrikeThrough();
        }

        public bool CanMerge()
        {
            if (IsReadOnly)
                return false;

            return Selection.CanMerge();
        }

#if !WPF
        internal void CanExecuteCommands()
        {
            CutCommand.ExecuteChanged();
            CopyCommand.ExecuteChanged();
            InsertTableCommand.ExecuteChanged();
            InsertTableDialogCommand.ExecuteChanged();
            MergeSelectedCellsCommand.ExecuteChanged();
            ChangePageLayoutCommand.ExecuteChanged();
            CanExecuteInsertDeleteTableCommands();
        }

        internal void CanExecuteInsertDeleteTableCommands()
        {
            DeleteColumnCommand.ExecuteChanged();
            DeleteRowCommand.ExecuteChanged();
            DeleteTableCommand.ExecuteChanged();
            InsertRowCommand.ExecuteChanged();
            InsertColumnCommand.ExecuteChanged();
        }

        internal void CanExecuteSelectCommands()
        {
            SelectCellCommand.ExecuteChanged();
            SelectColumnCommand.ExecuteChanged();
            SelectRowCommand.ExecuteChanged();
            SelectTableCommand.ExecuteChanged();
        }
#endif

        public void PrintDocument()
        {
            if (Viewer != null)
            {
                Viewer.PrintDocument();
            }
        }

        public void ChangeTextAlignment(TextAlignment alignement)
        {
            if (IsReadOnly)
                return;
            Selection.ChangeTextAlignment(alignement);
        }

        public void ChangeDoubleStrikeThrough()
        {
            if (IsReadOnly)
                return;
            Selection.ChangeDoubleStrikeThrough();
        }

        public void ChangeSuperscript()
        {
            if (IsReadOnly)
                return;
            Selection.ChangeSuperscript();
        }

        public void ChangeSubscript()
        {
            if (IsReadOnly)
                return;
            Selection.ChangeSubscript();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listType"></param>
        public void ChangeListType(ListType listType)
        {
            if (IsReadOnly)
                return;

            Selection.ChangeListType(listType);
        }

        /// <summary>
        /// Inserts the inline into currrent text position
        /// </summary>
        /// <param name="inline"></param>
        public void InsertInlineInParagraph(Inline inline)
        {
            if (Viewer != null)
            {
                Viewer.InsertInline(inline);
            }
        }

        public void InsertTableInBlocks(int RowCount, int ColumnCount)
        {
            if (Viewer != null)
            {
                Viewer.InsertTable(RowCount, ColumnCount);
            }
        }

        public void InsertRowInTable(RowPlacement rowplace)
        {
            if (Viewer != null)
            {
                Viewer.InsertRow(rowplace);
            }
        }

        public void InsertColumnInTable(ColumnPlacement columnplace)
        {
            if (Viewer != null)
            {
                Viewer.InsertColumn(columnplace);
            }
        }

        public void DeleteTableFromBlocks()
        {
            if (Viewer != null)
            {
                Viewer.DeleteTable();
            }
        }

        public void DeleteRowFromTable()
        {
            if (Viewer != null)
            {
                Viewer.DeleteRow();
            }
        }

        public void DeleteColumnFromTable()
        {
            if (Viewer != null)
            {
                Viewer.DeleteColumn();
            }
        }

        public void SelectCellInTable()
        {
            if (Viewer != null)
            {
                Viewer.SelectCell();
            }
        }

        public void SelectRowInTable()
        {
            if (Viewer != null)
            {
                Viewer.SelectRow();
            }
        }

        public void SelectColumnInTable()
        {
            if (Viewer != null)
            {
                Viewer.SelectColumn();
            }
        }

        public void SelectTableInBlocks()
        {
            if (Viewer != null)
            {
                Viewer.SelectTable();
            }
        }

        public void MergeSelectedCellsInTable()
        {
            if (Viewer != null)
            {
                Viewer.MergeSelectedCells();
            }
        }

        public void ChangeTableCellBackground(Color color)
        {
            if (Viewer != null)
            {
                Viewer.ChangeTableCellBackground(color);
            }
        }

        public void ChangeTableBorderColor(Color color)
        {
            if (Viewer != null)
            {
                Viewer.ChangeTableBorderColor(color);
            }
        }

        /// <summary>
        /// Creates the empty document
        /// </summary>
        public void CreateEmptyDocument()
        {
            if (Document != null)
            {
                Document.ClearLines();
                Document = null;
                History.ClearHistory();
            }
            if (Viewer != null)
            {
                Viewer.CreateBlockOnEmpty();
                PositionHandler = new DocumentPositionHandler(Document);
                Viewer.IsSelected = false;
                UpdateEditorLayout();
            }
        }

        /// <summary>
        /// Changes the page layout
        /// </summary>
        /// <param name="layout"></param>
        public void ChangePageLayout(PageLayout layout)
        {
            if (PageLayout != layout)
            {
                PageLayout = layout;
                this.Focus();
            }
        }
        #endregion

        #region Open Document

        public void OpenDocument()
        {
            Stream documentStream = null;
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = "All supported files (*.docx,*.doc,*.html,*.xaml,*.txt)|*.docx;*.doc;*.html;*.xaml;*.txt | Word Document 2007-2010 (*.docx)|*.docx| Word Document (*.doc)|*.doc|Web Page (*.html)|*.html|XAML File (*.xaml)|*.xaml|Text File (*.txt)|*.txt",
                FilterIndex = 1,
                Multiselect = false
            };
            string fileName = string.Empty, fileExtension = string.Empty;
            try
            {
                if ((bool)openDialog.ShowDialog())
                {
#if WPF
                    documentStream = openDialog.OpenFile();
                    FileInfo file = new FileInfo(openDialog.FileName);
                    fileName = file.Name;
                    DocumentTitle = fileName.Remove(fileName.LastIndexOf("."));
                    fileExtension = file.Extension;
                    FileOpeningEventArgs args = new FileOpeningEventArgs { DocumentStream = documentStream, FormatType = fileExtension };
                    if (this.FileOpening != null)
                        this.FileOpening(this, args);

                    if (!args.Handled && fileExtension != string.Empty && documentStream != null)
                    {
                        switch (fileExtension)
                        {
                            case ".html":
                                this.ImportHTML(documentStream);
                                break;
                            case ".xaml":
                                this.ImportXAML(documentStream);
                                break;
                            case ".txt":
                                this.ImportText(documentStream);
                                break;
                            default:
                                break;
                        }
                    }
#else
                    FileInfo file = openDialog.File;
                    documentStream = file.OpenRead();
                    StreamReader textStream = file.OpenText();
                    fileName = file.Name;
                    DocumentTitle = fileName.Remove(fileName.LastIndexOf("."));
                    fileExtension = file.Extension;
                    FileOpeningEventArgs args = new FileOpeningEventArgs();
                    if (fileExtension != ".txt")
                        args.DocumentStream = documentStream;
                    else
                        args.DocumentStream = textStream.BaseStream;
                    args.FormatType = fileExtension;
                    if (this.FileOpening != null)
                        this.FileOpening(this, args);
                    if (!args.Handled && fileExtension != string.Empty && documentStream != null
                       && textStream != null && textStream.BaseStream != null)
                    {
                        switch (fileExtension)
                        {
                            case ".html":
                                this.ImportHTML(documentStream);
                                break;
                            case ".xaml":
                                this.ImportXAML(documentStream);
                                break;
                            case ".txt":
                                this.ImportText(textStream.BaseStream);
                                break;
                            default:
                                break;
                        }
                    }
#endif
                }
            }
            catch (Exception exception)
            {
                //Removes the existing contents and creates an empty document.
                if (fileExtension.StartsWith(".doc"))
                    Document = null;
                if (OpenFailed != null)
                {
                    OpenFailedEventArgs args = new OpenFailedEventArgs();
                    args.Exception = exception;
                    if (fileName != string.Empty)
                        fileName += " ";
                    args.Message = "Opening the " + fileName + "file failed!";
                    OpenFailed(this, args);
                }
            }
        }

        #endregion

        public void SaveDocument(string FileExtension)
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Word Document 2007-2010(*.docx)|*.docx| Word Document (*.doc)|*.doc| Web Page (*.html)|*.html|XAML File (*.xaml)|*.xaml|Text File (*.txt)|*.txt",
                FilterIndex = ChooseFilterindex(FileExtension)
            };
            try
            {
                if ((bool)saveDialog.ShowDialog())
                {
                    Stream stream = saveDialog.OpenFile();
                    int index = saveDialog.FilterIndex;
                    string fileExtension = saveDialog.SafeFileName.Substring(saveDialog.SafeFileName.IndexOf('.'));
                    FileSavingEventArgs args = new FileSavingEventArgs { DoucmentStream = stream, FormatType = fileExtension };
                    if (this.FileSaving != null)
                        this.FileSaving(this, args);
                    if (!args.Handled)
                    {
                        if (fileExtension != string.Empty)
                        {
                            switch (fileExtension)
                            {
                                case ".html":
                                    saveDialog.Filter = "Web Page (*.html)|*.html";
                                    this.ExportToHTML(stream);
                                    break;
                                case ".xaml":
                                    saveDialog.Filter = "XAML File (*.xaml)|*.xaml";
                                    this.ExportToXAML(stream);
                                    break;
                                case ".txt":
                                    saveDialog.Filter = "Text File (*.txt)|*.txt";
                                    this.ExportToText(stream);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                if (SaveFailed != null)
                {
                    SaveFailedEventArgs args = new SaveFailedEventArgs();
                    args.Exception = exception;
                    args.Message = "Saving the file failed!";
                    SaveFailed(this, args);
                }
            }
        }
        #endregion

        private int ChooseFilterindex(string ext)
        {
            switch (ext)
            {
                case ".docx":
                    return 1;
                case ".doc":
                    return 2;
                case ".html":
                    return 3;
                case ".xaml":
                    return 4;
                case ".txt":
                    return 5;
                default:
                    break;
            }
            return 1;
        }

        internal void FireSelectionChanged(SelectionChangedEventArgs args)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, args); 
            }
        }

#if !WPF
        public void OnStyleChanged(Windows.Controls.Theming.VisualStyle visualStyle)
        {
            VisualStyle = visualStyle;
            SkinManager.SetVisualStyle(ContextMenu, visualStyle);
        }
#endif

        public void Dispose()
        {
            if (Viewer != null)
            {
                Viewer.RemoveViewer();
            }
        }

        public void OnStyleChanged(string visualStyle)
        {
            m_contextmenuflag = false;
        }

#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            double scale = e.DeltaManipulation.Scale.X;
            if (ZoomFactor <= 1 && scale > 1)
            {
                ZoomFactor += 0.01;
            }
            else if (ZoomFactor >= 0.05 && scale < 1)
            {
                ZoomFactor -= 0.01;
            }
            base.OnManipulationDelta(e);
        }
#endif

    }


    /// <summary>
    /// Specifies the page layout
    /// </summary>
    public enum PageLayout
    {
        /// <summary>
        /// Content will be displayed in pages
        /// </summary>
        Pages,
        /// <summary>
        /// Content will be displyaed in a continuous page.
        /// </summary>
        Continuous
    }

    /// <summary>
    /// Specifies single or double strike through
    /// </summary>
    public enum StrikeThrough
    {
        /// <summary>
        /// No strike will be drawn
        /// </summary>
        None,
        /// <summary>
        /// Draws single strike
        /// </summary>
        SingleStrike,
        /// <summary>
        /// Draws double strike
        /// </summary>
        DoubleStrike
    }
}
