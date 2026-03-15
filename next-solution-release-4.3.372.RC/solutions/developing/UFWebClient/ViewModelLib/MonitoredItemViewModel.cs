using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#if WINDOWS_PHONE
using UFWebClient.ScreenManagerPollServiceReference;
#else
using UFWebClient.ScreenManagerServiceReference;
#endif

namespace UFWebClient.ViewModelLib
{
    public class MonitoredItemViewModel : ViewModelBase
    {
        public Object _value;
        public Object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                    return;

                _value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        public DataValue _dataValue;
        public DataValue DataValue
        {
            get
            {
                return _dataValue;
            }
            set
            {
                if (_dataValue == value)
                    return;
                _dataValue = value;
                NotifyOfPropertyChange(() => DataValue);
            }
        }

        public Range _range;
        public Range Range
        {
            get
            {
                return _range;
            }
            set
            {
                if (_range == value)
                    return;
                _range = value;
                NotifyOfPropertyChange(() => Range);
            }
        }

        public BuiltInType builtInType { get; set; }
        public int valueRank { get; set; }

        public ExpandedNodeId nodeId { get; set; }
        public bool IsChanging { get; set; }

    }
}
