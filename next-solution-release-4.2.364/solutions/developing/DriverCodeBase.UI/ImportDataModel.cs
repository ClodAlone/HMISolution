using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DriverCodeBase.UI
{

    public enum eImportButtons
    {
        eiBtnLoadFromFile,
        eiBtnLoadFromDevice,
        eiBtnSelAll,
        eiBtnClear,
        eiBtnImport,
        eiBtnUpdateSymbol
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets station name. </summary>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate string GetStationName();
    
    /// <summary>   Import Data Model. </summary>
    public class ImportDataModel
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the children. </summary>
        ///
        /// <value> The children. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ObservableCollection<ImportData> Children { get; protected set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the get station. </summary>
        ///
        /// <value> The name of the get station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GetStationName getStationName { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="inGetStationName" type="GetStationName">   Name of the in get station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataModel(GetStationName inGetStationName)
        {
            Children = new ObservableCollection<ImportData>();
            getStationName = inGetStationName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the children of this item. </summary>
        ///
        /// <param name="parent" type="object"> The parent. </param>
        ///
        /// <returns>   The children. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public System.Collections.IEnumerable GetChildren(object parent)
        {
            if (parent == null)
                return Children;
            return (parent as ImportData).Children;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Query if 'parent' has children. </summary>
        ///
        /// <param name="parent" type="object"> The parent. </param>
        ///
        /// <returns>   true if children, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool HasChildren(object parent)
        {
            return (parent as ImportData).Children.Count > 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds import data. </summary>
        ///
        /// <returns>   An ImportData. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual ImportData addImportData()
        {
            ImportData importData = new ImportData(this);
            return importData;
        }
    }
}
