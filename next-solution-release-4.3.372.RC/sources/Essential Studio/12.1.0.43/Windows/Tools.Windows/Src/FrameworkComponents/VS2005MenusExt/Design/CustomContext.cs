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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region CustomContext
	abstract class CustomContext : ITypeDescriptorContext
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseContext"></param>
		public CustomContext(ITypeDescriptorContext baseContext)
		{
			m_baseContext = baseContext;
		}
		#endregion

		#region ITypeDescriptorContext implementation
		/// <summary>
		/// 
		/// </summary>
		IContainer ITypeDescriptorContext.Container
		{
			get
			{
				if (m_baseContext != null)
				{
					return m_baseContext.Container;
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		object ITypeDescriptorContext.Instance
		{
			get { return this.Instance; }
		}
		/// <summary>
		/// 
		/// </summary>
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
		{
			get
			{
				if (m_baseContext != null)
				{
					return m_baseContext.PropertyDescriptor;
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void ITypeDescriptorContext.OnComponentChanged()
		{
			if (m_baseContext != null)
			{
				m_baseContext.OnComponentChanged();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ITypeDescriptorContext.OnComponentChanging()
		{
			if (m_baseContext != null)
			{
				return m_baseContext.OnComponentChanging();
			}
			return false;
		}
		#endregion

		#region IServiceProvider implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		object IServiceProvider.GetService(Type serviceType)
		{
			if (m_baseContext != null)
			{
				return m_baseContext.GetService(serviceType);
			}
			return null;
		}
		#endregion

		#region Properties
		abstract protected object Instance {get;}
		protected ITypeDescriptorContext BaseContext
		{
			get { return m_baseContext; }
		}
		#endregion

		#region Fields
		ITypeDescriptorContext m_baseContext = null;
		#endregion
	}
	#endregion
}
#endif