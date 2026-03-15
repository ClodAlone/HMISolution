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

#region File Usign
using System;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    [
    ToolboxItem(false)
    ]
    internal class GradienPanelExtPropertyGrid : PropertyGrid
    {
        #region Class Members
        private IServiceProvider m_provider = null;
        #endregion

        #region Class Properties
        [
        Browsable( false ),
        DefaultValue( null )
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
        #endregion

        #region Class Initialize/Finalize Methods
        public GradienPanelExtPropertyGrid() : base()
        {
        }

        public GradienPanelExtPropertyGrid( IServiceProvider provider ) : this()
        {
            m_provider = provider;
        }
        #endregion

        #region Class Overrides
        protected override object GetService( Type service )
        {
            object svc = base.GetService( service );

            if( svc == null && service == typeof( IDesignerHost ) )
            {
                if( m_provider != null )
                {
                    svc = m_provider.GetService( service );
                }
            }

            return svc;
        }
        #endregion
    }
}
