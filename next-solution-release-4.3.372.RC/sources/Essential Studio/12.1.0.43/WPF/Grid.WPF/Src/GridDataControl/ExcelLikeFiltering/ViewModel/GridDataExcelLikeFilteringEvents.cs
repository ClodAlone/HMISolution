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
using Syncfusion.Windows.ComponentModel;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{

    #region OKButtonClick

    public delegate void OkButtonClickEventHandler(object sender, OkButtonClikEventArgs args);

    public class OkButtonClikEventArgs : SyncfusionRoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OkButtonClikEventArgs"/> class.
        /// </summary>
         public OkButtonClikEventArgs()
        {
        }
#if !SILVERLIGHT
         /// <summary>
         /// Initializes a new instance of the <see cref="OkButtonClikEventArgs"/> class.
         /// </summary>
         /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
         /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
         public OkButtonClikEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
#else
         public OkButtonClikEventArgs(object source) :
             base(source)
         {
         }
#endif


         /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public List<FilterElement> UnCheckedElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the edited values.
        /// </summary>
        /// <value>The edited values.</value>
        public List<FilterElement> CheckedElement
        {
            get;
            internal set;
        }

    }

#endregion

    #region ClearMenuItemClick

    public delegate void ClearMenuItemClickEventHandler(object sender, SyncfusionRoutedEventArgs args); 

    #endregion

    #region PopupOpened    

    public delegate void PopupOpenedEventHandler(object sender, PopupOpenedEventArgs args);

    public class PopupOpenedEventArgs : SyncfusionRoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupOpenedEventArgs"/> class.
        /// </summary>
         public PopupOpenedEventArgs()
        {
        }
#if !SILVERLIGHT
         /// <summary>
         /// Initializes a new instance of the <see cref="PopupOpenedEventArgs"/> class.
         /// </summary>
         /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
         /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
         public PopupOpenedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
#endif

         /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public List<FilterElement> ItemsSource
        {
            get;
            set;
        }     

    }
    #endregion

    #region SortMenuItemClick

        public delegate void SortMenuItemClickEventHandler(object sender,SortMenuItemClickEventArgs args);

        public class SortMenuItemClickEventArgs : SyncfusionRoutedEventArgs
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="SortMenuItemClickEventArgs"/> class.
            /// </summary>
            public SortMenuItemClickEventArgs()
            {
            }
#if !SILVERLIGHT
            /// <summary>
            /// Initializes a new instance of the <see cref="SortMenuItemClickEventArgs"/> class.
            /// </summary>
            /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
            /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
            public SortMenuItemClickEventArgs(RoutedEvent routedEvent, object source) :
                base(routedEvent, source)
            {
            }
#else
        public SortMenuItemClickEventArgs(object source) :
            base(source)
            {
            }
#endif

            /// <summary>
            /// Gets or sets the sort string.
            /// </summary>
            /// <value>The sort string.</value>
            public String SortString
            {
                get;
                set;
            }
        }
    #endregion

    #region AdvanceFilteringOkButtonClick
        public delegate void AdvanceFilteringOkButtonClickEventHandler(object sender,AdvanceFilteringOkButtonClickEventArgs args);

        public class AdvanceFilteringOkButtonClickEventArgs : SyncfusionRoutedEventArgs
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="AdvanceFilteringOkButtonClickEventArgs"/> class.
            /// </summary>
            public AdvanceFilteringOkButtonClickEventArgs()
            {
            }
#if !SILVERLIGHT
             /// <summary>
            /// Initializes a new instance of the <see cref="AdvanceFilteringOkButtonClickEventArgs"/> class.
            /// </summary>
            /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
            /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
            public AdvanceFilteringOkButtonClickEventArgs(RoutedEvent routedEvent, object source) :
                base(routedEvent, source)
            {
            }
#else
            public AdvanceFilteringOkButtonClickEventArgs(object source) :
                base(source)
            {
            } 
#endif


            /// <summary>
            /// Gets or sets the filter value1.
            /// </summary>
            /// <value>The filter value1.</value>
            public object FilterValue1
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the filter value2.
            /// </summary>
            /// <value>The filter value2.</value>
            public object FilterValue2
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the filter type1.
            /// </summary>
            /// <value>The filter type1.</value>
            public object FilterType1
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the filter type2.
            /// </summary>
            /// <value>The filter type2.</value>
            public object FilterType2
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the type of the predicate.
            /// </summary>
            /// <value>The type of the predicate.</value>
            public object PredicateType
            {
                get;
                set;
            }
        }
    #endregion

    #region OnFilterElementPropertyChanged   
     
        public delegate void OnFilterElementPropertyChangedEventHandler(object sender, OnFilterElementPropertyChangedEventArgs args);

        public class OnFilterElementPropertyChangedEventArgs : SyncfusionRoutedEventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OnFilterElementPropertyChangedEventArgs"/> class.
            /// </summary>
            public OnFilterElementPropertyChangedEventArgs()
            {
            }
#if !SILVERLIGHT
             /// <summary>
            /// Initializes a new instance of the <see cref="OnFilterElementPropertyChangedEventArgs"/> class.
            /// </summary>
            /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
            /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
            public OnFilterElementPropertyChangedEventArgs(RoutedEvent routedEvent, object source) :
                base(routedEvent, source)
            {
            }
#else
            public OnFilterElementPropertyChangedEventArgs(object source) :
                base(source)
            {
            }
#endif


            /// <summary>
            /// Gets or sets the filter element.
            /// </summary>
            /// <value>The filter element.</value>
            public FilterElement FilterElement
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the select all checked.
            /// </summary>
            /// <value>The select all checked.</value>
            public Nullable<bool> SelectAllChecked
            {
                get;
                set;
            }
        }
    #endregion

    #region SelectAllCheckBoxChecked

    public delegate void SelectAllCheckBoxCheckedEventHandler(object sender, SelectAllCheckBoxCheckedEventArgs args);

    public class SelectAllCheckBoxCheckedEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxCheckedEventArgs"/> class.
        /// </summary>
        public SelectAllCheckBoxCheckedEventArgs()
        {
        }
#if !SILVERLIGHT
         /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxCheckedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public SelectAllCheckBoxCheckedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
#else
        public SelectAllCheckBoxCheckedEventArgs(object source) :
            base(source)
        {
        }
#endif


        /// <summary>
        /// Gets or sets the filter elements.
        /// </summary>
        /// <value>The filter elements.</value>
        public List<FilterElement> FilterElements
        {
            get;
            set;
        }
    }

    #endregion

    #region SelectAllCheckBoxUnChecked
    public delegate void SelectAllCheckBoxUnCheckedEventHandler(object sender, SelectAllCheckBoxUnCheckedEventArgs args);

    public class SelectAllCheckBoxUnCheckedEventArgs : SyncfusionRoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxUnCheckedEventArgs"/> class.
        /// </summary>
        public SelectAllCheckBoxUnCheckedEventArgs()
        {
        }
#if !SILVERLIGHT
         /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxUnCheckedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public SelectAllCheckBoxUnCheckedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
#else
        public SelectAllCheckBoxUnCheckedEventArgs(object source) :
            base(source)
        {
        }
#endif


        /// <summary>
        /// Gets or sets the filter elements.
        /// </summary>
        /// <value>The filter elements.</value>
        public List<FilterElement> FilterElements
        {
            get;
            set;
        }
    }
    #endregion

       
}
