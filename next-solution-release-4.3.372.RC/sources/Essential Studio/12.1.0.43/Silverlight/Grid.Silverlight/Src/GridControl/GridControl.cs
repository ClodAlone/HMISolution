#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if!WinRT
using System.Security.Permissions;
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridControl : GridControlBase
    {
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
                return true;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    GC.SuppressFinalize(this);
        //}

    }
}
