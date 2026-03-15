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

namespace Opc.Ua.StackTest
{
    /// <summary>
    /// A class which displays the test parameters for a test case.
    /// </summary>
    public partial class TestParametersCtrl : Opc.Ua.Client.Controls.BaseListCtrl
    {
        /// <summary>
        /// Initializes GUI Components.
        /// Initializes column names.
        /// </summary>
        public TestParametersCtrl()
        {
            InitializeComponent();              
			SetColumns(m_ColumnNames);
        }

        #region Private Fields
       
        // An object of type TestCase.<see ref="TestCase"/>      
        private TestCase m_testcase;
       
		// The columns to display in the control.		
		private readonly object[][] m_ColumnNames = new object[][]
		{
			new object[] { "Name",  HorizontalAlignment.Left, null },  
			new object[] { "Value", HorizontalAlignment.Left,   null }
		};
		#endregion
        
        /// <summary>
        /// Displays a test case in the control.
        /// </summary>
        public void Initialize(TestCase testcase)
        {
            ItemsLV.Items.Clear();

            m_testcase = testcase;

            if (testcase != null)
            {
                AddParamater("TestID",   testcase.TestId);
                AddParamater("TestCase", testcase.Name);

                if (testcase.SeedSpecified)
                {
                    AddParamater("Seed", testcase.Seed);
                }
                
                if (testcase.StartSpecified)
                {
                    AddParamater("Start", testcase.Start);
                }
                
                if (testcase.CountSpecified)
                {
                    AddParamater("Count", testcase.Count);
                }

                AddParamater("SkipTest", testcase.SkipTest);

                if (testcase.Parameter != null)
                {
                    foreach (TestParameter parameter in testcase.Parameter)
                    {
                        AddItem(parameter);
                    }
                }
            }

            AdjustColumns();
        } 

        private void AddParamater(string name, object value)
        {
            TestParameter parameter = new TestParameter();

            parameter.Name  = name;
            parameter.Value = String.Format("{0}", value);

            AddItem(parameter);
        }

        #region Overridden Methods
        /// <see cref="Opc.Ua.Client.Controls.BaseListCtrl.EnableMenuItems" />
		protected override void EnableMenuItems(ListViewItem clickedItem)
		{
            // TBD
		}

        /// <see cref="Opc.Ua.Client.Controls.BaseListCtrl.UpdateItem(ListViewItem,object)" />
        protected override void UpdateItem(ListViewItem listItem, object item)
        {
			TestParameter parameter = item as TestParameter;

			if (parameter == null)
			{
				base.UpdateItem(listItem, item);
				return;
			}

			listItem.SubItems[0].Text = String.Format("{0}", parameter.Name);
			listItem.SubItems[1].Text = String.Format("{0}", parameter.Value);

            listItem.ImageKey = "SimpleItem";
			listItem.Tag = item;
        }
        #endregion
    }
}
