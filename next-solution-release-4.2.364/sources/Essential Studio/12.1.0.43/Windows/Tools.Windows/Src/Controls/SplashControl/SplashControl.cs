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
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The SplashControl class provides an easy to use class that can be used
    /// to display splash screens.
    /// </summary>
    /// <remarks>
    /// The SplashControl can just be dragged and dropped on to your form
    /// from the controls toolbox. The SplashControl is implemented as a component
    /// that is not visible at run time. It is visible in the component area
    /// of your form where you can select it and set its properties.
    /// <para>
    /// If the <see cref="AutoMode"/> property is set to true, the SplashControl
    /// will automatically be launched from the Load event of the host form (your
    /// application's startup form).
    /// </para>
    /// <para>
    /// If the <see cref="AutoMode"/> is set to false, you need to call <see cref="ShowSplash"/>
    /// at the appropriate time and the Splash screen will be displayed.
    /// </para>
    /// <para>
    /// The SplashControl is by default a timed display splash screen. What this
    /// means is that the Splash screen will be displayed for a specified period
    /// and then closed.
    /// </para>
    /// <para>
    /// The <see cref="TimerInterval"/> property specifies for how long the splash
    /// screen should be visible. The unit of measurement for this property is milli
    /// seconds.
    /// </para>
    /// <para>
    /// The Splash screen can be made into a non timed splash screen by setting the
    /// <see cref="TimerInterval"/> property to -1.
    /// </para>
    /// <para>
    /// In this case you have to call <see cref="HideSplash"/> method to close the non timed
    /// Splash screen.This approach is more suitable when you are doing some
    /// background processing in the main form and you want to keep the SplashScreen
    /// up till the work is done.
    /// </para>
    /// <para>
    /// The <see cref="SplashImage"/> property needs to be set to your image for
    /// the Splash screen. The SplashControl can only display a image as the
    /// splash screen. Please refer to the <see cref="SplashPanel"/> class if you want
    /// more flexibility in building a splash screen (or splash message box) that
    /// provides more control in terms of displaying information and collecting response.
    /// </para>
    /// <para>
    /// You can handle the <see cref="BeforeSplash"/> event to process any code just
    /// before the Splash screen is displayed.
    /// </para>
    /// <para>
    /// The <see cref="SplashDisplayed"/> event is raised after the Splash screen
    /// is shown on the screen and the <see cref="SplashClosed"/> event is raised
    /// after the Splash screen is closed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <coderef file="tools\samples\notification package\SplashControlDemo\CS\MainForm.cs" name="SplashControl InitializeComponent" lang="C#">
    /// <code lang="C#">
    ///             // InitializeComponent
    ///             System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
    ///             // Create the splash control
    ///             this.splashControl1 = new SplashControl();
    ///             // Setting AutoMode to true will automatically launch
    ///             // splash screen - no additional code is required
    ///             this.splashControl1.AutoMode = true;
    ///             // The start position for the splash screen
    ///             this.splashControl1.DesktopAlignment = Syncfusion.Windows.Forms.Tools.SplashPanel.SplashAlignment.Center;
    ///             // The host form for the splash control
    ///             this.splashControl1.HostForm = this;
    ///             // Specifies if the splash screen should appear animated
    ///             this.splashControl1.ShowAnimation = false;
    ///             // The Splash image - specified through the designer
    ///             this.splashControl1.SplashImage = ((System.Drawing.Bitmap)(resources.GetObject("splashControl1.SplashImage")));
    ///             // The time period for which the splash should appear
    ///             this.splashControl1.TimerInterval = 10000;
    /// </code></coderef>
    /// <coderef file="tools\samples\notification package\SplashControlDemo\VB\MainForm.vb" name="SplashControl InitializeComponent" lang="VB"><code lang="VB">
    ///            ' InitializeComponent
    ///            Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(MainForm))
    ///            ' Create the splash control
    ///            Me.splashControl1 = New SplashControl()
    ///            ' Setting AutoMode to true will automatically launch
    ///            ' splash screen - no additional code is required
    ///            Me.splashControl1.AutoMode = True
    ///            ' The start position for the splash screen
    ///            Me.splashControl1.DesktopAlignment = Syncfusion.Windows.Forms.Tools.SplashPanel.SplashAlignment.Center
    ///            ' The host form for the splash control
    ///            Me.splashControl1.HostForm = Me
    ///            ' Specifies if the splash screen should appear animated
    ///            Me.splashControl1.ShowAnimation = False
    ///            ' The Splash image - specified through the designer
    ///            Me.splashControl1.SplashImage = CType(resources.GetObject("splashControl1.SplashImage"), System.Drawing.Bitmap)
    ///            ' The time period for which the splash should appear
    ///            Me.splashControl1.TimerInterval = 10000
    /// </code></coderef>
    /// </example>
    [
    Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.SplashControlDesigner), typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(SplashControl), "ToolboxIcons.SplashControl.bmp"),
    ToolboxItemFilter("System.Windows.Forms"),
    Description("Provides an easy to use class that can be used to display splash screens.")
    ]
    public class SplashControl : Component, ISplashParent
    {
       #region FIELDS

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Indicates whether the splash control will operate in auto mode.
        /// In auto mode the control automatically binds to the host form's
        /// load event and displays the splash screen for the specified period
        /// of time.
        /// </summary>
        private bool autoModeValue;

        /// <summary>
        /// The maximum display time allowed. The property DisplayTime
        /// uses this to reject any values greater than this.
        /// </summary>
       private static int maxDisplayTime;

        /// <summary>
        /// The host form for this control. Accessors provided for
        /// this field by <see cref="HostForm"/>.
        /// </summary>
        private Form hostFormObject;

        /// <summary>
        /// Holds the current display status of the splash window.
        /// </summary>
        private bool isShowingValue;

        /// <summary>
        /// The form to be used as the Splash Form. Accessors provided 
        /// by SplashForm.
        /// </summary>
        private DefaultPanel splashControlDefaultPanelObject = null;

        /// <summary>
        /// The background image for the default splash screen. Accessors
        /// provided by <see cref="SplashImage"/>.
        /// </summary>
        private Image splashImageValue = null;

        /// <summary>
        /// Hides the host form for the duration of the Splash.
        /// </summary>
        private bool hideHostForm = false;

        /// <summary>
        /// The location of the host form before it is hidden.
        /// </summary>
        private Point hostFormLocation = Point.Empty;

        /// <summary>
        /// The HostForm's size before being hidden.
        /// </summary>
        private Size hostFormSize = Size.Empty;

        /// <summary>
        /// Specifies the initial WindowState of the HostForm.
        /// </summary>
        private FormWindowState hostFormWindowState = FormWindowState.Normal;

        /// <summary>
        /// Holds the HostForm's ShowInTaskbar value.
        /// </summary>
        private bool hostFormShowInTaskBar = true;

        /// <summary>
        /// Indicates a custom panel if specified.
        /// </summary>
        private SplashPanel customPanel = null;

        /// <summary>
        /// Indicates whether a custom panel is to be used as the SplashScreen.
        /// </summary>
        private bool useCustomPanel;

        /// <summary>
        /// Indicates whether the Splash Screen's owner is to be disabled when in AutoMode.
        /// </summary>
        private bool autoModeDisableOwner = false;

        /// <summary>
        /// Indicates whether the needed information of host form is saved to restore.
        /// </summary>
        private bool hostFormSaved = false;

        /// <summary>
        /// Indicates whether the SplashControl needs to be disposed
        /// </summary>
        private bool closeSplashForm = false;

        #endregion

        #region INITIALIZATION

        static SplashControl()
        {
            SplashControl.maxDisplayTime = 60000;
        }

        /// <summary>
        /// Initializes a new instance of the SplashControl class. 
        /// </summary>
        /// <remarks>
        /// The SplashControl class uses the <see cref="SplashPanel"/> class as
        /// the splash screen. It creates a SplashPanel object internally and manipulates
        /// it based on the settings and the methods invoked.
        /// <para>
        /// The <see cref="AutoMode"/> property is initialized to true.
        /// </para>
        /// <para>
        /// You need to specify the <see cref="SplashImage"/> property for the 
        /// splash screen's background.
        /// </para>
        /// </remarks>
        public SplashControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SplashControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.splashControlDefaultPanelObject = new DefaultPanel();
            this.splashControlDefaultPanelObject.SplashParent = this;
            this.autoModeValue = true;
            this.isShowingValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the SplashControl class.
        /// </summary>
        /// <param name="backImage">The background image for the default splash screen.</param>
        /// <remarks>
        /// This sets the background image of the <see cref="SplashPanel"/> that will be
        /// displayed as the splash screen.
        /// </remarks>
        public SplashControl(Image backImage)
            : this()
        {
            // Since the constructor is initialized with a Image, we
            // use the interal form
            if (this.splashControlDefaultPanelObject != null)
            {
                ((DefaultPanel)this.splashControlDefaultPanelObject).BackgroundImage = backImage;
            }
            else
                this.splashControlDefaultPanelObject = new DefaultPanel(backImage);
        }

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        #region SPLASH ACTION EVENTS
        /// <summary>
        /// The event that is raised before the splash screen is
        /// displayed.
        /// </summary>
        /// <remarks>
        /// The <see cref="CancelEventHandler"/> can be used by a handling class
        /// to cancel the splash screen from being displayed. You can also handle
        /// this event to get notified just before the Splash screen is displayed.
        /// You can perform custom actions in the handler as per your requirements.
        /// </remarks>
        [Description("The event that is raised before the splash screen displayed.")]
        public event CancelEventHandler BeforeSplash;

        /// <summary>
        /// The event that is raised after the splash screen is displayed.
        /// </summary>
        /// <remarks>
        /// This event is raised after the Splash screen is made visible to the
        /// user. This can also be handled to do some custom processing like
        /// displaying a status message in the main form.
        /// </remarks>
        [Description("The event that is raised after the splash screen is displayed.")]
        public event EventHandler SplashDisplayed;

        /// <summary>
        /// This event is raised when the splash window is closed.
        /// </summary>
        /// <remarks>
        /// This event is raised after the splash screen has been closed.
        /// Handle this to perform any step after the screen is gone. One 
        /// use would be to hide the main form before displaying the splash
        /// screen and then making the main form visible in this event's
        /// handler.
        /// </remarks>
        [Description("This event is raised when the splash window is closed.")]
        public event EventHandler SplashClosed;

        /// <summary>
        /// The event that is raised before the <see cref="SplashControl"/> is closed.
        /// </summary>
        /// <remarks>
        /// This event is raised before the SplashControl is closed and it can be
        /// handled to stop the SplashControl from closing or to some custom processing.
        /// </remarks>
        [Description("The event that is raised before the SplashControl is closed.")]
        public event CancelEventHandler SplashClosing;

        /// <summary>
        /// Raises the <see cref="SplashClosing"/> event. When overriding this
        /// make sure you call this base version.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// The <see cref="CancelEventHandler"/> can be used by a handling class
        /// to cancel the splash screen from being closed. You can also handle
        /// this event to get notified just before the Splash screen is closed.
        /// You can perform custom actions in the handler as per your requirements.
        /// </remarks>
        protected virtual void OnSplashClosing(CancelEventArgs e)
        {
            if (this.SplashClosing != null)
            {
                if (null != this.HostForm && this.HostForm.InvokeRequired)
                {
                    this.HostForm.BeginInvoke(this.SplashClosing, new object[] { this, e });
                }
                else
                {
                    this.SplashClosing(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SplashClosed"/> event. When overriding this
        /// make sure you call this base version.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="SplashClosed"/> event is raised after the splash screen has been closed.
        /// Handle this to perform any step after the screen is gone. One
        /// use would be to hide the main form before displaying the splash
        /// screen and then making the main form visible in this event's
        /// handler.
        /// </para>
        /// <coderef file="tools\samples\notification package\SplashControlDemo\CS\MainForm.cs" name="SplashControl SplashClosed event." lang="C#"><code lang="C#">
        /// private void splashControl1_SplashClosed(object sender, System.EventArgs e)
        /// {
        /// MessageBox.Show("The Splash screen has closed.");
        /// }
        /// </code></coderef>
        /// <coderef file="tools\samples\notification package\SplashControlDemo\VB\MainForm.vb" name="SplashControl SplashClosed event." lang="VB"><code lang="VB">
        /// Private Sub splashControl1_SplashClosed(ByVal sender As Object, ByVal e As System.EventArgs)
        /// MessageBox.Show("The Splash screen has closed.")
        /// End Sub
        /// </code></coderef>
        /// </remarks>
        protected virtual void OnSplashClosed(EventArgs e)
        {
            if (this.SplashClosed != null)
            {
                if (null != this.HostForm && this.HostForm.InvokeRequired)
                {
                    this.HostForm.BeginInvoke(this.SplashClosed, new object[] { this, e });
                }
                else
                {
                    this.SplashClosed(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="BeforeSplash"/> event. When overriding this
        /// make sure you call this base version.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// The <see cref="CancelEventHandler"/> can be used by a handling class
        /// to cancel the splash screen from being displayed. You can also handle
        /// this event to get notified just before the Splash screen is displayed.
        /// You can perform custom actions in the handler as per your requirements.
        /// </remarks>
        protected virtual void OnBeforeSplash(CancelEventArgs e)
        {
            if (this.BeforeSplash != null)
            {
                if (null != this.HostForm && this.HostForm.InvokeRequired)
                {
                    this.HostForm.BeginInvoke(this.BeforeSplash, new object[] { this, e });
                }
                else
                {
                    this.BeforeSplash(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SplashDisplayed"/> event. When overriding this
        /// make sure you call this base version.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// The <see cref="SplashDisplayed"/> event is raised after the Splash screen is made visible to the
        /// user. This can also be handled to do some custom processing like
        /// displaying a status message in the main form.
        /// </remarks>
        protected virtual void OnSplashDisplayed(EventArgs e)
        {
            if (this.SplashDisplayed != null)
            {
                if (null != this.HostForm && this.HostForm.InvokeRequired)
                {
                    this.HostForm.BeginInvoke(this.SplashDisplayed, new object[] { this, e });
                }
                else
                {
                    this.SplashDisplayed(this, e);
                }
            }
        }

        #endregion

        #region BEHAVIOR
        /// <summary>
        /// Gets or sets a value indicating whether the splash screen should appear on the
        /// screen in an animated manner.
        /// </summary>
        /// <remarks>
        /// The splash screen will be animated if the value is set to true.
        /// </remarks>
        [
        Category("Appearance"),
        Browsable(true),
        DefaultValue(false),
        Description("Indicates whether the splash screen should appear on the screen in an animated manner.")
        ]
        public bool ShowAnimation
        {
            get
            {
                return this.SplashControlPanel.ShowAnimation;
            }

            set
            {
                this.SplashControlPanel.ShowAnimation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the host form should be hidden when the splash screen is 
        /// displayed.
        /// </summary>
        [
        Category("Behavior"),
        Browsable(true),
        DefaultValue(false),
        Description("Specifies if the host form should be hidden when the splash screen is displayed.")
        ]
        public bool HideHostForm
        {
            get
            {
                return this.hideHostForm;
            }

            set
            {
                this.hideHostForm = value;
            }
        }

        /// <summary>
        /// Gets or sets the initial <see cref="Form.WindowState"/> of the <see cref="HostForm"/>
        /// </summary>
        [
        Category("Behavior"),
        Browsable(true),
        DefaultValue(FormWindowState.Normal),
        Description("Specifies if the host form should be hidden when the splash screen is displayed.")
        ]
        public FormWindowState HostFormWindowState
        {
            get
            {
                return this.hostFormWindowState;
            }

            set
            {
                this.hostFormWindowState = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the splash screen is currently being displayed.
        /// </summary>
        /// <remarks>
        /// This property is not visible at runtime. This returns an internal
        /// value that is maintained to indicate if the Splash panel is visible.
        /// </remarks>
        [
        Browsable(false)
        ]
        public bool IsShowing
        {
            get
            {
                return this.isShowingValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SplashControl control operates in AutoMode.
        /// </summary>
        /// <remarks>
        /// When AutoMode is set to true, the control will automatically bind 
        /// to the Load event of the host form, hide the host form and display 
        /// the splash form as per the <see cref="TimerInterval"/> settings.
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("Specifies if the SplashControl should automatically launch the splash screen."),
        DefaultValue(true)
        ]
        public bool AutoMode
        {
            get
            {
                return this.autoModeValue;
            }

            set
            {
                if (value == false)
                {
                    if (this.HostForm != null)
                        this.HostForm.Load -= new EventHandler(this.HandleHostFormLoad);
                }
                else
                {
                    if (this.HostForm != null)
                        this.HostForm.Load += new EventHandler(this.HandleHostFormLoad);
                }
                this.autoModeValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Splash Control is displayed modally in AutoMode.
        /// </summary>
        /// <remarks>
        /// When AutoModeModal is set to true, the splash screen will be displayed
        /// modally.
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("Specifies if the SplashControl displays modally when in AutoMode."),
        DefaultValue(false)
        ]
        public bool AutoModeDisableOwner
        {
            get
            {
                return this.autoModeDisableOwner;
            }

            set
            {
                this.autoModeDisableOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets the desktop alignment for the splash screen.
        /// </summary>
        /// <remarks>
        /// The <see cref="SplashAlignment"/> type lists the values that
        /// this property can take. The default value is <see cref="SplashAlignment.Center"/>
        /// which places the splash screen in the center of the screen.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Sets the desktop alignment for the splash control"),
        DefaultValue(SplashAlignment.Center)
        ]
        public SplashAlignment DesktopAlignment
        {
            get
            {
                return this.SplashControlPanel.DesktopAlignment;
            }

            set
            {
                this.SplashControlPanel.DesktopAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Splash screen shown by the SplashControl
        /// is shown as a TopMost window when displayed.
        /// </summary>
        /// <value>
        /// True if the Splash screen is displayed as a TopMost window; false otherwise. The default is true.
        /// </value>
        /// <remarks>
        /// Displaying the Splash screen as a TopMost window makes the Splash screen appear on top of
        /// all other windows.
        /// <para>
        /// If this property is to be false and the Splash screen is displayed in non modal mode, the
        /// Splash screen might be hidden by the Form displaying the Splash screen. If you want the 
        /// Splash screen to be the TopMost window with respect to the application/Form displaying 
        /// it only, you should display the Splash screen modally with this property set to false.
        /// </para>
        /// </remarks>
        [
        Category("Appearance"),
        Description("Specifies if the Splashscreen is to be displayed as a TopMost window."),
        DefaultValue(true)
        ]
        public bool ShowAsTopMost
        {
            get
            {
                return this.SplashControlPanel.ShowAsTopMost;
            }

            set
            {
                this.SplashControlPanel.ShowAsTopMost = value;
            }
        }

        /// <summary>
        /// Gets or sets the time interval for which the splash screen is to be
        /// displayed (in milliseconds).
        /// </summary>
        /// <remarks>
        /// The default value is 5000 milliseconds (or 5 seconds).
        /// <para>
        /// Set the value to -1 if you want to treat this as a non timed
        /// splash screen. In this case you will need to call <see cref="HideSplash"/>
        /// to close the window.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("The time interval for which the splash screen is to be displayed (in milliseconds)."),
        DefaultValue(5000)
        ]
        public int TimerInterval
        {
            get
            {
                return this.SplashControlPanel.TimerInterval;
            }

            set
            {
                if (value < SplashControl.maxDisplayTime)
                    this.SplashControlPanel.TimerInterval = value;
                else
                    this.SplashControlPanel.TimerInterval = SplashControl.maxDisplayTime;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to dispose splash control's internal form after it has been displayed once.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if splash control's form should be disposed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Gets or sets a value indicating whether to close splash control after it has been displayed once.
        /// If this property is true, SplashControl will be closed after displaying once. In this case trying to show the SplashControl 
        /// more than once will throw an error. If this property is set to false, SplashControl can be displayed any number of times.
        /// </remarks>
        [
        Browsable(true),
        Description("Indicates whether the SplashControl's internal form needs to be disposed after displaying once."),
        DefaultValue(false)
        ]
        public bool CloseSplashForm
        {
            get
            {
                return this.closeSplashForm;
            }
            set
            {
                this.closeSplashForm = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal <see cref="SplashPanel"/> that is displayed as the splash screen.
        /// </summary>
        /// <remarks>
        /// This property is not available at design time. It can be accessed
        /// at run time to set a different <see cref="SplashPanel"/> derived
        /// object as the splash screen.
        /// <para>
        /// The default method to changing the <see cref="SplashPanel"/> object's
        /// look and feel is to set the <see cref="SplashImage"/> property to your
        /// image.
        /// </para>
        /// </remarks>
        [
        Browsable(false)
        ]
        public DefaultPanel SplashControlPanel
        {
            get
            {
                return this.splashControlDefaultPanelObject;
            }

            set
            {
                this.splashControlDefaultPanelObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the host form of this Splash Control.
        /// </summary>
        /// <remarks>
        /// This property is automatically set when the SplashControl is dragged
        /// and dropped on a form. This is used to hook into the Load event of the
        /// host form at run time to launch the splash screen automatically when
        /// the <see cref="AutoMode"/> is set to true.
        /// </remarks>
        [
        Browsable(false)
        ]
        public Form HostForm
        {
            get
            {
                return this.hostFormObject;
            }

            set
            {
                if (value != null && value != hostFormObject)
                {
                    this.hostFormObject = value;
                    if (this.DesignMode == false)
                    {
                        if (this.AutoMode == true)
                            this.hostFormObject.Load += new EventHandler(this.HandleHostFormLoad);
                    }
                }
            }
        }

        /// <summary>
        /// Hides the splash screen if its being displayed.
        /// </summary>
        /// <remarks>
        /// This method does not do anything if the splash screen is
        /// not being displayed.
        /// </remarks>
        public void HideSplash()
        {
            if (this.DesignMode)
            {
                if (this.UseCustomSplashPanel == true && this.CustomSplashPanel != null)
                    this.CustomSplashPanel.HideSplash();
                else
                    this.SplashControlPanel.HideSplash();
            }
            else if (this.isShowingValue)
            {
                if (this.HostForm != null)
                {
                    this.HostForm.Activate();
                    this.HostForm.BringToFront();
                }
                if (this.UseCustomSplashPanel == true && this.CustomSplashPanel != null)
                    this.CustomSplashPanel.HideSplash();
                else
                    this.SplashControlPanel.HideSplash();
            }
        }

        /// <summary>
        /// Event handler for the Load event of the host form. This
        /// applies only when the control's <see cref="AutoMode"/> 
        /// is set to true.
        /// </summary>
        /// <param name="sender">The host form control.</param>
        /// <param name="e">The event args.</param>
        /// <remarks>
        /// This method is not invoked when the <see cref="AutoMode"/> is set to
        /// false. You need to call <see cref="ShowSplash"/> in an appropriate 
        /// place in your form's code to display the splash screen.
        /// </remarks>
        private void HandleHostFormLoad(object sender, EventArgs e)
        {
            this.ShowSplash(this.AutoModeDisableOwner);
        }

        private void HandleHostFormVisibleChanged(object sender, EventArgs e)
        {
            if (HostForm.Visible)
            {
                SaveHostForm();
            }
            HostForm.VisibleChanged -= new EventHandler(HandleHostFormVisibleChanged);
        }

        private void RestoreHostForm()
        {
            if (this.HideHostForm && !this.DesignMode && hostFormSaved)
            {
                this.HostForm.WindowState = this.hostFormWindowState;
                this.HostForm.ShowInTaskbar = this.hostFormShowInTaskBar;
                this.HostForm.Size = this.hostFormSize;
                this.HostForm.Location = this.hostFormLocation;
                hostFormSaved = false;
            }
        }

        private void SaveHostForm()
        {
            if (this.HostForm != null)
            {
                if (HostForm.Visible)
                {
                    if (this.HideHostForm && !this.DesignMode && !hostFormSaved)
                    {
                        this.hostFormShowInTaskBar = this.HostForm.ShowInTaskbar;
                        this.hostFormLocation = this.HostForm.Location;
                        this.hostFormSize = this.HostForm.Size;

                        this.HostForm.Location = new Point(-2000, -2000);
                        this.HostForm.Size = Size.Empty;
                        this.HostForm.ShowInTaskbar = false;

                        hostFormSaved = true;
                    }
                }
                else
                {
                    HostForm.VisibleChanged += new EventHandler(HandleHostFormVisibleChanged);
                }
            }
        }

        /// <summary>
        /// Displays the splash screen.
        /// </summary>
        /// <param name="disableOwner">Indicates whether the splash screen should be displayed modally.</param>
        /// <remarks>
        /// You will need to call this method only when the <see cref="AutoMode"/> property
        /// is set to false.
        /// </remarks>
        public void ShowSplash(bool disableOwner)
        {
            SplashPanel displayPanel = null;
            if (this.UseCustomSplashPanel == true && this.CustomSplashPanel != null)
            {
                displayPanel = this.CustomSplashPanel;
                this.CustomSplashPanel.SplashParent = this;
                this.CustomSplashPanel.TimerInterval = this.TimerInterval;
                this.CustomSplashPanel.ShowAsTopMost = this.ShowAsTopMost;
                this.CustomSplashPanel.ShowAnimation = this.ShowAnimation;
                this.CustomSplashPanel.DesktopAlignment = this.DesktopAlignment;

                this.CustomSplashPanel.FormIcon = this.FormIcon;
                this.CustomSplashPanel.ShowInTaskbar = this.ShowInTaskbar;
                this.CustomSplashPanel.Text = this.Text;
            }
            else
                displayPanel = this.splashControlDefaultPanelObject;
                                        
            this.isShowingValue = true;

            SaveHostForm();

            if (disableOwner || this.HostForm != null)
                displayPanel.ShowSplash(Point.Empty, this.HostForm, disableOwner);
            else
            {
                displayPanel.ShowSplash(Point.Empty, null, disableOwner);
            }
        }

        /// <summary>
        /// Overloaded. Displays the Splash Panel as a modal dialog.
        /// </summary>
        /// <param name="ownerForm">The owner form.</param>
        /// <returns>The DialogResult value.</returns>
        public DialogResult ShowDialogSplash(Form ownerForm)
        {
            return this.ShowDialogSplash(Point.Empty, ownerForm);
        }

        /// <summary>
        /// Displays the Splash Panel as a modal dialog.
        /// </summary>
        /// <param name="location">The location at which the Splash Panel is to be displayed.</param>
        /// <param name="ownerForm">The owner form.</param>
        /// <returns>The DialogResult value.</returns>
        public DialogResult ShowDialogSplash(Point location, Form ownerForm)
        {
            SplashPanel displayPanel = null;
            if (this.UseCustomSplashPanel == true && this.CustomSplashPanel != null)
            {
                displayPanel = this.CustomSplashPanel;
                this.CustomSplashPanel.SplashParent = this;
                this.CustomSplashPanel.TimerInterval = this.TimerInterval;
                this.CustomSplashPanel.ShowAsTopMost = this.ShowAsTopMost;
                this.CustomSplashPanel.ShowAnimation = this.ShowAnimation;
                this.CustomSplashPanel.DesktopAlignment = this.DesktopAlignment;

                this.CustomSplashPanel.FormIcon = this.FormIcon;
                this.CustomSplashPanel.ShowInTaskbar = this.ShowInTaskbar;
                this.CustomSplashPanel.Text = this.Text;
            }
            else
                displayPanel = this.splashControlDefaultPanelObject;

            this.isShowingValue = true;

            return displayPanel.ShowDialogSplash(location, ownerForm);
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
                    components.Dispose();

                this.splashControlDefaultPanelObject.SplashParent = null;

                this.splashControlDefaultPanelObject.Dispose();

                if (hostFormObject != null)
                {
                    if (AutoMode)
                    {
                        hostFormObject.Load -= new EventHandler(HandleHostFormLoad);
                    }
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the default splash screen background image.
        /// </summary>
        /// <remarks>
        /// The SplashImage can be any image that can be assigned to the 
        /// Windows Forms <see cref="Image"/> class. 
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies the default splash screen background image."),
        DefaultValue(null)
        ]
        public Image SplashImage
        {
            get
            {
                return (Image)this.splashImageValue;
            }

            set
            {
                if (value != null && value != splashImageValue)
                {
                    this.splashImageValue = value;
                    if (this.splashControlDefaultPanelObject != null)
                        ((DefaultPanel)this.splashControlDefaultPanelObject).BackgroundImage = this.splashImageValue;
                    else
                        this.splashControlDefaultPanelObject = new DefaultPanel(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color to be used to make the SplashImage transparent.
        /// </summary>
        [
        Description("The color to be used to make the SplashImage transparent.")
        ]
        public Color TransparentColor
        {
            get
            {
                return this.SplashControlPanel.TransparentColor;
            }

            set
            {
                this.SplashControlPanel.TransparentColor = value;
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
        /// Gets or sets a custom <see cref="SplashPanel"/> if the default SplashPanel
        /// is not to be used.
        /// </summary>
        [
        Description("Gets or sets custom splash panel to be used."),
        DefaultValue(null)
        ]
        public SplashPanel CustomSplashPanel
        {
            get
            {
                return this.customPanel;
            }

            set
            {
                this.customPanel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomSplashPanel"/> is to be used.
        /// </summary>
        [
        Description("Specifies whether a custom splash panel should be used."),
        DefaultValue(false)
        ]
        public bool UseCustomSplashPanel
        {
            get
            {
                return this.useCustomPanel;
            }

            set
            {
                this.useCustomPanel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SplashPanel is to be shown in the Taskbar.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates whether the SplashPanel is to be shown in the Taskbar.")]
        public bool ShowInTaskbar
        {
            get
            {
                return this.SplashControlPanel.ShowInTaskbar;
            }

            set
            {
                this.SplashControlPanel.ShowInTaskbar = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon for the SplashPanel.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the icon for the SplashPanel.")]
        public Icon FormIcon
        {
            get
            {
                return this.SplashControlPanel.FormIcon;
            }

            set
            {
                this.SplashControlPanel.FormIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the text for the Splash Control.
        /// </summary>
        public string Text
        {
            get
            {
                return this.SplashControlPanel.Text;
            }

            set
            {
                this.SplashControlPanel.Text = value;
            }
        }

        private bool ShouldSerializeText()
        {
            if (this.Text == String.Empty)
                return false;
            else
                return true;
        }

        #endregion

        #region ISPLASHPARENT

        /// <summary>
        /// This is the notification from the <see cref="SplashPanel"/> used by this Splash Control.
        /// Implementation for <see cref="ISplashParent"/>.
        /// </summary>
        /// <param name="splashPanel">The <see cref="SplashPanel"/> object that has closed.</param>
        /// <param name="splashCloseType">Specifies how the splash screen is closing.</param>
        /// <remarks>
        /// This raises the <see cref="SplashClosed"/> event.
        /// </remarks>
        public void SplashClosedNotify(SplashPanel splashPanel, SplashCloseType splashCloseType)
        {
            if (this.isShowingValue)
            {
                this.OnSplashClosed(EventArgs.Empty);
                this.isShowingValue = false;
                this.RestoreHostForm();
                if(this.HostForm != null)
                   this.HostForm.BringToFront();
            }

            if (closeSplashForm)
            {
                splashPanel.SplashForm.Dispose();
            }
        }

        /// <summary>
        /// This is the notification from the <see cref="SplashPanel"/> that the
        /// splash screen is being displayed.
        /// </summary>
        /// <param name="splashPanel">The <see cref="SplashPanel"/> that is being made visible.</param>
        /// <remarks>
        /// This raises the <see cref="BeforeSplash"/> event.
        /// </remarks>
        /// <returns>Return bool value</returns>
        public bool BeforeSplashNotify(SplashPanel splashPanel)
        {
            CancelEventArgs args = new CancelEventArgs();
            this.OnBeforeSplash(args);
            return args.Cancel;
        }

        /// <summary>
        /// This is the notification from the <see cref="SplashPanel"/> that the
        /// splash screen is closing.
        /// </summary>
        /// <param name="splashPanel">The <see cref="SplashPanel"/> that is closing.</param>
        /// <remarks>
        /// This raises the <see cref="SplashClosing"/> event.
        /// </remarks>
        /// <returns>Return true if event is cancelled</returns>
        public bool SplashClosingNotify(SplashPanel splashPanel)
        {
            CancelEventArgs args = new CancelEventArgs();
            this.OnSplashClosing(args);
            return args.Cancel;
        }

        /// <summary>
        /// This is the notification from the <see cref="SplashPanel"/> that the
        /// splash screen has been displayed.
        /// </summary>
        /// <param name="splashPanel">The <see cref="SplashPanel"/> that has been made visible.</param>
        /// <remarks>
        /// This raises the <see cref="SplashDisplayed"/> event.
        /// </remarks>
        public void SplashDisplayedNotify(SplashPanel splashPanel)
        {
            EventArgs args = new EventArgs();
            this.OnSplashDisplayed(args);
        }

        #endregion

        #region DEFAULTPANEL
        /// <summary>
        /// The default SplashScreen class that will be used internally by
        /// the <see cref="SplashControl"/>.
        /// </summary>
        /// <remarks>
        /// This <see cref="SplashPanel"/> derived class is used by the SplashControl class
        /// as the splash screen that is displayed. The <see cref="BackImage"/> property
        /// is added to this derived class. This is set to the <see cref="SplashControl.SplashImage"/>
        /// object.
        /// <para>
        /// The size of the SplashPanel is set to the size of BackImage.
        /// </para>
        /// </remarks>
        [
        ToolboxItem(false)
        ]
        public class DefaultPanel : SplashPanel
        {
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.Container components = null;

            /// <summary>
            /// Initializes a new instance of the DefaultPanel class. 
            /// </summary>
            /// <remarks>
            /// <para>
            /// The <see cref="SplashPanel.AnimationSpeed"/> is set to 25 and the <see cref="SplashPanel.ShowAnimation"/>
            /// is turned off by default.
            /// </para>
            /// <para>
            /// You need to set the <see cref="BackImage"/> property.
            /// </para>
            /// </remarks>
            public DefaultPanel()
                : base()
            {
                this.AnimationSpeed = 25;
                this.ShowAnimation = false;
               
                // Required for Windows Form Designer support
                InitializeComponent();
            }

            /// <summary>
            /// Initializes a new instance of the DefaultPanel class.
            /// </summary>
            /// <param name="backImage">The image to be displayed in the splash window.</param>
            /// <remarks>
            /// The <see cref="SplashControl"/> creating this sets the BackImage
            /// to be its <see cref="SplashControl.SplashImage"/>
            /// </remarks>
            public DefaultPanel(Image backImage)
                : this()
            {
                this.BackgroundImage = backImage;
            }

            /// <summary>
            /// Gets / sets the BackImage with an image.
            /// </summary>
            /// <remarks>
            /// The image to be used as the background image.
            /// The size of the SplashPanel is set to the size of BackImage.
            /// </remarks>
            [
            Browsable(true),
            Category("Appearance")
            ]
            public override Image BackgroundImage
            {
                get
                {
                    return base.BackgroundImage;
                }

                set
                {
                    base.BackgroundImage = value;
                    if (value != null)
                        this.Size = value.Size;
                }
            }

            /// <summary>
            /// Cleans up any resources being used.
            /// </summary>
            /// <param name="disposing">bool disposing</param>
            [Syncfusion.Documentation.DocumentationExclude()]
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                base.Dispose(disposing);
            }

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                this.SuspendLayout();
                
                // SplashControlDefaultPanel
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.Size = new System.Drawing.Size(488, 277);
                this.ResumeLayout(false);
            }
        }
        #endregion
    }
}
