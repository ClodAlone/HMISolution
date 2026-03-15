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

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    internal class DockingInitializer : DefaultInitializer
    {
        public DockingInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
               
                Grid grid= new Grid();
                DockingManager.SetHeader(grid,"Dock Window 1");
                DockingManager.SetState(grid,DockState.Dock);
                DockingManager rr= new DockingManager();
                item.Properties["Children"].Collection.Add(grid);
                item.Properties["DockFill"].SetValue(true);
                item.Properties["Width"].SetValue(300d);
                item.Properties["Height"].SetValue(300d);
                scope.Complete();
            }
        }
    }
}
