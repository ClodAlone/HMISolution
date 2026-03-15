#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    #region CellTemplateSelector

    /// <summary>
    /// Represents a class for selecting the cell teplate
    /// </summary>
    public class CellTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the Default Template
        /// </summary>
        public DataTemplate DefaultTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Disabled Template
        /// </summary>
        public DataTemplate DisabledTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the Type of template
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            LoopingSelectorItem selectoritem = container as LoopingSelectorItem;
            if (selectoritem._state==LoopingSelectorItem.State.Disabled)
                return DisabledTemplate;
            else
                return DefaultTemplate;
        }
    }

    #endregion
}
