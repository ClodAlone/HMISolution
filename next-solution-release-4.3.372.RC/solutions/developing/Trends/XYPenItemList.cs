using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using UFUAEditor.ComponentService;
using Utilities;

namespace Trends
{
    [SvgValueConverter(typeof(ConvertXYPenItemList))]
    public class XYPenItemList : ObservableCollection<XYPenItem>
    {
        #region Constructors
        public XYPenItemList() 
        { }

        public XYPenItemList(XYPenItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new XYPenItem(item));
        }
        #endregion

        public List<object> ToDictionary(IUFUAEditorManager UFUAEditor, DocumentManager.ComponentService.IDocument document)
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(UFUAEditor, document, list)));
            return res;
        }
    }

    internal class ConvertXYPenItemList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            DocumentManager.ComponentService.IDocument iDocument = document as DocumentManager.ComponentService.IDocument;
            IUFUAEditorManager UFUAEditor = iDocument?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var entity = value as XYPenItemList;
            if (entity == null)
                return (new XYPenItemList()).ToDictionary(UFUAEditor, iDocument);

            return entity.ToDictionary(UFUAEditor, iDocument);
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<object>);
            }
        }
    }
}
