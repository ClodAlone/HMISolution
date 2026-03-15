#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// LineItemsCollection class is used to hold the collection of individual lineitems.
    /// </summary>
    /// <remarks>
    /// LineItemsCollection class contains the lineitem collection and each lineitem object has the line information
    /// </remarks>
    /// <seealso cref="T:Syncfusion.Windows.Edit.LineItem">Syncfusion.Windows.Edit.LineItem</seealso>
#if SyncfusionFramework4_0

    using System.ComponentModel;
    using System.Windows;

    [DesignTimeVisible(false)]
#endif
    public class LineItemsCollection : ObservableCollection<LineItem>
    {
        #region Constructor

        /// <summary>
        ///
        /// </summary>
        public LineItemsCollection()
            : base()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lines"></param>
        public LineItemsCollection(IEnumerable<string> lines)
            : base(lines.Select(str => new LineItem(str)))
        {
        }

        #endregion Constructor

        #region Delegates and Events

        /// <summary>
        /// Occurs when <see cref="LineItem"/> is Changed.
        /// </summary>
        [Description("Occurs when <see cref='LineItem'> is Changed")]
        internal event LineItemTextchanged OnLineItemTextchanged;

        /// Handles <see cref="LineItem"/> event.
        /// <summary>
        /// <param name="e">A <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> value that contains the event data.</param>
        /// </summary>
        internal delegate void LineItemTextchanged(DependencyPropertyChangedEventArgs e);

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (LineItem item in e.NewItems)
                    item.OnItemTextChanged += new LineItem.ItemTextChanged(item_OnItemTextChanged);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.NewItems != null)
            {
                foreach (LineItem item in e.NewItems)
                    item.OnItemTextChanged -= new LineItem.ItemTextChanged(item_OnItemTextChanged);
            }
        }

        private void item_OnItemTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OnLineItemTextchanged != null)
                OnLineItemTextchanged(e);
        }
    }

    /// <summary>
    /// Handles <see cref="LineItemsColletionChanged"/> event.
    /// </summary>
    /// <param name="e">A <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> value that contains the event data.</param>
    internal delegate void LineItemsColletionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e);

        #endregion Delegates and Events

    /// <summary>
    /// FormatsCollection class is used to contain a collection of Formats in
    /// LanguageBase
    /// </summary>
    /// <remarks>
    /// The FormatsCollection class contains the collection of Format information of Lexems.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.Windows.Edit.EditFormats">Syncfusion.Windows.Edit.EditFormats</seealso>
#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    public class FormatsCollection : ObservableCollection<EditFormats>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatsCollection"/> class.
        /// </summary>
        public FormatsCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatsCollection"/> class.
        /// </summary>
        /// <param name="collection">Gets the EditFormats object to collection from the reporting source</param>
        public FormatsCollection(IEnumerable<EditFormats> collection)
            : base(collection)
        {
        }
    }

    /// <summary>
    /// LexemCollection class is used to contain a collection of Formats in Lexems
    /// </summary>
    /// <remarks>
    /// The LexemCollection class contain the collection of lexems and its properties.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.Windows.Edit.Lexem">Syncfusion.Windows.Edit.Lexem</seealso>
#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    public class LexemCollection : ObservableCollection<Lexem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexemCollection"/> class.
        /// </summary>
        public LexemCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexemCollection"/> class.
        /// </summary>
        /// <param name="collection">Gets the Lexem object to collection from the reporting source</param>
        public LexemCollection(IEnumerable<Lexem> collection)
            : base(collection)
        {
        }
    }
}