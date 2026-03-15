//-------------------------------------------------------------------------------------------------
// <copyright file="GridRangeInfoConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;

using System.ComponentModel.Design;
using System.CodeDom;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///      GridRangeInfoConverter is a class that can be used to convert
    ///      ranges from one data type to another. Access this
    ///      class through the TypeDescriptor.
    /// </summary>
    public class GridRangeInfoConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRangeInfoConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>       
        /// <param name="context">Format
        /// context. </param>
        /// <param name="sourceType">The type
        /// you want to convert from. </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>        
        /// <param name="context">Format
        /// context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The object to convert. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                s = s.Trim();
                if (s.Length == 0)
                {
                    return GridRangeInfo.Empty;
                }

                GridRangeInfo range = GridRangeInfo.Parse(s);
                if (range.IsEmpty)
                {
                    throw new ArgumentException(SR.GetString(SR.TextParseFailedFormat, s, SR.TopLeftBottomRight));
                }

                return range;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>        
        /// <param name="context">Format
        /// context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The object to convert. </param>   
        /// <param name="destinationType">The type to convert the
        /// value parameter to. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(@"destinationType");
            }

            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return string.Empty;
                }

                GridRangeInfo range = (GridRangeInfo)value;
                return range.ToString("G", null);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        /// <override/>
        /// <summary>
        /// Creates an instance of the type that this <see
        /// cref="TypeConverter" /> is associated with, using the
        /// specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <param name="propertyValues">A collection
        /// of new property values. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> representing the given <see
        /// cref="T:System.Collections.IDictionary" />, or null if the object cannot be
        /// created. This method always returns null.
        /// </returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            GridRangeInfoType type = (GridRangeInfoType)propertyValues["RangeType"];
            int top = (int)propertyValues["Top"];
            int left = (int)propertyValues["Left"];
            int bottom = (int)propertyValues["Bottom"];
            int right = (int)propertyValues["Right"];

            if (context != null)
            {
                // Find out what attribute changed 
                object o = context.Instance is GridRangeInfo ? context.Instance : context.PropertyDescriptor.GetValue(context.Instance);
                if (o is GridRangeInfo)
                {
                    GridRangeInfo oldRange = (GridRangeInfo)o;

                    if (type == oldRange.RangeType || oldRange.IsEmpty)
                    {
                        if (top != oldRange.Top)
                        {
                            if (top > bottom)
                            { 
                                bottom = top;
                            }

                            if (oldRange.IsEmpty)
                            {
                                type = GridRangeInfoType.Cells;
                            }
                        }

                        if (bottom != oldRange.Bottom)
                        {
                            if (top > bottom)
                            {
                                top = bottom;
                            }

                            if (oldRange.IsEmpty)
                            {
                                type = GridRangeInfoType.Cells;
                            }
                        }

                        if (left != oldRange.Left)
                        {
                            if (left > right)
                            {
                                right = left;
                            }

                            if (oldRange.IsEmpty)
                            {
                                type = GridRangeInfoType.Cells;
                            }
                        }

                        if (right != oldRange.Right)
                        {
                            if (left > right)
                            {
                                left = right;
                            }

                            if (oldRange.IsEmpty)
                            {
                                type = GridRangeInfoType.Cells;
                            }
                        }
                    }
                }
            }

            return new GridRangeInfo(type, top, left, bottom, right);
        }

        /// <override/>
        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see
        /// cref="TypeConverter.CreateInstance(System.Collections.IDictionary)"
        /// /> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <returns>
        /// returns true.
        /// </returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of properties for the specified object type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The object that represents the type.</param>
        /// <param name="attributes">An array of System.Attibute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection propertyDescriptorCollection
                = TypeDescriptor.GetProperties(typeof(GridRangeInfo), attributes);

            string[] atts = new string[]
            {
                "RangeType",
                "Top",
                "Left",
                "Bottom",
                "Right"
            };

            return propertyDescriptorCollection.Sort(atts);
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <returns>
        /// returns true.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    /// <summary>
    ///      GridRangeInfoCodeDomSerializer serializes a GridRangeInfo object
    ///      into a series of CodeDOM statements.
    /// </summary>
    public class GridRangeInfoCodeDomSerializer : CodeDomSerializer
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRangeInfoCodeDomSerializer()
            : base()
        {
        }

        // Methods.

        /// <override/>
        /// <summary>
        /// Deserializes the specified serialized CodeDOM object into an object.
        /// </summary>
        /// <param name="manager">A serialization manager interface that is used during the
        /// deserialization process. </param>
        /// <param name="codeObject">A serialized CodeDOM object to deserialize. </param>
        /// <returns>
        /// The deserialized CodeDOM object.
        /// </returns>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            return null;
        }

        /// <override/>
        /// <summary>
        /// Serializes the specified object into a CodeDOM object.
        /// </summary>
        /// <param name="manager">The serialization manager to use during serialization.
        /// </param>
        /// <param name="value">The object to serialize. </param>
        /// <returns>
        /// A CodeDOM object representing the object that has been serialized.
        /// </returns>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(@"manager");
            }

            if (!(value is GridRangeInfo) || value == null)
            {
                throw new ArgumentException(@"value");
            }

            CodeMethodReferenceExpression cmre = new CodeMethodReferenceExpression();
            cmre.TargetObject = new CodeTypeReferenceExpression(value.GetType());  
            //// name below

            CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression();
            cmie.Method = cmre;
            //// parameters below

            GridRangeInfo range = (GridRangeInfo)value;

            switch (range.RangeType)
            {
                case GridRangeInfoType.Empty:
                    cmre.MethodName = "EmptyRange";
                    // no parameters
                    break;

                case GridRangeInfoType.Table:
                    cmre.MethodName = "Table";
                    // no parameters
                    break;

                case GridRangeInfoType.Rows:
                    if (range.Top != range.Bottom)
                    {
                        cmre.MethodName = "Rows";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Top));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Bottom));
                    }
                    else
                    {
                        cmre.MethodName = "Row";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Top));
                    }

                    break;

                case GridRangeInfoType.Cols:
                    if (range.Left != range.Right)
                    {
                        cmre.MethodName = "Cols";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Left));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Right));
                    }
                    else
                    {
                        cmre.MethodName = "Col";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Left));
                    }

                    break;

                case GridRangeInfoType.Cells:
                    if (range.Width > 1 || range.Height > 1)
                    {
                        cmre.MethodName = "Cells";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Top));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Left));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Bottom));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Right));
                    }
                    else
                    {
                        cmre.MethodName = "Cell";
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Top));
                        cmie.Parameters.Add(this.SerializeToExpression(manager, range.Left));
                    }

                    break;
            }

            return cmie;
        }

        // Properties.

        /// <internalonly/>
        /// <summary>Gets default. Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static GridRangeInfoCodeDomSerializer Default
        {
            get
            {
                if (GridRangeInfoCodeDomSerializer.defaultSerializer == null)
                {
                    GridRangeInfoCodeDomSerializer.defaultSerializer = new GridRangeInfoCodeDomSerializer();
                }

                return GridRangeInfoCodeDomSerializer.defaultSerializer;
            }
        }
        // Fields.
        private static GridRangeInfoCodeDomSerializer defaultSerializer;
    }

