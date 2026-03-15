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
#if WPF
using System.Windows;
using System.Windows.Markup;
#else
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public sealed class HeaderFooters : CompositeNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (Section == null)
                    return null;
                return Section.Document;
            }
        }
        /// <summary>
        /// Gets/sets the Section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        internal SectionAdv Section
        {
            get
            {
                return Owner as SectionAdv;
            }
        }
        /// <summary>
        /// Gets or sets the odd page / default header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public HeaderFooter Header
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.OddHeader];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.OddHeader] = value;
            }
        }
        /// <summary>
        /// Gets or sets the odd page / default footer.
        /// </summary>
        /// <value>
        /// The footer.
        /// </value>
        public HeaderFooter Footer
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.OddFooter];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.OddFooter] = value;
            }
        }
        /// <summary>
        /// Gets or sets the even page header.
        /// </summary>
        /// <value>
        /// The even header.
        /// </value>
        public HeaderFooter EvenHeader
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.EvenHeader];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.EvenHeader] = value;
            }
        }
        /// <summary>
        /// Gets or sets the even page footer.
        /// </summary>
        /// <value>
        /// The even footer.
        /// </value>
        public HeaderFooter EvenFooter
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.EvenFooter];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.EvenFooter] = value;
            }
        }
        /// <summary>
        /// Gets or sets the first page header.
        /// </summary>
        /// <value>
        /// The first page header.
        /// </value>
        public HeaderFooter FirstPageHeader
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.FirstPageHeader];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.FirstPageHeader] = value;
            }
        }
        /// <summary>
        /// Gets or sets the first page footer.
        /// </summary>
        /// <value>
        /// The first page footer.
        /// </value>
        public HeaderFooter FirstPageFooter
        {
            get
            {
                return (HeaderFooter)ChildNodes[(int)HeaderFooterType.FirstPageFooter];
            }
            set
            {
                ChildNodes[(int)HeaderFooterType.FirstPageFooter] = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooters"/> class.
        /// </summary>
        public HeaderFooters()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooters"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal HeaderFooters(Node owner)
            : base(owner)
        {
            ChildNodes = new HeaderFooterCollection(this);
            HeaderFooter hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.EvenHeader;
            ChildNodes.Add(hf);
            hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.OddHeader;
            ChildNodes.Add(hf);
            hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.EvenFooter;
            ChildNodes.Add(hf);
            hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.OddFooter;
            ChildNodes.Add(hf);
            hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.FirstPageHeader;
            ChildNodes.Add(hf);
            hf = new HeaderFooter(this);
            hf.Type = HeaderFooterType.FirstPageFooter;
            ChildNodes.Add(hf);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones the items of this instance to the headerfooters.
        /// </summary>
        /// <param name="headerfooter">The headerfooter.</param>
        internal void CloneItemsTo(HeaderFooters headerfooters)
        {
            FirstPageHeader.CloneItemsTo(headerfooters.FirstPageHeader);
            FirstPageFooter.CloneItemsTo(headerfooters.FirstPageFooter);
            Header.CloneItemsTo(headerfooters.Header);
            Footer.CloneItemsTo(headerfooters.Footer);
            EvenHeader.CloneItemsTo(headerfooters.EvenHeader);
            EvenFooter.CloneItemsTo(headerfooters.EvenFooter);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                HeaderFooter headerFooter = ChildNodes[i] as HeaderFooter;
                headerFooter.Dispose();
                ChildNodes.Remove(headerFooter);
                i--;
            }
        }
        #endregion
    }
#if WPF
    [ContentProperty("Blocks")]
#else
    [ContentProperty(Name = "Blocks")]
#endif
    public sealed class HeaderFooter : CompositeNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (Section == null)
                    return null;
                return Section.Document;
            }
        }
        /// <summary>
        /// Gets/sets the Section
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        internal SectionAdv Section
        {
            get
            {
                if (Owner is HeaderFooters)
                    return (Owner as HeaderFooters).Section;
                return null;
            }
        }
        /// <summary>
        /// Gets the Block Collection
        /// </summary>
        public BlockAdvCollection Blocks
        {
            get
            {
                return ChildNodes as BlockAdvCollection;
            }
        }
        /// <summary>
        /// Gets or sets the Header Footer type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        internal HeaderFooterType Type
        {
            get
            {
                return (HeaderFooterType)GetValue(HeaderFooterTypeProperty);
            }
            set
            {
                SetValue(HeaderFooterTypeProperty, value);
            }
        }
        /// <summary>
        /// The header footer type property
        /// </summary>
        internal static readonly DependencyProperty HeaderFooterTypeProperty = DependencyProperty.Register("HeaderFooterType", typeof(HeaderFooterType), typeof(HeaderFooter), new PropertyMetadata(HeaderFooterType.OddHeader));
        internal List<HeaderFooterWidget> LayoutedWidgets;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooter"/> class.
        /// </summary>
        public HeaderFooter()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooter"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal HeaderFooter(Node owner)
            : base(owner)
        {
            ChildNodes = new BlockAdvCollection(this);
            LayoutedWidgets = new List<HeaderFooterWidget>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Clones the items of this instance to the headerfooter.
        /// </summary>
        /// <param name="headerfooter">The headerfooter.</param>
        internal void CloneItemsTo(HeaderFooter headerfooter)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                headerfooter.Blocks.Add(Blocks[i].Clone());
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        internal HeaderFooterWidget LayoutItems(PageLayoutViewer viewer)
        {
            HeaderFooterWidget widget = new HeaderFooterWidget(this, Section);
            widget.Page = viewer.CurrentRenderingPage;
            LayoutedWidgets.Add(widget);
            widget.UpdateWidgetLocation(viewer.ClientActiveArea);
            for (int i = 0; i < Blocks.Count; i++)
            {
                if (Blocks[i] is TableAdv)
                    (Blocks[i] as TableAdv).TableWidgets.Clear();
                viewer.UpdateClientArea(Blocks[i], true);
                Blocks[i].LayoutItems(viewer);
                viewer.UpdateClientArea(Blocks[i], false);
            }
            widget.Height = viewer.ClientActiveArea.Top - viewer.ClientArea.Top;
            if (Type == HeaderFooterType.OddFooter
                || Type == HeaderFooterType.EvenFooter
                || Type == HeaderFooterType.FirstPageFooter)
                widget.ShiftChildLocation(viewer.ClientArea.Top - viewer.ClientActiveArea.Top);
            return widget;
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal void ClearWidgets()
        {
            for (int i = 0; i < LayoutedWidgets.Count; i++)
            {
                HeaderFooterWidget widget = LayoutedWidgets[i];
                widget.Dispose();
                LayoutedWidgets.Remove(widget);
                i--;
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            ClearValue(HeaderFooterTypeProperty);
            SetOwner(null);
            for (int i = 0; i < Blocks.Count; i++)
            {
                BlockAdv block = Blocks[i];
                block.Dispose();
                Blocks.Remove(block);
                i--;
            }
            ClearWidgets();
        }
        #endregion
    }
}
