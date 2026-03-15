//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableDropDownList.cs" company="syncfusion">
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
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using ITreeTableSummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Defines the model / data part of a drop-down ListControl-like grid.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableDropDownListCellModel"/> can serve as model for several <see cref="GridTableDropDownListCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableDropDownListCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableDropDownListCellModel : GridDropDownGridListControlCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableDropDownListCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableDropDownListCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableDropDownListCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableDropDownListCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableDropDownListCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }
#if ASPNET
#else
        /// <override/>
        /// <summary>
        /// Creates a cell renderer for this cell model.
        /// </summary>
        /// <param name="control">The grid control base.</param>
        /// <returns>returns the Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableDropDownListCellRenderer(control, this);
        }
#endif

        /// <override/>
        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="value">Cell value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>Formatted text.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridTableCellStyleInfoIdentity id = GridTableCellStyleInfo.GetTableCellIdentity(style);

            GridCellTextEventArgs ea = new GridCellTextEventArgs(string.Empty, style, value, textInfo);
            Grid.RaiseQueryCellFormattedText(ea);
            if (ea.Handled)
            {
                return ea.Text;
            }
            else
            {
                NumberFormatInfo nfi = null;
                CultureInfo ci = style.CultureInfo;

                FieldDescriptor columnFieldDescriptor = id.Column.FieldDescriptor;
                FieldDescriptor relatedFieldDescriptor = columnFieldDescriptor.GetRelatedDescriptor();
                FieldDescriptor nestedRelatedFieldDescriptor = columnFieldDescriptor.GetNestedRelatedDescriptor();
                Type propertyType = relatedFieldDescriptor.GetPropertyType();
                RelationDescriptor rd = columnFieldDescriptor.GetRelation();

                if (relatedFieldDescriptor == null
                    || ((rd.RelationKind == RelationKind.ForeignKeyReference || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                    && (propertyType == typeof(System.Byte[]) || rd.RelationKeys.Count == 0)))
                {
                    //// ForeignListItems
                    return string.Empty;
                }

                //// Replace the last relation key (the foreign key id with the value that was passed in).
                Record record = Record.GetParentRecord(id.DisplayElement);

                if (columnFieldDescriptor.IsComplexPropertyField())
                {
                    value = columnFieldDescriptor.GetValueFromDataRow(value);
                    propertyType = columnFieldDescriptor.GetPropertyType();
                }
                else if (rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                {
                    ChildTable childTable = record.GetRelatedChildTable(rd);

                    if (childTable == null)
                    {
                        return string.Empty;
                    }

                    // Now get the value from the related childTable.
                    Table relatedTable = childTable.ParentTable;
                    ITreeTableSummary[] summaries = childTable.GetSummaries(relatedTable);
                    SummaryDescriptor sd = columnFieldDescriptor.GetRelatedSummaryDescriptor();
                    if (sd == null)
                    {
                        Console.WriteLine("Could not find summary for " + columnFieldDescriptor.Name + " in " + relatedTable.ParentTableDescriptor.Name);
                    }

                    int index = relatedTable.TableDescriptor.Summaries.IndexOf(sd);
                    if (index != -1)
                    {
                        VectorSummary summary = summaries[index] as VectorSummary;

                        StringBuilder sb = new StringBuilder();
                        for (int n = 0; n < summary.Values.Length; n++)
                        {
                            if (n > 0)
                            {
                                sb.Append(", ");
                            }

                            sb.Append(summary.Values[n] == null || summary.Values[n] is DBNull ? "null" : summary.Values[n].ToString());
                        }

                        value = sb.ToString();
                    }

                    propertyType = typeof(string);
                }            
                else
                {
                    //// ForeignListItems
                    Record relatedRecord = record.GetRelatedRecord(rd, rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField, value);

                    if (relatedRecord == null)
                    {
                        return string.Empty;
                    }

                    //// Now get the value from the related record.
                    value = relatedRecord.GetValue(relatedFieldDescriptor);
                    propertyType = nestedRelatedFieldDescriptor.GetPropertyType();
                }

                return GridCellValueConvert.FormatValue(value, propertyType, style.Format, ci, nfi);
            }
        }
    }