#if SyncfusionFramework2_0

    internal class GridRangeInfoListEditor : Form
    {
        RangesList ranges;
        private Syncfusion.Windows.Forms.Grid.GridDataBoundGrid gridDataBoundGrid1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.Label label1;

        public GridRangeInfoListEditor(RangesList ranges)
        {
            InitializeComponent();

            this.ranges = ranges;
            this.gridDataBoundGrid1.DataSource = ranges;
            this.gridDataBoundGrid1.AllowResizeToFit = false;
            this.gridDataBoundGrid1.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.SystemTheme;
            this.gridDataBoundGrid1.ThemesEnabled = true;
            this.gridDataBoundGrid1.Model.ColWidths[0] = 15;
            this.gridDataBoundGrid1.Model.ColWidths[1] = this.gridDataBoundGrid1.Width - this.gridDataBoundGrid1.Model.ColWidths[0];
            this.gridDataBoundGrid1.KeyDown += new KeyEventHandler(gridDataBoundGrid1_KeyDown);

            this.gridDataBoundGrid1.Model.HideRows[0] = true;
            this.gridDataBoundGrid1.TableStyle.Font = new GridFontInfo(this.Font);
            this.gridDataBoundGrid1.CurrentCellKeyDown += new KeyEventHandler(gridDataBoundGrid1_CurrentCellKeyDown);
            this.gridDataBoundGrid1.CurrentCell.MoveTo(1, 1);
            this.gridDataBoundGrid1.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
            this.gridDataBoundGrid1.Model.Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.GrayWhenLostFocus;
            this.gridDataBoundGrid1.Model.Rows.DefaultSize += 2;

            this.gridDataBoundGrid1.Model.CellModels.Add("RangeDropDown", new GridRangeInfoCellModel(this.gridDataBoundGrid1.Model, new GridRangeInfoDropDownUserControl(GridRangeInfo.Empty)));
            this.gridDataBoundGrid1.Model.ColStyles[1].CellType = "RangeDropDown";
            this.gridDataBoundGrid1.Model.ColStyles[1].ShowButtons = GridShowButtons.ShowCurrentCell;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridDataBoundGrid1 = new Syncfusion.Windows.Forms.Grid.GridDataBoundGrid();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataBoundGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridDataBoundGrid1
            // 
            this.gridDataBoundGrid1.AllowDragSelectedCols = true;
            this.gridDataBoundGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridDataBoundGrid1.Location = new System.Drawing.Point(12, 46);
            this.gridDataBoundGrid1.Name = "gridDataBoundGrid1";
            this.gridDataBoundGrid1.OptimizeInsertRemoveCells = true;
            this.gridDataBoundGrid1.ShowCurrentCellBorderBehavior = Syncfusion.Windows.Forms.Grid.GridShowCurrentCellBorder.GrayWhenLostFocus;
            this.gridDataBoundGrid1.Size = new System.Drawing.Size(130, 273);
            this.gridDataBoundGrid1.SmartSizeBox = false;
            this.gridDataBoundGrid1.SortBehavior = Syncfusion.Windows.Forms.Grid.GridSortBehavior.None;
            this.gridDataBoundGrid1.TabIndex = 0;
            this.gridDataBoundGrid1.Text = "gridDataBoundGrid1";
            this.gridDataBoundGrid1.UseListChangedEvent = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(12, 325);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(107, 325);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ranges";
            // 
            // GridRangeInfoListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 358);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.gridDataBoundGrid1);
            this.Name = "GridRangeInfoListEditor";
            this.Text = "RangeListEditor";
            ((System.ComponentModel.ISupportInitialize)(this.gridDataBoundGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        protected override void OnEnter(EventArgs e)
        {
            this.ActiveControl = this.gridDataBoundGrid1;

            base.OnEnter(e);
        }

        void gridDataBoundGrid1_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (!gridDataBoundGrid1.CurrentCell.IsEditing)
                {
                    if (this.gridDataBoundGrid1.Selections.Ranges.FilterRangeType(GridRangeInfoType.Rows).Count == 0)
                    {
                        int row = this.gridDataBoundGrid1.CurrentCell.RowIndex;
                        this.gridDataBoundGrid1.CurrentCell.MoveDown();
                        this.gridDataBoundGrid1.DeleteRecordsAtRowIndex(row, row);
                        e.Handled = true;
                    }
                }
            }
        }

        void gridDataBoundGrid1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.gridDataBoundGrid1.CurrentCell.ConfirmChanges();
                this.gridDataBoundGrid1.CurrentCell.MoveDown();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (!gridDataBoundGrid1.CurrentCell.IsEditing)
                {
                    if (this.gridDataBoundGrid1.Selections.Ranges.FilterRangeType(GridRangeInfoType.Rows).Count == 0)
                    {
                        int row = this.gridDataBoundGrid1.CurrentCell.RowIndex;
                        this.gridDataBoundGrid1.CurrentCell.MoveDown();
                        this.gridDataBoundGrid1.DeleteRecordsAtRowIndex(row, row);
                        e.Handled = true;
                    }
                }
            }
        }
    }

    internal class GridRangeInfoItem
    {
        GridRangeInfo range = new GridRangeInfo();

        public GridRangeInfoItem(GridRangeInfo range)
        {
            Range = range.ToString();
        }

        public GridRangeInfoItem()
        {
        }

        public GridRangeInfo GetRangeInfo()
        {
            return GridRangeInfo.Parse(Range);
        }

        public string Range
        {
            get
            {
                return range.ToString();
            }

            set
            {
                range = GridRangeInfo.Parse(value);
            }
        }
    }

    internal class RangesList : BindingList<GridRangeInfoItem>
    {
    }

    internal class GridRangeInfoCellModel : GridTextBoxCellModel
    {
        internal GridRangeInfoDropDownUserControl ddUser;

        public GridRangeInfoCellModel(GridModel grid, GridRangeInfoDropDownUserControl ddUser)
            : base(grid)
        {
            this.ddUser = ddUser;
            ButtonBarSize = new Size(grid.Rows.DefaultSize - 2, grid.Rows.DefaultSize - 2);
        }

        ////note that this method create our new derived renderer
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridRangeInfoCellRenderer(control, this);
        }
    }

    internal class GridRangeInfoCellRenderer : GridTextBoxCellRenderer
    {
        internal GridRangeInfoDropDownUserControl ddUser;

        public GridRangeInfoCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.ddUser = ((GridRangeInfoCellModel)this.Model).ddUser;
            this.ddUser.Visible = false;
            DropDownPart = new GridDropDownCellImp(this);
            DropDownButton = new GridCellComboBoxButton(this);

            ////hook the usercontrol save and cancel events...
            this.ddUser.UserControlSave += new EventHandler(user_Save);
            this.ddUser.UserControlCancel += new EventHandler(user_Cancel);
        }

        public new GridDropDownContainer DropDownContainer
        {
            get { return (GridDropDownContainer)base.DropDownContainer; }
        }

        ////here we add the user control to the popup window.
        protected override void InitializeDropDownContainer()
        {
            base.InitializeDropDownContainer();

            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.Controls.Add(this.ddUser);
            }
        }

        ////used to setup the dropdown before it is shown. The RaiseCurrentCellShowingDropDown
        ////code is commeted out as it requires teh 2.0 code base. This commented code allow
        ////for an event that might cancel teh drop or set its size.
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            GridCurrentCell cc = this.Grid.CurrentCell;

            this.DropDownContainer.Size = ddUser.Size;
            this.ddUser.SetValuesFromString(this.ControlText);
            ddUser.Visible = true;
        }

        ////used to change teh ControlValue if dropdown was closed with Done.
        public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (e.PopupCloseType == PopupCloseType.Done)
            {
                if (this.NotifyCurrentCellChanging())
                {
                    ControlValue = this.ddUser.GetValuesToString();
                    this.NotifyCurrentCellChanged();
                }
            }

            Grid.InvalidateRange(GridRangeInfo.Cell(RowIndex, ColIndex), GridRangeOptions.MergeCoveredCells); // Merge all cells

            base.DropDownContainerCloseDropDown(sender, e);
        }

        public override bool IsRelatedControl(Control control, bool askPopupParent)
        {
            if (control != null && control.GetType().Name.IndexOf("GridViewListBox") != -1)
            {
                return true;
            }

            return base.IsRelatedControl(control, askPopupParent);
        }

        ////handler for the user control Save event
        private void user_Save(object sender, EventArgs e)
        {
            // closethe dropdown with a Done setting
            this.Grid.CurrentCell.CloseDropDown(Syncfusion.Windows.Forms.PopupCloseType.Done);
            this.Grid.CurrentCell.MoveDown();
        }

        ////handler for the user control Cancel event
        private void user_Cancel(object sender, EventArgs e)
        {
            // close the dropdown with a Canceled setting
            this.Grid.CurrentCell.CloseDropDown(Syncfusion.Windows.Forms.PopupCloseType.Canceled);
        }

        ////used to move the grid value into the cell control
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            base.OnInitialize(rowIndex, colIndex);
            ddUser.SetValuesFromString(this.Grid.Model[rowIndex, colIndex].Text);
        }
    }

    internal class GridRangeInfoDropDownUserControl : System.Windows.Forms.UserControl
    {
        public event EventHandler UserControlSave;
        public event EventHandler UserControlCancel;
        GridRangeInfo range;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CommandsVisibleIfAvailable = false;
            this.propertyGrid1.Location = new System.Drawing.Point(12, 12);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(198, 190);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(24, 208);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(122, 208);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // GridRangeInfoDropDownUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "GridRangeInfoDropDownUserControl";
            this.Size = new System.Drawing.Size(222, 246);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;

        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
            }
        }

        public GridRangeInfoDropDownUserControl(GridRangeInfo range)
        {
            InitializeComponent();

            this.range = range;
            this.propertyGrid1.CommandsVisibleIfAvailable = false;
            this.propertyGrid1.PropertySort = PropertySort.NoSort;
            this.propertyGrid1.SelectedObject = this.range;
            this.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
        }

        void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.changed = true;
            this.range = (GridRangeInfo)propertyGrid1.SelectedObject;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            bool saveChanged = this.changed;
            this.changed = false;
            if (saveChanged && UserControlSave != null)
            {
                UserControlSave(this, EventArgs.Empty);
            }
            else if (UserControlCancel != null)
            {
                UserControlCancel(this, EventArgs.Empty);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            changed = false;
            if (UserControlCancel != null)
            {
                UserControlCancel(this, EventArgs.Empty);
            }
        }

        private bool changed = false;

        public void SetValuesFromString(string val)
        {
            this.range = GridRangeInfo.Parse(val);
            this.propertyGrid1.SelectedObject = range;
            changed = false;
        }

        public string GetValuesToString()
        {
            return this.range.ToString();
        }
    }
#endif
}

