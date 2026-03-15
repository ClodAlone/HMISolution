//-------------------------------------------------------------------------------------------------
// <copyright file="GridEngineFactory.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    
    /*/// <remarks>
    /// The virtual GridGroupingControl.CreateEngine function by default calls GridEngineFactory.CreateEngine
    /// to create an engine object.
    /// <para/>
    /// There are multiple alternatives that let you customize the engine object that is used within a GridGroupingControl.
    /// <list type="bullet"/>
    /// <remarks/>*/
#if ASPNET
    /// <item><term>
    /// To attach a custom GridEngine to a single grid control, listen to that grid control's CreateEngine event and provide
    /// the custom GridEngine instance in the handler. Note that the engine will be created as soon as someone references the GridControl.Engine
    /// property, which happens a lot in our implementation. So, subscribe to the CreateEngine event as soon as possible. Look at our
    /// CustomSectionInGroups sample for a good pattern. Subscribing in InitializeComponent is usally too late.
    /// </term></item>
#endif
    /*/// <item><term>
    /// A global solution for all controls in your app is to derive a class from GridEngineFactoryBase,
    /// override CreateEngine, and assign an instance of the derived GridEngineFactoryBase object to GridEngineFactory.Factory
    /// </term></item>
    /// <item><term>
    /// Instantiate manually a GridEngine object and call the constructor of GridGroupingControl, passing in the engine
    /// object as parameter.
    /// </term></item>
    /// <item><term>
    /// Derive GridGroupingControl and override the virtual */
#if ASPNET
    /// OnCreateEngine method.
#else
    /*/// CreateEngine method.*/
#endif
    /// <term/><item/>
    /// <list/>
    /// <para/>
    /// In your derived engine class you can then further customize classes that are created. The GridEngine class
    /// contains virtual factory methods such as <see cref="GridEngine.CreateTableControl"/> or <see cref="GridEngine.CreateTable"/>
    /// that are called to create objects that belong to a GridGroupingControl
    /// and its engine elements.
    /// <remarks/>
    /// <seealso cref="GridEngineFactory"/>
    ///  <summary>
    /// A global factory class with the purpose of instantiating a GridEngine object.
    /// </summary>
    public sealed class GridEngineFactory
    {
        [ThreadStatic]
        static GridEngineFactoryBase factory;

        /// <summary>
        /// Occurs when the <see cref="Factory"/> property was changed.
        /// </summary>
        public static event EventHandler FactoryChanged;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridEngineFactory()
        {
        }

        /// <summary>
        /// Lets you assign an application-wide global instance of a derived
        /// GridEngineFactoryBase object with an overriden GridEngineFactoryBase.CreateEngine
        /// factory method.
        /// </summary>
        public static GridEngineFactoryBase Factory
        {
            get
            {
                if (factory == null)
                {
                    factory = new GridEngineFactoryBase(true);
                }

                return factory;
            }

            set
            {
                if (factory != value)
                {
                    factory = value;
                    if (FactoryChanged != null)
                    {
                        FactoryChanged(typeof(GridEngineFactory), EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Calls the virtual GridEngineFactoryBase.CreateEngine factory method and instantiates a
        /// GridEngine object.
        /// </summary>
        /// <returns>
        /// If the GridEngineFactoryBase.CreateEngine method is not overridden, an empty <see cref="GridEngine"/>
        /// object is returned, otherwise an object derived from GridEngine may be returned.
        /// </returns>
        public static GridEngine CreateEngine()
        {
            return Factory.CreateEngine();
        }
    }

    /// <summary>
    /// A factory class object with the purpose of instantiating a GridEngine object.
    /// </summary>
    /// <remarks>
    /// The virtual GridGroupingControl.CreateEngine function by default calls GridEngineFactory.CreateEngine
    /// to create an engine object.
    /// <para/>
    /// There are multiple alternatives that let you customize the engine object that is used within a GridGroupingControl.
    /// <list type="bullet">
    /// <item><term>
    /// A global solution for all controls in your app is to derive a class from GridEngineFactoryBase,
    /// override CreateEngine, and assign an instance of the derived GridEngineFactoryBase object to GridEngineFactory.Factory.
    /// </term></item>
    /// <item><term>
    /// Instantiate manually a GridEngine object and call the constructor of GridGroupingControl, passing in the engine
    /// object as parameter.
    /// </term></item>
    /// <item><term>
    /// Derive GridGroupingControl and override the virtual CreateEngine method.
    /// </term></item>
    /// </list>
    /// <para/>
    /// In your derived engine class, you can then further customize classes that are created. The GridEngine class
    /// contains virtual factory methods such as <see cref="GridEngine.CreateTableControl"/> or <see cref="GridEngine.CreateTable"/>
    /// that are called to create objects that belong to a GridGroupingControl
    /// and its engine elements.
    /// </remarks>
    /// <seealso cref="GridGroupingControl"/>
    public class GridEngineFactoryBase
    {
        /// <summary>
        /// Initializes a <see cref="GridEngineFactoryBase"/>
        /// </summary>
        public GridEngineFactoryBase()
        {
            isDefault = false;
        }

        /// <summary>
        /// Initializes a <see cref="GridEngineFactoryBase"/> and optionally marks it as "Default".
        /// </summary>
        /// <param name="isDefault">True if this engine should be marked as default.</param>
        public GridEngineFactoryBase(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        bool isDefault;

        /// <summary>
        /// Returns True when this factory has been auto-initialized.
        /// </summary>
        public virtual bool IsDefault
        {
            get
            {
                return isDefault;
            }
        }

        // Factory methods:

        /// <summary>
        /// Override this method if you would like to have a derived engine object. The method is called from
        /// GridGroupingControl.CreateEngine to create a default engine object when no engine objects were manually
        /// instantiated. See also the discussion in <see cref="GridEngineFactory"/>.
        /// </summary>
        /// <returns>The new grid engine.</returns>
        public virtual GridEngine CreateEngine()
        {
            return new GridEngine();
        }
    }
}