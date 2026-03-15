#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !SILVERLIGHT && !WP
using System.Threading.Tasks;
#endif
namespace Syncfusion.Data
{
    public class VirtualGroupRecordEntry: GroupRecordEntry
    {
        #region Ctor

        public VirtualGroupRecordEntry(NodeEntry parent, int level):base(parent,level)
        {
            Records = new VirtualRecordEntryList(parent as Group);
        }

        #endregion

        #region overrides

        protected override void InitializeRecords()
        {
          
            
        }

        #endregion
    }

    public class VirtualRecordEntryList : RecordsEntryList
    {
        #region Members

        Group parentGroup;

        #endregion

        #region Ctor

        public VirtualRecordEntryList(Group group):base()
        {
            parentGroup = group;
        }

        #endregion

        #region

        public override RecordEntry this[int index]
        {
            get
            {
                return parentGroup.GetRecordAt(index);
            }
            set
            {
                throw new NotImplementedException("Do not set the value in Group Records");
            }
        }

        public override int Count
        {
            get
            {
                return parentGroup.GetRecordCount();
            }
        }

        public override int IndexOf(RecordEntry item)
        {
            return parentGroup.GetRecordIndex(item);
        }

        public override int IndexOfRecord(object data)
        {
            return parentGroup.GetRecordIndex((RecordEntry)data);
        }

        #endregion
    }
}
