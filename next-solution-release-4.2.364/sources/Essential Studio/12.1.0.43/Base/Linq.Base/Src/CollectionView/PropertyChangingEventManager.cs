#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.ComponentModel;
    using System.Collections.Specialized;
    using System.Collections;

    public class PropertyChangingEventManager : WeakEventManager
    {
        // Fields
        private static readonly string AllListenersKey = "<All Listeners>";

        // Methods
        private PropertyChangingEventManager()
        {
        }

        public static void AddListener(INotifyPropertyChanging source, IWeakEventListener listener, string propertyName)
        {
            CurrentManager.PrivateAddListener(source, listener, propertyName);
        }

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
        {
            WeakEventManager.ListenerList empty;
            string propertyName = args.PropertyName;
            using (base.ReadLock)
            {
                HybridDictionary dictionary = (HybridDictionary)base[sender];
                if (dictionary == null)
                {
                    empty = WeakEventManager.ListenerList.Empty;
                }
                else if (!string.IsNullOrEmpty(propertyName))
                {
                    WeakEventManager.ListenerList list2 = (WeakEventManager.ListenerList)dictionary[propertyName];
                    WeakEventManager.ListenerList list3 = (WeakEventManager.ListenerList)dictionary[string.Empty];
                    if (list3 == null)
                    {
                        if (list2 != null)
                        {
                            empty = list2;
                        }
                        else
                        {
                            empty = WeakEventManager.ListenerList.Empty;
                        }
                    }
                    else if (list2 != null)
                    {
                        empty = new WeakEventManager.ListenerList(list2.Count + list3.Count);
                        int num = 0;
                        int count = list2.Count;
                        while (num < count)
                        {
                            empty.Add(list2[num]);
                            num++;
                        }
                        int num3 = 0;
                        int num4 = list3.Count;
                        while (num3 < num4)
                        {
                            empty.Add(list3[num3]);
                            num3++;
                        }
                    }
                    else
                    {
                        empty = list3;
                    }
                }
                else
                {
                    empty = (WeakEventManager.ListenerList)dictionary[AllListenersKey];
                    if (empty == null)
                    {
                        int capacity = 0;
                        foreach (DictionaryEntry entry in dictionary)
                        {
                            capacity += ((WeakEventManager.ListenerList)entry.Value).Count;
                        }
                        empty = new WeakEventManager.ListenerList(capacity);
                        foreach (DictionaryEntry entry2 in dictionary)
                        {
                            WeakEventManager.ListenerList list4 = (WeakEventManager.ListenerList)entry2.Value;
                            int num6 = 0;
                            int num7 = list4.Count;
                            while (num6 < num7)
                            {
                                empty.Add(list4[num6]);
                                num6++;
                            }
                        }
                        dictionary[AllListenersKey] = empty;
                    }
                }
                empty.BeginUse();
            }
            try
            {
                base.DeliverEventToList(sender, args, empty);
            }
            finally
            {
                empty.EndUse();
            }
        }

        private void PrivateAddListener(INotifyPropertyChanging source, IWeakEventListener listener, string propertyName)
        {
            using (base.WriteLock)
            {
                HybridDictionary dictionary = (HybridDictionary)base[source];
                if (dictionary == null)
                {
                    dictionary = new HybridDictionary(true);
                    base[source] = dictionary;
                    this.StartListening(source);
                }
                WeakEventManager.ListenerList list = (WeakEventManager.ListenerList)dictionary[propertyName];
                if (list == null)
                {
                    list = new WeakEventManager.ListenerList();
                    dictionary[propertyName] = list;
                }
                if (WeakEventManager.ListenerList.PrepareForWriting(ref list))
                {
                    dictionary[propertyName] = list;
                }
                list.Add(listener);
                dictionary.Remove(AllListenersKey);
                base.ScheduleCleanup();
            }
        }

        private void PrivateRemoveListener(INotifyPropertyChanging source, IWeakEventListener listener, string propertyName)
        {
            using (base.WriteLock)
            {
                HybridDictionary dictionary = (HybridDictionary)base[source];
                if (dictionary != null)
                {
                    WeakEventManager.ListenerList list = (WeakEventManager.ListenerList)dictionary[propertyName];
                    if (list != null)
                    {
                        if (WeakEventManager.ListenerList.PrepareForWriting(ref list))
                        {
                            dictionary[propertyName] = list;
                        }
                        list.Remove(listener);
                        if (list.IsEmpty)
                        {
                            dictionary.Remove(propertyName);
                        }
                    }
                    if (dictionary.Count == 0)
                    {
                        this.StopListening(source);
                        base.Remove(source);
                    }
                    dictionary.Remove(AllListenersKey);
                }
            }
        }

        protected override bool Purge(object source, object data, bool purgeAll)
        {
            bool flag = false;
            if (!purgeAll)
            {
                HybridDictionary dictionary = (HybridDictionary)data;
                ICollection keys = dictionary.Keys;
                string[] array = new string[keys.Count];
                keys.CopyTo(array, 0);
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    if (array[i] != AllListenersKey)
                    {
                        bool isEmpty = purgeAll || (source == null);
                        if (!isEmpty)
                        {
                            WeakEventManager.ListenerList list = (WeakEventManager.ListenerList)dictionary[array[i]];
                            if (WeakEventManager.ListenerList.PrepareForWriting(ref list))
                            {
                                dictionary[array[i]] = list;
                            }
                            if (list.Purge())
                            {
                                flag = true;
                            }
                            isEmpty = list.IsEmpty;
                        }
                        if (isEmpty)
                        {
                            dictionary.Remove(array[i]);
                        }
                    }
                }
                if (dictionary.Count == 0)
                {
                    purgeAll = true;
                    if (source != null)
                    {
                        base.Remove(source);
                    }
                }
                else if (flag)
                {
                    dictionary.Remove(AllListenersKey);
                }
            }
            if (!purgeAll)
            {
                return flag;
            }
            if (source != null)
            {
                this.StopListening(source);
            }
            return true;
        }

        public static void RemoveListener(INotifyPropertyChanging source, IWeakEventListener listener, string propertyName)
        {
            CurrentManager.PrivateRemoveListener(source, listener, propertyName);
        }

        protected override void StartListening(object source)
        {
            INotifyPropertyChanging changed = (INotifyPropertyChanging)source;
            changed.PropertyChanging += this.OnPropertyChanging;
        }

        protected override void StopListening(object source)
        {
            INotifyPropertyChanging changed = (INotifyPropertyChanging)source;
            changed.PropertyChanging -= this.OnPropertyChanging;
        }

        // Properties
        private static PropertyChangingEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(PropertyChangingEventManager);
                PropertyChangingEventManager currentManager = (PropertyChangingEventManager)WeakEventManager.GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new PropertyChangingEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }
    }
}