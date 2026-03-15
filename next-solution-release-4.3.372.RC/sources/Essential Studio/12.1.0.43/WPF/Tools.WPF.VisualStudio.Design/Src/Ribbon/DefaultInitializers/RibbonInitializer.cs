// <copyright file="RibbonInitializer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Class Represents the Initializer
    /// </summary>
    internal class RibbonInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonInitializer"/> class.
        /// </summary>
        public RibbonInitializer( )
        {
        }

        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.
        /// </exception>
        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                ApplicationMenu menu = new ApplicationMenu();
                item.Properties["ApplicationMenu"].SetValue(menu);
                RibbonTab tab = new RibbonTab();
                //tab.Name = "RibbonTab1";
                tab.Caption = "New RibbonTab1";
                RibbonTab tab1 = new RibbonTab();
                //tab1.Name = "RibbonTab2";
                tab1.Caption = "New RibbonTab2";                                
                RibbonBar bar = new RibbonBar();
                //bar.Name = "RibbonBar1";
                bar.Header = "New RibbonBar1";
                tab.Items.Add(bar);                
                item.Properties["Items"].Collection.Add(tab);
                item.Properties["Items"].Collection.Add(tab1);                
                QuickAccessToolBar qat = new QuickAccessToolBar();
                //qat.Name = "QuickAccessToolBar1";
                item.Properties["QuickAccessToolBar"].SetValue(qat);
				item.Properties["Width"].SetValue(300d);
                item.Properties["Height"].SetValue(150d);
                scope.Complete();
            }
        }
    }
}
