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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Windows.Controls;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    internal class DocumentContainerInitializer : DefaultInitializer
    {
        public DocumentContainerInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                Grid item1 = new Grid();
                DocumentContainer.SetHeader(item1, "item1");
                item.Properties["Items"].Collection.Add(item1);

                Grid item2 = new Grid();
                DocumentContainer.SetHeader(item2, "item2");
                item.Properties["Items"].Collection.Add(item2);

                Grid item3 = new Grid();
                DocumentContainer.SetHeader(item3, "item3");
                item.Properties["Items"].Collection.Add(item3);

                scope.Complete();
            }
        }
    }
}
