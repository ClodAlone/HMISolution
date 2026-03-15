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
using Syncfusion.Windows.Forms.Edit.Interfaces;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Summary description for ReferenceConfig.
	/// </summary>
	[XmlRoot( "reference" )]
	public class ReferenceConfig
		: IReferenceConfig
	{
		#region Class members
		/// <summary>
		/// ID of the configuration, reference is linked to.
		/// </summary>
		private int m_ID = -1;
		/// <summary>
		/// Referenced lexem.
		/// </summary>
		private IConfigLexem m_lexemConfig;
		/// <summary>
		/// Parent of the reference.
		/// </summary>
		private IConfigLexem m_parent;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ReferenceConfig()
		{
		}
		#endregion

		#region Class Properties
		/// <summary>
		/// GET, SET ID of the configuration, reference is linked to.
		/// </summary>
		[XmlAttribute]
		public int RefID
		{
			get
			{
				return m_ID;
			}
			set
			{
				if( m_ID != value )
				{
					m_ID = value;
					m_lexemConfig = null;
				}

			}
		}
		/// <summary>
		/// Referenced lexem.
		/// </summary>
		[XmlIgnore]
		public IConfigLexem ReferencedLexem
		{
			get
			{
				if( m_lexemConfig == null )
				{
					IConfigLexem parent = m_parent;

					while( parent != null && !( parent is IConfigLanguage ) )
					{
						parent = parent.ParentConfig;
					}

					if( parent == null )
						return null;

					m_lexemConfig = ( parent as IConfigLanguage ).FindConfig( m_ID );
				}

				return m_lexemConfig;
			}
		}

		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Sets parent of the reference.
		/// </summary>
		/// <param name="parent">Parent to be set.</param>
		internal void SetParent( IConfigLexem parent )
		{
			m_parent = parent;
			m_lexemConfig = null;
		}
		#endregion
	}
}
