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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Data;
using Syncfusion.Linq;

namespace Syncfusion.Windows.Controls.Grid
{
    public class AdvanceFilteringViewModel : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceFilteringViewModel"/> class.
        /// </summary>
        public AdvanceFilteringViewModel()
        {

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get_s the combo items.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<String> Get_ComboItems()
        {
            ObservableCollection<String> items = new ObservableCollection<string>();
            if (this.ColumnType == GridDataResourceWrapper.TextFilters)
            {
                items.Add(GridDataResourceWrapper.EqualsSmall);
                items.Add(GridDataResourceWrapper.DoesNotEquals);
                items.Add(GridDataResourceWrapper.BeginsWith);
                items.Add(GridDataResourceWrapper.EndsWithSmall);
                items.Add(GridDataResourceWrapper.ContainsSmall);

            }
            else if (this.ColumnType == GridDataResourceWrapper.NumberFilters)
            {
                items.Add(GridDataResourceWrapper.EqualsSmall);
                items.Add(GridDataResourceWrapper.DoesNotEquals);
                items.Add(GridDataResourceWrapper.IsGreaterThan);
                items.Add(GridDataResourceWrapper.IsGreaterThanOrEqualto);
                items.Add(GridDataResourceWrapper.IsLessThan);
                items.Add(GridDataResourceWrapper.IsLessThanorEqualto);
            }
            else if (this.ColumnType == GridDataResourceWrapper.DateFilters)
            {
                items.Add(GridDataResourceWrapper.EqualsSmall);
                items.Add(GridDataResourceWrapper.DoesNotEquals);
                items.Add(GridDataResourceWrapper.isbefore);
                items.Add(GridDataResourceWrapper.isbeforeorequalto);
                items.Add(GridDataResourceWrapper.isafter);
                items.Add(GridDataResourceWrapper.isafterorequalto);
            }


            return items;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the filter predicate selected item1.
        /// </summary>
        /// <value>The filter predicate selected item1.</value>
        public String FilterPredicateSelectedItem1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter predicate selected item2.
        /// </summary>
        /// <value>The filter predicate selected item2.</value>
        public String FilterPredicateSelectedItem2
        {
            get;
            set;
        }

        String filterValueSelectedItem1;

        /// <summary>
        /// Gets or sets the filter value selected item1.
        /// </summary>
        /// <value>The filter value selected item1.</value>
        public String FilterValueSelectedItem1
        {
            get
            {
                return filterValueSelectedItem1;
            }
            set
            {
                filterValueSelectedItem1 = value;
                RaisePropertyChanged("FilterValueSelectedItem1");
            }
        }

        String filterValueSelectedItem2;

        /// <summary>
        /// Gets or sets the filter value selected item2.
        /// </summary>
        /// <value>The filter value selected item2.</value>
        public String FilterValueSelectedItem2
        {
            get
            {
                return filterValueSelectedItem2;
            }
            set
            {
                filterValueSelectedItem2 = value;
                RaisePropertyChanged("FilterValueSelectedItem2");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is or checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is or checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsOrChecked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter element.
        /// </summary>
        /// <value>The filter element.</value>
        public List<FilterElement> FilterElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the combo box items.
        /// </summary>
        /// <value>The combo box items.</value>
        public ObservableCollection<String> ComboBoxItems
        {
            get
            {
                return this.Get_ComboItems();
            }

        }

        private string _columnName;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
                RaisePropertyChanged("ColumnName");
            }
        }

        public string _columnType;

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>The type of the column.</value>
        public string ColumnType
        {
            get
            {
                return _columnType;
            }
            set
            {
                _columnType = value;
                RaisePropertyChanged("ColumnType");
            }
        }

        #endregion

        #region Property Changed Event Handler

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String PropertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion
    }
}
