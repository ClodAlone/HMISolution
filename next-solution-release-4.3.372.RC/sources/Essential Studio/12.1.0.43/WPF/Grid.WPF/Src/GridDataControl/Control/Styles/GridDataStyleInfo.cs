#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Styles;

    public class GridDataStyleInfo : GridStyleInfo
    {
        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridDataStyleInfoStore"/> that holds data for this <see cref="GridDataStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridDataStyleInfoStore"/> object.</param>
        public GridDataStyleInfo(GridStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridDataStyleInfoIdentity"/> that holds the indentity for this <see cref="GridDataStyleInfo"/>.
        /// </param>
        public GridDataStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridDataStyleInfoIdentity"/> that holds the indentity for this <see cref="GridDataStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridDataStyleInfoStore"/> that holds data for this <see cref="GridDataStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridDataStyleInfoStore"/> object.
        /// </param>
        public GridDataStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        public GridDataStyleInfo()
            : base(new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        public GridDataStyleInfo(GridDataStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Holds identity information such as row and column index for the current <see cref="GridDataStyleInfo"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
#if !SILVERLIGHT
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public new GridDataTableStyleInfoIdentity CellIdentity
        {
            get
            {
                return this.Identity as GridDataTableStyleInfoIdentity;
            }

            set
            {
                this.Identity = value;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (CellIdentity != null)
            {
                CellIdentity.Dispose();
                CellIdentity = null;
            }
        }

        public new GridDataTableModel GridModel
        {
            get
            {
                GridDataTableStyleInfoIdentity cellIdentity = this.CellIdentity;
                if (cellIdentity != null)
                {
                    return this.CellIdentity.GridModel as GridDataTableModel;
                }

                return null;
            }
        }

        /// <summary>
        /// The <see cref="GridDataStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        // [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // public new GridDataStyleInfoStore Store
        // {
        //    get { return (GridDataStyleInfoStore)base.Store; }
        // }

        /// <override/>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Creates a new <see cref="GridDataStyleInfo"/> and copies its cell and identity information from the current object. The new
        /// instance will be made offline so that changes in this style object are not be stored in the GridData
        /// </summary>
        /// <returns>A new <see cref="GridDataStyleInfo"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        public new GridDataStyleInfo GetOffLineCopy()
        {
            return new GridDataStyleInfo(((GridDataTableStyleInfoIdentity)Identity).MakeOfflineIdentity(), (GridStyleInfoStore)this.Store.Clone());
        }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    [StaticDataField("sd")]
    public class GridDataStyleInfoStore : GridStyleInfoStore
    {
        private static StaticData sd = new StaticData(typeof(GridDataStyleInfoStore), typeof(GridDataStyleInfo), false);

        /// <overload>
        /// Initializes a <see cref="GridDataStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDataStyleInfoStore"/>.
        /// </summary>
        public GridDataStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridDataStyleInfo();
            }
        }

#if !SyncfusionFramework4_0 && !SILVERLIGHT
        /// <summary>
        /// Initializes a new <see cref="GridDataStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDataStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
            {
                new GridDataStyleInfo();
            }
        }
#endif

        //internal new static StaticData StaticData
        //{
        //    get
        //    {
        //        return sd;
        //    }
        //}

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new GridDataStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}
