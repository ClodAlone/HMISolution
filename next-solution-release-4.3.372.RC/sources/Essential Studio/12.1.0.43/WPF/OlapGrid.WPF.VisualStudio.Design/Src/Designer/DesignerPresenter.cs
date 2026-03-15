#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// The Presenter used for binding the view and model together and displays the view in the wizard.
    /// </summary>
    public sealed class DesignerPresenter
    {
        #region Members

        // private WizardWindow wizardWindowInstance = WizardWindow.Instance;
        private WizardWindow wizardWindowInstance = null;
        private List<WizardWindow> wizardWindows = new List<WizardWindow>();

        #endregion

        #region Constructor

        public DesignerPresenter(object model, IVisualDesigner view)
        {
            this.View = view;
            this.View.Model = this.Model = model;        
        }

        #endregion

        #region Properties

        public IVisualDesigner View
        {
            get;
            set;
        }

        public object Model
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        public void ShowWizard(string wizardName)
        {
            //if (this.wizardWindowInstance == null)
            //{
            //    System.Windows.MessageBox.Show("Only once");
            //    this.WizardWindow = new WizardWindow();
            //    this.WizardWindow.UIPropertiesLayout = (this.View as System.Windows.Controls.UserControl);
            //}

            //if (this.wizardWindows.Count > 0)
            //{

            //    var v = (from w in this.wizardWindows
            //             where w.Name == wizardName
            //             select w);

            //    if (v == null)
            //    {
            //        this.wizardWindowInstance = new WizardWindow();
            //        this.wizardWindowInstance.Name = wizardName;
            //        this.wizardWindows.Add(this.wizardWindowInstance);
            //        this.wizardWindowInstance.Show();
            //    }
            //    else
            //    {
            //        (v as WizardWindow).Show();
            //    }
            //}
            this.wizardWindowInstance = new WizardWindow();
            this.wizardWindowInstance.Show();
        }

        #endregion
    }
}
