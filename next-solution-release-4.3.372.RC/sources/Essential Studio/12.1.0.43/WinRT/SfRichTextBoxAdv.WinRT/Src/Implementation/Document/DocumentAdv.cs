#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
#if WPF
using System.Windows.Markup;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Sections")]
#else
    [ContentProperty(Name = "Sections")]
#endif
    public class DocumentAdv : CompositeNode
    {
        #region Fields
        private SfRichTextBoxAdv ownerControl;
        Dictionary<ListLevelAdv, int> renderedListLevels;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the owner control.
        /// </summary>
        /// <value>
        /// The owner control.
        /// </value>
        public SfRichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
            internal set
            {
                ownerControl = value;
                LayoutItems();
            }
        }
        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public SectionAdvCollection Sections
        {
            get
            {
                return ChildNodes as SectionAdvCollection;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        internal bool IsEmpty
        {
            get
            {
                if (Sections.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (SectionAdv section in Sections)
                    {
                        if (section.Blocks.Count > 0)
                            return false;
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// Gets the document start.
        /// </summary>
        /// <value>
        /// The document start.
        /// </value>
        public TextPosition DocumentStart
        {
            get
            {
                return GetDocumentStart();
            }
        }
        /// <summary>
        /// Gets the document end.
        /// </summary>
        /// <value>
        /// The document end.
        /// </value>
        public TextPosition DocumentEnd
        {
            get
            {
                return GetDocumentEnd();
            }
        }
        /// <summary>
        /// Gets or sets the paragraph format for the current document.
        /// </summary>
        /// <value>
        /// The paragraph format.
        /// </value>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                return (ParagraphFormat)GetValue(ParagraphFormatProperty);
            }
            set
            {
                if (value != null)
                    SetValue(ParagraphFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the character format for the current document.
        /// </summary>
        /// <value>
        /// The character format.
        /// </value>
        public CharacterFormat CharacterFormat
        {
            get
            {
                return (CharacterFormat)GetValue(CharacterFormatProperty);
            }
            set
            {
                if (value != null)
                    SetValue(CharacterFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the background color for the current document.
        /// </summary>
        /// <value>
        /// The background color.
        /// </value>
        internal Color BackgroundColor
        {
            get
            {
                return (Color)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }
        /// <summary>
        /// Gets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        internal Color Background
        {
            get
            {
                Color background = BackgroundColor;
                background = background == Color.FromArgb(0, 0, 0, 0) ? Colors.White : background;
                return background;
            }
        }
        /// <summary>
        /// Gets the lists. Inherits the list formattings from a abstract list, and additionally defines some properties to be override.
        /// </summary>
        /// <value>
        /// The lists.
        /// </value>
        internal List<ListAdv> Lists
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the abstract lists.
        /// </summary>
        /// <value>
        /// The abstract lists.
        /// </value>
        internal List<AbstractListAdv> AbstractLists
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the default width of the tab.
        /// </summary>
        /// <value>
        /// The default width of the tab.
        /// </value>
        public double DefaultTabWidth
        {
            get
            {
                return (double)GetValue(DefaultTabWidthProperty);
            }
            set
            {
                SetValue(DefaultTabWidthProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the ParagraphFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the ParagraphFormat dependency property.</returns>
        internal static readonly DependencyProperty ParagraphFormatProperty = DependencyProperty.Register("ParagraphFormat", typeof(ParagraphFormat), typeof(DocumentAdv), new PropertyMetadata(null, OnParagraphFormatChanged));
        /// <summary>
        /// Identifies the CharacterFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the CharacterFormat dependency property.</returns>
        internal static readonly DependencyProperty CharacterFormatProperty = DependencyProperty.Register("CharacterFormat", typeof(CharacterFormat), typeof(DocumentAdv), new PropertyMetadata(null, OnCharacterFormatChanged));
        /// <summary>
        /// Identifies the BackgroundColor dependency property.
        /// </summary>
        /// <returns>The identifier of the BackgroundColor dependency property.</returns>
        internal static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(DocumentAdv), new PropertyMetadata(Colors.White, OnBackgroundColorChanged));
        /// <summary>
        /// Identifies the Default tab width dependency property.
        /// </summary>
        /// <returns>The identifier of the Default tab width dependency property.</returns>
        public static readonly DependencyProperty DefaultTabWidthProperty = DependencyProperty.Register("DefaultTabWidth", typeof(double), typeof(DocumentAdv), new PropertyMetadata(48d, OnDefaultTabWidthChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when paragraph format changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnParagraphFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as ParagraphFormat).SetOwner(d as DocumentAdv);
        }
        /// <summary>
        /// Called when character format changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCharacterFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as CharacterFormat).SetOwner(d as DocumentAdv);
        }
        /// <summary>
        /// Called when background color changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Color background = (Color)e.NewValue;
            SfRichTextBoxAdv richTextBoxAdv = (d as DocumentAdv).OwnerControl;
            if (background != null && richTextBoxAdv != null && richTextBoxAdv.Viewer != null
                && richTextBoxAdv.LayoutType != LayoutType.Block && richTextBoxAdv.IsDocumentLoaded)
                richTextBoxAdv.Viewer.UpdatePageBackground((d as DocumentAdv).Background);
        }
        /// <summary>
        /// Called when [default tab width changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDefaultTabWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // To do implementaions
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAdv" /> class.
        /// </summary>
        public DocumentAdv()
            : base(null)
        {
            CharacterFormat = new CharacterFormat(this);
            ParagraphFormat = new ParagraphFormat(this);
            ChildNodes = new SectionAdvCollection(this);
            Lists = new List<ListAdv>();
            AbstractLists = new List<AbstractListAdv>();
            renderedListLevels = new Dictionary<ListLevelAdv, int>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="block">The block.</param>
        internal void UpdateListItems(BlockAdv block)
        {
            //Clears previous rendered list levels
            renderedListLevels.Clear();
            for (int i = 0; i < Sections.Count; i++)
            {
                bool isListUpdated = Sections[i].UpdateListItems(block);
                if (isListUpdated)
                    break;
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        internal void LayoutItems()
        {
            if (OwnerControl != null && OwnerControl.IsLayoutEnabled)
            {
#if DEBUG
                if (OwnerControl.IsDocumentLoaded)
                    OwnerControl.PerformanceInfo.RenderingStartTime = DateTime.Now;
#endif
                ClearWidgets();
                OwnerControl.Viewer.ClearContainer();
                for (int i = 0; i < Sections.Count; i++)
                {
                    Sections[i].LayoutItems();
                }
                OwnerControl.Viewer.UpdateScrollBars();
#if DEBUG
                if (OwnerControl.IsDocumentLoaded)
                    OwnerControl.PerformanceInfo.RTERenderingTime = DateTime.Now - OwnerControl.PerformanceInfo.RenderingStartTime;
#endif
            }
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal void ClearWidgets()
        {
            //Clears previous rendered list levels
            renderedListLevels.Clear();
            //Clear previous rendered widgets.
            for (int i = 0; i < Sections.Count; i++)
            {
                Sections[i].ClearWidgets();
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (OwnerControl != null && OwnerControl.Document != null)
                OwnerControl.ClearValue(SfRichTextBoxAdv.DocumentProperty);
            ownerControl = null;
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            ParagraphFormat.Dispose();
            ClearValue(ParagraphFormatProperty);
            ClearValue(BackgroundColorProperty);
            for (int i = 0; i < Sections.Count; i++)
            {
                SectionAdv section = Sections[i];
                section.Dispose();
                Sections.Remove(section);
                i--;
            }
            if (renderedListLevels != null)
            {
                renderedListLevels.Clear();
                renderedListLevels = null;
            }
            for (int i = 0; i < Lists.Count; i++)
            {
                ListAdv list = Lists[i];
                list.Dispose();
                Lists.Remove(list);
                i--;
            }
            Lists = null;
            for (int i = 0; i < AbstractLists.Count; i++)
            {
                AbstractListAdv abstractList = AbstractLists[i];
                abstractList.Dispose();
                AbstractLists.Remove(abstractList);
                i--;
            }
            AbstractLists = null;
        }
        /// <summary>
        /// Gets the built in list.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <returns></returns>
        internal ListAdv GetOrCreateList(string listName)
        {
            if (Lists == null)
                Lists = new List<ListAdv>();
            ListAdv list = GetListAdv(listName);
            if (list == null)
            {
                list = new ListAdv(this);
                list.Name = listName;
                list.AbstractList = new AbstractListAdv(this);
                ListLevelAdv listLevel = list.AbstractList.AddListLevel();
                if (listName.StartsWith("_Bullet"))
                {
                    list.AbstractList.ListType = ListType.Bullet;
                    listLevel.ListLevelPattern = ListLevelPattern.Bullet;
                    listLevel.BulletCharacter = listName.EndsWith("Dot") ? ListLevelAdv.DOTBULLET : listName.EndsWith("Square") ? ListLevelAdv.SQUAREBULLET : ListLevelAdv.ARROWBULLET;
                    listLevel.CharacterFormat.FontFamily = listName.EndsWith("Square") ? new FontFamily("Wingdings") : new FontFamily("Symbol");
                }
                else
                {
                    list.AbstractList.ListType = ListType.Numbering;
                    listLevel.ListLevelPattern = listName.EndsWith("LowerRoman") ? ListLevelPattern.LowRoman
                        : listName.EndsWith("LowerLetter") ? ListLevelPattern.LowLetter
                        : listName.EndsWith("UpperLetter") ? ListLevelPattern.UpLetter : ListLevelPattern.Arabic;
                    listLevel.StartAt = 1;
                }
                //listLevel.CharacterFormat.FontSize = 14.667;
                Lists.Add(list);
            }
            return list;
        }
        /// <summary>
        /// Gets the list adv.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <returns></returns>
        internal ListAdv GetListAdv(string listName)
        {
            if (Lists == null)
                return null;
            foreach (ListAdv list in Lists)
            {
                if (list.Name == listName)
                    return list;
            }
            return null;
        }
        /// <summary>
        /// Gets the list number.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        internal string GetListNumber(ListLevelAdv listLevel)
        {
            if (renderedListLevels.ContainsKey(listLevel))
                renderedListLevels[listLevel]++;
            else
                renderedListLevels.Add(listLevel, listLevel.StartAt);
            return listLevel.GetListText(renderedListLevels[listLevel]);
        }
        /// <summary>
        /// Gets the text position of document start.
        /// </summary>
        /// <returns></returns>
        internal TextPosition GetDocumentStart()
        {
            if (Sections.Count == 0 || Sections[0].Blocks.Count == 0)
                return null;
            if (OwnerControl != null && !OwnerControl.IsDocumentLoaded)
            {
                LineWidget lineWidget = OwnerControl.Viewer.GetLineWidget(new Point(0, 0));
                if (lineWidget == null)
                    return null;
            }
            TextPosition textPosition = null;
            BlockAdv block = null;
            if (Sections[0].Blocks[0] is TableAdv)
                block = (Sections[0].Blocks[0] as TableAdv).GetFirstParagraphInFirstCell();
            else if (Sections[0].Blocks[0] is ParagraphAdv)
                block = Sections[0].Blocks[0] as ParagraphAdv;
            if (block is ParagraphAdv)
            {
                textPosition = new TextPosition(OwnerControl);
                textPosition.SetPosition(block as ParagraphAdv, true);
            }
            return textPosition;
        }
        /// <summary>
        /// Gets the text position of document end.
        /// </summary>
        /// <returns></returns>
        internal TextPosition GetDocumentEnd()
        {
            TextPosition textPosition = null;
            TextPosition documentStart = DocumentStart;
            if (documentStart != null && Sections.Count > 0)
            {
                BlockAdv block = Sections.Last().Blocks.Last();
                BlockAdv prevBlock = block;
                if (!OwnerControl.IsDocumentLoaded)
                {
                    if (prevBlock != null)
                        prevBlock = block.PreviousBlock;
                    if (prevBlock == null && Sections.Count > 1)
                        prevBlock = Sections[Sections.Count - 2].Blocks.Last();
                }
                if (prevBlock != null)
                    block = prevBlock;
                if (block is TableAdv)
                    block = (block as TableAdv).GetLastParagraphInLastCell();
                if (block is ParagraphAdv)
                {
                    double offset = prevBlock == null ? 0 : (block as ParagraphAdv).GetEndOffset();
                    textPosition = new TextPosition(OwnerControl);
                    textPosition.SetPosition(block as ParagraphAdv, offset);
                }
                else
                {
                    textPosition = new TextPosition(OwnerControl);
                    textPosition.SetPosition(documentStart.Paragraph, documentStart.Paragraph.GetEndOffset());
                }
            }
            return textPosition;
        }
        #endregion
    }
}
