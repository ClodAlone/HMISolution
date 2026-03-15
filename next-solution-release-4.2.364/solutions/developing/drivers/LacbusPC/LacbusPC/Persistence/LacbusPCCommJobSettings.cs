using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace LacbusPC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class LacbusPCCommJobSettings : CommJobSettings
    {
        #region Constructors

        public LacbusPCCommJobSettings(Session session, LacbusPCCommJob job)
            : base(session, job)
        {
            _LacbusPCDatumNumber = job.LacbusPCDatumNumber;
            _LacbusPCDatumType = job.LacbusPCDatumType;
            _LacbusPCDatumFormat = job.LacbusPCDatumFormat;
            _LacbusPCCommunicationDuration = job.LacbusPCCommunicationDuration;
            _LacbusPCDatumCategory = job.LacbusPCDatumCategory;
            _LacbusPCConversionMinRawValue = job.LacbusPCConversionMinRawValue;
            _LacbusPCConversionMaxRawValue = job.LacbusPCConversionMaxRawValue;
            _LacbusPCConversionMinValue = job.LacbusPCConversionMinValue;
            _LacbusPCConversionMaxValue = job.LacbusPCConversionMaxValue;
        }

        public LacbusPCCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected LacbusPCCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _LacbusPCDatumType = DatumTypes.DigitalInput;
            _LacbusPCDatumNumber = 0;
            _LacbusPCDatumFormat = DatumFormats.Logical;
            _LacbusPCCommunicationDuration = 0;
            _LacbusPCDatumCategory = DatumCategories.Instantaneous;
            _LacbusPCConversionMinRawValue = 0.0;
            _LacbusPCConversionMaxRawValue = 0.0;
            _LacbusPCConversionMinValue = 0.0;
            _LacbusPCConversionMaxValue = 0.0;
        }

        #region Properties

        /// <summary>
        /// Datum Number
        /// </summary>
        private UInt16 _LacbusPCDatumNumber;
        public UInt16 LacbusPCDatumNumber
        {
            get
            {
                return _LacbusPCDatumNumber;
            }

            set
            {
                SetPropertyValue("LacbusPCDatumNumber", ref _LacbusPCDatumNumber, value);
            }
        }

        /// <summary>
        /// Datum Type
        /// </summary>
        private DatumTypes _LacbusPCDatumType;
        public DatumTypes LacbusPCDatumType
        {
            get { return _LacbusPCDatumType; }
            set { SetPropertyValue("LacbusPCDatumType", ref _LacbusPCDatumType, value); }
        }

        /// <summary>
        /// Datum Format
        /// </summary>
        private DatumFormats _LacbusPCDatumFormat;
        public DatumFormats LacbusPCDatumFormat
        {
            get { return _LacbusPCDatumFormat; }
            set { SetPropertyValue("LacbusPCDatumFormat", ref _LacbusPCDatumFormat, value); }
        }

        /// <summary>
        /// Communication Duration
        /// </summary>
        private UInt16 _LacbusPCCommunicationDuration;
        public UInt16 LacbusPCCommunicationDuration
        {
            get
            {
                return _LacbusPCCommunicationDuration;
            }

            set
            {
                SetPropertyValue("LacbusPCCommunicationDuration", ref _LacbusPCCommunicationDuration, value);
            }
        }

        /// <summary>
        /// Datum Category
        /// </summary>
        private DatumCategories _LacbusPCDatumCategory;
        public DatumCategories LacbusPCDatumCategory
        {
            get { return _LacbusPCDatumCategory; }
            set { SetPropertyValue("LacbusPCDatumCategory", ref _LacbusPCDatumCategory, value); }
        }

        /// <summary>
        /// Minimum Raw Value
        /// </summary>
        private double _LacbusPCConversionMinRawValue;
        public double LacbusPCConversionMinRawValue
        {
            get
            {
                return _LacbusPCConversionMinRawValue;
            }

            set
            {
                SetPropertyValue("LacbusPCConversionMinRawValue", ref _LacbusPCConversionMinRawValue, value);
            }
        }

        /// <summary>
        /// Maximum Raw Value
        /// </summary>
        private double _LacbusPCConversionMaxRawValue;
        public double LacbusPCConversionMaxRawValue
        {
            get
            {
                return _LacbusPCConversionMaxRawValue;
            }

            set
            {
                SetPropertyValue("LacbusPCConversionMaxRawValue", ref _LacbusPCConversionMaxRawValue, value);
            }
        }

        /// <summary>
        /// Minimum Converted Value
        /// </summary>
        private double _LacbusPCConversionMinValue;
        public double LacbusPCConversionMinValue
        {
            get
            {
                return _LacbusPCConversionMinValue;
            }

            set
            {
                SetPropertyValue("LacbusPCConversionMinValue", ref _LacbusPCConversionMinValue, value);
            }
        }

        /// <summary>
        /// Maximum Converted Value
        /// </summary>
        private double _LacbusPCConversionMaxValue;
        public double LacbusPCConversionMaxValue
        {
            get
            {
                return _LacbusPCConversionMaxValue;
            }

            set
            {
                SetPropertyValue("LacbusPCConversionMaxValue", ref _LacbusPCConversionMaxValue, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
