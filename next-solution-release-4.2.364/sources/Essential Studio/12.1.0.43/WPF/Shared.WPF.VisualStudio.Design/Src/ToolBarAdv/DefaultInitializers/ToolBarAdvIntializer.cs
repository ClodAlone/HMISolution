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
using System.Windows.Controls;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    internal class ToolBarAdvIntializer : DefaultInitializer
    {
        public ToolBarAdvIntializer()
        {
        }
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                
                ToolBarAdv tool = new ToolBarAdv();
              
                //item.Properties["Items"].Collection.Add(tool);


                item.Properties["Height"].SetValue(40d);
                item.Properties["Width"].SetValue(105d);
               
                
                
            scope.Complete();
            }

        }
    }
}
