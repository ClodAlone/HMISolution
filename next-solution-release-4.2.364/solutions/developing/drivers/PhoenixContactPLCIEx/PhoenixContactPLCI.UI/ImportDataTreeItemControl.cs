using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace PhoenixContactPLCI.UI
{
    public class ImportDataTreeItemControlPhoenixContactPLCI : ImportDataTreeItemControl
    {        
        #region Ctor
        public ImportDataTreeItemControlPhoenixContactPLCI(object header, ImageSource icon = null) 
            : base(header, icon)
        {            
            if ((header.GetType() == typeof(ImportDataPhoenixContactPlci)) || (header.GetType().BaseType == typeof(ImportDataPhoenixContactPlci)))
            {
                var tag = header as ImportDataPhoenixContactPlci;
                TagName = tag.TagTreeName;
            }
        }
        #endregion Ctor
    }
}
