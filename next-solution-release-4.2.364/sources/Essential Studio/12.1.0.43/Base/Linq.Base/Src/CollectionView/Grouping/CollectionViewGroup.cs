#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;

    public class CollectionViewGroupRoot : CollectionViewGroup
    {
        public CollectionViewGroupRoot(Group group)
            : base(group.Key)
        {
            if (group.IsBottomLevel)
            {
                this.isbottomlevel = true;
                foreach (var record in group.Records)
                {
                    this.ProtectedItems.Add(record.Data);
                    this.ProtectedItemCount++;
                }
            }
            else
            {
                foreach (var innergroup in group.Groups)
                {
                    var collectionviewgroup = new CollectionViewGroupRoot(innergroup);
                    innergroup.CollectionViewGroup = collectionviewgroup;
                    this.ProtectedItems.Add(collectionviewgroup);
                    this.ProtectedItemCount++;
                }
            }
        }

        public CollectionViewGroupRoot(Group group, bool oneStep) :
            base(group.Key)
        {

        }

        private bool isbottomlevel = false;

        public override bool IsBottomLevel
        {
            get { return this.isbottomlevel; }
        }

        public void AddItem(object data)
        {
            if (data is Group)
            {
                var group = data as Group;
                var cvgroup = new CollectionViewGroupRoot(group, true);
                group.CollectionViewGroup = cvgroup;
                this.ProtectedItems.Add(cvgroup);
            }
            else
            {
                this.ProtectedItems.Add(((RecordEntry)data).Data);
            }
            this.ProtectedItemCount++;
        }

        public void RemoveItem(object data)
        {
            if (data is Group)
            {
                foreach (var obj in this.ProtectedItems)
                {
                    var group = data as Group;
                    var cvgroup = obj as CollectionViewGroupRoot;
                    if (cvgroup.Name == group.Key)
                    {
                        this.ProtectedItems.Remove(cvgroup);
                        break;
                    }
                }
            }
            else
            {
                this.ProtectedItems.Remove(((RecordEntry)data).Data);
            }
            this.ProtectedItemCount--;
        }

        public void InsertItem(int index, object data)
        {
            if (data is Group)
            {
                foreach (var obj in this.ProtectedItems)
                {
                    var group = data as Group;
                    var cvgroup = obj as CollectionViewGroupRoot;
                    if (cvgroup.Name == group.Key)
                    {
                        this.ProtectedItems.Insert(index, cvgroup);
                        break;
                    }
                }
            }
            else
            {
                this.ProtectedItems.Insert(index, ((RecordEntry)data).Data);
            }
            this.ProtectedItemCount++;
        }
    }
}
