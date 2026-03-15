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

#region file using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents an error that occurred during setting an external data source 
	/// by the <see cref="AutoComplete"/> control.
	/// </summary>
	/// <remarks>
	/// This exception packages the exception that was raised by the system when 
	/// there is a problem with the data source. The original exception can be 
	/// accessed through the <see cref="Exception.InnerException"/> property.
	/// <para>
	/// The most common source for this exception to be raised is an incorrect
	/// Data source specified. Please refer to the <see cref="Syncfusion.Windows.Forms.Tools.AutoComplete.DataSource"/>
	/// property for more information.
	/// </para>
	/// </remarks>
	public class DataSourceException : Exception
	{
		/// <summary>
		/// Overloaded. Initializes a new instance of the <see cref="DataSourceException"/> class
		/// using a message and an original exception.
		/// </summary>
		/// <param name="message">The message for the exception.</param>
		/// <param name="inner">The original exception.</param>
		/// <remarks>
		/// The content of the <paramref name="message">message</paramref> parameter is intended to be
		/// understood by humans. The caller of this constructor is required to
		/// ensure that this string has been localized for the current system culture.
		/// This message takes into account the current system culture.
		/// <para>
		/// The <paramref name="inner">inner</paramref> parameter is the original exception
		/// raised by the system in the process of setting the data source.
		/// </para>
		/// <para>
		/// The following table shows the initial property values for an instance
		/// of <see cref="DataSourceException"/>.
		/// </para>
		/// <list type="table">
		/// <listheader><term>Property</term><description>Value</description></listheader>
		/// <item><term><see cref="message"/></term><description>The error message string.</description></item>
		/// <item><term><see cref="Exception.InnerException"/></term><description>The inner exception reference.</description></item>
		/// </list>
		/// </remarks>
		public DataSourceException( String message, Exception inner ) : 
			base( message, inner )
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="DataSourceException"/> class
		/// using a message.
		/// </summary>
		/// <param name="message">The message for the exception.</param>
		/// <remarks>
		/// The content of the <paramref name="message">message</paramref> parameter is intended to be
		/// understood by humans. The caller of this constructor is required to
		/// ensure that this string has been localized for the current system culture.
		/// This message takes into account the current system culture.
		/// <para>
		/// The following table shows the initial property values for an instance
		/// of <see cref="DataSourceException"/>.
		/// </para>
		/// <list type="table">
		/// <listheader><term>Property</term><description>Value</description></listheader>
		/// <item><term><see cref="message"/></term><description>The error message string.</description></item>
		/// <item><term><see cref="Exception.InnerException"/></term><description>A null reference (Nothing in Visual Basic).</description></item>
		/// </list>
		/// </remarks>
		public DataSourceException( String message ) : 
			base( message )
		{
		}
	}
}