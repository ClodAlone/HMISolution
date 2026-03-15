//-------------------------------------------------------------------------------------------------
// <copyright file="CalcUtils.cs" company="syncfusion">
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#if !SILVERLIGHT && !WINDOWS_UWP && !WP && !NET_STANDARD
namespace Syncfusion.Calculate
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;

    internal class Utilities
    {
#region Security Related
        /// <summary>
        ///  Calls SecurityPermission.Demand to find out if SecurityPermission is available.
        /// </summary>
        /// <returns>True if SecurityPermission is available.</returns>
        public static bool IsSecurityPermissionAvailable()
        {
            bool secPerm = true;
            SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
            try
            {
                perm.Demand();
            }
            catch (System.Security.SecurityException)
            {
                secPerm = false;
            }

            return secPerm;
        }
#endregion

#region Licensing Related
        internal static void ValidateLicense(Type typeToValidate)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedWebComponent(typeToValidate);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }
#endregion
    }
}
#endif