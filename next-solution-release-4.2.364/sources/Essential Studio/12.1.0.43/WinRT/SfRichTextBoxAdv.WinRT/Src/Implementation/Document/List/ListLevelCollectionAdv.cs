#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class ListLevelCollectionAdv : BaseNode
    {
        #region Fields
        internal ObservableCollection<ListLevelAdv> InnerList = new ObservableCollection<ListLevelAdv>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="ListLevelAdv"/> with the specified level index.
        /// </summary>
        /// <value>
        /// The <see cref="ListLevelAdv"/>.
        /// </value>
        /// <param name="levelIndex">Index of the level.</param>
        /// <returns></returns>
        internal ListLevelAdv this[int levelIndex]
        {
            get
            {
                return InnerList[levelIndex];
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLevelCollectionAdv"/> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        internal ListLevelCollectionAdv(BaseNode baseNode)
            : base(baseNode)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Adds the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <exception cref="System.ArgumentNullException">level</exception>
        internal void Add(ListLevelAdv level)
        {
            if (level == null)
                throw new ArgumentNullException("level");

            level.SetOwner(OwnerBase);
            InnerList.Add(level);
        }
        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        internal int IndexOf(ListLevelAdv level)
        {
            return InnerList.IndexOf(level);
        }
        /// <summary>
        /// Removes all levels. 
        /// </summary>
        internal void Clear()
        {
            InnerList.Clear();
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            if (InnerList != null)
            {
                for (int i = 0; i < InnerList.Count; i++)
                {
                    ListLevelAdv level = InnerList[i];
                    level.Dispose();
                    InnerList.Remove(level);
                    i--;
                }
                InnerList = null;
            }
        }
        #endregion
    }
}
