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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;

namespace Syncfusion.Windows.PropertyGrid
{
    public interface ITypeEditor
    {
        // Summary:
        //     Fired when the value of the current property has changed.
        //event PropertyChangedEventHandler ValueChanged;

        // Summary:
        //     Attaches (initializes) the editor with the current property.
        void Attach(PropertyViewItem property, PropertyItem info);
        //
        // Summary:
        //     Creates ands initializes a new instance of the editor
        object Create(PropertyInfo PropertyInfo);
        //
        // Summary:
        //     Detaches (releases) the editor that was attached with the property passed
        //     as parameter.
        void Detach(PropertyViewItem property);
    }
}
