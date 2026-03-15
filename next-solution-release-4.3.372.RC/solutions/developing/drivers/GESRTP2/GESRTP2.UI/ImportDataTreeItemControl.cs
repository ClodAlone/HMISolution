using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace GESRTP2.UI
{
    public class ImportDataTreeItemControlGESRTP2 : ImportDataTreeItemControl
    {
        #region DP        
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlGESRTP2(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataGESRTP2)) || (header.GetType().BaseType == typeof(ImportDataGESRTP2)))
            {
                var tag = header as ImportDataGESRTP2;
                TagAddress = tag.GeAddress;
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
            if (Tag as ImportDataGESRTP2 != null)
                TagAddress = ((ImportDataGESRTP2)Tag).GeAddress;
        }
        #endregion     
    }
}
