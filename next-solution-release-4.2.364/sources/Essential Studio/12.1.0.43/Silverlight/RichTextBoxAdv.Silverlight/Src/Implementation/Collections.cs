#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class BlockCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Initializes new instance of BlockCollection class
        /// </summary>
        public BlockCollection()
        {
            
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        internal void AddBlockAtIndex(int index, T block)
        {
            if (index < 0 || index > this.Count)
                return;

            if (index == this.Count)
            {
                base.Add(block);
            }
            else if (index < this.Count)
            {
                base.Insert(index, block);
            }
        }
    }

    public class InlineCollection : ObservableCollection<Inline>
    {
        /// <summary>
        /// Initializes new instance of InlineCollection class
        /// </summary>
        public InlineCollection()
        {

        }
    }

    public class FontCollection : ObservableCollection<FontFamily>
    {
        public FontCollection()
        {

        }
    }
}
