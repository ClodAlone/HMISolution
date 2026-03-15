using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace EIB
{
    public sealed class EIBTag : Tag
    {
        #region Constructors

        public EIBTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public EIBTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return EIBDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)EIBDynSettings; }
        }

        #endregion

        #region Properties

        private readonly EIBDynTagSettings _EIBDynSettings = new EIBDynTagSettings();
        public EIBDynTagSettings EIBDynSettings
        {
            get { return _EIBDynSettings; }
        }
        #endregion
    }
}
