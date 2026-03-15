//-------------------------------------------------------------------------------------------------
// <copyright file="GridElements.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// This interface provides routines for getting and setting row heights for elements. 
    /// Implement it in derived elements when custom row heights are needed.
    /// </summary>
    public interface IGridRowHeight
    {
        /// <summary>
        /// Determines if elements support storing row heights.
        /// </summary>
        /// <returns>returns boolean value to indicate support row height</returns>
        bool SupportsRowHeight();

        /// <summary>
        /// The row height.
        /// </summary>
        int RowHeight { get; set; }

        /// <summary>
        /// Checks if row height was modified.
        /// </summary>
        bool HasRowHeight { get; }
    }

    /// <summary>
    /// The section with the CaptionRow.
    /// </summary>
    /// <remarks>
    /// This is the first section within a group which provides the caption bar above the
    /// column headers. CaptionSection is a display element and will be an item returned by
    /// the Table.DisplayElements and Table.GroupedElements collection.<para/>
    /// <para/>
    /// CaptionSection is also a Section that can be accessed through the Group.Sections
    /// collection of its parent group.
    /// <para/>
    /// If a caption should be displayed at the beginning of each group, then CaptionSections
    /// are created when the grouping for a table is initialized. The grouping engine
    /// loops through all sorted records and categorizes them. For each new group, the virtual
    /// TableDescriptor.CreateGroup factory method is called. The TableDescriptor.CreateGroup method instantiates a CaptionSection by calling the
    /// virtual TableDescriptor.CreateCaptionSection factory method. <para/>
    /// </remarks>
    /// <example>The following example creates a string based on the CaptionSection's parent
    /// group number of direct child elements (as usually shown in the caption element for a
    ///  group in GroupingGrid). Use the ParentGroup property to get access to the parent
    ///  Group of this CaptionSection:
    /// <code lang="C#">
    ///             CaptionSection cs;
    ///             object cat = cs.ParentGroup.Category;
    ///             if (cat == null)
    ///                 cat = "(null)";
    ///             string caption = "{2} Item(s)";
    ///             string groupName = cs.ParentGroup.Name;
    ///             if (groupName != "")
    ///                 caption = "{0}: {1} - " + caption;
    ///             return string.Format(caption, groupName, cat, cs.ParentGroup.GetChildCount());
    /// </code>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateCaptionSection, Group.Caption,
    /// Group.Sections, IDisplayElement
    /// </example>
    public class GridCaptionSection : CaptionSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridCaptionSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
#if ASPNET
        private string pagingCaptionText;
        /// <summary>
        /// Specifies the text that will be added to the caption with paging information pertaining to this group.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string PagingCaptionText{get{return this.pagingCaptionText;}set{this.pagingCaptionText = value;}}

        private int pageCount = -1;
        /// <summary>
        /// Specifies the number of pages in the grid.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int PageCount { get { return this.pageCount; } set { this.pageCount = value; } }

        private int currentPage = -1;
        /// <summary>
        /// Specifies the current page in the grid.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int CurrentPage { get { return this.currentPage; } set { this.currentPage = value; } }
