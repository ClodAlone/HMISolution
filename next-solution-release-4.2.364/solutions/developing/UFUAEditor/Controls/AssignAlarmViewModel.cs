using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
#if !NET_STANDARD
using System.Windows.Data;
#endif

namespace UFUAEditor.Controls
{
    public enum AssignAlarmType
    {
        Single,
        AnyTagBit,
        AnyTagElement
    }

#if !NET_STANDARD
    [ValueConversion(typeof(Boolean), typeof(Double))]
    internal class BooleanToOpacityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Double))
                throw new InvalidOperationException("The target must be a Double");

            if (value is Boolean)
            {
                var enabled = (Boolean)value;
                if (!enabled)
                    return 0.5;
            }

            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class AssignAlarmViewModel : INotifyPropertyChanged
    {
        #region Declarations
        readonly AlarmPrototypeList alarmPrototypeList;
        #endregion 

        #region Constructors

        public AssignAlarmViewModel(AlarmPrototypeList prototypecontrol)
        {
            alarmPrototypeList = prototypecontrol;
            alarmPrototypeList.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
            alarmPrototypeList.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);

            alarmPrototypeList.treeListControl.SelectedItemChanged += (s, e) => { OnPropertyChanged("IsSingleAlarmDefinitionSelected"); };
        }

        #endregion

        #region Properties

        public AlarmPrototypeList AlarmPrototypeList
        { 
            get
            {
                return alarmPrototypeList;
            }
            /*
            private set
            {
                if (alarmPrototypeList == value)
                    return;

                alarmPrototypeList = value;
                OnPropertyChanged("AlarmPrototypeList");
            }
            */
        }

        public bool IsSingleAlarmDefinitionSelected
        {
            get
            {
                var selAlrs = alarmPrototypeList.GetSelectedAlarmDefinitions();
                return selAlrs != null && selAlrs.Count() == 1;
            }
        }

        public IEnumerable<UFUAModel.UFUAAlarmDefinition> SelectedAlarmDefinitions
        {
            get
            {
                return alarmPrototypeList.GetSelectedAlarmDefinitions();
            }
        }

        bool setAlarmText;
        public bool SetAlarmText
        {
            get
            {
                return setAlarmText;
            }
            set
            {
                if (setAlarmText == value)
                    return;

                setAlarmText = value;
                OnPropertyChanged("SetAlarmText");
            }
        }

        string prefixAlarmText;
        public string PrefixAlarmText
        {
            get
            {
                return prefixAlarmText;
            }
            set
            {
                if (prefixAlarmText == value)
                    return;

                prefixAlarmText = value;
                OnPropertyChanged("PrefixAlarmText");
            }
        }

        AssignAlarmType assignAlarmType = AssignAlarmType.Single;
        public AssignAlarmType AssignAlarmType
        {
            get
            {
                return assignAlarmType;
            }
            set
            {
                if (assignAlarmType == value)
                    return;

                assignAlarmType = value;
                OnPropertyChanged("AssignAlarmType");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
#endif
}
