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
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class AbstractListAdv : BaseNode
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
        /// Gets or sets the type of the list.
        /// </summary>
        /// <value>
        /// The type of the list.
        /// </value>
        internal ListType ListType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>
        /// The levels.
        /// </value>
        internal ListLevelCollectionAdv Levels
        {
            get;
            private set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAdv"/> class.
        /// </summary>
        /// <param name="documentAdv">The documentadv.</param>
        internal AbstractListAdv(DocumentAdv documentAdv)
            : base(documentAdv)
        {
            Levels = new ListLevelCollectionAdv(this);
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Adds the list level.
        /// </summary>
        /// <returns></returns>
        internal ListLevelAdv AddListLevel()
        {
            ListLevelAdv listLevelAdv = new ListLevelAdv(this);
            Levels.Add(listLevelAdv);
            return listLevelAdv;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            Levels.Dispose();
            Levels = null;
        }
        #endregion
    }
}
