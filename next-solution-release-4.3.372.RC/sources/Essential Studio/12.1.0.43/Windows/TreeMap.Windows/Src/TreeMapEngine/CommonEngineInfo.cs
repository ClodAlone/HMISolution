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
using System.Collections;

namespace Syncfusion.Windows.Forms.TreeMap
{
    #region internal classes

    internal class KeysCalculationValues : IComparable, IEquatable<KeysCalculationValues>
    {
        public List<IComparable> Keys { get; set; }
        public List<SummaryBase> Values { get; set; }
        public IComparer[] Comparers { get; set; }
        public int RecordKey = -1;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            int c = 0;
            if (Keys != null)
            {
                int i = 0;
                var objX = obj as KeysCalculationValues;
                if (objX != null)
                {
                    while (c == 0 && i < Keys.Count)
                    {
                        if (Comparers != null && Comparers[i] != null)
                        {
                            c = Comparers[i].Compare(Keys[i], objX.Keys[i]);
                        }
                        else if (Keys[i] != null)
                        {
                            c = Keys[i].CompareTo(objX.Keys[i]);
                        }
                        else
                        {
                            c = objX.Keys[i] == null ? 0 : 1;
                        }
                        i++;
                    }
                }
            }
            return c;
        }

        public override string ToString()
        {
            return Keys.Aggregate(string.Empty, (current, item) => current + ((current == string.Empty) ? item.ToString() : "-" + item.ToString()));
        }

        #endregion

        public bool Equals(KeysCalculationValues other)
        {
            var objY = other;
            if (Keys == null && objY.Keys == null)
                return true;
            if (Keys == null || objY.Keys == null)
                return false;
            if (Keys.Count == objY.Keys.Count)
                return CompareTo(objY) == 0;

            return false;
        }
    }

    #endregion

    #region BinaryList class

    internal class BinaryList : List<IComparable>
    {
        public int AddIfUnique(IComparable o)
        {
            int loc = -1;
            if (o != null)
            {
                loc = BinarySearch(o);
                if (loc < 0)
                {
                    Insert(-loc - 1, o);
                }
            }
            return loc;
        }

        public int IsUniqueKey(IComparable o)
        {
            int loc = -1;
            if (o != null)
            {
                loc = BinarySearch(o);
                if (loc < 0)
                {
                    return -1;
                }
            }
            return loc;
        }

        public int AddKeyValue(IComparable o)
        {
            int loc = -1;

            if (o != null)
            {
                loc = BinarySearch(o);

                if (loc < 0)
                {
                    loc = -loc - 1;
                    Insert(loc, o);
                }
                else
                {
                    var keys = from key in this
                               where ((KeysCalculationValues)key).Equals((KeysCalculationValues)o)
                               select key;

                    List<IComparable> comparables = keys as List<IComparable> ?? keys.ToList();
                    if (comparables.Any())
                    {
                        loc = IndexOf(comparables.Last()) + 1;
                    }

                    Insert(loc, o);
                }
            }

            return loc;
        }
    }
    #endregion
}
