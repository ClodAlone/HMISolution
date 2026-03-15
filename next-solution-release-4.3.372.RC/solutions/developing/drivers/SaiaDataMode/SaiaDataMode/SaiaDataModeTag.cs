using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverBaseInterfaces;

namespace SaiaDataMode
{


    public sealed class SaiaDataModeTag : Tag
    {
        #region Constructors

        public SaiaDataModeTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public SaiaDataModeTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return SaiaDataModeDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)SaiaDataModeDynSettings; }
        }

        #endregion

        #region Properties

        private readonly SaiaDataModeDynTagSettings _SaiaDataModeDynSettings = new SaiaDataModeDynTagSettings();
        public SaiaDataModeDynTagSettings SaiaDataModeDynSettings
        {
            get { return _SaiaDataModeDynSettings; }
        }

        #endregion        

    }
}
