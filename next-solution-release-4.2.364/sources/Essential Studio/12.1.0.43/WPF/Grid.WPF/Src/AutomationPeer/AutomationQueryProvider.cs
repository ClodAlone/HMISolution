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
using Syncfusion.Linq;
using System.Text;
using System.Windows.Automation;
using System.Linq.Expressions;
using System.Reflection;

namespace Syncfusion.Windows.Automation.Linq
{
    public class AutomationQueryProvider : QueryProviderBase
    {
        private AutomationElement automationElement;
        private TreeScope treeScope;
        public AutomationQueryProvider(AutomationElement element, TreeScope treeScope)
        {
            this.automationElement = element;
            this.treeScope = treeScope;
        }

        public override object Execute(System.Linq.Expressions.Expression expression)
        {
            using (var result = new AutomationExpressionTranslator(expression))
            {
                var condition = result.Validate();
                if (condition != null)
                {
                    //return this.automationElement.FindAll(treeScope, condition);
                    return this.automationElement.FindFirst(treeScope, condition);
                }
                return null;
            }
        }
    }

    internal class AutomationExpressionTranslator : Syncfusion.Linq.ExpressionVisitor, IDisposable
    {
        private Expression currentExpression;
        private List<Condition> conditions = null;
        public AutomationExpressionTranslator(Expression expression)
        {
            this.currentExpression = expression;
            this.conditions = new List<Condition>();
        }

        public Condition Validate()
        {
            if (this.currentExpression == null)
            {
                throw new ArgumentNullException("CurrentExpression");
            }

            this.Visit(this.currentExpression);
            if (this.conditions.Count > 0)
            {
                return new AndCondition(this.conditions.ToArray());
            }

            return null;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Equal)
            {
                var leftExp = b.Left as MemberExpression;
                if (leftExp != null)
                {
                    // left expression contains the property
                    var aProp = this.GetAutomationProperty(leftExp.Member);
                    if (aProp != null)
                    {
                        if (b.Right.NodeType == ExpressionType.MemberAccess)
                        {
                            var memberExp = b.Right as MemberExpression;
                            var controlTypeMember = typeof(ControlType).GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).FirstOrDefault(m => m.Name == memberExp.Member.Name);
                            if (controlTypeMember != null)
                            {
                                var controlType = controlTypeMember.GetValue(null);
                                var propertyCond = new PropertyCondition(aProp, controlType);
                                this.conditions.Add(propertyCond);
                            }
                        }
                        else if (b.Right.NodeType == ExpressionType.Constant)
                        {
                            // right expression contains the value
                            var constValue = b.Right as ConstantExpression;
                            if (constValue != null)
                            {
                                var propertyCond = new PropertyCondition(aProp, constValue.Value);
                                this.conditions.Add(propertyCond);
                            }
                        }
                    }
                }
            }
            else if (b.NodeType == ExpressionType.AndAlso)
            {
                this.Visit(b.Left);
                this.Visit(b.Right);
            }

            return b;
        }

        private AutomationProperty GetAutomationProperty(MemberInfo mInfo)
        {
            switch (mInfo.Name)
            {
                case "Name":
                    return AutomationElement.NameProperty;
                case "ClassName":
                    return AutomationElement.ClassNameProperty;
                case "ControlType":
                    return AutomationElement.ControlTypeProperty;
                case "AutomationId":
                    return AutomationElement.AutomationIdProperty;
            }

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.conditions.Clear();
            this.currentExpression = null;
        }

        #endregion
    }
}
