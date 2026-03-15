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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Chart;
using System.Windows.Forms.Design;
using Microsoft.Win32;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart.Design
{
    /// <summary>
    /// This is a designer for the ChartControl control.
    /// </summary>
    ///<internalonly/>
    [DocumentationExclude()]
    public class ChartControlDesigner : ControlDesigner
    {
        #region Constants
        /// <summary>
        /// Store default filter for open template dialog.
        /// </summary>
        private const string c_templateFileFilter = "*.xml | *.xml";

        /// <summary>
        /// Store default question on reset chart settings.
        /// </summary>
        private const string c_resetMesaage = "Are you sure want to reset settings?";

        /// <summary>
        /// Store default path to  keys of the chart control.
        /// </summary>
        public const string c_registryPath = @"Software\Syncfusion\Essential Suite\Chart";

        /// <summary>
        /// Store auto show key name in registry.
        /// </summary>
        public const string c_autoShowWizardRegistryKey = "AutoShowWizard";

        private const string c_registryInstallPath = @"Software\Syncfusion\Essential Suite\InstalledVersions\{0}";
        #endregion

        #region Members
        /// <summary>
        /// Designer verb for showing the wizard.
        /// </summary>
        protected DesignerVerb m_dvDisplayChartWizard = null;

        /// <summary>
        /// Designer verb for saving the template.
        /// </summary>
        protected DesignerVerb m_dvSaveTemplate = null;

        /// <summary>
        /// Designer verb for resetting the template.
        /// </summary>
        protected DesignerVerb m_dvResetTemplate = null;

        /// <summary>
        /// Designer verb for loading the template.
        /// </summary>
        protected DesignerVerb m_dvLoadTemplate = null;

        /// <summary>
        /// Designer verb for reverting the old settings.
        /// </summary>
        protected DesignerVerb m_dvApplyClassicStyle = null;

        /// <summary>
        /// Designer verb for reverting the default settings.
        /// </summary>
        protected DesignerVerb m_dvRevertDefaultStyle = null;

        /// <summary>
        /// The collection of designer verbs
        /// </summary>
        protected DesignerVerbCollection m_dvcVerbs = null;

#if SyncfusionFramework2_0
        private DesignerActionListCollection m_actionLists;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether auto run wizard.
        /// </summary>
        public static bool AutoRunWizard
        {
            get
            {
                byte autoShow = 0;

                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(c_registryPath);

                    if (key != null)
                    {
                        string[] values = key.GetValueNames();

                        if (values.Length == 1 && string.Compare(values[0], c_autoShowWizardRegistryKey) == 0)
                        {
                            object o = key.GetValue(c_autoShowWizardRegistryKey);
                            autoShow = byte.Parse(o.ToString());
                        }
                        else
                        {
                            key = Registry.CurrentUser.CreateSubKey(c_registryPath);
                            key.SetValue(c_autoShowWizardRegistryKey, 1);
                            autoShow = 1;
                        }
                    }
                    else
                    {
                        key = Registry.CurrentUser.CreateSubKey(c_registryPath);
                        key.SetValue(c_autoShowWizardRegistryKey, 1);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                return autoShow != 0;
            }

            set
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(c_registryPath);
                key.SetValue(c_autoShowWizardRegistryKey, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets the <see cref="ChartControl"/>.
        /// </summary>
        public ChartControl ChartControl
        {
            get
            {
                return Control as ChartControl;
            }
        }

        /// <summary>
        /// The designer verbs collection.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return m_dvcVerbs;
            }
        }

        /// <summary>
        /// Gets the registry path.
        /// </summary>
        /// <value>The registry path.</value>
        public static string RegistryPath
        {
            get
            {
                Type type = typeof(ChartControlDesigner);

                return string.Format(c_registryInstallPath, type.Assembly.GetName().Version);
            }
        }

        /// <summary>
        /// Gets the install path.
        /// </summary>
        /// <value>The install path.</value>
        public static string InstallPath
        {
            get
            {
                string installPath = String.Empty;

                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath);

                if (key == null)
                {
                    key = Registry.LocalMachine.OpenSubKey(RegistryPath);
                }

                if (key != null)
                {
                    object value = key.GetValue(String.Empty);

                    if (value != null)
                    {
                        installPath = value.ToString();
                    }
                }
                
                return installPath;
            }
        }
#if SyncfusionFramework2_0
        /// <summary>
        /// This member overrides the <see cref="Syncfusion.Windows.Forms.Chart.Design.ChartControlDesigner.ActionLists"/> property.
        /// </summary>        
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == m_actionLists)
                {
                    m_actionLists = new DesignerActionListCollection();
                    m_actionLists.Add(
                            new ChartControlActionList(this.Component));
                }

                return m_actionLists;
            }
        }
