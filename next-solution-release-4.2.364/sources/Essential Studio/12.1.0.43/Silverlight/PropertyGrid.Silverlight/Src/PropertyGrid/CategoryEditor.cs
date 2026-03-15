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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.PropertyGrid
{
    public class CategoryEditorProperty
    {
        public string Name
        {
            get;
            set;
        }
    }

    public class CategoryEditor
    {
        public string Category
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public virtual DataTemplate EditorTemplate
        {
            get;
            set;
        }

        private CategoryEditorPropertyCollection _Properties = new CategoryEditorPropertyCollection();
        public CategoryEditorPropertyCollection Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }
    }
}
