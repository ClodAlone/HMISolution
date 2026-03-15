#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Diagram.Base.Wizard;
using Syncfusion.Windows.Forms.Diagram.Controls;

namespace Syncfusion.Windows.Forms.Diagram.Wizards
{
    class DiagramWindowsOnLoadWizard : DiagramLoadBaseWizard
    {
        public DiagramWindowsOnLoadWizard(Control curWindowsControl)
        {
            if (curWindowsControl is Syncfusion.Windows.Forms.Diagram.Controls.Diagram)
            {
                this.btnLoadEDD.Enabled = true;
                this.btnLoadEDP.Enabled = false;
            }
            if (curWindowsControl is PaletteGroupView)
            {
                this.btnLoadEDD.Enabled = false;
                this.btnLoadEDP.Enabled = true;
            }

            this.BackgroundImage = DiagramWizardResources.WizardBackImage;
            this.btnCancel.Image = DiagramWizardResources.ButtonNormalImage;
            this.btnOk.Image = DiagramWizardResources.ButtonNormalImage;
            this.btnLoadEDD.Image = DiagramWizardResources.ButtonImage;
            this.btnLoadEDP.Image = DiagramWizardResources.ButtonImage;
            this.btnDiagramBuilder.Image = DiagramWizardResources.ButtonDbImage;
            this.btnSymbolDesigner.Image = DiagramWizardResources.ButtonSbImage;

            this.btnOk.HoverImage = DiagramWizardResources.ButtonHoverImage;
            this.btnOk.NormalImage = DiagramWizardResources.ButtonNormalImage;
            this.btnCancel.HoverImage = DiagramWizardResources.ButtonHoverImage;
            this.btnCancel.NormalImage = DiagramWizardResources.ButtonNormalImage;
        }
    }
}
