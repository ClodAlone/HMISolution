#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.UI.Xaml.Maps
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
#if WINRT
    using Windows.Foundation;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Represents the MapLayerCommandBase. Inherits form <see cref="ICommand"/>
    /// </summary>
    /// <remarks>
    /// MapLayerCommandBase is base class for all commands in the map. It is inherited from <see cref="ICommand"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public abstract class MapLayerCommandBase : ICommand
    {
        private SfMap mapControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapLayerCommandBase">MapLayerCommandBase</see> class. 
        /// </summary>
        /// <param name="mapControl">SfMap which is having commands and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="MapLayerCommandBase"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public MapLayerCommandBase(SfMap mapControl)
        {
            this.mapControl = mapControl;
        }

        /// <summary>
        /// Gets the SfMap for the command.
        /// </summary>
        /// <value>Type :<see cref="SfMap"/>SfMap which is having Commands and its implementations</value>
        /// <remarks>
        /// This property is used to get the SfMap, which is having and  implementations of the commands.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public SfMap MapControl
        {
            get { return this.mapControl; }
        }

        #region ICommand Members

        ///<summary>
        /// Determines whether this Command can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the current command target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
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
        /// Occurs when CanExecute value is changed. 
        /// </summary>
        /// <remarks>
        /// This is the event handler to handle the execution when CanExecute value is changed.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public event EventHandler CanExecuteChanged;

        ///<summary>
        /// Executes the Command on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the handler</para>
        /// </param>
        [ClassReference(IsReviewed = false)]
        public abstract void Execute(object parameter);

        #endregion
    }

    /// <summary>
    /// Represents the ShapeFileResetCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the ShapeFileResetCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ShapeFileResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeFileResetCommand">ShapeFileResetCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ShapeFileResetCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="ShapeFileResetCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ShapeFileResetCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether ShapeFileResetCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the ShapeFileResetCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the ShapeFileResetCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ShapeFileResetCommand handler.</para>
        /// </param>
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as ShapeFileLayer).Reset();
        }
    }

    /// <summary>
    /// Represents the ShapeFileRefreshCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the ShapeFileRefreshCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ShapeFileRefreshCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeFileRefreshCommand">ShapeFileRefreshCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ShapeFileRefreshCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="ShapeFileRefreshCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ShapeFileRefreshCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether ShapeFileRefreshCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the ShapeFileRefreshCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            if (this.MapControl.LayeredContent != null)
            {
                if ((MapControl.LayeredContent as ShapeFileLayer).MapShapes.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        ///<summary>
        /// Executes the ShapeFileRefreshCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ShapeFileRefreshCommand handler.</para>
        /// </param>         
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            (this.MapControl.LayeredContent as ShapeFileLayer).Refresh();
        }
    }


    /// <summary>
    /// Represents the PanResetCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the PanResetCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class PanResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.PanResetCommand">PanResetCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ShapeFileRefreshCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="PanResetCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public PanResetCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether PanResetCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the PanResetCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the PanResetCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the PanResetCommand handler.</para>
        /// </param>         
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            if (this.MapControl.LayeredContent is ShapeFileLayer)
            {
                if (MapControl.EnablePan)
                {
                    this.MapControl.startPoint = new Point(0, 0);
                    this.MapControl.Pan(0, 0);
                }
            }
        }
    }

    /// <summary>
    /// Represents the ZoomResetCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the ZoomResetCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ZoomResetCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ZoomResetCommand">ZoomResetCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ZoomResetCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="ZoomResetCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ZoomResetCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether ZoomResetCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the ZoomResetCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the ZoomResetCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ZoomResetCommand handler.</para>
        /// </param> 
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            if (MapControl.EnablePan)
            {
                this.MapControl.ZoomLevel = this.MapControl.MinZoom;
            }
            if ((this.MapControl.LayeredContent as ShapeFileLayer) != null)
                this.MapControl.panTranslatePoint = new Point((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);
#if WINRT
            if (MapControl.MapView == MapViews.SmartView)
            {
#if SyncfusionFramework4_5_1
                this.MapControl.scrollContent.ChangeView(null, null, MapControl.MinZoom);
#else
                this.MapControl.scrollContent.ZoomToFactor(this.MapControl.MinZoom);
#endif
            }
#endif
        }
    }

    /// <summary>
    /// Represents the ZoomInCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the ZoomInCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ZoomInCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ZoomInCommand">ZoomInCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ZoomInCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="ZoomInCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ZoomInCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether ZoomInCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the ZoomInCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the ZoomInCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ZoomInCommand handler.</para>
        /// </param> 
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            if (this.MapControl.EnableZoom)
            {
                if (this.MapControl.LayeredContent is ShapeFileLayer)
                {
                    this.MapControl.ZoomLevel += 1;
                }
                this.MapControl.panTranslatePoint = new Point((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);


            }
        }
    }

    /// <summary>
    /// Represents the ZoomOutCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the ZoomOutCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ZoomOutCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ZoomOutCommand">ZoomOutCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having ZoomOutCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="ZoomOutCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ZoomOutCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether ZoomOutCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the ZoomOutCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the ZoomOutCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ZoomOutCommand handler.</para>
        /// </param> 
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {
            if (this.MapControl.EnableZoom)
            {
                if (this.MapControl.LayeredContent is ShapeFileLayer)
                {
                    this.MapControl.ZoomLevel -= 1;
                }
                this.MapControl.panTranslatePoint = new Point((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);


            }

        }
    }

    /// <summary>
    /// Represents the PanCommand class in the SfMap. It is inherited from <see cref="MapLayerCommandBase"/>.
    /// </summary>
    /// <remarks>
    ///  This class represents the PanCommand in the map. Also it is inherited from <see cref="MapLayerCommandBase"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class PanCommand : MapLayerCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.PanCommand">PanCommand</see> class. 
        /// </summary>
        ///  <param name="mapControl">SfMap which is having PanCommand and its implementations.</param>      
        /// <remarks>
        /// Initialize the MapControl property of the <see cref="PanCommand"/>.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public PanCommand(SfMap mapControl)
            : base(mapControl)
        {
        }

        ///<summary>
        /// Determines whether PanCommand can execute in its current state.
        /// </summary> 
        /// <param name="parameter">
        /// Type :<see cref="object"/>
        /// <para>object to determine can execute.</para>
        /// </param>
        /// <returns>
        /// Type :<see cref="System.Boolean"/>
        /// <para>true if the command can execute on the PanCommand target; otherwise, false.</para>
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        /// Executes the ZoomOutCommand on the  target.
        /// </summary>
        /// <param name="parameter">
        /// Type :<see cref="System.Object"/>
        /// <para>Parameter to be passed to the ZoomOutCommand handler.It will reoresent the direction to be panned.</para>
        /// </param> 
        [ClassReference(IsReviewed = false)]
        public override void Execute(object parameter)
        {

            if (MapControl.EnablePan)
            {
                string dirction = parameter.ToString();
                this.MapControl.startPoint = new Point(0, 0);
                if (dirction.ToLower() == "left")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        this.MapControl.Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X - 5, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);
                    }
                    PanEventArgs args = new PanEventArgs(0d, 0d, PanMode.Left);
                    this.MapControl.OnPanning(this, args);


                }
                else if (dirction.ToLower() == "right")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        this.MapControl.Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X + 5, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);
                    }
                    PanEventArgs args = new PanEventArgs(0d, 0d, PanMode.Right);
                    this.MapControl.OnPanning(this, args);

                }
                else if (dirction.ToLower() == "top")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        this.MapControl.Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y - 5);
                    }
                    PanEventArgs args = new PanEventArgs(0d, 0d, PanMode.Top);
                    this.MapControl.OnPanning(this, args);

                }
                else if (dirction.ToLower() == "bottom")
                {
                    if (this.MapControl.LayeredContent is ShapeFileLayer)
                    {
                        this.MapControl.Pan((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y + 5);
                    }
                    PanEventArgs args = new PanEventArgs(0d, 0d, PanMode.Bottom);
                    this.MapControl.OnPanning(this, args);

                }
                this.MapControl.panTranslatePoint = new Point((this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.MapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);

            }
        }
    }
}

