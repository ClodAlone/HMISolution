using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;

namespace Fatek
{
    public sealed class FatekTag : Tag
    {
        #region Constructors

        public FatekTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            TotalJobSize = 0;
        }

        public FatekTag(TagDefinition tag)
            : base(tag)
        {
            TotalJobSize = 0;
        }

        #endregion

        #region Override Properties/Functions
        public override bool BuildDynamicSettings(String dynamicSettings)
        {            
            return FatekDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)FatekDynSettings; }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        readonly FatekDynTagSettings _FatekDynSettings = new FatekDynTagSettings();
        public FatekDynTagSettings FatekDynSettings
        {
            get { return _FatekDynSettings; }
        }

        public uint TotalJobSize { get; set; }

        #endregion        
    }
}
