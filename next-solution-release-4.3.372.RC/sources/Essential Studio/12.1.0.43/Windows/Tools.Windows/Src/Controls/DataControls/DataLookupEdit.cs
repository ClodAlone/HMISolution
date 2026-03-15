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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// CurrencyEdit class encapsulates a CurrencyTextBox control and
    /// adds the ability to drop down a popup calculator
    /// </summary>
    [
    ToolboxItem(false),
    Designer("Syncfusion.Windows.Forms.Tools.Design.ButtonEditDesigner", typeof(System.ComponentModel.Design.IDesigner)),
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public class DataLookupEdit : ButtonEdit, IDataListViewOwner
    {
        /// <summary>
        /// The calculator button.
        /// </summary>
        private ButtonEditChildButton btnDown;

        /// <summary>
        /// The popup control container for the calculator.
        /// </summary>
        private SizablePopupControlContainer dropDownContainer;

        /// <summary>
        /// The Calculator Control.
        /// </summary>
        private DataListView dropDownList;

        /// <summary>
        /// The collection of Columns specifying the attributes of the columns of
        /// the List displayed with the matches.
        /// </summary>
        private AutoCompleteDataColumnInfoCollection columnInfoList;

        /// <summary>
        /// Event raised before the calculator popup is displayed.
        /// </summary>
        public event CancelEventHandler BeforeDataListPopupDisplay;

        private bool isInitialized;

        #region IDataViewListOwner

        /// <summary>
        /// This method implements IDataViewListOwner.GetImageColumnIndex.
        /// </summary>
        /// <returns>The index of the column in the data source that provides the
        /// index of images in the assigned image list.</returns>
        public int GetImageColumnIndex()
        {
            return -1;
        }

        /// <summary>
        /// This method implements IDataViewListOwner.GetColumnWidth.
        /// </summary>
        /// <param name="columnIndex">The index of the column for which to 
        /// return the width.
        /// </param>
        /// <returns>The width of the column.</returns>
        public int GetColumnWidth(int columnIndex)
        {
            return 100;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the DataLookupEdit class.
        /// </summary>
        public DataLookupEdit()
        {
            InitializeComponent();
            this.imageListValue = null;
            this.btnDown = new ButtonEditChildButton();
            this.isInitialized = false;
            this.SuspendLayout();
            this.Buttons.Add(btnDown);
            this.btnDown.ButtonAlign = ButtonAlignment.Right;
            this.btnDown.ButtonType = ButtonTypes.Down;
            this.btnDown.Click += new EventHandler(this.HandleDownBtnClick);

            // Create the drop down list and the container
            this.dropDownList = new DataListView();
            this.dropDownContainer = new SizablePopupControlContainer(this.DropDownList);

            this.ResumeLayout();
        }

        /// <summary>
        /// Gets or sets the columns that will be displayed in the popup control when the AutoCompleteMode is
        /// set to AutoSuggest. The Columns property is a collection of AutoCompleteDataColumnInfo objects
        /// that specify the attributes of a column.
        /// </summary>
        /// <seealso cref="AutoCompleteDataColumnInfoCollection"/>
        /// <seealso cref="AutoCompleteDataColumnInfo"/>
        [
        Category("AutoComplete"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        MergableProperty(false),
        Localizable(true),
        Description(@"The collection of Columns for the drop down list when in AutoSuggest
		Mode.")
        ]
        public AutoCompleteDataColumnInfoCollection Columns
        {
            get
            {
                return this.columnInfoList;
            }

            set
            {
                this.columnInfoList = value;
            }
        }

        /// <summary>
        /// Get the  AutoComplete DataColumnInfoCollection
        /// </summary>
        /// <returns>Returns AutoComplete DataColumnInfoCollection</returns>
        public AutoCompleteDataColumnInfoCollection GetColumns()
        {
            return this.Columns;
        }

        /// <summary>
        /// Gets or sets the drop down list displaying the matches.
        /// </summary>
        [Browsable(false),
        Category("AutoComplete"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description(@"The drop down list that is displayed with the matching items
		when the mode is AutoSuggest.")]
        public DataListView DropDownList
        {
            get
            {
                return this.dropDownList;
            }

            set
            {
                this.dropDownList = value;
            }
        }

        /// <summary>
        /// The image list that will be used by the AutoComplete object.
        /// </summary>
        private ImageList imageListValue;

        /// <summary>
        /// Initialize the dropdown
        /// </summary>
        /// <returns>Returns true if the the dropdown is initialized.</returns>
        protected virtual bool InitializeDropDown()
        {
            try
            {
                if (this.isInitialized == false)
                {
                    if (this.Columns.Count == 0)
                        this.Columns.Add(new AutoCompleteDataColumnInfo(string.Empty, 150, true));

                    if (this.dropDownList == null)
                        this.dropDownList = new DataListView();

                    if (this.dropDownContainer == null)
                        this.dropDownContainer = new SizablePopupControlContainer(this.DropDownList);

                    this.WireEvents();
                    this.isInitialized = true;

                    this.DropDownList.ListOwner = this;

                    // this.DropDownList.InitializeListView(this.columnInfoList);
                    this.dropDownContainer.InitializeContainer();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Wire events.
        /// </summary>
        protected virtual void WireEvents()
        {
            this.dropDownContainer.CloseUp += new PopupClosedEventHandler(this.HandlePopupClosed);
            this.dropDownList.SelectedIndexChanged += new System.EventHandler(this.HandleListSelectionChanged);
            this.dropDownList.DoubleClick += new System.EventHandler(this.HandleListDoubleClick);
        }

        /// <summary>
        /// Gets or sets the ImageList that will specify the images that will be used
        /// by the popup control when in AutoSuggest Mode.
        /// </summary>
        [Browsable(true),
        Category("AutoComplete"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description(@"The imagelist to be used by the control to display icons
		for the items in the history list")]
        public ImageList ImageList
        {
            get
            {
                return this.imageListValue;
            }

            set
            {
                this.imageListValue = value;
            }
        }

        /// <summary>
        /// Gets or sets DataList view
        /// </summary>
        public DataListView ListView
        {
            get
            {
                return this.dropDownList;
            }

            set
            {
                this.dropDownList = value;
            }
        }

        /// <summary>
        /// Handle ListSelectionChanged
        /// </summary>
        /// <param name="sender"> Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void HandleListSelectionChanged(object sender, System.EventArgs e)
        {
            string selectedValue = String.Empty;
        }

        /// <summary>
        /// Setting the selected value.
        /// </summary>
        /// <param name="selectedValue">Selected Value</param>
        private void SetSelectedValue(string selectedValue)
        {
            this.TextBox.Text = selectedValue;
        }

        /// <summary>
        /// Raises when popup closed.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">PopupClosedEventArgs that contains the event data.</param>
        private void HandlePopupClosed(object sender, PopupClosedEventArgs e)
        {
            if (e.PopupCloseType == PopupCloseType.Done)
            {
                string selectedValue = String.Empty;
                if (this.dropDownList.SelectedItems.Count > 0)
                {
                    selectedValue = (string)this.dropDownList.SelectedItems[0].Text;
                }
                object[] itemArray = new object[1];

                this.SetSelectedValue(selectedValue);
            }
        }

        /// <summary>
        /// Handles the DoubleClick event of the drop down list.
        /// </summary>
        /// <param name="sender">The drop down list.</param>
        /// <param name="e">The EventArgs object with event data.</param>
        private void HandleListDoubleClick(object sender, System.EventArgs e)
        {
            if (this.IsDropDownShowing())
                this.dropDownContainer.HidePopup(PopupCloseType.Done);
        }

        /// <summary>
        /// Checking whether dropdown is showing or not
        /// </summary>
        /// <returns> Returns true if Dropdown is showing.</returns>
        private bool IsDropDownShowing()
        {
            if (this.dropDownContainer != null)
            {
                return this.dropDownContainer.IsShowing();
            }

            return false;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
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

        }
        #endregion

        /// <summary>
        /// Handles the calculator button's click event.
        /// </summary>
        /// <param name="sender">The calculator event.</param>
        /// <param name="e">The event data.</param>
        private void HandleDownBtnClick(object sender, System.EventArgs e)
        {
            if (this.IsDropDownShowing() == false)
                this.DisplayDataListView();
        }

        /// <summary>
        /// Gets or sets the datasource for the DataButtonEdit.
        /// </summary>
        [
        Description(@"Indicates the source of data for the DataButtonEdit"),
        DefaultValue(null),
        Category(@"Data"),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter(@"System.Windows.Forms.Design.DataSourceConverter, System.Design")
        ]
        public object DataSource
        {
            get
            {
                if (this.DataListView != null)
                    return this.DataListView.DataSource;
                else
                    return null;
            }

            set
            {
                this.DataListView.DataSource = value;
            }
        }

        /// <summary>
        /// Gets or sets DataListview
        /// </summary>
        [Browsable(false)]
        public DataListView DataListView
        {
            get
            {
                return this.dropDownList;
            }

            set
            {
                this.dropDownList = value;
            }
        }

        /// <summary>
        /// Displays the calculator.
        /// </summary>
        public void DisplayDataListView()
        {
            ShowDataListView(Point.Empty);
        }

        /// <summary>
        /// Shows the calculator at the specified location.
        /// </summary>
        /// <param name="location">Location of the Popup. </param>
        public void ShowDataListView(Point location)
        {
            this.InitializeDropDown();
            this.dropDownContainer.ParentControl = this;
            if (this.DataListView.DataSource != null)
                this.DataListView.PopulateList();
            this.dropDownContainer.ShowPopup(location);
        }

        /// <summary>
        /// Raise Before DataListPopupDisplay Event
        /// </summary>
        /// <returns>Returns true if Datalistpopup display</returns>
        protected bool RaiseBeforeDataListPopupDisplayEvent()
        {
            bool bCancel = false;
            CancelEventArgs arg = new CancelEventArgs(bCancel);
            return this.OnBeforeDataListPopupDisplay(arg);
        }

        /// <summary>
        /// Invokes the BeforeCalculatorPopupDisplay event.
        /// </summary>
        /// <param name="args">A BeforeCalculatorPopupDisplayEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnBeforeCalculatorPopupDisplay method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnBeforeCalculatorPopupDisplay in a derived
        /// class, be sure to call the base class's OnBeforeCalculatorPopupDisplay method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        /// <returns>Returns true if evet is cancelled.</returns>
        protected virtual bool OnBeforeDataListPopupDisplay(CancelEventArgs args)
        {
            if (this.BeforeDataListPopupDisplay != null)
                this.BeforeDataListPopupDisplay(this, args);

            return args.Cancel;
        }

        /// <summary>
        /// Handles the CalculatorControl's PopupClosed event.
        /// </summary>
        /// <param name="sender">The popup control container.</param>
        /// <param name="e">The event data.</param>
        private void HandleDataListPopupClosedEvent(object sender, EventArgs e)
        {
            this.CloseDataList();
        }

        /// <summary>
        /// Closes the popup calculator if it is displayed.
        /// </summary>
        public void CloseDataList()
        {
            if (this.dropDownContainer.IsShowing())
                this.dropDownContainer.HidePopup(PopupCloseType.Done);
        }

        /// <summary>
        /// Handles the popup control container's BeforePopup event.
        /// </summary>
        /// <param name="sender">The popup control container.</param>
        /// <param name="e">The event data.</param>
        private void HandleDataListPopupContainerBeforePopup(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
