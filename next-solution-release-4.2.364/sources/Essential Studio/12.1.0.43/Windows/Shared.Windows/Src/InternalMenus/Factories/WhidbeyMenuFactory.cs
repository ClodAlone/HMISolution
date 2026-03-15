#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Text;

namespace Syncfusion.Windows.Forms.InternalMenus
{
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
	internal class WhidbeyMenuFactory: MenuFactory
	{
		public WhidbeyMenuFactory()
		{

		}
		#region Factory Methods
		public override MenuImp CreateMenuImp()
		{
			return new WhidbeyMenuImp();
		}
		public override ToolBarImp CreateToolBarImp()
		{
			return new WhidbeyToolBarImp();
		}
		#endregion

	}
#endif
}
