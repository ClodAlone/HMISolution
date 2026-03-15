#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Syncfusion.Windows
{
    using System.Windows;
    /// <summary>
    /// 
    /// </summary>
    public class ThumbButtonInfoCollection : FreezableCollection<ThumbButtonInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ThumbButtonInfoCollection();
        }

        /// <summary>
        /// A frozen empty ThumbButtonInfoCollection.
        /// </summary>
        internal static ThumbButtonInfoCollection Empty
        {
            get
            {
                if (s_empty == null)
                {
                    var collection = new ThumbButtonInfoCollection();
                    collection.Freeze();
                    s_empty = collection;
                }

                return s_empty;
            }
        }

        private static ThumbButtonInfoCollection s_empty;
    }
}
