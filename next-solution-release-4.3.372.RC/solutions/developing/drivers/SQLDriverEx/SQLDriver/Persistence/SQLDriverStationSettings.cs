using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SQLDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SQLDriverStationSettings : StationSettings
    {
        #region Data
        const string defaultSQLDriverTableName = "SQLDriverTable";
        const string defaultSQLDriverColumnNameTagName = "TagName";
        const string defaultSQLDriverColumnValue = "TagValue";
        #endregion
        #region Constructors

        public SQLDriverStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SQLDriverStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _SQLDriverTableName = ((SQLDriverStationSettings)st).SQLDriverTableName;
            _SQLDriverColumnNameTagName = ((SQLDriverStationSettings)st).SQLDriverColumnNameTagName;
            _SQLDriverColumnValue = ((SQLDriverStationSettings)st).SQLDriverColumnValue;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.MaxRetriesBeforeError = 0;
            _SQLDriverTableName = defaultSQLDriverTableName;
            _SQLDriverColumnNameTagName = defaultSQLDriverColumnNameTagName;
            _SQLDriverColumnValue = defaultSQLDriverColumnValue;

        }

        #region Properties

        private string _SQLDriverTableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the SQL Table Name. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SQLDriverTableName
        {
            get { return _SQLDriverTableName; }
            set
            {
                SetPropertyValue("SQLDriverTableName", ref _SQLDriverTableName, value);
                OnPropertyChanged("SQLDriverTableName");
            }
        }

        private string _SQLDriverColumnNameTagName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the SQL Column Of The Name. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SQLDriverColumnNameTagName
        {
            get { return _SQLDriverColumnNameTagName; }
            set
            {
                SetPropertyValue("SQLDriverColumnNameTagName", ref _SQLDriverColumnNameTagName, value);
            }
        }

        private string _SQLDriverColumnValue;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the SQL Column Of The Value. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SQLDriverColumnValue
        {
            get { return _SQLDriverColumnValue; }
            set
            {
                SetPropertyValue("SQLDriverColumnValue", ref _SQLDriverColumnValue, value);
            }
        }

        private bool _SQLDriverMultiColumn;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the SQL Column Of The Value. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SQLDriverMultiColumn
        {
            get { return _SQLDriverMultiColumn; }
            set
            {
                SetPropertyValue("SQLDriverMultiColumn", ref _SQLDriverMultiColumn, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            char limiter = '|';
            if (sBase != null)
                return sBase;

            if (propertyName == "Channel")
            {
                    if ((DriverSettings != null) &&
                               ((from c in DriverSettings.StationSettings
                                 where ((c != this) &&
                                        (c as SQLDriverStationSettings).SQLDriverTableName == SQLDriverTableName &&
                                         (c as SQLDriverStationSettings).Channel == this.Channel)
                                 select c).ToList().Count > 0))
                    {
                        return Properties.Resources.DuplicatedStationInTheChannel;
                    }
                return null;
            }
            else if ((propertyName == "SQLDriverMultiColumn") || (propertyName == "SQLDriverColumnValue"))
            {
                if (SQLDriverMultiColumn && !SQLDriverColumnValue.Contains(limiter))
                {
                    return Properties.Resources.ErrorMultiColumn;
                }
                return null;
            }
            else if (propertyName == "SQLDriverTableName")
            {
                if ((DriverSettings != null) &&
                           ((from c in DriverSettings.StationSettings
                             where ((c != this) &&
                                    (c as SQLDriverStationSettings).SQLDriverTableName == SQLDriverTableName  &&
                                     (c as SQLDriverStationSettings).Channel == this.Channel)
                             select c).ToList().Count > 0))
                {
                        return Properties.Resources.DuplicatedStation;
                }
                if (String.IsNullOrEmpty(SQLDriverTableName))
                    return Properties.Resources.ErrorTableNameMissing;
            }
            if(propertyName == "SQLDriverColumnNameTagName")
            {
                if (String.IsNullOrEmpty(SQLDriverColumnNameTagName))
                    return Properties.Resources.ErrorColumnNameMissing;
            }
            if(propertyName == "SQLDriverColumnValue")
            {
                if (String.IsNullOrEmpty(SQLDriverColumnValue))
                    return Properties.Resources.ErrorColumnValueMissing;
            }
            return null;
        }

        #endregion

        #region INotifyPropertyChanged Members
        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Channel":
                    OnPropertyChanged(new PropertyChangedEventArgs("SQLDriverTableName"));
                    break;
                case "SQLDriverTableName":
                    OnPropertyChanged(new PropertyChangedEventArgs("Channel"));
                    break;
            }
        }
        #endregion
    }
}
