using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverBaseInterfaces;

namespace NaisFp
{


    public sealed class NaisFpTag : Tag
    {
        #region Constructors

        public NaisFpTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public NaisFpTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return NaisFpDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)NaisFpDynSettings; }
        }

        #endregion

        #region Properties

        private readonly NaisFpDynTagSettings _NaisFpDynSettings = new NaisFpDynTagSettings();
        public NaisFpDynTagSettings NaisFpDynSettings
        {
            get { return _NaisFpDynSettings; }
        }

        #endregion        

    }
}
