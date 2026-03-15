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
    /// Represents IntegerTextBoxInitializer to add default childs
    /// </summary>
    internal class IntegerTextBoxInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerTextBoxInitializer"/> class.
        /// </summary>
        public IntegerTextBoxInitializer()
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
                item.Properties["Text"].SetValue("100");
                item.Properties["NumberGroupSeparator"].SetValue(",");
                item.Properties["MinWidth"].SetValue(100d);
                item.Properties["Width"].SetValue(100d);
                scope.Complete();
            }
        }
    }
}
