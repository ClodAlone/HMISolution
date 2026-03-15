//ChildrenFinder.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace XamlQuery
{
    internal class ChildrenFinder
    {
        #region Properties

        public ControlSet Children { get; set; }

        #endregion

        #region Constructors

        public ChildrenFinder()
        {
            this.Children = new ControlSet();
        }

        #endregion

        #region Methods

        public void All(DependencyObject control)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(control);
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, childIndex);
                this.Children.Add(child);
                All(child);
            }
        }

        public void All_Strict(DependencyObject control)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(control);
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                //DependencyObject child = LogicalTreeHelper.GetChild(control, childIndex);
                DependencyObject child = VisualTreeHelper.GetChild(control, childIndex);
                this.Children.Add(child);
                All_Strict(child);
            }
        }

        public void FindByType<T>(DependencyObject control)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(control);
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, childIndex);
                if (child is T)
                {
                    this.Children.Add(child);
                }
                FindByType<T>(child);
            }
        }

        #endregion
    }
}