#endif
        #endregion

        #region Constrcutor
        /// <summary>
        /// Initializes a new instance of the ChartControlDesigner class.
        /// </summary>
        public ChartControlDesigner()
        {
            m_dvDisplayChartWizard = new DesignerVerb("Chart Wizard...", new EventHandler(this.HandleChartWizard));
            m_dvDisplayChartWizard.Enabled = true;

            m_dvLoadTemplate = new DesignerVerb("Load Template...", new EventHandler(this.HandleLoadTemplate));
            m_dvLoadTemplate.Enabled = true;

            m_dvResetTemplate = new DesignerVerb("Reset Template", new EventHandler(this.HandleResetTemplate));
            m_dvResetTemplate.Enabled = true;

            m_dvSaveTemplate = new DesignerVerb("Save Template...", new EventHandler(this.HandleSaveTemplate));
            m_dvSaveTemplate.Enabled = true;

            this.m_dvApplyClassicStyle = new DesignerVerb("Apply Classic Style", new EventHandler(this.HandleApplyClassicStyle));
            this.m_dvApplyClassicStyle.Enabled = true;

            this.m_dvRevertDefaultStyle = new DesignerVerb("Apply Default Style", new EventHandler(this.HandleRevertDefaultStyle));
            this.m_dvRevertDefaultStyle.Enabled = true;

            DesignerVerb[] dvarray = new DesignerVerb[] { m_dvDisplayChartWizard, m_dvSaveTemplate, m_dvLoadTemplate, 
                 m_dvResetTemplate, m_dvApplyClassicStyle, m_dvRevertDefaultStyle };
            m_dvcVerbs = new DesignerVerbCollection(dvarray);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Called when the designer is initialized.
        /// </summary>
        [Obsolete("This method is not used anymore")]
        public override void OnSetComponentDefaults()
        {
            base.OnSetComponentDefaults();

            if (this.ChartControl != null)
            {               
                if (AutoRunWizard)
                {
                    this.ChartControl.DisplayWizard();
                }
            }
        }

        /// <summary>
        /// Overrides Dispose.  Here we remove our handler for the selection changed
        /// event.  With designers, it is critical that they clean up any events they
        /// have attached.  Otherwise, during the course of an editing session many
        /// designers may get created and never destroyed.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Adjusts the set of properties the component exposes through a TypeDescriptor.
        /// </summary>
        /// <param name="properties">An IDictionary that contains the properties for the class of the component.</param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            String[] strcolln = new String[1];
            strcolln[0] = "DialogResult";

            RemovePropertyBrowsable(this.Component, strcolln, properties);
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Called when the Save Template verb is selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleSaveTemplate(object sender, EventArgs e)
        {
            bool templateAll = false;
            string msg = "Do you want store only the appearance?";

            if (MessageBox.Show(msg, "Template Storing Option...", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                templateAll = true;
            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = c_templateFileFilter;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ChartTemplate.StoreAllProperties = templateAll;
                ChartTemplate template = new ChartTemplate(typeof(ChartControl));                
                template.Scan(this.ChartControl);
                template.Save(sfd.FileName);
            }
        }

        /// <summary>
        /// Called when the Load Template verb is selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleLoadTemplate(object sender, EventArgs e)
        {
            ChartTemplateWizard templateWizard = new ChartTemplateWizard();

            if (templateWizard.ShowDialog() == DialogResult.OK)
            {
                if (templateWizard.Template != null)
                {
                    templateWizard.Template.Apply(this.ChartControl);
                }
                else
                {
                    MessageBox.Show(ChartWizardResources.NullTemplateSelectedText,
                        ChartWizardResources.NullTemplateSelectedTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Called when the reset Template verb is selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleResetTemplate(object sender, EventArgs e)
        {
            if (MessageBox.Show(c_resetMesaage, "Reset...", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ChartTemplate template = new ChartTemplate(typeof(ChartControl));
                template.Reset(this.ChartControl);
            }
        }

        /// <summary>
        /// Handles the apply classic style.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleApplyClassicStyle(object sender, EventArgs e)
        {
            ChartAppearanceStyles.ApplyClassicStyle(this.ChartControl);
        }

        /// <summary>
        /// Handles the revert default style.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleRevertDefaultStyle(object sender, EventArgs e)
        {
            ChartAppearanceStyles.RevertDefaultStyle(this.ChartControl);
        }

        /// <summary>
        /// Called when the Wizard verb is selected.
        /// </summary>
        /// <param name="sender">A sender of event.</param>
        /// <param name="e">Argument.</param>
        public void HandleChartWizard(object sender, EventArgs e)
        {
            if (this.ChartControl != null)
            {
                this.ChartControl.DisplayWizard();
            }
        }

        /// <summary>
        /// Remove a set of properties.
        /// </summary>
        /// <param name="control">The control to which the changes apply.</param>
        /// <param name="strcolln">The array of property names to exclude.</param>
        /// <param name="properties">Contains the properties for the class of the component.</param>
        static private void RemovePropertyBrowsable(IComponent control, String[] strcolln, IDictionary properties)
        {
            foreach (String property in strcolln)
            {
                PropertyDescriptor prop = (PropertyDescriptor)properties[property];
                if ((prop != null) && (prop.IsBrowsable == true))
                {
                    AttributeCollection mac = prop.Attributes;
                    bool bnondef = false;
                    foreach (Attribute mematt in mac)
                    {
                        // Is Browsable a default attribute? If so, break.
                        if (mematt as BrowsableAttribute != null)
                        {
                            bnondef = true;
                            break;
                        }
                    }

                    int ncount = (bnondef == true) ? mac.Count : mac.Count + 1;
                    Attribute[] arrmematt = new Attribute[ncount];
                    mac.CopyTo(arrmematt, 0);
                    if (bnondef == true)
                        arrmematt[Array.IndexOf(arrmematt, BrowsableAttribute.Yes)] = BrowsableAttribute.No;
                    else
                        arrmematt[ncount - 1] = BrowsableAttribute.No;

                    properties[property] = TypeDescriptor.CreateProperty(control.GetType(), prop, arrmematt);
                }
            }
        }
        #endregion
    }
}