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

namespace Syncfusion.Styles
{
    /// <summary>
    /// Provides style identity information for nested expandable objects of the GridStyleInfo and TreeStyleInfo classes.
    /// </summary>
    public class CachedStyleInfoSubObjectIdentity : StyleInfoSubObjectIdentity
    {
        // Cache
        IStyleInfo[] cachedBaseStyles = null;

        /// <summary>
        /// Creates a new <see cref="StyleInfoSubObjectIdentity"/> object and associates it with a <see cref="StyleInfoBase"/>.
        /// </summary>
        /// <param name="owner">The <see cref="StyleInfoBase"/> that owns this subobject.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> descriptor for this expandable subobject.</param>
        public CachedStyleInfoSubObjectIdentity(StyleInfoBase owner, StyleInfoProperty sip)
            : base(owner, sip)
        {
        }

        /// <override/>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null)
                cachedBaseStyles = base.GetBaseStyles(thisStyleInfo);
            return cachedBaseStyles;
        }

        /// <override/>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
        }
    }
}
