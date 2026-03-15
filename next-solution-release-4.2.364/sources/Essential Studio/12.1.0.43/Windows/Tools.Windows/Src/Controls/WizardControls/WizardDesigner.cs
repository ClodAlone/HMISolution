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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// WizardControl Designer.
    /// </summary>
    public class WizardDesigner : ParentControlDesigner, IAllowMakeDirty
    {
       private IComponentChangeService m_ccs = null;

        public WizardDesigner()
        {
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (this.Wizard != null)
            {
                this.Wizard.CardLayout.ComponentDesigner = this;
            }

            m_ccs = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (m_ccs != null)
            {
                m_ccs.ComponentRemoving += new ComponentEventHandler(ComponentRemoving);
            }
        }

        void IAllowMakeDirty.SetDirty()
        {
            System.ComponentModel.Design.IDesignerHost iDesignerHost;

            iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
            if (iDesignerHost != null)
                this.RaiseComponentChanged(null, null, null);
        }

        protected override bool DrawGrid
        {
            get { return false; }
        }

        protected Wizard Wizard
        {
            get { return (Wizard)Control; }
        }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerb addPage = new DesignerVerb("Add Page", new EventHandler(OnAddPage));
                DesignerVerb removePage = new DesignerVerb("Remove Page", new EventHandler(OnRemovePage));
                DesignerVerb nextPage = new DesignerVerb("Next Page", new EventHandler(OnNextPage));
                DesignerVerb previousPage = new DesignerVerb("Previous Page", new EventHandler(OnPreviousPage));
                return new DesignerVerbCollection(new DesignerVerb[] { addPage, removePage, previousPage, nextPage });
            }
        }

        protected virtual void OnAddPage(object sender, EventArgs e)
        {
            IDesignerHost ih = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (ih == null) return;

            WizardPage page = ih.CreateComponent(typeof(WizardPage)) as WizardPage;
            this.Wizard.AddPage(page);
        }
        protected virtual void OnRemovePage(object sender, EventArgs e)
        {
            if (this.Wizard.SelectedPage == null)
                MessageBox.Show("There is no selected wizard page in the wizard control to remove.");
            else
            {
                DialogResult ds = MessageBox.Show("Are you sure you want to remove the selected WizardPage and all of it's child controls?", "WizardPage delete confirmation message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ds == DialogResult.Yes)
                    this.Wizard.RemovePage();
            }
        }

        protected virtual void OnNextPage(object sender, EventArgs e)
        {
            this.Wizard.NextPage();
        }

        protected virtual void OnPreviousPage(object sender, EventArgs e)
        {
            this.Wizard.PreviousPage();
        }

        protected virtual void ComponentRemoving(object sender, ComponentEventArgs e)
        {
            if (Control != null && e.Component == this.Wizard.PageContainer)
            {
                throw new NotSupportedException("You can`t delete this component");
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            InitializeNewComponent();
        }
#else
		public override void OnSetComponentDefaults()
		{
			base.OnSetComponentDefaults();

			InitializeNewComponent();
		}
#endif
        protected virtual void InitializeNewComponent()
        {
            Wizard wizard = this.Wizard;
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            WizardContainer pageContainer = (WizardContainer)host.CreateComponent(typeof(WizardContainer));
            wizard.Controls.Add(pageContainer);
            wizard.SetPageContainer(pageContainer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_ccs = null;
            }

            base.Dispose(disposing);
        }
    }
}
