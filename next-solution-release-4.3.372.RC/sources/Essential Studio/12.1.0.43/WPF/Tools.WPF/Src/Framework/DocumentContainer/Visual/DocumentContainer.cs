// <copyright file="DocumentContainer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Implementation
        /// <summary>
        /// Prepares the main menu.
        /// </summary>
        private void PrepareMainMenu()
        {
            Visual parent = VisualUtils.FindRootVisual(this);
            if (parent != null)
            {
                try
                {
                    Menu menu = (Menu)GetMenu(parent, typeof(Menu));

                    if (null != menu)
                    {
                        DefaultMenuItemsPanelTemplate = menu.ItemsPanel;
                        BindingUtils.SetBinding(menu, this, Menu.ItemsPanelProperty, DocumentContainer.MenuItemsPanelTemplateProperty);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets the menu.
        /// </summary>
        /// <param name="rootelement">The root element</param>
        /// <param name="typeChild">The type child.</param>
        /// <returns>Menu root element</returns>
        private static Menu GetMenu(DependencyObject rootelement, Type typeChild)
        {
            if (rootelement != null)
            {
                int cnt = VisualTreeHelper.GetChildrenCount(rootelement);

                for (int i = 0; i < cnt; ++i)
                {
                    DependencyObject visual = VisualTreeHelper.GetChild(rootelement, i);

                    if (typeChild.IsInstanceOfType(visual) && DocumentContainer.GetIsCommandMenu(visual))
                    {
                        return (Menu)visual;
                    }

                    Menu menu = GetMenu(visual, typeChild);

                    if (null != menu)
                    {
                        return menu;
                    }
                }

                return null;
            }
            else
            {
                throw new ArgumentNullException("root element");
            }
        }
        #endregion
    }
}
