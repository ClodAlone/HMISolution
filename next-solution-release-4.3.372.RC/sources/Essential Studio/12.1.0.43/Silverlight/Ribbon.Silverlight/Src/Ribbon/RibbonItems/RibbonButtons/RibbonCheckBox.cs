#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents ribbon's check-box control.
	/// </summary>
    public class RibbonCheckBox : CheckBox, IRibbonControl
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonCheckBox"/> class.
		/// </summary>
		public RibbonCheckBox()
		{
			this.DefaultStyleKey = typeof(RibbonCheckBox);
			this.IsTabStop = false;
		}

        /// <summary>
        /// Initializes the <see cref="RibbonCheckBox"/> class.
        /// </summary>
        static RibbonCheckBox()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
		#endregion
	}
}
