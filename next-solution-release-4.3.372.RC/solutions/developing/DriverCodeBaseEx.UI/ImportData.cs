using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAModel;
using System.Collections.ObjectModel;

namespace DriverCodeBaseEx.UI
{
    /// <summary>   Import data. </summary>
    public class ImportData
    {
        /// <summary>   The children. </summary>
        private readonly ObservableCollection<ImportData> _children = new ObservableCollection<ImportData>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the children. </summary>
        ///
        /// <value> The children. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ObservableCollection<ImportData> Children
        {
            get { return _children; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <returns>   A string that represents the current object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Name;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportData()
        {
            _Id = 0;
            _Name = string.Empty;
            _Address = string.Empty;
            _szType = string.Empty;
            _Description = string.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportData(ImportDataModel inDataModel) : this()
        {            
            dataModel = inDataModel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the data model. </summary>
        ///
        /// <value> The data model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ImportDataModel dataModel { get; set; }

        /// <summary>   The identifier. </summary>
        private int _Id;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        /// <summary>   Identifier for the parent. </summary>
        private int _parentId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int parentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        

        /// <summary>   The name. </summary>
        private string _Name;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string Name
        {
            get
            {
                return dataModel.getStationName() != "_" ?
                       dataModel.getStationName() + _Name : _Name;
            }
            set { _Name = value; }
        }
        /// <summary>   The address. </summary>
        private string _DynAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the GE address. </summary>
        ///
        /// <value> The address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DynAddress
        {
            get { return _DynAddress; }
            set { _DynAddress = value; }
        }
        /// <summary>   The address. </summary>
        private string _Address;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the address. </summary>
        ///
        /// <value> The address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        /// <summary>   true to select, false to deselect. </summary>
        private bool _Select;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the select. </summary>
        ///
        /// <value> true if select, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Select
        {
            get { return _Select; }
            set { _Select = value; }
        }

        
        /// <summary>   The type. </summary>
        private uint _ArrayDimension;
        public uint ArrayDimension
        {
            get { return _ArrayDimension; }
            set { _ArrayDimension = value; }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        private string _szType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type. </summary>
        ///
        /// <value> The size type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string szType
        {
            get { return _szType; }
            set { _szType = value; }
        }

        /// <summary>   Type of the tag. </summary>
        private DataType _TagType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type of the tag. </summary>
        ///
        /// <value> The type of the tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataType TagType
        {
            get { return _TagType; }
            set { _TagType = value; }
        }

        public virtual DataType TagTypeView
        {
            get { return _TagType; }
            set { _TagType = value; }
        }

        /// <summary>   The parent. </summary>
        private ImportData _Parent;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the parent. </summary>
        ///
        /// <value> The parent. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportData Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }
        /// <summary>   The tree level. </summary>
        private uint _TreeLevel;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the tree level. </summary>
        ///
        /// <value> The tree level. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint TreeLevel
        {
            get { return _TreeLevel; }
            set { _TreeLevel = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the name of the tree. </summary>
        ///
        /// <value> The name of the tree. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string TreeName
        {
            get { return getTreeName(); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tree name. </summary>
        ///
        /// <returns>   The tree name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private string getTreeName()
        {
            string treeName = Name;
            if (Parent != null)
            {
                treeName = Parent.getTreeName() + "." + treeName;
            }
            return treeName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the type of the used to select icon  </summary>
        ///
        /// <value> The type of the tag used to select icon </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual DataType IconTagType
        {
            get { return _TagType; }            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get - set the orginal element's import order </summary>
        ///
        /// <value> The orginal element's import order  </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private uint _ImportSortId;
        public uint ImportSortId
        {
            get { return _ImportSortId; }
            set { _ImportSortId = value; }
        }
    };
}
