using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace CoDeSys.UI
{
    public class ImportDataTreeItemControlCoDeSys : ImportDataTreeItemControl
    {
        #region DP        
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlCoDeSys(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataCoDeSys)) || (header.GetType().BaseType == typeof(ImportDataCoDeSys)))
            {
                var tag = header as ImportDataCoDeSys;
                TagName = tag.TreeName;                
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
            if (Tag as ImportDataCoDeSys != null)
            {
                TagName = ((ImportDataCoDeSys)Tag).TreeName;                
            }
        }
        #endregion     
    }
}
