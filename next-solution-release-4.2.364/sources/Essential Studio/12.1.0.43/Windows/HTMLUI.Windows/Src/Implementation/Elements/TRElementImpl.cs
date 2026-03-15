#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;TR&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Tr)]
    public class TRElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Tr;

        /// <summary>
        /// XPath for retrieving cells inside the row.
        /// </summary>
        private const string DEF_TD_XPATH = "node()[ name()='" + TagName.Td + "' ]";

        /// <summary>
        /// XPath for retrieving cells inside the row.
        /// </summary>
        private const string DEF_TH_XPATH = "node()[ name()='" + TagName.Th + "' ]";

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class members
        /// <summary>
        /// Quantity of TD tags in children.
        /// </summary>
        private int m_iCells;

        /// <summary>
        /// Quantity of cells together with colspan.
        /// </summary>
        private int m_iVirtualCells;

        /// <summary>
        /// Parent table for the cell element.
        /// </summary>
        private TABLEElementImpl m_table;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Gets the real quantity of TD tags in children.
        /// </summary>
        public int CellsCount
        {
            get
            {
                return m_iCells;
            }
        }

        /// <summary>
        /// Gets the quantity of cells including the Colspan settings.
        /// </summary>
        public int VirtualCellsCount
        {
            get
            {
                return m_iVirtualCells;
            }
        }

        /// <summary>
        /// Gets or sets the parent table for this cell element.
        /// </summary>
        protected internal TABLEElementImpl Table
        {
            get
            {
                return m_table;
            }
            set
            {
                if (m_table != value)
                {
                    m_table = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the TRElementImpl class 
        /// </summary>
        static TRElementImpl()
        {
            Type type = typeof(TRElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the TRElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public TRElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Overloaded. Disposes element.
        /// </summary>
        protected override void OnDispose()
        {
            EventBaseCollection children = this.Children as EventBaseCollection;

            if (children != null)
            {
                children.Inserted -= new CollectionEventHandler(ChildrenChanged);
                children.Removed -= new CollectionEventHandler(ChildrenChanged);
            }

            base.OnDispose();

            if (m_table != null)
            {
                m_table = null;
            }
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Raised when the collection of children has changed.
        /// Here we have to update our data about row's dimensions.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        protected internal void ChildrenChanged(object sender, CollectionEventArgs e)
        {
            CalculateCellsCounts(this.Storage);

            // Force parent table to update it's data about table's dimensions.
            this.Table.ChildrenChanged(sender, e);
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size instance</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Type = ElementType.BlockNewLineSimple;
            this.Size = Size.Empty;

            if (!this.IsVisible) return this.Size;

            this.Size = CalulateCellsSizes();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();

            // Set border to 0 width.
            ((HTMLFormat)this.Format).QuietMode = true;
            this.Format.Left.Width = 0;
            this.Format.Top.Width = 0;
            this.Format.Right.Width = 0;
            this.Format.Bottom.Width = 0;
            ((HTMLFormat)this.Format).QuietMode = false;

            this.SkipWidth = true;
        }

        /// <summary>
        /// Overridden. Calculates the positions of the child element.
        /// </summary>
        /// <param name="curPosition">Current position.</param>
        /// <param name="bounds">Bounds of the element.</param>
        /// <returns>Array of blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            this.QuietMode = true;

            BlocksCollection blocks = base.CalculateChildPositions(curPosition, bounds);

            if (this.MainBlock != null)
            {
                this.MainBlock.DrawProperties = false;
            }

            this.QuietMode = false;
            OnLocationCalculated(EventArgs.Empty);

            return blocks;
        }

        /// <summary>
        /// Overridden. Reparses the inner HTLML data of the element.
        /// </summary>
        /// <param name="htmlToParse">HTML code for parsing.</param>
        protected override void ReparseInnerHTML(string htmlToParse)
        {
            base.ReparseInnerHTML(htmlToParse);

            CalculateCellsCounts(this.Storage);
        }

        /// <summary>
        /// Calculates the number of cells in the row.
        /// </summary>
        /// <param name="element">XML element for the object.</param>
        protected void CalculateCellsCounts(XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            XmlNodeList elements = element.SelectNodes(DEF_TD_XPATH);

            m_iCells = 0;
            m_iVirtualCells = 0;

            CalculateCellsCount(element, elements);

            elements = element.SelectNodes(DEF_TH_XPATH);
            CalculateCellsCount(element, elements);
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down after this element.
        /// </summary>
        /// <param name="currentPos">Current position.</param>
        protected override void MoveFinalCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            // Define size of the row element.
            if (this.MainBlock != null)
            {
                this.MainBlock.FinalWidth = this.MainBlock.Width;
                this.MainBlock.FinalHeight = this.MainBlock.Height;

                this.Width = this.MainBlock.FinalWidth;
                this.Height = this.MainBlock.FinalHeight;
            }
        }

        /// <summary>
        /// Overridden. Initializes an element.
        /// </summary>
        protected internal override void InitializeElement()
        {
            base.InitializeElement();
            CalculateCellsCounts(this.Storage);

            EventBaseCollection children = this.Children as EventBaseCollection;

            children.Inserted += new CollectionEventHandler(ChildrenChanged);
            children.Removed += new CollectionEventHandler(ChildrenChanged);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculates the number of cells defined in the list of nodes.
        /// </summary>
        /// <param name="element">XML element of the object.</param>
        /// <param name="elements">List of elements.</param>
        private void CalculateCellsCount(XmlElement element, XmlNodeList elements)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null)
                throw new ArgumentNullException("elements");

            XmlNode node = null;
            XmlElement elm = null;

            for (int i = 0, len = elements.Count; i < len; i++)
            {
                node = elements[i];
                if (node is XmlElement == false) continue;

                elm = node as XmlElement;

                if (elm.ParentNode == element)
                {
                    if (elm.HasAttribute(AttributeName.Colspan))
                    {
                        int count = AttributeParser.GetInteger(elm.GetAttribute(AttributeName.Colspan));
                        if (count > 1)
                        {
                            m_iVirtualCells += count - 1;
                        }
                    }

                    m_iVirtualCells++;
                    m_iCells++;
                }
            }
        }

        /// <summary>
        /// Calculates the size of all child cells and defines minWidth as sum of min width of each cell.
        /// </summary>
        /// <returns>Size of the cells.</returns>
        private Size CalulateCellsSizes()
        {
            Size result = Size.Empty;

            if (!this.HasChildren) return result;

            IHTMLElement[] cellsList = GetCells();

            if (cellsList.Length == 0) return Size.Empty;

            int minWidth = 0;

            // Go through cells in row.
            TDElementImpl cell = null;

            for (int i = 0, len = cellsList.Length; i < len; i++)
            {
                cell = cellsList[i] as TDElementImpl;
                cell.CalculateSize();
                minWidth += cell.MinWidth;
                result.Width += cell.Width;
                result.Height = Math.Max(result.Height, cell.Height);
                this.MinHeight = Math.Max(this.MinHeight, cell.MinHeight);
            }

            this.MinWidth = minWidth;

            return result;
        }

        /// <summary>
        /// Returns an array of cells in the row. Returns the TD and TH elements.
        /// </summary>
        /// <returns>Array of cells in the row.
        /// Returns TD and TH elements.</returns>
        protected internal IHTMLElement[] GetCells()
        {
            IHTMLElement[] result = this.Children.GetElementsByName(new string[] { TagName.Td, TagName.Th });

            return result;
        }
        #endregion
    }
}
