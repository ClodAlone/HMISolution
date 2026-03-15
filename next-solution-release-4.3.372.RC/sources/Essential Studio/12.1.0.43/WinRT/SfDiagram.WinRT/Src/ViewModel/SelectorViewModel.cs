#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Diagram
{
    public partial class SelectorViewModel : 
        GroupViewModel, 
        ISelector
    {
        public SelectorViewModel()
        {
            MinWidth = 10;
            MinHeight = 10;
            Nodes = new ObservableCollection<object>();
            Connectors = new ObservableCollection<object>();
            Groups = new ObservableCollection<object>();
        }
    }
}
