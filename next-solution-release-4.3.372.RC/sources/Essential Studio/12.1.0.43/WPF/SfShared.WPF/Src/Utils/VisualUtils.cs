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
using System.Windows.Media;

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Represents a class for the visual utils
    /// </summary>
    public class VisualUtils
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.VisualUtils"/> class.
        /// </summary>
        public VisualUtils() { }

        /// <summary>
        /// Gets the decendant
        /// </summary>
        /// <param name="startingFrom"></param>
        /// <param name="typeDescendant"></param>
        /// <returns></returns>
        public static Visual FindDescendant(Visual startingFrom, Type typeDescendant)
        {
            Visual visual = null;
            bool result = false;
            int iCount = VisualTreeHelper.GetChildrenCount(startingFrom);

            for (int i = 0; i < iCount; ++i)
            {
                Visual child = VisualTreeHelper.GetChild(startingFrom, i) as Visual;

                if (typeDescendant.IsInstanceOfType(child))
                {
                    visual = child;
                    result = true;
                }

                if (!result)
                {
                    if (child != null)
                    {
                        visual = FindDescendant(child, typeDescendant);

                        if (visual != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return visual;
        }
    }
}
