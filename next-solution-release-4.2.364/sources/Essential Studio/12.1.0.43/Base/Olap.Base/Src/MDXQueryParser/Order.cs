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
using Syncfusion.Olap.Reports;

namespace Syncfusion.Olap.MDXQueryParser
{
    public class Order : IKeyword
    {
        public string Name { get; set; }

        public MemberCollection members { get; set; }
        
        public Tuple tuple { get; set; }

        public string SortingOrder { get; set; }
        
        public bool Validate()
        {
            bool result=false;
            if (this.Name != null)
            {
                if ((this.tuple.Validate()) || (this.ValidateMember()))
                {
                    result = true;
                }
                if (SortingOrder != null)
                {
                    return (ValidateSortOrder() && result);
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
        private bool ValidateSortOrder()
        {   
            if ((SortingOrder == SortOrder.ASC.ToString()) || (SortingOrder == SortOrder.BASC.ToString()) || (SortingOrder == SortOrder.BDESC.ToString()) || (SortingOrder == SortOrder.DESC.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidateMember()
        {
            if (this.members != null)
            {
                foreach (IMember member in this.members)
                {
                    if (member.UniqueName.Contains("MEASURES") && member.UniqueName.Contains("."))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
