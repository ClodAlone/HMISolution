using System;
using System.Runtime.Serialization;

namespace UFUAModel.Helpers
{
    [DataContract]
    public class EngineeringUnitData
    {
        #region Constructors
        public EngineeringUnitData()
        { }

        public EngineeringUnitData(UFUAEngineeringUnit eu)
        {
            unitName = eu.UnitName;
            euRange = new Opc.Ua.Range(eu.EURangeHigh.Value, eu.EURangeLow.Value);
            instrumentRange = new Opc.Ua.Range(eu.InstrumentRangeHigh.Value, eu.InstrumentRangeLow.Value);
        }
        #endregion

        #region Public Properties
        public string UnitName
        {
            get
            {
                return unitName;
            }
        }

        public Opc.Ua.Range EURange
        {
            get
            {
                return euRange;
            }
        }

        public Opc.Ua.Range InstrumentRange
        {
            get
            {
                return instrumentRange;
            }
        }
        #endregion

        #region Provate Members
        [DataMember]
        string unitName;
        [DataMember]
        Opc.Ua.Range euRange;
        [DataMember]
        Opc.Ua.Range instrumentRange;
        #endregion
    }
}
