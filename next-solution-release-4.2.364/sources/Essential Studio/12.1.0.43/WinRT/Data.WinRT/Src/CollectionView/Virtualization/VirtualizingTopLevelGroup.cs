#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Syncfusion.Data.Extensions;

#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif

#if WinRT
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif

namespace Syncfusion.Data
{
    public class VirtualizingTopLevelGroup : TopLevelGroup
    {
        #region Private Members

        int yAmountCache = 0;

        #endregion

        #region Ctor

        public VirtualizingTopLevelGroup(CollectionViewAdv collectionView)
            : base(collectionView)
        {
           
        }

        #endregion

        #region Overrides

        public override int Populate(IEnumerable<Extensions.GroupResult> groupsToPopulate)
        {
            if (groupsToPopulate != null)
            {
                this.yAmountCache = this.Populate(groupsToPopulate, this.Groups, this, this.Level + 1);
            }
            return this.yAmountCache;
        }

        protected override int Populate(IEnumerable<Extensions.GroupResult> groupsToPopulate, List<Group> groups, Group parent, int level)
        {
            var parentCounter = 0;
            var enumerator = groupsToPopulate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var groupResult = enumerator.Current;
                var group = this.CreateNewGroup(parent, groupResult, level);
                var counter = 1;
                groups.Add(group);
                if (groupResult.SubGroups == null)
                {
                    // create a record details collection
                    group.CreateDetailsForRecords(group,level);
                    // add the count of records
                    counter += groupResult.Count;
                    group.SetDirty();
                    group.SetSourceYAmountDirty();
                    group.GetYAmountCache();
                    group.GetSourceYAmountCache();
                    parentCounter += counter;
                }
                else
                {
                    group.CreateDetailsForGroups(level);
                    counter += this.Populate(groupResult.SubGroups, group.Groups, group, level + 1);
                    group.SetDirty();
                    group.SetSourceYAmountDirty();
                    group.GetYAmountCache();
                    group.GetSourceYAmountCache();
                    parentCounter += counter;
                    counter = 0;
                }
                if (IsTopLevelGroup)
                {
                    if ((this as TopLevelGroup).CollectionView.AutoExpandGroups)
                        group.IsExpanded = true;
                }
                else
                {
                    if (this.toplevelGroup != null && toplevelGroup.CollectionView.AutoExpandGroups)
                        group.IsExpanded = true;
                }
            }
            return parentCounter;
        }

        protected override Group CreateNewGroup(Group parent, Extensions.GroupResult groupResult, int level)
        {
            return new VirtualGroup(parent, level) { Key = groupResult.Key, toplevelGroup= this as TopLevelGroup , GroupSource=groupResult.Items.OfType<object>() };
        }

        public override Group CreateNewGroup(Group parent, object Key, int level)
        {
            return new VirtualGroup(parent, level) { Key = Key };
        }

        public override int Remove(object record, bool isInSourceCollectionChange)
        {
            var parent = this.GetGroup(record, this);
            var index = parent.GetRecordIndex(record);
            if (index < 0)
            {
                if (isInSourceCollectionChange)
                    this.DisplayElements.RemoveItem(this, record);
                return -1;
            }

            if (index >= 0)
            {
                var item = parent.Records[index];
                if (item != null)
                {
                    var parentGroup = item.Parent as Group;
                    var removedAt = this.DisplayElements.RemoveNode(item, true);
                    if (parentGroup != null)
                    {
                        while (parentGroup.Parent != null)
                        {
                            parentGroup = parentGroup.Parent;
                            parentGroup.SetDirty();
                        }
                        return removedAt;
                    }
                    return removedAt;
                }
            }
            return -1;
        }

        public override int IndexOf(object record)
        {
            int index = -1;
            var item = GetItem(record, this, ref index);
            return item != null ? index : -1;
        }

        protected override Type GetUnderlyingSourceType()
        {
            if (this.CollectionView.SourceCollection == null)
            {
                return (this.CollectionView as CollectionViewAdv).SourceType;
            }
            return base.GetUnderlyingSourceType();
        }

        #endregion

        #region Private Methods

        internal Group GetGroup(object item, Group baseGroup)
        {
             PropertyGroupDescription pgd = null;
             foreach (var group in baseGroup.Groups)
             {
                 int c = -1;
#if WinRT
                 if (!(group.Key is Nullable) && group.Key != null)
#else
                if (!(group.Key is DBNull) && group.Key != null)
#endif
                 {
                     pgd = this.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                     IValueConverter converter = pgd != null ? pgd.Converter : null;
                     object o = this.GetPropertyValue(group.Level - 1, item);
                     c = ((IComparable)group.Key).CompareTo((IComparable)o);
                 }
                 else
                 {
                     pgd = this.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                     IValueConverter converter = pgd != null ? pgd.Converter : null;
                     object key = this.GetPropertyValue(group.Level, item);
                     if (key == null)
                     {
                         c = 0;
                     }
                     else
                     {
                         continue;
                     }
                 }

                 if (c == 0)
                 {
                     if (group.IsBottomLevel)
                     {
                         return group;
                     }
                     return GetGroup(item, group);
                 }
             }
             return baseGroup;
        }

        private object GetPropertyValue(int level, object data)
        {
            var enumerator = this.GroupDescriptions.OfType<PropertyGroupDescription>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (level <= 0)
                {
                    break;
                }

                level--;
            }
            object value = null;
            var pgd = enumerator.Current as PropertyGroupDescription;
            {
                var provider = this.CollectionView.GetPropertyAccessProvider();
                if (provider != null)
                {
                    value = provider.GetValue(data, pgd.PropertyName);
                    if (value == null)
                    {
                        var propertyinfo = data.GetType().GetProperty(pgd.PropertyName);
                        if (propertyinfo == null)
                        {
                            var valueFunc = (this.CollectionView as CollectionViewAdv).GetFunc(pgd.PropertyName);
                            if (valueFunc != null)
                                value = valueFunc(pgd.PropertyName, data);
                        }
                    }
                }
            }
            return value;
        }

        private object GetItem(object record, Group group, ref int index)
        {
            object item = null;
            foreach (var innergroup in group.Groups)
            {
                index++;
                if (this.CheckKey(innergroup, record))
                {
                    if (innergroup.IsBottomLevel)
                    {
                        foreach (var originalrecord in (innergroup as VirtualGroup).InternalList)
                        {
                            index++;
                            if (originalrecord == record)
                            {
                                item = originalrecord;
                                break;
                            }
                        }
                    }
                    else
                    {
                        item = GetItem(record, innergroup, ref index);
                        if (item != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    index += innergroup.GetYAmountCache() - 1;
                }
                if (item != null)
                {
                    break;
                }
            }
            return item;
        }

        #endregion
    }
}
