using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAModel
{
    public class ImportTag
    {
        #region Declarations
        readonly Opc.Ua.NodeId nodeId;
        #endregion

        #region Constructors
        public ImportTag()
        { }

        public ImportTag(Opc.Ua.NodeId nodeId)
        {
            this.nodeId = nodeId;
        }

        public ImportTag(UFUAModel.UFUATag tag)
        {
            Name = tag.Name;
            Description = tag.Description;
            if (tag.DataType.HasValue)
                DataType = tag.DataType.Value;
            if (tag.ModelType.HasValue)
                ModelType = tag.ModelType.Value;
            Folder = tag.FolderPath;
            ArrayDimension = tag.ArrayDimension;
            if (tag.EnumStringsFlat != null)
                EnumsString = tag.EnumStringsFlat.Split('|');
            DynSettings = tag.DynamicSettings;
            //PropertyName = tag.PrototypeName;
            PrototypeModel = tag.PrototypeModel;
        }
        #endregion

        #region Properties
        public Opc.Ua.NodeId NodeId
        {
            get
            {
                return nodeId;
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        private DataType _DataType;
        public DataType DataType
        {
            get { return _DataType; }
            set
            {
                _DataType = value;
            }
        }
        private string _DynSettings;
        public string DynSettings
        {
            get { return _DynSettings; }
            set
            {
                _DynSettings = value;
            }
        }
        private ModelType _ModelType;
        public ModelType ModelType
        {
            get { return _ModelType; }
            set
            {
                _ModelType = value;
            }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
            }
        }
        private string _PrototypeModel;
        public string PrototypeModel
        {
            get { return _PrototypeModel; }
            set
            {
                _PrototypeModel = value;
            }
        }
        //private string _PropertyName;
        //public string PropertyName
        //{
        //    get { return _PropertyName; }
        //    set
        //    {
        //        _PropertyName = value;
        //    }
        //}
        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
            }
        }
 
        // FOGBUGZ 11412 and 11413
        private uint _ArrayDimension;
        public uint ArrayDimension
        {
            get { return _ArrayDimension; }
            set
            {
                _ArrayDimension = value;
            }
        }

        //FOGBUGZ 22600
        private string[] _EnumString;

        public string[] EnumsString
        {
            get { return _EnumString; }
            set
            {
                _EnumString = value;
            }
        }

        #endregion

        #region Methods
        public bool IsCompatible(ImportTag instance, List<UFUAModel.ImportPrototype> prototypes)
        {
            if ((!String.IsNullOrEmpty(Name) || !String.IsNullOrEmpty(instance.Name)) &&
                Name != instance.Name)
                return false;
            else if ((!String.IsNullOrEmpty(Folder) || !String.IsNullOrEmpty(instance.Folder)) &&
                Folder != instance.Folder)
                return false;
            else if (DataType != instance.DataType)
                return false;
            else if (ModelType != instance.ModelType)
                return false;
            else if ((!String.IsNullOrEmpty(DynSettings) || !String.IsNullOrEmpty(instance.DynSettings)) &&
                 DynSettings != instance.DynSettings)
                return false;
            else if ((!String.IsNullOrEmpty(PrototypeModel) || !String.IsNullOrEmpty(instance.PrototypeModel)) &&
                 PrototypeModel != instance.PrototypeModel)
            {
                var pFound1 = (from c in prototypes/*.AsParallel()*/
                              where c.Name == PrototypeModel
                              select c).FirstOrDefault();
                var pFound2 = (from c in prototypes/*.AsParallel()*/
                               where c.Name == instance.PrototypeModel
                               select c).FirstOrDefault();

                if (pFound1 == null || pFound2 == null)
                    return false;
                else if (!pFound1.IsCompatible(pFound2, prototypes))
                    return false;
                //else
                //    pFound1.Name = pFound2.Name;
            }
            //else if ((!String.IsNullOrEmpty(PropertyName) || !String.IsNullOrEmpty(instance.PropertyName)) &&
            //     PropertyName != instance.PropertyName)
            //    return false;
            else if (ArrayDimension != instance.ArrayDimension)
                return false;

            if (EnumsString != null && instance.EnumsString != null)
            {
                if (EnumsString.Length != instance.EnumsString.Length)
                    return false;

                for (int ii = 0; ii < EnumsString.Length; ii++)
                {
                    if (EnumsString[ii] != instance.EnumsString[ii])
                        return false;
                }
            }
            else if (EnumsString == null && instance.EnumsString != null && instance.EnumsString.Length > 0)
                return false;
            else if (EnumsString != null && EnumsString.Length > 0 && instance.EnumsString == null)
                return false;

            return true;
        }
        #endregion
    }
}
