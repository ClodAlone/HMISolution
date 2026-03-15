#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Tree View Item path.
    /// </summary>
    public class TreeViewItemPath
    {
        /// <summary>
        /// Represents the Empty Path.
        /// </summary>
        public static readonly TreeViewItemPath Empty = new TreeViewItemPath();
        private object[] path;

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public object[] FullPath
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Gets the last node.
        /// </summary>
        /// <value>The last node.</value>
        public object LastNode
        {
            get
            {
                if (path.Length > 0)
                {
                    return path[path.Length - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the first node.
        /// </summary>
        /// <value>The first node.</value>
        public object FirstNode
        {
            get
            {
                if (path.Length > 0)
                {
                    return path[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemPath"/> class.
        /// </summary>
        public TreeViewItemPath()
        {
            path = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemPath"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public TreeViewItemPath(object node)
        {
            path = new object[] { node };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemPath"/> class.
        /// </summary>
        /// <param name="oPath">The o path.</param>
        public TreeViewItemPath(object[] oPath)
        {
            path = oPath;
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            return path.Length == 0;
        }
    }

    /// <summary>
    /// Represents the MemberValueConverter Class.
    /// </summary>
    public class MemberValueConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object();
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}