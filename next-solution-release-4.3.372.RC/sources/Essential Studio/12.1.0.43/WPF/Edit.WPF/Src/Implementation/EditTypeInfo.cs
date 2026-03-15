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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class EditTypeInfo : IIntellisenseItem
    {
        private ImageSource icon;

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FullName
        {
            get
            {
                return (Namespace + "." + Name).TrimStart('.');
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsInterface { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsEnum { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsClass { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsProperty { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsEvent { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsMethod { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsInstance { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsNamespace { get; set; }

        internal bool IsLexem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        public EditTypeInfo BaseType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<IIntellisenseItem> NestedItems { get; set; }

        #region ICustomIntellisenseItem Members

        /// <summary>
        ///
        /// </summary>
        public string Text
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                if (icon == null)
                {
                    icon = InitializeIcon(this);
                }
                return icon;
            }
            set
            {
                icon = value;
            }
        }

        private ImageSource InitializeIcon(EditTypeInfo item)
        {
            string imageUri = "/Syncfusion.Edit.Wpf;component/Resources/";

            if (item.IsClass)
            {
                imageUri += "class.png";
            }
            else if (item.IsEnum)
            {
                imageUri += "enum.png";
            }
            else if (item.IsInterface)
            {
                imageUri += "interface.png";
            }
            else if (item.IsInstance)
            {
                imageUri += "field.png";
            }
            else if (item.IsLexem)
            {
                imageUri += this.GetIconForLexem(item.LexemType);//"keyword.png";
            }
            else if (item.IsProperty)
            {
                imageUri += "property.png";
            }
            else if (item.IsEvent)
            {
                imageUri += "event.png";
            }
            else if (item.IsMethod)
            {
                imageUri += "method.png";
            }
            else
            {
                imageUri += "ns.png";
            }

            return new BitmapImage(new Uri(imageUri, UriKind.Relative));
        }

        private string GetIconForLexem(EditTokenType editTokenType)
        {
            switch (editTokenType)
            {
                case EditTokenType.Literals:
                case EditTokenType.Comment:
                case EditTokenType.Operator:
                case EditTokenType.Custom:
                    return "keyword.png";
                case EditTokenType.Keyword:
                    return "keyword.png";
                case EditTokenType.Preprocessor:
                case EditTokenType.CodeSnippet:
                case EditTokenType.NamespaceDeclaration:
                    return "codesnippet.png";
                case EditTokenType.Property:
                    return "property.png";
                default:
                    return "keyword.png";
            }
        }

        /// <summary>
        ///
        /// </summary>
        public EditTokenType LexemType
        {
            get;
            set;
        }

        #endregion ICustomIntellisenseItem Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    /// usidljdjfdsj;fkv
    /// </summary>
    public class EditTypeCollection : IList<IIntellisenseItem>
    {
        private List<IIntellisenseItem> items = null;

        /// <summary>
        ///
        /// </summary>
        public EditTypeCollection()
        {
            items = new List<IIntellisenseItem>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="collection"></param>
        public EditTypeCollection(IEnumerable<IIntellisenseItem> collection)
        {
            items = new List<IIntellisenseItem>(collection);
        }

        #region IList<IIntellisenseItem> Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(IIntellisenseItem item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, IIntellisenseItem item)
        {
            this.items.Insert(index, item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            IIntellisenseItem tempItem = this[index];
            this.items.RemoveAt(index);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IIntellisenseItem this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        #endregion IList<IIntellisenseItem> Members

        #region ICollection<IIntellisenseItem> Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void Add(IIntellisenseItem item)
        {
            this.items.Add(item);
        }

        /// <summary>
        ///
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IIntellisenseItem item)
        {
            return this.items.Contains(item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(IIntellisenseItem[] array, int arrayIndex)
        {
            this.items.CopyTo(array);
        }

        /// <summary>
        ///
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IIntellisenseItem item)
        {
            bool tempVal = this.items.Remove(item);
            return tempVal;
        }

        #endregion ICollection<IIntellisenseItem> Members

        #region IEnumerable<IIntellisenseItem> Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IIntellisenseItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion IEnumerable<IIntellisenseItem> Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainsType(string Name)
        {
            var items = this.items.Where(type => type != null && type.Text == Name);
            if (items.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IIntellisenseItem GetEditTypeInfo(string Name)
        {
            var items = this.items.Where(type => type != null && type.Text == Name);
            try
            {
                if (items.Count() > 0)
                {
                    return items.ElementAt(0);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}