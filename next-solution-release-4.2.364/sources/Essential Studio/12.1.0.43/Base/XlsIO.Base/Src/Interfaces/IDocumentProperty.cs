#region Header

//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//

#endregion Header

namespace Syncfusion.XlsIO
{
    using System;

    /// <summary>
    /// Represents single document property.
    /// </summary>
    public interface IDocumentProperty
    {
        #region Properties

        /// <summary>
        /// Gets / sets boolean value.
        /// </summary>
        bool Boolean
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets DateTime value.
        /// </summary>
        DateTime DateTime
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets double value.
        /// </summary>
        double Double
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets 4-bytes signed integer value.
        /// </summary>
        int Int32
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets integer value.
        /// </summary>
        int Integer
        {
            get; set;
        }

        /// <summary>
        /// Indicates whether property is built-in. Read-only.
        /// </summary>
        bool IsBuiltIn
        {
            get;
        }

        /// <summary>
        /// Returns or sets the source of a linked custom document property. Read/write String.
        /// </summary>
        string LinkSource
        {
            get; set;
        }

        /// <summary>
        /// True if the value of the custom document property is linked to the content
        /// of the container document. False if the value is static. Read/write Boolean.
        /// </summary>
        bool LinkToContent
        {
            get; set;
        }

        /// <summary>
        /// Returns property name. Read-only.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns property id for built-in properties. Read-only.
        /// </summary>
        ExcelBuiltInProperty PropertyId
        {
            get;
        }

        /// <summary>
        /// Gets / sets string value.
        /// </summary>
        string Text
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets TimeSpan value.
        /// </summary>
        TimeSpan TimeSpan
        {
            get; set;
        }

        /// <summary>
        /// Gets / sets property value.
        /// </summary>
        object Value
        {
            get; set;
        }

        #endregion Properties
    }
}