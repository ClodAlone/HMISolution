using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBControls
{
    public class ColumnItemList : List<ColumnItem>
    {
        #region Constructors
        public ColumnItemList()
        { }

        public ColumnItemList(List<ColumnItem> collection)
            : base(collection)
        { }

        public ColumnItemList(ColumnItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new ColumnItem(item));
        }
        #endregion

        public ColumnItem this[string colName]
        {
            get
            {
                return (from ColumnItem item in this.AsParallel() where item.ColName == colName select item).FirstOrDefault();
            }
        }
    }
}
