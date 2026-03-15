using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OPCUAViewModel;
using Utilities;

namespace SpreadSheet
{
    [DataContract(Name = "SpreadSheetData")]
    [Serializable]
    public class SpreadSheetItem : Observable
    {
        #region Constructors
        public SpreadSheetItem()
        { }

        public SpreadSheetItem(SpreadSheetItem instance)
        {
            if (instance == null)
                return;


            Sheet = instance.Sheet;
            CellName = instance.CellName;
            TagName = instance.TagName;
            TagReferenceXml = instance.TagReferenceXml;
        }
        #endregion

        private int _Sheet = 0;
        public int Sheet
        {
            get { return _Sheet; }
            set
            {
                Set<int>(ref _Sheet, value, "Sheet");
            }
        }

        private string _CellName;
        public string CellName
        {
            get { return _CellName; }
            set
            {
                Set<String>(ref _CellName, value, "CellName");
            }
        }

        private string _TagName;
        public string TagName
        {
            get { return _TagName; }
            set
            {
                Set<String>(ref _TagName, value, "TagName");
            }
        }

        private string _TagReferenceXml;
        public String TagReferenceXml
        {
            get { return _TagReferenceXml; }
            set
            {
                Set<String>(ref _TagReferenceXml, value, "TagReferenceXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(_TagReferenceXml))
                        return _TagReferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                try
                {
                    TagName = value.ResolvedNodeId.ToString();
                    TagReferenceXml = value.ToXml();
                    OnPropertyChanged("TagReference");
                }
                catch (Exception)
                {
                    TagName = string.Empty;
                    TagReferenceXml = string.Empty;
                    OnPropertyChanged("TagReference");
                }
            }
        }
    }

    [CollectionDataContract(
        Name = "SpreadSheetDataList",
        ItemName = "SpreadSheetData")]
    [Serializable]
    public class SpreadSheetDataList : List<SpreadSheetItem>
    {
        #region Constructors
        public SpreadSheetDataList()
        { }

        public SpreadSheetDataList(IEnumerable<SpreadSheetItem> collection) : 
            base(collection)
        { }

        public SpreadSheetDataList(SpreadSheetDataList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new SpreadSheetItem(item));
        }
        #endregion
    }
}
