#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    /// <summary>
    /// This contains Commands of Maplayers
    /// </summary>
    public abstract class MapLayerCommandBase : ICommand
    {
        private MapControl mapControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapLayerCommandBase"/> class.
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        public MapLayerCommandBase(MapControl mapControl)
        {
            this.mapControl = mapControl;
        }

        /// <summary>
        /// Gets Map control instance.
        /// </summary>
        /// <value>
        /// MapControl
        /// </value>
        public MapControl MapControl
        {
            get { return this.mapControl; }
        }

        #region ICommand Members

        /// <summary>
        ///  This method is called when Command is Invoked
        /// </summary>
        /// <param name="parameter">Object</param>
        /// <returns>
        /// Bool
        /// </returns>
        public virtual bool CanExecute(object parameter)
        {
            var canExecuteHandler = this.CanExecuteChanged;
            if (canExecuteHandler != null)
            {
                canExecuteHandler(this, EventArgs.Empty);
            }

            return true;
        }

        /// <summary>
        /// Occurs when CanExecute method is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///  This method executes when CanExecuteChanged event is triggered
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);

        #endregion
    }

    /// <summary>
    ///  ShapeFileResetCommand reset the shapefiles in the Maps
    /// </summary>
    public class ShapeFileResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFileResetCommand"/> class.
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        public ShapeFileResetCommand(MapControl mapControl)
            : base(mapControl)
        {
        }

        /// <summary>
        ///  This method is invoked whenever ShapeFileResetCommand is called
        /// </summary>
        /// <param name="parameter">Object</param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        ///  This method is invoked whenever ShapeFileResetCommand is called
        /// </summary>
        /// <param name="parameter">Object</param>
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as ShapeFileLayer).Reset();
        }
    }
    /// <summary>
    ///  ShapeFileRefreshCommand clears the shapefiles in the Maps
    /// </summary>
    public class ShapeFileRefreshCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFileRefreshCommand"/> class.
        /// </summary>
        /// <param name="mapControl">Map Control</param>
        public ShapeFileRefreshCommand(MapControl mapControl)
            : base(mapControl)
        {
        }

        /// <summary>
        ///  This method is invoked whenever ShapeFileRefreshCommand is called
        /// </summary>
        /// <param name="parameter">Object</param>
        public override bool CanExecute(object parameter)
        {
            if (this.MapControl.LayeredContent != null)
            {
                return (this.MapControl.LayeredContent as ShapeFileLayer).HasFileData;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  This method is invoked whenever ShapeFileRefreshCommand is called
        /// </summary>
        /// <param name="parameter">Object</param>
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as ShapeFileLayer).Refresh();
        }
    }

    /// <summary>
    ///  This command helps to check whether the selected Item is null or not
    /// </summary>
    public class ShapeFileSelectedItemNullCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFileSelectedItemNullCommand"/>
        /// class.
        /// </summary>
        /// <param name="mapControl">Map control</param>
        public ShapeFileSelectedItemNullCommand(MapControl mapControl)
            : base(mapControl)
        {
        }

        /// <summary>
        ///  This method is invoked whenever ShapeFileSelectedItemNullCommand is called
        /// </summary>
        /// <param name="parameter">MapItem</param>
        public override bool CanExecute(object parameter)
        {
            if (this.MapControl.LayeredContent != null)
            {
                return (this.MapControl.LayeredContent as ShapeFileLayer).SelectedItem != null;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is invoked whenever ShapeFileSelectedItemNullCommand is called
        /// </summary>
        /// <param name="parameter">MapItem</param>
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as ShapeFileLayer).SetSelectedItem(null);
        }
    }

    /// <summary>
    ///  It helps to Resets all the panning of map
    /// </summary>
    public class PanResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.PanResetCommand"/> class.
        /// </summary>
        /// <param name="mapControl">Map control</param>
        public PanResetCommand(MapControl mapControl)
            : base(mapControl)
        {
        }

        /// <summary>
        /// This method is invoked whenever PanResetCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>

        public override bool CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// This method is invoked whenever PanResetCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override void Execute(object parameter)
        {
            if (this.MapControl.LayeredContent is ShapeFileLayer)
            {
                (this.MapControl.LayeredContent as ShapeFileLayer).Pan(0, 0);
            }
            else if ((this.MapControl.LayeredContent is ImageryLayer))
            {
                (this.MapControl.LayeredContent as ImageryLayer).PanMap(0, 0);
            }
        }
    }


    /// <summary>
    ///  It helps to Resets  the Zooming of the map
    /// </summary>
    public class ZoomResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ZoomResetCommand"/> class.
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        public ZoomResetCommand(MapControl mapControl)
            : base(mapControl)
        {
        }
        /// <summary>
        /// This method is invoked whenever ZoomResetCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// This method is invoked whenever ZoomResetCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as MapLayer).ZoomLevel = 1;
        }
    }

    /// <summary>
    ///  This method helps to Zoom or Enlarge the map. Without Mouse Interaction Zooming
    /// can be done by this command
    /// </summary>
    public class ZoomInCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ZoomInCommand"/> class.
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        public ZoomInCommand(MapControl mapControl)
            : base(mapControl)
        {
        }

        /// <summary>
        /// This method is invoked whenever ZoomInCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// This method is invoked whenever ZoomInCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override void Execute(object parameter)
        {
            if (this.MapControl.EnableZoom)
            {
                if (this.MapControl.LayeredContent is ShapeFileLayer)
                {
                    (this.MapControl.LayeredContent as ShapeFileLayer).ZoomLevel += 0.5;
                }
                else if (this.MapControl.LayeredContent is ImageryLayer)
                {
                    (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel += 1;
                }
            }
        }
    }

    /// <summary>
    ///  This method helps to minimize zoom level of  the map. Without Mouse Interaction
    /// Zooming can be done by this command
    /// </summary>
    public class ZoomOutCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ZoomOutCommand"/> class.
        /// </summary>
        /// <param name="mapControl">Map Control</param>
        public ZoomOutCommand(MapControl mapControl)
            : base(mapControl)
        {
        }
        /// <summary>
        /// This method is invoked whenever ZoomOutCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// This method is invoked whenever ZoomOutCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override void Execute(object parameter)
        {
            if (this.MapControl.EnableZoom)
            {
                if (this.MapControl.LayeredContent is ShapeFileLayer)
                {
                    (this.MapControl.LayeredContent as ShapeFileLayer).ZoomLevel -= 0.5; ;
                }
                else if (this.MapControl.LayeredContent is ImageryLayer)
                {
                    (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel -= 1;
                }
            }
           
        }
    }
    /// <summary>
    ///  This method helps to Pan the map in any direction. Without Mouse Interaction
    /// Panning can be done by this command
    /// </summary>
    public class PanCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.PanCommand"/> class.
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        public PanCommand(MapControl mapControl)
            : base(mapControl)
        {
        }
        /// <summary>
        /// This method is invoked whenever PanCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// This method is invoked whenever PanCommand is called
        /// </summary>
        /// <param name="parameter">MapControl</param>
        public override void Execute(object parameter)
        {
            if (MapControl.EnablePan)
            {
                string dirction = parameter.ToString();
                if (dirction.ToLower() == "left")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        (this.MapControl.LayeredContent as ShapeFileLayer).Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X - 5, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);
                    }
                    else if (this.MapControl.LayeredContent is ImageryLayer)
                    {
                        (this.MapControl.LayeredContent as ImageryLayer).PanMap((this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.X - (100 * (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel), (this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.Y);
                    }
                }
                else if (dirction.ToLower() == "right")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        (this.MapControl.LayeredContent as ShapeFileLayer).Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X + 5, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);
                    }
                    else if (this.MapControl.LayeredContent is ImageryLayer)
                    {
                        (this.MapControl.LayeredContent as ImageryLayer).PanMap((this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.X + (100 * (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel), (this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.Y);
                    }
                }
                else if (dirction.ToLower() == "top")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        (this.MapControl.LayeredContent as ShapeFileLayer).Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y - 5);
                    }
                    else if (this.MapControl.LayeredContent is ImageryLayer)
                    {
                        (this.MapControl.LayeredContent as ImageryLayer).PanMap((this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.X, (this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.Y - (100 * (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel));
                    }
                }
                else if (dirction.ToLower() == "bottom")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        (this.MapControl.LayeredContent as ShapeFileLayer).Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y + 5);
                    }
                    else if (this.MapControl.LayeredContent is ImageryLayer)
                    {
                        (this.MapControl.LayeredContent as ImageryLayer).PanMap((this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.X, (this.MapControl.LayeredContent as ImageryLayer).bingmapPanTransform.Y + (100 * (this.MapControl.LayeredContent as ImageryLayer).ZoomLevel));
                    }
                }
            }
        }
    }
}

