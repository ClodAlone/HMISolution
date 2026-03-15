using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverSettingsInterfaces
{
    /// <summary>
    /// ICommunicationDriverWebEditing interface 
    /// </summary>
    public interface ICommunicationDriverWebEditing
    {
        object GeneralSettingsEditor { get; }
        object DynamicSettingsEditor(String dynamicSettings);
        String GetDynamicSettings(object value);
        object ImportTagsEditor { get; }
    }
}
