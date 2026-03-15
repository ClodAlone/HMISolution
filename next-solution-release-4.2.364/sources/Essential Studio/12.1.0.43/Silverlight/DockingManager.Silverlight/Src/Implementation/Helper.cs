#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Helper class.
    /// </summary>
    internal static class nHelper
    {
        /// <summary>
        /// Clones the specified source.
        /// </summary>
        /// <typeparam name="T">The Generic type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The Generic Value.</returns>
        public static T Clone<T>(this T source) where T : DependencyObject
        {
            if (source != null)
            {
                Type t = source.GetType();
                if (source.GetType() == typeof(Window))
                {

                }
                T no = (T)Activator.CreateInstance(t);
                PropertyInfo[] ps = t.GetProperties();
                Type wt = t;
                while (wt.BaseType != typeof(DependencyObject))
                {
                    FieldInfo[] fi = wt.GetFields(BindingFlags.Static | BindingFlags.Public);
                    for (int i = 0; i < fi.Length; i++)
                    {
                        {
                            DependencyProperty dp = fi[i].GetValue(source) as DependencyProperty;
                            if (dp != null && fi[i].Name != "NameProperty")
                            {
                                DependencyObject obj = source.GetValue(dp) as DependencyObject;
                                if (obj != null)
                                {
                                    object o = obj.Clone();
                                    no.SetValue(dp, o);
                                }
                                else
                                {
                                    if (fi[i].Name != "CountProperty" &&
                                        fi[i].Name != "GeometryTransformProperty" &&
                                        fi[i].Name != "ActualWidthProperty" &&
                                        fi[i].Name != "ActualHeightProperty" &&
                                        fi[i].Name != "MaxWidthProperty" &&
                                        fi[i].Name != "MaxHeightProperty" &&
                                        fi[i].Name != "StyleProperty")
                                    {
                                        try
                                        {
                                            no.SetValue(dp, source.GetValue(dp));
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }

                    wt = wt.BaseType;
                }

                PropertyInfo[] pis = t.GetProperties();

                for (int i = 0; i < pis.Length; i++)
                {
                    if (pis[i].Name != "Name" && pis[i].Name != "Parent" && pis[i].CanRead && pis[i].CanWrite && !pis[i].PropertyType.IsArray && !pis[i].PropertyType.IsSubclassOf(typeof(DependencyObject)) && pis[i].GetIndexParameters().Length == 0 && pis[i].GetValue(source, null) != null && pis[i].GetValue(source, null) == (object)default(int) && pis[i].GetValue(source, null) == (object)default(double) && pis[i].GetValue(source, null) == (object)default(float))
                    {
                        pis[i].SetValue(no, pis[i].GetValue(source, null), null);
                    }
                    else if (pis[i].PropertyType.GetInterface("IList", true) != null)
                    {
                        int cnt = (int)pis[i].PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, pis[i].GetValue(source, null), null);
                        for (int c = 0; c < cnt; c++)
                        {
                            object val = pis[i].PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, pis[i].GetValue(source, null), new object[] { c });
                            object nVal = val;
                            DependencyObject v = val as DependencyObject;
                            if (v != null)
                            {
                                nVal = v.Clone();
                            }

                            if (pis[i].GetValue(no, null) == null)
                            {
                                object obj = Activator.CreateInstance(pis[i].PropertyType);
                                pis[i].SetValue(no, obj, null);
                            }

                            try
                            {
                                pis[i].PropertyType.InvokeMember("Add", BindingFlags.InvokeMethod, null, pis[i].GetValue(no, null), new object[] { nVal });
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if (pis.Length == 72)
                    {
                        try
                        {
                            if (i == 15)
                            {
                                pis[i].SetValue(no, pis[i].GetValue(source, null), null);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                return no;
            }

            else
            {
                return null;
            }
        }        
    }
}
