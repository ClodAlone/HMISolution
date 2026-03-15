//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownGridListControlCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the model / data part of a drop-down ListControl-like grid.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDropDownGridListControlCellModel"/> can serve as model for several <see cref="GridDropDownGridListControlCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDropDownGridListControlCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDropDownGridListControlCellModel : GridComboBoxCellModel
    {
        internal bool isCombobox = false;
        ////bool allowDoubleClickChangeSelectedIndex = true;

        /// <overload>
        /// Initializes a new <see cref="GridDropDownGridListControlCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownGridListControlCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownGridListControlCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
            this.isCombobox = false;
        }

        /// <overload>
        /// Initializes a new <see cref="GridDropDownGridListControlCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownGridListControlCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>  
        /// <param name="isCombobox">is dropdown is combobox</param>  
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownGridListControlCellModel(GridModel grid, bool isCombobox)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
            this.isCombobox = isCombobox;
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownGridListControlCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDropDownGridListControlCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GridDropDownGridListControlCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The grid control for which the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridDropDownGridListControlCellRenderer"/> specific for the specified grid.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridDropDownGridListControlCellRenderer(control, this);
        }

#if reuse
        /// <summary>
        /// This is called to initialize a datasource on demand. This lets you calculate the datasource
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.DataSource is returned.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public virtual object GetDataSource(GridStyleInfo style)
        {
            return Grid.GetStyleDataSource(style);
        }

        /// <summary>
        /// Initializes a <see cref="ListBox"/> with data binding information from a <see cref="GridStyleInfo"/>
        /// object.  
        /// </summary>
        /// <param name="listBox">The list box to be initialized with data binding information1717-0306-0465-1665</param>
        /// <param name="style">The style object with binding information.</param>
        /// <param name="exclusive">A place holder that indicates returns whether the list box is filled with an exclusive 
        /// list of possible choices or if non-standard values are allowed.
        /// </param>
        public virtual void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = style.ExclusiveChoiceList;
            object dataSource = GetDataSource(style);
            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                if (dataSource != null)
                {
                    //                    listBox.BackColor = style.Interior.BackColor;
                    //                    listBox.Font = style.Font.GdipFont;
                    //                    listBox.ForeColor = style.TextColor;
                    //listBox.ImageList = style.ImageList;

                    if (listBox.BindingContext == null
                        || dataSource != listBox.DataSource
                        || listBox.DisplayMember != style.DisplayMember
                        || listBox.ValueMember != style.ValueMember)
                    {
                        // fill with Choices
                        listBox.DataSource = null;
                        listBox.DisplayMember = style.DisplayMember;
                        listBox.ValueMember = style.ValueMember;
                        listBox.DataSource = dataSource;
                        listBox.BindingContext = this.BindingContext;
                    }
                }
                else
                {
                    Type valueType = style.CellValueType;
                    if (valueType != null)
                    {
                        listBox.Items.Clear();
                        TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                        if (tc != null && tc.GetStandardValuesSupported())
                        {
                            ICollection collection = tc.GetStandardValues();
                            foreach (object item in collection)
                                listBox.Items.Add(item.ToString());

                            exclusive = tc.GetStandardValuesExclusive();
                        }
                    }
                }
            }
            else
            {
                listBox.Items.Clear();
                if (style.ChoiceList != null)
                {
                    foreach (string item in style.ChoiceList)
                        listBox.Items.Add(item);
                }
            }
        }

        GridComboBoxListBoxHelper listBox;

        internal GridComboBoxListBoxHelper ListBox
        {
            get
            {
                if (listBox == null)
                {
                    listBox = new GridComboBoxListBoxHelper();
                }
                return listBox;
            }
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            if (text.Length > 0 && (style.ChoiceList == null || style.ChoiceList.Count == 0))    
            {
                if (style.DisplayMember != style.ValueMember)
                {
                    bool exclusive;
                    this.FillWithChoices(ListBox, style, out exclusive);
                    if (ListBox.DataSource != null)
                    {
                        int index = ListBox.FindStringExact(text);
                        if (index != -1)
                        {
                            //listBox.SelectedIndex = index;
                            style.CellValue = ListBox.GetItemValue(index);
                            return true;
                        }
                        if (exclusive)
                            return false;
                    }
                }
            }

            return base.ApplyFormattedText(style, text, textInfo);
        }

        /// <summary>
        /// Returns the index in the drop-down list box for the specified cell value / key.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value (same as ValueMember).</param>
        /// <returns>The index in the drop-down list box or -1 if not found.</returns>
        public int FindValue(GridStyleInfo style, object value)
        {
            if (value == null || value is string && value.Equals(""))
                return -1;

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)    
            {
                bool exclusive;
                this.FillWithChoices(ListBox, style, out exclusive);
                if (ListBox.BindingContext != null && ListBox.DataSource != null)
                {
                    //ListBox.SelectedIndex = -1;
                    //ListBox.SelectedValue = value;
                    return ListBox.FindValue(value);
                }
            }
            
            return -1;
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText. 
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        { 
            if (value == null || value is string && value.Equals(""))
                return "";

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)    
            {
                if (style.DataSource != null && style.DisplayMember != style.ValueMember)
                {
                    bool exclusive;
                    this.FillWithChoices(ListBox, style, out exclusive);
                    if (ListBox.BindingContext != null)
                    {
                        if (style.ValueMember == "")
                        {
                            if (value is string || value.GetType().IsPrimitive)
                            {
                                if (exclusive)
                                    return "";
                            }
                            else
                                return ListBox.GetItemText(value);
                        }
                        else
                        {
                            int index = ListBox.FindValue(value);
                            if (index != -1)
                            {
                                return ListBox.GetItemText(index);
                            }

                            if (exclusive)
                                return "";
                        }
                    }
                }
            }

            return base.GetFormattedText(style, value, textInfo);

        }
    
        /// <summary>
        /// Lets you enable or turn off selecting the next index when user double clicks inside cell.
        /// </summary>
        public bool AllowDoubleClickChangeSelectedIndex
        {
            get
            {
                return allowDoubleClickChangeSelectedIndex;
            }
            set
            {
                allowDoubleClickChangeSelectedIndex = value;
            }
        }
#endif
    }
}
