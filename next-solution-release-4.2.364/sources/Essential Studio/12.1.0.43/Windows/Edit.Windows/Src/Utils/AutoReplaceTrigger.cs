#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Stores info about one autoreplace trigger.
	/// </summary>
	public class AutoReplaceTrigger
	{
		#region Fields
		/// <summary>
		/// Text that should be deleted.
		/// </summary>
		private string m_strFrom;
		/// <summary>
		/// Text that should be inserted.
		/// </summary>
		private string m_strTo;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets text that should be deleted.
		/// </summary>
		[ XmlAttribute ]
		public string From
		{
			get
			{
				return m_strFrom;
			}
			set
			{
				m_strFrom = value;
			}
		}
		/// <summary>
		/// Gets or sets text that should be inserted.
		/// </summary>
		[ XmlAttribute ]
		public string To
		{
			get
			{
				return m_strTo;
			}
			set
			{
				m_strTo = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of AutoReplaceTrigger.
		/// </summary>
		public AutoReplaceTrigger()
		{
		}
		/// <summary>
		/// Creates and initializes new instance of AutoReplaceTrigger.
		/// </summary>
		/// <param name="strFrom">Text that should be deleted.</param>
		/// <param name="strTo">Text that should be inserted.</param>
		public AutoReplaceTrigger( string strFrom, string strTo )
		{
			m_strFrom = strFrom;
			m_strTo = strTo;
		}
		#endregion
	}
}
