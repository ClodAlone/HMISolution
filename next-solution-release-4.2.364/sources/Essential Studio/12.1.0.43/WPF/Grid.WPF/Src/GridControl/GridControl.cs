#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Security.Permissions;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

#if ENABLE_PARTIAL_TRUST
using System.Security;
#endif

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// GridControl provides a simplified way to use a grid control and model in one place. It inherits the features from <see cref="GridControlBase"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif    

#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]
#endif

    public class GridControl : GridControlBase
    {
        static GridControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridControl), new FrameworkPropertyMetadata(typeof(GridControl)));
           
        }

        /// <summary>
        /// Initializes a new <see cref="GridControl"/>.
        /// </summary>
        public GridControl()
        {
            if (IsSecurityGranted)
            {
                ValidateLicense();
            }
            Model = new GridModel();
            if (Model != null)
            {
                Model.ActiveGridView = this;
            }
        }

        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

       
    }
}
