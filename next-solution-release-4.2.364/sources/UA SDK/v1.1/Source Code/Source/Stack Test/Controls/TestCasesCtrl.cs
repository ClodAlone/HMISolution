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
using Opc.Ua.Client.Controls;

namespace Opc.Ua.StackTest
{
    public partial class TestCasesCtrl : Opc.Ua.Client.Controls.BaseListCtrl
    {
        /// <summary>
        /// Initializes GUI Components.
        /// Initializes column names.
        /// </summary>
        public TestCasesCtrl()
        {
            InitializeComponent();                 
			SetColumns(m_ColumnNames);
        }

        #region Private Fields
        
        // An object of type TestSequence. <see cref="TestSequence"/>      
        private TestSequence m_sequence;
        
        // An object of type TestParametersCtrl. <see cref="TestParametersCtrl"/>       
        private TestParametersCtrl m_ParametersCTRL;
       
		// The columns to display in the control.		
		private readonly object[][] m_ColumnNames = new object[][]
		{
			new object[] { "ID",    HorizontalAlignment.Center, null },  
			new object[] { "Name",  HorizontalAlignment.Left,   null }
		};
		#endregion
        
        /// <summary>
        /// The control used to display the parameters for the selected test case.
        /// </summary>
        public TestParametersCtrl ParametersCTRL
        {
            get { return m_ParametersCTRL;  }
            set { m_ParametersCTRL = value; }
        }

        /// <summary>
        /// The test sequence, which the client must execute.
        /// </summary>
        public TestSequence SequenceToExecute
        {
            get { return m_sequence; }
        }

        /// <summary>
        /// Displays a test sequence in the control.
        /// </summary>
        public void Initialize(TestSequence sequence)
        {
            ItemsLV.Items.Clear();

            m_sequence = sequence;

            if (sequence != null)
            {
                foreach (TestCase testcase in sequence.TestCase)
                {
                    AddItem(testcase);
                }
            }

            AdjustColumns();
        } 

        #region Overridden Methods
        /// <see cref="Opc.Ua.Client.Controls.BaseListCtrl.SelectItems" />
        protected override void SelectItems()
        {
            if (ItemsLV.SelectedItems.Count == 1)
            {
                m_ParametersCTRL.Initialize(ItemsLV.SelectedItems[0].Tag as TestCase);
            }

            base.SelectItems();
        }

        /// <see cref="Opc.Ua.Client.Controls.BaseListCtrl.EnableMenuItems" />
		protected override void EnableMenuItems(ListViewItem clickedItem)
		{
            foreach (ListViewItem item in ItemsLV.SelectedItems)
            {
                TestCase testcase = item.Tag as TestCase;

                if (testcase != null)
                {
                    IncludeTestMI.Checked = !testcase.SkipTest;
                    IncludeTestMI.Enabled = true;
                    break;
                }
            }
		}

        /// <see cref="Opc.Ua.Client.Controls.BaseListCtrl.UpdateItem(ListViewItem,object)" />
        protected override void UpdateItem(ListViewItem listItem, object item)
        {
			TestCase testcase = item as TestCase;

			if (testcase == null)
			{
				base.UpdateItem(listItem, item);
				return;
			}
            
			listItem.SubItems[0].Text = String.Format("{0}", testcase.TestId);

            if (!String.IsNullOrEmpty(testcase.DisplayText))
            {
			    listItem.SubItems[1].Text = String.Format("{0}", testcase.DisplayText);
            }
            else
            {
			    listItem.SubItems[1].Text = String.Format("{0}", testcase.Name);
            }

            if (testcase.SkipTest)
            {
                listItem.ImageKey = "Property";
            }
            else
            {
                listItem.ImageKey = "Method";
            }

			listItem.Tag = item;
        }

        private void IncludeTestMI_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem item in ItemsLV.SelectedItems)
                {
                    TestCase testcase = item.Tag as TestCase;

                    if (testcase != null)
                    {
                        testcase.SkipTest = !IncludeTestMI.Checked;
                        UpdateItem(item, testcase);
                    }
                }
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
    }
}
