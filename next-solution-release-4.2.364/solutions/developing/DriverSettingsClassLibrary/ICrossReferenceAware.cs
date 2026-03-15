using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverSettingsInterfaces
{
    /// <summary>   Interface for State CommandVariable. </summary>
    public interface ICrossReferenceAware
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a map of the State/Command variable of the driver/all channels/all stations. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Dictionary<string, string> GetTagList(ComunicationSettingsContext2 settingsContext);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Update the State/Command variable of the driver/all channels/all stations. </summary>
        ///
        /// <param name="tagMap" type="List<string>">   map with updated values (using ToXML()). </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetTagList(List<string> tagMap, ComunicationSettingsContext2 settingsContext);
    }
}
