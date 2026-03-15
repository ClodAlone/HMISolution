#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    public interface ICustomXmlPart
    {
        #region Interface properties
        /// <summary>
        /// Returns the index number of the object within the collection of similar
        /// objects. Read-only Long.
        /// </summary>
        byte[] Data { get; set; }
        /// <summary>
        /// Returns or sets the name of the object. Read / write String.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Returns or sets the name of the object, in the language of the user.
        /// Read / write String for Name.
        /// </summary>
        ICustomXmlSchemaCollection Schemas { get; }
        #endregion

        #region Interface methods
        /// <summary>
        /// Deletes the object.
        /// </summary>
        ICustomXmlPart Clone();
        #endregion
    }
}
