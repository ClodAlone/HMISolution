#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [ToolboxItem(false)]
    internal class TreeViewAdvEditorPropertyGrid : PropertyGrid
    {
        private IServiceProvider m_provider = null;

        [
          Browsable(false),
            DefaultValue(null)
          ]
        public IServiceProvider Provider
        {
            get
            {
                return m_provider;
            }
            set
            {
                m_provider = value;
            }
        }

        public TreeViewAdvEditorPropertyGrid()
            : base()
        {
        }

        public TreeViewAdvEditorPropertyGrid(IServiceProvider provider)
            :
              this()
        {
            m_provider = provider;
        }

        protected override object GetService(Type service)
        {
            object svc = base.GetService(service);

            if (svc == null && service == typeof(IDesignerHost))
            {
                if (m_provider != null)
                {
                    svc = m_provider.GetService(service);
                }
            }

            return svc;
        }
    }
}