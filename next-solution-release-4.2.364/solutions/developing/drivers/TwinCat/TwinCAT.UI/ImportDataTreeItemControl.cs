using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace TwinCAT.UI
{
    public class ImportDataTreeItemControlTwinCAT : ImportDataTreeItemControl
    {
        #region DP        
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlTwinCAT(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataTwinCAT)) || (header.GetType().BaseType == typeof(ImportDataTwinCAT)))
            {
                var tag = header as ImportDataTwinCAT;
                TagName = tag.ImportTagName;
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
            if (Tag as ImportDataTwinCAT != null)
            {
                TagName = ((ImportDataTwinCAT)Tag).ImportTagName;
            }
        }
        #endregion     
    }
}
