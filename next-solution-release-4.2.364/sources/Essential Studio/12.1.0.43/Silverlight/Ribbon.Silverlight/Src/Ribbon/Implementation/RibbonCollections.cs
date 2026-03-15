#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
    /// Collection of <see cref="RibbonItemsCollection"/> instances.
	/// </summary>
	public class RibbonItemsCollection : ObservableCollection<UIElement>
	{
	}

	/// <summary>
    /// Collection of <see cref="RibbonTemplatesCollection"/> control templates.
	/// </summary>
	public class RibbonTemplatesCollection : ObservableCollection<ControlTemplate>
	{
	}

	/// <summary>
	/// Collection of <see cref="RibbonTab"/> instances.
	/// </summary>
	public class RibbonTabCollection : ObservableCollection<RibbonTab>
	{
	}
}
