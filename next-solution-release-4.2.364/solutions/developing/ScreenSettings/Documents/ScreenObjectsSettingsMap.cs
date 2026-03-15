using System;
using System.Collections.Generic;
using System.Text;
using ScreenSettings.Properties;
using System.IO;
using ScreenSettings.Entities;
using UFInterfaces;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using UFInterfaces.Constants;

namespace ScreenSettings
{
    [CollectionDataContract
            (Name = "ScreenObjectsSettingsMap",
            ItemName = "entry",
            KeyName = "XamlName",
            ValueName = "Settings", 
            Namespace = Namespaces.UriProgea)]
    public class ScreenObjectsSettingsMap : Dictionary<String, ScreenEntity> { }
}
