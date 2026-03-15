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
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The base class for wizard-like controls.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.WizardDesigner))]
    public class Wizard : System.Windows.Forms.UserControl, ISupportInitialize
    {
        private Syncfusion.Windows.Forms.Tools.CardLayout cardLayout;
        private WizardContainer pageContainer = null;
        private IDesignerHost dh = null;

        public delegate void WizardPageSelectEventHandler(object sender, WizardPageSelectEventArgs e);

        /// <summary>
        /// This event is fired when the Next button is pressed or the NextPage method of the Wizard is called.
        /// </summary>
        [Description("This event is fired when the Next button is pressed or the NextPage method of the Wizard is called.")]
        [Category("Wizard Events")]
        public event System.EventHandler Next;

        /// <summary>
        /// This event is fired when the Back button is pressed or the PreviousPage method of the Wizard is called
        /// </summary>
        [Description("This event is fired when the Back button is pressed or the PreviousPage method of the Wizard is called.")]
        [Category("Wizard Events")]
        public event System.EventHandler Back;

        /// <summary>
        /// This event gets fired when selected page is about to change
        /// </summary>
        [Description("This event is fired when the selected page is about to change.")]
        [Category("Wizard Events")]
        public event WizardPageSelectEventHandler BeforePageSelect;

        /// <summary>
        /// This event gets fired when selected page get changed
        /// </summary>
        [Description("This event is fired when the selected page has changed.")]
        [Category("Wizard Events")]
        public event WizardPageSelectEventHandler AfterPageSelect;

        /// <summary>
        /// Called when AfterPageSelect event is fired
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnAfterPageSelect(WizardPageSelectEventArgs e)
        {
            if (AfterPageSelect != null)
            {
                try 
                { 
                    AfterPageSelect(this, e);
                }
                catch 
                {
                }
            }
            if (SelectedPage != null)
                SelectedPage.RaisePageLoad();
        }

        /// <summary>
        /// Called when OnBeforePageSelect event is fired
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnBeforePageSelect(WizardPageSelectEventArgs e)
        {
            if (BeforePageSelect != null)
            {
                try 
                { 
                    BeforePageSelect(this, e); 
                }
                catch 
                { 
                }
            }
        }

        /// <summary>
        /// Called when Next event is fired
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnNext(EventArgs e)
        {
            if (Next != null)
            {
                try 
                { 
                    Next(this, e);
                }
                catch 
                { 
                }
            }
        }

        /// <summary>
        /// Called when Back event is fired
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected void OnBack(EventArgs e)
        {
            if (Back != null)
            {
                try 
                { 
                    Back(this, e); 
                }
                catch 
                { 
                }
            }
        }

        /// <summary>
        /// Gets the Panel based class that holds the pages.
        /// </summary>
        [Description("The Panel based class that holds the pages.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WizardContainer PageContainer
        {
            get
            {
                return pageContainer;
            }

            // set{pageContainer = value;}
        }

        /// <summary>
        /// Gets the Designer Host.
        /// </summary>
        protected IDesignerHost DesignerHost
        {
            get
            {
                return this.dh;
            }
        }

        /// <summary>
        /// Sets the container for the pages.
        /// </summary>
        /// <param name="container">Wizard Container</param>
        public virtual void SetPageContainer(WizardContainer container)
        {
            if (this.pageContainer != container)
            {
                if (pageContainer != null)
                {
                    pageContainer.ControlAdded -= new ControlEventHandler(PageContainer_ControlAdded);
                    pageContainer.ControlRemoved -= new ControlEventHandler(PageContainer_ControlRemoved);
                }

                pageContainer = container;

                if (pageContainer != null)
                {
                    pageContainer.ControlAdded += new ControlEventHandler(PageContainer_ControlAdded);
                    pageContainer.ControlRemoved += new ControlEventHandler(PageContainer_ControlRemoved);
                }
            }
        }

        /// <summary>
        /// Gets the CardLayout component used to switch between the pages.
        /// </summary>
        [Description("The CardLayout component used to switch between the pages.")]
        [Category("Appearance")]
        public Syncfusion.Windows.Forms.Tools.CardLayout CardLayout
        {
            get { return cardLayout; }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected WizardPage[] wizardPgs = new WizardPage[0];

        /// <summary>
        /// Gets or sets the array that holds the pages.
        /// </summary>
        [Description("The array that holds the pages.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        public virtual WizardPage[] WizardPages
        {
            get
            {
                if (pageContainer == null) return wizardPgs;
                else
                {
                    ArrayList list = new ArrayList();
                    for (int i = 0; i < pageContainer.Controls.Count; i++)
                    {
                        if (pageContainer.Controls[i] is WizardPage)
                            list.Add(this.pageContainer.Controls[i]);
                    }
                    return list.ToArray(typeof(WizardPage)) as WizardPage[];
                }
            }
            set
            {
                wizardPgs = value;
                if (pageContainer == null) return;
                this.pageContainer.Controls.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    this.pageContainer.Controls.Add(value[i]);
                    value[i].SettingsChanged += new EventHandler(this.PageSettingsChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        [Description("The selected page.")]
        [Category("Apearance")]
        public virtual WizardPage SelectedPage
        {
            get
            {
                if (cardLayout.SelectedCard != null)
                {
                    return cardLayout.GetComponentFromName(cardLayout.SelectedCard) as WizardPage;
                }
                else
                    return null;
            }
            set
            {
                if (value == null) return;

                this.SetRedraw(false);

                if (Validate())
                {
                    WizardPageSelectEventArgs args = new WizardPageSelectEventArgs(value);
                    OnBeforePageSelect(args);

                    cardLayout.SelectedCard = cardLayout.GetCardName(args.Page);

                    OnAfterPageSelect(new WizardPageSelectEventArgs(SelectedPage));
                }
                RefreshAppearance();

                this.SetRedraw(true);
                this.Invalidate(true);
            }
        }

        void ISupportInitialize.BeginInit() 
        { 
        }
        void ISupportInitialize.EndInit()
        {
            EndInit();
        }
  
        /// <summary>
        /// Called by the designer when the control is created for the first time.
        /// </summary>
        public void EndInit()
        {
            this.InitDesignerHostRef();

            this.OnEndInit();

            if (!this.DesignMode)
            {
                RefreshAppearance();
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnEndInit()
        {
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void InitDesignerHostRef()
        {
            this.dh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
        }
        public Wizard()
        {
            // This call is required by the Windows.Forms Form Designer.
            cardLayout = new Syncfusion.Windows.Forms.Tools.CardLayout();
            cardLayout.BeginInit();
            cardLayout.LayoutMode = Syncfusion.Windows.Forms.Tools.CardLayoutMode.Fill;
            InitializeComponent();
            cardLayout.EndInit();

            // TODO: Add any initialization after the InitForm call
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new System.Drawing.Size(208, 416);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Adds the specified page to the wizard.
        /// </summary>
        /// <param name="page">The <see cref="WizardPage"/> to add.</param>
        public void AddPage(WizardPage page)
        {
            NativeMethods.LockWindowUpdate(this.Handle);
            pageContainer.Controls.Add(page);
            NativeMethods.LockWindowUpdate(IntPtr.Zero);
        }

        private void PageContainer_ControlRemoved(object sender, ControlEventArgs e)
        {
            WizardPage page = e.Control as WizardPage;
            if (page != null)
            {
                page.SettingsChanged -= new EventHandler(PageSettingsChanged);
            }
        }
        private void PageContainer_ControlAdded(object sender, ControlEventArgs e)
        {
            WizardPage page = e.Control as WizardPage;
            if (page != null)
            {
                // Update some settings in the wizard page:
                // Don't need Fill setting since this is managed by the CardLayout.
                // CardLayout in fact, doesn't like it.
                // page.Dock = DockStyle.Fill;
                PropertyDescriptor pd = TypeDescriptor.GetProperties(page).Find("DrawGrid", false);
                if (pd != null)
                {
                    pd.SetValue(page, true);
                }
                page.SettingsChanged += new EventHandler(PageSettingsChanged);

                if (cardLayout.GetCardName(page) == String.Empty)
                {
                    // This means the control has not been added to the CardLayout, yet.
                    // Go ahead and add it.
                    cardLayout.SetCardName(page, cardLayout.GetNewCardName());
                }
                cardLayout.Last();
                page.LayoutName = cardLayout.GetCardName(page);
                RefreshAppearance();
            }
        }

        protected void PageSettingsChanged(object sender, System.EventArgs e)
        {
            PageSettingsChanged();
        }

        protected virtual void PageSettingsChanged()
        {
            RefreshAppearance();
        }

        /// <summary>
        /// Removes the selectedPage.
        /// </summary>
        public void RemovePage()
        {
            if (this.SelectedPage == null)
                return;
            try
            {
                WizardPage selPage = this.SelectedPage;
                this.pageContainer.Controls.Remove(selPage);

                // WizardPages.Remove(SelectedPage);
                if (dh != null)
                    dh.DestroyComponent(selPage);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Removes the specified page.
        /// </summary>
        /// <param name="page">The <see cref="WizardPage"/> to remove.</param>
        public void RemovePage(WizardPage page)
        {
            try
            {
                this.pageContainer.Controls.Remove(page);

                // WizardPages.Remove(page);
                if (dh != null)
                    dh.DestroyComponent(page);
            }
            catch 
            {
            }
        }

        /// <summary>
        /// Selects the next page.
        /// </summary>
        public void NextPage()
        {
            if (SelectedPage != null)
            {
                if (DesignMode || SelectedPage.NextPage == null)
                {
                    int nextPageIndex = this.cardLayout.NextCardIndex;
                    WizardPage nextPage = null;
                    if (nextPageIndex != -1)
                    {
                        IList controls = this.cardLayout.GetControls();

                        nextPage = controls[nextPageIndex] as WizardPage;
                    }
                    this.SelectedPage = nextPage;
                   }
                else SelectedPage = SelectedPage.NextPage;
            }
            OnNext(EventArgs.Empty);
        }

        /// <summary>
        /// Selects the previous page.
        /// </summary>
        public void PreviousPage()
        {
            if (SelectedPage != null)
            {
                if (DesignMode || SelectedPage.PreviousPage == null)
                {
                    int previousPageIndex = this.cardLayout.PreviousCardIndex;
                    WizardPage previousPage = null;
                    if (previousPageIndex != -1)
                    {
                        IList controls = this.cardLayout.GetControls();
                        previousPage = controls[previousPageIndex] as WizardPage;
                    }
                    this.SelectedPage = previousPage;
                    }
                else
                    SelectedPage = SelectedPage.PreviousPage;
            }
            OnBack(EventArgs.Empty);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void RefreshAppearance()
        {
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool GetIsMirrored()
        {
            return RightToLeft.Yes == RightToLeft;
        }

        /// <summary>
        /// Allow changes in wizard to be redrawn or prevents changes from being redrawn based on parameter.
        /// </summary>
        /// <param name="redrawValue">Indicates whether to redraw changes.</param>
        private void SetRedraw(bool redrawValue)
        {
            if (this.IsHandleCreated)
            {
                NativeMethodsHelper.SetRedrawWindow(this.Handle, redrawValue, false);
            }
        }
    }
}
