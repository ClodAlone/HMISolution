/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Opc.Ua.Server;

namespace Opc.Ua.StackTest
{
    /// <summary>
    /// A class executes the tests. 
    /// It provides UI functionality for executing the test cases on the server side.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructors
        /// <summary>
        /// This constructor gets the test server configuration.
        /// </summary>
        /// <param name="server"></param>
        public MainForm(TestServer server)
        {
            InitializeComponent();
            m_server = server;
        }
        #endregion
        
        #region Private Fields
        // This flag is used for exiting from the application        
        private bool m_exit;
        
        // An object of class TestServer <see cref="TestServer"/>        
        private TestServer m_server;
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Shows the diagnostics window and starts the update timer.
        /// </summary>
        private void ShowStatus()
        {            
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            Timer.Enabled = true;
        }
        
        /// <summary>
        /// Hides the diagnostics window and starts the update timer.
        /// </summary>
        private void HideStatus()
        {            
            this.WindowState = FormWindowState.Minimized;
            Timer.Enabled = false;
        }

        /// <summary>
        /// This method displays an unhandled exception.
        /// </summary>
        /// <param name="caption">This parameter stores the caption value for the message box.</param>
        /// <param name="method">Method details.</param>
		/// <param name="e">Exception</param>
		public static void HandleException(string caption, MethodBase method, Exception e)
		{
            if (String.IsNullOrEmpty(caption))
            {
                caption = method.Name;
            }

			MessageBox.Show(e.Message, caption);
		}
        #endregion
        
        #region Event Handlers
        /// <summary>
        /// This event is used to exit from the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMI_Click(object sender, EventArgs e)
        {
            m_exit = true;
            Close();
        }

        /// <summary>
        /// Shows the diagnostics window and starts the update timer.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ShowStatus();          
            }
            catch (Exception exception)
            {
				HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        /// <summary>
        /// This event handles the closing of the main form.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">FormClosingEventArgs</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing && !m_exit)
                {
                    e.Cancel = true;
                    HideStatus();
                }       
            }
            catch (Exception exception)
            {
				HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        /// <summary>
        /// Shows the diagnostics window and starts the update timer.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void ShowMI_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatus();          
            }
            catch (Exception exception)
            {
				HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        /// <summary>
        /// This method updates the timer.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                ServerStatusDataType status = m_server.GetStatus();

                StartTimeTB.Text   = Utils.Format("{0:HH:mm:ss.ff}", status.StartTime.ToLocalTime());
                CurrentTimeTB.Text = Utils.Format("{0:HH:mm:ss.ff}", status.CurrentTime.ToLocalTime());
            }
            catch (Exception exception)
            {
				HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
    }
}
