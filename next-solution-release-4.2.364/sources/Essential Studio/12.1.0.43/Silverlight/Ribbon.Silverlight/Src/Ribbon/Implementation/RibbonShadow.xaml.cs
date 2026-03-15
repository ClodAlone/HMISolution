#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Shadow implementation
	/// </summary>
	public sealed class RibbonShadow : Grid
	{
		#region Constructor

		/// <summary>
		/// Initializ a new instance of <see cref="RibbonShadow"/>
		/// </summary>
		public RibbonShadow()
		{
			System.Windows.Application.LoadComponent(this, new System.Uri("/Syncfusion.Ribbon.Silverlight;component/Ribbon/Implementation/RibbonShadow.xaml", System.UriKind.Relative));
		}

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.IsHitTestVisible = false;
		}
		#endregion
	}
}
