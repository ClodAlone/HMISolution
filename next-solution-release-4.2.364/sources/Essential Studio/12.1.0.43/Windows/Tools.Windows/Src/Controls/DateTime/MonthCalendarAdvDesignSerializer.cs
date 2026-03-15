#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Class Using
using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    public class MonthCalendarAdvDesignSerializer : CodeDomSerializer
    {
        #region Class Constants

        /// <summary>
        /// Name NoneButton property of the MonthCalendarAdv.
        /// </summary>
        private const string c_sNoneButtonName = "NoneButton";

        /// <summary>
        /// Name TodyaButton property of the MonthCalendarAdv.
        /// </summary>
        private const string c_sTodayButtonName = "TodayButton";

        /// <summary>
        /// Name Appearance property for NoneButton and TodyaButton of the MonthCalendarAdv.
        /// </summary>
        private const string c_sButtonAppearanceName = "Appearance";

        /// <summary>
        /// Name ButtonUseVisualStyle property for NoneButton and TodyaButton of the MonthCalendarAdv.
        /// </summary>
        private const string c_sButtonUseVisualStyleName = "UseVisualStyle";

        /// <summary>
        /// Name Office2007 color scheme property of the ButtonAdv.
        /// </summary>
        private const string c_sButtonOfficeColorScheme = "Office2007ColorScheme";

        /// <summary>
        /// Number of the comment lines.
        /// </summary>
        private const int c_iNumberCommentLines = 3;
        #endregion

        #region Class Overrides
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.
                GetSerializer(typeof(MonthCalendarAdvDesignSerializer).BaseType, typeof(CodeDomSerializer));

            return baseSerializer.Deserialize(manager, codeObject);
        }
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.
                GetSerializer(typeof(MonthCalendarAdv).BaseType, typeof(CodeDomSerializer));

            object codeObject = baseSerializer.Serialize(manager, value);
            CodeStatementCollection statements = codeObject as CodeStatementCollection;
            MonthCalendarAdv calendar = value as MonthCalendarAdv;

            if (statements != null && calendar != null)
            {
                // Organize collection fo the code statement
                CodeStatementCollection organizeStatements = new CodeStatementCollection();
                CodeAssignStatement assignStatement = null;
                CodePropertyReferenceExpression propertyExp = null;
                CodePropertyReferenceExpression targetPropertyExp = null;
                string propertyValue = String.Empty;
                int insetIndex = -1;

                for (int i = 0; i < statements.Count; i++)
                {
                    assignStatement = statements[i] as CodeAssignStatement;

                    if (assignStatement != null)
                    {
                        propertyExp = assignStatement.Left as CodePropertyReferenceExpression;

                        if (propertyExp != null)
                        {
                            targetPropertyExp = propertyExp.TargetObject as CodePropertyReferenceExpression;
                        }

                        if (propertyExp != null && targetPropertyExp != null)
                        {
                            if (targetPropertyExp.PropertyName == c_sNoneButtonName)
                            {
                                // set property for NoneButton of the MonthCalendarAdv
                                if (propertyExp.PropertyName == c_sButtonAppearanceName)
                                {
                                    // Appearance property value for NoneButton of the MonthCalendarAdv
                                    propertyValue = calendar.NoneButton.Appearance.ToString();
                                    SetFieldReferenceExpression(assignStatement, propertyValue);
                                }
                                else if (propertyExp.PropertyName == c_sButtonOfficeColorScheme)
                                {
                                    // Office2007ColorScheme property value for NoneButton of the MonthCalendarAdv
                                    propertyValue = calendar.NoneButton.Office2007ColorScheme.ToString();
                                    SetFieldReferenceExpression(assignStatement, propertyValue);
                                }
                                else if (propertyExp.PropertyName == c_sButtonUseVisualStyleName)
                                {
                                    // set UseVisualStyle property value for NoneButton of the MonthCalendarAdv
                                    SetCodePrimitiveExpression(assignStatement, calendar.NoneButton.UseVisualStyle);
                                }
                            }
                            else if (targetPropertyExp.PropertyName == c_sTodayButtonName)
                            {
                                // set property for TodayButton of the MonthCalendarAdv
                                if (propertyExp.PropertyName == c_sButtonAppearanceName)
                                {
                                    // Appearance property value for TodayButton of the MonthCalendarAdv
                                    propertyValue = calendar.TodayButton.Appearance.ToString();
                                    SetFieldReferenceExpression(assignStatement, propertyValue);
                                }
                                else if (propertyExp.PropertyName == c_sButtonOfficeColorScheme)
                                {
                                    // Office2007ColorScheme property value for TodayButton of the MonthCalendarAdv
                                    propertyValue = calendar.TodayButton.Office2007ColorScheme.ToString();
                                    SetFieldReferenceExpression(assignStatement, propertyValue);
                                }
                                else if (propertyExp.PropertyName == c_sButtonUseVisualStyleName)
                                {
                                    // set UseVisualStyle property value for TodayButton of the MonthCalendarAdv
                                    SetCodePrimitiveExpression(assignStatement, calendar.TodayButton.UseVisualStyle);
                                }
                            }

                            if ((targetPropertyExp.PropertyName == c_sNoneButtonName
                                || targetPropertyExp.PropertyName == c_sTodayButtonName)
                                && insetIndex == -1)
                            {
                                insetIndex = i - c_iNumberCommentLines;
                            }
                        }
                    }

                    if (insetIndex > 0 && assignStatement != null && (targetPropertyExp == null
                        || (targetPropertyExp != null
                        && targetPropertyExp.PropertyName != c_sNoneButtonName
                        && targetPropertyExp.PropertyName != c_sTodayButtonName)))
                    {
                        organizeStatements.Insert(insetIndex, statements[i]);
                        insetIndex++;
                    }
                    else
                    {
                        organizeStatements.Add(statements[i]);
                    }
                }

                codeObject = organizeStatements;
            }

            return codeObject;
        }
        #endregion

        #region Class Utility Methods

        /// <summary>
        /// Sets CodeFieldReferenceExpression of the CodeAssignStatement.
        /// </summary>
        /// <param name="assignStatement">Assign Statement</param>
        /// <param name="propertyValue">Property Value</param>
        private void SetFieldReferenceExpression(CodeAssignStatement assignStatement, string propertyValue)
        {
            if (assignStatement != null)
            {
                CodeFieldReferenceExpression fieldRefButton = assignStatement.Right as CodeFieldReferenceExpression;

                if (fieldRefButton != null)
                {
                    fieldRefButton.FieldName = propertyValue;
                }
            }
        }

        /// <summary>
        /// Sets CodePrimitiveExpression of the CodeAssignStatement.
        /// </summary>
        /// <param name="assignStatement">Assign Statement</param>
        /// <param name="propertyValue">Property Value</param>
        private void SetCodePrimitiveExpression(CodeAssignStatement assignStatement, object propertyValue)
        {
            if (assignStatement != null)
            {
                CodePrimitiveExpression primitive = assignStatement.Right as CodePrimitiveExpression;

                if (primitive != null)
                {
                    primitive.Value = propertyValue;
                }
            }
        }
        #endregion
    }
#endif
}
