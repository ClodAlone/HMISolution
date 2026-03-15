#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
   public class SparkLineSource
    {
       public List<object> List = new List<object>();
       internal bool IsNegative = false;
        /// <summary>
        /// Convert the object source item to IEnumerable item.
        /// </summary>
        public IEnumerable GetSourceList(object source,ISparkLine sparkline)
        {
          //  IEnumerable result;

            List.Clear();
            if (source != null)
            {
                if (source is DataTable)
                {
                    IEnumerable values = ((DataTable)source).Rows;
                    foreach (object o in values)
                    {
                        if (o is DataRow)
                        {
                            object x = ((DataRow)o)[0];
                            List.Add(x);
                        }
                    }
                }

                if (source is ICollection)
                {
                    IEnumerable collection = source as IEnumerable;

                    foreach (object item in collection)
                    {
                        PropertyDescriptor descriptor = null;
                        if (TypeDescriptor.GetProperties(item).Count > 0)
                        {
                            descriptor = TypeDescriptor.GetProperties(item)[0];
                            object x = descriptor.GetValue(item);
                            List.Add(x);
                        }

                        else
                        {
                            List.Add(Convert.ToDouble(item));
                        }
                    }
                }
            }


            if (List.Count > 0)
            {

                double[] tempList = new double[List.Count];

                sparkline.StartPoint = Convert.ToDouble(List[0]);
                sparkline.EndPoint = Convert.ToDouble(List[List.Count - 1]);
                int j = 0;
                double max = sparkline.StartPoint;
                double min = sparkline.EndPoint;
               sparkline.NegativeItem = new double[List.Count];
                for (int i = 0; i < List.Count; i++)
                {
                    double currentValue = Convert.ToDouble(List[i]);
                    tempList[i] = currentValue;
                    if (currentValue < 0)
                    {
                        sparkline.NegativeItem[j] = currentValue;
                        IsNegative = true;
                        j++;
                    }
                    if (tempList[i] > max)
                    {
                        max = tempList[i];
                    }
                    else if (tempList[i] < min)
                    {
                        min = tempList[i];
                    }
                }
                sparkline.HighPoint = max;
                sparkline.LowPoint = min;
            }

            return List;
        }
    }
}
