#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Reflection;

namespace Syncfusion.UI.Xaml.Schedule
{
    #region ExtensionMethods

    internal static class ExtensionMethods
    {
        internal static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }
    }

    #endregion

    #region TypeInfo

    internal class TypeInfo
    {
        internal Type Type { get; set; }

        internal TypeInfo(Type type)
        {
            Type = type;
        }

        internal PropertyInfo GetDeclaredProperty(string name)
        {
            return Type.GetProperty(name);
        }
    }

    #endregion

    #region EnumHelper

    internal static class EnumHelper
    {
        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            return fields.Select(field => field.GetValue(enumType)).ToArray();
        }
    }

    #endregion
}

