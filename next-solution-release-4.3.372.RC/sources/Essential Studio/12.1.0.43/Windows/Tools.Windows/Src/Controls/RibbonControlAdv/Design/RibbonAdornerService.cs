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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// 
	/// </summary>
	class RibbonAdornerService : IDisposable
	{
		#region Constructors
		public RibbonAdornerService(IDesignerHost designerHost)
		{
			m_designerHost = designerHost;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public Adorner Adorner
		{
			get
			{
				if (m_adorner == null)
				{
					m_adorner = new Adorner();
				}

				BehaviorService bs = this.BehaviorService;
				if (bs != null)
				{
					IList adorners = bs.Adorners;
					if (adorners.Count > 0 && adorners.IndexOf(m_adorner) < 0)
					{
						adorners.Insert(1, m_adorner);
					}
				}
				
				return m_adorner;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		BehaviorService BehaviorService
		{
			get
			{
				if (m_designerHost != null)
				{
					return m_designerHost.GetService(typeof(BehaviorService)) as BehaviorService;
				}
				return null;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public static RibbonAdornerService Get(ISite site)
		{
			if (site != null)
			{
				IServiceContainer svcContainer = site.GetService(typeof(IServiceContainer)) as IServiceContainer;
				if (svcContainer != null)
				{
					RibbonAdornerService raSvc = svcContainer.GetService(typeof(RibbonAdornerService)) as RibbonAdornerService;
					if (raSvc == null)
					{
						IDesignerHost designerHost = svcContainer.GetService(typeof(IDesignerHost)) as IDesignerHost;
						if (designerHost != null)
						{
							raSvc = new RibbonAdornerService(designerHost);
							svcContainer.AddService(typeof(RibbonAdornerService), raSvc);
						}
					}
					return raSvc;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="glyphs"></param>
		public void AddGlyphs(GlyphCollection glyphs)
		{
			this.Adorner.Glyphs.AddRange(glyphs);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="glyphs"></param>
		public void RemoveGlyphs(GlyphCollection glyphs)
		{
			GlyphCollection glyphCollection = Adorner.Glyphs;

			foreach (Glyph g in glyphs)
			{
				glyphCollection.Remove(g);
			}
		}
		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (m_adorner != null)
			{
				BehaviorService bs = this.BehaviorService;
				if (bs != null)
				{
					bs.Adorners.Remove(m_adorner);
				}

				m_adorner.Glyphs.Clear();
				m_adorner = null;
			}
		}

		#endregion

		#region Fields
		IDesignerHost m_designerHost;
		Adorner m_adorner;
		#endregion
	}
}
#endif
