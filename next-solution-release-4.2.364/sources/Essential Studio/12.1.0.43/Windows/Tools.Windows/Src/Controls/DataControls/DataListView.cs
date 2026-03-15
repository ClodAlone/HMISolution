#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// This interface should be implemented by classes that act 
    /// as the owner for the <see cref="DataListView"/> class. 
    /// </summary>
    /// <remarks>
    /// The DataListView class will query its owner for special formatting
    /// conditions such as column width and any images that need to be 
    /// displayed. The need for this class is to abstract the formatting
    /// information from the DataListView class. The DataListView class can
    /// display a DataSource even if the <see cref="DataListView.ListOwner"/>
    /// property is not set using default formatting values.
    /// </remarks>
    [
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public interface IDataListViewOwner
    {
        /// <summary>
        /// Returns the index of the column that has information about the
        /// images (if any).
        /// </summary>
        /// <returns>The index of the Image column.</returns>
        int GetImageColumnIndex();

        /// <summary>
        /// Returns the width of the specified column.
        /// </summary>
        /// <param name="columnIndex">The index of the column for which the 
        /// minimum width is required.</param>
        /// <returns>The width of the column.</returns>
        int GetColumnWidth(int columnIndex);
    }

    /// <summary>
    /// DataListView extends the ListView class to provide DataBinding 
    /// support.
    /// </summary>
    /// <remarks>
    /// The <see cref="DataListView"/> creates appropriate columns 
    /// to reflect the columns in the DataSource and populates them.
    /// </remarks>
    [
    ToolboxItem(false),
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public class DataListView : System.Windows.Forms.ListView
    {
        private const int WS_EX_LAYOUTRTL = 0x400000;
        private const int WS_EX_NOINHERITLAYOUT = 0x100000;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// The data source for the DataListView.
        /// </summary>
        private object dataSource = new object();

        /// <summary>
        /// Indicates whether the image information will be provided by the owner.
        /// </summary>
        private bool imageFromOwner = false;

        /// <summary>
        /// The owner.
        /// </summary>
        private IDataListViewOwner listOwner;

        /// <summary>
        /// The index of the image column.
        /// </summary>
        private int imageColumnIndex = -1;

        /// <summary>
        /// Initializes a new instance of the DataListView class.
        /// </summary>
        public DataListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            base.SetStyle(ControlStyles.Selectable, false);

            InitializeComponent();
            base.View = View.Details;
            base.FullRowSelect = true;
        }

        /// <summary>
        /// Overrides Control.OnResize.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeListColumns(this.Width);
        }

        /// <summary>
        /// Populates the ListView based on the columns in the 
        /// DataSource.
        /// </summary>
        public virtual void PopulateList()
        {
            if (this.DataSource != null)
            {
                this.SetColumns();
                this.SetData();
            }
        }

        /// <summary>
        /// Sets the columns based on the DataSource.
        /// </summary>
        public void SetColumns()
        {
            try
            {
                this.Clear();

                if (this.DataSource != null && this.DataSource is DataTable)
                {
                    DataTable table = (DataTable)dataSource;
                    int columnIndex = 0;
                    foreach (DataColumn column in table.Columns)
                    {
                        if (columnIndex != this.imageColumnIndex && column.ColumnMapping != MappingType.Hidden)
                            this.AddColumn(column, columnIndex);
                        columnIndex++;
                    }
                }
                else if (this.DataSource != null && this.DataSource is DataView)
                {
                    DataView tableView = (DataView)dataSource;
                    int columnIndex = 0;
                    foreach (DataColumn column in tableView.Table.Columns)
                    {
                        if (columnIndex != this.imageColumnIndex && column.ColumnMapping != MappingType.Hidden)
                            this.AddColumn(column, columnIndex);

                        columnIndex++;
                    }
                }
                else
                {
                    this.AddColumn(String.Empty, 0);
                }
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem setting the columns in the DataListView.", e);
            }
        }

        private bool _mirrored = false;
        [Description("Change to the right-to-left layout."), DefaultValue(false),
        Localizable(true), Category("Appearance"), Browsable(true)]
        public bool Mirrored
        {
            get
            {
                return _mirrored;
            }
            set
            {
                if (_mirrored != value)
                {
                    _mirrored = value;
                    base.OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp;
                cp = base.CreateParams;
                if (_mirrored)
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
                return cp;
            }
        }

        /// <summary>
        /// Sets the control Height based on the item count subject 
        /// to a maximum height.
        /// </summary>
        /// <param name="maxHeight">The maximum height.</param>
        public virtual void AdjustHeight(int maxHeight)
        {
            if (this.Items.Count > 0)
            {
                int adjustedMaxHeight = GetAdjustedMaxHeight(maxHeight);

                int totalHeight = this.GetItemRect(0).Height * this.Items.Count + this.GetItemRect(0).Height;
                if (totalHeight < adjustedMaxHeight)
                    this.Height = totalHeight;
                else
                    this.Height = adjustedMaxHeight;
            }
        }

        protected virtual int GetAdjustedMaxHeight(int actualMaxHeight)
        {
            int itemsAllowed = actualMaxHeight / this.GetItemRect(0).Height;
            return itemsAllowed * this.GetItemRect(0).Height;
        }

        /// <summary>
        /// Gets or sets the index of the image column.
        /// </summary>
        [
        Description(@"Indicates the index of the image column in the data"),
        DefaultValue(-1)
        ]
        public int ImageColumnIndex
        {
            get
            {
                return this.imageColumnIndex;
            }

            set
            {
                this.imageColumnIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the owner object that will provide formatting information.
        /// </summary>
        [
        Description(@"The owner object that will provide formatting information"),
        DefaultValue(null)
        ]
        public IDataListViewOwner ListOwner
        {
            get
            {
                return this.listOwner;
            }

            set
            {
                this.listOwner = value;
                this.imageColumnIndex = this.listOwner.GetImageColumnIndex();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image field will be provided by the Owner object.
        /// </summary>
        [
        Description(@"TSpecifies if the image field will be provided by the Owner object"),
        DefaultValue(false)
        ]
        public bool ImageFromOwner
        {
            get
            {
                return this.imageFromOwner;
            }

            set
            {
                this.imageFromOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets the data source that the ListView is displaying data for.
        /// </summary>
        [
        Description(@"Indicates the source of data for the DataListView"),
        DefaultValue(null),
        Category(@"Data"),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter(@"System.Windows.Forms.Design.DataSourceConverter, System.Design")
        ]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (value != null && (value is IList || value is IListSource))
                {
                    object oldDataSource = dataSource;

                    dataSource = value;

                   

                    OnDataSourceChanged(oldDataSource, dataSource);
                    this.PopulateList();
                }
                return;
            }
        }

        protected virtual void OnDataSourceChanged(object oldDataSource, object newDataSource)
        {
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
                if (dataSource != null)
                    dataSource = null;

                listOwner = null;
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            if (this.ListOwner != null && this.ListOwner is AutoComplete)
            {
                AutoComplete ac = (AutoComplete)this.ListOwner;
                this.AccessibleDescription = ac.AccessibleDescription;
                this.AccessibleName = ac.AccessibleName;
                this.AccessibleRole = this.AccessibleRole;
            }
        }
        #endregion

        /// <summary>
        /// Adds a column to the ListView and also sets its width
        /// if available.
        /// </summary>
        /// <param name="column">Datacolumn object</param>
        /// <param name="index">Index value</param>
        private void AddColumn(object column, int index)
        {
            ColumnHeader header = new ColumnHeader();
            if (column is DataColumn)
                header.Text = ((DataColumn)column).ColumnName;

            // Set the column width
            if (this.ListOwner != null)
                header.Width = this.ListOwner.GetColumnWidth(index);
            else
                header.Width = AutoComplete.MinColumnWidth;

            this.Columns.Add(header);
        }

        /// <summary>
        /// Sets the data for the ListView from the DataSource.
        /// </summary>
        public void SetData()
        {
            if (this.DataSource is DataTable)
            {
                DataTable table = (DataTable)dataSource;
                for (int n = 0; n < table.Rows.Count; n++)
                    this.AddListRow(table.Rows[n]);
            }
            else if (this.DataSource is DataView)
            {
                DataView tableView = (DataView)dataSource;
                foreach (DataRowView currentRow in tableView)
                    this.AddListRow(currentRow);
            }
            else if (this.DataSource is IList || this.DataSource is IListSource)
            {
                IList list = null;
                if (this.DataSource is IListSource)
                    list = ((IListSource)this.DataSource).GetList();
                else
                    list = (IList)this.DataSource;

                for (int n = 0; n < list.Count; n++)
                    this.AddListRow(list[n]);
            }
        }

        /// <summary>
        /// Adds a row of data to the ListView.
        /// </summary>
        /// <param name="currentRow">The row of data to be added.</param>
        private void AddListRow(object currentRow)
        {
            try
            {
                ListViewItem newItem = new ListViewItem();
                if (currentRow is DataRow)
                {
                    int columnsCount = ((DataRow)currentRow).Table.Columns.Count;
                    object[] dataArray = new object[columnsCount];
                    dataArray = ((DataRow)currentRow).ItemArray;

                    for (int i = 0; i < columnsCount; i++)
                    {
                        if (i != this.ImageColumnIndex)
                        {
                            if (i == 0)
                                newItem.Text = dataArray[i].ToString();
                            else
                                newItem.SubItems.Add(dataArray[i].ToString());
                        }
                        else
                        {
                            string columnIndex = dataArray[i].ToString();
                            newItem.ImageIndex = Convert.ToInt16(columnIndex);
                        }
                    }
                }
                else if (currentRow is DataRowView)
                {
                    int columnsCount = ((DataRowView)currentRow).DataView.Table.Columns.Count;
                    object[] dataArray = new object[columnsCount];
                    dataArray = ((DataRowView)currentRow).Row.ItemArray;

                    for (int i = 0; i < columnsCount; i++)
                    {
                        if (i != this.ImageColumnIndex)
                        {
                            if (i == 0)
                                newItem.Text = dataArray[i].ToString();
                            else
                                newItem.SubItems.Add(dataArray[i].ToString());
                        }
                        else
                        {
                            if (dataArray[i] != null)
                            {
                                string columnIndex = dataArray[i].ToString();
                                if (columnIndex != null && columnIndex != String.Empty)
                                    newItem.ImageIndex = Convert.ToInt16(columnIndex);
                            }
                        }
                    }
                }
                else
                {
                    newItem.Text = currentRow.ToString();
                }
                this.Items.Add(newItem);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Resizes the columns.
        /// </summary>
        /// <param name="totalWidth">The new total width.</param>
        public virtual void ResizeListColumns(int totalWidth)
        {
            try
            {
                int minNeededWidth = 0;
                int averageExcess = 0;
                int imageColumnIndex = this.ImageColumnIndex;
                int columnWidth = 0;

                if (this.Columns.Count > 0)
                {
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        if (this.ListOwner != null)
                        {
                            if (i != imageColumnIndex)
                            {
                                minNeededWidth += this.ListOwner.GetColumnWidth(i);
                            }
                        }
                        else
                        {
                            if (i != imageColumnIndex)
                            {
                                minNeededWidth += AutoComplete.MinColumnWidth;
                            }
                        }
                    }
                }
                if (imageColumnIndex >= 0 && imageColumnIndex < this.Columns.Count)
                {
                    totalWidth -= 20;
                }
                if (minNeededWidth < totalWidth)
                {
                    if (this.Columns.Count > 0)
                        averageExcess = ((totalWidth) - minNeededWidth) / this.Columns.Count;
                    else
                        averageExcess = (totalWidth) - minNeededWidth;
                    for (int j = 0; j < this.Columns.Count; j++)
                    {
                        if (this.ListOwner != null)
                        {
                            if (j != imageColumnIndex)
                            {
                                columnWidth = this.ListOwner.GetColumnWidth(j);
                            }
                        }
                        else
                        {
                            if (j != imageColumnIndex)
                            {
                                columnWidth = AutoComplete.MinColumnWidth;
                            }
                        }
                        this.Columns[j].Width = columnWidth + averageExcess;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
