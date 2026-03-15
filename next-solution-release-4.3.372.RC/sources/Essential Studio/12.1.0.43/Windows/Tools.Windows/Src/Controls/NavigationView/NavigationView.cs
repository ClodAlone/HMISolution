#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
    using Navigation;
    using Navigation.Design;
    using Navigation.Layouting;

    /// <summary>
    /// Breadcrumb navigation view.
    /// </summary>
    [DefaultProperty("Bars")]
    [DefaultEvent("BarSelectionChanged")]
    [Designer(typeof(NavigationViewDesigner))]
    [TypeDescriptionProvider(typeof(NavigationViewTypeDescriptionProvider))]
    [ToolboxBitmap(typeof(NavigationView), "ToolboxIcons.NavigationView.bmp")]
    public partial class NavigationView :
        Control,
        ISupportOffice2007Theme,
        ISuppportHistory,
        ISupportInitialize,
        IMessageFilter,
        IContainBars,
        IVisualStyle 
    {
        #region Constants

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


        /// <summary>
        /// Default height of <see cref="NavigationView"/> control.
        /// </summary>
        public const int DefaultHeight = 22;

        /// <summary>
        /// Default <see cref="Bar"/>'s path separator.
        /// </summary>
        public const string DefaultPathSeparator = @"\";

        #endregion

        #region Fields

        private BarCollection _bars = null;
        private CustomButtonCollection _customButtons = null;
        private VisualStyles _visualStyle = VisualStyles.Office2007;
        private Office2007Theme _office2007Theme = Office2007Theme.Managed;
        private Bar _selectedBar = null;
        private List<Bar> _path = null;
        private Syncfusion.Windows.Forms.HistoryManager _historyMan = null;
        private bool _historyEnabled = false;
        private bool _editingMode = false;
        private ImageList _imageList = null;
        private ImageList _disabledImageList = null;
        private bool _showHistoryButtons = false;
        private Visualizer _visualizer;
        private int _initializing = 0;
        private Size _popupDropDownSize;
        private PopupHost _popupHost;
        private bool _showBorder = false;
        private Color _borderColor = Color.Navy;
        private bool _showRootBarText = false;
        private bool allowEdit = true;

        #endregion

        #region Contruction ans initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationView"/> class.
        /// </summary>
        public NavigationView()
        {
            InitializeComponent();
            SetupComponents();
            SetupControl();

            CTRLSIZE = new System.Drawing.Size(141, 21); 

        }

        private void SetupComponents()
        {
            _visualizer = new Visualizer(this, this.VisualStyle);
            _popupHost = new NavigationView.PopupHost();

            _recentList.Parent = _popupContainer;

            _popupContainer.Parent = this;
            _popupContainer.ParentControl = this;
            _popupContainer.PopupHost = _popupHost;

            AdwiseRecentListPopup();
            RegisterComponents();
        }

        private void RegisterComponents()
        {
            this.components.Add(_popupContainer);
            this.components.Add(_recentList);
            this.components.Add(_popupHost);
            this.components.Add(_visualizer);
        }

        private void SetupControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when <see cref="NavigationView.SelectedBar"/> property is changed.
        /// </summary>
        [Description("Occurs when SelectedBar property is changed.")]
        public event EventHandler<BarSelectionChangedEventArgs> BarSelectionChanged;

        /// <summary>
        /// Occurs when <see cref="NavigationView.SelectedBar"/> property is changing.
        /// </summary>
        [Description("Occurs when SelectedBar property is changing.")]
        public event EventHandler<BarSelectionChangingEventArgs> BarSelectionChanging;

        /// <summary>
        /// Occurs when <see cref="Bar"/> is added to <see cref="NavigationView.Bars"/> collection.
        /// </summary>
        [Description("Occurs when bar is added to NavigationView.Bars collection.")]
        public event CollectionChangeEventHandler BarAdded;

        /// <summary>
        /// Occurs when <see cref="Bar"/> is removed from <see cref="NavigationView.Bars"/> collection.
        /// </summary>
        [Description("Occurs when bar is removed from NavigationView.Bars collection.")]
        public event CollectionChangeEventHandler BarRemoved;

        /// <summary>
        /// Occurs when when the recently visited popup is shown.
        /// </summary>
        [Description("Occurs when when the recently visited popup is shown.")]
        public event EventHandler PopupShown;

        /// <summary>
        /// Occurs when when the recently visited popup is closed.
        /// </summary>
        [Description("Occurs when when the recently visited popup is closed.")]
        public event EventHandler PopupClosed;

        internal event PropertyChangedEventHandler BarPropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="Bar"/>s.
        /// </summary>
        [Description("Gets the collection of Bars.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Editor(typeof(BarCollectionEditor), typeof(UITypeEditor))]
        public BarCollection Bars
        {
            get
            {
                if (_bars == null)
                {
                    _bars = new BarCollection();
                    _bars.CollectionChanged += new CollectionChangeEventHandler(OnBarsCollectionChanged);
                    _bars.ItemPropertyChanged += new PropertyChangedEventHandler(OnBarPropertyChanged);
                }

                return _bars;
            }
        }

        /// <summary>
        /// Gets the collection of visible <see cref="Bar"/>s.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BarCollection VisibleBars
        {
            get
            {
                BarCollection visibleBars = new BarCollection();

                foreach (Bar bar in this.Bars)
                {
                    if (bar.Visible)
                    {
                        visibleBars.Add(bar);
                    }
                }

                return visibleBars;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="NavigationButton"/>s.
        /// </summary>
        [Description("Gets the collection of NavigationButtons.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomButtonCollection CustomButtons
        {
            get
            {
                if (_customButtons == null)
                {
                    _customButtons = new CustomButtonCollection();

                    AdwiseCustomButtons();
                }

                return _customButtons;
            }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        [Description("Gets or sets the visual style.")]
        [DefaultValue(VisualStyles.Office2007)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public VisualStyles VisualStyle
        {
            get
            {
                return _visualStyle;
            }
            set
            {
                if (_visualStyle != value)
                {
                    _visualStyle = value;
                    OnVisualStyleChanged();
                    setButtonstyle(value);
                }
            }
        }

        private void setButtonstyle(VisualStyles style)
        {
            for (int i = 0; i < this.CustomButtons.Count ; i++)
            {
                if (style == VisualStyles.Metro)
                    this.CustomButtons[i].Appearance = ButtonAppearance.Metro;
                else if (style == VisualStyles.Office2007 )
                    this.CustomButtons[i].Appearance = ButtonAppearance.Office2007;
                else if (style == VisualStyles.Vista )
                    this.CustomButtons[i].Appearance = ButtonAppearance.Office2007 ;
            }
        }
        /// <summary>
        /// Gets or sets the current selected bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        [Editor(typeof(SelectedBarEditor), typeof(UITypeEditor))]
        [Category("Behavior")]
        [Description("Current selected bar.")]
        public Bar SelectedBar
        {
            get
            {
                return _selectedBar;
            }
            set
            {
                if (this.SelectedBar != value)
                {
                    if (this.HistoryEnabled && this.HistoryManager != null)
                    {
                        Command barCmd = new Command(this, value, this.SelectedBar);
                        this.HistoryManager.Do(barCmd);
                    }

                    SelectBar(value);
                }
            }
        }

        private void SelectBar(Bar bar)
        {
            List<Bar> path = new List<Bar>();

            if (bar != null)
            {
                if (!bar.Visible)
                {
                    throw new InvalidOperationException("Hidden bar can't be selected.");
                }
                else if (!bar.Enabled)
                {
                    throw new InvalidOperationException("Disabled bar can't be selected.");
                }
            }

            if (bar == null || IsValidBar(bar, _bars, path))
            {
                if (OnSelectedBarChanging(bar, false))
                {
                    Bar prevBar = _selectedBar;

                    _selectedBar = bar;

                    OnSelectedBarChanged(path, prevBar);
                }
            }
            else
            {
                throw new ArgumentException("Selected bar doesn't belong to the control.");
            }
        }

        /// <summary>
        /// Gets the chain (path) of the <see cref="Bar"/>s ended with <see cref="NavigationView.SelectedBar"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Bar> SelectedBars
        {
            get
            {
                return this.Path;
            }
        }

        /// <summary>
        /// Gets or sets the chain (path) of the <see cref="Bar"/>s ended with <see cref="NavigationView.SelectedBar"/>.
        /// </summary>
        protected List<Bar> Path
        {
            get
            {
                if (_path == null)
                {
                    _path = new List<Bar>();
                }

                return _path;
            }
            set
            {
                if (value != null && _path != value)
                {
                    for (int i = 0, count = value.Count; i < count; i++)
                    {
                        Bar bar = value[i];

                        if (!bar.Visible)
                        {
                            throw new ArgumentException("Collection can't contain hidden bar.");
                        }
                        else if (i > 0)
                        {
                            Bar parent = value[i - 1];

                            if (!parent.Bars.Contains(bar))
                            {
                                throw new ArgumentException("Collection doesn't contain valid hierarchy.");
                            }
                        }
                        else if (!IsValidBar(bar))
                        {
                            throw new ArgumentException("Collection contains bar that doesn't belong to the control.");
                        }
                    }

                    _path = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is in editing mode.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Browsable(false)]
        public bool InEditingMode
        {
            get
            {
                return _editingMode;
            }
            set
            {
                if (_editingMode != value)
                {
                    _editingMode = value;

                    OnEditingModeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bar should go into edit mode.
        /// </summary>
        [Description("Gets or sets whether the bar should go into edit mode."), DefaultValue(true)]
        public bool AllowEditMode
        {
            get
            {
                return allowEdit;
            }
            set
            {
                if (allowEdit != value)
                {
                    allowEdit = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image list for <see cref="Bar"/>s.
        /// </summary>
        [Description("Image list for bars.")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public ImageList ImageList
        {
            get
            {
                return _imageList;
            }
            set
            {
                if (_imageList != value)
                {
                    _imageList = value;
                    _recentList.SmallImageList = value;

                    OnImageListChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the disabled image list for <see cref="Bar"/>s.
        /// </summary>
        [Description("Disabled image list for bars.")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public ImageList DisabledImageList
        {
            get
            {
                return _disabledImageList;
            }
            set
            {
                if (_disabledImageList != value)
                {
                    _disabledImageList = value;

                    OnDisabledImageListChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show history buttons.
        /// </summary>
        /// <value><c>true</c> to show history buttons; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to show history buttons.")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool ShowHistoryButtons
        {
            get
            {
                return _showHistoryButtons;
            }
            set
            {
                if (_showHistoryButtons != value)
                {
                    _showHistoryButtons = value;

                    OnShowHistoryButtonsChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="NavigationView"/> is initializing.
        /// </summary>
        [Browsable(false)]
        public bool Initializing
        {
            get
            {
                return _initializing > 0;
            }
        }

        /// <summary>
        /// Gets the text box for edit mode.
        /// </summary>
        protected internal TextBoxExt TextBox
        {
            get
            {
                return _textBox;
            }
        }

        /// <summary>
        /// Gets or sets the size of the recently visited popup drop-down.
        /// </summary>
        [Description("Size of the recently visited popup drop-down.")]
        [Category("Behavior")]
        public Size PopupDropDownSize
        {
            get
            {
                return _popupDropDownSize;
            }
            set
            {
                _popupDropDownSize = value;
            }
        }

        private bool ShouldSerializePopupDropDownSize()
        {
            return !this.PopupDropDownSize.IsEmpty;
        }

        private void ResetPopupDropDownSize()
        {
            this.PopupDropDownSize = Size.Empty;
        }

        /// <summary>
        /// Gets or sets  the recently visited paths.
        /// </summary>
        /// <value>The array of strings.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] RecentlyVisitedPaths
        {
            get
            {
                int i = 0;
                string[] paths = new string[_recentList.Items.Count];

                foreach (ListViewItem lvi in _recentList.Items)
                {
                    paths[i++] = lvi.Text;
                }

                return paths;
            }
            set
            {
                foreach (string path in value)
                {
                    Bar bar = GetAvailableBarFromPath(path, DefaultPathSeparator);

                    AddBarToRecentList(bar);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show border.
        /// </summary>
        /// <value><c>true</c> to show border; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to show bars' area border.")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool ShowBorder
        {
            get
            {
                return _showBorder;
            }
            set
            {
                if (_showBorder != value)
                {
                    _showBorder = value;
                    _visualizer.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the bars' area border.
        /// </summary>
        /// <value>The color of the border.</value>
        [Description("Color of the bars' area border.")]
        [DefaultValue(typeof(Color), "Navy")]
        [Category("Appearance")]
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate();
                }
            }
        }

        #region Hidden

        /// <summary>
        /// Gets or sets the background image displayed in the control.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Drawing.Image"/> that represents the image to display in the background of the control.</returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image layout as defined in the <see cref="T:System.Windows.Forms.ImageLayout"/> enumeration.
        /// </summary>
        /// <value></value>
        /// <returns>One of the values of <see cref="T:System.Windows.Forms.ImageLayout"/> (<see cref="F:System.Windows.Forms.ImageLayout.Center"/> , <see cref="F:System.Windows.Forms.ImageLayout.None"/>, <see cref="F:System.Windows.Forms.ImageLayout.Stretch"/>, <see cref="F:System.Windows.Forms.ImageLayout.Tile"/>, or <see cref="F:System.Windows.Forms.ImageLayout.Zoom"/>). <see cref="F:System.Windows.Forms.ImageLayout.Tile"/> is the default value.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The specified enumeration value does not exist. </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Drawing.Color"/> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"/> property.</returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to show text of root <see cref="Bar"/>s.
        /// </summary>
        [Description("Indicates whether to show text of root Bars")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool ShowRootBarText
        {
            get
            {
                return _showRootBarText;
            }
            set
            {
                if (_showRootBarText != value)
                {
                    _showRootBarText = value;

                    _visualizer.UpdateLayout();
                }
            }
        }

        #endregion

        #region Implementation

        private void OnVisualStyleChanged()
        {
            _visualizer.Style = this._visualStyle;
        }

        private bool OnSelectedBarChanging(Bar bar, bool cancel)
        {
            if (this.BarSelectionChanging != null)
            {
                BarSelectionChangingEventArgs e = new BarSelectionChangingEventArgs(cancel, bar);

                this.BarSelectionChanging(this, e);

                cancel = e.Cancel;
            }

            return !cancel;
        }

        private void OnSelectedBarChanged(List<Bar> path, Bar prevBar)
        {
            _path = path;

            _visualizer.UpdateLayout();
            FireBarSelectionChangedEvent();
        }

        private void FireBarSelectionChangedEvent()
        {
            if (this.BarSelectionChanged != null)
            {
                BarSelectionChangedEventArgs e = new BarSelectionChangedEventArgs(this.SelectedBar);

                this.BarSelectionChanged(this, e);
            }
        }

        private void OnImageListChanged()
        {
            _autoComplete.ImageList = this.ImageList;
            _visualizer.UpdateLayout();
        }

        private void OnDisabledImageListChanged()
        {
        }

        private void OnBarsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            CollectionChangeEventHandler handler = null;
            Bar bar = (Bar)e.Element;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        AdwiseBar(bar);
                        handler = this.BarAdded;
                        break;
                    }
                case CollectionChangeAction.Remove:
                    {
                        UnadwiseBar(bar);
                        handler = this.BarRemoved;
                        break;
                    }
            }

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void AdwiseBar(Bar bar)
        {
            BarCollection bars = bar.Bars;

            bar.PropertyChanged += new PropertyChangedEventHandler(OnBarPropertyChanged);
            bars.CollectionChanged += new CollectionChangeEventHandler(OnBarsCollectionChanged);

            foreach (Bar b in bars)
            {
                AdwiseBar(b);
            }
        }

        private void UnadwiseBar(Bar bar)
        {
            BarCollection bars = bar.Bars;

            bar.PropertyChanged -= new PropertyChangedEventHandler(OnBarPropertyChanged);
            bars.CollectionChanged -= new CollectionChangeEventHandler(OnBarsCollectionChanged);

            foreach (Bar b in bars)
            {
                UnadwiseBar(b);
            }
        }

        private void OnBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.BarPropertyChanged != null)
            {
                this.BarPropertyChanged(sender, e);
            }

            Bar.Properties prop = Bar.Helper.ParsePropertyEnumString(e.PropertyName);

            if (prop == Bar.Properties.AutoExpand)
            {
                Point pt = PointToClient(Cursor.Position);

                if (this.ClientRectangle.Contains(pt))
                {
                    MouseEventArgs me = new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0);

                    HandleMouse(me, AreaStates.Hot);
                }
            }
            else
            {
                if (prop == Bar.Properties.Enabled || prop == Bar.Properties.Visible)
                {
                    Bar bar = sender as Bar;

                    if (bar != null && bar.IsSelected(this))
                    {
                        if (!bar.Enabled)
                        {
                            bar.Enabled = true;
                            throw new InvalidOperationException("Selected bar can't be disabled.");
                        }
                        else if (!bar.Visible)
                        {
                            bar.Visible = true;
                            throw new InvalidOperationException("Selected bar can't be hidden.");
                        }
                    }
                }

                _visualizer.UpdateLayout();
            }
        }

        private void OnShowHistoryButtonsChanged()
        {
            _visualizer.UpdateLayout(false);
            this.Height = 0; // To reset control's height.
            UpdateHistoryButtonsState();
        }

        private void UpdateHistoryButtonsState()
        {
            _visualizer.UpdateHistoryButtonsState(true);
        }

        private void OnLayoutChanged()
        {
            _visualizer.UpdateLayout();
            UpdateTextBoxLayout();
        }

        private void Invalidate(List<LayoutInfo> areas)
        {
            foreach (LayoutInfo li in areas)
            {
                Rectangle bounds = li.Bounds;

                if (li.Area == HitTestAreas.BarImage)
                {
                    bounds.Inflate(1, 1);
                }

                Invalidate(bounds);
            }
        }

        #region Mouse handling

        private void HandleMouse(MouseEventArgs e, AreaStates normal, AreaStates leftButton)
        {
            List<LayoutInfo> areas = _visualizer.States.ClearAreaStates();

            _visualizer.UpdateHistoryButtonsState(false);

            Point pt = new Point(e.X, e.Y);
            LayoutInfo li = _visualizer.HitTest(pt);

            if (li != null)
            {
                AreaStates state = (e.Button != MouseButtons.Left) ? normal : leftButton;

                if (li.Area == HitTestAreas.Bar)
                {
                    areas.Add(li);
                    HandleMouseOverBarArea(e, pt, ref li, ref state);
                }
                else if (_parentBarItem.Tag != null && _popupMenu.IsShowing())
                {
                    _popupMenu.Hide();
                }

                AreaStates prevState = _visualizer.States.GetAreaState(li);

                if (prevState != AreaStates.Disabled && state != prevState)
                {
                    _visualizer.States.SetAreaState(li, state);
                    areas.Add(li);
                }
            }

            Invalidate(areas);
        }

        private void HandleMouseOverBarArea(MouseEventArgs e, Point pt, ref LayoutInfo li, ref AreaStates state)
        {
            BarLayoutInfo bli = (BarLayoutInfo)li;
            Bar bar = this.SelectedBars[bli.Index];

            if (state != AreaStates.Disabled && _parentBarItem.Tag != null && _popupMenu.IsShowing() && bar.AutoExpand)
            {
                state = AreaStates.Pressed;

                if (_parentBarItem.Tag != bli)
                {
                    pt = bli.DropDown.Bounds.Location;
                    e = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);

                    _popupMenu.Hide();
                    HandleMouseClick(e);
                }
            }

            AreaStates prevBarState = _visualizer.States.GetAreaState(bli);

            li = bli.HitTest(pt);

            if (li.Area == HitTestAreas.BarDropDown && state == AreaStates.Hot)
            {
                _visualizer.States.SetAreaState(bli, _popupMenu.IsShowing() ? state : AreaStates.Active);
            }
            else if (prevBarState != AreaStates.Disabled && prevBarState != state)
            {
                _visualizer.States.SetAreaState(bli, state);
            }
        }

        private void HandleMouse(MouseEventArgs e, AreaStates normal)
        {
            HandleMouse(e, normal, normal);
        }

        private void HandleMouseClick(MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            LayoutInfo li = _visualizer.HitTest(pt);

            if (li != null)
            {
                AreaStates state = _visualizer.States.GetAreaState(li);

                if (state != AreaStates.Disabled)
                {
                    switch (li.Area)
                    {
                        case HitTestAreas.HistoryBackButton:
                            {
                                HistoryBack();
                                break;
                            }

                        case HitTestAreas.HistoryForwardButton:
                            {
                                HistoryForward();
                                break;
                            }

                        case HitTestAreas.HistoryDropDownButton:
                            {
                                ShowHistoryDropDown();
                                break;
                            }

                        case HitTestAreas.BarImage:
                        case HitTestAreas.Space:
                            {
                                if (this.AllowEditMode)
                                    this.InEditingMode = true;
                                break;
                            }

                        case HitTestAreas.DropDownButton:
                            {
                                if (this.AllowEditMode)
                                {
                                    this.InEditingMode = true;
                                    ShowRecentlyVisitedDropDown();
                                }
                                break;
                            }

                        case HitTestAreas.Bar:
                            {
                                HandleMouseClickOnBarArea(pt, li);
                                break;
                            }
                    }
                }
            }
        }

        private void HandleMouseClickOnBarArea(Point pt, LayoutInfo li)
        {
            BarLayoutInfo barLayout = (BarLayoutInfo)li;
            LayoutInfo barAreaLayout = barLayout.HitTest(pt);
            Bar bar = this.SelectedBars[barLayout.Index];

            switch (barAreaLayout.Area)
            {
                case HitTestAreas.BarDropDown:
                    {
                        PopulateAndShowPopup(bar, barLayout);
                        break;
                    }
                case HitTestAreas.BarImage:
                case HitTestAreas.BarText:
                    {
                        this.SelectedBar = bar;
                        break;
                    }
            }
        }

        #endregion

        #region Popup

        private void PopulateAndShowPopup(Bar bar, BarLayoutInfo barLayout)
        {
            BarItems items = _parentBarItem.Items;
            BarCollection bars = new BarCollection();
            bool showChevron = barLayout.ShowShevron;
            int groupAfter = -1;

            items.SuspendEvents();

            bool topBars = showChevron || (!this.ShowRootBarText && bar == _visualizer.Layouter.RootLayoutBar);

            if (topBars)
            {
                for (int i = 0; i <= barLayout.Index; ++i)
                {
                    bars.Insert(0, this.SelectedBars[i]);
                }

                PopulateSubBarsPopup(bars, true);

                groupAfter = items.Count;
            }

            PopulateSubBarsPopup(bar.Bars, !topBars);

            if (topBars && groupAfter < items.Count)
            {
                BarItem item = items[groupAfter];

                item = items[groupAfter];
                _parentBarItem.BeginGroupAt(item);
            }

            items.ResumeEvents(true);

            BarPopupEventArgs args = new BarPopupEventArgs(bar);
            this.OnBarPopup(args);

            if (args.Cancel)
                return;

            _parentBarItem.MaximumItemsToDisplay = args.MaximumItemsToDisplay;

            ShowBarPopup(barLayout);
        }

        private void PopulateSubBarsPopup(BarCollection subBars, bool clearItems)
        {
            if (clearItems)
            {
                ClearPopup();
            }

            BarItems items = _parentBarItem.Items;

            foreach (Bar subBar in subBars)
            {
                PopulateSubBar(
                    subBar, 
                    items, 
                    OnPopupMenuItemClick, 
                    delegate(BarItem bi)            
                    {
                        bi.Tag = subBar;

                        if (this.SelectedBars.Contains(subBar))
                        {
                            BoldBarItem(bi);
                        }
                    });
            }
        }

        private delegate void ProcessBarItem(BarItem bi);

        private BarItem PopulateSubBar(Bar subBar, BarItems items, EventHandler eventHandler, ProcessBarItem processBarItem)        
        {
            BarItem item = null;

            if (subBar.Visible)
            {
                item = new BarItem(subBar.Text);

                item.Image = (subBar.Image != null) ? (ImageExt)subBar.Image : null;
                item.DisabledImage = (subBar.DisabledImage != null) ? (ImageExt)subBar.DisabledImage : null;
                item.ImageIndex = subBar.ImageIndex;
                item.DisabledImageIndex = subBar.DisabledImageIndex;
                item.ImageList = this.ImageList;
                item.DisabledImageList = this.DisabledImageList;
                item.Enabled = subBar.Enabled;
                item.Click += eventHandler;
                item.CustomTextFont = this.Font;

                if (processBarItem != null)
                {
                    processBarItem(item);
                }

                items.Add(item);
            }

            return item;
        }

        private void BoldBarItem(BarItem bi)
        {
            Font font = new Font(bi.CustomTextFont, FontStyle.Bold);

            bi.CustomTextFont = font;
        }

        private void ClearPopup()
        {
            BarItems items = _parentBarItem.Items;

            foreach (BarItem item in items)
            {
                item.Dispose();
            }

            items.Clear();
        }

        /// <summary>
        /// Occurs when dropdown is popped up, which is a cancellable event.
        /// </summary>
        /// <remarks>
        /// <example>
        /// The following example shows how this can be used better
        /// <code lang="C#">
        /// Example use case:
        ///this.navigationView1.BarPopup += new EventHandler<Syncfusion.Windows.Forms.Tools.BarPopupEventArgs>(navigationView1_BarPopup);
        ///
        ///void navigationView1_BarPopup(object sender, Syncfusion.Windows.Forms.Tools.BarPopupEventArgs e)
        ///{
        ///    if (e.CurrentBar.Text.Equals("Dell"))
        ///        e.Cancel = true;
        ///
        ///    if(e.CurrentBar.Text.Equals("Program Files"))
        ///        e.MaximumItemsToDisplay = 13;
        ///    else
        ///        e.MaximumItemsToDisplay = 5;
        ///}
        /// </code>
        /// 
        /// <code lang="VB">
        /// 
        /// Private Me.navigationView1.BarPopup += New EventHandler(Of Syncfusion.Windows.Forms.Tools.BarPopupEventArgs)(AddressOf navigationView1_BarPopup)
        ///
		/// Private Sub navigationView1_BarPopup(ByVal sender As Object, ByVal e As Syncfusion.Windows.Forms.Tools.BarPopupEventArgs)
		///	If e.CurrentBar.Text.Equals("Dell") Then
		///		e.Cancel = True
		///	End If
        ///
		///	If e.CurrentBar.Text.Equals("Program Files") Then
		///		e.MaximumItemsToDisplay = 13
		///	Else
		///		e.MaximumItemsToDisplay = 5
		///	End If
		/// End Sub
        /// </code>
        /// </example>
        /// </remarks>
        [Description("Occurs when the drop down is popped up.")]
        public event EventHandler<BarPopupEventArgs> BarPopup;

        protected virtual void OnBarPopup(BarPopupEventArgs e)
        {
            if (BarPopup != null)
            {
                this.BarPopup(this, e);
            }
        }

        private void ShowBarPopup(BarLayoutInfo barLayout)
        {
            if (_parentBarItem.Items.Count > 0)
            {
                switch (this.VisualStyle)
                {
                    case VisualStyles.Office2007:
                        {
                            _parentBarItem.Style = Forms.VisualStyle.Office2007;
                            _parentBarItem.Office2007Theme = this.Office2007ColorTheme;
                            _popupMenu.ForcedVistaStyle = false;
                            break;
                        }
                    case VisualStyles.Vista:
                        {
                            _popupMenu.ForcedVistaStyle = true;
                            break;
                        }
                    case VisualStyles.Metro:
                        {
                            //need metro implementation
                            _popupMenu.ForcedVistaStyle = true;
                            break;
                        }
                }

                Rectangle rcDD = barLayout.DropDown.Bounds;
                int offset = rcDD.Width / 2 + 1;
                Point ptPopup;

                if (RightToLeft.Yes != this.RightToLeft)
                {
                    ptPopup = new Point(rcDD.Left - offset, rcDD.Bottom);
                }
                else
                {
                    ptPopup = new Point(rcDD.Right + offset, rcDD.Bottom);
                }

                _parentBarItem.Tag = barLayout;

                _popupMenu.Show(this, ptPopup);
            }
            else
            {
                _popupMenu.Hide();
            }
        }

        private void OnPopupMenuItemClick(object sender, EventArgs e)
        {
            BarItem item = (BarItem)sender;
            Bar navBar = item.Tag as Bar;

            if (navBar != null && navBar.Enabled)
            {
                this.SelectedBar = navBar;
            }
        }

        private void OnPopupMenuClosed(object sender, EventArgs e)
        {
            List<LayoutInfo> areas = _visualizer.States.ClearAreaStates();

            _parentBarItem.Tag = null;
            _visualizer.UpdateLayout();

            Invalidate(areas);
        }

        #endregion

        #region History

        private void HistoryBack()
        {
            try
            {
                this.InEditingMode = false;
                this.HistoryEnabled = false;
                this.HistoryManager.Undo();
            }
            finally
            {
                this.HistoryEnabled = true;
                UpdateHistoryButtonsState();
            }
        }

        private void HistoryForward()
        {
            try
            {
                this.InEditingMode = false;
                this.HistoryEnabled = false;
                this.HistoryManager.Redo();
            }
            finally
            {
                this.HistoryEnabled = true;
                UpdateHistoryButtonsState();
            }
        }

        private void ShowHistoryDropDown()
        {
            PopulateHistoryDropDown();

            if (_parentBarItem.Items.Count > 0)
            {
                Rectangle hddBounds = _visualizer.Layouter.GetLayout(HitTestAreas.HistoryDropDownButton).Bounds;
                Point ptPopup = new Point(hddBounds.Left, this.ClientRectangle.Bottom);

                _popupMenu.Show(this, ptPopup);
            }
        }

        private void PopulateHistoryDropDown()
        {
            HistoryManager hm = this.HistoryManager as HistoryManager;

            if (hm != null)
            {
                ClearPopup();

                BarItems items = _parentBarItem.Items;

                items.SuspendEvents();

                List<ICommand> undoes = new List<ICommand>(hm.Undoes);

                undoes.Reverse();
                PopulateHistoryCommands(items, undoes);

                BarItem current = PopulateSubBar(this.SelectedBar, items, null, null);

                if (current != null)
                {
                    BoldBarItem(current);
                }

                PopulateHistoryCommands(items, hm.Redoes);

                items.ResumeEvents(true);
            }
        }

        public class HistoryDropDownInfo
        {
            public HistoryDropDownInfo(Bar bar, int index)
            {
                Bar = bar;
                Index = index;
            }

            public Bar Bar;
            public int Index;
        }

        private void PopulateHistoryCommands(BarItems items, IList<ICommand> undo)
        {
            foreach (Command cmd in undo)
            {
                PopulateSubBar(
                    cmd.Bar, 
                    items, 
                    OnHistoryDropDownItemClick, 
                    delegate(BarItem bi) { bi.Tag = new HistoryDropDownInfo(cmd.Bar, items.Count); } );
            }
        }

        private void OnHistoryDropDownItemClick(object sender, EventArgs e)
        {
            BarItem item = (BarItem)sender;
            HistoryDropDownInfo info = item.Tag as HistoryDropDownInfo;
            HistoryManager hm = this.HistoryManager as HistoryManager;

            if (info != null && hm != null)
            {
                Bar navBar = info.Bar;

                if (navBar != null && navBar != this.SelectedBar && navBar.Enabled)
                {
                    int index = info.Index;
                    int selectedIndex = hm.Undoes.Count;
                    int count = Math.Abs(index - selectedIndex);

                    try
                    {
                        this.HistoryEnabled = false;

                        if (index < selectedIndex)
                        {
                            while (--count >= 0)
                            {
                                hm.Undo();
                            }
                        }
                        else
                        {
                            while (--count >= 0)
                            {
                                hm.Redo();
                            }
                        }
                    }
                    finally
                    {
                        this.HistoryEnabled = true;
                    }
                }
            }
        }

        #endregion

        #region Custom buttons

        private void AdwiseCustomButtons()
        {
            _customButtons.CollectionChanged += new CollectionChangeEventHandler(OnCustomButtonsCollectionChanged);
        }

        private void UnadwiseCustomButtons()
        {
            _customButtons.CollectionChanged -= new CollectionChangeEventHandler(OnCustomButtonsCollectionChanged);
        }

        private void OnCustomButtonsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            CustomButton btn = e.Element as CustomButton;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        this.Controls.Add(btn);
                        btn.Layout += new LayoutEventHandler(OnCustomButtonLayout);
                        break;
                    }

                case CollectionChangeAction.Remove:
                    {
                        btn.Layout -= new LayoutEventHandler(OnCustomButtonLayout);
                        this.Controls.Remove(btn);
                        break;
                    }
            }

            _visualizer.UpdateLayout();
        }

       public void OnCustomButtonLayout(object sender, LayoutEventArgs e)
        {
            _visualizer.UpdateLayout();
        }

        #endregion

        #region Editing mode

        private void OnEditingModeChanged()
        {
            _visualizer.UpdateLayout();

            if (this.AllowEditMode && this.InEditingMode)
            {
                if (this.TextBox.Parent != this)
                {
                    this.TextBox.Parent = this;
                }

                UpdateTextBoxLayout();

                this.TextBox.Text = this.SelectedPath;
                this.TextBox.SelectAll();

                UpdateAutocompleteItems(DefaultPathSeparator);

                this.TextBox.Visible = true;
                this.TextBox.Focus();
            }
            else
            {
                this.TextBox.Visible = false;
            }
        }

        private void UpdateTextBoxLayout()
        {
            LayoutInfo editLayout = _visualizer.Layouter.GetLayout(HitTestAreas.TextBox);

            if (editLayout != null)
            {
                this.TextBox.Bounds = editLayout.Bounds;
            }
        }

        private void UpdateAutocompleteItems(string separator)
        {
            foreach (Bar bar in this.Bars)
            {
                IEnumerator<BarInfo> enumerator = GetAllPaths(bar, separator, String.Empty).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    BarInfo bi = (BarInfo)enumerator.Current;

                    if (bi.Path != DefaultPathSeparator)
                    {
                        _autoComplete.AddHistoryItem(bi.Path, bi.ImageIndex);
                    }
                }
            }
        }

        private IEnumerable<BarInfo> GetAllPaths(Bar bar, string separator, string parentPath)
        {
            string path = String.IsNullOrEmpty(parentPath) ? bar.Text : parentPath + bar.Text;

            yield return new BarInfo(path, bar);

            foreach (Bar b in bar.Bars)
            {
                if (b.Enabled && b.Visible)
                {
                    IEnumerator<BarInfo> enumerator = GetAllPaths(b, separator, path + separator).GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        private string GetSelectedPath(string separator)
        {
            return GetBarPath(null, separator);
        }

        private string GetBarPath(Bar bar, string separator)
        {
            List<Bar> bars = this.SelectedBars;
            StringBuilder path = new StringBuilder();

            foreach (Bar b in bars)
            {
                path.Append(b.Text);
                path.Append(separator);
                if (bar == b)
                {
                    break;
                }
            }

            int pathLen = path.Length;
            int sepLen = separator.Length;

            if (pathLen > sepLen)
            {
                path.Remove(pathLen - sepLen, sepLen);
            }

            return path.ToString();
        }

        private string SelectedPath
        {
            get
            {
                return GetSelectedPath(DefaultPathSeparator);
            }
        }

        #endregion

        #region Text box

        private void OnTextBoxLostFocus(object sender, EventArgs e)
        {
            this.InEditingMode = false;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        if (!_autoComplete.IsDropDownShowing())
                        {
                            this.InEditingMode = false;
                        }

                        break;
                    }
                case Keys.Enter:
                    {
                        if (!_autoComplete.IsDropDownShowing())
                        {
                            SelectPath(this.TextBox.Text);
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if ((e.Modifiers & Keys.Alt) != 0)
                        {
                            ShowRecentlyVisitedDropDown();
                        }
                        else
                        {
                            _autoComplete.AutoCompletePopup.RightToLeft = this.RightToLeft;
                            _autoComplete.AutoCompletePopup.ShowPopup(GetPopupLocation());
                        }
                        break;
                    }
                default:
                    {
                        e.Handled = false;
                        break;
                    }
            }
        }

        private Bar GetAvailableBarFromPath(string path, string separator)
        {
            Bar bar = null;
            string[] barTexts = path.Split(new string[] { separator }, StringSplitOptions.None);
            BarCollection bars = this.Bars;
            IEnumerator enumer = barTexts.GetEnumerator();

            while (enumer.MoveNext())
            {
                string text = (string)enumer.Current;

                if (text != separator)
                {
                    int i = bars.FindByText(text);

                    if (i >= 0)
                    {
                        bar = bars[i];
                        bars = bar.Bars;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return bar;
        }

        private void SelectPath(string path)
        {
            this.InEditingMode = false;

            Bar bar = GetAvailableBarFromPath(path, DefaultPathSeparator);
            this.SelectedBar = bar;

            AddBarToRecentList(bar);
        }

        #endregion

        #region Autocomplete

        private void OnAutoCompleteItemSelected(object sender, AutoCompleteItemEventArgs args)
        {
            SelectPath(args.SelectedValue);
        }

        private void OnAutoCompleteDropDownClosed(object sender, PopupClosedEventArgs e)
        {
            if (this.PopupClosed != null)
            {
                this.PopupClosed(this, EventArgs.Empty);
            }
        }

        private void OnAutoCompleteDropDownDisplayed(object sender, EventArgs e)
        {
            if (this.PopupShown != null)
            {
                this.PopupShown(this, EventArgs.Empty);
            }
        }

        private Point GetPopupLocation()
        {
            LayoutInfo layoutSpace = _visualizer.Layouter.GetLayout(HitTestAreas.Space);
            Point location = new Point(0, layoutSpace.Bounds.Bottom);
            if (RightToLeft.Yes != this.RightToLeft)
            {
                if (this.ShowHistoryButtons)
                {
                    LayoutInfo layoutHDDB = _visualizer.Layouter.GetLayout(HitTestAreas.HistoryDropDownButton);
                    location.X = layoutHDDB.Bounds.Right;
                }
            }
            else
            {
                LayoutInfo layoutDDB = _visualizer.Layouter.GetLayout(HitTestAreas.DropDownButton);

                location.X = layoutDDB.Bounds.Right;
            }

            return PointToScreen(location);
        }

        #endregion

        #region Recently visited drop-down

        private void ShowRecentlyVisitedDropDown()
        {
            if (_recentList.Items.Count > 0)
            {
                LayoutInfo li = _visualizer.Layouter.GetLayout(HitTestAreas.DropDownButton);
                Size size = this.PopupDropDownSize;

                if (size.IsEmpty)
                {
                    bool bLTR = this.RightToLeft != RightToLeft.Yes;
                    int width = -1;

                    width += bLTR ? (li.Bounds.Left - this.ClientRectangle.Left) : (this.ClientRectangle.Width - li.Bounds.Right);

                    if (this.ShowHistoryButtons)
                    {
                        LayoutInfo layoutHBDD = _visualizer.Layouter.GetLayout(HitTestAreas.HistoryDropDownButton);

                        width -= bLTR ? layoutHBDD.Bounds.Right : (this.ClientRectangle.Width - layoutHBDD.Bounds.Left);
                    }

                    Screen screen = Screen.FromControl(this);

                    size = new Size(width, width * screen.Bounds.Height / screen.Bounds.Width);

                    int height = 0;

                    foreach (ListViewItem lvi in _recentList.Items)
                    {
                        int itemHeight = _recentList.GetItemRect(lvi.Index).Height;

                        if ((height + itemHeight) > size.Height)
                        {
                            break;
                        }
                        else
                        {
                            height += itemHeight;
                        }
                    }

                    size.Height = height;
                    _popupContainer.ClientSize = new Size(size.Width, size.Height);
                }
                else
                {
                    _popupContainer.Width = size.Width;
                    _popupContainer.Height = size.Height;
                }

                _popupContainer.RightToLeft = this.RightToLeft;
                _recentList.RightToLeftLayout = this.RightToLeft == RightToLeft.Yes;

                switch (this.VisualStyle)
                {
                    case VisualStyles.Office2007:
                        {
                            _scrollersFrame.OfficeColorScheme = (Office2007ColorScheme)this.Office2007ColorTheme;
                            break;
                        }

                    case VisualStyles.Vista:
                        {
                            _scrollersFrame.OfficeColorScheme = Office2007ColorScheme.Managed;
                            break;
                        }
                }

                _popupContainer.ShowPopup(GetPopupLocation());

                _columnHeader.Width = _recentList.Width;
            }
        }

        private void OnRecentItemClicked(object sender, EventArgs e)
        {
            _popupContainer.HidePopup();
            this.InEditingMode = false;

            string path = _recentList.FocusedItem.Text;

            SelectPath(path);
        }

        private void AddBarToRecentList(Bar bar)
        {
            string path = GetBarPath(bar, DefaultPathSeparator);

            if (path.Length > 0)
            {
                _recentList.Items.RemoveByKey(path);
                _recentList.Items.Insert(0, path, path, bar.ImageIndex);
            }
        }

        private void AdwiseRecentListPopup()
        {
            _popupContainer.Popup += new EventHandler(OnRecentListPopupShown);
            _popupContainer.CloseUp += new PopupClosedEventHandler(OnRecentListPopupClosed);
        }

        private void UnadwiseRecentListPopup()
        {
            _popupContainer.Popup -= new EventHandler(OnRecentListPopupShown);
            _popupContainer.CloseUp -= new PopupClosedEventHandler(OnRecentListPopupClosed);
        }

        private void OnRecentListPopupShown(object sender, EventArgs e)
        {
            Application.AddMessageFilter(this);
        }

        private void OnRecentListPopupClosed(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether <see cref="NavigationView"/> contains the specified bar.
        /// </summary>
        /// <param name="bar">The bar to be verified.</param>
        /// <returns>
        /// <c>true</c> if the control contains the specified bar; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidBar(Bar bar)
        {
            return IsValidBar(bar, _bars, null);
        }

        /// <summary>
        /// Determines whether <see cref="BarCollection"/> contains the specified bar.
        /// </summary>
        /// <param name="bar">The bar to be verified.</param>
        /// <param name="bars">The collection of bars.</param>
        /// <param name="path">If not null, will contain hierarchy of parent bars <see cref="Bar"/>s of searched bar.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the specified bar; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Method perfomes deep (recursive) search.</remarks>
        protected bool IsValidBar(Bar bar, BarCollection bars, List<Bar> path)
        {
            bool valid = bars.Contains(bar);

            if (!valid)
            {
                foreach (Bar b in bars)
                {
                    if (IsValidBar(bar, b.Bars, path))
                    {
                        valid = true;

                        if (path != null)
                        {
                            path.Insert(0, b);
                        }

                        break;
                    }
                }
            }
            else if (path != null)
            {
                path.Add(bar);
            }

            return valid;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnadwiseRecentListPopup();

                if (_customButtons != null)
                {
                    UnadwiseCustomButtons();

                    _customButtons.Clear();
                }

                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///Size changed
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
             base.OnSizeChanged(e);
             if (!EnableTouchMode && this.DesignMode)
             {
                 CTRLSIZE = this.Size;
             }
        }
        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="pe">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            _visualizer.RenderControl(pe.Graphics);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _visualizer.UpdateLayout();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Layout"/> event.
        /// </summary>
        /// <param name="levent">A <see cref="T:System.Windows.Forms.LayoutEventArgs"/> that contains the event data.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            OnLayoutChanged();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.RightToLeftChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            _visualizer.States.ClearAreaStates();
            _visualizer.UpdateLayout();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            HandleMouse(e, AreaStates.Hot, AreaStates.Pressed);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            HandleMouseClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            HandleMouse(e, AreaStates.Pressed);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            HandleMouse(e, AreaStates.Default);
        }

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The default <see cref="T:System.Drawing.Size"/> of the control.</returns>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(base.DefaultSize.Width, DefaultHeight);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_parentBarItem.Tag == null || !_popupMenu.IsShowing())
            {
                List<LayoutInfo> areas = _visualizer.States.ClearAreaStates();
                _visualizer.UpdateHistoryButtonsState(false);
                Invalidate(areas);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.ControlAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"/> that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            CustomButton btn = e.Control as CustomButton;

            if (btn != null && !this.CustomButtons.Contains(btn))
            {
                this.CustomButtons.Add(btn);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.ControlRemoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"/> that contains the event data.</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            CustomButton btn = e.Control as CustomButton;

            if (btn != null && this.CustomButtons.Contains(btn))
            {
                this.CustomButtons.Remove(btn);
            }
        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new <see cref="P:System.Windows.Forms.Control.Left"/> property value of the control.</param>
        /// <param name="y">The new <see cref="P:System.Windows.Forms.Control.Top"/> property value of the control.</param>
        /// <param name="width">The new <see cref="P:System.Windows.Forms.Control.Width"/> property value of the control.</param>
        /// <param name="height">The new <see cref="P:System.Windows.Forms.Control.Height"/> property value of the control.</param>
        /// <param name="specified">A bitwise combination of the <see cref="T:System.Windows.Forms.BoundsSpecified"/> values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            LayoutInfo layoutDD = _visualizer.Layouter.GetLayout(HitTestAreas.DropDownButton);

            if (layoutDD != null)
            {
                height = layoutDD.Bounds.Height;

                if (this.ShowHistoryButtons)
                {
                    LayoutInfo layoutHBB = _visualizer.Layouter.GetLayout(HitTestAreas.HistoryBackButton);
                    int heightHBB = layoutHBB.Bounds.Height;

                    if (layoutHBB != null && height < heightHBB)
                    {
                        height = heightHBB;
                    }
                }
            }
            float scaleFactorValue = this.EnableTouchMode ? 1.5f : 1.0f;
            height = (int)(CTRLSIZE.Height * scaleFactorValue);
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.FontChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            SetBounds(0, 0, 0, 0, BoundsSpecified.Height);

        }

        #endregion

        #region For Touch

        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;

            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));

            foreach (CustomButton item in this.CustomButtons)
            {
                item.ApplyScaleToControl(scaleFactor);
            }
            
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();

        }

        #endregion

        #region ISupportOffice2007Theme implementation

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                {
                    VisualStyle = VisualStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = VisualStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = VisualStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Black;
                }
                else if (value == "Managed")
                {
                    VisualStyle = VisualStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Managed;
                }
                else if (value == "Vista")
                    VisualStyle = VisualStyles.Vista;
            }
        }
        /// <summary>
        /// Gets or sets <see cref="Office2007Theme"/> to use.
        /// </summary>
        [Description("Specifies Office2007Theme to use.")]
        [DefaultValue(Office2007Theme.Managed)]
        [Category("Appearance")]
        public Office2007Theme Office2007ColorTheme
        {
            get
            {
                return _office2007Theme;
            }
            set
            {
                if (_office2007Theme != value)
                {
                    _office2007Theme = value;

                    if (this.VisualStyle == VisualStyles.Office2007)
                    {
                        _visualizer.UpdateLayout(true);
                    }
                }
            }
        }

        /// <summary>
        /// Enables rendering with <see cref="Office2007Theme"/>.
        /// </summary>
        void ISupportOffice2007Theme.EnableOffice2007Style()
        {
            this.VisualStyle = VisualStyles.Office2007;
        }

        #endregion

        #region ISuppportHistory implementation

        /// <summary>
        /// Gets or sets the <see cref="HistoryManager"/> to use.
        /// </summary>
        [Description("Gets or sets the history manager to use.")]
        [DefaultValue(null)]
        [TypeConverter(typeof(ReferenceConverter))]
        [Category("Behavior")]
        public Syncfusion.Windows.Forms.HistoryManager HistoryManager
        {
            get
            {
                return _historyMan;
            }
            set
            {
                if (_historyMan != value)
                {
                    _historyMan = value;
                    UpdateHistoryButtonsState();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether registered items are in history list.
        /// </summary>
        [Description("Indicates whether registered items are in history list.")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool HistoryEnabled
        {
            get
            {
                return _historyEnabled;
            }
            set
            {
                if (_historyEnabled != value)
                {
                    if (this.HistoryManager != null)
                    {
                        _historyEnabled = value;
                        UpdateHistoryButtonsState();
                    }
                    else
                    {
                        throw new InvalidOperationException("HistoryManager is not set.");
                    }
                }
            }
        }

        #endregion

        #region ISupportInitialize implementation

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            ++_initializing;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (_initializing > 0)
            {
                --_initializing;
            }
        }

        #endregion

        #region IMessageFilter implementation

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            Point pt = Cursor.Position;

            if (_popupHost.IsShowing() && NativeMethods.WM_MOUSEFIRST <= m.Msg && m.Msg <= NativeMethods.WM_MOUSELAST
                && _popupHost.Bounds.Contains(pt))
            {
                pt = _recentList.PointToClient(pt);

                ListViewItem lvi = _recentList.GetItemAt(pt.X, pt.Y);

                if (lvi != null)
                {
                    _recentList.SelectedItems.Clear();
                    lvi.Selected = true;
                }
            }

            return false;
        }

        #endregion

        #region Classes

        public class Command :
            ICommand,
            IDisposable
        {
            #region Fields

            private Bar _bar;
            private Bar _prevBar;
            private NavigationView _nv;

            #endregion

            #region Construction

            public Command(NavigationView nv, Bar bar, Bar prevBar)
            {
                _bar = bar;
                _prevBar = prevBar;
                _nv = nv;
            }

            #endregion

            #region Properties

            public Bar Bar
            {
                get
                {
                    return _bar;
                }
            }

            public Bar PrevBar
            {
                get
                {
                    return _prevBar;
                }
            }

            #endregion

            #region ICommand implementation

            /// <summary>
            /// Executes action.
            /// </summary>
            public void Execute()
            {
                if (_bar != null)
                {
                    _nv.SelectedBar = _bar;
                }
            }

            /// <summary>
            /// Reverses command.
            /// </summary>
            public void Reverse()
            {
                Bar bar = _bar;

                _bar = _prevBar;
                _prevBar = bar;
            }

            #endregion

            #region IDisposable implementation

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _nv = null;
                _bar = null;
            }

            #endregion
        }

        [ToolboxItem(false)]
        public class PopupMenu :
            Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu
        {
            private bool _forcedVistaStyle;

            public PopupMenu(IContainer container) :
                base(container)
            {
            }

            public bool ForcedVistaStyle
            {
                set
                {
                    _forcedVistaStyle = value;
                }
            }

            #region Overrides

            /// <summary>
            /// Indicates whether a specified control is part of the popup hierarchy.
            /// </summary>
            /// <param name="control">Control value</param>
            /// <param name="askParent">Bool vaue</param>
            /// <returns>Returns true if related control</returns>
            public override bool IsRelatedControl(Control control, bool askParent)
            {
                return (control == this.GetParentControl()) || base.IsRelatedControl(control, askParent);
            }

            /// <summary>
            /// Child closing method.
            /// </summary>
            /// <param name="childUI">Popup Child</param>
            /// <param name="popupCloseType">Popup CloseType</param>
            public override void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
            {
                base.ChildClosing(childUI, popupCloseType);

                NavigationView nv = this.GetParentControl() as NavigationView;

                if (nv != null)
                {
                    nv.OnPopupMenuClosed(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Gets the available menu grid.
            /// </summary>
            /// <value></value>
            protected override MenuGrid MenuGrid
            {
                get
                {
                    MenuGrid grid = base.MenuGrid;

                    grid.ForcedVistaStyle = _forcedVistaStyle;
                    grid.ThemesEnabled = _forcedVistaStyle;

                    return grid;
                }
            }

            #endregion
        }

        public class PopupHost :
            Syncfusion.Windows.Forms.PopupHost
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_MOUSEACTIVATE)
                {
                    m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        public class BarInfo
        {
            public string Path;
            public int ImageIndex;

            public BarInfo(string path, Bar bar)
            {
                this.Path = path;
                this.ImageIndex = bar.ImageIndex;
            }
        }

        #endregion
    }

    #region BarPopEventArgs

    /// <summary>
    /// EventArgs for BarPopup event of Navigation view
    /// </summary>
    public class BarPopupEventArgs : CancelEventArgs
    {
        int maximumItemsToDisplay = 19; //  19 items are defaulted in Vista and Windows 7
        Bar _currentBar = null;

        #region Constructor

        public BarPopupEventArgs()
            : base()
        {
        }

        public BarPopupEventArgs(Bar currentBar)
            : base()
        {
            this._currentBar = currentBar;
        }

        public BarPopupEventArgs(bool cancel)
            : base(cancel)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently picked bar.
        /// </summary>
        public Bar CurrentBar
        {
            get
            {
                return this._currentBar;
            }
        }

        /// <summary>
        /// Gets or Sets maximum count of child items to be displayed at one time.
        /// </summary>
        public int MaximumItemsToDisplay
        {
            get
            {
                return this.maximumItemsToDisplay;
            }
            set
            {
                if (this.maximumItemsToDisplay != value)
                    this.maximumItemsToDisplay = value;
            }
        }

        #endregion

    }

#endregion
}
