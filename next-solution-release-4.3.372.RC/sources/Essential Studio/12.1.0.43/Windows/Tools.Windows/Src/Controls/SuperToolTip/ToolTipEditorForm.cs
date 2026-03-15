#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// ToolTipEditor Form
    /// </summary>
    public partial class ToolTipEditorForm : Form
    {
        #region *** ToolTipPropertyGrid

        public class ToolTipPropertyGrid : PropertyGrid
        {
            #region Constructors

            public ToolTipPropertyGrid(IServiceProvider parentProvider)
            {
                m_parentProvider = parentProvider;
            }
            #endregion

            #region Overrides

            protected override object GetService(Type service)
            {
                object svc = base.GetService(service);

                if (svc == null && service == typeof(System.ComponentModel.Design.IDesignerHost))
                {
                    if (m_parentProvider != null)
                    {
                        svc = m_parentProvider.GetService(service);
                    }
                }

                return svc;
            }
            #endregion

            #region Fields
            private IServiceProvider m_parentProvider;
            #endregion
        }
        #endregion

        #region Constructors

        public ToolTipEditorForm(ToolTipInfo toolTipInfo, IServiceProvider provider)
        {
            Initialize(toolTipInfo, provider);
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;
        }
        #endregion

        #region Implementation

        private void Initialize(ToolTipInfo toolTipInfo, IServiceProvider provider)
        {
            if (InitializeSurface(provider))
            {
                this.ToolTipControl.Info = new ToolTipInfo(toolTipInfo);
                SubscribeToServicesEvents();
            }
        }

        public bool InitializeSurface(IServiceProvider provider)
        {
            bool bResult = false;

            m_designSurface = new DesignSurface(typeof(ToolTipControl));

            Control designView = m_designSurface.View as Control;
            if (designView != null)
            {
                m_host = m_designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (m_host != null)
                {
                    m_selectionSvc = m_designSurface.GetService(typeof(ISelectionService)) as ISelectionService;
                    if (m_selectionSvc != null)
                    {
                        designView.Dock = DockStyle.Fill;

                        m_propertyGrid = new ToolTipPropertyGrid(provider);
                        m_propertyGrid.Size = new Size(250, m_propertyGrid.Size.Height);
                        m_propertyGrid.Dock = DockStyle.Right;
                        m_propertyGrid.PropertySort = PropertySort.CategorizedAlphabetical;
                        m_propertyGrid.ToolbarVisible = true;
                        m_propertyGrid.CommandsVisibleIfAvailable = false;

                        Splitter splitter = new Splitter();
                        splitter.Dock = DockStyle.Right;

                        Panel designPanel = new Panel();
                        designPanel.Dock = DockStyle.Fill;
                        designPanel.BorderStyle = BorderStyle.Fixed3D;

                        designPanel.Controls.Add(designView);
                        designPanel.Controls.Add(splitter);
                        designPanel.Controls.Add(m_propertyGrid);

                        this.Controls.Add(designPanel);

                        bResult = true;
                    }
                }
            }

            return bResult;
        }

        public void SubscribeToServicesEvents()
        {
            if (m_selectionSvc != null)
            {
                m_selectionSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
                OnSelectionChanged(m_selectionSvc, EventArgs.Empty);
            }
        }
        #endregion

        #region Event handler

        public void OnSelectionChanged(object sender, EventArgs e)
        {
            ICollection componentsCollection = m_selectionSvc.GetSelectedComponents();
            object[] componentsArray = new object[componentsCollection.Count];

            int i = 0;
            foreach (object obj in componentsCollection)
            {
                componentsArray[i] = (obj is ToolTipControl) ? ((ToolTipControl)obj).Info : obj;
                i++;
            }

            m_propertyGrid.SelectedObjects = componentsArray;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region Properties

        public ToolTipInfo ToolTip
        {
            get
            {
                return this.ToolTipControl.Info;
            }
        }

        public ToolTipControl ToolTipControl
        {
            get
            {
                return m_host.RootComponent as ToolTipControl;
            }
        }
        #endregion

        #region Fields
        private PropertyGrid m_propertyGrid = null;
        private DesignSurface m_designSurface = null;
        private IDesignerHost m_host = null;
        private ISelectionService m_selectionSvc = null;
        #endregion
    }
}