#endif
    }

    /// <summary>
    /// Represents the row for the column header row in a group.
    /// </summary>
    /// <remarks>
    /// A ColumnHeaderRow is displayed at the top of a table or group. ColumnHeaderRow is a place holder
    /// item where the grouping grid should display column headers. <para/>
    /// <para/>
    /// ColumnHeaderRow is a RowElement and an element of the ColumnHeaderSection.ColumnHeaderRows
    /// collection of a parent ColumnHeaderSection. <para/>
    /// <para/>
    /// ColumnHeaderRows are created and removed internally within a ColumnHeaderSection when the
    /// section is created and when the TableDescriptor.RowsPerRecord property of a TableDescriptor
    /// is changed.<para/>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.RowsPerRecord, IDisplayElement, ColumnHeaderSection, ColumnHeaderSection.ColumnHeaderRows,
    /// ColumnHeaderRowCollection
    /// <para/>
    /// </remarks>
    public class GridColumnHeaderRow : ColumnHeaderRow, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridColumnHeaderRow(ColumnHeaderSection parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridFilterBarRow : FilterBarRow, IGridTableCellAppearanceSource
    {
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridFilterBarRow(FilterBarSection parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// Represents the row for the caption bar of a group.
    /// </summary>
    public class GridCaptionRow : CaptionRow, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes the new element in the specified CaptionSection.
        /// </summary>
        /// <param name="parent">The parent that this element belongs to.</param>
        public GridCaptionRow(CaptionSection parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// The section with the AddNewRecord:
    /// </summary>
    /// <remarks>
    /// This is the section within a group which provides the AddNewRecord that is shown above the
    /// table records and / or below the records for each group and implements logic to add new records.
    /// See the AddNewRecord class for more information.<para/>
    /// <para/>
    /// The AddNewRecordSection is a RecordsDetails section and thus also a container element. Normally,
    ///  a RecordsDetails section has a collection of one or multiple records but with a
    ///  AddNewRecordSection you have exactly one child record, the AddNewRecord.<para/>
    /// <para/>
    /// If AddNewRecord(s) should be displayed at end of each group, then AddNewRecordSection(s) are
    /// created when the grouping for a table is initialized. The grouping engine loops through all
    /// sorted records and categorizes them. For each new group, the virtual TableDescriptor.CreateGroup
    /// factory method is called. The TableDescriptor.CreateGroup method instantiates an AddNewRecordSection
    /// by calling the virtual TableDescriptor.CreateAddNewRecordSection factory method. <para/>
    /// <para/>
    /// One AddNewRecord is displayed at the top of the table below the column headers. It belongs
    /// to the AddNewRecordSection of the TopLevelGroup of a table. The TopLevelGroup is created
    /// with the virtual TableDescriptor.CreateGroup factory method. The TableDescriptor.CreateGroup
    /// method instantiates an AddNewRecordSection by calling the virtual
    /// TableDescriptor.CreateAddNewRecordSection factory method.<para/>
    /// <para/>
    /// Because AddNewRecordSection is a container element and not a display element, it will not
    /// be an item returned by the Table.DisplayElements and Table.GroupedElements collection. Instead,
    /// AddNewRecordSection can be accessed only through the Group.Sections collection of its parent
    /// group.<para/>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateAddNewRecordSection, Table.TopLevelGroup,
    /// AddNewRecord, IContainerElement, Group.Sections
    /// </remarks>
    public class GridAddNewRecordSection : AddNewRecordSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridAddNewRecordSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// The section with the ColumnHeaderRow(s):
    /// </summary>
    /// <remarks>
    /// A ColumnHeaderSection holds ColumnHeaderRow(s) that are displayed at the top of a
    /// table or group. The ColumnHeaderRow is a place holder item where the grouping grid
    /// should display column headers. <para/>
    /// <para/>
    /// The ColumnHeaderSection is a container element and has a collection of one or
    /// multiple ColumnHeaderRow elements.<para/>
    /// <para/>
    /// If ColumnHeaderSections should be displayed at the top of each group, then
    /// ColumnHeaderSections are created when the grouping for a table is initialized.
    /// The grouping engine loops through all sorted records and categorizes them. For
    /// each new group, the virtual TableDescriptor.CreateGroup factory method is called.
    /// The TableDescriptor.CreateGroup method instantiates a ColumnHeaderSection by
    /// calling the virtual TableDescriptor.CreateColumnHeaderSection factory method.
    /// <para/>
    /// <para/>
    /// One set of ColumnHeaderRows are displayed at the top of the table below the CaptionSection.
    /// The rows belong to the ColumnHeaderSection of the TopLevelGroup
    /// of a table. The TopLevelGroup is created with the virtual
    /// TableDescriptor.CreateGroup factory method. The TableDescriptor.CreateGroup
    /// method instantiates a ColumnHeaderSection by calling the virtual
    /// TableDescriptor.CreateColumnHeaderSection factory method. <para/>
    /// <para/>
    /// Because the ColumnHeaderSection is a container element and not a display element,
    /// it will not be an item returned by the Table.DisplayElements and
    /// Table.GroupedElements collection. Instead, the ColumnHeaderSection can be
    /// accessed only through the Group.Sections collection of its parent group
    /// or TopLevelGroup.<para/>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateColumnHeaderSection,
    /// Table.TopLevelGroup, IContainerElement, Group.Sections
    /// </remarks>
    public class GridColumnHeaderSection : ColumnHeaderSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridColumnHeaderSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// An empty section with 1 visible element. Can be used as a place holder to appear in <see cref="Table.DisplayElements"/>. GetYAmountCount will
    /// return <see cref="Table.DefaultEmptySectionHeight"/> of the ParentTable.
    /// </summary>
    public class GridEmptySection : EmptySection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridEmptySection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A hidden section with 1 visible element. Used as a place holder at the very first
    /// row in the GridNestedTableControl needed for scrolling logic.
    /// </summary>
    public class GridHiddenSection : GridEmptySection
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridHiddenSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        /// <summary>
        /// Gets the number of visible elements.
        /// </summary>
        /// <returns>returns value 1.</returns>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <override/>
        /// <summary>Gets the element height.</summary>
        /// <returns>returns value 0.</returns>
        public override double GetYAmountCount()
        {
            return 0;
        }
    }

    /// <summary>
    /// A place holder section with 1 visible element for showing a header above a group in <see cref="Table.DisplayElements"/>.
    /// </summary>
    public class GridGroupHeaderSection : GroupHeaderSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridGroupHeaderSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A preview section with one visible element for showing a preview below a group in <see cref="Table.DisplayElements"/>
    /// when a group is collapsed.
    /// </summary>
    public class GridGroupPreviewSection : GroupPreviewSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridGroupPreviewSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A place holder section with 1 visible element for showing a footer below a group in <see cref="Table.DisplayElements"/>.
    /// </summary>
    public class GridGroupFooterSection : GroupFooterSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridGroupFooterSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    ///  A place holder section with 1 visible element for showing a filter bar.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridFilterBarSection : FilterBarSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        public GridFilterBarSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// The details section of a group with nested child groups.
    /// </summary>
    public class GridGroupsDetails : GroupsDetails, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridGroupsDetails(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A nested table inside a record. You can access nested tables in a record with the <see cref="Record.NestedTables"/>
    /// property of a <see cref="Record"/>. Each nested table has a <see cref="ChildTable"/> that provides the link to the
    /// to the child records in a related table.
    /// </summary>
    public class GridNestedTable : NestedTable, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new object with the specified parent.
        /// </summary>
        /// <param name="parent">The parent element this object is created in.</param>
        public GridNestedTable(RecordNestedTablesPart parent)
            : base(parent)
        {
        }

        /// <summary>
        /// The <see cref="GridChildTable"/> provides the link to the
        /// to the child records in a related table.
        /// </summary>
        public new GridChildTable ChildTable
        {
            get
            {
                return (GridChildTable)base.ChildTable;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// The part of a record with nested tables. You can access nested tables in a record with the <see cref="Record.NestedTables"/>
    /// property of a <see cref="Record"/>.
    /// </summary>
    public class GridRecordNestedTablesPart : RecordNestedTablesPart, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new object with the given record as parent.
        /// </summary>
        /// <param name="parent">The parent record this object is created in.</param>
        public GridRecordNestedTablesPart(Record parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A Record row is a place-holder element shown in the <see cref="Table.DisplayElements"/>
    /// for each row in a record. Each record can have multiple record rows. You get access to this element through the <see cref="Record.RecordRows"/>
    /// property of a <see cref="Record"/>.
    /// </summary>
    public class GridRecordRow : RecordRow, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new object in the specified record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GridRecordRow(RecordRowsPart parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A RecordPreview row is an optional element shown only in the <see cref="Table.DisplayElements"/>
    /// when a record is collapsed. You get access to this element through the <see cref="Record.RecordPreviewRows"/>
    /// property of a <see cref="Record"/>.
    /// </summary>
    public class GridRecordPreviewRow : RecordPreviewRow, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes an object with the specified parent element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GridRecordPreviewRow(RecordPreviewRowsPart parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return base.ParentRecord as GridRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A part in a record that serves as container for <see cref="RecordRow"/>
    /// elements in a record.
    /// </summary>
    public class GridRecordRowsPart : RecordRowsPart, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new object with the given record as parent.
        /// </summary>
        /// <param name="parent">The parent record this object is created in.</param>
        public GridRecordRowsPart(Record parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// The details section of a group with records.
    /// </summary>
    public class GridRecordsDetails : RecordsDetails, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridRecordsDetails(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A base class for sections that can contain one or multiple row elements.
    /// </summary>
    public class GridRowElementsSection : RowElementsSection, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GridRowElementsSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// Implement this interface for elements and keep
    /// a cache with style information for individual cells
    /// if you want reduce the number of
    /// QueryCellStyleInfo calls being raised for cells of this element.
    /// </summary>
    /// <remarks>
    /// Here is example code to implement this interface. It is important to keep a version value and compare the version with GridEngine.Version. This ensures that when changes are made to the engine or table that the cache is reset:
    /// <example>
    /// <code lang="C#">
    ///     public class GridRecordRowWithCache : GridRecordRow, IGridTableCellAppearanceSource, IGridTableCellStyleCache
    ///     {
    ///         public GridRecordRowWithCache(RecordRowsPart parent)
    ///             : base(parent)
    ///         {
    ///         }
    /// <para/>
    ///         protected override void Dispose(bool disposing)
    ///         {
    ///             styles = null;
    ///             base.Dispose (disposing);
    ///         }
    /// <para/>
    ///         #region IGridTableCellStyleCache Members
    ///         ArrayList styles;
    ///         int version;
    /// <para/>
    ///         public ArrayList Styles
    ///         {
    ///             get
    ///             {
    ///                 if (styles == null || this.version != Engine.Version)
    ///                 {
    ///                     this.version = Engine.Version;
    ///                     styles = new ArrayList();
    ///                 }
    ///                 return styles;
    ///             }
    ///         }
    /// <para/>
    ///         public GridTableCellStyleInfo GetStyleAtColumn(int colIndex)
    ///         {
    ///             if (Styles != null &amp;&amp; styles.Count &gt; colIndex)
    ///             {
    ///                 return styles[colIndex] as GridTableCellStyleInfo;
    ///             }
    /// <para/>
    ///             return null;
    ///         }
    /// <para/>
    ///         public void SetStyleAtColumn(int colIndex, GridTableCellStyleInfo style)
    ///         {
    ///             if (version != Engine.Version)
    ///                 ResetStyles();
    /// <para/>
    ///             if (Styles.Count &lt;= colIndex)
    ///                 Styles.AddRange(new object[colIndex-Styles.Count+1]);
    /// <para/>
    ///             styles[colIndex] = style;
    ///         }
    ///         public void ResetStyles()
    ///         {
    ///             styles = null;
    ///         }
    /// <para/>
    ///         #endregion
    ///     }
    /// </code>
    /// </example>
    /// <para/>
    /// </remarks>
    public interface IGridTableCellStyleCache
    {
        /// <summary>
        /// Returns the style for the specified column index.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The style object.</returns>
        GridTableCellStyleInfo GetStyleAtColumn(int colIndex);

        /// <summary>
        /// Saves the style at the specified column index.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The style object.</param>
        void SetStyleAtColumn(int colIndex, GridTableCellStyleInfo style);

        /// <summary>
        /// Empties the cache.
        /// </summary>
        void ResetStyles();
    }

    /// <summary>
    /// This is a specialized version of the <see cref="GridCaptionRow"/> class. It implements the
    /// <see cref="IGridTableCellStyleCache"/> and keeps
    /// a cache with style information for individual cells.
    /// </summary>
    public class GridCaptionRowWithCache : GridCaptionRow, IGridTableCellAppearanceSource, IGridTableCellStyleCache
    {
        /// <summary>
        /// Initializes a new object in the specified record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GridCaptionRowWithCache(CaptionSection parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            styles = null;
            base.Dispose(disposing);
        }

        #region IGridTableCellStyleCache Members
        ArrayList styles;
        int version;

        ArrayList Styles
        {
            get
            {
                // Engine.Version will be increased whenever changes to the schema or the source
                // list were made.
                if (styles == null || this.version != Engine.Version)
                {
                    this.version = Engine.Version;
                    styles = new ArrayList();
                }

                return styles;
            }
        }

        GridTableCellStyleInfo IGridTableCellStyleCache.GetStyleAtColumn(int colIndex)
        {
            if (Styles != null && styles.Count > colIndex)
            {
                return styles[colIndex] as GridTableCellStyleInfo;
            }

            return null;
        }

        void IGridTableCellStyleCache.SetStyleAtColumn(int colIndex, GridTableCellStyleInfo style)
        {
            if (version != Engine.Version)
            {
                styles = null;
            }

            if (Styles.Count <= colIndex)
            {
                Styles.AddRange(new object[colIndex - Styles.Count + 1]);
            }

            styles[colIndex] = style;
        }

        void IGridTableCellStyleCache.ResetStyles()
        {
            styles = null;
        }
        #endregion
    }

    /// <summary>
    /// This is a specialized version of the <see cref="GridRecordRow"/> class. It implements the
    /// <see cref="IGridTableCellStyleCache"/> and keeps
    /// a cache with style information for individual cells.
    /// </summary>
    public class GridRecordRowWithCache : GridRecordRow, IGridTableCellAppearanceSource, IGridTableCellStyleCache
    {
        /// <summary>
        /// Initializes a new object in the specified record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public GridRecordRowWithCache(RecordRowsPart parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            styles = null;
            base.Dispose(disposing);
        }

        #region IGridTableCellStyleCache Members
        ArrayList styles;
        int version;

        ArrayList Styles
        {
            get
            {
                // Engine.Version will be increased whenever changes to the schema or the source
                // list were made.
                if (styles == null || this.version != Engine.Version)
                {
                    this.version = Engine.Version;
                    styles = new ArrayList();
                }

                return styles;
            }
        }

        GridTableCellStyleInfo IGridTableCellStyleCache.GetStyleAtColumn(int colIndex)
        {
            if (Styles != null && styles.Count > colIndex)
            {
                return styles[colIndex] as GridTableCellStyleInfo;
            }

            return null;
        }

        void IGridTableCellStyleCache.SetStyleAtColumn(int colIndex, GridTableCellStyleInfo style)
        {
            if (version != Engine.Version)
            {
                styles = null;
            }

            if (Styles.Count <= colIndex)
            {
                Styles.AddRange(new object[colIndex - Styles.Count + 1]);
            }

            styles[colIndex] = style;
        }

        void IGridTableCellStyleCache.ResetStyles()
        {
            styles = null;
        }

        #endregion
    }
}
