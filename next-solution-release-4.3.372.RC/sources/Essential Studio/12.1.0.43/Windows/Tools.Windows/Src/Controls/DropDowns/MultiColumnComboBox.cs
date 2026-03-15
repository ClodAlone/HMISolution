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
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents a combo box that can show multiple columns in the dropdown.
    /// </summary>
    /// <remarks>
    /// <p>This combo box is meant to be used in data bound mode where it will
    /// show all the records and the different fields in the data source in a 
    /// multi column grid, in the dropdown.</p>
    /// <p>Data binding is done as usual through the <see cref="DataSource"/>,
    /// <see cref="DisplayMember"/> and <see cref="ValueMember"/> properties.
    /// The <b>DisplayMember</b> is used to determine which field will be shown 
    /// in the combo.</p>
    /// <p>If you bind an array of objects of custom type then the public properties
    ///  in that type will correspond to each column in the dropdown multi-column grid.</p>
    /// <p>Note that in this version you cannot populate the <see cref="Items"/> of this combo manually.</p>
    /// </remarks>
    [Designer(typeof(Syncfusion.Windows.Forms.Tools.MultiColumnComboBoxDesigner),
        typeof(System.ComponentModel.Design.IDesigner))]
    [Description("Represents a combo box that can show multiple columns in the dropdown.")]
    public class MultiColumnComboBox : ComboBoxBaseDataBound
    {
        #region INIT
        /// <summary>Metrocolor</summary>
        private Color m_metroColor = ColorTranslator.FromHtml("#16A5DC");
        ///<summary></summary>
        private MetroColorTable metroColorTbl = new MetroColorTable();
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiColumnComboBox"/> class.
        /// </summary>       
        public MultiColumnComboBox()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MultiColumnComboBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.MouseWheel += new MouseEventHandler(MultiColumnComboBox_MouseWheel);
        }

        /// <summary>
        /// Creates the ListControl.
        /// </summary>
        /// <returns>Returns List control</returns>
        protected override ListControl CreateListControl()
        {
            GridListBox lb = new GridListBox();
            return lb;
        }

        protected override void InitListControl(ListControl listControl)
        {
            this.ListBox.TabStop = false;
            this.ListBox.MultiColumn = true;
            base.InitListControl(listControl);
        }

        /// <summary>
        /// Updates the list box border style.
        /// </summary>
        protected override void UpdateListBoxBorderStyle()
        {
            if (this.ListBox == null)
                return;

            if (this.DropDownStyle == ComboBoxStyle.Simple)
            {
                this.ListBox.ThemesEnabled = false;
                this.ListBox.BorderStyle = BorderStyle.None;
            }
            else
            {
                if (this.FlatStyle == ComboFlatStyle.System)
                {
                    this.ListBox.ThemesEnabled = true;
                    this.ListBox.BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    this.ListBox.ThemesEnabled = false;
                    this.ListBox.BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        /// <summary>
        /// Gets the height of the list box border depending on Border Styles.
        /// </summary>
        /// <returns>Returns Listbox border height</returns>
        private int GetListBoxBorderHeight()
        {
            if (this.ListBox.BorderStyle == BorderStyle.FixedSingle)
                return 2;
            else if (this.ListBox.BorderStyle == BorderStyle.None)
                return 0;
            else if (this.ListBox.ThemesEnabled)
                return 2;
            else
                return 4;
        }
      
        protected override void OnDataManagerPositionChanged()
        {
            if (this.DataManager != null)
            {
                if (this.Items.Count > this.DataManager.Position
                    && this.ListBox.Items != null
                    && this.ListBox.Items.Count > this.DataManager.Position)
                    this.SelectedIndex = this.DataManager.Position;
            }
        }
        #endregion INIT

        #region PROPERTIES
        /// <summary>
        /// Gets or sets the theme color of the MultiColumnComboBoxAdv
        /// </summary>       
        [
        Browsable(true), 
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the pressed background color of the control.")
        ]
        new public Color MetroColor
        {
            get { return m_metroColor; }
            set
            {
                m_metroColor = value;
                OnStyleChanged();
            }
        }
        /// <summary>
        /// Gets or sets the index of the currently selected item.
        /// </summary>
        /// <remarks >Overriden.</remarks>
        public override int SelectedIndex
        {
            get
            {
                // The ListBox's SelectedIndex seems to return 0 even if the items count is 0.
                // Hence the override.
                if (this.ListBox == null ||
                    (this.ListBox.DataSource == null &&
                        (this.ListBox.Items == null || this.ListBox.Items.Count == 0)))
                    return -1;
                return base.SelectedIndex;
            }
            set 
            { 
                base.SelectedIndex = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple columns or a single column should be shown in the dropdown.
        /// </summary>
        [DefaultValue(true), Category("Appearance"),
        Description("Specifies whether multiple columns or a single column should be shown in the dropdown.")]
        public virtual bool MultiColumn
        {
            get
            {
                if (this.ListBox != null)
                    return this.ListBox.MultiColumn;
                else
                    return true;
            }
            set
            {
                if (this.ListBox != null)
                    this.ListBox.MultiColumn = value;
            }
        }

        /// <summary>
        /// Gets ComboBoxBaseDataBound items collection.
        /// </summary>
        /// <remarks>Do not add/remove items through this property.</remarks>
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
             "System.Drawing.Design.UITypeEditor, System.Drawing"),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ComboBoxBaseDataBound.ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        /// <summary>
        /// Gets the dropdown list box, which is an instance of the <see cref="GridListBox"/>
        /// associated with this combo.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public virtual GridListBox ListBox
        {
            get
            {
                if (this.ListControl == null)
                {
                    this.ListControl = this.CreateListControl();
                    this.InitListControl(this.ListControl);
                }

                return this.ListControl as GridListBox;
            }
        }

        /// <summary>
        /// Indicates whether the control should resize to avoid
        /// showing partial items.
        /// </summary>     
        [Browsable(false)]
        public override bool IntegralHeight
        {
            get
            {
                return base.IntegralHeight;
            }
            set
            {
                if (this.IntegralHeight != value)
                {
                    base.IntegralHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets an advanced appearance for this control.
        /// </summary>
        public new VisualStyle Style
        {
            get
            {
                return base.Style;
            }
            set
            {
                if (base.Style != value)
                {
                    base.Style = value;
                    if (value != VisualStyle.Default)
                        this.ListBox.ThemesEnabled = true;
                    this.OnStyleChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets Office 2007 color scheme.
        /// </summary>
        public new Office2007Theme Office2007ColorTheme
        {
            get
            {
                return base.Office2007ColorTheme;
            }
            set
            {
                if (base.Office2007ColorTheme != value)
                {
                    base.Office2007ColorTheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets Office 2010 color scheme.
        /// </summary>
        public new Office2010Theme Office2010ColorTheme
        {
            get
            {
                return base.Office2010ColorTheme;
            }
            set
            {
                if (base.Office2010ColorTheme != value)
                {
                    base.Office2010ColorTheme = value;
                    this.OnStyleChanged();
                }
            }
        }

        #endregion PROPERTIES

        #region LISTBOX_PROPERTIES
        /// <summary>
        /// Gets or sets a value indicating whether column headers should be displayed in the dropdown.
        /// </summary>
        [DefaultValue(true) /*This will work as long as the GridListControl's Default is also true.*/]
        [Description("Indicates if column headers should be displayed in the dropdown."),
        Category("Appearance")]
        public bool ShowColumnHeader
        {
            get
            {
                if (this.ListBox == null)
                    return true;

                return this.ListBox.ShowColumnHeader;
            }
            set
            {
                if (this.ListBox != null)
                    this.ListBox.ShowColumnHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Drawing.Color"/> for alpha blended row selections in the dropdown.
        /// </summary>
        /// <value>
        /// A <see cref="System.Drawing.Color"/> for alpha blended row selections. It is important to set the alpha value to be less
        /// than 255 when calling <see cref="System.Drawing.Color.FromArgb"/>.
        /// </value>
        [Description("Specifies the color for alpha blended selections in the dropdown.")]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                if (this.ListBox == null)
                    return Color.Empty;

                return this.ListBox.AlphaBlendSelectionColor;
            }
            set
            {
                if (this.ListBox != null)
                    this.ListBox.AlphaBlendSelectionColor = value;
            }
        }
        private void ResetAlphaBlendSelectionColor()
        {
            if (this.ListBox != null)
                this.ListBox.ResetAlphaBlendSelectionColor();
        }
        private bool ShouldSerializeAlphaBlendSelectionColor()
        {
            if (this.ListBox == null)
                return false;
            else
                return this.ListBox.ShouldSerializeAlphaBlendSelectionColor();
        }
        #endregion LISTBOX_PROPERTIES

        #region OVERRIDES

        /// <summary>
        /// Raises the DisplayMemberChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <override/>
        protected override void OnDisplayMemberChanged(EventArgs e)
        {
            base.OnDisplayMemberChanged(e);

            if (this.ListBox == null)
            {
                this.ListControl = this.CreateListControl();
                this.InitListControl(this.ListControl);
            }

            this.ListBox.DisplayMember = this.DisplayMember;

            this.UpdateText(false);
        }

        protected override void NativeAdd(object item)
        {
            // Don't do anything as we are data-binding the GridListBox.
        }

        protected override void NativeClear()
        {
            // Don't do anything as we are data-binding the GridListBox.

            // In a ComboBox or ComboBoxAdv, the ListBox's SelectedIndex will be
            // reset to -1, but that's not the case with the GridListBox, so we are
            // resetting it manually.
            if (this.ListBox != null)
                this.ListBox.SelectedIndex = -1;
        }

        protected override void NativeInsert(int index, object item)
        {
            // Don't do anything as we are data-binding the GridListBox.
        }

        protected override void NativeRemoveAt(int index)
        {
            // Don't do anything as we are data-binding the GridListBox.
        }

        protected override void SetItemsCore(IList items)
        {
            base.SetItemsCore(items);

            if (this.ListBox != null)
                this.ListBox.DataSource = this.DataSource;

            // Workaround:
            // The GridListControl seems to set the SelectedIndex as soon as it's
            // data binding is set. If that happens then the combo's text will
            // never get updated. So, resetting it to -1 and the next line will
            // set it back to the right position and also update the combo text.
            // this.ListBox.SelectedIndex = -1;

            // this.SelectedIndex = this.DataManager.Position;
        }

        /// <summary>
        /// Raises the DataSourceChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks >If Control's DataSource is null,then the ListBox DataSource resets to null</remarks>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            if (this.DataSource == null)
                this.ListBox.DataSource = null;
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">String for searching</param>
        /// <returns>Returns the Found index</returns>
        public override int FindString(string s)
        {
            return this.FindString(s, -1);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">string for searching</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Returns the Found index</returns>
        public override int FindString(string s, int startIndex)
        {
            if (s == null)
                return -1;

            return this.ListBox.FindString(s, startIndex);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">String for searching</param>
        /// <returns>Returns the Found index</returns>
        public override int FindStringExact(string s)
        {
            return this.FindStringExact(s, -1);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">string for searching</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Returns the Found index </returns>
        public override int FindStringExact(string s, int startIndex)
        {
            if (s == null || this.ListBox.DataSource == null)
                return -1;

            if (this.Items.Count == 0)
                return -1;

            if (startIndex < 0)
                startIndex = 0;

            if (startIndex > this.Items.Count)
                return -1;

            return this.ListBox.FindStringExact(s, startIndex);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="text">String text</param>
        /// <param name="ignoreCase">Bool value for ignore case</param>
        /// <returns>Returns the found index</returns>
        public override int FindStringExact(string text, bool ignoreCase)
        {
            if (text == null || this.ListBox.DataSource == null) return -1;

            int foundIndex = this.ListBox.FindItem(text, false, -1, ignoreCase);
            return foundIndex;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)                                         
        {
            if (this.DropDownStyle == ComboBoxStyle.Simple && this.IntegralHeight
                && height > this.EditPortionHeight)
            {
                // Restrict the height to be a multiple of the ItemHeight.
                int specifiedListHeight = this.GetEmbeddedChildBounds(height).Height;
                int listNCHeight = this.GetListNCHeight();

                int n = (specifiedListHeight - listNCHeight) / this.ListBox.ItemHeight;

                int rightListHeight = n * this.ListBox.ItemHeight + listNCHeight;

                int delta = rightListHeight - specifiedListHeight;

                height += delta;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Called before the popUp is displayed
        /// </summary>
        /// <remarks >Overridden.Depending on the number of items set as MaxDropDownItems,ListBox adjusts its height.</remarks>
        protected override void OnBeforePopup()
        {
            int itemsToShow = 0;
            if (this.ListBox.Items != null)
                itemsToShow = this.ListBox.Items.Count > this.MaxDropDownItems ? this.MaxDropDownItems : this.ListBox.Items.Count;

            if (itemsToShow == 0)
                return;

            IList items = this.ListBox.Items;

            bool needVScroll = null != items ? (itemsToShow < this.ListBox.Items.Count) : false;

            int headerHeight = 0;
            if (this.ListBox.ShowColumnHeader)
                headerHeight = this.ListBox.Grid.RowHeights[0];

            // Calculate the preferred width:
            int listWidth = this.DropDownWidth;
            if (this.ShouldSerializeDropDownWidth() == false)
            {
                // Replace the default setting with a longer one if necessary.
                // TODO: Get only the "preferred widths" not the latest widths.
                // GridControl grid = this.ListBox.Grid;
                // grid.AllowResizeToFit = false;//turn off grid default sizing
                // grid.Model.ColWidths.ResizeToFit(GridRangeInfo.Table(), GridResizeToFitOptions.IncludeHeaders);
                int prefWidth = this.ListBox.Grid.ColWidths.GetTotal(0, this.ListBox.Grid.Model.ColCount);
                if (needVScroll)
                    prefWidth += SystemInformation.VerticalScrollBarWidth + 2;

                if (prefWidth > listWidth)
                    listWidth = prefWidth;
            }

            // Make sure the handle is created so that when ItemHeight is called, it will be the right value.
            IntPtr handle = this.ListBox.Handle;
            bool bHscrollBarVisible = this.ListBox.Grid.HScrollBar != null && this.ListBox.Grid.HScrollBar.InnerScrollBar != null && this.ListBox.Grid.HScrollBar.InnerScrollBar.Visible;     
            itemsToShow = bHscrollBarVisible ? itemsToShow + 1 : itemsToShow;
            int listHeight = itemsToShow * this.ListBox.ItemHeight + headerHeight + this.GetListBoxBorderHeight();
            listWidth += this.GetListBoxBorderHeight();

            this.ListBox.ClientSize = new Size(listWidth, listHeight);

            // Check if ScrollBar is disposed
            if (bHscrollBarVisible && this.ListBox.Grid.HScrollBar.InnerScrollBar == null)
            {
                itemsToShow = itemsToShow - 1;

                listHeight = itemsToShow * this.ListBox.ItemHeight + headerHeight + this.GetListBoxBorderHeight();

                this.ListBox.ClientSize = new Size(listWidth, listHeight);
            }

            if (this.PopupContainer.Width != listWidth)
                this.PopupContainer.Width = listWidth;

            base.OnBeforePopup();
        }

        protected override void OnPopupClosed(PopupClosedEventArgs e)
        {
            if ((e.PopupCloseType == PopupCloseType.Canceled || e.PopupCloseType == PopupCloseType.Deactivated) && this.Text != this.ListControl.Text)
                this.ListControl.SelectedIndex = this.FindStringExact(this.Text, -1);
            base.OnPopupClosed(e);
        }

        /// <summary>
        /// Gets / sets text.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (AllowNewText)
                {
                    if (NumberOnly)
                    {
                        base.Text = GetNumbersOnlyText(value);
                    }
                    else
                    {
                        base.Text = value;
                    }
                }
                if (String.IsNullOrEmpty(value) || value.Trim().Length == 0)
                {
                    isComboTextSet = true;
                }
            }
        }
        [Browsable(false)]
        public MetroColorTable ScrollMetroColorTable
        {
            get { return metroColorTbl; }
            set
            {
                metroColorTbl = value;
                this.ListBox.Grid.MetroColorTable = value;
                this.ListBox.Invalidate();
            }
        }
        protected override void OnReadOnlyChanged(EventArgs e)
        {
            base.OnReadOnlyChanged(e);
            this.DropDownButton.Enabled = this.Enabled;
            if (this.PopupControl != null)
                this.PopupControl.Enabled = this.Enabled;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.DropDownButton.Enabled = this.Enabled;
            if (this.PopupControl != null && this.DroppedDown)
                this.PopupControl.Enabled = this.Enabled;
        }
        protected override void ProcessDDMouseDown()
        {
            if (!this.ContainsFocus)
            {
                if (this.TextBox.Visible)
                    this.TextBox.Focus();
                else
                    this.Focus();
            }

            if (this.PopupContainer.IsShowing())
                this.HidePopup();
            else if (this.ContainsFocus)
            {
                this.ShowPopup();
            }
        }

        /// <summary>
        /// Updates the <see cref="Text"/> property based on the PopupControl's selected value.
        /// </summary>
        /// <param name="fireEvent">Indicates whether the <see cref="SelectionChangeCommitted"/> event should be fired if the text is changed.</param>
        /// <returns>True if the <see cref="SelectionChangeCommitted"/> event was fired; False otherwise.</returns>
        /// <remarks>You normally do not have to call this method. However when you 
        /// programmatically update the SelectedValue of a plug in the list control, 
        /// you might have to call this method to update the combo's text based on that new value.</remarks>
        public override bool UpdateText(bool fireEvent)
        {
            bool retValue = false;
            retValue = base.UpdateText(fireEvent);
            return retValue;
        }

        protected override void InitPopupContainer()
        {
            base.InitPopupContainer();
            this.PopupContainer.HandleCreated += new EventHandler(PopupContainer_HandleCreated);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.PopupContainer != null)
                {
                    this.PopupContainer.HandleCreated -= new EventHandler(PopupContainer_HandleCreated);
                }

                this.MouseWheel -= new MouseEventHandler(MultiColumnComboBox_MouseWheel);
            }
            base.Dispose(disposing);
        }
        #endregion OVERRIDES

        #region HELPFUL_METHODS
  
        private void PopupContainer_HandleCreated(object sender, EventArgs e)
        {
            if (this.Style == VisualStyle.Office2007)
            {
                this.AttachScrollersFrame(this.ListBox.Grid);
                this.InitScrollersFrame();
            }
        }

        /// <summary>
        /// Returns string with numbers only.
        /// </summary>
        /// <param name="text">Text string</param>
        /// <returns>Returns string with numbers only</returns>
        private string GetNumbersOnlyText(string text)
        {
            string retText = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= '0' && text[i] <= '9')
                {
                    retText += text[i];
                }
            }

            return retText;
        }

        private void OnStyleChanged()
        {
            if (base.Style == VisualStyle.Office2007)
            {
                if (this.ListBox.Grid.IsHandleCreated && this.PopupContainer.IsHandleCreated)
                {
                    this.AttachScrollersFrame(this.ListBox.Grid);
                    this.InitScrollersFrame();
                }

                switch (this.Office2007ColorTheme)
                {
                    case Office2007Theme.Blue:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2007Blue;
                        break;

                    case Office2007Theme.Silver:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2007Silver;
                        break;

                    case Office2007Theme.Black:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2007Black;
                        break;
                }
            }
            else if (base.Style == VisualStyle.Office2010)
            {
                if (this.ListBox.Grid.IsHandleCreated && this.PopupContainer.IsHandleCreated)
                {
                    this.AttachScrollersFrame(this.ListBox.Grid);
                    this.InitScrollersFrame();
                }

                switch (this.Office2010ColorTheme)
                {
                    case Office2010Theme.Blue:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2010Blue;
                        break;

                    case Office2010Theme.Silver:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2010Silver;
                        break;

                    case Office2010Theme.Black:
                        this.ListBox.GridVisualStyles = GridVisualStyles.Office2010Black;
                        break;
                }
            }
            else if (base.Style == VisualStyle.Metro)
            {
                this.ListBox.Grid.AlphaBlendSelectionColor = Color.FromArgb(64, 178, 180, 191);
                base.MetroColor = MetroColor;
                this.Style = VisualStyle.Metro;

                if (this.ListBox.Grid.IsHandleCreated && this.PopupContainer.IsHandleCreated)
                {
                    this.AttachScrollersFrame(this.ListBox.Grid);
                    this.InitScrollersFrame();
                }
                this.ListBox.ThemesEnabled = true;    
                                       
                this.ListBox.GridVisualStyles = GridVisualStyles.Metro;
                this.ListBox.AlphaBlendSelectionColor = Color.FromArgb(64, m_metroColor); 
                this.Invalidate();
            }
            else
            {
                this.ListBox.GridVisualStyles = GridVisualStyles.SystemTheme;
            }
        }

        private void MultiColumnComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.ListBox.Grid.IsHandleCreated && this.Style == VisualStyle.Office2007)
            {
                Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(this.ListBox.Grid.Handle, Syncfusion.Runtime.InteropServices.NativeMethods.WM_MOUSEWHEEL, IntPtr.Zero, IntPtr.Zero);               
            }
        }
        #endregion HELPFUL_METHODS
    }

    public class MultiColumnComboBoxDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public MultiColumnComboBoxDesigner()
            : base()
        {
        }
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    actionLists.Add(
                        new MultiColumnComboBoxActionList(this.Component));
                }
                return actionLists;
            }
        }

#endif
    }

    /// <summary>
    /// Specifies the list box used in a <see cref="MultiColumnComboBox"/>.
    /// </summary>
    /// <remarks>The documentation for the base class <b>GridListControl</b>
    /// is available as part of Essential Grid.</remarks>
    [ToolboxItem(false)]
    public class GridListBox : GridListControl
    {
        protected override GridListControlChild CreateGridChild()
        {
            return new MCGridListControlChild(this);
        }
        
        protected override void OnBindingContextChanged(EventArgs e)
        {
            // This gets called every time the popup is shown, which is not good.
            // So, not calling the base class. The list will get initialized when the 
            // DataSource property is set.

            // preventSelectedIndexChange = true;
            // base.OnBindingContextChanged(e);
            // preventSelectedIndexChange = false;
        }
        protected override void WndProc(ref Message m)
        {
            // Prevent the list box from processing the LeftMouse Down message, 
            // since it activates it's parent Form in it's Default Wnd Proc. 
            if (m.Msg == 0x201/*WM_LBUTTONDOWN*/)
            {
                // This is true only when in dropdown.
                if (this.FindForm() is PopupHost)
                    return;
                if (!this.Parent.ContainsFocus)
                {
                    this.Parent.Focus();
                }
            }

            base.WndProc(ref m);
        }
    }
    [ToolboxItem(false), Documentation.DocumentationExclude()]
    public class MCGridListControlChild : GridListControlChild
    {
        public MCGridListControlChild(GridListControl listControl)
            : base(listControl)
        {
        }
        protected override void WndProc(ref Message m)
        {
            // Prevent the list box from processing the LeftMouse Down message, 
            // since it activates it's parent Form in it's Default Wnd Proc. 
            if (m.Msg == 0x201/*WM_LBUTTONDOWN*/)
            {
                // This is true only when in dropdown.
                if (this.FindForm() is PopupHost)
                    return;
                if (this.ParentCombo != null && !this.ParentCombo.ContainsFocus)
                {
                    this.ParentCombo.Focus();
                }
            }

            base.WndProc(ref m);
        }
        private MultiColumnComboBox ParentCombo
        {
            get
            {
                Control parent = this.Parent;
                while (parent != null && !(parent is MultiColumnComboBox))
                {
                    parent = parent.Parent;
                }
                return parent as MultiColumnComboBox;
            }
        }
    }
}
