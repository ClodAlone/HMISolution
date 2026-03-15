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
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools
{
    public enum Theme
    {
        Default,
        Metro
    }
    /// <summary>
    /// The WizardControl is a control derived class that enables the creation of wizard applications. 
    /// The manipulation of the WizardPages is easy through the designer verbs that are available to the user and it`s properties.
    /// </summary>
    [
        Designer(typeof(WizardControlDesigner)),
        ToolboxItem(true),
        ToolboxBitmap(typeof(WizardControl), "ToolboxIcons.WizardControl.bmp"),
        Description("Enables the creation of wizard applications.")
    ]
    public class WizardControl :
        Wizard
    {
        #region Variables

        private const int DEF_MIN_BORDER_OFFSET = 6;
        private const int DEF_INSETS = DEF_MIN_BORDER_OFFSET - 1;
        private const int DEF_DESCRIPTION_LABEL_OFFSET = 5;
        private System.ComponentModel.IContainer components;
        private Syncfusion.Windows.Forms.Tools.GridBagLayout gridBagLayout;
        private Syncfusion.Windows.Forms.Tools.WizardButton nextButton;
        private Syncfusion.Windows.Forms.Tools.WizardButton backButton;
        private Syncfusion.Windows.Forms.Tools.WizardButton finishButton;
        private Syncfusion.Windows.Forms.Tools.WizardButton cancelButton;
        private Syncfusion.Windows.Forms.Tools.WizardButton helpButton;
        private System.Windows.Forms.Panel spacerPanel;
        private GradientPanel bannerPanel = null;
        private PictureBox banner = null;
        private Label title = null;
        private Label description = null;
        private System.Windows.Forms.Panel lineSeparator;
        private Label m_labelStopper = null;
        private bool m_bButtonsMirrored = false;
        private bool m_bCausesValidation = true;

        /// <summary>
        /// Auto layout banner in banner control.
        /// </summary>
        private bool m_bAutoLayoutBanner = true;

        /// <summary>
        /// Auto layout title in banner control.
        /// </summary>
        private bool m_bAutoLayoutTitle = true;

        /// <summary>
        /// Auto layout title in description control.
        /// </summary>
        private bool m_bAutoLayoutDescription = true;

        /// <summary>
        /// Occurs before Help button is clicked
        /// </summary>
        [Description("This event is fired when the Help button is pressed.")]
        [Category("Wizard Events")]
        public event System.EventHandler Help;

        /// <summary>
        /// Occurs before Finish button is clicked
        /// </summary>
        [Description("This event is fired when the Finish button is pressed.")]
        [Category("Wizard Events")]
        public event System.EventHandler Finish;

        /// <summary>
        /// Occurs before Cancel button is clicked
        /// </summary>
        [Description("This event is fired when the Cancel button is pressed.")]
        [Category("Wizard Events")]
        public event System.EventHandler Cancel;

        #endregion
        #region Event Methods

        /// <summary>
        /// Raises the Help event
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnHelp(EventArgs e)
        {
            if (Help != null)
            {
                try
                { 
                    Help(this, e);
                }
                catch
                { 
                }
            }
        }

        /// <summary>
        /// Raises the Finish event
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnFinish(EventArgs e)
        {
            if (Finish != null)
            {
                Finish(this, e);
            }
        }

        /// <summary>
        /// Raises the Cancel event
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnCancel(EventArgs e)
        {
            if (Cancel != null)
            {
                try 
                { 
                    Cancel(this, e);
                }
                catch 
                { 
                }
            }
        }

        #endregion
        #region Properties
        private Theme metro = Theme.Default;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Theme Style
        {
            get
            {
               
                return metro;
            }
            set
            {
                metro = value;

                if (metro == Theme.Metro)
                {
                    helpButton.FlatStyle = FlatStyle.Standard;
                    helpButton.UseVisualStyle = true;                   
                    helpButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
                    finishButton.FlatStyle = FlatStyle.Standard;
                    finishButton.UseVisualStyle = true;
                    finishButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
                    nextButton.FlatStyle = FlatStyle.Standard;
                    nextButton.UseVisualStyle = true;
                    nextButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
                    backButton.FlatStyle = FlatStyle.Standard;
                    backButton.UseVisualStyle = true;                  
                    backButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
                    cancelButton.FlatStyle = FlatStyle.Standard;
                    cancelButton.UseVisualStyle = true;                   
                    cancelButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
                    if (bannerPanel != null)
                    {
                        bannerPanel.BorderStyle = BorderStyle.FixedSingle;
                    }
                    if(banner!=null)
                    {
                        banner.BorderStyle = BorderStyle.FixedSingle;
                    }
                    lineSeparator.BorderStyle = BorderStyle.FixedSingle;
                    if(PageContainer!=null)
                   PageContainer.BorderStyle = BorderStyle.FixedSingle;
                   this.BorderStyle = BorderStyle.FixedSingle;
                  
                }
                else
                {
                    helpButton.UseVisualStyle = false;
                    helpButton.FlatStyle = FlatStyle.System;
                    helpButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
                    finishButton.UseVisualStyle = false;
                    finishButton.FlatStyle = FlatStyle.System;
                    finishButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
                    backButton.UseVisualStyle = false;
                    backButton.FlatStyle = FlatStyle.System;
                    backButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
                    nextButton.UseVisualStyle = false;
                    nextButton.FlatStyle = FlatStyle.System;
                    nextButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
                    cancelButton.UseVisualStyle = false;
                    cancelButton.FlatStyle = FlatStyle.System;
                    cancelButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
                    if (banner != null)
                        banner.BorderStyle = BorderStyle.Fixed3D;
                    lineSeparator.BorderStyle = BorderStyle.None;
                    if (PageContainer != null)
                        PageContainer.BorderStyle = BorderStyle.None;
                    this.BorderStyle = BorderStyle.None;
             
                }
            }
        }

       #region Buttons
        /// <summary>
        /// Gets the Help button of the WizardControl.
        /// </summary>
        [Description("The Help button of the WizardControl.")]
        [Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonAdv HelpButton
        {
            get { return helpButton; }
        }

        /// <summary>
        /// Gets the Finish button of the WizardControl.
        /// </summary>
        [Description("Gets the Finish button of the WizardControl.")]
        [Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonAdv FinishButton
        {
            get { return finishButton; }
        }

        /// <summary>
        /// Gets the Next button of the WizardControl.
        /// </summary>
        [Description("Gets the Next button of the WizardControl.")]
        [Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonAdv NextButton
        {
            get { return nextButton; }
        }

        /// <summary>
        /// Gets the Back button of the WizardControl.
        /// </summary>
        [Description("Gets the Back button of the WizardControl.")]
        [Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonAdv BackButton
        {
            get { return backButton; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ValidatePage event being fired.
        /// </summary>
        [
        DefaultValue(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Buttons"),
        Description("Enables/Disables ValidatePage event being fired.")
        ]
        public bool BackButtonCausesValidation
        {
            get
            {
                return m_bCausesValidation;
            }
            set
            {
                m_bCausesValidation = value;
            }
        }

        /// <summary>
        /// Gets the Cancel button of the WizardControl.
        /// </summary>
        [Description("Gets the Cancel button of the WizardControl.")]
        [Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonAdv CancelButton
        {
            get { return cancelButton; }
        }
        #endregion

        #region Banner Controls

        /// <summary>
        /// Gets or sets a value indicating whether auto layout banner in banner control.
        /// </summary>
        [Description("Gets or sets auto layout banner in banner control.")]
        [Category("BannerControls")]
        [DefaultValue(true)]
        public bool AutoLayoutBanner
        {
            get
            {
                return m_bAutoLayoutBanner;
            }
            set
            {
                if (m_bAutoLayoutBanner != value)
                {
                    m_bAutoLayoutBanner = value;

                    if (value)
                    {
                        this.RepositionBannerControls();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto layout title in banner control.
        /// </summary>
        [Description("Gets or sets auto layout title in banner control.")]
        [Category("BannerControls")]
        [DefaultValue(true)]
        public bool AutoLayoutTitle
        {
            get
            {
                return m_bAutoLayoutTitle;
            }
            set
            {
                if (m_bAutoLayoutTitle != value)
                {
                    m_bAutoLayoutTitle = value;

                    if (value)
                    {
                        this.RepositionBannerControls();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto layout title in description control.
        /// </summary>
        [Description("Gets or sets auto layout title in description control.")]
        [Category("BannerControls")]
        [DefaultValue(true)]
        public bool AutoLayoutDescription
        {
            get
            {
                return m_bAutoLayoutDescription;
            }
            set
            {
                if (m_bAutoLayoutDescription != value)
                {
                    m_bAutoLayoutDescription = value;

                    if (value)
                    {
                        this.RepositionBannerControls();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the label that shows the description of the current page.
        /// </summary>
        /// <remarks>
        /// <p>The specified Label, if not a child of the BannerPanel will then be made a
        /// child of the BannerPanel.</p>
        /// </remarks>
        [Description(@"Gets/Sets label that shows the description of the current page.")]
        [Category("BannerControls")]
        public Label Description
        {
            get 
            {
                return description; 
            }
            set
            {
                if (description != value)
                {
                    if (value != null && value.Parent != this.BannerPanel
                        && this.BannerPanel != null)
                    {
                        value.Parent = this.BannerPanel;
                    }
                    description = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the label that shows the title of the current page.
        /// </summary>
        /// <remarks>
        /// <p>The specified Label, if not a child of the BannerPanel will then be made a
        /// child of the BannerPanel.</p>
        /// </remarks>
        [Description(@"Gets/Sets label that shows the title of the current page.")]
        [Category("BannerControls")]
        public Label Title
        {
            get
            { 
                return title; 
            }
            set
            {
                if (title != value)
                {
                    if (value != null && value.Parent != this.BannerPanel
                        && this.BannerPanel != null)
                    {
                        value.Parent = this.BannerPanel;
                    }
                    title = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the array that holds the pages.
        /// </summary>
        [Description("The array that holds the pages.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        public new WizardControlPage[] WizardPages
        {
            get
            {
                if (PageContainer == null)
                {
                    WizardControlPage[] pages = new WizardControlPage[] { };
                    Array.Copy(wizardPgs, pages, wizardPgs.Length);

                    return pages;
                }
                else
                {
                    ArrayList list = new ArrayList();
                    for (int i = 0; i < PageContainer.Controls.Count; i++)
                    {
                        if (PageContainer.Controls[i] is WizardControlPage)
                            list.Add(this.PageContainer.Controls[i]);
                    }
                    return list.ToArray(typeof(WizardControlPage)) as WizardControlPage[];
                }
            }
            set
            {
                wizardPgs = value;
                if (PageContainer == null) return;
                this.PageContainer.Controls.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    this.PageContainer.Controls.Add(value[i]);
                    value[i].SettingsChanged += new EventHandler(this.PageSettingsChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the PictureBox that shows the Banner Image of the Wizard.
        /// </summary>
        [Description(@"Gets/Sets PictureBox that shows the Banner Image of the Wizard.")]
        [Category("BannerControls")]

        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PictureBox Banner
        {
            get
            {
                return banner;
            }
            set
            {
                if (banner != value)
                {
                    if (value != null && value.Parent != this.BannerPanel)
                    {
                        throw new ArgumentException("Specified Banner control should be a child of the BannerPanel on top of the WizardControl");
                    }
                    banner = value;
                }
            }
        }

        #endregion

        #region Panels
        /// <summary>
        /// Gets or sets the Banner Panel of the WizardControl.
        /// </summary>
        [Description("Gets/Sets the Banner Panel of the WizardControl")]
        [Category("Panels")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public GradientPanel BannerPanel
        {
            get 
            { 
                return bannerPanel; 
            }
            set
            {
                if (bannerPanel != value)
                {
                    if (bannerPanel != null)
                    {
                        bannerPanel.ControlRemoved -= new ControlEventHandler(this.BannerPanel_ControlRemoved);
                    }

                    bannerPanel = value;

                    bannerPanel.ControlRemoved += new ControlEventHandler(this.BannerPanel_ControlRemoved);
                }
            }
        }

        /// <summary>
        /// Gets or sets the PageContainer of the WizardControl.
        /// </summary>
        [Description("Gets/Sets the PageContainer of the WizardControl.")]
        [Category("Panels")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public WizardContainer WizardPageContainer
        {
            get 
            { 
                return PageContainer;
            }
            set
            {
                this.SetPageContainer(value);
                if (value == null) return;

                SetPageContainerConstraints();
            }
        }
        #endregion

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Tools.GridBagLayout"/> used to layout the different
        /// controls in the WizardControl.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The GridBagLayout used to layout the different regions of the wizard.")]
        [Browsable(false)]
        public GridBagLayout GridBagLayout
        {
            get { return gridBagLayout; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WizardPage SelectedPage
        {
            get { return base.SelectedPage; }
            set { base.SelectedPage = value; }
        }

        /// <summary>
        /// Gets or sets the selected wizard control page.
        /// </summary>
        [Description("Specifies the selected wizard control page.")]
        [Category("Appearance")]
        public WizardControlPage SelectedWizardPage
        {
            get 
            { 
                return (WizardControlPage)base.SelectedPage;
            }
            set
            { 
                base.SelectedPage = value;
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Initializes the dynamic controls used in the wizard.
        /// </summary>
        protected override void OnEndInit()
        {
            Initialize();

            if (!this.DesignMode)
            {
                if (this.PageContainer != null)
                {
                    for (int i = 0; i < this.PageContainer.Controls.Count; i++)
                    {
                        WizardControlPage page = this.PageContainer.Controls[i] as WizardControlPage;

                        if (page != null)
                        {
                            this.CardLayout.SetCardName(page, page.LayoutName);
                        }
                    }
                }

                this.CardLayout.First();
                this.CardLayout.Next();
                this.CardLayout.Previous();
            }

            if (GetIsMirrored())
            {
                RepositionNavigationButtons(true);
            }
        }

        public WizardControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(WizardControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            WizardButton button = null;

            foreach (Control control in this.Controls)
            {
                button = control as WizardButton;
                if (button != null)
                {
                    button.VisibleChanged += new EventHandler(Button_VisibleChanged);
                }
            }

            this.CancelButton.TabIndex = 1;
            this.BackButton.TabIndex = 2;
            this.NextButton.TabIndex = 3;
            this.FinishButton.TabIndex = 4;
            this.HelpButton.TabIndex = 5;
            this.cancelButton.m_Parent = this.backButton.m_Parent = this.nextButton.m_Parent = this.finishButton.m_Parent =
            this.helpButton.m_Parent = this;
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
            this.components = new System.ComponentModel.Container();
            this.nextButton = new Syncfusion.Windows.Forms.Tools.WizardButton();
            this.backButton = new Syncfusion.Windows.Forms.Tools.WizardButton();
            this.finishButton = new Syncfusion.Windows.Forms.Tools.WizardButton();
            this.cancelButton = new Syncfusion.Windows.Forms.Tools.WizardButton();
            this.gridBagLayout = new Syncfusion.Windows.Forms.Tools.GridBagLayout(this.components);
            this.helpButton = new Syncfusion.Windows.Forms.Tools.WizardButton();
            this.spacerPanel = new System.Windows.Forms.Panel();
            this.lineSeparator = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.gridBagLayout)).BeginInit();
            this.SuspendLayout();
            // 
            // nextButton
            // 
            this.nextButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.gridBagLayout.SetConstraints(this.nextButton,
                new GridBagConstraints(3, 5, 1, 1, 0, 0, AnchorTypes.East, FillType.None,
                    new Insets(0, 5, 0, 5), 0, 0, false));
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nextButton.Location = new System.Drawing.Point(408, 238);
            this.nextButton.Name = "nextButton";
            this.gridBagLayout.SetPreferredSize(this.nextButton, new System.Drawing.Size(75, 23));
            this.nextButton.Text = "Next >>";
            this.nextButton.Click += new System.EventHandler(this.NextButton_Click);
            this.nextButton.SizeChanged += new System.EventHandler(this.Button_SizeChanged);
            // 
            // backButton
            // 
            this.backButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.gridBagLayout.SetConstraints(this.backButton,
                new GridBagConstraints(2, 5, 1, 1, 0, 0, AnchorTypes.East, FillType.None,
                    new Insets(5, 5, 0, 5), 0, 0, false));
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.backButton.Location = new System.Drawing.Point(333, 238);
            this.backButton.Name = "backButton";
            this.gridBagLayout.SetPreferredSize(this.backButton, new System.Drawing.Size(75, 23));
            this.backButton.Text = "<< Back";
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            this.backButton.SizeChanged += new System.EventHandler(this.Button_SizeChanged);
            // 
            // finishButton
            // 
            this.finishButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.gridBagLayout.SetConstraints(this.finishButton,
                new GridBagConstraints(4, 5, 1, 1, 0, 0, AnchorTypes.East, FillType.None,
                    new Insets(5, 5, 0, 5), 0, 0, false));
            this.finishButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.finishButton.Location = new System.Drawing.Point(488, 238);
            this.finishButton.Name = "finishButton";
            this.gridBagLayout.SetPreferredSize(this.finishButton, new System.Drawing.Size(75, 23));
            this.finishButton.Text = "Finish";
            this.finishButton.Click += new System.EventHandler(this.FinishButton_Click);
            this.finishButton.SizeChanged += new System.EventHandler(this.Button_SizeChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.gridBagLayout.SetConstraints(this.cancelButton,
                new GridBagConstraints(1, 5, 1, 1, 0, 0, AnchorTypes.East, FillType.None,
                    new Insets(5, 5, 0, 5), 0, 0, false));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(253, 238);
            this.cancelButton.Name = "cancelButton";
            this.gridBagLayout.SetPreferredSize(this.cancelButton, new System.Drawing.Size(75, 23));
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            this.cancelButton.SizeChanged += new System.EventHandler(this.Button_SizeChanged);
            // 
            // gridBagLayout
            // 
            this.gridBagLayout.ContainerControl = this;
            // 
            // helpButton
            // 
            this.gridBagLayout.SetConstraints(this.helpButton,
                new GridBagConstraints(5, 5, 1, 1, 0, 0, AnchorTypes.Center, FillType.None,
                    new Insets(5, 0, 5, 0), 0, 0, false));
            this.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.helpButton.Location = new System.Drawing.Point(568, 238);
            this.helpButton.Name = "helpButton";
            this.helpButton.Text = "Help";
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            this.helpButton.SizeChanged += new System.EventHandler(this.Button_SizeChanged);
            // 
            // spacerPanel
            // 
            this.gridBagLayout.SetConstraints(this.spacerPanel,
                new GridBagConstraints(0, 4, 1, 1, 20, 0, AnchorTypes.Center, FillType.None,
                    new Tools.Insets(0, 0, 0, 0), 0, 0, false));
            this.spacerPanel.Name = "spacerPanel";
            this.gridBagLayout.SetPreferredSize(this.spacerPanel, new System.Drawing.Size(0, 0));
            this.spacerPanel.Size = new System.Drawing.Size(0, 0);
            this.spacerPanel.TabIndex = 0;
            // 
            // lineSeparator
            // 
            this.gridBagLayout.SetConstraints(this.lineSeparator,
                new GridBagConstraints(0, 4, 10, 1, 1, 0, AnchorTypes.Center, FillType.Horizontal,
                    new Insets(0, 0, 0, 0), 0, 0, false));
            this.lineSeparator.Location = new System.Drawing.Point(0, 230);
            this.gridBagLayout.SetMinimumSize(this.lineSeparator, new System.Drawing.Size(269, 3));
            this.lineSeparator.Name = "lineSeparator";
            this.gridBagLayout.SetPreferredSize(this.lineSeparator, new System.Drawing.Size(269, 3));
            this.lineSeparator.Size = new System.Drawing.Size(648, 3);
            this.lineSeparator.TabIndex = 10;
            this.lineSeparator.Paint += new System.Windows.Forms.PaintEventHandler(this.LineSeparator_Paint);
            // 
            // WizardControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lineSeparator,
																		  this.helpButton,
																		  this.cancelButton,
																		  this.finishButton,
																		  this.backButton,
																		  this.nextButton,
																		  this.spacerPanel});
            this.Name = "WizardControl";
            this.Size = new System.Drawing.Size(648, 496);
            ((System.ComponentModel.ISupportInitialize)(this.gridBagLayout)).EndInit();
            this.ResumeLayout(false);

        }

        internal void Initialize()
        {
            this.PageContainer.BringToFront();
            this.PageContainer.Size = new System.Drawing.Size(688, 482);
            this.PageContainer.TabIndex = 1;
            this.PageContainer.Controls.AddRange(this.WizardPages);

            this.CardLayout.ContainerControl = this.PageContainer;

            SetPageContainerConstraints();

            // Create the default banner, title and description only when the control the dragged and dropped the very first time.
            // Loading will be true when opening the design-time when we will not insert our default controls.
            bannerPanel.Height = 70;
            bannerPanel.Dock = DockStyle.Top;
            GridBagConstraints cons = this.GridBagLayout.GetConstraintsRef(bannerPanel);
            cons.GridPosX = 0;
            cons.GridPosY = 0;
            cons.WeightX = 10;
            cons.CellSpanX = 10;
            cons.Fill = FillType.Both;

            if (banner != null)
            {
                banner.BorderStyle = BorderStyle.FixedSingle;
                banner.Size = new Size(61, 61);
            }

            if (title != null)
            {
                title.Text = "This is the title of the Wizard Page.";
                title.Font = Syncfusion.Drawing.FontUtil.CreateFont(title.Font, FontStyle.Bold);
                title.Size = new Size(200, 15);
            }

            if (description != null)
            {
                description.Text = "This is the description of the Wizard Page.";
                description.Size = new Size(210, 15);
            }
        }

        #endregion

        private void BannerPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control == this.Title)
                this.Title = null;
            else if (e.Control == this.Banner)
                this.Banner = null;
            else if (e.Control == this.Description)
                this.Description = null;
        }
        private void NextButton_Click(object sender, System.EventArgs e)
        {
            if (SelectedWizardPage != null)
            {
                SelectedWizardPage.RaiseNextClick();
            }
            this.NextPage();
        }

        private void BackButton_Click(object sender, System.EventArgs e)
        {
            if (SelectedWizardPage != null)
            {
                SelectedWizardPage.RaiseBackClick();
            }
            this.PreviousPage();
        }

        private void FinishButton_Click(object sender, System.EventArgs e)
        {
            if (SelectedWizardPage != null)
            {
                SelectedWizardPage.RaiseFinishClick();
            }
            OnFinish(EventArgs.Empty);
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            if (SelectedWizardPage != null)
            {
                SelectedWizardPage.RaiseCancelClick();
            }
            OnCancel(EventArgs.Empty);
        }
        private void HelpButton_Click(object sender, System.EventArgs e)
        {
            if (SelectedWizardPage != null)
            {
                SelectedWizardPage.RaiseHelpClick();
            }
            OnHelp(EventArgs.Empty);
        }

        private void CalculateDescriptionSize()
        {
            if (description != null)
            {
                description.AutoSize = true;

                int maxWidth = this.Width - description.Location.X - DEF_DESCRIPTION_LABEL_OFFSET;
                int maxHeight = description.Height;

                Size prefSize = description.Size;
                description.AutoSize = false;

                description.Width = (prefSize.Width > maxWidth) ? maxWidth : prefSize.Width;
                description.Height = (prefSize.Height > maxHeight) ? maxHeight : prefSize.Height;
            }
        }
        protected override void RefreshAppearance()
        {
            WizardControlPage page = CardLayout.GetComponentFromName(CardLayout.SelectedCard) as WizardControlPage;
            if (page == null)
            {
                // MessageBox.Show("Page is null");
                return;
            }

            helpButton.Visible = page.HelpVisible;
            helpButton.Enabled = page.HelpEnabled;

            finishButton.Visible = page.FinishVisible;
            finishButton.Enabled = page.FinishEnabled;

            nextButton.Visible = page.NextVisible;
            nextButton.Enabled = page.NextEnabled;

            backButton.Visible = page.BackVisible;
            backButton.Enabled = page.BackEnabled;

            cancelButton.Visible = page.CancelVisible;
            cancelButton.Enabled = page.CancelEnabled;

            bannerPanel.Visible = !page.FullPage;

            if (title != null)
            {
                title.Text = page.Title;
                if (!title.AutoSize)
                {
                    using (Graphics g = title.CreateGraphics())
                    {
                        title.Width = g.MeasureString(title.Text, title.Font).ToSize().Width + 5;
                    }
                }
            }
            if (description != null)
            {
                description.Text = page.Description;
                CalculateDescriptionSize();
            }
            RepositionBannerControls();

            bool bIsMirrored = GetIsMirrored();
            GridBagConstraints cons = gridBagLayout.GetConstraints(cancelButton);
            if (page.CancelOverFinish)
            {
                cons.GridPosX = bIsMirrored ? 1 : 4;
                this.finishButton.Visible = false;
                ((Control)this.cancelButton).TabIndex = 4;
            }
            else
            {
                cons.GridPosX = bIsMirrored ? 4 : 1;
                this.finishButton.Visible = true;
                ((Control)this.cancelButton).TabIndex = 1;
            }
            gridBagLayout.SetConstraints(cancelButton, cons);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            CalculateDescriptionSize();
        }

        private void LineSeparator_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(SystemColors.ControlDark), new Point(0, 0), new Point(lineSeparator.Width, 0));
            e.Graphics.DrawLine(new Pen(SystemColors.ControlLight), new Point(0, 1), new Point(lineSeparator.Width, 1));
            e.Graphics.DrawLine(new Pen(SystemColors.ControlLight), new Point(0, 2), new Point(lineSeparator.Width, 2));
        }

        private void Button_SizeChanged(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                this.gridBagLayout.SetPreferredSize(sender as Control, ((Control)sender).Size);
            }
        }
        internal bool CanPerformClick(Button btn)
        {
            CancelEventArgs e = new CancelEventArgs(false);
            if (btn == this.nextButton || btn == this.finishButton || btn == this.backButton || btn == this.cancelButton)
            {
                if (this.SelectedPage != null)
                {
                    if (btn == this.nextButton || btn == this.finishButton || (btn == this.backButton && this.BackButtonCausesValidation))
                        this.SelectedWizardPage.RaiseValidatePage(e);
                    if (e.Cancel == true)
                        return !e.Cancel;
                }
                if (btn == this.nextButton)
                    this.OnBeforeNext(e);
                else if (btn == this.backButton)
                    this.OnBeforeBack(e);
                else if (btn == this.finishButton)
                    this.OnBeforeFinish(e);
                else if (btn == this.cancelButton)
                    this.OnBeforeCancel(e);
            }
            return !e.Cancel;
        }

        /// <summary>
        /// Occurs before the Next button is clicked.
        /// </summary>
        /// <remarks>You can validate the current <see cref="SelectedPage"/> and cancel the next button click, if necessary.</remarks>
        [Category("Wizard Events")]
        [Description("Occurs before the Next button is clicked.")]
        public event CancelEventHandler BeforeNext;

        /// <summary>
        /// Occurs before the Back button is clicked.
        /// </summary>
        /// <remarks>You can validate the current <see cref="SelectedPage"/> and cancel the back button click, if necessary.</remarks>
        [Category("Wizard Events"), Description("Occurs before the Back button is clicked.")]
        public event CancelEventHandler BeforeBack;

        /// <summary>
        /// Occurs before the Finish button is clicked.
        /// </summary>
        /// <remarks>You can validate the current <see cref="SelectedPage"/> and cancel the finish button click, if necessary.</remarks>
        [Category("Wizard Events"), Description("Occurs before the Finish button is clicked.")]
        public event CancelEventHandler BeforeFinish;

        /// <summary>
        /// Occurs before the Cancel button is clicked.
        /// </summary>
        /// <remarks>You can cancel the cancel button click if necessary.</remarks>
        [Category("Wizard Events"), Description("Occurs before the Finish button is clicked.")]
        public event CancelEventHandler BeforeCancel;

        /// <summary>
        /// Raises the BeforeNext event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that lets you cancel the action.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBeforeNext method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBeforeNext 
        /// in a derived class, be sure to call the base class's 
        /// OnBeforeNext method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBeforeNext(CancelEventArgs e)
        {
            if (this.BeforeNext != null)
                this.BeforeNext(this, e);
        }

        /// <summary>
        /// Raises the BeforeBack event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that lets you cancel the action.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBeforeBack method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBeforeBack 
        /// in a derived class, be sure to call the base class's 
        /// OnBeforeBack method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBeforeBack(CancelEventArgs e)
        {
            if (this.BeforeBack != null)
                this.BeforeBack(this, e);
        }

        /// <summary>
        /// Raises the BeforeCancel event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that lets you cancel the action.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The BeforeCancel method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding BeforeCancel 
        /// in a derived class, be sure to call the base class's 
        /// BeforeCancel method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        private void OnBeforeCancel(CancelEventArgs e)
        {
            if (this.BeforeCancel != null)
                this.BeforeCancel(this, e);
        }
        /// <summary>
        /// Raises the BeforeFinish event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that lets you cancel the action.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBeforeFinish method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBeforeFinish 
        /// in a derived class, be sure to call the base class's 
        /// OnBeforeFinish method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBeforeFinish(CancelEventArgs e)
        {
            if (this.BeforeFinish != null)
                this.BeforeFinish(this, e);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SetPageContainerConstraints()
        {
            this.gridBagLayout.SetConstraints(this.PageContainer, new GridBagConstraints(0, 2, 10, 1, 1, 1, AnchorTypes.Center, FillType.Both, new Insets(0, 0, 0, 5), 0, 0, false));
            this.gridBagLayout.SetMinimumSize(this.PageContainer, new System.Drawing.Size(560, 248));
            this.gridBagLayout.SetPreferredSize(this.PageContainer, new System.Drawing.Size(560, 400));
        }

        /// <summary>
        /// Positions the Navigation buttons.
        /// </summary>
        /// <param name="bIsMirrored">bool Mirrored</param>
        protected void RepositionNavigationButtons(bool bIsMirrored)
        {
            if (m_bButtonsMirrored == bIsMirrored)
                return;

            WizardButton[] awbButtons = new WizardButton[]
            {
                nextButton,
                backButton,
                finishButton,
                cancelButton,
                helpButton
            };

            int nRiteMostPos = -1;

            for (int nBtn = 0; nBtn < awbButtons.Length; ++nBtn)
            {
                WizardButton wbCurBtn = awbButtons[nBtn];
                GridBagConstraints gbcCurBtnConstraints = gridBagLayout.GetConstraintsRef(wbCurBtn);

                gbcCurBtnConstraints.GridPosX = 5 - gbcCurBtnConstraints.GridPosX;

                if (gbcCurBtnConstraints.GridPosX > nRiteMostPos)
                {
                    nRiteMostPos = gbcCurBtnConstraints.GridPosX;
                }

                // Mirror Insets
                Insets insNew = gbcCurBtnConstraints.Insets;
                insNew.Left = gbcCurBtnConstraints.Insets.Right;
                insNew.Right = gbcCurBtnConstraints.Insets.Left;

                gbcCurBtnConstraints.Insets = insNew;

                // Mirror Anchor
                AnchorTypes atAnchor = gbcCurBtnConstraints.Anchor;
                switch (atAnchor)
                {
                    case AnchorTypes.East: atAnchor = AnchorTypes.West;
                        break;
                    case AnchorTypes.West: atAnchor = AnchorTypes.East;
                        break;
                }

                gbcCurBtnConstraints.Anchor = atAnchor;
            }

            if (bIsMirrored)
            {
                if (null == m_labelStopper)
                {
                    m_labelStopper = new Label();
                    m_labelStopper.BackColor = Color.Transparent;
                    m_labelStopper.Visible = true;

                    Controls.Add(m_labelStopper);
                }

                if (nRiteMostPos > 0)
                {
                    this.gridBagLayout.SetConstraints(m_labelStopper, new GridBagConstraints(nRiteMostPos, 5, 5, 1, 10000, 0, AnchorTypes.West, FillType.Horizontal, new Insets(0, 0, 0, 0), 0, 0, false));
                }
            }
            else
            {
                if (null != m_labelStopper)
                {
                    Controls.Remove(m_labelStopper);
                    m_labelStopper = null;
                }
            }

            m_bButtonsMirrored = bIsMirrored;
        }

        protected void RepositionBannerControls()
        {
            RepositionBannerControls(GetIsMirrored());
        }

        /// <summary>
        /// Occurs when Banner Panel controls are laid out. 
        /// </summary>
        [Description("This event is fired when a banner control is about to be relocated by wizard control.")]
        [Category("BannerControls")]
        public event CancelEventHandler BannerControlLocationChanging;

        /// <summary>
        /// Raises the BannerControlLocationChanging event
        /// </summary>
        /// <param name="banerControl">Banner control</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void OnBannerControlLocationChanging(Control banerControl, CancelEventArgs e)
        {
            if (banerControl == null)
                throw new ArgumentNullException("banerControl");
            if (e == null)
                throw new ArgumentNullException("e");

            if (BannerControlLocationChanging != null)
            {
                BannerControlLocationChanging(banerControl, e);
            }
        }

        protected void RepositionBannerControls(bool bIsMirrored)
        {
            CancelEventArgs e = new CancelEventArgs();

            if (null != banner && this.AutoLayoutBanner)
            {
                OnBannerControlLocationChanging(banner, e);

                if (!e.Cancel)
                {
                    int nBannerOffsetX = 10;
                    int nBannerX = bIsMirrored ? nBannerOffsetX : bannerPanel.Width - banner.Width - nBannerOffsetX;
                    banner.Location = new Point(nBannerX, (bannerPanel.Height - banner.Height) / 2);
                    banner.Anchor = AnchorStyles.Bottom | (bIsMirrored ? AnchorStyles.Left : AnchorStyles.Right);
                }
            }

            e.Cancel = false;

            if (null != title && this.AutoLayoutTitle)
            {
                OnBannerControlLocationChanging(title, e);

                if (!e.Cancel)
                {
                    int nTitleOffsetX = 10;
                    int nTitleX = bIsMirrored ? bannerPanel.Width - title.Width - nTitleOffsetX : nTitleOffsetX;
                    title.Location = new Point(nTitleX, 10);
                    title.Anchor = bIsMirrored ? AnchorStyles.Right : AnchorStyles.Left;
                }
            }

            e.Cancel = false;

            if (null != description && this.AutoLayoutDescription)
            {
                OnBannerControlLocationChanging(description, e);

                if (!e.Cancel)
                {
                    int nDescriptionOffsetX = 20;
                    int nDescriptionX = bIsMirrored ? bannerPanel.Width - description.Width - nDescriptionOffsetX : nDescriptionOffsetX;
                    description.Location = new Point(nDescriptionX, 30);
                    description.Anchor = bIsMirrored ? AnchorStyles.Right : AnchorStyles.Left;
                }
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            bool bIsMirrored = GetIsMirrored();
            RepositionNavigationButtons(bIsMirrored);
            RepositionBannerControls(bIsMirrored);
        }

        #endregion

        private void Button_VisibleChanged(object sender, EventArgs e)
        {
            WizardButton button = sender as WizardButton;

            if (button != null)
            {
                GridBagConstraints constraints = this.GridBagLayout.GetConstraints(button);

                if (constraints != null)
                {
                    Insets insets = constraints.Insets;

                    if (this.Width - button.Right < DEF_MIN_BORDER_OFFSET)
                    {
                        insets.Right = DEF_INSETS;
                    }

                    if (button.Left < DEF_MIN_BORDER_OFFSET)
                    {
                        insets.Left = DEF_INSETS;
                    }

                    constraints.Insets = insets;
                    this.GridBagLayout.SetConstraints(button, constraints);
                }
            }
        }
    }
    [ToolboxItem(false)]
    internal class WizardButton : ButtonAdv
    {
        internal WizardControl m_Parent;
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get 
            { 
                return base.TabIndex; 
            }
            set
            {
                // Prevent anyone else setting the tabindex.
                // base.TabIndex = value;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (m_Parent != null)
            {
                if (!this.m_Parent.CanPerformClick(this))
                    return;
            }
            base.OnClick(e);
        }

        /// <summary>
        /// Gets or sets the FlatStyle for the Wizard Button.
        /// </summary>
        [Browsable(true), System.ComponentModel.DefaultValue(FlatStyle.System)]
        public new FlatStyle FlatStyle
        {
            get
            { 
                return base.FlatStyle; 
            }
            set
            {
                base.FlatStyle = value;
            }
        }
    }
}
