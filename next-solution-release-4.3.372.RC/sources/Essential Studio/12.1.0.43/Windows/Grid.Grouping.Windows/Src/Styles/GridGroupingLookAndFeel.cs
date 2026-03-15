//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingLookAndFeel.cs" company="syncfusion">
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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO;

using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Styles;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using Syncfusion.Windows.Forms.Grid.Grouping.Design;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Provides support for serializing and restoring the look and feel of a <see cref="GridGroupingControl"/>.
    /// The GridGroupingLookAndFeel maintains a copy for properties of the GridGroupingControl and offers
    /// methods to read and write these settings. After creating a GridGroupingLookAndFeel object from
    /// an XML file, you can apply its settings to a GridGroupingControl.
    /// </summary>
    public class GridGroupingLookAndFeel
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridGroupingLookAndFeel()
            : base()
        {
        }

        /// <summary>
        /// Extracts settings from a GridGroupingControl.
        /// </summary>
        /// <param name="groupingControl">The source object.</param>
        public void InitializeFrom(GridGroupingControl groupingControl)
        {
            if (groupingControl != null)
            {
                if (groupingControl.Appearance.IsModified)
                {
                    Appearance.InitializeFrom(groupingControl.Appearance);
                }
                else
                {
                    ResetAppearance();
                }

                this.ChildGroupOptions = groupingControl.ChildGroupOptions;
                this.TopLevelGroupOptions = groupingControl.TopLevelGroupOptions;
                this.TableOptions = groupingControl.TableOptions;
                this.NestedTableGroupOptions = groupingControl.NestedTableGroupOptions;
                this.BaseStyles = groupingControl.BaseStyles;
   
#if ASPNET
#else
                this.GridVisualStyles = groupingControl.GridVisualStyles;
                this.GridOfficeScrollBars = groupingControl.GridOfficeScrollBars;
                this.Office2010ScrollBarsColorScheme = groupingControl.Office2010ScrollBarsColorScheme;
                this.Office2007ScrollBars = groupingControl.Office2007ScrollBars;
                this.Office2007ScrollBarsColorScheme = groupingControl.Office2007ScrollBarsColorScheme;
#endif
            }
        }

        /// <summary>
        /// Applies settings to a GridGroupingControl.
        /// </summary>
        /// <param name="groupingControl">The target object.</param>
        public void ApplyTo(GridGroupingControl groupingControl)
        {
            if (groupingControl != null)
            {
                if (Appearance.IsModified)
                {
                    groupingControl.Appearance.InitializeFrom(Appearance);
                }
                else
                {
                    groupingControl.Appearance.Reset();
                }

                groupingControl.ChildGroupOptions = this.ChildGroupOptions;
                groupingControl.TopLevelGroupOptions = this.TopLevelGroupOptions;
                groupingControl.TableOptions = this.TableOptions;
                groupingControl.NestedTableGroupOptions = this.NestedTableGroupOptions;
#if ASPNET
                groupingControl.BaseStyles.Clear();
                GridTableBaseStyle[] temp = new GridTableBaseStyle[this.BaseStyles.Count];
                this.BaseStyles.CopyTo(temp, 0);
                groupingControl.BaseStyles.AddRange(temp);
#else
                groupingControl.BaseStyles = this.BaseStyles;
                groupingControl.GridOfficeScrollBars = this.GridOfficeScrollBars;
                groupingControl.Office2010ScrollBarsColorScheme = this.Office2010ScrollBarsColorScheme;
                groupingControl.Office2007ScrollBars = this.Office2007ScrollBars;
                groupingControl.Office2007ScrollBarsColorScheme = this.Office2007ScrollBarsColorScheme;
                groupingControl.GridVisualStyles = this.GridVisualStyles;
#endif
            }
        }

        #region TableOptions
        GridTableOptionsStyleInfo tableOptions;

        /// <summary>
        /// Lets you set table-wide properties such as the width of the indent column or whether header rows should be visible.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridTableOptionsStyleInfo TableOptions
        {
            get
            {
                if (tableOptions == null)
                {
                    tableOptions = new GridTableOptionsStyleInfo();
                }

                return tableOptions;
            }

            set
            {
                TableOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TableOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTableOptions()
        {
            return tableOptions != null && !tableOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="TableOptions"/> object.
        /// </summary>
        public void ResetTableOptions()
        {
            TableOptions = GridTableOptionsStyleInfo.Empty;
        }
        #endregion

        #region ChildGroupOptions
        GridGroupOptionsStyleInfo childGroupOptions;

        /// <summary>
        /// Lets you control the look of inner groups such as whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo ChildGroupOptions
        {
            get
            {
                if (childGroupOptions == null)
                {
                    childGroupOptions = new GridGroupOptionsStyleInfo();
                }

                return childGroupOptions;
            }

            set
            {
                ChildGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="ChildGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeChildGroupOptions()
        {
            return childGroupOptions != null && !childGroupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="ChildGroupOptions"/> object.
        /// </summary>
        public void ResetChildGroupOptions()
        {
            ChildGroupOptions = GridGroupOptionsStyleInfo.Empty;
        }
        #endregion

        #region TopLevelGroupOptions
        GridGroupOptionsStyleInfo topLevelGroupOptions;

        /// <summary>
        /// Lets you control the look of the topmost group such as whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo TopLevelGroupOptions
        {
            get
            {
                if (topLevelGroupOptions == null)
                {
                    topLevelGroupOptions = new GridGroupOptionsStyleInfo();
                }

                return topLevelGroupOptions;
            }

            set
            {
                TopLevelGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TopLevelGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTopLevelGroupOptions()
        {
            return topLevelGroupOptions != null && !topLevelGroupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="TopLevelGroupOptions"/> object.
        /// </summary>
        public void ResetTopLevelGroupOptions()
        {
            TopLevelGroupOptions = GridGroupOptionsStyleInfo.Empty;
        }

        #endregion

        #region NestedTableGroupOptions
        GridGroupOptionsStyleInfo nestedTableGroupOptions;

        /// <summary>
        /// Lets you control the look of the topmost group of nested tables such as whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo NestedTableGroupOptions
        {
            get
            {
                if (nestedTableGroupOptions == null)
                {
                    nestedTableGroupOptions = new GridGroupOptionsStyleInfo();
                }

                return nestedTableGroupOptions;
            }

            set
            {
                NestedTableGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableGroupOptions()
        {
            return nestedTableGroupOptions != null && !nestedTableGroupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableGroupOptions"/> object.
        /// </summary>
        public void ResetNestedTableGroupOptions()
        {
            NestedTableGroupOptions = GridGroupOptionsStyleInfo.Empty;
        }
        #endregion

        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the control. This property lets you control almost any aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance();
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
            appearance = null;
        }
        #endregion

        #region BaseStyles
        GridTableBaseStyleCollection baseStyles;

        /// <summary>
        /// Maintains a collection of BaseStyles. Users can add BaseStyles to the engine (also in design time) and
        /// then inherit style settings through the GridStyleInfo.BaseStyle property in <see cref="GridTableCellStyleInfo"/>
        /// property of <see cref="GridTableCellAppearance"/>.
        /// </summary>
        [Category("Look and Feel")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection of BaseStyles used in this grid.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public GridTableBaseStyleCollection BaseStyles
        {
            get
            {
                if (baseStyles == null)
                {
                    baseStyles = new GridTableBaseStyleCollection();
                }

                return baseStyles;
            }

            set
            {
                if (value != null)
                {
                    BaseStyles.InitializeFrom(value);
                }
                else
                {
                    ResetBaseStyles();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="BaseStyles"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeBaseStyles()
        {
            return baseStyles != null && baseStyles.Count > 0;
        }

        /// <summary>
        /// Resets the <see cref="BaseStyles"/> property.
        /// </summary>
        public void ResetBaseStyles()
        {
            baseStyles = null;
        }
        #endregion

        #region GridVisualStyles
        GridVisualStyles gridVisualStyles = GridVisualStyles.SystemTheme;

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2007, Office2003
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#if ASPNET
        [Browsable(false)]
#else
        [Browsable(true)]
#endif
        public GridVisualStyles GridVisualStyles
        {
            get 
            { 
                return gridVisualStyles; 
            }

            set 
            {
                if (gridVisualStyles != value)
                {
                    gridVisualStyles = value;
                }
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="GridVisualStyles"/> object.
        /// </summary>
        public void ResetGridVisualStyles()
        {
            gridVisualStyles = GridVisualStyles.SystemTheme;
        }

        /// <summary>
        /// Determines whether <see cref="GridVisualStyles"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGridVisualStyles()
        {
            return gridVisualStyles != GridVisualStyles.SystemTheme;
        }
        #endregion

        #region Office2007ScrollBarsColorScheme
        Office2007ColorScheme office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;

        /// <summary>
        /// Gets / sets the style of Office2007 scroll bars
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#if ASPNET
        [Browsable(false)]
#else
        [Browsable(true)]
#endif
        public Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return office2007ScrollBarsColorScheme;
            }

            set
            {
                if (office2007ScrollBarsColorScheme != value)
                {
                    office2007ScrollBarsColorScheme = value;
                }
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="Office2007ScrollBarsColorScheme"/> object.
        /// </summary>
        public void ResetOffice2007ScrollBarsColorScheme()
        {
            office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
        }

        /// <summary>
        /// Determines whether <see cref="Office2007ScrollBarsColorScheme"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeOffice2007ScrollBarsColorScheme()
        {
            return office2007ScrollBarsColorScheme != Office2007ColorScheme.Blue;
        }
        #endregion
        
        #region Office2007ScrollBars
        bool office2007ScrollBars = false;

        /// <summary>
        /// Toggles between standard and Office2007 scrollbars.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#if ASPNET
        [Browsable(false)]
#else
        [Browsable(true)]
#endif
        public bool Office2007ScrollBars
        {
            get
            {
                return office2007ScrollBars;
            }

            set
            {
                if (office2007ScrollBars != value)
                {
                    office2007ScrollBars = value;
                }
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="Office2007ScrollBars"/> object.
        /// </summary>
        public void ResetOffice2007ScrollBars()
        {
            office2007ScrollBars = false;
        }

        /// <summary>
        /// Determines whether <see cref="Office2007ScrollBars"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeOffice2007ScrollBars()
        {
            return office2007ScrollBars;
        }
        #endregion

        #region Office2010ScrollBarsColorScheme
        Office2010ColorScheme office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;

        /// <summary>
        /// Gets / sets the style of Office2007 scroll bars
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#if ASPNET
        [Browsable(false)]
#else
        [Browsable(true)]
#endif
        public Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return office2010ScrollBarsColorScheme;
            }

            set
            {
                if (office2010ScrollBarsColorScheme != value)
                {
                    office2010ScrollBarsColorScheme = value;
                }
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="Office2010ScrollBarsColorScheme"/> object.
        /// </summary>
        public void ResetOffice2010ScrollBarsColorScheme()
        {
            office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
        }

        /// <summary>
        /// Determines whether <see cref="Office2010ScrollBarsColorScheme"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeOffice2010ScrollBarsColorScheme()
        {
            return office2010ScrollBarsColorScheme != Office2010ColorScheme.Blue;
        }
        #endregion

        #region GridOfficeScrollBars
        OfficeScrollBars gridOfficeScrollBars = OfficeScrollBars.None;

        /// <summary>
        /// Toggles between standard, Office2007 and Office2010 scrollbars.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#if ASPNET
        [Browsable(false)]
#else
        [Browsable(true)]
#endif
        public OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return gridOfficeScrollBars;
            }

            set
            {
                if (gridOfficeScrollBars != value)
                {
                    gridOfficeScrollBars = value;
                }
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="GridOfficeScrollBars"/> object.
        /// </summary>
        public void ResetGridOfficeScrollBars()
        {
            gridOfficeScrollBars = OfficeScrollBars.None;
        }

        /// <summary>
        /// Determines whether <see cref="GridOfficeScrollBars"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGridOfficeScrollBars()
        {
            return (gridOfficeScrollBars != OfficeScrollBars.None);
        }
        #endregion

        [ThreadStatic]
        private static XmlSerializer xmlSerializer;

        /// <summary>
        /// Returns the <see cref="XmlSerializer"/> that can be used to
        /// serialize and deserialize this object to XML.
        /// </summary>
        /// <returns>The xml serializer.</returns>
        public static XmlSerializer GetXmlSerializer()
        {
            if (xmlSerializer == null)
            {
                xmlSerializer = new XmlSerializer(typeof(GridGroupingLookAndFeel));
            }

            return xmlSerializer;
        }

        /// <overload>
        /// Creates a <see cref="GridGroupingLookAndFeel"/> object from a valid XML stream.
        /// </overload>
        /// <summary>
        /// Creates a <see cref="GridGroupingLookAndFeel"/> object from a valid XML stream.
        /// </summary>
        /// <param name="xr">The XML stream.</param>
        /// <returns>A <see cref="GridEngine"/> object.</returns>
        public static GridGroupingLookAndFeel CreateFromXml(XmlReader xr)
        {
            XmlSerializer serializer = GetXmlSerializer();
            object obj = serializer.Deserialize(xr);
            xr.Close();
            return obj as GridGroupingLookAndFeel;
        }

        /// <summary>
        /// Creates a <see cref="GridGroupingLookAndFeel"/> object from a valid XML stream.
        /// </summary>
        /// <param name="r">The TextReader with XML stream.</param>
        /// <returns>A <see cref="GridEngine"/> object.</returns>
        public static GridGroupingLookAndFeel CreateFromXml(TextReader r)
        {
            System.Xml.XmlReader xr = new System.Xml.XmlTextReader(r);
            return CreateFromXml(xr);
        }

        /// <overload>
        /// Saves the settings to an XML stream.
        /// </overload>
        /// <summary>
        /// Saves the settings to an XML stream.
        /// </summary>
        /// <param name="xw">The XMLWriter</param>
        public void WriteXml(XmlWriter xw)
        {
            XmlSerializer serializer = GetXmlSerializer();
            serializer.Serialize(xw, this);
        }

        /// <summary>
        /// Saves the settings to an XML stream.
        /// </summary>
        /// <param name="w">The TextWriter</param>
        public void WriteXml(TextWriter w)
        {
            System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(w);
            xw.Formatting = Formatting.Indented;
            WriteXml(xw);
        }
    }
}