#if ASPNET
#else
    /// <summary>
    /// Defines the renderer part of a drop-down ListControl-like grid that lets users drop-down a grid
    /// with information about a related foreign key table that was set up using a RelationKind.ForeignKeyReference,
    /// RelationKind.ListItemReference or RelationKind.ForeignKeyKeyWords relation.
    /// </summary>    
    public class GridTableDropDownListCellRenderer : GridDropDownGridListControlCellRenderer
    {
        GridTableControl dropDownTableControl = null;
        Panel dropdownPanel;
        ////Button modifyButton = null;
        ButtonAdv modifyButton = null;
        bool isTableControlDropDown = false;

        GridTableCellStyleInfo tableStyleInfo;
        GridTableCellStyleInfoIdentity id;
        FieldDescriptor fd;
        FieldDescriptor relatedFieldDescriptor;
        GridTable table;
        GridTableModel tableModel;

        RelationDescriptorCollection tableDescriptorRelations;

        /// <summary>
        /// Initializes a new GridTableDropDownListCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableDropDownListCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            ITableProvider igetTable = (ITableProvider)grid;
            tableDescriptorRelations = igetTable.GetTable().TableDescriptor.Relations;

            ////            if (grid is GridTableControl)
            ////            {
            ////                GridTableControl tableControl = (GridTableControl) grid;
            ////
            ////                tableDescriptorRelations = tableControl.TableDescriptor.Relations.Inner;
            ////            }
            ////            else if (grid is GridListControlChild)
            ////            {
            ////                GridListControlChild glcc = (GridListControlChild) grid;
            ////                GridGroupTypedListRecordsCollection gls = glcc.ListControl.DataSource as GridGroupTypedListRecordsCollection;
            ////                if (gls != null && gls.TableDescriptor != null)
            ////                    tableDescriptorRelations = gls.TableDescriptor.Relations.Inner;
            ////            }

            tableDescriptorRelations.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(Relations_Changed);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tableDescriptorRelations.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(Relations_Changed);
                tableDescriptorRelations = null;
                ResetDropDownTableControl();
            }

            base.Dispose(disposing);
        }

        void ResetDropDownTableControl()
        {
            if (this.dropDownTableControl != null)
            {
                dropDownTableControl.Table.GroupCollapsed -= new GroupEventHandler(Table_GroupCollapsed);
                dropDownTableControl.Table.GroupExpanded -= new GroupEventHandler(Table_GroupExpanded);
                dropDownTableControl.Table.CurrentRecordContextChange -= new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
                dropDownTableControl.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(dropDownTableControl_PrepareViewStyleInfo);

                this.dropDownTableControl.Dispose();
                this.dropDownTableControl = null;
                tableStyleInfo = null;
                id = null;
                fd = null;
                relatedFieldDescriptor = null;
                table = null;
                tableModel = null;

                dropdownPanel.Dispose();
                dropdownPanel = null;
                modifyButton = null;
            }

            isTableControlDropDown = false;
        }

        private void Relations_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            if ((e.Action != ListPropertyChangedType.ItemPropertyChanged
                && e.Action != ListPropertyChangedType.Move
                && e.Action != ListPropertyChangedType.Insert
                && e.Action != ListPropertyChangedType.Add)
                || (e.Action == ListPropertyChangedType.ItemPropertyChanged &&
                (e.Property == "RelationKind" || e.Property == "ChildTableName"
                    || e.Property == "Name" || e.Property == "MappingName")))
            {
                ResetDropDownTableControl();
                ResetListControlPart();
            }
        }

        #region Strong Typed Property Overrides
        /// <summary>
        /// The parent <see cref="GridTableControl"/> that this cell renderer belongs to.
        /// </summary>
        public GridTableControl GridTableControl
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }

        /// <summary>
        /// The <see cref="GridNestedTableControlCellModel"/> that this cell renderer belongs to.
        /// </summary>
        public new GridTableDropDownListCellModel Model
        {
            get
            {
                return (GridTableDropDownListCellModel)base.Model;
            }
        }

        #endregion

        /// <override/>
        protected override void OnEnsureListControlPart()
        {
            // Make sure text box is disable if this is an Image cell.
            GridTableCellStyleInfoIdentity id = GridTableCellStyleInfo.GetTableCellIdentity(StyleInfo);
            if (id.Column != null)
            {
                FieldDescriptor fd = id.Column.FieldDescriptor;
                FieldDescriptor rfd = fd.GetNestedRelatedDescriptor();
                if (rfd != null)
                {
                    this.DisableTextBox |= rfd.GetPropertyType() == typeof(System.Byte[]);
                }
            }
        }

        /// <summary>
        /// Creates the grid that is displayed in the drop-down window.
        /// </summary>
        /// <returns>A <see cref="GridListControl"/> to be placed in the drop-down container.</returns>
        protected override GridListControl CreateListControlPart()
        {
            GridDropDownGridListControlPart gc = new GridTableDropDownListControlPart();
            // gc.DropDownRows = dropDownListRows; - not needed (using DockStyle.Fill)
            gc.Grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(listControlPartGrid_PrepareViewStyleInfo);
            gc.AutoSizeColumns = false;
            gc.Dock = DockStyle.Fill;
            gc.FillLastColumn = true;
            gc.Grid.VScrollPixel = true;
            gc.Grid.HScrollPixel = true;
            return gc;
        }
        
        /// <summary>
        /// A reference to the modify button displayed in the top right corner of the dropdown grid
        /// when the underlying table is editable.
        /// </summary>
        public ButtonAdv ModifyButton
        {
            get
            {
                if (modifyButton == null)
                {
                    modifyButton = new ButtonAdv();
                    Bitmap pencil = GridGroupingBitmaps.IconPainter.GetBitmap("SFPENCIL.BMP");
                    modifyButton.Image = pencil;
                    modifyButton.FlatStyle = FlatStyle.Popup;
                    modifyButton.Size = new Size(pencil.Width + 4, pencil.Height + 4);
                    modifyButton.TabStop = false;
                    modifyButton.Click += new EventHandler(modifyButton_Click);
                    modifyButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    modifyButton.Visible = false;
                    modifyButton.KeepFocusRectangle = false;
                    this.ListControlPart.Controls.Add(modifyButton);
                }

                return modifyButton;
            }
        }

        ////        /// <override/>
        ////        protected override bool OnSaveChanges()
        ////        {
        ////            if (allowChanges && CurrentCell.IsModified)
        ////            {
        ////                //// Save Control Value directly - do not call base (which ends up calling style.FormattedText = this.TextBoxText;)
        ////                GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
        ////                style.CellValue = this.ControlValue;
        ////                return true;
        ////            }
        ////            return false;
        ////        }

        bool allowChanges = true;

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            tableStyleInfo = StyleInfo as GridTableCellStyleInfo;
            id = tableStyleInfo.TableCellIdentity;
            if (id.Column != null)
            {
                fd = id.Column.FieldDescriptor;
                relatedFieldDescriptor = fd != null ? fd.GetNestedRelatedDescriptor() : null;
                table = (GridTable)id.Table.RelatedTables[fd.GetRelation().Name];

                if (tableModel == null)
                {
                    tableModel = id.Table.Engine.GroupingControl.CreateTableModel();
                }

                table.TableModel = tableModel;
                tableModel.Table = table;
            }

            //// ForeignListItems
            if (fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                this.DisableTextBox = true;
                this.ShowTableControl(true);
                table.FilteredChildTable = Record.GetParentRecord(id.DisplayElement).GetRelatedChildTable(fd.GetRelation());
                TextBoxText = tableStyleInfo.FormattedText;
                allowChanges = false;
                return;
            }

            this.ShowTableControl(false);
            allowChanges = true;

            InitGroupTypedListRecordsCollection();

            base.OnInitialize(rowIndex, colIndex);
        }

        void InitGroupTypedListRecordsCollection()
        {
            GroupTypedListRecordsCollection coll = (GroupTypedListRecordsCollection)Model.GetDataSource(StyleInfo);
            if (coll != null)
            {
                table.FilteredChildTable = coll.Group as ChildTable;
                ////GridGroupOptionsStyleInfo go = ((GridChildTable) coll.Group).GroupOptions;
                ////                Record r = id.DisplayElement.ParentRecord.GetRelatedRecord(fd.GetRelation());
                ////                int index = coll.Group.FlattenedRecords.IndexOf(r);
                ////                this.ListControlPart.SelectedIndex = index;
            }
            else
            {
                table.FilteredChildTable = null;
            }
        }
        
        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);

            // Manually draw image cells.
            GridTableCellStyleInfoIdentity id = GridTableCellStyleInfo.GetTableCellIdentity(style);
            if (id.Column != null)
            {
                ////TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, style.CellValue);
                FieldDescriptor fd = id.Column.FieldDescriptor;
                FieldDescriptor relatedFieldDescriptor = fd != null ? fd.GetRelatedDescriptor() : null;
                FieldDescriptor nestedRelatedFieldDescriptor = fd != null ? fd.GetNestedRelatedDescriptor() : null;

                if (nestedRelatedFieldDescriptor != null && nestedRelatedFieldDescriptor.GetPropertyType() == typeof(System.Byte[]))
                {
                    Record r = null;
                    object cellValue = null;

                    if (this.IsEditing && CurrentCell.IsModified && Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && ListControlPart.SelectedIndex != -1)
                    {
                        r = (Record)ListControlPart.Items[ListControlPart.SelectedIndex];
                        cellValue = r.GetValue(relatedFieldDescriptor);
                    }
                    else
                    {
                        r = Record.GetParentRecord(id.DisplayElement);
                        cellValue = r.GetValue(fd);
                    }

                    Image image = ImageUtil.ConvertToImage(cellValue);
                    Rectangle clipRectangle = this.GetCellBoundsCoreInt(rowIndex, colIndex, false);
                    bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                    GridImageUtil.DrawImage(image, clipRectangle, g, clientRectangle, style, isTextRightToLeft);
                }
            }
        }

        /// <override/>
        protected override void SynchronizeDisplayText(int index)
        {
            if (allowChanges && index >= 0 && index < ListControlPart.Items.Count
                && id.Column != null && this.NotifyCurrentCellChanging())
            {
                Record item = (Record)ListControlPart.Items[index];
                object value = this.GetPrimaryKeyValue(item);
                this.ApplyPrimaryKeyValue(value);
            }
        }

        /// <override/>
        /// <summary>Lets you customize and redirect the mouse wheel behavior to a cell renderer.</summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        /// <returns>True if the parent grid should not be scrolled; False otherwise.</returns>
        public override bool ProcessMouseWheel(MouseEventArgs e)
        {
            if (this.isTableControlDropDown)
            {
                this.dropDownTableControl.MouseControllerDispatcher.ProcessCancelMode();  // Cancel Mouse tracking
                this.dropDownTableControl.RaiseMouseWheel(e);
                return this.IsDroppedDown;
            }

            return base.ProcessMouseWheel(e);
        }

        /// <override/>
        /// <summary>
        /// Occurs when the popup child is dropped down and made visible.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            ModifyButton.Location = new Point(ListControlPart.Grid.ClientRectangle.Right - modifyButton.Width - 2, 2);

            base.DropDownContainerShowedDropDown(sender, e);

            if (this.isTableControlDropDown)
            {
                this.dropDownTableControl.Focus();
            }
            else if (fd.GetRelation().RelationKind != RelationKind.ForeignKeyKeyWords)
            {
                //// Directly set list control index from ControlValue
                this.ListControlPart.SelectedIndex = Model.FindValue(StyleInfo, ControlValue);
            }
        }

        /// <override/>
        /// <summary>Handles the MouseUp event for the list box and closes the drop-down.</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void ListControlMouseUp(object sender, MouseEventArgs e)
        {
            if (this.isTableControlDropDown || fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                return;
            }

            base.ListControlMouseUp(sender, e);
        }

        /// <override/>
        /// <summary>
        /// Indicates that the popup child was closed.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (this.isTableControlDropDown || fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                ////                if (e.PopupCloseType == PopupCloseType.Canceled)
                ////                    this.table.CancelEdit();
                ////                else
                this.table.EndEdit();
                this.table.ResetCurrentRecord();
            }

            this.ShowTableControl(false);

            base.DropDownContainerCloseDropDown(sender, e);

            if (!allowChanges || fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                CurrentCell.Refresh();
                return;
            }

            if (!CurrentCell.IsInEndEdit && CurrentCell.IsModified)
            {
                TextBox.SelectAll();
                CurrentCell.ConfirmChanges();
            }
        }

        ////        ///// <override/>
        ////        protected override void SetControlValue(object value, bool initControlText)
        ////        {
        ////            if (this.ListControlPart.ValueMember == string.Empty && value is Record)
        ////                value = ((Record) value).GetData();
        ////            base.SetControlValue(value, initControlText);
        ////        }

        void OnShowingDropDownTableControl()
        {
            dropDownTableControl.CurrentCell.ResetCurrentCellWithoutDeactivate();
            GroupTypedListRecordsCollection coll = (GroupTypedListRecordsCollection)this.ListControlPart.DataSource;
            int index = this.allowChanges ? Model.FindValue(tableStyleInfo, ControlValue) : -1;
            if (dropDownTableControl.VScrollPixel)
            {
                dropDownTableControl.InternalSetCurrentVScrollPixelPos(dropDownTableControl.GetVScrollPixelMinimum());
            }
            else
            {
                dropDownTableControl.InternalSetTopRow(dropDownTableControl.VScrollBar.Minimum);
            }

            dropDownTableControl.InternalSetCurrentHScrollPixelPos(dropDownTableControl.GetHScrollPixelMinimum());
            if (index != -1)
            {
                table.CurrentElement = coll[index];
            }

            dropDownTableControl.synchronizeGridShouldRestoreCurrentCell = true;

            //// fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            if (this.selectButton != null)
            {
                this.selectButton.Visible = this.allowChanges;
            }
        }

        /// <override/>
        /// <summary>Occurs when the drop down container is about to be shown</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            TextBox.SelectAll();

            if (fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords && !isTableControlDropDown)
            {
                ShowTableControl(true);
            }

            if (this.isTableControlDropDown)
            {
                OnShowingDropDownTableControl();
            }
            else
            {
                IGridGroupOptionsSource igs = null;

                GridStyleInfo style = StyleInfo;
                object ds = Model.GetDataSource(style);
                if (ds != null)
                {
                    int count = ((IList)ds).Count;
                    this.ListControlPart.SetDataBinding(this.ListControlPart.BindingContext, ds, style.DisplayMember, style.ValueMember);
                    GroupTypedListRecordsCollection coll = ds as GroupTypedListRecordsCollection;
                    if (coll != null)
                    {
                        igs = coll.Group as IGridGroupOptionsSource;
                    }
                }

                this.ListControlPart.ShowColumnHeader = igs == null || igs.GroupOptions.ShowColumnHeaders;

                base.DropDownContainerShowingDropDown(sender, e);

                if (table != null && ((table.SourceListAllowEdit && table.TableDescriptor.AllowEdit)
                    || (table.SourceListAllowNew && table.TableDescriptor.AllowNew)))
                {
                    ModifyButton.Visible = true;
                    SetModifyButtonStyle(ModifyButton, table.TableOptions.GridVisualStyles);
                    ModifyButton.BringToFront();
                }
                else
                {
                    ModifyButton.Visible = false;
                }

                ListControlPart.BackColor = this.table.Appearance.RecordFieldCell.BackColor;
                ListControlPart.ForeColor = this.table.Appearance.RecordFieldCell.TextColor;
            }

            AutoSizeHeightOfGrid();
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            GridTableCellStyleInfoIdentity id = GridTableCellStyleInfo.GetTableCellIdentity(e.Style);
            if (id.Column != null)
            {
                FieldDescriptor fd = id.Column.FieldDescriptor;
                FieldDescriptor relatedFieldDescriptor = fd != null ? fd.GetNestedRelatedDescriptor() : null;

                if (relatedFieldDescriptor != null && relatedFieldDescriptor.GetPropertyType() == typeof(System.Byte[]))
                {
                    //// Avoid an exception when setting ListControl.DisplayMember to an Image property.
                    e.Style.DropDownStyle = GridDropDownStyle.Exclusive;
                    e.Style.DisplayMember = e.Style.ValueMember;
                }
            }

            base.OnPrepareViewStyleInfo(e);
        }

        object GetPrimaryKeyValue(Record r)
        {
            object value = DBNull.Value;
            if (!(r is AddNewRecord))
            {
                SortColumnDescriptorCollection pks = table.TableDescriptor.PrimaryKeyColumns;
                if (pks.Count > 0)
                {
                    value = r.GetValue(pks[pks.Count - 1]);
                }
                else
                {
                    value = r.GetData(); // this is the case for RelationKind.ItemListReference
                }
            }

            return value;
        }

        /// <override/>
        protected override void OnSetControlText(string text)
        {
            try
            {
                if (!this.isTableControlDropDown)
                {
                    base.OnSetControlText(text);
                }
                else
                {
                    if (!this.InTextBoxChanged)
                    {
                        TextBoxText = text;
                    }
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        void ApplyPrimaryKeyValue(object value)
        {
            if (!allowChanges)
            {
                return;
            }

            if (!HasControlValue || value != ControlValue)
            {
                ////                if (relatedFieldDescriptor != null && relatedFieldDescriptor.GetPropertyType() != typeof(System.Byte[]))
                ////                {
                ////                    TextBoxText = Model.GetFormattedText(tableStyleInfo, value, 0);
                ////                }
                ControlValue = value;
                CurrentCell.IsModified = true;
                CurrentCell.Invalidate();
            }
        }

        /// <summary>
        /// This method is called from GridCurrentCell.Validate after GridCurrentCell.Validating event has been
        /// fired. The default version checks if the active text fits any criteria as specified
        /// in the style object: It can be parsed into a cell value and meets GridCellValidateValueInfo criteria.
        /// </summary>
        /// <returns>
        /// True if the modified text is valid; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnValidate()
        {
            if (this.HasControlValue)
            {
                return true;
            }

            return base.OnValidate();
        }

        ////Button selectButton;
        ButtonAdv selectButton;
        ButtonAdv closeButton;

        void ShowTableControl(bool showTableControl)
        {
            if (id == null || id.Column == null)
            {
                return;
            }

            if (isTableControlDropDown == showTableControl && dropdownPanel != null)
            {
                return;
            }

            isTableControlDropDown = showTableControl;

            int height = 1000;
            int width = 1000;

            //// No need to worry about size here, just for anchoring purposes.
            //// AutoSizeHeightOfGrid will be called afterward.

            GridDropDownContainer dropDownContainer = DropDownContainer;
            dropDownContainer.CausesValidation = false;

            if (showTableControl)
            {
                if (dropdownPanel == null)
                {
                    tableModel.Table = table;

                    Console.WriteLine(this.DropDownContainer.Controls.Count);

                    dropdownPanel = new Panel();
                    dropdownPanel.BorderStyle = BorderStyle.None;
                    dropdownPanel.ClientSize = new Size(width, height + 40);
                    dropdownPanel.CausesValidation = false;

                    EnsureDropDownTableControl(tableModel);
                    dropDownTableControl.Size = new Size(width, height);
                    dropDownTableControl.Anchor = ~AnchorStyles.None;
                    dropDownTableControl.CausesValidation = false;

                    ////Button closeButton = new Button();
                    closeButton = new ButtonAdv();
                    closeButton.Text = "Close";
                    closeButton.Size = new Size(60, 20);
                    closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    closeButton.Visible = true;
                    closeButton.CausesValidation = false;
                    closeButton.KeepFocusRectangle = false;

                    ////selectButton = new Button();                // ForeignListItems
                    selectButton = new ButtonAdv();
                    selectButton.Size = new Size(60, 20);
                    selectButton.Text = "Select";
                    selectButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    selectButton.Visible = true;
                    selectButton.CausesValidation = false;
                    selectButton.KeepFocusRectangle = false;

                    dropdownPanel.Controls.Add(dropDownTableControl);
                    dropdownPanel.Controls.Add(closeButton);
                    dropdownPanel.Controls.Add(selectButton);
                    closeButton.Location = new Point(dropdownPanel.Width - closeButton.Width - 10, dropdownPanel.Height - closeButton.Height - 10);
                    selectButton.Location = new Point(closeButton.Left - selectButton.Width - 10, dropdownPanel.Height - closeButton.Height - 10);

                    closeButton.Click += new EventHandler(closeButton_Click);
                    selectButton.Click += new EventHandler(selectButton_Click);

                    SetDropDownStyle(closeButton, selectButton, dropdownPanel, tableModel.Options.GridVisualStyles);
                }
                else
                {
                    dropdownPanel.ClientSize = new Size(width, height + 40);
                    tableModel.Table = table;
                }

                dropDownContainer.Size = new Size(width + 2, height + 40);
                dropDownContainer.Controls.Clear();
                dropDownContainer.Controls.Add(dropdownPanel);

                dropdownPanel.Anchor = ~AnchorStyles.None;
                dropDownContainer.BorderStyle = BorderStyle.FixedSingle;

                SetDropDownStyle(closeButton, selectButton, dropdownPanel, tableModel.Options.GridVisualStyles);

                dropDownTableControl.Table.GroupCollapsed += new GroupEventHandler(Table_GroupCollapsed);
                dropDownTableControl.Table.GroupExpanded += new GroupEventHandler(Table_GroupExpanded);
                dropDownTableControl.Table.CurrentRecordContextChange += new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
                dropDownTableControl.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(dropDownTableControl_PrepareViewStyleInfo);
            }
            else
            {
                if (dropDownTableControl != null)
                {
                    dropDownTableControl.Table.GroupCollapsed -= new GroupEventHandler(Table_GroupCollapsed);
                    dropDownTableControl.Table.GroupExpanded -= new GroupEventHandler(Table_GroupExpanded);
                    dropDownTableControl.Table.CurrentRecordContextChange -= new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
                    dropDownTableControl.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(dropDownTableControl_PrepareViewStyleInfo);
                }

                dropDownContainer.Controls.Clear();
                dropDownContainer.Controls.Add(this.ListControlPart);

                dropDownContainer.BorderStyle = BorderStyle.None;
            }
        }

        private void SetModifyButtonStyle(ButtonAdv modifyButton, GridVisualStyles gridVisualStyles)
        {
            switch (gridVisualStyles)
            {
                case GridVisualStyles.Office2007Blue:
                    modifyButton.Appearance = ButtonAppearance.Office2007;
                    modifyButton.Office2007ColorScheme = Office2007Theme.Blue;
                    modifyButton.UseVisualStyle = true;
                    break;
                case GridVisualStyles.Office2007Black:
                    modifyButton.Appearance = ButtonAppearance.Office2007;
                    modifyButton.Office2007ColorScheme = Office2007Theme.Black;
                    modifyButton.UseVisualStyle = true;
                    break;
                case GridVisualStyles.Office2007Silver:
                    modifyButton.Appearance = ButtonAppearance.Office2007;
                    modifyButton.Office2007ColorScheme = Office2007Theme.Silver;
                    modifyButton.UseVisualStyle = true;
                    break;
                case GridVisualStyles.Office2003:
                    modifyButton.Appearance = ButtonAppearance.Office2003;
                    modifyButton.UseVisualStyle = true;
                    break;
                default:
                    modifyButton.Appearance = ButtonAppearance.WindowsXP;
                    modifyButton.UseVisualStyle = false;
                    modifyButton.BackColor = SystemColors.ControlLight;
                    break;
            }
        }

        private void SetDropDownStyle(ButtonAdv closeButton, ButtonAdv selectButton, Panel dropdownPanel, GridVisualStyles style)
        {
            switch (style)
            {
                case GridVisualStyles.Office2007Blue:
                    closeButton.Appearance = ButtonAppearance.Office2007;
                    closeButton.Office2007ColorScheme = Office2007Theme.Blue;
                    closeButton.UseVisualStyle = true;
                    closeButton.FlatStyle = FlatStyle.Standard;
                    selectButton.Appearance = ButtonAppearance.Office2007;
                    selectButton.Office2007ColorScheme = Office2007Theme.Blue;
                    selectButton.UseVisualStyle = true;
                    selectButton.FlatStyle = FlatStyle.Standard;
                    dropdownPanel.BackColor = Color.FromArgb(227, 239, 255);
                    break;
                case GridVisualStyles.Office2007Black:
                    closeButton.Appearance = ButtonAppearance.Office2007;
                    closeButton.Office2007ColorScheme = Office2007Theme.Black;
                    closeButton.UseVisualStyle = true;
                    closeButton.FlatStyle = FlatStyle.Standard;
                    selectButton.Appearance = ButtonAppearance.Office2007;
                    selectButton.Office2007ColorScheme = Office2007Theme.Black;
                    selectButton.UseVisualStyle = true;
                    selectButton.FlatStyle = FlatStyle.Standard;
                    dropdownPanel.BackColor = Color.FromArgb(240, 241, 242);
                    break;
                case GridVisualStyles.Office2007Silver:
                    closeButton.Appearance = ButtonAppearance.Office2007;
                    closeButton.Office2007ColorScheme = Office2007Theme.Silver;
                    closeButton.UseVisualStyle = true;
                    closeButton.FlatStyle = FlatStyle.Standard;
                    selectButton.Appearance = ButtonAppearance.Office2007;
                    selectButton.Office2007ColorScheme = Office2007Theme.Silver;
                    selectButton.UseVisualStyle = true;
                    selectButton.FlatStyle = FlatStyle.Standard;
                    dropdownPanel.BackColor = Color.FromArgb(240, 241, 242);
                    break;
                case GridVisualStyles.Office2003:
                    closeButton.Appearance = ButtonAppearance.Office2003;
                    closeButton.UseVisualStyle = true;
                    closeButton.FlatStyle = FlatStyle.Standard;
                    selectButton.Appearance = ButtonAppearance.Office2003;
                    selectButton.UseVisualStyle = true;
                    selectButton.FlatStyle = FlatStyle.Standard;
                    if (XPThemes.IsSilverThemeOn)
                    {
                        dropdownPanel.BackColor = Color.FromArgb(224, 223, 227);
                    }
                    else
                    {
                        dropdownPanel.BackColor = Color.FromArgb(236, 233, 216);
                    }

                    break;
                default:
                    closeButton.Appearance = ButtonAppearance.WindowsXP;
                    closeButton.FlatStyle = FlatStyle.System;
                    selectButton.Appearance = ButtonAppearance.WindowsXP;
                    selectButton.FlatStyle = FlatStyle.System;
                    dropdownPanel.BackColor = SystemColors.Control;

                    break;
            }
        }

        private void EnsureDropDownTableControl(GridTableModel tableModel)
        {
            if (dropDownTableControl == null)
            {
                dropDownTableControl = GridTableControl.GroupingControl.CreateTableControl(tableModel);
                dropDownTableControl.groupingControl = GridTableControl.GroupingControl;
                dropDownTableControl.BindingContext = new BindingContext();
                dropDownTableControl.BorderStyle = BorderStyle.None;
                dropDownTableControl.ThemesEnabled = this.Grid.ThemesEnabled;
                dropDownTableControl.HorizontalScrollTips = false;
                dropDownTableControl.VerticalScrollTips = false;
                dropDownTableControl.HorizontalThumbTrack = true;
                dropDownTableControl.VerticalThumbTrack = true;
                dropDownTableControl.HScrollBehavior = GridScrollbarMode.Disabled;
                dropDownTableControl.VScrollBehavior = GridScrollbarMode.Disabled;
                dropDownTableControl.HScroll = false;
                dropDownTableControl.VScroll = false;

                tableModel.Options.ControllerOptions = GridControllerOptions.ClickCells;
                tableModel.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
                tableModel.Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
                tableModel.Options.RefreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshRow;
                dropDownTableControl.DropDownContainerParent = this.DropDownContainer;
            }
            else
            {
                dropDownTableControl.Model = tableModel;
            }
        }

        /// <override/>
        protected override void InitializeDropDownContainer()
        {
            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.Controls.Add(this.ListControlPart);

                ModifyButton.Visible = false;
                ModifyButton.Location = new Point(ListControlPart.Grid.Right - modifyButton.Width - 2, 2);

                DropDownContainer.IgnoreDialogKey = true;
            }
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            Point p = ListControlPart.PointToScreen(Point.Empty);

            // Replace ListControl with TableControl in-place.
            this.DropDownContainer.SuspendLayout();
            this.ShowTableControl(true);
            AutoSizeHeightOfGrid();
            OnShowingDropDownTableControl();
            this.DropDownContainer.ResumeLayout();
        }

        private void Table_GroupCollapsed(object sender, GroupEventArgs e)
        {
            AutoSizeHeightOfGrid();
            this.dropDownTableControl.synchronizeGridShouldInvalidate = true;
            this.dropDownTableControl.synchronizeGridShouldnextUpdateScrollBars = true;
        }

        private void Table_GroupExpanded(object sender, GroupEventArgs e)
        {
            AutoSizeHeightOfGrid();
            this.dropDownTableControl.synchronizeGridShouldInvalidate = true;
            this.dropDownTableControl.synchronizeGridShouldnextUpdateScrollBars = true;
        }

        void AutoSizeHeightOfGrid()
        {
            GridControlBase grid;
            int height;

            if (this.isTableControlDropDown || fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                grid = dropDownTableControl;
                if (table.Engine.SupportsYAmount)
                {
                    height = (int)table.DisplayElements.YAmountCount + 2;
                }
                else
                {
                    height = (int)grid.GetRowRangeHeight(0, grid.Model.RowCount) + 2;
                }
            }
            else
            {
                grid = ListControlPart.Grid;
                grid.Model.RowHeights[0] = this.table.TableOptions.ColumnHeaderRowHeight;
                grid.Model.Rows.DefaultSize = this.table.TableOptions.RecordRowHeight;
                height = (int)this.ListControlPart.Grid.RowHeights.GetTotal(0, this.ListControlPart.Grid.RowCount) + 2;
            }

            bool vScroll = false;
            bool hScroll = false;

            if (isTableControlDropDown || fd.GetRelation().RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                height += 40;
            }

            tableModel.UpdateColumnWidths(true);
            int width = table.GetTotalWidthOfTable(false) + 4;

            Size maxSize = table.TableOptions.MaxDropDownTableSize;

            if (height > maxSize.Height)
            {
                width += SystemInformation.VerticalScrollBarWidth;
                height = maxSize.Height;
                vScroll = true;
            }

            if (width > maxSize.Width)
            {
                width = maxSize.Width;
                height += SystemInformation.VerticalScrollBarWidth;
                hScroll = true;
            }

            DropDownContainer.Size = new Size(width, height);
            DropDownContainer.PopupHost.Size = DropDownContainer.Size;

            if (vScroll)
            {
                grid.VScrollBehavior = GridScrollbarMode.Enabled | GridScrollbarMode.AutoScroll;
                grid.VScroll = true;
            }
            else
            {
                grid.VScrollBehavior = GridScrollbarMode.Disabled;
                grid.VScroll = false;

                // Sometimes the current selected item may have been set
                // as top row index before. Adjust the top row index if
                // dropdown actually can display all rows.
                grid.TopRowIndex = grid.GetFirstScrollableRow();
            }

            if (hScroll)
            {
                height += SystemInformation.VerticalScrollBarWidth;

                grid.HScrollBehavior = GridScrollbarMode.Enabled | GridScrollbarMode.AutoScroll;
                grid.HScroll = true;
            }
            else
            {
                grid.HScrollBehavior = GridScrollbarMode.Disabled;
                grid.HScroll = false;
                grid.LeftColIndex = grid.GetFirstScrollableCol();
            }
        }

        private void Table_CurrentRecordContextChange(object sender, CurrentRecordContextChangeEventArgs e)
        {
            this.dropDownTableControl.InvalidateRange(table.GetCurrentRecordRangeInfo());
            switch (e.Action)
            {
                case CurrentRecordAction.EndEditCalled:
                    this.Grid.CurrentCell.Lock();
                    break;
                case CurrentRecordAction.EndEditComplete:
                    AutoSizeHeightOfGrid();
                    if (!(e.Record is AddNewRecord))
                    {
                        this.Grid.Refresh();  // repaint all foreign key values in case existing record was changed.
                    }

                    this.Grid.CurrentCell.Unlock();
                    break;
            }
        }

        private void dropDownTableControl_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            GridTableCellStyleInfo style = (GridTableCellStyleInfo)e.Style;
            if (style.TableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AlternateRecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AddNewRecordFieldCell)
            {
                Record r = Record.GetParentRecord(style.TableCellIdentity.DisplayElement);
                if (r.IsCurrent)
                {
                    GridControlBase grid = (GridControlBase)sender;
                    if (!grid.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex))
                    {
                        e.Style.BackColor = SystemColors.Highlight;
                        e.Style.TextColor = SystemColors.HighlightText;
                    }
                }
            }
        }

        private void listControlPartGrid_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            e.Style.ShowButtons = GridShowButtons.Hide;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Record r = table.CurrentRecord;
            if (r != null && fd != null)
            {
                if (!r.EndEdit(out r))
                {
                    return;
                }
            }

            CurrentCell.CloseDropDown(PopupCloseType.Canceled);
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            Record r = table.CurrentRecord;
            if (r != null && fd != null)
            {
                bool b = (r is AddNewRecord) ? r.EndEdit(out r) : r.EndEdit();
                if (b)
                {
                    object value = GetPrimaryKeyValue(r);
                    ApplyPrimaryKeyValue(value);
                    TextBox.SelectAll();
                    CurrentCell.ConfirmChanges();
                }
            }
        }
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridTableDropDownListControlPart : GridDropDownGridListControlPart
    {
        /// <summary>
        /// Create Child Grid
        /// </summary>
        /// <returns>returns GridListControlChild</returns>
        /// <internalonly/>
        protected override GridListControlChild CreateGridChild()
        {
            return new GridTableListControlChild(this);
        }
    }

#endif
    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridTableListControlModel : GridListControlModel, ITableProvider
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTableListControlModel()
            : base()
        {
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns the grouping table</returns>
        /// <internalonly/>
        public Table GetTable()
        {
            GridGroupTypedListRecordsCollection gls = ListControl.DataSource as GridGroupTypedListRecordsCollection;
            if (gls != null && gls.TableDescriptor != null)
            {
                return gls.Table;
            }

            return null;
        }
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridTableListControlChild : GridListControlChild, ITableProvider
    {
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridTableListControlChild(GridListControl listControl)
            : base(listControl, new GridTableListControlModel())
        {
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns the Grouping table</returns>
        /// <internalonly/>
        public Table GetTable()
        {
            return ((GridTableListControlModel)Model).GetTable();
        }
    }
}
