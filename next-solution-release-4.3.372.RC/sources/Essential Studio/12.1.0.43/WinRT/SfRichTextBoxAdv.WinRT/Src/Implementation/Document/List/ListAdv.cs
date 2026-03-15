#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class ListAdv : BaseNode
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>
        /// The owner document.
        /// </value>
        internal DocumentAdv OwnerDocument
        {
            get
            {
                return OwnerBase as DocumentAdv;
            }
        }
        /// <summary>
        /// Gets or sets the abstract list.
        /// </summary>
        /// <value>
        /// The abstract list.
        /// </value>
        internal AbstractListAdv AbstractList
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        internal string Name
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAdv"/> class.
        /// </summary>
        /// <param name="documentAdv">The documentadv.</param>
        internal ListAdv(DocumentAdv documentAdv)
            : base(documentAdv)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            AbstractList = null;
        }
        #endregion
    }
}
