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

using System;
using System.Diagnostics;

namespace Syncfusion.Styles
{
    /// <summary>
    /// Provides style identity information for subobjects.
    /// </summary>
	[DebuggerStepThrough()]
    public class StyleInfoSubObjectIdentity : StyleInfoIdentityBase
    {
        StyleInfoBase owner;
        StyleInfoProperty sip;

        /// <summary>
        /// Releases all the resources used by the component.
        /// </summary>
        public override void Dispose()
        {
            owner = null;
            sip = null;
            base.Dispose();
        }

        /// <summary>
        /// Returns the owner style of the subobject.
        /// </summary>
        public StyleInfoBase Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Returns the identifier of the subobject in the owner object.
        /// </summary>
        public StyleInfoProperty Sip
        {
            get { return sip; }
        }

        /// <summary>
        /// Instantiates a new <see cref="StyleInfoSubObjectIdentity"/> for a 
        /// given owner and <see cref="StyleInfoProperty"/>.
        /// </summary>
        /// <param name="owner">The owner style of the sub object.</param>
        /// <param name="sip">The identifier of the subobject in the owner object.</param>
        public StyleInfoSubObjectIdentity(StyleInfoBase owner, StyleInfoProperty sip)
        {
            this.owner = owner;
            this.sip = sip;
        }

        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            // get base styles and then get the GridFontInfo for each of the base styles.
            IStyleInfo[] baseStyles = null;
            if (owner.Identity != null)
                baseStyles = owner.Identity.GetBaseStyles(owner);
            if (baseStyles != null)
            {
                IStyleInfo[] subObject = new StyleInfoBase[baseStyles.Length + 1];
                int l = 0;
                foreach (StyleInfoBase baseStyle in baseStyles)
                    if (baseStyle.HasValue(sip))
                        subObject[l++] = (StyleInfoBase)baseStyle.GetValue(sip);

                StyleInfoBase sb = owner.GetDefaultStyle();
                if (sb != null && sb.HasValue(sip))
                    subObject[l++] = (StyleInfoBase)sb.GetValue(sip);

                if (l == subObject.Length)
                    return subObject;

                StyleInfoBase[] fi = new StyleInfoBase[l];
                if (l > 0)
                    Array.Copy(subObject, 0, fi, 0, l);
                return fi;
            }
            return null;
        }
    }
}
