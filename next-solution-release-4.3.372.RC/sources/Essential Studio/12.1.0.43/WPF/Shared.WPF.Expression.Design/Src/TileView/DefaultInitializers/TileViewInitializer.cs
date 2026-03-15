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


using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Shared.WPF.Expression.Design
{
    /// <summary>
    /// Represents TileViewInitializer to add default children
    /// </summary>
    internal class TileViewInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileViewInitializer"/> class.
        /// </summary>
        public TileViewInitializer()
        {
        }
        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.</exception>
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                TileViewItem TileItem = new TileViewItem();

                item.Properties["Items"].Collection.Add(TileItem);
                item.Properties["Items"].Collection.Add(TileItem);
                item.Properties["Items"].Collection.Add(TileItem);
                item.Properties["Items"].Collection.Add(TileItem);

                item.Properties["Items"].Collection[0].Properties["Header"].SetValue("Item 1");
                item.Properties["Items"].Collection[0].Properties["Name"].SetValue("TileViewItem1");

                item.Properties["Items"].Collection[1].Properties["Header"].SetValue("Item 2");
                item.Properties["Items"].Collection[1].Properties["Name"].SetValue("TileViewItem2");

                item.Properties["Items"].Collection[2].Properties["Header"].SetValue("Item 3");
                item.Properties["Items"].Collection[2].Properties["Name"].SetValue("TileViewItem3");

                item.Properties["Items"].Collection[3].Properties["Header"].SetValue("Item 4");
                item.Properties["Items"].Collection[3].Properties["Name"].SetValue("TileViewItem4");

                scope.Complete();
            }
        }
    }

}

