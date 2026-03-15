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
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
using Syncfusion.Shared.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Collections;

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
            Utils.SparseSetValue(item.Properties["Height"], 400d);

            TileViewItem tileviewitem = new TileViewItem();
            item.Properties["Items"].Collection.Add(tileviewitem);
            TileViewItem tileviewitem1 = new TileViewItem();
            item.Properties["Items"].Collection.Add(tileviewitem1);
            TileViewItem tileviewitem2 = new TileViewItem();
            item.Properties["Items"].Collection.Add(tileviewitem2);
            TileViewItem tileviewitem3 = new TileViewItem();
            item.Properties["Items"].Collection.Add(tileviewitem3);

            item.Properties["Items"].Collection[0].Properties["Header"].SetValue("New Tile Item");
            item.Properties["Items"].Collection[1].Properties["Header"].SetValue("New Tile Item");
            item.Properties["Items"].Collection[2].Properties["Header"].SetValue("New Tile Item");
            item.Properties["Items"].Collection[3].Properties["Header"].SetValue("New Tile Item");
            item.Properties["Items"].Collection[4].Properties["Header"].SetValue("New Tile Item");
           
        }
    }
}
