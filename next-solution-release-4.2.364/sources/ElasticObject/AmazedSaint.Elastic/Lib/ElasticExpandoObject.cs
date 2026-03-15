using System;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Dynamic;
using System.Linq.Expressions;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;
using System.Linq;

namespace AmazedSaint.Elastic
{
    public class ElasticExpandoObject : DynamicObject, INotifyPropertyChanged
    {
      
        private Dictionary<string, ElasticExpandoObject> attributes = new Dictionary<string, ElasticExpandoObject>();
        private Dictionary<string, List<ElasticExpandoObject>> elements = new Dictionary<string, List<ElasticExpandoObject>>();

        private object wrappedObject;
        

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return attributes.Keys;
           
        }

        /// <summary>
        /// Currenet set of property values
        /// </summary>
        internal Dictionary<string, ElasticExpandoObject> Attributes
        {
            get { return attributes; }
        }


        /// <summary>
        /// Returns a set of child elements
        /// </summary>
        internal IEnumerable<ElasticExpandoObject> Elements
        {
            get
            {
                foreach (var list in elements.Values)
                    foreach(var e in list)
                        yield return e;
            }
        }


        ElasticExpandoObject parent = null;

        /// <summary>
        /// This items parent
        /// </summary>
        internal ElasticExpandoObject InternalParent
        {
            get { return parent; }
            set { parent = value; }
        }

        private object content;
        internal object InternalContent
        {
            get { return content; }
            set { this.content = value; }
        }

       
        /// <summary>
        /// Value of this element
        /// </summary>
        private object value;
        internal object InternalValue
        {
            get { return value; }
            set { this.value = value; }
        }


        /// <summary>
        /// Name of this element
        /// </summary>
        string name = string.Empty;
        public string InternalName
        {
            get { return name; }
            set { name = value; }
        }

        


        public ElasticExpandoObject()
        {
            this.name = "id" + Guid.NewGuid().ToString();
        }

        public ElasticExpandoObject(object objToWrap)
        {
            this.name = "id" + Guid.NewGuid().ToString();
            wrappedObject = objToWrap;
        }


        public ElasticExpandoObject(string name)
        {
            this.name = name;
        }

        public ElasticExpandoObject(object value, ElasticExpandoObject parent, string name) : this(name)
        {
            InternalValue = value;
            this.parent = parent;
        }


        /// <summary>
        /// Fully qualified name
        /// </summary>
        public string InternalFullName
        {
            get
            {
                string path = this.name;
                var parent = this.parent;

                while (parent != null)
                {
                    path = parent.InternalName + "_" + path;
                    parent = parent.parent;
                }

                return path;
            }
        }


        /// <summary>
        /// Add a member to this element, with the specified value
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal ElasticExpandoObject CreateOrGetAttribute(string memberName, object value)
        {
            if (!attributes.ContainsKey(memberName))
            {
                attributes[memberName] = new ElasticExpandoObject(value, this,memberName);
                return attributes[memberName];
            }
            else
            {
                return attributes[memberName];
            }

        }


#if !WINDOWS_UWP
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {

          var obj = new ElasticExpandoObject(null, this, binder.Name);

          this.elements.Add(obj);
          result = obj;
          return true;

          //return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>
        /// Catch a binary operation
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="arg"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {

            if (binder.Operation == ExpressionType.And)
            {
                this.InternalContent = arg;
                result = this;
                return true;
            }

            if (binder.Operation == ExpressionType.LeftShift)
            {
                if (arg is string)
                {
                    var exp = new ElasticExpandoObject(null, this, arg as string);
                    elements.Add(exp);
                    result = exp;
                    return true;
                }

                else if (arg is ExpandoObject)
                {
                    var eobj = arg as ElasticExpandoObject;
                    eobj.parent = this;
                    if (!elements.Contains(eobj))
                        this.elements.Add(eobj);
                    result = eobj;
                    return true;
                }

            }

           return base.TryBinaryOperation(binder, arg, out result);
        }

        /// <summary>
        /// Try the unary operation.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.OnesComplement)
            {
                result = this.ToXElement();
                return true;
            }

            return base.TryUnaryOperation(binder, out result);
        }


        /// <summary>
        /// Handle the indexer operations
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if ((indexes.Length == 1) && indexes[0] == null)
            {
                result = elements;
            }
            else
            {
                result = elements.FindAll
                    (c => indexes.Cast<string>().Contains(c.InternalName));
            }
            return true;
        }



        /// <summary>
        /// Catch a get member invocation
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (attributes.ContainsKey(binder.Name))
            {
                 result = attributes[binder.Name].value;
            }
            else
            {

                var obj = elements.FirstOrDefault(item => item.InternalName == binder.Name);
                if (obj != null)
                {
                    result = obj;
                }
                else
                {
                    var exp = new ElasticExpandoObject(null, this, binder.Name);
                    elements.Add(exp);
                    result = exp;
                }
            }

            return true;
        }

        
        /// <summary>
        /// Catch a set member invocation
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var memberName = binder.Name;

            if (value is ElasticExpandoObject)
            {
                var eobj=value as ElasticExpandoObject;
                eobj.parent = this;
                if (!elements.Contains(eobj))
                    this.elements.Add(eobj);
            }
            else
            {
                if (!attributes.ContainsKey(memberName))
                {
                    attributes[memberName] = new ElasticExpandoObject(value, this, memberName);
                }
                else
                {
                    attributes[memberName].InternalValue = value;
                }
            }

            OnPropertyChanged(memberName);

            return true;
        }
#endif

#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

#endregion
    }
   

}


