#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
namespace System.ComponentModel
#else
using System.Diagnostics;

namespace System.ComponentModel
#endif
{
#if WinRT
    [Conditional("Obsolete")]
    internal class BrowsableAttribute : Attribute
    {
        internal BrowsableAttribute(bool value)
        {
        }
    }
#endif
    //[Conditional("Obsolete")]
    //internal class CategoryAttribute : Attribute
    //{
    //    internal CategoryAttribute(string value)
    //    {
    //    }
    //}

    //[Conditional("Obsolete")]
    //internal class DescriptionAttribute : Attribute
    //{
    //    internal DescriptionAttribute(string value)
    //    {
    //    }
    //}

    [Conditional("Obsolete")]
    internal class DesignerSerializationVisibilityAttribute : Attribute
    {
        internal DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility value)
        {
        }
    }

    [Conditional("Obsolete")]
    internal class NotifyParentPropertyAttribute : Attribute
    {
        internal NotifyParentPropertyAttribute(bool value)
        {
        }
    }

    internal enum DesignerSerializationVisibility
    {
        Hidden
    }

}

namespace System
{
    [Conditional("Obsolete")]
    internal class SerializableAttribute : Attribute
    {
    }
}
