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

using System.ComponentModel;
using System.Reflection;
using System;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()
	]
	class ToolsComp
	{
		static ToolsComp()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SharedResolver.Resolve);
		}

		public static Type GetPublicType()
		{
			return typeof(ToolsComp);
		}

		public ToolsComp()
		{
		}

		virtual public int GetVer()
		{
			return 0;
		}
	}
}