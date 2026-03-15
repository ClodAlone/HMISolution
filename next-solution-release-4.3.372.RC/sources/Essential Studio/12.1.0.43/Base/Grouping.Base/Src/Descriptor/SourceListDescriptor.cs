//-------------------------------------------------------------------------------------------------
// <copyright file="SourceListDescriptor.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Extracts and manages properties schema information from an underlying list and maintains an
    /// <see cref="ItemPropertiesVersion"/> counter that is increased every time the
    /// underlying collection of <see cref="PropertyDescriptor"/> objects is changed.
    /// </summary>
    public class SourceListDescriptor : DescriptorBase, ITypedList, IItemPropertiesSource
    {
        // Fields
        string name = "Default";
        internal bool nameModified = false;
        PropertyDescriptorCollection itemProperties = PropertyDescriptorCollection.Empty;
        IBindingList bindingList;
        internal object list;
        Type type;
        internal int itemPropertiesVersion = 0;
        bool hasItemProperties = false;
        bool inbindingList_ListChanged = false;
        internal bool inSetItemProperties = false;
        SourceListSet sourceListSet;

        //// Events

        internal object GetList()
        {
            return list;
        }

        /// <summary>
        /// Occurs before the underlying collection is changed. When an <see cref="IBindingList.ListChanged"/>
        /// event is handled, this event is raised before the <see cref="ItemProperties"/> collection is updated.
        /// </summary>
        [Browsable(false)]
        public event ListChangedEventHandler ListChanging;

        /// <summary>
        /// Occurs after the underlying collection was changed. When an <see cref="IBindingList.ListChanged"/>
        /// event is handled, this event is raised after the <see cref="ItemProperties"/> collection is updated.
        /// </summary>
        [Browsable(false)]
        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// Occurs before the <see cref="Name"/> of this table is changed.
        /// </summary>
        [Browsable(false)]
        public event EventHandler NameChanging;

        /// <summary>
        /// Occurs after the <see cref="Name"/> of this table is changed.
        /// </summary>
        [Browsable(false)]
        public event EventHandler NameChanged;

        /// <summary>
        /// Occurs before the <see cref="ItemProperties"/> collection of this table is changed.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ItemPropertiesChanging;

        /// <summary>
        /// Occurs after the <see cref="ItemProperties"/> collection of this table was changed.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ItemPropertiesChanged;

        /// <summary>
        /// Occurs when the <see cref="ItemProperties"/> is accessed for the first time and has not
        /// been initialized before.
        /// </summary>
        [Browsable(false)]
        public event EventHandler InitializeItemProperties;

        /// <summary>
        /// Initializes a new empty SourceListDescriptor.
        /// </summary>
        public SourceListDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new SourceListDescriptor and initalializes the <see cref="ItemProperties"/>
        /// collection from properties of the specified type.
        /// </summary>
        /// <param name="type">The Type value.</param>
        public SourceListDescriptor(Type type)
        {
            this.type = type;
            SetItemProperties(type);
        }

        /// <summary>
        /// Initializes a new SourceListDescriptor and initializes the <see cref="ItemProperties"/>
        /// collection from properties of the specified list.
        /// </summary>
        /// <param name="list">A list from which the collection has to be created.</param>
        public SourceListDescriptor(object list)
        {
            this.list = list;
            SetItemProperties(list);
        }

        /// <summary>
        /// Initializes a new SourceListDescriptor and initializes the <see cref="ItemProperties"/>
        /// collection from properties of the specified <see cref="PropertyDescriptorCollection"/>.
        /// </summary>
        /// <param name="name">The name value.</param>
        /// <param name="itemProperties">A collection of properties.</param>
        public SourceListDescriptor(string name, PropertyDescriptorCollection itemProperties)
        {
            SetItemProperties(itemProperties);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reset();
                name = "Disposed";

                this.itemProperties = null;
            }

            base.Dispose(disposing);
        }

        private void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            inbindingList_ListChanged = true;
            switch (e.ListChangedType)
            {
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                case ListChangedType.Reset:
                    OnListChanging(e);
                    SetItemProperties(list);
                    OnListChanged(e);
                    break;
            }

            inbindingList_ListChanged = false;
        }
        
        /// <summary>
        /// Gets / sets a reference to a <see cref="SourceListSet"/>. A <see cref="SourceListSet"/>
        /// manages multiple <see cref="SourceListDescriptor"/> objects (or TableDescriptor objects) that belong to the same
        /// <see cref="Engine"/> object.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SourceListSet SourceListSet
        {
            get
            {
                return sourceListSet;
            }

            set
            {
                if (sourceListSet != value)
                {
                    sourceListSet = value;
                    if (this.hasItemProperties)
                    {
                        AddRelationsToSourceListSet();
                    }
                }
            }
        }

        /// <summary>Gets the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Gets / sets the name of this table.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (!nameModified)
                {
                    PropertyDescriptorCollection pdc = ItemProperties; // this call ensures that list has been attached.
                    if (this.list is ITypedList)
                    {
                        string s = ((ITypedList)list).GetListName(null);
                        return s != null ? s : "(null)";
                    }
                    else if (type != null)
                    {
                        return type.Name;
                    }
                    else if (list != null)
                    {
                        return list.GetType().Name;
                    }
                }

                return name;
            }

            set
            {
                if (value == null)
                {
                    ResetName();
                }
                else if (!nameModified || name != value)
                {
                    OnNameChanging(EventArgs.Empty);
                    name = value;
                    nameModified = true;
                    OnNameChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="Name"/>  was modified from its default value.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public virtual bool ShouldSerializeName()
        {
            return nameModified;
        }

        /// <summary>
        /// Resets the <see cref="Name"/> to its default value.
        /// </summary>
        public void ResetName()
        {
            if (nameModified)
            {
                OnNameChanging(EventArgs.Empty);
                if (this.list == null && this.type == null)
                {
                    nameModified = true;
                    name = "Default";
                }
                else
                {
                    nameModified = false;
                    name = string.Empty;
                }

                OnNameChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="PropertyDescriptorCollection"/> with properties for
        /// each record in the table.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual PropertyDescriptorCollection ItemProperties
        {
            get
            {
                // Allow late binding - first create empty SourceListDescriptor and let
                // user fill in data on demand the first time ItemProperties is called.
                if (!this.hasItemProperties)
                {
                    OnInitializeItemProperties(EventArgs.Empty);
                }

                return itemProperties;
            }

            set
            {
                SetItemProperties(value);
            }
        }

        /// <summary>
        /// Determines if the <see cref="ItemProperties"/> was initalized.
        /// </summary>
        /// <returns>True if the object should be serialized; False otherwise.</returns>
        public bool ShouldSerializeItemProperties()
        {
            return hasItemProperties;
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public void ResetItemProperties()
        {
            this.hasItemProperties = false;
        }

        PropertyDescriptorCollection IItemPropertiesSource.GetItemProperties()
        {
            return ItemProperties;
        }

        /// <summary>
        /// Gets / sets the version of the <see cref="ItemProperties"/> collection. This value
        /// is increased each time the <see cref="ItemProperties"/> collection is modified.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int ItemPropertiesVersion
        {
            get
            {
                return itemPropertiesVersion;
            }

            set
            {
                itemPropertiesVersion = value;
            }
        }

        // Methods
        void AddRelationsToSourceListSet()
        {
        }

        private bool ignoreSetItemProperties = false;

        /// <summary>
        /// Gets or sets a property indicating whether subsequent calls to SetItemProperties on
        /// this TableDescriptor should be ignored. The default is false. You should set this property
        /// true if you manually want to add/merge PropertyDescriptors. It will prevent that
        /// the grid does not reinitialize the ItemProperties with results from calls to 
        /// ITypedList.GetItemProperties later on.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IgnoreSetItemProperties
        {
            get { return ignoreSetItemProperties; }
            set { ignoreSetItemProperties = value; }
        }

        /// <summary>
        /// Initializes the <see cref="ItemProperties"/> collection.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void SetItemProperties(PropertyDescriptorCollection properties)
        {
            if (inSetItemProperties || IgnoreSetItemProperties)
            {
                return;
            }

            if (this.itemProperties != properties)
            {
                if (!this.hasItemProperties)
                {
                    hasItemProperties = true;
                    this.itemProperties = properties;
                    itemPropertiesVersion++;
                }
                else
                {
                    inSetItemProperties = true;
                    if (itemProperties == null || !ListUtil.IsEqualItemProperties(itemProperties, properties))
                    {
                        OnItemPropertiesChanging(EventArgs.Empty);
                        itemPropertiesVersion++;
                        hasItemProperties = true;
                        this.itemProperties = properties;
                        OnItemPropertiesChanged(EventArgs.Empty);
                        if (Engine.HelpTracing)
                        {
                            TraceUtil.TraceCurrentMethodInfo(this);
                            TraceUtil.TraceCalledFrom();
                        }
                    }
                    else
                    {
                        this.itemProperties = properties;
                    }

                    inSetItemProperties = false;
                }
            }

            hasItemProperties = true;
        }

        /// <summary>
        /// Initializes the <see cref="ItemProperties"/> collection from the properties of the specified type.
        /// </summary>
        /// <param name="type">The type with public properties.</param>
        public void SetItemProperties(Type type)
        {
            Reset();
            if (this.name == "Default")
            {
                name = string.Empty;
                nameModified = false;
            }

            this.type = type;
            SetItemProperties(ListUtil.GetItemProperties(type));
        }

        /// <summary>
        /// Initializes the <see cref="ItemProperties"/> collection from the instance properties of the specified list.
        /// </summary>
        /// <param name="list">The list with properties.</param>
        public void SetItemProperties(object list)
        {
            Reset();
            ////            if (list == null)
            //            {
            //                if (this.itemProperties != null && this.itemProperties.Count > 0)
            //                {
            //                    this.itemProperties = PropertyDescriptorCollection.Empty;
            //                    this.itemPropertiesVersion++;
            //                }
            //                return;
            ////            }

            if (this.name == "Default")
            {
                name = ListUtil.GetListName(list);
                nameModified = false;
            }

            this.list = list;
            this.bindingList = list as IBindingList;
            if (bindingList != null && bindingList.SupportsChangeNotification)
            {
                bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
            }

            SetItemProperties(ListUtil.GetItemProperties(list));
        }

        // Protected Methods

        /// <summary>
        /// Raises the <see cref="ListChanging"/> event.
        /// </summary>      
        protected virtual void OnListChanging(ListChangedEventArgs e)
        {
            if (ListChanging != null)
            {
                ListChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event.
        /// </summary>      
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
            {
                ListChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NameChanged"/> event.
        /// </summary>    
        protected virtual void OnNameChanged(EventArgs e)
        {
            if (NameChanged != null)
            {
                NameChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NameChanging"/> event.
        /// </summary>   
        protected virtual void OnNameChanging(EventArgs e)
        {
            if (NameChanging != null)
            {
                NameChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ItemPropertiesChanging"/> event.
        /// </summary>     
        protected virtual void OnItemPropertiesChanging(EventArgs e)
        {
            if (!inbindingList_ListChanged)
            {
                OnListChanging(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
            }

            if (ItemPropertiesChanging != null)
            {
                ItemPropertiesChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ItemPropertiesChanged"/> event.
        /// </summary>      
        protected virtual void OnItemPropertiesChanged(EventArgs e)
        {
            if (ItemPropertiesChanged != null)
            {
                ItemPropertiesChanged(this, e);
            }

            if (!inbindingList_ListChanged)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
            }
        }

        /// <summary>
        /// Resets this object and clears cached properties.
        /// </summary>
        public override void Reset()
        {
            this.type = null;
            this.list = null;
            if (bindingList != null && bindingList.SupportsChangeNotification)
            {
                bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
            }

            this.bindingList = null;
            if (!this.nameModified)
            {
                this.name = "Default";
                this.nameModified = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="InitializeItemProperties"/> event.
        /// </summary>    
        protected virtual void OnInitializeItemProperties(EventArgs e)
        {
            if (InitializeItemProperties != null)
            {
                InitializeItemProperties(this, e);
            }
        }

        #region ITypedList Members

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return this.ItemProperties;
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return Name;
        }

        #endregion
    }
}