using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace OmronEthernetIP.UI
{
    public class ImportDataTreeItemControlOmronEthernetIP : ImportDataTreeItemControl
    {
        #region DP                
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlOmronEthernetIP(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            
            if ((header.GetType() == typeof(ImportDataOmronEthernetIP)) || (header.GetType().BaseType == typeof(ImportDataOmronEthernetIP)))
            {
                var tag = header as ImportDataOmronEthernetIP;
                TagType = tag.ElemTypeView;                
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
            if (Tag as ImportDataOmronEthernetIP != null)
            {
                TagType = ((ImportDataOmronEthernetIP)Tag).ElemTypeView;
            }
        }
        #endregion
    }
}
