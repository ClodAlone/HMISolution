//-------------------------------------------------------------------------------------------------
// <copyright file="ExpressionFieldDescriptor.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    #region ExpressionFieldDescriptorCollection
    /// <summary>
    /// A collection of <see cref="ExpressionFieldDescriptor"/> fields with run-time formula expressions. 
    /// An instance of this collection is returned by the <see cref="TableDescriptor.ExpressionFields"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class ExpressionFieldDescriptorCollection : FieldDescriptorCollection
    {
        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static new readonly ExpressionFieldDescriptorCollection Empty = new ExpressionFieldDescriptorCollection(null);

        /// <override/>
        protected override FieldDescriptor InternalCreateFieldDescriptor(string name)
        {
            return base.InternalCreateFieldDescriptor(name);
        }

        /// <override/>
        protected override void EnsureInitialized(bool populate)
        {
        }

        /// <override/>
        protected override FieldDescriptorCollection CreateCollection(TableDescriptor td, FieldDescriptor[] columnDescriptors)
        {
            return new ExpressionFieldDescriptorCollection((TableDescriptor)td, columnDescriptors);
        }

        /// <override/>
        protected override int InternalAdd(string name)
        {
            return Add(new ExpressionFieldDescriptor(name));
        }
        
        /// <override/>
        protected override void CheckType(object obj)
        {
            if (obj != null && !(obj is ExpressionFieldDescriptor))
            {
                throw new ArgumentException("Wrong type");
            }
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public ExpressionFieldDescriptorCollection()
        {
        }

        internal ExpressionFieldDescriptorCollection(TableDescriptor tableDescriptor)
            : base(tableDescriptor)
        {
        }

        internal ExpressionFieldDescriptorCollection(TableDescriptor tableDescriptor, FieldDescriptor[] columnDescriptors)
            : base(tableDescriptor, columnDescriptors)
        {
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="values">The array whose elements should be added to the end of the collection. 
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic). 
        /// </param>
        public void AddRange(ExpressionFieldDescriptor[] values)
        {
            base.AddRange((FieldDescriptor[])values);
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public new ExpressionFieldDescriptorCollection Clone()
        {
            return (ExpressionFieldDescriptorCollection)InternalClone();
        }

        /// <summary>Compares two descriptor values.</summary>
        /// <returns>True if the descriptors are equivalent; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is ExpressionFieldDescriptorCollection))
            {
                return false;
            }

            return InternalEquals((ExpressionFieldDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Returns hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public new ExpressionFieldDescriptor this[int index]
        {
            get
            {
                return (ExpressionFieldDescriptor)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public new ExpressionFieldDescriptor this[string name]
        {
            get
            {
                return (ExpressionFieldDescriptor)base[name];
            }

            set
            {
                base[name] = value;
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(ExpressionFieldDescriptor[] array, int index)
        {
            int n = 0;
            foreach (ExpressionFieldDescriptor item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public new ExpressionFieldDescriptorCollectionEnumerator GetEnumerator()
        {
            return new ExpressionFieldDescriptorCollectionEnumerator(this);
        }

        /// <override/>
        protected internal override void SuggestName(FieldDescriptor value)
        {
            int n = 1;
            foreach (ExpressionFieldDescriptor col in this)
            {
                if (col.Name.StartsWith("Expr "))
                {
                    double d;
                    if (double.TryParse(col.Name.Substring("Expr ".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = (int)d + 1;
                    }
                }
            }

            value.Name = "Expr " + n.ToString();
            value.nameModified = false;
        }
    }

    /// <summary>
    /// Enumerator class for the <see cref="ExpressionFieldDescriptor"/> elements of an <see cref="ExpressionFieldDescriptorCollection"/>.
    /// </summary>
    public class ExpressionFieldDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        ExpressionFieldDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public ExpressionFieldDescriptorCollectionEnumerator(ExpressionFieldDescriptorCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public ExpressionFieldDescriptor Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }

    #endregion

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="ExpressionFieldDescriptor"/> objects. <see cref="ExpressionFieldDescriptorTypeConverter"/> 
    /// is an <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the 
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class ExpressionFieldDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The Value.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is ExpressionFieldDescriptor))
            {
                ////        public ExpressionFieldDescriptor(string name, string expression)

                ExpressionFieldDescriptor expressionField = (ExpressionFieldDescriptor)value;
                Type type = typeof(ExpressionFieldDescriptor);

                if (!expressionField.ShouldSerializeResultType())
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(string) }),
                        new object[] { expressionField.Name, expressionField.Expression },
                        true);
                }
                else
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string) }),
                        new object[] { expressionField.Name, expressionField.Expression, expressionField.ResultType },
                        true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <summary>
        /// Gets a list of <see cref="PropertyDescriptor"/> for a given value.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The Value.</param>
        /// <param name="attributes">An array of type System.Attribute that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "ResultType",
                "Expression",
            };

            return pds.Sort(atts);
        }
    }
    #endregion

    #region ExpressionFieldDescriptor

    /// <summary>
    /// ExpressionFieldDescriptor is a <see cref="FieldDescriptor"/> with support for run-time formula expressions.
    /// <para/>
    /// Expression fields are managed by the <see cref="ExpressionFieldDescriptorCollection"/> that
    /// is returned by the <see cref="TableDescriptor.ExpressionFields"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(ExpressionFieldDescriptorTypeConverter))]
    public class ExpressionFieldDescriptor : FieldDescriptor
    {
        string resultType = "System.String";
        string expression = string.Empty;
        bool expressionModified = false;
        bool resultTypeModified = false;
        string compiledExpression = null;
        int fieldsVersion = -1;

        /// <summary>
        /// Initializes a new empty expression field.
        /// </summary>
        public ExpressionFieldDescriptor()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new expression field with a specified name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public ExpressionFieldDescriptor(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new expression field with a name and expression.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="expression">The expression. See the Grid User's Guide for expression syntax and examples.</param>
        public ExpressionFieldDescriptor(string name, string expression)
            : base(name, string.Empty, true, string.Empty)
        {
            this.expression = expression;
            expressionModified = true;
        }

        /// <summary>
        /// Initializes a new expression field with a name, expression, and result type.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="expression">The expression. See the Grid User's Guide for expression syntax and examples.</param>
        /// <param name="resultType">The result type.</param>
        public ExpressionFieldDescriptor(string name, string expression, Type resultType)
            : base(name, string.Empty, true, string.Empty)
        {
            this.expression = expression;
            expressionModified = true;
            this.resultType = String.Concat(resultType.FullName, ",", resultType.AssemblyQualifiedName.Split(',')[1]); 
            this.resultTypeModified = true;
        }

        /// <summary>
        /// Initializes a new expression field with a name, expression, and result type.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="expression">The expression. See the Grid User's Guide for expression syntax and examples.</param>
        /// <param name="resultType">The result type.</param>
        public ExpressionFieldDescriptor(string name, string expression, string resultType)
            : base(name, string.Empty, true, string.Empty)
        {
            this.expression = expression;
            expressionModified = true;
            this.resultType = resultType;
            this.resultTypeModified = true;
        }

        /// <summary>Creates a new expression field descriptor from another descriptor.</summary>
        /// <param name="other">Field descriptor which is used to create the new descriptor.</param>
        /// <override/>
        public override void InitializeFrom(FieldDescriptor other)
        {
            ExpressionFieldDescriptor gcd = (ExpressionFieldDescriptor)other;
            Expression = gcd.Expression;
            ResultType = gcd.ResultType;
            base.InitializeFrom(other);
        }

        /// <summary>Creates a copy of this descriptor.</summary>
        /// <returns>Copy of this descriptor.</returns>
        /// <override/>
        public override FieldDescriptor Clone()
        {
            ExpressionFieldDescriptor ed = new ExpressionFieldDescriptor();
            base.CopyAllMembersTo(ed);
            this.CopyExpressionFieldMembersTo(ed);
            return ed;
        }

        [Documentation.DocumentationExclude()]
        protected void CopyExpressionFieldMembersTo(ExpressionFieldDescriptor ed)
        {
            ed.compiledExpression = this.compiledExpression;
            ed.expression = this.expression;
            ed.expressionModified = this.expressionModified;
            ed.fieldsVersion = -1; ////this.fieldsVersion;
            ed.index = this.index;
            ed.resultType = this.resultType;
            ed.resultTypeModified = this.resultTypeModified;
        }

        /// <summary>Compares this descriptor with another descriptor.</summary>
        /// <returns>True if both the descriptors are equivalent; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            if (!(obj is ExpressionFieldDescriptor))
            {
                return false;
            }

            return InternalEquals((ExpressionFieldDescriptor)obj);
        }

        bool InternalEquals(ExpressionFieldDescriptor other)
        {
            return other.Expression == Expression
                && other.ResultType == ResultType
                && other.Name == Name;
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Returns the hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        /// <summary>
        /// The result type that the expression should be converted to (default is System.String).
        /// </summary>
        [Browsable(true),
        TypeConverter(typeof(ResultTypeConverter)),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("The result type that the expression should be converted to (default is System.String).")]
        public string ResultType
        {
            get
            {
                return resultType;
            }

            set
            {
                if (resultType != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ResultType"));
                    resultType = value;
                    resultTypeModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ResultType"));
                }
            }
        }

        /// <summary>
        /// Determines if <see cref="ResultType"/> was modified.
        /// </summary>
        /// <returns>True if the content was modified; False otherwise.</returns>
        public bool ShouldSerializeResultType()
        {
            return resultTypeModified;
        }

        /// <summary>Gets the property type.</summary>
        /// <returns>Property type.</returns>
        /// <override/>
        public override Type GetPropertyType()
        {
            if (this.ResultType != null)
            {
                return Type.GetType(this.ResultType);
            }

            return typeof(string);
        }
        
        /// <summary>
        /// Resets the result type to System.String.
        /// </summary>
        public void ResetResultType()
        {
            resultType = "System.String";
            resultTypeModified = false;
        }
        
        /// <summary>
        /// The formula expression. See the Grid user's guid for syntax and examples.
        /// </summary>
        [Description("The formula expression.")]
        public string Expression
        {
            get
            {
                return expression;
            }

            set
            {
                if (expression != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Expression"));
                    expression = value;
                    expressionModified = true;
                    compiledExpression = null;
                    string s = GetCompiledExpression(); // Force recalc
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Expression"));
                }
            }
        }

        private bool ShouldSerializeExpression()
        {
            return expressionModified;
        }

        /// <summary>
        /// Resets the formula expression to empty.
        /// </summary>
        public void ResetExpression()
        {
            expression = string.Empty;
            expressionModified = false;
            compiledExpression = null;
        }

        /// <summary>
        /// Gets a string that holds pre-compiled information about the expression.
        /// </summary>
        /// <returns>A string that holds pre-compiled information about the expression.</returns>
        public string GetCompiledExpression()
        {
            if (expression.Length > 0 && TableDescriptor != null)
            {
                int tableDescriptorfieldsVersion = this.TableDescriptor.Fields.Version;
                if (this.fieldsVersion != tableDescriptorfieldsVersion)
                {
                    IExpressionFieldEvaluator eval = GetExpressionEvaluator();
                    if (eval != null)
                    {
                        string s = eval.PutTokensInFormula(expression.ToLower());
                        compiledExpression = eval.Parse(s);
                        this.fieldsVersion = tableDescriptorfieldsVersion;
                    }
                }
            }

            return compiledExpression;
        }

        /// <summary>
        /// Returns an array of field descriptor names that the expression references
        /// </summary>
        /// <returns>returns array of field descriptor names</returns>
        protected override string DetermineReferencedFields()
        {
            ArrayList al = new ArrayList();
            string expr = expression.ToLower().Replace(" ", string.Empty);
            foreach (FieldDescriptor fd in TableDescriptor.Fields)
            {
                string search = "[" + fd.Name.ToLower() + "]";
                if (expr.IndexOf(search) != -1)
                {
                    al.Add(fd.Name);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (string s in al)
            {
                if (sb.Length == 0)
                {
                    sb.Append(s);
                }
                else
                {
                    sb.Append(";" + s);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Resets the compiled expression. It will be recompiled later on demand.
        /// </summary>
        public void ResetCompiledExpression()
        {
            compiledExpression = null;
        }

        /// <summary>
        /// Not used for expression fields.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new string MappingName
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        /// <summary>
        /// Not used for expression fields.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new string DefaultValue
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        /// <summary>
        /// Not used for expression fields.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override bool ReadOnly
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        /// <summary>
        /// Calculates the expression result value for the specified record.
        /// </summary>
        /// <param name="record">The Record.</param>
        /// <returns>The Value of the record.</returns>
        public override object GetValue(Record record)
        {
            IExpressionFieldEvaluator eval = GetExpressionEvaluator();
            return eval.ComputeFormulaValueAt(GetCompiledExpression(), record);
        }
        
        IExpressionFieldEvaluator GetExpressionEvaluator()
        {
            return TableDescriptor.ExpressionFieldEvaluator;
        }
    }

    /// <summary>
    /// An interface that binds the formula logic to expression fields in the grouping engine. A
    /// default implementation of this interface is part of the grouping engine and there is normally
    /// no need to provide your own implementation. <para/>
    /// If you want to customize the formula calculation
    /// you need to implement this interface, override the <see cref="Engine.CreateExpressionFieldEvaluator"/>
    /// of the <see cref="Engine"/> class, and create an instance of your implementation in your overriden method
    /// of CreateExpressionFieldEvaluator.
    /// </summary>
    public interface IExpressionFieldEvaluator
    {
        /// <summary>
        /// Calculates the expression result for the given record.
        /// </summary>
        string ComputeFormulaValueAt(string formula, Record position);

        /// <summary>
        /// Replaces field references with internal tokens.
        /// </summary>
        string PutTokensInFormula(string formula);

        /// <summary>
        /// Parses an expression and returns a pre-compiled expression.
        /// </summary>
        string Parse(string text);
    }

    #endregion

    /// <summary>
    /// Implements a <see cref="TypeConverter"/> for the <see cref="ExpressionFieldDescriptor.ResultType"/> property in
    /// <see cref="ExpressionFieldDescriptor"/>.
    /// </summary>
    public class ResultTypeConverter : TypeConverter
    {
        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>        
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type
        /// you want to convert from. </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(System.String))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to
        /// use as the current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>       
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be
        /// performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return value;
            }

            if (value is Type)
            {
                Type type = (Type)value;
                return String.Concat(type.FullName, ",", type.AssemblyQualifiedName.Split(',')[1]);
            }

            return base.ConvertFrom(context, culture, value);
        }

        // no string conversion

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If
        /// null is passed, the current culture is assumed. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the
        /// value parameter to. </param>       
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter
        /// is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be
        /// performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value as string;
            }

            if (destinationType == typeof(Type))
            {
                string s = value as string;

                if (s == null || s.Length == 0)
                {
                    return typeof(object);
                }

                switch (s)
                {
                    case "System.String": return typeof(string);
                    case "System.Double": return typeof(System.Double);
                    case "System.Int32": return typeof(System.Int32);
                    case "System.Boolean": return typeof(System.Boolean);
                    case "System.DateTime": return typeof(System.DateTime);
                    case "System.Int16": return typeof(System.Int16);
                    case "System.Int64": return typeof(System.Int64);
                    case "System.Single": return typeof(System.Single);
                    case "System.Byte": return typeof(System.Byte);
                    case "System.Char": return typeof(System.Char);
                    case "System.Decimal": return typeof(System.Decimal);
                    case "System.UInt16": return typeof(System.UInt16);
                    case "System.UInt32": return typeof(System.UInt32);
                    case "System.UInt64": return typeof(System.UInt64);
                }

                return Type.GetType((string)value);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return svc;
        }

        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if the <see
        /// cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" />
        /// should be called to find a common set of values the object supports; otherwise,
        /// false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Fields
        static ResultTypeConverter()
        {
            values = new string[] 
                {
                    "System.String",
                    "System.Double",
                    "System.Int32",
                    "System.Boolean",
                    ////"System.Drawing.Color, System.Drawing",
                    "System.DateTime",
                    "System.Int16",
                    "System.Int64",
                    "System.Single",
                    "System.Byte",
                    "System.Char",
                    "System.Decimal",
                    "System.UInt16",
                    "System.UInt32",
                    "System.UInt64",
                ////"System.Windows.Forms.DockStyle, System.Windows.Forms",
                /*typeof(System.String),
                    typeof(System.Double),
                    typeof(System.Int32),
                    typeof(System.Boolean),
                    typeof(System.Drawing.Color),
                    typeof(System.DateTime),
                    typeof(System.Int16),
                    typeof(System.Int64),
                    typeof(System.Single),
                    typeof(System.SByte),
                    typeof(System.Byte),
                    typeof(System.Char),
                    typeof(System.Decimal),
                    typeof(System.DBNull),
                    typeof(System.UInt16),
                    typeof(System.UInt32),
                    typeof(System.UInt64),*/
            };
            Array.Sort(values);
            ////            Type[] types = new Type[values.Length];
            //            for (int i = 0; i < values.Length; i++)
            //                types[i] = Type.GetType(values[i]);
            svc = new TypeConverter.StandardValuesCollection(values);
        }

        private static string[] values;
        private static TypeConverter.StandardValuesCollection svc;
    }
}

