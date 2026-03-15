using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Utilities;
using UFUAEditor.ComponentService;

namespace Trends
{
    [SvgValueConverter(typeof(ConvertPenItemList))]
    public class PenItemList : ObservableCollection<PenItem>
    {
        #region Constructors
        public PenItemList() 
        { }

        public PenItemList(PenItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new PenItem(item));
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

    internal class ConvertPenItemList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            DocumentManager.ComponentService.IDocument iDocument = document as DocumentManager.ComponentService.IDocument;
            IUFUAEditorManager UFUAEditor = iDocument?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var entity = value as PenItemList;
            if (entity == null)
                return (new PenItemList()).ToDictionary(UFUAEditor, iDocument);

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
