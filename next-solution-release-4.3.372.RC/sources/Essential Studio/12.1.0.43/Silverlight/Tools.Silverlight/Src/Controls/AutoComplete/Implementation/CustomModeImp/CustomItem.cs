#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class represents items for auto-complete, where the source is Custom.
    /// </summary>
    internal class CustomItem : object, IAutocompleteItem
    {
        /// <summary>
        /// This member contains item's text. 
        /// </summary>                               
        private readonly string MText = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.CustomItem">CustomItem</see>
        /// class.
        /// </summary>
        /// <param name="inputString">This is item text.</param>
        internal CustomItem(string inputString)
        {
            this.MText = inputString;
        }

        /// <summary>
        /// Gets the value of the m_Text member.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete Name=&quot;AutoComplete&quot; Text=&quot;Auto
        /// Complete&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteControl=new AutoComplete();</para>
        /// <para>AutoCompleteControl.Text=&quot;Auto Complete&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type: string
        /// </value>
        public string Text
        {
            get
            {
                return this.MText;
            }
        }
    }
}