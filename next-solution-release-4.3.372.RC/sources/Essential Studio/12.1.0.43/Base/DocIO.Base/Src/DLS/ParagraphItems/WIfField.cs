#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region Fiel using directives
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WIfField.
    /// </summary>
    public class WIfField : WField
    {
        #region Fields
        private string m_expression1;
        private string m_expression2;
        private string m_operator;
        private string m_trueText;
        private string m_falseText;

        private PseudoMergeField m_expField1;
        private PseudoMergeField m_expField2;
        private List<Entity> m_trueTextField;
        private List<Entity> m_falseTextField;

        private Regex m_operatorExp;
        private Regex m_fieldExp;
        private List<PseudoMergeField> m_mergeFields;
        private WFieldMark nestedFieldEnd = null;
        //private WTextBody m_fieldTextBody;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Expression1 of IF field.
        /// </summary>
        /// <value>The expression1.</value>
        internal PseudoMergeField Expression1
        {
            get
            {
                if (m_expField1 == null)
                {
                    CheckExpStrings();
                    m_expField1 = new PseudoMergeField(m_expression1);
                }
                return m_expField1;
            }
        }
        /// <summary>
        /// Gets the Expression2 of IF field.
        /// </summary>
        /// <value>The expression1.</value>
        internal PseudoMergeField Expression2
        {
            get
            {
                if (m_expField2 == null)
                {
                    CheckExpStrings();
                    m_expField2 = new PseudoMergeField(m_expression2);
                }
                return m_expField2;
            }
        }
        /// <summary>
        /// Gets the TrueText field.
        /// </summary>
        /// <value>The true text field.</value>
        internal List<Entity> TrueTextField
        {
            get
            {
                if (m_trueTextField == null)
                {
                    m_trueTextField = new List<Entity>();
                }
                return m_trueTextField;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<Entity> FalseTextField
        {
            get
            {
                if (m_falseTextField == null)
                {                   
                    m_falseTextField = new List<Entity>();
                }
                return m_falseTextField;
            }
        }
        /// <summary>
        /// Gets the operator expression.
        /// </summary>
        /// <value>The operator expression.</value>
        private Regex OperatorExpression
        {
            get
            {
                if (m_operatorExp == null)
                {
                    m_operatorExp = new Regex("([<>=]+)");
                }
                return m_operatorExp;
            }
        }
        /// <summary>
        /// Gets the field expression.
        /// </summary>
        /// <value>The field expression.</value>
        private Regex FieldExpression
        {
            get
            {
                if (m_fieldExp == null)
                {
                    m_fieldExp = new Regex("\\s+\"?([^\"]*)\"");
                }

                return m_fieldExp;
            }
        }
        /// <summary>
        /// Gets the merge fields.
        /// </summary>
        /// <value>The merge fields.</value>
        internal List<PseudoMergeField> MergeFields
        {
            get
            {
                if (m_mergeFields == null)
                {
                    m_mergeFields = new List<PseudoMergeField>();
                    UpdateMergeFields();
                }
                return m_mergeFields;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WIfField"/> class.
        /// </summary>
        public WIfField(IWordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Implementation
        #region UpdatedResults
        /// <summary>
        /// Updates if field.
        /// </summary>
        internal void UpdateIfField()
        {
            ParseResult();
            string fieldCode = RemoveMergeFormat(NestedFieldCode);
            fieldCode = RemoveText(fieldCode, "if");
            List<string> arguments = SplitIfArguments(fieldCode);
            string text = string.Empty;
            string result = string.Empty;
            try
            {
                UpdateIfFieldResult((UpdateCondition(arguments[0]) == "1"));
            }
            catch (Exception e)
            {
                FieldResult = "Error! Unknown op code for conditional.";
            }
        }
        /// <summary>
        /// Gets the text in the entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Entity text</returns>
        private string GetEntityText(Entity entity)
        {
            m_bIsFieldSeparator = false;
            m_bIsSkip = false;
            m_nestedFields.Clear();
            string result = string.Empty;
            if (entity is ParagraphItem)
                result = UpdateTextForParagraphItem(entity);
            else if (entity is WParagraph)
                result = UpdateTextForTextBodyItem(entity);
            result = result.Replace("\r", string.Empty);
            return result;
        }
        /// <summary>
        /// Gets the Field Result as String
        /// </summary>
        /// <param name="fieldResult">List of entity.</param>
        /// <returns>Field Result as string</returns>
        private string ConvertFieldResultToString(List<Entity> fieldResult)
        {
            string result = string.Empty;
            foreach (Entity entity in fieldResult)
                result += GetEntityText(entity);
            return result;
        }
        /// <summary>
        /// Updates the field result entities between FieldSeparator and FieldEnd.
        /// </summary>
        /// <param name="result">The result.</param>       
        private void UpdateIfFieldResult(bool result)
        {
            List<Entity> fieldResult = new List<Entity>();
            fieldResult = result ? TrueTextField : FalseTextField;
            FieldResult = ConvertFieldResultToString(fieldResult);
            if (OwnerParagraph == null
                || FieldEnd == null)
                return;
            //Insert FieldSeparator if the FieldSeparator is null.
            CheckFieldSeparator();
            //Remove previous result
            RemovePreviousResult();
            //Insert result in between FieldSeparator and FieldEnd
            int paraEntityCount = 0;
            if (OwnerParagraph == FieldEnd.OwnerParagraph)
            {                 
                for (int i = 0; i < fieldResult.Count; i++)
                    OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), fieldResult[i]);
            }
            else
            {
                if (FieldSeparator.OwnerParagraph != FieldEnd.OwnerParagraph)
                {
                    int index = Range.Items.Count > 0 ? Range.Items.Count - 1 : 0;
                    WParagraph lastParagraphInRange = Range.Items[index] as WParagraph;
                    WTextBody textBody = lastParagraphInRange.OwnerTextBody;
                    for (int i = 0; i < fieldResult.Count; i++)
                    {
                        if (fieldResult[i] is ParagraphItem)
                        {
                            paraEntityCount++;
                            FieldSeparator.OwnerParagraph.Items.Add(fieldResult[i]);
                            if (i == fieldResult.Count - 1)
                            {
                                FieldSeparator.OwnerParagraph.Items.Add(FieldEnd);
                                if (lastParagraphInRange.ChildEntities.Count == 0)
                                    textBody.Items.Remove(lastParagraphInRange);
                            }
                        }
                        else
                        {
                            textBody.Items.Insert(lastParagraphInRange.GetIndexInOwnerCollection(), fieldResult[i]);
                            if (i == fieldResult.Count - 1 && paraEntityCount>1 && fieldResult[i] is WParagraph)
                            {
                                (fieldResult[i] as WParagraph).ChildEntities.Add(FieldEnd);
                                if (lastParagraphInRange.ChildEntities.Count == 0)
                                    textBody.Items.Remove(lastParagraphInRange);
                            }
                        }
                    }

                }
                else
                    UpdateIfFieldResult(fieldResult);
            }
            m_bIsFieldRangeUpdated = false;
        }
        /// <summary>
        /// Updates the field result entities between FieldSeparator and FieldEnd.
        /// </summary>
        /// <param name="fieldResult">List of field result entities.</param>       
        private void UpdateIfFieldResult(List<Entity> fieldResult)
        {
            Entity clonedParagraph = FieldSeparator.OwnerParagraph.Clone();
            int len = FieldSeparator.OwnerParagraph.ChildEntities.Count;
            int separatorIndex = FieldSeparator.GetIndexInOwnerCollection();
            for (int i = separatorIndex + 1; i < len; i++)
                FieldSeparator.OwnerParagraph.ChildEntities.RemoveAt(separatorIndex + 1);
            for (int i = 0; i <= separatorIndex; i++)
                (clonedParagraph as WParagraph).ChildEntities.RemoveAt(0);
            WParagraph lastParagraphInRange = FieldSeparator.OwnerParagraph;
            WTextBody textBody = lastParagraphInRange.OwnerTextBody;
            int index = Range.Items.Count > 0 ? Range.Items.Count - 1 : 0;
            lastParagraphInRange = Range.Items[index] as WParagraph;
            textBody = lastParagraphInRange.OwnerTextBody;
            for (int i = 0; i < fieldResult.Count; i++)
            {
                if (fieldResult[i] is ParagraphItem)
                {
                    FieldSeparator.OwnerParagraph.Items.Add(fieldResult[i]);
                    if (i == fieldResult.Count - 1)
                    {
                        List<Entity> paraEntities = new List<Entity>();
                        foreach (Entity entity in (clonedParagraph as WParagraph).ChildEntities)
                            paraEntities.Add(entity);
                        foreach (Entity entity in paraEntities)
                            FieldSeparator.OwnerParagraph.Items.Add(entity);
                        for (int j = 0; j < FieldSeparator.OwnerParagraph.ChildEntities.Count; j++)
                            if (FieldSeparator.OwnerParagraph.ChildEntities[j] is WFieldMark)
                                FieldEnd = (FieldSeparator.OwnerParagraph.ChildEntities[j] as WFieldMark);
                    }
                }
                else
                {
                    if ((i == fieldResult.Count - 1) && (fieldResult[i] is WParagraph))
                    {
                        List<Entity> paraEntities = new List<Entity>();
                        foreach (Entity entity in (clonedParagraph as WParagraph).ChildEntities)
                            paraEntities.Add(entity);
                        foreach (Entity entity in paraEntities)
                            (fieldResult[i] as WParagraph).ChildEntities.Add(entity);
                        for (int j = 0; j < (fieldResult[i] as WParagraph).ChildEntities.Count; j++)
                            if ((fieldResult[i] as WParagraph).ChildEntities[j] is WFieldMark)
                                FieldEnd = ((fieldResult[i] as WParagraph).ChildEntities[j] as WFieldMark);
                    }
                    else
                        textBody.Items.Insert(lastParagraphInRange.GetIndexInOwnerCollection(), fieldResult[i]);
                }
            }
        }
        /// <summary>
        /// Parse the Field range to  get expression and Results
        /// </summary>        
        /// <returns>expression text</returns>
        internal string ParseResult()
        {
            //clear the results
            TrueTextField.Clear();
            FalseTextField.Clear();

            Entity entity = null;
            string expressionText = FieldCode;
            int entityIndex = 0;
            string resultText = string.Empty;            
            bool expressionFound = false;
            bool readTrueText = false;
            bool readFasleText = false;
            bool firtsLoop = true;
            bool readresult = false;
            bool isContinuousAdd = false;

            //Get the cloned field range
            List<Entity> fieldRange = GetUpdatedRange();

            while (entity != null || firtsLoop)
            {
                firtsLoop = false;                
                //Find expression in the range
                if (!expressionFound)
                    ReadExpression(ref expressionText, ref entity, ref readTrueText, ref expressionFound);
                //Find True result items from the range
                if (readTrueText && entity != null)
                    ReadTrueResult(ref resultText, ref entity, ref readTrueText);
                //check whether true result starts with quotes or not. If not, set isContinuousAdd to true.
                if (!readTrueText && !readFasleText && expressionFound)
                {
                    resultText = resultText.TrimStart();
                    if (!(resultText.StartsWith(ControlChar.DoubleQuote.ToString()) 
                        || resultText.StartsWith(ControlChar.RightDoubleQuote.ToString()) 
                        || resultText.StartsWith(ControlChar.LeftDoubleQuote.ToString())
                        || resultText.StartsWith(ControlChar.DoubleLowQuote.ToString())) && (resultText.EndsWith(ControlChar.DoubleQuote.ToString()) || resultText.EndsWith(ControlChar.RightDoubleQuote.ToString()) || resultText.EndsWith(ControlChar.LeftDoubleQuote.ToString())))
                    {
                        isContinuousAdd = true;
                    }
                    resultText = string.Empty;
                    readFasleText = true;
                }
                //Find False result items from range
                if (readFasleText && entity != null)
                    ReadFalseResult(ref resultText, ref entity, ref readFasleText, ref isContinuousAdd);

                if (entity == null)
                {
                    if (fieldRange.Count <= entityIndex)
                        break;
                    if (!readTrueText && !readFasleText && expressionFound)
                        break;

                    entity = fieldRange[entityIndex];
                    entityIndex++;
                }
                if (!expressionFound)
                    expressionText += GetEntityText(entity);
            }
            TrimFieldResults(TrueTextField);
            TrimFieldResults(FalseTextField);
            return expressionText;
        }
        #endregion

        #region Clone Range
        /// <summary>
        /// Gets the updated cloned field Range
        /// </summary>        
        /// <returns>List of cloned Entity</returns>
        private List<Entity> GetUpdatedRange()
        {
            bool endReached = false;
            List<Entity> fieldRange = new List<Entity>();
            int entityIndex = 0;
            while (entityIndex < Range.Items.Count)
            {
                Entity entity = (Range.Items[entityIndex] as Entity);
                if (entity is ParagraphItem)
                {
                    if (FieldSeparator == entity)
                        endReached = true;
                    else
                    {
                        if (nestedFieldEnd == null)
                            GetClonedParagraphItem(entity, ref fieldRange);
                        if (nestedFieldEnd != null && entity == nestedFieldEnd)
                            nestedFieldEnd = null;
                    }
                }
                if (entity is WParagraph)
                {
                    WParagraph para = (entity.Clone() as WParagraph);
                    WParagraph paraOrginal = (entity as WParagraph);
                    List<Entity> cloneEntities = new List<Entity>();                    
                    int count = paraOrginal.ChildEntities.Count;
                    for (int i = 0; i < count; i++)
                    {
                      Entity item=  paraOrginal.ChildEntities[i];
                      if (FieldSeparator == item)
                      {
                          endReached = true;
                          break;
                      }
                      if (nestedFieldEnd == null)
                          GetClonedParagraphItem(item, ref cloneEntities);
                      if (nestedFieldEnd != null && item == nestedFieldEnd)
                          nestedFieldEnd = null;
                      count = paraOrginal.ChildEntities.Count;
                    }
                    para.ClearItems();
                    bool entityAdded = false;
                    List<Entity> bodyItem = new List<Entity>();
                    foreach (Entity item in cloneEntities)
                    {
                        if (item is ParagraphItem)
                            para.ChildEntities.Add(item);
                        else
                            bodyItem.Add(item);
                    }                    
                    fieldRange.Add(para);
                    foreach (Entity item in bodyItem)
                        fieldRange.Add(item);
                }
                if (entity is WTable)
                    if (nestedFieldEnd == null)
                        fieldRange.Add(GetClonedTable(entity));
                if (endReached)
                    break;
                entityIndex++;
            }
            return fieldRange;
        }
        /// <summary>
        /// Gets the cloned Field entiy
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityList">The entity list.</param>        
        private void GetClonedFieldItem(Entity entity, ref List<Entity> entityList)
        {
            //Retrieves the paragraph item to be updated as field result.            
            if (entity is WField
                && (entity as WField).FieldSeparator != null
                && (entity as WField).FieldEnd != null
                && (entity as WField).FieldType != FieldType.FieldHyperlink)
            {
                (entity as WField).Update();
                nestedFieldEnd = (entity as WField).FieldEnd;
                // add the double quotes before the inner if field result
                Entity roughTextRange = new WTextRange(entity.Document);
                (roughTextRange as WTextRange).Text = ControlChar.DoubleQuote.ToString();
                entityList.Add(roughTextRange);
                int index = 0;
                if ((entity as WField).Range.Items.Contains((entity as WField).FieldSeparator))
                    index = (entity as WField).Range.Items.IndexOf((entity as WField).FieldSeparator) + 1;
                else if ((entity as WField).Range.Items.Contains((entity as WField).FieldSeparator.OwnerParagraph))
                    index = (entity as WField).Range.Items.IndexOf((entity as WField).FieldSeparator.OwnerParagraph);
                for (int i = index; i < (entity as WField).Range.Items.Count; i++)
                {
                    if ((entity as WField).Range.Items[i] == (entity as WField).FieldEnd)
                        break;
                    else if ((entity as WField).Range.Items[i] == (entity as WField).FieldSeparator.OwnerParagraph)
                    {
                        if (((entity as WField).Range.Items[i] as WParagraph).LastItem == (entity as WField).FieldSeparator)
                            continue;
                        WParagraph clonedPara = ((entity as WField).Range.Items[i] as Entity).Clone() as WParagraph;
                        clonedPara.ClearItems();
                        Entity item = (entity as WField).FieldSeparator.NextSibling as Entity;
                        item = item.NextSibling as Entity;
                        while (item != null)
                        {
                            clonedPara.Items.Add(item.Clone());
                            item = item.NextSibling as Entity;
                        }
                        entityList.Add(clonedPara);
                    }
                    else
                        entityList.Add(((entity as WField).Range.Items[i] as Entity).Clone());
                }
                // add the double quotes after the inner if field result
                entityList.Add(roughTextRange.Clone ());
            }
            else
                entityList.Add(entity.Clone());
        }
        /// <summary>
        /// Gets the cloned paragraph item
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityList">The entity list.</param>
        private void GetClonedParagraphItem(Entity entity, ref List<Entity> entityList)
        {
            if (entity is WField)
                GetClonedFieldItem(entity, ref entityList);
            else
                entityList.Add(entity.Clone());
        }
        /// <summary>
        ///Splits Entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="remaining">The remaining text.</param>
        private Entity SplitEntity(Entity entity, string remaining)
        {
            Entity clonedEntity = entity.Clone();
            string firstPart = GetEntityText(entity);
            firstPart = firstPart.Substring(0, firstPart.Length - remaining.Length);
            if (entity is WTextRange)
            {
                (entity as WTextRange).Text = remaining;
                (clonedEntity as WTextRange).Text = firstPart;
            }
            if (entity is WParagraph)
            {
                WParagraph para = (clonedEntity as WParagraph);
                WParagraph clonedPara = (entity as WParagraph);
                string code = GetEntityText(para);
                int length = code.Length - remaining.Length;
                code = string.Empty;
                for (int i = 0; i < para.ChildEntities.Count; i++)
                {
                    code += GetEntityText(para.ChildEntities[i]);
                    if (code.Length >= length)
                    {
                        code = code.Remove(0, length);
                        Entity toInsert = SplitEntity(clonedPara.ChildEntities[i], code);
                        for (int j = 0; j < i; j++)
                            clonedPara.ChildEntities.RemoveAt(0);
                        para.ChildEntities.RemoveAt(i);
                        para.ChildEntities.Insert(i, toInsert);
                        int count = para.ChildEntities.Count;
                        if (i != count - 1)
                            for (int j = i + 1; j < count; j++)
                                para.ChildEntities.RemoveAt(i + 1);
                        break;
                    }
                }
            }
            return clonedEntity;
        }
        #endregion

        #region Parse expression
        /// <summary>
        /// Reads the expression from range
        /// </summary>
        /// <param name="expressionText">The entity.</param>
        /// <param name="readTrueText">Reference to readTrueText flag.</param>
        /// <param name="expressionFound">Reference to expressionFound flag</param>
        private void ReadExpression(ref string expressionText, ref Entity entity, ref bool readTrueText, ref bool expressionFound)
        {
            string remainingTextAfterOperataor = expressionText;
            if (ContainsOperator(ref remainingTextAfterOperataor))
            {
                string remaining = remainingTextAfterOperataor;
                if (ReachedEndOfExpression(ref remaining))
                {
                    expressionFound = true;
                    expressionText = expressionText.Substring(0, expressionText.Length - remaining.Length);
                    if (entity == null)
                        entity = GetTextRange(remaining);
                    else
                        SplitEntity(entity, remaining);
                    readTrueText = true;
                }
                else
                    entity = null;
            }else
                entity = null;
        }
        /// <summary>
        /// Checks the text reached end of expression.
        /// </summary>
        /// <param name="text">The text.Text after the expression remains in the text if expression found</param>       
        /// <returns>"true" if the text reached end of expression otherwise false</returns>
        private bool ReachedEndOfExpression(ref string text)
        {
            //Remove space at beging
            if (text.StartsWith(" "))
                text = text.TrimStart(' ');
            if (text.StartsWith(ControlChar.DoubleQuote.ToString()) 
                || text.StartsWith(ControlChar.RightDoubleQuote.ToString()) 
                || text.StartsWith(ControlChar.LeftDoubleQuote.ToString())
                || text.StartsWith(ControlChar.DoubleLowQuote.ToString()))
            {
                //if string starts with " then search for next " 
                if (text.Length > 0)
                    text = text.Remove(0, 1);
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ControlChar.DoubleQuote || text[i] == ControlChar.RightDoubleQuote || text[i] == ControlChar.LeftDoubleQuote)
                    {                        
                        text = text.Length - 1 == i ? string.Empty : text.Substring(i + 1);
                        return true;
                    }
                }
            }
            else
            {
                //if string not starts with " then search for next " or space
                if (text.Length > 0)
                    text = text.Remove(0, 1);
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ' || text[i] == ControlChar.DoubleQuote 
                        || text[i] == ControlChar.RightDoubleQuote 
                        || text[i] == ControlChar.LeftDoubleQuote 
                        || text[i] == ControlChar.DoubleLowQuote)
                    {                        
                        text = text.Length - 1 == i ? string.Empty : text.Substring(i + 1);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks the text contains operator or not.
        /// </summary>
        /// <param name="text">The text.Text after the operator remains in the text if operator found</param>
        /// <returns>"true" if the text conatins operator otherwise false</returns>
        private bool ContainsOperator(ref string text)
        {
            string[] operators = new string[] { "<=", ">=", "<>", "=", "<", ">" };
            foreach (string op in operators)
            {
                if (text.Contains(op))
                {
                    int oppIndex = text.LastIndexOf(op);
                    text = text.Substring(oppIndex);
                    text = text.Replace(op, string.Empty);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Parse result
        /// <summary>
        /// Reads the True result entity
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="readTrueText">Reference to readTrueText flag.</param>
        /// <param name="expressionFound">Reference to expressionFound flag</param>
        private void ReadTrueResult(ref string text, ref Entity entity, ref bool readTrueText)
        {
            text += GetEntityText(entity);
            string remaining = text;
            if (ReachedEndOfExpression(ref remaining))
            {
                Entity temp = SplitEntity(entity, remaining);
                if (temp != null)
                    TrueTextField.Add(temp);
                text = text.Substring(0, text.Length - remaining.Length);
                readTrueText = false;
            }
            else
            {
                TrueTextField.Add(entity);
                entity = null;
            }
        }
        /// <summary>
        /// Reads the False result entity
        /// </summary>        
        /// <param name="entity">The entity.</param>        
        private void ReadFalseResult(ref string text, ref Entity entity, ref bool readFalseText, ref bool isContinuousAdd)
        {
            if (!isContinuousAdd)
            {
                text += GetEntityText(entity);
                string remaining = text;
                if (ReachedEndOfExpression(ref remaining))
                {
                    readFalseText = false;
                    Entity temp = SplitEntity(entity, remaining);
                    if (temp != null)
                        FalseTextField.Add(temp);
                    text = string.Empty;
                }
                else
                    FalseTextField.Add(entity);
            }
            else
            {
                string result = GetEntityText(entity);
                if (result.Contains(ControlChar.DoubleQuote.ToString()) 
                    || result.Contains(ControlChar.LeftDoubleQuote.ToString()) 
                    || result.Contains(ControlChar.RightDoubleQuote.ToString())
                    || result.Contains(ControlChar.DoubleLowQuote.ToString()))
                {
                    int index = GetIndexOfDoubleQuote(result);
                    string subString = result.Substring(0, index);
                    Entity temp = SplitEntity(entity, subString);
                    if (temp != null)
                        FalseTextField.Add(temp);
                    readFalseText = false;
                }
                else
                    FalseTextField.Add(entity);
            }
            entity = null;
        }
        /// <summary>
        /// Removes the unwanted text/entity in the Result
        /// </summary>
        /// <param name="fieldResult">The field result.</param>
        private void TrimFieldResults(List<Entity> fieldResult)
        {
            TrimDoubleQuotes(fieldResult);
            if (fieldResult.Count > 0 && fieldResult[0] is WParagraph)
            {
                int i = 0;
                WParagraph para = (fieldResult[0] as WParagraph);
                foreach (Entity e in para.ChildEntities)
                    fieldResult.Insert(i++, e.Clone());
                fieldResult.Remove(para);
                if (fieldResult.Count == 0)
                    if (fieldResult[0] is WFieldMark)
                        fieldResult.RemoveAt(0);
            }
        }
        /// <summary>
        /// Trim Double Quotes
        /// </summary>
        private void TrimDoubleQuotes(List<Entity> fieldResult)
        {
            bool isStart = true;
            bool isEnd = false;
            int endIndex = fieldResult.Count;
            for (int i = 0; i < fieldResult.Count; i++)
            {
                if (fieldResult[i] is WTextRange)
                {
                    WTextRange textRange = (fieldResult[i] as WTextRange);
                    TrimDoubleQuotesinTextRange(ref textRange, ref isStart, ref isEnd);
                }
                //Check whether start the quotation is in paragraph
                if (isStart && fieldResult[i] is WParagraph)
                {
                    for (int j = 0; j < (fieldResult[i] as WParagraph).ChildEntities.Count; j++)
                    {
                        if ((fieldResult[i] as WParagraph).ChildEntities[j] is WTextRange)
                        {
                            WTextRange textRange = ((fieldResult[i] as WParagraph).ChildEntities[j] as WTextRange);
                            TrimDoubleQuotesinTextRange(ref textRange, ref isStart, ref isEnd);
                        }
                    }
                }
                //Check the end quotation is in Paragraph
                if (!isStart && !isEnd && fieldResult[i] is WParagraph)
                {
                    for (int j = 0; j < (fieldResult[i] as WParagraph).ChildEntities.Count; j++)
                    {
                        if ((fieldResult[i] as WParagraph).ChildEntities[j] is WTextRange)
                        {
                            WTextRange textRange = ((fieldResult[i] as WParagraph).ChildEntities[j] as WTextRange);
                            TrimDoubleQuotesinTextRange(ref textRange, ref isStart, ref isEnd);
                        }
                    }
                    endIndex = i;
                }
                if (isEnd)
                    break;
            }
            for (int i = fieldResult.Count - 1; i > endIndex; i--)
            {
                fieldResult.RemoveAt(i);
            }
        }
        /// <summary>
        /// Trims the double quotesin text range.
        /// </summary>
        /// <param name="textRange">The text range.</param>
        /// <param name="isStart">if set to <c>true</c> [is start].</param>
        /// <param name="isEnd">if set to <c>true</c> [is end].</param>
        private void TrimDoubleQuotesinTextRange(ref WTextRange textRange, ref bool isStart, ref bool isEnd)
        {
            int index = -1;
            if (isStart && textRange.Text.StartsWith(" "))
            {
                textRange.Text = textRange.Text.TrimStart();
            }
            //set isStart to false if the result start with space.
            if (isStart && !(textRange.Text.Contains(ControlChar.DoubleLowQuote.ToString())
                ||textRange.Text.StartsWith(ControlChar .DoubleQuote .ToString ())
                ||textRange.Text.StartsWith(ControlChar .RightDoubleQuote .ToString ())
                ||textRange.Text.StartsWith(ControlChar .LeftDoubleQuote .ToString ())) && textRange.Text != string.Empty)
            {
                isStart = false;
            }
            if (isStart && (textRange.Text.Contains(ControlChar.DoubleLowQuote.ToString())
                || textRange.Text.Contains(ControlChar.DoubleQuote.ToString())
                || textRange.Text.Contains(ControlChar.RightDoubleQuote.ToString())
                || textRange.Text.Contains(ControlChar.LeftDoubleQuote.ToString())))
            {
                string text = textRange.Text;
                // Get Index of Double quote character
                index = GetIndexOfDoubleQuote(text);
                index++;
                textRange.Text = text.Substring(index, text.Length - index);
                isStart = false;
            }
            if (!isStart && !isEnd && (textRange.Text.Contains(ControlChar.DoubleLowQuote.ToString())
                || textRange.Text.Contains(ControlChar.DoubleQuote.ToString())
                || textRange.Text.Contains(ControlChar.RightDoubleQuote.ToString())
                || textRange.Text.Contains(ControlChar.LeftDoubleQuote.ToString())))
            {
                string text = textRange.Text;
                // Get Index of Double quote character
                index = GetIndexOfDoubleQuote(text);
                textRange.Text = text.Remove(index, text.Length - index);
                isEnd = true;
            }

        }

        /// <summary>
        /// Get Index of Double quote character
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int GetIndexOfDoubleQuote(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ControlChar.DoubleLowQuote || text[i] == ControlChar.DoubleQuote
                  || text[i] == ControlChar.LeftDoubleQuote || text[i] == ControlChar.RightDoubleQuote)
                    return i;
            }
            return 0;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldCode"></param>
        protected internal override void ParseFieldCode(string fieldCode)
        {
            UpdateFieldCode(fieldCode);
        }
        /// <summary>
        /// Updates field code
        /// </summary>
        /// <param name="fieldCode"></param>
        protected internal override void UpdateFieldCode(string fieldCode)
        {
            char[] separator = new char[1] { '\\' };
            string[] fieldValues = fieldCode.Split(separator);
            ParseFieldFormat(fieldValues);
            m_fieldValue = fieldValues[0].Replace("IF", string.Empty);
        }
        /// <summary>
        /// Checks the expression strings.
        /// </summary>
        private void CheckExpStrings()
        {
            if (m_expression1 == null)
            {
                ParseFieldValue();
            }
        }
        /// <summary>
        /// Parses the field value.
        /// </summary>
        private void ParseFieldValue()
        {
            //IF field has the following model:
            //{IF  Expression1  Operator  Expression2  TrueText  FalseText}
            if (m_fieldValue == null || m_fieldValue == string.Empty)
                return;

            Match opMatch = OperatorExpression.Match(m_fieldValue);
            // Update Operator
            m_operator = (opMatch.Groups[0]).Value;
            int position = opMatch.Index;

            // Find Expression 1
            m_expression1 = (m_fieldValue.Substring(0, position)).Replace("IF", string.Empty);

            position += m_operator.Length;
            string expPart = m_fieldValue.Substring(position, m_fieldValue.Length - position);

            MatchCollection matches = FieldExpression.Matches(expPart);
            if (matches.Count != 3)
                return;

            // Define expression2
            int startPos = matches[0].Index;
            int endPos = matches[1].Index;
            m_expression2 = expPart.Substring(startPos, endPos - startPos);

            // Define trueText
            startPos = matches[1].Index;
            endPos = matches[2].Index;
            m_trueText = expPart.Substring(startPos, endPos - startPos);
            // Define falseText
            startPos = endPos;
            m_falseText = expPart.Substring(startPos, expPart.Length - startPos);
        }
        /// <summary>
        /// Updates the expression string.
        /// </summary>
        internal void UpdateExpString()
        {
            string whiteSpace = " ";

            if (m_expField1 != null && m_expField1.Value != null)
            {
                m_expression1 = "\"" + m_expField1.Value + "\"" + whiteSpace;
            }
            if (m_expField2 != null && m_expField2.Value != null)
            {
                m_expression2 = "\"" + m_expField2.Value + "\"" + whiteSpace;
            }

            if (m_expression1 != null && m_expression2 != null && m_trueText != null && m_falseText != null)
            {
                m_fieldValue = m_expression1 + m_operator + whiteSpace + m_expression2 + m_trueText + m_falseText;
            }
        }
        /// <summary>
        /// Updates the merge fields.
        /// </summary>
        internal void UpdateMergeFields()
        {
            if (Expression1.FitMailMerge)
                m_mergeFields.Add(Expression1);
            if (Expression2.FitMailMerge)
                m_mergeFields.Add(Expression2);
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WIfField field = base.CloneImpl() as WIfField;

            field.m_expField1 = null;
            field.m_expField2 = null;
            field.m_trueTextField = null;
            field.m_falseTextField = null;

            return field;
        }

        #endregion
    }
    /// <summary>
    /// Class represents pseudo merge field inside IF field
    /// </summary>
    internal class PseudoMergeField
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private bool m_fitMailMerge;
        private string m_name;
        private string m_value;
        private Regex m_nameExp;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        internal string Name
        {
            get
            {
                return m_name;
            }
        }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        internal string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether fit mail merge.
        /// </summary>
        /// <value>if it fits the mail merge, set to <c>true</c>.</value>
        internal bool FitMailMerge
        {
            get
            {
                return m_fitMailMerge;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Regex NameExpression
        {
            get
            {
                if (m_nameExp == null)
                {
                    m_nameExp = new Regex("MERGEFIELD\\s+\"?([^\"]+)\"");
                }
                return m_nameExp;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoMergeField"/> class.
        /// </summary>
        /// <param name="fieldText">The field text.</param>
        internal PseudoMergeField(string fieldText)
        {
            if (fieldText == null)
                return;

            if (fieldText.IndexOf("MERGEFIELD") == -1)
            {
                char[] splitter = new char[1] { '\"' };
                string[] textParts = fieldText.Split(splitter);
                if (textParts.Length == 1)
                {
                    m_value = fieldText.Trim();
                }
                else if (textParts.Length == 3)
                {
                    m_value = textParts[1];
                }
                else
                {
                    m_value = string.Empty;
                }
            }
            else
            {
                Match match = NameExpression.Match(fieldText);
                if (match.Groups.Count > 1)
                {
                    m_name = match.Groups[1].Value;
                    m_fitMailMerge = true;
                }
            }
        }
        #endregion
    }
}
