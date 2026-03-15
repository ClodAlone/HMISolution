#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using Syncfusion.Collections;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Syncfusion.Windows.Forms.Tools.Renderers;


namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents the method that will handle the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.DrawItem"/> event of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/> class.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="drawItemInfo">A DrawTabEventArgs that contains the event data.</param>
    public delegate void DrawTabEventHandler(object sender, DrawTabEventArgs drawItemInfo);

    /// <summary>
    /// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.DrawItem"/> event of
    /// <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/>.
    /// </summary>
    /// <remarks>
    /// <para>This class contains all the information needed for the user to paint the specified
    /// item. It provides the BackColor, ForeColor, Bounds (includes space for the border),
    /// BoundsInterior(does not include space for the borders), etc. It also provides access
    /// to the default drawing logic of the tabs using its DrawXXX methods.</para>
    /// <para>
    /// In case you use the default drawing logic for drawing the borders, then you can use
    /// the BoundsInterior to get the rectangular area without the borders to draw your custom
    /// interior.
    /// </para>
    /// </remarks>
    public class DrawTabEventArgs : EventArgs
    {
        private Font font;
        private readonly Graphics graphics;
        private readonly int index;
        private Rectangle rect;
        private DrawItemState state;
        private Rectangle rectInterior;
        private Brush textBrush;
        private Color foreColor;
        private Color backColor;
        internal DrawDefaultBackground defaultDrawBackground;
        internal DrawDefaultBorders defaultDrawBorders;
        internal DrawDefaultInterior defaultDrawInterior;

        /// <summary>
        /// Performs default background drawing.
        /// </summary>
        public delegate void DrawDefaultBackground(DrawTabEventArgs drawItemInfo);
        /// <summary>
        /// Performs default border drawing.
        /// </summary>
        public delegate void DrawDefaultBorders(DrawTabEventArgs drawItemInfo);
        /// <summary>
        /// Performs default image and text drawing.
        /// </summary>
        public delegate void DrawDefaultInterior(DrawTabEventArgs drawItemInfo);

        /// <summary>
        /// Creates a new instance of the DrawTabEventArgs.
        /// </summary>
        /// <param name="g">The Graphics object into which to draw.</param>
        /// <param name="font">The font using which to draw the tab.</param>
        /// <param name="bounds">The exterior bounds of the tab.</param>
        /// <param name="index">The index of this tab in the TabControlAdv.</param>
        /// <param name="state">The state of this item.</param>
        /// <param name="foreColor">The color of the text.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="boundsInterior">The interior bounds of this tab.</param>
        /// <param name="defaultDrawBackground">A reference to the method that performs default background drawing.</param>
        /// <param name="defaultDrawBorders">A reference to the method that performs default border drawing.</param>
        /// <param name="defaultDrawInterior">A reference to the method that performs default image and text drawing.</param>
        public DrawTabEventArgs(Graphics g, Font font, Rectangle bounds, int index, DrawItemState state, Color foreColor, Color backColor,
            Rectangle boundsInterior,
            DrawDefaultBackground defaultDrawBackground,
            DrawDefaultBorders defaultDrawBorders,
            DrawDefaultInterior defaultDrawInterior)
        {
            this.graphics = g;
            this.font = font;
            this.rect = bounds;
            this.index = index;
            this.state = state;

            this.defaultDrawBackground = defaultDrawBackground;
            this.defaultDrawBorders = defaultDrawBorders;
            this.defaultDrawInterior = defaultDrawInterior;

            this.backColor = backColor;
            this.foreColor = foreColor;
            this.rectInterior = boundsInterior;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        public DrawTabEventArgs(DrawTabEventArgs args)
        {
            this.graphics = args.Graphics;
            this.font = args.Font;
            this.rect = args.Bounds;
            this.index = args.Index;
            this.state = args.State;

            this.defaultDrawBackground = args.defaultDrawBackground;
            this.defaultDrawBorders = args.defaultDrawBorders;
            this.defaultDrawInterior = args.defaultDrawInterior;

            this.backColor = args.BackColor;
            this.foreColor = args.ForeColor;
            this.rectInterior = args.BoundsInterior;
        }
        /// <summary>
        /// Gets / sets the background color.
        /// </summary>
        /// <value>A Color value.</value>
        public Color BackColor
        {
            get
            {
                return this.backColor;
            } // end of method get_BackColor
            set
            {
                this.backColor = value;
            }
        }
        /// <summary>
        /// Gets / sets the color of the text.
        /// </summary>
        /// <value>A Color value.</value>
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            } // end of method get_BackColor
            set
            {
                this.foreColor = value;
            }
        }
        /// <summary>
        /// Returns the index value of the item that is being drawn.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
        }
        /// <summary>
        /// Returns the graphics surface to draw the item on.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }
        /// <summary>
        /// Gets / sets the state of the item being drawn.
        /// </summary>
        public DrawItemState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }
        /// <summary>
        /// Returns the interior of the tab minus the borders.
        /// </summary>
        /// <value>A Rectangle specifying the interior area.</value>
        public Rectangle BoundsInterior
        {
            get { return rectInterior; }
        }
        /// <summary>
        /// Returns the rectangle that represents the bounds of the item that is
        /// being drawn.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return this.rect;
            }
            set
            {
                if (this.rect != value)
                {
                    this.rect = value;
                }
            }
        }
        /// <summary>
        /// Gets / sets the font assigned to the tab being drawn.
        /// </summary>
        public Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                this.font = value;
            }
        }
        /// <summary>
        /// Gets / sets the text brush, with which to draw the text in the tabs.
        /// </summary>
        /// <value>A <see cref="System.Drawing.Brush"/> instance with which to draw the text.</value>
        /// <remarks>
        /// This value will be null when the <see cref="TabControlAdv.DrawItem"/> event gets fired.
        /// If you then specify a brush before calling the <see cref="DrawTabEventArgs.DrawInterior"/> method,
        /// that brush will be used to draw the text.
        /// </remarks>
        public Brush TextBrush
        {
            get { return this.textBrush; }
            set { this.textBrush = value; }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.DrawItemEventArgs.DrawBackground"/>.
        /// </summary>
        public virtual void DrawBackground()
        {
            if (defaultDrawBackground != null)
                defaultDrawBackground(this);
        }
        /// <summary>
        /// Draws the borders within the bounds specified in the DrawTabEventArgs
        /// constructor and with the appropriate color.
        /// </summary>
        /// <remarks>Notes to Inheritors:  When overriding DrawBorders in a derived class, be sure to call the base class's DrawBorders method.
        /// </remarks>
        public void DrawBorders()
        {
            if (this.defaultDrawBorders != null)
                defaultDrawBorders(this);
        }
        /// <summary>
        /// Draws the text and image within the bounds specified in the DrawTabEventArgs
        /// constructor and with the appropriate color.
        /// </summary>
        /// <remarks>Notes to Inheritors:  When overriding DrawInterior in a derived class, be sure to call the base class's DrawInterior method.
        /// </remarks>
        public void DrawInterior()
        {
            if (this.defaultDrawInterior != null)
                defaultDrawInterior(this);
        }
    }

    /// <summary>
    /// Specifies the mechanism by which the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/> will get the default properties
    /// associated with a tab renderer.
    /// </summary>
    public interface ITabDefaultProperties
    {
        /// <summary>
        /// Specifies the default tab panel background color.
        /// </summary>
        Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default tab fore color.
        /// </summary>
        Color DefaultTabForeColor(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default active tab color.
        /// </summary>
        Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default inactive tab color.
        /// </summary>
        Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default single border color.
        /// </summary>
        Color DefaultFixedSingleBorderColor(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default tab panel font.
        /// </summary>
        Font DefaultTabPanelFont(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default active tab font.
        /// </summary>
        Font DefaultActiveTabFont(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the default inactive tab font.
        /// </summary>
        Font DefaultInactiveTabFont(ITabPanelData panelData, ITabControl tabControl);
        /// <summary>
        /// Specifies the amount in X and Y directions, in which a selected tab will
        /// overlap the inactive tab.
        /// </summary>
        /// <param name="tabSize">The user specified tab size, if any. If no size was specified by the user, then
        /// this will be SizeF.Empty.</param>
        /// <remarks>If you return for example (6, 3) then the tab will overlap by 3 pixels
        /// to its left and right and by 3 pixels on top.</remarks>
        SizeF GetOverlapSize(SizeF tabSize);
        /// <summary>
        /// Indicates whether to draw the tabs from left to right or from right to left.
        /// </summary>
        /// <remarks>
        /// This is useful when you implement overlapped tabs.
        /// </remarks>
        bool DrawLeftToRight { get; }
        /// <summary>
        /// Indicates whether to draw ellipsis if text width is larger than tab width.
        /// </summary>
        bool DrawEllipsis { get; }
        /// <summary>
        /// Indicates whether this tab type should be made available in the design-time property grid for the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property.
        /// </summary>
        bool ShowInDesignMode { get; }
        /// <summary>
        /// Draws the tab panel's background.
        /// </summary>
        void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds);
        /// <summary>
        /// Indicates whether the background will be a solid color.
        /// </summary>
        /// <returns>True if solid; false otherwise.</returns>
        bool IsBackgroundSolid();
    }

    /// <summary>
    /// Manages custom <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/>s (custom tab styles).
    /// </summary>
    /// <remarks>
    /// <para>When you create a custom tab style, you have to register it with the framework
    /// through the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererFactory.RegisterTabType"/> method.</para>
    /// </remarks>
    public abstract class TabRendererFactory
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        private static Hashtable tabRenderers = new Hashtable();
        private static Hashtable tabPanelPropertyExtenders = new Hashtable();
        /// <summary>
        /// Registers custom tab styles with the framework.
        /// </summary>
        /// <param name="tabStyleName">A unique name associated with your tab style.</param>
        /// <param name="tabType">A <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation.</param>
        /// <param name="tabPanelPropertyExtender">A <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/> implementation.</param>
        /// <remarks>
        /// <para>Each "tab style" (that you can specify in the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property)
        /// is represented by a type that implements the <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> interface
        /// and a corresponding <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/> implementation to specify certain default tab control properties.
        /// Register these implementations with this method before specifying them in the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property.
        /// A good place to do the registration is from your <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation's
        /// static constructor.
        /// </para>
        /// <para>Note that your <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation should have a non-default constructor that takes
        /// the following 2 parameters: (<see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> and <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/>).</para>
        /// </remarks>
        public static void RegisterTabType(string tabStyleName, Type tabType, ITabDefaultProperties tabPanelPropertyExtender)
        {
            if (tabType == null)
                throw new ArgumentNullException("tabType");
            if (tabPanelPropertyExtender == null)
                throw new ArgumentNullException("tabPanelPropertyExtender");

            if (typeof(ITabRenderer).IsAssignableFrom(tabType))
            {
                tabPanelPropertyExtenders[tabStyleName] = tabPanelPropertyExtender;

                TabRendererFactory.tabRenderers[tabStyleName] = tabType;
            }
            else
                throw new ArgumentException("Cannot Register TabType that does not implement ITabRenderer.");
        }
        /// <summary>
        /// Returns a new instance of a registered <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> given the custom tab style name.
        /// </summary>
        /// <param name="tabStyle">The custom tab style name.</param>
        /// <param name="tabControl">The tab control that requests a new instance.</param>
        /// <param name="panelRenderer">The panel renderer that requests a new instance.</param>
        /// <returns>A <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> instance.</returns>
        public static ITabRenderer CreateNewTabRenderer(string tabStyle, ITabControl tabControl, ITabPanelRenderer panelRenderer)
        {
            Type tabRendererType = tabRenderers[tabStyle] as Type;
            if (tabRendererType != null)
            {
                Type[] constructorArgs = { typeof(ITabControl), typeof(ITabPanelRenderer) };
                ConstructorInfo constructor = tabRendererType.GetConstructor(constructorArgs);
                if (constructor != null)
                {
                    Object[] args = { tabControl, panelRenderer };
                    return constructor.Invoke(args) as ITabRenderer;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns a list of registered <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> types.
        /// </summary>
        /// <param name="forDesignTime">Indicates whether this list is for design-time.</param>
        /// <returns>An ArrayList of <see cref="System.Type"/> instances.</returns>
        /// <remarks>
        /// If forDesignTime is true, then tab styles with the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.ShowInDesignMode"/>
        /// property set to false will be excluded from the returned list.
        /// </remarks>
        public static ArrayList GetRegisteredRenderers(bool forDesignTime)
        {
            if (forDesignTime)
            {
                ArrayList rendererList = new ArrayList();
                foreach (Type rendererType in tabRenderers.Values)
                {
                    string tabStyleName = Design.ReflectionHelper.GetTabNameFromType(rendererType);

                    ITabDefaultProperties defaultProps = tabPanelPropertyExtenders[tabStyleName] as ITabDefaultProperties;

                    if (defaultProps != null && defaultProps.ShowInDesignMode)
                        rendererList.Add(rendererType);
                }
                return rendererList;
            }
            else
                return new ArrayList(tabRenderers.Values);
        }
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance associated with the specified tab style.
        /// </summary>
        /// <param name="tabStyle">The tab style name.</param>
        /// <returns>A <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/> instance.</returns>
        public static ITabDefaultProperties GetRegisteredExtender(string tabStyle)
        {
            return tabPanelPropertyExtenders[tabStyle] as ITabDefaultProperties;
        }
    }
    #region ENUMS
    /// <summary>
    /// Specifies the sizing mode of a <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/> class.
    /// </summary>
    public enum TabSizeMode
    {
        /// <summary>
        /// The width of each tab is sized to accommodate what is displayed on the tab,
        /// and the size of tabs in a row are not adjusted to fill the entire width of
        /// the container control.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The width of each tab is sized so that each row of tabs fills the entire
        /// width of the container control. This is only applicable to tab controls with
        /// more than one row.
        /// </summary>
        FillToRight = 1,
        /// <summary>
        /// All tabs in a control are of the same width.
        /// </summary>
        Fixed = 2,
        /// <summary>
        /// The width of each tab is shrunk so that all the tabs are visible. This property
        /// can be set only when in single-line mode.
        /// </summary>
        ShrinkToFit = 3
    };
    /// <summary>
    /// Specifies the relative alignment of the Image with respect to the text in a <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/>.
    /// </summary>
    [Serializable]
    public enum RelativeImageAlignment
    {
        /// <summary>
        /// The image will be drawn to the left of the text.
        /// </summary>
        LeftOfText = 0,
        /// <summary>
        /// The image will be drawn to the right of the text.
        /// </summary>
        RightOfText = 1,
        /// <summary>
        /// The image will be drawn above the text.
        /// </summary>
        AboveText = 2,
        /// <summary>
        /// The image will be drawn below the text.
        /// </summary>
        BelowText = 3,
        /// <summary>
        /// The text will be drawn over the image.
        /// </summary>
        Overlap = 4
    };
    /// <summary>
    /// Represents the mode in which scrolling will take place in a <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv"/>.
    /// </summary>
    public enum ScrollIncrement
    {
        /// <summary>
        /// Scroll by tab.
        /// </summary>
        Tab,
        /// <summary>
        /// Scroll by page.
        /// </summary>
        Page
    }

    /// <summary>
    /// Specifies the direction in which to scroll.
    /// </summary>
    public enum ScrollDirection
    {
        /// <summary>
        /// Scrolls to the left.
        /// </summary>
        Left,
        /// <summary>
        /// Scrolls to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the preferred vertical alignment of the tabs when the tabs are aligned to the left or right of the control.
    /// </summary>
    public enum TabVerticalAlignment
    {
        /// <summary>
        /// Tabs are rendered based on the Control's RightToLeft property setting.
        /// </summary>
        Default,
        /// <summary>
        /// Tabs are aligned to the top of the control regardless of the RightToLeft property value.
        /// </summary>
        Top,
        /// <summary>
        /// Tabs are aligned to the bottom of the control regardless of the RightToLeft property value.
        /// </summary>
        Bottom
    }

    #endregion ENUMS
    #region DATA_MODEL_INTERFACES
    /// <summary>
    /// Handles the <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.PropertyChanged"/> event.
    /// </summary>
    /// <param name="sender">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/> source of the event.</param>
    /// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.TabPanelPropertyChangedEventArgs"/> that contains the event data.</param>
    public delegate void TabPanelPropertyChangedEventHandler(ITabPanelData sender, TabPanelPropertyChangedEventArgs args);

    /// <summary>
    /// The class that contains data for the <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.PropertyChanged"/> event.
    /// </summary>
    public class TabPanelPropertyChangedEventArgs
    {
        private string changedProperty;
        private object oldValue;
        private object newValue;

        /// <summary>
        /// Creates a new instance of the TabPanelPropertyChangedEventArgs class.
        /// </summary>
        /// <param name="affectedProperty">The affected property.</param>
        /// <param name="oldValue">The old value for the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        public TabPanelPropertyChangedEventArgs(string affectedProperty, object oldValue, object newValue)
        {
            this.changedProperty = affectedProperty;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Returns the property that was changed.
        /// </summary>
        public string PropertyName
        {
            get { return this.changedProperty; }
        }

        /// <summary>
        /// Returns the old value of the property.
        /// </summary>
        public object OldValue
        {
            get { return this.oldValue; }
        }
        /// <summary>
        /// Returns the new value of the property.
        /// </summary>
        public object NewValue
        {
            get { return this.newValue; }
        }
    }
    /// <summary>
    /// The interface that represents the tab panel's data.
    /// </summary>
    public interface ITabPanelData
    {
        /// <summary>
        /// The tab style with which to draw.
        /// </summary>
        string TabStyle { get; }

        /// <summary>
        /// Returns the border style with which to draw.
        /// </summary>
        BorderStyle BorderStyle { get; }

        /// <summary>
        /// Returns the tab alignment with which to align the tabs.
        /// </summary>
        TabAlignment Alignment { get; }
        /// <summary>
        /// Returns the tab size if in fixed width mode.
        /// </summary>
        SizeF TabSize { get; }
        /// <summary>
        /// Returns the tab gap between tabs.
        /// </summary>
        int TabGap { get; }
        /// <summary>
        /// Returns the vertical alignment of the tabs when they are aligned to the right or left.
        /// </summary>
        TabVerticalAlignment VerticalAlignment { get; }
        /// <summary>
        /// Indicates whether to draw the tabs in multiline mode.
        /// </summary>
        bool Multiline { get; }
        /// <summary>
        /// Indicates whether the selected tab should be moved to the front row when in multiline mode.
        /// </summary>
        bool KeepSelectedTabInFrontRow { get; }
        /// <summary>
        /// Returns the tab sizing mode.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Tools.TabSizeMode"/> value.</value>
        TabSizeMode SizeMode { get; }
        /// <summary>
        /// Returns the tab panel's back color.
        /// </summary>
        Color BackColor { get; set; }
        /// <summary>
        /// Returns the border color when in <see cref="System.Windows.Forms.BorderStyle.FixedSingle"/> mode.
        /// </summary>
        Color FixedSingleBorderColor { get; }
        /// <summary>
        /// Returns the tab panel's font.
        /// </summary>
        Font Font { get; set; }
        /// <summary>
        /// Returns the active tab's font.
        /// </summary>
        Font ActiveTabFont { get; set; }
        /// <summary>
        /// Returns the active tab's color.
        /// </summary>
        Color ActiveTabColor { get; }
        /// <summary>
        /// Returns the inactive tab's color.
        /// </summary>
        Color InactiveTabColor { get; }
        /// <summary>
        /// Returns the imagelist.
        /// </summary>
        ImageList ImageList { get; }
        /// <summary>
        /// Returns the selected tab index.
        /// </summary>
        int SelectedIndex { get; set; }
        /// <summary>
        /// Returns the text alignment of the text in the tab.
        /// </summary>
        StringAlignment TextAlignment { get; }
        /// <summary>
        /// Returns the text line alignment of the text in the tab.
        /// </summary>
        StringAlignment TextLineAlignment { get; }
        /// <summary>
        /// Returns the relative image alignment of the images with respect to the text in the tab.
        /// </summary>
        RelativeImageAlignment ImageAlignmentR { get; }
        /// <summary>
        /// Returns y-position of the image.
        /// </summary>
        int ImageOffset { get; }
        /// <summary>
        /// Adjusts the gap between the tabControlAdv's top and the tabs.
        /// </summary>
        int AdjustTopGap { get; }
        /// <summary>
        /// Indicates whether image should be disabled when TabPage is not selected.
        /// </summary>
        bool DisableInactivePageImage { get; }
        /// <summary>
        /// Indicates whether the text and the image should be in the same level.
        /// </summary>
        bool LevelTextAndImage { get; }
        /// <summary>
        ///Indicates whether to rotate the tabs when aligned vertically.
        /// </summary>
        bool RotateTextWhenVertical { get; }

        /// <summary>
        /// Indicates whether to rotate the tabs when aligned left.
        /// </summary>
        bool RotateText180WhenLeftAligned { get; }

        /// <summary>
        /// Returns the left and top padding to use when calculating the tab positions.
        /// </summary>
        Point Padding { get; }

        /// <summary>
        /// Indicates whether hot tracking is enabled.
        /// </summary>
        bool HotTrack { get; }
        /// <summary>
        /// Indicates whether tooltips are enabled.
        /// </summary>
        bool ShowToolTips { get; }
        /// <summary>
        /// Indicates whether users are allowed to move tabs.
        /// </summary>
        bool UserMoveTabs { get; }

        /// <summary>
        /// Returns a collection of <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>.
        /// </summary>
        TabDataCollection TabsData
        {
            get;
        }

        /// <summary>
        /// Indicates whether a tab is selectable.
        /// </summary>
        /// <param name="tabIndex">The tab's index.</param>
        /// <param name="visually">Indicates whether check is for visual selection or programmatic selection.</param>
        /// <returns>True if selectable; false otherwise.</returns>
        bool IsTabSelectable(int tabIndex, bool visually);

        /// <summary>
        /// Creates a new <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>.
        /// </summary>
        ITabData CreateNewTabData();
        /// <summary>
        /// This event is thrown when one of the tab's property is changed.
        /// </summary>
        event TabPanelPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This event is fired before the <see cref="SelectedIndex"/> property is changed.
        /// </summary>
        event SelectedIndexChangingEventHandler SelectedIndexChanging;

        /// <summary>
        /// This will fire the property changed event indicating the change in SelectedIndex.
        /// </summary>
        void OnSelectedIndexChanged(int previousIndex, int curIndex);
    }

    /// <summary>
    /// The extended interface that represents the tab panel's data.
    /// </summary>
    public interface ITabPanelData2 :
        ITabPanelData
    {
        /// <summary>
        /// Indicates whether SuperToolTips are enabled.
        /// </summary>
        bool ShowSuperToolTips
        {
            get;
        }
    }

    /// <summary>
    /// The interface that represents the data associated with a tab.
    /// </summary>
    public interface ITabData : IDisposable
    {
        /// <summary>
        /// Gets / sets the font of the tab.
        /// </summary>
        Font Font
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets the backcolor of the tab.
        /// </summary>
        Color BackColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets the backcolor of the tab to check if it is applied or not.
        /// </summary>
        Color M_Backcolor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets the forecolor of the tab.
        /// </summary>
        Color ForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets / sets the text of the tab.
        /// </summary>
        string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the enabled state of the tab.
        /// </summary>
        bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the visible state of the tab.
        /// </summary>
        bool TabVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets / sets the tooltip of the tab.
        /// </summary>
        string ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        Image Image
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        Size ImageSize
        {
            get;
            set;
        }

        bool ImageChanged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets / sets the image index of the tab.
        /// </summary>
        int ImageIndex
        {
            get;
            set;
        }
        /// <summary>
        /// This event will be thrown when the tab's bounds were affected.
        /// </summary>
        event EventHandler BoundsAffected;
        /// <summary>
        /// This event is thrown when the tabs property is changed.
        /// </summary>
        event EventHandler PropertyChanged;
    }

    /// <summary>
    /// The extended interface that represents the data associated with a tab.
    /// </summary>
    public interface ITabData2 :
        ITabData
    {
        /// <summary>
        /// Gets or sets the super tooltip info.
        /// </summary>
        ToolTipInfo SuperTooltip
        {
            get;
            set;
        }
    }

    #endregion DATA_MODEL_INTERFACES
    #region DATA_MODEL_IMP
    /// <summary>
    /// The default implementation of the <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/> interface.
    /// </summary>
    public class TabPanelData :
        ITabPanelData2
    {
        internal bool isTabRemoving = false;
        /// <summary>
        /// Adjusts y-position of the image.
        /// </summary>
        private int m_imageOffset = 0;
        /// <summary>
        /// Adjusts y-position of the image.
        /// </summary>
        public int ImageOffset
        {
            get
            {
                return m_imageOffset;
            }

            set
            {
                if (m_imageOffset != value)
                {
                    int oldPos = m_imageOffset;
                    m_imageOffset = value;
                    this.OnPropertyChanged("ImageOffset", oldPos, m_imageOffset);
                }
            }
        }

        /// <summary>
        /// Adjusts the gap between the tabControlAdv's top and the tabs.
        /// </summary>
        private int m_adjustTopGap = 0;

        /// <summary>
        /// Adjusts the gap between the tabControlAdv's top and the tabs.
        /// </summary>
        public int AdjustTopGap
        {
            get
            {
                return m_adjustTopGap;
            }
            set
            {
                int oldValue = m_adjustTopGap;
                m_adjustTopGap = value;
                this.OnPropertyChanged("AdjustTopGap", oldValue, m_adjustTopGap);
            }
        }

        /// <summary>
        /// Indicates whether image should be disabled when TabPage is not selected.
        /// </summary>
        private bool m_bHideInactivaPageImage = false;
        /// <summary>
        /// Indicates whether image should be disabled when TabPage is not selected.
        /// </summary>
        public bool DisableInactivePageImage
        {
            get
            {
                return m_bHideInactivaPageImage;
            }

            set
            {
                if (m_bHideInactivaPageImage != value)
                {
                    m_bHideInactivaPageImage = value;
                    this.OnPropertyChanged("DisableInactivePageImage", !m_bHideInactivaPageImage, m_bHideInactivaPageImage);
                }
            }
        }

        /// <summary>
        /// Indicates whether the text and the image should be in the same level.
        /// </summary>
        private bool m_bLevelTextAndImage = false;

        /// <summary>
        /// Indicates whether the text and the image should be in the same level.
        /// </summary>
        public bool LevelTextAndImage
        {
            get
            {
                return m_bLevelTextAndImage;
            }
            set
            {
                if (m_bLevelTextAndImage != value)
                {
                    m_bLevelTextAndImage = value;
                    this.OnPropertyChanged("LevelTextAndImage", !m_bLevelTextAndImage, m_bLevelTextAndImage);
                }
            }
        }

        private string tabStyle = TabRenderer3D.TabStyleName;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TabStyle"/>.
        /// </summary>
        public string TabStyle
        {
            get { return tabStyle; }
            set
            {
                if (tabStyle != value)
                {
                    string oldStyle = tabStyle;
                    tabStyle = value;
                    this.OnPropertyChanged("TabStyle",
                        oldStyle, tabStyle);
                }
            }
        }

        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.BorderStyle"/>.
        /// </summary>
        public BorderStyle BorderStyle
        {
            get { return this.borderStyle; }
            set
            {
                if (this.borderStyle != value)
                {
                    BorderStyle oldStyle = this.borderStyle;
                    this.borderStyle = value;
                    this.OnPropertyChanged("BorderStyle", oldStyle, this.borderStyle);
                }
            }
        }

        private TabAlignment alignment = TabAlignment.Top;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.Alignment"/>.
        /// </summary>
        public TabAlignment Alignment
        {
            get { return alignment; }
            set
            {
                if (alignment != value)
                {
                    TabAlignment oldAlign = alignment;
                    alignment = value;
                    this.OnPropertyChanged("Alignment",
                        oldAlign, alignment);
                }
            }
        }
        private SizeF tabSize = SizeF.Empty;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TabSize"/>.
        /// </summary>
        public SizeF TabSize
        {
            get { return tabSize; }
            set
            {
                if (tabSize != value)
                {
                    SizeF oldSize = tabSize;
                    tabSize = value;
                    this.OnPropertyChanged("TabSize",
                        oldSize, tabSize);
                }
            }
        }
        private int tabGap = 0;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TabGap"/>.
        /// </summary>
        public int TabGap
        {
            get { return tabGap; }
            set
            {
                if (tabGap != value)
                {
                    int oldTabGap = tabGap;
                    tabGap = value;
                    this.OnPropertyChanged("TabGap",
                        oldTabGap, tabGap);
                }
            }
        }

        private TabVerticalAlignment m_tvaVerticalAlignment = TabVerticalAlignment.Default;

        public TabVerticalAlignment VerticalAlignment
        {
            get
            {
                return m_tvaVerticalAlignment;
            }
            set
            {
                TabVerticalAlignment tvaNewAlignment = value;
                TabVerticalAlignment tvaOldAlignment = m_tvaVerticalAlignment;
                if (tvaOldAlignment != tvaNewAlignment)
                {
                    m_tvaVerticalAlignment = tvaNewAlignment;
                    this.OnPropertyChanged("VerticalAlignment", tvaOldAlignment, tvaNewAlignment);
                }
            }
        }

        private bool multiline = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.Multiline"/>.
        /// </summary>
        public bool Multiline
        {
            get { return multiline; }
            set
            {
                if (multiline != value)
                {
                    bool oldMultiline = multiline;
                    multiline = value;
                    this.OnPropertyChanged("Multiline",
                        oldMultiline, multiline);
                }
            }
        }

        private bool keepSelectedTabInFrontRow = true;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.KeepSelectedTabInFrontRow"/>.
        /// </summary>
        public bool KeepSelectedTabInFrontRow
        {
            get
            {
                return this.keepSelectedTabInFrontRow;
            }
            set
            {
                if (this.keepSelectedTabInFrontRow != value)
                {
                    bool oldVal = keepSelectedTabInFrontRow;
                    keepSelectedTabInFrontRow = value;
                    this.OnPropertyChanged("KeepSelectedTabInFrontRow",
                        oldVal, keepSelectedTabInFrontRow);
                }
            }
        }

        private Color fixedSingleBorderColor = Color.Empty;
        /// <summary>
        /// Gets / sets the border color when in <see cref="System.Windows.Forms.BorderStyle.FixedSingle"/> mode.
        /// </summary>
        public Color FixedSingleBorderColor
        {
            get { return this.fixedSingleBorderColor; }
            set
            {
                if (this.fixedSingleBorderColor != value)
                {
                    Color oldVal = fixedSingleBorderColor;
                    fixedSingleBorderColor = value;
                    this.OnPropertyChanged("FixedSingleBorderColor",
                        oldVal, fixedSingleBorderColor);
                }
            }
        }
        private ImageList imageList;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.ImageList"/>.
        /// </summary>
        public virtual ImageList ImageList
        {
            get { return imageList; }
            set
            {
                if (imageList != value)
                {
                    ImageList oldImageList = imageList;
                    imageList = value;
                    this.OnPropertyChanged("ImageList",
                        oldImageList, imageList);
                    if (oldImageList != null)
                        oldImageList.RecreateHandle -= new EventHandler(this.ImageList_Recreated);

                    if (imageList != null)
                        imageList.RecreateHandle += new EventHandler(this.ImageList_Recreated);
                }
            }
        }
        private void ImageList_Recreated(object sender, EventArgs e)
        {
            this.OnPropertyChanged("ImageList",
                ImageList, ImageList);
        }
        private int selectedIndex = -1;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.SelectedIndex"/>.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                // Adjust the selected index to reflect current settings
                if (selectedIndex >= this.tabCollection.Count)
                    ChangeSelectedIndex(this.tabCollection.Count - 1);

                if ((selectedIndex < 0 || selectedIndex >= this.tabCollection.Count)
                    && this.tabCollection.Count > 0)
                    ChangeSelectedIndex(0);

                return selectedIndex;
            }
            set
            {
                if (this.selectedIndex != value)
                    ChangeSelectedIndex(value);
            }
        }
        /// <summary>
        /// Changes the selected tab index.
        /// </summary>
        internal virtual void ChangeSelectedIndex(int index)
        {
            if (this.tabCollection.Count <= index)
                throw new ArgumentOutOfRangeException();

            int nTabsCount = this.tabCollection.Count;

            // This version doesn't support setting a -1 selected index.
            if ((index == -1 && nTabsCount > 0) || (index != -1 && !this.IsTabSelectable(index, false)))
                return;

            // Ignore the first change in selected index.
            if (this.selectedIndex != -1 && !isTabRemoving)
            {
                SelectedIndexChangingEventArgs args = new SelectedIndexChangingEventArgs(index);
                this.OnSelectedIndexChanging(args);
                if (args.Cancel)
                    return;
            }

            int previousIndex = selectedIndex;
            this.selectedIndex = index;

            // Ignore the first change in selected index.
            if (previousIndex != -1 || nTabsCount != 1)
            {
                ((ITabPanelData)this).OnSelectedIndexChanged(previousIndex, selectedIndex);
            }
        }
        /// <summary>
        /// Indicates whether the tab is selectable, visually or programmatically.
        /// </summary>
        /// <param name="tabIndex">The index of the tab page.</param>
        /// <param name="visually">Indicates whether check is for visual selection or programatic selection.</param>
        /// <returns>True if the tab page can be selected; false otherwise.</returns>
        public virtual bool IsTabSelectable(int tabIndex, bool visually)
        {
            if (tabIndex >= this.tabCollection.Count)
                return false;

            // In design mode, make it selectable
            if (parent is ITabControl && ((ITabControl)parent).IsDesignMode())
                return true;

            ITabData tabData = this.tabCollection[tabIndex] as ITabData;

            if (visually)
            {
                // If disabled or invisible prevent selection.
                if (!tabData.Enabled || !tabData.TabVisible)
                    return false;
            }
            return true;
        }

        void ITabPanelData.OnSelectedIndexChanged(int previousIndex, int curIndex)
        {
            this.OnPropertyChanged("SelectedIndex",
                previousIndex, curIndex);
        }
        private TabSizeMode sizeMode = TabSizeMode.Normal;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.SizeMode"/>.
        /// </summary>
        public TabSizeMode SizeMode
        {
            get { return sizeMode; }
            set
            {
                if (sizeMode != value)
                {
                    TabSizeMode oldMode = sizeMode;
                    sizeMode = value;
                    this.OnPropertyChanged("SizeMode",
                        oldMode, sizeMode);
                }
            }
        }
        private Color backColor = Color.Empty;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.BackColor"/>.
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }//return parent.BackColor;}
            set
            {
                if (value != backColor/*parent.BackColor*/)
                {
                    Color oldBackColor = backColor;
                    backColor = value;
                    this.OnPropertyChanged("BackColor",
                        oldBackColor, value);
                }
            }
        }
        private Font font = null;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.Font"/>.
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                if (value != font)
                {
                    Font oldFont = font;
                    font = value;
                    this.OnPropertyChanged("Font",
                        oldFont, value);
                }
            }
        }

        private StringAlignment textAlignmentH = StringAlignment.Center;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TextAlignment"/>.
        /// </summary>
        public StringAlignment TextAlignment
        {
            get { return textAlignmentH; }
            set
            {
                if (textAlignmentH != value)
                {
                    StringAlignment oldTextAlignH = textAlignmentH;
                    textAlignmentH = value;
                    this.OnPropertyChanged("TextAlignment",
                        oldTextAlignH, textAlignmentH);
                }
            }
        }
        private StringAlignment textAlignmentV = StringAlignment.Center;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TextLineAlignment"/>.
        /// </summary>
        public StringAlignment TextLineAlignment
        {
            get { return textAlignmentV; }
            set
            {
                if (textAlignmentV != value)
                {
                    StringAlignment oldVertAlign = textAlignmentV;
                    textAlignmentV = value;
                    this.OnPropertyChanged("TextLineAlignment",
                        oldVertAlign, textAlignmentV);
                }
            }
        }
        private RelativeImageAlignment imageAlignmentR = RelativeImageAlignment.LeftOfText;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.ImageAlignmentR"/>.
        /// </summary>
        public RelativeImageAlignment ImageAlignmentR
        {
            get { return imageAlignmentR; }
            set
            {
                if (imageAlignmentR != value)
                {
                    RelativeImageAlignment oldRIA = imageAlignmentR;
                    imageAlignmentR = value;
                    this.OnPropertyChanged("ImageAlignmentR",
                        oldRIA, imageAlignmentR);
                }
            }
        }

        private bool rotateWhenVertical = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.RotateTextWhenVertical"/>.
        /// </summary>
        public bool RotateTextWhenVertical
        {
            get { return this.rotateWhenVertical; }
            set
            {
                if (this.rotateWhenVertical != value)
                {
                    this.rotateWhenVertical = value;
                    this.OnPropertyChanged("RotateTextWhenVertical",
                        !this.rotateWhenVertical, this.rotateWhenVertical);
                }
            }
        }

        private bool rotateText180WhenLeftAligned = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.RotateText180WhenLeftAligned"/>.
        /// </summary>
        public bool RotateText180WhenLeftAligned
        {
            get { return this.rotateText180WhenLeftAligned; }
            set
            {
                if (this.rotateText180WhenLeftAligned != value)
                {
                    this.rotateText180WhenLeftAligned = value;
                    this.OnPropertyChanged("RotateText180WhenLeftAligned",
                        !this.rotateText180WhenLeftAligned, this.rotateText180WhenLeftAligned);
                }
            }
        }

        private Point padding = TabControlAdv.DEFAULT_PADDING;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.Padding"/>.
        /// </summary>
        public Point Padding
        {
            get { return padding; }
            set
            {
                if (padding != value)
                {
                    Point oldPad = padding;
                    padding = value;
                    this.OnPropertyChanged("Padding",
                        oldPad, padding);
                }
            }
        }

        private bool hotTrack = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.HotTrack"/>.
        /// </summary>
        public bool HotTrack
        {
            get { return hotTrack; }
            set
            { hotTrack = value; }
        }
        private bool showToolTips = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.ShowToolTips"/>.
        /// </summary>
        public bool ShowToolTips
        {
            get { return showToolTips; }
            set { showToolTips = value; }
        }
        private bool dragAndDrop = false;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.UserMoveTabs"/>.
        /// </summary>
        public bool UserMoveTabs
        {
            get { return dragAndDrop; }
            set { dragAndDrop = value; }
        }

        private TabDataCollection tabCollection;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.TabsData"/>.
        /// </summary>
        public TabDataCollection TabsData
        {
            get
            {
                return this.tabCollection;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.CreateNewTabData"/>.
        /// </summary>
        public virtual ITabData CreateNewTabData()
        {
            return new TabData();
        }

        private Control parent;
        /// <summary>
        /// Create a new instance of the TabPanelData class.
        /// </summary>
        /// <param name="parent">The Control parent.</param>
        public TabPanelData(Control parent)
        {
            tabCollection = new TabDataCollection(this);
            this.parent = parent;
        }
        private Font activeTabFont;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.ActiveTabFont"/>.
        /// </summary>
        public Font ActiveTabFont
        {
            get { return activeTabFont; }
            set
            {
                if (activeTabFont != value)
                {
                    Font oldFont = activeTabFont;
                    activeTabFont = value;
                    this.OnPropertyChanged("ActiveTabFont",
                        oldFont, activeTabFont);
                }
            }
        }
        private Color activeTabColor;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.ActiveTabColor"/>.
        /// </summary>
        public Color ActiveTabColor
        {
            get { return activeTabColor; }
            set
            {
                if (activeTabColor != value)
                {
                    Color oldActiveTabColor = activeTabColor;
                    activeTabColor = value;
                    this.OnPropertyChanged("ActiveTabColor",
                        oldActiveTabColor, activeTabColor);
                }
            }
        }
        private Color inactiveTabColor;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData.InactiveTabColor"/>.
        /// </summary>
        public Color InactiveTabColor
        {
            get { return inactiveTabColor; }
            set
            {
                if (inactiveTabColor != value)
                {
                    Color oldInactiveTabColor = inactiveTabColor;
                    inactiveTabColor = value;
                    this.OnPropertyChanged("InactiveTabColor",
                        oldInactiveTabColor, inactiveTabColor);
                }
            }
        }

        /// <summary>
        /// Fired when one of the <see cref="TabPanelData"/>'s properties is changed.
        /// </summary>
        public event TabPanelPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fired before the <see cref="SelectedIndex"/> property is changed.
        /// </summary>
        public event SelectedIndexChangingEventHandler SelectedIndexChanging;

        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="property">The property name that changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected void OnPropertyChanged(string property, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new TabPanelPropertyChangedEventArgs(property, oldValue, newValue));
        }

        /// <summary>
        /// Fires the <see cref="SelectedIndexChanging"/> event.
        /// </summary>
        /// <param name="args">A <see cref="SelectedIndexChangingEventArgs"/> instance with information regarding this event.</param>
        protected void OnSelectedIndexChanging(SelectedIndexChangingEventArgs args)
        {
            if (SelectedIndexChanging != null)
                SelectedIndexChanging(this, args);
        }

        #region ITabPanelData2 implementation

        private bool m_bShowSuperToolTips;

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData2.ShowSuperToolTips"/>.
        /// </summary>
        public bool ShowSuperToolTips
        {
            get
            {
                return m_bShowSuperToolTips;
            }
            set
            {
                m_bShowSuperToolTips = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> objects.
    /// </summary>
    public class TabDataCollection : ArrayList
    {
        private ITabPanelData owner;

        /// <summary>
        /// Creats a new instance of the TabDataCollection class.
        /// </summary>
        /// <param name="owner">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/> that contains this collection.</param>
        public TabDataCollection(ITabPanelData owner)
        {
            this.owner = owner;
        }

        // Properties
        /// <summary>
        /// Gets / sets the indexer for this collection.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> object.</value>
        public override object this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                if (value is ITabData)
                {
                    if (base[index] != value)
                    {
                        base[index] = value;
                        OnCollectionAffected();
                        this.owner.SelectedIndex = this.owner.SelectedIndex;
                    }
                }
                else
                {
                    throw new System.ArgumentException("Only objects of type ITabData can be added to this list", "value");
                }
                // inform owner
            }
        }

        // Methods

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.RemoveAt"/>.
        /// </summary>
        public override void RemoveAt(int index)
        {
            TabPanelData tab = this.owner as TabPanelData;
            OnCollectionChanging();
            if (tab != null)
                tab.isTabRemoving = true;
            base.RemoveAt(index);
            OnCollectionAffected();
            if (tab != null)
                tab.isTabRemoving = false;

        } // end of method RemoveAt


        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Clear"/>.
        /// </summary>
        public override void Clear()
        {
            OnCollectionChanging();
            base.Clear();
            OnCollectionAffected();
        } // end of method Clear

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Remove"/>.
        /// </summary>
        public override void Remove(object value)
        {
            if (value is ITabData)
            {
                OnCollectionChanging();
                int beforeRemoveCount = this.Count;
                base.Remove(value);
                if (beforeRemoveCount != this.Count)
                    OnCollectionAffected();
            }
            else
            {
                throw new System.ArgumentException("Only objects of type ITabData can be removed from the list", "value");
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.RemoveRange"/>.
        /// </summary>
        public override void RemoveRange(int index, int count)
        {
            OnCollectionChanging();
            base.RemoveRange(index, count);
            OnCollectionAffected();
        }

        /// <summary>
        /// Moves items from one location to another.
        /// </summary>
        /// <param name="from">The starting index.</param>
        /// <param name="to">The target index.</param>
        /// <param name="count">The number of items to move.</param>
        public virtual void Move(int from, int to, int count)
        {
            if (from == to || from < 0 || to < 0 ||
                to + count > this.Count || from + count > this.Count)
                return;

            //ITabData[] tabDatas = new ITabData[count];
            ITabData[] tabDataFrom = new ITabData[count];
            ITabData[] tabDataTo = new ITabData[count];

            // Cache the tabs before moving them.
            for (int i = 0; i < count; i++)
            {
              //  tabDatas[i] = (ITabData)this[i + from];
                tabDataFrom[i] = (ITabData)this[i + from];
            }
            // Cache the tabs before moving them.
            for (int i = 0; i < count; i++)
            {             
                tabDataTo[i] = (ITabData)this[i + to];
            }

            OnCollectionChanging();
            base.RemoveRange(to, count);
            base.InsertRange(to, tabDataFrom);
            base.RemoveRange(from, count);
            base.InsertRange(from,tabDataTo);
            OnCollectionAffected();
        }

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.InsertRange"/>.
        /// </summary>
        public override void InsertRange(int index, ICollection c)
        {
            //IEnumerator enumerator = c.GetEnumerator();

            foreach (object collectionElement in c)
            {
                if (!(collectionElement is ITabData))
                    throw new System.ArgumentException("Only objects of type ITabData can be inserted in the list", "ICollection");
            }
            OnCollectionChanging();
            base.InsertRange(index, c);
            OnCollectionAffected();
        }

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Insert"/>.
        /// </summary>
        public override void Insert(int index, object value)
        {
            if (value is ITabData)
            {
                OnCollectionChanging();
                base.Insert(index, value);
                OnCollectionAffected();
            }
            else
            {
                throw new System.ArgumentException("Only objects of type ITabData can be inserted in the list", "value");
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Add"/>.
        /// </summary>
        public override int Add(object value)
        {
            if (value is ITabData)
            {
                OnCollectionChanging();
                int pos = base.Add(value);
                OnCollectionAffected();
                return pos;
            }
            else
            {
                throw new System.ArgumentException("Only objects of type ITabData can be added to the list", "value");
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Reverse(int, int)"/>.
        /// </summary>
        public override void Reverse(int index, int count)
        {
            OnCollectionChanging();
            base.Reverse(index, count);
            OnCollectionAffected();
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.SetRange"/>.
        /// </summary>
        public override void SetRange(int index, ICollection c)
        {
            OnCollectionChanging();
            base.SetRange(index, c);
            OnCollectionAffected();
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Sort(int, int, IComparer)"/>.
        /// </summary>
        public override void Sort(int index, int counter, IComparer comparer)
        {
            OnCollectionChanging();
            base.Sort(index, counter, comparer);
            OnCollectionAffected();
        }
        /// <summary>
        /// Fired when this collection is affected.
        /// </summary>
        public event EventHandler CollectionAffected;
        private object prevSelectedObject;
        private int prevSelectedIndex;
        /// <summary>
        /// Called when the collection is about to change.
        /// </summary>
        protected virtual void OnCollectionChanging()
        {
            this.prevSelectedIndex = this.owner.SelectedIndex;
            this.prevSelectedObject = null;
            if (this.prevSelectedIndex != -1)
                this.prevSelectedObject = this[this.prevSelectedIndex];
        }
        /// <summary>
        /// Calls the CollectionAffected event.
        /// </summary>
        protected virtual void OnCollectionAffected()
        {
            if (CollectionAffected != null)
            {
                CollectionAffected(this, EventArgs.Empty);
            }

            // Changing the SelectedIndex if necessary
            if (this.prevSelectedObject != null)
            {
                int newIndex = -1;
                newIndex = this.IndexOf(prevSelectedObject);
                if (newIndex != -1)
                {
                    this.owner.SelectedIndex = newIndex;
                }
                else
                {
                    // A get_SelectedIndex might throw a selected index changed event
                    // hence calling OnSelectedIndexChanged only if the selected index didn't change
                    if (this.owner.SelectedIndex == this.prevSelectedIndex)
                        this.owner.OnSelectedIndexChanged(this.prevSelectedIndex, this.owner.SelectedIndex);

                    this.prevSelectedIndex = -1;
                    this.prevSelectedObject = null;
                }
            }
        }
    }

    /// <summary>
    /// The default <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> implementation.
    /// </summary>
    public class TabData :
        ITabData2
    {
        private Image image;
        private Size imageSize = new Size(16, 16);

        /// <summary>
        /// Creates a new instance of the TabData.
        /// </summary>
        public TabData()
        {
        }

        public TabData(TabData tabData)
        {
            CopyFrom(tabData);
        }

        protected void CopyFrom(TabData tabData)
        {
            if (null != tabData)
            {
                this.Font = tabData.Font;
                this.BackColor = tabData.BackColor;
                this.ForeColor = tabData.ForeColor;
                this.Text = tabData.Text;
                this.Enabled = tabData.Enabled;
                this.TabVisible = tabData.TabVisible;
                this.ToolTip = tabData.ToolTip;
                this.ImageIndex = tabData.ImageIndex;
            }
        }

        ~TabData()
        {
            this.Dispose(false);
        }
        /// <summary>
        ///Disposes this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// <param name="disposing">True if called by Dispose; false if called by the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
        private Font font;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.Font"/>.
        /// </summary>
        public virtual Font Font
        {
            get
            {
                return font;
            }
            set
            {
                if (font != value)
                {
                    font = value;
                    OnBoundsAffected();
                }
            }
        }
        private Color backColor = Color.Empty;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.BackColor"/>.
        /// </summary>
        public virtual Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    OnPropertyChanged();
                }
            }
        }
        private Color m_backcolor = Color.Empty;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.M_backcolor"/>.
        /// </summary>
        public virtual Color M_Backcolor
        {
            get
            {
                return m_backcolor;
            }
            set
            {
                if (m_backcolor != value)
                {
                    m_backcolor = value;
                    OnPropertyChanged();
                }
            }
        }
        private string text = "";
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.Text"/>.
        /// </summary>
        public virtual string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnBoundsAffected();
                }
            }
        }
        internal void SetTextWithoutBoundsAffected(string value)
        {
            this.text = value;
        }
        private bool enabled = true;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.Enabled"/>.
        /// </summary>
        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnBoundsAffected();
                }
            }
        }
        private bool tabVisible = true;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.TabVisible"/>.
        /// </summary>
        public virtual bool TabVisible
        {
            get { return this.tabVisible; }
            set
            {
                if (this.tabVisible != value)
                {
                    this.tabVisible = value;
                    OnBoundsAffected();
                }
            }
        }
        private string tooltip = "";
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.ToolTip"/>.
        /// </summary>
        public virtual string ToolTip
        {
            get { return tooltip; }
            set
            {
                tooltip = value;
            }
        }

        private Color foreColor = Color.Empty;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.ToolTip"/>.
        /// </summary>
        public virtual Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _imageIndex = -1;
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData.ImageIndex"/>.
        /// </summary>
        public virtual int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                if (_imageIndex != value)
                {
                    _imageIndex = value;
                    OnBoundsAffected();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public virtual Image Image
        {
            get { return image; }
            set
            {
                image = value;
                OnBoundsAffected();
            }
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        public Size ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                OnBoundsAffected();
            }
        }

        private bool imageChanged = false;

        public bool ImageChanged
        {
            get
            {
                return imageChanged;
            }
            set
            {
                imageChanged = value;
            }
        }

        internal void SetImageIndexWithoutBoundsAffected(int i)
        {
            this._imageIndex = i;
        }
        /// <summary>
        /// This method throws the <see cref="Syncfusion.Windows.Forms.Tools.TabData.BoundsAffected"/> event.
        /// </summary>
        protected virtual void OnBoundsAffected()
        {
            if (BoundsAffected != null)
                BoundsAffected(this, EventArgs.Empty);
        }
        /// <summary>
        /// This method throws the <see cref="Syncfusion.Windows.Forms.Tools.TabData.PropertyChanged"/> event.
        /// </summary>
        protected virtual void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, EventArgs.Empty);
        }
        /// <summary>
        /// Fired when the tab data's bounds are affected.
        /// </summary>
        public event EventHandler BoundsAffected;
        /// <summary>
        /// Fired when the tab data's property is changed.
        /// </summary>
        public event EventHandler PropertyChanged;

        #region ITabData2 implementation

        ToolTipInfo m_superToolTip;

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabData2.SuperTooltip"/>.
        /// </summary>
        virtual public ToolTipInfo SuperTooltip
        {
            get
            {
                return m_superToolTip;
            }
            set
            {
                m_superToolTip = value;
            }
        }

        #endregion
    }
    #endregion DATA_MODEL_IMP
    #region VIEW_INTERFACES
    /// <summary>
    /// The interface that represents the parent tab control.
    /// </summary>
    /// <remarks>
    /// Implement this interface if you need to create a custom tab control that uses
    /// the <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> and <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> framework.
    /// </remarks>
    public interface ITabControl
    {
        /// <summary>
        /// Bounds of the tabControl.
        /// </summary>
        Rectangle ClientRectangle { get; }
        /// <summary>
        /// Called when the tab panel's bounds are affected.
        /// </summary>
        void OnTabPanelBoundsAffected();
        /// <summary>
        /// Called to force a repaint in the specified rectangle.
        /// </summary>
        /// <param name="affectedRect">The rectangle to repaint.</param>
        void OnRepaint(RectangleF affectedRect);
        /// <summary>
        /// Returns the <see cref="System.Drawing.Graphics"/> object used by this Control for painting.
        /// </summary>
        Graphics GetGraphics();
        /// <summary>
        /// Returns the <see cref="System.Windows.Forms.Control"/> instance.
        /// </summary>
        Control GetControl();
        /// <summary>
        /// Called to let custom drawing of the tabs.
        /// </summary>
        /// <param name="eventArgs">The <see cref="Syncfusion.Windows.Forms.Tools.DrawTabEventArgs"/> object containing some information regarding this call.</param>
        /// <returns>True if custom drawing was performed; false otherwise.</returns>
        bool OnDrawItem(DrawTabEventArgs eventArgs);
        /// <summary>
        /// Called to notify a change in scroll position.
        /// </summary>
        void OnScrollPositionChanged();
        /// <summary>
        /// Queries if this is design mode.
        /// </summary>
        /// <returns>True if in design mode; false otherwise.</returns>
        bool IsDesignMode();
        /// <summary>
        /// Returns the tab panel renderer used to draw the tab panel.
        /// </summary>
        ITabPanelRenderer Renderer { get; }
        /// <summary>
        /// Returns the drawing utility object that helps draw themed tabs.
        /// </summary>
        ThemedTabDrawing ThemedDrawing { get; }
        /// <summary>
        /// Indicates whether XP Themes should be used if available for drawing.
        /// </summary>
        bool ThemesEnabled { get; }
        /// <summary>
        /// Indicates whether to validate the current active tab page.
        /// </summary>
        /// <returns></returns>
        bool ValidateFocusedTab();
        /// <summary>
        /// Gets or sets a value indicating whether the control interprets an ampersand character (&) to be an access key prefix character.
        /// </summary>
        bool UseMnemonic { get; }
        /// <summary>
        /// Indicates multiline text.
        /// </summary>
        bool MultilineText { get; }
        /// <summary>
        /// Office2007 color scheme.
        /// </summary>
        Office2007Theme Office2007ColorScheme { get; }
        /// <summary>
        /// Office2010 color scheme.
        /// </summary>
        Office2010Theme Office2010ColorTheme { get; }
        /// <summary>
        /// Tabs border visibility.
        /// </summary>
        bool BorderVisible { get; }
        /// <summary>
        /// Tabs border width.
        /// </summary>
        int BorderWidth { get; }
    }


    /// <summary>
    /// The interface you should implement to create a custom tab panel renderer.
    /// </summary>
    /// <remarks>
    /// <para>The base interface that the <see cref="Syncfusion.Windows.Forms.Tools.SingleLineTabPanelRenderer"/>
    /// and the <see cref="Syncfusion.Windows.Forms.Tools.MultilineTabPanelRenderer"/> derive from.</para>
    /// </remarks>
    public interface ITabPanelRenderer
    {
        /// <summary>
        /// If to draw image anyway.
        /// </summary>
        bool ForceDrawImage { get; set; }
        /// <summary>
        /// Indicates whether the parent tab control should forward mouse move messages to the renderer.
        /// </summary>
        bool NeedMouseMove { get; }
        /// <summary>
        /// Indicates whether the tabs need to be laid out due to change in some property.
        /// </summary>
        bool NeedLayout { get; }
        /// <summary>
        /// Lays out the tabs according to the current bounds.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="fromPaint">Indicates whether this method was called from the Paint event.</param>
        void Layout(Graphics g, bool fromPaint);
        /// <summary>
        /// Gets / sets the <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/>
        /// that contains information regarding tab panel.
        /// </summary>
        ITabPanelData TabPanelData { get; set; }
        ArrayList Renderers { get; }
        /// <summary>
        /// Gets / sets the current bounds of the tab panel.
        /// </summary>
        RectangleF Bounds { get; set; }
        #region SCROLLING_SUPPORT
        /// <summary>
        /// Indicates whether scrolling is supported.
        /// </summary>
        bool ScrollingSupported { get; }
        /// <summary>
        /// Indicates whether scrolling to the left is allowed for the current layout.
        /// </summary>
        bool CanScrollLeft { get; }
        /// <summary>
        /// Indicates whether scrolling to the right is allowed for the current layout.
        /// </summary>
        bool CanScrollRight { get; }

        bool IsMirrored { get; }
        /// <summary>
        /// Scrolls the tabs based on the specified <see cref="Syncfusion.Windows.Forms.Tools.ScrollIncrement"/>
        /// and <see cref="Syncfusion.Windows.Forms.Tools.ScrollDirection"/>
        /// </summary>
        void Scroll(ScrollIncrement increment, ScrollDirection direction);
        #endregion SCROLLING_SUPPORT
        /// <summary>
        /// Returns the preferred size of the tab panel.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context to help calculate the sizes.</param>
        /// <param name="size">The <see cref="System.Drawing.SizeF"/> that should contain the preferred size when returned.</param>
        /// <remarks>
        /// <para>A non-zero width in the size parameter indicates the available width
        /// and requires you to provide the preferred height for that width. Zero width
        /// in the size parameter indicates you to provide the preferred width and height
        /// for the tab panel, assuming infinite available width.</para>
        /// </remarks>
        /// <summary>
        /// Returns the preferred size, if any, of the tabs. 
        /// </summary>
        void GetPreferredSize(Graphics g, ref SizeF size);
        /// <summary>
        /// Returns the bounds of the specified tab.
        /// </summary>
        Rectangle GetTabBounds(int i);
        /// <summary>
        /// Indicates whether the background color is solid.
        /// </summary>
        /// <returns>True if solid; false otherwise.</returns>
        bool IsBackgroundSolid();
        /// <summary>
        /// Returns the tab panel's backcolor.
        /// </summary>
        Color TabPanelBackColor { get; }
        /// <summary>
        /// Paints the tab panel background.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="backColor">The background <see cref="System.Drawing.Color"/>.</param>
        /// <param name="rect">The background bounds.</param>
        void OnPaintPanelBackground(Graphics g, Color backColor, Rectangle rect);
        /// <summary>
        /// Paints the tab panel with the tabs.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="clipRect">The clipping rectangle to use while drawing.</param>
        void OnPaint(Graphics g, Rectangle clipRect);
        /// <summary>
        /// Returns the tab position under the specified co-ords.
        /// </summary>
        /// <param name="mousePosition">The mouse position in client or transformed drawing co-ords.</param>
        /// <param name="inTransformedCoOrds">Indicates whether the mouse position is in transformed drawing co-ordinates or client co-ordinates.</param>
        /// <returns>The hit tab's index; -1 if none found.</returns>
        int HitTestTabs(PointF mousePosition, bool inTransformedCoOrds);
        /// <summary>
        /// Returns the mouse position.
        /// </summary>
        Point GetMousePosition();
        /// <summary>
        /// Called by the parent tab control to forward mouse move messages.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> that has some information regarding this event.</param>
        void OnMouseMove(MouseEventArgs e);
        /// <summary>
        /// Called by the parent tab control to forward mouse leave messages.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> that has some information regarding this event.</param>
        void OnMouseLeave(EventArgs e);
        /// <summary>
        /// Called by the parent tab control to forward mouse down messages.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> that has some information regarding this event.</param>
        void OnMouseDown(MouseEventArgs e);
        /// <summary>
        /// Called by the parent tab control to forward mouse up messages.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> that has some information regarding this event.</param>
        void OnMouseUp(MouseEventArgs e);
        /// <summary>
        /// Called by the parent tab control to forward got focus messages.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        void OnGotFocus(EventArgs e);
        /// <summary>
        /// Called by the parent tab control to forward lost focus messages.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        void OnLostFocus(EventArgs e);
        /// <summary>
        /// Indicates whether a tab is currently being moved.
        /// </summary>
        /// <returns>True if moving; false otherwise.</returns>
        bool IsMovingTab();
        /// <summary>
        /// Cancels current tab dragging, if any.
        /// </summary>
        void CancelTabDrag();
        /// <summary>
        /// Ensures that the current scroll position is valid.
        /// </summary>
        /// <param name="makeSelectedTabVisible">Indicates whether to make the selected tab visible.</param>
        /// <param name="invalidate">Indicates whether to redraw the invalid regions.</param>
        void ValidateScrollOffset(bool makeSelectedTabVisible, bool invalidate);
        /// <summary>
        /// Transforms the RectangleF in client co-ordinates to rotated drawing co-ordinates or vice-versa.
        /// </summary>
        /// <param name="rect">The RectangleF to transform.</param>
        /// <param name="apply">Indicates whether to transform to drawing co-ordinates or to transform to client co-ordinates.</param>
        /// <returns>
        /// The transformed <see cref="System.Drawing.RectangleF"/>.
        /// </returns>
        RectangleF ApplyDrawingTransform(RectangleF rect, bool apply);
    }


    /// <summary>
    /// Implement this interface to provide certain default properties for the tab panel.
    /// </summary>
    /// <remarks>
    /// <para>You should implement this interface when you create custom tabs along with the
    /// <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation.
    /// A default implementation is available using <see cref="Syncfusion.Windows.Forms.Tools.TabUIDefaultProperties"/>.
    /// </para>
    /// </remarks>
    public interface ITabPanelDefaultProperties
    {
        /// <summary>
        /// Returns the default active tab's color.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color"/>.</returns>
        Color DefaultActiveTabColor();
        /// <summary>
        /// Returns the default inactive tab's color.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color"/>.</returns>
        Color DefaultInactiveTabColor();
        /// <summary>
        /// Returns the tab's fore color.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color"/>.</returns>
        Color DefaultTabForeColor();
        /// <summary>
        /// Returns the default tab panel background color.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color"/>.</returns>
        Color DefaultTabPanelBackgroundColor();
        /// <summary>
        /// Returns the default tab panel font.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Font"/>.</returns>
        Font DefaultTabPanelFont();
        /// <summary>
        /// Returns the default active tab font.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Font"/>.</returns>
        Font DefaultActiveTabFont();
        /// <summary>
        /// Returns the default inactive tab font.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Font"/>.</returns>
        Font DefaultInactiveTabFont();
        /// <summary>
        /// Returns the default single border Color.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color"/>.</returns>
        Color DefaultFixedSingleBorderColor();
    }

    /// <summary>
    /// The interface to implement when you create custom tab styles (tab renderers).
    /// </summary>
    /// <remarks>
    /// <para>Use the default <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase"/> implementation
    /// when you want to create custom tab renderers. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererFactory"/>
    /// for information on how to register your custom renderer with the framework and other requirements.</para>
    /// <para>Note that your implementation <bold>should have a non-default constructor</bold> that takes
    /// the following 2 parameters: (<see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> and <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/>).</para>
    /// </remarks>
    public interface ITabRenderer :
        IDisposable
    {
        /// <summary>
        /// Indicates whether this tab is visible.
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// If to draw image anyway.
        /// </summary>
        bool ForceDrawImage { get; set; }
        /// <summary>
        /// Returns the preferred size for this tab.
        /// </summary>
        SizeF GetPreferredSize(Graphics g);

        /// <summary>
        /// Indicates whether the text is shrunk.
        /// </summary>
        bool IsTextShrunk();
        /// <summary>
        /// Gets / sets the <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>
        /// containing information regarding this tab.
        /// </summary>
        ITabData TabData { get; set; }
        /// <summary>
        /// Gets / sets the current bounds of this tab.
        /// </summary>
        RectangleF Bounds { get; set; }
        /// <summary>
        /// Indicates whether hot tracking is set on in this tab.
        /// </summary>
        bool HotTrack { get; set; }
        /// <summary>
        /// Indicates whether the specified mouse position is within this tab.
        /// </summary>
        /// <param name="mousePosition">The point to verify.</param>
        /// <returns>True if the point is within the tab bounds; false otherwise.</returns>
        bool HitTest(PointF mousePosition);
        /// <summary>
        /// Returns the rectangle representing the dirty portion of the tab.
        /// </summary>
        RectangleF GetRedrawBounds();
        /// <summary>
        /// Returns the current tab bounds. This includes any overlapped region.
        /// </summary>
        RectangleF GetCurrentBounds();
        /// <summary>
        /// Returns the current tab bounds. This includes any overlapped region.
        /// </summary>
        RectangleF GetBoundsForScrolling();
        /// <summary>
        /// Gets / sets the <see cref="System.Windows.Forms.TabAlignment"/>.
        /// </summary>
        TabAlignment TabAlignment { get; set; }
        /// <summary>
        /// Paints the tab.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="clipRect">The clipping rectangle to use while drawing.</param>
        void OnPaint(Graphics g, Rectangle clipRect);
        /// <summary>
        /// Notifies the tab that its properties have changed.
        /// </summary>
        void TabPropertyChanged();
        /// <summary>
        /// Represents the method you should call from your implementation to allow for custom drawing of the tabs.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Tools.DrawItemCallback"/> instance.</value>
        DrawItemCallback DrawItemCallback { get; set; }
        /// <summary>
        /// Indicates the border color of the tabs.
        /// </summary>
        Color TabBorderColor { get; }
    }
    #endregion VIEW_INTERFACES
    #region VIEW_IMP
    /// <summary>
    /// An <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance
    /// that implements the single-line tab mode.
    /// </summary>
    public class SingleLineTabPanelRenderer : TabPanelRenderer
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Syncfusion.Windows.Forms.Tools.SingleLineTabPanelRenderer"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> parent.</param>
        public SingleLineTabPanelRenderer(ITabControl parent)
        {
            this.parent = parent;
        }
        /// <summary>
        /// Offset for tab page.
        /// </summary>
        private const int TAB_PAGE_OFFSET = 10;

        public float m_fScrollOffsetX = 0.0F;
        private float m_fLastKnownPreferredWidth = -1.0F;

        public float fpadX = 0.0F;
        public float fpadY = 0.0F;

        /// <summary>
        /// Gets / sets the padding to use to the left of the tabs while calculating the tab positions.
        /// </summary>
        /// <value>A float value representing the horizontal padding.</value>
        public float PadX
        {
            get { return this.fpadX; }
            set { this.fpadX = value; }
        }

        /// <summary>
        /// Gets / sets the padding to use to the top of the tabs while calculating the tab positions.
        /// </summary>
        /// <value>A float value representing the vertical padding.</value>
        public float PadY
        {
            get { return this.fpadY; }
            set { this.fpadY = value; }
        }

        /// <summary>
        /// Returns the current scroll position.
        /// </summary>
        /// <remarks>A float value representing the scroll offset.</remarks>
        protected float ScrollOffsetX
        {
            get { return m_fScrollOffsetX; }
        }

        protected float LastKnownPreferredWidth
        {
            get
            {
                if (-1.0F == m_fLastKnownPreferredWidth)
                {
                   // Initializes/updates m_fLastKnownPreferredWidth
                    using (Graphics g = parent.GetGraphics())
                    {                       
                        GetPreferredSize(g);
                    }
                }
                return m_fLastKnownPreferredWidth;
            }
            set { m_fLastKnownPreferredWidth = value; }
        }


        /// <summary>
        /// Indicates whether scrolling is supported by this renderer.
        /// </summary>
        public override bool ScrollingSupported
        {
            get { return true; }
        }
        /// <summary>
        /// Indicates whether the tab can scroll left based on the current dimensions.
        /// </summary>
        public override bool CanScrollLeft
        {
            get
            {
                bool bIsMirrored = GetIsMirroredForVerticalAlignment();
                if (bIsMirrored)
                {
                    return (m_fScrollOffsetX + this.tdbounds.Width < LastKnownPreferredWidth);
                }
                else
                {
                    return (m_fScrollOffsetX > 0);
                }
            }
        }
        /// <summary>
        /// Indicates whether the tab can scroll right based on the current dimensions.
        /// </summary>
        public override bool CanScrollRight
        {
            get
            {
                bool bIsMirrored = GetIsMirroredForVerticalAlignment();
                if (bIsMirrored)
                {
                    return (m_fScrollOffsetX > 0);
                }
                else
                {
                    return (m_fScrollOffsetX + this.tdbounds.Width < LastKnownPreferredWidth);
                }
            }
        }

        public bool NeedRotateTextWhenVertical
        {
            get
            {
                return this.TabPanelData != null
                       && (this.TabPanelData.Alignment == TabAlignment.Left || this.TabPanelData.Alignment == TabAlignment.Right)
                       && this.TabPanelData.RotateTextWhenVertical;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabPanelRenderer.TabPanel_PropertyChanged"/>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        protected override void TabPanel_PropertyChanged(ITabPanelData sender, TabPanelPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SizeMode")
            {
                if (this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
                    m_fScrollOffsetX = 0;
            }
            base.TabPanel_PropertyChanged(sender, e);
        }

        /// <summary>
        /// Scrolls by the specified increment in the specified direction.
        /// </summary>
        /// <param name="siIncrement">The scroll increment.</param>
        /// <param name="sdPhysicalDirection">The direction.</param>
        public override void Scroll(ScrollIncrement siIncrement, ScrollDirection sdPhysicalDirection)
        {
            Control ctrlParent = parent.GetControl();
            if (!ctrlParent.IsHandleCreated || !ctrlParent.Visible)
                return;

            ITabPanelData itpdTabData = TabPanelData;
            TabDataCollection tdcTabs = itpdTabData.TabsData;

            int nTabCount = tdcTabs.Count;
            if (0 == nTabCount)
                return;

            float oldScrollOffsetX = m_fScrollOffsetX;

            Graphics g = this.parent.GetGraphics();

            ITabDefaultProperties itdpTabDefaultProperties = TabRendererFactory.GetRegisteredExtender(itpdTabData.TabStyle);
            SizeF overlappedSize = itdpTabDefaultProperties.GetOverlapSize(itpdTabData.TabSize);

            bool bScrollRightMost = false, bScrollLeftMost = false;
            bool bIsMirrored = GetIsMirroredForVerticalAlignment();

            ScrollDirection sdLogicalDirection = sdPhysicalDirection;
            if (bIsMirrored)
            {
                sdLogicalDirection = (ScrollDirection.Left == sdPhysicalDirection ? ScrollDirection.Right : ScrollDirection.Left);
            }

            // Determine special scrolling cases
            if (1 == nTabCount)
            {
                if (ScrollDirection.Left == sdLogicalDirection)
                    bScrollLeftMost = true;
                else
                    bScrollRightMost = true;
            }

            // Get the current tab aligned to the left edge
            float fHitTestX = bIsMirrored ?
                (tdbounds.Right - overlappedSize.Width) :
                (tdbounds.Left + overlappedSize.Width);
            PointF ptHitTest = new PointF(fHitTestX,
                this.tdbounds.Y + ((this.tdbounds.Bottom - this.tdbounds.Y) / 2));
            if (bIsMirrored && this.tabRenderers[0] is TabRendererWorkbookMode)
                ptHitTest.X -= 1;

            int nOutermostTabIdx = this.HitTestTabs(ptHitTest, true);
            if (nOutermostTabIdx == -1)
                nOutermostTabIdx = 0;

            if (!bScrollRightMost && !bScrollLeftMost)
            {
                if (ScrollDirection.Left == sdLogicalDirection && 0 == nOutermostTabIdx)
                    bScrollLeftMost = true;

                if (ScrollDirection.Right == sdLogicalDirection && (nTabCount - 1) == nOutermostTabIdx)
                    bScrollRightMost = true;
            }

            if (!bScrollRightMost && !bScrollLeftMost)
            {
                if (ScrollIncrement.Tab == siIncrement)
                {
                    // Align the next tab to the edge
                    int nNextTabIdx=nOutermostTabIdx;
                    ITabRenderer itrTabRenderer=null;
                    RectangleF nextTabBounds=RectangleF.Empty;
                    do
                    {
                        nNextTabIdx = (ScrollDirection.Left == sdLogicalDirection ? nNextTabIdx - 1 : nNextTabIdx + 1);
                        if (nNextTabIdx >= 0 && nNextTabIdx < tabRenderers.Count)
                        {
                            itrTabRenderer = tabRenderers[nNextTabIdx] as ITabRenderer;
                            nextTabBounds = itrTabRenderer.GetBoundsForScrolling();
                        }
                        else
                        {
                            itrTabRenderer = null;
                        }                        
                    } while (itrTabRenderer!=null&&!itrTabRenderer.TabData.TabVisible&& nextTabBounds ==RectangleF.Empty );

                    float fDiffForLeftAlign = bIsMirrored ?
                        (this.tdbounds.Right - nextTabBounds.Right) :
                        (nextTabBounds.Left - this.tdbounds.Left);

                    m_fScrollOffsetX += fDiffForLeftAlign;
                }
                else if (ScrollIncrement.Page == siIncrement)
                {
                    if (sdLogicalDirection == ScrollDirection.Left)
                    {
                        if (this.tdbounds.Width < m_fScrollOffsetX)
                        {
                            m_fScrollOffsetX -= this.tdbounds.Width;
                        }
                        else
                        {
                            bScrollLeftMost = true;
                        }
                    }
                    else
                    {
                        // Align the current tab intersecting at the right bounds to the left edge.
                        int tabAtRightEdge = this.HitTestTabs(new PointF(bIsMirrored ? this.tdbounds.Left + 1 : this.tdbounds.Right - 1,
                            this.tdbounds.Y + ((this.tdbounds.Bottom - this.tdbounds.Y) / 2)), true);

                        if (tabAtRightEdge == -1)
                            tabAtRightEdge = this.TabPanelData.TabsData.Count - 1;

                        tabAtRightEdge++;

                        if (tabAtRightEdge >= tabRenderers.Count)
                        {
                            tabAtRightEdge = tabRenderers.Count - 1;
                        }

                        RectangleF tabAtRightEdgeBounds = ((ITabRenderer)tabRenderers[tabAtRightEdge]).GetCurrentBounds();
                        float diffForLeftAlign = bIsMirrored ?
                            this.tdbounds.Right - tabAtRightEdgeBounds.Right :
                            tabAtRightEdgeBounds.Left - this.tdbounds.Left;

                        m_fScrollOffsetX += diffForLeftAlign;
                    }
                }
            }

            if (bScrollRightMost)
            {
                m_fScrollOffsetX = LastKnownPreferredWidth - this.tdbounds.Width;
            }
            else if (bScrollLeftMost)
                m_fScrollOffsetX = 0;

            if (m_fScrollOffsetX < 0)
                m_fScrollOffsetX = 0;

            this.ComputeTabPositions(g);
            this.ValidateScrollOffset(false, false);

            g.Dispose();
            g = null;

            if (oldScrollOffsetX != m_fScrollOffsetX)
                this.parent.OnScrollPositionChanged();

            this.parent.OnRepaint(this.Bounds);
        }

        /// <summary>
        /// Validates the current scroll offset, recalculating it, if necessary.
        /// </summary>
        /// <param name="makeSelectedTabVisible">Indicates whether to make the selected tab visible.</param>
        /// <param name="invalidate">Indicates whether to force a repaint if recalculation is necessary.</param>
        public override void ValidateScrollOffset(bool makeSelectedTabVisible, bool invalidate)
        {
            if (!this.parent.GetControl().IsHandleCreated || this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
            {
                if (!this.parent.GetControl().IsHandleCreated)
                    this.SetNeedLayout(true);
                return;
            }

            float oldScrollOffsetX = m_fScrollOffsetX;

            bool bNeedInvalidation = false;
            Graphics g = this.parent.GetGraphics();

            // Check if offset is too much.
            if (m_fScrollOffsetX + this.tdbounds.Width > LastKnownPreferredWidth)
            {
                float diff = m_fScrollOffsetX + this.tdbounds.Width - LastKnownPreferredWidth;
                m_fScrollOffsetX -= diff;
                if (m_fScrollOffsetX < 0)
                    m_fScrollOffsetX = 0;
                ComputeTabPositions(g);
                bNeedInvalidation = true;
            }

            // Make sure selected tab is visible
            if (makeSelectedTabVisible && this.TabPanelData.SelectedIndex != -1)
            {
                ComputeTabPositions(g);

                RectangleF selectedTabBounds = ((ITabRenderer)this.tabRenderers[this.TabPanelData.SelectedIndex]).GetCurrentBounds();
                // If we have to change the scroll offset, do not exactly align the selected tab to the corners,
                // instead align it with some space to the border, so that adjacent tabs could be partialy drawn
                float rightOffset = 0, leftOffset = 0;
                // There is an element to the right
                if (this.tabRenderers.Count > this.TabPanelData.SelectedIndex + 1)
                {
                    rightOffset = ((ITabRenderer)this.tabRenderers[this.TabPanelData.SelectedIndex + 1]).GetCurrentBounds().Width;
                    rightOffset = rightOffset < TAB_PAGE_OFFSET ? rightOffset : TAB_PAGE_OFFSET;
                }
                else if (this.TabPanelData.SelectedIndex == this.tabRenderers.Count - 1)
                {
                    rightOffset = TAB_PAGE_OFFSET;
                }

                // There is an element to the left
                if (this.TabPanelData.SelectedIndex - 1 >= 0)
                {
                    leftOffset = ((ITabRenderer)this.tabRenderers[this.TabPanelData.SelectedIndex - 1]).GetCurrentBounds().Width;
                    leftOffset = leftOffset < TAB_PAGE_OFFSET ? leftOffset : TAB_PAGE_OFFSET;
                }

                bool bIsMirrored = GetIsMirroredForVerticalAlignment();
                if (bIsMirrored)
                {
                    ExchangeFloats(ref rightOffset, ref leftOffset);
                }

                if (!this.tdbounds.Contains(selectedTabBounds))
                {
                    // Selected tab is not visible, either align it to the right end or left end
                    float diffForRightAlign = selectedTabBounds.Right - this.tdbounds.Right;
                    float diffForLeftAlign = this.tdbounds.Left - selectedTabBounds.Left;

                    if (bIsMirrored)
                    {
                        ExchangeFloats(ref diffForRightAlign, ref diffForLeftAlign);
                    }

                    bool bRightAlign = true;
                    if (Math.Abs(diffForLeftAlign) < Math.Abs(diffForRightAlign)
                        || selectedTabBounds.Width >= this.tdbounds.Width)
                        bRightAlign = false;

                    // Adjust the scroll offset accordingly
                    if (bRightAlign)
                        m_fScrollOffsetX += diffForRightAlign + rightOffset;
                    else
                        m_fScrollOffsetX -= diffForLeftAlign + leftOffset;

                    if (m_fScrollOffsetX < 0 || this.TabPanelData.SelectedIndex == 0)
                        m_fScrollOffsetX = 0;

                    ComputeTabPositions(g);
                    bNeedInvalidation = true;
                }
            }

            g.Dispose();

            if (oldScrollOffsetX != m_fScrollOffsetX)
                this.parent.OnScrollPositionChanged();

            if (invalidate && bNeedInvalidation)
                this.parent.OnRepaint(this.Bounds);
        }

        private static void ExchangeFloats(ref float x, ref float y)
        {
            float t = y;
            y = x;
            x = t;
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabPanelRenderer.OnBoundsAffected"/>.
        /// </summary>
        protected override void OnBoundsAffected()
        {
            this.SetNeedLayout(true);
            LastKnownPreferredWidth = -1.0F;
            base.OnBoundsAffected();
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabPanelRenderer.OnSelectionChanged"/>
        /// </summary>
        /// <param name="previousIndex"></param>
        /// <param name="currentIndex"></param>
        protected override void OnSelectionChanged(int previousIndex, int currentIndex)
        {
            base.OnSelectionChanged(previousIndex, currentIndex);
            if (NeedLayout)
                // Let it be processed in the Layout event
                bringSelectedTabToFront = true;
            else
                this.ValidateScrollOffset(true, true);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabPanelRenderer.ComputeTabPositions"/>
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        public override void ComputeTabPositions(Graphics g)
        {
            if (/*tabPositionsKnown ||*/ tabRenderers.Count <= 0)
                return;

            ITabPanelData itpdTabPanelData = this.TabPanelData;
            bool bShrinkToFit = false;

            int hiddenTabs = this.GetHiddenTabsCount();

            ITabDefaultProperties defaultProperties = TabRendererFactory.GetRegisteredExtender(itpdTabPanelData.TabStyle);
            SizeF overlappedSize = defaultProperties.GetOverlapSize(itpdTabPanelData.TabSize);

            if (defaultProperties.GetType() == typeof(OneNoteStyleRendererProperty))
            {
                if (this.parent != null && this.parent.Renderer != null)
                {
                    ITabPanelData tabData = this.parent.Renderer.TabPanelData;
                    if (tabData != null && (tabData.Alignment == TabAlignment.Left ||
                        tabData.Alignment == TabAlignment.Right) && tabData.RotateTextWhenVertical)
                    {
                        overlappedSize = new SizeF(overlappedSize.Height, overlappedSize.Width);
                    }
                }
            }

            bool bIsMirrored = GetIsMirroredForVerticalAlignment();

            // Compute the beginning left and bottom and height
            float fLeftRaw = 0;
            if (bIsMirrored)
            {
                fLeftRaw = tdbounds.Right - (fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX);
            }
            else
            {
                fLeftRaw = tdbounds.X + (fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX);
            }

            float offset = fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX;
            if (itpdTabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
            {
                SizeF sizePreferedBounds = GetPreferredSize(g);
                sizePreferedBounds.Width -= overlappedSize.Width;

                if (tdbounds.Width - offset < sizePreferedBounds.Width)
                {
                    bShrinkToFit = true;
                }
            }

            float fTabOffsetLeft = (float)Math.Round((double)fLeftRaw);
            float fBottom = tdbounds.Bottom - 1;
            float fHeight = 0;//overlappedSize.Height;

            if ((TabPanelData.SizeMode == TabSizeMode.Normal ||
                  TabPanelData.SizeMode == TabSizeMode.FillToRight ||
                  TabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
                && this.TabPanelData.TabSize == Size.Empty)
            {
                fHeight += GetLargestHeight(g);
            }
            else
            {
                if (!this.NeedRotateTextWhenVertical || this.TabPanelData.SizeMode == TabSizeMode.Fixed)
                {
                    fHeight += TabPanelData.TabSize.Height;
                }
                else
                {
                    fHeight += TabPanelData.TabSize.Width;
                }
            }
            //Tracks the availabel width after Reducing the LargerTab that can be added to other immediate smaller tab than the previous one.
            float availabaleWidth = 0;
            // iterate the renderers and update their bounds
            foreach (ITabRenderer renderer in tabRenderers)
            {
                if (this.ShouldDrawVisible(renderer.TabData))
                {
                    // Compute Preferred Size or use fixed size
                    SizeF tabSize = new SizeF(0, 0);
                    if (this.TabPanelData.SizeMode == TabSizeMode.Normal ||
                        this.TabPanelData.SizeMode == TabSizeMode.FillToRight ||
                        this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
                        tabSize = renderer.GetPreferredSize(g);
                    else if (this.TabPanelData.SizeMode == TabSizeMode.Fixed)
                        tabSize = this.TabPanelData.TabSize;

                    float fTabGapWidth = TabPanelData.TabGap;
                    if (bShrinkToFit)
                    {
                        int nActualTabCount = TabPanelData.TabsData.Count - hiddenTabs;
                        float fCleanAvgTabWidth = (this.tdbounds.Width - this.fpadX) / nActualTabCount;

                        float fFraction = fCleanAvgTabWidth / 10.0F;
                        if (fFraction < fTabGapWidth)
                        {
                            fTabGapWidth = fFraction;
                        }
                        float fAvgTabWidth = ((this.tdbounds.Width - offset - this.fpadX - (nActualTabCount - 1) * fTabGapWidth) / nActualTabCount);
                        if (tabSize.Width > (fAvgTabWidth + availabaleWidth))
                        {
                            tabSize.Width = fAvgTabWidth + availabaleWidth;
                            availabaleWidth = 0;
                        }
                        else if (tabSize.Width > fAvgTabWidth)
                            tabSize.Width = fAvgTabWidth;
                        else
                            availabaleWidth += fAvgTabWidth - tabSize.Width;
					
                    }

                    renderer.Visible = true;

                    if (bIsMirrored)
                    {
                        fTabOffsetLeft -= tabSize.Width;
                        // rectBounds.X = tdbounds.Width - rectBounds.Right;
                    }
                    RectangleF rectBounds = new RectangleF(fTabOffsetLeft, fBottom - fHeight + 1, tabSize.Width, fHeight);

                    renderer.Bounds = rectBounds;
                    renderer.TabAlignment = this.TabPanelData.Alignment;

                    if (bIsMirrored)
                    {
                        fTabOffsetLeft -= fTabGapWidth;
                    }
                    else
                    {
                        fTabOffsetLeft += (tabSize.Width + fTabGapWidth);
                    }
                }
                else
                {
                    renderer.Visible = false;
                    renderer.Bounds = RectangleF.Empty;
                }
            }
        }

        /// <summary>
        /// Overloaded. Returns the preferred size.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        /// <returns>The preferred size.</returns>
        protected SizeF GetPreferredSize(Graphics g)
        {
            SizeF returnValue = SizeF.Empty;
            GetPreferredSize(g, ref returnValue);
            return returnValue;
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.GetPreferredSize"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="preferredSize">The preferred size.</param>
        public override void GetPreferredSize(Graphics g, ref SizeF preferredSize)
        {
            preferredSize = new SizeF(0, 0);

            if (this.TabPanelData.Multiline == false
                && tabRenderers.Count > 0)
            {
                // Single line logic...
                // Tab's preferred size
                if (this.TabPanelData.SizeMode == TabSizeMode.Fixed)
                {
                    preferredSize.Width = (this.TabPanelData.TabSize.Width * (this.TabPanelData.TabsData.Count - this.GetHiddenTabsCount()));
                    preferredSize.Height = this.TabPanelData.TabSize.Height;
                }
                else
                {
                    foreach (ITabRenderer tabRenderer in this.tabRenderers)
                    {
                        if (this.ShouldDrawVisible(tabRenderer.TabData))
                        {
                            SizeF preferredTabSize = tabRenderer.GetPreferredSize(g);
                            preferredSize.Width += preferredTabSize.Width;

                            if (preferredSize.Height < preferredTabSize.Height)
                                preferredSize.Height = preferredTabSize.Height;
                        }
                    }
                    // Use the height irrespective of the mode if avaialable.
                    if (this.TabPanelData.TabSize != Size.Empty)
                    {
                        if (!this.NeedRotateTextWhenVertical)
                        {
                            preferredSize.Height = this.TabPanelData.TabSize.Height;
                        }
                        else
                        {
                            preferredSize.Height = this.TabPanelData.TabSize.Width;
                        }
                    }
                }

                // Add to it the overlapped Size and...
                ITabPanelData tpd = this.TabPanelData;
                SizeF overlappedSize = TabRendererFactory.GetRegisteredExtender(tpd.TabStyle).GetOverlapSize(tpd.TabSize);
                overlappedSize.Height += TabPanelData.AdjustTopGap;

                preferredSize.Width += overlappedSize.Width + fpadX;
                preferredSize.Height += overlappedSize.Height + fpadY;
                // ... tab gap
                preferredSize.Width += this.TabPanelData.TabGap * (this.TabPanelData.TabsData.Count - 1);
            }

            if (parent as MDITabPanel != null)
            {
                MDITabPanel tabPanel = parent as MDITabPanel;

                if (tabPanel != null && (tabPanel.Parent is TabHost)
                    && preferredSize == Size.Empty)
                {
                    preferredSize = this.Bounds.Size;
                }
            }

            LastKnownPreferredWidth = preferredSize.Width;
        }
    }

    /// <summary>
    /// Called by <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation to allow custom drawing.
    /// </summary>
    /// <param name="eventArgs">A <see cref="Syncfusion.Windows.Forms.Tools.DrawTabEventArgs"/> instance.</param>
    /// <returns>True to indicate custom drawing was done; false otherwise.</returns>
    public delegate bool DrawItemCallback(DrawTabEventArgs eventArgs);

    /// <summary>
    /// Handle the <see cref="TabControlAdv.SelectedIndexChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="SelectedIndexChangingEventArgs"/> instance.</param>
    public delegate void SelectedIndexChangingEventHandler(object sender, SelectedIndexChangingEventArgs args);

    /// <summary>
    /// Class that encloses certain utility methods to be used by tab renderer implementations.
    /// </summary>
    public class TabUtils
    {
        /// <summary>
        /// Transforms (or removes the transform) a given rectangle, based on alignment, such that
        /// drawing code written for the TabAlignment.Top logic can be reused for all other alignments.
        /// </summary>
        /// <param name="g">The associated Graphics object when the paint event occurs.</param>
        /// <param name="align">The current alignment.</param>
        /// <param name="rect">The RectangleF that is to be transformed.</param>
        /// <param name="apply">Indicates whether to transform or to remove the transform.</param>
        /// <returns>The transformed rectangle, if apply is true; the rectangle on which
        /// the reverse transform is applied otherwise.</returns>
        /// <remarks>
        /// Use this in conjunction with the TabRendererBase.ApplyTransform in your custom
        /// implementation of TabRendererBase.
        /// </remarks>
        public static RectangleF ApplyTransform(Graphics g, TabAlignment align, RectangleF rect, bool apply)
        {
            if (align == TabAlignment.Top)
                return rect;

            // Transform based on current alignment
            PointF[] points = new PointF[2];

            Matrix transformMatrix = null;

            switch (align)
            {
                case TabAlignment.Left:
                    if (apply)
                    {
                        g.RotateTransform(90.0F);
                        g.ScaleTransform(-1, 1, MatrixOrder.Append);
                        points[0].X = rect.Left; points[0].Y = rect.Top;
                        points[1].X = rect.Right - 1; points[1].Y = rect.Bottom - 1;
                    }
                    else
                    {
                        g.ScaleTransform(-1, 1);
                        g.RotateTransform(-90.0F, MatrixOrder.Append);
                        points[0].X = rect.Left; points[0].Y = rect.Top;
                        points[1].X = rect.Right - 1; points[1].Y = rect.Bottom - 1;
                    }
                    break;
                case TabAlignment.Right:
                    if (apply)
                    {
                        g.RotateTransform(270.0F);
                        points[0].X = rect.Right - 1; points[0].Y = rect.Top;
                        points[1].X = rect.Left; points[1].Y = rect.Bottom - 1;
                    }
                    else
                    {
                        g.RotateTransform(-270.0F);
                        points[0].X = rect.Left; points[0].Y = rect.Bottom - 1;
                        points[1].X = rect.Right - 1; points[1].Y = rect.Top;
                    }
                    break;
                case TabAlignment.Bottom:
                    if (apply)
                    {
                        g.ScaleTransform(1, -1);
                        points[0].X = rect.Left; points[0].Y = rect.Bottom - 1;
                        points[1].X = rect.Right - 1; points[1].Y = rect.Top;
                    }
                    else
                    {
                        g.ScaleTransform(1, -1);
                        points[0].X = rect.Left; points[0].Y = rect.Bottom - 1;
                        points[1].X = rect.Right - 1; points[1].Y = rect.Top;
                    }
                    break;
            };
            transformMatrix = g.Transform;
            g.ResetTransform();

            transformMatrix.TransformPoints(points);

            points[0] = Point.Round(points[0]);
            points[1] = Point.Round(points[1]);

            return new RectangleF(points[0].X, points[0].Y, points[1].X - points[0].X + 1F,
                points[1].Y - points[0].Y + 1F);
        }
    }


    /// <summary>
    /// A default <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> implementation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Syncfusion.Windows.Forms.Tools.SingleLineTabPanelRenderer"/> and <see cref="Syncfusion.Windows.Forms.Tools.MultilineTabPanelRenderer"/>
    /// renderers derive from this class.
    /// </remarks>
    public abstract class TabPanelRenderer :
        ITabPanelRenderer,
        ITabPanelDefaultProperties
    {
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.GetPreferredSize"/>.
        /// </summary>
        public abstract void GetPreferredSize(Graphics g, ref SizeF preferredSize);
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.TabPanelRenderer.ComputeTabPositions"/>.
        /// </summary>
        public abstract void ComputeTabPositions(Graphics g);

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.GetTabBounds"/>.
        /// </summary>
        public virtual Rectangle GetTabBounds(int index)
        {
            if (index < 0 || index >= this.tabRenderers.Count)
                return Rectangle.Empty;

            RectangleF tabBounds = ((ITabRenderer)this.tabRenderers[index]).GetCurrentBounds();

            return Rectangle.Round(ApplyDrawingTransform(tabBounds, false));
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public ArrayList Renderers
        {
            get { return this.tabRenderers; }
        }

        /// <summary>
        /// Calls the <see cref="TabControlAdv.OnDrawItem"/> method.
        /// </summary>
        public bool DrawItemCallback(DrawTabEventArgs eventArgs)
        {
            return parent.OnDrawItem(eventArgs);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool bringSelectedTabToFront = false;
        private bool needLayout = false;
        private bool m_bForceDrawImage = false;

        public bool ForceDrawImage
        {
            get
            {
                return m_bForceDrawImage;
            }
            set
            {
                if (m_bForceDrawImage != value)
                {
                    m_bForceDrawImage = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the layout is required.
        /// </summary>
        public bool NeedLayout
        {
            get { return needLayout; }
        }
        /// <summary>
        /// Sets the tab positions that needs to be recalculated.
        /// </summary>
        /// <param name="needLayout"></param>
        protected void SetNeedLayout(bool needLayout)
        {
            this.needLayout = needLayout;
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.Layout"/>.
        /// </summary>
        /// <param name="g">A <see cref="System.Drawing.Graphics"/> instance.</param>
        public virtual void Layout(Graphics g, bool fromPaint)
        {
            if (NeedLayout)
            {
                SetNeedLayout(false);
            }
            ComputeTabPositions(g);
            ValidateScrollOffset(bringSelectedTabToFront, !fromPaint);
            bringSelectedTabToFront = false;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected RectangleF tdbounds;
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.Bounds"/>.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return ApplyDrawingTransform(tdbounds, false);
            }
            set
            {
                tdbounds = ApplyDrawingTransform(value, true);
                this.SetNeedLayout(true);
            }
        }
        /// <summary>
        /// Hash containing [ control ]:[ ToolTip ] pairs.
        /// </summary>
        private Hashtable m_toolTipHash = new Hashtable();
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.ScrollingSupported"/>.
        /// </summary>
        public virtual bool ScrollingSupported
        {
            get { return false; }
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.CanScrollLeft"/>.
        /// </summary>
        public virtual bool CanScrollLeft
        {
            get { return false; }
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.CanScrollRight"/>.
        /// </summary>
        public virtual bool CanScrollRight
        {
            get { return false; }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual bool IsMirrored
        {
            get
            {
                TabControlAdv tcaTabCtrl = parent as TabControlAdv;
                return tcaTabCtrl != null ? tcaTabCtrl.IsMirrored : false;
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool GetIsMirroredForVerticalAlignment()
        {
            bool bIsMirrored = false;

            TabControlAdv tcaTabCtrl = parent as TabControlAdv;
            if (null != tcaTabCtrl)
            {
                bIsMirrored = tcaTabCtrl.GetIsMirroredForVerticalAlignment();
            }

            return bIsMirrored;
        }

        /// <summary>
        /// Returns the default active tab color.
        /// </summary>
        /// <returns>The Color instance.</returns>
        public virtual Color DefaultActiveTabColor()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultActiveTabColor(this.TabPanelData, parent);
        }

        /// <summary>
        /// Returns the default inactive tab color.
        /// </summary>
        /// <returns>The Color instance.</returns>
        public virtual Color DefaultInactiveTabColor()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultInactiveTabColor(this.TabPanelData, parent);
        }

        /// <summary>
        /// Returns the default tab forecolor.
        /// </summary>
        /// <returns>The Color instance.</returns>
        public virtual Color DefaultTabForeColor()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultTabForeColor(this.TabPanelData, parent);
        }

        /// <summary>
        /// Returns the default tab panel background color.
        /// </summary>
        /// <returns>The Color instance.</returns>
        public virtual Color DefaultTabPanelBackgroundColor()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultTabPanelBackgroundColor(this.TabPanelData, parent);
        }

        /// <summary>
        /// Returns the default single border color.
        /// </summary>
        /// <returns>The Color instance.</returns>
        public virtual Color DefaultFixedSingleBorderColor()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultFixedSingleBorderColor(this.TabPanelData, parent);
        }
        /// <summary>
        /// Returns the default tab panel font.
        /// </summary>
        /// <returns>The Font instance.</returns>
        public virtual Font DefaultTabPanelFont()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultTabPanelFont(this.TabPanelData, parent);
        }

        /// <summary>
        /// Returns the default active tab font.
        /// </summary>
        /// <returns>The Font instance.</returns>
        public virtual Font DefaultActiveTabFont()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultActiveTabFont(this.TabPanelData, parent);
        }
        /// <summary>
        /// Returns the default inactive tab font.
        /// </summary>
        /// <returns>The Font instance.</returns>
        public virtual Font DefaultInactiveTabFont()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.DefaultInactiveTabFont(this.TabPanelData, parent);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal virtual bool ShouldDrawVisible(ITabData tabData)
        {
            return this.parent.IsDesignMode() || tabData.TabVisible;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int GetHiddenTabsCount()
        {
            int hiddenTabs = 0;
            foreach (ITabRenderer tabRenderer in this.tabRenderers)
            {
                if (this.ShouldDrawVisible(tabRenderer.TabData) == false)
                    hiddenTabs++;
            }
            return hiddenTabs;
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.Scroll"/>.
        /// </summary>
        public virtual void Scroll(ScrollIncrement increment, ScrollDirection direction) { return; }

        #region MEMBERS_TAB_POSITION_AND_COUNT_SENSITIVE
        protected ArrayList tabRenderers;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int currentHotTrackTab = -1;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int currentTooltipTab = -1;
        #endregion MEMBERS_TAB_POSITION_AND_COUNT_SENSITIVE
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.ValidateScrollOffset"/>.
        /// </summary>
        /// <param name="makeSelectedTabVisible"></param>
        /// <param name="invalidate"></param>
        public virtual void ValidateScrollOffset(bool makeSelectedTabVisible, bool invalidate) { }
        /// <summary>
        /// Transforms the RectangleF in client co-ordinates to rotated drawing co-ordinates.
        /// </summary>
        /// <param name="rect">The RectangleF to transform.</param>
        /// <param name="apply">Indicates whether to transform to drawing co-ordinates or to transform to client co-ordinates.</param>
        /// <returns>
        /// The transformed <see cref="System.Drawing.RectangleF"/>.
        /// </returns>
        /// <remarks>
        /// The TabPanelRenderer class performs the drawing of the tabs in a transformed space to
        /// accommodate the top, bottom, left and right alignment of the tabs. The transform
        /// is such that irrespective of the tab alignment the TabPanelRenderer can use the same code
        /// to draw the tabs.
        /// </remarks>
        public virtual RectangleF ApplyDrawingTransform(RectangleF rect, bool apply)
        {
            TabAlignment align = this.TabPanelData.Alignment;
            Graphics g = this.parent.GetGraphics();
            return TabUtils.ApplyTransform(g, align, rect, apply);
        }

        /// <summary>
        /// Creates a new instance of the TabPanelRenderer class.
        /// </summary>
        public TabPanelRenderer()
        {
            tabRenderers = new ArrayList();
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.NeedMouseMove"/>.
        /// </summary>
        public bool NeedMouseMove
        {
            get
            {
                bool bResult = this.TabPanelData.HotTrack || this.TabPanelData.ShowToolTips;

                if (!bResult)
                {
                    ITabPanelData2 data = this.TabPanelData as ITabPanelData2;

                    if (data != null)
                    {
                        bResult = data.ShowSuperToolTips;
                    }
                }

                return bResult;
            }
        }
        protected ITabControl parent;
        private ITabPanelData tabPanelData;
        /// <summary>
        /// Called when a <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/> instance is detached from this renderer.
        /// </summary>
        public virtual void Detach()
        {
            if (this.TabPanelData != null)
            {
                TabPanelData.PropertyChanged -= new TabPanelPropertyChangedEventHandler(this.TabPanel_PropertyChanged);
                TabPanelData.TabsData.CollectionAffected -= new EventHandler(this.Tabs_CollectionChanged);

                foreach (ITabData tabData in TabPanelData.TabsData)
                {
                    tabData.BoundsAffected -= new EventHandler(this.Tab_BoundsAffected);
                    tabData.PropertyChanged -= new EventHandler(this.Tab_PropertyChanged);
                }
            }

            if (m_toolTip != null)
            {
                m_toolTip.Dispose();
                m_toolTip = null;
            }

            RemoveTabRenderers();
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.TabPanelData"/>.
        /// </summary>
        public ITabPanelData TabPanelData
        {
            get
            {
                return tabPanelData;
            }
            set
            {
                if (tabPanelData != value)
                {
                    if (tabPanelData != null)
                    {
                        Detach();
                    }
                    tabPanelData = value;
                    if (tabPanelData != null)
                    {
                        tabPanelData.PropertyChanged += new TabPanelPropertyChangedEventHandler(this.TabPanel_PropertyChanged);
                        tabPanelData.TabsData.CollectionAffected += new EventHandler(this.Tabs_CollectionChanged);
                        OnTabsCollectionChanged();
                    }
                }
            }
        }
        /// <summary>
        /// Creates a new <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> given a tab style name.
        /// </summary>
        /// <param name="tabStyle">The tab style name.</param>
        /// <param name="tabData">The <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> containing the data for the renderer. Can be null.</param>
        /// <returns>The new <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/>.</returns>
        protected ITabRenderer CreateNewRenderer(string tabStyle, ITabData tabData)
        {
            ITabRenderer renderer = TabRendererFactory.CreateNewTabRenderer(tabStyle, this.parent, this);
            renderer.ForceDrawImage = this.ForceDrawImage;

            if (renderer != null)
            {
                renderer.TabData = tabData;
            }

            return renderer;
        }
        private void Tabs_CollectionChanged(object sender, EventArgs e)
        {
            OnTabsCollectionChanged();
        }

        /// <summary>
        /// Removes the <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/>s.
        /// </summary>
        protected virtual void RemoveTabRenderers()
        {
            for (int i = this.tabRenderers.Count - 1; i >= 0; i--)
            {
                ITabRenderer tabRenderer = (ITabRenderer)this.tabRenderers[i];

                if (tabRenderer != null)
                {
                    if (this.TabPanelData.TabsData.IndexOf(tabRenderer.TabData) == -1)
                        this.tabRenderers.RemoveAt(i);

                    tabRenderer.Dispose();
                }
            }
        }

        /// <summary>
        /// Called when the tabs collection is changed.
        /// </summary>
        protected virtual void OnTabsCollectionChanged()
        {
            this.ResetHotTracking();
            this.currentTooltipTab = -1;
            // Parse through the new collection and update tabRenderers list
            int i = 0;
            TabDataCollection tabPanelCollection = this.TabPanelData.TabsData;
            if (tabPanelCollection.Count < this.tabRenderers.Count)
                RemoveTabRenderers();

            foreach (ITabData tabData in tabPanelCollection)
            {
                bool bNewTabData = false;
                if (tabRenderers.Count > i)
                {
                    // Verify if the existing renderer is bound to the right tabData.
                    ITabData oldTabData = ((ITabRenderer)tabRenderers[i]).TabData;
                    if (oldTabData != tabData)
                    {
                        // Release handlers for old tabData
                        oldTabData.BoundsAffected -= new EventHandler(this.Tab_BoundsAffected);
                        oldTabData.PropertyChanged -= new EventHandler(this.Tab_PropertyChanged);
                        ((ITabRenderer)tabRenderers[i]).TabData = tabData;
                        ((ITabRenderer)tabRenderers[i]).Visible = this.ShouldDrawVisible(tabData);

                        bNewTabData = true;	// Possibly a new tabData
                    }
                }
                else
                {
                    // insert new renderer.
                    ITabRenderer renderer = CreateNewRenderer(TabPanelData.TabStyle, tabData);

                    if (renderer != null)
                    {
                        tabRenderers.Insert(i, renderer);

                        TabRendererBase rendererBase = renderer as TabRendererBase;
                        TabControlAdv tabControl = this.parent as TabControlAdv;

                        if (rendererBase != null && tabControl != null && !rendererBase.ShowCloseButton)
                        {
                            if (!tabControl.TabPages[i].ShowCloseButton)
                            {
                                rendererBase.ShowCloseButton = false;
                            }
                            else
                            {
                                rendererBase.ShowCloseButton = tabControl.ShouldDrawCloseButton(i);
                            }
                        }

                        renderer.DrawItemCallback = new DrawItemCallback(this.DrawItemCallback);
                    }
                    bNewTabData = true;
                }

                // Setup handlers for new tabData
                if (bNewTabData)
                {
                    tabData.BoundsAffected += new EventHandler(this.Tab_BoundsAffected);
                    tabData.PropertyChanged += new EventHandler(this.Tab_PropertyChanged);
                }
                i++;
            }
            this.OnBoundsAffected();
        }

        /// <summary>
        /// Returns the largest height for any tab.
        /// </summary>
        /// <param name="g">A Graphics object.</param>
        /// <returns>The largest height.</returns>
        public virtual float GetLargestHeight(Graphics g)
        {
            float height = 0;
            if ((TabPanelData.Alignment == TabAlignment.Left || TabPanelData.Alignment == TabAlignment.Right)
                && TabPanelData.RotateTextWhenVertical)
            {
                float largestHeight = 0F;
                // First pass: Compute the largest height
                foreach (ITabRenderer renderer in tabRenderers)
                {
                    if (ShouldDrawVisible(renderer.TabData))
                    {
                        float itemHeight = renderer.GetPreferredSize(g).Height;
                        if (largestHeight < itemHeight)
                        {
                            largestHeight = itemHeight;
                        }
                    }
                }
                height = largestHeight;
            }
            else if (tabRenderers.Count > 0)
            {
                float maxHeight = 0;
                foreach (ITabRenderer tabRenderer in this.tabRenderers)
                {
                    if (maxHeight < tabRenderer.GetPreferredSize(g).Height)
                    {
                        maxHeight = tabRenderer.GetPreferredSize(g).Height;
                    }
                }
                height += maxHeight;
            }

            return height;
        }

        /// <summary>
        /// Called when a property is changed that requires recalculating the preferred size and layout.
        /// </summary>
        protected virtual void OnBoundsAffected()
        {
            this.SetNeedLayout(true);
            parent.OnTabPanelBoundsAffected();
        }
        /// <summary>
        /// The event handler that gets called when the corresponding <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/>'s property is changed.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelData"/>.</param>
        /// <param name="e">A <see cref="Syncfusion.Windows.Forms.Tools.TabPanelPropertyChangedEventArgs"/> containing information regarding this event.</param>
        protected virtual void TabPanel_PropertyChanged(ITabPanelData sender, TabPanelPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedIndex":
                    OnSelectionChanged((int)e.OldValue, (int)e.NewValue); break;

                case "InactiveTabColor":
                case "ActiveTabColor":
                case "TextAlignment":
                case "TextLineAlignment":
                case "BackColor":
                case "RotateText180WhenLeftAligned":
                case "DisableInactivePageImage":
                case "LevelTextAndImage":
                case "ImageOffset":
                    {
                        if (parent.GetControl().IsHandleCreated && parent.GetControl().Visible)
                            parent.OnRepaint(this.Bounds);
                        break;
                    }
                case "FixedSingleBorderColor":
                    {
                        if (parent.GetControl().IsHandleCreated && parent.GetControl().Visible)
                            parent.GetControl().Invalidate();
                        break;
                    }
                case "TabStyle":
                    OnTabStyleChanged(); break;
                case "ActiveTabFont":
                case "Font":
                case "ImageAlignmentR":
                case "AdjustTopGap":
                case "RotateTextWhenVertical":
                case "Padding":
                case "SizeMode":
                case "TabSize":
                case "TabGap":
                case "Alignment":
                case "VerticalAlignment":
                case "Multiline":
                case "ImageList":
                case "BorderStyle":
                    OnBoundsAffected(); break;
            }
        }
        /// <summary>
        /// Called when the associated <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/>'s
        /// <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> is changed.
        /// </summary>
        protected internal virtual void OnTabStyleChanged()
        {
            // Delete current tabRenderers, if any
            RemoveTabRenderers();
            this.tabRenderers.Clear();
            OnTabsCollectionChanged();
            OnBoundsAffected();
        }
        /// <summary>
        /// Called when the associated <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/>'s selected index is changed.
        /// </summary>
        /// <param name="previousIndex">The previously selected tab index.</param>
        /// <param name="currentIndex">The newly selected tab index.</param>
        protected virtual void OnSelectionChanged(int previousIndex, int currentIndex)
        {
            this.InvalidateTabs(currentIndex, previousIndex);
        }

        /// <summary>
        /// Invalidates the union of the tab regions specified by their indices.
        /// </summary>
        /// <param name="tab1">The beginning tab index.</param>
        /// <param name="tab2">The ending tab index.</param>
        protected virtual void InvalidateTabs(int tab1, int tab2)
        {
            if (!this.parent.GetControl().Visible || !this.parent.GetControl().IsHandleCreated)
                return;

            RectangleF rectTab1 = RectangleF.Empty, rectTab2 = RectangleF.Empty, union;

            if ((tab1 != -1) && (tab1 < tabRenderers.Count))
                rectTab1 = ((ITabRenderer)tabRenderers[tab1]).GetRedrawBounds();

            if ((tab2 != -1) && (tab2 < tabRenderers.Count))
                rectTab2 = ((ITabRenderer)tabRenderers[tab2]).GetRedrawBounds();

            if (rectTab1 == RectangleF.Empty && rectTab2 == RectangleF.Empty)
                return;

            if (rectTab1 == RectangleF.Empty)
                union = rectTab2;
            else if (rectTab2 == RectangleF.Empty)
                union = rectTab1;
            else
                union = RectangleF.Union(rectTab1, rectTab2);

            RectangleF unionInflated = new RectangleF((float)Math.Floor(union.Left), (float)Math.Floor(union.Top),
                    (float)Math.Ceiling(union.Right) - (float)Math.Floor(union.Left) + 1,
                    (float)Math.Ceiling(union.Bottom) - (float)Math.Floor(union.Top) + 1
                    );
            // Invoke a paint
            this.parent.OnRepaint(ApplyDrawingTransform(unionInflated, false));
        }

        /// <summary>
        /// The event handler that gets called when a <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>'s property is changed.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>.</param>
        /// <param name="e">An EventArgs instance containing information regarding this event.</param>
        protected virtual void Tab_PropertyChanged(object sender, EventArgs e)
        {
            ITabData tabData = (ITabData)sender;
            if (this.TabPanelData != null)
            {
                int index = this.TabPanelData.TabsData.IndexOf(tabData);

                if (this.tabRenderers.Count <= index)
                    return;

                ((ITabRenderer)tabRenderers[index]).TabPropertyChanged();

                if (this.parent.GetControl().IsHandleCreated && this.parent.GetControl().Visible
                    // This event doesn't get called for TabVisible.
                    && this.ShouldDrawVisible(tabData))
                {
                    // Get the rect to redraw
                    SizeF overlappedSize = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle).GetOverlapSize(this.TabPanelData.TabSize);
                    overlappedSize.Height += TabPanelData.AdjustTopGap;

                    RectangleF tabRect = ((ITabRenderer)tabRenderers[index]).Bounds;
                    tabRect.Inflate(new SizeF(overlappedSize.Width, 0));
                    tabRect.Offset(0, -overlappedSize.Height);
                    tabRect.Height += overlappedSize.Height;

                    // Invoke a paint
                    this.parent.OnRepaint(ApplyDrawingTransform(tabRect, false));
                }
            }
        }

        /// <summary>
        /// The event handler that gets called when a <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>'s bounds are affected.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>.</param>
        /// <param name="e">An EventArgs instance containing information regarding this event.</param>
        protected virtual void Tab_BoundsAffected(object sender, EventArgs e)
        {
            ITabData tabData = (sender as ITabData);

            //int index = this.TabPanelData.TabsData.IndexOf(tabData);
            int index = this.parent.Renderer.TabPanelData.TabsData.IndexOf(tabData);
            if (index != -1 && this.tabRenderers.Count > index)
                ((ITabRenderer)tabRenderers[index]).TabPropertyChanged();

            this.OnBoundsAffected();
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual bool IsBackgroundSolid()
        {
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            return extender.IsBackgroundSolid();
        }


        /// <summary>
        /// Paints the tab panel background.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="backColor">The background <see cref="System.Drawing.Color"/>.</param>
        /// <param name="rect">The rectangle that should be used for the painting region.</param>
        public virtual void OnPaintPanelBackground(Graphics g, Color backColor, Rectangle rect)
        {
            // Fill the whole panel (the panel renderer will not fill the whole if there were scroll buttons
            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            extender.OnPaintPanelBackground(this.parent, g, backColor, rect);
        }

        /// <summary>
        /// Returns the backcolor of TabPanel.
        /// </summary>
        /// <value></value>
        public virtual Color TabPanelBackColor
        {
            get
            {
                Color bg = this.TabPanelData.BackColor;
                if (bg == Color.Empty)
                    bg = this.DefaultTabPanelBackgroundColor();

                return bg;
            }
        }
        /// <summary>
        /// Draws the tabs.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="clipRect">The rectangle that should be clipped from the drawing region.</param>
        public virtual void OnPaint(Graphics g, Rectangle clipRect)
        {
            if (this.parent != null && (this.parent is BackStage) && (this.parent as BackStage).Alignment == TabAlignment.Right)
            {
                using (SolidBrush brush = new SolidBrush((this.parent as BackStage).SelectedTab.BackColor))
                {
                    g.FillRectangle(brush, new Rectangle(clipRect.X, clipRect.Y, 2, clipRect.Height));
                }
            }
            TabControlAdv tabCotnrol = this.parent as TabControlAdv;
            if (tabRenderers.Count == 0 && (tabCotnrol == null || !tabCotnrol.ReserveTabSpace))
            {
                return;
            }

            if (NeedLayout)
            {
                SetNeedLayout(false);
                this.Layout(g, true);
            }

            Region oldClipRegion = g.Clip;

            RectangleF bounds = this.Bounds;
            if (this.parent != null)
            {
                MDITabPanel panel = this.parent.GetControl() as MDITabPanel;

                if (panel != null && !panel.IsCloseButtonActive())
                {
                    bounds.Width += MDITabPanel.CLOSE_BUTTON_AREA_WIDTH;
                }
            }

            g.SetClip(new Region(bounds), CombineMode.Intersect);

            // Draw Background (it's important that we draw the BG after drawing the borders)
            Color bg = this.TabPanelData.BackColor;
            if (bg == Color.Empty)
                bg = this.DefaultTabPanelBackgroundColor();

            RectangleF rectBG = bounds;

            // Tab panel background is adjusted so it is not clipping tabcontrol borders
            // previously drawn in Draw3DBorders()
            bool bIsMirrored = IsMirrored;
            int nWidthHighlight = 1;
            int nWidthShade = 2;

            switch (this.TabPanelData.Alignment)
            {
                case TabAlignment.Left:
                    // LTR: left side adjustment of 1 pixel for highlight
                    // RTL: left side adjustment of 2 pixels for shade
                    rectBG.Width -= bIsMirrored ? nWidthShade : nWidthHighlight;
                    break;
                case TabAlignment.Right:
                    // RTL: right side adjustment of 1 pixels for highlight
                    if (bIsMirrored)
                    {
                        rectBG.X += 1;
                        rectBG.Width -= 1;
                    }
                    // LTR: right side adjustment of 2 pixel for shade
                    else
                    {
                        rectBG.X += 2;
                        rectBG.Width -= 2;
                    }
                    break;
                case TabAlignment.Top: rectBG.Height -= 1; break;
                case TabAlignment.Bottom: rectBG.Offset(0, 2); rectBG.Height -= 2; break;
            }

            ITabDefaultProperties extender = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle);
            if (rectBG.Width != 0 && rectBG.Height != 0)
            {
                extender.OnPaintPanelBackground(this.parent, g, bg, Rectangle.Ceiling(rectBG));
            }

            // Check out if we should draw left to right or right to left (useful typically for single line tabs)
            // Depends on the drawing style of the tab renderers
            bool bDrawLeftToRight = true;

            if (!TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle).DrawLeftToRight)
                bDrawLeftToRight = false;

            // Invoke each tabRenderer's Paint
            // Except, draw the selected tab finally.
            int curSelectedIndex = this.TabPanelData.SelectedIndex;

            bool incrementPaint = false;

            if (bIsMirrored)
            {
                TabControlAdv tabControl = this.parent as TabControlAdv;

                if (tabControl != null)
                {
                    incrementPaint = !tabControl.RotateTabsWhenRTL;
                }
            }

            if (bDrawLeftToRight || incrementPaint)
            {
                int i = 0;
                foreach (ITabRenderer renderer in this.tabRenderers)
                {
                    if (i != curSelectedIndex && renderer.Visible)
                    {
                        renderer.OnPaint(g, clipRect);
                    }
                    i++;
                }
            }
            else
            {
                for (int i = this.tabRenderers.Count - 1; i >= 0; i--)
                {
                    if (i != curSelectedIndex)
                    {
                        ITabRenderer renderer = (ITabRenderer)this.tabRenderers[i];
                        if (renderer.Visible)
                            renderer.OnPaint(g, clipRect);
                    }
                }
            }
            if (curSelectedIndex != -1)
            {
                ITabRenderer renderer = (ITabRenderer)this.tabRenderers[curSelectedIndex];
                if (renderer.Visible)
                    renderer.OnPaint(g, clipRect);
            }

            g.SetClip(oldClipRegion, CombineMode.Replace);
        }

        /// <summary>
        /// Called by the tab control when mouse hovers on the control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnMouseHover(EventArgs e)
        {
            if (m_bShouldShowToolTipFirstTime && (this.ShouldShowToolTips || this.ShouldShowSuperToolTips))
            {
                m_bShouldShowToolTipFirstTime = false;

                StartShowingToolTip(DEF_TOOLTIP_INITIAL_TIMER_INTERVAL);
            }
        }


        /// <summary>
        /// Called by the tab control when mouse enter the bounds of the controls.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnMouseEnter(EventArgs e)
        {
        }

        /// <summary>
        /// Called by the tab control when mouse move has occurred.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance.</param>
        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (this.tabRenderers.Count <= 0)
                return;


            Point mousePosition = new Point(e.X, e.Y);

            // Transform it to the renderer co-ords
            RectangleF rectMousePos = this.ApplyDrawingTransform(new RectangleF((PointF)mousePosition, new SizeF(0, 0)), true);
            mousePosition = new Point((int)Math.Round(rectMousePos.Left), (int)Math.Round(rectMousePos.Top));


            int xPosOfMouse = mousePosition.X;
            float sumOfTabsLength = 0;
            RectangleF tabsBounds = this.tdbounds;

            switch (tabPanelData.Alignment)
            {
                case TabAlignment.Bottom:
                case TabAlignment.Top:
                    for (int i = 0; i < this.tabRenderers.Count; i++)
                    {
                        sumOfTabsLength += ((ITabRenderer)this.tabRenderers[i]).GetCurrentBounds().Width;
                        if (i > 0 && this.parent != null && this.parent is TabControlAdv)
                            sumOfTabsLength += (this.parent as TabControlAdv).TabGap;
                    }
                    sumOfTabsLength -= 4;

                    if (xPosOfMouse > sumOfTabsLength + tabsBounds.X ||
                            xPosOfMouse > tabsBounds.Width + tabsBounds.X ||
                            xPosOfMouse < tabsBounds.X)
                    {
                        this.ShowToolTip(null);
                        this.ShowSuperTooltip(null);
                    }
                    break;

                case TabAlignment.Left:
                case TabAlignment.Right:
                    if (xPosOfMouse > tabsBounds.Width + tabsBounds.X ||
                            xPosOfMouse < tabsBounds.X)
                    {
                        this.ShowToolTip(null);
                        this.ShowSuperTooltip(null);
                    }
                    break;

            }


            // To ensure that dnd moving is disabled after mouse up.
            // This should happen normally, but sometimes when the form takes longer
            // to activate, the dnd state doesn't get reset on mouseup!
            if (e.Button != MouseButtons.Left)
                this.Moving = false;

            // Need to do mouse processing all the time...
            // Turn on hot tracking if necessary
            //			if(this.TabPanelData.HotTrack == false
            //				&& !this.Moving)
            //				return;


            int newHitTab = -1;

            // First check the selected tab (considering overlap)
            if (this.tdbounds.Contains(mousePosition))
            {
                if (this.TabPanelData.SelectedIndex != -1 &&
                    ((ITabRenderer)this.tabRenderers[this.TabPanelData.SelectedIndex]).HitTest(mousePosition))
                    newHitTab = this.TabPanelData.SelectedIndex;
                else
                    newHitTab = HitTestTabs(mousePosition, true);
            }

            if (Moving)
            {
                if (this.HasMouseMovedEnough())
                    this.ptScreenMouseDown = Point.Empty;
                if (newHitTab != -1)
                    this.OnDragMoveTabs(e);
            }

            if ((tabPanelData.TabStyle.Equals("VS2010Style") || TabPanelData.HotTrack) && newHitTab != currentHotTrackTab && !parent.IsDesignMode())
            {
                // Reset the previous hottracktab
                if (this.currentHotTrackTab != -1)
                    ((ITabRenderer)this.tabRenderers[this.currentHotTrackTab]).HotTrack = false;
                // Set the new hottracktab
                if (newHitTab != -1)
                    ((ITabRenderer)this.tabRenderers[newHitTab]).HotTrack = true;

                // Update cache
                int prevHotTrackTab = this.currentHotTrackTab;
                this.currentHotTrackTab = newHitTab;

                // Redraw tabs
                this.InvalidateTabs(prevHotTrackTab, this.currentHotTrackTab);
            }

            if (newHitTab != this.currentTooltipTab)
            {
                // Set the new tooltip
                if (newHitTab != -1)
                {
                    if (!m_bShouldShowToolTipFirstTime && (this.ShouldShowToolTips || this.ShouldShowSuperToolTips))
                    {
                        // Hide previous tooltip
                        ShowToolTip(null);
                        ShowSuperTooltip(null);

                        // Start showing new tooltip
                        StartShowingToolTip(DEF_TOOLTIP_TIMER_INTERVAL);
                    }
                }

                // Update cache
                this.currentTooltipTab = newHitTab;
            }
        }


        #region ToolTip implementation
        private static readonly Point DEF_TOOLTIP_OFFSET = new Point(13, 15);
        private const int DEF_TOOLTIP_TIMER_INTERVAL = 100;
        private const int DEF_TOOLTIP_INITIAL_TIMER_INTERVAL = 500;

        private bool m_bisTabSelecting = false;
        private bool m_bShouldShowToolTipFirstTime = true;
        private System.Windows.Forms.Timer m_toolTipTimer = null;
        private ToolTipAdv m_toolTip = null;
        private SuperToolTip m_superToolTip = null;
        /// <summary>
        /// Returns the ToolTip text
        /// </summary>
        protected string ToolTipText
        {
            get
            {
                string strToolTipText = string.Empty;

                if (this.TabPanelData != null)
                {
                    if (this.TabPanelData.ShowToolTips &&
                        this.tabPanelData.TabsData != null && currentTooltipTab >= 0 &&
                        currentTooltipTab < this.TabPanelData.TabsData.Count)
                    {
                        ITabData tabData = this.TabPanelData.TabsData[currentTooltipTab] as ITabData;

                        if (tabData != null && tabData.ToolTip != null)
                        {
                            strToolTipText = tabData.ToolTip;
                        }
                    }
                    else if (this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit &&
                        currentTooltipTab >= 0 && currentTooltipTab < this.tabRenderers.Count)
                    {
                        ITabRenderer renderer = this.tabRenderers[currentTooltipTab] as ITabRenderer;
                        if (renderer != null && renderer.IsTextShrunk() && renderer.TabData != null)
                        {
                            strToolTipText = renderer.TabData.ToolTip;
                        }
                    }
                }

                return strToolTipText;
            }
        }


        /// <summary>
        /// Gets the SuperToolTip info.
        /// </summary>
        protected ToolTipInfo SuperToolTip
        {
            get
            {
                ToolTipInfo superToolTip = null;

                if (this.TabPanelData != null)
                {
                    ITabPanelData2 tabPanelData = this.TabPanelData as ITabPanelData2;

                    if (tabPanelData != null && tabPanelData.ShowSuperToolTips && this.tabPanelData.TabsData != null
                        && currentTooltipTab >= 0 && currentTooltipTab < this.TabPanelData.TabsData.Count)
                    {
                        ITabData2 tabData = this.TabPanelData.TabsData[currentTooltipTab] as ITabData2;

                        if (tabData != null && tabData.ToolTip != null)
                        {
                            superToolTip = tabData.SuperTooltip;
                        }
                    }
                    else if (this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit &&
                        currentTooltipTab >= 0 && currentTooltipTab < this.tabRenderers.Count)
                    {
                        ITabRenderer renderer = this.tabRenderers[currentTooltipTab] as ITabRenderer;
                        if (renderer != null && renderer.IsTextShrunk() && renderer.TabData != null)
                        {
                            ITabData2 tabData = renderer.TabData as ITabData2;

                            if (tabData != null)
                            {
                                superToolTip = tabData.SuperTooltip;
                            }
                        }
                    }
                }

                return superToolTip;
            }
        }

        private void InitTimer()
        {
            if (m_toolTipTimer == null)
            {
                m_toolTipTimer = new System.Windows.Forms.Timer();
                m_toolTipTimer.Tick += new EventHandler(OnToolTipTimerTick);
            }
        }

        private void InitToolTip()
        {
            if (this.parent != null)
            {
                Control owner = this.parent.GetControl();

                if (owner != null)
                {
                    if (m_toolTip == null)
                    {
                        m_toolTip = new ToolTipAdv(owner);

                        m_toolTip.BackColor = SystemColors.Info;
                        m_toolTip.BorderStyle = BorderStyle.FixedSingle;
                    }

                    if (m_superToolTip == null)
                    {
                        m_superToolTip = new SuperToolTip(owner);
                    }
                }
            }
        }
        /// <summary>
        /// Returns whether tooltips should be shown or not.
        /// </summary>
        protected bool ShouldShowToolTips
        {
            get
            {
                bool bShouldShow = false;

                if (this.TabPanelData != null)
                {
                    bShouldShow = (this.TabPanelData.ShowToolTips ||
                        this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit);

                    if ((this.parent != null && this.parent.IsDesignMode()) ||
                        m_bisTabSelecting)
                    {
                        bShouldShow = false;
                    }
                }
                return bShouldShow;
            }
        }

        /// <summary>
        /// Returns whether tooltips should be shown or not.
        /// </summary>
        protected bool ShouldShowSuperToolTips
        {
            get
            {
                bool bShouldShow = false;
                ITabPanelData2 tabPanelData = this.TabPanelData as ITabPanelData2;

                if (tabPanelData != null)
                {
                    bShouldShow = (tabPanelData.ShowSuperToolTips || this.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit);

                    if ((this.parent != null && this.parent.IsDesignMode()) || m_bisTabSelecting)
                    {
                        bShouldShow = false;
                    }
                }

                return bShouldShow;
            }
        }

        private void OnToolTipTimerTick(object sender, EventArgs e)
        {
            InitToolTip();

            ShowToolTip(this.ToolTipText);
            ShowSuperTooltip(this.SuperToolTip);

            m_toolTipTimer.Stop();
        }

        /// <summary>
        /// Validates position to show tooltip in.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>True, if position is in tab's bounds to show tooltip for, otherwise- false.</returns>
        private bool IsValidToolTipPosition(Point pos)
        {
            bool bShoudShow = false;

            if (this.parent != null)
            {
                Control control = this.parent.GetControl();

                if (control != null)
                {
                    if (this.Renderers != null && currentTooltipTab >= 0 &&
                        currentTooltipTab < this.Renderers.Count)
                    {
                        pos = control.PointToClient(pos);
                        RectangleF rectMousePos = this.ApplyDrawingTransform(
                            new RectangleF(pos, SizeF.Empty), true);

                        pos = Point.Round(rectMousePos.Location);
                        ITabRenderer renderer = this.Renderers[currentTooltipTab] as ITabRenderer;

                        bShoudShow = renderer.HitTest(pos);
                    }
                }
            }

            return bShoudShow;
        }

        /// <summary>
        /// Shows or Hides Tooltip window.
        /// </summary>
        /// <param name="text">Text to show in toolTip. If text is null or empty
        /// string, toolTip is hidden.</param>
        protected void ShowToolTip(string text)
        {
            if (m_toolTip != null)
            {
                // Hide tooltip
                if (text == null || text == string.Empty || !this.ShouldShowToolTips)
                {
                    if (m_toolTip.Visible)
                    {
                        m_toolTip.HidePopup();
                    }
                }

                // Show tooltip
                else
                {
                    Point showPos = Control.MousePosition;

                    // Show tooltip only when mouse is over Tab to show tooltip for
                    if (this.IsValidToolTipPosition(showPos))
                    {
                        showPos.Offset(DEF_TOOLTIP_OFFSET.X, DEF_TOOLTIP_OFFSET.Y);
                        m_toolTip.Text = text;
                        m_toolTip.ShowPopup(showPos);
                    }
                }
            }
        }

        /// <summary>
        /// Shows or hides super tooltip.
        /// </summary>
        /// <param name="info">The super tooltip info.</param>
        protected void ShowSuperTooltip(ToolTipInfo info)
        {
            if (m_superToolTip != null)
            {
                // Hide super tooltip
                if (info == null || !this.ShouldShowSuperToolTips)
                {
                    m_superToolTip.Hide();
                }
                // Show super tooltip
                else
                {
                    Point showPos = Control.MousePosition;

                    // Show tooltip only when mouse is over Tab to show tooltip for
                    if (this.IsValidToolTipPosition(showPos))
                    {
                        showPos.Offset(DEF_TOOLTIP_OFFSET.X, DEF_TOOLTIP_OFFSET.Y);
                        m_superToolTip.Show(info, showPos);
                    }
                }
            }
        }

        /// <summary>
        /// Start showing tooltips
        /// </summary>
        protected void StartShowingToolTip(int interval)
        {
            if (interval < 0)
            {
                throw new ArgumentException("interval");
            }

            if (m_toolTipTimer == null)
            {
                InitTimer();
            }

            if (m_toolTipTimer != null)
            {
                m_toolTipTimer.Interval = interval;
                m_toolTipTimer.Start();
            }
        }

        /// <summary>
        /// Stops showing tooltips
        /// </summary>
        protected void StopShowingToolTip()
        {
            if (m_toolTipTimer != null)
            {
                ShowToolTip(null);
                ShowSuperTooltip(null);

                m_bShouldShowToolTipFirstTime = !m_bisTabSelecting;
                m_toolTipTimer.Interval = DEF_TOOLTIP_INITIAL_TIMER_INTERVAL;
                m_toolTipTimer.Stop();
            }
        }
        #endregion

        /// <summary>
        /// Returns the tab index at the specified location.
        /// </summary>
        /// <param name="mousePosition">The location where hit test is to be performed.</param>
        /// <param name="inTransformedCoOrds">Indicates whether the above location is in absolute or transformed co-ordinates.</param>
        /// <returns>
        /// The hit tab index; -1 if no tab was found.
        /// </returns>
        public int HitTestTabs(PointF mousePosition, bool inTransformedCoOrds)
        {
            if (!inTransformedCoOrds)
            {
                // Transform it to the renderer co-ords
                RectangleF rectMousePos = this.ApplyDrawingTransform(new RectangleF(mousePosition, new SizeF(0, 0)), true);
                mousePosition = new Point((int)Math.Round(rectMousePos.Left), (int)Math.Round(rectMousePos.Top));
            }

            // Sometimes the mouse might be out of the renderer bounds (possible in the presence of X button area, for example),
            // so just return -1.
            if (!this.tdbounds.Contains(mousePosition))
                return -1;

            int i = 0, hitTab = -1;

            foreach (ITabRenderer renderer in this.tabRenderers)
            {
                if (renderer.HitTest(mousePosition))
                {
                    hitTab = i;
                }
                i++;
            }
            return hitTab;
        }

        /// <summary>
        /// Called by the tab control when mouse leave had occurred.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance.</param>

        public virtual void OnMouseLeave(EventArgs e)
        {
            this.Moving = false;
            ResetHotTracking();
            StopShowingToolTip();
        }
        private int selIndexOnDragStart = -1;
        private bool internalDragging = false;
        private RectangleF dragIgnoreRectTransformed = RectangleF.Empty;

        /// <summary>
        /// Returns the selected tab index when drag and drop started.
        /// </summary>
        protected int SelectedIndexOnDragStart
        {
            get { return this.selIndexOnDragStart; }
        }

        /// <summary>
        /// The rectangular region where drop should not be performed during drag and drop.
        /// </summary>
        /// <remarks>
        /// This region is in transformed co-ordinates.
        /// </remarks>
        protected RectangleF DragIgnoreRect
        {
            get { return this.dragIgnoreRectTransformed; }
        }

        private Point ptScreenMouseDown = Point.Empty;
        /// <summary>
        /// Indicates whether the user is moving tabs using a drag-and-drop.
        /// </summary>
        protected virtual bool Moving
        {
            get { return internalDragging; }
            set
            {
                if (internalDragging != value)
                {
                    internalDragging = value;
                    if (internalDragging == false)
                    {
                        this.parent.GetControl().Capture = false;
                        this.selIndexOnDragStart = -1;
                        this.dragIgnoreRectTransformed = RectangleF.Empty;
                    }
                    else
                    {
                        this.parent.GetControl().Capture = true;
                        this.selIndexOnDragStart = this.TabPanelData.SelectedIndex;
                    }
                }
            }
        }
        /// <summary>
        /// Cancels the current tab drag-and-drop.
        /// </summary>
        public virtual void CancelTabDrag()
        {
            // Move the current selected index to its previous position.
            if (this.TabPanelData.SelectedIndex != this.selIndexOnDragStart)
                this.MoveTabs(this.TabPanelData.SelectedIndex, 1, this.selIndexOnDragStart);
            Moving = false;
        }

        /// <summary>
        /// Gets the mouse position
        /// </summary>
        public Point GetMousePosition()
        {
            Point mousePosition = Control.MousePosition;
            this.ptScreenMouseDown = mousePosition;
            mousePosition = this.parent.GetControl().PointToClient(mousePosition);

            // Transform it to the renderer co-ords
            RectangleF rectMousePos = this.ApplyDrawingTransform(new RectangleF((PointF)mousePosition, new SizeF(0, 0)), true);
            mousePosition = new Point((int)Math.Round(rectMousePos.Left), (int)Math.Round(rectMousePos.Top));

            return mousePosition;
        }

        public Point GetMousePosition(Point mousePosition)
        {
            // Transform it to the renderer co-ords
            RectangleF rectMousePos = this.ApplyDrawingTransform(new RectangleF((PointF)mousePosition, new SizeF(0, 0)), true);
            mousePosition = new Point((int)Math.Round(rectMousePos.Left), (int)Math.Round(rectMousePos.Top));
            return mousePosition;
        }

        /// <summary>
        /// Called by the tab control when a mouse down occurs.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance.</param>
        public virtual void OnMouseDown(MouseEventArgs e)
        {
            if (this.TabPanelData == null && this.tabRenderers.Count <= 0)
                return;
            Point mousePos = GetMousePosition(e.Location);
            bool bJustSelected = false;

            // First check the selected tab (considering overlap)
            int hitTab = -1;
            if (this.TabPanelData.SelectedIndex != -1 &&
                ((ITabRenderer)this.tabRenderers[this.TabPanelData.SelectedIndex]).HitTest(mousePos))
            {
                // already selected, nothing more to do.
                hitTab = this.TabPanelData.SelectedIndex;
            }
            else
            {
                hitTab = this.HitTestTabs(mousePos, true);
                if (hitTab != -1)
                {
                    if (this.TabPanelData.SelectedIndex != hitTab)
                    {
                        if (this.TabPanelData.IsTabSelectable(hitTab, true))
                        {
                            TabControlAdv tabControl = parent as TabControlAdv;

                            if (tabControl != null)
                            {
                                TabPageAdv tabPage = tabControl.TabPages[hitTab];
                                if (tabPage != null)
                                {
                                    tabPage.SetSelectedAtDesignTime();
                                }
                            }

                            if ((tabControl.ValidateStatus == ValidateStatus.Failed)
                                || ((tabControl.ValidateStatus == ValidateStatus.None) &&
                                !this.parent.ValidateFocusedTab()))
                            {
                                // Cannot validate focused control, so don't change tabs.
                                return;
                            }
                            this.TabPanelData.SelectedIndex = hitTab;
                            bJustSelected = true;

                            // Check if the hit is still valid after the change in selected tab
                            int hitTab2 = this.HitTestTabs(mousePos, true);

                            if (hitTab != hitTab2)
                                hitTab = -1;	// To prevent further processing.
                        }
                        else
                            hitTab = -1;
                    }
                }
            }
            if (this.TabPanelData != null && this.TabPanelData.UserMoveTabs && !this.parent.IsDesignMode())
            {
                if ((Control.MouseButtons == MouseButtons.Left) && hitTab != -1 && !bJustSelected)
                    Moving = true;
            }

            if (hitTab < 0 && this.TabPanelData != null)
            {
                hitTab = this.TabPanelData.SelectedIndex;
            }
            if (hitTab < 0)
                return;

            // when click on the selected tab, then set focus to control hosted by tabpage
            TabRendererBase tabRenderer = tabRenderers[hitTab] as TabRendererBase;
            if (tabRenderer != null)
            {
                m_bisTabSelecting = true;
                StopShowingToolTip();

                MDIChildTabData tabData = tabRenderer.TabData as MDIChildTabData;
                if (tabData != null)
                {
                    Form frmMDIChild = tabData.MdiChild;
                    if (frmMDIChild != null)
                    {
                        System.Windows.Forms.Control.ControlCollection children = tabData.MdiChild.Controls;

                        if (children.Count > 0)
                        {
                            Control control = tabData.MdiChild.Controls[0];
                            control.Focus();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Indicates whether a tab is currently being moved.
        /// </summary>
        /// <returns>True if moving; false otherwise.</returns>
        public virtual bool IsMovingTab()
        {
            return this.Moving;
        }
        /// <summary>
        /// Called as the tab is dragged.
        /// </summary>
        /// <remarks>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance.</param>
        /// </remarks>
        protected virtual void OnDragMoveTabs(MouseEventArgs e)
        {
            if (!this.HasMouseMovedEnough())
                return;
            // Transform it to the renderer co-ords
            Point mousePosition = Control.MousePosition;
            mousePosition = this.parent.GetControl().PointToClient(mousePosition);
            RectangleF rectMousePos = this.ApplyDrawingTransform(new RectangleF((PointF)mousePosition, new SizeF(0, 0)), true);
            mousePosition = new Point((int)Math.Round(rectMousePos.Left), (int)Math.Round(rectMousePos.Top));

            if (this.dragIgnoreRectTransformed != RectangleF.Empty
                && this.dragIgnoreRectTransformed.Contains(mousePosition))
                return;

            int nDropTab = this.HitTestTabs(mousePosition, true);

            if (nDropTab != -1
                && nDropTab != this.TabPanelData.SelectedIndex)
            {
                if (nDropTab != this.selIndexOnDragStart)
                    this.dragIgnoreRectTransformed = ((ITabRenderer)this.tabRenderers[nDropTab]).Bounds;
                else
                    this.dragIgnoreRectTransformed = Rectangle.Empty;

                TabControlAdv tabControl = this.parent as TabControlAdv;
                TabMovingEventArgs movingArgs = new TabMovingEventArgs(this.tabPanelData.SelectedIndex, nDropTab);
                tabControl.OnTabMoving(movingArgs);
                if (!movingArgs.Cancel)
                {
                    if (movingArgs.From < movingArgs.Target )
                    {
                        for (int tabindex = movingArgs.From; tabindex < movingArgs.Target; tabindex++)
                        {
                            MoveTabs(tabindex, 1, tabindex + 1);
                            tabControl.OnTabsOrderChanged();
                        }
                    }
                    else
                    {
                        for (int tabindex = movingArgs.From; tabindex > movingArgs.Target; tabindex--)
                        {
                            MoveTabs(tabindex, 1, tabindex - 1);
                            tabControl.OnTabsOrderChanged();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called by the tab control when a mouse up occurs.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance.</param>
        public virtual void OnMouseUp(MouseEventArgs e)
        {
            m_bisTabSelecting = false;

            if (this.tabRenderers.Count <= 0)
                return;

            if (Moving == true && this.TabPanelData.UserMoveTabs == true)
            {
                this.OnDragMoveTabs(e);
                Moving = false;
            }
            this.ptScreenMouseDown = Point.Empty;
        }
        private bool HasMouseMovedEnough()
        {
            if (this.ptScreenMouseDown == Point.Empty)
                return true;

            Point ptCurrent = Control.MousePosition;
            if (Math.Abs(ptCurrent.X - this.ptScreenMouseDown.X) > 2
                || Math.Abs(ptCurrent.Y - this.ptScreenMouseDown.Y) > 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Called by the tab control when focussed.
        /// </summary>
        /// <param name="e">The EventArgs instance.</param>
        public virtual void OnGotFocus(EventArgs e)
        {
            this.InvalidateTabs(this.TabPanelData.SelectedIndex, -1);
        }
        /// <summary>
        /// Called by the tab control when it has lost focus.
        /// </summary>
        /// <param name="e">The EventArgs instance.</param>
        public virtual void OnLostFocus(EventArgs e)
        {
            this.InvalidateTabs(this.TabPanelData.SelectedIndex, -1);

            m_bisTabSelecting = false;
        }
        /// <summary>
        /// Moves tabs in groups from one position to other.
        /// </summary>
        /// <param name="nTabsToMoveFrom">The beginning position of the group.</param>
        /// <param name="nCount">The number of tabs to move.</param>
        /// <param name="nMoveToIndex">The destination position.</param>
        protected virtual void MoveTabs(int nTabsToMoveFrom, int nCount, int nMoveToIndex)
        {
            this.TabPanelData.TabsData.Move(nTabsToMoveFrom, nMoveToIndex, nCount);
            this.SetNeedLayout(true);
        }

        /// <summary>
        /// Resets hot tracking state, if any.
        /// </summary>
        protected void ResetHotTracking()
        {
            if (this.currentHotTrackTab != -1)
            {
                ((ITabRenderer)this.tabRenderers[this.currentHotTrackTab]).HotTrack = false;
                int prevHotTrackTab = this.currentHotTrackTab;
                this.currentHotTrackTab = -1;
                this.InvalidateTabs(prevHotTrackTab, -1);
            }
        }
    }

    /// <summary>
    /// A <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance
    /// that implements the multi-line tab mode.
    /// </summary>
    public class MultilineTabPanelRenderer : TabPanelRenderer
    {
        private int primitiveWidth = 0;

        /// <summary>
        /// Creates a new instance of the <see cref="Syncfusion.Windows.Forms.Tools.MultilineTabPanelRenderer"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public MultilineTabPanelRenderer(ITabControl parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer.GetPreferredSize"/>.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="preferredSize"></param>
        public override void GetPreferredSize(Graphics g, ref SizeF preferredSize)
        {
            if (this.tabRenderers.Count == 0)
            {
                preferredSize = new SizeF(0, 0);
                return;
            }

            float availableWidth = preferredSize.Width;
            if (availableWidth == 0)
                availableWidth = GetMinimumWidth(g);

            SizeF overlappedSize = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle).GetOverlapSize(this.TabPanelData.TabSize);
            overlappedSize.Height += TabPanelData.AdjustTopGap;

            float minHeight = 0, currentLineHeight = 0, maxHeight = 0;
            int rows = 1;

            // Parse through the tab renderers asking for PreferredSize
            float usedUpLineWidth = overlappedSize.Width;

            foreach (ITabRenderer tabRenderer in this.tabRenderers)
            {
                if (this.ShouldDrawVisible(tabRenderer.TabData))
                {
                    SizeF preferredTabSize = SizeF.Empty;

                    if (this.TabPanelData.SizeMode == TabSizeMode.Fixed)
                        preferredTabSize = this.TabPanelData.TabSize;
                    else
                        preferredTabSize = tabRenderer.GetPreferredSize(g);

                    TabControlAdv tabControl = this.parent as TabControlAdv;
                    if (tabControl != null && tabControl.TabPrimitivesHost.Visible)
                    {
                        primitiveWidth = tabControl.TabPrimitivesHost.Size.Width;
                    }

                    if (preferredTabSize.Width + usedUpLineWidth
                        <= (availableWidth - primitiveWidth) || usedUpLineWidth <= overlappedSize.Width)
                        // enough space available in current line
                        usedUpLineWidth += preferredTabSize.Width;
                    else
                    {
                        // move to next line
                        minHeight += currentLineHeight;

                        rows++;

                        // This item becomes the first item in the new line
                        usedUpLineWidth = overlappedSize.Width + preferredTabSize.Width;
                        if (maxHeight < currentLineHeight)
                            maxHeight = currentLineHeight;
                        currentLineHeight = 0F;
                    }
                    if (currentLineHeight < preferredTabSize.Height)
                        currentLineHeight = preferredTabSize.Height;
                }
            }

            if ((this.TabPanelData.Alignment == TabAlignment.Left
                || this.TabPanelData.Alignment == TabAlignment.Right)
                && this.TabPanelData.RotateTextWhenVertical)
            {
                if (maxHeight < currentLineHeight) maxHeight = currentLineHeight;
                preferredSize = new SizeF(availableWidth, maxHeight * rows + overlappedSize.Height);
            }
            else
                preferredSize = new SizeF(availableWidth, minHeight + currentLineHeight + overlappedSize.Height);

            if (this.TabPanelData.TabSize != Size.Empty && this.TabPanelData.SizeMode == TabSizeMode.Fixed)
            {
                preferredSize.Height = this.TabPanelData.TabSize.Height * rows + overlappedSize.Height;
            }
        }
        
        /// </override>
        protected override void InvalidateTabs(int tab1, int tab2)
        {
            // In multiline mode, we usually have to invalidate the whole area
            // otherwise we will end up invalidating some nodes partially.
            this.parent.OnRepaint(this.Bounds);
        }

        /// <summary>
        /// Returns the minimum width required in the tab panel to show all the tabs.
        /// </summary>
        /// <param name="g">A <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <returns>The minimum width in float.</returns>
        protected float GetMinimumWidth(Graphics g)
        {
            float minWidth = 0;
            if (this.tabRenderers.Count > 0)
            {
                foreach (ITabRenderer tabRenderer in this.tabRenderers)
                {
                    if (this.ShouldDrawVisible(tabRenderer.TabData))
                    {
                        float tabMinWidth = tabRenderer.GetPreferredSize(g).Width;
                        if (minWidth < tabMinWidth)
                            minWidth = tabMinWidth;
                    }
                }
                minWidth += TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle).GetOverlapSize(this.TabPanelData.TabSize).Width;
            }
            return minWidth;
        }

        /// <summary>
        /// Computes the tab positions given the Graphics context.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <remarks>
        /// <para>Called by the Layout to calculate the tab positions.</para>
        /// </remarks>
        public override void ComputeTabPositions(Graphics g)
        {
            if (tabRenderers.Count <= 0)
                return;

            // Parse through the tab renderers

            // Cache the bounds temporarily, to adjust for FillToRight style
            Hashtable computedBounds = new Hashtable();
            ArrayListExt lineEmptySpaces = new ArrayListExt();	// The empty space at the end of each line in multi-line mode

            float top, availableWidth, commonHeight;
            SizeF overlappedSize = TabRendererFactory.GetRegisteredExtender(this.TabPanelData.TabStyle).GetOverlapSize(this.TabPanelData.TabSize);
            overlappedSize.Height += TabPanelData.AdjustTopGap;

            float overlapWidth = (float)Math.Ceiling(overlappedSize.Width / 2);
            availableWidth = this.tdbounds.Width - overlapWidth;	// The overlapped portion to the right

            float usedUpLineWidth = overlapWidth;// The overlapped portion to the left
            // Assuming height to be constant between all tabs
            if ((this.TabPanelData.SizeMode == TabSizeMode.Normal ||
                this.TabPanelData.SizeMode == TabSizeMode.FillToRight))
                commonHeight = this.GetLargestHeight(g);
            else
                commonHeight = this.TabPanelData.TabSize.Height;

            top = this.tdbounds.Bottom - commonHeight;
            // Compute the first tab's bounds
            ITabRenderer firstTab = (ITabRenderer)this.tabRenderers[0];
            float preferredWidth = 0.0f;

            bool bIsMirrored = GetIsMirroredForVerticalAlignment();

            if (this.ShouldDrawVisible(firstTab.TabData))
            {
                if (this.TabPanelData.SizeMode == TabSizeMode.Normal ||
                    this.TabPanelData.SizeMode == TabSizeMode.FillToRight)
                    preferredWidth = firstTab.GetPreferredSize(g).Width;
                else
                    preferredWidth = this.TabPanelData.TabSize.Width;

                // Set the first tab's Bounds irrespective of space availability
                float fRectLeft = bIsMirrored ?
                    (tdbounds.Right - usedUpLineWidth - preferredWidth) :
                    (tdbounds.Left + usedUpLineWidth);
                computedBounds[firstTab] = new RectangleF(fRectLeft, top, preferredWidth,
                    commonHeight);

                usedUpLineWidth += preferredWidth;
            }
            else
            {
                computedBounds[firstTab] = RectangleF.Empty;
            }

            ArrayListExt renderers2D = new ArrayListExt();

            renderers2D.Add(new ArrayList());
            int curArrayIndex = 0;
            ((ArrayList)renderers2D[curArrayIndex]).Add(this.tabRenderers[0]);

            // Compute the bounds for the rest of the tabs.
            for (int i = 1; i < this.tabRenderers.Count; i++)
            {
                ITabRenderer tabRenderer = (ITabRenderer)this.tabRenderers[i];

                if (this.ShouldDrawVisible(tabRenderer.TabData))
                {
                    if (this.TabPanelData.SizeMode == TabSizeMode.Normal ||
                        this.TabPanelData.SizeMode == TabSizeMode.FillToRight)
                        preferredWidth = tabRenderer.GetPreferredSize(g).Width;
                    else
                        preferredWidth = this.TabPanelData.TabSize.Width;

                    // Check if there isn't any more space available
                    if (preferredWidth + usedUpLineWidth > availableWidth)
                    {
                        renderers2D.Add(new ArrayList());
                        curArrayIndex++;

                        // Store the empty space available in this line.
                        lineEmptySpaces.Add(availableWidth - usedUpLineWidth);

                        // move to next line
                        top -= (commonHeight - 1);	// There will be a overlap of 1 pixel height between the 2 lines
                        usedUpLineWidth = overlapWidth;
                    }

                    ((ArrayList)renderers2D[curArrayIndex]).Add(this.tabRenderers[i]);

                    // Set Bounds
                    float fRectLeft = bIsMirrored ?
                        (tdbounds.Right - usedUpLineWidth - preferredWidth) :
                        (tdbounds.Left + usedUpLineWidth);
                    computedBounds[tabRenderer] = new RectangleF(fRectLeft, top, preferredWidth,
                        commonHeight);

                    usedUpLineWidth += preferredWidth;
                }
                else
                {
                    computedBounds[tabRenderer] = RectangleF.Empty;
                }
            }

            // Add the empty space in the last line too
            lineEmptySpaces.Add(availableWidth - usedUpLineWidth);

            ArrayList adjustedRenderersOrder = this.tabRenderers;

            if (this.TabPanelData.KeepSelectedTabInFrontRow)
            {
                adjustedRenderersOrder = this.AdjustRenderersToMoveSelectedIndexToFront(renderers2D, this.TabPanelData.SelectedIndex, lineEmptySpaces);
            }

            // Do another pass to adjust the computed bounds if mode is set to FillToRight
            int curLine = 0;
            top = this.tdbounds.Bottom - commonHeight;
            usedUpLineWidth = overlapWidth;

            for (int i = 0; i < adjustedRenderersOrder.Count; i++)
            {
                ITabRenderer renderer = (ITabRenderer)adjustedRenderersOrder[i];

                if (this.ShouldDrawVisible(renderer.TabData))
                {
                    preferredWidth = ((RectangleF)computedBounds[renderer]).Width;

                    if (this.TabPanelData.SizeMode == TabSizeMode.FillToRight &&
                        preferredWidth + overlapWidth > availableWidth)
                        preferredWidth = availableWidth - overlapWidth;

                    if (usedUpLineWidth + preferredWidth > availableWidth
                        && usedUpLineWidth > overlapWidth)
                    {
                        // move to next line
                        top -= (commonHeight - 1);
                        usedUpLineWidth = overlapWidth;

                        curLine++;
                    }

                    // Only if there is more than 1 line...
                    if (lineEmptySpaces.Count > 1 && this.TabPanelData.SizeMode == TabSizeMode.FillToRight)
                    {
                        if (curLine < 0)
                        {
                            curLine = 0;
                        }
                        else if (curLine >= lineEmptySpaces.Count)
                        {
                            curLine = lineEmptySpaces.Count - 1;
                        }

                        float emptySpace = (float)lineEmptySpaces[curLine];

                        if (emptySpace >= 0)
                            preferredWidth += (float)Math.Round((preferredWidth / (availableWidth - emptySpace - overlapWidth)) * emptySpace);
                        else
                            preferredWidth = availableWidth - overlapWidth;
                    }
                    // Set Bounds
                    float fRectLeft = bIsMirrored ?
                        (tdbounds.Right - usedUpLineWidth - preferredWidth) :
                        (tdbounds.Left + usedUpLineWidth);
                    renderer.Bounds = new RectangleF(fRectLeft, top, preferredWidth,
                        commonHeight);

                    renderer.TabAlignment = this.TabPanelData.Alignment;

                    usedUpLineWidth += preferredWidth;

                    renderer.Visible = true;
                }
                else
                {
                    renderer.Bounds = RectangleF.Empty;
                    renderer.Visible = false;
                }
            }
        }

        private ArrayList AdjustRenderersToMoveSelectedIndexToFront(ArrayListExt renderers2D, int selectedIndex, ArrayListExt emptySpaceInEachLine)
        {
            int currentArrayIndex = -1;
            bool selectedIndexRowFound = false;
            int lastIndexInCurrentRow = 0;
            int selectedIndexRow = -1;

            // Find the row where the selected index is.
            while (!selectedIndexRowFound || renderers2D.Count <= currentArrayIndex)
            {
                currentArrayIndex++;

                if (currentArrayIndex >= renderers2D.Count)
                {
                    selectedIndexRowFound = true;
                    selectedIndexRow = (renderers2D.Count > 0) ? --currentArrayIndex : 0;
                    break;
                }

                lastIndexInCurrentRow += ((ArrayList)renderers2D[currentArrayIndex]).Count;

                if (selectedIndex < lastIndexInCurrentRow)
                {
                    selectedIndexRowFound = true;
                    selectedIndexRow = currentArrayIndex;
                    break;
                }
            }

            if (selectedIndexRow != -1)
            {
                if (selectedIndexRow == 0)
                    return this.tabRenderers;
                else
                {
                    // Move the rows around, such that the selected index row is moved to top.
                    renderers2D.Move(selectedIndexRow, 0, 1);
                    emptySpaceInEachLine.Move(selectedIndexRow, 0, 1);
                    // Now prepare the new renderers order:
                    ArrayList tabRenderers = new ArrayList();
                    foreach (ArrayList list in renderers2D)
                    {
                        foreach (ITabRenderer renderer in list)
                            tabRenderers.Add(renderer);
                    }
                    return tabRenderers;
                }
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Specifies certain default properties for a tab renderer used by TabControlAdv.
    /// </summary>
    /// <remarks>
    /// Use this class only when you are deriving from TabRendererBase to create a custom
    /// tab renderer.
    /// </remarks>
    public class TabUIDefaultProperties : ITabDefaultProperties
    {
        /// <summary>
        ///Indicates whether to draw from left to right.
        /// </summary>
        public virtual bool DrawLeftToRight
        {
            get { return true; }
        }
        /// <summary>
        /// Indicates whether to draw ellipsis if text width is larger than tab width.
        /// </summary>
        public virtual bool DrawEllipsis
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Returns the default backcolor for the panel.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Color value.</returns>
        /// <remarks>
        /// This implementation returns the tab control's BackColor.
        /// </remarks>
        public virtual Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return tabControl.GetControl().BackColor;
        }

        /// <summary>
        /// Returns the default forecolor for the tabs.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Color value.</returns>
        /// <remarks>
        /// This implementation returns SystemColors.WindowText.
        /// </remarks>
        public virtual Color DefaultTabForeColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return SystemColors.WindowText;
        }


        /// <summary>
        /// Returns the default active tab's color.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Color value.</returns>
        /// <remarks>
        /// This implementation returns the tab control's BackColor.
        /// </remarks>
        public virtual Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return tabControl.GetControl().BackColor;
        }
        /// <summary>
        /// Returns the default inactive tab's color.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Color value.</returns>
        /// <remarks>
        /// This implementation returns the tab control's BackColor.
        /// </remarks>
        public virtual Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return tabControl.GetControl().BackColor;
        }
        /// <summary>
        /// Returns the default single border color.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Color value.</returns>
        /// <remarks>
        /// This implementation returns a system color.
        /// </remarks>
        public virtual Color DefaultFixedSingleBorderColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return SystemColors.WindowText;
        }
        /// <summary>
        /// Returns the default tab panel font.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Font value.</returns>
        /// <remarks>
        /// This implementation returns the tab control's font.
        /// </remarks>
        public virtual Font DefaultTabPanelFont(ITabPanelData panelData, ITabControl tabControl)
        {
            return tabControl.GetControl().Font;
        }
        /// <summary>
        /// Returns the default inactive tab panel font.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Font value.</returns>
        /// <remarks>
        /// If the panelData's Font is not null, it is returned. If not, the default tab panel font is
        /// returned.
        /// </remarks>
        public virtual Font DefaultInactiveTabFont(ITabPanelData panelData, ITabControl tabControl)
        {
            if (panelData.Font != null)
                return panelData.Font;
            else
                return this.DefaultTabPanelFont(panelData, tabControl);
        }
        /// <summary>
        /// Returns the default active tab panel font.
        /// </summary>
        /// <param name="panelData">The tab panel data.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <returns>A Font value.</returns>
        /// <remarks>
        /// Returns the default inactive tab font, after making it bold.
        /// </remarks>
        public virtual Font DefaultActiveTabFont(ITabPanelData panelData, ITabControl tabControl)
        {
            return this.DefaultInactiveTabFont(panelData, tabControl); ;
        }
        /// <summary>
        /// Returns the overlap size.
        /// </summary>
        /// <returns>The overlap size.</returns>
        /// <remarks>This implementation returns (0, 0).</remarks>
        public virtual SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(0, 0);
        }

        /// <summary>
        /// Indicates whether this tab type should be made available in the design-time property grid for the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property.
        /// </summary>
        public virtual bool ShowInDesignMode
        {
            get { return true; }
        }

        /// <summary>
        /// Draws the background of the tab panel.
        /// </summary>
        /// <param name="tabControl">The parent <see cref="ITabControl"/> implementation.</param>
        /// <param name="g">The Graphics into which to draw.</param>
        /// <param name="bgColor">The background color.</param>
        /// <param name="bounds">The rectangular bounds of the tab panel.</param>
        public virtual void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            // Take a look at TabControlAdv.Init for notes on why we need this check.
            if (tabControl.GetControl().BackColor != bgColor)
                g.FillRectangle(new SolidBrush(bgColor), bounds);
        }
        /// <summary>
        /// Indicates whether the background color is solid.
        /// </summary>
        /// <returns>True if solid; false otherwise.</returns>
        public virtual bool IsBackgroundSolid()
        {
            return true;
        }
    }

    /// <summary>
    /// A default <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer"/> implementation
    /// from which your custom tab renderers could derive.
    /// </summary>
    public abstract class TabRendererBase : ITabRenderer
    {
        private bool currentlyAnimating = false;
        private const int c_ADJUST_WIDTH_ONE_NOTE_STYLE = 4;
        private const int c_ADJUST_WIDTH_DOCKING_WHIDBEY_BETA = 12;
        internal const int c_DRAW_TEXT_FLAGS = DrawTextFormats.DT_SINGLELINE;
        private GraphicsState m_savedState = null;
                
        protected virtual void SaveGraphicsState(Graphics g, ref RectangleF curBounds)
        {
            if (this.NeedRotateTextWhenVertical)
            {
                m_savedState = g.Save();
                g.ResetTransform();
                curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, curBounds, false);
            }
        }

        protected virtual void RestoreGraphicsState(Graphics g)
        {
            if (m_savedState != null)
            {
                g.Restore(m_savedState);
                m_savedState = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool NeedMirroredBackground()
        {
            bool bIsMirrored = panelRenderer.IsMirrored;

            if (bIsMirrored)
            {
                TabControlAdv tabControl = this.TabControl as TabControlAdv;

                if (tabControl != null && !tabControl.RotateTabsWhenRTL)
                {
                    bIsMirrored = false;
                }
            }

            return bIsMirrored;
        }

        private bool m_bShouldDrawText = true;
        private ITabData tabData;
        private bool hotTrack = false;
        private DrawItemCallback drawItemCallback;
        internal ITabControl parent;

        [Syncfusion.Documentation.DocumentationExclude()]
        protected ITabPanelRenderer panelRenderer;

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int ImageTextPadding = 2;
        private RectangleF bounds;	// based on horizontal alignment
        private TabAlignment tabAlignment;
        private RectangleF lastDrawnBounds = RectangleF.Empty;
        private SizeF cachedTextPrefSize = SizeF.Empty;
        private bool isTextShrunk = false;
        private bool visible = true;
        private bool m_bForceDrawImage = false;
        private RectangleF m_rectText;
        /// <summary>
        /// Special graphics for text measuring.
        /// </summary>
        private static Graphics m_measure;

        /// <summary>
        /// Get special measure graphics that allowing measuring without control creation.
        /// </summary>
        private static Graphics MeasureGraphics
        {
            get
            {
                if (m_measure == null)
                {
                    m_measure = Graphics.FromImage(new Bitmap(1, 1));
                }

                return m_measure;
            }
            set
            {
                if (m_measure != null)
                {
                    m_measure.Dispose();
                }

                m_measure = value;
            }
        }

        public bool ForceDrawImage
        {
            get
            {
                return m_bForceDrawImage;
            }
            set
            {
                if (m_bForceDrawImage != value)
                {
                    m_bForceDrawImage = value;
                }
            }
        }

        private Rectangle m_lastDrawnTextBounds = Rectangle.Empty;

        /// <summary>
        /// Returns last drawn text bounds.
        /// </summary>
        public Rectangle TextBounds
        {
            get
            {
                return m_lastDrawnTextBounds;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text should be drawn.
        /// </summary>
        public bool ShouldDrawText
        {
            get
            {
                return m_bShouldDrawText;
            }
            set
            {
                if (value != m_bShouldDrawText)
                {
                    m_bShouldDrawText = value;
                }
            }
        }

        /// <summary>
        /// Returns y-coordinate offset for Label Edit control,
        /// in Tab's captions editable case.
        /// </summary>
        public virtual int LabelEditOffsetY
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> associated with this tab.
        /// </summary>
        public virtual ITabData TabData
        {
            get
            {
                return tabData;
            }
            set
            {
                tabData = value;
            }
        }

        /// <summary>
        /// Indicates whether this tab should be drawn visible.
        /// </summary>
        public virtual bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        /// <summary>
        /// Indicates whether hot tracking is on.
        /// </summary>
        public bool HotTrack
        {
            get { return hotTrack; }
            set { if (hotTrack != value)hotTrack = value; }
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.DrawItemCallback"/>.
        /// </summary>
        public DrawItemCallback DrawItemCallback
        {
            get { return drawItemCallback; }
            set { drawItemCallback = value; }
        }

        /// <summary>
        /// Returns the parent <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/>.
        /// </summary>
        public ITabControl TabControl
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase"/>.
        /// </summary>
        /// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
        /// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
        public TabRendererBase(ITabControl parent, ITabPanelRenderer panelRenderer)
        {
            this.parent = parent;
            this.panelRenderer = panelRenderer;
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.TabPropertyChanged"/>.
        /// </summary>
        public virtual void TabPropertyChanged()
        {
            // Will delete cached data.
        }
        /// <summary>
        /// Indicates whether the text is shrunk.
        /// </summary>
        public virtual bool IsTextShrunk()
        {
            return this.isTextShrunk;
        }

        /// <summary>
        /// Indicates the border color of the tabs.
        /// </summary>
        public virtual Color TabBorderColor
        {
            get
            {
                return SystemColors.ControlDark;
            }
        }

        private const int DEF_CLOSE_BUTTON_SIZE = 7;
        private const int DEF_CLOSE_BUTTON_PADDING = 5;

        /// <summary>
        /// The value that indicates whether close button should be visible for each tab.
        /// </summary>
        private bool m_bShowCloseButton = false;

        /// <summary>
        /// Gets or sets the value whether close button should be visible for each tab.
        /// </summary>
        public bool ShowCloseButton
        {
            get
            {
                return m_bShowCloseButton;
            }
            set
            {
                if (value != m_bShowCloseButton)
                {
                    m_bShowCloseButton = value;
                }
            }
        }

        /// <summary>
        /// The value that indicates whether close button should be visible for each tab.
        /// </summary>
        private Color  closeButtonBackColor  = Color.White;

        /// <summary>
        /// Gets or sets the value whether close button should be visible for each tab.
        /// </summary>
        public Color CloseButtonBackColor
        {
            get
            {
                return closeButtonBackColor;
            }
            set
            {
                if (value != closeButtonBackColor)
                {
                    closeButtonBackColor = value;
                }
            }
        }

        /// <summary>
        /// The value that indicates whether close button should be visible for each tab.
        /// </summary>
        private bool m_bShowCloseButtonBackColor = false;

        /// <summary>
        /// Gets or sets the value whether close button should be visible for each tab.
        /// </summary>
        public bool ShowCloseButtonBackColor
        {
            get
            {
                return m_bShowCloseButtonBackColor;
            }
            set
            {
                if (value != m_bShowCloseButtonBackColor)
                {
                    m_bShowCloseButtonBackColor = value;
                }
            }
        }
        private bool isMetro = false;

        /// <summary>
        /// Gets or sets the value of TabStyle name.
        /// </summary>
        internal bool IsMetro
        {
            get
            {
                return isMetro;
            }
            set
            {
                if (value != isMetro)
                {
                    isMetro = value;
                }
            }
        }
        /// <summary>
        /// Bounds of the close button.
        /// </summary>
        private Rectangle m_lastCloseButtonBounds = Rectangle.Empty;

        /// <summary>
        /// Gets the bounds of the close button.
        /// </summary>
        public Rectangle CloseButtonBounds
        {
            get
            {
                return m_lastCloseButtonBounds;
            }
        }

        /// <summary>
        /// The value that indicates whether the close button is clicked.
        /// </summary>
        private bool m_bIsCloseButtonClicked = false;

        /// <summary>
        /// Gets or sets the value whether close button is clicked.
        /// </summary>
        public virtual bool CloseButtonClicked
        {
            get
            {
                return m_bIsCloseButtonClicked;
            }
            set
            {
                if (m_bIsCloseButtonClicked != value)
                    m_bIsCloseButtonClicked = value;
            }
        }

        /// <summary>
        /// Checks, does CloseButton's bounds contain specified point.
        /// </summary>
        /// <param name="pt">Point to check.</param>
        /// <returns> true, if CloseButton's bounds contain specified point, otherwise - false. </returns>
        public virtual bool CloseButtonHitTest(Point pt)
        {
            return m_lastCloseButtonBounds.Contains(pt);
        }

        /// <summary>
        /// Gets CloseButton size.
        /// </summary>
        public virtual int CloseButtonSize
        {
            get
            {
                return DEF_CLOSE_BUTTON_SIZE;
            }
        }

        /// <summary>
        /// Gets correct close button point.
        /// </summary>
        protected virtual Point CorrectCloseButtonPosition
        {
            get
            {
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets space( in pixels ) between tab's interior and close button.
        /// </summary>
        public virtual int CloseButtonPadding
        {
            get
            {
                return DEF_CLOSE_BUTTON_PADDING;
            }
        }

        /// <summary>
        /// The value that indicates whether mouse is over the close button.
        /// </summary>
        private bool m_bHitCloseButton = false;

        /// <summary>
        /// Gets or sets the value whether mouse is over the close button.
        /// </summary>
        public virtual bool HitCloseButton
        {
            get
            {
                return m_bHitCloseButton;
            }
            set
            {
                if (m_bHitCloseButton != value)
                    m_bHitCloseButton = value;
            }
        }
        protected virtual void DrawCloseButton(Graphics g, RectangleF closeButtonBounds)
        {
            DrawCloseButton(g, closeButtonBounds, null);
        }

        /// <summary>
        /// Draws the close button.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="closeButtonBounds">The bounds of the close button.</param>
        protected virtual void DrawCloseButton(Graphics g, RectangleF closeButtonBounds, DrawTabEventArgs e)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if ((int)closeButtonBounds.Width >= this.CloseButtonSize &&
                (int)closeButtonBounds.Height >= this.CloseButtonSize)
            {
                GraphicsState savedState = null;

                if (ShouldDrawRotatedWhenVertical)
                {
                    savedState = g.Save();
                    g.ResetTransform();
                }

                closeButtonBounds.X += CorrectCloseButtonPosition.X;
                closeButtonBounds.Y += CorrectCloseButtonPosition.Y;

                closeButtonBounds.Height--;
                Rectangle rect = Rectangle.Ceiling(closeButtonBounds);
                Pen pen;
                if (this.parent is TabControlAdv)
                {


                    if ((this.parent as TabControlAdv).TabStyle == typeof(TabRendererMetro))
                    {
                        IsMetro = true;
                    }
                    else
                    {
                        IsMetro = false;
                    }
                    if (e != null)
                    {
                       if(IsMetro && e.Font.Bold )
                        pen = new Pen(Color.FromArgb(200, Color.White));
                       else
                           pen = new Pen(Color.FromArgb(200, Color.Black));
                    }
                    else
                        pen = new Pen(Color.FromArgb(200, Color.Black));
                }
                else
                    pen = new Pen(Color.FromArgb(200, Color.Black));


                if (this.HitCloseButton && !IsMetro)
                {
                    SolidBrush brush = new SolidBrush(CloseButtonBackColor);
                    g.FillRectangle(brush, rect.X - 2, rect.Y - 2, rect.Width + 5, rect.Height + 5);
                    brush.Dispose();
                }
                g.DrawLine(pen, rect.X + 1, rect.Y,
                    rect.X + rect.Width, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X, rect.Y,
                    rect.X + rect.Width - 1, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X + rect.Width - 1, rect.Y,
                    rect.X, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X + rect.Width, rect.Y,
                    rect.X + 1, rect.Y + rect.Height);
                pen.Dispose();
                if (this.HitCloseButton && !IsMetro)
                {
                    DrawCloseButtonBorder(g, closeButtonBounds);
                }

                if (savedState != null)
                {
                    g.Restore(savedState);
                }

                m_lastCloseButtonBounds = Rectangle.Round(closeButtonBounds);
            }
        }

        /// <summary>
        /// Draws the close button border.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="closeButtonBounds">The bounds of the close button.</param>
        private void DrawCloseButtonBorder(Graphics g, RectangleF closeButtonBounds)
        {
            RectangleF rect = closeButtonBounds;
            rect.Inflate(3, 3);
            rect.Height++;
            rect.Width++;

            using (Bitmap bmp = new Bitmap((int)rect.Width, (int)rect.Height))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    ControlPaint.DrawBorder3D(gr, new Rectangle(0, 0, (int)rect.Width, (int)rect.Height),
                        this.CloseButtonClicked ? Border3DStyle.Sunken : Border3DStyle.Raised,
                        Border3DSide.Top | Border3DSide.Left | Border3DSide.Right | Border3DSide.Bottom);
                }

                if (panelRenderer != null && panelRenderer.TabPanelData != null &&
                    (panelRenderer.TabPanelData.Alignment == TabAlignment.Bottom ||
                    (panelRenderer.TabPanelData.Alignment == TabAlignment.Right && !this.panelRenderer.TabPanelData.RotateTextWhenVertical)))
                {
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                g.DrawImage(bmp, rect.X, rect.Y);
            }
        }

        /// <summary>
        /// Gets the close button bounds.
        /// </summary>
        protected virtual RectangleF GetCloseButtonBounds(DrawTabEventArgs drawItemInfo)
        {
            TabPanelData panelData = panelRenderer.TabPanelData as TabPanelData;

            RectangleF closeButtonBounds = Rectangle.Empty;

            if (ShowCloseButton)
            {
                Graphics g = drawItemInfo.Graphics;
                SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);

                RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment,
                    drawItemInfo.BoundsInterior, true);
                rectTextAndImage.Inflate(-(float)Math.Ceiling(overlapSize.Width / 2), 0);

                closeButtonBounds.Size = new SizeF(CloseButtonSize, CloseButtonSize);

                RectangleF interiorBounds = drawItemInfo.BoundsInterior;
                interiorBounds.Inflate(-(float)Math.Ceiling(overlapSize.Width / 2), 0);

                Rectangle itemBounds = Rectangle.Round(TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true));
                if (this.ShouldDrawRotatedWhenVertical)
                {
                    itemBounds = drawItemInfo.Bounds;
                }

                Rectangle buttonRect = Rectangle.Empty;

                if (ShouldDrawRotatedWhenVertical)
                {
                    if (this.TabAlignment == TabAlignment.Right)
                    {
                        if (panelRenderer.IsMirrored)
                        {
                            closeButtonBounds.Location = new PointF(interiorBounds.Right +
                                CloseButtonPadding, interiorBounds.Top);
                        }
                        else
                        {
                            closeButtonBounds.Location = interiorBounds.Location;
                            closeButtonBounds.Offset(-(CloseButtonSize + CloseButtonPadding) + 1, 0);
                        }
                    }
                    else if (this.TabAlignment == TabAlignment.Left)
                    {
                        if (panelRenderer.IsMirrored)
                        {
                            closeButtonBounds.Location = interiorBounds.Location;
                            closeButtonBounds.Offset(-(CloseButtonSize + CloseButtonPadding) + 1, 0);
                        }
                        else
                        {
                            closeButtonBounds.Location = new PointF(interiorBounds.Right +
                                CloseButtonPadding, interiorBounds.Top);
                        }
                    }

                    buttonRect = Rectangle.Ceiling(interiorBounds);
                }
                else
                {
                    if (this.panelRenderer.IsMirrored)
                    {
                        closeButtonBounds.Location = rectTextAndImage.Location;
                        closeButtonBounds.Offset(-(CloseButtonSize + CloseButtonPadding), 0);
                    }
                    else
                    {
                        closeButtonBounds.Location = new PointF(rectTextAndImage.Right +
                            CloseButtonPadding,
                            rectTextAndImage.Top);
                    }

                    buttonRect = Rectangle.Ceiling(rectTextAndImage);
                }

                closeButtonBounds.Height = Math.Min(closeButtonBounds.Height, buttonRect.Height);
                closeButtonBounds.Y = (buttonRect.Height - closeButtonBounds.Height) / 2 + buttonRect.Y;

                closeButtonBounds.Intersect(itemBounds);
            }

            return closeButtonBounds;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldDrawRotatedWhenVertical
        {
            get
            {
                return (this.panelRenderer.TabPanelData.RotateTextWhenVertical &&
                    (this.TabAlignment == TabAlignment.Left || this.TabAlignment ==
                    TabAlignment.Right));
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual RectangleF CorrectInteriorBounds(RectangleF rectTextAndImage)
        {
            TabPanelData panelData = this.panelRenderer.TabPanelData as TabPanelData;

            if (ShowCloseButton)
            {
                int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;
                bool bRotateTextWhenVertical = ShouldDrawRotatedWhenVertical;

                if (bRotateTextWhenVertical)
                {
                    rectTextAndImage.Height -= closeButtonOffset;
                }
                else
                {
                    rectTextAndImage.Width -= closeButtonOffset;
                }
                if (this.panelRenderer.IsMirrored)
                {
                    if (bRotateTextWhenVertical)
                    {
                        rectTextAndImage.Offset(0, closeButtonOffset);
                    }
                    else
                    {
                        rectTextAndImage.Offset(closeButtonOffset, 0);
                    }
                }
            }

            return rectTextAndImage;
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.GetPreferredSize"/>.
        /// </summary>
        ///
        private delegate Font CFnGetTabFont(ITabPanelData a, ITabControl b);

        private const string DEF_MEASURE_STR = "X";

        /// <summary>
        /// Returns the preferred size for the tabs.
        /// </summary>
        public virtual SizeF GetPreferredSize(Graphics g)
        {
            return GetItemPreferredSize(g);
        }

        protected virtual SizeF GetItemPreferredSize(Graphics g)
        {
            ITabPanelData itpdPanelData = panelRenderer.TabPanelData;

            // Compute the size based on tab properties
            // Compute assuming horizontal tab alignment.
            // Get Text size
            SizeF preferredSize = SizeF.Empty;

            string measuredString = (tabData.Text == string.Empty) ? DEF_MEASURE_STR :
                tabData.Text;

            SizeF[] aSizes = new SizeF[2];
            for (int nPass = 0; nPass < 2; ++nPass)
            {
                bool bInactive = (0 == nPass);
                Font tabFont = (bInactive ? tabData.Font : itpdPanelData.ActiveTabFont);
                if (tabFont == null)
                {
                    tabFont = this.GetTabFont(bInactive);
                }

                aSizes[nPass] = MeasureText(g, measuredString, tabFont);
                aSizes[nPass] = Size.Ceiling(aSizes[nPass]);
            }

            preferredSize.Width += Math.Max(aSizes[0].Width, aSizes[1].Width);
            preferredSize.Height += Math.Max(aSizes[0].Height, aSizes[1].Height);

            if (tabData.Text == string.Empty)
            {
                preferredSize.Width = 0;
            }

            this.cachedTextPrefSize = preferredSize;

            TabPanelData tabPanelData = panelRenderer.TabPanelData as TabPanelData;

            if (ShowCloseButton)
            {
                preferredSize.Width += this.CloseButtonSize + this.CloseButtonPadding;
            }

            bool bVerticalAlignment = tabPanelData.Alignment == TabAlignment.Right ||
                tabPanelData.Alignment == TabAlignment.Left;

            Size imageSize = (itpdPanelData.ImageList != null ? itpdPanelData.ImageList.ImageSize : Size.Empty);

            // Rotate ImageSize based on alignment (Images will be drawn straight whatever the alignment is.
            if (this.tabAlignment == TabAlignment.Left || this.tabAlignment == TabAlignment.Right)
            {
                imageSize = new Size(imageSize.Height, imageSize.Width);

                if (itpdPanelData.RotateTextWhenVertical)
                {
                    preferredSize = new SizeF(preferredSize.Height, preferredSize.Width);
                }
            }

            if ((!imageSize.IsEmpty && tabData.Image==null) ||(!imageSize.IsEmpty&& tabData.ImageIndex != -1))
            {
                if ((itpdPanelData.ImageAlignmentR == RelativeImageAlignment.LeftOfText ||
                    itpdPanelData.ImageAlignmentR == RelativeImageAlignment.RightOfText))
                {
                    if (itpdPanelData.RotateTextWhenVertical && bVerticalAlignment)
                    {
                        preferredSize.Height += TabRendererBase.ImageTextPadding;
                        preferredSize.Height += imageSize.Height;

                        if (tabData.ImageIndex != -1 && !(this.parent is BackStage))
                        {
                            preferredSize.Width = Math.Max(preferredSize.Width, imageSize.Width);
                        }
                    }
                    else
                    {
                        if (tabData.ImageIndex != -1)
                        {
                            preferredSize.Width += TabRendererBase.ImageTextPadding;
                            preferredSize.Width += imageSize.Width;
                        }
                        preferredSize.Height = Math.Max(preferredSize.Height, imageSize.Height);
                    }
                }
                else if ((itpdPanelData.ImageAlignmentR == RelativeImageAlignment.AboveText ||
                    itpdPanelData.ImageAlignmentR == RelativeImageAlignment.BelowText))
                {
                    if (itpdPanelData.RotateTextWhenVertical && bVerticalAlignment)
                    {
                        if (tabData.ImageIndex != -1)
                        {
                            preferredSize.Width += TabRendererBase.ImageTextPadding;
                            preferredSize.Width += imageSize.Width;
                        }
                        preferredSize.Height = Math.Max(preferredSize.Height, imageSize.Height);
                    }
                    else
                    {
                        preferredSize.Height += TabRendererBase.ImageTextPadding;
                        preferredSize.Height += imageSize.Height;

                        if (tabData.ImageIndex != -1)
                        {
                            preferredSize.Width = Math.Max(preferredSize.Width, imageSize.Width);
                        }
                    }
                }
                else if (itpdPanelData.ImageAlignmentR == RelativeImageAlignment.Overlap)
                {
                    preferredSize.Height = Math.Max(preferredSize.Height, imageSize.Height);

                    if (tabData.ImageIndex != -1)
                    {
                        preferredSize.Width = Math.Max(preferredSize.Width, imageSize.Width);
                    }
                }
            }
            else if (tabData.Image != null)
            {
                if ((itpdPanelData.ImageAlignmentR == RelativeImageAlignment.LeftOfText ||
                    itpdPanelData.ImageAlignmentR == RelativeImageAlignment.RightOfText))
                {
                    if (itpdPanelData.RotateTextWhenVertical && bVerticalAlignment)
                    {
                        preferredSize.Height += TabRendererBase.ImageTextPadding;
                        preferredSize.Height += tabData.ImageSize.Height;
                       
                        preferredSize.Width = Math.Max(preferredSize.Width, tabData.ImageSize.Width);
                    }
                    else
                    {
                        preferredSize.Width += TabRendererBase.ImageTextPadding;
                        preferredSize.Width += tabData.ImageSize.Width;
                        preferredSize.Height = Math.Max(preferredSize.Height, tabData.ImageSize.Height);
                    }
                }
                else if ((itpdPanelData.ImageAlignmentR == RelativeImageAlignment.AboveText ||
                    itpdPanelData.ImageAlignmentR == RelativeImageAlignment.BelowText))
                {
                    if (itpdPanelData.RotateTextWhenVertical && bVerticalAlignment)
                    {
                        preferredSize.Width += TabRendererBase.ImageTextPadding;
                        preferredSize.Width += tabData.ImageSize.Width;
                        preferredSize.Height = Math.Max(preferredSize.Height, tabData.ImageSize.Height);
                    }
                    else
                    {
                        preferredSize.Height += TabRendererBase.ImageTextPadding;
                        preferredSize.Height += tabData.ImageSize.Height;

                        preferredSize.Width = Math.Max(preferredSize.Width, tabData.ImageSize.Width);
                    }
                }
                else if (itpdPanelData.ImageAlignmentR == RelativeImageAlignment.Overlap)
                {
                    preferredSize.Height = Math.Max(preferredSize.Height, tabData.ImageSize.Height);

                    preferredSize.Width = Math.Max(preferredSize.Width, tabData.ImageSize.Width);
                }
            }

            preferredSize = this.CorrectPreferredSize(preferredSize);

            // for overlapping border
            preferredSize.Height += 1;
            if(this.parent is BackStage )
            preferredSize.Width += 7;
            // For borders
            preferredSize += new SizeF(2, 2);

            return preferredSize;
        }

        protected Font GetTabFont(bool isInactive)
        {
            ITabPanelData itpdPanelData = panelRenderer.TabPanelData;
            ITabDefaultProperties defaultProperties = TabRendererFactory.GetRegisteredExtender(itpdPanelData.TabStyle);

            CFnGetTabFont pfnGetTabFont = isInactive ?
                        new CFnGetTabFont(defaultProperties.DefaultInactiveTabFont) :
                        new CFnGetTabFont(defaultProperties.DefaultActiveTabFont);
            return pfnGetTabFont(itpdPanelData, parent);
        }

        private Icon Icon
        {
            get
            {
                Icon icon = null;

                MDIChildTabData mdiChildData = this.TabData as MDIChildTabData;

                if (mdiChildData != null && mdiChildData.Icon != null)
                {
                    icon = mdiChildData.Icon;
                }

                return icon;
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual SizeF CorrectPreferredSize(SizeF preferredSize)
        {
            ITabPanelData panelData = null;
            if (this.panelRenderer != null)
            {
                panelData = this.panelRenderer.TabPanelData;
                if (panelData != null)
                {
                    preferredSize.Width += (panelData.Padding.X * 2);
                    preferredSize.Height += (panelData.Padding.Y * 2);
                }
            }

            return preferredSize;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool NeedRotateTextWhenVertical
        {
            get
            {
                return ((this.TabAlignment == TabAlignment.Left ||
                    this.TabAlignment == TabAlignment.Right) && (this.panelRenderer != null &&
                    this.panelRenderer.TabPanelData != null &&
                    this.panelRenderer.TabPanelData.RotateTextWhenVertical));
            }
        }
        /// <summary>
        /// Returns the position where the text should be drawn given a layout rectangle and the Graphics context.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The font with which to draw.</param>
        /// <param name="rectLayout">The layout rectangle.</param>
        /// <param name="stringformat">The <see cref="System.Drawing.StringFormat"/>.</param>
        /// <returns>The position where the text should be drawn.</returns>
        public virtual RectangleF GetTextPosition(Graphics g, string text, Font font,
            RectangleF rectLayout, StringFormat stringformat)
        {
            ITabPanelData itpdPanelData = panelRenderer.TabPanelData;
            if (itpdPanelData.TabStyle == TabRendererVS2008.TabStyleName && !this.NeedRotateTextWhenVertical)
            {
                rectLayout.Height -= (this.TabControl.BorderWidth - 5);
            }

            SizeF sizeRect;
            if (tabAlignment == TabAlignment.Top)
            {
                sizeRect = MeasureText(g, text, font);
            }
            else
            {
                GraphicsState oldState = g.Save();
                g.ResetTransform();

                sizeRect = MeasureText(g, text, font);

                g.Restore(oldState);

                if (this.tabAlignment != TabAlignment.Bottom && this.panelRenderer.TabPanelData.RotateTextWhenVertical)
                    sizeRect = new SizeF(sizeRect.Height, sizeRect.Width);
            }
            // Ceil it
            sizeRect = Size.Ceiling(sizeRect);

            // MeasureString won't give me the exact position, though it knows it, hence calculating manually
            float fRectY = rectLayout.Y;
            float fRectX = rectLayout.X;
            bool bTextWidthIsLarger = (sizeRect.Width > rectLayout.Width);

            switch (stringformat.Alignment)
            {
                case StringAlignment.Far:
                    if (!bTextWidthIsLarger)
                    {
                        if (itpdPanelData.TabStyle != TabRendererOffice2003.TabStyleName)
                            fRectX += rectLayout.Width - sizeRect.Width;
                    }

                    break;
                case StringAlignment.Center:
                default:
                    if (!bTextWidthIsLarger)
                    {
                        fRectX += (float)Math.Ceiling((rectLayout.Width - sizeRect.Width) / 2.0F);
                    }

                    break;
            }

            bool bTextHeightIsLarger = (sizeRect.Height > rectLayout.Height);

            switch (stringformat.LineAlignment)
            {
                case StringAlignment.Far:
                    if (!bTextHeightIsLarger)
                    {
                            fRectY += rectLayout.Height - sizeRect.Height;
                    }
                    break;
                case StringAlignment.Center:
                    if (!bTextHeightIsLarger)
                    {
                        fRectY += (float)Math.Ceiling((rectLayout.Height - sizeRect.Height) / 2.0F);
                    }
                    break;
            }

            RectangleF rectText = new RectangleF(fRectX, fRectY,
                            sizeRect.Width < rectLayout.Width ? sizeRect.Width : rectLayout.Width,
                            sizeRect.Height < rectLayout.Height ? sizeRect.Height : rectLayout.Height);

            return rectText;
        }

        /// <summary>
        /// Adjusts image Y position when RelativeImageAlignment is RightOfText or LeftOfText.
        /// </summary>
        /// <param name="recvtImage"></param>
        /// <param name="rectTextAndImage"></param>
        private void AdjustImageRectYPos(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saLineAlign = itpdTabPanelData.TextLineAlignment;

            switch (saLineAlign)
            {
                case StringAlignment.Center:
                    {
                        rectImage.Y = m_rectText.Top + (m_rectText.Height - rectImage.Height) / 2F;
                        break;
                    }
                case StringAlignment.Near:
                    {
                        rectImage.Y = rectTextAndImage.Top;
                        break;
                    }
                case StringAlignment.Far:
                    {
                        rectImage.Y = rectTextAndImage.Bottom - rectImage.Height;
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image X position when RelativeImageAlignment is AboveText or BelowText.
        /// </summary>
        /// <param name="recvtImage"></param>
        /// <param name="rectTextAndImage"></param>
        private void AdjustImageRectXPos(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saTextAlign = itpdTabPanelData.TextAlignment;

            switch (saTextAlign)
            {
                case StringAlignment.Center:
                    {
                        rectImage.X = m_rectText.Left + (m_rectText.Width - rectImage.Width) / 2F;
                        break;
                    }
                case StringAlignment.Near:
                    {
                        rectImage.X = rectTextAndImage.Left;
                        break;
                    }
                case StringAlignment.Far:
                    {
                        rectImage.X = m_rectText.Right - m_rectText.Width;
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image bounds when RelativeImageAlignment is AboveText.
        /// </summary>
        /// <param name="rectImage"></param>
        private void AdjustImageRectAboveText(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saTextAlign = itpdTabPanelData.TextAlignment;
            StringAlignment saLineAlign = itpdTabPanelData.TextLineAlignment;

            switch (saLineAlign)
            {
                case StringAlignment.Near:
                    {
                        m_rectText.Offset(0F, rectImage.Height + TabRendererBase.ImageTextPadding);
                        rectImage.Y = rectTextAndImage.Top;
                        break;
                    }
                case StringAlignment.Far:
                    {
                        rectImage.Y = m_rectText.Top - rectImage.Height - TabRendererBase.ImageTextPadding;
                        break;
                    }
                case StringAlignment.Center:
                    {
                        m_rectText.Offset(0F, (rectImage.Height + TabRendererBase.ImageTextPadding) / 2F);
                        rectImage.Y = m_rectText.Top - rectImage.Height - TabRendererBase.ImageTextPadding;
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image bounds when RelativeImageAlignment is BelowText.
        /// </summary>
        /// <param name="rectImage"></param>
        private void AdjustImageRectBelowText(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saTextAlign = itpdTabPanelData.TextAlignment;
            StringAlignment saLineAlign = itpdTabPanelData.TextLineAlignment;

            switch (saLineAlign)
            {
                case StringAlignment.Near:
                    {
                        rectImage.Y = m_rectText.Bottom + TabRendererBase.ImageTextPadding;
                        break;
                    }
                case StringAlignment.Far:
                    {
                        m_rectText.Offset(0F, -(rectImage.Height - TabRendererBase.ImageTextPadding));
                        rectImage.Y = rectTextAndImage.Bottom - rectImage.Height;
                        break;
                    }
                case StringAlignment.Center:
                    {
                        m_rectText.Offset(0F, -((rectImage.Height + TabRendererBase.ImageTextPadding) / 2F));
                        rectImage.Y = m_rectText.Bottom + TabRendererBase.ImageTextPadding;
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image bounds when RelativeImageAlignment is LeftOfText.
        /// </summary>
        /// <param name="rectImage"></param>
        private void AdjustImageRectLeftOfText(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saTextAlign = itpdTabPanelData.TextAlignment;
            StringAlignment saLineAlign = itpdTabPanelData.TextLineAlignment;
            bool bIsMirrored = panelRenderer.IsMirrored;
            int nOffsetSign = bIsMirrored ? -1 : 1;

            switch (saTextAlign)
            {
                case StringAlignment.Near:
                    {
                        if (bIsMirrored)
                        {
                            rectImage.X = rectTextAndImage.Right - rectImage.Width;
                            m_rectText.X = rectImage.Left - TabRendererBase.ImageTextPadding - m_rectText.Width;
                        }
                        else
                        {
                            rectImage.X = rectTextAndImage.Left;
                            m_rectText.X = rectImage.Right + TabRendererBase.ImageTextPadding;
                        }
                        break;
                    }
                case StringAlignment.Far:
                    {
                        if (bIsMirrored)
                        {
                            m_rectText.X = rectTextAndImage.Left;
                            rectImage.X = m_rectText.Right + TabRendererBase.ImageTextPadding;
                        }
                        else
                        {
                            m_rectText.X = rectTextAndImage.Right - m_rectText.Width;
                            rectImage.X = m_rectText.Left - TabRendererBase.ImageTextPadding - rectImage.Width;
                        }
                        break;
                    }
                case StringAlignment.Center:
                    {
                        float fImageLeftOverflow = 0.0F;

                        m_rectText.Offset(nOffsetSign * (rectImage.Width + TabRendererBase.ImageTextPadding) / 2F, 0F);

                        if (bIsMirrored)
                        {
                            rectImage.X = m_rectText.Right + TabRendererBase.ImageTextPadding;
                            fImageLeftOverflow = rectImage.Right - rectTextAndImage.Right;
                        }
                        else
                        {
                            rectImage.X = m_rectText.Left - rectImage.Width - TabRendererBase.ImageTextPadding;
                            fImageLeftOverflow = rectTextAndImage.Left - rectImage.Left;
                        }

                        if (fImageLeftOverflow > 0.0F)
                        {
                            float fShift = nOffsetSign * fImageLeftOverflow;
                            rectImage.X += fShift;
                            m_rectText.X += fShift;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image bounds when RelativeImageAlignment is RightOfText.
        /// </summary>
        /// <param name="rectImage"></param>
        private void AdjustImageRectRightOfText(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            StringAlignment saTextAlign = itpdTabPanelData.TextAlignment;
            StringAlignment saLineAlign = itpdTabPanelData.TextLineAlignment;
            bool bIsMirrored = panelRenderer.IsMirrored;
            int nOffsetSign = bIsMirrored ? -1 : 1;

            switch (saTextAlign)
            {
                case StringAlignment.Near:
                    {
                        if (bIsMirrored)
                        {
                            m_rectText.X = rectTextAndImage.Right - m_rectText.Width;
                            rectImage.X = m_rectText.Left - TabRendererBase.ImageTextPadding - rectImage.Width;
                        }
                        else
                        {
                            m_rectText.X = rectTextAndImage.Left;
                            rectImage.X = m_rectText.Right + TabRendererBase.ImageTextPadding;
                        }
                        break;
                    }
                case StringAlignment.Far:
                    {
                        if (bIsMirrored)
                        {
                            rectImage.X = rectTextAndImage.Left;
                            m_rectText.X = rectImage.Right + TabRendererBase.ImageTextPadding;
                        }
                        else
                        {
                            rectImage.X = rectTextAndImage.Right - rectImage.Width;
                            m_rectText.X = rectImage.Left - TabRendererBase.ImageTextPadding - m_rectText.Width;
                        }
                        break;
                    }
                case StringAlignment.Center:
                    {
                        float fTextLeftOverflow = 0.0F;
                        m_rectText.Offset(-nOffsetSign * (rectImage.Width + TabRendererBase.ImageTextPadding) / 2F, 0F);

                        if (bIsMirrored)
                        {
                            rectImage.X = m_rectText.Left - TabRendererBase.ImageTextPadding - rectImage.Width;
                            fTextLeftOverflow = m_rectText.Right - rectTextAndImage.Right;
                        }
                        else
                        {
                            rectImage.X = m_rectText.Right + TabRendererBase.ImageTextPadding;
                            fTextLeftOverflow = rectTextAndImage.Left - m_rectText.Left;
                        }

                        if (fTextLeftOverflow > 0.0F)
                        {
                            float fShift = nOffsetSign * fTextLeftOverflow;
                            rectImage.X += fShift;
                            m_rectText.X += fShift;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Adjusts image bounds when RelativeImageAlignment is Overlap.
        /// </summary>
        /// <param name="rectImage"></param>
        private void AdjustImageRectOverlap(ref RectangleF rectImage, RectangleF rectTextAndImage)
        {
            rectImage.X = rectTextAndImage.Left;
            rectImage.Y = rectTextAndImage.Top;

            if (rectTextAndImage.Width > rectImage.Width)
            {
                rectImage.Offset((float)Math.Ceiling((rectTextAndImage.Width - rectImage.Width) / 2), 0);
            }
            if (rectTextAndImage.Height > rectImage.Height)
            {
                rectImage.Offset(0, (float)Math.Ceiling((rectTextAndImage.Height - rectImage.Height) / 2));
            }
        }

        /// <summary>
        /// Gets the image rectangle.
        /// </summary>
        /// <param name="imgImage"></param>
        /// <returns></returns>
        private RectangleF GetImageRectangle(RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            RelativeImageAlignment riaImgAlign = itpdTabPanelData.ImageAlignmentR;
            bool bVerticalAlignment = itpdTabPanelData.Alignment == TabAlignment.Right ||
                itpdTabPanelData.Alignment == TabAlignment.Left;
            
                ImageList ilImgList = itpdTabPanelData.ImageList;
                Size sizeImage = (null != ilImgList ? ilImgList.ImageSize : Size.Empty);
            

            // Rotate ImageSize based on alignment.
            if (tabAlignment == TabAlignment.Left || tabAlignment == TabAlignment.Right)
            {
                sizeImage = new Size(sizeImage.Height, sizeImage.Width);
            }

            RectangleF rectImage = new RectangleF(new PointF(0, 0), sizeImage);

            switch (riaImgAlign)
            {
                case RelativeImageAlignment.AboveText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (itpdTabPanelData.Alignment == TabAlignment.Bottom)
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.BelowText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (itpdTabPanelData.Alignment == TabAlignment.Bottom)
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.LeftOfText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.RightOfText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.Overlap:
                    {
                        AdjustImageRectOverlap(ref rectImage, rectTextAndImage);
                        break;
                    }
            }

            return rectImage;
        }

        /// <summary>
        /// Gets the image rectangle.
        /// </summary>
        /// <param name="imgImage"></param>
        /// <returns></returns>
        private RectangleF GetAnimateImageRectangle(RectangleF rectTextAndImage)
        {
            ITabPanelData itpdTabPanelData = this.panelRenderer.TabPanelData;
            RelativeImageAlignment riaImgAlign = itpdTabPanelData.ImageAlignmentR;
            bool bVerticalAlignment = itpdTabPanelData.Alignment == TabAlignment.Right ||
                itpdTabPanelData.Alignment == TabAlignment.Left;
            
            Size sizeImage = (null != tabData.Image ? tabData.ImageSize : Size.Empty);
      
            // Rotate ImageSize based on alignment.
            if (tabAlignment == TabAlignment.Left || tabAlignment == TabAlignment.Right)
            {
                sizeImage = new Size(sizeImage.Height, sizeImage.Width);
            }

            RectangleF rectImage = new RectangleF(new PointF(0, 0), sizeImage);

            switch (riaImgAlign)
            {
                case RelativeImageAlignment.AboveText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (itpdTabPanelData.Alignment == TabAlignment.Bottom)
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.BelowText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (itpdTabPanelData.Alignment == TabAlignment.Bottom)
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.LeftOfText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.RightOfText:
                    {
                        if (itpdTabPanelData.RotateTextWhenVertical && bVerticalAlignment)
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectAboveText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectBelowText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectXPos(ref rectImage, rectTextAndImage);
                        }
                        else
                        {
                            if (tabAlignment != TabAlignment.Left)
                            {
                                AdjustImageRectRightOfText(ref rectImage, rectTextAndImage);
                            }
                            else
                            {
                                AdjustImageRectLeftOfText(ref rectImage, rectTextAndImage);
                            }

                            AdjustImageRectYPos(ref rectImage, rectTextAndImage);
                        }
                        break;
                    }
                case RelativeImageAlignment.Overlap:
                    {
                        AdjustImageRectOverlap(ref rectImage, rectTextAndImage);
                        break;
                    }
            }

            return rectImage;
        }

        /// <summary>
        /// Draws the focus rectangle in the specified rectangle.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="focusRect">The layout rectangle.</param>
        /// <param name="fore">The forecolor with which to draw.</param>
        /// <param name="back">The backcolor with which to draw.</param>
        protected virtual void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            ControlPaint.DrawFocusRectangle(g, Rectangle.Round(focusRect), fore, back);
        }
        bool isBackStage = false;
        /// <summary>
        /// Draws the text and image of the tab, given the context.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="rectTextAndImage">The layout rectangle.</param>
        /// <param name="e">The object that has information regarding this tab and paint event.</param>
        protected virtual void DrawTextAndImage(Graphics g, RectangleF rectTextAndImage, DrawTabEventArgs e)
        {
            if (rectTextAndImage.Width < 0 || rectTextAndImage.Height < 0)
                return;

            bool bEnabled = ((int)e.State & (int)DrawItemState.Disabled) <= 0;
            bool bIsMirrored = panelRenderer.IsMirrored;

            ITabPanelData itpdTabPanelData = panelRenderer.TabPanelData;
            isBackStage = false;
            // Determine text position
            using (StringFormat sfFormat = new StringFormat())
            {
                sfFormat.Alignment = itpdTabPanelData.TextAlignment;
                sfFormat.LineAlignment = itpdTabPanelData.TextLineAlignment;
                if (this.TabControl is BackStage)
                {
                    isBackStage = true;
                    sfFormat.LineAlignment = StringAlignment.Near;
                }
                if (TabControl.UseMnemonic)
                {
                    sfFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                }
                else
                {
                    sfFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                }

                sfFormat.FormatFlags = StringFormatFlags.NoWrap;
                if (bIsMirrored)
                {
                    sfFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                }

                if (TabRendererFactory.GetRegisteredExtender(itpdTabPanelData.TabStyle).DrawEllipsis)
                {
                    sfFormat.Trimming = StringTrimming.EllipsisCharacter;
                }

                m_rectText = GetTextPosition(g, tabData.Text, e.Font, rectTextAndImage, sfFormat);

                int nImgIndex = tabData.ImageIndex;
                ImageList ilImgList = itpdTabPanelData.ImageList;
                Image imgImage = null;
                if (nImgIndex != -1 && ilImgList != null && ilImgList.Images.Count > nImgIndex)
                {
                    imgImage =(Image) ilImgList.Images[nImgIndex];
                }
                Size sizeImage = (null != ilImgList) ? ilImgList.ImageSize : Size.Empty;

                // Rotate ImageSize based on alignment (Images will be drawn straight whatever the alignment is.
                if (tabAlignment == TabAlignment.Left || tabAlignment == TabAlignment.Right)
                {
                    sizeImage = new Size(sizeImage.Height, sizeImage.Width);
                }

                RelativeImageAlignment riaImgAlign = itpdTabPanelData.ImageAlignmentR;
                TabAlignment taTabAlign = itpdTabPanelData.Alignment;

                RectangleF rectImage = RectangleF.Empty;
                // Determine Image position
                if (imgImage != null && tabData.ImageIndex!=-1)
                {
                    rectImage = GetImageRectangle(rectTextAndImage);
                    // If there isn't enough space available for text and image, adjust their positions.
                    if (!rectTextAndImage.Contains(RectangleF.Union(rectImage, m_rectText)))
                    {
                        int adjustWidth = 0;
                        if (itpdTabPanelData.TabStyle == OneNoteStyleRenderer.TabStyleName
                                    || itpdTabPanelData.TabStyle == TabRendererOffice2003.TabStyleName
                                    || itpdTabPanelData.TabStyle == TabRendererWhidbey.TabStyleName)
                        {
                            adjustWidth = c_ADJUST_WIDTH_ONE_NOTE_STYLE;
                        }
                        else if (itpdTabPanelData.TabStyle == TabRendererDockingWhidbeyBeta.TabStyleName)
                        {
                            adjustWidth = c_ADJUST_WIDTH_DOCKING_WHIDBEY_BETA;
                        }

                        // Not enough space
                        // Give image higher priority over text for the following alignments
                        switch (riaImgAlign)
                        {
                            case RelativeImageAlignment.AboveText:
                            case RelativeImageAlignment.LeftOfText:
                                {
                                    float offsetHeight = rectTextAndImage.Top - rectImage.Top;
                                    if (offsetHeight < 0)
                                    {
                                        offsetHeight = 0;
                                    }
                                    float offsetWidth = rectTextAndImage.Left - rectImage.Left;
                                    if (offsetWidth < 0)
                                    {
                                        offsetWidth = 0;
                                    }
                                    // Push image and text down
                                    rectImage.Offset(offsetWidth, offsetHeight);
                                    m_rectText.Offset(offsetWidth, offsetHeight);

                                    if (rectTextAndImage.Width < m_rectText.Width + rectImage.Width + adjustWidth)
                                    {
                                        m_rectText.Width = rectTextAndImage.Width - rectImage.Width - adjustWidth;
                                        if (TabAlignment == TabAlignment.Left)
                                        {
                                            rectImage.X = m_rectText.Right;
                                        }
                                        if (this.panelRenderer.TabPanelData.RotateTextWhenVertical)
                                        {
                                            if (m_rectText.Width < rectTextAndImage.Width)
                                                m_rectText.Width = rectTextAndImage.Width;
                                        }
                                    }
                                    break;
                                }
                            case RelativeImageAlignment.BelowText:
                                {
                                    float offsetWidth = rectTextAndImage.Left - rectImage.Left;
                                    rectImage.Offset(offsetWidth, 0);
                                    break;
                                }
                            case RelativeImageAlignment.RightOfText:
                                {
                                    float offsetHeight = rectTextAndImage.Top - rectImage.Top;
                                    rectImage.Offset(0, offsetHeight);

                                    if (rectTextAndImage.Width < m_rectText.Width + rectImage.Width + adjustWidth)
                                    {
                                        m_rectText.Width = rectTextAndImage.Width - rectImage.Width - adjustWidth;
                                        if (TabAlignment != TabAlignment.Left)
                                        {
                                            rectImage.X = m_rectText.Right;
                                        }
                                    }
                                    break;
                                }
                        }
                    }

                    int offset = 0;
                    if (panelRenderer.TabPanelData.ImageOffset > ImageTextPadding)
                    {
                        offset = panelRenderer.TabPanelData.ImageOffset - ImageTextPadding;
                    }
                    RectangleF rect = new RectangleF(rectImage.X, rectImage.Y, rectImage.Width, rectImage.Height - offset);

                    // Enough space available, go ahead and draw

                    // Draw the Image through the ImageList to support transparency in images.
                    // While drawing using ImageList we cannot have transformed graphics.
                    GraphicsState gs = g.Save();
                    g.ResetTransform();

                    RectangleF rectTrue = TabUtils.ApplyTransform(g, taTabAlign, rectImage, false);
                    Rectangle ri = Rectangle.Ceiling(rectTrue);

                    bool selectedTab = panelRenderer.TabPanelData.SelectedIndex == -1 ? false :
                    panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] == tabData;

                    if (riaImgAlign != RelativeImageAlignment.AboveText
                            && riaImgAlign != RelativeImageAlignment.BelowText
                            && riaImgAlign != RelativeImageAlignment.Overlap)
                    {
                        switch (tabAlignment)
                        {
                            case TabAlignment.Top:
                                ri.Y -= itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.Y += 2;
                                }
                                break;

                            case TabAlignment.Bottom:
                                ri.Y += itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.Y -= 2;
                                }
                                break;

                            case TabAlignment.Left:
                                ri.X -= itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.X += 2;
                                }
                                break;

                            case TabAlignment.Right:
                                ri.X += itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.X -= 2;
                                }
                                break;
                        }
                    }


                    // Sometimes the above transform will make the width a pixel wider than the original and
                    // the DrawImageViaImageList chokes on it.
                    if (ri.Width > panelRenderer.TabPanelData.ImageList.ImageSize.Width)
                        ri.Width = panelRenderer.TabPanelData.ImageList.ImageSize.Width;

                    if (ri.Height > panelRenderer.TabPanelData.ImageList.ImageSize.Height)
                        ri.Height = panelRenderer.TabPanelData.ImageList.ImageSize.Height;

                    Icon icon = this.Icon;

                    if (icon != null)
                    {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                        g.DrawIcon( icon, ri );
#else
                        g.DrawImage(imgImage, ri);
#endif
                    }
                    else
                    {
                        if (bEnabled)
                        {

                            if (!itpdTabPanelData.DisableInactivePageImage || selectedTab)
                            {
                                g.DrawImage(imgImage, ri);
                            }
                            else
                            {
                                ControlPaint.DrawImageDisabled(g, imgImage, (int)ri.Left, (int)ri.Top, e.BackColor);
                            }
                        }
                        else
                        {
                            ControlPaint.DrawImageDisabled(g, imgImage, (int)ri.Left, (int)ri.Top, e.BackColor);
                        }
                    }

                    g.Restore(gs);

                    imgImage.RotateFlip(TabAlignment.Right == tabAlignment ? RotateFlipType.Rotate90FlipNone : RotateFlipType.RotateNoneFlipNone);
                }
                else if(tabData.Image!=null)
                {
                    rectImage = GetAnimateImageRectangle(rectTextAndImage);
                    // If there isn't enough space available for text and image, adjust their positions.
                    if (!rectTextAndImage.Contains(RectangleF.Union(rectImage, m_rectText)))
                    {
                        int adjustWidth = 0;
                        if (itpdTabPanelData.TabStyle == OneNoteStyleRenderer.TabStyleName
                                    || itpdTabPanelData.TabStyle == TabRendererOffice2003.TabStyleName
                                    || itpdTabPanelData.TabStyle == TabRendererWhidbey.TabStyleName)
                        {
                            adjustWidth = c_ADJUST_WIDTH_ONE_NOTE_STYLE;
                        }
                        else if (itpdTabPanelData.TabStyle == TabRendererDockingWhidbeyBeta.TabStyleName)
                        {
                            adjustWidth = c_ADJUST_WIDTH_DOCKING_WHIDBEY_BETA;
                        }

                        // Not enough space
                        // Give image higher priority over text for the following alignments
                        switch (riaImgAlign)
                        {
                            case RelativeImageAlignment.AboveText:
                            case RelativeImageAlignment.LeftOfText:
                                {
                                    float offsetHeight = rectTextAndImage.Top - rectImage.Top;
                                    if (offsetHeight < 0)
                                    {
                                        offsetHeight = 0;
                                    }
                                    float offsetWidth = rectTextAndImage.Left - rectImage.Left;
                                    if (offsetWidth < 0)
                                    {
                                        offsetWidth = 0;
                                    }
                                    // Push image and text down
                                    rectImage.Offset(offsetWidth, offsetHeight);
                                    m_rectText.Offset(offsetWidth, offsetHeight);

                                    if (rectTextAndImage.Width < m_rectText.Width + rectImage.Width + adjustWidth)
                                    {
                                        m_rectText.Width = rectTextAndImage.Width - adjustWidth;
                                        if (TabAlignment == TabAlignment.Left)
                                        {
                                            rectImage.X = m_rectText.X;
                                            m_rectText.Y = rectImage.Y;
                                        }
                                    }
                                    break;
                                }
                            case RelativeImageAlignment.BelowText:
                                {
                                    float offsetWidth = rectTextAndImage.Left - rectImage.Left;
                                    rectImage.Offset(offsetWidth, 0);
                                    break;
                                }
                            case RelativeImageAlignment.RightOfText:
                                {
                                    float offsetHeight = rectTextAndImage.Top - rectImage.Top;
                                    rectImage.Offset(0, offsetHeight);

                                    if (rectTextAndImage.Width < m_rectText.Width + rectImage.Width + adjustWidth)
                                    {
                                        m_rectText.Width = rectTextAndImage.Width - rectImage.Width - adjustWidth;
                                        if (TabAlignment != TabAlignment.Left)
                                        {
                                            rectImage.X = m_rectText.Right;
                                        }
                                    }
                                    break;
                                }
                        }
                    }

                    int offset = 0;
                    if (panelRenderer.TabPanelData.ImageOffset > ImageTextPadding)
                    {
                        offset = panelRenderer.TabPanelData.ImageOffset - ImageTextPadding;
                    }
                    RectangleF rect = new RectangleF(rectImage.X, rectImage.Y, rectImage.Width, rectImage.Height - offset);

                    // Enough space available, go ahead and draw

                    // Draw the Image through the ImageList to support transparency in images.
                    // While drawing using ImageList we cannot have transformed graphics.
                    GraphicsState gs = g.Save();
                    g.ResetTransform();

                    RectangleF rectTrue = TabUtils.ApplyTransform(g, taTabAlign, rectImage, false);
                    Rectangle ri = Rectangle.Ceiling(rectTrue);
                    bool selectedTab = panelRenderer.TabPanelData.SelectedIndex == -1 ? false :
                    panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] == tabData;

                    if (riaImgAlign != RelativeImageAlignment.AboveText
                            && riaImgAlign != RelativeImageAlignment.BelowText
                            && riaImgAlign != RelativeImageAlignment.Overlap)
                    {
                        switch (tabAlignment)
                        {
                            case TabAlignment.Top:
                                ri.Y -= itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.Y += 2;
                                }
                                break;

                            case TabAlignment.Bottom:
                                ri.Y += itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.Y -= 2;
                                }
                                break;

                            case TabAlignment.Left:
                                ri.X -= itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.X += 2;
                                }
                                break;

                            case TabAlignment.Right:
                                ri.X += itpdTabPanelData.ImageOffset;
                                if (!selectedTab && itpdTabPanelData.ImageOffset >= 2)
                                {
                                    ri.X -= 2;
                                }
                                break;
                        }
                    }
                    
                     ImageRect = ri;

                    g.Restore(gs);
                 }

                if (itpdTabPanelData.LevelTextAndImage && imgImage != null)
                {
                    m_rectText.Y = (float)Math.Floor(rectImage.Bottom - m_rectText.Height - itpdTabPanelData.ImageOffset);
                }

                GraphicsState savedState = null;
                if ((taTabAlign == TabAlignment.Left || taTabAlign == TabAlignment.Right)
                    && itpdTabPanelData.RotateTextWhenVertical)
                {
                    savedState = g.Save();
                    g.ResetTransform();
                    m_rectText = TabUtils.ApplyTransform(g, taTabAlign, m_rectText, false);
                }

                isTextShrunk = false;
                if (m_rectText.Width > 0 && m_rectText.Height > 0)
                {
                    if (panelRenderer.Renderers[0] is TabRendererDockingVS2012 || panelRenderer.Renderers[0] is TabGroupRendererVS2012)
                    {
                        RectangleF rc = new RectangleF(m_rectText.X + (isBackStage ? 10 : 0), m_rectText.Y, m_rectText.Width, m_rectText.Height); ;
                        if (itpdTabPanelData.RotateText180WhenLeftAligned)
                            rc = new RectangleF(m_rectText.X, m_rectText.Y - 9, m_rectText.Width, m_rectText.Height);
                        this.DrawText(g, rc, tabData.Text, sfFormat, e);
                    }
                    else
                    {
                        RectangleF rc = new RectangleF(m_rectText.X + (isBackStage ? 10 : 0), m_rectText.Y, m_rectText.Width, m_rectText.Height);
                        if (m_rectText.Width < e.Bounds.Width)
                            sfFormat.Trimming = StringTrimming.None;
                        if (m_rectText.Width > 110)
                        {
                            sfFormat.Trimming = StringTrimming.EllipsisCharacter;
                            rc = new RectangleF(m_rectText.X + (isBackStage ? 10 : 0), m_rectText.Y, (isBackStage ? 110 : m_rectText.Width), m_rectText.Height);
                        }
                        this.DrawText(g, rc, tabData.Text, sfFormat, e);
                    }
                   
                }
                else
                {
                    m_lastDrawnTextBounds = Rectangle.Round(rectTextAndImage);
                    this.isTextShrunk = true;
                }

                if (savedState != null)
                {
                    g.Restore(savedState);
                }
            }
        }

        /// <summary>
        /// Starts the animator.
        /// </summary>
        private void StartAnimator()
        {
            if (!this.parent.IsDesignMode() && ImageAnimator.CanAnimate(tabData.Image))
            {
                ImageAnimator.Animate(tabData.Image, new EventHandler(this.Image_FrameChanged));
                currentlyAnimating = true;
            }
        }

        /// <summary>
        /// Handles the FrameChanged event of the Image control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Image_FrameChanged(object sender, EventArgs e)
        {
            this.parent.GetControl().Invalidate(ImageRect);
        }
         
        /// <summary>
        /// Draws the text of the tab, given the context.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> instance.</param>
        /// <param name="rectText">The layout rectangle.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="format">The <see cref="System.Drawing.StringFormat"/> with which to draw.</param>
        /// <param name="e">The object that has information regarding this tab and paint event.</param>
        protected virtual void DrawText(Graphics g, RectangleF rectText, string text,
            StringFormat format, DrawTabEventArgs e)
        {
            Font textFont = e.Font;

            if (isBackStage)
            {
                if(panelRenderer is BackStage2013TabRenderer )
                 e.ForeColor = Color.White ;
                textFont = new System.Drawing.Font(e.Font, System.Drawing.FontStyle.Regular);
            }
            else
            {
                textFont = e.Font;
            }

            //Brush brush = e.TextBrush;
            //if (brush == null)
            //{
            //    brush = new SolidBrush(e.ForeColor);
            //}

            bool enabled = ((int)e.State & (int)DrawItemState.Disabled) <= 0;

            if (cachedTextPrefSize.Width > rectText.Width)
            {
                isTextShrunk = true;
            }

            m_lastDrawnTextBounds = Rectangle.Round(rectText);

            if (tabAlignment == TabAlignment.Right || tabAlignment == TabAlignment.Top)
            {
                if (m_bShouldDrawText)
                {
                    DrawTextInternal(g, text, textFont, e.ForeColor, rectText, format, tabAlignment, enabled);
                }
            }
            else
            {
                // Reset transform temporarily
                GraphicsState oldState = g.Save();
                RectangleF rectTextTransformed = RectangleF.Empty;
                if (tabAlignment == TabAlignment.Bottom)
                {
                    g.ResetTransform();
                    rectTextTransformed = TabUtils.ApplyTransform(g, tabAlignment, rectText, false);
                }
                else if (tabAlignment == TabAlignment.Left)
                {
                    if (!panelRenderer.TabPanelData.RotateTextWhenVertical)
                    {
                        g.ScaleTransform(-1, 1);
                        rectTextTransformed = new RectangleF(-rectText.Right + 1, rectText.Top,
                            rectText.Width, rectText.Height);
                        if (panelRenderer.TabPanelData.RotateText180WhenLeftAligned)
                        {
                            g.RotateTransform(180.0f);
                            rectTextTransformed = new RectangleF(rectText.Left, -rectText.Top - 1 - rectText.Height,
                                rectText.Width, rectText.Height);
                        }
                    }
                    else
                        rectTextTransformed = rectText;
                }

                m_lastDrawnTextBounds = Rectangle.Round(rectTextTransformed);

                if (isBackStage)
                {
                    if ((this.TabControl as BackStage).OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                    {
                        e.ForeColor = Color.White;
                    }
                }
                if (m_bShouldDrawText)
                {
                    DrawTextInternal(g, text, textFont, e.ForeColor, rectTextTransformed, format, tabAlignment, enabled);
                }

                g.Restore(oldState);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="textFont"></param>
        /// <param name="rectText"></param>
        /// <param name="format"></param>
        /// <param name="tabAlignment"></param>
        private void DrawTextInternal(Graphics g, string text, Font textFont, Color foreColor, RectangleF rectText,
                    StringFormat format, TabAlignment tabAlignment, bool enabled)
        {
            if (tabAlignment == TabAlignment.Top || tabAlignment == TabAlignment.Bottom)
            {
                if (enabled)
                {
                    DrawTextNative(g, text, textFont, foreColor, rectText);
                }
                else
                {
                    rectText.Offset(1, 1);
                    Color color = ControlPaint.LightLight(SystemColors.Control);
                    DrawTextNative(g, text, textFont, color, rectText);

                    rectText.Offset(-1, -1);
                    color = ControlPaint.Dark(color);
                    DrawTextNative(g, text, textFont, color, rectText);
                }
            }
            else
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (!SystemInformation.IsFontSmoothingEnabled)
                {
                    if (enabled)
                    {
                        using (Bitmap bmp = GetTextBitmap(text, textFont, rectText, foreColor))
                        {
                            g.DrawImage(bmp, new PointF(rectText.X, rectText.Y));
                        }
                    }
                    else
                    {
                        using (Bitmap bmp = GetDisabledTextBitmap(text, textFont, rectText, SystemColors.Control))
                        {
                            g.DrawImage(bmp, new PointF(rectText.X, rectText.Y));
                        }
                    }

                    return;
                }
#endif
                // ClearType / standard font smoothing.
                using (Brush brush = new SolidBrush(foreColor))
                {
                    if (this.TabControl is BackStage)
                        rectText = new RectangleF(rectText.X + 10, rectText.Y, rectText.Width, rectText.Height);
                    if (this.TabControl != null && this.TabControl is BackStage)
                    {
                        if((this.TabControl as BackStage).RightToLeft == RightToLeft.Yes)
                        {
                            rectText = new RectangleF(rectText.X + 20, rectText.Y, rectText.Width, rectText.Height);
                        }
                    }
                    if (enabled)
                    {
                        TextRenderingHint prevHint = g.TextRenderingHint;

                        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        g.DrawString(text, textFont, brush, rectText, format);

                        g.TextRenderingHint = prevHint;
                    }
                    else
                    {
                        ControlPaint.DrawStringDisabled(g, text, textFont, SystemColors.Control, rectText, format);
                    }
                }
            }
        }

        /// <summary>
        /// Draw text by native GDI API.
        /// </summary>
        /// <param name="g">graphics which handle we have to use.</param>
        /// <param name="f">Font which we have to use for text drawing.</param>
        /// <param name="color">Text color.</param>
        private void DrawTextNative(Graphics g, string text, Font f, Color color, RectangleF textRect)
        {
            ITabPanelData itpdTabPanelData = panelRenderer.TabPanelData;
            bool bIsMirrored = panelRenderer.IsMirrored;

            int nFlags = c_DRAW_TEXT_FLAGS;

            switch (itpdTabPanelData.TextAlignment)
            {
                case StringAlignment.Center:
                    nFlags |= DrawTextFormats.DT_CENTER;
                    break;

                case StringAlignment.Near:
                    nFlags |= DrawTextFormats.DT_LEFT;
                    break;

                case StringAlignment.Far:
                    nFlags |= DrawTextFormats.DT_RIGHT;
                    break;
            }

            switch (itpdTabPanelData.TextLineAlignment)
            {
                case StringAlignment.Center:
                    nFlags |= DrawTextFormats.DT_VCENTER;
                    break;

                case StringAlignment.Near:
                    nFlags |= DrawTextFormats.DT_TOP;
                    break;

                case StringAlignment.Far:
                    nFlags |= DrawTextFormats.DT_BOTTOM;
                    break;
            }

            if (!TabControl.UseMnemonic)
            {
                nFlags |= DrawTextFormats.DT_NOPREFIX;
            }

            if (bIsMirrored)
            {
                nFlags |= DrawTextFormats.DT_RTLREADING;
            }

            if (TabRendererFactory.GetRegisteredExtender(itpdTabPanelData.TabStyle).DrawEllipsis)
            {
                nFlags |= DrawTextFormats.DT_END_ELLIPSIS;
            }

            if (TabControl.MultilineText)
            {
                nFlags &= ~DrawTextFormats.DT_END_ELLIPSIS;
                nFlags &= ~DrawTextFormats.DT_SINGLELINE;
                nFlags |= DrawTextFormats.DT_EXPANDTABS;
            }

            IntPtr clipRgn = g.Clip.GetHrgn(g);
            IntPtr hdc = g.GetHdc();
            IntPtr hFont = f.ToHfont();
            
            IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

            NativeMethods.SelectClipRgn(hdc, clipRgn);
            Color invertedColor = Color.FromArgb(0, color.B, color.G, color.R);
            NativeMethods.SetTextColor(hdc, invertedColor.ToArgb() & 0xFFFFFF);
            NativeMethods.SetBkMode(hdc, 1); // TRANSPARENT

            Rectangle r = Rectangle.Ceiling(textRect);
            NativeMethods.RECT rect = new NativeMethods.RECT(r);

            NativeMethods.DrawText(hdc, text, text.Length, ref rect, nFlags);

            prevFont = NativeMethods.SelectObject(hdc, prevFont);
            NativeMethods.DeleteObject(hFont);

            NativeMethods.DeleteObject(clipRgn);

            NativeMethods.SelectClipRgn(hdc, IntPtr.Zero);
      
            g.ReleaseHdc(hdc);
        }

        /// <summary>
        /// Draws text to bitmap.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textFont"></param>
        /// <param name="rectText"></param>
        /// <param name="foreColor"></param>
        /// <returns></returns>
        private Bitmap GetTextBitmap(string text, Font textFont, RectangleF rectText, Color foreColor)
        {
            Bitmap bmp = new Bitmap((int)rectText.Width, (int)rectText.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                DrawTextNative(graphics, text, textFont, foreColor,
                    new Rectangle(0, 0, (int)rectText.Width, (int)rectText.Height));
            }

            return bmp;
        }

        /// <summary>
        /// Draws disabled text to bitmap.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textFont"></param>
        /// <param name="rectText"></param>
        /// <param name="foreColor"></param>
        /// <returns></returns>
        private Bitmap GetDisabledTextBitmap(string text, Font textFont, RectangleF rectText, Color foreColor)
        {
            Bitmap bmp = new Bitmap((int)rectText.Width, (int)rectText.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Rectangle rectToDraw = new Rectangle(0, 0, (int)rectText.Width, (int)rectText.Height);
                rectToDraw.Offset(1, 1);
                Color color = ControlPaint.LightLight(foreColor);
                DrawTextNative(graphics, text, textFont, color, rectToDraw);

                rectToDraw.Offset(-1, -1);
                color = ControlPaint.Dark(color);
                DrawTextNative(graphics, text, textFont, color, rectToDraw);
            }

            return bmp;
        }

        /// <summary>
        /// Measures text according to <see cref="System.Windows.SystemInformation.FormsFontSmoothingType"/>.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private SizeF MeasureText(Graphics g, string text, Font font)
        {
            SizeF sizeRect = SizeF.Empty;

            if (tabAlignment == TabAlignment.Top || tabAlignment == TabAlignment.Bottom)
            {
                Graphics graphics = MeasureGraphics; // Graphics for measuring text.
                lock (graphics)
                {
                    if (TabControl.MultilineText)
                    {
                        sizeRect = ControlDrawing.MeasureDisplayStringSize(graphics, text, font, panelRenderer.IsMirrored, 1);
                    }
                    else
                    {
                        sizeRect = ControlDrawing.MeasureDisplayStringSize(graphics, text, font, panelRenderer.IsMirrored);
                    }
                }
            }
            else
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (!SystemInformation.IsFontSmoothingEnabled)
                {
                    Graphics graphics = MeasureGraphics; // Graphics for measuring text.
                    lock (graphics)
                    {
                        if (TabControl.MultilineText)
                        {
                            sizeRect = ControlDrawing.MeasureDisplayStringSize(graphics, text, font, panelRenderer.IsMirrored, 1);
                        }
                        else
                        {
                            sizeRect = ControlDrawing.MeasureDisplayStringSize(graphics, text, font, panelRenderer.IsMirrored);
                        }
                    }
                }
                else  // ClearType / standart font smoothing.
                {
                    sizeRect = g.MeasureString(text, font);
                }
#else
                sizeRect = g.MeasureString( text, font );
#endif
            }

            return sizeRect;
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.Bounds"/>.
        /// </summary>
        public virtual RectangleF Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
            }
        }
        private Rectangle imageRect;

        public Rectangle ImageRect
        {
            get { return imageRect; }
            set { imageRect = value; }
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.TabAlignment"/>.
        /// </summary>
        public TabAlignment TabAlignment
        {
            get { return tabAlignment; }
            set { tabAlignment = value; }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual RectangleF CorrectBounds(RectangleF bounds)
        {
            return bounds;
        }

        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.OnPaint"/>.
        /// </summary>
        public virtual void OnPaint(Graphics g, Rectangle clippingRect)
        {
            RectangleF clipRect = TabUtils.ApplyTransform(g, TabAlignment, clippingRect, true);
            if (!this.GetCurrentBounds().IntersectsWith(clipRect))
                return;

            // Default Properties provider
            ITabDefaultProperties defaultProperties = TabRendererFactory.GetRegisteredExtender(panelRenderer.TabPanelData.TabStyle);

            //ITabDefaultProperties tabPanelPropertyExtender =
            //	TabRendererFactory.GetRegisteredExtender(this.panelRenderer.TabPanelData.TabStyle);

            // Is selected tab?
            bool selectedTab = panelRenderer.TabPanelData.SelectedIndex == -1 ?
                false : panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] == tabData;

            // Determine current bounds
            RectangleF currentBounds = GetCurrentBounds();
            currentBounds = CorrectBounds(currentBounds);
            RectangleF rectTextAndImage = GetInteriorBounds(currentBounds, selectedTab);

            if (!selectedTab)
            {
                // Overlapped area for unselected tabs is adjusted regarding RTL setting:
                // LTR: 1 pixel adjustment for Left alignment, 2 pixels for Right alignment
                // RTL: 1 pixel adjustment for Right alignment, 2 pixels for Left alignment
                bool bReduceHeightForSide = (tabAlignment == (panelRenderer.IsMirrored ?
                    TabAlignment.Right : TabAlignment.Left));
                // Unselected tab
                if (panelRenderer.TabPanelData.TabStyle != "VS2005DockingStyleBeta")
                {
                    if (tabAlignment == TabAlignment.Top || bReduceHeightForSide)
                        currentBounds.Height -= 1;
                    else
                        currentBounds.Height -= 2;
                }
                else
                {
                    currentBounds.Height += 2;
                }
            }

            lastDrawnBounds = currentBounds;	// Necessary for efficient painting

            currentBounds = TabUtils.ApplyTransform(g, tabAlignment, currentBounds, false);

            rectTextAndImage = TabUtils.ApplyTransform(g, tabAlignment, rectTextAndImage, false);

            // Determine back ground color
            Color bgColor = tabData.BackColor;
            if (selectedTab)
            {
                if (panelRenderer.TabPanelData.TabStyle == "VS2005DockingStyleBeta")
                {
                    if (tabAlignment == TabAlignment.Top || tabAlignment == TabAlignment.Bottom)
                        currentBounds.Height += 2;
                    else if (tabAlignment == TabAlignment.Right || tabAlignment == TabAlignment.Left)
                    {
                        if (!NeedRotateTextWhenVertical)
                            currentBounds.Width += 2;
                        else
                            currentBounds.Width -= 3;
                    }
                }
                bgColor = panelRenderer.TabPanelData.ActiveTabColor;
                if (bgColor == Color.Empty)
                {
                    if (panelRenderer.Renderers[0] is TabRendererDockingVS2012)
                    {
                        bgColor = Color.FromArgb(239, 239, 242);
                    }
                    else
                    {
                        bgColor = defaultProperties.DefaultActiveTabColor(panelRenderer.TabPanelData, parent);
                    }
                }
            }
            else
            {
                bgColor = panelRenderer.TabPanelData.InactiveTabColor != Color.Empty ? panelRenderer.TabPanelData.InactiveTabColor : tabData.BackColor;
                if (bgColor == Color.Empty)
                {
                    if (panelRenderer.Renderers[0] is TabRendererDockingVS2012)
                    {
                        bgColor = Color.FromArgb(239, 239, 242);
                    }
                    else
                    {
                        bgColor = defaultProperties.DefaultInactiveTabColor(panelRenderer.TabPanelData, parent);
                    }
                }
            }

            // Determine Text font and forecolor
            Color foreColor = Color.Empty;
            foreColor = selectedTab ? GetActiveForeColor() : GetForeColor();
            Font textFont = tabData.Font;

            if (textFont == null)
            {
                if (selectedTab)
                {
                    textFont = panelRenderer.TabPanelData.ActiveTabFont;
                    if (textFont == null)
                    {
                        textFont = defaultProperties.DefaultActiveTabFont(panelRenderer.TabPanelData, parent);
                    }
                }
                else
                {
                    if (textFont == null)
                    {
                        textFont = defaultProperties.DefaultInactiveTabFont(panelRenderer.TabPanelData, parent);
                    }
                }
            }

            // Determine DrawItemState
            DrawItemState state = DrawItemState.None;
            if (hotTrack)
                state |= DrawItemState.HotLight;

            if (!tabData.Enabled)
                state |= DrawItemState.Disabled;

            if (selectedTab)
            {
                state |= DrawItemState.Selected;

                bool bShouldFocus = this.parent.GetControl().Focused;
                TabControlAdv tabControl = this.parent as TabControlAdv;

                if (tabControl != null)
                {
                    bShouldFocus = bShouldFocus && tabControl.FocusOnTabClick;
                }

                if (bShouldFocus)
                    state |= DrawItemState.Focus;
            }
            if (tabData.ImageChanged)
            {
                currentlyAnimating = false;
                tabData.ImageChanged = false;
            }
            if (!currentlyAnimating && tabData.Image != null && selectedTab && tabData.ImageIndex == -1 && tabData.Enabled)
            {
                StartAnimator();
            }
            else if (!selectedTab && currentlyAnimating && tabData.ImageIndex == -1)
            {
                ImageAnimator.StopAnimate(tabData.Image, new EventHandler(this.Image_FrameChanged));
                currentlyAnimating = false;
            }
           
            // Create DrawTabEventArgs
            DrawTabEventArgs drawItemEventArgs = new DrawTabEventArgs(g, textFont, Rectangle.Round(currentBounds),
                this.panelRenderer.TabPanelData.TabsData.IndexOf(tabData), state, foreColor, bgColor, Rectangle.Round(rectTextAndImage),
                new DrawTabEventArgs.DrawDefaultBackground(DrawBackground),
                new DrawTabEventArgs.DrawDefaultBorders(DrawBorders),
                new DrawTabEventArgs.DrawDefaultInterior(DrawInterior));

            if (drawItemCallback == null || !drawItemCallback(drawItemEventArgs))
            {
                if (!(panelRenderer.Renderers[0] is TabRendererDockingVS2012))
                DrawBackground(drawItemEventArgs);
                DrawBorders(drawItemEventArgs);
                DrawInterior(drawItemEventArgs);
            }

            if (tabData.Image != null && tabData.ImageIndex == -1)
            {
                g.DrawImage(tabData.Image, ImageRect);
                ImageAnimator.UpdateFrames();
            }
        }

        /// <summary>
        /// Gets the interior bounds
        /// </summary>
        protected virtual RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
        {
            // Determine rect for Text and Image and Adjust for padding
            RectangleF rectTextAndImage = currentBounds;
            rectTextAndImage = RectangleF.Inflate(rectTextAndImage, -this.panelRenderer.TabPanelData.Padding.X,
                -this.panelRenderer.TabPanelData.Padding.Y);

            rectTextAndImage = CorrectInteriorBounds(rectTextAndImage);

            if (selectedTab)
            {
                SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
                rectTextAndImage = RectangleF.Inflate(rectTextAndImage, (float)Math.Ceiling(-overlapSize.Width / 2), (float)Math.Ceiling(-overlapSize.Height / 2));
            }
            return rectTextAndImage;
        }
        /// <summary>
        /// Draws the background of the tab.
        /// </summary>
        /// <param name="drawItemInfo">The object that has information regarding this tab and paint event.</param>
        protected abstract void DrawBackground(DrawTabEventArgs drawItemInfo);
        /// <summary>
        /// Draws the borders of the tab.
        /// </summary>
        /// <param name="drawItemInfo">The object that has information regarding this tab and paint event.</param>
        protected abstract void DrawBorders(DrawTabEventArgs drawItemInfo);
        /// <summary>
        /// Draws the interior of the tab.
        /// </summary>
        /// <param name="drawItemInfo">The object that has information regarding this tab and paint event.</param>
        protected abstract void DrawInterior(DrawTabEventArgs drawItemInfo);

        /// <summary>
        /// Indicates the selected state of the item.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual bool IsSelectedState(DrawItemState state)
        {
            return ((state & DrawItemState.Selected) > 0);
        }

        /// <summary>
        /// Indicates the hotLight state of the item.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual bool IsHotLightState(DrawItemState state)
        {
            return ((state & DrawItemState.HotLight) > 0);
        }

        /// <summary>
        /// Rotates/scales the provided graphics object by an angle based on the current alignment
        /// such that you can use drawing code that assumes Top alignment for a tab.
        /// </summary>
        /// <param name="g">The Graphics object to apply transformation on.</param>
        /// <remarks>
        /// You should normally use this in the DrawInterior, DrawBackground, DrawBorders
        /// overrides to transform the incoming Graphics object.
        /// Use this in conjunction with the TabUtils.ApplyTransform to transform
        /// the incoming bounds of the above overrides.
        /// </remarks>
        protected virtual void ApplyTransform(Graphics g)
        {
            switch (this.TabAlignment)
            {
                case TabAlignment.Top: return;
                case TabAlignment.Left: g.ScaleTransform(-1, 1); g.RotateTransform(-90.0F, MatrixOrder.Append); return;
                case TabAlignment.Right: g.RotateTransform(-270.0F); return;
                case TabAlignment.Bottom: //g.RotateTransform(-180.0F);return;
                    g.ScaleTransform(1, -1); return;
            }
        }
        /// <summary>
        /// See <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.GetRedrawBounds"/>.
        /// </summary>
        public virtual RectangleF GetRedrawBounds()
        {
            RectangleF curBounds = GetCurrentBounds();
            return RectangleF.Union(curBounds, lastDrawnBounds);
        }

        /// <summary>
        /// Returns the forecolor with which to draw the tab text.
        /// </summary>
        /// <returns>Fore color.</returns>
        protected virtual Color GetForeColor()
        {
            Color foreColor = this.tabData.ForeColor;
            if (foreColor == Color.Empty)
            {
                if (panelRenderer.Renderers[0] is TabRendererDockingVS2012)
                {
                    ITabDefaultProperties tabPanelPropertyExtender =
                       TabRendererFactory.GetRegisteredExtender(panelRenderer.TabPanelData.TabStyle);
                    foreColor = tabPanelPropertyExtender.DefaultTabForeColor(panelRenderer.TabPanelData, parent);
                    bool selectedTab = panelRenderer.TabPanelData.SelectedIndex == -1 ?
                false : panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] == tabData;
                    if (selectedTab)
                        foreColor = Color.Blue;
                    else
                        foreColor = tabPanelPropertyExtender.DefaultTabForeColor(panelRenderer.TabPanelData, parent);
                }
                else
                {
                    ITabDefaultProperties tabPanelPropertyExtender =
                        TabRendererFactory.GetRegisteredExtender(panelRenderer.TabPanelData.TabStyle);
                    foreColor = tabPanelPropertyExtender.DefaultTabForeColor(panelRenderer.TabPanelData, parent);
                }
            }
            TabControlAdv adv = this.TabControl.GetControl() as TabControlAdv;
            if ( adv!=null && adv.HotTrack && hotTrack)
            {
                foreColor = SystemColors.HotTrack;
            }
            return foreColor;
        }

        /// <summary>
        /// Returns the forecolor with which to draw the tab text on active tab.
        /// </summary>
        /// <returns>Fore color.</returns>
        protected virtual Color GetActiveForeColor()
        {
            return GetForeColor();
        }

        /// <summary>
        /// Returns the current drawing bounds.
        /// </summary>
        /// <returns>The bounds as a rectangle.</returns>
        /// <remarks>
        /// <para>
        /// If this is the selected tab, then this method adds the overlapped size to the
        /// bounds returned by <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.Bounds"/>.
        /// </para>
        /// </remarks>
        public virtual RectangleF GetCurrentBounds()
        {
            if (this.panelRenderer.TabPanelData == null) return RectangleF.Empty;
            bool tabSelected = this.panelRenderer.TabPanelData.SelectedIndex == -1 ?
                false :
                this.panelRenderer.TabPanelData.TabsData[this.panelRenderer.TabPanelData.SelectedIndex] == this.tabData;

            // Assuming drawing within bounds if not selected
            if (!tabSelected || !this.Visible)
            {
                return this.bounds;
            }
            // Assuming drawing within bounds + overlapped size if selected
            else
            {
                SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
                RectangleF overlappedRect = this.bounds;
                overlappedRect.Inflate((float)Math.Ceiling(overlappedSize.Width / 2), 0);
                overlappedRect.Offset(0, -overlappedSize.Height);
                overlappedRect.Height += overlappedSize.Height;

                return overlappedRect;
            }
        }

        /// <summary>
        /// Returns the current drawing bounds.
        /// Overrided in OneNoteStyleRenderer.
        /// </summary>
        /// <returns>The bounds as a rectangle.</returns>
        /// <remarks>
        /// <para>
        /// If this is the selected tab, then this method adds the overlapped size to the
        /// bounds returned by <see cref="Syncfusion.Windows.Forms.Tools.ITabRenderer.Bounds"/>.
        /// </para>
        /// </remarks>
        public virtual RectangleF GetBoundsForScrolling()
        {
            return GetCurrentBounds();
        }

        /// <summary>
        /// Returns the overlapped size, if any, of the tabs.
        /// </summary>
        /// <returns>The overlap size.</returns>
        public virtual SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(0, 0);
        }
        /// <summary>
        /// Indicates whether the specified position is within the current bounds.
        /// </summary>
        /// <param name="mousePosition">The mouse position to test.</param>
        /// <returns>True if hit; false otherwise.</returns>
        public virtual bool HitTest(PointF mousePosition)
        {
            if (GetCurrentBounds().Contains(mousePosition))
                return true;
            else
                return false;
        }
        
        #region IDisposable implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TabRendererBase"/> is reclaimed by garbage collection.
        /// </summary>
        ~TabRendererBase()
        {
            Dispose(false);
        }

        #endregion
    }

    #endregion VIEW_IMP

}