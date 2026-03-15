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
using System.Xml.Serialization;

#region *** IMacro
namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Interface that represents single lexical macro.
	/// </summary>
	public interface IMacro
	{
		/// <summary>
		/// Gets macro name.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets name of the macro surrounded with braces.
		/// </summary>
		string NameInConfig { get; }
		/// <summary>
		/// Gets or sets corresponding regular expression.
		/// </summary>
		string Regex { get; set; }
		/// <summary>
		/// Gets or sets value indicating whether macro is enabled.
		/// </summary>
		bool Enabled { get; set; }
	}
}
#endregion

#region *** Macro
namespace Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal
{
	/// <exclude/>
	/// <summary>
	/// This class intended for internal use only.
	/// </summary>
	[XmlRoot( "macro" )]
	public sealed class Macro
		: IMacro
	{
		#region Fields
		/// <summary>
		/// Macro name.
		/// </summary>
		private string m_strName = string.Empty;
		/// <summary>
		/// Corresponding regular expression.
		/// </summary>
		private string m_strRegex = string.Empty;
		/// <summary>
		/// Indicates whether macro is enabled.
		/// </summary>
		private bool m_bEnabled = true;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets Macro name.
		/// </summary>
		[XmlAttribute]
		public string Name
		{
			get
			{
				return m_strName;
			}
			set
			{
				m_strName = value;
			}
		}
		/// <summary>
		/// Gets name of the macro surrounded with braces.
		/// </summary>
		[XmlIgnore]
		public string NameInConfig
		{
			get
			{
				return "{" + Name.ToLower() + "}";
			}
		}
		/// <summary>
		/// Gets or sets corresponding regular expression.
		/// </summary>
		[XmlAttribute]
		public string Regex
		{
			get
			{
				return m_strRegex;
			}
			set
			{
				m_strRegex = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether macro is enabled.
		/// </summary>
		[XmlAttribute]
		public bool Enabled
		{
			get
			{
				return m_bEnabled;
			}
			set
			{
				m_bEnabled = value;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of the class.
		/// </summary>
		public Macro()
		{
		}
		/// <summary>
		/// Creates and initializes new instance of class.
		/// </summary>
		/// <param name="name">Macro name.</param>
		/// <param name="regEx">Corresponding regular expression.</param>
		/// <param name="bEnabled">Indicates whether macro is enabled.</param>
		public Macro( string name, string regEx, bool bEnabled )
		{
			if( null == name || name.Length == 0 )
				throw new ArgumentNullException( "name" );

			if( null == regEx || regEx.Length == 0 )
				throw new ArgumentNullException( "regEx" );

			m_strName = name.ToLower();
			m_strRegex = regEx;
			m_bEnabled = bEnabled;
		}
		#endregion

		#region IMacro Explicit Members
		/// <summary>
		/// Gets name of the macro.
		/// </summary>
		string Syncfusion.Windows.Forms.Edit.Implementation.Config.IMacro.Name
		{
			get
			{
				return m_strName;
			}
		}
		#endregion
	}
}
#endregion