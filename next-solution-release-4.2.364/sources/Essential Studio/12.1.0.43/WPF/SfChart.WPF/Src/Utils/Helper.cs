#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Syncfusion.UI.Xaml.Charts
{
#if SyncfusionFramework4_0
    internal static class ExtensionMethods
    {
        internal static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }
    }

    internal class TypeInfo
    {
        internal Type Type { get; set; }

        internal TypeInfo(Type type)
        {
            Type = type;
        }

        internal PropertyInfo GetDeclaredProperty(string name)
        {
            return this.Type.GetProperty(name);
        }
    }
#endif
}
