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
using System.ComponentModel;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Design
{
	[Editor(typeof(DesignBindingEditor), typeof(UITypeEditor))]
	class DesignBinding 
	{
        
		// Fields
		private object dataSource;
		private string dataMember;
		public static DesignBinding Null;
        
		// Constructors
		public DesignBinding(object dataSource, string dataMember)
		{
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
        
		static DesignBinding()
		{
			DesignBinding.Null = new DesignBinding(null, null);
		}
       
		// Methods
        
		public bool IsNull 
		{ 
			get
			{
				return (this.dataSource == null);
			}
		}
        
		public object DataSource 
		{ 
			get
			{
				return this.dataSource;
			}
		}
        
		public string DataMember 
		{ 
			get
			{
				return this.dataMember;
			}
		}
        
		public string DataField 
		{ 
			get
			{
				int n;

				n = this.dataMember.LastIndexOf(".");
				if (n == -1)
					return this.dataMember;
				return this.dataMember.Substring(n + 1);
			}
		}
        
		public bool Equals(object dataSource, string dataMember)
		{
			if (dataSource == this.dataSource)
				return String.Compare(dataMember, this.dataMember, true) == 0;
			return false;
		}   
	}
}