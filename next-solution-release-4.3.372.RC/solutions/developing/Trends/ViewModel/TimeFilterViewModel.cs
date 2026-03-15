using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelpers;
using WPFUtilities;

namespace Trends
{
    /// <summary>
    /// ViewModel class for the time filter control
    /// </summary>
    public class TimeFilterViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Represents an item of the select box time filter
        /// </summary>
        public class TimeFilterItem : INotifyPropertyChanged
        {
            string content;
            /// <summary>
            /// Item's visible and translatable string
            /// </summary>
            public string Content
            {
                get
                {
                    return content;
                }
                set
                {
                    if (value != content)
                    {
                        content = value;
                        OnPropertyChanged(nameof(Content));
                    }
                }
            }
            /// <summary>
            /// Item's corresponding DateSpan item, useful for subsequent calculations
            /// </summary>
            public DateSpan Tag { get; set; }

            /// <summary>
            /// Constructor for the time filter selectbox item
            /// </summary>
            /// <param name="content">Item's visible and translatable string</param>
            /// <param name="tag">Item's corresponding DateSpan item, useful for subsequent calculations</param>
            public TimeFilterItem(string content, DateSpan tag)
            {
                Content = content;
                Tag = tag;
            }

            #region Methods
            public override string ToString()
            {
                return Content;
            }
            #endregion

            #region INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;
            /// <summary>
            /// Invokes the PropertyChanged event triggering the bindings update
            /// </summary>
            /// <param name="propName"></param>
            void OnPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
            #endregion
        }
        /// <summary>
        /// Available time filters collection that populates the control
        /// </summary>
        public ObservableCollection<TimeFilterItem> TimeFilterItems { get; set; } = new ObservableCollection<TimeFilterItem>()
            {
                new TimeFilterItem(Properties.Resources.Minute, DateSpan.Minute),
                new TimeFilterItem(Properties.Resources.Hour, DateSpan.Hour),
                new TimeFilterItem(Properties.Resources.Day, DateSpan.Day),
                new TimeFilterItem(Properties.Resources.Week, DateSpan.Week),
                new TimeFilterItem(Properties.Resources.Month, DateSpan.Month),
                new TimeFilterItem(Properties.Resources.Year, DateSpan.Year)
            };
        TimeFilterItem selectedItem;
        /// <summary>
        /// Represents the currently selected item of the source collection
        /// </summary>
        public TimeFilterItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        #region ctor
        public TimeFilterViewModel() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Translates the items' content in the current runtime culture
        /// </summary>
        public void TranslateContents(string stringPlaceolder, IDictionary<string, string> stringlist)
        {
            foreach (var item in TimeFilterItems)
            {
                item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_{item.Tag}", stringlist, Properties.Resources.ResourceManager.GetObject($"{item.Tag}").ToString());
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Invokes the PropertyChanged event triggering the bindings update
        /// </summary>
        /// <param name="propName"></param>
        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
