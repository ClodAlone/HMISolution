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
using System.Windows.Automation;
using Syncfusion.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;

namespace Syncfusion.Windows.Automation.Linq
{
    public static class AutomationQueryableExtensions
    {
        public static AutomationQueryable AsQueryable(this AutomationElement element)
        {
            return element.AsQueryable(TreeScope.Children);
        }

        public static AutomationQueryable AsQueryable(this AutomationElement element, TreeScope treeScope)
        {
            var provider = new AutomationQueryProvider(element, treeScope);
            return new AutomationQueryable(provider);
        }

        //public static IEnumerable<AutomationElement> Where(this AutomationQueryable source, Expression<Func<AutomationTypeHolder, bool>> predicate)
        //{
        //    if (source == null)
        //    {
        //        throw new ArgumentNullException("source");
        //    }
        //    if (predicate == null)
        //    {
        //        throw new ArgumentNullException("predicate");
        //    }

        //    var result = (AutomationElementCollection)source.Provider.Execute(predicate);
        //    foreach (AutomationElement el in result)
        //    {
        //        yield return el;
        //    }
        //}

        public static AutomationElement First(this AutomationQueryable source, Expression<Func<AutomationTypeHolder, bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var result = (AutomationElement)source.Provider.Execute(predicate);
            return result;
            //var result = (IEnumerable)source.Provider.Execute(predicate);
            //AutomationElement el = null;
            //var enumerator = result.GetEnumerator();
            //if (enumerator.MoveNext())
            //{
            //    el = (AutomationElement)enumerator.Current;
            //}
            //return el;
        }

        public static AutomationElement FirstOrDefault(this AutomationQueryable source, Expression<Func<AutomationTypeHolder, bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var result = source.First(predicate);
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public class AutomationQueryable : QueryBase<AutomationTypeHolder>
    {
        public AutomationQueryable(QueryProviderBase provider)
            : base(provider)
        {
        }
    }

    public class AutomationTypeHolder
    {
        public AutomationTypeHolder()
        {
        }

        public AutomationTypeHolder(string className, string name, ControlType controlType, string automationId)
        {
            this.ClassName = className;
            this.Name = name;
            this.ControlType = controlType;
            this.AutomationId = automationId;
        }

        public string ClassName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public ControlType ControlType
        {
            get;
            set;
        }

        public string AutomationId
        {
            get;
            set;
        }
    }
}
