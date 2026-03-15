// <copyright file="HubTileService.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
namespace Syncfusion.Windows.Controls.Notification
#else
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// HiubTileService that provides the helper methods to <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
    /// </summary>
    /// <remarks>
    /// Hub Tile Service is a class which provides the helper methods to freeze and
    /// unfreeze the animation by passing HubTile instance or GroupName as argument.
    /// </remarks>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
#if WPF
    [CLSCompliant(false)]
#endif
    public static class HubTileService
    {
        #region Variables

        private static List<WeakReference> Tiles = new List<WeakReference>();

        #endregion

        #region Helper Methods

        internal static void Enqueue(HubTileBase tile)
        {
            WeakReference reference = new WeakReference(tile, false);
            Tiles.Add(reference);
        }

        internal static void Dequeue(HubTileBase tile)
        {
            foreach (WeakReference reference in Tiles)
            {
                if (reference.Target == tile)
                {
                    Tiles.Remove(reference);
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Freeze the particular <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
        /// </summary>
        /// <remarks>
        /// The Return value is void.
        /// </remarks>
        /// <param name="tile">one of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.</param>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(System.String)">UnFreeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(System.String)">Freeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">UnFreezee(HubTile)</seealso>
        public static void Freeze(HubTileBase tile)
        {
            foreach (WeakReference reference in Tiles)
            {
                if (reference.Target == tile)
                {
                    HubTileBase hubtile = reference.Target as HubTileBase;
                    hubtile.IsFrozen = true;
                    break;
                }
            }
        }

        /// <summary>
        ///  Freeze the particular group of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile">HubTile&apos;s</see>
        /// </summary>
        /// <param name="groupname">name of the group.</param>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(System.String)">UnFreeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">Freeze(HubTile)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">UnFreezee(HubTile)</seealso>
        public static void Freeze(string groupname)
        {
            foreach (WeakReference reference in Tiles)
            {
                HubTileBase tile = reference.Target as HubTileBase;
                if (tile != null && tile.GroupName == groupname)
                {
                    tile.IsFrozen = false;
                }
            }
        }
        /// <summary>
        /// Unfreeze the particular <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
        /// </summary>
        /// <remarks>
        /// The Return value is void.
        /// </remarks>
        /// <param name="tile">one of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.</param>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(System.String)">Freeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(System.String)">UnFreeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">Freeze(HubTile)</seealso>
        public static void UnFreeze(SfHubTile tile)
        {
            foreach (WeakReference reference in Tiles)
            {
                if (reference.Target == tile)
                {
                    HubTileBase hubtile = reference.Target as HubTileBase;
                    hubtile.IsFrozen = false;
                    break;
                }
            }
        }

        /// <summary>
        ///  UnFreeze the particular group of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile">HubTile&apos;s</see>
        /// </summary>
        /// <param name="groupname">name of the group.</param>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(System.String)">Freeze(String)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.Freeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">Freeze(HubTile)</seealso>
        /// <seealso
        /// cref="M:Syncfusion.UI.Xaml.Controls.Notification.HubTileService.UnFreeze(Syncfusion.UI.Xaml.Controls.Notification.HubTile)">UnFreezee(HubTile)</seealso>
        public static void UnFreeze(string groupname)
        {
            foreach (WeakReference reference in Tiles)
            {
                HubTileBase tile = reference.Target as HubTileBase;
                if (tile != null && tile.GroupName == groupname)
                {
                    tile.IsFrozen = false;
                }
            }
        }

        #endregion
    }
}
