using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
namespace TrendRealTimeSlim
{
    public class PointSettings : ObservableCollection<PointSetting>
    {
        #region Constructors
        public PointSettings()
        { }

        public PointSettings(PointSettings instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new PointSetting(item));
        }
        #endregion
    }
}
