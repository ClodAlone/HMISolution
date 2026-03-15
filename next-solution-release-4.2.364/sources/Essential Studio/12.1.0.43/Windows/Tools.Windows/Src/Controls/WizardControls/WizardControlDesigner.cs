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

using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// WizardControl Designer.
    /// </summary>
    public class WizardControlDesigner : WizardDesigner
    {
        public WizardControlDesigner()
        {
        }

        protected override void ComponentRemoving(object sender, ComponentEventArgs e)
        {
            base.ComponentRemoving(sender, e);

            if (Control != null && e.Component == WControl.BannerPanel)
            {
                throw new NotSupportedException("You are not alowed to remove this component. If you want the Panel to dissapear change the FullPage property of the active WizardPage");
            }
        }

        protected WizardControl WControl
        {
            get { return (WizardControl)Control; }
        }

        protected override void OnMouseDragBegin(int x, int y)
        {
            WControl.Capture = true;
            if (WControl.NextButton.Visible && WControl.NextButton.ClientRectangle.Contains(WControl.NextButton.PointToClient(new Point(x, y))))
            {
                WControl.NextPage();
                return;
            }
            if (WControl.BackButton.Visible && WControl.BackButton.ClientRectangle.Contains(WControl.BackButton.PointToClient(new Point(x, y))))
            {
                WControl.PreviousPage();
                return;
            }
            base.OnMouseDragBegin(x, y);
        }

        protected override void OnAddPage(object sender, EventArgs e)
        {
            IDesignerHost ih = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (ih == null) return;

            WizardControlPage page = ih.CreateComponent(typeof(WizardControlPage)) as WizardControlPage;
            WControl.AddPage(page);
        }

        protected override void InitializeNewComponent()
        {
            base.InitializeNewComponent();

            WizardControl wizard = this.WControl;
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            GradientPanel bannerPanel = (GradientPanel)host.CreateComponent(typeof(GradientPanel));
            wizard.Controls.Add(bannerPanel);
            wizard.BannerPanel = bannerPanel;

            PictureBox banner = (PictureBox)host.CreateComponent(typeof(PictureBox));
            wizard.BannerPanel.Controls.Add(banner);
            wizard.Banner = banner;

            Label title = (Label)host.CreateComponent(typeof(Label));
            title.AutoSize = true;
            wizard.BannerPanel.Controls.Add(title);
            wizard.Title = title;

            Label description = (Label)host.CreateComponent(typeof(Label));
            description.AutoSize = true;
            wizard.BannerPanel.Controls.Add(description);
            wizard.Description = description;

            wizard.EndInit();
            wizard.GridBagLayout.ComponentDesigner = this;
        }
    }

    public class WizardPageDesigner : ParentControlDesigner
    { 
        public override /*ControlDesigner*/ SelectionRules SelectionRules
        {
            get
            {
                System.Windows.Forms.Design.SelectionRules selectionRules;
                selectionRules = base.SelectionRules;
                selectionRules = (SelectionRules)(selectionRules /*& ~(SelectionRules.AllSizeable)*/ & SelectionRules.Locked);
                return selectionRules;
            }
        }

        protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
        {
            System.Windows.Forms.Panel panel;
            panel = (System.Windows.Forms.Panel)this.Component;
            if (panel.BorderStyle == BorderStyle.None)
                Syncfusion.Drawing.DrawingUtils.DrawDesignTimeBorder(pe.Graphics, panel);
            base.OnPaintAdornments(pe);
        }
    }

    public class WizardContainerDesigner :
        ParentControlDesigner
    {
        #region Overrides

        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules selectionRules = (SelectionRules)(base.SelectionRules & SelectionRules.Locked);

                return selectionRules;
            }
        }

        public override bool CanParent(Control control)
        {
            return false;
        }

        protected override void OnDragEnter(DragEventArgs de)
        {
            de.Effect = DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            de.Effect = DragDropEffects.None;
        }

        #endregion
    }
}
