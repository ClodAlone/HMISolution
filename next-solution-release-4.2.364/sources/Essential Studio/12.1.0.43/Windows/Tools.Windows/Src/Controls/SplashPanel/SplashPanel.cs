#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region File Using
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    #region SPLASHPANEL
    /// <summary>
    /// The SplashPanel class is a <see cref="System.Windows.Forms.Panel"/> derived
    /// class that will let you design custom splash screens
    /// within the Form's designer. The SplashPanel can take multiple child controls
    /// that can be used to display information or collect information from the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SplashPanel class is a panel class that has uses beyond the typical
    /// splash screen. It can be used to create non obtrusive message boxes
    /// such as the Microsoft MSN messenger (as of MSN Messenger version 3.0 )
    /// message window that informs user that a new mail has arrived. These kind
    /// of messages boxes are made very easy to create and use with the SplashPanel class.
    /// </para>
    /// <para>
    /// To design a custom splash, drag and drop it off the toolbox
    /// into a Form during design-time. Then populate it with
    /// appropriate Controls just like you would any other Panel.
    /// The splash panel can also appear in an animated manner on the
    /// screen. The startup position of the splash panel can also be
    /// specified through the <see cref="DesktopAlignment"/> property.
    /// </para>
    /// <para>
    /// The Splash Panel itself can be set to have appealing gradient and
    /// pattern backgrounds by specifying the <see cref="BackgroundColor"/>
    /// property.
    /// </para>
    /// <para>
    /// When you are ready to display the splash, call this <see cref="ShowSplash"/> method.
    /// This will show the splash panel at the specified location.
    /// </para>
    /// <para>The SplashPanel uses a <see cref="System.Windows.Forms.Timer"/>
    /// internally to automatically close the Splash screen after the set
    /// interval is elapsed. This behavior can be changed by setting
    /// the SplashPanel's <see cref="SplashPanel.TimerInterval"/> property to
    /// -1. The SplashPanel can be explicitly closed by calling <see cref="SplashPanel.HideSplash"/>
    /// </para>
    /// <para>The SplashPanel also raises the <see cref="BeforeSplash"/>, <see cref="SplashDisplayed"/>
    /// and <see cref="SplashClosing"/> events that you can handle. You could for example
    /// set the focus on a Control within the SplashPanel in the
    /// SplashDisplayed event handler.</para>
    /// </remarks>
    /// <example>
    /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\SplashPanelDemo\CS\MainForm.cs" name="SplashPanel InitializeComponent" lang="CS">
    /// <code lang="C#">
    ///      // Create the splash panel
    ///      this.splashPanel1 = new SplashPanel();
    ///      this.button1 = new Button();
    ///      // The animation speed
    ///      this.splashPanel1.AnimationSpeed = 10;
    ///      // The background
    ///      this.splashPanel1.BackgroundColor = new BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.SystemColors.HighlightText, System.Drawing.SystemColors.Highlight);
    ///      // The border style
    ///      this.splashPanel1.BorderStyle = System.Windows.Forms.Border3DStyle.Bump;
    ///      // The child controls - added through the designer
    ///      this.splashPanel1.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                                 this.linkLabel1});
    ///      // The startup location for the splash panel
    ///      this.splashPanel1.DesktopAlignment = SplashPanel.SplashAlignment.SystemTray;
    ///      // Specifies whether the window should appear animated
    ///      this.splashPanel1.ShowAnimation = true;
    ///      // The interval for which the panel is to be displayed
    ///      this.splashPanel1.TimerInterval = 5000;
    ///      this.button1.Click += new System.EventHandler(this.button1_Click);
    ///      </code>
    ///      </coderef>
    ///      <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\SplashPanelDemo\VB\MainForm.vb" name="SplashPanel InitializeComponent" lang="VB"><code lang="VB">
    ///     ' Create the splash panel
    ///     Me.splashPanel1 = New SplashPanel()
    ///     Me.button1 = New Button()
    ///     ' The animation speed
    ///     Me.splashPanel1.AnimationSpeed = 10
    ///     ' The background
    ///     Me.splashPanel1.BackgroundColor = New BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.SystemColors.HighlightText, System.Drawing.SystemColors.Highlight)
    ///     ' The border style
    ///     Me.splashPanel1.BorderStyle = System.Windows.Forms.Border3DStyle.Bump
    ///     ' The child controls - added through the designer
    ///     Me.splashPanel1.Controls.AddRange(New System.Windows.Forms.Control() {Me.linkLabel1})
    ///     ' The startup location for the splash panel
    ///     Me.splashPanel1.DesktopAlignment = SplashPanel.SplashAlignment.SystemTray
    ///     ' Specifies whether the window should appear animated
    ///     Me.splashPanel1.ShowAnimation = True
    ///     ' The interval for which the panel is to be displayed
    ///     Me.splashPanel1.TimerInterval = 5000
    ///     AddHandler Me.button1.Click, New System.EventHandler(AddressOf button1_Click)
    ///     </code>
    ///     </coderef>
    /// </example>
    [
     Designer(
        typeof(Syncfusion.Windows.Forms.Tools.SplashPanelDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxItem(true),
    ToolboxBitmap(typeof(SplashPanel), "ToolboxIcons.SplashPanel.bmp"),
    Description("Panel derived class which let you design custom splash screens")
    ]
    public class SplashPanel : Panel, ISplashWrapperFormListener
    {
        #region Private delegates

        /// <summary>
        /// Delagate used in ShowSplash for executed Invoke method.
        /// </summary>
        /// <param name="location">Point location</param>
        /// <param name="ownerForm">Owner form</param>
        /// <param name="disableOwner">Disable Owner</param>
        private delegate void ShowSplashDelegate(Point location, Form ownerForm, bool disableOwner);

        /// <summary>
        /// Delagate used in PrepareSplash for executed Invoke method.
        /// </summary>
        /// <param name="location">Pont Location</param>
        /// <param name="ownerForm">Owner form</param>
        /// <returns>Returns bool value</returns>
        private delegate bool PrepareSplashDelegate(Point location, Form ownerForm);

        /// <summary>
        /// Delagate used in HideSplash for executed Invoke method.
        /// </summary>
        /// <param name="splashCloseType">Splash close type</param>
        /// <param name="delayInMilliseconds">Delay InMilliseconds</param>
        private delegate void HideSplashDelegate(SplashCloseType splashCloseType, int delayInMilliseconds);

        #endregion

        #region FIELDS

        /// <summary>
        /// The parent of this splash panel.
        /// </summary>
        private ISplashParent splashParentObject;

        /// <summary>
        /// The wrapper form that will host this splash.
        /// </summary>
        private WrapperForm splashWrapperFormObject;

        /// <summary>
        /// The discrete location specified for displaying the splash panel.
        /// </summary>
        private Point discreetLocationValue;

        /// <summary>
        /// Indicates whether the splash display is to be animated.
        /// </summary>
        private bool showAnimationValue;

        /// <summary>
        /// The desktop alignment.
        /// </summary>
        private SplashAlignment desktopAlignmentValue;

        /// <summary>
        /// The timer used to specify when to dispose the splash window.
        /// </summary>
        private System.Timers.Timer timerObject;

        /// <summary>
        /// The border 3D style.
        /// </summary>
        private Border3DStyle border3dStyleValue;

        /// <summary>
        /// The display interval in milliseconds. Accessors provided
        /// for this field by DisplayTime.
        /// </summary>
        private int timerIntervalValue;

        /// <summary>
        /// The background Brush info.
        /// </summary>
        private BrushInfo backgroundColorValue;

        /// <summary>
        /// Specifies how fast the animation is shown when the window is displayed.
        /// </summary>
        private int animationSpeed;

        /// <summary>
        /// Specifies the number of pixels to increase or decrease in slide style.
        /// </summary>
        private int animationSteps = 3;
        
        /// <summary>
        /// Specifies how the splash was closed.
        /// </summary>
        private SplashCloseType splashCloseType;

        /// <summary>
        /// The sliding style.
        /// </summary>
        private SlideStyle slideStyle = SlideStyle.Default;

        private AnimationDirection animationDirection = AnimationDirection.Default;
        /// <summary>
        /// Indicates whether the SplashPanel should be closed when the mouse is over it.
        /// </summary>
        private bool suspendAutoCloseWhenMouseOver = false;

        /// <summary>
        /// Indicates whether the mouse is over the SplashPanel or any other child control.
        /// </summary>
        private bool mouseOver = false;

        /// <summary>
        /// Used for animation.
        /// </summary>
        public static int AnimationInterval = 50;

        /// <summary>
        /// To enable delayed closing.
        /// </summary>
        private System.Timers.Timer delayTimer;

        /// <summary>
        /// Indicates whether the SplashPanel is to be displayed as the TopMost Window.
        /// </summary>
        private bool showAsTopMost = true;

        /// <summary>
        /// Indicates whether the Splash Panel should be in the Taskbar when shown.
        /// </summary>
        private bool showInTaskbar = false;

        /// <summary>
        /// The icon to be used when displayed in the Taskbar.
        /// </summary>
        private Icon icon = null;

        /// <summary>
        /// The text to be used when in the Taskbar.
        /// </summary>
        private string titleText = String.Empty;

        /// <summary>
        /// Used for converting the background image into a region.
        /// </summary>
        private Color transparentColor = Color.Empty;

        /// <summary>
        /// Region to be passed onto the Wrapper Form when its not null.
        /// </summary>
        private Region formRegion = null;

        /// <summary>
        /// The border type for the Splash Panel.
        /// </summary>
        private SplashBorderType borderType = SplashBorderType.Border3D;

        /// <summary>
        /// Indicates whether the SplashPanel closes when the user clicks on it.
        /// </summary>
        private bool closeOnClick;

        /// <summary>
        /// Indicates whether the SplashPanel can be moved by the user.
        /// </summary>
        private bool allowMove;

        /// <summary>
        /// Indicates whether the SplashPanel can be resized by the user.
        /// </summary>
        private bool allowResize;

        /// <summary>
        /// The width and height of the border area for resizing.
        /// </summary>
        private int borderTestWith = 3, borderTestHeight = 3;

        /// <summary>
        /// Indicates whether the SplashPanel was moved. If this is true the CloseOnClick property will be ignored.
        /// </summary>
        private bool wasMoved = false;

        /// <summary>
        /// Flag for checking the mouse entering and leaving.
        /// </summary>
        internal bool mouseEnter = false;
        /// <summary>
        /// Indicates whether the SplashPanel closes when the user activate the another window.
        /// </summary>
        private bool closeOnLostFocus;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Initializes a new instance of the SplashPanel class.
        /// </summary>
        /// <remarks>
        /// The default value for the <see cref="TimerInterval"/> is set to
        /// 5000 milli seconds.
        /// The splash panel has animation turned and by default will appear in the
        /// middle of the screen.
        /// </remarks>
        public SplashPanel()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SplashPanel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.timerIntervalValue = 5000;
            this.animationSpeed = 10;
            this.showAnimationValue = true;
            this.desktopAlignmentValue = SplashAlignment.Center;
            this.timerObject = new System.Timers.Timer();
            this.delayTimer = new System.Timers.Timer();
            this.border3dStyleValue = Border3DStyle.Bump;
            base.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.backgroundColorValue = new BrushInfo(GradientStyle.Vertical, SystemColors.Highlight, SystemColors.HighlightText);
            CTRLSIZE = this.Size;
        }

        #endregion

        #region Overrides

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCCALCSIZE:
                    // Redraw on resize
                    m.Result = m.WParam == IntPtr.Zero ? IntPtr.Zero : (IntPtr)0x0300;
                    return;
                case NativeMethods.WM_NCHITTEST:
                    if (this.AllowResize)
                    {
                        int x = NativeMethods.LOWORD(m.LParam);
                        int y = NativeMethods.HIWORD(m.LParam);

                        Point pt = PointToClient(new Point(x, y));
                        if (GetCurrentBorderArea(pt) != NativeMethods.HTNOWHERE)
                        {
                            m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                            return;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool value</param>
        protected override void Dispose(bool disposing)
        {
            backgroundColorValue = null;

            if (disposing && splashWrapperFormObject != null)
            {
                splashWrapperFormObject.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region SPLASH ACTION EVENTS

        #region SPLASHPARENT
        /// <summary>
        /// Gets or sets a value indicating SplashPanel can take a class that implements  <see cref="ISplashParent"/>
        /// as its parent for notification purposes.
        /// </summary>
        /// <value>An instance that implements ISplashParent.</value>
        /// <remarks>
        /// You can implement the ISplashParent interface in your classes and set the
        /// SplashParent property of the <see cref="SplashPanel"/> to your class to
        /// get notifications.
        /// </remarks>
        [
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public ISplashParent SplashParent
        {
            get { return this.splashParentObject; }
            set { this.splashParentObject = value; }
        }
        #endregion

        #region BEFORESPLASH
        /// <summary>
        /// Occurs when the splash is about to be shown.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event provides a way for the user to stop a SplashPanel from being
        /// displayed. If you set the <see cref="CancelEventHandler"/> to be true,
        /// the SplashPanel will not be displayed.
        /// </para>
        /// <para>
        /// You can also access the SplashPanel's <see cref="SplashWrapperForm"/> and make 
        /// changes to it if you want to do any modifications.
        /// </para>
        /// </remarks>
        [Category("SplashBehavior")]
        [Description("Occurs when the splash is about to be shown.")]
        public event CancelEventHandler BeforeSplash;

        /// <summary>
        /// Raises the BeforeSplash event.
        /// </summary>
        /// <param name="args">A CancelEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnBeforeSplash method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnBeforeSplash in a derived 
        /// class, be sure to call the base class' OnBeforeSplash method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBeforeSplash(CancelEventArgs args)
        {
            if (BeforeSplash != null)
                BeforeSplash(this, args);
        }

        /// <summary>
        /// Invokes the <see cref="OnBeforeSplash"/> method to raise
        /// the <see cref="BeforeSplash"/> event.
        /// </summary>
        /// <returns>The value set for the <see cref="CancelEventArgs"/>
        /// object used by the event data. A return value of true means the SplashPanel will 
        /// not be displayed.
        /// </returns>
        /// <remarks>
        /// This method creates a <see cref="CancelEventArgs"/> object to use as the
        /// event data and invokes the <see cref="OnBeforeSplash"/> method.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool RaiseBeforeSplashEvent()
        {
            bool cancel = false;
            CancelEventArgs args = new CancelEventArgs(false);

            // First inform the parent
            if (this.SplashParent != null)
                cancel = this.SplashParent.BeforeSplashNotify(this);

            if (cancel == false)
            {
                this.OnBeforeSplash(args);
                cancel = args.Cancel;
                if (cancel == false)
                    this.WireChildEvents();
            }

            return cancel;
        }
        #endregion

        #region SPLASHDISPLAYED

        /// <summary>
        /// Occurs after the SplashPanel has been displayed.
        /// </summary>
        /// <remarks>
        /// This event informs the handler that the SplashPanel is visible now.
        /// You could display a status message or some feedback to the user in
        /// another part of the application.
        /// </remarks>
        [Category("SplashBehavior")]
        [Description("Occurs after the SplashPanel has been displayed.")]
        public event EventHandler SplashDisplayed;

        /// <summary>
        /// Raises the <see cref="SplashDisplayed"/> event.
        /// </summary>
        /// <param name="args">An EventArgs instance containing
        /// data pertaining to this event.</param>
        /// <remarks>
        /// The OnSplashDisplayed method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnSplashDisplayed in a derived 
        /// class, be sure to call the base class' OnSplashDisplayed method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnSplashDisplayed(EventArgs args)
        {
            if (this.SplashDisplayed != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(this.SplashDisplayed, new object[] { this, args });
                }
                else
                {
                    this.SplashDisplayed(this, args);
                }
            }
        }

        /// <summary>
        /// Raises the SplashDisplayed event.
        /// </summary>
        protected void RaiseSplashDisplayedEvent()
        {
            // Inform the parent first.
            if (this.SplashParent != null)
                this.SplashParent.SplashDisplayedNotify(this);

            this.OnSplashDisplayed(EventArgs.Empty);
        }

        #endregion

        #region SPLASHCLOSING

        /// <summary>
        /// Occurs when a SplashPanel is being closed.
        /// </summary>
        /// <remarks>
        /// This event can be handled to prevent a SplashPanel from being closed
        /// and also to do custom processing.
        /// </remarks>
        [Category("SplashBehavior")]
        [Description("Occurs when a SplashPanel is being closed.")]
        public event CancelEventHandler SplashClosing;

        /// <summary>
        /// Raises the SplashClosing event.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplashClosing(CancelEventArgs args)
        {
            if (this.SplashClosing != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(this.SplashClosing, new object[] { this, args });
                }
                else
                {
                    this.SplashClosing(this, args);
                }
            }
        }

        /// <summary>
        /// Invokes the <see cref="OnBeforeSplash"/> method to raise
        /// the <see cref="BeforeSplash"/> event.
        /// </summary>
        /// <returns>The value set for the <see cref="CancelEventArgs"/>
        /// object used by the event data. A return value of true means the SplashPanel will 
        /// not be displayed.
        /// </returns>
        /// <remarks>
        /// This method creates a <see cref="CancelEventArgs"/> object to use as the
        /// event data and invokes the <see cref="OnBeforeSplash"/> method.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool RaiseSplashClosingEvent()
        {
            bool cancel = false;
            CancelEventArgs args = new CancelEventArgs(false);

            // Inform the parent first
            if (this.SplashParent != null)
                cancel = this.SplashParent.SplashClosingNotify(this);

            if (cancel == false)
            {
                this.OnSplashClosing(args);
                cancel = args.Cancel;
            }
            return cancel;
        }

        #endregion

        #region SPLASHCLOSED
        /// <summary>
        /// Occurs when a SplashPanel is closed.
        /// </summary>
        /// <remarks>
        /// Handling this event will tell you whether the splash was
        /// closed or canceled by the user. This, in some cases, will then let you
        /// know whether or not you should accept changes in the splash.
        /// <para>
        /// The delegate for the event is <see cref="SplashClosedEventHandler"/>.
        /// </para>
        /// <para>
        /// You could handle this event and do any post SplashPanel displayed processing
        /// in there. For example, you have an application that displays non obtrusive
        /// message boxes using the SplashPanel class, you can handle this event to
        /// check if the user has made any change or selection.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///             this.splashPanel1.SplashClosing += new Syncfusion.Windows.Forms.Tools.SplashClosedEventHandler(this.splashPanel1_SplashClosing);
        ///             // splashPanel1_SplashClosing event handler
        ///             MessageBox.Show("SplashPanel closing event handler");</code>
        ///             <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\SplashPanelDemo\VB\MainForm.vb" name="SplashPanel SplashClosing event" lang="VB"><code lang="VB">
        ///            AddHandler Me.splashPanel1.SplashClosing, New Syncfusion.Windows.Forms.Tools.SplashClosedEventHandler(AddressOf splashPanel1_SplashClosing)
        ///            ' splashPanel1_SplashClosing event handler
        ///            MessageBox.Show("SplashPanel closing event handler")</code></coderef>
        /// </example>
        [Category("SplashBehavior")]
        [Description("Occurs when a SplashPanel is closed.")]
        public event SplashClosedEventHandler SplashClosed;

        /// <summary>
        /// Raises the SplashClosing event.
        /// </summary>
        /// <param name="args">A SplashClosedEventArgs instance containing
        /// data pertaining to this event.</param>
        /// <remarks>
        /// The OnSplashClosing method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnSplashClosing in a derived 
        /// class, be sure to call the base class' OnSplashClosing method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnSplashClosed(SplashClosedEventArgs args)
        {
            if (this.SplashClosed != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(this.SplashClosed, new object[] { this, args });
                }
                else
                {
                    this.SplashClosed(this, args);
                }
            }
        }

        internal delegate void SplashClosedNotifyHandler(SplashPanel splashPanelObject, SplashCloseType splashCloseType);

        /// <summary>
        /// Raises the SplashCosed event.
        /// </summary>
        /// <param name="args"> EventArgs that contains the event data.</param>
        protected void RaiseSplashClosedEvent(SplashClosedEventArgs args)
        {
            // Inform the parent first
            if (this.SplashParent != null)
            {
                if (this.InvokeRequired)
                {
                    SplashClosedNotifyHandler notifyDelegate = new SplashClosedNotifyHandler(this.SplashParent.SplashClosedNotify);

                    this.BeginInvoke(notifyDelegate, new object[] { this, args.SplashCloseType });
                }
                else
                {
                    this.SplashParent.SplashClosedNotify(this, args.SplashCloseType);
                }
            }

            OnSplashClosed(args);
        }

        #endregion

        #endregion

        #region FOCUS
        /// <summary>
        /// Occurs when the mouse enters the visible part of the SplashPanel or any of its child
        /// controls.
        /// </summary>
        /// <remarks>
        /// This event informs that the mouse has entered the SplashPanel.
        /// </remarks>
        [Category(@"Mouse")]
        [Description("Occurs when the mouse enters the visible part of the SplashPanel or any of its child controls.")]
        public event EventHandler SplashMouseEnter;

        /// <summary>
        /// Occurs when the mouse leaves the visible part of the SplashPanel or any of its child
        /// controls.
        /// </summary>
        /// <remarks>
        /// This event informs that the mouse has left the SplashPanel.
        /// </remarks>
        [Category(@"Mouse")]
        [Description("Occurs when the mouse leaves the visible part of the SplashPanel or any of its child controls.")]
        public event EventHandler SplashMouseLeave;

        /// <summary>
        /// Raises the <see cref="SplashMouseEnter"/> event.
        /// </summary>
        /// <param name="args">An EventArgs instance containing
        /// data pertaining to this event.</param>
        /// <remarks>
        /// The OnSplashMouseEnter method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnSplashMouseEnter in a derived 
        /// class, be sure to call the base class' OnSplashMouseEnter method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected void OnSplashMouseEnter(EventArgs args)
        {
            mouseEnter = true;
            if (this.SplashMouseEnter != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(this.SplashMouseEnter, new object[] { this, args });
                }
                else
                {
                    this.SplashMouseEnter(this, args);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SplashMouseLeave"/> event.
        /// </summary>
        /// <param name="args">An EventArgs instance containing
        /// data pertaining to this event.</param>
        /// <remarks>
        /// The OnSplashMouseLeave method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnSplashMouseLeave in a derived 
        /// class, be sure to call the base class' OnSplashMouseLeave method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected void OnSplashMouseLeave(EventArgs args)
        {
            mouseEnter = false;
            if (this.SplashMouseLeave != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(this.SplashMouseLeave, new object[] { this, args });
                }
                else
                {
                    this.SplashMouseLeave(this, args);
                }
            }
        }

        /// <summary>
        /// Overrides OnMouseEnter to support SuspendAutoCloseOnMouseOver.
        /// </summary>
        /// <param name="e">The event data.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnMouseEnter(EventArgs e)
        {
            HandleMouseEnter();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Suspends the auto closing of the SplashPanel after the 
        /// TimerInterval.
        /// </summary>
        public void SuspendAutoCloseMode()
        {
            if (this.timerObject.Enabled == true)
                this.StopTimer();
        }

        /// <summary>
        /// Restores the auto closing of the SplashPanel.
        /// </summary>
        public void RestoreAutoCloseMode()
        {
            if (this.timerObject.Enabled == false)
                this.StartTimer();
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnMouseLeave(EventArgs e)
        {
            HandleMouseLeave();
            base.OnMouseLeave(e);
        }

        private void HandleMouseLeave()
        {
            mouseEnter = false;
            if (MouseInWindow() == false && IsShowing() == true)
            {
                if (this.mouseOver == true)
                {
                    // Mouse has left the SplashPanel
                    if (this.suspendAutoCloseWhenMouseOver)
                        RestoreAutoCloseMode();

                    this.OnSplashMouseLeave(EventArgs.Empty);
                }
                this.mouseOver = false;
            }
        }

        private void HandleMouseEnter()
        {
            mouseEnter = true;
            if (MouseInWindow() == true && IsShowing() == true)
            {
                if (this.mouseOver == false)
                {
                    // Mouse has entered the SplashPanel
                    if (this.suspendAutoCloseWhenMouseOver)
                        SuspendAutoCloseMode();

                    this.OnSplashMouseEnter(EventArgs.Empty);
                }
                this.mouseOver = true;
            }
        }

        private void HandleChildMouseEnter(object sender, System.EventArgs e)
        {
            HandleMouseEnter();
        }

        private void HandleChildMouseLeave(object sender, System.EventArgs e)
        {
            HandleMouseLeave();
        }

        private void HandleChildMouseDown(object sender, MouseEventArgs e)
        {
            HandleMouseDown(e, true);
        }

        private void HandleChildMouseUp(object sender, MouseEventArgs e)
        {
            HandleMouseUp(e);
        }

        private void HandleChildMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePt = new Point(e.X, e.Y);
            Control c = sender as Control;
            Point screenPt = c.PointToScreen(mousePt);
            mousePt = this.PointToClient(screenPt);
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, mousePt.X, mousePt.Y, e.Delta);
            HandleMouseMove(mea);
        }

        private bool MouseInWindow()
        {
            Point mousePoint = this.PointToClient(Control.MousePosition);
            return this.ClientRectangle.Contains(mousePoint);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SplashPanel should not be closed when the mouse is over it.
        /// </summary>
        [
        Category("Behavior"),
        Description("Specifies if the SplashPanel should not be closed when the mouse is over it."),
        DefaultValue(false)
        ]
        public bool SuspendAutoCloseWhenMouseOver
        {
            get
            {
                return this.suspendAutoCloseWhenMouseOver;
            }

            set
            {
                this.suspendAutoCloseWhenMouseOver = value;
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void WireChildEvents()
        {
            foreach (Control c in this.Controls)
            {
                c.MouseEnter += new EventHandler(this.HandleChildMouseEnter);
                c.MouseLeave += new EventHandler(this.HandleChildMouseLeave);

                c.MouseDown += new MouseEventHandler(this.HandleChildMouseDown);
                c.MouseUp += new MouseEventHandler(this.HandleChildMouseUp);
                c.MouseMove += new MouseEventHandler(this.HandleChildMouseMove);
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void UnWireChildEvents()
        {
            foreach (Control c in this.Controls)
            {
                c.MouseLeave -= new EventHandler(this.HandleChildMouseLeave);
                c.MouseEnter -= new EventHandler(this.HandleChildMouseEnter);

                c.MouseDown -= new MouseEventHandler(this.HandleChildMouseDown);
                c.MouseUp -= new MouseEventHandler(this.HandleChildMouseUp);
                c.MouseMove -= new MouseEventHandler(this.HandleChildMouseMove);
            }
            this.StopTimer();
        }

        private Point mouseDown;
        private bool bMoveDragging = false;

        /// <summary>
        /// Raises the mouse down event.
        /// </summary>
        /// <param name="mea">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs mea)
        {
            HandleMouseDown(mea, false);
            base.OnMouseDown(mea);
        }

        private void HandleMouseDown(MouseEventArgs mea, bool child)
        {
            if (this.AllowMove && child == false)
            {
                this.Capture = true;
                this.bMoveDragging = true;
                this.mouseDown = new Point(mea.X, mea.Y);
            }
        }

        /// <summary>
        /// Raises the MouseUp event.
        /// </summary>
        /// <param name="mea">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseUp(MouseEventArgs mea)
        {
            HandleMouseUp(mea);
            base.OnMouseUp(mea);
        }

        private void HandleMouseUp(MouseEventArgs mea)
        {
            this.Capture = false;
            this.bMoveDragging = false;
        }

        /// <summary>
        /// Raises the MouseMove event.
        /// </summary>
        /// <param name="mea">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseMove(MouseEventArgs mea)
        {
            HandleMouseMove(mea);
            base.OnMouseMove(mea);
        }

        private void HandleMouseMove(MouseEventArgs mea)
        {
            if (this.bMoveDragging)
            {
                if (this.SplashForm != null)
                {
                    Point location = this.SplashForm.Location;

                    location.X += mea.X - mouseDown.X;
                    location.Y += mea.Y - mouseDown.Y;

                    this.SplashForm.Location = location;
                    this.wasMoved = true;
                }
            }
        }

        internal int GetCurrentBorderArea(Point pt)
        {
            Rectangle clientRect = this.ClientRectangle;
            Rectangle clientWithoutBorder = this.ClientRectangle;
            clientWithoutBorder.Inflate(-1 * this.borderTestWith, -1 * this.borderTestHeight);

            if (clientRect.Contains(pt) && clientWithoutBorder.Contains(pt) == false)
            {
                // Mouse point is within the border

                // TopRight
                Rectangle testRect = new Rectangle(
                    clientRect.X + clientRect.Width - this.borderTestWith,
                    clientRect.Y,
                    this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTTOPRIGHT;

                // Right
                testRect = new Rectangle(
                    clientRect.X + clientRect.Width - this.borderTestWith,
                    clientRect.Y + this.borderTestHeight,
                    this.borderTestWith,
                    clientRect.Height - 2 * this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTRIGHT;

                // BottomRight
                testRect = new Rectangle(
                    clientRect.X + clientRect.Width - this.borderTestWith,
                    clientRect.Y + clientRect.Height - this.borderTestHeight,
                    this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTBOTTOMRIGHT;

                // Bottom
                testRect = new Rectangle(
                    clientRect.X + this.borderTestWith,
                    clientRect.Y + clientRect.Height - this.borderTestHeight,
                    clientRect.Width - 2 * this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTBOTTOM;

                // BottomLeft
                testRect = new Rectangle(
                    clientRect.X,
                    clientRect.Y + clientRect.Height - this.borderTestHeight,
                    this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTBOTTOMLEFT;

                // Left
                testRect = new Rectangle(
                    clientRect.X,
                    clientRect.Y + this.borderTestHeight,
                    this.borderTestWith,
                    clientRect.Height - 2 * this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTLEFT;

                // TopLeft
                testRect = new Rectangle(
                    clientRect.X,
                    clientRect.Y,
                    this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTTOPLEFT;

                // Top
                testRect = new Rectangle(
                    clientRect.X + this.borderTestWith,
                    clientRect.Y,
                    clientRect.Width - 2 * this.borderTestWith,
                    this.borderTestHeight);

                if (testRect.Contains(pt))
                    return NativeMethods.HTTOP;
            }

            return NativeMethods.HTNOWHERE;
        }

        #endregion

        #region APPEARANCE

        private Region GetRegionFromImage(Image backgroundImage, Color transparentColor)
        {
            Bitmap backgroundBitmap = new Bitmap(backgroundImage);
            Color trColor = Color.FromArgb((int)transparentColor.R, (int)transparentColor.G, (int)transparentColor.B);
            GraphicsPath visiblePath = new GraphicsPath();
            int bitmapHeight = backgroundBitmap.Height;
            int bitmapWidth = backgroundBitmap.Width;
            int currentPos = 0;
            Rectangle partRect = new Rectangle(0, 0, 0, 0);

            for (int heightIndex = 0; heightIndex < bitmapHeight; heightIndex++)
            {
                for (int widthIndex = 0; widthIndex < bitmapWidth; widthIndex++)
                {
                    // Add to the region if the color is different from the transparent color
                    if (backgroundBitmap.GetPixel(widthIndex, heightIndex) != trColor)
                    {
                        currentPos = widthIndex;
                        while ((widthIndex < bitmapWidth) && (backgroundBitmap.GetPixel(widthIndex, heightIndex) != trColor))
                            widthIndex++;
                        partRect = new Rectangle(currentPos, heightIndex, widthIndex - currentPos, 1);
                        visiblePath.AddRectangle(partRect);
                    }
                }
            }

            Region formRegion = new Region(visiblePath);
            visiblePath.Dispose();
            backgroundBitmap.Dispose();
            return formRegion;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SplashPanel is to be shown in the Taskbar.
        /// </summary>
        [
        DefaultValue(false),
        Description("Specifies if the SplashPanel is to be shown in the Taskbar")
        ]
        public bool ShowInTaskbar
        {
            get
            {
                return this.showInTaskbar;
            }

            set
            {
                this.showInTaskbar = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon for the SplashPanel when displayed in the Taskbar.
        /// </summary>
        [
        DefaultValue(null),
        Description("Specifies the icon for the SplashPanel when displayed in the Taskbar")
        ]
        public Icon FormIcon
        {
            get
            {
                return this.icon;
            }

            set
            {
                this.icon = value;
            }
        }

        /// <summary>
        /// Gets or sets the text when displayed in the Taskbar.
        /// </summary>
        [
        Browsable(true),
        Description("Specifies the text when displayed in the Taskbar")
        ]
        public new string Text
        {
            get
            {
                return this.titleText;
            }

            set
            {
                this.titleText = value;
            }
        }

        private bool ShouldSerializeText()
        {
            if (this.Text == String.Empty)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Draws the background of the panel using the information in the 
        /// <see cref="BackgroundColor"/> property.
        /// </summary>
        /// <param name="g">The graphics object to draw on.</param>
        /// <remarks>
        /// Override this virtual function if you want to draw a different background 
        /// on change the way the drawing is done.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void DrawBackground(Graphics g)
        {
            NativeMethods.RECT rc = new NativeMethods.RECT();
            if (NativeMethods.GetClientRect(this.Handle, ref rc))
            {
                Rectangle rcPaint = new Rectangle(rc.left, rc.top, rc.Width, rc.Height);
                BrushPaint.FillRectangle(g, rcPaint, this.BackgroundColor);
            }
        }

        /// <summary>
        /// Gets or sets the background color and other styles.
        /// </summary>
        /// <remarks>
        /// The <see cref="SplashPanel"/> provides this property to enable specialized
        /// custom gradient backgrounds for the splash screens you create.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("The background gradient and other styles can be set.")
        ]
        public BrushInfo BackgroundColor
        {
            get
            {
                return this.backgroundColorValue;
            }

            set
            {
                this.backgroundColorValue = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the SlideStyle for the SplashPanel.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("The sliding style for the SplashPanel."),
        DefaultValue(SlideStyle.Default)
        ]
        public SlideStyle SlideStyle
        {
            get
            {
                return this.slideStyle;
            }

            set
            {
                this.slideStyle = value;
            }
        }
        private MarqueePosition marquePosition = MarqueePosition.BottomLeft;
        public MarqueePosition MarqueePosition
        {
            get 
            {
                return marquePosition;
            }
            set
            { 
                marquePosition = value;
            }
        }
        private SplashPanelMarqueeDirection marqueeDirection = SplashPanelMarqueeDirection.LeftToRight;
        public SplashPanelMarqueeDirection MarqueeDirection
        {
            get
            { 
                return marqueeDirection;
            }
            set
            {
                marqueeDirection = value;
                if (this.MarqueePosition == Tools.MarqueePosition.BottomLeft && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.RightToLeft)
                {
                    marquePosition = Tools.MarqueePosition.BottomRight;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.BottomLeft && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.TopToBottom)
                {
                    marquePosition = Tools.MarqueePosition.Topleft;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.BottomRight && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.LeftToRight)
                {
                    marquePosition = Tools.MarqueePosition.BottomLeft;
                }
                else if (this.marquePosition == Tools.MarqueePosition.BottomRight && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.TopToBottom)
                {
                    marquePosition = Tools.MarqueePosition.Topright;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.Topleft && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.RightToLeft)
                {
                    marquePosition = Tools.MarqueePosition.Topright;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.Topleft && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.BottomToTop)
                {
                    marquePosition = Tools.MarqueePosition.BottomLeft;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.Topright && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.BottomToTop)
                {
                    marquePosition = Tools.MarqueePosition.BottomRight;
                }
                else if (this.MarqueePosition == Tools.MarqueePosition.Topright && this.MarqueeDirection == Tools.SplashPanelMarqueeDirection.LeftToRight)
                {
                    marquePosition = Tools.MarqueePosition.Topleft;
                }
            }
        }
        
        public AnimationDirection AnimationDirection
        {
            get 
            {
                return animationDirection;
            }
            set
            {
                if (value == Tools.AnimationDirection.Default)
                    animationDirection = value;
               else  if (value == AnimationDirection.LeftToRight && desktopAlignmentValue == SplashAlignment.LeftBottom || desktopAlignmentValue == SplashAlignment.LeftTop)
                {
                    animationDirection = value;
                }
                else  if(value == AnimationDirection.RightToLeft && desktopAlignmentValue == SplashAlignment.RightBottom || desktopAlignmentValue == SplashAlignment.RightTop)
              
                {
                    animationDirection = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the SplashPanel is shown as a TopMost
        /// window when displayed.
        /// </summary>
        /// <value>
        /// True if the SplashPanel is displayed as a TopMost window; false otherwise. The default is true.
        /// </value>
        /// <remarks>
        /// Displaying the SplashPanel as a TopMost window makes the SplashPanel appear on top of
        /// all other windows.
        /// <para>
        /// If this property is false and the SplashPanel is displayed in non modal mode, the
        /// SplashPanel might be hidden by the Form displaying the SplashPanel. If you want the 
        /// SplashPanel to be the TopMost window with respect to the application/Form displaying 
        /// it only, you should display the SplashPanel modally with this property set to false.
        /// </para>
        /// </remarks>
        [
        Category("Appearance"),
        Description("Specifies if the SplashPanel is to be displayed as a TopMost window."),
        DefaultValue(true)
        ]
        public bool ShowAsTopMost
        {
            get
            {
                return this.showAsTopMost;
            }

            set
            {
                this.showAsTopMost = value;
            }
        }

        /// <summary>
        /// Gets or sets the 3D border for the SplashPanel.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="SplashPanel"/> class provides 3D border styles for its
        /// borders.
        /// </para>
        /// <para>
        /// You can set the border style for the SplashPanel to any of the
        /// values supported by the <see cref="Border3DStyle"/> enumeration. Setting
        /// the BorderStyle property to <see cref="Border3DStyle.Adjust"/> results in
        /// no border being visible.
        /// </para>
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("The 3D border for the splash panel."),
        DefaultValue(Border3DStyle.Bump)
        ]
        public new Border3DStyle BorderStyle
        {
            get
            {
                return this.border3dStyleValue;
            }

            set
            {
                this.border3dStyleValue = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the type of border.
        /// </summary>
        [
        Description("Specifies the type of border."),
        DefaultValue(SplashBorderType.Border3D)
        ]
        public SplashBorderType BorderType
        {
            get
            {
                return this.borderType;
            }

            set
            {
                this.borderType = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed at which the animation unfolds on the screen and the SplashPanel becomes visible.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The same speed is also used when the <see cref="SplashPanel"/> is closed.
        /// To achieve the best animation effect, the animation speed should be set relative with
        /// respect to the height and width of the <see cref="SplashPanel"/>.
        /// </para>
        /// <para>
        /// The value for the animation speed is restricted to be between 5 and 50.
        /// </para>
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("The speed at which the animation unfolds on the screen and the SplashPanel becomes visible."),
        DefaultValue(10)
        ]
        public int AnimationSpeed
        {
            get
            {
                return this.animationSpeed;
            }

            set
            {
                if (value >= 2 && value <= 50)
                    this.animationSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of pixels at which the animation unfolds in sliding style.
        /// </summary>
        [
       DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
       Category("Appearance"),
       Description("Gets or sets the number of pixels at which the animation unfolds in sliding style."),
       DefaultValue(3)
       ]
        public int AnimationSteps
        {
            get 
            {
                return animationSteps; 
            }
            set 
            {
                if(value >= 1)
                    animationSteps = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window display should be animated.
        /// </summary>
        /// <remarks>
        /// Set this property to true if you want the splash window to appear
        /// in an animated manner.
        /// <para>
        /// See the <see cref="AnimationSpeed"/> property for changing the speed of animation.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("Specifies if the window display should be animated."),
        DefaultValue(true)
        ]
        public bool ShowAnimation
        {
            get
            {
                return this.showAnimationValue;
            }

            set
            {
                this.showAnimationValue = value;
            }
        }

        /// <summary>
        /// Gets or sets how the splash screen is aligned when it appears 
        /// initially with respect to the desktop.
        /// </summary>
        /// <remarks>
        /// <see cref="SplashAlignment"/> lists the possible values for this property.
        /// <para>The default value for the DesktopAlignment is <see cref="SplashAlignment.Center"/>
        /// which sets the SplashPanel to appear in the middle of the screen.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        DefaultValue(SplashAlignment.Center),
        Description("Specifies how the splash screen has to be aligned when it appears initially with respect to the desktop.")
        ]
        public SplashAlignment DesktopAlignment
        {
            get
            {
                return this.desktopAlignmentValue;
            }

            set
            {
                this.desktopAlignmentValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the period of time the splash window should be visible for.
        /// </summary>
        /// <remarks>
        /// The unit of measurement for this is in milliseconds. The default value is
        /// 5000 milliseconds which translates to 5 seconds.
        /// <para>
        /// The time is taken into account after the window appears on the
        /// screen.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Behavior"),
        DefaultValue(5000),
        Description("Gets or sets the period of time the splash window should be visible for.")
        ]
        public int TimerInterval
        {
            get
            {
                return this.timerIntervalValue;
            }

            set
            {
                this.timerIntervalValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="SplashWrapperForm"/> that will be
        /// used to host this SplashPanel when displayed.
        /// </summary>
        /// <value>The SplashWrapperForm object that will host this SplashPanel.</value>
        /// <remarks>
        /// SplashWrapperForm is the top level Form based control that hosts
        /// this Splash Panel when displayed.
        /// <para>
        /// The SplashPanel usually creates a custom SplashWrapperForm when it is asked to display 
        /// itself. However, you can provide your own SplashWrapperForm if you have a customized
        /// version.
        /// </para>
        /// <para>
        /// You could also get a reference to the <see cref="SplashWrapperForm"/>
        /// that the SplashPanel uses by default and make changes to it. 
        /// The SplashPanel creates a default SplashWrapperForm when there is no 
        /// SplashWrapperForm supplied to it, but ShowSplash was called to show the splash screen.
        /// Hence, the best place to get the default SplashWrapperForm
        /// associated with this SplashPanel is in the <see cref="BeforeSplash"/> event handler.
        /// </para>
        /// <note type="note">This property is not available at runtime.</note>
        /// </remarks>
        [
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        private WrapperForm SplashWrapperForm
        {
            get
            {
                return this.splashWrapperFormObject;
            }

            set
            {
                this.splashWrapperFormObject = value;
            }
        }

        /// <summary>
        /// Gets the splash form.
        /// </summary>
        /// <value>The splash form.</value>
        [
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public Form SplashForm
        {
            get
            {
                if (this.splashWrapperFormObject == null)
                    return null;
                else
                    return this.splashWrapperFormObject;
            }
        }

        /// <summary>
        /// Gets or sets the transparent color for the background.
        /// </summary>
        [
        Description("Specifies the transparent color for the background.")
        ]
        public Color TransparentColor
        {
            get
            {
                return this.transparentColor;
            }

            set
            {
                if (value == Color.Empty)
                {
                    this.transparentColor = Color.Empty;
                    this.formRegion = null;
                }
                else if (this.transparentColor != value)
                {
                    this.transparentColor = value;
                    this.RefreshRegionFromImage();
                }
            }
        }

        private bool ShouldSerializeTransparentColor()
        {
            if (this.TransparentColor == Color.Empty)
                return false;
            else
                return true;
        }

        private void ResetTransparentColor()
        {
            this.TransparentColor = Color.Empty;
        }

        /// <summary>
        /// Refreshes the region from BackgroundImage.
        /// </summary>
        public void RefreshRegionFromImage()
        {
            if (this.BackgroundImage != null)
                this.formRegion = GetRegionFromImage(this.BackgroundImage, this.TransparentColor);
        }

        /// <summary>
        /// Gets or sets the backcolor for the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false)]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// Overrides OnPaintBackground to paint the user specified <see cref="BackgroundColor"/>
        /// as the background.
        /// </summary>
        /// <param name="pe">The PaintEventArgs object that has the event data.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            if (this.BackgroundImage == null)
            {
                if (this.BackgroundColor != null)
                    this.DrawBackground(pe.Graphics);
                else
                    base.OnPaintBackground(pe);
            }
            else
                base.OnPaintBackground(pe);
        }

        #endregion

        #region ISPLASHWRAPPERFORMLISTENER

        /// <summary>
        /// This method implements the <see cref="ISplashWrapperFormListener"/> interface
        /// to receive notification from the <see cref="SplashWrapperForm"/> when the Splash window has been
        /// displayed.
        /// </summary>
        /// <remarks>
        /// The SplashPanel receives notification from the SplashWrapperForm that actually displays this
        /// SplashPanel on the desktop that the SplashPanel has been displayed. This is needed for the SplashPanel
        /// to start its internal timer so that the SplashPanel can be closed in the time interval set in <see cref="TimerInterval"/>.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SplashFormDisplayedNotify()
        {
            this.StartTimer();
            this.RaiseSplashDisplayedEvent();
        }

        /// <summary>
        /// This methods is an implementation of the ISplashWrapperFormListener interface for 
        /// receiving notification from the <see cref="SplashWrapperForm"/> when the window
        /// is closed.
        /// </summary>
        /// <remarks>The <see cref="SplashClosing"/> event is raised in response to this
        /// method being invoked.</remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SplashFormClosedNotify()
        {
            this.UnWireChildEvents();
            this.RaiseSplashClosedEvent(new SplashClosedEventArgs(this.splashCloseType));
        }
        #endregion

        #region BEHAVIOR

        /// <summary>
        /// Displays the SplashPanel.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void Show()
        {
            base.Show();
        }

        /// <summary>
        /// Indicates whether the splash is currently displayed.
        /// </summary>
        /// <returns>True indicates splash is displayed; false otherwise.</returns>
        /// <remarks>
        /// The SplashPanel considered visible if the <see cref="SplashWrapperForm"/> that embeds this
        /// SplashPanel is visible.
        /// The <see cref="HideSplash"/> method uses this method to ascertain if the SplashPanel is
        /// indeed being displayed.
        /// </remarks>
        public bool IsShowing()
        {
            if (this.SplashWrapperForm != null)
                return this.SplashWrapperForm.Visible;
            else
                return false;
        }

        /// <summary>
        /// Displays the SplashPanel at the specified location.
        /// </summary>
        /// <param name="location">A  point in screen coordinates. The value can be Point.Empty.</param>
        /// <param name="ownerForm">The form that will embed the SplashForm. This can be null.</param>
        /// <param name="disableOwner">Indicates whether the owner form is to be disabled.</param>
        public virtual void ShowSplash(Point location, Form ownerForm, bool disableOwner)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowSplashDelegate(ShowSplash), new object[] { location, ownerForm, disableOwner });
            }
            else
            {
                this.PrepareSplash(location, ownerForm);
                this.SplashWrapperForm.ShowSplash(disableOwner);
            }
            this.RefreshRegionFromImage();
        }

        /// <summary>
        /// Displays the SplashPanel.
        /// </summary>
        public void ShowSplash()
        {
            this.ShowSplash(Point.Empty, null, false);
        }

        /// <summary>
        /// Overloaded. Displays the Splash Panel as a modal dialog.
        /// </summary>
        /// <param name="location">The location at which the Splash Panel is to be displayed.</param>
        /// <param name="ownerForm">The owner form.</param>
        /// <returns>The DialogResult value.</returns>
        public DialogResult ShowDialogSplash(Point location, Form ownerForm)
        {
            bool splashPrepare = PrepareSplash(location, ownerForm);
            if (!splashPrepare)
                return DialogResult.None;

            return this.SplashWrapperForm.ShowDialogSplash();
        }

        /// <summary>
        /// Displays the Splash Panel as a modal dialog.
        /// </summary>
        /// <param name="ownerForm">The owner form.</param>
        /// <returns>The DialogResult value.</returns>
        public DialogResult ShowDialogSplash(Form ownerForm)
        {
            return this.ShowDialogSplash(Point.Empty, ownerForm);
        }

        private bool PrepareSplash(Point location, Form ownerForm)
        {
            if (this.InvokeRequired)
            {
                return (bool)this.Invoke(new PrepareSplashDelegate(PrepareSplash), new object[] { location, ownerForm });
            }
            else
            {
                if (this.IsShowing() == true)
                    return false;

                if (this.SplashWrapperForm == null)
                {
                    this.SplashWrapperForm = new WrapperForm(this);
                }
                else if (!this.SplashWrapperForm.Controls.Contains(this))
                {
                    this.SplashWrapperForm.AttachSplash();
                }

                this.SplashWrapperForm.SplashWrapperFormOwner = ownerForm;

                if (this.RaiseBeforeSplashEvent() == true)
                    return false;

                this.discreetLocationValue = location;

                // See if we need to refresh the region
                if (this.formRegion == null && this.BackgroundImage != null && this.TransparentColor != Color.Empty)
                    this.RefreshRegionFromImage();

                // Set the region if needed
                if (this.formRegion != null)
                {
                    this.SplashWrapperForm.Region = this.formRegion;
                    this.SplashWrapperForm.CustomRegion = true;
                    this.SplashWrapperForm.ComputeControlLocation();
                }

                return true;
            }
        }

        /// <summary>
        /// Hides a splash with the specified SplashCloseType mode.
        /// </summary>
        /// <param name="splashCloseType">A <see cref="SplashCloseType"/> value.</param>
        /// <param name="delayInMilliseconds">The time period for which the close should be delayed.</param>
        /// <remarks>
        /// This method will close the SplashPanel when it is being displayed. It invokes
        /// <see cref="IsShowing"/> to ascertain that the SplashPanel is being displayed
        /// before any further action is taken.
        /// <para>
        /// The method will have no effect if the SplashPanel is not currently being displayed.
        /// </para>
        /// </remarks>
        public virtual void HideSplash(SplashCloseType splashCloseType, int delayInMilliseconds)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new HideSplashDelegate(HideSplash), new object[] { splashCloseType, delayInMilliseconds });
            }
            else
            {
                if (this.IsShowing() == true)
                {
                    this.timerObject.Stop();
                    this.splashCloseType = splashCloseType;
                    if (delayInMilliseconds > 0)
                    {
                        this.delayTimer.Interval = delayInMilliseconds;
                        this.delayTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.HandleDelayTimerElapsedEvent);
                        this.delayTimer.Enabled = true;
                        this.delayTimer.Start();
                    }
                    else
                    {
                        this.SplashWrapperForm.HideSplash();
                    }
                }
            }
        }

        /// <summary>
        /// Hides a splash with the specified SplashCloseType mode.
        /// </summary>
        /// <param name="splashCloseType">A <see cref="SplashCloseType"/> value.</param>
        public virtual void HideSplash(SplashCloseType splashCloseType)
        {
            this.HideSplash(splashCloseType, 0);
        }

        /// <summary>
        /// Cancels and hides a splash that is open.
        /// </summary>
        /// <remarks>
        /// Call this method to Hide a SplashPanel window that is being displayed currently.
        /// The SplashPanel will be closed with the <see cref="SplashCloseType "/> set to
        /// <see cref="SplashCloseType.Canceled"/>. This method actually invokes the overloaded
        /// version that takes a <see cref="SplashCloseType "/> as the parameter.
        /// </remarks>
        public virtual void HideSplash()
        {
            this.HideSplash(SplashCloseType.Canceled);
        }

        /// <summary>
        /// Stops the timer object that is used to track the appearance of the SplashPanel.
        /// </summary>
        /// <remarks>
        /// This method stops the <see cref="Timer"/> and closes the timer object.
        /// This is invoked by the <see cref="HandleTimerEvent"/> method and you will not need to
        /// call this directly.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void StopTimer()
        {
            if (this.timerObject != null && this.IsShowing() == true)
            {
                this.timerObject.Elapsed -= new System.Timers.ElapsedEventHandler(HandleTimerEvent);
                this.timerObject.Stop();
                this.timerObject.Close();
            }
        }
        // For closing the SplashPanel Manually
        private bool autohide =true;
        /// <summary>
        /// Gets or sets a value indicating whether the SplashPanel should auto hide.
        /// </summary>
        [
         Browsable(true),
        Category("Behavior"),
        Description("Gets or sets a value indicating whether the SplashPanel should auto hide."),
        DefaultValue(true)
        ]
        public bool Autohide
        {
            get
            {
                return autohide;
            }
            set
            {
                autohide = value;
            }
        }
        /// <summary>
        /// Starts the <see cref="Timer"/> object that will be used to show the
        /// SplashPanel for a specified period of time.
        /// </summary>
        /// <remarks>Change the <see cref="TimerInterval"/> property if you want to
        /// change the period for which the SplashPanel is displayed.
        /// <para>This method is invoked by <see cref="SplashFormDisplayedNotify"/> and you will not need to
        /// call this directly.</para>
        /// <para>
        /// This method will do nothing if the <see cref="TimerInterval"/> property is
        /// set to -1.
        /// </para>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void StartTimer()
        {
            if (this.TimerInterval != -1 &&autohide)
            {
                if (this.timerObject != null && this.timerObject.Enabled == false)
                {
                    this.timerObject.Elapsed += new System.Timers.ElapsedEventHandler(HandleTimerEvent);
                    this.timerObject.Interval = this.timerIntervalValue;
                    this.timerObject.Start();
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="System.Timers.Timer.Elapsed"/> event of the <see cref="Timer"/> object
        /// that is used to track the time period for displaying the SplashPanel.
        /// </summary>
        /// <param name="sender">The Timer object.</param>
        /// <param name="e">The event data for the Timer's elapsed event.</param>
        /// <remarks>
        /// This method hides the <see cref="SplashPanel"/> and closes the <see cref="Timer"/>
        /// object.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void HandleTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool cancel = false;
            cancel = this.RaiseSplashClosingEvent();
            if (cancel == false)
            {
                this.StopTimer();
                HideSplash(SplashCloseType.TimedOut);
            }
        }

        private void HandleDelayTimerElapsedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.delayTimer.Stop();
            this.delayTimer.Enabled = false;
            this.SplashWrapperForm.HideSplash();
        }

        /// <summary>
        /// Gets or sets the location to display the splash window. This is a <see cref="Point"/>
        /// value that is in screen coordinates.
        /// </summary>
        /// <remarks>
        /// You can display a discrete location for the <see cref="SplashPanel"/>
        /// to be displayed at. The location parameter passed to the constructor
        /// of SplashPanel is used to set this value initially.
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("Gets or sets the location to display the splash window. This is a Point value that is in screen coordinates.")
        ]
        public Point DiscreetLocation
        {
            get
            {
                return this.discreetLocationValue;
            }

            set
            {
                this.discreetLocationValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CloseOnClick property closes the SplashPanel. If the user clicks inside the
        /// SplashPanel and moves the SplashPanel (this is allowed if the AllowMove property is set to true),
        /// the SplashPanel will not be closed. The SplashPanel will not be closed if the click is on a Child 
        /// control.
        /// </summary>
        [DefaultValue(false)]
        [Description(@"The SplashPanel will be closed if this set to true and the user clicks the SplashPanel.")]
        public bool CloseOnClick
        {
            get
            {
                return this.closeOnClick;
            }

            set
            {
                this.closeOnClick = value;
            }
        }

        [DefaultValue(false)]
        [Description(@"The SplashPanel will be closed if this set to true and the user change the focus from the splash.")]

        public bool CloseOnLostFocus
        {
            get
            {
                return this.closeOnLostFocus;
            }
            set
            {
                this.closeOnLostFocus = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Click"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            HandleClick();
            base.OnClick(e);
        }

        private void HandleClick()
        {
            if (this.CloseOnClick && this.IsShowing())
            {
                // Do not close if the user just moved the SplashPanel. To enable the properties AllowMove and CloseOnClick to set to true.
                if (this.wasMoved)
                    this.wasMoved = false;
                else
                    this.HideSplash();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AllowMove property allows the user to click within the SplashPanel and 
        /// move the SplashPanel on the screen. 
        /// </summary>
        [DefaultValue(false)]
        [Description(@"Indicates whether the AllowMove property allows the user to click within the SplashPanel and move the SplashPanel on the screen.")]
        public bool AllowMove
        {
            get
            {
                return this.allowMove;
            }

            set
            {
                this.allowMove = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AllowResize property allows the user to resize the SplashPanel.
        /// Resize handles will be displayed when the user moves the mouse near the border
        /// of the SplashPanel.
        /// </summary>
        [DefaultValue(false)]
        [Description(@"The SplashPanel can be resized by the user if this is set to true.")]
        public bool AllowResize
        {
            get
            {
                return this.allowResize;
            }

            set
            {
                this.allowResize = value;
            }
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
        ///gets or Sets the touchmode 
        /// </summary>
		[DefaultValue(false)]
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

        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");
                fi.SetValue(ctrl, this.EnableTouchMode, null);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion
    }

    #endregion

    public class SplashPanelDesigner : System.Windows.Forms.Design.ScrollableControlDesigner
    {
        public SplashPanelDesigner()
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
                        new SplashPanelActionList(this.Component));
                }
                return actionLists;
            }
        }

#endif
    }

    #region SPLASHCLOSEDEVENTHANDLER

    /// <summary>
    /// Handles the SplashClosing event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A SplashClosedEventArgs that 
    /// contains the event data.</param>
    public delegate void SplashClosedEventHandler(object sender, SplashClosedEventArgs args);

    #endregion

    #region GetComputedSizeEventHandler
    /// <summary>
    /// Delegate used for the thread-safe invoke of the GetComputedSizeInternal method.
    /// </summary>
    /// <param name="controlSize">Control size</param>
    /// <returns>return size</returns>
    internal delegate Size GetComputedSizeEventHandler(Size controlSize);
    #endregion

    #region SPLASHCLOSEDEVENTARGS

    /// <summary>
    /// Provides data for the <see cref="SplashPanel.SplashClosing"/> event.
    /// </summary>
    /// <remarks>
    /// The SplashClosed event is raised when a <see cref="SplashPanel"/>
    /// is closed. The <see cref="SplashCloseType"/> specifies the manner
    /// in which the SplashPanel was closed.
    /// <para>
    /// See the <see cref="SplashPanel.SplashClosing"/> event for more information.
    /// </para>
    /// </remarks>
    public class SplashClosedEventArgs
    {
        /// <summary>
        /// The Splash close type object.
        /// </summary>
        private SplashCloseType splashCloseType;

        /// <summary>
        /// Initializes a new instance of the SplashClosedEventArgs class.
        /// </summary>
        /// <param name="splashCloseType">A SplashCloseType value.</param>
        /// <remarks>
        /// The SplashCloseType property is initialized with the 
        /// value passed in.
        /// </remarks>
        public SplashClosedEventArgs(SplashCloseType splashCloseType)
        {
            this.splashCloseType = splashCloseType;
        }

        /// <summary>
        /// Gets the <see cref="SplashCloseType"/> value indicating the way in which 
        /// the Splash was closed.
        /// </summary>
        /// <remarks>
        /// The SplashCloseType value indicates how the SplashPanel was closed.
        /// </remarks>
        public SplashCloseType SplashCloseType
        {
            get { return this.splashCloseType; }
        }
    }

    #endregion

    #region ISPLASHPARENT
    /// <summary>
    /// A generic interface that defines a SplashPanel parent. 
    /// </summary>
    /// <remarks>Any object/Control that wants to act as a <see cref="SplashPanel"/>
    /// parent should implement this interface.
    /// <para>
    /// The SplashPanel provides an easy way to display different types of messages
    /// and also collect user input in an unobtrusive manner. This interface
    /// allows your class to get notifications from the SplashPanel object 
    /// you want to monitor without handling events.
    /// </para>
    /// </remarks>
    public interface ISplashParent
    {
        /// <summary>
        /// Invoker for notifying the splash parent before the splash panel is 
        /// displayed.
        /// </summary>
        /// <param name="splashPanelObject">The splash panel object.</param>
        /// <remarks>
        /// This method will be called by the <see cref="SplashPanel.OnBeforeSplash"/> 
        /// method. 
        /// </remarks>
        /// <returns>Return bool value</returns>
        bool BeforeSplashNotify(SplashPanel splashPanelObject);

        /// <summary>
        /// Invoker for notifying the splash parent after the splash panel
        /// is displayed.
        /// </summary>
        /// <param name="splashPanelObject">The splash panel object</param>
        /// <remarks>
        /// This method will be called by the <see cref="SplashPanel.OnSplashDisplayed"/> 
        /// method. 
        /// </remarks> 
        void SplashDisplayedNotify(SplashPanel splashPanelObject);

        /// <summary>
        /// Invoker for notifying the splash parent before the splash panel
        /// is closed.
        /// </summary>
        /// <param name="splashPanelObject">The splash panel object</param>
        /// <remarks>
        /// This method will be called by the <see cref="SplashPanel.OnSplashClosing"/> 
        /// method. 
        /// </remarks> 
        /// <returns>splash Panel Object</returns>
        bool SplashClosingNotify(SplashPanel splashPanelObject);

        /// <summary>
        /// Invoker for notifying the splash parent when the <see cref="SplashPanel"/>
        /// has closed.
        /// </summary>
        /// <param name="splashPanelObject">The child splash panel that was closed.</param>
        /// <param name="splashCloseType">A SplashCloseType value.</param>
        /// <remarks>
        /// This method will be called by the <see cref="SplashPanel.OnSplashClosed"/> 
        /// method. 
        /// </remarks>
        void SplashClosedNotify(SplashPanel splashPanelObject, SplashCloseType splashCloseType);
    }

    #endregion

    #region ISPLASHWRAPPERFORMLISTENER

    /// <summary>
    /// This interface is implemented by the <see cref="SplashPanel"/> class
    /// to get notifications from the <see cref="WrapperForm"/> when
    /// the wrapper form displays and closes a splash screen.
    /// </summary>
    internal interface ISplashWrapperFormListener
    {
        /// <summary>
        /// Informs the listener that the splash screen has been displayed.
        /// </summary>
        /// <remarks>
        /// This method is invoked from the <see cref="WrapperForm.ShowWindow"/>
        /// method.
        /// </remarks>
        void SplashFormDisplayedNotify();

        /// <summary>
        /// Informs the listener that the splash screen has been closed.
        /// </summary>
        /// <remarks>
        /// This method is invoked from the <see cref="WrapperForm.HideWindow"/>
        /// method.
        /// </remarks>
        void SplashFormClosedNotify();
    }

    #endregion

    #region SPLASHALIGNMENT

    /// <summary>
    /// Specifies the positioning of the splash with the desktop.
    /// </summary>
    public enum SplashAlignment
    {
        /// <summary>
        /// The Splash window will be made visible at the location closest
        /// to the system tray clock (if available).
        /// </summary>
        SystemTray,

        /// <summary>
        /// The splash window will be shown at the center of the screen.
        /// </summary>
        Center,

        /// <summary>
        /// The splash window will be shown at the top-left of the screen.
        /// </summary>
        LeftTop,

        /// <summary>
        /// The splash window will be shown at the  bottom -left of the screen.
        /// </summary>
        LeftBottom,

        /// <summary>
        /// The splash window will be shown at the top-right of the screen.
        /// </summary>
        RightTop,

        /// <summary>
        /// The splash window will be shown at the bottom-right of the screen.
        /// </summary>
        RightBottom,

        /// <summary>
        /// Does not indicate any of the above alignments.
        /// </summary>
        Custom
    }
    public enum AnimationDirection
    {
        Default,
        RightToLeft,
        LeftToRight
    }
    public enum MarqueePosition
    {
        Topleft,
        Topright,
        BottomLeft,
        BottomRight
    }
    public enum SplashPanelMarqueeDirection
    {
        RightToLeft,
        LeftToRight,
        BottomToTop,
        TopToBottom
    }
    #endregion

    #region SPLASHCLOSETYPE

    /// <summary>
    /// Specifies the way in which a splash control container was closed.
    /// </summary>
    /// <remarks>
    /// Signifies the mode in which the splash control container was closed.
    /// This can be interpreted by the parent of the splash control container.
    /// </remarks>
    public enum SplashCloseType
    {
        /// <summary>
        /// The user wants the changes made in the splash to be applied.
        /// </summary>
        Done,

        /// <summary>
        /// The user canceled the splash and expects the changes, if any to be ignored.
        /// </summary>
        Canceled,

        /// <summary>
        /// The popup was deactivated because it was displayed for the specified
        /// time. This should be considered to be equivalent to the Done
        /// type as the user may want some changes saved.
        /// </summary>
        TimedOut
    }

    #endregion

    #region TASKBARDOCKPOSITION

    /// <summary>
    /// The current docking position of the windows task bar.
    /// </summary>
    internal enum TaskbarDockPosition
    {
        /// <summary>
        /// The taskbar is docked at the bottom of the current screen.
        /// </summary>
        Bottom = 0,

        /// <summary>
        /// The taskbar is docked at the left of the current screen.
        /// </summary>
        Left,

        /// <summary>
        /// The taskbar is docked at the top of the current screen.
        /// </summary>
        Top,

        /// <summary>
        /// The taskbar is docked at the right of the current screen.
        /// </summary>
        Right
    }

    #endregion

    #region SLIDESTYLE

    /// <summary>
    /// Specifies to the animation functions in 
    /// the <see cref="SplashPanel"/> class the nature of the slide
    /// animation to be performed.
    /// </summary>
    public enum SlideStyle
    {
        /// <summary>
        /// Slides horizontally from left to right. This animation effect
        /// is typically used when the window is displayed at the left edge of the
        /// screen.
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Slides vertically from bottom to top. This slide animation effect is
        /// typically used when the location of the window is at the right end 
        /// of the screen.
        /// </summary>
        Vertical = 1,

        /// <summary>
        /// Slides horizontally from left to right. This animation effect
        /// is typically used when the window is displayed at the left edge of the
        /// screen.
        /// </summary>
        LeftToRight = 2,

        /// <summary>
        /// Slides vertically from bottom to top. This slide animation effect is
        /// typically used when the location of the window is at the right end 
        /// of the screen.
        /// </summary>
        BottomToTop = 3,

        /// <summary>
        /// Slides vertically from right to left.
        /// </summary>
        RightToLeft = 4,

        /// <summary>
        /// Slides vertically from top to bottom.
        /// </summary>
        TopToBottom = 5,

        /// <summary>
        /// Fades from transparent to opaque.
        /// </summary>
        FadeIn = 6,

        /// <summary>
        /// Slides vertically
        /// </summary>
        Slide = 7,

        /// <summary>
        /// The default style.
        /// </summary>
        Default,
        /// <summary>
        /// Marquee style.
        /// </summary>
        Marquee
    }

    #endregion

    #region WRAPPERFORM

    /// <summary>
    /// The Form derived class that hosts a <see cref="SplashPanel"/> when it is
    /// displayed.
    /// </summary>
    /// <remarks>
    /// You will normally not have to use this class or refer to
    /// an instance of this class. An instance of this class will
    /// be automatically generated by the SplashPanel which
    /// will then set this as its parent when <see cref="ShowSplash"/> 
    /// is called on it.
    /// </remarks>
    internal class WrapperForm : Form
    {        
        #region Filds

        /// <summary>
        /// Borders indent.
        /// </summary>
        private const int c_iBordersIndent = 4;

        /// <summary>
        /// Minimum animation size.
        /// </summary>
        private const int c_iMinAnimationSize = 1;

        /// <summary>
        /// Fade animation increment delay.
        /// </summary>
        private const double c_dFadeAnimationDelay = 200.0;

        /// <summary>
        /// Specifies how to space the SplashPanel within this host.
        /// </summary>
        private int borderGap;

        /// <summary>
        /// This object will be notified by the SplashWrapperForm
        /// of important events.
        /// </summary>
        private ISplashWrapperFormListener splashFormListener;

        /// <summary>
        /// The preferred width of the <see cref="WrapperForm"/> when it
        /// becomes visible.
        /// </summary>
        private int preferredWidth;

        /// <summary>
        /// The preferred height of the <see cref="WrapperForm"/> when it
        /// becomes visible.
        /// </summary>
        private int preferredHeight;

        /// <summary>
        /// The timer that is used to animate the window. The animation
        /// is actually created by changing the height and width of the 
        /// window at regular time intervals.
        /// </summary>
        /// <remarks>
        /// The Windows API AnimateWindow does not work properly. It crashes
        /// when some controls are added to the SplashPanel.
        /// </remarks>
        private System.Windows.Forms.Timer animationTimer;

        /// <summary>
        /// The timer that is used when the SplashPanel is being
        /// closed to create the animation effect.
        /// </summary>
        private System.Windows.Forms.Timer hideAnimationTimer;

        /// <summary>
        /// The <see cref="SplashPanel"/> that will be hosted by this
        /// SplashWrapperForm object.
        /// </summary>
        private SplashPanel splashPanel;

        /// <summary>
        /// The <see cref="SlideStyle"/> for the animation.
        /// </summary>
        private SlideStyle slideStyle = SlideStyle.Default;

        /// <summary>
        /// The owner form - we don't use the Owner property as that requires a
        /// TopLevel window.
        /// </summary>
        private Form splashWrapperFormOwner = null;

        /// <summary>
        /// Result when the SplashPanel is invoked as Modal.
        /// </summary>
        private DialogResult dialogResult = DialogResult.None;

        private bool customRegion = false;

        private bool m_bInitializing;

        private Rectangle m_rcRegion;

        private int increment = 0;

        private bool incre = true;

        private int inc = 0;
        #endregion

        #region Private delegates

        private delegate void SetBoundsDelegate(int x, int y, int width, int height);
        private delegate void SetDimensionDelegate(int dimension); // Any single dimension
        private delegate void SetDoubleDelegate(double dblVal); // Any single double value
        private delegate void SetBoolDelegate(bool val); // Any boolean value

        #endregion

        /// <summary>
        /// Initializes a new instance of the WrapperForm class.
        /// </summary>
        /// <param name="splashPanel">The SplashPanel that will be displayed
        /// by this SplashWrapperForm.</param>
        /// <remarks>
        /// The SplashWrapperForm will be initialized with the <see cref="SplashPanel"/>
        /// object that it will host. The SplashWrapperForm will only be valid for the
        /// duration that the SplashPanel is visible. 
        /// <para>
        /// The SplashWrapperForm works closely with the SplashPanel object it hosts
        /// and will not be visible and the user will see the SplashPanel
        /// as the SplashWrapperForm.
        /// </para>
        /// <para>
        /// The SplashWrapperForm is set to be a top level window.
        /// </para>
        /// </remarks>
        public WrapperForm(SplashPanel splashPanel) :
            base()
        {
            m_bInitializing = true;

            Control parent = splashPanel.Parent;

            this.borderGap = 4;
            this.SplashPanel = splashPanel;
            this.splashFormListener = splashPanel;
            this.preferredWidth = splashPanel.Width;
            this.preferredHeight = splashPanel.Height;

            this.AttachSplash();

            this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick,false);
            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.SetBorderStyle();
            this.BackColor = splashPanel.BackColor;
            this.ShowInTaskbar = splashPanel.ShowInTaskbar;
            this.Icon = splashPanel.FormIcon;
            this.Text = splashPanel.Text;

            if (parent != null)
            {
                this.Font = parent.Font;
                this.ForeColor = parent.ForeColor;
                this.BackColor = parent.BackColor;
                this.RightToLeft = parent.RightToLeft;

                // this.Enabled = parent.Enabled;
                // this.Visible = parent.Visible;
                this.BindingContext = parent.BindingContext;

                Form parentForm = parent as Form;

                if (parentForm != null)
                {
                    this.RightToLeftLayout = parentForm.RightToLeftLayout;
                }
            }

            this.TabStop = false;
            this.HScroll = false;
            this.VScroll = false;
            this.ControlBox = false;
            this.Location = new Point(-1000, -1000);
            this.StartPosition = FormStartPosition.Manual;

            this.animationTimer = new System.Windows.Forms.Timer();
            this.animationTimer.Interval = SplashPanel.AnimationInterval;
            this.animationTimer.Tick += new EventHandler(HandleAnimationTimer);

            this.hideAnimationTimer = new System.Windows.Forms.Timer();
            this.hideAnimationTimer.Interval = 50;
            this.hideAnimationTimer.Tick += new EventHandler(HandleHideAnimationTimer);

            this.animationTimer.Enabled = false;
            this.hideAnimationTimer.Enabled = false;
            this.TopLevel = true;
            this.Visible = false;

            this.MinimumSize = new Size(20, 20);

            m_bInitializing = false;
        }

        /// <summary>
        /// Sets the border style for this SplashHost. The BorderStyle
        /// is a <see cref="Border3DStyle"/> value that is painted
        /// by this control in the OnPaint handler.
        /// </summary>
        /// <remarks>
        /// You can override this method to provide your own FormBorderStyle for 
        /// this SplashWrapperForm.
        /// </remarks>
        protected virtual void SetBorderStyle()
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }

        /// <summary>
        /// Gets Splash wrapper form Listener
        /// This is the same as the <see cref="SplashPanel"/> hosted by the
        /// SplashWrapperForm. This will be used by the SplashWrapperForm to
        /// notify the SplashPanel of important events so that the SplashPanel
        /// can respond.
        /// </summary>
        /// <value>
        /// A SplashPanel object that implements ISplashWrapperFormListener.
        /// </value>
        /// <remarks>
        /// The value cannot be set through this property. All
        /// SplashWrapperForm objects are single use only. The splash host will
        /// be destroyed after the splash window is hiddden and
        /// the only way to set the SplashPanel for a 
        /// SplashHost would be through the SplashHost contructor.
        /// </remarks>
        public ISplashWrapperFormListener SplashWrapperFormListener
        {
            get
            {
                return this.splashFormListener;
            }
        }

        public bool CustomRegion
        {
            set
            {
                this.customRegion = value;
            }
        }

        #region Overrides

        protected override void OnDeactivate(EventArgs e)
        {
            if (this.SplashPanel.CloseOnLostFocus && this.SplashPanel.IsShowing())
            {
                this.splashPanel.HideSplash();
            }
            base.OnDeactivate(e);
        }

        /// <summary>
        /// Overrides the OnPaint method to paint the 3D border style.
        /// </summary>
        /// <param name="pe">The PaintEventArgs event data.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (this.customRegion == false && this.SplashPanel.BorderType == SplashBorderType.Border3D)
                this.Draw3DBorder(pe.Graphics, this.SplashPanel.BorderStyle);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!m_bInitializing)
            {
                Rectangle rcPanel = this.ClientRectangle;

                if (this.customRegion == false && this.SplashPanel.BorderType == SplashBorderType.Border3D)
                {
                    int border = this.borderGap / 2;
                    rcPanel.Inflate(-border, -border);
                }

                this.SplashPanel.Bounds = rcPanel;
            }
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Draws the 3D border that is specified in the <see cref="SplashPanel.Border3DStyle"/>
        /// property.
        /// </summary>
        /// <param name="g">The graphics object that is to be drawn on.</param>
        /// <param name="border3DStyleValue">The Border3DStyle value to be used for drawing the border.</param>
        /// <remarks>
        /// You can override this virtual function and draw your own implementation
        /// of the 3D border.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Draw3DBorder(Graphics g, Border3DStyle border3DStyleValue)
        {
            ControlPaint.DrawBorder3D(g, this.ClientRectangle, border3DStyleValue, Border3DSide.Bottom | Border3DSide.Top | Border3DSide.Right | Border3DSide.Left);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Overrides CreateParams to add extended styles for ToolWindow and 
        /// NoActivate.
        /// </summary>
        /// <remarks>
        /// The SplashWrapperForm object needs to be Top level window and
        /// also should not be activated. These styles are set in this method.
        /// </remarks>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                /* Bug fix. (Defect 10231)

                Version version = Environment.OSVersion.Version;
                // Otherwise has trouble createing window in NT4.0
                if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major > 5)
                {
                    cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                }
                */

                cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW ; //| 0x02000000 /*WS_EX_COMPOSITED*/;

                return cp;
            }
        }

        public override Size MinimumSize
        {
            get
            {
                if (this.hideAnimationTimer != null && this.hideAnimationTimer.Enabled)
                {
                    return Size.Empty;
                }
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SplashPanel"/> object that is hosted by this 
        /// <see cref="WrapperForm"/>.
        /// </summary>
        /// <remarks>
        /// The SplashPanel object is the reason the SplashWrapperForm
        /// exists. This SplashPanel will be the only visible part of the
        /// SplashWrapperForm. You should not add any other controls that
        /// will be visible to the SplashWrapperForm.
        /// </remarks>
        private SplashPanel SplashPanel
        {
            get
            {
                return this.splashPanel;
            }

            set
            {
                this.splashPanel = value;
                this.splashFormListener = value;
            }
        }

        [Browsable(false)]
        public Form SplashWrapperFormOwner
        {
            get
            {
                return this.splashWrapperFormOwner;
            }

            set
            {
                this.splashWrapperFormOwner = value;
                
                this.Owner = this.SplashWrapperFormOwner;
            }
        }

        private Rectangle RegionBounds
        {
            get
            {
                if (m_rcRegion.IsEmpty)
                {
                    return new Rectangle(Point.Empty, this.Size);
                }
                return m_rcRegion;
            }
            set
            {
                if (m_rcRegion != value)
                {
                    m_rcRegion = value;
                    this.Region = m_rcRegion.IsEmpty ? null : new Region(m_rcRegion);
                }
            }
        }
        #endregion

        /// <summary>
        /// Displays the <see cref="SplashPanel"/> that this SplashWrapperForm is hosting.
        /// </summary>
        /// <param name="disableOwner">Indicates whether the SplashWrapperForm should
        /// disable the owner form.</param>
        /// <remarks>
        /// This method calls <see cref="ShowWindow"/> or <see cref="ShowWindowAnimated"/>
        /// depending on what the <see cref="Syncfusion.Windows.Forms.Tools.SplashPanel.ShowAnimation"/> property is set to.
        /// </remarks>
        public virtual void ShowSplash(bool disableOwner)
        {
            PrepareSplash();
            SplashWindowShowMode mode = SplashWindowShowMode.Modeless;
            if (disableOwner == true && this.SplashWrapperFormOwner != null)
                mode = SplashWindowShowMode.DisableOwner;

            if (this.SplashPanel.ShowAnimation == true)
            {
                this.ShowWindowAnimated(mode);
            }
            else
                this.ShowWindow(mode);
        }

        /// <summary>
        /// Displays the SplashPanel as a Modal dialog.
        /// </summary>
        /// <returns>Returns dialog result</returns>
        public DialogResult ShowDialogSplash()
        {
            PrepareSplash();
            if (this.SplashPanel.ShowAnimation == true)
                this.ShowWindowAnimated(SplashWindowShowMode.Modal);
            else
                this.ShowWindow(SplashWindowShowMode.Modal);

            return this.dialogResult;
        }

        private void PrepareSplash()
        {
            this.ComputeMySize();
            if (this.SplashPanel.SlideStyle == SlideStyle.Slide)
            {
                if (this.SplashPanel.DesktopAlignment == SplashAlignment.RightBottom || this.SplashPanel.DesktopAlignment == SplashAlignment.SystemTray ||
                    this.SplashPanel.DesktopAlignment == SplashAlignment.LeftBottom || (this.SplashPanel.DesktopAlignment == SplashAlignment.RightTop && this.GetAppbarPosition() == NativeMethods.ABE_TOP) ||
                    (this.SplashPanel.DesktopAlignment == SplashAlignment.LeftTop && this.GetAppbarPosition() == NativeMethods.ABE_TOP))
                {
                    this.Region = new Region(new Rectangle(0, 0, 0, 0));
                    this.increment = 0;
                }
            }
            this.Location = this.GetDisplayLocation(this.SplashPanel.DesktopAlignment);
            this.SplashPanel.BringToFront();
            this.SplashPanel.Show();
        }

        public void AdjustSize()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(AdjustSizeInternal));
            }
            else
            {
                AdjustSizeInternal();
            }
        }

        protected void AdjustSizeInternal()
        {
            this.ComputeControlLocation();
            this.ComputeMySize();
        }

        /// <summary>
        /// Places the hosted SplashPanel within the bounds of the SplashWrapperForm.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method sizes the hosted <see cref="SplashPanel"/>
        /// to occupy the entire area of the SplashWrapperForm except for the border.
        /// <para>
        /// This method is invoked by the <see cref="AttachSplash"/> method.
        /// </para>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal virtual void ComputeControlLocation()
        {
            if (this.customRegion == false && this.SplashPanel.BorderType == SplashBorderType.Border3D)
                this.SplashPanel.Location = new Point(this.borderGap / 2, this.borderGap / 2);
            else
                this.SplashPanel.Location = new Point(0, 0);
        }

        /// <summary>
        /// Displays the splash window without any animation.
        /// </summary>
        /// <param name="mode">Indicates whteher the owner form is to be disabled.</param>
        /// <remarks>
        /// This method is invoked by the <see cref="ShowSplash"/> method
        /// if the <see cref="Syncfusion.Windows.Forms.Tools.SplashPanel.ShowAnimation"/> property is set to false.
        /// <para>
        /// The <see cref="SplashPanel"/> is also notified that the window has been
        /// displayed.
        /// </para>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ShowWindow(SplashWindowShowMode mode)
        {
            this.SplashPanel.BringToFront();
            this.SplashPanel.Visible = true;

            if (mode == SplashWindowShowMode.Modeless)
            {
                this.ShowOptimized();
                if (this.SplashPanel.ShowAsTopMost == false)
                    this.Activate();
                this.SplashWrapperFormListener.SplashFormDisplayedNotify();
            }
            else if (mode == SplashWindowShowMode.DisableOwner)
            {
                this.SplashWrapperFormOwner.Visible = true;
                this.SplashWrapperFormOwner.Enabled = false;
                this.ShowOptimized();
                if (this.SplashPanel.ShowAsTopMost == false)
                    this.Activate();
                this.SplashWrapperFormListener.SplashFormDisplayedNotify();
            }
            else if (mode == SplashWindowShowMode.Modal)
            {
                this.SplashWrapperFormListener.SplashFormDisplayedNotify();
                this.ShowDialogOptimized(this.SplashWrapperFormOwner);
            }
        }

        /// <summary>
        /// No animation.
        /// </summary>
        private void ShowOptimized()
        {
            Point currentPt = this.Location;
            Point outsidePt = new Point(-1000, -1000);
            this.Location = outsidePt;
            this.Show();
            this.Location = currentPt;
        }

        private void ShowDialogOptimized(IWin32Window owner)
        {
            this.dialogResult = this.ShowDialog(owner);
        }

        /// <summary>
        /// Displays the splash host window in an animated manner.
        /// </summary>
        /// <param name="mode">Indicates whether the window is to be displayed modally.</param>
        /// <remarks>
        /// This method is invoked by the <see cref="ShowSplash"/> method when the
        /// <see cref="Syncfusion.Windows.Forms.Tools.SplashPanel.ShowWindowAnimated"/> property is set to true.
        /// <para>
        /// The <see cref="SlideStyle"/>  used for the animation is based on the value
        /// returned by the <see cref="GetSlideStyle"/> method.
        /// </para>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ShowWindowAnimated(SplashWindowShowMode mode)
        {
            this.hideAnimationTimer.Enabled = false;
            this.slideStyle = this.GetSlideStyle(this.SplashPanel.DesktopAlignment);

            if (this.SplashPanel.SlideStyle != SlideStyle.Slide)
            {
                switch (this.slideStyle)
                {
                    case SlideStyle.Horizontal:
                    case SlideStyle.LeftToRight:
                        this.RegionBounds = new Rectangle(0, 0, 0, Height);
                        break;
                    case SlideStyle.Vertical:
                    case SlideStyle.BottomToTop:
                    default:
                        this.RegionBounds = new Rectangle(0, this.Height - 4, this.Width, 4);
                        break;
                    case SlideStyle.TopToBottom:
                        this.RegionBounds = new Rectangle(0, 0, this.Width, 0);
                        break;
                    case SlideStyle.RightToLeft:
                        this.RegionBounds = new Rectangle(this.Width - c_iBordersIndent, 0, c_iBordersIndent, this.Height);
                        break;
                    case SlideStyle.FadeIn:
                        this.Opacity = 0;
                        break;
                }
            }
            else if (this.SplashPanel.SlideStyle == SlideStyle.Slide)
            {
                switch (this.SplashPanel.DesktopAlignment)
                {
                    case SplashAlignment.LeftBottom:
                        this.Region = new Region(new Rectangle(0,0,0,0));
                        break;
                    case SplashAlignment.RightBottom:
                        this.Region = new Region(new Rectangle(0,0,0,0));
                        break;
                    case SplashAlignment.SystemTray:
                        this.Region = new Region(new Rectangle(0,0,0,0));
                        break;
                }
            } 

            this.animationTimer.Enabled = true;
            this.animationTimer.Start();
            if (mode == SplashWindowShowMode.Modeless)
            {
                this.Show();
            }
            else if (mode == SplashWindowShowMode.DisableOwner)
            {
                this.SplashWrapperFormOwner.Visible = true;
                this.SplashWrapperFormOwner.Enabled = false;
                this.Show();
            }
            else if (mode == SplashWindowShowMode.Modal)
            {
                this.ShowDialogOptimized(this.SplashWrapperFormOwner);
            }
        }

        /// <summary>
        /// Overrides Show and display using SetWindowPos and SWP_NOACTIVATE so that
        /// the focus is not grabbed.
        /// </summary>
        protected new void Show()
        {
            this.SetVisibleNoActivate(true);
        }

        internal void ShowForm(IntPtr handle, Point location, Size size)
        {
            IntPtr hWndInsertAfter = (IntPtr)NativeMethods.HWND_NOTOPMOST;
            int uFlags = (int)NativeMethods.SetWindowPosFlags.SWP_SHOWWINDOW;

            if (this.SplashPanel.ShowAsTopMost)
            {
                hWndInsertAfter = (IntPtr)NativeMethods.HWND_TOPMOST;
                uFlags |= (int)NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE;
            }

            NativeMethods.SetWindowPos(handle, hWndInsertAfter, location.X, location.Y, size.Width, size.Height, uFlags);
        }

        protected void SetVisibleNoActivate(bool visible)
        {
            if (visible)
            {
                ShowForm(this.Handle, this.Location, this.Size);
                this.Visible = true;
            }
            else
                this.Visible = false;
        }

        #region Implementation

        /// <summary>
        /// The animation of the splash window is implemented using a timer
        /// and its Elapsed event is handled in this method.
        /// </summary>
        /// <param name="sender">The animation timer object.</param>
        /// <param name="e">The elapsed event args event data.</param>
        private void HandleAnimationTimer(object sender, EventArgs e)
        {
            // Console.WriteLine("Animation Timer");
            try
            {
                if (this.animationTimer.Enabled == true)
                {
                    int animationIncrement = this.SplashPanel.AnimationSteps;

                    if (this.SplashPanel.SlideStyle != SlideStyle.Slide && this.splashPanel.SlideStyle != SlideStyle.Marquee)
                    {
                        switch (this.slideStyle)
                        {
                            case SlideStyle.Horizontal:
                            case SlideStyle.LeftToRight:
                                {
                                    int width = this.RegionBounds.Width + animationIncrement;

                                    if (width < preferredWidth)
                                    {
                                        this.RegionBounds = new Rectangle(0, 0, width, Height);
                                    }
                                    else
                                    {
                                        this.RegionBounds = Rectangle.Empty;
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SlideStyle.Vertical:
                            case SlideStyle.BottomToTop:
                            default:
                                {
                                    int height = this.RegionBounds.Height + animationIncrement;

                                    if (height < preferredHeight)
                                    {
                                        this.RegionBounds = new Rectangle(0, this.Height - height, this.Width, height);
                                    }
                                    else
                                    {
                                        this.RegionBounds = Rectangle.Empty;
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SlideStyle.TopToBottom:
                                {
                                    int height = this.RegionBounds.Height + animationIncrement;

                                    if (height < preferredHeight)
                                    {
                                        this.RegionBounds = new Rectangle(0, 0, this.Width, height);
                                    }
                                    else
                                    {
                                        this.RegionBounds = Rectangle.Empty;
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SlideStyle.RightToLeft:
                                {
                                    int width = this.RegionBounds.Width + animationIncrement;
                                    if (width < preferredWidth)
                                    {
                                        this.RegionBounds = new Rectangle(this.Width - width, 0, width, Height);
                                    }
                                    else
                                    {
                                        this.RegionBounds = Rectangle.Empty;
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SlideStyle.FadeIn:
                                if (this.Opacity < 1.0)
                                {
                                    this.Opacity += animationIncrement / c_dFadeAnimationDelay;
                                }
                                else
                                {
                                    this.Opacity = 1.0;
                                    this.animationTimer.Stop();
                                }
                                break;
                        }
                    }
                    else if (this.SplashPanel.SlideStyle == SlideStyle.Slide)
                    {
                        Rectangle r = Screen.GetWorkingArea(this);
                        switch(this.SplashPanel.DesktopAlignment)
                        {
                            case SplashAlignment.RightBottom:
                                if (splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                                {
                                    if (r.Right < this.Left + animationIncrement + splashPanel.Width)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left - animationIncrement, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                        this.animationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Bottom + animationIncrement)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, increment));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SplashAlignment.SystemTray:
                                if (this.GetAppbarPosition()!=NativeMethods.ABE_TOP)
                                {
                                    if (this.Bottom > r.Bottom + animationIncrement)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, increment));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                        this.animationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Top < r.Top - animationIncrement)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                        this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        if (this.Top != r.Top)
                                        {
                                            this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                        }
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SplashAlignment.RightTop:
                                if (splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                                {
                                    if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                    {
                                        if (r.Right < this.Left + animationIncrement + splashPanel.Width)
                                        {
                                            this.Bounds = new Rectangle(this.Left - animationIncrement, 0, this.Width, this.Height);
                                        }
                                        else
                                        {
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.animationTimer.Stop();
                                        }
                                    }
                                    else
                                    {
                                        if (this.Top < r.Top - animationIncrement)
                                        {
                                            increment += animationIncrement;
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                            this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                        }
                                        else
                                        {
                                            increment += animationIncrement;
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                            this.animationTimer.Stop();
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                    {
                                        if (this.Top < r.Y - animationIncrement)
                                        {
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                        }
                                        else
                                        {
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.animationTimer.Stop();
                                        }
                                    }
                                    else
                                    {
                                        if (this.Top < r.Top - animationIncrement)
                                        {
                                            increment += animationIncrement;
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                            this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                        }
                                        else
                                        {
                                            increment += animationIncrement;
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                            this.animationTimer.Stop();
                                        }
                                    }
                                }
                                break;
                            case SplashAlignment.LeftBottom:
                                if (splashPanel.AnimationDirection == AnimationDirection.LeftToRight)
                                {
                                    if (this.Left + animationIncrement < r.Left)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left + animationIncrement, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                        this.animationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Bottom + animationIncrement)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, increment));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                        this.animationTimer.Stop();
                                    }
                                }
                                break;
                            case SplashAlignment.LeftTop:

                                if (splashPanel.AnimationDirection == AnimationDirection.LeftToRight)
                                {
                                    if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                    {
                                        if (this.Left + animationIncrement < r.Left)
                                        {

                                            this.Bounds = new Rectangle(this.Left + animationIncrement, 0, this.Width, this.Height);
                                        }
                                        else
                                        {
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.animationTimer.Stop();
                                        }
                                    }
                                    else
                                    {
                                        if (this.Top < r.Top - animationIncrement)
                                        {
                                            increment += animationIncrement;
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                            this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                        }
                                        else
                                        {
                                            increment += animationIncrement;
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                            this.animationTimer.Stop();
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                    {
                                        if (this.Top < r.Y - animationIncrement)
                                        {
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                        }
                                        else
                                        {
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.animationTimer.Stop();
                                        }
                                    }
                                    else
                                    {
                                        if (this.Top < r.Top - animationIncrement)
                                        {
                                            increment += animationIncrement;
                                            this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                            this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                        }
                                        else
                                        {
                                            increment += animationIncrement;
                                            if (this.Top != r.Top)
                                            {
                                                this.Bounds = new Rectangle(this.Left, r.Top, this.Width, this.Height);
                                            }
                                            this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                            this.animationTimer.Stop();
                                        }
                                    }
                                }
                                break;
                            case SplashAlignment.Center:
                                this.animationTimer.Stop();
                                break;
                            case SplashAlignment.Custom:
                                this.animationTimer.Stop();
                                break;
                            default:
                                if (this.Bottom > r.Bottom + animationIncrement)
                                {
                                    increment += animationIncrement;
                                    this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, increment));
                                }
                                else
                                {
                                    increment += animationIncrement;
                                    this.Bounds = new Rectangle(this.Left, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                    this.animationTimer.Stop();
                                }
                                break;
                         }
                    }
                    else if (this.splashPanel.SlideStyle == SlideStyle.Marquee)
                    {
                        Rectangle r = Screen.GetWorkingArea(this);
                        switch (this.splashPanel.MarqueePosition)
                        {
                            case MarqueePosition.BottomLeft:
                                if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.LeftToRight)
                                {
                                    if (this.Left + animationIncrement < r.Width)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left + animationIncrement, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Left = -splashPanel.Width;
                                    }
                                }
                                else if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.BottomToTop)
                                {
                                    if (this.Top + this.splashPanel.Height > 0)
                                    {
                                        increment += animationIncrement;
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, increment));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Top = r.Bottom + this.splashPanel.Height / 2;
                                    }
                                }
                                break;
                            case MarqueePosition.BottomRight:
                                if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.RightToLeft)
                                {
                                    if (0 < this.Left + splashPanel.Width)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left - animationIncrement, this.Top - (this.Bottom - r.Bottom), this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Left = r.Width;
                                    }
                                }
                                else if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.BottomToTop)
                                {
                                    if (this.Top + this.splashPanel.Height > 0)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Top = r.Bottom + this.splashPanel.Height / 2;
                                    }
                                }
                                break;
                            case MarqueePosition.Topleft:
                                if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.LeftToRight)
                                {
                                    if (this.Left + animationIncrement < r.Width)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left + animationIncrement, 0, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Left = -splashPanel.Width;
                                    }
                                }
                                else if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.TopToBottom)
                                {
                                    if (this.Top - this.splashPanel.Height / 2 < r.Bottom)
                                    {
                                        increment += animationIncrement;
                                        this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                        this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                    }
                                    else
                                    {
                                        this.Top = -this.splashPanel.Height;
                                    }
                                }
                                break;
                            case MarqueePosition.Topright:
                                if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.RightToLeft)
                                {
                                    if (0 < this.Left + splashPanel.Width)
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                        this.Bounds = new Rectangle(this.Left - animationIncrement, 0, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Left = r.Width;
                                    }
                                }
                                else if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.TopToBottom)
                                {
                                    if (this.Top - this.splashPanel.Height / 2 < r.Bottom)
                                    {
                                        increment += animationIncrement;
                                        this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                        this.Region = new Region(new Rectangle(0, this.Height - increment, this.Width, increment));
                                    }
                                    else
                                    {
                                        this.Top = -this.splashPanel.Height;
                                    }
                                }
                                break;
                        }
                    }

                    this.Invalidate(this.ClientRectangle, false);
                    if (this.animationTimer.Enabled == false)
                    {
                        this.SplashWrapperFormListener.SplashFormDisplayedNotify();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void SetWidth(int width)
        {
            this.Width = width;
        }

        private void SetTop(int top)
        {
            this.Top = top;
        }

        private void SetHeight(int height)
        {
            this.Height = height;
        }

        private void SetLeft(int left)
        {
            this.Left = left;
        }

        private void SetVisible(bool visible)
        {
            this.Visible = visible;
        }

        private void SetOpacity(double opacity)
        {
            this.Opacity = opacity;
        }

        #endregion

        /// <summary>
        /// The handler for the elapsed event of the Hide animation timer. Used
        /// when the splash window is closing.
        /// </summary>
        /// <param name="sender">The hide animation timer.</param>
        /// <param name="e">The elapsed event data.</param>
        private void HandleHideAnimationTimer(object sender, EventArgs e)
        {
            bool flag = splashPanel.mouseEnter && splashPanel.SuspendAutoCloseWhenMouseOver;
            if (this.hideAnimationTimer.Enabled && !flag && !IsDisposed)
            {
                int animationIncrement = this.SplashPanel.AnimationSteps;

                if (this.SplashPanel.SlideStyle != SlideStyle.Slide)
                {
                    switch (this.slideStyle)
                    {
                        case SlideStyle.Horizontal:
                        case SlideStyle.LeftToRight:
                            {
                                int width = this.RegionBounds.Width - animationIncrement;
                                if (width > 1)
                                {
                                    this.RegionBounds = new Rectangle(0, 0, width, this.Height);
                                }
                                else
                                {
                                    this.Visible = false;
                                    this.RegionBounds = Rectangle.Empty;
                                    this.hideAnimationTimer.Stop();
                                }
                            }
                            break;
                        case SlideStyle.Vertical:
                        case SlideStyle.BottomToTop:
                        default:
                            {
                                int height = this.RegionBounds.Height - animationIncrement;
                                if (height > 1)
                                {
                                    this.RegionBounds = new Rectangle(0, this.Height - height, this.Width, height);
                                }
                                else
                                {
                                    this.Visible = false;

                                    this.RegionBounds = Rectangle.Empty;
                                    this.hideAnimationTimer.Stop();
                                }
                            }
                            break;
                        case SlideStyle.TopToBottom:
                            {
                                int height = this.RegionBounds.Height - animationIncrement;
                                if (height > c_iMinAnimationSize)
                                {
                                    this.RegionBounds = new Rectangle(0, 0, this.Width, height);
                                }
                                else
                                {
                                    this.Visible = false;

                                    this.RegionBounds = Rectangle.Empty;
                                    this.hideAnimationTimer.Stop();
                                }
                            }
                            break;
                        case SlideStyle.RightToLeft:
                            {
                                int width = this.RegionBounds.Width - animationIncrement;
                                if (width > c_iMinAnimationSize)
                                {
                                    this.RegionBounds = new Rectangle(this.Width - width, 0, width, this.Height);
                                }
                                else
                                {
                                    this.Visible = false;

                                    this.RegionBounds = Rectangle.Empty;
                                    this.hideAnimationTimer.Stop();
                                }
                            }
                            break;
                        case SlideStyle.FadeIn:
                            if (this.Opacity > 0.0)
                            {
                                this.Opacity -= animationIncrement / c_dFadeAnimationDelay;
                            }
                            else
                            {
                                this.Visible = false;

                                this.Opacity = 1.0;
                                this.hideAnimationTimer.Stop();
                            }
                            break;
                    }
                }
                else if (this.SplashPanel.SlideStyle == SlideStyle.Slide)
                {
                    Rectangle r=Screen.GetWorkingArea(this);
                    switch (this.SplashPanel.DesktopAlignment)
                    {
                        case SplashAlignment.RightBottom:
                            if (splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                            {
                                if (this.Top <= r.Bottom)
                                {
                                    if (incre)
                                    {
                                        increment -= animationIncrement;
                                        inc = increment;
                                    }
                                    else
                                    {
                                        increment -= animationIncrement;
                                        inc -= animationIncrement;
                                    }
                                    if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                    {
                                        inc=increment+(this.Height%animationIncrement)-animationIncrement;
                                        incre = false;
                                    }
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                    this.Bounds = new Rectangle(this.Left + animationIncrement, this.Top, this.Width, this.Height);
                                }
                                else
                                {
                                    this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                    this.Visible = false;
                                    this.hideAnimationTimer.Stop();
                                    increment = 0;
                                    incre = true;
                                    inc = 0;
                                }
                            }
                            else
                            {
                                if (this.Top <= r.Bottom)
                                {
                                    if (incre)
                                    {
                                        increment -= animationIncrement;
                                        inc = increment;
                                    }
                                    else
                                    {
                                        increment -= animationIncrement;
                                        inc -= animationIncrement;
                                    }
                                    if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                    {
                                        inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                        incre = false;
                                    }
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, inc));
                                    this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                }
                                else
                                {
                                    this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                    this.Visible = false;
                                    this.hideAnimationTimer.Stop();
                                    increment = 0;
                                    incre = true;
                                    inc = 0;
                                }
                            }
                            break;
                        case SplashAlignment.SystemTray:
                            if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                            {
                                if (this.Top <= r.Bottom)
                                {
                                    if (incre)
                                    {
                                        increment -= animationIncrement;
                                        inc = increment;
                                    }
                                    else
                                    {
                                        increment -= animationIncrement;
                                        inc -= animationIncrement;
                                    }
                                    if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                    {
                                        inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                        incre = false;
                                    }
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, inc));
                                    this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                }
                                else
                                {
                                    this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                    this.Visible = false;
                                    this.hideAnimationTimer.Stop();
                                    increment = 0;
                                    incre = true;
                                    inc = 0;
                                }
                            }
                            else
                            {
                                if (this.Bottom > r.Top)
                                {
                                    if (incre)
                                    {
                                        increment -= animationIncrement;
                                        inc = increment;
                                    }
                                    else
                                    {
                                        increment -= animationIncrement;
                                        inc -= animationIncrement;
                                    }
                                    if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                    {
                                        inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                        incre = false;
                                    }
                                    if (inc < this.Height)
                                    {
                                        this.Region = new Region(new Rectangle(0, this.Height - inc, this.Width, inc));
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                }
                                else
                                {
                                    this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                    this.Visible = false;
                                    this.hideAnimationTimer.Stop();
                                    increment = 0;
                                    incre = true;
                                    inc = 0;
                                }
                            }
                            break;
                        case SplashAlignment.RightTop:
                            if (splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                            {
                                if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                {
                                    if (this.Bottom >= r.Y)
                                    {
                                        this.Bounds = new Rectangle(this.Left + animationIncrement, 0, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Top)
                                    {
                                        if (incre)
                                        {
                                            increment -= animationIncrement;
                                            inc = increment;
                                        }
                                        else
                                        {
                                            increment -= animationIncrement;
                                            inc -= animationIncrement;
                                        }
                                        if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                        {
                                            inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                            incre = false;
                                        }
                                        if (inc < this.Height)
                                        {
                                            this.Region = new Region(new Rectangle(0, this.Height - inc, this.Width, inc));
                                            this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                        }
                                    }
                                    else
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                        increment = 0;
                                        incre = true;
                                        inc = 0;
                                    }
                                }
                            }
                            else
                            {

                                if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                {
                                    if (this.Bottom >= r.Y)
                                    {
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Top)
                                    {
                                        if (incre)
                                        {
                                            increment -= animationIncrement;

                                            inc = increment;
                                        }
                                        else
                                        {
                                            increment -= animationIncrement;
                                            inc -= animationIncrement;
                                        }
                                        if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                        {
                                            inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                            incre = false;
                                        }
                                        if (inc < this.Height)
                                        {
                                            this.Region = new Region(new Rectangle(0, this.Height - inc, this.Width, inc));
                                            this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                        }
                                    }
                                    else
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                        increment = 0;
                                        incre = true;
                                        inc = 0;
                                    }
                                }
                            }
                            break;
                        case SplashAlignment.LeftBottom:
                            if (this.Top <= r.Bottom)
                            {
                                if (incre)
                                {
                                    increment -= animationIncrement;
                                    inc = increment;
                                }
                                else
                                {
                                    increment -= animationIncrement;
                                    inc -= animationIncrement;
                                }
                                if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                {
                                    inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                    incre = false;
                                }
                                if (splashPanel.AnimationDirection == AnimationDirection.LeftToRight)
                                {
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                                    this.Bounds = new Rectangle(this.Left - animationIncrement, this.Top, this.Width, this.Height);
                                }
                                else
                                {
                                    this.Region = new Region(new Rectangle(0, 0, this.Width, inc));
                                    this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                                }
                            }
                            else
                            {
                                this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                this.Visible = false;
                                this.hideAnimationTimer.Stop();
                                increment = 0;
                                incre = true;
                                inc = 0;
                            }
                            break;
                        case SplashAlignment.LeftTop:
                            if (splashPanel.AnimationDirection == AnimationDirection.LeftToRight)
                            {
                                if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                {
                                    if (this.Bottom >= r.Y)
                                    {
                                        this.Bounds = new Rectangle(this.Left - animationIncrement, 0, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Top)
                                    {
                                        if (incre)
                                        {
                                            increment -= animationIncrement;
                                            inc = increment;
                                        }
                                        else
                                        {
                                            increment -= animationIncrement;
                                            inc -= animationIncrement;
                                        }
                                        if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                        {
                                            inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                            incre = false;
                                        }
                                        if (inc < this.Height)
                                        {
                                            this.Region = new Region(new Rectangle(0, this.Height - inc, this.Width, inc));
                                            this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                        }
                                    }
                                    else
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();

                                        increment = 0;
                                        incre = true;
                                        inc = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (this.GetAppbarPosition() != NativeMethods.ABE_TOP)
                                {
                                    if (this.Bottom >= r.Y)
                                    {
                                        this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                    }
                                    else
                                    {
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                    }
                                }
                                else
                                {
                                    if (this.Bottom > r.Top)
                                    {
                                        if (incre)
                                        {
                                            increment -= animationIncrement;
                                            inc = increment;
                                        }
                                        else
                                        {
                                            increment -= animationIncrement;
                                            inc -= animationIncrement;
                                        }
                                        if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                        {
                                            inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                            incre = false;
                                        }
                                        if (inc < this.Height)
                                        {
                                            this.Region = new Region(new Rectangle(0, this.Height - inc, this.Width, inc));
                                            this.Bounds = new Rectangle(this.Left, this.Top - animationIncrement, this.Width, this.Height);
                                        }
                                    }
                                    else
                                    {
                                        this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                        this.Visible = false;
                                        this.hideAnimationTimer.Stop();
                                        increment = 0;
                                        incre = true;
                                        inc = 0;
                                    }
                                }
                            }
                            break;
                        case SplashAlignment.Center:
                            this.Visible = false;
                            this.hideAnimationTimer.Stop();
                            break;
                        case SplashAlignment.Custom:
                            this.Visible = false;
                            this.hideAnimationTimer.Stop();
                            break;
                        default:
                            if (this.Top <= r.Bottom)
                            {
                                if (incre)
                                {
                                    increment -= animationIncrement;
                                    inc = increment;
                                }
                                else
                                {
                                    increment -= animationIncrement;
                                    inc -= animationIncrement;
                                }
                                if (this.Height - increment != animationIncrement && increment >= this.Height - animationIncrement)
                                {
                                    inc = increment + (this.Height % animationIncrement) - animationIncrement;
                                    incre = false;
                                }
                                this.Region = new Region(new Rectangle(0, 0, this.Width, inc));
                                this.Bounds = new Rectangle(this.Left, this.Top + animationIncrement, this.Width, this.Height);
                            }
                            else
                            {
                                this.Region = new Region(new Rectangle(0, 0, 0, 0));
                                this.Visible = false;
                                this.hideAnimationTimer.Stop();
                                increment = 0;
                                incre = true;
                                inc = 0;
                            }
                            break;
                     }
                }

                this.Invalidate(this.ClientRectangle, false);
                if (this.hideAnimationTimer.Enabled == false)
                {
                    this.SplashWrapperFormListener.SplashFormClosedNotify();
                }
            }
        }

        /// <summary>
        /// Hides this SplashWrapperForm.
        /// </summary>
        /// <remarks>
        /// This method is invoked by the <see cref="HideSplash"/> method.
        /// <para>
        /// This method also notifies the <see cref="SplashPanel"/> that the
        /// window is being closed.
        /// </para>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void HideWindow()
        {
            this.Hide();
            if (this.SplashWrapperFormOwner != null && this.SplashWrapperFormOwner.Enabled == false)
                this.SplashWrapperFormOwner.Enabled = true;
            this.SplashWrapperFormListener.SplashFormClosedNotify();
        }

        /// <summary>
        /// Private helper function used for the animated display.
        /// </summary>
        /// <param name="desktopAlignment">The relative alignment with the desktop.</param>
        /// <returns>The style to be passed to the AnimateWindow function.</returns>
        private SlideStyle GetSlideStyle(SplashAlignment desktopAlignment)
        {
            SplashAlignment align = desktopAlignment;
            SlideStyle slideStyle = this.SplashPanel.SlideStyle;

            if (slideStyle == SlideStyle.Default)
            {
                switch (align)
                {
                    case SplashAlignment.LeftBottom:
                        slideStyle = SlideStyle.Horizontal;
                        break;
                    case SplashAlignment.LeftTop:
                        slideStyle = SlideStyle.Horizontal;
                        break;
                    case SplashAlignment.RightBottom:
                        slideStyle = SlideStyle.Vertical;
                        break;
                    case SplashAlignment.RightTop:
                        slideStyle = SlideStyle.Vertical;
                        break;
                    case SplashAlignment.SystemTray:
                        switch (this.GetAppbarPosition())
                        {
                            case NativeMethods.ABE_LEFT: // LeftBottom
                                slideStyle = SlideStyle.Horizontal;
                                break;
                            case NativeMethods.ABE_RIGHT: // RightBottom
                                slideStyle = SlideStyle.Horizontal;
                                break;
                            case NativeMethods.ABE_TOP: // RightTop
                                slideStyle = SlideStyle.Vertical;
                                break;
                            case NativeMethods.ABE_BOTTOM: // RightBottom
                                slideStyle = SlideStyle.Vertical;
                                break;
                            default:
                                slideStyle = SlideStyle.Vertical;
                                break;
                        }
                        break;

                    default:
                        slideStyle = SlideStyle.Horizontal;
                        break;
                }
            }

            return slideStyle;
        }

        /// <summary>
        /// Private helper method that gets the position of the Windows Taskbar.
        /// </summary>
        /// <returns>The position of the windows taskbar.</returns>
        private int GetAppbarPosition()
        {
            NativeMethods.APPBARDATA appBarData = new NativeMethods.APPBARDATA();
            appBarData.cbSize = Marshal.SizeOf(appBarData);
            if (NativeMethods.SHAppBarMessage(NativeMethods.ABM_GETTASKBARPOS, ref appBarData) > 0)
                return appBarData.uEdge;
            else
                return NativeMethods.ABE_BOTTOM;
        }

        /// <summary>
        /// The display location based on the current work area and the 
        /// desired alignment.
        /// </summary>
        /// <param name="desktopAlignment">Splash Alignment</param>
        /// <returns>The point where the splash window is to be displayed.</returns>
        private Point GetDisplayLocation(SplashAlignment desktopAlignment)
        {
            Point pt = new Point(10, 10);
            Control ctrl = this.SplashWrapperFormOwner != null ? this.SplashWrapperFormOwner : this;
            Rectangle workRect = Screen.GetWorkingArea(ctrl);

            if (SplashPanel.SlideStyle != SlideStyle.Slide && splashPanel.SlideStyle!= SlideStyle.Marquee)
            {
                switch (desktopAlignment)
                {
                    case SplashAlignment.Center:
                        pt = new Point(workRect.X +( workRect.Width - this.Width) / 2, workRect.Y +(workRect.Height - this.Height) / 2);
                        break;

                    case SplashAlignment.SystemTray:

                        int appbarDock = this.GetAppbarPosition();
                        switch (appbarDock)
                        {
                            case NativeMethods.ABE_LEFT: // LeftBottom
                                pt = new Point(workRect.X + 10, workRect.Y + workRect.Height - this.Height);
                                break;
                            case NativeMethods.ABE_RIGHT: // RightBottom
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + workRect.Height - this.Height);
                                break;
                            case NativeMethods.ABE_TOP: // RightTop
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + 10);
                                break;
                            case NativeMethods.ABE_BOTTOM: // RightBottom
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + workRect.Height - this.Height);
                                break;
                            default:
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + workRect.Height - this.Height);
                                break;
                        }
                        break;

                    case SplashAlignment.LeftBottom:
                        pt = new Point(workRect.X + 10, workRect.Y + workRect.Height - this.Height);
                        break;

                    case SplashAlignment.LeftTop:
                        pt = new Point(workRect.X + 10, workRect.Y + 10);
                        break;

                    case SplashAlignment.RightBottom:
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + workRect.Height - this.Height);
                        break;

                    case SplashAlignment.RightTop:
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y + 10);
                        break;

                    case SplashAlignment.Custom:
                        pt = this.SplashPanel.DiscreetLocation;
                        break;

                    default:
                        pt = new Point(workRect.X + (workRect.Width - this.Width) / 2, workRect.Y + (workRect.Height - this.Height) / 2);
                        break;
                }
            }
            else if (SplashPanel.SlideStyle == SlideStyle.Slide)
            {
                switch(desktopAlignment)
                {
                    case SplashAlignment.Center:
                        pt = new Point(workRect.X + (workRect.Width - this.Width) / 2, workRect.Y+(workRect.Height - this.Height) / 2);
                        break;
                    case SplashAlignment.SystemTray:
                        int appbarDock = this.GetAppbarPosition();
                        switch (appbarDock)
                        {
                            case NativeMethods.ABE_LEFT: // LeftBottom
                                pt = new Point(workRect.X + 10, workRect.Height);
                                break;
                            case NativeMethods.ABE_RIGHT: // RightBottom
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Height);
                                break;
                            case NativeMethods.ABE_TOP: // RightTop
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y - this.Height);
                                break;
                            case NativeMethods.ABE_BOTTOM: // RightBottom
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Height);
                                break;
                            default:
                                pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Height);
                                break;
                        }
                        break;
                    case SplashAlignment.RightBottom:
                        if(splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10 + splashPanel.Width, workRect.Bottom);
                        else
                            pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Bottom);
                        break;
                    case SplashAlignment.RightTop:
                        if(splashPanel.AnimationDirection == AnimationDirection.RightToLeft)
                            pt = new Point(workRect.X + workRect.Width - this.Width - 10 + splashPanel.Width, workRect.Bottom);
                        else
                            pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y - this.Height);
                        break;
                    case SplashAlignment.LeftBottom:
                        if(splashPanel.AnimationDirection == AnimationDirection.Default)
                            pt = new Point(workRect.X + 10, workRect.Bottom);
                        else
                            pt = new Point(workRect.X -this.Width , workRect.Bottom); 
                        break;
                    case SplashAlignment.LeftTop:
                        if(splashPanel.AnimationDirection == AnimationDirection.Default)
                        pt = new Point(workRect.X + 10, workRect.Y-this.Height);
                        else
                            pt = new Point(workRect.X -this.Width , workRect.Y - this.Height);
                        break;
                    case SplashAlignment.Custom:
                        pt = this.SplashPanel.DiscreetLocation;
                        break;
                    default:
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Height);
                        break;
                 }
            }
            else if (splashPanel.SlideStyle == SlideStyle.Marquee)
            {
                if (splashPanel.MarqueePosition == MarqueePosition.BottomLeft)
                {
                    if (splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.LeftToRight)
                    {
                        pt = new Point(workRect.X - this.Width, workRect.Bottom); 
                    }
                    else if (splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.BottomToTop)
                    {
                        pt = new Point(workRect.X + 10, workRect.Bottom);
                    }
                }
                else if (this.splashPanel.MarqueePosition == MarqueePosition.BottomRight)
                {
                    if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.RightToLeft)
                    {
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10 + splashPanel.Width, workRect.Bottom);
                    }
                    else if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.BottomToTop)
                    {
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Bottom);
                    }
                }
                else if (this.splashPanel.MarqueePosition == MarqueePosition.Topleft)
                {
                    if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.LeftToRight)
                    {
                        pt = new Point(workRect.X - this.Width, workRect.Y - this.Height);
                    }
                    else
                    {
                        pt = new Point(workRect.X + 10, workRect.Y - this.Height);
                    }
                }
                else if (this.splashPanel.MarqueePosition == MarqueePosition.Topright)
                {
                    if (this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.TopToBottom)
                    {
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10, workRect.Y - this.Height);
                    }
                    else if(this.splashPanel.MarqueeDirection == SplashPanelMarqueeDirection.RightToLeft)
                    {
                        pt = new Point(workRect.X + workRect.Width - this.Width - 10 + splashPanel.Width, workRect.Bottom);
                    }
                }
            }
            return pt;
        }

        /// <summary>
        /// Attaches the SplashPanel to the SplashWrapperForm.
        /// </summary>
        /// <remarks>
        /// This method associates the <see cref="SplashPanel"/> object with this
        /// SplashWrapperForm. This adds the SplashPanel control to the form and
        /// sets its position.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal virtual void AttachSplash()
        {
            try
            {
                if (SplashPanel != null)
                {
                    this.Controls.Add(SplashPanel);
                    ComputeControlLocation();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sets the size of the host to the size of the SplashPanel and adjusts for 
        /// the borderwidth.
        /// </summary>
        /// <remarks>
        /// The size of the <see cref="SplashPanel"/> is adjusted to be the
        /// size of the SplashWrapperForm except for the border.
        /// </remarks>
        protected virtual void ComputeMySize()
        {
            Size formSize = GetComputedSize(SplashPanel.Size);

            if (this.Size != formSize)
                this.Size = formSize;
        }

        protected Size GetComputedSizeInternal(Size controlSize)
        {
            Size formSize = controlSize;
            if (this.customRegion == false && this.SplashPanel.BorderType == SplashBorderType.Border3D)
                formSize += new Size(borderGap, borderGap);
            return formSize;
        }
        internal Size GetComputedSize(Size controlSize)
        {
            if (InvokeRequired)
            {
                return (Size)this.Invoke(new GetComputedSizeEventHandler(GetComputedSizeInternal), new object[] { controlSize });                  
            }
            else
            {
                return GetComputedSizeInternal(controlSize);
            }
        }

        /// <summary>
        /// Hides the SplashWrapperForm.
        /// </summary>
        /// <remarks>
        /// This method calls the <see cref="HideWindow"/> or <see cref="HideWindowAnimated"/>
        /// method to hide the splash window. If this is a modal window, the DialogResult
        /// is set to DialogResult.OK.
        /// </remarks>
        public void HideSplash()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(HideSplashInternal));
            }
            else
            {
                HideSplashInternal();
            }
        }

        /// <summary>
        /// Hides the SplashWrapperForm.
        /// </summary>
        /// <remarks>
        /// This method calls the <see cref="HideWindow"/> or <see cref="HideWindowAnimated"/>
        /// method to hide the splash window. If this is a modal window, the DialogResult
        /// is set to DialogResult.OK.
        /// </remarks>
        protected virtual void HideSplashInternal()
        {
            if (this.SplashPanel.ShowAnimation == true)
                this.HideWindowAnimated();
            else
                this.HideWindow();
        }

        /// <summary>
        /// Hides the SplashWrapperForm window in an animated manner.
        /// </summary>
        /// <remarks>
        /// Called by the <see cref="HideSplash"/> method.
        /// </remarks>
        private void HideWindowAnimated()
        {
             this.animationTimer.Stop();
            this.hideAnimationTimer.Start();
            if (this.SplashWrapperFormOwner != null && this.SplashWrapperFormOwner.Enabled == false)
                this.SplashWrapperFormOwner.Enabled = true;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_ENABLE:
                    // If owner form is shown using ShowDialog methods then WrappesForm has disabled state.
                    // To workaround Enable state is set for WrapperForm.
                    NativeMethods.EnableWindow(this.Handle, true);
                    break;
                case NativeMethods.WM_NCCALCSIZE:
                    // Redraw on resize
                    m.Result = m.WParam == IntPtr.Zero ? IntPtr.Zero : (IntPtr)0x0300;
                    return;
                case NativeMethods.WM_ERASEBKGND:
                    m.Result = (IntPtr)1;
                    return;
                case NativeMethods.WM_NCHITTEST:
                    {
                        SplashPanel panel = this.SplashPanel;
                        if (panel != null && panel.AllowResize)
                        {
                            int x = NativeMethods.LOWORD(m.LParam);
                            int y = NativeMethods.HIWORD(m.LParam);

                            Point pt = panel.PointToClient(new Point(x, y));

                            int hitTest = panel.GetCurrentBorderArea(pt);
                            if (hitTest != NativeMethods.HTNOWHERE)
                            {
                                m.Result = (IntPtr)hitTest;
                                return;
                            }
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }
    }
    #endregion

    /// <summary>
    /// Specifies the mode in which to show the Splash.
    /// </summary>
    internal enum SplashWindowShowMode
    {
        /// <summary>
        /// Represents Modeless
        /// </summary>
        Modeless = 0,

        /// <summary>
        /// Represents Disable owner
        /// </summary>
        DisableOwner = 1,

        /// <summary>
        /// Represents Modal
        /// </summary>
        Modal = 2
    }

    /// <summary>
    /// Specifies the border type for the Splash.
    /// </summary>
    public enum SplashBorderType
    {
        /// <summary>
        /// Borders in 3D style
        /// </summary>
        Border3D = 0,

        /// <summary>
        /// No border. 
        /// </summary>
        None = 1
    }
}