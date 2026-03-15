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
using System.Text;
using System.Windows.Media;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
using Syncfusion.Shared.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Shared.Silverlight.VisualStudio.Design
{
    internal class TileViewInitializer : DefaultInitializer
    {

        public TileViewInitializer()
            : base()
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context)
        {   
            Utils.SparseSetValue(item.Properties["Width"], 400d);
            Utils.SparseSetValue(item.Properties["Height"], 300d);

            TileViewItem tileviewitem = new TileViewItem();
            tileviewitem.Header = "Item 1";            
            TileViewItem tileviewitem1 = new TileViewItem();
            tileviewitem1.Header = "Item 2";            
            TileViewItem tileviewitem2 = new TileViewItem();
            tileviewitem2.Header = "Item 3";            
            TileViewItem tileviewitem3 = new TileViewItem();
            tileviewitem3.Header = "Item 4";            

            item.Properties["Items"].Collection.Add(tileviewitem);
            item.Properties["Items"].Collection.Add(tileviewitem1);
            item.Properties["Items"].Collection.Add(tileviewitem2);
            item.Properties["Items"].Collection.Add(tileviewitem3);
           
        }
    }
}
