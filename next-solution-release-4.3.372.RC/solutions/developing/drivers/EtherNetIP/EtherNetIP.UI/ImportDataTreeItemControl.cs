using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace EtherNetIP.UI
{
    public class ImportDataTreeItemControlEthernetIP : ImportDataTreeItemControl
    {
        #region DP        
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlEthernetIP(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataEthernetIP)) || (header.GetType().BaseType == typeof(ImportDataEthernetIP)))
            {
                var tag = header as ImportDataEthernetIP;
                TagType = tag.ElemType;
            }
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Force control's properties to be updated with new values (based on user's inteface parameters (Add Station Name, ecc))
        /// </summary>
        public override void UpdateAllColumns() 
        {
            base.UpdateAllColumns();
            if (Tag as ImportDataEthernetIP != null)
                TagType = ((ImportDataEthernetIP)Tag).ElemType;
        }
        #endregion     
    }
}
