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

namespace Syncfusion.XlsIO.Implementation.Exceptions
{
	/// <summary>
	/// Represents exception, that occurred during parse excel structure from xml stream.
    /// </summary>
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
	[ Serializable ]
#endif
    public class XmlReadingException : ApplicationException
  {
    #region Class constants
    /// <summary>
    /// Represents default error message.
    /// </summary>
    private const string DEF_ERROR = "Some problem occured during parse.";
    #endregion

    #region Class constructors
    /// <summary>
    /// Initializes a new instance of the class with default error message.
    /// </summary>
    public XmlReadingException()
      : base( DEF_ERROR )
    {
    }
    /// <summary>
    /// Initializes a new instance of the class with a specified error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public XmlReadingException( string message )
      : base(  DEF_ERROR + ". Error message: " + message )
    {
    }
    /// <summary>
    /// Creates new instance of xml reading exception.
    /// </summary>
    /// <param name="strBlock">Represents block of xml nodes where occurred exception.</param>
    /// <param name="strDescription">Represents some description.</param>
    public XmlReadingException( string strBlock, string strDescription )
      : base( "Exception occured in " + strBlock + " of xml structure. Error message: " + strDescription )
    {}
    #endregion
  }
}
