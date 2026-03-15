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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;    
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Class Represents the Initializer
    /// </summary>
    internal class CheckListBoxInitializer:DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListBoxInitializer"/> class.
        /// </summary>
        public CheckListBoxInitializer()
        {
        }

        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.
        /// </exception>
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                CheckListBoxItem chkItem = new CheckListBoxItem();

                item.Properties["Items"].Collection.Add(chkItem);
                item.Properties["Items"].Collection[0].Properties["Content"].SetValue("Item 1");
                //item.Properties["Items"].Collection[0].Properties["Name"].SetValue("Item1");

                item.Properties["Items"].Collection.Add(chkItem);
                item.Properties["Items"].Collection[1].Properties["Content"].SetValue("Item 2");
                //item.Properties["Items"].Collection[1].Properties["Name"].SetValue("Item2");

                item.Properties["Items"].Collection.Add(chkItem);
                item.Properties["Items"].Collection[2].Properties["Content"].SetValue("Item 3");
                //item.Properties["Items"].Collection[2].Properties["Name"].SetValue("Item3");

                item.Properties["Items"].Collection.Add(chkItem);
                item.Properties["Items"].Collection[3].Properties["Content"].SetValue("Item 4");
                //item.Properties["Items"].Collection[3].Properties["Name"].SetValue("Item4");

                scope.Complete();
            }
        }
    }
}
