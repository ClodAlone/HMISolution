////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DriverSerialExampleCommJobSettings.cs
//
// summary:	Implements the driver serial example communications job settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
namespace DriverSerialExample
{
    /// <summary>   Settings for the protocol's task(DriverSerialExampleCommJob). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverSerialExampleCommJobSettings : CommJobSettings, IDataErrorInfo
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from DriverSerialExampleCommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleCommJobSettings(Session session, DriverSerialExampleCommJob job)
            : base(session, job)
        {
            _FunctionCode = job.FunctionCode;
            _StartAddress = job.StartAddress;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from DriverSerialExampleCommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DriverSerialExampleCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _FunctionCode = FunctionCodes.MultipleRegisters;
            _StartAddress = 0;
        }

        #region Properties

        /// <summary>   The function code. </summary>
        private FunctionCodes _FunctionCode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Memory area Property.   </summary>
        ///
        /// <value> The function code. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public FunctionCodes FunctionCode
        {
            get
            {
                return _FunctionCode;
            }
            set
            {
                SetPropertyValue("FunctionCode", ref _FunctionCode, value);
            }
        }

        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _StartAddress, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var value = GetType().GetProperty(propertyName).GetValue(this, null);

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverSerialExampleCommJobSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
            }

            return null;
        }

        #endregion
    }
}
