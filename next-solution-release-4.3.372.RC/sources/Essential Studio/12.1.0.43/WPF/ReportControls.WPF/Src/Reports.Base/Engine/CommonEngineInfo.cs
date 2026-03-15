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
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.RDL.Internal
{
    #region internal classes

    internal class AscendingComparerHelper : IComparer
    {
        #region IComparer Members

        int IComparer.Compare(object a, object b)
        {
            Syncfusion.RDL.DOM.DataTypes type = Syncfusion.RDL.DOM.DataTypes.String;

            double double1 = 0;
            double double2 = 0;

            if (a != null && double.TryParse(a.ToString(), out double1))
            {
                a = double1;
            }

            if (b != null && double.TryParse(b.ToString(), out double2))
            {
                b = double2;
            }

            if (a is double || b is double)
            {
                type = Syncfusion.RDL.DOM.DataTypes.Float;
            }
            else if (a is DateTime || b is DateTime)
            {
                type = Syncfusion.RDL.DOM.DataTypes.DateTime;
            }

            if (type == Syncfusion.RDL.DOM.DataTypes.Float)
            {
                double c1 = double1;
                double c2 = double2;
                return c1.CompareTo(c2);
            }
            else if (type == Syncfusion.RDL.DOM.DataTypes.DateTime)
            {
                DateTime c1 = (DateTime)a;
                DateTime c2 = (DateTime)b;

                if (c1 != null)
                {
                    return c1.CompareTo(c2);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                string c1 = (a != null) ? a.ToString() : string.Empty;
                string c2 = (b != null) ? b.ToString() : string.Empty;

                if (c1 != null)
                {
                    return c1.CompareTo(c2);
                }
                else
                {
                    return -1;
                }
            }
        }

        #endregion
    }

    internal class DescendingComparerHelper : IComparer
    {
        #region IComparer Members

        int IComparer.Compare(object a, object b)
        {
            Syncfusion.RDL.DOM.DataTypes type = Syncfusion.RDL.DOM.DataTypes.String;

            double double1 = 0;
            double double2 = 0;

            if (a != null && double.TryParse(a.ToString(), out double1))
            {
                a = double1;
            }

            if (b != null && double.TryParse(b.ToString(), out double2))
            {
                a = double2;
            }

            if (a is double || b is double)
            {
                type = Syncfusion.RDL.DOM.DataTypes.Float;
            }
            else if (a is DateTime || b is DateTime)
            {
                type = Syncfusion.RDL.DOM.DataTypes.DateTime;
            }

            if (type == Syncfusion.RDL.DOM.DataTypes.Float)
            {
                double c1 = double1;
                double c2 = double2;

                if (c1!=0.0)
                {
                    return ((double)c1).CompareTo(c2) * -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (type == Syncfusion.RDL.DOM.DataTypes.DateTime)
            {
                DateTime c1 = (DateTime)a;
                DateTime c2 = (DateTime)b;

                if (c1 != null)
                {
                    return c1.CompareTo(c2) * -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                string c1 = (a != null) ? a.ToString() : string.Empty;
                string c2 = (b != null) ? b.ToString() : string.Empty;

                if (c1 != null)
                {
                    return c1.CompareTo(c2) * -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        #endregion
    }

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
                KeysCalculationValues objX = obj as KeysCalculationValues;
                if (objX != null)
                {
                    while (c == 0 && i < Keys.Count)
                    {
                        if (Comparers != null && Comparers[i] != null)
                        {
                            c = Comparers[i].Compare(this.Keys[i], objX.Keys[i]);
                        }
                        else if (this.Keys[i] != null && objX.Keys!=null && objX.Keys.Count>i)
                        {
                            try
                            {
                                c = this.Keys[i].CompareTo(objX.Keys[i]);
                            }
                            catch { }
                        }
                        else if (objX.Keys==null || objX.Keys!=null && objX.Keys.Count <= i)
                        {
                            c = 0;
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
            string key = string.Empty;
            foreach (var item in this.Keys)
            {
                if (item != null)
                {
                    key += (key == string.Empty) ? item.ToString() : "-" + item.ToString();
                }
            }
            return key;
        }

        #endregion

        public bool Equals(KeysCalculationValues other)
        {
            KeysCalculationValues objY = other as KeysCalculationValues;
            if (this.Keys == null && objY.Keys == null)
                return true;
            else if (this.Keys == null || objY.Keys == null)
                return false;
            else if (this.Keys.Count == objY.Keys.Count)
                return this.CompareTo(objY) == 0;

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
                loc = this.BinarySearch(o);
                if (loc < 0)
                {
                    this.Insert(-loc - 1, o);
                }
            }
            return loc;
        }

        public int AddIfUniqueKey(IComparable o)
        {
            int loc = -1;
            if (o != null)
            {
                loc = this.BinarySearch(o);
                if (loc < 0)
                {
                    loc = -loc - 1;
                    this.Insert(loc, o);
                }
            }
            return loc;
        }

        public int GetKey(IComparable o)
        {
            int loc = this.BinarySearch(o);
            return loc;
        }

        public int IsUniqueKey(IComparable o)
        {
            int loc = -1;
            if (o != null)
            {
                loc = this.BinarySearch(o);
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
                loc = this.BinarySearch(o);

                if (loc < 0)
                {
                    loc = -loc - 1;
                    this.Insert(loc, o);
                }
                else
                {
                    var keys = from key in this
                               where ((KeysCalculationValues)key).Equals((KeysCalculationValues)o)
                               select key;

                    if (keys.Count() > 0)
                    {
                        loc = this.IndexOf(keys.Last()) + 1;
                    }

                    this.Insert(loc, o);
                    //this.Insert(loc , o);
                }
            }

            return loc;
        }
    }
    #endregion
}
