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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    internal class SplitButtonAdvInitializer : DefaultInitializer
    {
        public SplitButtonAdvInitializer()
        {

        }
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                SplitButtonAdv split = new SplitButtonAdv();

               item.Properties["Height"].SetValue(39d);
               item.Properties["Width"].SetValue(105d);
               scope.Complete();
            }
        }
    }
    
     
}
