#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Diagram.Base.Wizard
{
    /// <summary>
    /// Base class for diagram wizard.
    /// </summary>
    public partial class DiagramLoadBaseWizard : Office2007Form
    {
        private static string m_pathToSampleFolder = @"SOFTWARE\Syncfusion\Essential Suite\InstalledVersions\";
        private static string c_autoShowWizardRegistryKey = "AutoShowWizard";
        private static string m_assemblyversion = Assembly.GetAssembly(typeof(DiagramLoadBaseWizard)).GetName().Version.ToString();

        /// <summary>
        /// Path of diagram file.
        /// </summary>
        private string m_EDDPath = String.Empty;
        private string[] m_EDPPath;

        /// <summary>
        /// Gets or sets the EDD path.
        /// </summary>
        /// <value>The EDD path.</value>
        public string EDDPath
        {
            get
            {
                return m_EDDPath;
            }
            set
            {
                m_EDDPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the EDP path.
        /// </summary>
        /// <value>The EDP path.</value>
        public string[] EDPPath
        {
            get
            {
                return m_EDPPath;
            }
            set
            {
                m_EDPPath = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramLoadBaseWizard"/> class.
        /// </summary>
        public DiagramLoadBaseWizard()
        {
            InitializeComponent();

            if (GetAutoRunWizard())
            {
                chbStartup.Checked = true;
            }
            else
            {
                chbStartup.Checked = false;
            }
        }

        /// <summary>
        /// Gets the auto run wizard.
        /// </summary>
        /// <returns>true, if auto run wizard.</returns>
        public static bool GetAutoRunWizard()
        {
            bool autoShow = true;

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(m_pathToSampleFolder);

                if (key != null)
                {
                    if (key.GetValue(c_autoShowWizardRegistryKey).ToString() == "0")
                    {
                        autoShow = false;
                    }
                    else
                    {
                        autoShow = true;
                    }
                }
                else
                {
                    key = Registry.CurrentUser.CreateSubKey(m_pathToSampleFolder);
                    key.SetValue(c_autoShowWizardRegistryKey, 1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return autoShow;
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <param name="assemblyVersion">The assembly version.</param>
        /// <returns>The assembly version.</returns>
        public static string GetVersion(string assemblyVersion)
        {
            string ver = assemblyVersion;
            string temp = ver.Substring(0, ver.IndexOf("."));
            ver = ver.Substring(ver.IndexOf(".") + 1);
            temp = temp + "." + ver.Substring(0, ver.IndexOf(".")).Substring(0, 1);
            ver = ver.Substring(ver.IndexOf(".") + 1);
            temp = temp + "." + ver;
            return temp;
        }

        /// <summary>
        /// Handles the Click event of the DiagramBuilder button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DiagramBuilder_Click(object sender, EventArgs e)
        {
            string pathToDiagramBuilder = @"\Utilities\Diagram\Windows Forms\DiagramBuilder\DiagramBuilder.exe";
            string workingDirectoryPath = @"\Utilities\Diagram\Windows Forms\DiagramBuilder";
            RegistryKey pRegKey = Registry.LocalMachine;
            m_assemblyversion = GetVersion(m_assemblyversion);
            pRegKey = pRegKey.OpenSubKey(m_pathToSampleFolder + m_assemblyversion);
            object subkeyvalue = pRegKey.GetValue("");

            System.Diagnostics.ProcessStartInfo startInfo;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo(subkeyvalue.ToString() + pathToDiagramBuilder);
            startInfo.WorkingDirectory = subkeyvalue.ToString() + workingDirectoryPath;
            startInfo.UseShellExecute = false;

            pProcess.StartInfo = startInfo;
            pProcess.Start();
            pProcess.WaitForExit();
        }

        private void SymbolDesigner_Click(object sender, EventArgs e)
        {
            string pathToDiagramBuilder = @"\Utilities\Diagram\Windows Forms\Symbol Designer\Syncfusion.SymbolDesigner.exe";
            string workingDirectoryPath = @"\Utilities\Diagram\Windows Forms\Symbol Designer";
            RegistryKey pRegKey = Registry.LocalMachine;
            m_assemblyversion = GetVersion(m_assemblyversion);
            pRegKey = pRegKey.OpenSubKey(m_pathToSampleFolder + m_assemblyversion);
            object subkeyvalue = pRegKey.GetValue("");

            System.Diagnostics.ProcessStartInfo startInfo;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo(subkeyvalue.ToString() + pathToDiagramBuilder);
            startInfo.WorkingDirectory = subkeyvalue.ToString() + workingDirectoryPath;
            startInfo.UseShellExecute = false;

            pProcess.StartInfo = startInfo;
            pProcess.Start();
            pProcess.WaitForExit();
        }

        private void LoadEDD_Click(object sender, EventArgs e)
        {
            if (dEDD.ShowDialog() == DialogResult.OK)
            {
                EDDPath = dEDD.FileName;
            }
        }
        
        private void LoadEDP_Click(object sender, EventArgs e)
        {
            if (dEDP.ShowDialog() == DialogResult.OK)
            {
                EDPPath = dEDP.FileNames;
            }
        }
        
        private void Startup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(m_pathToSampleFolder);
            if (chbStartup.Checked == true)
            {
                key.SetValue(c_autoShowWizardRegistryKey, 1);
            }
            else
            {
                key.SetValue(c_autoShowWizardRegistryKey, 0);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           
                EDDPath = string.Empty;
               
        }
    }
}
