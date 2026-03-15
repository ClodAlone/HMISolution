#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Represents Initializer to add default childs
    /// </summary>
    internal class RangeSliderInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSliderInitializer"/> class.
        /// </summary>
        public RangeSliderInitializer() { }


        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.</exception>
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                item.Properties["Maximum"].SetValue(10d);
                item.Properties["TickFrequency"].SetValue(1d);
                item.Properties["Background"].SetValue(Brushes.White);
                item.Properties["Foreground"].SetValue(Brushes.Black);
                item.Properties["TickPlacement"].SetValue(Tickplacement.Both);
                scope.Complete();
            }
        }
    }
}
