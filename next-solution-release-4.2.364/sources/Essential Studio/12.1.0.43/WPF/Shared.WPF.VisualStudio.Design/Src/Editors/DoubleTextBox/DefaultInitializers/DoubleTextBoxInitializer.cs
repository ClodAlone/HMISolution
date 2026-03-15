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
using Microsoft.Windows.Design.Model;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Represents DoubleTextBoxInitializer to add default child
    /// </summary>
    internal class DoubleTextBoxInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTextBoxInitializer"/> class.
        /// </summary>
        public DoubleTextBoxInitializer()
        {
        }

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
                item.Properties["Value"].SetValue(0.0);
                item.Properties["NumberGroupSeparator"].SetValue(",");
                item.Properties["NumberDecimalSeparator"].SetValue(".");
                item.Properties["NumberDecimalDigits"].SetValue(2);
                item.Properties["Width"].SetValue(100d);
                scope.Complete();
            }
        }
    }
}
