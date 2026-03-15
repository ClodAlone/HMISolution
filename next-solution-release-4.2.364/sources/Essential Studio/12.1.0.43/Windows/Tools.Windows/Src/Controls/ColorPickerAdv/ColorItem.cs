#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;

using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    [Serializable]
    public enum ColorItemState
    {
        /// <summary>
        /// Defines ColorItemstate Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Defines ColorItemstate Highlighted
        /// </summary>
        Highlighted,

        /// <summary>
        /// Defines ColorItemstate Selected
        /// </summary>
        Selected
    }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	[ToolboxItem(false)]
	[DesignTimeVisible(false)]	
    public class ColorItemBase : Component
    { }
#else
    public class ColorItemBase
    { 
    }
#endif

    [Serializable]
    [ToolboxItem(false)]
    [TypeConverter(typeof(ColorItemTypeConverter))]
    public class ColorItem :
        ColorItemBase
    {
        private Color m_color = Color.Empty;
        public Color Color
        {
            get 
            {
                return m_color; 
            }
            set
            {
                if (m_color != value)
                {
                    m_color = value;

                    this.OnColorChanged(EventArgs.Empty);
                }
            }
        }

        private Rectangle m_bounds = Rectangle.Empty;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Bounds
        {
            get 
            {
                return m_bounds; 
            }
            set
            {
                if (m_bounds != value)
                    m_bounds = value;
            }
        }

        private ColorItemState m_state = ColorItemState.Normal;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorItemState State
        {
            get 
            {
                return m_state; 
            }
            set
            {
                if (m_state != value)
                    m_state = value;
            }
        }

        private int m_index = -1;
        [DefaultValue(-1)]
        [Browsable(false)]
        public int Index
        {
            get 
            {
                return m_index; 
            }
            set
            {
                if (m_index != value)
                {
                    m_index = value;
                }
            }
        }

        private GroupColorItem m_baseItem;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal GroupColorItem BaseItem
        {
            get { return m_baseItem; }
        }

        public event EventHandler ColorChanged;

        protected virtual void OnColorChanged(EventArgs e)
        {
            if (this.ColorChanged != null)
                this.ColorChanged(this, e);
        }

        public ColorItem()
            : this(null, Color.White)
        {
        }

        public ColorItem(Color color)
            : this(null, color)
        {
        }

        public ColorItem(GroupColorItem baseItem, Color color)
        {
            m_color = color;
            m_baseItem = baseItem;
        }
    }

    [Serializable]
    [TypeConverter(typeof(GroupColorItemTypeConverter))]
    public class GroupColorItem :
        ColorItem
    {
        private ColorItemCollection m_inheritColorsCollection = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ColorItemCollection SubItems
        {
            get
            {
                if (m_inheritColorsCollection == null)
                    m_inheritColorsCollection = new ColorItemCollection(this.Group, false);

                return m_inheritColorsCollection;
            }
            set
            {
                if (m_inheritColorsCollection != value)
                    m_inheritColorsCollection = value;
            }
        }

        private ColorUIAdvGroup m_group = null;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorUIAdvGroup Group
        {
            get 
            { 
                return m_group; 
            }
            set
            {
                if (m_group != value)
                    m_group = value;
            }
        }

        public GroupColorItem(ColorUIAdvGroup group, Color color)
            : base(color)
        {
            m_group = group;

            if (group != null)
                m_inheritColorsCollection = new ColorItemCollection(group, false);
        }
    }

    public class GroupColorItemTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            GroupColorItem item = (GroupColorItem)value;

            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                if (item != null)
                {
                    if (item.Group != null)
                    {
                        System.Reflection.ConstructorInfo ci = typeof(GroupColorItem).GetConstructor( new Type[] { typeof(ColorUIAdvGroup), typeof(Color) });
                        System.ComponentModel.Design.Serialization.InstanceDescriptor descriptor = new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { item.Group, item.Color }, false);
                        return descriptor;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
    }

    [Serializable]
    [Editor(typeof(ColorItemCollectionEditor), typeof(UITypeEditor))]
    public class ColorItemCollection : CollectionBase
    {
        #region Members

        private ColorUIAdvGroup m_colorGroup;
        private bool m_bIsbaseCollection = false;

        #endregion

        #region Properties
        [DefaultValue(false)]
        public bool IsBaseCollection
        {
            get { return m_bIsbaseCollection; }
        }

        public ColorUIAdvGroup Group
        {
            get { return m_colorGroup; }
        }
        #endregion

        #region Events

        public event EventHandler CollectionChanged;

        #endregion

        protected void OnCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        #region Constructors

        public ColorItemCollection(ColorUIAdvGroup group, bool isBaseCollection)
        {
            if (group == null)
            {
                throw new NullReferenceException("ColorUIAdvGroup can't be NULL");
            }

            m_colorGroup = group;
            m_bIsbaseCollection = isBaseCollection;
        }

        public ColorItemCollection()
        {
        }

        #endregion

        #region Indexer

        public ColorItem this[int index]
        {
            get
            {
                return (ColorItem)this.List[index];
            }
            set
            {
                if (index < 0 || index >= this.List.Count)
                {
                    throw new IndexOutOfRangeException("index");
                }
                if (value == null)
                {
                    throw new NullReferenceException("value can't be NULL");
                }

                if (this.List[index] != value)
                {
                    this.List[index] = value;
                }
            }
        }
        #endregion

        #region Methods

        public void Add(ColorItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item can't be NULL");
            }

            this.List.Add(item);
        }

        public bool Contains(ColorItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item can't be NULL");
            }

            return this.List.Contains(item);
        }

        public void Remove(ColorItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item can't be NULL");
            }

            if (!this.Contains(item))
            {
                throw new NullReferenceException("Item doesn't exist in collection");
            }

            item.ColorChanged -= new EventHandler(Item_ColorChanged);

            this.List.Remove(item);
        }

        public int IndexOf(ColorItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item can't be NULL");
            }

            return this.List.IndexOf(item);
        }

        public void Insert(int index, ColorItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("Item can't be NULL");
            }

            if (index < 0 || index >= this.List.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            this.List.Insert(index, item);
        }

        #endregion

        #region Overrides

        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);

            ColorItem item = (ColorItem)value;

            if (m_colorGroup.GroupType == ColorUIAdvGroups.RecentColors)
                item.Index = 0;
            else
                if (item.Index == -1)
                    item.Index = this.Count;

            item.ColorChanged += new EventHandler(Item_ColorChanged);

            this.OnCollectionChanged();
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            m_colorGroup.UpdateGroupSize();

            if (!this.IsBaseCollection && m_colorGroup.ParentControl != null)
                m_colorGroup.ParentControl.Invalidate();
        }

        #endregion

        private void Item_ColorChanged(object sender, EventArgs args)
        {
            ColorItem item = sender as ColorItem;
            GroupColorItem baseItem = sender as GroupColorItem;

            ColorUIAdvGroup group;

            if (baseItem != null)
                group = baseItem.Group;
            else
                group = item.BaseItem.Group;

            if ((group.GroupType == ColorUIAdvGroups.ThemeColors || group.GroupType == ColorUIAdvGroups.StandardColors) &&
                !m_colorGroup.ParentControl.ChangedItems.ContainsKey(item))
                m_colorGroup.ParentControl.ChangedItems.Add(item, group);

            m_colorGroup.ParentControl.Invalidate(item.Bounds);
        }
    }

    public class ColorItemTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                ColorItem item = value as ColorItem;

                if (item != null)
                {
                    if (item.BaseItem != null)
                    {
                        System.Reflection.ConstructorInfo ci = typeof(ColorItem).GetConstructor(
                        new Type[] { typeof(GroupColorItem), typeof(Color) });

                        return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { item.BaseItem, item.Color });
                    }
                    else
                    {
                        System.Reflection.ConstructorInfo ci = typeof(ColorItem).GetConstructor(new Type[] { typeof(Color) });

                        return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { item.Color });
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
    }

    public class ColorItemCollectionEditor : CollectionEditor
    {
        private ColorUIAdvGroup m_group = null;
        private GroupColorItem m_baseItem = null;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                if (context.Instance is ColorUIAdvGroup)
                    m_group = context.Instance as ColorUIAdvGroup;
                else if (context.Instance is GroupColorItem)
                    m_baseItem = context.Instance as GroupColorItem;
            }

            return base.EditValue(context, provider, value);
        }

        public ColorItemCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            if (m_group != null)
                return new GroupColorItem(m_group, Color.White);
            else
                return new ColorItem(m_baseItem, Color.White);
        }
    }
}